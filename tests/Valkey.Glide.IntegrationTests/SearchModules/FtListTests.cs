// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Integration tests for <c>FT._LIST</c>:
/// <list type="bullet">
/// <item><see cref="Ft.ListAsync(BaseClient)"/></item>
/// </list>
/// </summary>
/// <seealso href="https://valkey.io/commands/ft._list/">Valkey commands – FT._LIST</seealso>
public class FtListTests(ClientFixture fixture) : IClassFixture<ClientFixture>
{
    #region Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task ListAsync_NoIndexes_ReturnsEmptySet(bool clusterMode)
    {
        var client = fixture.GetClient(clusterMode);
        await SkipUtils.IfSearchModuleNotLoaded(client);

        Assert.Empty(await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task ListAsync_MultipleIndexes_ReturnsAll(bool clusterMode)
    {
        var client = fixture.GetClient(clusterMode);
        await SkipUtils.IfSearchModuleNotLoaded(client);

        var indexes = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid().ToString()).ToList();
        foreach (var idx in indexes)
        {
            await Ft.CreateAsync(client, idx, new Ft.CreateTextField("field"));
        }

        Assert.Equivalent(indexes.ToHashSet(), await Ft.ListAsync(client));
        foreach (var idx in indexes)
        {
            await Ft.DropIndexAsync(client, idx);
        }
    }

    #endregion
}
