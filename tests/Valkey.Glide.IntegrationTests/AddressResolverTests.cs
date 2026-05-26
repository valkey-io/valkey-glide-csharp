// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.TestUtils.Client;
using static Valkey.Glide.TestUtils.Data;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for the address resolver callback.
/// </summary>
public class AddressResolverTests
{
    #region Tests

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task AddressResolver_IdentityResolver_IsInvokedAndConnects(bool useCluster)
    {
        var address = useCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        int invocationCount = 0;
        (string, ushort) Resolver(string host, ushort port)
        {
            _ = Interlocked.Increment(ref invocationCount);
            return (host, port);
        }

        using var client = await BuildClient(
            useCluster,
            host: address.Host,
            port: address.Port,
            resolver: Resolver);

        await AssertConnected(client);
        Assert.True(invocationCount > 0);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task AddressResolver_ResolvesAddress_ConnectsSuccessfully(bool useCluster)
    {
        var address = useCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;

        using var client = await BuildClient(
            useCluster: false,
            host: "nonexistent.invalid",
            port: 0001,
            resolver: (_, _) => (address.Host, address.Port));

        await AssertConnected(client);
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Builds a client configured with the given host, port, and address resolver.
    /// </summary>
    private static async Task<BaseClient> BuildClient(
        bool useCluster, string host, ushort port, AddressResolverDelegate resolver)
        => useCluster
            ? await GlideClusterClient.CreateClient(
                new ClusterClientConfigurationBuilder()
                    .WithAddress(host, port)
                    .WithRequestTimeout(TestConfiguration.DEFAULT_TIMEOUT)
                    .WithTls(TestConfiguration.TLS)
                    .WithAddressResolver(resolver)
                    .Build())
            : await GlideClient.CreateClient(
                new StandaloneClientConfigurationBuilder()
                    .WithAddress(host, port)
                    .WithRequestTimeout(TestConfiguration.DEFAULT_TIMEOUT)
                    .WithTls(TestConfiguration.TLS)
                    .WithAddressResolver(resolver)
                    .Build());

    #endregion
}
