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
}