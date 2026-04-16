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
}
