// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Stream commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#stream">Valkey – Stream Commands</seealso>
public interface IStreamBaseCommands
{
    /// <summary>
    /// Acknowledges a message in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xack/">Valkey commands – XACK</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="messageId">The message ID to acknowledge.</param>
    /// <returns><see langword="true"/> if the message was acknowledged, or <see langword="false"/> if it was not pending for the group.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var acknowledged = await client.StreamAcknowledgeAsync("mystream", "mygroup", "1526569495631-0");
    /// Console.WriteLine($"Message acknowledged: {acknowledged}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId);

    /// <summary>
    /// Acknowledges a message in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xack/">Valkey commands – XACK</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="messageIds">The message IDs to acknowledge.</param>
    /// <returns>The number of messages acknowledged.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var acked = await client.StreamAcknowledgeAsync("mystream", "mygroup", ["1526569495631-0", "1526569495632-0"]);
    /// Console.WriteLine($"Acknowledged {acked} message(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds);

    /// <summary>
    /// Removes a message from a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xdel/">Valkey commands – XDEL</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="messageId">The message ID to delete.</param>
    /// <returns>
    /// <see langword="true"/> if the message was deleted
    /// or <see langword="false"/> if it does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var deleted = await client.StreamDeleteAsync("mystream", "1526569495631-0");
    /// Console.WriteLine($"Deleted: {deleted}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> StreamDeleteAsync(ValkeyKey key, ValkeyValue messageId);

    /// <summary>
    /// Deletes messages from a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xdel/">Valkey commands – XDEL</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="messageIds">The message IDs to delete.</param>
    /// <returns>The number of messages deleted.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var deleted = await client.StreamDeleteAsync("mystream", ["1526569495631-0", "1526569495632-0"]);
    /// Console.WriteLine($"Deleted {deleted} message(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds);

    /// <summary>
    /// Returns information about a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-stream/">Valkey commands – XINFO STREAM</seealso>
    /// <param name="key">The stream key.</param>
    /// <returns>Information about the stream.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var info = await client.StreamInfoAsync("mystream");
    /// Console.WriteLine($"Stream length: {info.Length}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamInfo> StreamInfoAsync(ValkeyKey key);

    /// <summary>
    /// Returns the number of entries in a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xlen/">Valkey commands – XLEN</seealso>
    /// <param name="key">The stream key.</param>
    /// <returns>The number of entries in the stream.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var length = await client.StreamLengthAsync("mystream");
    /// Console.WriteLine($"Stream length: {length}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> StreamLengthAsync(ValkeyKey key);

    /// <summary>
    /// Returns information about the pending messages.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending/">Valkey commands – XPENDING</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <returns>Information about the pending messages.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var pending = await client.StreamPendingAsync("mystream", "mygroup");
    /// Console.WriteLine($"Pending: {pending.PendingMessageCount}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName);
}
