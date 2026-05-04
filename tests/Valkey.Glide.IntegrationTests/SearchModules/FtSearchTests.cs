// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Integration tests for <c>FT.SEARCH</c>:
/// <list type="bullet">
/// <item><see cref="Ft.SearchAsync(BaseClient, ValkeyKey, ValkeyValue)"/></item>
/// <item><see cref="Ft.SearchAsync(BaseClient, ValkeyKey, ValkeyValue, Ft.SearchOptions)"/></item>
/// </list>
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">Valkey commands – FT.SEARCH</seealso>
public class FtSearchTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_WildcardQuery_ReturnsAllDocuments(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, string prefix, _) = await CreatePopulatedIndexAsync(client);

        // Sort by price ascending for deterministic order: 10, 25, 50
        Ft.SearchResult result = await Ft.SearchAsync(client, index, "*",
            new Ft.SearchOptions
            {
                SortBy = new Ft.SearchSortBy { Field = "price", Order = SortOrder.Ascending },
            });

        Assert.Equal(3, result.TotalResults);
        Assert.Equal(3, result.Documents.Length);

        Assert.Equal($"{prefix}1", result.Documents[0].Key);
        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>
            {
                ["title"] = "Alpha Widget",
                ["price"] = "10",
                ["category"] = "electronics"
            },
            result.Documents[0].Fields);

        Assert.Equal($"{prefix}2", result.Documents[1].Key);
        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>
            {
                ["title"] = "Beta Gadget",
                ["price"] = "25",
                ["category"] = "electronics"
            },
            result.Documents[1].Fields);

        Assert.Equal($"{prefix}3", result.Documents[2].Key);
        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>
            {
                ["title"] = "Gamma Tool",
                ["price"] = "50",
                ["category"] = "hardware"
            },
            result.Documents[2].Fields);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_TextQuery_ReturnsMatchingDocuments(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await CreatePopulatedIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@title:Alpha");
        Assert.Equal(1, result.TotalResults);
        Assert.Equal(1, result.Documents.Length);

        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue> {
                ["title"] = "Alpha Widget",
                ["price"] = "10",
                ["category"] = "electronics" },
            result.Documents[0].Fields);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_WithLimit_ReturnsPaginatedResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await CreatePopulatedIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "*",
            new Ft.SearchOptions
            {
                Limit = new Ft.SearchLimit { Offset = 0, Count = 2 },
            });

        // TotalResults reflects the full match count, not the page size
        Assert.Equal(3, result.TotalResults);
        Assert.Equal(2, result.Documents.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_WithSortBy_ReturnsSortedResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await CreatePopulatedIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "*",
            new Ft.SearchOptions
            {
                SortBy = new Ft.SearchSortBy
                {
                    Field = "price",
                    Order = SortOrder.Ascending,
                },
            });

        Assert.Equal(3, result.TotalResults);
        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>[]
            {
                new() { ["title"] = "Alpha Widget", ["price"] = "10", ["category"] = "electronics" },
                new() { ["title"] = "Beta Gadget", ["price"] = "25", ["category"] = "electronics" },
                new() { ["title"] = "Gamma Tool", ["price"] = "50", ["category"] = "hardware" },
            },
            result.Documents.Select(d => d.Fields));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_WithReturnFields_ReturnsOnlySpecifiedFields(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await CreatePopulatedIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "*",
            new Ft.SearchOptions
            {
                Return = ["title", "price"],
            });

        Assert.Equal(3, result.TotalResults);

        foreach (Ft.SearchDocument doc in result.Documents)
        {
            Assert.Equivalent(
                new ValkeyValue[] { "title", "price" },
                doc.Fields.Keys);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_WithParams_ReturnsParameterizedResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await CreatePopulatedIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@price:[$minPrice +inf]",
            new Ft.SearchOptions
            {
                Params = new Dictionary<ValkeyValue, ValkeyValue>
                {
                    { "minPrice", "20" },
                },
            });

        Assert.Equal(2, result.TotalResults);
        Assert.Equal(2, result.Documents.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_SearchDocumentFieldAccess_FieldsAreAccessible(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, string prefix, _) = await CreatePopulatedIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@title:Alpha");
        Assert.Equal(1, result.TotalResults);
        Assert.Equal(1, result.Documents.Length);

        Ft.SearchDocument doc = result.Documents[0];
        Assert.Equal($"{prefix}1", doc.Key);
        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>
            {
                ["title"] = "Alpha Widget",
                ["price"] = "10",
                ["category"] = "electronics"
            },
            doc.Fields);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_TotalResults_ReflectsFullMatchCount(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await CreatePopulatedIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "*",
            new Ft.SearchOptions
            {
                Limit = new Ft.SearchLimit { Offset = 0, Count = 1 },
            });

        Assert.Equal(3, result.TotalResults);
        _ = Assert.Single(result.Documents);
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Creates a text+numeric+tag index with a unique prefix, populates hash documents,
    /// waits for indexing, and returns the index name and document keys for cleanup.
    /// </summary>
    private static async Task<(string IndexName, string Prefix, string[] DocKeys)> CreatePopulatedIndexAsync(
        BaseClient client)
    {
        var index = Guid.NewGuid().ToString();
        var prefix = $"{{{index}}}:";

        await Ft.CreateAsync(client, index,
        [
            new Ft.CreateTextField("title"),
            new Ft.CreateNumericField("price"),
            new Ft.CreateTagField("category"),
        ],
        new Ft.CreateOptions
        {
            DataType = Ft.DataType.Hash,
            Prefixes = [prefix],
        });

        string[] keys =
        [
            $"{prefix}1",
            $"{prefix}2",
            $"{prefix}3",
        ];

        _ = await client.HashSetAsync(keys[0],
        [
            new("title", "Alpha Widget"),
            new("price", "10"),
            new("category", "electronics"),
        ]);

        _ = await client.HashSetAsync(keys[1],
        [
            new("title", "Beta Gadget"),
            new("price", "25"),
            new("category", "electronics"),
        ]);

        _ = await client.HashSetAsync(keys[2],
        [
            new("title", "Gamma Tool"),
            new("price", "50"),
            new("category", "hardware"),
        ]);

        // Wait for indexing
        await Task.Delay(1000);

        return (index, prefix, keys);
    }

    #endregion
}
