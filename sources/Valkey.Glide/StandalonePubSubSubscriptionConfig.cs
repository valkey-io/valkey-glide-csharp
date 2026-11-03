// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

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
