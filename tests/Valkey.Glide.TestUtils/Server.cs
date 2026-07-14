// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Base class for a Valkey server.
/// </summary>
public abstract class Server : IDisposable
{
    #region Constants

    /// <summary>
    /// Custom command arguments to kill all normal clients.
    /// </summary>
    protected static readonly GlideString[] KillClientArgs = ["CLIENT", "KILL", "TYPE", "NORMAL"];

    /// <summary>
    /// Maximum time to wait for the initial client connection to succeed.
    /// </summary>
    /// <remarks>
    /// On Windows CI (WSL), cluster topology formation can take several seconds after the server
    /// process starts.
    /// </remarks>
    private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Delay between initial connection attempts.
    /// </summary>
    private static readonly TimeSpan ConnectionRetryDelay = TimeSpan.FromMilliseconds(500);

    #endregion
    #region Fields

    /// <summary>
    /// Name of the server.
    /// </summary>
    private readonly string _name = $"Server_{Guid.NewGuid():N}";

    /// <summary>
    /// Whether the server has been stopped.
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
    /// Address of the server.
    /// </summary>
    public Address Address { get; init; }

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
    /// <param name="replicaCount">Number of replicas per primary, or <see langword="null"/> to use the default.</param>
    protected Server(
        bool useClusterMode,
        bool useTls = false,
        int? replicaCount = null)
    {
        UseTls = useTls;

        Address = ServerManager.StartServer(
            name: _name,
            useClusterMode: useClusterMode,
            useTls: UseTls,
            replicaCount: replicaCount).First();

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
    #region Protected Methods

    /// <summary>
    /// Creates a client using the given factory.
    /// </summary>
    /// <typeparam name="T">The client type produced by the factory.</typeparam>
    /// <param name="factory">Factory that builds and connects a client.</param>
    /// <returns>The connected client.</returns>
    /// <exception cref="ConnectionException">
    /// Thrown when a connection could not be established after all attempts.
    /// </exception>
    protected static async Task<T> CreateClientAsync<T>(Func<Task<T>> factory)
        where T : BaseClient
    {
        ConnectionException? lastException = null;

        using CancellationTokenSource cts = new(ConnectionTimeout);
        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                return await factory();
            }
            catch (ConnectionException ex)
            {
                lastException = ex;
                await Task.Delay(ConnectionRetryDelay);
            }
        }

        throw lastException!;
    }

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
        => Config.BuildClusterConfig(
            address: Address,
            useTls: UseTls,
            trustedCertificate: UseTls ? CertificateData : null,
            password: _password);

    /// <inheritdoc cref="Server.CreateClientAsync()"/>
    public override async Task<BaseClient> CreateClientAsync()
        => await CreateClusterClientAsync();

    /// <summary>
    /// Builds and returns a cluster client for this server.
    /// </summary>
    public async Task<GlideClusterClient> CreateClusterClientAsync()
    {
        var config = CreateConfigBuilder().Build();
        Task<GlideClusterClient> factory() => GlideClusterClient.CreateClient(config);
        return await CreateClientAsync(factory);
    }

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
public sealed class StandaloneServer(
    bool useTls = false,
    int? replicaCount = null) : Server(useClusterMode: false, useTls: useTls, replicaCount: replicaCount)
{
    #region Public Methods

    /// <summary>
    /// Builds and returns a standalone client configuration builder for this server.
    /// </summary>
    public StandaloneClientConfigurationBuilder CreateConfigBuilder()
        => Config.BuildStandaloneConfig(
            address: Address,
            useTls: UseTls,
            trustedCertificate: UseTls ? CertificateData : null,
            password: _password);

    /// <inheritdoc cref="Server.CreateClientAsync()"/>
    public override async Task<BaseClient> CreateClientAsync()
        => await CreateStandaloneClientAsync();

    /// <summary>
    /// Builds and returns a standalone client for this server.
    /// </summary>
    public async Task<GlideClient> CreateStandaloneClientAsync()
    {
        var config = CreateConfigBuilder().Build();
        Task<GlideClient> factory() => GlideClient.CreateClient(config);
        return await CreateClientAsync(factory);
    }

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
