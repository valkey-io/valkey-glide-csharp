// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Integration tests for <see cref="Ft.ListAsync(BaseClient)"/>.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft._list/">Valkey commands – FT._LIST</seealso>
public class FtListTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ListAsync_NewIndex_AppearsInList(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        await Ft.CreateAsync(client, index, new Ft.CreateTextField("field"));

        var list = await Ft.ListAsync(client);
        Assert.Contains((ValkeyValue)index, list);

        await Ft.DropIndexAsync(client, index);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ListAsync_DroppedIndex_DisappearsFromList(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var index = Guid.NewGuid().ToString();
        await Ft.CreateAsync(client, index, new Ft.CreateTextField("field"));
        Assert.Contains((ValkeyValue)index, await Ft.ListAsync(client));

        await Ft.DropIndexAsync(client, index);
        Assert.DoesNotContain((ValkeyValue)index, await Ft.ListAsync(client));
    }

    #endregion
}
