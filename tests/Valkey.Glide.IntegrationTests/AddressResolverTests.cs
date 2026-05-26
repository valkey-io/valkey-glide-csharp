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

        var config = BuildConfig(useCluster, address, addressResolver: Resolver);
        await using var client = await CreateClient(config);

        await AssertConnected(client);
        Assert.True(count > 0);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task AddressResolver_ThrowsException_FallsBackToOriginalAddress(bool useCluster)
    {
        var address = useCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        var config = BuildConfig(useCluster, address, addressResolver: (_, _) => throw new InvalidOperationException("Resolution failed!"));
        await using var client = await CreateClient(config);

        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task AddressResolver_ReturnsEmptyHost_FallsBackToOriginalAddress(bool useCluster)
    {
        var address = useCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        var config = BuildConfig(useCluster, address, addressResolver: (_, _) => ("", address.Port));
        await using var client = await CreateClient(config);

        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task AddressResolver_ReturnsPortZero_FallsBackToOriginalAddress(bool useCluster)
    {
        var address = useCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        var config = BuildConfig(useCluster, address, addressResolver: (_, _) => (address.Host, 0));
        await using var client = await CreateClient(config);

        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task AddressResolver_ReturnsOversizedHost_FallsBackToOriginalAddress(bool useCluster)
    {
        var address = useCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        var config = BuildConfig(useCluster, address, addressResolver: (_, _) => (new string('a', 2048), address.Port));
        await using var client = await CreateClient(config);

        await AssertConnected(client);
    }
}
