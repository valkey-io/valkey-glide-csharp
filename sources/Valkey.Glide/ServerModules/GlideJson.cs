// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.ServerModules;

/// <summary>
/// Module for JSON commands.
/// Provides static methods to execute JSON commands on Valkey servers with the JSON module enabled.
/// </summary>
/// <seealso href="https://valkey.io/commands/?group=json">Valkey JSON Commands</seealso>
public static partial class GlideJson
{
    private const string JsonPrefix = "JSON.";

    // Command constants
    private const string JsonSet = JsonPrefix + "SET";
    private const string JsonGet = JsonPrefix + "GET";
    private const string JsonMGet = JsonPrefix + "MGET";
    private const string JsonDel = JsonPrefix + "DEL";
    private const string JsonForget = JsonPrefix + "FORGET";
    private const string JsonClear = JsonPrefix + "CLEAR";
    private const string JsonType = JsonPrefix + "TYPE";
    private const string JsonNumIncrBy = JsonPrefix + "NUMINCRBY";
    private const string JsonNumMultBy = JsonPrefix + "NUMMULTBY";
    private const string JsonStrAppend = JsonPrefix + "STRAPPEND";
    private const string JsonStrLen = JsonPrefix + "STRLEN";
    private const string JsonArrAppend = JsonPrefix + "ARRAPPEND";
    private const string JsonArrInsert = JsonPrefix + "ARRINSERT";
    private const string JsonArrIndex = JsonPrefix + "ARRINDEX";
    private const string JsonArrLen = JsonPrefix + "ARRLEN";
    private const string JsonArrPop = JsonPrefix + "ARRPOP";
    private const string JsonArrTrim = JsonPrefix + "ARRTRIM";
    private const string JsonObjLen = JsonPrefix + "OBJLEN";
    private const string JsonObjKeys = JsonPrefix + "OBJKEYS";
    private const string JsonToggle = JsonPrefix + "TOGGLE";
    private const string JsonDebug = JsonPrefix + "DEBUG";
    private const string JsonResp = JsonPrefix + "RESP";

    #region Helper Methods

    /// <summary>
    /// Executes a custom command on the client, handling both standalone and cluster clients.
    /// </summary>
    /// <param name="client">The client to execute the command on.</param>
    /// <param name="args">The command arguments.</param>
    /// <returns>The command result.</returns>
    private static async Task<object?> ExecuteCommandAsync(BaseClient client, GlideString[] args)
    {
        if (client is GlideClient gc)
        {
            return await gc.CustomCommand(args);
        }
        else if (client is GlideClusterClient cc)
        {
            ClusterValue<object?> result = await cc.CustomCommand(args);
            return result.SingleValue;
        }
        throw new ArgumentException("Unsupported client type. Expected GlideClient or GlideClusterClient.", nameof(client));
    }

