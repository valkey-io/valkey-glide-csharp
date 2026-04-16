// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// GLIDE-specific set commands that are not part of the shared interface.
/// </summary>
public partial interface IBaseClient
{
    /// <summary>
    /// Iterates elements over a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sscan"/>
    /// <param name="key">The key of the set.</param>
    /// <param name="options">Optional scan options including pattern and count hint.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Scan all members
    /// await foreach (ValkeyValue value in client.SetScanAsync(key))
    /// {
    ///     Console.WriteLine(value);
    /// }
    ///
    /// // Scan with pattern
    /// var options = new ScanOptions { MatchPattern = "*pattern*" };
    /// await foreach (ValkeyValue value in client.SetScanAsync(key, options))
    /// {
    ///     Console.WriteLine(value);
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ScanOptions? options = null);

    /// <summary>
    /// Returns the members of the set resulting from the union of all given sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunion"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sets to union.</param>
    /// <returns>A set containing all members that exist in at least one of the given sets.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b"]);
    /// await client.SetAddAsync("set2", ["b", "c"]);
    /// var result = await client.SetUnionAsync(["set1", "set2"]);  // result is {"a", "b", "c"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetUnionAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the members of the set resulting from the intersection of all given sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sinter"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sets to intersect.</param>
    /// <returns>A set containing only members that exist in all of the given sets.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// var result = await client.SetInterAsync(["set1", "set2"]);  // result is {"b", "c"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetInterAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the members of the set resulting from the difference between the first set and all successive sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sdiff"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sets. The first key is the base set; subsequent keys are subtracted from it.</param>
    /// <returns>A set containing members that exist in the first set but not in any of the subsequent sets.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// var result = await client.SetDiffAsync(["set1", "set2"]);  // result is {"a"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetDiffAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Stores the union of all given sets into <paramref name="destination"/>.
    /// Returns the number of elements in the resulting set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunionstore"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets to union.</param>
    /// <returns>The number of elements in the resulting set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b"]);
    /// await client.SetAddAsync("set2", ["b", "c"]);
    /// long count = await client.SetUnionStoreAsync("dest", ["set1", "set2"]); // count is 3, dest contains {"a", "b", "c"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Stores the intersection of all given sets into <paramref name="destination"/>.
    /// Returns the number of elements in the resulting set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sinterstore"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets to intersect.</param>
    /// <returns>The number of elements in the resulting set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// long count = await client.SetInterStoreAsync("dest", ["set1", "set2"]);  // count is 2, dest contains {"b", "c"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetInterStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Stores the difference between the first set and all successive sets into <paramref name="destination"/>.
    /// Returns the number of elements in the resulting set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sdiffstore"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets. The first key is the base set; subsequent keys are subtracted from it.</param>
    /// <returns>The number of elements in the resulting set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// long count = await client.SetDiffStoreAsync("dest", ["set1", "set2"]);  // count is 1, dest contains {"a"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetDiffStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the number of elements in the intersection of all given sets.
    /// Optionally limited by <paramref name="limit"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sintercard"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sets to intersect.</param>
    /// <param name="limit">
    /// The maximum number of elements to count. A value of <c>0</c> means no limit.
    /// </param>
    /// <returns>The number of elements in the intersection, bounded by <paramref name="limit"/> if specified.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// long count = await client.SetInterCardAsync(["set1", "set2"]);              // count is 2
    /// long limited = await client.SetInterCardAsync(["set1", "set2"], limit: 1);  // limited is 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <summary>
    /// Returns whether the specified value is a member of the set stored at <paramref name="key"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sismember"/>
    /// <param name="key">The key of the set.</param>
    /// <param name="value">The value to check for membership.</param>
    /// <returns><see langword="true"/> if the value is a member of the set; <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// bool isMember = await client.SetIsMemberAsync("myset", "a");   // isMember is true
    /// bool notMember = await client.SetIsMemberAsync("myset", "z");  // notMember is false
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetIsMemberAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Returns whether each specified value is a member of the set stored at <paramref name="key"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/smismember"/>
    /// <param name="key">The key of the set.</param>
    /// <param name="values">The values to check for membership.</param>
    /// <returns>
    /// An array of <see cref="bool"/> values, one per input value, where each element is
    /// <see langword="true"/> if the corresponding value is a member of the set, or <see langword="false"/> otherwise.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// bool[] results = await client.SetIsMemberAsync("myset", ["a", "z"]);  // results is [true, false]
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool[]> SetIsMemberAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Returns all members of the set stored at <paramref name="key"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/smembers"/>
    /// <param name="key">The key of the set.</param>
    /// <returns>A set containing all members, or an empty set if the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var members = await client.SetMembersAsync("myset");  // members is {"a", "b", "c"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetMembersAsync(ValkeyKey key);

    /// <summary>
    /// Returns the set cardinality (number of elements) of the set stored at <paramref name="key"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/scard"/>
    /// <param name="key">The key of the set.</param>
    /// <returns>The cardinality (number of elements) of the set, or 0 if the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// long count = await client.SetCardAsync("myset");  // count is 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetCardAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns the specified number of random elements from the set stored at <paramref name="key"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/spop"/>
    /// <param name="key">The key of the set.</param>
    /// <param name="count">
    /// The number of members to pop.
    /// If count is larger than the set's cardinality, pops the entire set.
    /// </param>
    /// <returns>A set of popped elements, or an empty set when the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var popped = await client.SetPopAsync("myset", 2);  // popped contains 2 random elements
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetPopAsync(ValkeyKey key, long count);
}
