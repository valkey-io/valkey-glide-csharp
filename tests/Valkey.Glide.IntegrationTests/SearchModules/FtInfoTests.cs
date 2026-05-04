// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

using static Valkey.Glide.Errors;

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
        var prefix = $"{{{index}}}:";

        await Ft.CreateAsync(client, index,
            [
                new Ft.CreateTextField("title"),
                new Ft.CreateTagField("category"),
                new Ft.CreateNumericField("price"),
            ],
            new Ft.CreateOptions
            {
                Prefixes = [prefix],
                StopWords = ["the", "a"],
            });

        _ = await client.HashSetAsync($"{prefix}1",
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
        Assert.Equal(prefix, Assert.Single(info.Prefixes));

        // Attributes
        Assert.Equal(3, info.Attributes.Length);

        // Document and term stats
        Assert.Equal(1L, info.NumDocs);
        Assert.Equal(2L, info.NumRecords);
        Assert.Equal(2L, info.TotalTermOccurrences);
        Assert.Equal(2L, info.NumTerms);
        Assert.Equal(0L, info.HashIndexingFailures);

        _ = Assert.IsType<bool>(info.BackfillInProgress);
        Assert.InRange(info.BackfillCompletePercent, 0.0, 1.0);
        Assert.True(info.MutationQueueSize >= 0);
        Assert.True(info.RecentMutationsQueueDelay >= 0.0);
        _ = Assert.IsType<Ft.InfoState>(info.State);

        Assert.Equal((ValkeyValue)",.<>{}[]\"':;!@#$%^&*()-+=~/\\|?", info.Punctuation);
        Assert.Equivalent(new ValkeyValue[] { "the", "a" }, info.StopWords);
        Assert.True(info.WithOffsets);
        Assert.Equal(4L, info.MinStemSize);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_TextFieldAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateTextField("$.title", "title") { NoStem = true, WithSuffixTrie = false };
        await Ft.CreateAsync(client, index, field);

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.IsType<Ft.InfoTextField>(Assert.Single(info.Attributes));

        Assert.Equal("$.title", attr.Identifier);
        Assert.Equal("title", attr.Attribute);

        Assert.True(attr.UserIndexedMemory >= 0);
        Assert.True(attr.NoStem);
        Assert.False(attr.WithSuffixTrie);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_TagFieldAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateTagField("category") { Separator = '|', CaseSensitive = true };
        await Ft.CreateAsync(client, index, field);

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.IsType<Ft.InfoTagField>(Assert.Single(info.Attributes));

        Assert.Equal("category", attr.Identifier);
        Assert.Equal("category", attr.Attribute);
        Assert.True(attr.UserIndexedMemory >= 0);

        Assert.Equal('|', attr.Separator);
        Assert.True(attr.CaseSensitive);
        Assert.Equal(0L, attr.Size);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_NumericFieldAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateNumericField("price");
        await Ft.CreateAsync(client, index, field);

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.IsType<Ft.InfoNumericField>(Assert.Single(info.Attributes));

        Assert.Equal("price", attr.Identifier);
        Assert.Equal("price", attr.Attribute);
        Assert.True(attr.UserIndexedMemory >= 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_VectorFieldFlatAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateVectorFieldFlat
        {
            Identifier = "embedding",
            Dimensions = 128,
            DistanceMetric = Ft.DistanceMetric.Cosine,
        };
        await Ft.CreateAsync(client, index, field);

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.IsType<Ft.InfoVectorFieldFlat>(Assert.Single(info.Attributes));

        Assert.Equal("embedding", attr.Identifier);
        Assert.Equal("embedding", attr.Attribute);
        Assert.True(attr.UserIndexedMemory >= 0);

        Assert.Equal(128L, attr.Dimensions);
        Assert.Equal(Ft.DistanceMetric.Cosine, attr.DistanceMetric);
        Assert.True(attr.Capacity >= 0);
        Assert.Equal(0L, attr.Size);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_VectorFieldHnswAttributes(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateVectorFieldHnsw
        {
            Identifier = "vec",
            Dimensions = 64,
            DistanceMetric = Ft.DistanceMetric.Euclidean,
            NumberOfEdges = 16,
            VectorsExaminedOnConstruction = 200,
            VectorsExaminedOnRuntime = 10,
        };
        await Ft.CreateAsync(client, index, field);

        var info = await Ft.InfoLocalAsync(client, index);
        var attr = Assert.IsType<Ft.InfoVectorFieldHnsw>(Assert.Single(info.Attributes));

        Assert.Equal("vec", attr.Identifier);
        Assert.Equal("vec", attr.Attribute);
        Assert.True(attr.UserIndexedMemory >= 0);

        Assert.Equal(64L, attr.Dimensions);
        Assert.Equal(Ft.DistanceMetric.Euclidean, attr.DistanceMetric);
        Assert.True(attr.Capacity >= 0);
        Assert.Equal(0L, attr.Size);
        Assert.Equal(16L, attr.M);
        Assert.Equal(200L, attr.EfConstruction);
        Assert.Equal(10L, attr.EfRuntime);
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
        await SkipUtils.IfJsonModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateTextField("$.title", "title");
        var options = new Ft.CreateOptions { DataType = Ft.DataType.Json };
        await Ft.CreateAsync(client, index, field, options);

        var info = await Ft.InfoLocalAsync(client, index);
        Assert.Equal(Ft.DataType.Json, info.KeyType);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_NoStopWords(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateTextField("title");
        var options = new Ft.CreateOptions { StopWords = Ft.CreateOptions.NoStopWords };
        await Ft.CreateAsync(client, index, field, options);

        var info = await Ft.InfoLocalAsync(client, index);
        Assert.Empty(info.StopWords);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_NoOffsets(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateTextField("title");
        var options = new Ft.CreateOptions { NoOffsets = true };
        await Ft.CreateAsync(client, index, field, options);

        var info = await Ft.InfoLocalAsync(client, index);
        Assert.False(info.WithOffsets);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoLocalAsync_NonExistentIndex_Throws(BaseClient client)
        => await Assert.ThrowsAsync<RequestException>(()
            => Ft.InfoLocalAsync(client, Guid.NewGuid().ToString()));

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
        => await Assert.ThrowsAsync<RequestException>(()
            => Ft.InfoClusterAsync(client, Guid.NewGuid().ToString()));

    #endregion

    #region InfoPrimaryAsync Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoPrimaryAsync_ReturnsPrimaryResult(GlideClusterClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var prefix = $"{{{index}}}:";

        var field = new Ft.CreateTextField("title");
        var options = new Ft.CreateOptions { Prefixes = [prefix] };
        await Ft.CreateAsync(client, index, field, options);

        _ = await client.HashSetAsync($"{prefix}1", [new("title", "hello world")]);
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
        => await Assert.ThrowsAsync<RequestException>(()
            => Ft.InfoPrimaryAsync(client, Guid.NewGuid().ToString()));

    #endregion
}
