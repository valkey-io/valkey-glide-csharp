// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Server Management" group for batch operations.
/// </summary>
internal interface IBatchServerManagementCommands
{
    /// <summary>
    /// Get all configuration parameters matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <returns>A task representing the batch operation.</returns>
    IBatch ConfigGetAsync(ValkeyValue pattern = default);

    /// <summary>
    /// Resets the statistics reported by Valkey using the INFO command.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    IBatch ConfigResetStatisticsAsync();

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    IBatch ConfigRewriteAsync();

    /// <summary>
    /// The CONFIG SET command is used to reconfigure the server at runtime.
    /// </summary>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <returns>A task representing the batch operation.</returns>
    IBatch ConfigSetAsync(ValkeyValue setting, ValkeyValue value);

    /// <summary>
    /// Returns the number of keys in the currently-selected database.
    /// </summary>
    /// <param name="database">The database ID.</param>
    /// <returns>A task representing the batch operation.</returns>
    IBatch DatabaseSizeAsync(int database = -1);

    /// <summary>
    /// Delete all the keys of all databases on the server.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    IBatch FlushAllDatabasesAsync();

    /// <summary>
    /// Delete all the keys of the database.
    /// </summary>
    /// <param name="database">The database ID.</param>
    /// <returns>A task representing the batch operation.</returns>
    IBatch FlushDatabaseAsync(int database = -1);

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    IBatch LastSaveAsync();

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    IBatch TimeAsync();

    /// <summary>
    /// The LOLWUT command displays the Valkey version and a piece of generative computer art.
    /// </summary>
    /// <returns>A task representing the batch operation.</returns>
    IBatch LolwutAsync();
}
