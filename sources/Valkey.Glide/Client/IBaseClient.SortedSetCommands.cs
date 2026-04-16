// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// GLIDE-specific sorted set commands that are not part of the shared interface.
/// </summary>
public partial interface IBaseClient
{
    /// <summary>
    /// Iterates elements of Sorted Set key and their associated scores using a cursor.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zscan"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="options">Optional scan options including pattern and count hint.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Scan all members
    /// await foreach (SortedSetEntry entry in client.SortedSetScanAsync(key))
    /// {
    ///     Console.WriteLine($"{entry.Element}: {entry.Score}");
    /// }
    ///
    /// // Scan with pattern
    /// var options = new ScanOptions { MatchPattern = "*pattern*" };
    /// await foreach (SortedSetEntry entry in client.SortedSetScanAsync(key, options))
    /// {
    ///     Console.WriteLine($"{entry.Element}: {entry.Score}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ScanOptions? options = null);
}
