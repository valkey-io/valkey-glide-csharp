// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Utility methods for pub/sub integration tests.
/// </summary>
public static class PubSubUtils
{
    #region Data

    /// <summary>Theory data for cluster mode (cluster vs standalone).</summary>
    public static TheoryData<bool> ClusterModeData => [true, false];

    /// <summary>Theory data for pub/sub channel modes (exact, pattern, and shard channels).</summary>
    public static TheoryData<PubSubChannelMode> ChannelModeData => [
        PubSubChannelMode.Exact,
        PubSubChannelMode.Pattern,
        PubSubChannelMode.Sharded];

    /// <summary>
    /// Theory data for all valid combinations of cluster mode and channel mode.
    /// </summary>
    public static TheoryData<bool, PubSubChannelMode> ClusterAndChannelModeData
    {
        get
        {
            var data = new TheoryData<bool, PubSubChannelMode>();
            foreach (var isCluster in ClusterModeData)
            {
                foreach (var channelMode in ChannelModeData)
                    if (IsChannelModeSupported(isCluster, channelMode))
                        data.Add(isCluster, channelMode);
            }

            return data;
        }
    }

    #endregion
    #region TimeSpans

    /// <summary>
    /// Maximum duration for pub/sub assertions.
    /// </summary>
    public static readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Retry interval for pub/sub assertions.
    /// </summary>
    public static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(0.5);

    #endregion
    #region Helpers

    /// <summary>
    /// Returns true if sharded pub/sub is supported.
    /// </summary>
    private static bool IsShardedSupported()
        => TestConfiguration.IsVersionAtLeast("7.0.0");

    /// <summary>
    /// Returns true if sharded pub/sub is supported for the given cluster mode.
    /// </summary>
    public static bool IsShardedSupported(bool isCluster)
        => isCluster && IsShardedSupported();

    /// <summary>
    /// Returns true if the given cluster mode and channel mode is supported.
    /// </summary>
    public static bool IsChannelModeSupported(bool isCluster, PubSubChannelMode channelMode)
        => channelMode != PubSubChannelMode.Sharded || IsShardedSupported(isCluster);

    /// <summary>
    /// Skips the current test unless sharded pub/sub is supported.
    /// </summary>
    public static void SkipUnlessShardedSupported()
    {
        if (!IsShardedSupported())
            Assert.Skip("Sharded pub/sub is supported for cluster clients since Valkey 7.0.0");
    }

    /// <summary>
    /// Skips the current test unless sharded pub/sub is supported for the given cluster mode.
    /// </summary>
    public static void SkipUnlessShardedSupported(bool isCluster)
    {
        if (!IsShardedSupported(isCluster))
            Assert.Skip("Sharded pub/sub is supported for cluster clients since Valkey 7.0.0");
    }

    /// <summary>
    /// Skips the current test unless the given cluster and channel mode are supported.
    /// </summary>
    public static void SkipUnlessChannelModeSupported(bool isCluster, PubSubChannelMode channelMode)
    {
        if (!IsChannelModeSupported(isCluster, channelMode))
            Assert.Skip("Sharded pub/sub is supported for cluster clients since Valkey 7.0.0");
    }

    /// <summary>
    /// Builds and returns the expected subscriptions for the given messages, indexed by channel mode.
    /// </summary>
    public static Dictionary<PubSubChannelMode, HashSet<string>> BuildSubscriptions(IEnumerable<PubSubMessage> messages)
    {
        var targets = new Dictionary<PubSubChannelMode, HashSet<string>>();

        foreach (var mode in Enum.GetValues<PubSubChannelMode>())
            targets[mode] = [];

        foreach (var message in messages)
        {
            var target = message.ChannelMode == PubSubChannelMode.Pattern ? message.Pattern! : message.Channel;
            targets[message.ChannelMode].Add(target);
        }
        return targets;
    }

    #endregion
    #region Builders

    /// <summary>
    /// Builds and returns a unique channel name for testing.
    /// </summary>
    public static string BuildChannel(string? id = null)
        => $"channel:{{{id ?? Guid.NewGuid().ToString()}}}";

    /// <summary>
    /// Returns a message appropriate for the given channel mode.
    /// </summary>
    public static PubSubMessage BuildMessage(PubSubChannelMode channelMode = PubSubChannelMode.Exact)
    {
        var id = Guid.NewGuid();

        var channel = $"{{test:{id}}}:channel";
        var pattern = $"{{test:{id}}}:*";
        var message = $"{{test:{id}}}:message";

        return channelMode switch
        {
            PubSubChannelMode.Exact => PubSubMessage.FromChannel(message, channel),
            PubSubChannelMode.Pattern => PubSubMessage.FromPattern(message, channel, pattern),
            PubSubChannelMode.Sharded => PubSubMessage.FromShardChannel(message, channel),
            _ => throw new ArgumentOutOfRangeException(nameof(channelMode))
        };
    }

