namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE pub/sub.
/// </summary>
public static class PubSub
{
    // Retry timespan for pub/sub assertions.
    public static readonly TimeSpan MaxDuration = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// Returns a unique exact channel message for testing.
    /// </summary>
    public static PubSubMessage ChannelMessage()
    {
        var id = Guid.NewGuid().ToString();
        var message = $"test-{id}-message";
        var channel = $"test-{id}-channel";

        return PubSubMessage.FromChannel(message, channel);
    }

    /// <summary>
    /// Returns a unique pattern channel message for testing.
    /// </summary>
    public static PubSubMessage PatternMessage()
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
    public static PubSubMessage ShardChannelMessage()
    {
        var id = Guid.NewGuid().ToString();
        var message = $"test-{id}-message";
        var channel = $"test-{id}-channel";

        return PubSubMessage.FromShardChannel(message, channel);
    }

    /// <summary>
    /// Asserts that the specified message has been received by the given client.
    /// </summary>
    public static async Task AssertReceivedAsync(BaseClient client, PubSubMessage expected)
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
    /// Asserts that the specified message has not been received by the given client.
    /// </summary>
    public static async Task AssertNotReceivedAsync(BaseClient client, PubSubMessage expected)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        queue.TryGetMessage(out PubSubMessage? received);
        if (received == null) return;

        Assert.NotEqual(expected.Message, received.Message);
    }
}
