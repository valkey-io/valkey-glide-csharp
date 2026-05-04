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
    /// <seealso href="https://valkey.io/commands/info/">Valkey commands – INFO</seealso>
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
    /// <seealso href="https://valkey.io/commands/info/">Valkey commands – INFO</seealso>
    /// <param name="sections">A list of <see cref="Section" /> values specifying which sections of information to
    /// retrieve. When no parameter is provided, the <see cref="Section.DEFAULT" /> option is assumed.</param>
    /// <returns>
    /// <inheritdoc cref="InfoAsync()" path="/returns" />
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Section[] sections = [Section.SERVER, Section.MEMORY];
    /// string info = await client.InfoAsync(sections);
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> InfoAsync(IEnumerable<Section> sections);

    /// <summary>
    /// Gets the values of configuration parameters.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/">Valkey commands – CONFIG GET</seealso>
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
    /// <seealso href="https://valkey.io/commands/config-resetstat/">Valkey commands – CONFIG RESETSTAT</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigResetStatisticsAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigResetStatisticsAsync();

    /// <summary>
    /// Rewrites the <c>valkey.conf</c> file the server was started with, applying the minimal changes needed
    /// to make it reflect the configuration currently used by the server, which may differ from the original
    /// because of the use of the <c>CONFIG SET</c> command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite/">Valkey commands – CONFIG REWRITE</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigRewriteAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigRewriteAsync();

    /// <summary>
    /// Reconfigures the server at runtime without the need to restart Valkey. Callers can change trivial
    /// parameters or switch from one persistence option to another using this command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/">Valkey commands – CONFIG SET</seealso>
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
    /// <seealso href="https://valkey.io/commands/dbsize/">Valkey commands – DBSIZE</seealso>
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
    /// <seealso href="https://valkey.io/commands/flushall/">Valkey commands – FLUSHALL</seealso>
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
    /// <seealso href="https://valkey.io/commands/flushdb/">Valkey commands – FLUSHDB</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushDatabaseAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync();

    /// <summary>
    /// Returns the time of the last DB save executed with success.
    /// A client may check if a <c>BGSAVE</c> command succeeded by reading the <c>LASTSAVE</c> value, then
    /// issuing a <c>BGSAVE</c> command and checking at regular intervals whether <c>LASTSAVE</c> changed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave/">Valkey commands – LASTSAVE</seealso>
    /// <returns>UNIX time of the last DB save executed with success.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var lastSave = await client.LastSaveAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<DateTimeOffset> LastSaveAsync();

    /// <summary>
    /// Returns the current server time in UTC format.
    /// Use the <see cref="DateTimeOffset.ToLocalTime"/> method to get local time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time/">Valkey commands – TIME</seealso>
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
    /// Displays a piece of generative computer art of the specific Valkey version and its optional arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/">Valkey commands – LOLWUT</seealso>
    /// <returns>A <see langword="string" /> containing the Valkey version and generative art.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string art = await client.LolwutAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> LolwutAsync();
}
