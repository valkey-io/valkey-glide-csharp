// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.ServerModules;

/// <summary>
/// Integration tests for <c>FT.AGGREGATE</c>:
/// <list type="bullet">
/// <item><see cref="Ft.AggregateAsync(BaseClient, ValkeyKey, ValkeyValue)"/></item>
/// <item><see cref="Ft.AggregateAsync(BaseClient, ValkeyKey, ValkeyValue, Ft.AggregateOptions)"/></item>
/// </list>
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
public class FtAggregateTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AggregateAsync_WildcardQuery_ReturnsRows(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateAggregateIndexAsync(client);

        Assert.Equal(5, (await Ft.AggregateAsync(client, index, "*")).Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AggregateAsync_GroupByWithCountReducer_ReturnsGroupedResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateAggregateIndexAsync(client);

        var rows = await Ft.AggregateAsync(client, index, "*",
            new Ft.AggregateOptions
            {
                LoadFields = ["@category"],
                Clauses =
                [
                    new Ft.AggregateGroupBy
                    {
                        Fields = ["@category"],
                        Reducers =
                        [
                            new Ft.AggregateReducer
                            {
                                Function = Ft.ReducerFunction.Count,
                                Name = "count",
                            },
                        ],
                    },
                    new Ft.AggregateSortBy
                    {
                        Expressions =
                        [
                            new Ft.AggregateSortExpression
                            {
                                Expression = "@category",
                                Order = SortOrder.Ascending,
                            },
                        ],
                    },
                ],
            });

        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>[]
            {
                new() { ["category"] = "electronics", ["count"] = "3" },
                new() { ["category"] = "hardware", ["count"] = "2" },
            },
            rows);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AggregateAsync_SortByPrice_ReturnsSortedResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateAggregateIndexAsync(client);

        var rows = await Ft.AggregateAsync(client, index, "*",
            new Ft.AggregateOptions
            {
                LoadFields = ["@price"],
                Clauses =
                [
                    new Ft.AggregateSortBy
                    {
                        Expressions =
                        [
                            new Ft.AggregateSortExpression
                            {
                                Expression = "@price",
                                Order = SortOrder.Ascending,
                            },
                        ],
                    },
                ],
            });

        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>[]
            {
                new() { ["price"] = "10" },
                new() { ["price"] = "15" },
                new() { ["price"] = "25" },
                new() { ["price"] = "30" },
                new() { ["price"] = "50" },
            },
            rows);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AggregateAsync_FilterByExpression_ReturnsFilteredResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateAggregateIndexAsync(client);

        var rows = await Ft.AggregateAsync(client, index, "*",
            new Ft.AggregateOptions
            {
                LoadFields = ["@price"],
                Clauses =
                [
                    new Ft.AggregateFilter("@price > 20"),
                    new Ft.AggregateSortBy
                    {
                        Expressions = [new Ft.AggregateSortExpression { Expression = "@price", Order = SortOrder.Ascending }],
                    },
                ],
            });

        Assert.Equivalent(new Dictionary<ValkeyValue, ValkeyValue>[]
            {
                new() { ["price"] = "25" },
                new() { ["price"] = "30" },
                new() { ["price"] = "50" },
            },
            rows);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AggregateAsync_ApplyTransformation_ReturnsTransformedResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateAggregateIndexAsync(client);

        var rows = await Ft.AggregateAsync(client, index, "*",
            new Ft.AggregateOptions
            {
                LoadFields = ["@price"],
                Clauses =
                [
                    new Ft.AggregateApply
                    {
                        Expression = "@price * 2",
                        Name = "double_price",
                    },
                    new Ft.AggregateSortBy
                    {
                        Expressions = [new Ft.AggregateSortExpression { Expression = "@price", Order = SortOrder.Ascending }],
                    },
                ],
            });

        Assert.Equivalent(new Dictionary<ValkeyValue, ValkeyValue>[]
            {
                new() { ["price"] = "10", ["double_price"] = "20" },
                new() { ["price"] = "15", ["double_price"] = "30" },
                new() { ["price"] = "25", ["double_price"] = "50" },
                new() { ["price"] = "30", ["double_price"] = "60" },
                new() { ["price"] = "50", ["double_price"] = "100" },
            },
            rows);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AggregateAsync_WithLimit_ReturnsLimitedResults(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateAggregateIndexAsync(client);

        var rows = await Ft.AggregateAsync(client, index, "*",
            new Ft.AggregateOptions
            {
                LoadFields = ["@price"],
                Clauses =
                [
                    new Ft.AggregateSortBy
                    {
                        Expressions = [new Ft.AggregateSortExpression { Expression = "@price", Order = SortOrder.Ascending }],
                    },
                    new Ft.AggregateLimit { Offset = 0, Count = 2 },
                ],
            });

        Assert.Equivalent(new Dictionary<ValkeyValue, ValkeyValue>[]
            {
                new() { ["price"] = "10" },
                new() { ["price"] = "15" },
            },
            rows);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AggregateAsync_PipelineGroupBySortByLimit_ReturnsOrderedLimitedGroups(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        (string index, _, _) = await FtUtils.CreateAggregateIndexAsync(client);

        var rows = await Ft.AggregateAsync(client, index, "*",
            new Ft.AggregateOptions
            {
                LoadFields = ["@category"],
                Clauses =
                [
                    new Ft.AggregateGroupBy
                    {
                        Fields = ["@category"],
                        Reducers =
                        [
                            new Ft.AggregateReducer
                            {
                                Function = Ft.ReducerFunction.Count,
                                Name = "count",
                            },
                        ],
                    },
                    new Ft.AggregateSortBy
                    {
                        Expressions =
                        [
                            new Ft.AggregateSortExpression
                            {
                                Expression = "@count",
                                Order = SortOrder.Descending,
                            },
                        ],
                    },
                    new Ft.AggregateLimit { Offset = 0, Count = 1 },
                ],
            });

        Assert.Equivalent(
            new Dictionary<ValkeyValue, ValkeyValue>[] {
                new() { ["category"] = "electronics", ["count"] = "3" } },
            rows);
    }

    #endregion
}
