// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Utility methods for pub/sub integration tests.
/// </summary>
public static class PubSubUtils
{
    #region Enums

    /// <summary>
    /// Unsubscribe mode for pub/sub integration tests.
    /// </summary>
    public enum UnsubscribeMode
    {
        Lazy,
        Blocking
    }

    /// <summary>
    /// Subscribe mode for pub/sub integration tests.
    /// </summary>
    public enum SubscribeMode
    {
        Config,
        Lazy,
        Blocking
    }

    #endregion
    #region Data

    /// <summary>
    /// Theory data for all valid combinations of cluster mode and channel mode.
    /// </summary>
    public static TheoryData<bool, PubSubChannelMode> ClusterAndChannelModeData
    {
        get
        {
            var data = new TheoryData<bool, PubSubChannelMode>();
            foreach (var isCluster in Data.ClusterMode)
            {
                foreach (PubSubChannelMode channelMode in Enum.GetValues<PubSubChannelMode>())
                {
                    if (!IsClusterAndChannelModeSupported(isCluster, channelMode))
                    {
                        continue;
                    }

                    data.Add(isCluster, channelMode);
                }
            }

            return data;
        }
    }

    /// <summary>
    /// Theory data for all valid combinations of cluster mode and subscribe mode.
    /// </summary>
    public static TheoryData<bool, SubscribeMode> ClusterAndSubscribeModeData
    {
        get
        {
            var data = new TheoryData<bool, SubscribeMode>();
            foreach (var isCluster in Data.ClusterMode)
            {
                foreach (SubscribeMode subscribeMode in Enum.GetValues<SubscribeMode>())
                {
                    data.Add(isCluster, subscribeMode);
                }
            }

            return data;
        }
    }

    /// <summary>
    /// Theory data for all valid combinations of cluster mode and unsubscribe mode.
    /// </summary>
    public static TheoryData<bool, UnsubscribeMode> ClusterAndUnsubscribeModeData
    {
        get
        {
            var data = new TheoryData<bool, UnsubscribeMode>();
            foreach (var isCluster in Data.ClusterMode)
            {
                foreach (UnsubscribeMode unsubscribeMode in Enum.GetValues<UnsubscribeMode>())
                {
                    data.Add(isCluster, unsubscribeMode);
                }
            }

            return data;
        }
    }

    /// <summary>
    /// Theory data for all valid combinations of cluster mode, channel mode, and subscribe mode.
    /// </summary>
    public static TheoryData<bool, PubSubChannelMode, SubscribeMode> ClusterChannelAndSubscribeModeData
    {
        get
        {
            var data = new TheoryData<bool, PubSubChannelMode, SubscribeMode>();
            foreach (var isCluster in Data.ClusterMode)
            {
                foreach (PubSubChannelMode channelMode in Enum.GetValues<PubSubChannelMode>())
                {
                    if (!IsClusterAndChannelModeSupported(isCluster, channelMode))
                    {
                        continue;
                    }

                    foreach (SubscribeMode subscribeMode in Enum.GetValues<SubscribeMode>())
                    {
                        data.Add(isCluster, channelMode, subscribeMode);
                    }
                }
            }

            return data;
        }
    }

    /// <summary>
    /// Theory data for all valid combinations of cluster mode, channel mode, and unsubscribe mode.
    /// </summary>
    public static TheoryData<bool, PubSubChannelMode, UnsubscribeMode> ClusterChannelAndUnsubscribeModeData
    {
        get
        {
            var data = new TheoryData<bool, PubSubChannelMode, UnsubscribeMode>();
            foreach (var isCluster in Data.ClusterMode)
            {
                foreach (PubSubChannelMode channelMode in Enum.GetValues<PubSubChannelMode>())
                {
                    if (!IsClusterAndChannelModeSupported(isCluster, channelMode))
                    {
                        continue;
                    }

                    foreach (UnsubscribeMode unsubscribeMode in Enum.GetValues<UnsubscribeMode>())
                    {
                        data.Add(isCluster, channelMode, unsubscribeMode);
                    }
                }
            }

            return data;
        }
    }

