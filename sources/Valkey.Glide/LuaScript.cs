// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;

namespace Valkey.Glide;

/// <summary>
/// Represents a Lua script with named parameter support for StackExchange.Redis compatibility.
/// Scripts are cached using weak references to avoid repeated parsing of the same script text.
/// </summary>
/// <remarks>
/// LuaScript provides a high-level API for executing Lua scripts with named parameters using
/// the @parameter syntax. Parameters are automatically extracted and converted to KEYS and ARGV
/// arrays based on their types.
///
/// Example:
/// <code>
/// var script = LuaScript.Prepare("return redis.call('GET', @key)");
/// var result = await script.EvaluateAsync(db, new { key = "mykey" });
/// </code>
/// </remarks>
public sealed class LuaScript
{
    private static readonly ConcurrentDictionary<string, WeakReference<LuaScript>> Cache = new();

    /// <summary>
    /// Gets the original script with @parameter syntax.
    /// </summary>
    public string OriginalScript { get; }

    /// <summary>
    /// Gets the executable script with KEYS[] and ARGV[] substitutions.
    /// </summary>
    public string ExecutableScript { get; }

    /// <summary>
    /// Gets the parameter names in order of first appearance.
    /// </summary>
    internal string[] Arguments { get; }

    /// <summary>
    /// Initializes a new instance of the LuaScript class.
    /// </summary>
    /// <param name="originalScript">The original script with @parameter syntax.</param>
    /// <param name="executableScript">The executable script with placeholders.</param>
    /// <param name="arguments">The parameter names.</param>
    internal LuaScript(string originalScript, string executableScript, string[] arguments)
    {
        OriginalScript = originalScript;
        ExecutableScript = executableScript;
        Arguments = arguments;
    }

