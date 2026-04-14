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
}