    /// <summary>
    /// Builds and returns a Valkey server based on cluster mode.
    /// </summary>
    public static Server BuildServer(bool isCluster)
    {
        return isCluster
            ? new ClusterServer()
            : new StandaloneServer();
    }

    #endregion
    #region Publish

    /// <summary>
    /// Builds and returns a publisher client with the specified cluster mode.
    /// </summary>
    public static BaseClient BuildPublisher(bool isCluster)
    {
        return isCluster
            ? TestConfiguration.DefaultClusterClient()
            : TestConfiguration.DefaultStandaloneClient();
    }

    /// <summary>
    /// Publishes the specified message using the appropriate publish method based on its channel mode.
    /// </summary>
    public static async Task PublishAsync(BaseClient publisher, PubSubMessage message)
        => await PublishAsync(publisher, [message]);

    /// <summary>
    /// Publishes the specified messages sequentially using the appropriate publish method based on each message's channel mode.
    /// </summary>
    public static async Task PublishAsync(BaseClient publisher, IEnumerable<PubSubMessage> messages)
    {
        foreach (var message in messages)
        {
            _ = message.ChannelMode switch
            {
                PubSubChannelMode.Exact => await publisher.PublishAsync(message.Channel, message.Message),
                PubSubChannelMode.Pattern => await publisher.PublishAsync(message.Channel, message.Message),
                PubSubChannelMode.Sharded => await ((GlideClusterClient)publisher).SPublishAsync(message.Channel, message.Message),
                _ => throw new InvalidOperationException($"Unsupported channel mode: {message.ChannelMode}")
            };
        }
    }

    #endregion

    #region Subscribe

    /// <summary>
    /// Subscription mode for pub/sub integration tests.
    /// </summary>
    public enum SubscribeMode
    {
        Config,
        Lazy,
        Blocking
    }

    /// <summary>Theory data for subscription modes.</summary>
    public static TheoryData<SubscribeMode> SubscribeModeData => [
        SubscribeMode.Config,
        SubscribeMode.Lazy,
        SubscribeMode.Blocking];

    /// <summary>
    /// Builds and returns a client that is subscribed to receive the specified message using the given subscription mode.
    /// </summary>
    public static async Task<BaseClient> BuildSubscriber(
        bool isCluster,
        PubSubMessage? message = null,
        SubscribeMode mode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null)
        => await BuildSubscriber(isCluster, message != null ? [message] : [], mode, callback, timeout);

    /// <summary>
    /// Builds and returns a client that is subscribed to receive the specified messages using the given subscription mode.
    /// </summary>
    public static async Task<BaseClient> BuildSubscriber(
        bool isCluster,
        IEnumerable<PubSubMessage> messages,
        SubscribeMode mode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null)
    {
        return isCluster
            ? await BuildClusterSubscriber(messages, mode, callback, timeout)
            : await BuildStandaloneSubscriber(messages, mode, callback, timeout);
    }

