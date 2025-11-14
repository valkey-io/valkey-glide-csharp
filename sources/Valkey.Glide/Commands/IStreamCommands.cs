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
    /// <param name="messageId">The message ID. Use null or "*" for auto-generation.</param>
    /// <param name="maxLength">The maximum length of the stream. If specified, old entries will be trimmed.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim. Only applicable when useApproximateMaxLength is true.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The ID of the added entry.</returns>
    /// <exception cref="RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Appends a new entry to a stream with multiple field-value pairs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <param name="messageId">The message ID. Use null or "*" for auto-generation.</param>
    /// <param name="maxLength">The maximum length of the stream. If specified, old entries will be trimmed.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim. Only applicable when useApproximateMaxLength is true.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The ID of the added entry.</returns>
    /// <exception cref="RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);
}