    /// <summary>
    /// Converts an object result to ValkeyValue.
    /// </summary>
    private static ValkeyValue ToValkeyValue(object? result)
    {
        if (result is null)
            return ValkeyValue.Null;
        if (result is GlideString gs)
            return gs;
        return result.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Converts a ValkeyKey to GlideString for command arguments.
    /// </summary>
    private static GlideString ToGlideString(ValkeyKey key) => (GlideString)key;

    /// <summary>
    /// Converts a ValkeyValue to GlideString for command arguments.
    /// </summary>
    private static GlideString ToGlideString(ValkeyValue value)
    {
        if (value.IsNull)
            return new GlideString(string.Empty);
        return new GlideString(value.ToString());
    }

    #endregion

    #region JSON.SET

    /// <summary>
    /// Sets the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <returns>"OK" if the value was set successfully, or <see cref="ValkeyValue.Null"/> if the condition was not met.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    /// <example>
    /// <code>
    /// ValkeyValue result = await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\"}");
    /// // result == "OK"
    /// </code>
    /// </example>
    public static Task<ValkeyValue> SetAsync(BaseClient client, ValkeyKey key, ValkeyValue path, ValkeyValue value)
        => SetAsync(client, key, path, value, SetCondition.None);

    /// <summary>
    /// Sets the JSON value at the specified path in the key with a condition.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <param name="condition">The condition for setting the value (NX or XX).</param>
    /// <returns>"OK" if the value was set successfully, or <see cref="ValkeyValue.Null"/> if the condition was not met.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static async Task<ValkeyValue> SetAsync(BaseClient client, ValkeyKey key, ValkeyValue path, ValkeyValue value, SetCondition condition)
    {
        GlideString[] args = BuildSetArgs(ToGlideString(key), ToGlideString(path), ToGlideString(value), condition);
        object? result = await ExecuteCommandAsync(client, args);
        return ToValkeyValue(result);
    }

    private static GlideString[] BuildSetArgs(GlideString key, GlideString path, GlideString value, SetCondition condition)
    {
        return condition switch
        {
            SetCondition.OnlyIfDoesNotExist => [JsonSet, key, path, value, "NX"],
            SetCondition.OnlyIfExists => [JsonSet, key, path, value, "XX"],
            _ => [JsonSet, key, path, value]
        };
    }

    #endregion

    #region JSON.GET

    /// <summary>
    /// Gets the entire JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The JSON document, or <see cref="ValkeyValue.Null"/> if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<ValkeyValue> GetAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonGet, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ToValkeyValue(result);
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <returns>The JSON value(s), or <see cref="ValkeyValue.Null"/> if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<ValkeyValue> GetAsync(BaseClient client, ValkeyKey key, ValkeyValue[] paths)
    {
        GlideString[] glidePaths = paths.Select(p => ToGlideString(p)).ToArray();
        GlideString[] args = BuildGetArgs(ToGlideString(key), glidePaths, null);
        object? result = await ExecuteCommandAsync(client, args);
        return ToValkeyValue(result);
    }

    /// <summary>
    /// Gets the entire JSON document stored at the key with formatting options.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="options">Formatting options for the JSON output.</param>
    /// <returns>The formatted JSON document, or <see cref="ValkeyValue.Null"/> if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<ValkeyValue> GetAsync(BaseClient client, ValkeyKey key, GetOptions options)
    {
        GlideString[] args = BuildGetArgs(ToGlideString(key), null, options);
        object? result = await ExecuteCommandAsync(client, args);
        return ToValkeyValue(result);
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key with formatting options.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <param name="options">Formatting options for the JSON output.</param>
    /// <returns>The formatted JSON value(s), or <see cref="ValkeyValue.Null"/> if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<ValkeyValue> GetAsync(BaseClient client, ValkeyKey key, ValkeyValue[] paths, GetOptions options)
    {
        GlideString[] glidePaths = paths.Select(p => ToGlideString(p)).ToArray();
        GlideString[] args = BuildGetArgs(ToGlideString(key), glidePaths, options);
        object? result = await ExecuteCommandAsync(client, args);
        return ToValkeyValue(result);
    }

    private static GlideString[] BuildGetArgs(GlideString key, GlideString[]? paths, GetOptions? options)
    {
        List<GlideString> args = [JsonGet, key];

        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }

        if (paths is not null && paths.Length > 0)
        {
            args.AddRange(paths);
        }

        return [.. args];
    }

    #endregion

    #region JSON.MGET

    /// <summary>
    /// Gets the JSON values at the specified path from multiple keys.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="keys">The keys where the JSON documents are stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON documents.</param>
    /// <returns>An array of JSON values, one for each key. Returns null for keys that don't exist or don't have the path.</returns>
    /// <seealso href="https://valkey.io/commands/json.mget/"/>
    public static async Task<ValkeyValue[]> MGetAsync(BaseClient client, ValkeyKey[] keys, ValkeyValue path)
    {
        GlideString[] args = BuildMGetArgs(keys, ToGlideString(path));
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToValkeyValueArray(result);
    }

    private static GlideString[] BuildMGetArgs(ValkeyKey[] keys, GlideString path)
    {
        List<GlideString> args = [JsonMGet];
        foreach (ValkeyKey key in keys)
        {
            args.Add(ToGlideString(key));
        }
        args.Add(path);
        return [.. args];
    }

    private static ValkeyValue[] ConvertToValkeyValueArray(object? result)
    {
        if (result is null)
            return [];
        if (result is object?[] arr)
            return arr.Select(ToValkeyValue).ToArray();
        return [ToValkeyValue(result)];
    }

    #endregion

    #region JSON.DEL

    /// <summary>
    /// Deletes the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of paths deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static async Task<long> DelAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonDel, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return (long)(result ?? 0L);
    }

