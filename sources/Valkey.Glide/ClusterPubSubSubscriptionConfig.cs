// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// PubSub subscription configuration for cluster clients.
/// </summary>
public sealed class ClusterPubSubSubscriptionConfig : BasePubSubSubscriptionConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClusterPubSubSubscriptionConfig"/> class.
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
