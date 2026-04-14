// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Set commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#set">Valkey – Set Commands</seealso>
public interface ISetBaseCommands
{
    /// <summary>
    /// Adds specified members to the set stored at key.
    /// Specified members that are already a member of this set are ignored.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sadd"/>
    /// <param name="key">The key where members will be added to its set.</param>
    /// <param name="value">The value to add to the set.</param>
    /// <returns><see langword="true"/> if the specified member was not already present in the set, else <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.SetAddAsync(key, value);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Adds specified members to the set stored at key.
    /// Specified members that are already a member of this set are ignored.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sadd"/>
    /// <param name="key">The key where members will be added to its set.</param>
    /// <param name="values">The values to add to the set.</param>
    /// <returns>The number of elements that were added to the set, not including all the elements already present into the set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.SetAddAsync(key, [value1, value2]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Removes specified members from the set stored at key.
    /// Specified members that are not a member of this set are ignored.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/srem"/>
    /// <param name="key">The key from which members will be removed.</param>
    /// <param name="value">The value to remove.</param>
    /// <returns><see langword="true"/> if the specified member was already present in the set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.SetRemoveAsync(key, value);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Removes specified members from the set stored at key.
    /// Specified members that are not a member of this set are ignored.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/srem"/>
    /// <param name="key">The key from which members will be removed.</param>
    /// <param name="values">The values to remove.</param>
    /// <returns>The number of members that were removed from the set, excluding non-existing members.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.SetRemoveAsync(key, [value1, value2]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Removes and returns one random member from the set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/spop"/>
    /// <param name="key">The key of the set.</param>
    /// <returns>The removed element, or <see cref="ValkeyValue.Null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.SetPopAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> SetPopAsync(ValkeyKey key);

    /// <summary>
    /// Returns a random element from the set value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/srandmember"/>
    /// <param name="key">The key from which to retrieve the set member.</param>
    /// <returns>The randomly selected element, or <see cref="ValkeyValue.Null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.SetRandomMemberAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key);

    /// <summary>
    /// Returns multiple random members from the set value stored at key.
    /// If <paramref name="count"/> is positive, returns unique elements (no repetition) up to count or the set size, whichever is smaller.
    /// If <paramref name="count"/> is negative, returns elements with possible repetition (the same element may be returned multiple times),
    /// and the number of returned elements is the absolute value of count.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/srandmember"/>
    /// <param name="key">The key from which to retrieve the set members.</param>
    /// <param name="count">The number of members to return.</param>
    /// <returns>
    ///	An array of random elements from the set.
    ///	When count is positive, the returned elements are unique (no repetitions).
    ///	When count is negative, the returned elements may contain duplicates.
    ///	If the set does not exist or is empty, an empty array is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.SetRandomMembersAsync(key, 3);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count);

    /// <summary>
    /// Moves <paramref name="value"/> from the set at <paramref name="source"/> to the set at <paramref name="destination"/>, removing it from the source set.
    /// Creates a new destination set if needed. The operation is atomic.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/smove"/>
    /// <note>When in cluster mode, <paramref name="source"/> and <paramref name="destination"/> must map to the same hash slot.</note>
    /// <param name="source">The key of the set to remove the element from.</param>
    /// <param name="destination">The key of the set to add the element to.</param>
    /// <param name="value">The set element to move.</param>
    /// <returns><see langword="true"/> if the element is moved, <see langword="false"/> if the source set does not exist or the element is not a member of the source set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.SetMoveAsync(sourceKey, destKey, value);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value);

    // TODO #287
    /// <summary>
    /// Iterates elements over a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sscan"/>
    /// <param name="key">The key of the set.</param>
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="pageSize">The page size to iterate by.</param>
    /// <param name="cursor">The cursor position to start at.</param>
    /// <param name="pageOffset">The page offset to start at.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await foreach (ValkeyValue value in client.SetScanAsync(key, "*pattern*"))
    /// {
    ///     // Process each value
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0);
}
