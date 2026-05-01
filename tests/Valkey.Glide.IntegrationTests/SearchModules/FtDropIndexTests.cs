// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Integration tests for <see cref="Ft.DropIndexAsync"/>.
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
        string index = Guid.NewGuid().ToString();
        await Ft.CreateAsync(client, index, new Ft.TextField("title"));

        await Ft.DropIndexAsync(client, index);
        Assert.DoesNotContain(index, await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public Task DropIndexAsync_IndexDoesNotExist_Throws(BaseClient client)
        => Assert.ThrowsAsync<Exception>(
            () => Ft.DropIndexAsync(client, Guid.NewGuid().ToString()));

    #endregion
}
