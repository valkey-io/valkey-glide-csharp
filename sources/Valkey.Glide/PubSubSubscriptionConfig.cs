// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Base configuration for PubSub subscriptions.
/// </summary>
public abstract class BasePubSubSubscriptionConfig
{
    internal MessageCallback? Callback { get; set; }
    internal object? Context { get; set; }
    internal Dictionary<uint, ISet<string>> Subscriptions { get; set; } = [];
    internal PubSubPerformanceConfig? PerformanceConfig { get; set; }

    /// <summary>
    /// Configure a message callback to be invoked when messages are received.
    /// </summary>
    /// <param name="callback">The callback function to invoke for received messages.</param>
    /// <param name="context">Optional context object to pass to the callback.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when callback is null.</exception>
    public T WithCallback<T>(MessageCallback callback, object? context = null) where T : BasePubSubSubscriptionConfig
    {
        Callback = callback ?? throw new ArgumentNullException(nameof(callback), "Callback cannot be null");
        Context = context;
        return (T)this;
    }

    /// <summary>
    /// Add a channel or pattern subscription.
    /// </summary>
    /// <param name="mode">The subscription mode.</param>
    /// <param name="channelOrPattern">The channel name or pattern to subscribe to.</param>
    /// <exception cref="ArgumentException">Thrown when channelOrPattern is null, empty, or whitespace.</exception>
    protected void AddSubscription(PubSubChannelMode mode, string channelOrPattern)
    {
        if (string.IsNullOrWhiteSpace(channelOrPattern))
        {
            throw new ArgumentException("Channel name or pattern cannot be null, empty, or whitespace", nameof(channelOrPattern));
        }

        uint modeValue = (uint)mode;
        if (!Subscriptions.ContainsKey(modeValue))
        {
            Subscriptions[modeValue] = new HashSet<string>();
        }

        Subscriptions[modeValue].Add(channelOrPattern);
    }
}

/// <summary>
/// PubSub subscription configuration for standalone clients.
/// </summary>
public sealed class StandalonePubSubSubscriptionConfig : BasePubSubSubscriptionConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StandalonePubSubSubscriptionConfig"/> class.
    /// </summary>
    public StandalonePubSubSubscriptionConfig() { }

    /// <summary>
    /// Add an exact channel subscription.
    /// </summary>
    /// <param name="channel">The channel name to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when channel is null, empty, or whitespace.</exception>
    public StandalonePubSubSubscriptionConfig WithChannel(string channel)
    {
        AddSubscription(PubSubChannelMode.Exact, channel);
        return this;
    }

    /// <summary>
    /// Add a pattern subscription.
    /// </summary>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when pattern is null, empty, or whitespace.</exception>
    public StandalonePubSubSubscriptionConfig WithPattern(string pattern)
    {
        AddSubscription(PubSubChannelMode.Pattern, pattern);
        return this;
    }
}

/// <summary>
/// PubSub subscription configuration for cluster clients.
/// </summary>
public sealed class ClusterPubSubSubscriptionConfig : BasePubSubSubscriptionConfig
{
    /// <summary>
    /// /// Initializes a new instance of the <see cref="ClusterPubSubSubscriptionConfig"/> class.
    /// </summary>
    public ClusterPubSubSubscriptionConfig() { }

    /// <summary>
    /// Add an exact channel subscription.
    /// </summary>
    /// <param name="channel">The channel name to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when channel is null, empty, or whitespace.</exception>
    public ClusterPubSubSubscriptionConfig WithChannel(string channel)
    {
        AddSubscription(PubSubChannelMode.Exact, channel);
        return this;
    }

    /// <summary>
    /// Add a pattern subscription.
    /// </summary>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when pattern is null, empty, or whitespace.</exception>
    public ClusterPubSubSubscriptionConfig WithPattern(string pattern)
    {
        AddSubscription(PubSubChannelMode.Pattern, pattern);
        return this;
    }

    /// <summary>
    /// Add a shard channel subscription.
    /// </summary>
    /// <param name="channel">The shard channel name to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when channel is null, empty, or whitespace.</exception>
    public ClusterPubSubSubscriptionConfig WithShardedChannel(string channel)
    {
        AddSubscription(PubSubChannelMode.Sharded, channel);
        return this;
    }
}
