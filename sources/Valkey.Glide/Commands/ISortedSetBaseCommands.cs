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
    /// Adds or updates a member in the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to add or update.</param>
    /// <param name="score">The score for the member.</param>
    /// <returns><see langword="true"/> if the member was added. <see langword="false"/> if the member was updated.</returns>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score);

    /// <summary>
    /// Adds or updates members in the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members and their scores to add or update.</param>
    /// <returns>The number of members added to the sorted set.</returns>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> members);

    /// <summary>
    /// Removes the specified member from the sorted set stored at key.
    /// Non existing members are ignored.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrem"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to remove from the sorted set.</param>
    /// <returns><see langword="true"/> if the member was removed. <see langword="false"/> if the member was not a member of the sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.SortedSetRemoveAsync(key, "member1");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member);

    /// <summary>
    /// Removes the specified members from the sorted set stored at key.
    /// Non existing members are ignored.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrem"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">A collection of members to remove from the sorted set.</param>
    /// <returns>The number of members that were removed from the sorted set, not including non existing members.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.SortedSetRemoveAsync(key, ["member1", "member2"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <summary>
    /// Returns a random member from the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrandmember"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The key of the sorted set.</param>
    /// <returns>The randomly selected element, or <see langword="null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.SortedSetRandomMemberAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key);

    /// <summary>
    /// Returns an array of random members from the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrandmember"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="count">The number of members to return.</param>
    /// <returns>An array of randomly selected elements.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.SortedSetRandomMembersAsync(key, 3);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count);

    /// <summary>
    /// Returns the score of member in the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zscore"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member whose score is to be retrieved.</param>
    /// <returns>The score of the member. If member does not exist in the sorted set, <see langword="null"/> is returned. If key does not exist, <see langword="null"/> is returned.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// double? score = await client.SortedSetScoreAsync(key, "member1");
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member);

    /// <summary>
    /// Returns the scores associated with the specified members in the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmscore"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to get the scores for.</param>
    /// <returns>
    /// An array of scores corresponding to members.
    /// If a member does not exist in the sorted set, the corresponding value in the list will be <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// double?[] scores = await client.SortedSetScoresAsync(key, ["member1", "member2", "member3"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);
}
