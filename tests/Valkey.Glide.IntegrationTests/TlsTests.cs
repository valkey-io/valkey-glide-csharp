// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests;

[Collection("GlideTests")]
public class TlsTests : IDisposable
{
    static readonly string ServerName = "tls-test-server";

    private readonly string _host;
    private readonly ushort _port;
    private readonly string _certificatePath;

    public TlsTests()
    {
        var addresses = ServerManager.StartStandaloneServer(ServerName, useTls: true);
        _host = addresses[0].host;
        _port = addresses[0].port;
        _certificatePath = ServerManager.CaCertificatePath;
    }

    public void Dispose()
    {
        ServerManager.StopServer(ServerName);
    }

    [Fact]
    public async Task TlsWithCaSignedCertificate_Standalone_Success()
    {
        var configBuilder = GetStandaloneConfigBuilder();
        configBuilder.WithTrustedCertificate(_certificatePath);

        await using var client = await GlideClient.CreateClient(configBuilder.Build());
        await ServerManager.AssertConnected(client);
    }

    private StandaloneClientConfigurationBuilder GetStandaloneConfigBuilder()
    {
        return new StandaloneClientConfigurationBuilder()
            .WithAddress(_host, _port)
            .WithTls(true);
    }
}
