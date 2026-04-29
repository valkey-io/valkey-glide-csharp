// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by Valkey GLIDE clients but NOT by StackExchange.Redis databases. Methods implemented
// by both should be added to <see cref="IPubSubClusterCommands"/> instead.

/// <summary>
/// Cluster-specific pub/sub commands for Valkey GLIDE clients (sharded pub/sub).
/// </summary>
/// <seealso href="https://valkey.io/commands/#pubsub">Valkey – Pub/Sub Commands</seealso>
/// <seealso href="https://glide.valkey.io/how-to/publish-and-subscribe-messages/">Valkey GLIDE – Pub/Sub Messaging</seealso>
public partial interface IGlideClusterClient : IPubSubClusterCommands
{
    #region PublishCommands

    /// <summary>
    /// Publishes a message to the specified sharded channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/spublish/">Valkey commands – SPUBLISH</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannel">The sharded channel to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    /// <returns>The number of clients that received the message.</returns>
    /// <remarks>
    /// Sharded channels are specific to cluster mode and route messages to specific shards based on the channel name.
    /// <example>
    /// <code>
    /// var subscriberCount = await clusterClient.SPublishAsync("shard-news", "Shard-specific news!");
    /// Console.WriteLine($"Delivered message to {subscriberCount} subscriber(s)");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<long> SPublishAsync(ValkeyKey shardedChannel, ValkeyValue message);

    #endregion
    #region SubscribeCommands

    /// <summary>
    /// Subscribes the client to the specified sharded channel and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">Valkey commands – SSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannel">The sharded channel to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SSubscribeAsync("shard-news", TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Subscribed to 'shard-news' sharded channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SSubscribeAsync(ValkeyKey shardedChannel, TimeSpan timeout);

    /// <summary>
    /// Subscribes the client to the specified sharded channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">Valkey commands – SSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannels">A collection of sharded channels to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SSubscribeAsync(["shard-news", "shard-updates"], TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Subscribed to 'shard-news' and 'shard-updates' sharded channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SSubscribeAsync(IEnumerable<ValkeyKey> shardedChannels, TimeSpan timeout);

    /// <summary>
    /// Subscribes the client to the specified sharded channel and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">Valkey commands – SSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannel">The sharded channel to subscribe to.</param>
    /// <remarks>
    /// The client subscribes asynchronously in the background.
    /// Use <see cref="IBaseClient.GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="SSubscribeAsync(ValkeyKey, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await clusterClient.SSubscribeLazyAsync("shard-news");
    /// Console.WriteLine("Subscribed to 'shard-news' sharded channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SSubscribeLazyAsync(ValkeyKey shardedChannel);

    /// <summary>
    /// Subscribes the client to the specified sharded channels and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">Valkey commands – SSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannels">A collection of sharded channels to subscribe to.</param>
    /// <remarks>
    /// The client subscribes asynchronously in the background.
    /// Use <see cref="IBaseClient.GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="SSubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await clusterClient.SSubscribeLazyAsync(["shard-news", "shard-updates"]);
    /// Console.WriteLine("Subscribed to 'shard-news' and 'shard-updates' sharded channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SSubscribeLazyAsync(IEnumerable<ValkeyKey> shardedChannels);

    #endregion
    #region UnsubscribeCommands

    /// <summary>
    /// Unsubscribes the client from all sharded channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">Valkey commands – SUNSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeAsync(TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from all sharded channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeAsync(TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from the specified sharded channel and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">Valkey commands – SUNSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannel">The sharded channel to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeAsync("shard-news", TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from 'shard-news' sharded channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeAsync(ValkeyKey shardedChannel, TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from the specified sharded channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">Valkey commands – SUNSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannels">A collection of sharded channels to unsubscribe from. If empty or <see cref="PubSub.AllShardedChannels"/>, unsubscribes from all sharded channels.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="timeout"/> is negative.</exception>
    /// <exception cref="Errors.TimeoutException">Thrown if server confirmation is not received within the specified <paramref name="timeout"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeAsync(["shard-news", "shard-updates"], TimeSpan.FromSeconds(5));
    /// Console.WriteLine("Unsubscribed from 'shard-news' and 'shard-updates' sharded channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeAsync(IEnumerable<ValkeyKey> shardedChannels, TimeSpan timeout);

    /// <summary>
    /// Unsubscribes the client from all sharded channels and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">Valkey commands – SUNSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="IBaseClient.GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="SUnsubscribeAsync(TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeLazyAsync();
    /// Console.WriteLine("Unsubscribed from all sharded channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeLazyAsync();

    /// <summary>
    /// Unsubscribes the client from the specified sharded channel and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">Valkey commands – SUNSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannel">The sharded channel to unsubscribe from.</param>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="IBaseClient.GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="SUnsubscribeAsync(ValkeyKey, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeLazyAsync("shard-news");
    /// Console.WriteLine("Unsubscribed from 'shard-news' sharded channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeLazyAsync(ValkeyKey shardedChannel);

    /// <summary>
    /// Unsubscribes the client from the specified sharded channels and returns without waiting for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">Valkey commands – SUNSUBSCRIBE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannels">A collection of sharded channels to unsubscribe from. If empty or <see cref="PubSub.AllShardedChannels"/>, unsubscribes from all sharded channels.</param>
    /// <remarks>
    /// The client unsubscribes asynchronously in the background.
    /// Use <see cref="IBaseClient.GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// See <see cref="SUnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/> for the blocking version.
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeLazyAsync(["shard-news", "shard-updates"]);
    /// Console.WriteLine("Unsubscribed from 'shard-news' and 'shard-updates' sharded channels");
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeLazyAsync(PubSub.AllShardedChannels);
    /// Console.WriteLine("Unsubscribed from all sharded channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeLazyAsync(IEnumerable<ValkeyKey> shardedChannels);

    #endregion
    #region IntrospectionCommands

    /// <summary>
    /// Lists the currently active sharded channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardchannels/">Valkey commands – PUBSUB SHARDCHANNELS</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <returns>A set of active sharded channel names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var shardedChannels = await clusterClient.PubSubShardChannelsAsync();
    /// Console.WriteLine($"Active sharded channels: {string.Join(", ", shardedChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<ISet<ValkeyKey>> PubSubShardChannelsAsync();

    /// <summary>
    /// Lists the currently active sharded channels matching the specified pattern.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardchannels/">Valkey commands – PUBSUB SHARDCHANNELS</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="pattern">A glob-style pattern to filter sharded channel names.</param>
    /// <returns>A set of active sharded channel names matching <paramref name="pattern"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var shardedChannels = await clusterClient.PubSubShardChannelsAsync("shard.*");
    /// Console.WriteLine($"Matching sharded channels: {string.Join(", ", shardedChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<ISet<ValkeyKey>> PubSubShardChannelsAsync(ValkeyKey pattern);

    /// <summary>
    /// Returns the number of subscribers for the specified sharded channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardnumsub/">Valkey commands – PUBSUB SHARDNUMSUB</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="shardedChannels">A collection of sharded channel names to query.</param>
    /// <returns>A dictionary mapping sharded channel names to their subscriber counts.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var subscriberCounts = await clusterClient.PubSubShardNumSubAsync(["shard-news", "shard-updates"]);
    /// foreach (var kvp in subscriberCounts)
    /// {
    ///     Console.WriteLine($"{kvp.Key}: {kvp.Value} subscribers");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<Dictionary<ValkeyKey, long>> PubSubShardNumSubAsync(IEnumerable<ValkeyKey> shardedChannels);

    #endregion
}
