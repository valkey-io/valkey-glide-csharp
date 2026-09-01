// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

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
    /// Maximum time to wait for the initial client connection to succeed.
    /// </summary>
    /// <remarks>
    /// On Windows CI (WSL), cluster topology formation can take several seconds after the server
    /// process starts.
    /// </remarks>
    private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(30);

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
    /// Username for the server.
    /// </summary>
    protected string? _username;

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
    public string? CertificatePath { get; }

    /// <summary>
    /// Certificate data for the server.
    /// </summary>
    public byte[]? CertificateData { get; }

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
        int? replicaCount = null,
        string? host = null)
    {
        UseTls = useTls;

        Address = ServerManager.StartServer(
            name: _name,
            useClusterMode: useClusterMode,
            useTls: UseTls,
            replicaCount: replicaCount,
            host: host)
            .First();

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
    /// <param name="host">Optional hostname.</param>
    public abstract Task<BaseClient> CreateClientAsync(string? host = null);

    /// <summary>
    /// Sets authentication credentials for the default user.
    /// </summary>
    /// <param name="password">The password for the default user.</param>
    public abstract Task SetAuthenticationAsync(string password);

    /// <summary>
    /// Sets authentication credentials for a new user.
    /// </summary>
    /// <param name="username">The username for the new user.</param>
    /// <param name="password">The password for the new user.</param>
    public abstract Task SetAuthenticationAsync(string username, string password);

    /// <summary>
    /// Clears all usernames and passwords.
    /// </summary>
    public abstract Task ClearAuthenticationAsync();

    /// <summary>
    /// Kill all normal clients on the server.
    /// </summary>
    public async Task KillClientsAsync()
    {
        await using var client = await CreateClientAsync();
        var options = new ClientFilterOptions().WithType(ClientType.Normal);
        _ = await client.ClientKillAsync(options);
    }

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
public sealed class ClusterServer(bool useTls = false, string? host = null) : Server(useClusterMode: true, useTls: useTls, host: host)
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
            username: _username,
            password: _password);

    /// <inheritdoc cref="Server.CreateClientAsync(string?)"/>
    public override async Task<BaseClient> CreateClientAsync(string? host = null)
        => await CreateClusterClientAsync(host);

    /// <summary>
    /// Builds and returns a cluster client for this server.
    /// </summary>
    /// <param name="host">Optional hostname.</param>
    public async Task<GlideClusterClient> CreateClusterClientAsync(string? host = null)
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithAddress(host ?? Address.Host, Address.Port);

        if (UseTls)
        {
            _ = builder.WithTls();
            _ = builder.WithTrustedCertificate(CertificateData!);
        }

        if (_password != null)
        {
            _ = builder.WithAuthentication(password: _password);
        }

        return await CreateClientAsync(()
            => GlideClusterClient.CreateClient(builder.Build()));
    }

    public override async Task SetAuthenticationAsync(string password)
    {
        await using var client = await CreateClusterClientAsync();
        await client.ConfigSetAsync("requirepass", password, Route.AllNodes);

        _username = null;
        _password = password;
    }

    public override async Task SetAuthenticationAsync(string username, string password)
    {
        await using GlideClusterClient client = await CreateClusterClientAsync();
        _ = await client.CustomCommand(["ACL", "SETUSER", username, "on", $">{password}", "~*", "+@all"], Route.AllNodes);

        _username = username;
        _password = password;
    }

    public override async Task ClearAuthenticationAsync()
    {
        await using var client = await CreateClusterClientAsync();
        if (_username is not null)
        {
            _ = await client.CustomCommand(["ACL", "DELUSER", _username], Route.AllNodes);
        }
        else
        {
            await client.ConfigSetAsync("requirepass", "", Route.AllNodes);
        }

        _username = null;
        _password = null;
    }

    #endregion
}

/// <summary>
/// Valkey standalone server.
/// </summary>
public sealed class StandaloneServer(
    bool useTls = false,
    int? replicaCount = null,
    string? host = null) : Server(useClusterMode: false, useTls: useTls, replicaCount: replicaCount, host: host)
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
            username: _username,
            password: _password);

    /// <inheritdoc cref="Server.CreateClientAsync(string?)"/>
    public override async Task<BaseClient> CreateClientAsync(string? host = null)
        => await CreateStandaloneClientAsync(host);

    /// <summary>
    /// Builds and returns a standalone client for this server.
    /// </summary>
    /// <param name="host">Optional hostname.</param>
    public async Task<GlideClient> CreateStandaloneClientAsync(string? host = null)
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAddress(host ?? Address.Host, Address.Port);

        if (UseTls)
        {
            _ = builder.WithTls();
            _ = builder.WithTrustedCertificate(CertificateData!);
        }

        if (_password != null)
        {
            _ = builder.WithAuthentication(password: _password);
        }

        return await CreateClientAsync(()
            => GlideClient.CreateClient(builder.Build()));
    }

    public override async Task SetAuthenticationAsync(string password)
    {
        await using GlideClient client = await CreateStandaloneClientAsync();
        await client.ConfigSetAsync("requirepass", password);

        _username = null;
        _password = password;
    }

    public override async Task SetAuthenticationAsync(string username, string password)
    {
        await using var client = await CreateStandaloneClientAsync();
        _ = await client.CustomCommand(["ACL", "SETUSER", username, "on", $">{password}", "~*", "+@all"]);

        _username = username;
        _password = password;
    }

    public override async Task ClearAuthenticationAsync()
    {
        await using GlideClient client = await CreateStandaloneClientAsync();
        if (_username is not null)
        {
            _ = await client.CustomCommand(["ACL", "DELUSER", _username]);
        }
        else
        {
            await client.ConfigSetAsync("requirepass", "");
        }

        _username = null;
        _password = null;
    }

    /// <summary>
    /// Creates a <see cref="ConnectionMultiplexer"/> connected to this server.
    /// The caller is responsible for disposing the returned connection.
    /// </summary>
    public async Task<ConnectionMultiplexer> CreateConnectionMultiplexerAsync()
    {
        var config = new ConfigurationOptions();
        config.EndPoints.Add(Address.Host, Address.Port);
        return await ConnectionMultiplexer.ConnectAsync(config);
    }

    #endregion
}