    /// <summary>
    /// Prepares a script with named parameters for execution.
    /// Scripts are cached using weak references to avoid repeated parsing.
    /// </summary>
    /// <param name="script">Script with @parameter syntax.</param>
    /// <returns>A LuaScript instance ready for execution.</returns>
    /// <exception cref="ArgumentException">Thrown when script is null or empty.</exception>
    /// <remarks>
    /// The Prepare method caches scripts using weak references. If a script is no longer
    /// referenced elsewhere, it may be garbage collected and will be re-parsed on next use.
    ///
    /// Example:
    /// <code>
    /// var script = LuaScript.Prepare("return redis.call('SET', @key, @value)");
    /// </code>
    /// </remarks>
    public static LuaScript Prepare(string script)
    {
        if (string.IsNullOrEmpty(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        // Check cache first
        if (Cache.TryGetValue(script, out WeakReference<LuaScript>? weakRef) && weakRef.TryGetTarget(out LuaScript? cachedScript))
        {
            return cachedScript;
        }

        // Parse the script
        (string originalScript, string executableScript, string[] parameters) = ScriptParameterMapper.PrepareScript(script);
        LuaScript luaScript = new(originalScript, executableScript, parameters);

        // Cache with weak reference
        Cache[script] = new WeakReference<LuaScript>(luaScript);

        return luaScript;
    }

    /// <summary>
    /// Purges the script cache, removing all cached scripts.
    /// </summary>
    /// <remarks>
    /// This method clears the internal cache of prepared scripts. Subsequent calls to
    /// Prepare will re-parse scripts even if they were previously cached.
    ///
    /// This is primarily useful for testing or when you want to ensure scripts are
    /// re-parsed (e.g., after modifying script text).
    /// </remarks>
    public static void PurgeCache() => Cache.Clear();

    /// <summary>
    /// Gets the number of scripts currently cached.
    /// </summary>
    /// <returns>The count of cached scripts, including those with weak references that may have been collected.</returns>
    /// <remarks>
    /// This count includes entries in the cache dictionary, but some may have weak references
    /// to scripts that have been garbage collected. The actual number of live scripts may be lower.
    ///
    /// This method is primarily useful for testing and diagnostics.
    /// </remarks>
    public static int GetCachedScriptCount() => Cache.Count;

    /// <summary>
    /// Evaluates the script on the specified database synchronously.
    /// </summary>
    /// <param name="db">The database to execute the script on.</param>
    /// <param name="parameters">An object containing parameter values. Properties/fields should match parameter names.</param>
    /// <param name="withKeyPrefix">Optional key prefix to apply to all keys.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The result of the script execution.</returns>
    /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
    /// <exception cref="ArgumentException">Thrown when parameters object is missing required properties or has invalid types.</exception>
    /// <remarks>
    /// This method extracts parameter values from the provided object and passes them to the script.
    /// Parameters of type ValkeyKey are treated as keys (KEYS array), while other types are treated
    /// as arguments (ARGV array).
    ///
    /// Example:
    /// <code>
    /// var script = LuaScript.Prepare("return redis.call('SET', @key, @value)");
    /// var result = script.Evaluate(db, new { key = new ValkeyKey("mykey"), value = "myvalue" });
    /// </code>
    /// </remarks>
    public ValkeyResult Evaluate(IDatabase db, object? parameters = null,
        ValkeyKey? withKeyPrefix = null, CommandFlags flags = CommandFlags.None)
    {
        if (db == null)
        {
            throw new ArgumentNullException(nameof(db));
        }

        (ValkeyKey[] keys, ValkeyValue[] args) = ExtractParametersInternal(parameters, withKeyPrefix);

        // Call IDatabase.ScriptEvaluate (will be implemented in task 15.1)
        // For now, we'll use Execute to call EVAL directly
        List<object> evalArgs = [ExecutableScript];
        evalArgs.Add(keys.Length);
        evalArgs.AddRange(keys.Cast<object>());
        evalArgs.AddRange(args.Cast<object>());

        return db.Execute("EVAL", evalArgs, flags);
    }

    /// <summary>
    /// Asynchronously evaluates the script on the specified database.
    /// </summary>
    /// <param name="db">The database to execute the script on.</param>
    /// <param name="parameters">An object containing parameter values. Properties/fields should match parameter names.</param>
    /// <param name="withKeyPrefix">Optional key prefix to apply to all keys.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the script execution.</returns>
    /// <exception cref="ArgumentNullException">Thrown when db is null.</exception>
    /// <exception cref="ArgumentException">Thrown when parameters object is missing required properties or has invalid types.</exception>
    /// <remarks>
    /// This method extracts parameter values from the provided object and passes them to the script.
    /// Parameters of type ValkeyKey are treated as keys (KEYS array), while other types are treated
    /// as arguments (ARGV array).
    ///
    /// Example:
    /// <code>
    /// var script = LuaScript.Prepare("return redis.call('SET', @key, @value)");
    /// var result = await script.EvaluateAsync(db, new { key = new ValkeyKey("mykey"), value = "myvalue" });
    /// </code>
    /// </remarks>
    public async Task<ValkeyResult> EvaluateAsync(IDatabaseAsync db, object? parameters = null,
        ValkeyKey? withKeyPrefix = null, CommandFlags flags = CommandFlags.None)
    {
        if (db == null)
        {
            throw new ArgumentNullException(nameof(db));
        }

        (ValkeyKey[] keys, ValkeyValue[] args) = ExtractParametersInternal(parameters, withKeyPrefix);

        // Call IDatabaseAsync.ScriptEvaluateAsync (will be implemented in task 15.1)
        // For now, we'll use ExecuteAsync to call EVAL directly
        List<object> evalArgs = [ExecutableScript];
        evalArgs.Add(keys.Length);
        evalArgs.AddRange(keys.Cast<object>());
        evalArgs.AddRange(args.Cast<object>());

        return await db.ExecuteAsync("EVAL", evalArgs, flags).ConfigureAwait(false);
    }

    /// <summary>
    /// Extracts parameters from an object and converts them to keys and arguments.
    /// </summary>
    /// <param name="parameters">The parameter object.</param>
    /// <param name="keyPrefix">Optional key prefix to apply.</param>
    /// <returns>A tuple containing the keys and arguments arrays.</returns>
    internal (ValkeyKey[] Keys, ValkeyValue[] Args) ExtractParametersInternal(object? parameters, ValkeyKey? keyPrefix)
    {
        if (parameters == null || Arguments.Length == 0)
        {
            return ([], []);
        }

        Type paramType = parameters.GetType();

        // Validate parameters
        if (!ScriptParameterMapper.IsValidParameterHash(paramType, Arguments,
            out string? missingMember, out string? badTypeMember))
        {
            if (missingMember != null)
            {
                throw new ArgumentException(
                    $"Parameter object is missing required property or field: {missingMember}",
                    nameof(parameters));
            }
            if (badTypeMember != null)
            {
                throw new ArgumentException(
                    $"Parameter '{badTypeMember}' has an invalid type. Only ValkeyKey, ValkeyValue, string, byte[], numeric types, and bool are supported.",
                    nameof(parameters));
            }
        }

        // Extract parameters
        Func<object, ValkeyKey?, (ValkeyKey[], ValkeyValue[])> extractor =
            ScriptParameterMapper.GetParameterExtractor(paramType, Arguments);

        return extractor(parameters, keyPrefix);
    }

    /// <summary>
    /// Loads the script on the server and returns a LoadedLuaScript synchronously.
    /// </summary>
    /// <param name="server">The server to load the script on.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A LoadedLuaScript instance that can be used to execute the script via EVALSHA.</returns>
    /// <exception cref="ArgumentNullException">Thrown when server is null.</exception>
    ///  /// <remarks>
    /// This meth script onto the server using the SCRIPT LOAD command.
    /// The returned LoadedLuaScript contains the SHA1 hash and can be used to execute
    /// the script more efficiently using EVALSHA.
    ///
    /// Example:
    /// <code>
    /// var script = LuaScript.Prepare("return redis.call('GET', @key)");
    /// var loaded = script.Load(server);
    /// var result = loaded.Evaluate(db, new { key = "mykey" });
    /// </code>
    /// </remarks>
    public LoadedLuaScript Load(IServer server, CommandFlags flags = CommandFlags.None)
    {
        if (server == null)
        {
            throw new ArgumentNullException(nameof(server));
        }

        // Replace placeholders in the executable script using a heuristic
        // We assume parameters named "key", "keys", or starting with "key" are keys
        string scriptToLoad = ScriptParameterMapper.ReplacePlaceholdersWithHeuristic(ExecutableScript, Arguments);

        // Call IServer.ScriptLoad (will be implemented in task 15.2)
        // For now, we'll use Execute to call SCRIPT LOAD directly
        ValkeyResult result = server.Execute("SCRIPT", ["LOAD", scriptToLoad], flags);
        string? hashString = (string?)result;

        if (string.IsNullOrEmpty(hashString))
        {
            throw new InvalidOperationException("SCRIPT LOAD returned null or empty hash");
        }

        // Convert hex string to byte array
        byte[] hash = Convert.FromHexString(hashString);
        return new LoadedLuaScript(this, hash, scriptToLoad);
    }

    /// <summary>
    /// Asynchronously loads the script on the server and returns a LoadedLuaScript.
    /// </summary>
    /// <param name="server">The server to load the script on.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A task representing the asynchronous operation, containing a LoadedLuaScript instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when server is null.</exception>
    /// <remarks>
    /// This method loads the script onto the server using the SCRIPT LOAD command.
    /// The returned LoadedLuaScript contains the SHA1 hash and can be used to execute
    /// the script more efficiently using EVALSHA.
    ///
    /// Example:
    /// <code>
    /// var script = LuaScript.Prepare("return redis.call('GET', @key)");
    /// var loaded = await script.LoadAsync(server);
    /// var result = await loaded.EvaluateAsync(db, new { key = "mykey" });
    /// </code>
    /// </remarks>
    public async Task<LoadedLuaScript> LoadAsync(IServer server, CommandFlags flags = CommandFlags.None)
    {
        if (server == null)
        {
            throw new ArgumentNullException(nameof(server));
        }

        // Replace placeholders in the executable script using a heuristic
        // We assume parameters named "key", "keys", or starting with "key" are keys
        string scriptToLoad = ScriptParameterMapper.ReplacePlaceholdersWithHeuristic(ExecutableScript, Arguments);

        // Call IServer.ScriptLoadAsync (will be implemented in task 15.2)
        // For now, we'll use ExecuteAsync to call SCRIPT LOAD directly
        ValkeyResult result = await server.ExecuteAsync("SCRIPT", ["LOAD", scriptToLoad], flags).ConfigureAwait(false);
        string? hashString = (string?)result;

        if (string.IsNullOrEmpty(hashString))
        {
            throw new InvalidOperationException("SCRIPT LOAD returned null or empty hash");
        }

        // Convert hex string to byte array
        byte[] hash = Convert.FromHexString(hashString);
        return new LoadedLuaScript(this, hash, scriptToLoad);
    }
}
///
