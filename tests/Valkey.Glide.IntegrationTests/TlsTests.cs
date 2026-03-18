// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Security.Cryptography;
using System.Text;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Fixture class for TLS tests.
/// </summary>
public class TlsFixture : IDisposable
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

/// <summary>
/// Tests for client configuration and connection with TLS.
/// </summary>
public class TlsTests(TlsFixture fixture) : IClassFixture<TlsFixture>
{

    #region Cluster Tests

    [Fact]
    public async Task Cluster_WithCertificateData_NotTrusted_Throws()
    {
        var server = fixture.TlsClusterServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(GetUntrustedCertificateData());

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_Malformed_Throws()
    {
        var server = fixture.TlsClusterServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(GetMalformedCertificateData());

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Cluster_WithCertificatePath_NotFound_Throws()
    {
        var configBuilder = fixture.TlsClusterServer.CreateConfigBuilder();
        _ = Assert.Throws<FileNotFoundException>(()
            => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Cluster_NoCertificate_TlsServer_Throws()
    {
        var configBuilder = fixture.TlsClusterServer.CreateConfigBuilder();
        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificate_NonTlsServer_Throws()
    {
        var server = fixture.NonTlsClusterServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(fixture.TlsClusterServer.CertificateData!);

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_Trusted_Succeeds()
    {
        var server = fixture.TlsClusterServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(server.CertificateData!);

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_Trusted_Succeeds()
    {
        var server = fixture.TlsClusterServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(server.CertificatePath!);

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithInsecureTls_WithTlsServer_Succeeds()
    {
        var server = fixture.TlsClusterServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithInsecureTls();

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithInsecureTls_WithNonTlsServer_Throws()
    {
        var server = fixture.NonTlsClusterServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTls().WithInsecureTls();

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    #endregion
    #region Standalone Tests

    [Fact]
    public async Task Standalone_WithCertificateData_NotTrusted_Throws()
    {
        var server = fixture.TlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(GetUntrustedCertificateData());

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_Malformed_Throws()
    {
        var server = fixture.TlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(GetMalformedCertificateData());

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Standalone_WithCertificatePath_InvalidThrows()
    {
        var server = fixture.TlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder();

        _ = Assert.Throws<FileNotFoundException>(()
            => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Standalone_NoCertificate_Throws()
    {
        var server = fixture.TlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder();

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificate_NonTlsServer_Throws()
    {
        var server = fixture.NonTlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder()
         .WithTrustedCertificate(GetUntrustedCertificateData());

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_Trusted_Succeeds()
    {
        var server = fixture.TlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(server.CertificateData!);

        using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithCertificatePath_Trusted_Succeeds()
    {
        var server = fixture.TlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTrustedCertificate(server.CertificatePath!);

        using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithInsecureTls_WithTlsServer_Succeeds()
    {
        var server = fixture.TlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithInsecureTls();

        using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithInsecureTls_WithNonTlsServer_Throws()
    {
        var server = fixture.NonTlsStandaloneServer;
        var configBuilder = server.CreateConfigBuilder()
            .WithTls().WithInsecureTls();

        _ = await Assert.ThrowsAsync<ConnectionException>(async ()
            => await GlideClient.CreateClient(configBuilder.Build()));
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Returns unsupported certificate data.
    /// </summary>
    private static byte[] GetUntrustedCertificateData()
    {
        const string untrustedCertificateContent = """
        -----BEGIN CERTIFICATE-----
        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
        8XYH0IsipsyoJMGBmC1C8nlXd1jTYS2LWp3i7rg=
        -----END CERTIFICATE-----
        """;
        return Encoding.UTF8.GetBytes(untrustedCertificateContent);
    }

    /// <summary>
    /// Returns malformed certificate data.
    /// </summary>
    private static byte[] GetMalformedCertificateData()
    {
        var certificateData = GetUntrustedCertificateData();

        // Flip the first byte to corrupt the certificate data.
        certificateData[0] ^= 0xFF;

        return certificateData;
    }
}
