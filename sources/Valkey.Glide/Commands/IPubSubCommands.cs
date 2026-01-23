// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// PubSub commands available in both standalone and cluster modes.
/// <br />
/// See more on <see href="https://valkey.io/commands/#pubsub">valkey.io</see>.
/// </summary>
public interface IPubSubCommands
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
    /// Lists the currently active channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-channels/">valkey.io</seealso>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of active channel names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string[] channels = await client.PubSubChannelsAsync();
    /// Console.WriteLine($"Active channels: {string.Join(", ", channels)}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string[]> PubSubChannelsAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Lists the currently active channels matching the specified pattern.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-channels/">valkey.io</seealso>
    /// <param name="pattern">A glob-style pattern to filter channel names.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of active channel names matching the pattern.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string[] channels = await client.PubSubChannelsAsync("news.*");
    /// Console.WriteLine($"News channels: {string.Join(", ", channels)}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string[]> PubSubChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of subscribers for the specified channels.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-numsub/">valkey.io</seealso>
    /// <param name="channels">An array of channel names to query.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A dictionary mapping channel names to their subscriber counts.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, long&gt; counts = await client.PubSubNumSubAsync(new[] { "channel1", "channel2" });
    /// foreach (var kvp in counts)
    /// {
    ///     Console.WriteLine($"{kvp.Key}: {kvp.Value} subscribers");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, long>> PubSubNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of active pattern subscriptions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pubsub-numpat/">valkey.io</seealso>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of patterns all clients are subscribed to.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long patternCount = await client.PubSubNumPatAsync();
    /// Console.WriteLine($"Active pattern subscriptions: {patternCount}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> PubSubNumPatAsync(CommandFlags flags = CommandFlags.None);
}
