// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;
using static Valkey.Glide.TestUtils.Data;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// DNS resolution tests.
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
public class DnsTests(DnsTestsFixture fixture) : IClassFixture<DnsTestsFixture>
{
    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task ConnectWithValidHostname_Succeeds(bool useCluster)
    {
        SkipIfDnsTestsNotEnabled();
        await using var client = await BuildClient(useCluster, useTls: false, Server.HostnameNoTls);
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task ConnectWithInvalidHostname_Fails(bool useCluster)
    {
        SkipIfDnsTestsNotEnabled();
        await Assert.ThrowsAsync<ConnectionException>(async ()
        => await BuildClient(useCluster, useTls: false, "nonexistent.invalid"));
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Tls_WithHostnameInCertificate_Succeeds(bool useCluster)
    {
        SkipIfDnsTestsNotEnabled();
        await using var client = await BuildClient(useCluster, useTls: true, Server.HostnameTls);
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Tls_WithHostnameNotInCertificate_Fails(bool useCluster)
    {
        SkipIfDnsTestsNotEnabled();
        await Assert.ThrowsAsync<ConnectionException>(async ()
        => await BuildClient(useCluster, useTls: true, Server.HostnameNoTls));
    }

    // Helper Methods
    // --------------

    /// <summary>
    /// Skips the current test if DNS tests are not enabled.
    /// </summary>
    private static void SkipIfDnsTestsNotEnabled()
    {
        const string envVar = "VALKEY_GLIDE_DNS_TESTS_ENABLED";
        Assert.SkipWhen(
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envVar)),
            $"DNS tests are disabled. Set the environment variable {envVar} to enable them.");
    }

    /// <summary>
    /// Builds and returns a client configured with the specified parameters.
    /// </summary>
    private async Task<BaseClient> BuildClient(bool useCluster, bool useTls, string host)
    {
        if (useCluster)
        {
            var server = useTls ? fixture.TlsClusterServer : fixture.ClusterServer;
            var port = server.Addresses.First().Port;

            var builder = new ClusterClientConfigurationBuilder();
            builder.WithAddress(host, port);

            if (useTls)
            {
                builder.WithTls();
                builder.WithTrustedCertificate(server.CertificateData!);
            }

            return await GlideClusterClient.CreateClient(builder.Build());
        }

        else
        {
            var server = useTls ? fixture.TlsStandaloneServer : fixture.StandaloneServer;
            var port = server.Addresses.First().Port;

            var builder = new StandaloneClientConfigurationBuilder();
            builder.WithAddress(host, port);

            if (useTls)
            {
                builder.WithTls();
                builder.WithTrustedCertificate(server.CertificateData!);
            }

            return await GlideClient.CreateClient(builder.Build());
        }
    }
}

/// <summary>
/// Fixture class for DNS tests.
/// </summary>
public class DnsTestsFixture : IDisposable
{
    public ClusterServer ClusterServer = new(useTls: false);
    public StandaloneServer StandaloneServer = new(useTls: false);
    public ClusterServer TlsClusterServer = new(useTls: true);
    public StandaloneServer TlsStandaloneServer = new(useTls: true);

    public void Dispose()
    {
        ClusterServer.Dispose();
        StandaloneServer.Dispose();
        TlsClusterServer.Dispose();
        TlsStandaloneServer.Dispose();
    }
}

