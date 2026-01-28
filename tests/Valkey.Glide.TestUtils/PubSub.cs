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
    /// Returns a unique message for testing.
    /// If specified, the message will include a pattern that matches the channel.
    /// </summary>
    public static PubSubMessage BuildMessage(bool withPattern = false)
    {
        var id = Guid.NewGuid().ToString();
        var message = $"test-{id}-message";
        var channel = $"test-{id}-channel";

        if (!withPattern)
        {
            return new PubSubMessage(message, channel);
        }

        var pattern = $"test-{id}-*";
        return new PubSubMessage(message, channel, pattern);
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
