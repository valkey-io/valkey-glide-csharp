// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// HyperLogLog commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#hyperloglog">Valkey – HyperLogLog Commands</seealso>
public interface IHyperLogLogBaseCommands
{
    /// <summary>
    /// Adds one element to a HyperLogLog data structure.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfadd/">Valkey commands – PFADD</seealso>
    /// <param name="key">The HyperLogLog key.</param>
    /// <param name="element">The element to add.</param>
    /// <returns>
    /// <see langword="true"/> if at least one HyperLogLog internal register was altered,
    /// <see langword="false"/> otherwise.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var altered = await client.HyperLogLogAddAsync("myhll", "element1");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element);

    /// <summary>
    /// Adds multiple elements to a HyperLogLog data structure.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfadd/">Valkey commands – PFADD</seealso>
    /// <param name="key">The HyperLogLog key.</param>
    /// <param name="elements">The elements to add.</param>
    /// <returns>
    /// <see langword="true"/> if at least one HyperLogLog internal register was altered,
    /// <see langword="false"/> otherwise.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var altered = await client.HyperLogLogAddAsync("myhll", ["a", "b", "c"]);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements);

    /// <summary>
    /// Returns the approximated cardinality of a HyperLogLog data structure.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfcount/">Valkey commands – PFCOUNT</seealso>
    /// <param name="key">The HyperLogLog key.</param>
    /// <returns>
    /// The approximated number of unique elements observed by the HyperLogLog.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HyperLogLogAddAsync("myhll", ["a", "b", "c"]);
    /// var count = await client.HyperLogLogLengthAsync("myhll");  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HyperLogLogLengthAsync(ValkeyKey key);

    /// <summary>
    /// Returns the approximated cardinality of the union of multiple HyperLogLog data structures.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfcount/">Valkey commands – PFCOUNT</seealso>
    /// <param name="keys">The HyperLogLog keys.</param>
    /// <returns>
    /// The approximated number of unique elements observed by the union of the HyperLogLogs.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HyperLogLogAddAsync("hll1", ["a", "b"]);
    /// await client.HyperLogLogAddAsync("hll2", ["b", "c"]);
    /// var count = await client.HyperLogLogLengthAsync(["hll1", "hll2"]);  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Merges multiple HyperLogLog values into a single key that approximates the cardinality of the union.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfmerge/">Valkey commands – PFMERGE</seealso>
    /// <param name="destination">The destination HyperLogLog key.</param>
    /// <param name="first">The first source HyperLogLog key.</param>
    /// <param name="second">The second source HyperLogLog key.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HyperLogLogMergeAsync("dest_hll", "hll1", "hll2");
    /// </code>
    /// </example>
    /// </remarks>
    Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <summary>
    /// Merges multiple HyperLogLog values into a single key that approximates the cardinality of the union.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/pfmerge/">Valkey commands – PFMERGE</seealso>
    /// <param name="destination">The destination HyperLogLog key.</param>
    /// <param name="sourceKeys">The source HyperLogLog keys.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HyperLogLogMergeAsync("dest_hll", ["hll1", "hll2", "hll3"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys);
}
