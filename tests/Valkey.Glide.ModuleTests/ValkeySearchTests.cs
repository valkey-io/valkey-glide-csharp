// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.ModuleTests;

/// <summary>
/// Exercises valkey-search FT.* commands through the typed client API against the search module
/// loaded by <c>utils/start-valkey-docker-modules.sh</c>.
/// </summary>
/// <remarks>
/// Most tests run against the standalone client. The cluster tests and the cluster branch of the
/// compatibility-level probe run against the cluster client. Each test skips when the connected
/// module does not provide the feature it covers. Compatibility-level tests assert the behaviour
/// for the level the module reports.
///
/// The module harness starts cluster nodes without the coordinator, so these tests do not run
/// with the coordinator enabled.
/// </remarks>
public class ValkeySearchTests(ModuleTestConfiguration config) : IAsyncLifetime
{
    public ModuleTestConfiguration Config { get; } = config;

    private BaseClient _standaloneForProbe = null!;
    private bool _capable;
    private string? _level;
    private readonly List<(BaseClient Client, string Index)> _indexesToDrop = [];

    public async ValueTask InitializeAsync()
    {
        _standaloneForProbe = ModuleTestConfiguration.CreateStandaloneClient();
        _capable = await SearchCapability.ProbeCapabilityAsync(_standaloneForProbe);
        _level = _capable ? await SearchCapability.EmulateReleaseLevelAsync(_standaloneForProbe) : null;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var (client, index) in _indexesToDrop)
        {
            try
            {
                await Ft.DropIndexAsync(client, index);
            }
            catch (Errors.RequestException ex)
            {
                Console.Error.WriteLine($"WARNING: cleanup failed to drop index '{index}': {ex.Message}");
            }
        }

