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
    /// Indicates whether the server has been stopped.
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// Indicates whether the server uses TLS.
    /// </summary>
    protected bool _useTls;

    /// <summary>
    /// Addresses of the server instances.
    /// </summary>
    protected IList<Address> _addresses;

    protected Server(bool useClusterMode, bool useTls)
    {
        _useTls = useTls;
        _addresses = ServerManager.StartServer(_name, useClusterMode: useClusterMode, useTls: _useTls);
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
    /// Builds and returns a client configuration builder for this Valkey server.
    /// </summary>
    public abstract object CreateConfigBuilder();

    /// <summary>
    /// Builds and returns a client for this Valkey server.
    /// </summary>
    public abstract Task<BaseClient> CreateClient();
}

/// <summary>
/// Valkey cluster server.
/// </summary>
public sealed class ClusterServer : Server
{
    public ClusterServer(bool useTls = false) : base(useClusterMode: true, useTls: useTls) { }

    /// <inheritdoc/>
    public override ClusterClientConfigurationBuilder CreateConfigBuilder()
    {
        var configBuilder = new ClusterClientConfigurationBuilder();
        configBuilder.WithTls(useTls: _useTls);

        foreach (var (host, port) in _addresses)
            configBuilder.WithAddress(host, port);

        return configBuilder;
    }

    /// <inheritdoc/>
    public override async Task<BaseClient> CreateClient()
        => await CreateClusterClient();

    /// <summary>
    /// Builds and returns a cluster client for this Valkey server.
    /// </summary>
    public async Task<GlideClusterClient> CreateClusterClient()
        => await GlideClusterClient.CreateClient(CreateConfigBuilder().Build());
}

/// <summary>
/// Valkey standalone server.
/// </summary>
public sealed class StandaloneServer : Server
{
    public StandaloneServer(bool useTls = false) : base(useClusterMode: false, useTls: useTls) { }

    /// <inheritdoc/>
    public override StandaloneClientConfigurationBuilder CreateConfigBuilder()
    {
        var configBuilder = new StandaloneClientConfigurationBuilder();
        configBuilder.WithTls(useTls: _useTls);

        foreach (var (host, port) in _addresses)
            configBuilder.WithAddress(host, port);

        return configBuilder;
    }

    /// <inheritdoc/>
    public override async Task<BaseClient> CreateClient()
        => await CreateStandaloneClient();

    /// <summary>
    /// Builds and returns a standalone client for this Valkey server.
    /// </summary>
    public async Task<GlideClient> CreateStandaloneClient()
        => await GlideClient.CreateClient(CreateConfigBuilder().Build());
}
