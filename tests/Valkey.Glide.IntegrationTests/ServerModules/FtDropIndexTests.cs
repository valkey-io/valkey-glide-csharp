// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests.ServerModules;

/// <summary>
/// Integration tests for <c>FT.DROPINDEX</c>:
/// <list type="bullet">
/// <item><see cref="Ft.DropIndexAsync(BaseClient, ValkeyKey)"/></item>
/// </list>
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.dropindex/">Valkey commands – FT.DROPINDEX</seealso>
public class FtDropIndexTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DropIndexAsync_IndexExists_Succeeds(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        string index = Guid.NewGuid().ToString();
        await Ft.CreateAsync(client, index, new Ft.CreateTextField("title"));

        await Ft.DropIndexAsync(client, index);
        Assert.DoesNotContain(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DropIndexAsync_IndexDoesNotExist_Throws(BaseClient client)
    {
        await SkipUtils.IfSearchModuleNotLoaded(client);

        _ = await Assert.ThrowsAsync<RequestException>(
            () => Ft.DropIndexAsync(client, Guid.NewGuid().ToString()));
    }

    #endregion
}
