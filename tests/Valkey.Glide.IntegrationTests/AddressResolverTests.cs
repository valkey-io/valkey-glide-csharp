// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Client;
using static Valkey.Glide.TestUtils.Config;
using static Valkey.Glide.TestUtils.Data;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for the address resolver callback.
/// </summary>
public class AddressResolverTests
{
    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task AddressResolver_IdentityResolver_IsInvokedAndConnects(bool useCluster)
    {
        var address = useCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        int count = 0;
        (string, ushort) Resolver(string host, ushort port)
        {
            _ = Interlocked.Increment(ref count);
            return (address.Host, address.Port);
        }

        var config = BuildConfig(useCluster, address, useTls: TestConfiguration.TLS, addressResolver: Resolver);
        await using var client = await CreateClient(config);

        await AssertConnected(client);
        Assert.True(count > 0);
    }
}
