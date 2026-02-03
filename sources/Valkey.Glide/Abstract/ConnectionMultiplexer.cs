// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;
using System.Net;

using Valkey.Glide.Internals;

using static Valkey.Glide.Commands.Options.InfoOptions;
using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide;

/// <summary>
/// Connection methods common to both standalone and cluster clients.<br />
/// See also <see cref="GlideClient" /> and <see cref="GlideClusterClient" />.
/// </summary>
public sealed class ConnectionMultiplexer : IConnectionMultiplexer, IDisposable, IAsyncDisposable
{
    /// <inheritdoc cref="ConnectAsync(string, TextWriter?)" />
    public static ConnectionMultiplexer Connect(string configuration, TextWriter? log = null)
        => Connect(ConfigurationOptions.Parse(configuration), log);

    /// <inheritdoc cref="ConnectAsync(string, Action{ConfigurationOptions}, TextWriter?)" />
    public static ConnectionMultiplexer Connect(string configuration, Action<ConfigurationOptions> configure, TextWriter? log = null)
        => Connect(ConfigurationOptions.Parse(configuration).Apply(configure), log);

    /// <inheritdoc cref="ConnectAsync(ConfigurationOptions, TextWriter?)" />
    public static ConnectionMultiplexer Connect(ConfigurationOptions configuration, TextWriter? log = null)
        => ConnectAsync(configuration, log).GetAwaiter().GetResult();

    /// <summary>
    /// Creates a new <see cref="ConnectionMultiplexer" /> instance.
    /// </summary>
    /// <param name="configuration">The string configuration to use for this multiplexer.</param>
    /// <param name="log">The log writer is not supported by GLIDE.</param>
    public static async Task<ConnectionMultiplexer> ConnectAsync(string configuration, TextWriter? log = null)
        => await ConnectAsync(ConfigurationOptions.Parse(configuration), log);

    /// <summary>
    /// Creates a new <see cref="ConnectionMultiplexer" /> instance.
    /// </summary>
    /// <param name="configuration">The string configuration to use for this multiplexer.</param>
    /// <param name="configure">Action to further modify the parsed configuration options.</param>
    /// <param name="log">The log writer is not supported by GLIDE.</param>
    public static async Task<ConnectionMultiplexer> ConnectAsync(string configuration, Action<ConfigurationOptions> configure, TextWriter? log = null)
        => await ConnectAsync(ConfigurationOptions.Parse(configuration).Apply(configure), log);

    /// <summary>
    /// Creates a new <see cref="ConnectionMultiplexer" /> instance.
    /// </summary>
    /// <param name="configuration">The configuration options to use for this multiplexer.</param>
    /// <param name="log">The log writer is not supported by GLIDE.</param>
    public static async Task<ConnectionMultiplexer> ConnectAsync(ConfigurationOptions configuration, TextWriter? log = null)
    {
        Utils.Requires<NotImplementedException>(log == null, "Log writer is not supported by GLIDE");

        bool isCluster = await IsCluster(configuration);

        ConnectionMultiplexer multiplexer = new(configuration);

        // Configure pub/sub to route messages to this multiplexer.
        BaseClientConfiguration config;
        if (isCluster)
        {
            var configBuilder = CreateClientConfigBuilder<ClusterClientConfigurationBuilder>(configuration);

            var subscriptionsConfig = new ClusterPubSubSubscriptionConfig();
            subscriptionsConfig.WithCallback((msg, ctx) => ((ConnectionMultiplexer)ctx!).OnMessage(msg), multiplexer);
            configBuilder.WithPubSubSubscriptions(subscriptionsConfig);

            config = configBuilder.Build();
        }

        else
        {
            var configBuilder = CreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(configuration);

            var subscriptionsConfig = new StandalonePubSubSubscriptionConfig();
            subscriptionsConfig.WithCallback((msg, ctx) => ((ConnectionMultiplexer)ctx!).OnMessage(msg), multiplexer);
            configBuilder.WithPubSubSubscriptions(subscriptionsConfig);

            config = configBuilder.Build();
        }

        multiplexer._db = await Database.Create(config);

        return multiplexer;
    }

    public EndPoint[] GetEndPoints(bool configuredOnly)
        => configuredOnly
            ? [.. RawConfig.EndPoints]
            : [.. GetServers().Select(s => s.EndPoint)];

    public IServer GetServer(string host, int port, object? asyncState = null)
        => GetServer(Format.ParseEndPoint(host, port), asyncState);

    public IServer GetServer(string hostAndPort, object? asyncState = null)
        => Format.TryParseEndPoint(hostAndPort, out EndPoint? ep)
            ? GetServer(ep, asyncState)
            : throw new ArgumentException($"The specified host and port could not be parsed: {hostAndPort}", nameof(hostAndPort));

    public IServer GetServer(IPAddress host, int port)
        => GetServer(new IPEndPoint(host, port));

