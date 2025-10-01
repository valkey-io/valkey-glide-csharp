// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Options.InfoOptions;

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Server Management" group for a standalone client.
/// <br />
/// See <see href="https://valkey.io/commands#server">Server Management Commands</see>.
/// </summary>
public interface IServerManagementCommands
{
    /// <summary>
    /// Get information and statistics about the server using <see cref="Section.DEFAULT" /> option.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <inheritdoc cref="IServerManagementClusterCommands.InfoAsync()" path="/remarks" />
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
    /// <inheritdoc cref="IServerManagementClusterCommands.InfoAsync(Section[])" path="/remarks" />
    /// <inheritdoc cref="IServerManagementClusterCommands.InfoAsync(Section[])" path="/param" />
    /// <returns>
    /// <inheritdoc cref="InfoAsync()" />
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string info = await client.InfoAsync([Section.SERVER, Section.MEMORY]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> InfoAsync(Section[] sections);

    /// <summary>
    /// Echo the given message back from the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/echo/"/>
    /// <param name="message">The message to echo</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>The echoed message as a <see cref="ValkeyValue"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.EchoAsync("Hello World");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Ping the server and measure the round-trip time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>The round-trip time as a <see cref="TimeSpan"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan latency = await client.PingAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Ping the server with a message and measure the round-trip time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="message">The message to send with the ping</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>The round-trip time as a <see cref="TimeSpan"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan latency = await client.PingAsync("test message");
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan> PingAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the values of configuration parameters.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/"/>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>All matching configuration parameters.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// KeyValuePair&lt;string, string&gt;[] config = await client.ConfigGetAsync("max*");
    /// </code>
    /// </example>
    /// </remarks>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Resets the statistics reported by the server using the INFO and LATENCY HISTOGRAM.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-resetstat/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigResetStatisticsAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file the server was started with,
    /// applying the minimal changes needed to make it reflecting the configuration currently
    /// used by the server, that may be different compared to the original one because of the use of the CONFIG SET command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigRewriteAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// You can change both trivial parameters or switch from one to another persistence option using this command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/"/>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigSetAsync("maxmemory", "100mb");
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of keys in the currently-selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dbsize/"/>
    /// <param name="database">The database ID.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>The number of keys in the currently selected database.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long keyCount = await client.DatabaseSizeAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Deletes all the keys of all the existing databases.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushAllDatabasesAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Deletes all the keys of the currently selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/"/>
    /// <param name="database">The database ID.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushDatabaseAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// A client may check if a BGSAVE command succeeded reading the LASTSAVE value, then issuing a BGSAVE command
    /// and checking at regular intervals every N seconds if LASTSAVE changed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>UNIX TIME of the last DB save executed with success.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// DateTime lastSave = await client.LastSaveAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<DateTime> LastSaveAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// Use the <see cref="DateTime.ToLocalTime"/> method to get local time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>The server's current time.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// DateTime serverTime = await client.TimeAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<DateTime> TimeAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Displays a piece of generative computer art of the specific Valkey version and it's optional arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A string containing the Valkey version and generative art.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string art = await client.LolwutAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> LolwutAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Changes the currently selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/select"/>
    /// <param name="index">The index of the database to select.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>A simple "OK" response.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string result = await client.SelectAsync(1);
    /// Console.WriteLine(result); // Output: "OK"
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> SelectAsync(long index, CommandFlags flags = CommandFlags.None);
}
