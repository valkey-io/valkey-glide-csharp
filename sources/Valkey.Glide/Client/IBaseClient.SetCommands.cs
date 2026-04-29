// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// GLIDE-specific set commands that are not part of the shared interface.
/// </summary>
public partial interface IBaseClient
{
    /// <summary>
    /// Iterates over elements of a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sscan/">Valkey commands – SSCAN</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="options">Optional scan options including pattern and count hint.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await foreach (var member in client.SetScanAsync("myset"))
    /// {
    ///     Console.WriteLine(member);
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var options = new ScanOptions { MatchPattern = "*pattern*" };
    /// await foreach (var member in client.SetScanAsync("myset", options))
    /// {
    ///     Console.WriteLine(member);
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ScanOptions? options = null);

    /// <summary>
    /// Returns the members of the set resulting from the union of all given sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunion/">Valkey commands – SUNION</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sets to union.</param>
    /// <returns>A set containing all members that exist in at least one of the given sets.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b"]);
    /// await client.SetAddAsync("set2", ["b", "c"]);
    /// var union = await client.SetUnionAsync(["set1", "set2"]);
    /// // union contains {"a", "b", "c"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetUnionAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the members of the set resulting from the intersection of all given sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sinter/">Valkey commands – SINTER</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sets to intersect.</param>
    /// <returns>A set containing only members that exist in all of the given sets.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// var intersection = await client.SetInterAsync(["set1", "set2"]);
    /// // intersection contains {"b", "c"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetInterAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the members of the set resulting from the difference between the first set and all successive sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sdiff/">Valkey commands – SDIFF</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sets. The first key is the base set; subsequent keys are subtracted from it.</param>
    /// <returns>A set containing members that exist in the first set but not in any of the subsequent sets.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// var difference = await client.SetDiffAsync(["set1", "set2"]);
    /// // difference contains {"a"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetDiffAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Stores the union of all given sets into <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sunionstore/">Valkey commands – SUNIONSTORE</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets to union.</param>
    /// <returns>The number of elements in the resulting set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b"]);
    /// await client.SetAddAsync("set2", ["b", "c"]);
    /// var count = await client.SetUnionStoreAsync("dest", ["set1", "set2"]);  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Stores the intersection of all given sets into <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sinterstore/">Valkey commands – SINTERSTORE</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets to intersect.</param>
    /// <returns>The number of elements in the resulting set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// var count = await client.SetInterStoreAsync("dest", ["set1", "set2"]);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetInterStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Stores the difference between the first set and all successive sets into <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sdiffstore/">Valkey commands – SDIFFSTORE</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key of the destination set.</param>
    /// <param name="keys">The keys of the sets. The first key is the base set; subsequent keys are subtracted from it.</param>
    /// <returns>The number of elements in the resulting set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("set1", ["a", "b", "c"]);
    /// await client.SetAddAsync("set2", ["b", "c", "d"]);
    /// var count = await client.SetDiffStoreAsync("dest", ["set1", "set2"]);  // 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetDiffStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the number of elements in the intersection of all given sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sintercard/">Valkey commands – SINTERCARD</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>Since Valkey 7.0.0.</note>
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
    /// var count = await client.SetInterCardAsync(["set1", "set2"]);  // 2
    ///
    /// var limited = await client.SetInterCardAsync(["set1", "set2"], limit: 1);  // 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <summary>
    /// Checks whether <paramref name="value"/> is a member of a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sismember/">Valkey commands – SISMEMBER</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="value">The value to check for membership.</param>
    /// <returns><see langword="true"/> if the value is a member of the set; <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var isMember = await client.SetIsMemberAsync("myset", "a");   // true
    ///
    /// var notMember = await client.SetIsMemberAsync("myset", "z");  // false
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetIsMemberAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Checks whether each specified value is a member of a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/smismember/">Valkey commands – SMISMEMBER</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="values">The values to check for membership.</param>
    /// <returns>
    /// An array of <see cref="bool"/> values, one per input value, where each element is
    /// <see langword="true"/> if the corresponding value is a member of the set, or <see langword="false"/> otherwise.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var membership = await client.SetIsMemberAsync("myset", ["a", "z"]);
    /// // membership[0] == true, membership[1] == false
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool[]> SetIsMemberAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Returns all members of a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/smembers/">Valkey commands – SMEMBERS</seealso>
    /// <param name="key">The set key.</param>
    /// <returns>A set containing all members, or an empty set if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var members = await client.SetMembersAsync("myset");
    /// // members contains {"a", "b", "c"}
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetMembersAsync(ValkeyKey key);

    /// <summary>
    /// Returns the number of elements in a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/scard/">Valkey commands – SCARD</seealso>
    /// <param name="key">The set key.</param>
    /// <returns>The cardinality (number of elements) of the set, or <c>0</c> if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var count = await client.SetCardAsync("myset");  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SetCardAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns the specified number of random elements from a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/spop/">Valkey commands – SPOP</seealso>
    /// <param name="key">The set key.</param>
    /// <param name="count">
    /// The number of members to pop.
    /// If <paramref name="count"/> is larger than the set's cardinality, the entire set is returned.
    /// </param>
    /// <returns>A set of popped elements, or an empty set if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAddAsync("myset", ["a", "b", "c"]);
    /// var popped = await client.SetPopAsync("myset", 2);
    /// // popped.Count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> SetPopAsync(ValkeyKey key, long count);
}
