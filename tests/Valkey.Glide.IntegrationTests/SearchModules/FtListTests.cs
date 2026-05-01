// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.SearchModules;

/// <summary>
/// Integration tests for <see cref="Ft.ListAsync"/>.
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
        Assert.Empty(await Ft.ListAsync(client));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task ListAsync_MultipleIndexes_ReturnsAll(bool clusterMode)
    {
        var client = fixture.GetClient(clusterMode);
        var indexes = Enumerable.Range(0, 3).Select(_ => Guid.NewGuid().ToString()).ToList();
        
        foreach (var idx in indexes)
            await Ft.CreateAsync(client, idx, new Ft.TextField("field"));

        Assert.Equivalent(indexes.ToHashSet(), await Ft.ListAsync(client));

        // Cleanup to avoid polluting other list tests.
        foreach (var idx in indexes)
            await Ft.DropIndexAsync(client, idx);
    }

    #endregion
}
