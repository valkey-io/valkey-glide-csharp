// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;
using static Valkey.Glide.TestUtils.Constants;
using static Valkey.Glide.TestUtils.Data;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// DNS resolution tests.
/// See <see href="../../DEVELOPER.md#dns-tests">DEVELOPER.md</see> for setup instructions.
/// </summary>
public class DnsTests(DnsTestsFixture fixture) : IClassFixture<DnsTestsFixture>
{
    #region Constants

    /// <summary>
    /// Environment variable for enabling DNS tests.
    /// See <see href="../../DEVELOPER.md#dns-tests">DEVELOPER.md</see> for more details.
    /// </summary>
    private const string DnsEnabledEnvVar = "VALKEY_GLIDE_DNS_TESTS_ENABLED";

    #endregion
    #region Tests

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task ConnectWithValidHostname_Succeeds(bool useCluster)
    {
        SkipIfDnsTestsNotEnabled();
        await using var client = await BuildClient(useCluster, useTls: false, HostnameNoTls);
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task ConnectWithInvalidHostname_Fails(bool useCluster)
    {
        SkipIfDnsTestsNotEnabled();
        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await BuildClient(useCluster, useTls: false, "NONEXISTENT.INVALID"));
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Tls_WithHostnameInCertificate_Succeeds(bool useCluster)
    {
        SkipIfDnsTestsNotEnabled();
        await using var client = await BuildClient(useCluster, useTls: true, HostnameTls);
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Tls_WithHostnameNotInCertificate_Fails(bool useCluster)
    {
        SkipIfDnsTestsNotEnabled();
        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await BuildClient(useCluster, useTls: true, HostnameNoTls));
    }

    [Fact]
    public void GetServers_WithDnsHostname_ReturnsDnsEndPoints()
    {
        SkipIfDnsTestsNotEnabled();
        Assert.SkipWhen(fixture.DnsClusterServer is null, "DNS cluster server not available.");

        var config = new ConfigurationOptions();
        var address = fixture.DnsClusterServer!.Address;
        config.EndPoints.Add(address.Host, address.Port);

        using var conn = ConnectionMultiplexer.Connect(config);

        var servers = conn.GetServers();
        Assert.NotEmpty(servers);

        foreach (var server in servers)
        {
            _ = Assert.IsType<DnsEndPoint>(server.EndPoint);
            Assert.Contains(HostnameTls, server.EndPoint.ToString());
        }
    }

    [Fact]
    public void GetEndPoints_WithDnsHostname_ReturnsDnsEndPoints()
    {
        SkipIfDnsTestsNotEnabled();
        Assert.SkipWhen(fixture.DnsClusterServer is null, "DNS cluster server not available.");

        var config = new ConfigurationOptions();
        var address = fixture.DnsClusterServer!.Address;
        config.EndPoints.Add(address.Host, address.Port);

        using var conn = ConnectionMultiplexer.Connect(config);

        var endpoints = conn.GetEndPoints(false);
        Assert.NotEmpty(endpoints);

        foreach (EndPoint endpoint in endpoints)
        {
            _ = Assert.IsType<DnsEndPoint>(endpoint);
            Assert.Contains(HostnameTls, endpoint.ToString());
        }
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Returns true if DNS tests are enabled.
    /// </summary>
    public static bool IsDnsTestsEnabled()
        => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(DnsEnabledEnvVar));

    /// <summary>
    /// Skips the current test if DNS tests are not enabled.
    /// </summary>
    private static void SkipIfDnsTestsNotEnabled()
        => Assert.SkipWhen(
            !IsDnsTestsEnabled(),
            $"DNS tests are disabled. See DEVELOPER.md for setup instructions.");

    /// <summary>
    /// Builds and returns a client configured with the specified parameters.
    /// </summary>
    private async Task<BaseClient> BuildClient(bool useCluster, bool useTls, string host)
    {
        if (useCluster)
        {
            var server = useTls ? fixture.TlsClusterServer! : fixture.ClusterServer!;
            var builder = new ClusterClientConfigurationBuilder()
                .WithAddress(host, server.Address.Port);

            if (useTls)
            {
                _ = builder.WithTls();
                _ = builder.WithTrustedCertificate(server.CertificateData!);
            }

            return await GlideClusterClient.CreateClient(builder.Build());
        }

        else
        {
            var server = useTls ? fixture.TlsStandaloneServer! : fixture.StandaloneServer!;
            var builder = new StandaloneClientConfigurationBuilder()
                .WithAddress(host, server.Address.Port);

            if (useTls)
            {
                _ = builder.WithTls();
                _ = builder.WithTrustedCertificate(server.CertificateData!);
            }

            return await GlideClient.CreateClient(builder.Build());
        }
    }

    #endregion
}

/// <summary>
/// Fixture class for DNS tests.
/// </summary>
public class DnsTestsFixture : IAsyncLifetime
{
    public ClusterServer? ClusterServer { get; private set; }
    public StandaloneServer? StandaloneServer { get; private set; }
    public ClusterServer? TlsClusterServer { get; private set; }
    public StandaloneServer? TlsStandaloneServer { get; private set; }
    public ClusterServer? DnsClusterServer { get; private set; }

    public ValueTask InitializeAsync()
    {
        // Only start the servers if DNS tests are enabled.
        if (DnsTests.IsDnsTestsEnabled())
        {
            ClusterServer = new(useTls: false);
            StandaloneServer = new(useTls: false);

            try
            {
                DnsClusterServer = new(host: HostnameTls);
            }
            catch
            {
                // DNS cluster may fail in environments where cluster-announce-hostname
                // isn't properly supported (e.g. wait_for_all_topology_views timeout).
            }

            try
            {
                TlsClusterServer = new(useTls: true);
                TlsStandaloneServer = new(useTls: true);
            }
            catch
            {
                // TLS servers may fail to start in some environments.
            }
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        ClusterServer?.Dispose();
        StandaloneServer?.Dispose();
        TlsClusterServer?.Dispose();
        TlsStandaloneServer?.Dispose();
        DnsClusterServer?.Dispose();

        return ValueTask.CompletedTask;
    }
}

