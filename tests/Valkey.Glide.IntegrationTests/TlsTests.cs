// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class TlsTests : IDisposable
{
    // Server name and certificate paths for the tests.
    // The cluster manager script generates a CA certificate at the specified path.
    // See 'valkey-glide/utils/cluster_manager.py' for more details.
    private static readonly string ServerName = $"TlsTests_{Guid.NewGuid():N}";
    private static readonly string CertificatePath = ServerManager.CaCertificatePath;

    private static readonly string InvalidCertificateContent = "INVALID_CERTIFICATE";
    private static readonly byte[] InvalidCertificateData = Encoding.UTF8.GetBytes(InvalidCertificateContent);

    public void Dispose()
    {
        ServerManager.StopServer(ServerName);
    }

    [Fact]
    public async Task Cluster_WithCertificateData_InvalidThrows()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(InvalidCertificateData);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
        Assert.Contains("invalid peer certificate", ex.Message);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_InvalidThrows()
    {
        using var invalidCertificateFile = new TempFile(InvalidCertificateData);

        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(invalidCertificateFile.Path);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
        Assert.Contains("invalid peer certificate", ex.Message);
    }

    [Fact]
    public async Task Cluster_NoCertificate_Throws()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
        Assert.Contains("invalid peer certificate", ex.Message);
    }

    [Fact]
    public async Task Cluster_WithCertificateData_NoTlsThrows()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: false);
        configBuilder.WithTrustedCertificate(InvalidCertificateData);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
        Assert.Contains("TLS is disabled", ex.Message);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_NoTlsThrows()
    {
        using var invalidCertificateFile = new TempFile(InvalidCertificateData);

        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: false);
        configBuilder.WithTrustedCertificate(invalidCertificateFile.Path);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClusterClient.CreateClient(configBuilder.Build()));
        Assert.Contains("TLS is disabled", ex.Message);
    }

    [Fact]
    public async Task Cluster_WithCertificateData_Success()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);

        var certData = File.ReadAllBytes(CertificatePath);
        configBuilder.WithTrustedCertificate(certData);

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task Cluster_WithCertificatePath_Success()
    {
        var configBuilder = ServerManager.StartClusterServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(CertificatePath);

        using var client = await GlideClusterClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    [Fact]
    public async Task Standalone_WithCertificateData_InvalidThrows()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(InvalidCertificateData);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
        Assert.Contains("invalid peer certificate", ex.Message);
    }

    [Fact]
    public async Task Standalone_WithCertificatePath_InvalidThrows()
    {
        var invalidCertificateFile = new TempFile(InvalidCertificateData);

        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);
        configBuilder.WithTrustedCertificate(invalidCertificateFile.Path);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
        Assert.Contains("invalid peer certificate", ex.Message);
    }

    [Fact]
    public async Task Standalone_NoCertificate_Throws()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
        Assert.Contains("invalid peer certificate", ex.Message);
    }

    [Fact]
    public async Task Standalone_WithCertificateData_NoTlsThrows()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: false);
        configBuilder.WithTrustedCertificate(InvalidCertificateData);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
        Assert.Contains("TLS is disabled", ex.Message);
    }

    [Fact]
    public async Task Standalone_WithCertificatePath_NoTlsThrows()
    {
        var invalidCertificateFile = new TempFile(InvalidCertificateData);

        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: false);
        configBuilder.WithTrustedCertificate(invalidCertificateFile.Path);

        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await GlideClient.CreateClient(configBuilder.Build()));
        Assert.Contains("TLS is disabled", ex.Message);
    }

    [Fact]
    public async Task Standalone_WithCertificateData_Success()
    {
        var configBuilder = ServerManager.StartStandaloneServer(ServerName, useTls: true);

        var certData = File.ReadAllBytes(CertificatePath);
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
