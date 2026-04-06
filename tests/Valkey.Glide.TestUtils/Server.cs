// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Base class for a Valkey server.
/// </summary>
public abstract class Server : IDisposable
{
    #region Constants

    /// <summary>
    /// Timeout for client connection and reconnection attempts.
    /// Use a longer timeout to allow for slower connections in CI environments.
    /// </summary>
    protected static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Custom command arguments to kill all normal clients.
    /// </summary>
    protected static readonly GlideString[] KillClientArgs = ["CLIENT", "KILL", "TYPE", "NORMAL"];

    #endregion
    #region Fields

    /// <summary>
    /// Name of the server.
    /// </summary>
    private readonly string _name = $"Server_{Guid.NewGuid():N}";

    /// <summary>
    /// Indicates whether the server has been stopped.
    /// See <see cref="Dispose" />.
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// Password for the server.
    /// </summary>
    protected string? _password;

    #endregion
    #region Public Properties

    /// <summary>
    /// Addresses of the server instances.
    /// </summary>
    public IList<Address> Addresses { get; init; }

    /// <summary>
    /// Indicates whether the server uses TLS.
    /// </summary>
    public bool UseTls { get; init; }

    /// <summary>
    /// Certificate data path for the server.
    /// </summary>
    public string? CertificatePath { get; private set; }

    /// <summary>
    /// Certificate data for the server.
    /// </summary>
    public byte[]? CertificateData { get; private set; }

    #endregion
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="useClusterMode">Whether to start in cluster mode.</param>
    /// <param name="useTls">Whether to enable TLS.</param>
    protected Server(bool useClusterMode, bool useTls)
    {
        UseTls = useTls;
        Addresses = ServerManager.StartServer(_name, useClusterMode: useClusterMode, useTls: UseTls);

        if (UseTls)
        {
            CertificatePath = ServerManager.ServerCertificatePath;
            CertificateData = File.ReadAllBytes(CertificatePath);
        }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~Server() => Dispose();

    #endregion
    #region Public Methods

    /// <summary>
    /// Stops the server.
    /// </summary>
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
    /// Builds and returns a client for this server.
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
    public abstract Task KillClientsAsync();

    #endregion
}

/// <summary>
/// Valkey cluster server.
/// </summary>
public sealed class ClusterServer(bool useTls = false) : Server(useClusterMode: true, useTls: useTls)
{
    #region Public Methods

    /// <summary>
    /// Builds and returns a cluster client configuration builder for this server.
    /// </summary>
    public ClusterClientConfigurationBuilder CreateConfigBuilder()
    {
        ClusterClientConfigurationBuilder configBuilder = new()
        {
            UseTls = UseTls,
            ConnectionTimeout = ConnectionTimeout
        };

        if (UseTls)
        {
            _ = configBuilder.WithTrustedCertificate(CertificateData!);
        }

        if (_password is not null)
        {
            _ = configBuilder.WithAuthentication(_password);
        }

        foreach ((string host, ushort port) in Addresses)
        {
            _ = configBuilder.WithAddress(host, port);
        }

        return configBuilder;
    }

    public override async Task<BaseClient> CreateClientAsync()
        => await CreateClusterClientAsync();

    /// <summary>
    /// Builds and returns a cluster client for this server.
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

    public override async Task KillClientsAsync()
    {
        using GlideClusterClient client = await CreateClusterClientAsync();
        _ = await client.CustomCommand(KillClientArgs);
    }

    #endregion
}

/// <summary>
/// Valkey standalone server.
/// </summary>
public sealed class StandaloneServer(bool useTls = false) : Server(useClusterMode: false, useTls: useTls)
{
    #region Public Methods

    /// <summary>
    /// Builds and returns a standalone client configuration builder for this server.
    /// </summary>
    public StandaloneClientConfigurationBuilder CreateConfigBuilder()
    {
        StandaloneClientConfigurationBuilder configBuilder = new()
        {
            UseTls = UseTls,
            ConnectionTimeout = ConnectionTimeout
        };

        if (UseTls)
        {
            _ = configBuilder.WithTrustedCertificate(CertificateData!);
        }

        if (_password is not null)
        {
            _ = configBuilder.WithAuthentication(_password);
        }

        foreach ((string host, ushort port) in Addresses)
        {
            _ = configBuilder.WithAddress(host, port);
        }

        return configBuilder;
    }

    public override async Task<BaseClient> CreateClientAsync()
        => await CreateStandaloneClientAsync();

    /// <summary>
    /// Builds and returns a standalone client for this server.
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

    public override async Task KillClientsAsync()
    {
        using GlideClient client = await CreateStandaloneClientAsync();
        _ = await client.CustomCommand(KillClientArgs);
    }

    #endregion
}
