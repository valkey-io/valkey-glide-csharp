// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Base class for a Valkey server.
/// </summary>
public abstract class Server : IDisposable
{
    /// <summary>
    /// Name of the server.
    /// </summary>
    private string _name = $"Server_{Guid.NewGuid():N}";

    /// <summary>
    /// Addresses of the server instances.
    /// </summary>
    public IList<Address> Addresses { get; private set; }

    /// <summary>
    /// Indicates whether the server uses TLS.
    /// </summary>
    protected bool UseTls = false;

    /// <summary>
    /// Certificate data path for the server.
    /// </summary>
    public string? CertificatePath { get; private set; } = null;

    /// <summary>
    /// Certificate data for the server.
    /// </summary>
    public byte[]? CertificateData { get; private set; } = null;

    /// <summary>
    /// Indicates whether the server has been stopped.
    /// </summary>
    private bool _disposed = false;

    protected Server(bool useClusterMode, bool useTls)
    {
        Addresses = ServerManager.StartServer(_name, useClusterMode: useClusterMode, useTls: useTls);

        if (useTls)
        {
            UseTls = true;
            CertificatePath = ServerManager.ServerCertificatePath;
            CertificateData = File.ReadAllBytes(CertificatePath);
        }
    }

    ~Server() => Dispose();

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        ServerManager.StopServer(_name);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Builds and returns a client for this server.
    /// </summary>
    public abstract Task<BaseClient> CreateClientAsync();
}

/// <summary>
/// Valkey cluster server.
/// </summary>
public sealed class ClusterServer : Server
{
    public ClusterServer(bool useTls = false) : base(useClusterMode: true, useTls: useTls) { }

    /// <summary>
    /// Builds and returns a cluster client configuration builder for this server.
    /// </summary>
    public ClusterClientConfigurationBuilder CreateConfigBuilder()
    {
        var configBuilder = new ClusterClientConfigurationBuilder();
        configBuilder.WithTls(useTls: UseTls);

        foreach (var (host, port) in Addresses)
            configBuilder.WithAddress(host, port);

        return configBuilder;
    }

    /// <inheritdoc/>
    public override async Task<BaseClient> CreateClientAsync()
        => await CreateClusterClientAsync();

    /// <summary>
    /// Builds and returns a cluster client for this server.
    /// </summary>
    public async Task<GlideClusterClient> CreateClusterClientAsync()
        => await GlideClusterClient.CreateClient(CreateConfigBuilder().Build());
}

/// <summary>
/// Valkey standalone server.
/// </summary>
public sealed class StandaloneServer : Server
{
    public StandaloneServer(bool useTls = false) : base(useClusterMode: false, useTls: useTls) { }

    /// <summary>
    /// Builds and returns a standalone client configuration builder for this server.
    /// </summary>
    public StandaloneClientConfigurationBuilder CreateConfigBuilder()
    {
        var configBuilder = new StandaloneClientConfigurationBuilder();
        configBuilder.WithTls(useTls: UseTls);

        foreach (var (host, port) in Addresses)
            configBuilder.WithAddress(host, port);

        return configBuilder;
    }

    /// <inheritdoc/>
    public override async Task<BaseClient> CreateClientAsync()
        => await CreateStandaloneClientAsync();

    /// <summary>
    /// Builds and returns a standalone client for this server.
    /// </summary>
    public async Task<GlideClient> CreateStandaloneClientAsync()
        => await GlideClient.CreateClient(CreateConfigBuilder().Build());
}
