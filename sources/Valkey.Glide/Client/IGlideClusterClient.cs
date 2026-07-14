// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <summary>
/// Interface for Valkey GLIDE cluster client.
/// </summary>
public partial interface IGlideClusterClient :
    IBaseClient,
    IGenericClusterCommands,
    IPubSubClusterCommands,
    IServerManagementClusterCommands,
    ITransactionClusterCommands
{
    /// <summary>
    /// Incrementally iterates over the matching keys in the cluster.
    /// </summary>
    /// <param name="options">Optional scan options including pattern, count hint, and type filter.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching keys.</returns>
    /// <example>
    /// <code>
    /// // Scan all keys
    /// await foreach (var key in client.ScanAsync())
    /// {
    ///     Console.WriteLine(key);
    /// }
    ///
    /// // Scan with pattern and type filter
    /// var options = new ScanOptions { MatchPattern = "user:*", Type = ValkeyType.String };
    /// await foreach (var key in client.ScanAsync(options))
    /// {
    ///     Console.WriteLine(key);
    /// }
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/scan/">SCAN command</seealso>
    /// <seealso href="https://glide.valkey.io/how-to/scan-cluster/">Valkey GLIDE – Scan a Cluster</seealso>
    IAsyncEnumerable<ValkeyKey> ScanAsync(ScanOptions? options = null);
}