    /// <summary>
    /// Theory data for all valid combinations of cluster mode, channel mode, subscribe mode, and unsubscribe mode.
    /// </summary>
    public static TheoryData<bool, PubSubChannelMode, SubscribeMode, UnsubscribeMode> AllModesData
    {
        get
        {
            var data = new TheoryData<bool, PubSubChannelMode, SubscribeMode, UnsubscribeMode>();
            foreach (var isCluster in Data.ClusterMode)
            {
                foreach (PubSubChannelMode channelMode in Enum.GetValues<PubSubChannelMode>())
                {
                    if (!IsClusterAndChannelModeSupported(isCluster, channelMode))
                    {
                        continue;
                    }

                    foreach (SubscribeMode subscribeMode in Enum.GetValues<SubscribeMode>())
                    {
                        foreach (UnsubscribeMode unsubscribeMode in Enum.GetValues<UnsubscribeMode>())
                        {
                            data.Add(isCluster, channelMode, subscribeMode, unsubscribeMode);
                        }
                    }
                }
            }

            return data;
        }
    }

    #endregion
    #region TimeSpans

    /// <summary>
    /// Maximum duration for pub/sub assertions.
    /// </summary>
    public static readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Retry interval for pub/sub assertions.
    /// </summary>
    public static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(0.5);

    #endregion
    #region Helpers

    /// <summary>
    /// Returns true if sharded pub/sub is supported.
    /// </summary>
    public static bool IsShardedSupported()
        => TestConfiguration.IsVersionAtLeast("7.0.0");

    /// <summary>
    /// Returns true if sharded pub/sub is supported for the given cluster mode.
    /// </summary>
    public static bool IsShardedSupported(bool isCluster)
        => isCluster && IsShardedSupported();

    /// <summary>
    /// Returns true if the given cluster mode and channel mode combination is supported.
    /// </summary>
    private static bool IsClusterAndChannelModeSupported(bool isCluster, PubSubChannelMode channelMode)
        => channelMode != PubSubChannelMode.Sharded || IsShardedSupported(isCluster);

    /// <summary>
    /// Skips the current test unless sharded pub/sub is supported.
    /// </summary>
    public static void SkipUnlessShardedSupported()
    {
        if (!IsShardedSupported())
        {
            Assert.Skip("Sharded pub/sub is supported for cluster clients since Valkey 7.0.0");
        }
    }

    #endregion
    #region Builders

    /// <summary>
    /// Builds and returns a unique channel for testing.
    /// </summary>
    public static ValkeyKey BuildChannel()
        => BuildChannel(Guid.NewGuid().ToString());

    /// <summary>
    /// Builds and returns a unique channel for testing from the given ID.
    /// </summary>
    private static ValkeyKey BuildChannel(string id)
        => (ValkeyKey)$"test:{{{id}}}:channel";

    /// <summary>
    /// Builds and returns a unique pattern for testing.
    /// </summary>
    public static ValkeyKey BuildPattern()
        => BuildPattern(Guid.NewGuid().ToString());

    /// <summary>
    /// Builds and returns a unique pattern for testing from the given ID.
    /// </summary>
    private static ValkeyKey BuildPattern(string id)
        => (ValkeyKey)$"test:{{{id}}}:*";

    /// <summary>
    /// Returns a message appropriate for the given channel mode.
    /// </summary>
    public static PubSubMessage BuildMessage(PubSubChannelMode channelMode = PubSubChannelMode.Exact)
    {
        Guid id = Guid.NewGuid();

        ValkeyKey channel = BuildChannel(id.ToString());
        ValkeyKey pattern = BuildPattern(id.ToString());
        string message = $"{{test:{id}}}:message";

        return channelMode switch
        {
            PubSubChannelMode.Exact => PubSubMessage.FromChannel(message, channel),
            PubSubChannelMode.Pattern => PubSubMessage.FromPattern(message, channel, pattern),
            PubSubChannelMode.Sharded => PubSubMessage.FromShardedChannel(message, channel),
            _ => throw new ArgumentOutOfRangeException(nameof(channelMode))
        };
    }