    /// <summary>
    /// Deletes the entire JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of paths deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static async Task<long> DelAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonDel, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return (long)(result ?? 0L);
    }

    #endregion

    #region JSON.FORGET

    /// <summary>
    /// Alias for <see cref="DelAsync(BaseClient, ValkeyKey, ValkeyValue)"/>.
    /// Deletes the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of paths deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static async Task<long> ForgetAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonForget, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return (long)(result ?? 0L);
    }

    /// <summary>
    /// Alias for <see cref="DelAsync(BaseClient, ValkeyKey)"/>.
    /// Deletes the entire JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of paths deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static async Task<long> ForgetAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonForget, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return (long)(result ?? 0L);
    }

    #endregion

    #region JSON.CLEAR

    /// <summary>
    /// Clears the JSON value at the specified path in the key (sets arrays to empty, objects to empty, numbers to 0).
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of values cleared.</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static async Task<long> ClearAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonClear, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return (long)(result ?? 0L);
    }

    /// <summary>
    /// Clears the JSON value at the root path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of values cleared.</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static async Task<long> ClearAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonClear, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return (long)(result ?? 0L);
    }

    #endregion

    #region JSON.TYPE

    /// <summary>
    /// Gets the type of the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of type strings (or null for non-existent paths).
    /// When a legacy path is provided, returns the type string.
    /// Types: "null", "boolean", "string", "number", "integer", "object", "array".
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static async Task<ValkeyResult> TypeAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonType, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Gets the type of the JSON value at the root path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The type string of the root value.</returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static async Task<ValkeyResult> TypeAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonType, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    #endregion

    #region JSON.NUMINCRBY

    /// <summary>
    /// Increments the numeric value at the specified path in the key by the given amount.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="increment">The amount to increment by.</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON string array of new values (or "null" for non-numeric matches).
    /// When a legacy path is provided, returns the new value as a string.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.numincrby/"/>
    public static async Task<ValkeyValue> NumIncrByAsync(BaseClient client, ValkeyKey key, ValkeyValue path, double increment)
    {
        GlideString[] args = [JsonNumIncrBy, ToGlideString(key), ToGlideString(path), increment.ToString()];
        object? result = await ExecuteCommandAsync(client, args);
        return ToValkeyValue(result);
    }

    #endregion

    #region JSON.NUMMULTBY

    /// <summary>
    /// Multiplies the numeric value at the specified path in the key by the given amount.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="multiplier">The amount to multiply by.</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON string array of new values (or "null" for non-numeric matches).
    /// When a legacy path is provided, returns the new value as a string.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.nummultby/"/>
    public static async Task<ValkeyValue> NumMultByAsync(BaseClient client, ValkeyKey key, ValkeyValue path, double multiplier)
    {
        GlideString[] args = [JsonNumMultBy, ToGlideString(key), ToGlideString(path), multiplier.ToString()];
        object? result = await ExecuteCommandAsync(client, args);
        return ToValkeyValue(result);
    }

    #endregion

    #region JSON.STRAPPEND

    /// <summary>
    /// Appends a string to the string value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON string value to append (must be a valid JSON string, e.g., "\"suffix\"").</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of new string lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the new string length.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static async Task<ValkeyResult> StrAppendAsync(BaseClient client, ValkeyKey key, ValkeyValue path, ValkeyValue value)
    {
        GlideString[] args = [JsonStrAppend, ToGlideString(key), ToGlideString(path), ToGlideString(value)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Appends a string to the string value at the root path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="value">The JSON string value to append (must be a valid JSON string, e.g., "\"suffix\"").</param>
    /// <returns>The new string length.</returns>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static async Task<ValkeyResult> StrAppendAsync(BaseClient client, ValkeyKey key, ValkeyValue value)
    {
        GlideString[] args = [JsonStrAppend, ToGlideString(key), ToGlideString(value)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    #endregion

    #region JSON.STRLEN

    /// <summary>
    /// Gets the length of the string value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of string lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the string length.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static async Task<ValkeyResult> StrLenAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonStrLen, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Gets the length of the string value at the root path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The string length.</returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static async Task<ValkeyResult> StrLenAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonStrLen, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    #endregion
}
