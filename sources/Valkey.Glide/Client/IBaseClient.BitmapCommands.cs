// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// BITFIELD commands for Valkey GLIDE clients.
/// These commands are not supported by StackExchange.Redis.
/// </summary>
public partial interface IBaseClient
{
    /// <summary>
    /// Reads or modifies the array of bits representing the string stored at key based on the specified subcommands.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The subcommands to execute (GET, SET, INCRBY, OVERFLOW).</param>
    /// <returns>
    /// An array of results from the executed subcommands.
    /// <para>
    /// <b>Important:</b> Null values in the array indicate overflow when using <c>OVERFLOW FAIL</c>.
    /// This differs from other clients that may convert null to 0, which would mask overflow failures.
    /// </para>
    /// </returns>
    /// <remarks>
    /// <para>
    /// When using <c>OVERFLOW FAIL</c>, if an operation would cause an overflow, the server returns null
    /// for that operation. This method preserves that null value to allow callers to detect overflow conditions.
    /// </para>
    /// <example>
    /// <code>
    /// var subCommands = new IBitFieldSubCommand[] {
    ///     new BitFieldOverflow(OverflowType.Fail),
    ///     new BitFieldIncrBy(Encoding.Unsigned(8), new BitOffset(0), 200),
    ///     new BitFieldIncrBy(Encoding.Unsigned(8), new BitOffset(0), 200) // Will overflow
    /// };
    /// long?[] results = await client.BitFieldAsync("mykey", subCommands);
    /// // results[0] = 200 (first increment succeeded)
    /// // results[1] = null (overflow occurred - NOT masked as 0)
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?[]> BitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands);

    /// <summary>
    /// Reads the array of bits representing the string stored at key based on the specified GET subcommands.
    /// This is a read-only variant of BITFIELD that can be routed to replicas.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield_ro"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The GET subcommands to execute.</param>
    /// <returns>An array of results from the executed GET subcommands.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var subCommands = new IBitFieldReadOnlySubCommand[] {
    ///     new BitFieldGet(Encoding.Unsigned(8), new BitOffset(0))
    /// };
    /// long[] results = await client.BitFieldReadOnlyAsync("mykey", subCommands);
    /// Console.WriteLine(results[0]); // Output: 65 (ASCII 'A')
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> BitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands);
}
