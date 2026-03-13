// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Fixture class to manage server lifecycle for TLS tests.
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

public class TlsTests(TlsFixture fixture) : IClassFixture<TlsFixture>
{
    private readonly TlsFixture _fixture = fixture;
    private static readonly string ServerCertificatePath = ServerManager.ServerCertificatePath;

    // Cluster TLS Tests
    // -----------------

    [Fact]
    public async Task Cluster_WithCertificateData_InvalidThrows()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_MalformedThrows()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(GetMalformedCertificateData());
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Cluster_WithCertificatePath_InvalidThrows()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder();
        _ = Assert.Throws<FileNotFoundException>(() => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Cluster_NoCertificate_Throws()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder();
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificate_NoTlsThrows()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.NonTlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_Success()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(GetValidCertificateData());
        await using GlideClusterClient client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_Success()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithTrustedCertificate(ServerCertificatePath);
        await using GlideClusterClient client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithInsecureTls_Success()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.TlsClusterServer.CreateConfigBuilder().WithInsecureTls();
        await using GlideClusterClient client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithInsecureTls_NoTlsServerThrows()
    {
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = _fixture.NonTlsClusterServer.CreateConfigBuilder().WithTls().WithInsecureTls();
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    // Standalone TLS Tests
    // --------------------

    [Fact]
    public async Task Standalone_WithCertificateData_InvalidThrows()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_MalformedThrows()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(GetMalformedCertificateData());
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Standalone_WithCertificatePath_InvalidThrows()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder();
        _ = Assert.Throws<FileNotFoundException>(() => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Standalone_NoCertificate_Throws()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder();
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificate_NoTlsThrows()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.NonTlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_Success()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(GetValidCertificateData());
        await using GlideClient client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithCertificatePath_Success()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithTrustedCertificate(ServerCertificatePath);
        await using GlideClient client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithInsecureTls_Success()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.TlsStandaloneServer.CreateConfigBuilder().WithInsecureTls();
        await using GlideClient client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithInsecureTls_NoTlsServerThrows()
    {
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = _fixture.NonTlsStandaloneServer.CreateConfigBuilder().WithTls().WithInsecureTls();
        _ = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    // Helper Methods
    // --------------

    /// <summary>
    /// Returns valid certificate data.
    /// The server must have been initialized prior to calling this method.
    /// </summary>
    private static byte[] GetValidCertificateData() => File.ReadAllBytes(ServerCertificatePath);

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
        byte[] certificateData = GetValidCertificateData();

        // Flip the first byte to corrupt the certificate data.
        certificateData[0] ^= 0xFF;

        return certificateData;
    }
}
