// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.ServerModules;

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
    public async Task SearchAsync_MatchAll_ReturnsAllDocuments(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, string[] keys) = await FtUtils.CreateSearchIndexAsync(client);

        // Sort by price ascending for deterministic order: 10, 25, 50
        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@price:[-inf +inf]",
            new Ft.SearchOptions
            {
                SortBy = new Ft.SearchSortBy { Field = "price", Order = SortOrder.Ascending },
            });

        Assert.Equal(3, result.TotalResults);
        Assert.Equal(3, result.Documents.Length);

        var expected = new[]
        {
            new {
                Key = (ValkeyKey)keys[0],
                Fields = (IDictionary<ValkeyValue, ValkeyValue>)new Dictionary<ValkeyValue, ValkeyValue> {
                    ["title"] = "Alpha Widget",
                    ["price"] = "10",
                    ["category"] = "electronics" } },
            new {
                Key = (ValkeyKey)keys[1],
                Fields = (IDictionary<ValkeyValue, ValkeyValue>)new Dictionary<ValkeyValue, ValkeyValue> {
                    ["title"] = "Beta Gadget",
                    ["price"] = "25",
                    ["category"] = "electronics" } },
            new {
                Key = (ValkeyKey)keys[2],
                Fields = (IDictionary<ValkeyValue, ValkeyValue>)new Dictionary<ValkeyValue, ValkeyValue> {
                    ["title"] = "Gamma Tool",
                    ["price"] = "50",
                    ["category"] = "hardware" } },
        };
        Assert.Equivalent(expected, result.Documents);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_TextQuery_ReturnsMatchingDocuments(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateSearchIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@title:Alpha");
        Assert.Equal(1, result.TotalResults);
        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>
            {
                ["title"] = "Alpha Widget",
                ["price"] = "10",
                ["category"] = "electronics"
            },
            result.Documents[0].Fields);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_WithLimit_ReturnsPaginatedResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateSearchIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@price:[-inf +inf]",
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
        (string index, _, _) = await FtUtils.CreateSearchIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@price:[-inf +inf]",
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
        (string index, _, _) = await FtUtils.CreateSearchIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@price:[-inf +inf]",
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
    public async Task SearchAsync_WithNumericFilter_ReturnsFilteredResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateSearchIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@price:[20 +inf]");

        Assert.Equal(2, result.TotalResults);
        Assert.Equal(2, result.Documents.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SearchAsync_SearchDocumentFieldAccess_FieldsAreAccessible(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, string[] keys) = await FtUtils.CreateSearchIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@title:Alpha");
        Assert.Equal(1, result.TotalResults);

        Ft.SearchDocument doc = Assert.Single(result.Documents);
        Assert.Equal(keys[0], doc.Key);
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
        (string index, _, _) = await FtUtils.CreateSearchIndexAsync(client);

        Ft.SearchResult result = await Ft.SearchAsync(client, index, "@price:[-inf +inf]",
            new Ft.SearchOptions
            {
                Limit = new Ft.SearchLimit { Offset = 0, Count = 1 },
            });

        Assert.Equal(3, result.TotalResults);
        _ = Assert.Single(result.Documents);
    }

    #endregion
}
