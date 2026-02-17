// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Subscription mode for pub/sub integration tests.
/// </summary>
public enum SubscriptionMode
{
    /// <summary>
    /// Configuration subscription (during client initialization).
    /// </summary>
    Config,

    /// <summary>
    /// Lazy subscription (returns immediately without waiting for server confirmation).
    /// </summary>
    Lazy,

    /// <summary>
    /// Blocking subscription (waits for server confirmation).
    /// </summary>
    Blocking
}

/// <summary>
/// Utility methods for pub/sub integration tests.
/// </summary>
public static class PubSubUtils
{
    #region Test Data

    /// <summary>
    /// Theory data for parameterized tests that support both standalone and cluster modes.
    /// </summary>
    public static TheoryData<bool> IsCluster => [true, false];

    /// <summary>
    /// Theory data for all valid combinations of cluster mode, channel mode, and subscription mode.
    /// </summary>
    public static TheoryData<bool, PubSubChannelMode, SubscriptionMode> SubscriptionData
    {
        get
        {
            var data = new TheoryData<bool, PubSubChannelMode, SubscriptionMode>();
            foreach (bool isCluster in new[] { false, true })
                foreach (PubSubChannelMode channelMode in Enum.GetValues<PubSubChannelMode>())
                {
                    if (!isCluster && channelMode == PubSubChannelMode.Sharded)
                        continue;
                    foreach (SubscriptionMode subMode in Enum.GetValues<SubscriptionMode>())
                        data.Add(isCluster, channelMode, subMode);
                }
            return data;
        }
    }

    #endregion
    #region Constants

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
    public static bool IsShardedSupported()
        => TestConfiguration.IsVersionAtLeast("7.0.0");

    /// <summary>
    /// Returns true if sharded pub/sub is supported for the given cluster mode.
    /// </summary>
    public static bool IsShardedSupported(bool isCluster)
        => isCluster && TestConfiguration.IsVersionAtLeast("7.0.0");

    /// <summary>
    /// Skips the current test unless sharded pub/sub is supported.
    /// </summary>
    public static void SkipUnlessShardedSupported()
    {
        if (!IsShardedSupported())
        {
            Assert.Skip("Sharded pub/sub is supported since Valkey 7.0.0");
        }
    }

    /// <summary>
    /// Skips the current test unless sharded pub/sub is supported for the given cluster mode.
    /// </summary>
    public static void SkipUnlessShardedSupported(bool isCluster)
    {
        if (!isCluster)
        {
            Assert.Skip("Sharded pub/sub is not supported for standalone clients.");
            return;
        }

        SkipUnlessShardedSupported();
    }

    /// <summary>
    /// Skips the current test unless the given cluster and channel mode are supported.
    /// </summary>
    public static void SkipUnlessChannelModeSupported(bool isCluster, PubSubChannelMode channelMode)
    {
        if (channelMode != PubSubChannelMode.Sharded)
            return;

        SkipUnlessShardedSupported(isCluster);
    }

    #endregion
    #region Builders

    /// <summary>
    /// Returns a message appropriate for the given channel mode.
    /// </summary>
    public static PubSubMessage BuildMessage(PubSubChannelMode channelMode) => channelMode switch
    {
        PubSubChannelMode.Exact => BuildChannelMessage(),
        PubSubChannelMode.Pattern => BuildPatternMessage(),
        PubSubChannelMode.Sharded => BuildShardChannelMessage(),
        _ => throw new ArgumentOutOfRangeException(nameof(channelMode))
    };

    /// <summary>
    /// Builds a subscriber that will receive the specified messages using the given subscription mode.
    /// </summary>
    public static Task<BaseClient> BuildSubscriber(
        bool isCluster,
        PubSubChannelMode channelMode,
        SubscriptionMode subscriptionMode,
        IEnumerable<PubSubMessage> messages)
    {
        var targets = channelMode == PubSubChannelMode.Pattern
            ? messages.Select(m => m.Pattern!)
            : messages.Select(m => m.Channel);

        return channelMode switch
        {
            PubSubChannelMode.Exact => BuildSubscriber(isCluster, subscriptionMode, channels: targets),
            PubSubChannelMode.Pattern => BuildSubscriber(isCluster, subscriptionMode, patterns: targets),
            PubSubChannelMode.Sharded => BuildSubscriber(isCluster, subscriptionMode, shardChannels: targets),
            _ => throw new ArgumentOutOfRangeException(nameof(channelMode))
        };
    }

