// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Integration tests for <c>FT.CREATE</c>:
/// <list type="bullet">
/// <item><see cref="Ft.CreateAsync(BaseClient, ValkeyKey, Ft.Field)"/></item>
/// <item><see cref="Ft.CreateAsync(BaseClient, ValkeyKey, Ft.Field, Ft.CreateOptions)"/></item>
/// <item><see cref="Ft.CreateAsync(BaseClient, ValkeyKey, IEnumerable{Ft.Field})"/></item>
/// <item><see cref="Ft.CreateAsync(BaseClient, ValkeyKey, IEnumerable{Ft.Field}, Ft.CreateOptions)"/></item>
/// </list>
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
public class FtCreateTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_TextField_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();
        
        var field = new Ft.TextField("title");
        await Ft.CreateAsync(client, index, field);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_TagField_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        var field = new Ft.TagField("category");
        await Ft.CreateAsync(client, index, field);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_NumericField_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        var field = new Ft.NumericField("price");
        await Ft.CreateAsync(client, index, field);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_VectorFieldFlat_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        var field = new Ft.VectorFieldFlat
        {
            Identifier = "embedding",
            Dimensions = 128,
            DistanceMetric = Ft.DistanceMetric.Cosine,
        };
        await Ft.CreateAsync(client, index, field);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_VectorFieldHnsw_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        var field = new Ft.VectorFieldHnsw
        {
            Identifier = "embedding",
            Dimensions = 64,
            DistanceMetric = Ft.DistanceMetric.Euclidean,
        };
        await Ft.CreateAsync(client, index, field);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_MultipleFieldTypes_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);
        var index = Guid.NewGuid().ToString();

        Ft.Field[] schema =
        [
            new Ft.TextField("title", "t"),
            new Ft.TagField("category"),
            new Ft.NumericField("price"),
            new Ft.VectorFieldFlat
            {
                Identifier = "vec",
                Dimensions = 32,
                DistanceMetric = Ft.DistanceMetric.InnerProduct,
            },
        ];
        await Ft.CreateAsync(client, index, schema);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    #endregion
}
