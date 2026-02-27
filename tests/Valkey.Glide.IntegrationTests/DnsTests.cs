// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for client connection with hosts.
/// <para>
/// To run these tests, you need to add the following mappings to your hosts
/// file then set the environment variable <c>VALKEY_GLIDE_DNS_TESTS_ENABLED</c>:
/// </para>
/// <list type="bullet">
/// <item><c>127.0.0.1 valkey.glide.test.tls.com</c></item>
/// <item><c>127.0.0.1 valkey.glide.test.no_tls.com</c></item>
/// <item><c>::1 valkey.glide.test.tls.com</c></item>
/// <item><c>::1 valkey.glide.test.no_tls.com</c></item>
/// </list>
/// </summary>
public class HostTests(HostTestsFixture fixture) : IClassFixture<HostTestsFixture>
{
    // Host name and address constants.
    // See 'cluster_manager.py' for details.
    private static readonly string hostTls = "valkey.glide.test.tls.com";
    private static readonly string hostNoTls = "valkey.glide.test.no_tls.com";
    private static readonly string HostAddressIpv4 = "127.0.0.1";
    private static readonly string HostAddressIpv6 = "::1";

    // Cluster and host address combinations to test.
    public static TheoryData<bool> ClusterModeData => [true, false];

    public static TheoryData<bool, string> HostAddressData => new()
    {
        { true, HostAddressIpv4 },
        { false, HostAddressIpv4 },
        { true, HostAddressIpv6 },
        { false, HostAddressIpv6 }
    };

    [Theory]
    [MemberData(nameof(ClusterModeData))]
    public async Task NoTls_Withhost_InMapping_Succeeds(bool useCluster)
    {
        await using var client = await BuildClient(useCluster, useTls: false, hostNoTls);
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData))]
    public async Task NoTls_Withhost_NotInMapping_Fails(bool useCluster)
    {
        await Assert.ThrowsAsync<ConnectionException>(async ()
        => await BuildClient(useCluster, useTls: false, "nonexistent.invalid"));
    }

    [Theory]
    [MemberData(nameof(HostAddressData))]
    public async Task NoTls_WithHostAddress_Succeeds(bool useCluster, string hostAddress)
    {
        await using var client = await BuildClient(useCluster, useTls: false, hostAddress);
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData))]
    public async Task Tls_Withhost_InCertificate_Succeeds(bool useCluster)
    {
        await using var client = await BuildClient(useCluster, useTls: true, hostTls);
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData))]
    public async Task Tls_Withhost_NotInCertificate_Fails(bool useCluster)
    {
        await Assert.ThrowsAsync<ConnectionException>(async ()
        => await BuildClient(useCluster, useTls: true, hostNoTls));
    }

    [Theory]
    [MemberData(nameof(HostAddressData))]
    public async Task Tls_WithHostAddress_Succeeds(bool useCluster, string hostAddress)
    {
        await using var client = await BuildClient(useCluster, useTls: true, hostAddress);
        await AssertConnected(client);
    }

    // Helper Methods
    // --------------

    /// <summary>
    /// Builds and returns a client with the specified cluster mode, TLS setting, and host.
    /// </summary>
    private async Task<BaseClient> BuildClient(bool useCluster, bool useTls, string host)
    {
        if (useCluster)
        {
            var cluster = useTls ? fixture.TlsClusterServer : fixture.NonTlsClusterServer;
            var port = cluster.Addresses.First().Port;
            var builder = cluster.CreateConfigBuilder()
                .WithAddress(host, port)
                .WithTls(useTls);

            if (useTls)
                builder.WithTrustedCertificate(cluster.CertificateData);

            return await GlideClusterClient.CreateClient(builder.Build());
        }

        else
        {
            var standalone = useTls ? fixture.TlsStandaloneServer : fixture.NonTlsStandaloneServer;
            var port = standalone.Addresses.First().Port;
            var builder = standalone.CreateConfigBuilder()
                .WithAddress(host, port)
                .WithTls(useTls);

            if (useTls)
                builder.WithTrustedCertificate(standalone.CertificateData);

            return await GlideClient.CreateClient(builder.Build());
        }
    }
}

/// <summary>
/// Fixture class for host tests.
/// </summary>
public class HostTestsFixture : IDisposable
{
    public ClusterServer TlsClusterServer = new(useTls: true);
    public ClusterServer NonTlsClusterServer = new(useTls: false);
    public StandaloneServer TlsStandaloneServer = new(useTls: true);
    public StandaloneServer NonTlsStandaloneServer = new(useTls: false);

    public void Dispose()
    {
        TlsClusterServer.Dispose();
        NonTlsClusterServer.Dispose();
        TlsStandaloneServer.Dispose();
        NonTlsStandaloneServer.Dispose();
    }
}

