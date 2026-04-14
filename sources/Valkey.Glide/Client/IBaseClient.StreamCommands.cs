// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

public partial interface IBaseClient
{
    /// <summary>
    /// Appends a new entry to a stream with a single field-value pair.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamField">The field name.</param>
    /// <param name="streamValue">The field value.</param>
    /// <param name="options">Optional arguments for the XADD command.</param>
    /// <returns>The ID of the added entry, or null if <see cref="StreamAddOptions.MakeStream"/> is <c>false</c> and the stream doesn't exist.</returns>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions? options = null);

    /// <summary>
    /// Appends a new entry to a stream with multiple field-value pairs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <param name="options">Optional arguments for the XADD command.</param>
    /// <returns>The ID of the added entry, or null if <see cref="StreamAddOptions.MakeStream"/> is <c>false</c> and the stream doesn't exist.</returns>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions? options = null);

    /// <summary>
    /// Claims pending messages for a consumer with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTime">The minimum idle time for the message to be claimed.</param>
    /// <param name="messageIds">A collection of message IDs to claim.</param>
    /// <param name="options">Optional arguments for the XCLAIM command.</param>
    /// <returns>An array of claimed stream entries.</returns>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options);

    /// <summary>
    /// Claims pending messages for a consumer with additional options, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTime">The minimum idle time for the message to be claimed.</param>
    /// <param name="messageIds">A collection of message IDs to claim.</param>
    /// <param name="options">Optional arguments for the XCLAIM command.</param>
    /// <returns>An array of claimed message IDs.</returns>
    Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options);
}
