// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

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
        var configBuilder = StartStandaloneServer();
        configBuilder.WithTrustedCertificate(ServerManager.CaCertificatePath);

        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task WithTrustedCertificate_FromPath_Cluster_Success()
    {
        var configBuilder = StartClusterServer();
        configBuilder.WithTrustedCertificate(ServerManager.CaCertificatePath);

        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task WithTrustedCertificate_FromBytes_Standalone_Success()
    {
        var configBuilder = StartStandaloneServer();
        var certBytes = File.ReadAllBytes(ServerManager.CaCertificatePath);
        configBuilder.WithTrustedCertificate(certBytes);

        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task WithTrustedCertificate_FromBytes_Cluster_Success()
    {
        var configBuilder = StartClusterServer();
        var certBytes = File.ReadAllBytes(ServerManager.CaCertificatePath);
        configBuilder.WithTrustedCertificate(certBytes);

        await using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    /// <summary>
    /// Starts a standalone server with TLS enabled and returns a configuration builder for it.
    /// </summary>
    private StandaloneClientConfigurationBuilder StartStandaloneServer()
    {
        var addresses = ServerManager.StartStandaloneServer(ServerName, useTls: true);

        var configBuilder = new StandaloneClientConfigurationBuilder();
        configBuilder.WithAddress(addresses[0].host, addresses[0].port);
        configBuilder.WithTls(true);

        return configBuilder;
    }

    /// <summary>
    /// Starts a cluster server with TLS enabled and returns a configuration builder for it.
    /// </summary>
    private ClusterClientConfigurationBuilder StartClusterServer()
    {
        var addresses = ServerManager.StartClusterServer(ServerName, useTls: true);

        var configBuilder = new ClusterClientConfigurationBuilder();
        configBuilder.WithAddress(addresses[0].host, addresses[0].port);
        configBuilder.WithTls(true);

        return configBuilder;
    }
}
