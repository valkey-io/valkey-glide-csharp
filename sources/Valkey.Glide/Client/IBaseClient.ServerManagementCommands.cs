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
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// This overload allows setting multiple configuration parameters in a single call.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/"/>
    /// <param name="parameters">A dictionary of configuration parameter names and their new values.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var parameters = new Dictionary&lt;ValkeyValue, ValkeyValue&gt;
    /// {
    ///     { "maxmemory", "100mb" },
    ///     { "maxmemory-policy", "allkeys-lru" }
    /// };
    /// await client.ConfigSetAsync(parameters);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters);

    /// <summary>
    /// Gets the values of configuration parameters matching multiple patterns.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/"/>
    /// <param name="patterns">The patterns of config values to get.</param>
    /// <returns>All matching configuration parameters.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var patterns = new ValkeyValue[] { "max*", "bind*" };
    /// var config = await client.ConfigGetAsync(patterns);
    /// </code>
    /// </example>
    /// </remarks>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns);

    /// <summary>
    /// Deletes all the keys of all the existing databases, with the specified flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/"/>
    /// <param name="mode">The flush mode to use. <see cref="FlushMode.Sync"/> waits for completion,
    /// <see cref="FlushMode.Async"/> returns immediately while flush continues in background.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var mode = FlushMode.Async;
    /// await client.FlushAllDatabasesAsync(mode);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushAllDatabasesAsync(FlushMode mode);

    /// <summary>
    /// Deletes all the keys of the currently selected database, with the specified flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/"/>
    /// <param name="mode">The flush mode to use. <see cref="FlushMode.Sync"/> waits for completion,
    /// <see cref="FlushMode.Async"/> returns immediately while flush continues in background.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var mode = FlushMode.Async;
    /// await client.FlushDatabaseAsync(mode);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync(FlushMode mode);

    /// <summary>
    /// Displays a piece of generative computer art and the Valkey version.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/"/>
    /// <param name="options">The LOLWUT options specifying version and/or parameters.</param>
    /// <returns>A string containing the Valkey version and generative art.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new LolwutOptions { Version = 6, Parameters = new int[] { 40, 20 } };
    /// string art = await client.LolwutAsync(options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> LolwutAsync(LolwutOptions options);
}
