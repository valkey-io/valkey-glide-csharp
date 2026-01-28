namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE pub/sub.
/// </summary>
public static class PubSub
{
    // Retry timespan for pub/sub assertions.
    private static readonly TimeSpan MaxRetryDuration = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan RetryInterval = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Returns a unique message for testing.
    /// </summary>
    public static PubSubMessage GetPubSubMessage(bool pattern = false)
    {
        var id = Guid.NewGuid().ToString();
        var message = $"test-{id}-message";
        var channel = $"test-{id}-channel";

        return pattern ?
            new PubSubMessage(message, channel, $"test-{id}-*")
            : new PubSubMessage(message, channel);
    }

    /// <summary>
    /// Asserts that the specified message is received on the specified channel (and pattern, if provided).
    /// </summary>
    public static async Task AssertReceived(BaseClient client, PubSubMessage expected)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var received = await queue.GetMessageAsync(cts.Token);

        Assert.Equal(expected.Message, received.Message);
        Assert.Equal(expected.Channel, received.Channel);
        Assert.Equal(expected.Pattern, received.Pattern);
    }

    /// <summary>
    /// Asserts that the specified message is not received.
    /// </summary>
    public static async Task AssertNotReceived(BaseClient client, PubSubMessage expected)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        queue.TryGetMessage(out PubSubMessage? received);
        if (received == null) return;

        Assert.NotEqual(expected.Message, received.Message);
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified channels.
    /// </summary>
    public static async Task AssertSubscribed(BaseClient client, string[] channels)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxRetryDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var channelCounts = await client.PubSubNumSubAsync(channels);
            if (channelCounts.All(kvp => kvp.Value > 0))
                return;

            await Task.Delay(RetryInterval, cts.Token);
        }

        Assert.Fail($"Expected at least 1 subscriber for channels '{string.Join(", ", channels)}'");
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified shard channels.
    /// </summary>
    public static async Task AssertSSubscribed(GlideClusterClient client, string[] shardChannels)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxRetryDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var channelCounts = await client.PubSubShardNumSubAsync(shardChannels);
            if (channelCounts.All(kvp => kvp.Value > 0))
                return;

            await Task.Delay(RetryInterval, cts.Token);
        }

        Assert.Fail($"Expected at least 1 subscriber for shard channels '{string.Join(", ", shardChannels)}'");
    }

    /// <summary>
    /// Asserts that there are no subscribers to each of the specified channels.
    /// </summary>
    public static async Task AssertUnsubscribed(BaseClient client, string[] channels)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxRetryDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var channelCounts = await client.PubSubNumSubAsync(channels);
            if (channelCounts.All(kvp => kvp.Value == 0))
                return;

            await Task.Delay(500, cts.Token);
        }

        Assert.Fail($"Expected 0 subscribers for channels '{string.Join(", ", channels)}'");
    }

    /// <summary>
    /// Asserts that there are no subscribers to each of the specified shard channels.
    /// </summary>
    public static async Task AssertSUnsubscribed(GlideClusterClient client, string[] shardChannels)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxRetryDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var channelCounts = await client.PubSubShardNumSubAsync(shardChannels);
            if (channelCounts.All(kvp => kvp.Value == 0))
                return;

            await Task.Delay(500, cts.Token);
        }

        Assert.Fail($"Expected 0 subscribers for shard channels '{string.Join(", ", shardChannels)}'");
    }
}
