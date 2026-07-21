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
    /// Moves key from the currently selected database to the specified destination database.
    /// When key already exists in the destination database, or it does not exist in the source database, it does nothing.
    /// It is possible to use MOVE as a locking primitive because of this.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/move"/>
    /// <param name="key">The key to move.</param>
    /// <param name="database">The database to move the key to.</param>
    /// <returns><see langword="true"/> if key was moved. <see langword="false"/> if key was not moved.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyKey key = "mykey";
    /// bool result = await client.MoveAsync(key, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> MoveAsync(ValkeyKey key, int database);

    /// <summary>
    /// Copies the value stored at the source to the destination key in the specified database. When
    /// replace is true, removes the destination key first if it already
    /// exists, otherwise performs no action.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/copy"/>
    /// <param name="source">The key to the source value.</param>
    /// <param name="destination">The key where the value should be copied to.</param>
    /// <param name="destinationDatabase">The database ID to store destination in.</param>
    /// <param name="replace">Whether to overwrite an existing values at destination.</param>
    /// <returns><see langword="true"/> if source was copied. <see langword="false"/> if source was not copied.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyKey source = "source";
    /// ValkeyKey dest = "dest";
    /// bool result = await client.CopyAsync(source, dest, 1, replace: true);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> CopyAsync(ValkeyKey source, ValkeyKey destination, int destinationDatabase, bool replace = false);

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