    /// <summary>
    /// Builds and returns the expected subscriptions for the given messages, indexed by channel mode.
    /// </summary>
    public static Dictionary<PubSubChannelMode, HashSet<ValkeyKey>> BuildSubscriptions(IEnumerable<PubSubMessage> messages)
    {
        var targets = new Dictionary<PubSubChannelMode, HashSet<ValkeyKey>>();

        foreach (PubSubChannelMode mode in Enum.GetValues<PubSubChannelMode>())
        {
            targets[mode] = [];
        }

        foreach (PubSubMessage message in messages)
        {
            ValkeyKey target =
                message.ChannelMode == PubSubChannelMode.Pattern
                ? message.Pattern!.Value
                : message.Channel;

            _ = targets[message.ChannelMode].Add(target);
        }
        return targets;
    }

    /// <summary>
    /// Builds and returns a publisher client with the specified cluster mode.
    /// </summary>
    public static BaseClient BuildPublisher(bool isCluster) => isCluster
            ? TestConfiguration.DefaultClusterClient()
            : TestConfiguration.DefaultStandaloneClient();

    /// <summary>
    /// Builds and returns a client that is subscribed to receive the specified message using the given subscription mode.
    /// </summary>
    public static async Task<BaseClient> BuildSubscriber(
        bool isCluster,
        PubSubMessage? message = null,
        SubscribeMode subscribeMode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null)
        => await BuildSubscriber(isCluster, message != null ? [message] : [], subscribeMode, callback, timeout);

    /// <summary>
    /// Builds and returns a client that is subscribed to receive the specified messages using the given subscription mode.
    /// </summary>
    public static async Task<BaseClient> BuildSubscriber(
        bool isCluster,
        IEnumerable<PubSubMessage> messages,
        SubscribeMode subscribeMode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null) => isCluster
            ? await BuildClusterSubscriber(messages, subscribeMode, callback, timeout)
            : await BuildStandaloneSubscriber(messages, subscribeMode, callback, timeout);

