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
    /// Returns the number of entries in a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xlen"/>
    /// <param name="key">The key of the stream.</param>
    /// <returns>The number of entries in the stream.</returns>
    Task<long> StreamLengthAsync(ValkeyKey key);

    /// <summary>
    /// Removes one or more messages from a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xdel"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="messageIds">A collection of message IDs to delete.</param>
    /// <returns>The number of messages deleted.</returns>
    Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds);
}
