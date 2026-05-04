// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Integration tests for:
/// <list type="bullet">
/// <item><see cref="Ft.CreateAsync(BaseClient, ValkeyKey, Ft.CreateField)"/></item>
/// <item><see cref="Ft.CreateAsync(BaseClient, ValkeyKey, Ft.CreateField, Ft.CreateOptions)"/></item>
/// <item><see cref="Ft.CreateAsync(BaseClient, ValkeyKey, IEnumerable{Ft.CreateField})"/></item>
/// <item><see cref="Ft.CreateAsync(BaseClient, ValkeyKey, IEnumerable{Ft.CreateField}, Ft.CreateOptions)"/></item>
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
        var field = new Ft.CreateTextField("title");
        await Ft.CreateAsync(client, index, field);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_TagField_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateTagField("category");
        await Ft.CreateAsync(client, index, field);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_NumericField_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateNumericField("price");
        await Ft.CreateAsync(client, index, field);

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_VectorFieldFlat_Succeeds(BaseClient client)
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

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_VectorFieldHnsw_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        var field = new Ft.CreateVectorFieldHnsw
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
        Ft.CreateField[] schema =
        [
            new Ft.CreateTextField("title", "t"),
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

        Assert.Contains(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CreateAsync_DuplicateIndex_Throws(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"));

        _ = await Assert.ThrowsAsync<RequestException>(
            () => Ft.CreateAsync(client, index, new Ft.CreateTextField("title")));
    }

    #endregion
}
