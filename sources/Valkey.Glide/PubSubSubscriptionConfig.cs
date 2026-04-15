// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Base configuration for PubSub subscriptions.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/publish-and-subscribe-messages/">Valkey GLIDE – Pub/Sub Messaging</seealso>
public abstract class BasePubSubSubscriptionConfig
{
    internal MessageCallback? Callback { get; set; }
    internal object? Context { get; set; }
    internal Dictionary<PubSubChannelMode, ISet<ValkeyKey>> Subscriptions { get; set; } = [];
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
    public virtual BasePubSubSubscriptionConfig WithChannel(ValkeyKey channel)
        => AddSubscription(PubSubChannelMode.Exact, channel);

    /// <summary>
    /// Add a pattern subscription.
    /// </summary>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public virtual BasePubSubSubscriptionConfig WithPattern(ValkeyKey pattern)
        => AddSubscription(PubSubChannelMode.Pattern, pattern);

    /// <summary>
    /// Add a channel or pattern subscription.
    /// </summary>
    /// <param name="mode">The channel subscription mode.</param>
    /// <param name="channelOrPattern">The channel or pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    protected BasePubSubSubscriptionConfig AddSubscription(PubSubChannelMode mode, ValkeyKey channelOrPattern)
    {
        channelOrPattern.AssertNotNull();

        if (!Subscriptions.ContainsKey(mode))
        {
            Subscriptions[mode] = new HashSet<ValkeyKey>();
        }

        _ = Subscriptions[mode].Add(channelOrPattern);

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

    /// <inheritdoc/>
    public override StandalonePubSubSubscriptionConfig WithCallback(MessageCallback callback, object? context = null)
        => (StandalonePubSubSubscriptionConfig)base.WithCallback(callback, context);

    /// <inheritdoc/>
    public override StandalonePubSubSubscriptionConfig WithChannel(ValkeyKey channel)
        => (StandalonePubSubSubscriptionConfig)base.WithChannel(channel);

    /// <inheritdoc/>
    public override StandalonePubSubSubscriptionConfig WithPattern(ValkeyKey pattern)
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

    /// <inheritdoc/>
    public override ClusterPubSubSubscriptionConfig WithCallback(MessageCallback callback, object? context = null)
        => (ClusterPubSubSubscriptionConfig)base.WithCallback(callback, context);

    /// <inheritdoc/>
    public override ClusterPubSubSubscriptionConfig WithChannel(ValkeyKey channel)
        => (ClusterPubSubSubscriptionConfig)base.WithChannel(channel);

    /// <inheritdoc/>
    public override ClusterPubSubSubscriptionConfig WithPattern(ValkeyKey pattern)
        => (ClusterPubSubSubscriptionConfig)base.WithPattern(pattern);

    /// <summary>
    /// Add a sharded channel subscription.
    /// </summary>
    /// <param name="channel">The sharded channel to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public ClusterPubSubSubscriptionConfig WithShardedChannel(ValkeyKey channel)
        => (ClusterPubSubSubscriptionConfig)AddSubscription(PubSubChannelMode.Sharded, channel);
}
