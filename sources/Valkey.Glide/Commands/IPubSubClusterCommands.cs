// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// PubSub commands specific to cluster clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/#pubsub">valkey.io</see>.
/// </summary>
public interface IPubSubClusterCommands : IPubSubCommands
{
    /// <summary>
    /// Publishes a message to the specified channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/publish/">valkey.io</seealso>
    /// <param name="channel">The channel to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of clients that received the message.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long subscriberCount = await clusterClient.PublishAsync("news", "Breaking news!");
    /// Console.WriteLine($"Message delivered to {subscriberCount} subscribers");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Publishes a message to the specified channel, with optional sharded mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/publish/">valkey.io</seealso>
    /// <seealso href="https://valkey.io/commands/spublish/">valkey.io</seealso>
    /// <param name="channel">The channel to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="sharded">If true, uses sharded channel publishing (SPUBLISH). If false, uses regular publishing (PUBLISH).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of clients that received the message.</returns>
    /// <remarks>
    /// Sharded channels are specific to cluster mode and route messages to specific shards based on the channel name.
    /// <example>
    /// <code>
    /// // Regular publish
    /// long count1 = await clusterClient.PublishAsync("news", "Breaking news!", false);
    ///
    /// // Sharded publish
    /// long count2 = await clusterClient.PublishAsync("shard-news", "Shard-specific news!", true);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> PublishAsync(string channel, string message, bool sharded, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Lists the currently active sharded channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardchannels/">valkey.io</seealso>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of active sharded channel names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string[] shardChannels = await clusterClient.PubSubShardChannelsAsync();
    /// Console.WriteLine($"Active shard channels: {string.Join(", ", shardChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string[]> PubSubShardChannelsAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Lists the currently active sharded channels matching the specified pattern.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardchannels/">valkey.io</seealso>
    /// <param name="pattern">A glob-style pattern to filter sharded channel names.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of active sharded channel names matching the pattern.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string[] shardChannels = await clusterClient.PubSubShardChannelsAsync("shard.*");
    /// Console.WriteLine($"Matching shard channels: {string.Join(", ", shardChannels)}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string[]> PubSubShardChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of subscribers for the specified sharded channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-shardnumsub/">valkey.io</seealso>
    /// <param name="channels">An array of sharded channel names to query.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A dictionary mapping sharded channel names to their subscriber counts.</returns>
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
    Task<Dictionary<string, long>> PubSubShardNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None);
}
