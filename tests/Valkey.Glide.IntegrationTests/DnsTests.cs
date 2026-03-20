// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
            var port = server.Addresses.First().Port;

            var builder = new ClusterClientConfigurationBuilder()
                .WithAddress(host, port);

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
            var port = server.Addresses.First().Port;

            var builder = new StandaloneClientConfigurationBuilder()
                .WithAddress(host, port);

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
public class DnsTestsFixture : IDisposable
{
    public ClusterServer? ClusterServer { get; }
    public StandaloneServer? StandaloneServer { get; }
    public ClusterServer? TlsClusterServer { get; }
    public StandaloneServer? TlsStandaloneServer { get; }

    public DnsTestsFixture()
    {
        // Only start the servers if DNS tests are enabled.
        if (DnsTests.IsDnsTestsEnabled())
        {
            ClusterServer = new(useTls: false);
            StandaloneServer = new(useTls: false);
            TlsClusterServer = new(useTls: true);
            TlsStandaloneServer = new(useTls: true);
        }
    }

    public void Dispose()
    {
        ClusterServer?.Dispose();
        StandaloneServer?.Dispose();
        TlsClusterServer?.Dispose();
        TlsStandaloneServer?.Dispose();
    }
}

