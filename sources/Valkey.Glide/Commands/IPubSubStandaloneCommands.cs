// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// PubSub commands specific to standalone clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/#pubsub">valkey.io</see>.
/// </summary>
public interface IPubSubStandaloneCommands : IPubSubCommands
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
    /// long subscriberCount = await client.PublishAsync("news", "Breaking news!");
    /// Console.WriteLine($"Message delivered to {subscriberCount} subscribers");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None);
}
