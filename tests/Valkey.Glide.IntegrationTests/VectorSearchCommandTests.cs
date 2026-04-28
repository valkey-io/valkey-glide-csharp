// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for FT.* (Valkey Search) commands.
/// Requires a Valkey or Redis server with the Search module loaded.
/// Set standalone-endpoints or cluster-endpoints env vars to point at such a server.
/// </summary>
public class ValkeySearchCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    // ── helpers ──────────────────────────────────────────────────────────────

    /// <summary>Skip the test if the FT module is not available on this server.</summary>
    private static async Task SkipIfModuleNotAvailable(BaseClient client)
    {
        try
        {
            _ = await client.FtListAsync();
        }
        catch (Exception ex) when (
            ex.Message.Contains("unknown command", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("ERR unknown", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("module", StringComparison.OrdinalIgnoreCase))
        {
            Assert.Skip("Valkey Search module not available on this server.");
        }
    }

    // ── FT.CREATE ────────────────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_SimpleHnswVectorIndex_ReturnsOk(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        await client.FtCreateAsync(Guid.NewGuid().ToString(),
            [new VectorFieldHnsw("vec", DistanceMetric.Euclidean, 2) { Alias = "VEC" }]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_JsonFlatVectorWithPrefix_ReturnsOk(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        await client.FtCreateAsync(Guid.NewGuid().ToString(),
            [new VectorFieldFlat("$.vec", DistanceMetric.Euclidean, 6) { Alias = "VEC" }],
            new FtCreateOptions { DataType = IndexDataType.Json, Prefixes = ["json:"] });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_HnswWithExtraParams_ReturnsOk(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        await client.FtCreateAsync(Guid.NewGuid().ToString(),
            [new VectorFieldHnsw("doc_embedding", DistanceMetric.Cosine, 1536)
            {
                NumberOfEdges = 40,
                VectorsExaminedOnConstruction = 250,
                VectorsExaminedOnRuntime = 40,
            }],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = ["docs:"] });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_MultipleFields_ReturnsOk(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        await client.FtCreateAsync(Guid.NewGuid().ToString(),
            [new TextField("title"), new NumericField("published_at"), new TagField("category")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = ["blog:post:"] });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_DuplicateIndex_Throws(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string idx = Guid.NewGuid().ToString();
        await client.FtCreateAsync(idx, [new TextField("title")]);
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtCreateAsync(idx, [new TextField("title")]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_BinaryIndexName_ReturnsOk(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        // Index names are Valkey keys and can contain arbitrary bytes.
        byte[] binaryName = [0x01, 0x02, 0x03, (byte)'i', (byte)'d', (byte)'x'];
        await client.FtCreateAsync((ValkeyKey)binaryName, [new TextField("title")]);
        await client.FtDropIndexAsync((ValkeyKey)binaryName);
    }

    // ── FT.DROPINDEX + FT._LIST ───────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtDropIndex_ExistingIndex_RemovedFromList(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string idx = Guid.NewGuid().ToString();
        await client.FtCreateAsync(idx, [new VectorFieldHnsw("vec", DistanceMetric.Euclidean, 2)]);

        ISet<string> before = await client.FtListAsync();
        Assert.Contains(idx, before);

        await client.FtDropIndexAsync(idx);

        ISet<string> after = await client.FtListAsync();
        Assert.DoesNotContain(idx, after);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtDropIndex_NonExistent_Throws(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtDropIndexAsync(Guid.NewGuid().ToString()));
    }

    // ── FT.SEARCH ────────────────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_QueryAsValkeyValue_Works(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";
        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "hello")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // ValkeyValue wrapping a byte[] is accepted as a query argument.
        FtSearchResult result = await client.FtSearchAsync(idx,
            (ValkeyValue)System.Text.Encoding.UTF8.GetBytes("@title:hello"));
        Assert.Equal(1L, result.TotalResults);

        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_InvalidBinaryQuery_Throws(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string idx = Guid.NewGuid().ToString();
        await client.FtCreateAsync(idx, [new TextField("title")]);

        // Raw non-ASCII bytes are not parseable as query syntax — the server must reject them.
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtSearchAsync(idx, (ValkeyValue)new byte[] { 0x01, 0x02, 0x03 }));

        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_VectorKnn_ReturnsBothDocs(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new VectorFieldHnsw("vec", DistanceMetric.Euclidean, 2) { Alias = "VEC" }],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        // Two 2D float32 vectors as raw bytes — matching the Go test exactly.
        // Go passes raw byte slices via string([]byte{...}); in C# we use byte[] → ValkeyValue
        // to avoid UTF-8 encoding corruption of bytes >= 0x80.
        byte[] vec0 = new byte[8];
        byte[] vec1 = [0, 0, 0, 0, 0, 0, 0x80, 0xBF];
        _ = await client.HashSetAsync(prefix + "0", [new HashEntry("vec", (ValkeyValue)vec0)]);
        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("vec", (ValkeyValue)vec1)]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtSearchResult result = await client.FtSearchAsync(idx, "*=>[KNN 2 @VEC $query_vec]",
            new FtSearchOptions
            {
                Params = [new FtSearchParam("query_vec", System.Text.Encoding.Latin1.GetString(vec0))],
                ReturnFields = [new FtSearchReturnField("vec")],
            });

        Assert.Equal(2L, result.TotalResults);
        Assert.Contains(result.Documents, d => d.Key == prefix + "0");
        Assert.Contains(result.Documents, d => d.Key == prefix + "1");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_NoContent_ReturnsKeysOnly(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new VectorFieldFlat("vec", DistanceMetric.Euclidean, 2) { Alias = "VEC" }],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        byte[] vec0 = new byte[8];
        _ = await client.HashSetAsync(prefix + "0", [new HashEntry("vec", System.Text.Encoding.Latin1.GetString(vec0))]);
        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("vec", System.Text.Encoding.Latin1.GetString(vec0))]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtSearchResult result = await client.FtSearchAsync(idx, "*=>[KNN 2 @VEC $query_vec]",
            new FtSearchOptions
            {
                NoContent = true,
                Params = [new FtSearchParam("query_vec", System.Text.Encoding.Latin1.GetString(vec0))],
            });

        Assert.Equal(2L, result.TotalResults);
        Assert.Equal(2, result.Documents.Count);
        Assert.Contains(result.Documents, d => d.Key == prefix + "0");
        Assert.Contains(result.Documents, d => d.Key == prefix + "1");
        // With NOCONTENT each doc should have empty fields
        foreach (var doc in result.Documents)
        {
            Assert.Empty(doc.Fields);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_NonExistentIndex_Throws(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtSearchAsync(Guid.NewGuid().ToString(), "*"));
    }

    // ── FT.SEARCH 1.2 options ─────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_SortByAsc_ReturnsOrderedResults(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new NumericField("price") { Sortable = true }, new TextField("name")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("price", "10"), new HashEntry("name", "Aardvark")]);
        _ = await client.HashSetAsync(prefix + "2", [new HashEntry("price", "20"), new HashEntry("name", "Mango")]);
        _ = await client.HashSetAsync(prefix + "3", [new HashEntry("price", "30"), new HashEntry("name", "Zebra")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // ASC — verify documents are returned in ascending price order
        FtSearchResult asc = await client.FtSearchAsync(idx, "@price:[1 +inf]",
            new FtSearchOptions { SortBy = new FtSearchSortBy("price", FtSearchSortOrder.Ascending) });
        Assert.Equal(3L, asc.TotalResults);
        Assert.Equal(3, asc.Documents.Count);
        var ascPrices = asc.Documents
            .Select(d => d.Fields["price"])
            .ToList();
        Assert.Equal(["10", "20", "30"], ascPrices);

        // DESC — verify documents are returned in descending price order
        FtSearchResult desc = await client.FtSearchAsync(idx, "@price:[1 +inf]",
            new FtSearchOptions { SortBy = new FtSearchSortBy("price", FtSearchSortOrder.Descending) });
        Assert.Equal(3L, desc.TotalResults);
        Assert.Equal(3, desc.Documents.Count);
        var descPrices = desc.Documents
            .Select(d => d.Fields["price"])
            .ToList();
        Assert.Equal(["30", "20", "10"], descPrices);

        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_TextQueryFlags_VerbatimInOrderSlop(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "hello world")]);
        _ = await client.HashSetAsync(prefix + "2", [new HashEntry("title", "hello there")]);
        _ = await client.HashSetAsync(prefix + "3", [new HashEntry("title", "goodbye world")]);
        _ = await client.HashSetAsync(prefix + "4", [new HashEntry("title", "world hello")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // VERBATIM — no stemming
        FtSearchResult verbatim = await client.FtSearchAsync(idx, "hello",
            new FtSearchOptions { Verbatim = true });
        Assert.Equal(3L, verbatim.TotalResults);

        // SLOP without INORDER — "hello world" with slop 1 matches 2 docs
        FtSearchResult slop = await client.FtSearchAsync(idx, "hello world",
            new FtSearchOptions { Slop = 1 });
        Assert.Equal(2L, slop.TotalResults);

        // SLOP + INORDER — only "hello world" (in order)
        FtSearchResult inOrder = await client.FtSearchAsync(idx, "hello world",
            new FtSearchOptions { InOrder = true, Slop = 1 });
        Assert.Equal(1L, inOrder.TotalResults);

        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_ShardScopeAndConsistency_ReturnsResults(BaseClient client)
    {
        // Just checking it doesn't throw. Result content isn't verified here.
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TagField("tag"), new NumericField("score")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("tag", "test"), new HashEntry("score", "1")]);
        _ = await client.HashSetAsync(prefix + "2", [new HashEntry("tag", "test"), new HashEntry("score", "2")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtSearchResult r1 = await client.FtSearchAsync(idx, "@tag:{test}",
            new FtSearchOptions
            {
                ShardScope = FtSearchShardScope.SomeShards,
                Consistency = FtSearchConsistencyMode.Inconsistent,
            });
        Assert.Equal(2L, r1.TotalResults);

        FtSearchResult r2 = await client.FtSearchAsync(idx, "@tag:{test}",
            new FtSearchOptions
            {
                ShardScope = FtSearchShardScope.AllShards,
                Consistency = FtSearchConsistencyMode.Consistent,
            });
        Assert.Equal(2L, r2.TotalResults);

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.AGGREGATE ─────────────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtAggregate_GroupByCondition_CountsPerGroup(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{bicycles-" + Guid.NewGuid() + "}:";
        string idx = prefix + "idx";

        await client.FtCreateAsync(idx,
            [
                new TextField("model"),
                new NumericField("price"),
                new TagField("condition") { Separator = "," },
            ],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        string[] conditions = ["new", "used", "used", "used", "used", "new", "new", "new", "new", "refurbished"];
        for (int i = 0; i < conditions.Length; i++)
        {
            _ = await client.HashSetAsync(prefix + i,
            [
                new HashEntry("model", "bike" + i),
                new HashEntry("price", (100 + (i * 10)).ToString()),
                new HashEntry("condition", conditions[i]),
            ]);
        }
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtAggregateRow[] result = await client.FtAggregateAsync(idx, "@price:[0 +inf]",
            new FtAggregateOptions
            {
                LoadFields = ["__key", "condition"],
                Clauses =
                [
                    new FtAggregateGroupBy("@condition")
                    {
                        Reducers = [new FtAggregateReducer(FtReducerFunction.Count) { Name = "bicycles" }]
                    }
                ]
            });

        Assert.Equal(3, result.Length);
        var condCounts = result.ToDictionary(
            r => r["condition"].ToString(),
            r => Convert.ToDouble(r["bicycles"]));
        Assert.Equal(5.0, condCounts["new"]);
        Assert.Equal(4.0, condCounts["used"]);
        Assert.Equal(1.0, condCounts["refurbished"]);

        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtAggregate_QueryFlags_VerbatimInOrderDialect(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new NumericField("score"), new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("score", "10"), new HashEntry("title", "hello world")]);
        _ = await client.HashSetAsync(prefix + "2", [new HashEntry("score", "20"), new HashEntry("title", "hello there")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // VERBATIM
        FtAggregateRow[] r1 = await client.FtAggregateAsync(idx, "@score:[1 +inf]",
            new FtAggregateOptions { Verbatim = true });
        Assert.Equal(2, r1.Length);

        // INORDER + SLOP
        FtAggregateRow[] r2 = await client.FtAggregateAsync(idx, "@score:[1 +inf]",
            new FtAggregateOptions { InOrder = true, Slop = 1 });
        Assert.Equal(2, r2.Length);

        // DIALECT
        FtAggregateRow[] r3 = await client.FtAggregateAsync(idx, "@score:[1 +inf]",
            new FtAggregateOptions { Dialect = 2 });
        Assert.Equal(2, r3.Length);

        // LOAD all — fields should be present
        FtAggregateRow[] r4 = await client.FtAggregateAsync(idx, "@score:[20 +inf]",
            new FtAggregateOptions { LoadAll = true });
        _ = Assert.Single(r4);
        Assert.Equal("hello there", r4[0]["title"].ToString());

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.INFO ───────────────────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtInfo_ExistingIndex_ReturnsIndexName(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string idx = Guid.NewGuid().ToString();
        await client.FtCreateAsync(idx,
            [
                new VectorFieldHnsw("$.vec", DistanceMetric.Cosine, 42) { Alias = "VEC" },
                new TextField("$.name") { Alias = "name" },
            ],
            new FtCreateOptions { DataType = IndexDataType.Json, Prefixes = ["123"] });

        Dictionary<string, object> info = await client.FtInfoAsync(idx);
        Assert.Equal(idx, info["index_name"]?.ToString());

        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtInfo_NonExistentIndex_Throws(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtInfoAsync(Guid.NewGuid().ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtInfo_WithLocalScope_ReturnsIndexName(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });
        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "hello world")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        Dictionary<string, object> localInfo = await client.FtInfoAsync(idx,
            new FtInfoOptions { Scope = FtInfoScope.Local });
        Assert.Equal(idx, localInfo["index_name"]?.ToString());
        Assert.NotNull(localInfo["num_docs"]);

        // LOCAL + ALLSHARDS + CONSISTENT
        Dictionary<string, object> withFlags = await client.FtInfoAsync(idx,
            new FtInfoOptions
            {
                Scope = FtInfoScope.Local,
                ShardScope = FtInfoShardScope.AllShards,
                Consistency = FtInfoConsistencyMode.Consistent,
            });
        Assert.Equal(idx, withFlags["index_name"]?.ToString());

        // LOCAL + SOMESHARDS + INCONSISTENT
        Dictionary<string, object> withAltFlags = await client.FtInfoAsync(idx,
            new FtInfoOptions
            {
                Scope = FtInfoScope.Local,
                ShardScope = FtInfoShardScope.SomeShards,
                Consistency = FtInfoConsistencyMode.Inconsistent,
            });
        Assert.Equal(idx, withAltFlags["index_name"]?.ToString());

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.ALIAS* ─────────────────────────────────────────────────────────────

    [Theory(Skip = "FT.ALIAS not supported yet")]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtAlias_AddUpdateDeleteList_WorksCorrectly(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string alias1 = "alias1-" + Guid.NewGuid();
        string alias2 = "alias2-" + Guid.NewGuid();
        string idx = Guid.NewGuid() + "-index";

        await client.FtCreateAsync(idx,
            [new VectorFieldFlat("vec", DistanceMetric.Euclidean, 2)]);

        // AliasAdd
        await client.FtAliasAddAsync(alias1, idx);

        Dictionary<string, string> aliases = await client.FtAliasListAsync();
        Assert.Equal(idx, aliases[alias1]);

        // Duplicate alias → error
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtAliasAddAsync(alias1, idx));

        // AliasUpdate creates alias2
        await client.FtAliasUpdateAsync(alias2, idx);

        aliases = await client.FtAliasListAsync();
        Assert.Equal(idx, aliases[alias1]);
        Assert.Equal(idx, aliases[alias2]);

        // AliasDel
        await client.FtAliasDelAsync(alias2);
        await client.FtAliasDelAsync(alias1);

        // Delete non-existent → error
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtAliasDelAsync(alias2));

        // AliasAdd with non-existent index → error
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtAliasAddAsync(alias1, "nonexistent_index"));

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.CREATE index-level options ─────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_IndexOptions_ScoreLanguageSkipInitialScan(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions
            {
                DataType = IndexDataType.Hash,
                Prefixes = [prefix],
                Score = 1.0,
                Language = "english",
                SkipInitialScan = true,
            });
        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_NoStopWords_IndexesStopWords(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix], NoStopWords = true });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "the quick fox")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtSearchResult r = await client.FtSearchAsync(idx, "the");
        Assert.Equal(1L, r.TotalResults);

        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_FieldOptions_NoStemSortableWeight(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [
                new TextField("title") { NoStem = true, Weight = 1.0, Sortable = true },
                new NumericField("price") { Sortable = true },
                new TagField("tag") { Separator = ",", Sortable = true },
            ],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "hello"), new HashEntry("price", "10"), new HashEntry("tag", "a,b")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // sortable numeric field works with SORTBY
        FtSearchResult r0 = await client.FtSearchAsync(idx, "@price:[1 +inf]",
            new FtSearchOptions { SortBy = new FtSearchSortBy("price", FtSearchSortOrder.Ascending) });
        Assert.Equal(1L, r0.TotalResults);

        // nostem: "hello" matches, "hellos" does not
        FtSearchResult r1 = await client.FtSearchAsync(idx, "hello");
        Assert.Equal(1L, r1.TotalResults);
        FtSearchResult r2 = await client.FtSearchAsync(idx, "hellos");
        Assert.Equal(0L, r2.TotalResults);

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.SEARCH WITHSORTKEYS ────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_WithSortKeys_ReturnsSortKeyPerDoc(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new NumericField("price") { Sortable = true }, new TextField("name")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("price", "10"), new HashEntry("name", "Aardvark")]);
        _ = await client.HashSetAsync(prefix + "2", [new HashEntry("price", "20"), new HashEntry("name", "Mango")]);
        _ = await client.HashSetAsync(prefix + "3", [new HashEntry("price", "30"), new HashEntry("name", "Zebra")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtSearchResult result = await client.FtSearchAsync(idx, "@price:[1 +inf]",
            new FtSearchOptions
            {
                SortBy = new FtSearchSortBy("price", FtSearchSortOrder.Ascending, withSortKeys: true),
            });

        Assert.Equal(3L, result.TotalResults);
        Assert.Equal(3, result.Documents.Count);

        // With WITHSORTKEYS, each doc has a SortKey property
        HashSet<string> foundPrices = [];
        foreach (var doc in result.Documents)
        {
            Assert.NotNull(doc.SortKey);
            foreach (string p in new[] { "10", "20", "30" })
            {
                if (doc.SortKey.Contains(p))
                {
                    _ = foundPrices.Add(p);
                }
            }
        }
        Assert.Contains("10", foundPrices);
        Assert.Contains("20", foundPrices);
        Assert.Contains("30", foundPrices);

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.SEARCH DIALECT ─────────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_Dialect_ReturnsResults(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new VectorFieldFlat("vec", DistanceMetric.Euclidean, 2) { Alias = "VEC" }],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        byte[] vec0 = new byte[8];
        _ = await client.HashSetAsync(prefix + "0", [new HashEntry("vec", System.Text.Encoding.Latin1.GetString(vec0))]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtSearchResult result = await client.FtSearchAsync(idx, "*=>[KNN 1 @VEC $query_vec]",
            new FtSearchOptions
            {
                Dialect = 2,
                Params = [new FtSearchParam("query_vec", System.Text.Encoding.Latin1.GetString(vec0))],
                ReturnFields = [new FtSearchReturnField("vec")],
            });

        Assert.Equal(1L, result.TotalResults);
        _ = Assert.Single(result.Documents);
        Assert.Contains(result.Documents, d => d.Key == prefix + "0");
    }

    // ── FT.CREATE MINSTEMSIZE ─────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_MinStemSize_ShortWordsNotStemmed(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix], MinStemSize = 6 });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "running")]);
        _ = await client.HashSetAsync(prefix + "2", [new HashEntry("title", "plays")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // "running" (7 chars) is stemmed to "run"
        FtSearchResult r1 = await client.FtSearchAsync(idx, "run");
        Assert.Equal(1L, r1.TotalResults);

        // "plays" (5 chars) is NOT stemmed (< 6 chars)
        FtSearchResult r2 = await client.FtSearchAsync(idx, "play");
        Assert.Equal(0L, r2.TotalResults);

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.CREATE STOPWORDS ───────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_StopWords_CustomStopWordsRejected(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix], StopWords = ["fox", "an"] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "the quick fox")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // Non-stop words are searchable
        FtSearchResult r1 = await client.FtSearchAsync(idx, "the");
        Assert.Equal(1L, r1.TotalResults);
        FtSearchResult r2 = await client.FtSearchAsync(idx, "quick");
        Assert.Equal(1L, r2.TotalResults);

        // Custom stop word "fox" should be rejected
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtSearchAsync(idx, "fox"));

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.CREATE NOOFFSETS ───────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_NoOffsets_SlopQueryFails(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix], Offsets = false });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "hello")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // Basic search works
        FtSearchResult r = await client.FtSearchAsync(idx, "hello");
        Assert.Equal(1L, r.TotalResults);

        // SLOP requires offsets — should fail
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtSearchAsync(idx, "hello", new FtSearchOptions { Slop = 1 }));

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.CREATE WITHSUFFIXTRIE / NOSUFFIXTRIE ──────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_WithSuffixTrie_SuffixQueryWorks(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title") { SuffixTrie = true }],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "hello world")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtSearchResult r = await client.FtSearchAsync(idx, "*orld");
        Assert.Equal(1L, r.TotalResults);

        await client.FtDropIndexAsync(idx);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_NoSuffixTrie_SuffixQueryFails(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title") { SuffixTrie = false }],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "hello world")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtSearchAsync(idx, "*orld"));

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.CREATE multiple prefixes ───────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_MultiplePrefixes_ReturnsOk(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        await client.FtCreateAsync(Guid.NewGuid().ToString(),
            [
                new TagField("author_id"),
                new TagField("author_ids"),
                new TextField("title"),
                new TextField("name"),
            ],
            new FtCreateOptions
            {
                DataType = IndexDataType.Hash,
                Prefixes = ["author:details:", "book:details:"],
            });
    }

    // ── FT.CREATE error cases ─────────────────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_NoFields_Throws(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtCreateAsync(Guid.NewGuid().ToString(), []));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtCreate_DuplicateFieldName_Throws(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        _ = await Assert.ThrowsAnyAsync<Exception>(() =>
            client.FtCreateAsync(Guid.NewGuid().ToString(),
                [new TextField("name"), new TextField("name")]));
    }

    // ── FT.AGGREGATE movies (APPLY + GROUPBY + SORTBY) ───────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtAggregate_Movies_ApplyGroupBySortBy(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{movies-" + Guid.NewGuid() + "}:";
        string idx = prefix + "idx";

        await client.FtCreateAsync(idx,
            [
                new TextField("title"),
                new NumericField("release_year"),
                new NumericField("rating"),
                new TagField("genre"),
                new NumericField("votes"),
            ],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        (string title, string year, string genre, string rating, string votes)[] movies =
        [
            ("Star Wars: Episode V - The Empire Strikes Back", "1980", "Action", "8.7", "1127635"),
            ("The Godfather", "1972", "Drama", "9.2", "1563839"),
            ("Heat", "1995", "Thriller", "8.2", "559490"),
            ("Star Wars: Episode VI - Return of the Jedi", "1983", "Action", "8.3", "906260"),
        ];
        for (int i = 0; i < movies.Length; i++)
        {
            var (title, year, genre, rating, votes) = movies[i];
            _ = await client.HashSetAsync(prefix + (11002 + i),
            [
                new HashEntry("title", title),
                new HashEntry("release_year", year),
                new HashEntry("genre", genre),
                new HashEntry("rating", rating),
                new HashEntry("votes", votes),
            ]);
        }
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtAggregateRow[] result = await client.FtAggregateAsync(idx, "@release_year:[0 +inf]",
            new FtAggregateOptions
            {
                LoadFields = ["genre", "rating", "votes"],
                Clauses =
                [
                    new FtAggregateApply("ceil(@rating)", "r_rating"),
                    new FtAggregateGroupBy("@genre")
                    {
                        Reducers =
                        [
                            new FtAggregateReducer(FtReducerFunction.Count) { Name = "nb_of_movies" },
                            new FtAggregateReducer(FtReducerFunction.Sum) { Args = ["@votes"], Name = "nb_of_votes" },
                            new FtAggregateReducer(FtReducerFunction.Avg) { Args = ["@r_rating"], Name = "avg_rating" },
                        ]
                    },
                    new FtAggregateSortBy([
                        new FtAggregateSortProperty("@avg_rating", SortOrder.Descending),
                        new FtAggregateSortProperty("@nb_of_votes", SortOrder.Descending)]),
                ]
            });

        Assert.Equal(3, result.Length);

        var genreMap = result.ToDictionary(
            r => r["genre"].ToString(),
            r => r);

        Assert.Equal(1.0, Convert.ToDouble(genreMap["Drama"]["nb_of_movies"]));
        Assert.Equal(1563840.0, Convert.ToDouble(genreMap["Drama"]["nb_of_votes"]));
        Assert.Equal(10.0, Convert.ToDouble(genreMap["Drama"]["avg_rating"]));
        Assert.Equal(2.0, Convert.ToDouble(genreMap["Action"]["nb_of_movies"]));
        Assert.Equal(2033900.0, Convert.ToDouble(genreMap["Action"]["nb_of_votes"]));
        Assert.Equal(9.0, Convert.ToDouble(genreMap["Action"]["avg_rating"]));
        Assert.Equal(1.0, Convert.ToDouble(genreMap["Thriller"]["nb_of_movies"]));
        Assert.Equal(559490.0, Convert.ToDouble(genreMap["Thriller"]["nb_of_votes"]));
        Assert.Equal(9.0, Convert.ToDouble(genreMap["Thriller"]["avg_rating"]));

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.INFO PRIMARY / CLUSTER scope ───────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtInfo_PrimaryAndClusterScope_AcceptedOrRejected(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new TextField("title")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });
        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("title", "hello world")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // PRIMARY scope — works if coordinator is enabled, otherwise rejected with specific error
        try
        {
            Dictionary<string, object> primaryInfo = await client.FtInfoAsync(idx,
                new FtInfoOptions { Scope = FtInfoScope.Primary });
            Assert.Equal(idx, primaryInfo["index_name"]?.ToString());
        }
        catch (Exception ex)
        {
            Assert.Contains("PRIMARY option is not valid", ex.Message);
        }

        // CLUSTER scope — works if coordinator is enabled, otherwise rejected with specific error
        try
        {
            Dictionary<string, object> clusterInfo = await client.FtInfoAsync(idx,
                new FtInfoOptions { Scope = FtInfoScope.Cluster });
            Assert.Equal(idx, clusterInfo["index_name"]?.ToString());
        }
        catch (Exception ex)
        {
            Assert.Contains("CLUSTER option is not valid", ex.Message);
        }

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.SEARCH WITHSORTKEYS + NOCONTENT ────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtSearch_WithSortKeysAndNoContent_NoContentTakesPrecedence(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{" + Guid.NewGuid() + "}:";
        string idx = prefix + "index";

        await client.FtCreateAsync(idx,
            [new NumericField("price") { Sortable = true }, new TextField("name")],
            new FtCreateOptions { DataType = IndexDataType.Hash, Prefixes = [prefix] });

        _ = await client.HashSetAsync(prefix + "1", [new HashEntry("price", "10"), new HashEntry("name", "Aardvark")]);
        _ = await client.HashSetAsync(prefix + "2", [new HashEntry("price", "20"), new HashEntry("name", "Mango")]);
        _ = await client.HashSetAsync(prefix + "3", [new HashEntry("price", "30"), new HashEntry("name", "Zebra")]);
        await Task.Delay(1000, TestContext.Current.CancellationToken);

        // When NOCONTENT is set, the server ignores WITHSORTKEYS entirely —
        // the response contains only key names with no sort keys or field data.
        // The client silently strips WITHSORTKEYS to match server behavior.
        FtSearchResult result = await client.FtSearchAsync(idx, "@price:[1 +inf]",
            new FtSearchOptions
            {
                SortBy = new FtSearchSortBy("price", FtSearchSortOrder.Ascending, withSortKeys: true),
                NoContent = true,
            });

        Assert.Equal(3L, result.TotalResults);
        Assert.Equal(3, result.Documents.Count);

        // NOCONTENT takes precedence: empty fields, no sort keys
        foreach (var doc in result.Documents)
        {
            Assert.Empty(doc.Fields);
            Assert.Null(doc.SortKey);
        }

        // All three keys should be present
        var keys = result.Documents.Select(d => d.Key).ToHashSet();
        Assert.Contains(prefix + "1", keys);
        Assert.Contains(prefix + "2", keys);
        Assert.Contains(prefix + "3", keys);

        await client.FtDropIndexAsync(idx);
    }

    // ── FT.AGGREGATE JSON index (bicycles) ────────────────────────────────────

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FtAggregate_JsonBicycles_GroupByCondition(BaseClient client)
    {
        await SkipIfModuleNotAvailable(client);
        string prefix = "{json-bicycles-" + Guid.NewGuid() + "}:";
        string idx = prefix + "idx";

        await client.FtCreateAsync(idx,
            [
                new TextField("$.model") { Alias = "model" },
                new NumericField("$.price") { Alias = "price" },
                new TagField("$.condition") { Alias = "condition", Separator = "," },
            ],
            new FtCreateOptions { DataType = IndexDataType.Json, Prefixes = [prefix] });

        // JSON.SET via CustomCommand — replace once JSON module commands are implemented
        string[] bicycles =
        [
            """{"brand":"Velorim","model":"Jigger","price":270,"condition":"new"}""",
            """{"brand":"Bicyk","model":"Hillcraft","price":1200,"condition":"used"}""",
            """{"brand":"Nord","model":"Chook air 5","price":815,"condition":"used"}""",
            """{"brand":"Eva","model":"Eva 291","price":3400,"condition":"used"}""",
            """{"brand":"Noka Bikes","model":"Kahuna","price":3200,"condition":"used"}""",
            """{"brand":"Breakout","model":"XBN 2.1 Alloy","price":810,"condition":"new"}""",
            """{"brand":"ScramBikes","model":"WattBike","price":2300,"condition":"new"}""",
            """{"brand":"Peaknetic","model":"Secto","price":430,"condition":"new"}""",
            """{"brand":"nHill","model":"Summit","price":1200,"condition":"new"}""",
            """{"model":"ThrillCycle","brand":"BikeShind","price":815,"condition":"refurbished"}""",
        ];

        try
        {
            for (int i = 0; i < bicycles.Length; i++)
            {
                // TODO: replace once native JSON commands are implemented
                if (client is GlideClient standaloneClient)
                {
                    _ = await standaloneClient.CustomCommand(["JSON.SET", prefix + i, ".", bicycles[i]]);
                }
                else if (client is GlideClusterClient clusterClient)
                {
                    _ = await clusterClient.CustomCommand(["JSON.SET", prefix + i, ".", bicycles[i]]);
                }
            }
        }
        catch (Exception ex) when (
            ex.Message.Contains("unknown command", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("ERR unknown", StringComparison.OrdinalIgnoreCase))
        {
            // JSON module not available — skip this test
            try { await client.FtDropIndexAsync(idx); } catch { /* best effort cleanup */ }
            Assert.Skip("JSON module not available on this server.");
            return;
        }

        await Task.Delay(1000, TestContext.Current.CancellationToken);

        FtAggregateRow[] result = await client.FtAggregateAsync(idx, "@price:[0 +inf]",
            new FtAggregateOptions
            {
                LoadFields = ["__key", "condition"],
                Clauses =
                [
                    new FtAggregateGroupBy("@condition")
                    {
                        Reducers = [new FtAggregateReducer(FtReducerFunction.Count) { Name = "bicycles" }]
                    }
                ]
            });

        Assert.Equal(3, result.Length);
        var condCounts = result.ToDictionary(
            r => r["$.condition"].ToString(),
            r => Convert.ToDouble(r["bicycles"]));
        Assert.Equal(5.0, condCounts["new"]);
        Assert.Equal(4.0, condCounts["used"]);
        Assert.Equal(1.0, condCounts["refurbished"]);

        await client.FtDropIndexAsync(idx);
    }
}
