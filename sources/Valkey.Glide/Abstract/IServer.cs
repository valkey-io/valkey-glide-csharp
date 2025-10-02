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
    /// Get the values of configuration parameters.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get"/>
    /// <param name="pattern">The pattern to match configuration parameters. If not specified, returns all parameters.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>An array of key-value pairs representing configuration parameters and their values.</returns>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reset the statistics reported by the INFO command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-resetstat"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Rewrite the configuration file with the current configuration.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Set a configuration parameter to the given value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set"/>
    /// <param name="setting">The configuration parameter to set.</param>
    /// <param name="value">The value to set for the configuration parameter.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the number of keys in the selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dbsize"/>
    /// <param name="database">The database index. If -1, uses the current database.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The number of keys in the database.</returns>
    Task<long> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Delete all the keys of all databases on the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Delete all the keys of the selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb"/>
    /// <param name="database">The database index. If -1, uses the current database.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the UNIX timestamp of the last successful save to disk.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The timestamp of the last successful save.</returns>
    Task<DateTime> LastSaveAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the current server time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The current server time.</returns>
    Task<DateTime> TimeAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Display some computer art and the Valkey version.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>A string containing computer art and version information.</returns>
    Task<string> LolwutAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
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
