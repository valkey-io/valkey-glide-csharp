// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class TlsTests : IDisposable
{
    // Test constants
    static readonly string ServerName = "TLS_TEST_SERVER";
    static readonly string InvalidCertificatePath = "/INVALID_PATH/CERTIFICATE.crt";
    static readonly byte[] InvalidCertificateData = Encoding.UTF8.GetBytes("INVALID_CERTIFICATE");

    public void Dispose()
    {
        // Stop the server after each test.
        ServerManager.StopServer(ServerName);
    }

    [Fact]
    public async Task Cluster_InvalidCertificateData_Throws()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(InvalidCertificateData);

        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_InvalidCertificatePath_Throws()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(InvalidCertificatePath);

        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_NoCertificate_Throws()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Cluster_WithCertificateData_Success()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);
        var certData = File.ReadAllBytes(ServerManager.CaCertificatePath);
        configBuilder.WithTrustedCertificate(certData);

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_Success()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(ServerManager.CaCertificatePath);

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_InvalidCertificateData_Throws()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(InvalidCertificateData);

        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_InvalidCertificatePath_Throws()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(InvalidCertificatePath);

        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_NoCertificate_Throws()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);
        await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
    }

    [Fact]
    public async Task Standalone_WithCertificateData_Success()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);
        var certData = File.ReadAllBytes(ServerManager.CaCertificatePath);
        configBuilder.WithTrustedCertificate(certData);

        using var client = await GlideClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithCertificatePath_Success()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(ServerManager.CaCertificatePath);

        using var client = await GlideClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }
}
