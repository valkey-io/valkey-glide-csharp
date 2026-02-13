// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Pub/sub commands specific to cluster clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/#pubsub">valkey.io</see>.
/// </summary>
public interface IPubSubClusterCommands
{
    #region PublishCommands

    /// <summary>
    /// Publishes a message to the specified shard channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/spublish/">valkey.io</seealso>
    /// <param name="shardChannel">The shard channel to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    /// <returns>The number of clients that received the message.</returns>
    /// <remarks>
    /// Sharded channels are specific to cluster mode and route messages to specific shards based on the channel name.
    /// <example>
    /// <code>
    /// long subscriberCount = await clusterClient.SPublishAsync("shard-news", "Shard-specific news!");
    /// Console.WriteLine($"Message delivered to {subscriberCount} subscribers");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<long> SPublishAsync(string shardChannel, string message);

    #endregion
    #region SubscribeCommands

    /// <summary>
    /// Subscribes the client to the specified shard channel and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">valkey.io</seealso>
    /// <param name="shardChannel">The shard channel to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task SSubscribeAsync(string shardChannel, TimeSpan timeout = default);

    /// <summary>
    /// Subscribes the client to the specified shard channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">valkey.io</seealso>
    /// <param name="shardChannels">A collection of shard channels to subscribe to.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task SSubscribeAsync(IEnumerable<string> shardChannels, TimeSpan timeout = default);

    /// <summary>
    /// Subscribes the client to the specified shard channel and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to subscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">valkey.io</seealso>
    /// <param name="shardChannel">The shard channel to subscribe to.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SSubscribeLazyAsync("shard-news");
    /// Console.WriteLine("Subscribed to 'shard-news' shard channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SSubscribeLazyAsync(string shardChannel);

    /// <summary>
    /// Subscribes the client to the specified shard channels and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to subscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">valkey.io</seealso>
    /// <param name="shardChannels">A collection of shard channels to subscribe to.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var shardChannels = new string[] { "shard-news", "shard-updates" };
    /// await clusterClient.SSubscribeLazyAsync(shardChannels);
    /// Console.WriteLine("Subscribed to 'shard-news' and 'shard-updates' shard channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SSubscribeLazyAsync(IEnumerable<string> shardChannels);

    #endregion
    #region UnsubscribeCommands

    /// <summary>
    /// Unsubscribes the client from all shard channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task SUnsubscribeAsync(TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from the specified shard channel and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <param name="shardChannel">The shard channel to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task SUnsubscribeAsync(string shardChannel, TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from the specified shard channels and waits for server confirmation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <param name="shardChannels">A collection of shard channels to unsubscribe from.</param>
    /// <param name="timeout">Maximum time to wait for server confirmation. Waits indefinitely if not specified or <see cref="TimeSpan.Zero"/>.</param>
    /// <exception cref="ArgumentException">Thrown if timeout is negative.</exception>
    /// <exception cref="TimeoutException">Thrown if server confirmation is not received within the specified timeout.</exception>
    abstract Task SUnsubscribeAsync(IEnumerable<string> shardChannels, TimeSpan timeout = default);

    /// <summary>
    /// Unsubscribes the client from all shard channels and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the
    /// actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeLazyAsync();
    /// Console.WriteLine("Unsubscribed from all shard channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeLazyAsync();

    /// <summary>
    /// Unsubscribes the client from the specified shard channel and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <param name="shardChannel">The shard channel to unsubscribe from.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeLazyAsync("shard-news");
    /// Console.WriteLine("Unsubscribed from 'shard-news' shard channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeLazyAsync(string shardChannel);

    /// <summary>
    /// Unsubscribes the client from the specified shard channels and returns without waiting for server confirmation.
    /// <para />
    /// The client will attempt to unsubscribe asynchronously in the background.
    /// Use <see cref="IPubSubCommands.GetSubscriptionsAsync"/> to verify the actual server subscription state.
    /// <para />
    /// If no shard channels or <see cref="PubSub.AllShardChannels"/> is specified, unsubscribes the client from all shard channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <param name="shardChannels">A collection of shard channels to unsubscribe from.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var shardChannels = new string[] { "shard-news", "shard-updates" };
    /// await clusterClient.SUnsubscribeLazyAsync(shardChannels);
    /// Console.WriteLine("Unsubscribed from 'shard-news' and 'shard-updates' shard channels");
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeLazyAsync(PubSub.AllShardChannels);
    /// Console.WriteLine("Unsubscribed from all shard channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeLazyAsync(IEnumerable<string> shardChannels);

    #endregion
    #region IntrospectionCommands

    /// <summary>
    /// Lists the currently active shard channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardchannels/">valkey.io</seealso>
    /// <returns>A set of active shard channel names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var shardChannels = await clusterClient.PubSubShardChannelsAsync();
    /// Console.WriteLine($"Active shard channels: {string.Join("', '", shardChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<ISet<string>> PubSubShardChannelsAsync();

    /// <summary>
    /// Lists the currently active shard channels matching the specified pattern.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardchannels/">valkey.io</seealso>
    /// <param name="pattern">A glob-style pattern to filter shard channel names.</param>
    /// <returns>A set of active shard channel names matching the pattern.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var shardChannels = await clusterClient.PubSubShardChannelsAsync("shard.*");
    /// Console.WriteLine($"Matching shard channels: {string.Join("', '", shardChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<ISet<string>> PubSubShardChannelsAsync(string pattern);

    /// <summary>
    /// Returns the number of subscribers for the specified shard channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardnumsub/">valkey.io</seealso>
    /// <param name="shardChannels">A collection of shard channel names to query.</param>
    /// <returns>A dictionary mapping shard channel names to their subscriber counts.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var shardChannels = new string[] { "shard-news", "shard-updates" };
    /// Dictionary&lt;string, long&gt; counts = await clusterClient.PubSubShardNumSubAsync(shardChannels);
    /// foreach (var kvp in counts)
    /// {
    ///     Console.WriteLine($"{kvp.Key}: {kvp.Value} subscribers");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<Dictionary<string, long>> PubSubShardNumSubAsync(IEnumerable<string> shardChannels);

    #endregion
}
