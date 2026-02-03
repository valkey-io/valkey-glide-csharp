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
    public virtual BasePubSubSubscriptionConfig WithCallback(MessageCallback callback, object? context = null)
    {
        ArgumentNullException.ThrowIfNull(callback);
        Callback = callback;
        Context = context;
        return this;
    }

    /// <summary>
    /// Add an exact channel subscription.
    /// </summary>
    /// <param name="channel">The channel name to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public virtual BasePubSubSubscriptionConfig WithChannel(string channel)
    {
        AddSubscription(PubSubChannelMode.Exact, channel);
        return this;
    }

    /// <summary>
    /// Add a pattern subscription.
    /// </summary>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public virtual BasePubSubSubscriptionConfig WithPattern(string pattern)
    {
        AddSubscription(PubSubChannelMode.Pattern, pattern);
        return this;
    }

    /// <summary>
    /// Add a channel or pattern subscription.
    /// </summary>
    /// <param name="mode">The subscription mode.</param>
    /// <param name="channelOrPattern">The channel name or pattern to subscribe to.</param>
    protected void AddSubscription(PubSubChannelMode mode, string channelOrPattern)
    {
        if (string.IsNullOrWhiteSpace(channelOrPattern))
            throw new ArgumentException("Channel name or pattern cannot be null, empty, or whitespace", nameof(channelOrPattern));

        uint modeValue = (uint)mode;
        if (!Subscriptions.ContainsKey(modeValue))
            Subscriptions[modeValue] = new HashSet<string>();

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

    /// <inheritdoc/>
    public override StandalonePubSubSubscriptionConfig WithCallback(MessageCallback callback, object? context = null)
    {
        base.WithCallback(callback, context);
        return this;
    }

    /// <inheritdoc/>
    public override StandalonePubSubSubscriptionConfig WithChannel(string channel)
    {
        base.WithChannel(channel);
        return this;
    }

    /// <inheritdoc/>
    public override StandalonePubSubSubscriptionConfig WithPattern(string pattern)
    {
        base.WithPattern(pattern);
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

    /// <inheritdoc/>
    public override ClusterPubSubSubscriptionConfig WithCallback(MessageCallback callback, object? context = null)
    {
        base.WithCallback(callback, context);
        return this;
    }

    /// <inheritdoc/>
    public override ClusterPubSubSubscriptionConfig WithChannel(string channel)
    {
        base.WithChannel(channel);
        return this;
    }

    /// <inheritdoc/>
    public override ClusterPubSubSubscriptionConfig WithPattern(string pattern)
    {
        base.WithPattern(pattern);
        return this;
    }

    /// <summary>
    /// Add a shard channel subscription.
    /// </summary>
    /// <param name="channel">The shard channel name to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    public ClusterPubSubSubscriptionConfig WithShardedChannel(string channel)
    {
        AddSubscription(PubSubChannelMode.Sharded, channel);
        return this;
    }
}
