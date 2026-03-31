// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;
using Valkey.Glide.Pipeline;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Pipeline.Options;

namespace Valkey.Glide;

/// <summary>
/// Client used for connection to standalone servers. Use <see cref="CreateClient"/> to request a client.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/client-initialization/?lang=csharp" />
public partial class GlideClient : BaseClient, IGenericCommands, IServerManagementCommands, IConnectionManagementCommands
{
    internal GlideClient() { }

    // TODO add pubsub and other params to example and remarks
    /// <summary>
    /// Creates a new <see cref="GlideClient" /> instance and establishes a connection to a standalone Valkey server.
    /// </summary>
    /// <remarks>
    /// <b>Remarks:</b>
    /// Use this static method to create and connect a <see cref="GlideClient" /> to a standalone Valkey server.<br />
    /// The client will automatically handle connection establishment, including any authentication and TLS configurations.
    /// <list type="bullet">
    ///   <item>
    ///     <b>Authentication</b>: If credentials are provided, the client will attempt to authenticate using the specified username and password.
    ///   </item>
    ///   <item>
    ///     <b>TLS</b>: If <see cref="ClientConfigurationBuilder{T}.UseTls" /> is set to <see langword="true" />, the client will establish a secure connection using <c>TLS</c>.
    ///   </item>
    ///   <item>
    ///     <b>Reconnection Strategy</b>: The <see cref="RetryStrategy" /> settings define how the client will attempt to reconnect in case of disconnections.
    ///   </item>
    /// </list>
    /// <example>
    /// <code>
    /// using Glide;
    /// using static Glide.ConnectionConfiguration;
    ///
    /// var config = new StandaloneClientConfigurationBuilder()
    ///     .WithAddress("primary.example.com", 6379)
    ///     .WithAddress("replica1.example.com", 6379)
    ///     .WithDatabaseId(1)
    ///     .WithAuthentication("user1", "passwordA")
    ///     .WithTls()
    ///     .WithConnectionRetryStrategy(5, 100, 2)
    ///     .Build();
    /// await using GlideClient client = await GlideClient.CreateClient(config);
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="config">The configuration options for the client, including server addresses, authentication credentials, TLS settings, database selection, reconnection strategy, and Pub/Sub subscriptions.</param>
    /// <returns>A task that resolves to a connected <see cref="GlideClient" /> instance.</returns>
    public static async Task<GlideClient> CreateClient(StandaloneClientConfiguration config)
        => await CreateClient(config, () => new GlideClient());

    /// <inheritdoc/>
    public async Task<object?[]?> Exec(Batch batch, bool raiseOnError)
        => await Batch(batch, raiseOnError);

    /// <inheritdoc/>
    public async Task<object?[]?> Exec(Batch batch, bool raiseOnError, BatchOptions options)
        => await Batch(batch, raiseOnError, options);

    /// <inheritdoc/>
    public async Task<object?> CustomCommand(IEnumerable<GlideString> args)
        => await Command(Request.CustomCommand([.. args]));

    /// <inheritdoc/>
    public async Task<string> InfoAsync() => await InfoAsync([]);

    /// <inheritdoc/>
    public async Task<string> InfoAsync(IEnumerable<InfoOptions.Section> sections)
        => await Command(Request.Info([.. sections]));

    /// <inheritdoc/>
    public async Task<ValkeyValue> EchoAsync(ValkeyValue message)
    {
        return await Command(Request.Echo(message));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> PingAsync()
        => await Command(Request.Ping());

    /// <inheritdoc/>
    public async Task<ValkeyValue> PingAsync(ValkeyValue message)
        => await Command(Request.Ping(message));

    /// <inheritdoc/>
    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default)
    {
        return await Command(Request.ConfigGetAsync(pattern));
    }

    /// <inheritdoc/>
    public async Task ConfigResetStatisticsAsync()
    {
        _ = await Command(Request.ConfigResetStatisticsAsync());
    }

    /// <inheritdoc/>
    public async Task ConfigRewriteAsync()
    {
        _ = await Command(Request.ConfigRewriteAsync());
    }

    /// <inheritdoc/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
    {
        _ = await Command(Request.ConfigSetAsync(setting, value));
    }

    /// <inheritdoc/>
    public async Task<long> DatabaseSizeAsync()
    {
        return await Command(Request.DatabaseSizeAsync());
    }

    /// <inheritdoc/>
    public async Task FlushAllDatabasesAsync()
    {
        _ = await Command(Request.FlushAllDatabasesAsync());
    }

    /// <inheritdoc/>
    public async Task FlushDatabaseAsync()
    {
        _ = await Command(Request.FlushDatabaseAsync());
    }

    /// <inheritdoc/>
    public async Task<DateTime> LastSaveAsync()
    {
        return await Command(Request.LastSaveAsync());
    }

    /// <inheritdoc/>
    public async Task<DateTime> TimeAsync()
    {
        return await Command(Request.TimeAsync());
    }

    /// <inheritdoc/>
    public async Task<string> LolwutAsync()
    {
        return await Command(Request.LolwutAsync());
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> ClientGetNameAsync()
    {
        return await Command(Request.ClientGetName());
    }

    /// <inheritdoc/>
    public async Task<long> ClientIdAsync()
    {
        return await Command(Request.ClientId());
    }

    /// <inheritdoc/>
    public async Task SelectAsync(long index)
    {
        _ = await Command(Request.Select(index));
    }

    /// <inheritdoc/>
    public async Task<(string cursor, ValkeyKey[] keys)> ScanAsync(string cursor, ScanOptions? options = null)
        => await Command(Request.ScanAsync(cursor, options));

    /// <inheritdoc/>
    public async Task WatchAsync(IEnumerable<ValkeyKey> keys)
    {
        _ = await Command(Request.Watch(keys));
    }

    /// <inheritdoc/>
    public async Task UnwatchAsync()
    {
        _ = await Command(Request.Unwatch());
    }

    /// <inheritdoc/>
    protected override async Task<Version> GetServerVersionAsync()
    {
        if (_serverVersion == null)
        {
            try
            {
                var infoResponse = await Command(Request.Info([InfoOptions.Section.SERVER]));
                _serverVersion = ParseServerVersion(infoResponse) ?? DefaultServerVersion;
            }
            catch
            {
                _serverVersion = DefaultServerVersion;
            }
        }

        return _serverVersion;
    }
}
