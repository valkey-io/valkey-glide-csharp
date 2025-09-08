// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

namespace Valkey.Glide;

public interface IServer
{
    /// <summary>
    /// Gets the address of the connected server.
    /// </summary>
    EndPoint EndPoint { get; }

    /// <summary>
    /// Gets whether the connection to the server is active and usable.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// The protocol being used to communicate with this server (if not connected/known, then the anticipated protocol from the configuration is returned, assuming success).
    /// </summary>
    Protocol Protocol { get; }

    /// <summary>
    /// Gets the version of the connected server.
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// Gets the operating mode of the connected server.
    /// </summary>
    ServerType ServerType { get; }

    /// <summary>
    /// Execute an arbitrary command against the server; this is primarily intended for
    /// executing modules, but may also be used to provide access to new features that lack
    /// a direct API.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="args">The arguments to pass for the command.</param>
    /// <returns>A dynamic representation of the command's result.</returns>
    /// <remarks>This API should be considered an advanced feature; inappropriate use can be harmful.</remarks>
    ValkeyResult Execute(string command, params object[] args);

    /// <inheritdoc cref="Execute(string, object[])"/>
    Task<ValkeyResult> ExecuteAsync(string command, params object[] args);

    /// <summary>
    /// Execute an arbitrary command against the server; this is primarily intended for
    /// executing modules, but may also be used to provide access to new features that lack
    /// a direct API.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="args">The arguments to pass for the command.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>A dynamic representation of the command's result.</returns>
    /// <remarks>This API should be considered an advanced feature; inappropriate use can be harmful.</remarks>
    ValkeyResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Execute(string, ICollection{object}, CommandFlags)"/>
    Task<ValkeyResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The INFO command returns information and statistics about the server in a format that is simple to parse by computers and easy to read by humans.
    /// </summary>
    /// <param name="section">The info section to get, if getting a specific one.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The entire raw <c>INFO</c> string.</returns>
    /// <remarks><seealso href="https://valkey.io/commands/info/"/></remarks>
    Task<string?> InfoRawAsync(ValkeyValue section = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The INFO command returns information and statistics about the server in a format that is simple to parse by computers and easy to read by humans.
    /// </summary>
    /// <param name="section">The info section to get, if getting a specific one.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>A grouping of key/value pairs, grouped by their section header.</returns>
    /// <remarks><seealso href="https://valkey.io/commands/info/"/></remarks>
    Task<IGrouping<string, KeyValuePair<string, string>>[]> InfoAsync(ValkeyValue section = default, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="InfoRawAsync(ValkeyValue, CommandFlags)"/>
    string? InfoRaw(ValkeyValue section = default, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="InfoAsync(ValkeyValue, CommandFlags)"/>
    IGrouping<string, KeyValuePair<string, string>>[] Info(ValkeyValue section = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// This command is often used to test if a connection is still alive, or to measure latency.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <returns>The observed latency.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan result = await client.PingAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// This command is often used to test if a connection is still alive, or to measure latency.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping"/>
    /// <param name="message">The message to send.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <returns>The observed latency.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan result = await client.PingAsync("ping!");
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan> PingAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the same message passed in.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/echo"/>
    /// <param name="message">The message to echo.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <returns>The provided message.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.EchoAsync(key, value);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Get all configuration parameters matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <returns>All matching configuration parameters.</returns>
    /// <remarks><seealso href="https://valkey.io/commands/config-get"/></remarks>
    KeyValuePair<string, string>[] ConfigGet(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ConfigGet(ValkeyValue, CommandFlags)"/>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Resets the statistics reported by Valkey using the INFO command.
    /// </summary>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <remarks><seealso href="https://valkey.io/commands/config-resetstat"/></remarks>
    void ConfigResetStatistics(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ConfigResetStatistics(CommandFlags)"/>
    Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file the server was started with,
    /// applying the minimal changes needed to make it reflecting the configuration currently
    /// used by the server, that may be different compared to the original one because of the use of the CONFIG SET command.
    /// </summary>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <remarks><seealso href="https://valkey.io/commands/config-rewrite"/></remarks>
    void ConfigRewrite(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ConfigRewrite(CommandFlags)"/>
    Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// You can change both trivial parameters or switch from one to another persistence option using this command.
    /// </summary>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <remarks><seealso href="https://valkey.io/commands/config-set"/></remarks>
    void ConfigSet(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ConfigSet(ValkeyValue, ValkeyValue, CommandFlags)"/>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of keys in the currently-selected database.
    /// </summary>
    /// <param name="database">The database ID.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <remarks><seealso href="https://valkey.io/commands/dbsize"/></remarks>
    long DatabaseSize(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="DatabaseSize(int, CommandFlags)"/>
    Task<long> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Delete all the keys of all databases on the server.
    /// </summary>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <remarks><seealso href="https://valkey.io/commands/flushall"/></remarks>
    void FlushAllDatabases(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="FlushAllDatabases(CommandFlags)"/>
    Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Delete all the keys of the database.
    /// </summary>
    /// <param name="database">The database ID.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <remarks><seealso href="https://valkey.io/commands/flushdb"/></remarks>
    void FlushDatabase(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="FlushDatabase(int, CommandFlags)"/>
    Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// A client may check if a BGSAVE command succeeded reading the LASTSAVE value, then issuing a BGSAVE command
    /// and checking at regular intervals every N seconds if LASTSAVE changed.
    /// </summary>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <returns>The last time a save was performed.</returns>
    /// <remarks><seealso href="https://valkey.io/commands/lastsave"/></remarks>
    DateTime LastSave(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="LastSave(CommandFlags)"/>
    Task<DateTime> LastSaveAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// Use the <see cref="DateTime.ToLocalTime"/> method to get local time.
    /// </summary>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <returns>The server's current time.</returns>
    /// <remarks><seealso href="https://valkey.io/commands/time"/></remarks>
    DateTime Time(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Time(CommandFlags)"/>
    Task<DateTime> TimeAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The LOLWUT command displays the Valkey version and a piece of generative computer art.
    /// </summary>
    /// <param name="flags">The command flags to use. Currently flags are ignored.</param>
    /// <returns>A string containing the Valkey version and generative art.</returns>
    /// <remarks><seealso href="https://valkey.io/commands/lolwut"/></remarks>
    string Lolwut(CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="Lolwut(CommandFlags)"/>
    Task<string> LolwutAsync(CommandFlags flags = CommandFlags.None);

    /// Gets the name of the current connection.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The name of the client connection as a <see cref="string"/>.
    /// If no name is assigned, <see langword="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await server.ClientGetNameAsync();
    /// if (result != ValkeyValue.Null)
    /// {
    ///     Console.WriteLine($"Connection name: {result}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("No connection name set");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the current connection ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-id"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The ID of the client connection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long connectionId = await server.ClientIdAsync();
    /// Console.WriteLine($"Connection ID: {connectionId}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ClientIdAsync(CommandFlags flags = CommandFlags.None);
}
