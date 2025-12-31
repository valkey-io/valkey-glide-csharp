// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;
using static Valkey.Glide.TestUtils.Server;

namespace Valkey.Glide.IntegrationTests;

public class TlsTests
{
    // Cluster TLS Tests
    // =================

    [Fact]
    public async Task Cluster_WithCertificateData_InvalidThrows()
    {
        using var server = new ClusterServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_MalformedThrows()
    {
        using var server = new ClusterServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(GetMalformedCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Cluster_WithCertificatePath_InvalidThrows()
    {
        using var server = new ClusterServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder();
        Assert.Throws<FileNotFoundException>(() => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Cluster_NoCertificate_Throws()
    {
        using var server = new ClusterServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder();
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificate_NoTlsThrows()
    {
        using var server = new ClusterServer(useTls: false);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_Success()
    {
        using var server = new ClusterServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(GetValidCertificateData());

        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_Success()
    {
        using var server = new ClusterServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(ServerCertificatePath);

        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithInsecureTls_Success()
    {
        using var server = new ClusterServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithInsecureTls();

        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithInsecureTls_NoTlsServerThrows()
    {
        using var server = new ClusterServer(useTls: false);
        var configBuilder = server.CreateConfigBuilder().WithTls().WithInsecureTls();
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    // Standalone TLS Tests
    // ====================

    [Fact]
    public async Task Standalone_WithCertificateData_InvalidThrows()
    {
        using var server = new StandaloneServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_MalformedThrows()
    {
        using var server = new StandaloneServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(GetMalformedCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Standalone_WithCertificatePath_InvalidThrows()
    {
        using var server = new StandaloneServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder();
        Assert.Throws<FileNotFoundException>(() => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Standalone_NoCertificate_Throws()
    {
        using var server = new StandaloneServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder();
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificate_NoTlsThrows()
    {
        using var server = new StandaloneServer(useTls: false);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_Success()
    {
        using var server = new StandaloneServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(GetValidCertificateData());

        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithCertificatePath_Success()
    {
        using var server = new StandaloneServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithTrustedCertificate(ServerCertificatePath);

        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithInsecureTls_Success()
    {
        using var server = new StandaloneServer(useTls: true);
        var configBuilder = server.CreateConfigBuilder().WithInsecureTls();

        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithInsecureTls_NoTlsServerThrows()
    {
        using var server = new StandaloneServer(useTls: false);
        var configBuilder = server.CreateConfigBuilder().WithTls().WithInsecureTls();
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    // Helper Methods
    // ==============

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
