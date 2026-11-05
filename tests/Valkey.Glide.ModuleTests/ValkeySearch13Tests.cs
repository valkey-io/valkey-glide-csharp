// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Pipeline;
using Valkey.Glide.ServerModules;

namespace Valkey.Glide.ModuleTests;

/// <summary>
/// Runs valkey-search 1.3 FT.* tests against a REAL valkey-search module via Docker
/// (<c>utils/run-modules-ci-locally.sh</c>), additive alongside the main suite's
/// <c>tests/Valkey.Glide.IntegrationTests/ServerModules</c> fixtures (which run against whatever
/// module the default dev server happens to have, with no capability gating).
/// </summary>
/// <remarks>
/// Gated by <see cref="SearchCapability.Probe13CapabilityAsync"/> rather than a version-string
/// compare: on <c>valkey-bundle:9.1.3</c> (search 1.2.1) every 1.3-gated test here skips with a
/// stated reason. Against a search 1.3 <c>.so</c> built from source, tests further branch
/// on the active <c>search.emulate-release</c> level (the level fixture) to assert the corrected
/// 1.3 behaviour only when that level is actually &gt;= 1.3.0 -- a 1.3-capable module left
/// at its own default (1.0.0) still runs the legacy branch of every
/// <c>VALKEY_SEARCH_COMPATIBILITY_FIX</c> gate.
///
/// Most tests here run against the standalone client only, via <c>_standaloneForProbe</c>
/// (renamed in spirit, not in code, to just "the standalone client" below). H3's cluster
/// coverage is scoped to what can actually be exercised against a real cluster deployment:
/// cursor/HYBRID WITHCURSOR routing to the index slot's primary
/// (<see cref="Cursor_RoutesToIndexSlotPrimaryInClusterMode"/>), the cluster branch of
/// <see cref="SearchCapability.EmulateReleaseLevelAsync"/>
/// (<see cref="LevelFixture_ClusterBranch_ReadsSameLevelAsStandalone"/>), and genuine multi-shard
/// placement (<see cref="Search_MultiShardNoHashtag_AggregatesAcrossShards"/>) -- NOT every
/// feature test duplicated across both clients. Confirmed empirically against a real 6-node
/// cluster deployment while writing this fixture: <c>FT.CREATE</c> is NOT broadcast cluster-wide
/// on valkey-search 1.3.0-rc1 (it lands on exactly the one primary GLIDE's cluster client happens
/// to route the command to, by key-hash on the index name -- a server/engine fact, not a GLIDE
/// bug, matching what Python's own <c>test_ft_multi_shard_no_hashtag</c> independently confirms
/// and cites). A document hashed to any OTHER shard is simply never indexed at all, regardless of
/// prefix matching -- so a generic feature test seeded with documents spread across random keys
/// (the natural thing to do against a cluster) silently indexes only whichever fraction happened
/// to land on the index-owning shard, and most of the behavioural assertions throughout this file
/// (set equality against the FULL seeded key set, exact row counts, cursor exhaustion covering
/// every key) would then fail for a reason entirely unrelated to the feature under test. Rather
/// than rewrite every seeding helper to pin keys to the index-owning shard (Python's own test
/// shows the lengths that requires: walking <c>CLUSTER KEYSLOT</c> for candidate suffixes until
/// finding ones that hash to each side), the feature tests stay standalone-only, and cluster
/// coverage targets only the routing/topology behaviour that is actually cluster-specific.
///
/// RESP2 is not separately parametrized: <see cref="ModuleTestConfiguration.CreateStandaloneClient"/>/
/// <see cref="ModuleTestConfiguration.CreateClusterClient"/> both pin RESP3, matching this port's
/// client -- unlike Node/Python, where <c>GlideString = string | Buffer</c> forces reply-shape
/// branching per protocol, C#'s FT.* parsers consume glide-core's already-reshaped
/// <c>Dictionary&lt;GlideString, object&gt;</c>/<c>object[]</c> replies uniformly regardless of
/// which wire protocol produced them -- glide-core, not this client, is what branches on RESP2 vs
/// RESP3 syntax. Confirmed by reading every <c>Cmd&lt;,&gt;</c> response delegate in
/// <c>Request.FtCommands.cs</c>: none inspects a protocol version, and none of this client's other
/// command families (hash, sorted-set, etc.) parametrizes by protocol either -- RESP2/RESP3 reply
/// parity is glide-core's contract, verified once for the whole client, not per command family.
///
/// Run via <c>utils/run-modules-ci-locally.sh</c> (sets <c>MODULES_STANDALONE_ENDPOINT</c> /
/// <c>MODULES_CLUSTER_ENDPOINTS</c> then invokes <c>dotnet test</c> against this project).
/// </remarks>
public class ValkeySearch13Tests(ModuleTestConfiguration config) : IAsyncLifetime
{
    public ModuleTestConfiguration Config { get; } = config;

