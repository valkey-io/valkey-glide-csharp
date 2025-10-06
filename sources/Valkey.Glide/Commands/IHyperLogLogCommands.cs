// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "HyperLogLog" group for a standalone client.
/// <br />
/// See <see href="https://valkey.io/commands#hyperloglog">HyperLogLog Commands</see>.
/// </summary>
public interface IHyperLogLogCommands
{
    /// <summary>
    /// Adds one element to the HyperLogLog data structure stored at the specified key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfadd/"/>
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <param name="element">The element to add to the HyperLogLog.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>
    /// <see langword="true"/> if at least one HyperLogLog internal register was altered, 
    /// <see langword="false"/> otherwise.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.HyperLogLogAddAsync("my_hll", "element1");
    /// Console.WriteLine(result); // Output: True (if this is a new element)
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Adds multiple elements to the HyperLogLog data structure stored at the specified key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfadd/"/>
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <param name="elements">The elements to add to the HyperLogLog.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>
    /// <see langword="true"/> if at least one HyperLogLog internal register was altered, 
    /// <see langword="false"/> otherwise.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.HyperLogLogAddAsync("my_hll", ["element1", "element2", "element3"]);
    /// Console.WriteLine(result); // Output: True (if at least one element is new)
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue[] elements, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the approximated cardinality computed by the HyperLogLog data structure stored at the specified key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfcount/"/>
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>
    /// The approximated number of unique elements observed by the HyperLogLog at key.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long count = await client.HyperLogLogLengthAsync("my_hll");
    /// Console.WriteLine(count); // Output: approximated cardinality
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the approximated cardinality of the union of the HyperLogLogs stored at the specified keys.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfcount/"/>
    /// <param name="keys">The keys of the HyperLogLogs.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>
    /// The approximated number of unique elements observed by the union of the HyperLogLogs.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long count = await client.HyperLogLogLengthAsync(["hll1", "hll2", "hll3"]);
    /// Console.WriteLine(count); // Output: approximated cardinality of union
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HyperLogLogLengthAsync(ValkeyKey[] keys, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Merges multiple HyperLogLog values into a unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfmerge/"/>
    /// <param name="destination">The key of the destination HyperLogLog.</param>
    /// <param name="first">The key of the first source HyperLogLog.</param>
    /// <param name="second">The key of the second source HyperLogLog.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HyperLogLogMergeAsync("dest_hll", "hll1", "hll2");
    /// </code>
    /// </example>
    /// </remarks>
    Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Merges multiple HyperLogLog values into a unique value that will approximate the cardinality of the union of the observed Sets of the source HyperLogLog structures.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfmerge/"/>
    /// <param name="destination">The key of the destination HyperLogLog.</param>
    /// <param name="sourceKeys">The keys of the source HyperLogLogs.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HyperLogLogMergeAsync("dest_hll", ["hll1", "hll2", "hll3"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey[] sourceKeys, CommandFlags flags = CommandFlags.None);
}