// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Sorted set commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#sorted-set">Valkey – Sorted Set Commands</seealso>
public interface ISortedSetBaseCommands
{
    /// <summary>
    /// Adds or updates the score for a member in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd/">Valkey commands – ZADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to add or update.</param>
    /// <param name="score">The score for the member.</param>
    /// <returns><see langword="true"/> if the member was added, <see langword="false"/> if the member was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var added = await client.SortedSetAddAsync("myzset", "member1", 1.0);
    /// Console.WriteLine($"Added: {added}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score);

    /// <summary>
    /// Adds or updates the scores for members in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd/">Valkey commands – ZADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">The members and their scores to add or update.</param>
    /// <returns>The number of members added to the sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entries = new[] { new SortedSetEntry("a", 1.0), new SortedSetEntry("b", 2.0) };
    /// var addedCount = await client.SortedSetAddAsync("myzset", entries);
    /// Console.WriteLine($"Added: {addedCount}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> members);

    /// <summary>
    /// Removes a member from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrem/">Valkey commands – ZREM</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to remove.</param>
    /// <returns><see langword="true"/> if the member was removed, <see langword="false"/> if the member does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", "member1", 1.0);
    /// var removed = await client.SortedSetRemoveAsync("myzset", "member1");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member);

    /// <summary>
    /// Removes one or more members from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrem/">Valkey commands – ZREM</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A collection of members to remove.</param>
    /// <returns>The number of members that were removed from the sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var removedCount = await client.SortedSetRemoveAsync("myzset", ["a", "b"]);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <summary>
    /// Returns a random member from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrandmember/">Valkey commands – ZRANDMEMBER</seealso>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The randomly selected element, or <see langword="null"/> when the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", [new SortedSetEntry("a", 1.0), new SortedSetEntry("b", 2.0)]);
    /// var member = await client.SortedSetRandomMemberAsync("myzset");  // "a" or "b"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key);

    /// <summary>
    /// Returns an array of random members from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrandmember/">Valkey commands – ZRANDMEMBER</seealso>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <param name="count">The number of members to return.</param>
    /// <returns>An array of randomly selected elements.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var members = await client.SortedSetRandomMembersAsync("myzset", 2);  // ["a", "b"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count);

    /// <summary>
    /// Returns the score of a member in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zscore/">Valkey commands – ZSCORE</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member whose score is to be retrieved.</param>
    /// <returns>The score of the member, or <see langword="null"/> if the member or key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", "member1", 1.5);
    /// var score = await client.SortedSetScoreAsync("myzset", "member1");  // 1.5
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member);

    /// <summary>
    /// Returns the scores associated with the specified members in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmscore/">Valkey commands – ZMSCORE</seealso>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">The members to get the scores for.</param>
    /// <returns>
    /// An array of scores corresponding to members.
    /// If a member does not exist in the sorted set, the corresponding value is <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var scores = await client.SortedSetScoresAsync("myzset", ["a", "b", "nonexistent"]);  // [1.0, 2.0, null]
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);
}
