// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a pre-loaded Lua script that can be executed using EVALSHA.
/// This provides StackExchange.Redis compatibility for scripts that have been loaded onto the server.
/// </summary>
/// <remarks>
/// LoadedLuaScript is created by calling IServer.ScriptLoad() or LuaScript.Load().
/// It contains the SHA1 hash of the script, allowing execution via EVALSHA without
/// transmitting the script source code.
///
/// Example:
/// <code>
/// var script = LuaScript.Prepare("return redis.call('GET', @key)");
/// var loaded = await script.LoadAsync(server);
/// var result = await loaded.EvaluateAsync(db, new { key = "mykey" });
/// </code>
/// </remarks>
public sealed class LoadedLuaScript
{
    /// <summary>
    /// Initializes a new instance of the LoadedLuaScript class.
    /// </summary>
    /// <param name="script">The LuaScript that was loaded.</param>
    /// <param name="hash">The SHA1 hash of the script.</param>
    internal LoadedLuaScript(LuaScript script, byte[] hash)
    {
        Script = script ?? throw new ArgumentNullException(nameof(script));
        Hash = hash ?? throw new ArgumentNullException(nameof(hash));
    }

    /// <summary>
    /// Gets the LuaScript that was loaded.
    /// </summary>
    private LuaScript Script { get; }

    /// <summary>
    /// Gets the original script text with @parameter syntax.
    /// </summary>
    public string OriginalScript => Script.OriginalScript;

    /// <summary>
    /// Gets the executable script text with KEYS[] and ARGV[] substitutions.
    /// </summary>
    public string ExecutableScript => Script.ExecutableScript;

    /// <summary>
    /// Gets the SHA1 hash of the script.
    /// </summary>
    public byte[] Hash { get; }

    /// <summary>
    /// Evaluates the loaded script using EVALSHA synchronously.
    /// </summary>
    /// <param name="db">The database to execute the script on.</param>
    /// <param name="parameters">An object containing parameter values. Properties/fields should match parameter names.</param>
    /// <param name="withKeyPrefix">Optional key prefix to apply to all keys.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The result of the script execution.</returns>
    /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
    /// <exception cref="ArgumentException">Thrown when parameters object is missing required properties or has invalid types.</exception>
    /// <remarks>
    /// This method uses EVALSHA to execute the script by its hash, which is more efficient than
    /// transmitting the full script source. If the script is not cached on the server, a NOSCRIPT
    /// error will be thrown.
    ///
    /// Example:
    /// <code>
    /// var loaded = await script.LoadAsync(server);
    /// var result = loaded.Evaluate(db, new { key = new ValkeyKey("mykey"), value = "myvalue" });
    /// </code>
    /// </remarks>
    public ValkeyResult Evaluate(IDatabase db, object? parameters = null,
        ValkeyKey? withKeyPrefix = null, CommandFlags flags = CommandFlags.None)
    {
        if (db == null)
        {
            throw new ArgumentNullException(nameof(db));
        }

        (ValkeyKey[] keys, ValkeyValue[] args) = Script.ExtractParametersInternal(parameters, withKeyPrefix);

        // Call IDatabase.ScriptEvaluate with hash (will be implemented in task 15.1)
        // For now, we'll use Execute to call EVALSHA directly
        List<object> evalArgs = [Hash];
        evalArgs.Add(keys.Length);
        evalArgs.AddRange(keys.Cast<object>());
        evalArgs.AddRange(args.Cast<object>());

        return db.Execute("EVALSHA", evalArgs, flags);
    }

    /// <summary>
    /// Asynchronously evaluates the loaded script using EVALSHA.
    /// </summary>
    /// <param name="db">The database to execute the script on.</param>
    /// <param name="parameters">An object containing parameter values. Properties/fields should match parameter names.</param>
    /// <param name="withKeyPrefix">Optional key prefix to apply to all keys.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the script execution.</returns>
    /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
    /// <exception cref="ArgumentException">Thrown when parameters object is missing required properties or has invalid types.</exception>
    /// <remarks>
    /// This method uses EVALSHA to execute the script by its hash, which is more efficient than
    /// transmitting the full script source. If the script is not cached on the server, a NOSCRIPT
    /// error will be thrown.
    ///
    /// Example:
    /// <code>
    /// var loaded = await script.LoadAsync(server);
    /// var result = await loaded.EvaluateAsync(db, new { key = new ValkeyKey("mykey"), value = "myvalue" });
    /// </code>
    /// </remarks>
    public async Task<ValkeyResult> EvaluateAsync(IDatabaseAsync db, object? parameters = null,
        ValkeyKey? withKeyPrefix = null, CommandFlags flags = CommandFlags.None)
    {
        if (db == null)
        {
            throw new ArgumentNullException(nameof(db));
        }

        (ValkeyKey[] keys, ValkeyValue[] args) = Script.ExtractParametersInternal(parameters, withKeyPrefix);

        // Call IDatabaseAsync.ScriptEvaluateAsync with hash (will be implemented in task 15.1)
        // For now, we'll use ExecuteAsync to call EVALSHA directly
        List<object> evalArgs = [Hash];
        evalArgs.Add(keys.Length);
        evalArgs.AddRange(keys.Cast<object>());
        evalArgs.AddRange(args.Cast<object>());

        return await db.ExecuteAsync("EVALSHA", evalArgs, flags).ConfigureAwait(false);
    }
}
