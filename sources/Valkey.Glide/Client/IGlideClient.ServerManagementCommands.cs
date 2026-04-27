// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Options.InfoOptions;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <summary>
/// Server management commands for Valkey GLIDE standalone client.
/// </summary>
/// <seealso href="https://valkey.io/commands/#server">Valkey – Server Management Commands</seealso>
public partial interface IGlideClient
{
    /// <summary>
    /// Get information and statistics about the server using <see cref="Section.DEFAULT" /> option.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <inheritdoc cref="IGlideClusterClient.InfoAsync()" path="/remarks" />
    /// <returns>A <see langword="string" /> containing the information for the sections requested.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string info = await client.InfoAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> InfoAsync();

    /// <summary>
    /// Get information and statistics about the server.<br />
    /// Starting from server version 7, command supports multiple <see cref="Section" /> arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <inheritdoc cref="IGlideClusterClient.InfoAsync(IEnumerable{Section})" path="/remarks" />
    /// <inheritdoc cref="IGlideClusterClient.InfoAsync(IEnumerable{Section})" path="/param" />
    /// <returns>
    /// <inheritdoc cref="InfoAsync()" />
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var sections = new Section[] { Section.SERVER, Section.MEMORY };
    /// string info = await client.InfoAsync(sections);
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> InfoAsync(IEnumerable<Section> sections);

    /// <summary>
    /// Gets the values of configuration parameters.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/"/>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <returns>All matching configuration parameters.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// KeyValuePair&lt;string, string&gt;[] config = await client.ConfigGetAsync("max*");
    /// </code>
    /// </example>
    /// </remarks>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default);

    /// <summary>
    /// Resets the statistics reported by the server using the INFO and LATENCY HISTOGRAM.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-resetstat/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigResetStatisticsAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigResetStatisticsAsync();

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file the server was started with,
    /// applying the minimal changes needed to make it reflecting the configuration currently
    /// used by the server, that may be different compared to the original one because of the use of the CONFIG SET command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigRewriteAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigRewriteAsync();

    /// <summary>
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// You can change both trivial parameters or switch from one to another persistence option using this command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/"/>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigSetAsync("maxmemory", "100mb");
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value);

    /// <summary>
    /// Returns the number of keys in the currently-selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dbsize/"/>
    /// <returns>The number of keys in the currently-selected database.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long keyCount = await client.DatabaseSizeAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> DatabaseSizeAsync();

    /// <summary>
    /// Deletes all the keys of all the existing databases.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushAllDatabasesAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushAllDatabasesAsync();

    /// <summary>
    /// Deletes all the keys of the currently selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushDatabaseAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync();

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// A client may check if a BGSAVE command succeeded reading the LASTSAVE value, then issuing a BGSAVE command
    /// and checking at regular intervals every N seconds if LASTSAVE changed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave/"/>
    /// <returns>UNIX TIME of the last DB save executed with success.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var lastSave = await client.LastSaveAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<DateTimeOffset> LastSaveAsync();

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// Use the <see cref="DateTimeOffset.ToLocalTime"/> method to get local time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time/"/>
    /// <returns>The server's current time.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var serverTime = await client.TimeAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<DateTimeOffset> TimeAsync();

    /// <summary>
    /// Displays a piece of generative computer art of the specific Valkey version and it's optional arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/"/>
    /// <returns>A string containing the Valkey version and generative art.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string art = await client.LolwutAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> LolwutAsync();
}
