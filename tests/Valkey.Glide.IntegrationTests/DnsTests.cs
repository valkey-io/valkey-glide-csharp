// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Assertions;
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
    internal const string DnsEnabledEnvVar = "VALKEY_GLIDE_DNS_TESTS_ENABLED";

    #endregion
    #region Test Data

    public static TheoryData<bool> TlsMode => [true, false];

    public static TheoryData<bool, bool> ClusterAndTlsMode => new()
    {
        { true, true },
        { true, false },
        { false, true },
        { false, false },
    };

    #endregion
    #region Constructor

    static DnsTests()
    {
        Assert.SkipWhen(
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable(DnsEnabledEnvVar)),
            "DNS tests are disabled. See DEVELOPER.md for setup instructions.");
    }

    #endregion
    #region Tests

    [Theory]
    [MemberData(nameof(ClusterAndTlsMode))]
    public async Task Connect_WithValidHostname_Succeeds(bool useCluster, bool useTls)
    {
        var server = GetServer(useCluster, useTls);
        var host = useTls ? HostnameTls : HostnameNoTls;
        await using var client = await server.CreateClientAsync(host);

        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(ClusterAndTlsMode))]
    public async Task Connect_WithInvalidHostname_Fails(bool useCluster, bool useTls)
        => _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GetServer(useCluster, useTls).CreateClientAsync("NONEXISTENT.INVALID"));

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Connect_WithHostnameNotInCertificate_Fails(bool useCluster)
        => _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GetServer(useCluster, useTls: true).CreateClientAsync(HostnameNoTls));

    /// <summary>
    /// Verifies that <see cref="ConnectionMultiplexer.GetServers"/> returns servers with <see cref="DnsEndPoint"/>
    /// instances when the cluster topology reports DNS hostnames (via <c>cluster-announce-hostname</c>).
    /// </summary>
    /// <seealso href="https://github.com/valkey-io/valkey-glide-csharp/issues/419">#419</seealso>
    [Theory]
    [MemberData(nameof(TlsMode))]
    public void GetServers_WithDnsHostname_ReturnsDnsEndPoints(bool useTls)
    {
        var server = GetServer(useCluster: true, useTls);
        var config = new ConfigurationOptions { Ssl = useTls };
        config.EndPoints.Add(server.Address.Host, server.Address.Port);

        if (useTls)
        {
            config.TrustIssuer(ServerManager.ServerCertificatePath);
        }

        using var conn = ConnectionMultiplexer.Connect(config);

        var servers = conn.GetServers();
        Assert.NotEmpty(servers);

        foreach (var s in servers)
        {
            _ = Assert.IsType<DnsEndPoint>(s.EndPoint);
            Assert.Contains(HostnameTls, s.EndPoint.ToString());
        }
    }

    /// <summary>
    /// Verifies that <see cref="ConnectionMultiplexer.GetEndPoints"/> returns <see cref="DnsEndPoint"/>
    /// instances when the cluster topology reports DNS hostnames (via <c>cluster-announce-hostname</c>).
    /// </summary>
    /// <seealso href="https://github.com/valkey-io/valkey-glide-csharp/issues/419">#419</seealso>
    [Theory]
    [MemberData(nameof(TlsMode))]
    public void GetEndPoints_WithDnsHostname_ReturnsDnsEndPoints(bool useTls)
    {
        var server = GetServer(useCluster: true, useTls);
        var config = new ConfigurationOptions { Ssl = useTls };
        config.EndPoints.Add(server.Address.Host, server.Address.Port);

        if (useTls)
        {
            config.TrustIssuer(ServerManager.ServerCertificatePath);
        }

        using var conn = ConnectionMultiplexer.Connect(config);

        var endpoints = conn.GetEndPoints(false);
        Assert.NotEmpty(endpoints);

        foreach (var endpoint in endpoints)
        {
            _ = Assert.IsType<DnsEndPoint>(endpoint);
            Assert.Contains(HostnameTls, endpoint.ToString());

            var s = conn.GetServer(endpoint);
            Assert.Equal(endpoint, s.EndPoint);
        }
    }

    #endregion
    #region Helpers

    private Server GetServer(bool useCluster, bool useTls) => (useCluster, useTls) switch
    {
        (true, true) => fixture.TlsClusterServer!,
        (true, false) => fixture.ClusterServer!,
        (false, true) => fixture.TlsStandaloneServer!,
        _ => fixture.StandaloneServer!,
    };

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

    public ValueTask InitializeAsync()
    {
        ClusterServer = new(host: HostnameTls);
        StandaloneServer = new(host: HostnameTls);
        TlsClusterServer = new(useTls: true, host: HostnameTls);
        TlsStandaloneServer = new(useTls: true, host: HostnameTls);

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        ClusterServer?.Dispose();
        StandaloneServer?.Dispose();
        TlsClusterServer?.Dispose();
        TlsStandaloneServer?.Dispose();

        return ValueTask.CompletedTask;
    }
}
