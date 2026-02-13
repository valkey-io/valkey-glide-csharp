// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Utility methods for pub/sub integration tests.
/// </summary>
public static class PubSubUtils
{
    #region Constants

    /// <summary>
    /// Maximum duration for pub/sub assertions.
    /// </summary>
    public static readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Retry interval for pub/sub assertions.
    /// </summary>
    public static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// Skip message for tests requiring sharded PubSub support.
    /// </summary>
    public const string SkipShardedPubSubMessage = "Sharded PubSub is supported since 7.0.0";

    /// <summary>
    /// Theory data for parameterized tests that support both standalone and cluster modes.
    /// </summary>
    public static TheoryData<bool> IsCluster => [true, false];

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

    #endregion
    #region Builders

    /// <summary>
    /// Returns a unique exact channel name for testing.
    /// </summary>
    public static string BuildChannel()
    {
        return $"test-{Guid.NewGuid()}-channel";
    }

    /// <summary>
    /// Builds and returns a unique pattern and matching channel for testing.
    /// </summary>
    public static (string, string) BuildChannelAndPattern()
    {
        var id = Guid.NewGuid().ToString();
        return ($"test-{id}-channel", $"test-{id}-*");
    }

    /// <summary>
    /// Returns a unique exact channel message for testing.
    /// </summary>
    public static PubSubMessage BuildChannelMessage()
    {
        var id = Guid.NewGuid().ToString();
        var message = $"test-{id}-message";
        var channel = $"test-{id}-channel";

        return PubSubMessage.FromChannel(message, channel);
    }

    /// <summary>
    /// Returns a unique pattern channel message for testing.
    /// </summary>
    public static PubSubMessage BuildPatternMessage()
    {
        var id = Guid.NewGuid().ToString();
        var message = $"test-{id}-message";
        var channel = $"test-{id}-channel";
        var pattern = $"test-{id}-*";

        return PubSubMessage.FromPattern(message, channel, pattern);
    }

    /// <summary>
    /// Returns a unique shard channel message for testing.
    /// </summary>
    public static PubSubMessage BuildShardChannelMessage()
    {
        var id = Guid.NewGuid().ToString();
        var message = $"test-{id}-message";
        var channel = $"test-{id}-channel";

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
        IEnumerable<string>? channels = null,
        IEnumerable<string>? patterns = null,
        IEnumerable<string>? shardChannels = null,
        MessageCallback? callback = null)
    {
        if (isCluster)
            return await BuildClusterSubscriber(channels, patterns, shardChannels, callback);

        if (shardChannels != null && shardChannels.Any())
            throw new ArgumentException("Shard channels are not supported for standalone clients.");

        return await BuildStandaloneSubscriber(channels, patterns, callback);
    }

    /// <summary>
    /// Builds and returns a standalone subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<GlideClient> BuildStandaloneSubscriber(
        IEnumerable<string>? channels = null,
        IEnumerable<string>? patterns = null,
        MessageCallback? callback = null)
    {
        var config = new StandalonePubSubSubscriptionConfig();

        if (channels != null) foreach (var ch in channels) config.WithChannel(ch);
        if (patterns != null) foreach (var p in patterns) config.WithPattern(p);
        if (callback != null) config.WithCallback(callback);

        var clientConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(config)
            .Build();

        return await GlideClient.CreateClient(clientConfig);
    }

    /// <summary>
    /// Builds and returns a cluster subscriber client with the specified subscriptions.
    /// </summary>
    public static async Task<GlideClusterClient> BuildClusterSubscriber(
        IEnumerable<string>? channels = null,
        IEnumerable<string>? patterns = null,
        IEnumerable<string>? shardChannels = null,
        MessageCallback? callback = null)
    {
        var config = new ClusterPubSubSubscriptionConfig();

        if (channels != null) foreach (var ch in channels) config.WithChannel(ch);
        if (patterns != null) foreach (var p in patterns) config.WithPattern(p);
        if (shardChannels != null) foreach (var ch in shardChannels) config.WithShardChannel(ch);
        if (callback != null) config.WithCallback(callback);

        var clientConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(config)
            .Build();

        return await GlideClusterClient.CreateClient(clientConfig);
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
    public static async Task AssertMessagesReceivedAsync(BaseClient client, IEnumerable<PubSubMessage> expected)
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
