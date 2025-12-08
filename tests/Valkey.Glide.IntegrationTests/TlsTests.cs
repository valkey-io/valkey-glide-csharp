// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

using static Valkey.Glide.Errors;
using static Valkey.Glide.TestUtils.Client;
using static Valkey.Glide.TestUtils.Server;

namespace Valkey.Glide.IntegrationTests;

public class TlsTests : IDisposable
{
    private static readonly string ServerName = $"TlsTests_{Guid.NewGuid():N}";

    public void Dispose()
    {
        StopServer(ServerName);
    }

    // Cluster TLS Tests
    // =================

    [Fact]
    public async Task Cluster_WithCertificateData_InvalidThrows()
    {
        var configBuilder = StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_MalformedThrows()
    {
        var configBuilder = StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(GetMalformedCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Cluster_WithCertificatePath_InvalidThrows()
    {
        var configBuilder = StartClusterServer(ServerName, useTls: true);
        Assert.Throws<FileNotFoundException>(() => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Cluster_NoCertificate_Throws()
    {
        var configBuilder = StartClusterServer(ServerName, useTls: true);
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificate_NoTlsThrows()
    {
        var configBuilder = StartClusterServer(ServerName, useTls: false);
        configBuilder.WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_Success()
    {
        var configBuilder = StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(GetValidCertificateData());

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_Success()
    {
        var configBuilder = StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(ServerCertificatePath);

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }


    // Standalone TLS Tests
    // ====================

    [Fact]
    public async Task Standalone_WithCertificateData_InvalidThrows()
    {
        var configBuilder = StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_MalformedThrows()
    {
        var configBuilder = StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(GetMalformedCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public void Standalone_WithCertificatePath_InvalidThrows()
    {
        var configBuilder = StartStandaloneServer(ServerName, useTls: true);
        Assert.Throws<FileNotFoundException>(() => configBuilder.WithTrustedCertificate("invalid/path/to/ca.crt"));
    }

    [Fact]
    public async Task Standalone_NoCertificate_Throws()
    {
        var configBuilder = StartStandaloneServer(ServerName, useTls: true);
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificate_NoTlsThrows()
    {
        var configBuilder = StartStandaloneServer(ServerName, useTls: false);
        configBuilder.WithTrustedCertificate(GetInvalidCertificateData());
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_Success()
    {
        var configBuilder = StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(GetValidCertificateData());

        using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithCertificatePath_Success()
    {
        var configBuilder = StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(ServerCertificatePath);

        using var client = await GlideClient.CreateClient(configBuilder.Build());
        await AssertConnected(client);
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
