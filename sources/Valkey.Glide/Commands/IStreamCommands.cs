// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports stream commands for standalone and cluster clients.
/// <br/>
/// See more on <see href="https://valkey.io/commands#stream">valkey.io</see>.
/// </summary>
public interface IStreamCommands
{
    /// <summary>
    /// Appends a new entry to a stream with a single field-value pair.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamField">The field name.</param>
    /// <param name="streamValue">The field value.</param>
    /// <param name="messageId">The message ID. Use null or "*" for auto-generation, or "&lt;ms&gt;-*" to auto-generate sequence number for a specific timestamp.</param>
    /// <param name="maxLength">The maximum length of the stream. If specified, old entries will be trimmed. Mutually exclusive with minId.</param>
    /// <param name="minId">Trim entries with IDs lower than this. Mutually exclusive with maxLength.</param>
    /// <param name="useApproximateTrimming">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim. Only applicable when useApproximateTrimming is true.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="noMakeStream">If true, the stream will not be created if it doesn't exist. Returns null if stream doesn't exist.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The ID of the added entry, or null if noMakeStream is true and the stream doesn't exist.</returns>
    /// <exception cref="RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Appends a new entry to a stream with multiple field-value pairs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <param name="messageId">The message ID. Use null or "*" for auto-generation, or "&lt;ms&gt;-*" to auto-generate sequence number for a specific timestamp.</param>
    /// <param name="maxLength">The maximum length of the stream. If specified, old entries will be trimmed. Mutually exclusive with minId.</param>
    /// <param name="minId">Trim entries with IDs lower than this. Mutually exclusive with maxLength.</param>
    /// <param name="useApproximateTrimming">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim. Only applicable when useApproximateTrimming is true.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="noMakeStream">If true, the stream will not be created if it doesn't exist. Returns null if stream doesn't exist.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The ID of the added entry, or null if noMakeStream is true and the stream doesn't exist.</returns>
    /// <exception cref="RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from a single stream starting from a given position.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="position">The position from which to start reading. Use "0-0" to read from the beginning.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="block">The maximum time to block waiting for entries (in milliseconds). Use null for non-blocking.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of stream entries, or an empty array if no entries are available.</returns>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null, int? block = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from multiple streams starting from given positions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread"/>
    /// <param name="streamPositions">Array of stream keys and their starting positions.</param>
    /// <param name="count">The maximum number of entries to return per stream.</param>
    /// <param name="block">The maximum time to block waiting for entries (in milliseconds). Use null for non-blocking.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of streams with their entries, or an empty array if no entries are available.</returns>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<ValkeyStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? count = null, int? block = null, CommandFlags flags = CommandFlags.None);
}