    /// <summary>
    /// Publishes the specified messages sequentially using the appropriate publish method for the channel mode.
    /// </summary>
    public static async Task PublishMessagesAsync(BaseClient publisher, PubSubChannelMode channelMode, IEnumerable<PubSubMessage> messages)
    {
        if (channelMode == PubSubChannelMode.Sharded)
        {
            if (publisher is not GlideClusterClient)
                throw new ArgumentException("Cluster client is required for publishing to shard channels.");

            foreach (var message in messages)
                await AssertSPublishAsync((GlideClusterClient)publisher, message);

            return;
        }

        foreach (var message in messages)
            await AssertPublishAsync(publisher, message);

        return;
    }

    /// <summary>
    /// Returns a unique exact channel name for testing.
    /// </summary>
    public static string BuildChannel()
    {
        return $"{{test:{Guid.NewGuid()}}}:channel";
    }

    /// <summary>
    /// Builds and returns a unique pattern and matching channel for testing.
    /// </summary>
    public static (string, string) BuildChannelAndPattern()
    {
        var id = Guid.NewGuid().ToString();
        return ($"{{test:{id}}}:channel", $"{{test:{id}}}:*");
    }

    /// <summary>
    /// Returns a unique exact channel message for testing.
    /// </summary>
    public static PubSubMessage BuildChannelMessage()
    {
        var id = Guid.NewGuid().ToString();
        var message = $"{{test:{id}}}:message";
        var channel = $"{{test:{id}}}:channel";

        return PubSubMessage.FromChannel(message, channel);
    }

    /// <summary>
    /// Returns a unique pattern channel message for testing.
    /// </summary>
    public static PubSubMessage BuildPatternMessage()
    {
        var id = Guid.NewGuid().ToString();
        var message = $"{{test:{id}}}:message";
        var channel = $"{{test:{id}}}:channel";
        var pattern = $"{{test:{id}}}:*";

        return PubSubMessage.FromPattern(message, channel, pattern);
    }

    /// <summary>
    /// Returns a unique shard channel message for testing.
    /// </summary>
    public static PubSubMessage BuildShardChannelMessage()
    {
        var id = Guid.NewGuid().ToString();
        var message = $"{{test:{id}}}:message";
        var channel = $"{{test:{id}}}:channel";

        return PubSubMessage.FromShardChannel(message, channel);
    }

    /// <summary>
    /// Builds and returns a client with the specified cluster mode.
    /// </summary>
    public static BaseClient BuildClient(bool isCluster)
        => isCluster ? BuildClusterClient() : BuildStandaloneClient();

    /// <summary>
    /// Builds and returns a standalone client.
    /// </summary>
    public static GlideClient BuildStandaloneClient()
        => TestConfiguration.DefaultStandaloneClient();

    /// <summary>
    /// Builds and returns a cluster client.
    /// </summary>
    public static GlideClusterClient BuildClusterClient()
        => TestConfiguration.DefaultClusterClient();

