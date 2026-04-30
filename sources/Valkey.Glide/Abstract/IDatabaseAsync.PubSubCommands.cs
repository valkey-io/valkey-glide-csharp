// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IPubSubBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <summary>
    /// Posts a message to the given channel.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/publish/">Valkey commands – PUBLISH</seealso>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of clients that received the message.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var subscriberCount = await db.PublishAsync(ValkeyChannel.Literal("news"), "Breaking news!");
    /// Console.WriteLine($"Delivered message to {subscriberCount} subscriber(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> PublishAsync(ValkeyChannel channel, ValkeyValue message, CommandFlags flags = CommandFlags.None);
}