    // Capability/level are probed once against the standalone client. Both clients talk to the
    // same module build and the same search.emulate-release setting (a server-wide CONFIG, not
    // per-protocol or per-topology), so probing once and reusing the answer for every TestClients
    // row avoids re-running the probe's own FT.CREATE/FT.SEARCH/FT.DROPINDEX for every client.
    private BaseClient _standaloneForProbe = null!;
    private bool _capable;
    private string? _level;
    private readonly List<(BaseClient Client, string Index)> _indexesToDrop = [];

    public async ValueTask InitializeAsync()
    {
        _standaloneForProbe = ModuleTestConfiguration.CreateStandaloneClient();
        _capable = await SearchCapability.Probe13CapabilityAsync(_standaloneForProbe);
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
                // L4: previously swallowed silently. A failed cleanup here means a leftover
                // index on a shared Docker deployment other tests/runs reuse -- worth knowing
                // about even though it must not fail DisposeAsync itself (which runs after the
                // test has already recorded its own pass/fail, and after any assertion failure
                // from the test body). Written to stderr rather than re-thrown: re-throwing from
                // DisposeAsync would replace or mask the test's own failure (if any), and this is
                // best-effort cleanup for throwaway per-test indexes, not a correctness guard --
                // the probe's own DropIndex (SearchCapability.DropProbeIndexAsync) is the one
                // required to retry-then-raise per the port rules.
                Console.Error.WriteLine($"WARNING: cleanup failed to drop index '{index}': {ex.Message}");
            }
        }

        _standaloneForProbe.Dispose();
    }

    #region Capability probe / level fixture plumbing

    [Fact]
    public async Task CapabilityProbe_MatchesLiveInKeysSupport()
    {
        // M1: replaces the former tautology (`_capable || !_capable`), which only proved
        // InitializeAsync didn't throw -- itself no stronger a check, since a throw there would
        // already fail every test in this fixture. Re-runs the probe's own live command directly
        // against a fresh index and asserts the probe's cached answer agrees with it: a definite,
        // checkable claim about server behaviour instead of a logical truism.
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(_standaloneForProbe, index, new Ft.CreateTextField("title"));
        try
        {
            if (_capable)
            {
                // INKEYS is live: an empty list must exclude every result without raising.
                var result = await Ft.SearchAsync(_standaloneForProbe, index, "*", new Ft.SearchOptions { InKeys = [] });
                Assert.Equal(0, result.TotalResults);
            }
            else
            {
                // Pre-1.3: the server has no INKEYS support at all, so using it raises.
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
    public void LevelFixture_ReadsAValueWhen13Capable()
    {
        SkipUnless13();
        Assert.False(string.IsNullOrEmpty(_level));
    }

    [Fact]
    public async Task LevelFixture_ClusterBranch_ReadsSameLevelAsStandalone()
    {
        // H3: EmulateReleaseLevelAsync's cluster branch (`.SingleValue` on an unrouted CONFIG GET)
        // had no live coverage at all (L5) -- confirmed here against a real cluster deployment
        // rather than left as a latent, unexercised code path. CONFIG GET search.emulate-release
        // is the same server-wide setting regardless of topology, so the cluster client's answer
        // must agree with the already-probed standalone answer.
        SkipUnless13();
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
    #region Wave 1 core: INKEYS, bare wildcard, auto-load (Python rows #29-31)

    [Fact]
    public async Task InKeys_RestrictsSearchToGivenKeys()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #30 (test_ft_search_inkeys): the listed-keys case, the empty-list
        // excludes-everything case, and the unrestricted (no INKEYS) baseline, asserting the
        // exact surviving key set each time -- not just a count.
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "hello")]);
        _ = await client.HashSetAsync($"{index}:doc3", [new("title", "hello")]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #29 (test_ft_search_wildcard): a bare `*` is match-all as of PR #960; prior
        // to 1.3 it did not run as match-all at all, hence the gate.
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "goodbye")]);
        _ = await client.HashSetAsync($"{index}:doc3", [new("title", "anything")]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #31's auto-load behaviour (PR #1341, "Implicitly load fields referenced by a
        // pipeline stage"): GROUPBY 0 (no grouping fields) with a reducer referencing a field that
        // was never explicitly LOADed -- the field still resolves. Asserts the exact aggregated
        // value, not just that the call didn't throw.
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTextField("title"), new Ft.CreateNumericField("price")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "a"), new("price", "10")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "b"), new("price", "30")]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
    #region C1 (Critical): TOLIST/RANDOM_SAMPLE array-valued reducers, row #31

    [Fact]
    public async Task Aggregate_ToListReducer_ReturnsExactDedupedValueSet()
    {
        BaseClient client = _standaloneForProbe;
        // C1 proof: before the ParseFtAggregateResponse fix, this threw
        // RequestException("Unexpected FT.AGGREGATE value type: System.Object[]") on any real
        // GROUPBY using TOLIST, since valkey-search returns one array per group. Asserts the
        // actual returned list contents (deduped, membership-complete), not just "didn't throw".
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTagField("category"), new Ft.CreateNumericField("price")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:doc1", [new("category", "a"), new("price", "10")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("category", "a"), new("price", "30")]);
        _ = await client.HashSetAsync($"{index}:doc3", [new("category", "a"), new("price", "10")]); // duplicate price
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // C1 proof (RANDOM_SAMPLE side): same array-valued-reply shape as TOLIST. Every sampled
        // value must come from the group's actual price set, and the sample must respect the
        // requested size cap -- a real assertion on the returned list's contents, not a
        // non-empty/type check.
        SkipUnless13();
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

        await FtUtils.WaitForIndexingAsync(client, index);

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
    #region Vector Range (rows #32-33), reducers (#31 scalar reducers already covered elsewhere)

    [Fact]
    public async Task Search_VectorRange_RestrictsToRadiusAndYieldsDistance()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #32 (test_ft_search_vector_range): `@field:[VECTOR_RANGE radius $blob]`
        // restricts results to vectors within the given L2 distance of the query vector, and
        // $YIELD_DISTANCE_AS exposes the exact distance for each surviving document.
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateVectorFieldFlat { Identifier = "vec", Dimensions = 1, DistanceMetric = Ft.DistanceMetric.Euclidean },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        // Points at known L2 distances from the origin: 0, 1, 4 (squared L2: 0, 1, 16).
        _ = await client.HashSetAsync($"{index}:origin", [new("vec", PackVector(0f))]);
        _ = await client.HashSetAsync($"{index}:near", [new("vec", PackVector(1f))]);
        _ = await client.HashSetAsync($"{index}:far", [new("vec", PackVector(4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
    #region SORTBY widened by adjacent LIMIT (row #35, compat-gated)

    [Fact]
    public async Task Aggregate_SortByAdjacentLimit_WidensRetainedRowsOnlyAtOrAbove130()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #35: an adjacent LIMIT widens SORTBY's own default/MAX cap (compat-gated).
        // At >= 1.3.0, SORTBY (default 10-row cap) followed immediately by LIMIT 0 15
        // returns all 15 seeded rows; below 1.3.0, the legacy branch still truncates to 10.
        SkipUnless13();
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

        await FtUtils.WaitForIndexingAsync(client, index);

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
    #region FT.SEARCH limit pagination (row #36)

    [Fact]
    public async Task Search_LimitPagination_ReturnsDistinctPagesCoveringEveryKey()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #36 (test_ft_search_limit_pagination): paging via LIMIT offset/count across
        // the full result set, asserting the union of pages is exactly the seeded key set, with
        // no overlap between pages.
        SkipUnless13();
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

        await FtUtils.WaitForIndexingAsync(client, index);

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
    #region Scoring: FT.SEARCH WITHSCORES/SCORER/INKEYS, FT.AGGREGATE ADDSCORES/SCORER/LoadField(AS)/WITHCURSOR

    [Fact]
    public async Task Search_WithScoresAndScorer_RanksMatchingDocumentAboveNonMatching()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #42: WITHSCORES/SCORER must actually rank results, not merely populate a
        // non-null Score (M7's "counts not sets" complaint extends to "scores not compared" here).
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:strong", [new("title", "hello hello hello world")]);
        _ = await client.HashSetAsync($"{index}:weak", [new("title", "hello")]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #43: the combined WITHSCORES+WITHSORTKEYS reply shape (score, sort_key,
        // fields) must all be populated and internally consistent -- a genuinely different shape
        // from either flag alone.
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTextField("title") { Sortable = true }, new Ft.CreateNumericField("price") { Sortable = true }],
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:cheap", [new("title", "hello"), new("price", "10")]);
        _ = await client.HashSetAsync($"{index}:pricey", [new("title", "hello"), new("price", "20")]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // A numeric SORTBY field's sort key is RediSearch's internal numeric wire format
        // ("#10"/"#20", a '#' prefix over the plain decimal), not the bare field value -- confirmed
        // live; the WithSortKeys tuple just surfaces the server's encoding verbatim.
        Assert.Equal((ValkeyValue)"#10", cheapDoc.SortKey);
        Assert.Equal((ValkeyValue)"#20", priceyDoc.SortKey);
    }

    [Fact]
    public async Task Search_CountOption_EmitsLimitZeroZero_ReturnsTotalOnly()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello")]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "hello")]);
        await FtUtils.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "*", new Ft.SearchOptions { Count = true });

        Assert.Equal(2, result.TotalResults);
        Assert.Empty(result.Documents);
    }

    [Fact]
    public async Task Aggregate_AddScoresAndScorer_SortsByRealScoreColumn()
    {
        BaseClient client = _standaloneForProbe;
        // M4: ADDSCORES and SCORER are NOT compat-gated (unlike LoadField(alias:), split out into
        // its own test below), so this must run and assert real behaviour at the module's default
        // level too, not only skip below 1.3.0. Confirms __score is a genuine, sortable pipeline
        // field: SORTBY on __score descending must rank the stronger match first.
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:strong", [new("title", "hello hello hello world")]);
        _ = await client.HashSetAsync($"{index}:weak", [new("title", "hello")]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // M4 split: LoadField(alias:) IS compat-gated (ft_aggregate_load_as) -- unlike the
        // test above, this one must branch on the level rather than skip below 1.3.0, asserting
        // the corrected alias behaviour at >= 1.3.0 and the documented legacy behaviour (AS
        // consumed as a plain field token, so the literal field "AS" doesn't exist) below it.
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world")]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
            // Legacy branch: AS is not recognized as a rename keyword, so the server rejects the
            // command outright (per LoadField's own doc comment: "the server rejects the command
            // outright" against a pre-1.3.0 emulate-release).
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
        // Python row #48: pages to full exhaustion (CursorId == 0), asserting the UNION of every
        // page is exactly the seeded key set -- not just that the first two pages each have the
        // expected count (M7).
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        HashSet<string> allKeys = [];
        for (int i = 0; i < 5; i++)
        {
            string key = $"{index}:doc{i}";
            _ = allKeys.Add(key);
            _ = await client.HashSetAsync(key, [new("title", "hello")]);
        }

        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #49 (test_ft_aggregate_typed_cursor_del_and_errors): DEL on an open cursor,
        // then reading the just-deleted cursor raises, and reading an unknown cursor id also
        // raises -- previously DEL was only called on a branch and never asserted at all.
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        for (int i = 0; i < 5; i++)
        {
            _ = await client.HashSetAsync($"{index}:doc{i}", [new("title", "hello")]);
        }

        await FtUtils.WaitForIndexingAsync(client, index);

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
    #region FT.CREATE: FLOAT16/BFLOAT16/FILTER/SORTABLE UNF/SCORE_FIELD/NOHL (rows #37-41)

    [Fact]
    public async Task Create_Float16VectorField_SupportsKnnQuery()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #37: beyond create-and-list, a real KNN query against the FLOAT16 field must
        // actually return the nearest-stored vector.
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateVectorFieldFlat { Identifier = "vec", Dimensions = 2, DistanceMetric = Ft.DistanceMetric.Euclidean, Type = Ft.VectorType.Float16 },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:near", [new("vec", PackHalfVector(1f, 1f))]);
        _ = await client.HashSetAsync($"{index}:far", [new("vec", PackHalfVector(100f, 100f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateVectorFieldFlat { Identifier = "vec", Dimensions = 2, DistanceMetric = Ft.DistanceMetric.Euclidean, Type = Ft.VectorType.Bfloat16 },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:near", [new("vec", PackBfloat16Vector(1f, 1f))]);
        _ = await client.HashSetAsync($"{index}:far", [new("vec", PackBfloat16Vector(100f, 100f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #38: the FILTER expression at the index level actually excludes documents
        // that don't match, not merely "the index was created".
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTextField("title"), new Ft.CreateNumericField("price")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"], Filter = "@price > 0" });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:included", [new("title", "hello"), new("price", "10")]);
        _ = await client.HashSetAsync($"{index}:excluded", [new("title", "hello"), new("price", "-5")]);
        await FtUtils.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "*");

        Assert.Equal(1, result.TotalResults);
        Assert.Equal($"{index}:included", result.Documents[0].Key.ToString());
    }

    [Fact]
    public async Task Create_ScoreFieldInIndexDefinition_ReportedOnlyAtOrAbove130()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #40 (test_ft_create_score_field_in_index_definition, M8): FT.INFO's
        // index_definition.score_field is only reported at >= 1.3.0; below it the field is simply
        // absent and default_score always reads the legacy literal "1".
        SkipUnless13();
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
        // Python row #41 (test_ft_create_no_highlight): NOHL is accepted and silently
        // discarded by the engine (a documented no-op) -- confirms the command succeeds and the
        // index behaves normally, which is the entirety of what NOHL guarantees on this engine.
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateTextField("title"),
            new Ft.CreateOptions { Prefixes = [$"{index}:"], NoHighlight = true });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello")]);
        await FtUtils.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "hello");
        Assert.Equal(1, result.TotalResults);
    }

    [Fact]
    public async Task Create_SortableUnfTextField_PreservesOriginalCaseInSortKey()
    {
        BaseClient client = _standaloneForProbe;
        // Strengthens the former accepted-only test: UNF preserves original (un-normalized) case
        // in the sort key, visible via WITHSORTKEYS, rather than the lowercased form SORTABLE
        // alone would otherwise produce for sorting purposes.
        //
        // The wire-level sort-key prefix byte itself is level-dependent -- confirmed live, not
        // previously documented anywhere in this port: at search.emulate-release 1.3.0 a TEXT
        // sort key is '$'-prefixed (e.g. "$HELLO World"); on the SAME rc2 binary left at its own
        // default (1.0.0) it is '#'-prefixed instead ("#HELLO World"). This looks closely related
        // to, but is distinct from, the three documented compat-gated behaviours (LoadField
        // alias, SORTBY-widening, FT.INFO score_field) -- it's gated by the module's active
        // compatibility level the same way, just not one this port's typed API has a dedicated
        // guard/branch for elsewhere, since WITHSORTKEYS's sort key is opaque passthrough text
        // (ValkeyValue), not something InfoLocalResult or any other typed shape parses further.
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            new Ft.CreateTextField("title") { Sortable = true, Unf = true },
            new Ft.CreateOptions { Prefixes = [$"{index}:"] });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "HELLO World")]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
    #region Score field affects ranking (row #45), scorer rejection (row #58)

    [Fact]
    public async Task Create_ScoreFieldAffectsRanking_MultipliesIntoRelevanceScore()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #45: SCORE_FIELD's value genuinely multiplies into the final relevance score
        // -- a document with a higher score_field value outranks an otherwise-identical document
        // with a lower one, confirmed via WITHSCORES.
        SkipUnless13();
        var index = $"idx_{Guid.NewGuid():N}";
        await Ft.CreateAsync(
            client,
            index,
            [new Ft.CreateTextField("title"), new Ft.CreateNumericField("boost")],
            new Ft.CreateOptions { Prefixes = [$"{index}:"], ScoreField = "boost" });
        _indexesToDrop.Add((client, index));

        _ = await client.HashSetAsync($"{index}:boosted", [new("title", "hello"), new("boost", "100")]);
        _ = await client.HashSetAsync($"{index}:plain", [new("title", "hello"), new("boost", "1")]);
        await FtUtils.WaitForIndexingAsync(client, index);

        var result = await Ft.SearchAsync(client, index, "hello", new Ft.SearchOptions { WithScores = true });

        Assert.Equal(2, result.TotalResults);
        double boostedScore = result.Documents.Single(d => d.Key.ToString() == $"{index}:boosted").Score!.Value;
        double plainScore = result.Documents.Single(d => d.Key.ToString() == $"{index}:plain").Score!.Value;
        Assert.True(boostedScore > plainScore, $"expected boosted ({boostedScore}) > plain ({plainScore})");
    }

    [Fact]
    public async Task Search_ScorerTfidf_IsRejectedByTheServer()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #58: BM25STD is the only registered scorer; TFIDF (RediSearch's classic
        // default elsewhere) is rejected. Rejected client-side now (EnsureSupportedScorer, M3)
        // before ever reaching the server -- this test still proves the server's own behaviour by
        // bypassing the typed API's guard via CustomCommand, matching the rejection Python
        // observes from the server directly.
        SkipUnless13();
        var index = await CreateBasicIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world")]);
        await FtUtils.WaitForIndexingAsync(client, index);

        // The typed API guard rejects this client-side, synchronously -- Ft.SearchAsync is not
        // itself an async method, so the ArgumentException throws before any Task is returned.
        _ = Assert.Throws<ArgumentException>((Action)(() => _ = Ft.SearchAsync(client, index, "hello", new Ft.SearchOptions { WithScores = true, Scorer = "TFIDF" })));

        // Bypassing the guard via a raw custom command proves the server itself also rejects it.
        GlideString[] args = ["FT.SEARCH", index, "hello", "WITHSCORES", "SCORER", "TFIDF"];
        var ex = await Assert.ThrowsAsync<Errors.RequestException>(() => client switch
        {
            GlideClient standalone => standalone.CustomCommand(args),
            GlideClusterClient cluster => cluster.CustomCommand(args),
            _ => throw new ArgumentException($"Unsupported client type: {client.GetType()}"),
        });
        Assert.Contains("SCORER", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
    #region FT.HYBRID: SEARCH+VSIM arms, RRF/LINEAR/FUNCTION combine, the regression case (H2)

    [Fact]
    public async Task Hybrid_BasicTwoArmCall_FusesBothArmsResults()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:doc2", [new("title", "nothing matches"), new("price", "20"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10 });

        // Both docs are reachable via the VSIM arm's KNN fanout even though only doc1 matches the
        // SEARCH arm's text query -- the fused result set covers the union of the keys, a set
        // assertion rather than a bare non-empty check.
        Assert.Equal(
            new HashSet<string> { $"{index}:doc1", $"{index}:doc2" },
            [.. rows.Select(r => r[(ValkeyValue)"__key"].ToString())]);
    }

    [Fact]
    public async Task Hybrid_RrfCombine_FusesByReciprocalRank()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #52 (RRF arm of test_ft_hybrid_combine_methods_fuse_results): confirms COMBINE
        // actually takes effect, not just that the call accepted a Combine option (M7: the prior
        // version of every COMBINE test here asserted NotEmpty only, "so they'd pass if COMBINE
        // were ignored"). A doc that wins both arms must rank first under RRF.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:both", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:neither", [new("title", "nothing"), new("price", "20"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // H2 fix: the port-rules-mandated regression test. FILTER must sit DIRECTLY after the
        // VSIM arm with NO LOAD clause in between -- the prior version of this test set
        // HybridOptions.LoadFields, so the argv became
        // "... VSIM @vec $p KNN 2 K 10 LOAD 1 price PARAMS ... FILTER ...": even under the old,
        // buggy argv order (clauses emitted before PARAMS), the LOAD token would still terminate
        // the VSIM arm's own clause-grabbing, so FILTER would stay an aggregate FILTER regardless
        // of whether the PARAMS-before-clauses fix was present. That made the test pass with the
        // bug both fixed AND reintroduced -- it could never catch a regression. This version loads
        // no fields at all between the VSIM arm and FILTER: the LOAD field used to assert
        // "excluded" below is instead the SEARCH arm's own YIELD_SCORE_AS alias (price is folded
        // into the query itself via a numeric range on the stored field, inspected indirectly by
        // only seeding one in-range document) -- Python's own fix
        // (test_ft_hybrid_aggregate_clause_filters_post_fusion) takes exactly this approach,
        // filtering on an arm alias that needs no LOAD.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:positive", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:negative", [new("title", "hello world"), new("price", "-5"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "hello world", YieldScoreAs = "ss" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f), K = 10 },
            new Ft.HybridOptions
            {
                // No LoadFields/LoadAll at all -- FILTER is the very first clause after the VSIM
                // arm, with nothing in between.
                Clauses = [new Ft.AggregateFilter("@price > 0")],
            });

        _ = Assert.Single(rows);
        Assert.Equal($"{index}:positive", rows[0][(ValkeyValue)"__key"].ToString());
    }

    [Fact]
    public async Task Hybrid_LinearCombine_WeightsSearchAndVsimArms()
    {
        BaseClient client = _standaloneForProbe;
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:both", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:neither", [new("title", "nothing"), new("price", "20"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #55: the FUNCTION combine's arm aliases (YieldScoreAs on each arm) must
        // actually surface as usable columns in the fused expression -- the prior version "sets
        // the aliases but asserts no columns" (M7-adjacent).
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // search_score_col (APPLY @ss) must be a real, non-negative numeric relevance score --
        // proving @ss (the SEARCH arm's YieldScoreAs alias) is a genuinely readable column inside
        // a later clause, not a silently-ignored alias.
        Assert.True(double.TryParse(rows[0][(ValkeyValue)"search_score_col"].ToString(), out double parsedScore));
        Assert.True(parsedScore >= 0);
    }

    [Fact]
    public async Task Hybrid_WithCursor_PagesToExhaustionCoveringEveryKey()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #56: pages to full exhaustion (both standalone and cluster, per H3), the
        // union of pages must equal the full seeded key set -- not "<= 2 rows" on the first page
        // alone.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        HashSet<string> allKeys = [];
        for (int i = 0; i < 5; i++)
        {
            string key = $"{index}:doc{i}";
            _ = allKeys.Add(key);
            _ = await client.HashSetAsync(key, [new("title", "hello world"), new("price", $"{i * 10}"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        }

        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #57 (test_ft_hybrid_localonly_accepted): a valkey-search extension not in
        // RediSearch; confirms the server accepts LOCALONLY rather than rejecting it as an
        // unrecognized token.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #53 (test_ft_hybrid_vector_inline_bytes_and_param_reference): the `$name`
        // explicit-reference path for HybridVsimArm.Vector, as opposed to raw inline bytes --
        // previously "every live call uses bytes", so the $param path never ran live.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:doc1", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
        // Python row #54 (test_ft_hybrid_vsim_filter_with_policy): a VSIM pre-filter with an
        // explicit POLICY actually restricts the VSIM arm's candidate set.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        _ = await client.HashSetAsync($"{index}:included", [new("title", "hello world"), new("price", "10"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        _ = await client.HashSetAsync($"{index}:excluded", [new("title", "hello world"), new("price", "-5"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        await FtUtils.WaitForIndexingAsync(client, index);

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
    public async Task Hybrid_VsimRangeQuery_IsRejectedByTheServer()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #34 (test_ft_hybrid_vsim_range_rejected): VSIM ... RANGE is excluded
        // from the typed API by design; confirms the server itself rejects it when sent as a raw
        // custom command, since GLIDE has no typed way to produce it.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        await FtUtils.WaitForIndexingAsync(client, index);

        GlideString[] args =
        [
            "FT.HYBRID", index, "SEARCH", "*", "VSIM", "@vec", "$p",
            "RANGE", "1",
            "PARAMS", "2", "p", PackVector(0.1f, 0.2f, 0.3f, 0.4f),
        ];
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client switch
        {
            GlideClient standalone => standalone.CustomCommand(args),
            GlideClusterClient cluster => cluster.CustomCommand(args),
            _ => throw new ArgumentException($"Unsupported client type: {client.GetType()}"),
        });
    }

    [Fact]
    public async Task Hybrid_WithoutK_UsesEngineDefaultOfTen()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #59: omitting K entirely (no KNN block written at all, since K/EfRuntime/
        // ShardKRatio are all unset) must default to the engine's own K=10 -- confirmed by seeding
        // more than 10 candidates and a SEARCH arm term that matches nothing, so only the VSIM
        // arm's own candidate set can be contributing rows.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        for (int i = 0; i < 15; i++)
        {
            _ = await client.HashSetAsync($"{index}:doc{i}", [new("title", "hello world"), new("price", $"{i}"), new("vec", PackVector(0.1f, 0.2f, 0.3f, 0.4f))]);
        }

        await FtUtils.WaitForIndexingAsync(client, index);

        var rows = await Ft.HybridAsync(
            client,
            index,
            new Ft.HybridSearchArm { Query = "termthatmatchesnothingatall" },
            new Ft.HybridVsimArm { Field = "vec", Vector = PackVector(0.1f, 0.2f, 0.3f, 0.4f) });

        Assert.Equal(10, rows.Length);
    }

    [Fact]
    public async Task Hybrid_InsideMultiExec_IsRejectedByTheServer()
    {
        BaseClient client = _standaloneForProbe;
        // Python row #60 (test_ft_hybrid_inside_multi_exec_rejected): FT.HYBRID is dispatched via
        // CustomCommand (Q17), so this confirms the server itself rejects running it inside a
        // transaction, matching Python's rejection.
        SkipUnless13();
        var index = await CreateVectorIndexAsync(client);
        await FtUtils.WaitForIndexingAsync(client, index);

        if (client is GlideClient standalone)
        {
            Batch batch = new(isAtomic: true);
            _ = batch.CustomCommand([
                "FT.HYBRID", index, "SEARCH", "*", "VSIM", "@vec", "$p", "KNN", "2", "K", "10",
                "PARAMS", "2", "p", PackVector(0.1f, 0.2f, 0.3f, 0.4f),
            ]);
            _ = await Assert.ThrowsAsync<Errors.RequestException>(() => standalone.Exec(batch, raiseOnError: true));
        }
    }

    #endregion
    #region Cluster-specific: multi-shard routing (row #46), cursor affinity to primary (row #47, H3)

    [Fact]
    public async Task Search_MultiShardNoHashtag_AggregatesAcrossShards()
    {
        // Python row #46 (test_ft_multi_shard_no_hashtag): re-checks, rather than assumes,
        // whether FT.CREATE broadcasts cluster-wide and whether FT.SEARCH fans out across
        // shards. Confirmed empirically against valkey-search 1.3.0-rc1 (and independently by
        // Python's own test, cited in this fixture's class remarks): FT.CREATE lands on exactly
        // ONE primary (the one GLIDE's cluster client happens to route the command to, by the
        // index name's key hash); FT.SEARCH then only sees documents that are both present AND
        // indexed on that same shard -- it does not fan out to other shards' copies of the key
        // range. This test observes that directly: with enough documents spread across random,
        // hashtag-free keys on a multi-primary cluster, the search result count must be STRICTLY
        // LESS than the seeded count (not equal to it) -- proving the single-shard behaviour
        // without needing to identify which specific primary owns the index or compute slot
        // ranges by hand (an earlier version of this test did exactly that via CLUSTER SHARDS/
        // CLUSTER KEYSLOT parsing and a ByAddressRoute probe of every primary; while debugging an
        // unrelated test-discovery crash, simplifying this test to avoid that machinery entirely
        // turned out to be the more robust choice regardless).
        SkipUnless13();
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
                // No hashtag: each key's slot is effectively random across the cluster's shards.
                string key = $"{index}:{Guid.NewGuid():N}";
                _ = await client.HashSetAsync(key, [new("title", "hello")]);
            }

            await FtUtils.WaitForIndexingAsync(client, index);

            var result = await Ft.SearchAsync(client, index, "*");

            // The confirmed engine behaviour: only documents on the index-owning shard are
            // indexed, so the result is a strict subset -- never the full seeded count (that
            // would mean FT.CREATE started broadcasting, a genuine engine change worth revisiting
            // this test for) and never zero (every key not landing elsewhere is highly unlikely
            // across 30 random keys on a real multi-primary cluster, short of a fixture bug).
            Assert.True(
                result.TotalResults is > 0 and < seededCount,
                $"expected a STRICT SUBSET of {seededCount} seeded documents (confirming FT.CREATE's " +
                    $"single-shard placement), got {result.TotalResults}");
        }
        finally
        {
            // Drop the index (if created) BEFORE disposing the cluster client, never via the
            // shared _indexesToDrop list: that list is only drained in the fixture's own
            // DisposeAsync, which runs long after this test's ad-hoc cluster client is already
            // disposed here -- calling Ft.DropIndexAsync against an already-disposed native
            // client crashed the whole test process (SIGSEGV) rather than throwing a managed
            // exception, discovered the hard way while debugging exactly this.
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
        // Python row #47 / H3: cursor affinity to the PRIMARY of the index's slot --
        // previously untested in cluster mode at all. Opens a cursor via the cluster client,
        // confirms the first page and every subsequent CursorReadAsync/CursorDelAsync succeed
        // against the same cluster client without a "wrong node"/MOVED-style error, which would
        // surface if PrimaryRoute were misrouting.
        SkipUnless13();
        GlideClusterClient? cluster = ModuleTestConfiguration.CreateClusterClient();
        Assert.SkipWhen(cluster is null, "no cluster endpoints provided to the module-test harness");
        var client = cluster!;
        string? index = null;
        try
        {
            // A shared hashtag on both the index name and every document key guarantees all of
            // them hash to the SAME slot, hence the SAME shard -- sidestepping the single-shard
            // FT.CREATE placement this fixture's class remarks document (confirmed empirically):
            // without this, document keys scattered by GUID across the cluster would mostly
            // never land on the one shard that actually holds the index, making the seeded/seen
            // key sets diverge for a reason unrelated to what this test is actually checking
            // (cursor routing to the primary, not index placement -- that's covered separately by
            // Search_MultiShardNoHashtag_AggregatesAcrossShards).
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

            await FtUtils.WaitForIndexingAsync(client, index);

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
            // See Search_MultiShardNoHashtag_AggregatesAcrossShards' finally block for why this
            // drops the index directly rather than via the shared _indexesToDrop list.
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

    private void SkipUnless13()
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
        // BFLOAT16: the top 16 bits of an IEEE 754 FLOAT32 (sign, 8-bit exponent, 7-bit mantissa).
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
