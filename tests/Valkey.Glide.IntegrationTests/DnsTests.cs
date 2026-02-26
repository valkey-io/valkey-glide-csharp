// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for client connection with hostnames.
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
public class DnsTests : IClassFixture<DnsServersFixture>
{
    private readonly DnsServersFixture fixture;

    // Hostname constants for testing.
    // See 'cluster_manager.py' for details.
    private const string HostnameTls = "valkey.glide.test.tls.com";
    private const string HostnameNoTls = "valkey.glide.test.no_tls.com";

    public DnsTests(DnsServersFixture fixture)
    {
        this.fixture = fixture;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task NoTls_HostnameInMapping_Succeeds(bool useCluster)
    {
        await using var client = await BuildClient(useCluster, useTls: false, HostnameNoTls);
        await AssertConnected(client);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task NoTls_HostnameNotInMapping_Fails(bool useCluster)
    {
        await Assert.ThrowsAsync<ConnectionException>(async ()
        => await BuildClient(useCluster, useTls: false, "nonexistent.invalid"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Tls_HostnameInCertificate_Succeeds(bool useCluster)
    {
        await using var client = await BuildClient(useCluster, useTls: true, HostnameTls);
        await AssertConnected(client);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Tls_HostnameNotInCertificate_Fails(bool useCluster)
    {
        await Assert.ThrowsAsync<ConnectionException>(async ()
        => await BuildClient(useCluster, useTls: true, HostnameNoTls));
    }

    // Helper Methods
    // --------------

    private async Task<BaseClient> BuildClient(bool useCluster, bool useTls, string hostname)
    {
        var server = useCluster
            ? (useTls ? fixture.TlsClusterServer : fixture.NonTlsClusterServer)
            : (useTls ? fixture.TlsStandaloneServer : fixture.NonTlsStandaloneServer);
        var port = server.Addresses.First().Port;

        var certificateBytes = File.ReadAllBytes(ServerManager.ServerCertificatePath);

        if (useCluster)
        {
            var builder = new ClusterClientConfigurationBuilder()
                .WithAddress(hostname, port)
                .WithTls(useTls);

            if (useTls)
                builder.WithTrustedCertificate(certificateBytes);

            return await GlideClusterClient.CreateClient(builder.Build());
        }

        else
        {
            var builder = new StandaloneClientConfigurationBuilder()
                .WithAddress(hostname, port)
                .WithTls(useTls);

            if (useTls)
                builder.WithTrustedCertificate(certificateBytes);

            return await GlideClient.CreateClient(builder.Build());
        }
    }
}

/// <summary>
/// Fixture class to manage server lifecycle for DNS tests.
/// </summary>
public class DnsServersFixture : IDisposable
{
    public Server TlsClusterServer = new ClusterServer(useTls: true);
    public Server NonTlsClusterServer = new ClusterServer(useTls: false);
    public Server TlsStandaloneServer = new StandaloneServer(useTls: true);
    public Server NonTlsStandaloneServer = new StandaloneServer(useTls: false);

    public void Dispose()
    {
        TlsClusterServer.Dispose();
        NonTlsClusterServer.Dispose();
        TlsStandaloneServer.Dispose();
        NonTlsStandaloneServer.Dispose();
    }
}

