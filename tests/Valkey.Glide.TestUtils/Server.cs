// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Base class for a Valkey server.
/// </summary>
public abstract class Server : IDisposable
{
    #region PrivateFields

    /// <summary>
    /// Name of the server.
    /// </summary>
    private readonly string _name = $"Server_{Guid.NewGuid():N}";

    #endregion
    #region ProtectedFields

    /// <summary>
    /// Indicates whether the server has been stopped.
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// Indicates whether the server uses TLS.
    /// </summary>
    protected bool _useTls;

    /// <summary>
    /// Password for the server.
    /// </summary>
    protected string? _password;

    /// <summary>
    /// Addresses of the server instances.
    /// </summary>
    protected IList<Address> _addresses;

    #endregion

    protected Server(bool useClusterMode, bool useTls)
    {
        _useTls = useTls;
        _addresses = ServerManager.StartServer(_name, useClusterMode: useClusterMode, useTls: _useTls);
    }

    ~Server() => Dispose();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        ServerManager.StopServer(_name);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Builds and returns a client for this Valkey server.
    /// </summary>
    public abstract Task<BaseClient> CreateClientAsync();

    /// <summary>
    /// Updates the password for the server to the given value.
    /// </summary>
    /// <param name="password">The new server password.</param>
    public abstract Task SetPasswordAsync(string password);

    /// <summary>
    /// Clears the password for the server.
    /// </summary>
    public abstract Task ClearPasswordAsync();

    /// <summary>
    /// Kill all normal clients on the server.
    /// </summary>
    public abstract Task KillClientAsync();
}

/// <summary>
/// Valkey cluster server.
/// </summary>
public sealed class ClusterServer(bool useTls = false) : Server(useClusterMode: true, useTls: useTls)
{
    /// <summary>
    /// Builds and returns a cluster client configuration builder for this Valkey server.
    /// </summary>
    public ClusterClientConfigurationBuilder CreateConfigBuilder()
    {
        ClusterClientConfigurationBuilder configBuilder = new();
        _ = configBuilder.WithTls(useTls: _useTls);

        if (_password is not null)
        {
            _ = configBuilder.WithAuthentication(_password);
        }

        foreach ((string host, ushort port) in _addresses)
        {
            _ = configBuilder.WithAddress(host, port);
        }

        return configBuilder;
    }

    /// <inheritdoc/>
    public override async Task<BaseClient> CreateClientAsync()
        => await CreateClusterClientAsync();

    /// <summary>
    /// Builds and returns a cluster client for this Valkey server.
    /// </summary>
    public async Task<GlideClusterClient> CreateClusterClientAsync()
        => await GlideClusterClient.CreateClient(CreateConfigBuilder().Build());

    public override async Task SetPasswordAsync(string password)
    {
        using GlideClusterClient client = await CreateClusterClientAsync();
        await client.ConfigSetAsync("requirepass", password, Route.AllNodes);
        _password = password;
    }

    public override async Task ClearPasswordAsync()
    {
        using GlideClusterClient client = await CreateClusterClientAsync();
        await client.ConfigSetAsync("requirepass", "", Route.AllNodes);
        _password = null;
    }

    public override async Task KillClientAsync()
    {
        using GlideClusterClient client = await CreateClusterClientAsync();
        _ = await client.CustomCommand(["CLIENT", "KILL", "TYPE", "NORMAL"]);
    }
}

/// <summary>
/// Valkey standalone server.
/// </summary>
public sealed class StandaloneServer(bool useTls = false) : Server(useClusterMode: false, useTls: useTls)
{

    /// <summary>
    /// Builds and returns a standalone client configuration builder for this Valkey server.
    /// </summary>
    public StandaloneClientConfigurationBuilder CreateConfigBuilder()
    {
        StandaloneClientConfigurationBuilder configBuilder = new();
        _ = configBuilder.WithTls(useTls: _useTls);

        if (_password is not null)
        {
            _ = configBuilder.WithAuthentication(_password);
        }

        foreach ((string host, ushort port) in _addresses)
        {
            _ = configBuilder.WithAddress(host, port);
        }

        return configBuilder;
    }

    /// <inheritdoc/>
    public override async Task<BaseClient> CreateClientAsync()
        => await CreateStandaloneClientAsync();

    /// <summary>
    /// Builds and returns a standalone client for this Valkey server.
    /// </summary>
    public async Task<GlideClient> CreateStandaloneClientAsync()
        => await GlideClient.CreateClient(CreateConfigBuilder().Build());

    public override async Task SetPasswordAsync(string password)
    {
        using GlideClient client = await CreateStandaloneClientAsync();
        await client.ConfigSetAsync("requirepass", password);
        _password = password;
    }

    public override async Task ClearPasswordAsync()
    {
        using GlideClient client = await CreateStandaloneClientAsync();
        await client.ConfigSetAsync("requirepass", "");
        _password = null;
    }

    public override async Task KillClientAsync()
    {
        using GlideClient client = await CreateStandaloneClientAsync();
        _ = await client.CustomCommand(["CLIENT", "KILL", "TYPE", "NORMAL"]);
    }
}