    public IServer GetServer(EndPoint endpoint, object? asyncState = null)
    {
        Utils.Requires<NotImplementedException>(asyncState is null, "Async state is not supported by GLIDE");
        foreach (IServer server in GetServers())
        {
            if (server.EndPoint.Equals(endpoint))
            {
                return server;
            }
        }
        throw new ArgumentException("The specified endpoint is not defined", nameof(endpoint));
    }

    // TODO currently this returns only primary node on standalone
    // https://github.com/valkey-io/valkey-glide/issues/4293
    public IServer[] GetServers()
    {
        // run INFO on all nodes, but disregard the node responses, we need node addresses only
        if (_db!.IsCluster)
        {
            Dictionary<string, string> info = _db.Command(Request.Info([]).ToMultiNodeValue(), Route.AllNodes).GetAwaiter().GetResult();
            return [.. info.Keys.Select(addr => new ValkeyServer(_db, IPEndPoint.Parse(addr)))];
        }
        else
        {
            // due to #4293, core ignores route on standalone and always return a single node response
            string info = _db.Command(Request.Info([]), Route.AllNodes).GetAwaiter().GetResult();
            // and there is no way to get IP address from server, assuming localhost (127.0.0.1)
            // we can try to get port only (in some deployments, this info is also missing)
            int port = 6379;
            foreach (string line in info.Split("\r\n"))
            {
                if (line.Contains("tcp_port:"))
                {
                    port = int.Parse(line.Split(':')[1]);
                }
            }
            return [new ValkeyServer(_db, new IPEndPoint(0x100007F, port))];
        }
    }

    public bool IsConnected => true;

    public bool IsConnecting => false;

    /// <inheritdoc/>
    public ISubscriber GetSubscriber(object? asyncState = null)
    {
        Utils.Requires<NotImplementedException>(asyncState is null, "Async state is not supported by GLIDE");
        return new Subscriber(this, _db!);
    }

    public IDatabase GetDatabase(int db = -1, object? asyncState = null)
    {
        Utils.Requires<NotImplementedException>(db == -1, "To switch the database, please use `SELECT` command.");
        Utils.Requires<NotImplementedException>(asyncState is null, "Async state is not supported by GLIDE");
        return _db!;
    }

    public long? GetConnectionId(EndPoint endpoint, ConnectionType connectionType)
        => GetConnectionIdAsync(endpoint, connectionType).GetAwaiter().GetResult();

