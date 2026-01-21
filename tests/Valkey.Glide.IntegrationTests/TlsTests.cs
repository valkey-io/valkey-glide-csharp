// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Fixture class to manage server lifecycle for TLS tests.
/// </summary>
public class TlsServersFixture : IDisposable
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

public class TlsTests(TlsServersFixture fixture) : IClassFixture<TlsServersFixture>
{
    private readonly TlsServersFixture _fixture = fixture;
    private static readonly string ServerCertificatePath = ServerManager.ServerCertificatePath;

    // Cluster TLS Tests
    // -----------------

    [Fact]
    public async Task Cluster_WithCertificateData_InvalidThrows()
    {
        var configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_MalformedThrows()
    {
        var configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(GetMalformedCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Cluster_WithCertificatePath_InvalidThrows()
    {
        var configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder();
        Assert.Throws<FileNotFoundException>(() => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Cluster_NoCertificate_Throws()
    {
        var configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder();
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificate_NoTlsThrows()
    {
        var configBuilder = _fixture.NonTlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_Success()
    {
        var configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(GetValidCertificateData());
        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_Success()
    {
        var configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(ServerCertificatePath);
        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithInsecureTls_Success()
    {
        var configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithInsecureTls();
        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithInsecureTls_NoTlsServerThrows()
    {
        var configBuilder = _fixture.NonTlsClusterServer.CreateConfigBuilder().WithTls().WithInsecureTls();
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    // Standalone TLS Tests
    // --------------------

    [Fact]
    public async Task Standalone_WithCertificateData_InvalidThrows()
    {
        var configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_MalformedThrows()
    {
        var configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(GetMalformedCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Standalone_WithCertificatePath_InvalidThrows()
    {
        var configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder();
        Assert.Throws<FileNotFoundException>(() => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Standalone_NoCertificate_Throws()
    {
        var configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder();
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificate_NoTlsThrows()
    {
        var configBuilder = _fixture.NonTlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_Success()
    {
        var configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(GetValidCertificateData());
        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithCertificatePath_Success()
    {
        var configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(ServerCertificatePath);
        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithInsecureTls_Success()
    {
        var configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithInsecureTls();
        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithInsecureTls_NoTlsServerThrows()
    {
        var configBuilder = _fixture.NonTlsStandaloneServer.CreateConfigBuilder().WithTls().WithInsecureTls();
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    // Helper Methods
    // --------------

    /// <summary>
    /// Returns valid certificate data.
    /// The server must have been initialized prior to calling this method.
    /// </summary>
    private static byte[] GetValidCertificateData()
    {
        return File.ReadAllBytes(ServerCertificatePath);
    }

    /// <summary>
    /// Returns invalid certificate data.
    /// </summary>
    private static byte[] GetInvalidCertificateData()
    {
        const string invalidCertificateContent = """
        -----BEGIN CERTIFICATE-----
        INVALID+CERTIFICATE+DATA
        -----END CERTIFICATE-----
        """;
        return Encoding.UTF8.GetBytes(invalidCertificateContent);
    }

    /// <summary>
    /// Returns malformed certificate data.
    /// The server must have been initialized prior to calling this method.
    /// </summary>
    private static byte[] GetMalformedCertificateData()
    {
        var certificateData = GetValidCertificateData();

        // Flip the first byte to corrupt the certificate data.
        certificateData[0] ^= 0xFF;

        return certificateData;
    }
}
