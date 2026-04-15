// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="pageSize">The number of elements to return per iteration (hint to the server).</param>
    /// <param name="cursor">The cursor position to start at.</param>
    /// <param name="pageOffset">The number of elements to skip from the first page.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await foreach (SortedSetEntry entry in client.SortedSetScanAsync(key, "*pattern*"))
    /// {
    ///     // Process each entry
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0);
}