    /// <summary>
    /// Builds and returns a cluster subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<GlideClusterClient> BuildClusterSubscriber(
        PubSubMessage messages,
        SubscribeMode subscribeMode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null
    ) => await BuildClusterSubscriber([messages], subscribeMode, callback, timeout);

    /// <summary>
    /// Builds and returns a cluster subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<GlideClusterClient> BuildClusterSubscriber(
        IEnumerable<PubSubMessage> messages,
        SubscribeMode subscribeMode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null
    )
    {
        // Get channels, patterns, and sharded channels.
        var targets = BuildSubscriptions(messages);
        var channels = targets[PubSubChannelMode.Exact];
        var patterns = targets[PubSubChannelMode.Pattern];
        var shardedChannels = targets[PubSubChannelMode.Sharded];

        // Build configuration.
        ConnectionConfiguration.ClusterClientConfigurationBuilder configBuilder = TestConfiguration.DefaultClusterClientConfig();

        if (subscribeMode == SubscribeMode.Config)
        {
            ClusterPubSubSubscriptionConfig pubSubConfig = new();

            foreach (var ch in channels)
            {
                _ = pubSubConfig.WithChannel(ch);
            }

            foreach (var p in patterns)
            {
                _ = pubSubConfig.WithPattern(p);
            }

            foreach (var sc in shardedChannels)
            {
                _ = pubSubConfig.WithShardedChannel(sc);
            }

            if (callback != null)
            {
                _ = pubSubConfig.WithCallback(callback);
            }

            _ = configBuilder.WithPubSubSubscriptions(pubSubConfig);
        }

        else if (callback != null)
        {
            ClusterPubSubSubscriptionConfig pubSubConfig = new();
            _ = pubSubConfig.WithCallback(callback);

            _ = configBuilder.WithPubSubSubscriptions(pubSubConfig);
        }

        // Build client.
        GlideClusterClient client = await GlideClusterClient.CreateClient(configBuilder.Build());

        // Dynamically subscribe.
        if (subscribeMode == SubscribeMode.Lazy)
        {
            if (channels.Count > 0)
            {
                await client.SubscribeLazyAsync(channels);
            }

            if (patterns.Count > 0)
            {
                await client.PSubscribeLazyAsync(patterns);
            }

            if (shardedChannels.Count > 0)
            {
                await client.SSubscribeLazyAsync(shardedChannels);
            }
        }
        else if (subscribeMode == SubscribeMode.Blocking)
        {
            timeout ??= MaxDuration;
            if (channels.Count > 0)
            {
                await client.SubscribeAsync(channels, timeout.Value);
            }

            if (patterns.Count > 0)
            {
                await client.PSubscribeAsync(patterns, timeout.Value);
            }

            if (shardedChannels.Count > 0)
            {
                await client.SSubscribeAsync(shardedChannels, timeout.Value);
            }
        }

        return client;
    }

    /// <summary>
    /// Builds and returns a standalone subscriber client with the specified subscriptions.
    /// </summary>
    private static async Task<GlideClient> BuildStandaloneSubscriber(
        IEnumerable<PubSubMessage> messages,
        SubscribeMode subscribeMode = SubscribeMode.Config,
        MessageCallback? callback = null,
        TimeSpan? timeout = null)
    {
        // Get channels and patterns.
        var targets = BuildSubscriptions(messages);

        if (targets[PubSubChannelMode.Sharded].Count > 0)
        {
            throw new ArgumentException("Standalone clients do not support sharded channel subscriptions.");
        }

        var channels = targets[PubSubChannelMode.Exact];
        var patterns = targets[PubSubChannelMode.Pattern];

        // Build configuration.
        ConnectionConfiguration.StandaloneClientConfigurationBuilder configBuilder = TestConfiguration.DefaultClientConfig();

        if (subscribeMode == SubscribeMode.Config)
        {
            StandalonePubSubSubscriptionConfig pubSubConfig = new();

            foreach (var ch in channels)
            {
                _ = pubSubConfig.WithChannel(ch);
            }

            foreach (var p in patterns)
            {
                _ = pubSubConfig.WithPattern(p);
            }

            if (callback != null)
            {
                _ = pubSubConfig.WithCallback(callback);
            }

            _ = configBuilder.WithPubSubSubscriptions(pubSubConfig);
        }

        else if (callback != null)
        {
            StandalonePubSubSubscriptionConfig pubSubConfig = new StandalonePubSubSubscriptionConfig().WithCallback(callback);
            _ = configBuilder.WithPubSubSubscriptions(pubSubConfig);
        }

        // Build client.
        GlideClient client = await GlideClient.CreateClient(configBuilder.Build());

        // Dynamically subscribe.
        if (subscribeMode == SubscribeMode.Lazy)
        {
            if (channels.Count > 0)
            {
                await client.SubscribeLazyAsync(channels);
            }

            if (patterns.Count > 0)
            {
                await client.PSubscribeLazyAsync(patterns);
            }
        }
        else if (subscribeMode == SubscribeMode.Blocking)
        {
            timeout ??= MaxDuration;
            if (channels.Count > 0)
            {
                await client.SubscribeAsync(channels, timeout.Value);
            }

            if (patterns.Count > 0)
            {
                await client.PSubscribeAsync(patterns, timeout.Value);
            }
        }

        return client;
    }

    /// <summary>
    /// Builds and returns a Valkey server based on cluster mode.
    /// </summary>
    public static Server BuildServer(bool isCluster) => isCluster
            ? new ClusterServer()
            : new StandaloneServer();

    #endregion
    #region Publish

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
        foreach (PubSubMessage message in messages)
        {
            _ = await (message.ChannelMode switch
            {
                PubSubChannelMode.Exact => publisher.PublishAsync(message.Channel, message.Message),
                PubSubChannelMode.Pattern => publisher.PublishAsync(message.Channel, message.Message),
                PubSubChannelMode.Sharded => ((GlideClusterClient)publisher).SPublishAsync(message.Channel, message.Message),
                _ => throw new InvalidOperationException($"Unsupported channel mode: {message.ChannelMode}")
            });
        }
    }

    #endregion
    #region Subscribe

    /// <summary>
    /// Subscribes the client to receive the given message and waits for server confirmation.
    /// </summary>
    public static async Task SubscribeAsync(BaseClient subscriber, PubSubMessage message)
        => await SubscribeAsync(subscriber, [message]);

    /// <summary>
    /// Subscribes the client to receive the given messages and waits for server confirmation.
    /// </summary>
    public static async Task SubscribeAsync(BaseClient subscriber, IEnumerable<PubSubMessage> messages)
    {
        var subscriptions = BuildSubscriptions(messages);

        if (subscriptions[PubSubChannelMode.Exact].Count > 0)
        {
            await subscriber.SubscribeAsync(subscriptions[PubSubChannelMode.Exact]);
        }

        if (subscriptions[PubSubChannelMode.Pattern].Count > 0)
        {
            await subscriber.PSubscribeAsync(subscriptions[PubSubChannelMode.Pattern]);
        }

        if (subscriptions[PubSubChannelMode.Sharded].Count > 0)
        {
            await ((GlideClusterClient)subscriber).SSubscribeAsync(subscriptions[PubSubChannelMode.Sharded]);
        }
    }

    /// <summary>
    /// Subscribes the client to receive the given message and return without waiting for server confirmation.
    /// </summary>
    public static async Task SubscribeLazyAsync(BaseClient subscriber, PubSubMessage message)
        => await SubscribeLazyAsync(subscriber, [message]);

    /// <summary>
    /// Subscribes the client to receive the given messages and return without waiting for server confirmation.
    /// </summary>
    public static async Task SubscribeLazyAsync(BaseClient subscriber, IEnumerable<PubSubMessage> messages)
    {
        var subscriptions = BuildSubscriptions(messages);

        if (subscriptions[PubSubChannelMode.Exact].Count > 0)
        {
            await subscriber.SubscribeLazyAsync(subscriptions[PubSubChannelMode.Exact]);
        }

        if (subscriptions[PubSubChannelMode.Pattern].Count > 0)
        {
            await subscriber.PSubscribeLazyAsync(subscriptions[PubSubChannelMode.Pattern]);
        }

        if (subscriptions[PubSubChannelMode.Sharded].Count > 0)
        {
            await ((GlideClusterClient)subscriber).SSubscribeLazyAsync(subscriptions[PubSubChannelMode.Sharded]);
        }
    }

    #endregion
    #region Unsubscribe

    /// <summary>
    /// Unsubscribes the client with the using the given mode from receiving the specified message.
    /// </summary>
    public static async Task UnsubscribeAsync(BaseClient subscriber, PubSubMessage message, UnsubscribeMode unsubscribeMode)
        => await UnsubscribeAsync(subscriber, [message], unsubscribeMode);

    /// <summary>
    /// Unsubscribes the client with the using the given mode from receiving the specified messages.
    /// </summary>
    public static async Task UnsubscribeAsync(BaseClient subscriber, IEnumerable<PubSubMessage> messages, UnsubscribeMode unsubscribeMode)
    {
        var unsubscriptionTargets = BuildSubscriptions(messages);

        // Unsubscribe from channels by mode.
        foreach (var item in unsubscriptionTargets)
        {
            var channels = item.Value;

            // Skip if no channels/patterns for this mode,
            // since an empty collection will unsuscribe from all.
            if (channels.Count == 0)
            {
                continue;
            }

            PubSubChannelMode channelMode = item.Key;
            await (channelMode switch
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
            });
        }
    }

    #endregion
    #region Assertions

    /// <summary>
    /// Asserts that the client is subscribed to receive the specified message.
    /// </summary>
    public static async Task AssertSubscribedAsync(BaseClient client, PubSubMessage message, SubscribeMode? mode = null)
        => await AssertSubscribedAsync(client, [message], mode);

    /// <summary>
    /// Asserts that the client is subscribed to receive the specified messages.
    /// </summary>
    public static async Task AssertSubscribedAsync(BaseClient client, IEnumerable<PubSubMessage> messages, SubscribeMode? mode = null)
    {
        var expectedSubscriptions = BuildSubscriptions(messages);

        // If subscribe mode is not specified, blocking, or config, expect subscription on first attempt (no retries).
        if (mode is null or SubscribeMode.Blocking or SubscribeMode.Config)
        {
            if (!await IsSubscribedAsync(client, expectedSubscriptions))
            {
                Assert.Fail("Expected subscriptions not found.");
            }

            return;
        }

        // For lazy mode, retry until subscribed or timeout occurs.
        using CancellationTokenSource cts = new(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            if (await IsSubscribedAsync(client, expectedSubscriptions))
            {
                return;
            }

            await Task.Delay(RetryInterval);
        }

        Assert.Fail("Expected subscriptions not found.");
    }

    /// <summary>543
    /// Asserts that the client is not subscribed to receive the specified message.
    /// </summary>
    public static async Task AssertNotSubscribedAsync(BaseClient client, PubSubMessage message, UnsubscribeMode mode)
        => await AssertNotSubscribedAsync(client, [message], mode);

    /// <summary>
    /// Asserts that the client is not subscribed to receive the specified messages.
    /// </summary>
    public static async Task AssertNotSubscribedAsync(BaseClient client, IEnumerable<PubSubMessage> messages, UnsubscribeMode mode)
    {
        var notExpected = BuildSubscriptions(messages);

        // If unsubscribe mode is blocking, expect unsubscription on first attempt (no retries).
        if (mode == UnsubscribeMode.Blocking)
        {
            if (!await IsNotSubscribedAsync(client, notExpected))
            {
                Assert.Fail("Unexpected subscriptions found.");
            }

            return;
        }

        // For lazy mode, retry until unsubscribed or timeout occurs.
        using CancellationTokenSource cts = new(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            if (await IsNotSubscribedAsync(client, notExpected))
            {
                return;
            }

            await Task.Delay(RetryInterval);
        }

        Assert.Fail("Unexpected subscriptions found.");
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

        try
        {
            HashSet<PubSubMessage> received = [];
            using CancellationTokenSource cts = new(MaxDuration);

            await foreach (PubSubMessage message in queue.GetMessagesAsync(cts.Token))
            {
                _ = received.Add(message);

                if (received.Count >= expected.Count())
                {
                    // Use set comparison because messages may be received out-of-order.
                    if (received.SetEquals(expected))
                    {
                        return;
                    }

                    Assert.Fail("Unexpected messages received.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Assert.Fail("Expected messages were not received within the timeout period.");
        }
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

        while (queue.TryGetMessage(out PubSubMessage? received))
        {
            Assert.DoesNotContain(received, expected);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns true if the client is subscribed to the specified channels, patterns, and sharded channels.
    /// </summary>
    private static async Task<bool> IsSubscribedAsync(BaseClient client, Dictionary<PubSubChannelMode, HashSet<ValkeyKey>> expected)
    {
        var actual = (await client.GetSubscriptionsAsync()).Actual;

        foreach (var item in expected)
        {
            var channelMode = item.Key;
            var expectedChannels = item.Value;
            var actualChannels = actual[channelMode];

            // Check that all expected channels are present in the actual set.
            foreach (var ch in expectedChannels)
            {
                if (!actualChannels.Contains(ch))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Returns true if the client is not subscribed to the specified channels, patterns, and sharded channels.
    /// </summary>
    private static async Task<bool> IsNotSubscribedAsync(BaseClient client, Dictionary<PubSubChannelMode, HashSet<ValkeyKey>> notExpected)
    {
        var actual = (await client.GetSubscriptionsAsync()).Actual;

        foreach (var item in notExpected)
        {
            var channelMode = item.Key;
            var notExpectedChannels = item.Value;
            var actualChannels = actual[channelMode];

            // Check that none of the not-expected channels are present in the actual set.
            foreach (var ch in notExpectedChannels)
            {
                if (actualChannels.Contains(ch))
                {
                    return false;
                }
            }
        }

        return true;
    }

    #endregion
}