    public async Task<long?> GetConnectionIdAsync(EndPoint endpoint, ConnectionType connectionType)
    {
        IServer server = GetServer(endpoint);
        long connectionId = await server.ClientIdAsync();
        return connectionId;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        lock (_lock)
        {
            if (_db is null)
            {
                return;
            }
            _db.Dispose();
            _db = null;
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync() => await Task.Run(Dispose);

    /// <inheritdoc/>
    public override string ToString() => _db!.ToString();

    internal ConfigurationOptions RawConfig { private set; get; }

    private readonly object _lock = new();
    private Database? _db;

    private ConnectionMultiplexer(ConfigurationOptions configuration)
    {
        RawConfig = configuration;
    }

    internal static T CreateClientConfigBuilder<T>(ConfigurationOptions configuration)
        where T : ClientConfigurationBuilder<T>, new()
    {
        T config = new();
        foreach (EndPoint ep in configuration.EndPoints)
        {
            config.Addresses += Utils.SplitEndpoint(ep);
        }

        config.UseTls = configuration.Ssl;
        foreach (var cert in configuration._trustedIssuers)
        {
            config.WithTrustedCertificate(cert);
        }

        _ = configuration.ConnectTimeout.HasValue ? config.ConnectionTimeout = TimeSpan.FromMilliseconds(configuration.ConnectTimeout.Value) : new();
        _ = configuration.ResponseTimeout.HasValue ? config.RequestTimeout = TimeSpan.FromMilliseconds(configuration.ResponseTimeout.Value) : new();
        _ = (configuration.User ?? configuration.Password) is not null ? config.WithAuthentication(configuration.User, configuration.Password!) : new();
        _ = configuration.ClientName is not null ? config.ClientName = configuration.ClientName : "";
        if (configuration.Protocol is not null)
        {
            config.ProtocolVersion = configuration.Protocol switch
            {
                Protocol.Resp2 => ConnectionConfiguration.Protocol.RESP2,
                Protocol.Resp3 => ConnectionConfiguration.Protocol.RESP3,
                _ => throw new ArgumentException($"Unknown value of Protocol: {configuration.Protocol}"),
            };
        }
        if (config is StandaloneClientConfigurationBuilder standalone)
        {
            _ = configuration.DefaultDatabase.HasValue ? standalone.DataBaseId = (uint)configuration.DefaultDatabase.Value : 0;
        }
        _ = configuration.ReconnectRetryPolicy.HasValue ? config.ConnectionRetryStrategy = configuration.ReconnectRetryPolicy.Value : new();
        _ = configuration.ReadFrom.HasValue ? config.ReadFrom = configuration.ReadFrom.Value : new();

        return config;
    }

    /// <inheritdoc/>
    void IConnectionMultiplexer.Close() => Dispose();

    /// <inheritdoc/>
    Task IConnectionMultiplexer.CloseAsync() => DisposeAsync().AsTask();

    #region Subscriptions

    private readonly ConcurrentDictionary<ValkeyChannel, Subscription> _subscriptions = new();

    /// <summary>
    /// Determines whether there is a subscription for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to check for a subscription.</param>
    /// <returns>True if there is a subscription for the specified channel, false otherwise.</returns>
    internal bool ContainsSubscription(ValkeyChannel channel)
        => _subscriptions.ContainsKey(channel);

    /// <summary>
    /// Adds a subscription handler for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <param name="handler">The handler to invoke when a message is received on the channel.</param>
    internal void AddSubscriptionHandler(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler)
    {
        lock (_subscriptions)
        {
            var subscription = _subscriptions.GetOrAdd(channel, _ => new Subscription());
            subscription.AddHandler(handler);
        }
    }

    /// <summary>
    /// Adds a subscription queue for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <returns>The subscription queue for the specified channel.</returns>
    internal void AddSubscriptionQueue(ValkeyChannel channel, ChannelMessageQueue queue)
    {
        lock (_subscriptions)
        {
            var subscription = _subscriptions.GetOrAdd(channel, _ => new Subscription());
            subscription.AddQueue(queue);
        }
    }

    /// <summary>
    /// Removes a subscription handler for the specified channel.
    /// If the subscription is empty after removing the handler, the subscription is removed.
    /// </summary>
    /// <param name="channel">The channel to unsubscribe from.</param>
    /// <param name="handler">The handler to remove.</param>
    /// <returns>True if the subscription should be removed from the server (no handlers/queues remain), false otherwise.</returns>
    internal void RemoveSubscriptionHandler(ValkeyChannel channel, Action<ValkeyChannel, ValkeyValue> handler)
    {
        lock (_subscriptions)
        {
            if (_subscriptions.TryGetValue(channel, out var subscription))
            {
                subscription.RemoveHandler(handler);

                if (subscription.IsEmpty())
                    RemoveSubscription(channel);
            }
        }
    }

    /// <summary>
    /// Removes a subscription queue for the specified channel.
    /// If the subscription is empty after removing the queue, the subscription is removed.
    /// </summary>
    /// <param name="queue">The queue to remove.</param>
    internal void RemoveSubscriptionQueue(ChannelMessageQueue queue)
    {
        var channel = queue.Channel;

        lock (_subscriptions)
        {
            if (_subscriptions.TryGetValue(channel, out var subscription))
            {
                subscription.RemoveQueue(queue);

                if (subscription.IsEmpty())
                    RemoveSubscription(channel);
            }
        }
    }

    /// <summary>
    /// Removes the subscription for the specified channel.
    /// </summary>
    /// <param name="channel">The channel to unsubscribe from.</param>
    internal void RemoveSubscription(ValkeyChannel channel)
    {
        lock (_subscriptions)
        {
            _subscriptions.Remove(channel, out var subscription);
            subscription?.Clear();
        }
    }

    /// <summary>
    /// Removes all subscriptions.
    /// </summary>
    internal void RemoveAllSubscriptions()
    {
        lock (_subscriptions)
        {
            foreach (var subscription in _subscriptions.Values)
                subscription.Clear();

            _subscriptions.Clear();
        }
    }

    /// <summary>
    /// Handles incoming Pub/Sub messages and routes them to the appropriate subscription handlers.
    /// </summary>
    /// <param name="message">The incoming PubSubMessage.</param>
    internal void OnMessage(PubSubMessage message)
    {
        var channel = ValkeyChannel.FromPubSubMessage(message);
        if (_subscriptions.TryGetValue(channel, out var sub))
        {
            sub.OnMessage(channel, message.Message);
        }
    }

    #endregion

    /// <summary>
    /// Determines whether the given configuration corresponds to a cluster server.
    /// </summary>
    /// <param name="configuration">The configuration options to check.</param>
    /// <returns>True if the configuration corresponds to a cluster server; otherwise, false.</returns>
    private static async Task<bool> IsCluster(ConfigurationOptions configuration)
    {
        // Create standalone client to determine server type.
        var standaloneConfigBuilder = CreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(configuration);
        GlideClient standalone = GlideClient.CreateClient(standaloneConfigBuilder.Build()).GetAwaiter().GetResult();

        string info = standalone.InfoAsync([Section.CLUSTER]).GetAwaiter().GetResult();
        bool isCluster = info.Contains("cluster_enabled:1");

        await standalone.DisposeAsync().AsTask();

        return isCluster;
    }
}
