// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Sets commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#set">Valkey – Set Commands</seealso>
public interface ISetBaseCommands
{
    /// <summary>
    /// Adds a member to a set. Ignores members that are already present.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sadd/">Valkey commands – SADD</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="value">The value to add.</param>
    /// <returns><see langword="true"/> if the member was not already present in the set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var added = await client.SetAddAsync("myset", "member");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Adds one or more members to a set. Ignores members that are already present.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sadd/">Valkey commands – SADD</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="values">The values to add.</param>
    /// <returns>The number of elements that were added to the set, not including elements already present.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var addedCount = await client.SetAddAsync("myset", ["a", "b", "c"]);  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Removes a member from a set. Ignores members that are not present.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/srem/">Valkey commands – SREM</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="value">The value to remove.</param>
    /// <returns><see langword="true"/> if the member was present in the set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", "member");
    /// var removed = await client.SetRemoveAsync("myset", "member");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Removes one or more members from a set. Ignores members that are not present.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/srem/">Valkey commands – SREM</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="values">The values to remove.</param>
    /// <returns>The number of members that were removed from the set, excluding non-existing members.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var removedCount = await client.SetRemoveAsync("myset", ["a", "b"]);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Removes and returns a random member from a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/spop/">Valkey commands – SPOP</seealso>
    /// <param name="key">The set key.</param>
    /// <returns>The removed element, or <see cref="ValkeyValue.Null"/> when the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var popped = await client.SetPopAsync("myset");  // "a", "b", or "c"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> SetPopAsync(ValkeyKey key);

    /// <summary>
    /// Returns a random element from a set without removing it.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/srandmember/">Valkey commands – SRANDMEMBER</seealso>
    /// <param name="key">The set key.</param>
    /// <returns>The randomly selected element, or <see cref="ValkeyValue.Null"/> when the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var member = await client.SetRandomMemberAsync("myset");  // "a", "b", or "c"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key);

    /// <summary>
    /// Returns multiple random members from a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/srandmember/">Valkey commands – SRANDMEMBER</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="count">The number of members to return.
    /// If positive, returns unique elements up to <paramref name="count"/> or the set size, whichever is smaller.
    /// If negative, allows duplicates and returns exactly <c>abs(count)</c> elements.
    /// </param>
    /// <returns>
    /// An array of random elements from the set, or an empty array if the set does not exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var members = await client.SetRandomMembersAsync("myset", 2);  // ["a", "c"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count);

    /// <summary>
    /// Moves a member from one set to another atomically.
    /// Creates the destination set if needed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/smove/">Valkey commands – SMOVE</seealso>
    /// <note>When in cluster mode, <paramref name="source"/> and <paramref name="destination"/> must map to the same hash slot.</note>
    /// <param name="source">The source set key.</param>
    /// <param name="destination">The destination set key.</param>
    /// <param name="value">The set element to move.</param>
    /// <returns><see langword="true"/> if the element was moved, <see langword="false"/> if the source set does not exist or the element is not a member of the source set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("source", ["a", "b"]);
    /// var moved = await client.SetMoveAsync("source", "dest", "a");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value);
}
