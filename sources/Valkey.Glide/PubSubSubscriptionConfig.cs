// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Base configuration for PubSub subscriptions.
/// </summary>
public abstract class BasePubSubSubscriptionConfig
{
    internal MessageCallback? Callback { get; set; }
    internal object? Context { get; set; }
    internal Dictionary<PubSubChannelMode, ISet<string>> Subscriptions { get; set; } = [];
    internal PubSubPerformanceConfig? PerformanceConfig { get; set; }

    /// <summary>
    /// Configure a message callback to be invoked when messages are received.
    /// </summary>
    /// <param name="callback">The callback function to invoke for received messages.</param>
    /// <param name="context">Optional context object to pass to the callback.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public virtual BasePubSubSubscriptionConfig WithCallback(MessageCallback callback, object? context = null)
    {
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));
        Callback = callback;
        Context = context;
        return this;
    }

    /// <summary>
    /// Add an exact channel subscription.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public virtual BasePubSubSubscriptionConfig WithChannel(string channel)
        => AddSubscription(PubSubChannelMode.Exact, channel);

    /// <summary>
    /// Add a pattern subscription.
    /// </summary>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public virtual BasePubSubSubscriptionConfig WithPattern(string pattern)
        => AddSubscription(PubSubChannelMode.Pattern, pattern);

    /// <summary>
    /// Add a channel or pattern subscription.
    /// </summary>
    /// <param name="mode">The channel subscription mode.</param>
    /// <param name="channelOrPattern">The channel or pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    protected BasePubSubSubscriptionConfig AddSubscription(PubSubChannelMode mode, string channelOrPattern)
    {
        if (string.IsNullOrWhiteSpace(channelOrPattern))
            throw new ArgumentException("Channel name or pattern cannot be null, empty, or whitespace", nameof(channelOrPattern));

        if (!Subscriptions.ContainsKey(mode))
            Subscriptions[mode] = new HashSet<string>();

        Subscriptions[mode].Add(channelOrPattern);

        return this;
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

    public override StandalonePubSubSubscriptionConfig WithCallback(MessageCallback callback, object? context = null)
        => (StandalonePubSubSubscriptionConfig)base.WithCallback(callback, context);

    public override StandalonePubSubSubscriptionConfig WithChannel(string channel)
        => (StandalonePubSubSubscriptionConfig)base.WithChannel(channel);

    public override StandalonePubSubSubscriptionConfig WithPattern(string pattern)
        => (StandalonePubSubSubscriptionConfig)base.WithPattern(pattern);
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

    public override ClusterPubSubSubscriptionConfig WithCallback(MessageCallback callback, object? context = null)
        => (ClusterPubSubSubscriptionConfig)base.WithCallback(callback, context);

    public override ClusterPubSubSubscriptionConfig WithChannel(string channel)
        => (ClusterPubSubSubscriptionConfig)base.WithChannel(channel);

    public override ClusterPubSubSubscriptionConfig WithPattern(string pattern)
        => (ClusterPubSubSubscriptionConfig)base.WithPattern(pattern);

    /// <summary>
    /// Add a sharded channel subscription.
    /// </summary>
    /// <param name="channel">The sharded channel to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public ClusterPubSubSubscriptionConfig WithShardedChannel(string channel)
        => (ClusterPubSubSubscriptionConfig)AddSubscription(PubSubChannelMode.Sharded, channel);
}