    /// <summary>
    /// Builds and returns a cluster subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<GlideClusterClient> BuildClusterSubscriber(
        PubSubMessage messages,
        SubscribeMode mode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null
    ) => await BuildClusterSubscriber([messages], mode, callback, timeout);

    /// <summary>
    /// Builds and returns a cluster subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<GlideClusterClient> BuildClusterSubscriber(
        IEnumerable<PubSubMessage> messages,
        SubscribeMode mode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null
    )
    {
        // Validate arguments.
        if (mode != SubscribeMode.Config && callback != null)
            throw new ArgumentException($"Callbacks are only supported for {SubscribeMode.Config} subscriptions.");

        if (mode != SubscribeMode.Blocking && timeout != null)
            throw new ArgumentException($"Timeouts are only supported for {SubscribeMode.Blocking} subscriptions.");

        // Get channels, patterns, and shard channels.
        var targets = BuildSubscriptions(messages);
        var channels = targets[PubSubChannelMode.Exact];
        var patterns = targets[PubSubChannelMode.Pattern];
        var shardChannels = targets[PubSubChannelMode.Sharded];

        var configBuilder = TestConfiguration.DefaultClusterClientConfig();
        if (mode == SubscribeMode.Config)
        {
            var pubSubConfig = new ClusterPubSubSubscriptionConfig();

            foreach (var ch in channels) pubSubConfig.WithChannel(ch);
            foreach (var p in patterns) pubSubConfig.WithPattern(p);
            foreach (var sc in shardChannels) pubSubConfig.WithShardChannel(sc);
            if (callback != null) pubSubConfig.WithCallback(callback);

            var config = configBuilder.WithPubSubSubscriptions(pubSubConfig).Build();
            return await GlideClusterClient.CreateClient(config);
        }

        var client = await GlideClusterClient.CreateClient(configBuilder.Build());

        if (mode == SubscribeMode.Lazy)
        {
            if (channels.Count > 0) await client.SubscribeLazyAsync(channels);
            if (patterns.Count > 0) await client.PSubscribeLazyAsync(patterns);
            if (shardChannels.Count > 0) await client.SSubscribeLazyAsync(shardChannels);
        }
        else
        {
            timeout ??= MaxDuration;
            if (channels.Count > 0) await client.SubscribeAsync(channels, timeout.Value);
            if (patterns.Count > 0) await client.PSubscribeAsync(patterns, timeout.Value);
            if (shardChannels.Count > 0) await client.SSubscribeAsync(shardChannels, timeout.Value);
        }

        return client;
    }

    #endregion
    #region Unsubscribe

    /// <summary>
    /// Unsubscribe mode for pub/sub integration tests.
    /// </summary>
    public enum UnsubscribeMode
    {
        Lazy,
        Blocking
    }

    /// <summary>
    /// Theory data for unsubscribe modes.
    /// </summary>
    public static TheoryData<UnsubscribeMode> UnsubscribeModeData => [
        UnsubscribeMode.Lazy,
        UnsubscribeMode.Blocking];

    /// <summary>
    /// Unsubscribes the client with the using the given mode from receiving the specified message.
    /// </summary>
    public static async Task UnsubscribeAsync(BaseClient subscriber, UnsubscribeMode unsubscribeMode, PubSubMessage message)
        => await UnsubscribeAsync(subscriber, unsubscribeMode, [message]);

    /// <summary>
    /// Unsubscribes the client with the using the given mode from receiving the specified messages.
    /// </summary>
    public static async Task UnsubscribeAsync(BaseClient subscriber, UnsubscribeMode unsubscribeMode, PubSubMessage[] messages)
    {
        var unsubscriptionTargets = BuildSubscriptions(messages);

        // Unsubscribe from channels by mode.
        foreach (var item in unsubscriptionTargets)
        {
            var channels = item.Value;
            if (channels.Count == 0)
                continue;

            var channelMode = item.Key;
            _ = channelMode switch
            {
                PubSubChannelMode.Exact =>
                    unsubscribeMode == UnsubscribeMode.Lazy
                        ? subscriber.UnsubscribeLazyAsync(channels)
                        : subscriber.UnsubscribeAsync(channels),

                PubSubChannelMode.Pattern =>
                    unsubscribeMode == UnsubscribeMode.Lazy
                        ? subscriber.PUnsubscribeLazyAsync(channels)
                        : subscriber.PUnsubscribeAsync(channels),

                PubSubChannelMode.Sharded =>
                    unsubscribeMode == UnsubscribeMode.Lazy
                        ? ((GlideClusterClient)subscriber).SUnsubscribeLazyAsync(channels)
                        : ((GlideClusterClient)subscriber).SUnsubscribeAsync(channels),

                _ => throw new InvalidOperationException($"Unsupported channel mode: {channelMode}")
            };
        }
    }

    #endregion
    #region Assertions

    /// <summary>
    /// Asserts that the client is subscribed to receive the specified message.
    /// </summary>
    public static async Task AssertSubscribedAsync(BaseClient client, PubSubMessage message)
        => await AssertSubscribedAsync(client, [message]);

    /// <summary>
    /// Asserts that the client is subscribed to receive the specified messages.
    /// </summary>
    public static async Task AssertSubscribedAsync(BaseClient client, IEnumerable<PubSubMessage> messages)
    {
        var expectedSubscriptions = BuildSubscriptions(messages);

        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var isSubset = true;
            var actual = (await client.GetSubscriptionsAsync()).Actual;

            foreach (var item in expectedSubscriptions)
            {
                var mode = item.Key;
                var expected = item.Value;

                if (!expected.IsSubsetOf(actual[mode]))
                {
                    isSubset = false;
                    break;
                }
            }

            if (isSubset)
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Unexpected subscribers.");
    }

    /// <summary>
    /// Asserts that the client is not subscribed to receive the specified message.
    /// </summary>
    public static async Task AssertNotSubscribedAsync(BaseClient client, PubSubMessage message)
        => await AssertNotSubscribedAsync(client, [message]);

    /// <summary>
    /// Asserts that the client is not subscribed to receive the specified messages.
    /// </summary>
    public static async Task AssertNotSubscribedAsync(BaseClient client, IEnumerable<PubSubMessage> messages)
    {
        var notExpected = BuildSubscriptions(messages);

        // Retry until unsubscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            bool hasOverlap = false;
            var actual = (await client.GetSubscriptionsAsync()).Actual;

            foreach (var item in notExpected)
            {
                var mode = item.Key;
                var expected = item.Value;

                if (expected.Overlaps(actual[mode]))
                {
                    hasOverlap = true;
                    break;
                }
            }

            if (!hasOverlap)
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Unexpected subscribers.");
    }

    /// <summary>
    /// Asserts that the given client has received the specified message.
    /// </summary>
    public static async Task AssertReceivedAsync(BaseClient client, PubSubMessage expected)
        => await AssertReceivedAsync(client, [expected]);

    /// <summary>
    /// Asserts that the given client has received the specified messages.
    /// </summary>
    public static async Task AssertReceivedAsync(BaseClient client, IEnumerable<PubSubMessage> expected)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        // Retry until all expected messages are received or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            if (queue.Count >= expected.Count())
            {
                var received = new List<PubSubMessage>();
                while (queue.TryGetMessage(out PubSubMessage? msg))
                    received.Add(msg!);

                Assert.Equivalent(expected, received);
                return;
            }

            await Task.Delay(RetryInterval);
        }

        Assert.Fail("Expected messages were not received within the timeout period.");
    }

    /// <summary>
    /// Asserts that the given client has not received the specified message.
    /// </summary>
    public static Task AssertNotReceivedAsync(BaseClient client, PubSubMessage expected)
        => AssertNotReceivedAsync(client, [expected]);

    /// <summary>
    /// Asserts that the given client has not received the specified messages.
    /// </summary>
    public static Task AssertNotReceivedAsync(BaseClient client, IEnumerable<PubSubMessage> expected)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        while (queue!.TryGetMessage(out PubSubMessage? received))
            Assert.DoesNotContain(received, expected);

        return Task.CompletedTask;
    }

    #endregion

    /// <summary>
    /// Builds and returns a standalone subscriber client with the specified subscriptions.
    /// </summary>
    private static async Task<GlideClient> BuildStandaloneSubscriber(
        IEnumerable<PubSubMessage> messages,
        SubscribeMode mode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null
    )
    {
        // Validate arguments.
        if (messages.Any(m => m.ChannelMode == PubSubChannelMode.Sharded))
            throw new ArgumentException("Standalone clients do not support shard channel subscriptions.");

        if (mode != SubscribeMode.Config && callback != null)
            throw new ArgumentException("Callbacks are only supported for config-based subscriptions.");

        if (mode != SubscribeMode.Blocking && timeout != null)
            throw new ArgumentException("Timeouts are only supported for blocking subscriptions.");

        // Get channels and patterns.
        var targets = BuildSubscriptions(messages);
        var channels = targets[PubSubChannelMode.Exact];
        var patterns = targets[PubSubChannelMode.Pattern];

        var configBuilder = TestConfiguration.DefaultClientConfig();
        if (mode == SubscribeMode.Config)
        {
            var pubSubConfig = new StandalonePubSubSubscriptionConfig();

            foreach (var ch in channels) pubSubConfig.WithChannel(ch);
            foreach (var p in patterns) pubSubConfig.WithPattern(p);
            if (callback != null) pubSubConfig.WithCallback(callback);

            var config = configBuilder.WithPubSubSubscriptions(pubSubConfig).Build();
            return await GlideClient.CreateClient(config);
        }

        var client = await GlideClient.CreateClient(configBuilder.Build());

        if (mode == SubscribeMode.Lazy)
        {
            if (channels.Count > 0) await client.SubscribeLazyAsync(channels);
            if (patterns.Count > 0) await client.PSubscribeLazyAsync(patterns);
        }
        else
        {
            timeout ??= MaxDuration;
            if (channels.Count > 0) await client.SubscribeAsync(channels, timeout.Value);
            if (patterns.Count > 0) await client.PSubscribeAsync(patterns, timeout.Value);
        }

        return client;
    }
}
