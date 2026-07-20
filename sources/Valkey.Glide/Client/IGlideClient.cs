// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <summary>
/// Interface for Valkey GLIDE standalone client.
/// </summary>
public partial interface IGlideClient :
    IBaseClient,
    IGenericCommands,
    IServerManagementStandaloneCommands
{
    /// <summary>
    /// Atomically transfers keys from the source instance to the destination instance.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/migrate/">Valkey commands – MIGRATE</seealso>
    /// <param name="keys">The keys to migrate. Must not be empty.</param>
    /// <param name="options">The migrate options.</param>
    /// <returns><see langword="true"/> if at least one key was migrated successfully, <see langword="false"/> if no keys were found.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key1", "value1");
    /// await client.SetAsync("key2", "value2");
    /// using var options = new MigrateOptions("desthost", 6379, 0, TimeSpan.FromSeconds(5));
    /// var migrated = await client.MigrateAsync(["key1", "key2"], options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> MigrateAsync(IEnumerable<ValkeyKey> keys, MigrateOptions options);

    /// <summary>
    /// Incrementally iterates over the matching keys in the database.
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
    IAsyncEnumerable<ValkeyKey> ScanAsync(ScanOptions? options = null);
}
