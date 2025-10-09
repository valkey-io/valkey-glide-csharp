// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace Valkey.Glide;

/// <summary>
/// PubSub subscription modes for standalone clients.
/// </summary>
public enum PubSubChannelMode
{
    /// <summary>Exact channel name subscription.</summary>
    Exact = 0,
    /// <summary>Pattern-based subscription.</summary>
    Pattern = 1
}

/// <summary>
/// PubSub subscription modes for cluster clients.
/// </summary>
public enum PubSubClusterChannelMode
{
    /// <summary>Exact channel name subscription.</summary>
    Exact = 0,
    /// <summary>Pattern-based subscription.</summary>
    Pattern = 1,
    /// <summary>Sharded channel subscription (cluster-specific).</summary>
    Sharded = 2
}

/// <summary>
/// Base configuration for PubSub subscriptions.
/// </summary>
public abstract class BasePubSubSubscriptionConfig
{
    internal MessageCallback? Callback { get; set; }
    internal object? Context { get; set; }
    internal Dictionary<uint, List<string>> Subscriptions { get; set; } = [];

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
    /// Validates the subscription configuration.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when configuration is invalid.</exception>
    internal virtual void Validate()
    {
        if (Subscriptions.Count == 0)
        {
            throw new ArgumentException("At least one subscription must be configured");
        }

        foreach (KeyValuePair<uint, List<string>> kvp in Subscriptions)
        {
            if (kvp.Value == null || kvp.Value.Count == 0)
            {
                throw new ArgumentException($"Subscription mode {kvp.Key} has no channels or patterns configured");
            }

            foreach (string channelOrPattern in kvp.Value)
            {
                if (string.IsNullOrWhiteSpace(channelOrPattern))
                {
                    throw new ArgumentException("Channel name or pattern cannot be null, empty, or whitespace");
                }
            }
        }
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
    public StandalonePubSubSubscriptionConfig()
    {
    }

    /// <summary>
    /// Add a channel or pattern subscription.
    /// </summary>
    /// <param name="mode">The subscription mode (Exact or Pattern).</param>
    /// <param name="channelOrPattern">The channel name or pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when channelOrPattern is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when mode is not valid for standalone clients.</exception>
    public StandalonePubSubSubscriptionConfig WithSubscription(PubSubChannelMode mode, string channelOrPattern)
    {
        if (string.IsNullOrWhiteSpace(channelOrPattern))
        {
            throw new ArgumentException("Channel name or pattern cannot be null, empty, or whitespace", nameof(channelOrPattern));
        }

        if (!Enum.IsDefined(typeof(PubSubChannelMode), mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode), "Invalid PubSub channel mode for standalone client");
        }

        uint modeValue = (uint)mode;
        if (!Subscriptions.ContainsKey(modeValue))
        {
            Subscriptions[modeValue] = [];
        }

        if (!Subscriptions[modeValue].Contains(channelOrPattern))
        {
            Subscriptions[modeValue].Add(channelOrPattern);
        }

        return this;
    }

    /// <summary>
    /// Add an exact channel subscription.
    /// </summary>
    /// <param name="channel">The channel name to subscribe to.</param>
    ///    /// <returns>Thistion instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when channel is null, empty, or whitespace.</exception>
    public StandalonePubSubSubscriptionConfig WithChannel(string channel) => WithSubscription(PubSubChannelMode.Exact, channel);

    /// <summary>
    /// Add a pattern subscription.
    /// </summary>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when pattern is null, empty, or whitespace.</exception>
    public StandalonePubSubSubscriptionConfig WithPattern(string pattern) => WithSubscription(PubSubChannelMode.Pattern, pattern);

    /// <summary>
    /// Validates the standalone subscription configuration.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when configuration is invalid.</exception>
    internal override void Validate()
    {
        base.Validate();

        // Ensure only valid modes for standalone clients are used
        foreach (uint mode in Subscriptions.Keys)
        {
            if (mode is not ((uint)PubSubChannelMode.Exact) and not ((uint)PubSubChannelMode.Pattern))
            {
                throw new ArgumentException($"Subscription mode {mode} is not valid for standalone clients");
            }
        }
    }
}

/// <summary>
/// PubSub subscription configuration for cluster clients.
/// </summary>
public sealed class ClusterPubSubSubscriptionConfig : BasePubSubSubscriptionConfig
{
    /// <summary>
    /// /// Initializes a ne of the <see cref="ClusterPubSubSubscriptionConfig"/> class.
    /// </summary>
    public ClusterPubSubSubscriptionConfig()
    {
    }

    /// <summary>
    /// Add a channel, pattern, or sharded subscription.
    /// </summary>
    /// <param name="mode">The subscription mode (Exact, Pattern, or Sharded).</param>
    /// <param name="channelOrPattern">The channel name or pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when channelOrPattern is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when mode is not valid for cluster clients.</exception>
    public ClusterPubSubSubscriptionConfig WithSubscription(PubSubClusterChannelMode mode, string channelOrPattern)
    {
        if (string.IsNullOrWhiteSpace(channelOrPattern))
        {
            throw new ArgumentException("Channel name or pattern cannot be null, empty, or whitespace", nameof(channelOrPattern));
        }

        if (!Enum.IsDefined(typeof(PubSubClusterChannelMode), mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode), "Invalid PubSub channel mode for cluster client");
        }

        uint modeValue = (uint)mode;
        if (!Subscriptions.ContainsKey(modeValue))
        {
            Subscriptions[modeValue] = [];
        }

        if (!Subscriptions[modeValue].Contains(channelOrPattern))
        {
            Subscriptions[modeValue].Add(channelOrPattern);
        }

        return this;
    }

    /// <summary>
    /// Add an exact channel subscription.
    /// </summary>
    /// <param name="channel">The channel name to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when channel is null, empty, or whitespace.</exception>
    public ClusterPubSubSubscriptionConfig WithChannel(string channel) => WithSubscription(PubSubClusterChannelMode.Exact, channel);

    /// <summary>
    /// Add a pattern subscription.
    /// </summary>
    /// <param name="pattern">The pattern to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when pattern is null, empty, or whitespace.</exception>
    public ClusterPubSubSubscriptionConfig WithPattern(string pattern) => WithSubscription(PubSubClusterChannelMode.Pattern, pattern);

    /// <summary>
    /// Add a sharded channel subscription.
    /// </summary>
    /// <param name="channel">The sharded channel name to subscribe to.</param>
    /// <returns>This configuration instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when channel is null, empty, or whitespace.</exception>
    public ClusterPubSubSubscriptionConfig WithShardedChannel(string channel) => WithSubscription(PubSubClusterChannelMode.Sharded, channel);

    /// <summary>
    /// Validates the cluster subscription configuration.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when configuration is invalid.</exception>
    internal override void Validate()
    {
        base.Validate();

        // Ensure only valid modes for cluster clients are used
        foreach (uint mode in Subscriptions.Keys)
        {
            if (mode is not ((uint)PubSubClusterChannelMode.Exact) and
                not ((uint)PubSubClusterChannelMode.Pattern) and
                not ((uint)PubSubClusterChannelMode.Sharded))
            {
                throw new ArgumentException($"Subscription mode {mode} is not valid for cluster clients");
            }
        }
    }
}