    /// <summary>
    /// Builds and returns a subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<BaseClient> BuildSubscriber(
        bool isCluster,
        SubscriptionMode mode = SubscriptionMode.Config,
        IEnumerable<string>? channels = null,
        IEnumerable<string>? patterns = null,
        IEnumerable<string>? shardChannels = null,
        MessageCallback? callback = null,
        TimeSpan? timeout = null)
    {
        // Validate parameters.
        if (!isCluster && shardChannels != null && shardChannels.Any())
            throw new ArgumentException("Shard channels are not supported for standalone clients.");

        if (mode != SubscriptionMode.Blocking && timeout != null)
            throw new ArgumentException("Timeout is only suported for blocking subscriptions.");

        return isCluster
            ? await BuildClusterSubscriber(mode, channels, patterns, shardChannels, callback, timeout)
            : await BuildStandaloneSubscriber(mode, channels, patterns, callback, timeout);
    }

    /// <summary>
    /// Builds and returns a standalone subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<GlideClient> BuildStandaloneSubscriber(
        SubscriptionMode mode = SubscriptionMode.Config,
        IEnumerable<string>? channels = null,
        IEnumerable<string>? patterns = null,
        MessageCallback? callback = null,
        TimeSpan? timeout = null)
    {
        var configBuilder = TestConfiguration.DefaultClientConfig();

        if (mode == SubscriptionMode.Config)
        {
            var pubSubConfig = new StandalonePubSubSubscriptionConfig();

            if (channels != null) foreach (var ch in channels) pubSubConfig.WithChannel(ch);
            if (patterns != null) foreach (var p in patterns) pubSubConfig.WithPattern(p);
            if (callback != null) pubSubConfig.WithCallback(callback);

            var config = configBuilder.WithPubSubSubscriptions(pubSubConfig).Build();
            return await GlideClient.CreateClient(config);
        }

        var client = await GlideClient.CreateClient(configBuilder.Build());

        if (mode == SubscriptionMode.Lazy)
        {
            if (channels != null && channels.Any()) await client.SubscribeLazyAsync(channels);
            if (patterns != null && patterns.Any()) await client.PSubscribeLazyAsync(patterns);
        }
        else
        {
            timeout ??= MaxDuration;
            if (channels != null && channels.Any()) await client.SubscribeAsync(channels, timeout.Value);
            if (patterns != null && patterns.Any()) await client.PSubscribeAsync(patterns, timeout.Value);
        }

        return client;
    }

    /// <summary>
    /// Builds and returns a cluster subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<GlideClusterClient> BuildClusterSubscriber(
        SubscriptionMode mode = SubscriptionMode.Config,
        IEnumerable<string>? channels = null,
        IEnumerable<string>? patterns = null,
        IEnumerable<string>? shardChannels = null,
        MessageCallback? callback = null,
        TimeSpan? timeout = null)
    {
        var configBuilder = TestConfiguration.DefaultClusterClientConfig();

        if (mode == SubscriptionMode.Config)
        {
            var pubSubConfig = new ClusterPubSubSubscriptionConfig();

            if (channels != null) foreach (var ch in channels) pubSubConfig.WithChannel(ch);
            if (patterns != null) foreach (var p in patterns) pubSubConfig.WithPattern(p);
            if (shardChannels != null) foreach (var ch in shardChannels) pubSubConfig.WithShardChannel(ch);
            if (callback != null) pubSubConfig.WithCallback(callback);

            var clientConfig = configBuilder.WithPubSubSubscriptions(pubSubConfig).Build();
            return await GlideClusterClient.CreateClient(clientConfig);
        }

        var client = await GlideClusterClient.CreateClient(configBuilder.Build());

        if (mode == SubscriptionMode.Lazy)
        {
            if (channels != null && channels.Any()) await client.SubscribeLazyAsync(channels);
            if (patterns != null && patterns.Any()) await client.PSubscribeLazyAsync(patterns);
            if (shardChannels != null && shardChannels.Any()) await client.SSubscribeLazyAsync(shardChannels);
        }
        else
        {
            timeout ??= MaxDuration;
            if (channels != null && channels.Any()) await client.SubscribeAsync(channels, timeout.Value);
            if (patterns != null && patterns.Any()) await client.PSubscribeAsync(patterns, timeout.Value);
            if (shardChannels != null && shardChannels.Any()) await client.SSubscribeAsync(shardChannels, timeout.Value);
        }

        return client;
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
    #region Assertions

    /// <summary>
    /// Asserts that publishing the specified message results in at least one subscriber receiving it.
    /// </summary>
    public static async Task AssertPublishAsync(BaseClient client, PubSubMessage message)
    {
        // Retry until published or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            if (await client.PublishAsync(message.Channel, message.Message) > 0L)
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least one subscriber for channel '{message.Channel}'.");
    }

    /// <summary>
    /// Asserts that publishing the specified message to a shard channel results in at least one subscriber receiving it.
    /// </summary>
    public static async Task AssertSPublishAsync(GlideClusterClient client, PubSubMessage message)
    {
        // Retry until published or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            if (await client.SPublishAsync(message.Channel, message.Message) > 0L)
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least one subscriber for shard channel '{message.Channel}'");
    }

    /// <summary>
    /// Asserts that the client is subscribed to receive the specified messages over the given channel mode.
    /// </summary>
    public static Task AssertSubscribedAsync(BaseClient client, PubSubChannelMode channelMode, IEnumerable<PubSubMessage> messages)
    {
        var targets = channelMode == PubSubChannelMode.Pattern
            ? messages.Select(m => m.Pattern!)
            : messages.Select(m => m.Channel);

        return channelMode switch
        {
            PubSubChannelMode.Exact => AssertSubscribedAsync(client, targets),
            PubSubChannelMode.Pattern => AssertPSubscribedAsync(client, targets),
            PubSubChannelMode.Sharded => client is GlideClusterClient clusterClient
                ? AssertSSubscribedAsync(clusterClient, targets)
                : throw new ArgumentException("Cluster client is required for shard channel subscriptions."),
            _ => throw new ArgumentOutOfRangeException(nameof(channelMode))
        };
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified targets based on the given channel mode.
    /// </summary>
    public static Task AssertSubscribedAsync(BaseClient client, PubSubChannelMode channelMode, IEnumerable<string> targets)
    {
        return channelMode switch
        {
            PubSubChannelMode.Exact => AssertSubscribedAsync(client, targets),
            PubSubChannelMode.Pattern => AssertPSubscribedAsync(client, targets),
            PubSubChannelMode.Sharded => client is GlideClusterClient clusterClient
                ? AssertSSubscribedAsync(clusterClient, targets)
                : throw new ArgumentException("Cluster client is required for asserting shard channel subscriptions."),
            _ => throw new ArgumentOutOfRangeException(nameof(channelMode))
        };
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified channels.
    /// </summary>
    public static async Task AssertSubscribedAsync(BaseClient client, IEnumerable<string> channels)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        var channelSet = channels.ToHashSet();
        while (!cts.Token.IsCancellationRequested)
        {
            var subscriptions = await client.GetSubscriptionsAsync();
            var actualChannels = subscriptions.Actual[PubSubChannelMode.Exact];

            if (channelSet.IsSubsetOf(actualChannels))
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least one subscriber for channels '{string.Join("', '", channels)}'");
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified patterns.
    /// </summary>
    public static async Task AssertPSubscribedAsync(BaseClient client, IEnumerable<string> patterns)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        var patternSet = patterns.ToHashSet();
        while (!cts.Token.IsCancellationRequested)
        {
            var subscriptions = await client.GetSubscriptionsAsync();
            var actualPatterns = subscriptions.Actual[PubSubChannelMode.Pattern];

            if (patternSet.IsSubsetOf(actualPatterns))
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least one subscriber for patterns '{string.Join("', '", patterns)}'");
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified shard channels.
    /// </summary>
    public static async Task AssertSSubscribedAsync(GlideClusterClient client, IEnumerable<string> shardChannels)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        var shardChannelSet = shardChannels.ToHashSet();
        while (!cts.Token.IsCancellationRequested)
        {
            var subscriptions = await client.GetSubscriptionsAsync();
            var actualShardChannels = subscriptions.Actual[PubSubChannelMode.Sharded];

            if (shardChannelSet.IsSubsetOf(actualShardChannels))
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least one subscriber for shard channels '{string.Join("', '", shardChannels)}'");
    }

    /// <summary>
    /// Asserts that there are no subscribers to each of the specified channels.
    /// If no channels are specified, asserts that there are no subscribers to any channels.
    /// </summary>
    public static async Task AssertNotSubscribedAsync(BaseClient client, IEnumerable<string> channels)
    {
        // Retry until unsubscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        var channelSet = channels.ToHashSet();
        while (!cts.Token.IsCancellationRequested)
        {
            var subscriptions = await client.GetSubscriptionsAsync();
            var actualChannels = subscriptions.Actual[PubSubChannelMode.Exact];

            if (channelSet.Count == 0)
            {
                if (actualChannels.Count == 0)
                    return;
            }
            else
            {
                if (!actualChannels.Overlaps(channelSet))
                    return;
            }

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected zero subscribers for channels '{string.Join("', '", channels)}'");
    }

    /// <summary>
    /// Asserts that there are no subscribers to each of the specified patterns.
    /// If no patterns are specified, asserts that there are no subscribers to any patterns.
    /// </summary>
    public static async Task AssertNotPSubscribedAsync(BaseClient client, IEnumerable<string> patterns)
    {
        // Retry until unsubscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        var patternSet = patterns.ToHashSet();
        while (!cts.Token.IsCancellationRequested)
        {
            var subscriptions = await client.GetSubscriptionsAsync();
            var actualPatterns = subscriptions.Actual[PubSubChannelMode.Pattern];

            if (patternSet.Count == 0)
            {
                if (actualPatterns.Count == 0)
                    return;
            }
            else
            {
                if (!actualPatterns.Overlaps(patternSet))
                    return;
            }

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected zero subscribers for patterns '{string.Join("', '", patterns)}'");
    }

    /// <summary>
    /// Asserts that there are no subscribers to each of the specified shard channels.
    /// If no shard channels are specified, asserts that there are no subscribers to any shard channels.
    /// </summary>
    public static async Task AssertNotSSubscribedAsync(GlideClusterClient client, IEnumerable<string> shardChannels)
    {
        // Retry until unsubscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        var shardChannelSet = shardChannels.ToHashSet();
        while (!cts.Token.IsCancellationRequested)
        {
            var subscriptions = await client.GetSubscriptionsAsync();
            var actualShardChannels = subscriptions.Actual[PubSubChannelMode.Sharded];

            if (shardChannelSet.Count == 0)
            {
                if (actualShardChannels.Count == 0)
                    return;
            }
            else
            {
                if (!actualShardChannels.Overlaps(shardChannelSet))
                    return;
            }

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected zero subscribers for shard channels '{string.Join("', '", shardChannels)}'");
    }

    /// <summary>
    /// Asserts that the specified messages have been received by the given client.
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

    // TODO pub/sub: remove?
    /// <summary>
    /// Asserts that the specified message has not been received by the given client.
    /// </summary>
    public static Task AssertNotReceivedAsync(BaseClient client, PubSubMessage expected)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        queue.TryGetMessage(out PubSubMessage? received);
        if (received == null) return Task.CompletedTask;

        Assert.NotEqual(expected.Message, received.Message);
        return Task.CompletedTask;
    }

    #endregion
}
