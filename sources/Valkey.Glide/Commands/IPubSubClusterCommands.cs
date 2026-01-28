// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// PubSub commands specific to cluster clients.
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
    /// <param name="channel">The shard channel to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
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
    abstract Task<long> SPublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None);

    #endregion
    #region SubscribeCommands

    /// <summary>
    /// Subscribes the client to the specified shard channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">valkey.io</seealso>
    /// <param name="channel">The shard channel to subscribe to.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A task that completes when the client has been subscribed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SSubscribeAsync("shard-news");
    /// Console.WriteLine("Subscribed to shard-news channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SSubscribeAsync(string channel, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Subscribes the client to the specified shard channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ssubscribe/">valkey.io</seealso>
    /// <param name="channels">An array of shard channels to subscribe to.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A task that completes when the client has been subscribed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SSubscribeAsync(["shard-news", "shard-updates"]);
    /// Console.WriteLine("Subscribed to shard-news and shard-updates channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SSubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None);

    #endregion
    #region UnsubscribeCommands

    /// <summary>
    /// Unsubscribes the client from all shard channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A task that completes when the client has been unsubscribed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeAsync();
    /// Console.WriteLine("Unsubscribed from all shard channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Unsubscribes the client from the specified shard channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <param name="channel">The shard channel to unsubscribe from.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A task that completes when the client has been unsubscribed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeAsync("shard-news");
    /// Console.WriteLine("Unsubscribed from shard-news channel");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Unsubscribes the client from the specified shard channels.
    /// If no channels are specified, unsubscribes the client from all channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunsubscribe/">valkey.io</seealso>
    /// <param name="channels">An array of shard channels to unsubscribe from.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A task that completes when the client has been unsubscribed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeAsync(["shard-news", "shard-updates"]);
    /// Console.WriteLine("Unsubscribed from shard-news and shard-updates channels");
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// await clusterClient.SUnsubscribeAsync([]);
    /// Console.WriteLine("Unsubscribed from all shard channels");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task SUnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None);

    #endregion
    #region PubSubInfoCommands

    /// <summary>
    /// Lists the currently active shard channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardchannels/">valkey.io</seealso>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of active shard channel names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string[] shardChannels = await clusterClient.PubSubShardChannelsAsync();
    /// Console.WriteLine($"Active shard channels: {string.Join(", ", shardChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<string[]> PubSubShardChannelsAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Lists the currently active shard channels matching the specified pattern.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardchannels/">valkey.io</seealso>
    /// <param name="pattern">A glob-style pattern to filter shard channel names.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of active shard channel names matching the pattern.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string[] shardChannels = await clusterClient.PubSubShardChannelsAsync("shard.*");
    /// Console.WriteLine($"Matching shard channels: {string.Join(", ", shardChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<string[]> PubSubShardChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of subscribers for the specified shard channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardnumsub/">valkey.io</seealso>
    /// <param name="channels">An array of shard channel names to query.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A dictionary mapping shard channel names to their subscriber counts.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, long&gt; counts = await clusterClient.PubSubShardNumSubAsync(new[] { "shard1", "shard2" });
    /// foreach (var kvp in counts)
    /// {
    ///     Console.WriteLine($"{kvp.Key}: {kvp.Value} subscribers");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    abstract Task<Dictionary<string, long>> PubSubShardNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None);

    #endregion
}
