// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Integration tests for <c>FT.INFO</c>:
/// <list type="bullet">
/// <item><see cref="Ft.InfoLocalAsync(BaseClient, ValkeyKey)"/></item>
/// <item><see cref="Ft.InfoLocalAsync(BaseClient, ValkeyKey, Ft.InfoOptions)"/></item>
/// <item><see cref="Ft.InfoClusterAsync(GlideClusterClient, ValkeyKey)"/></item>
/// <item><see cref="Ft.InfoClusterAsync(GlideClusterClient, ValkeyKey, Ft.InfoOptions)"/></item>
/// <item><see cref="Ft.InfoPrimaryAsync(GlideClusterClient, ValkeyKey)"/></item>
/// <item><see cref="Ft.InfoPrimaryAsync(GlideClusterClient, ValkeyKey, Ft.InfoOptions)"/></item>
/// </list>
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
public class FtInfoTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region InfoLocalAsync Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_AllProperties(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index,
            [
                new Ft.CreateTextField("title"),
                new Ft.CreateTagField("category"),
                new Ft.CreateNumericField("price"),
            ],
            new Ft.CreateOptions
            {
                Prefixes = ["doc:"],
                StopWords = ["the", "a"],
            });

        // Add a document so stats are non-trivial
        _ = await client.HashSetAsync("doc:1",
            [new("title", "hello world"),
            new("category", "test"),
            new("price", "42")]);

        // Allow indexing to complete
        await Task.Delay(500, TestContext.Current.CancellationToken);

        var info = await Ft.InfoLocalAsync(client, index);

        // InfoResult base
        Assert.Equal(index, info.IndexName);

        // Index definition
        Assert.Equal(Ft.DataType.Hash, info.KeyType);
        Assert.Equal("doc:", Assert.Single(info.Prefixes));

        // Attributes
        Assert.Equal(3, info.Attributes.Length);

        // Document and term stats (1 doc with "hello world" in title, "test" in category, "42" in price)
        Assert.Equal(1L, info.NumDocs);
        Assert.Equal(2L, info.NumRecords);           // "hello" + "world" in the TEXT field
        Assert.Equal(2L, info.TotalTermOccurrences); // one occurrence of each
        Assert.Equal(2L, info.NumTerms);             // two distinct terms
        Assert.Equal(0L, info.HashIndexingFailures);

        // Backfill state
        _ = Assert.IsType<bool>(info.BackfillInProgress);
        Assert.InRange(info.BackfillCompletePercent, 0.0, 1.0);
        Assert.True(info.MutationQueueSize >= 0);
        Assert.True(info.RecentMutationsQueueDelay >= 0.0);
        _ = Assert.IsType<Ft.InfoState>(info.State);

        // Tokenization config — default punctuation from Valkey
        Assert.Equal((ValkeyValue)",.<>{}[]\"':;!@#$%^&*()-+=~/\\|?", info.Punctuation);

        // Stop words
        Assert.Equivalent(new ValkeyValue[] { "the", "a" }, info.StopWords);

        // Offsets and stemming (server defaults).
        Assert.True(info.WithOffsets);
        Assert.Equal(4L, info.MinStemSize);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_TextFieldAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index,
            new Ft.CreateTextField("$.title", "title") { NoStem = true, WithSuffixTrie = false });

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.Single(info.Attributes);
        var textAttr = Assert.IsType<Ft.InfoTextField>(attr);

        // InfoField base properties
        Assert.Equal((ValkeyValue)"$.title", textAttr.Identifier);
        Assert.Equal((ValkeyValue)"title", textAttr.Attribute);
        Assert.True(textAttr.UserIndexedMemory >= 0);

        // TEXT-specific properties
        Assert.True(textAttr.NoStem);
        Assert.False(textAttr.WithSuffixTrie);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_TagFieldAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index,
            new Ft.CreateTagField("category") { Separator = '|', CaseSensitive = true });

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.Single(info.Attributes);
        var tagAttr = Assert.IsType<Ft.InfoTagField>(attr);

        // InfoField base properties
        Assert.Equal((ValkeyValue)"category", tagAttr.Identifier);
        Assert.Equal((ValkeyValue)"category", tagAttr.Attribute);
        Assert.True(tagAttr.UserIndexedMemory >= 0);

        // TAG-specific properties
        Assert.Equal('|', tagAttr.Separator);
        Assert.True(tagAttr.CaseSensitive);
        Assert.True(tagAttr.Size >= 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_NumericFieldAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateNumericField("price"));

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.Single(info.Attributes);
        var numAttr = Assert.IsType<Ft.InfoNumericField>(attr);

        // InfoField base properties
        Assert.Equal((ValkeyValue)"price", numAttr.Identifier);
        Assert.Equal((ValkeyValue)"price", numAttr.Attribute);
        Assert.True(numAttr.UserIndexedMemory >= 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_VectorFieldFlatAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateVectorFieldFlat
        {
            Identifier = "embedding",
            Dimensions = 128,
            DistanceMetric = Ft.DistanceMetric.Cosine,
        });

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.Single(info.Attributes);
        var flatAttr = Assert.IsType<Ft.InfoVectorFieldFlat>(attr);

        // InfoField base properties
        Assert.Equal((ValkeyValue)"embedding", flatAttr.Identifier);
        Assert.Equal((ValkeyValue)"embedding", flatAttr.Attribute);
        Assert.True(flatAttr.UserIndexedMemory >= 0);

        // InfoVectorField properties
        Assert.Equal(128L, flatAttr.Dimensions);
        Assert.Equal(Ft.DistanceMetric.Cosine, flatAttr.DistanceMetric);
        Assert.True(flatAttr.Capacity >= 0);
        Assert.True(flatAttr.Size >= 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_VectorFieldHnswAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateVectorFieldHnsw
        {
            Identifier = "vec",
            Dimensions = 64,
            DistanceMetric = Ft.DistanceMetric.Euclidean,
            NumberOfEdges = 16,
            VectorsExaminedOnConstruction = 200,
            VectorsExaminedOnRuntime = 10,
        });

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.Single(info.Attributes);
        var hnswAttr = Assert.IsType<Ft.InfoVectorFieldHnsw>(attr);

        // InfoField base properties
        Assert.Equal((ValkeyValue)"vec", hnswAttr.Identifier);
        Assert.Equal((ValkeyValue)"vec", hnswAttr.Attribute);
        Assert.True(hnswAttr.UserIndexedMemory >= 0);

        // InfoVectorField properties
        Assert.Equal(64L, hnswAttr.Dimensions);
        Assert.Equal(Ft.DistanceMetric.Euclidean, hnswAttr.DistanceMetric);
        Assert.True(hnswAttr.Capacity >= 0);
        Assert.True(hnswAttr.Size >= 0);

        // HNSW-specific properties
        Assert.Equal(16L, hnswAttr.M);
        Assert.Equal(200L, hnswAttr.EfConstruction);
        Assert.Equal(10L, hnswAttr.EfRuntime);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_MultipleFieldTypes_ReturnsCorrectSubclasses(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        Ft.CreateField[] schema =
        [
            new Ft.CreateTextField("title"),
            new Ft.CreateTagField("category"),
            new Ft.CreateNumericField("price"),
            new Ft.CreateVectorFieldFlat
            {
                Identifier = "vec",
                Dimensions = 32,
                DistanceMetric = Ft.DistanceMetric.InnerProduct,
            },
        ];
        await Ft.CreateAsync(client, index, schema);

        var info = await Ft.InfoLocalAsync(client, index);
        Assert.Equal(4, info.Attributes.Length);
        Assert.Contains(info.Attributes, a => a is Ft.InfoTextField);
        Assert.Contains(info.Attributes, a => a is Ft.InfoTagField);
        Assert.Contains(info.Attributes, a => a is Ft.InfoNumericField);
        Assert.Contains(info.Attributes, a => a is Ft.InfoVectorFieldFlat);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_JsonIndex_ReturnsJsonKeyType(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateTextField("$.title", "title"),
            new Ft.CreateOptions { DataType = Ft.DataType.Json });

        var info = await Ft.InfoLocalAsync(client, index);
        Assert.Equal(Ft.DataType.Json, info.KeyType);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_NoStopWords(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"),
            new Ft.CreateOptions { StopWords = Ft.CreateOptions.NoStopWords });

        var info = await Ft.InfoLocalAsync(client, index);
        Assert.Empty(info.StopWords);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_NoOffsets(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"),
            new Ft.CreateOptions { NoOffsets = true });

        var info = await Ft.InfoLocalAsync(client, index);
        Assert.False(info.WithOffsets);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_NonExistentIndex_Throws(BaseClient client)
        => await Assert.ThrowsAsync<Exception>(() => Ft.InfoLocalAsync(client, Guid.NewGuid().ToString()));

    #endregion

    #region InfoClusterAsync Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoClusterAsync_ReturnsClusterResult(GlideClusterClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"));

        var info = await Ft.InfoClusterAsync(client, index);
        Assert.Equal(index, info.IndexName);
        _ = Assert.IsType<bool>(info.BackfillInProgress);
        Assert.InRange(info.BackfillCompletePercentMin, 0.0, 1.0);
        Assert.InRange(info.BackfillCompletePercentMax, 0.0, 1.0);
        Assert.True(info.BackfillCompletePercentMax >= info.BackfillCompletePercentMin);
        _ = Assert.IsType<Ft.InfoState>(info.State);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoClusterAsync_WithOptions(GlideClusterClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"));

        var info = await Ft.InfoClusterAsync(client, index, new Ft.InfoOptions { SomeShards = true });
        Assert.Equal(index, info.IndexName);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoClusterAsync_NonExistentIndex_Throws(GlideClusterClient client)
        => await Assert.ThrowsAsync<Exception>(() => Ft.InfoClusterAsync(client, Guid.NewGuid().ToString()));

    #endregion

    #region InfoPrimaryAsync Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoPrimaryAsync_ReturnsPrimaryResult(GlideClusterClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"),
            new Ft.CreateOptions { Prefixes = ["pdoc:"] });

        // Add a document so stats are non-trivial
        _ = await client.HashSetAsync("pdoc:1", [new("title", "hello world")]);
        await Task.Delay(500, TestContext.Current.CancellationToken);

        var info = await Ft.InfoPrimaryAsync(client, index);
        Assert.Equal(index, info.IndexName);
        Assert.Equal(1L, info.NumDocs);
        Assert.Equal(2L, info.NumRecords); // "hello" + "world"
        Assert.Equal(0L, info.HashIndexingFailures);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoPrimaryAsync_WithOptions(GlideClusterClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"));

        var info = await Ft.InfoPrimaryAsync(client, index, new Ft.InfoOptions { SomeShards = true });
        Assert.Equal(index, info.IndexName);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoPrimaryAsync_NonExistentIndex_Throws(GlideClusterClient client)
        => await Assert.ThrowsAsync<Exception>(() => Ft.InfoPrimaryAsync(client, Guid.NewGuid().ToString()));

    #endregion
}
