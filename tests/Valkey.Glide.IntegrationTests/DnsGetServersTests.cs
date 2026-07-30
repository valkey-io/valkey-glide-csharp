// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Constants;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests that <see cref="ConnectionMultiplexer.GetServers"/> correctly handles DNS hostnames
/// in cluster topology responses (GitHub issue #419).
/// </summary>
public class DnsGetServersTests(DnsGetServersFixture fixture) : IClassFixture<DnsGetServersFixture>
{
    private const string DnsEnabledEnvVar = "VALKEY_GLIDE_DNS_TESTS_ENABLED";

    [Fact]
    public void GetServers_WithDnsHostnameTopology_ReturnsDnsEndPoints()
    {
        SkipIfDnsTestsNotEnabled();

        var config = new ConfigurationOptions();
        config.EndPoints.Add(fixture.Server!.Address.Host, fixture.Server.Address.Port);
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
        config.EndPoints.Add(fixture.Server!.Address.Host, fixture.Server.Address.Port);
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

    private static void SkipIfDnsTestsNotEnabled()
        => Assert.SkipWhen(
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable(DnsEnabledEnvVar)),
            "DNS tests are disabled. See DEVELOPER.md for setup instructions.");
}

/// <summary>
/// Fixture that starts a cluster with <c>cluster-announce-hostname</c> set to a DNS name.
/// </summary>
public class DnsGetServersFixture : IAsyncLifetime
{
    public ClusterServer? Server { get; private set; }

    public ValueTask InitializeAsync()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("VALKEY_GLIDE_DNS_TESTS_ENABLED")))
        {
            Server = new(host: HostnameTls);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Server?.Dispose();
        return ValueTask.CompletedTask;
    }
}
