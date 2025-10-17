// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;
using Valkey.Glide.Pipeline;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Pipeline.Options;

namespace Valkey.Glide;

// TODO add wiki link
/// <summary>
/// Client used for connection to standalone servers. Use <see cref="CreateClient"/> to request a client.
/// </summary>
public class GlideClient : BaseClient, IGenericCommands, IServerManagementCommands, IConnectionManagementCommands
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
    ///     .WithDataBaseId(1)
    ///     .WithAuthentication("user1", "passwordA")
    ///     .WithTls()
    ///     .WithConnectionRetryStrategy(5, 100, 2)
    ///     .Build();
    /// using GlideClient client = await GlideClient.CreateClient(config);
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="config">The configuration options for the client, including server addresses, authentication credentials, TLS settings, database selection, reconnection strategy, and Pub/Sub subscriptions.</param>
    /// <returns>A task that resolves to a connected <see cref="GlideClient" /> instance.</returns>
    public static async Task<GlideClient> CreateClient(StandaloneClientConfiguration config)
        => await CreateClient(config, () => new GlideClient());

    public async Task<object?[]?> Exec(Batch batch, bool raiseOnError)
        => await Batch(batch, raiseOnError);

    public async Task<object?[]?> Exec(Batch batch, bool raiseOnError, BatchOptions options)
        => await Batch(batch, raiseOnError, options);

    public async Task<object?> CustomCommand(GlideString[] args)
        => await Command(Request.CustomCommand(args));

    public async Task<string> InfoAsync() => await InfoAsync([]);

    public async Task<string> InfoAsync(InfoOptions.Section[] sections)
        => await Command(Request.Info(sections));

    public async Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Echo(message));
    }

    public async Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Ping());
    }

    public async Task<TimeSpan> PingAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Ping(message));
    }





    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ConfigGetAsync(pattern));
    }

    public async Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigResetStatisticsAsync());
    }

    public async Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigRewriteAsync());
    }

    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigSetAsync(setting, value));
    }

    public async Task<long> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.DatabaseSizeAsync(database));
    }

    public async Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.FlushAllDatabasesAsync());
    }

    public async Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.FlushDatabaseAsync(database));
    }

    public async Task<DateTime> LastSaveAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.LastSaveAsync());
    }

    public async Task<DateTime> TimeAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.TimeAsync());
    }

    public async Task<string> LolwutAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.LolwutAsync());
    }

    public async Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ClientGetName());
    }

    public async Task<long> ClientIdAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ClientId());
    }

    public async Task<string> SelectAsync(long index, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Select(index));
    }

    public async IAsyncEnumerable<ValkeyKey> KeysAsync(int database = -1, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");

        long currentCursor = cursor;
        int currentOffset = pageOffset;

        do
        {
            (long nextCursor, ValkeyKey[] keys) = await Command(Request.ScanAsync(currentCursor, pattern, pageSize));

            IEnumerable<ValkeyKey> keysToYield = currentOffset > 0 ? keys.Skip(currentOffset) : keys;

            foreach (ValkeyKey key in keysToYield)
            {
                yield return key;
            }

            currentCursor = nextCursor;
            currentOffset = 0;
        } while (currentCursor != 0);
    }

    protected override async Task InitializeServerVersionAsync()
    {
        try
        {
            var infoResponse = await Command(Request.Info([InfoOptions.Section.SERVER]));
            var versionMatch = System.Text.RegularExpressions.Regex.Match(infoResponse, @"(?:valkey_version|redis_version):([\d\.]+)");
            if (versionMatch.Success)
            {
                _serverVersion = new Version(versionMatch.Groups[1].Value);
            }
        }
        catch
        {
            // If we can't get version, assume newer version (use SORT_RO)
            _serverVersion = new Version(8, 0, 0);
        }
    }
}
