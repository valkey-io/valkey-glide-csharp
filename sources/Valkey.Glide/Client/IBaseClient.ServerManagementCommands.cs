// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by Valkey GLIDE clients but NOT by StackExchange.Redis databases. Methods implemented
/// by both should be added to the corresponding Commands interface instead.

/// <summary>
/// Server management commands for Valkey GLIDE clients that have no StackExchange.Redis equivalent.
/// </summary>
/// <seealso href="https://valkey.io/commands/#server">Valkey – Server Management Commands</seealso>
public partial interface IBaseClient
{
    /// <summary>
    /// Sets multiple server configuration parameters at runtime.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/">Valkey commands – CONFIG SET</seealso>
    /// <param name="parameters">A dictionary of configuration parameter names and their new values.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var parameters = new Dictionary&lt;ValkeyValue, ValkeyValue&gt;
    /// {
    ///     ["maxmemory"] = "100mb",
    ///     ["maxmemory-policy"] = "allkeys-lru",
    /// };
    /// await client.ConfigSetAsync(parameters);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters);

    /// <summary>
    /// Gets the values of configuration parameters matching the specified patterns.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/">Valkey commands – CONFIG GET</seealso>
    /// <param name="patterns">The patterns to match against configuration parameter names.</param>
    /// <returns>An array of key-value pairs for all matching configuration parameters.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] patterns = ["max*", "bind*"];
    /// var config = await client.ConfigGetAsync(patterns);  // Patterns matching max* or bind*
    /// </code>
    /// </example>
    /// </remarks>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns);

    /// <summary>
    /// Deletes all keys from all databases.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/">Valkey commands – FLUSHALL</seealso>
    /// <param name="mode">The flush mode. <see cref="FlushMode.Sync"/> waits for completion,
    /// <see cref="FlushMode.Async"/> returns immediately while the flush continues in the background.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushAllDatabasesAsync(FlushMode.Async);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushAllDatabasesAsync(FlushMode mode);

    /// <summary>
    /// Deletes all keys from the currently selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/">Valkey commands – FLUSHDB</seealso>
    /// <param name="mode">The flush mode. <see cref="FlushMode.Sync"/> waits for completion,
    /// <see cref="FlushMode.Async"/> returns immediately while the flush continues in the background.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushDatabaseAsync(FlushMode.Async);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync(FlushMode mode);

    /// <summary>
    /// Displays generative computer art and the Valkey version.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/">Valkey commands – LOLWUT</seealso>
    /// <param name="options">The LOLWUT options specifying version and/or parameters.</param>
    /// <returns>A string containing the Valkey version and generative art.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new LolwutOptions { Version = 6, Parameters = [40, 20] };
    /// var art = await client.LolwutAsync(options);
    /// Console.WriteLine(art);  // Print art to console
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> LolwutAsync(LolwutOptions options);
}
