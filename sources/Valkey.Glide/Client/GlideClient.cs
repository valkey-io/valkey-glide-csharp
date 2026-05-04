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
/// <seealso href="https://glide.valkey.io/how-to/client-initialization/">Valkey GLIDE – Client Initialization</seealso>
public partial class GlideClient :
    BaseClient,
    IGlideClient
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
    /// var config = new ConnectionConfiguration.StandaloneClientConfigurationBuilder()
    ///     .WithAddress("primary.example.com", 6379)
    ///     .WithAddress("replica1.example.com", 6379)
    ///     .WithDatabaseId(1)
    ///     .WithAuthentication("user1", "passwordA")
    ///     .WithTls()
    ///     .WithConnectionRetryStrategy(5, 100, 2)
    ///     .Build();
    /// await using var client = await GlideClient.CreateClient(config);
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="config">The configuration options for the client, including server addresses, authentication credentials, TLS settings, database selection, reconnection strategy, and Pub/Sub subscriptions.</param>
    /// <returns>A task that resolves to a connected <see cref="GlideClient" /> instance.</returns>
    public static async Task<GlideClient> CreateClient(StandaloneClientConfiguration config)
        => await CreateClient(config, () => new GlideClient());

    /// <inheritdoc cref="IGenericCommands.Exec(Batch, bool)"/>
    public async Task<object?[]?> Exec(Batch batch, bool raiseOnError)
        => await Batch(batch, raiseOnError);

    /// <inheritdoc cref="IGenericCommands.Exec(Batch, bool, BatchOptions)"/>
    public async Task<object?[]?> Exec(Batch batch, bool raiseOnError, BatchOptions options)
        => await Batch(batch, raiseOnError, options);

    /// <inheritdoc cref="IGenericCommands.CustomCommand(IEnumerable{GlideString})"/>
    public async Task<object?> CustomCommand(IEnumerable<GlideString> args)
        => await Command(Request.CustomCommand([.. args]));

    /// <inheritdoc cref="IGlideClient.InfoAsync()"/>
    public async Task<string> InfoAsync() => await InfoAsync([]);

    /// <inheritdoc cref="IGlideClient.InfoAsync(IEnumerable{InfoOptions.Section})"/>
    public async Task<string> InfoAsync(IEnumerable<InfoOptions.Section> sections)
        => await Command(Request.Info([.. sections]));

    /// <inheritdoc cref="IGlideClient.ConfigGetAsync(ValkeyValue)"/>
    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default)
    {
        return await Command(Request.ConfigGetAsync(pattern));
    }

    /// <inheritdoc cref="IGlideClient.ConfigResetStatisticsAsync()"/>
    public async Task ConfigResetStatisticsAsync()
    {
        _ = await Command(Request.ConfigResetStatisticsAsync());
    }

    /// <inheritdoc cref="IGlideClient.ConfigRewriteAsync()"/>
    public async Task ConfigRewriteAsync()
    {
        _ = await Command(Request.ConfigRewriteAsync());
    }

    /// <inheritdoc cref="IGlideClient.ConfigSetAsync(ValkeyValue, ValkeyValue)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
    {
        _ = await Command(Request.ConfigSetAsync(setting, value));
    }

    /// <inheritdoc cref="IBaseClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})"/>
    public override async Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters)
    {
        _ = await Command(Request.ConfigSetAsync(parameters));
    }

    /// <inheritdoc cref="IGlideClient.DatabaseSizeAsync()"/>
    public async Task<long> DatabaseSizeAsync()
    {
        return await Command(Request.DatabaseSizeAsync());
    }

    /// <inheritdoc cref="IGlideClient.FlushAllDatabasesAsync()"/>
    public async Task FlushAllDatabasesAsync()
    {
        _ = await Command(Request.FlushAllDatabasesAsync());
    }

    /// <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)"/>
    public override async Task FlushAllDatabasesAsync(FlushMode mode)
    {
        _ = await Command(Request.FlushAllDatabasesAsync(mode));
    }

    /// <inheritdoc cref="IGlideClient.FlushDatabaseAsync()"/>
    public async Task FlushDatabaseAsync()
    {
        _ = await Command(Request.FlushDatabaseAsync());
    }

    /// <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)"/>
    public override async Task FlushDatabaseAsync(FlushMode mode)
    {
        _ = await Command(Request.FlushDatabaseAsync(mode));
    }

    /// <inheritdoc cref="IGlideClient.LastSaveAsync()"/>
    public Task<DateTimeOffset> LastSaveAsync()
        => Command(Request.LastSaveAsync());

    /// <inheritdoc cref="IGlideClient.TimeAsync()"/>
    public Task<DateTimeOffset> TimeAsync()
        => Command(Request.TimeAsync());

    /// <inheritdoc cref="IGlideClient.LolwutAsync()"/>
    public async Task<string> LolwutAsync()
    {
        return await Command(Request.LolwutAsync());
    }

    /// <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)"/>
    public override async Task<string> LolwutAsync(LolwutOptions options)
    {
        return await Command(Request.LolwutAsync(options));
    }

    /// <inheritdoc cref="IBaseClient.ConfigGetAsync(IEnumerable{ValkeyValue})"/>
    public override async Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns)
    {
        return await Command(Request.ConfigGetAsync(patterns));
    }

    /// <inheritdoc/>
    public async Task<(string cursor, ValkeyKey[] keys)> ScanAsync(string cursor, ScanOptions? options = null)
        => await Command(Request.ScanAsync(cursor, options));

    /// <inheritdoc cref="IGlideClient.ScanAsync(ScanOptions?)"/>
    public async IAsyncEnumerable<ValkeyKey> ScanAsync(ScanOptions? options = null)
    {
        string currentCursor = "0";

        do
        {
            (string nextCursor, ValkeyKey[] keys) = await Command(Request.ScanAsync(currentCursor, options));

            foreach (ValkeyKey key in keys)
            {
                yield return key;
            }

            currentCursor = nextCursor;
        } while (currentCursor != "0");
    }

    /// <inheritdoc cref="ITransactionBaseCommands.WatchAsync(IEnumerable{ValkeyKey})"/>
    public async Task WatchAsync(IEnumerable<ValkeyKey> keys)
    {
        _ = await Command(Request.Watch(keys));
    }

    /// <inheritdoc cref="ITransactionCommands.UnwatchAsync()"/>
    public async Task UnwatchAsync()
    {
        _ = await Command(Request.Unwatch());
    }

    /// <inheritdoc cref="BaseClient.GetServerVersionAsync()"/>
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
