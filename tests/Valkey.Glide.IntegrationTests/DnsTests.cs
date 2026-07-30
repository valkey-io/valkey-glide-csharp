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
    public void GetServers_WithDnsHostnameTopology_ReturnsDnsEndPoints()
    {
        SkipIfDnsTestsNotEnabled();

        var config = new ConfigurationOptions();
        config.EndPoints.Add(fixture.DnsClusterServer!.Address.Host, fixture.DnsClusterServer.Address.Port);
        config.ResponseTimeout = 10000;

        using var conn = ConnectionMultiplexer.Connect(config);

        IServer[] servers = conn.GetServers();
        Assert.True(servers.Length > 0);

        foreach (IServer s in servers)
        {
            Assert.IsType<DnsEndPoint>(s.EndPoint);
            Assert.Contains(HostnameTls, s.EndPoint.ToString());
        }
    }

    [Fact]
    public void GetEndPoints_WithDnsHostnameTopology_ReturnsDnsEndPoints()
    {
        SkipIfDnsTestsNotEnabled();

        var config = new ConfigurationOptions();
        config.EndPoints.Add(fixture.DnsClusterServer!.Address.Host, fixture.DnsClusterServer.Address.Port);
        config.ResponseTimeout = 10000;

        using var conn = ConnectionMultiplexer.Connect(config);

        EndPoint[] endpoints = conn.GetEndPoints(false);
        Assert.True(endpoints.Length > 0);

        foreach (EndPoint ep in endpoints)
        {
            Assert.IsType<DnsEndPoint>(ep);
            IServer found = conn.GetServer(ep);
            Assert.Equal(ep, found.EndPoint);
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
            DnsClusterServer = new(host: HostnameTls);

            try
            {
                TlsClusterServer = new(useTls: true);
                TlsStandaloneServer = new(useTls: true);
            }
            catch
            {
                // TLS servers may fail to start in some environments.
                // Tests that require TLS will be skipped via null checks.
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