        _standaloneForProbe.Dispose();
    }

    #region Capability probe / level fixture plumbing

    [Fact]
    public async Task CapabilityProbe_MatchesLiveInKeysSupport()
    {
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(_standaloneForProbe, index, new Ft.CreateTextField("title"));
        try
        {
            if (_capable)
            {
                var result = await Ft.SearchAsync(_standaloneForProbe, index, "*", new Ft.SearchOptions { InKeys = [] });
                Assert.Equal(0, result.TotalResults);
            }
            else
            {
                _ = await Assert.ThrowsAsync<Errors.RequestException>(
                    () => Ft.SearchAsync(_standaloneForProbe, index, "*", new Ft.SearchOptions { InKeys = [] }));
            }
        }
        finally
        {
            await Ft.DropIndexAsync(_standaloneForProbe, index);
        }
    }

    [Fact]
    public async Task LevelFixture_ClusterBranch_ReadsSameLevelAsStandalone()
    {
        SkipUnlessCapable();
        GlideClusterClient? cluster = ModuleTestConfiguration.CreateClusterClient();
        Assert.SkipWhen(cluster is null, "no cluster endpoints provided to the module-test harness");
        try
        {
            string? clusterLevel = await SearchCapability.EmulateReleaseLevelAsync(cluster!);
            Assert.False(string.IsNullOrEmpty(clusterLevel));
            Assert.Equal(_level, clusterLevel);
        }
        finally
        {
            cluster?.Dispose();
        }
    }

    #endregion
    #region Core: INKEYS, bare wildcard, implicit LOAD

    [Fact]
    public async Task InKeys_RestrictsSearchToGivenKeys()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "hello")]);
        _ = await client.HashSetAsync($"{index}:doc3", [new("title", "hello")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var unrestricted = await Ft.SearchAsync(client, index, "hello");
        Assert.Equal(3, unrestricted.TotalResults);

        var restricted = await Ft.SearchAsync(
            client, index, "hello", new Ft.SearchOptions { InKeys = [$"{index}:doc1", $"{index}:doc2"] });
        Assert.Equal(2, restricted.TotalResults);
        Assert.Equal(
            new HashSet<string> { $"{index}:doc1", $"{index}:doc2" },
            [.. restricted.Documents.Select(d => d.Key.ToString())]);

        var empty = await Ft.SearchAsync(client, index, "hello", new Ft.SearchOptions { InKeys = [] });
        Assert.Equal(0, empty.TotalResults);
        Assert.Empty(empty.Documents);
    }

    [Fact]
    public async Task BareWildcard_MatchesEveryDocument()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "goodbye")]);
        _ = await client.HashSetAsync($"{index}:doc3", [new("title", "anything")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "*");

        Assert.Equal(3, result.TotalResults);
        Assert.Equal(
            new HashSet<string> { $"{index}:doc1", $"{index}:doc2", $"{index}:doc3" },
            [.. result.Documents.Select(d => d.Key.ToString())]);
    }

    [Fact]
    public async Task Aggregate_GroupByZero_AutoLoadsReferencedFieldWithNoExplicitLoad()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTextField("title"), new Ft.CreateNumericField("price")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "a"), new("price", "10")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "b"), new("price", "30")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.AggregateAsync(
            client,
            index,
            "*",
            new Ft.AggregateOptions
            {
                Clauses =
                [
                    new Ft.AggregateGroupBy
                    {
                        Fields = [],
                        Reducers = [new Ft.AggregateReducer { Function = Ft.ReducerFunction.Sum, Expressions = ["@price"], Name = "total" }],
                    },
                ],
            });

        _ = Assert.Single(rows);
        Assert.Equal((ValkeyValue)"40", rows[0][(ValkeyValue)"total"]);
    }

    #endregion
    #region Array-valued reducers: TOLIST and RANDOM_SAMPLE

    [Fact]
    public async Task Aggregate_ToListReducer_ReturnsExactDedupedValueSet()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTagField("category"), new Ft.CreateNumericField("price")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:doc1", [new("category", "a"), new("price", "10")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("category", "a"), new("price", "30")]);
        _ = await client.HashSetAsync($"{index}:doc3", [new("category", "a"), new("price", "10")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.AggregateAsync(
            client,
            index,
            "*",
            new Ft.AggregateOptions
            {
                Clauses =
                [
                    new Ft.AggregateGroupBy
                    {
                        Fields = ["@category"],
                        Reducers = [Ft.AggregateReducer.ToList("@price", "prices")],
                    },
                ],
            });

        _ = Assert.Single(rows);
        var pricesJson = rows[0][(ValkeyValue)"prices"].ToString();
        var prices = System.Text.Json.JsonSerializer.Deserialize<string[]>(pricesJson!)!;
        Assert.Equal(new HashSet<string> { "10", "30" }, [.. prices]);
    }

    [Fact]
    public async Task Aggregate_RandomSampleReducer_ReturnsValuesDrawnFromGroup()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTagField("category"), new Ft.CreateNumericField("price")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        HashSet<string> seededPrices = ["10", "20", "30", "40", "50"];
        int i = 0;
        foreach (var price in seededPrices)
        {
            _ = await client.HashSetAsync($"{index}:doc{i++}", [new("category", "a"), new("price", price)]);
        }

        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.AggregateAsync(
            client,
            index,
            "*",
            new Ft.AggregateOptions
            {
                Clauses =
                [
                    new Ft.AggregateGroupBy
                    {
                        Fields = ["@category"],
                        Reducers = [Ft.AggregateReducer.RandomSample("@price", 3, "sample")],
                    },
                ],
            });

        _ = Assert.Single(rows);
        var sampleJson = rows[0][(ValkeyValue)"sample"].ToString();
        var sample = System.Text.Json.JsonSerializer.Deserialize<string[]>(sampleJson!)!;
        Assert.True(sample.Length <= 3, $"RANDOM_SAMPLE size 3 must cap the result; got {sample.Length}");
        Assert.True(sample.Length > 0, "RANDOM_SAMPLE over a non-empty group must return at least one value");
        Assert.All(sample, v => Assert.Contains(v, seededPrices));
    }

    #endregion
    #region Vector range

    [Fact]
    public async Task Search_VectorRange_RestrictsToRadiusAndYieldsDistance()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateVectorFieldFlat { Identifier = "vec", Dimensions = 1, DistanceMetric = Ft.DistanceMetric.Euclidean },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:origin", [new("vec", PackVector(0f))]);
        _ = await client.HashSetAsync($"{index}:near", [new("vec", PackVector(1f))]);
        _ = await client.HashSetAsync($"{index}:far", [new("vec", PackVector(4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(
            client,
            index,
            "@vec:[VECTOR_RANGE 1.5 $blob]=>{$YIELD_DISTANCE_AS: dist}",
            new Ft.SearchOptions { Params = new Dictionary<ValkeyValue, ValkeyValue> { [(ValkeyValue)"blob"] = (ValkeyValue)PackVector(0f) } });

        Assert.Equal(2, result.TotalResults);
        Assert.Equal(
            new HashSet<string> { $"{index}:origin", $"{index}:near" },
            [.. result.Documents.Select(d => d.Key.ToString())]);
        var originDoc = result.Documents.Single(d => d.Key.ToString() == $"{index}:origin");
        Assert.Equal((ValkeyValue)"0", originDoc.Fields[(ValkeyValue)"dist"]);
    }

    #endregion
    #region SORTBY widened by adjacent LIMIT

    [Fact]
    public async Task Aggregate_SortByAdjacentLimit_WidensRetainedRowsOnlyWhenCompatibilityLevelAllows()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateNumericField("n") { Sortable = true },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        for (int i = 1; i <= 15; i++)
        {
            _ = await client.HashSetAsync($"{index}:d{i}", [new("n", i.ToString())]);
        }

        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.AggregateAsync(
            client,
            index,
            "*",
            new Ft.AggregateOptions
            {
                Clauses =
                [
                    new Ft.AggregateSortBy { Expressions = [new Ft.AggregateSortExpression { Expression = "@n", Order = SortOrder.Ascending }] },
                    new Ft.AggregateLimit { Offset = 0, Count = 15 },
                ],
            });

        if (SearchCapability.IsAtLeast130(_level))
        {
            Assert.Equal(15, rows.Length);
        }
        else
        {
            Assert.Equal(10, rows.Length);
        }
    }

    #endregion
    #region FT.SEARCH LIMIT pagination

    [Fact]
    public async Task Search_LimitPagination_ReturnsDistinctPagesCoveringEveryKey()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateNumericField("n") { Sortable = true },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        HashSet<string> allKeys = [];
        for (int i = 1; i <= 7; i++)
        {
            string key = $"{index}:d{i}";
            _ = allKeys.Add(key);
            _ = await client.HashSetAsync(key, [new("n", i.ToString())]);
        }

        await SearchIndexing.WaitForIndexingAsync(client, index);

        var page1 = await Ft.SearchAsync(client, index, "*", new Ft.SearchOptions { Limit = new Ft.SearchLimit { Offset = 0, Count = 3 } });
        var page2 = await Ft.SearchAsync(client, index, "*", new Ft.SearchOptions { Limit = new Ft.SearchLimit { Offset = 3, Count = 3 } });
        var page3 = await Ft.SearchAsync(client, index, "*", new Ft.SearchOptions { Limit = new Ft.SearchLimit { Offset = 6, Count = 3 } });

        Assert.Equal(3, page1.Documents.Length);
        Assert.Equal(3, page2.Documents.Length);
        _ = Assert.Single(page3.Documents);

        var page1Keys = page1.Documents.Select(d => d.Key.ToString()).ToHashSet();
        var page2Keys = page2.Documents.Select(d => d.Key.ToString()).ToHashSet();
        var page3Keys = page3.Documents.Select(d => d.Key.ToString()).ToHashSet();

        Assert.Empty(page1Keys.Intersect(page2Keys));
        Assert.Empty(page2Keys.Intersect(page3Keys));
        Assert.Empty(page1Keys.Intersect(page3Keys));
        Assert.Equal(allKeys.Order(), page1Keys.Union(page2Keys).Union(page3Keys).Order());
    }

    #endregion
    #region Scoring: FT.SEARCH and FT.AGGREGATE WITHSCORES, SCORER, ADDSCORES, LOAD AS, WITHCURSOR

    [Fact]
    public async Task Search_WithScoresAndScorer_RanksMatchingDocumentAboveNonMatching()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:strong", [new("title", "hello hello hello world")]);
        _ = await client.HashSetAsync($"{index}:weak", [new("title", "hello")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(
            client,
            index,
            "hello",
            new Ft.SearchOptions { WithScores = true, Scorer = "BM25STD" });

        Assert.Equal(2, result.TotalResults);
        Assert.All(result.Documents, d => Assert.NotNull(d.Score));
        double strongScore = result.Documents.Single(d => d.Key.ToString() == $"{index}:strong").Score!.Value;
        double weakScore = result.Documents.Single(d => d.Key.ToString() == $"{index}:weak").Score!.Value;
        Assert.True(strongScore > weakScore, $"expected strong ({strongScore}) > weak ({weakScore})");
    }

    [Fact]
    public async Task Search_WithScoresAndWithSortKeys_ReturnsThreeTupleShape()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTextField("title") { Sortable = true }, new Ft.CreateNumericField("price") { Sortable = true }],
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:cheap", [new("title", "hello"), new("price", "10")]);
        _ = await client.HashSetAsync($"{index}:pricey", [new("title", "hello"), new("price", "20")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(
            client,
            index,
            "*",
            new Ft.SearchOptions
            {
                WithScores = true,
                SortBy = new Ft.SearchSortBy { Field = "price", Order = SortOrder.Ascending, WithSortKeys = true },
            });

        Assert.Equal(2, result.TotalResults);
        Assert.All(
            result.Documents,
            d =>
            {
                _ = Assert.NotNull(d.Score);
                Assert.False(d.SortKey.IsNull);
                Assert.True(d.Fields.ContainsKey((ValkeyValue)"title"));
            });

        var cheapDoc = result.Documents.Single(d => d.Key.ToString() == $"{index}:cheap");
        var priceyDoc = result.Documents.Single(d => d.Key.ToString() == $"{index}:pricey");
        Assert.Equal((ValkeyValue)"#10", cheapDoc.SortKey);
        Assert.Equal((ValkeyValue)"#20", priceyDoc.SortKey);
    }

    [Fact]
    public async Task Search_CountOption_EmitsLimitZeroZero_ReturnsTotalOnly()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "hello")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "*", new Ft.SearchOptions { Count = true });

        Assert.Equal(2, result.TotalResults);
        Assert.Empty(result.Documents);
    }

    [Fact]
    public async Task Aggregate_AddScoresAndScorer_SortsByRealScoreColumn()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:strong", [new("title", "hello hello hello world")]);
        _ = await client.HashSetAsync($"{index}:weak", [new("title", "hello")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.AggregateAsync(
            client,
            index,
            "hello",
            new Ft.AggregateOptions
            {
                AddScores = true,
                Scorer = "BM25STD",
                LoadFields = ["__key"],
                Clauses = [new Ft.AggregateSortBy { Expressions = [new Ft.AggregateSortExpression { Expression = "@__score", Order = SortOrder.Descending }] }],
            });

        Assert.Equal(2, rows.Length);
        Assert.Equal((ValkeyValue)$"{index}:strong", rows[0][(ValkeyValue)"__key"]);
        Assert.Equal((ValkeyValue)$"{index}:weak", rows[1][(ValkeyValue)"__key"]);
    }

    [Fact]
    public async Task Aggregate_LoadFieldAlias_AssertsLegacyOrCorrectedBehaviourAtEachLevel()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        if (SearchCapability.IsAtLeast130(_level))
        {
            var rows = await Ft.AggregateAsync(
                client,
                index,
                "hello",
                new Ft.AggregateOptions { LoadFieldEntries = [new Ft.LoadField("title", "renamed_title")] });

            _ = Assert.Single(rows);
            Assert.Equal((ValkeyValue)"hello world", rows[0][(ValkeyValue)"renamed_title"]);
            Assert.False(rows[0].ContainsKey((ValkeyValue)"title"));
        }
        else
        {
            _ = await Assert.ThrowsAsync<Errors.RequestException>(() => Ft.AggregateAsync(
                client,
                index,
                "hello",
                new Ft.AggregateOptions { LoadFieldEntries = [new Ft.LoadField("title", "renamed_title")] }));
        }
    }

    [Fact]
    public async Task Aggregate_WithCursor_PagesToExhaustionCoveringEveryKey()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        HashSet<string> allKeys = [];
        for (int i = 0; i < 5; i++)
        {
            string key = $"{index}:doc{i}";
            _ = allKeys.Add(key);
            _ = await client.HashSetAsync(key, [new("title", "hello")]);
        }

        await SearchIndexing.WaitForIndexingAsync(client, index);

        HashSet<string> seenKeys = [];
        var page = await Ft.AggregateWithCursorAsync(
            client,
            index,
            "*",
            new Ft.AggregateOptions { LoadFields = ["__key"], WithCursorOptions = new Ft.WithCursor { Count = 2 } });
        foreach (var row in page.Rows)
        {
            _ = seenKeys.Add(row[(ValkeyValue)"__key"].ToString()!);
        }

        while (page.CursorId != 0)
        {
            page = await Ft.CursorReadAsync(client, index, page.CursorId);
            foreach (var row in page.Rows)
            {
                _ = seenKeys.Add(row[(ValkeyValue)"__key"].ToString()!);
            }
        }

        Assert.Equal(allKeys, seenKeys);
    }

    [Fact]
    public async Task Cursor_DelThenRead_RaisesForDeletedAndUnknownCursorIds()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        for (int i = 0; i < 5; i++)
        {
            _ = await client.HashSetAsync($"{index}:doc{i}", [new("title", "hello")]);
        }

        await SearchIndexing.WaitForIndexingAsync(client, index);

        var page = await Ft.AggregateWithCursorAsync(
            client,
            index,
            "*",
            new Ft.AggregateOptions { WithCursorOptions = new Ft.WithCursor { Count = 1 } });
        Assert.NotEqual(0, page.CursorId);
        long openCursorId = page.CursorId;

        await Ft.CursorDelAsync(client, index, openCursorId);

        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => Ft.CursorReadAsync(client, index, openCursorId));
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => Ft.CursorReadAsync(client, index, 999999999));
    }

    #endregion
    #region FT.CREATE: vector types, FILTER, SORTABLE UNF, SCORE_FIELD, NOHL

    [Fact]
    public async Task Create_Float16VectorField_SupportsKnnQuery()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateVectorFieldFlat { Identifier = "vec", Dimensions = 2, DistanceMetric = Ft.DistanceMetric.Euclidean, Type = Ft.VectorType.Float16 },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:near", [new("vec", PackHalfVector(1f, 1f))]);
        _ = await client.HashSetAsync($"{index}:far", [new("vec", PackHalfVector(100f, 100f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(
            client,
            index,
            "*=>[KNN 1 @vec $blob]",
            new Ft.SearchOptions { Params = new Dictionary<ValkeyValue, ValkeyValue> { [(ValkeyValue)"blob"] = (ValkeyValue)PackHalfVector(1f, 1f) } });

        Assert.Equal(1, result.TotalResults);
        Assert.Equal($"{index}:near", result.Documents[0].Key.ToString());
    }

    [Fact]
    public async Task Create_Bfloat16VectorField_SupportsKnnQuery()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateVectorFieldFlat { Identifier = "vec", Dimensions = 2, DistanceMetric = Ft.DistanceMetric.Euclidean, Type = Ft.VectorType.Bfloat16 },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:near", [new("vec", PackBfloat16Vector(1f, 1f))]);
        _ = await client.HashSetAsync($"{index}:far", [new("vec", PackBfloat16Vector(100f, 100f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(
            client,
            index,
            "*=>[KNN 1 @vec $blob]",
            new Ft.SearchOptions { Params = new Dictionary<ValkeyValue, ValkeyValue> { [(ValkeyValue)"blob"] = (ValkeyValue)PackBfloat16Vector(1f, 1f) } });

        Assert.Equal(1, result.TotalResults);
        Assert.Equal($"{index}:near", result.Documents[0].Key.ToString());
    }

    [Fact]
    public async Task Create_WithFilterOption_ExcludesNonMatchingDocuments()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTextField("title"), new Ft.CreateNumericField("price")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"], Filter = "@price > 0" });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:included", [new("title", "hello"), new("price", "10")]);
        _ = await client.HashSetAsync($"{index}:excluded", [new("title", "hello"), new("price", "-5")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "*");

        Assert.Equal(1, result.TotalResults);
        Assert.Equal($"{index}:included", result.Documents[0].Key.ToString());
    }

    [Fact]
    public async Task Create_ScoreFieldInIndexDefinition_ReportedOnlyWhenCompatibilityLevelAllows()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateTextField("title"),
            new Ft.CreateOptions { Prefixes = [$"{index}:"], ScoreField = "boost" });
        _indexesToDrop.Add((client, index));

        var info = await Ft.InfoLocalAsync(client, index);

        if (SearchCapability.IsAtLeast130(_level))
        {
            Assert.Equal((ValkeyValue)"boost", info.ScoreField);
            _ = Assert.NotNull(info.DefaultScore);
        }
        else
        {
            Assert.True(info.ScoreField.IsNull);
            Assert.Equal(1.0, info.DefaultScore);
        }
    }

    [Fact]
    public async Task Create_NoHighlight_IsAcceptedAsAWireCompatibleNoOp()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateTextField("title"),
            new Ft.CreateOptions { Prefixes = [$"{index}:"], NoHighlight = true });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "hello");
        Assert.Equal(1, result.TotalResults);
    }

    [Fact]
    public async Task Create_SortableUnfTextField_PreservesOriginalCaseInSortKey()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateTextField("title") { Sortable = true, Unf = true },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "HELLO World")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(
            client,
            index,
            "*",
            new Ft.SearchOptions { SortBy = new Ft.SearchSortBy { Field = "title", WithSortKeys = true } });

        Assert.Equal(1, result.TotalResults);
        string expectedPrefix = SearchCapability.IsAtLeast130(_level) ? "$" : "#";
        Assert.Equal((ValkeyValue)$"{expectedPrefix}HELLO World", result.Documents[0].SortKey);
    }

    #endregion
    #region SCORE_FIELD ranking and scorer validation

    [Fact]
    public async Task Create_ScoreFieldAffectsRanking_MultipliesIntoRelevanceScore()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTextField("title"), new Ft.CreateNumericField("boost")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"], ScoreField = "boost" });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:boosted", [new("title", "hello"), new("boost", "100")]);
        _ = await client.HashSetAsync($"{index}:plain", [new("title", "hello"), new("boost", "1")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "hello", new Ft.SearchOptions { WithScores = true });

        Assert.Equal(2, result.TotalResults);
        double boostedScore = result.Documents.Single(d => d.Key.ToString() == $"{index}:boosted").Score!.Value;
        double plainScore = result.Documents.Single(d => d.Key.ToString() == $"{index}:plain").Score!.Value;
        Assert.True(boostedScore > plainScore, $"expected boosted ({boostedScore}) > plain ({plainScore})");
    }

    [Fact]
    public async Task Search_UnregisteredScorer_SurfacesTheServerRequestError()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world")]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var ex = await Assert.ThrowsAsync<Errors.RequestException>(() =>
            Ft.SearchAsync(client, index, "hello", new Ft.SearchOptions { WithScores = true, Scorer = "TFIDF" }));
        Assert.Contains("SCORER", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
    #region FT.HYBRID: SEARCH and VSIM arms, combine methods, FILTER placement

    [Fact]
    public async Task Hybrid_BasicTwoArmCall_FusesBothArmsResults()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "nothing matches"), new("price", "20"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10 });

        Assert.Equal(
            new HashSet<string> { $"{index}:doc1", $"{index}:doc2" },
            [.. rows.Select(r => r[(ValkeyValue)"__key"].ToString())]);
    }

    [Fact]
    public async Task Hybrid_RrfCombine_FusesByReciprocalRank()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:both", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:neither", [new("title", "nothing"), new("price", "20"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10 },
            new Ft.HybridOptions { Combine = new Ft.HybridRrf { Constant = 60 } });

        Assert.NotEmpty(rows);
        Assert.Equal($"{index}:both", rows[0][(ValkeyValue)"__key"].ToString());
    }

    [Fact]
    public async Task Hybrid_FilterImmediatelyAfterVsimArmWithNoLoad_IsNotClaimedAsVsimPrefilter()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:positive", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:negative", [new("title", "hello world"), new("price", "-5"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world", YieldScoreAs = "ss" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10 },
            new Ft.HybridOptions
            {
                Clauses = [new Ft.AggregateFilter("@price > 0")],
            });

        _ = Assert.Single(rows);
        Assert.Equal($"{index}:positive", rows[0][(ValkeyValue)"__key"].ToString());
    }

    [Fact]
    public async Task Hybrid_LinearCombine_WeightsSearchAndVsimArms()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:both", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:neither", [new("title", "nothing"), new("price", "20"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10 },
            new Ft.HybridOptions { Combine = new Ft.HybridLinear { Alpha = 0.5, Beta = 0.5 } });

        Assert.NotEmpty(rows);
        Assert.Equal($"{index}:both", rows[0][(ValkeyValue)"__key"].ToString());
    }

    [Fact]
    public async Task Hybrid_FunctionCombine_YieldsBothArmScoreColumns()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world", YieldScoreAs = "ss" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10, YieldScoreAs = "vs" },
            new Ft.HybridOptions
            {
                Combine = new Ft.HybridFunction { Expr = "@ss + @vs", YieldScoreAs = "fused" },
                Clauses = [new Ft.AggregateApply { Expression = "@ss", Name = "search_score_col" }],
            });

        _ = Assert.Single(rows);
        Assert.True(rows[0].ContainsKey((ValkeyValue)"fused"));
        Assert.True(rows[0].ContainsKey((ValkeyValue)"search_score_col"));
        Assert.True(double.TryParse(rows[0][(ValkeyValue)"search_score_col"].ToString(), out double parsedScore));
        Assert.True(parsedScore >= 0);
    }

    [Fact]
    public async Task Hybrid_WithCursor_PagesToExhaustionCoveringEveryKey()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        HashSet<string> allKeys = [];
        for (int i = 0; i < 5; i++)
        {
            string key = $"{index}:doc{i}";
            _ = allKeys.Add(key);
            _ = await client.HashSetAsync(key, [new("title", "hello world"), new("price", $"{i * 10}"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        }

        await SearchIndexing.WaitForIndexingAsync(client, index);

        HashSet<string> seenKeys = [];
        var page = await Ft.HybridWithCursorAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10 },
            new Ft.HybridOptions
            {
                Combine = new Ft.HybridRrf(),
                Clauses = [new Ft.AggregateLimit { Offset = 0, Count = 10 }],
                WithCursorOptions = new Ft.WithCursor { Count = 2 },
            });
        foreach (var row in page.Rows)
        {
            _ = seenKeys.Add(row[(ValkeyValue)"__key"].ToString()!);
        }

        while (page.CursorId != 0)
        {
            page = await Ft.CursorReadAsync(client, index, page.CursorId);
            foreach (var row in page.Rows)
            {
                _ = seenKeys.Add(row[(ValkeyValue)"__key"].ToString()!);
            }
        }

        Assert.Equal(allKeys, seenKeys);
    }

    [Fact]
    public async Task Hybrid_LocalOnly_IsAcceptedByTheServer()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10 },
            new Ft.HybridOptions { LocalOnly = true });

        Assert.NotEmpty(rows);
    }

    [Fact]
    public async Task Hybrid_VectorGivenAsExplicitParamReference_MatchesInlineBytesResult()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        byte[] vectorBytes = PackVector(0.1f, 0.2f, 0.3f, 0.4f);

        var inlineRows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = vectorBytes, K = 10 });

        var paramRows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = "$myvec", K = 10 },
            new Ft.HybridOptions { Params = new Dictionary<ValkeyValue, ValkeyValue> { [(ValkeyValue)"myvec"] = (ValkeyValue)vectorBytes } });

        Assert.Equal(
            inlineRows.Select(r => r[(ValkeyValue)"__key"].ToString()).ToHashSet(),
            [.. paramRows.Select(r => r[(ValkeyValue)"__key"].ToString())]);
    }

    [Fact]
    public async Task Hybrid_VsimFilterWithPolicy_RestrictsToMatchingDocuments()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:included", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:excluded", [new("title", "hello world"), new("price", "-5"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "nonmatchingterm" },
            new Ft.HybridVsimArm
            {
                Field = "vec",
                Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f),
                K = 10,
                Filter = new Ft.HybridVsimFilter("@price:[0 +inf]") { Policy = Ft.HybridFilterPolicy.Adhoc },
            });

        Assert.Equal(
            new HashSet<string> { $"{index}:included" },
            [.. rows.Select(r => r[(ValkeyValue)"__key"].ToString())]);
    }

    [Fact]
    public async Task Hybrid_WithoutK_UsesEngineDefaultOfTen()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnlessCapable();
        var index = await CreateVectorIndexAsync(client);
        for (int i = 0; i < 15; i++)
        {
            _ = await client.HashSetAsync($"{index}:doc{i}", [new("title", "hello world"), new("price", $"{i}"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        }

        await SearchIndexing.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "termthatmatchesnothingatall" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f) });

        Assert.Equal(10, rows.Length);
    }

    #endregion
    #region Cluster-specific: multi-shard placement and cursor routing

    [Fact]
    public async Task Search_MultiShardNoHashtag_AggregatesAcrossShards()
    {
        SkipUnlessCapable();
        GlideClusterClient? cluster = ModuleTestConfiguration.CreateClusterClient();
        Assert.SkipWhen(cluster is null, "no cluster endpoints provided to the module-test harness");
        var client = cluster!;
        string? index = null;
        try
        {
            index = $"idx_{Guid.NewGuid():N}";
            await Ft.CreateAsync(
                client,
                index,
                new Ft.CreateTextField("title"),
                new Ft.CreateOptions { Prefixes = [$"{index}:"] });

            const int seededCount = 30;
            for (int i = 0; i < seededCount; i++)
            {
                string key = $"{index}:{Guid.NewGuid():N}";
                _ = await client.HashSetAsync(key, [new("title", "hello")]);
            }

            await SearchIndexing.WaitForIndexingAsync(client, index);

            var result = await Ft.SearchAsync(client, index, "*");

            Assert.True(
                result.TotalResults is > 0 and < seededCount,
                $"expected a STRICT SUBSET of {seededCount} seeded documents (confirming FT.CREATE's " +
                    $"single-shard placement), got {result.TotalResults}");
        }
        finally
        {
            if (index is not null)
            {
                try
                {
                    await Ft.DropIndexAsync(client, index);
                }
                catch (Errors.RequestException)
                {
                }
            }

            cluster?.Dispose();
        }
    }

    [Fact]
    public async Task Cursor_RoutesToIndexSlotPrimaryInClusterMode()
    {
        SkipUnlessCapable();
        GlideClusterClient? cluster = ModuleTestConfiguration.CreateClusterClient();
        Assert.SkipWhen(cluster is null, "no cluster endpoints provided to the module-test harness");
        var client = cluster!;
        string? index = null;
        try
        {
            string hashtag = Guid.NewGuid().ToString("N");
            index = $"idx_{{{hashtag}}}";
            await Ft.CreateAsync(
                client,
                index,
                new Ft.CreateTextField("title") { Sortable = true },
                new Ft.CreateOptions { Prefixes = [$"{{{hashtag}}}:"] });
            HashSet<string> allKeys = [];
            for (int i = 0; i < 5; i++)
            {
                string key = $"{{{hashtag}}}:doc{i}";
                _ = allKeys.Add(key);
                _ = await client.HashSetAsync(key, [new("title", "hello")]);
            }

            await SearchIndexing.WaitForIndexingAsync(client, index);

            HashSet<string> seenKeys = [];
            var page = await Ft.AggregateWithCursorAsync(
                client,
                index,
                "*",
                new Ft.AggregateOptions { LoadFields = ["__key"], WithCursorOptions = new Ft.WithCursor { Count = 2 } });
            foreach (var row in page.Rows)
            {
                _ = seenKeys.Add(row[(ValkeyValue)"__key"].ToString()!);
            }

            while (page.CursorId != 0)
            {
                page = await Ft.CursorReadAsync(client, index, page.CursorId);
                foreach (var row in page.Rows)
                {
                    _ = seenKeys.Add(row[(ValkeyValue)"__key"].ToString()!);
                }
            }

            Assert.Equal(allKeys, seenKeys);
        }
        finally
        {
            if (index is not null)
            {
                try
                {
                    await Ft.DropIndexAsync(client, index);
                }
                catch (Errors.RequestException)
                {
                }
            }

            cluster?.Dispose();
        }
    }

    #endregion
    #region Helpers

    private void SkipUnlessCapable()
        => Assert.SkipWhen(!_capable, "search module is pre-1.3 (no live INKEYS support); 1.3 tests skip on this build");

    private async Task<string> CreateBasicIndexAsync(BaseClient client)
    {
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateTextField("title") { Sortable = true },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));
        return index;
    }

    private async Task<string> CreateVectorIndexAsync(BaseClient client)
    {
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [
                new Ft.CreateTextField("title"),
                new Ft.CreateNumericField("price"),
                new Ft.CreateVectorFieldFlat { Identifier = "vec", Dimensions = 4, DistanceMetric = Ft.DistanceMetric.Euclidean },
            ],
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));
        return index;
    }

    private static byte[] PackVector(params float[] values)
    {
        byte[] bytes = new byte[values.Length * sizeof(float)];
        Buffer.BlockCopy(values, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    private static byte[] PackHalfVector(params float[] values)
    {
        byte[] bytes = new byte[values.Length * 2];
        for (int i = 0; i < values.Length; i++)
        {
            _ = BitConverter.TryWriteBytes(bytes.AsSpan(i * 2, 2), (Half)values[i]);
        }

        return bytes;
    }

    private static byte[] PackBfloat16Vector(params float[] values)
    {
        byte[] bytes = new byte[values.Length * 2];
        for (int i = 0; i < values.Length; i++)
        {
            uint bits = BitConverter.SingleToUInt32Bits(values[i]);
            ushort truncated = (ushort)(bits >> 16);
            _ = BitConverter.TryWriteBytes(bytes.AsSpan(i * 2, 2), truncated);
        }

        return bytes;
    }

    #endregion
}
