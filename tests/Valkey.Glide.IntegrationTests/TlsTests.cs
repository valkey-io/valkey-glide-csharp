// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

[Collection("GlideTests")]
public class TlsTests : IDisposable
{
    static readonly string ServerName = "tls-test-server";

    public void Dispose()
    {
        ServerManager.StopServer(ServerName);
    }

    [Fact]
    public async Task WithTrustedCertificate_FromPath_Standalone_Success()
    {
        var configBuilder = ServerManager.GetStandaloneServerConfig(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(ServerManager.CaCertificatePath);

        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task WithTrustedCertificate_FromPath_Cluster_Success()
    {
        var configBuilder = ServerManager.GetClusterServerConfig(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(ServerManager.CaCertificatePath);

        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task WithTrustedCertificate_FromBytes_Standalone_Success()
    {
        var certBytes = File.ReadAllBytes(ServerManager.CaCertificatePath);

        var configBuilder = ServerManager.GetStandaloneServerConfig(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(certBytes);

        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task WithTrustedCertificate_FromBytes_Cluster_Success()
    {
        var certBytes = File.ReadAllBytes(ServerManager.CaCertificatePath);

        var configBuilder = ServerManager.GetClusterServerConfig(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(certBytes);

        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }
}
