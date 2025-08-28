// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Server Management" group for batch operations.
/// </summary>
public interface IBatchServerManagementCommands
{
    /// <summary>
    /// Get all configuration parameters matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <returns>A task representing the batch operation.</returns>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default);

    /// <summary>
    /// Resets the statistics reported by Valkey using the INFO command.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    Task ConfigResetStatisticsAsync();

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    Task ConfigRewriteAsync();

    /// <summary>
    /// The CONFIG SET command is used to reconfigure the server at runtime.
    /// </summary>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <returns>A task representing the batch operation.</returns>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value);

    /// <summary>
    /// Returns the number of keys in the currently-selected database.
    /// </summary>
    /// <param name="database">The database ID.</param>
    /// <returns>A task representing the batch operation.</returns>
    Task<long> DatabaseSizeAsync(int database = -1);

    /// <summary>
    /// Delete all the keys of all databases on the server.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    Task FlushAllDatabasesAsync();

    /// <summary>
    /// Delete all the keys of the database.
    /// </summary>
    /// <param name="database">The database ID.</param>
    /// <returns>A task representing the batch operation.</returns>
    Task FlushDatabaseAsync(int database = -1);

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    Task<DateTime> LastSaveAsync();

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    Task<DateTime> TimeAsync();

    /// <summary>
    /// The LOLWUT command displays the Valkey version and a piece of generative computer art.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    Task<string> LolwutAsync();
}
