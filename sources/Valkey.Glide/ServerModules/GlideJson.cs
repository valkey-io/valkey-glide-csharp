// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

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

    #region JSON.SET

    /// <summary>
    /// Sets the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <returns>"OK" if the value was set successfully.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    /// <example>
    /// <code>
    /// string? result = await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\"}");
    /// // result == "OK"
    /// </code>
    /// </example>
    public static async Task<string?> SetAsync(GlideClient client, string key, string path, string value)
        => await SetAsync(client, key, path, value, JsonSetCondition.None);

    /// <summary>
    /// Sets the JSON value at the specified path in the key with a condition.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <param name="condition">The condition for setting the value (NX or XX).</param>
    /// <returns>"OK" if the value was set successfully, or null if the condition was not met.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    /// <example>
    /// <code>
    /// // Set only if the key does not exist
    /// string? result = await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\"}", JsonSetCondition.OnlyIfDoesNotExist);
    /// </code>
    /// </example>
    public static async Task<string?> SetAsync(GlideClient client, string key, string path, string value, JsonSetCondition condition)
    {
        GlideString[] args = BuildSetArgs(key, path, value, condition);
        object? result = await client.CustomCommand(args);
        return result?.ToString();
    }

    /// <summary>
    /// Sets the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <returns>"OK" if the value was set successfully.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static async Task<string?> SetAsync(GlideClient client, GlideString key, GlideString path, GlideString value)
        => await SetAsync(client, key, path, value, JsonSetCondition.None);

    /// <summary>
    /// Sets the JSON value at the specified path in the key with a condition.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <param name="condition">The condition for setting the value (NX or XX).</param>
    /// <returns>"OK" if the value was set successfully, or null if the condition was not met.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static async Task<string?> SetAsync(GlideClient client, GlideString key, GlideString path, GlideString value, JsonSetCondition condition)
    {
        GlideString[] args = BuildSetArgs(key, path, value, condition);
        object? result = await client.CustomCommand(args);
        return result?.ToString();
    }

    /// <summary>
    /// Sets the JSON value at the specified path in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <returns>"OK" if the value was set successfully.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static async Task<string?> SetAsync(GlideClusterClient client, string key, string path, string value)
        => await SetAsync(client, key, path, value, JsonSetCondition.None);

    /// <summary>
    /// Sets the JSON value at the specified path in the key with a condition (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <param name="condition">The condition for setting the value (NX or XX).</param>
    /// <returns>"OK" if the value was set successfully, or null if the condition was not met.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static async Task<string?> SetAsync(GlideClusterClient client, string key, string path, string value, JsonSetCondition condition)
    {
        GlideString[] args = BuildSetArgs(key, path, value, condition);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue?.ToString();
    }

    /// <summary>
    /// Sets the JSON value at the specified path in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <returns>"OK" if the value was set successfully.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static async Task<string?> SetAsync(GlideClusterClient client, GlideString key, GlideString path, GlideString value)
        => await SetAsync(client, key, path, value, JsonSetCondition.None);

    /// <summary>
    /// Sets the JSON value at the specified path in the key with a condition (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <param name="condition">The condition for setting the value (NX or XX).</param>
    /// <returns>"OK" if the value was set successfully, or null if the condition was not met.</returns>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static async Task<string?> SetAsync(GlideClusterClient client, GlideString key, GlideString path, GlideString value, JsonSetCondition condition)
    {
        GlideString[] args = BuildSetArgs(key, path, value, condition);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue?.ToString();
    }

    /// <summary>
    /// Builds the command arguments for JSON.SET.
    /// </summary>
    private static GlideString[] BuildSetArgs(GlideString key, GlideString path, GlideString value, JsonSetCondition condition)
    {
        return condition switch
        {
            JsonSetCondition.OnlyIfDoesNotExist => [JsonSet, key, path, value, "NX"],
            JsonSetCondition.OnlyIfExists => [JsonSet, key, path, value, "XX"],
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
    /// <returns>The JSON document as a string, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    /// <example>
    /// <code>
    /// string? result = await GlideJson.GetAsync(client, "mykey");
    /// // result == "{\"name\":\"John\",\"age\":30}"
    /// </code>
    /// </example>
    public static async Task<string?> GetAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonGet, key];
        object? result = await client.CustomCommand(args);
        return result?.ToString();
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <returns>
    /// The JSON value(s) as a string. When a single JSONPath is provided, returns a JSON array of matching values.
    /// When a single legacy path is provided, returns the single matching value.
    /// When multiple paths are provided, returns a JSON object with each path as a key.
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    /// <example>
    /// <code>
    /// // Single JSONPath - returns array
    /// string? result = await GlideJson.GetAsync(client, "mykey", ["$.name"]);
    /// // result == "[\"John\"]"
    ///
    /// // Single legacy path - returns single value
    /// string? result = await GlideJson.GetAsync(client, "mykey", [".name"]);
    /// // result == "\"John\""
    ///
    /// // Multiple paths - returns object
    /// string? result = await GlideJson.GetAsync(client, "mykey", ["$.name", "$.age"]);
    /// // result == "{\"$.name\":[\"John\"],\"$.age\":[30]}"
    /// </code>
    /// </example>
    public static async Task<string?> GetAsync(GlideClient client, string key, string[] paths)
    {
        GlideString[] glidePaths = paths.Select(p => (GlideString)p).ToArray();
        GlideString[] args = BuildGetArgs(key, glidePaths, null);
        object? result = await client.CustomCommand(args);
        return result?.ToString();
    }

    /// <summary>
    /// Gets the entire JSON document stored at the key with formatting options.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="options">Formatting options for the JSON output (indent, newline, space).</param>
    /// <returns>The formatted JSON document as a string, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    /// <example>
    /// <code>
    /// var options = new JsonGetOptions { Indent = "  ", Newline = "\n", Space = " " };
    /// string? result = await GlideJson.GetAsync(client, "mykey", options);
    /// // result is pretty-printed JSON
    /// </code>
    /// </example>
    public static async Task<string?> GetAsync(GlideClient client, string key, JsonGetOptions options)
    {
        GlideString[] args = BuildGetArgs(key, null, options);
        object? result = await client.CustomCommand(args);
        return result?.ToString();
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key with formatting options.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <param name="options">Formatting options for the JSON output (indent, newline, space).</param>
    /// <returns>
    /// The formatted JSON value(s) as a string. When a single JSONPath is provided, returns a JSON array of matching values.
    /// When a single legacy path is provided, returns the single matching value.
    /// When multiple paths are provided, returns a JSON object with each path as a key.
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<string?> GetAsync(GlideClient client, string key, string[] paths, JsonGetOptions options)
    {
        GlideString[] glidePaths = paths.Select(p => (GlideString)p).ToArray();
        GlideString[] args = BuildGetArgs(key, glidePaths, options);
        object? result = await client.CustomCommand(args);
        return result?.ToString();
    }

    /// <summary>
    /// Gets the entire JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The JSON document as a GlideString, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<GlideString?> GetAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonGet, key];
        object? result = await client.CustomCommand(args);
        return result is null ? null : (GlideString?)result.ToString();
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <returns>
    /// The JSON value(s) as a GlideString. When a single JSONPath is provided, returns a JSON array of matching values.
    /// When a single legacy path is provided, returns the single matching value.
    /// When multiple paths are provided, returns a JSON object with each path as a key.
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<GlideString?> GetAsync(GlideClient client, GlideString key, GlideString[] paths)
    {
        GlideString[] args = BuildGetArgs(key, paths, null);
        object? result = await client.CustomCommand(args);
        return result is null ? null : (GlideString?)result.ToString();
    }

    /// <summary>
    /// Gets the entire JSON document stored at the key with formatting options.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="options">Formatting options for the JSON output (indent, newline, space).</param>
    /// <returns>The formatted JSON document as a GlideString, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<GlideString?> GetAsync(GlideClient client, GlideString key, JsonGetOptions options)
    {
        GlideString[] args = BuildGetArgs(key, null, options);
        object? result = await client.CustomCommand(args);
        return result is null ? null : (GlideString?)result.ToString();
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key with formatting options.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <param name="options">Formatting options for the JSON output (indent, newline, space).</param>
    /// <returns>
    /// The formatted JSON value(s) as a GlideString. When a single JSONPath is provided, returns a JSON array of matching values.
    /// When a single legacy path is provided, returns the single matching value.
    /// When multiple paths are provided, returns a JSON object with each path as a key.
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<GlideString?> GetAsync(GlideClient client, GlideString key, GlideString[] paths, JsonGetOptions options)
    {
        GlideString[] args = BuildGetArgs(key, paths, options);
        object? result = await client.CustomCommand(args);
        return result is null ? null : (GlideString?)result.ToString();
    }

    /// <summary>
    /// Gets the entire JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The JSON document as a string, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<string?> GetAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonGet, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue?.ToString();
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <returns>
    /// The JSON value(s) as a string. When a single JSONPath is provided, returns a JSON array of matching values.
    /// When a single legacy path is provided, returns the single matching value.
    /// When multiple paths are provided, returns a JSON object with each path as a key.
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<string?> GetAsync(GlideClusterClient client, string key, string[] paths)
    {
        GlideString[] glidePaths = paths.Select(p => (GlideString)p).ToArray();
        GlideString[] args = BuildGetArgs(key, glidePaths, null);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue?.ToString();
    }

    /// <summary>
    /// Gets the entire JSON document stored at the key with formatting options (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="options">Formatting options for the JSON output (indent, newline, space).</param>
    /// <returns>The formatted JSON document as a string, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<string?> GetAsync(GlideClusterClient client, string key, JsonGetOptions options)
    {
        GlideString[] args = BuildGetArgs(key, null, options);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue?.ToString();
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key with formatting options (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <param name="options">Formatting options for the JSON output (indent, newline, space).</param>
    /// <returns>
    /// The formatted JSON value(s) as a string. When a single JSONPath is provided, returns a JSON array of matching values.
    /// When a single legacy path is provided, returns the single matching value.
    /// When multiple paths are provided, returns a JSON object with each path as a key.
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<string?> GetAsync(GlideClusterClient client, string key, string[] paths, JsonGetOptions options)
    {
        GlideString[] glidePaths = paths.Select(p => (GlideString)p).ToArray();
        GlideString[] args = BuildGetArgs(key, glidePaths, options);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue?.ToString();
    }

    /// <summary>
    /// Gets the entire JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The JSON document as a GlideString, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<GlideString?> GetAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonGet, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue is null ? null : (GlideString?)result.SingleValue.ToString();
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <returns>
    /// The JSON value(s) as a GlideString. When a single JSONPath is provided, returns a JSON array of matching values.
    /// When a single legacy path is provided, returns the single matching value.
    /// When multiple paths are provided, returns a JSON object with each path as a key.
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<GlideString?> GetAsync(GlideClusterClient client, GlideString key, GlideString[] paths)
    {
        GlideString[] args = BuildGetArgs(key, paths, null);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue is null ? null : (GlideString?)result.SingleValue.ToString();
    }

    /// <summary>
    /// Gets the entire JSON document stored at the key with formatting options (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="options">Formatting options for the JSON output (indent, newline, space).</param>
    /// <returns>The formatted JSON document as a GlideString, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<GlideString?> GetAsync(GlideClusterClient client, GlideString key, JsonGetOptions options)
    {
        GlideString[] args = BuildGetArgs(key, null, options);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue is null ? null : (GlideString?)result.SingleValue.ToString();
    }

    /// <summary>
    /// Gets the JSON value(s) at the specified path(s) in the key with formatting options (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <param name="options">Formatting options for the JSON output (indent, newline, space).</param>
    /// <returns>
    /// The formatted JSON value(s) as a GlideString. When a single JSONPath is provided, returns a JSON array of matching values.
    /// When a single legacy path is provided, returns the single matching value.
    /// When multiple paths are provided, returns a JSON object with each path as a key.
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static async Task<GlideString?> GetAsync(GlideClusterClient client, GlideString key, GlideString[] paths, JsonGetOptions options)
    {
        GlideString[] args = BuildGetArgs(key, paths, options);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue is null ? null : (GlideString?)result.SingleValue.ToString();
    }

    /// <summary>
    /// Builds the command arguments for JSON.GET.
    /// </summary>
    private static GlideString[] BuildGetArgs(GlideString key, GlideString[]? paths, JsonGetOptions? options)
    {
        List<GlideString> args = [JsonGet, key];

        // Add formatting options if provided
        if (options is not null)
        {
            args.AddRange(options.ToArgs());
        }

        // Add paths if provided
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
    /// <returns>
    /// An array with the value at the path for each key. When a key does not exist,
    /// the corresponding array element will be null. When a JSONPath is provided,
    /// returns stringified JSON arrays for each key.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.mget/"/>
    /// <example>
    /// <code>
    /// // Set up some JSON documents
    /// await GlideJson.SetAsync(client, "doc1", "$", "{\"name\":\"John\"}");
    /// await GlideJson.SetAsync(client, "doc2", "$", "{\"name\":\"Jane\"}");
    ///
    /// // Get values from multiple keys
    /// string?[] results = await GlideJson.MGetAsync(client, ["doc1", "doc2", "nonexistent"], "$.name");
    /// // results[0] == "[\"John\"]"
    /// // results[1] == "[\"Jane\"]"
    /// // results[2] == null
    /// </code>
    /// </example>
    public static async Task<string?[]> MGetAsync(GlideClient client, string[] keys, string path)
    {
        GlideString[] args = BuildMGetArgs(keys.Select(k => (GlideString)k).ToArray(), path);
        object? result = await client.CustomCommand(args);
        return ParseMGetResult(result);
    }

    /// <summary>
    /// Gets the JSON values at the specified path from multiple keys.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="keys">The keys where the JSON documents are stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON documents.</param>
    /// <returns>
    /// An array with the value at the path for each key. When a key does not exist,
    /// the corresponding array element will be null. When a JSONPath is provided,
    /// returns stringified JSON arrays for each key.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.mget/"/>
    public static async Task<GlideString?[]> MGetAsync(GlideClient client, GlideString[] keys, GlideString path)
    {
        GlideString[] args = BuildMGetArgs(keys, path);
        object? result = await client.CustomCommand(args);
        return ParseMGetResultAsGlideString(result);
    }

    /// <summary>
    /// Gets the JSON values at the specified path from multiple keys (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="keys">The keys where the JSON documents are stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON documents.</param>
    /// <returns>
    /// An array with the value at the path for each key. When a key does not exist,
    /// the corresponding array element will be null. When a JSONPath is provided,
    /// returns stringified JSON arrays for each key.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.mget/"/>
    public static async Task<string?[]> MGetAsync(GlideClusterClient client, string[] keys, string path)
    {
        GlideString[] args = BuildMGetArgs(keys.Select(k => (GlideString)k).ToArray(), path);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseMGetResult(result.SingleValue);
    }

    /// <summary>
    /// Gets the JSON values at the specified path from multiple keys (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="keys">The keys where the JSON documents are stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON documents.</param>
    /// <returns>
    /// An array with the value at the path for each key. When a key does not exist,
    /// the corresponding array element will be null. When a JSONPath is provided,
    /// returns stringified JSON arrays for each key.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.mget/"/>
    public static async Task<GlideString?[]> MGetAsync(GlideClusterClient client, GlideString[] keys, GlideString path)
    {
        GlideString[] args = BuildMGetArgs(keys, path);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseMGetResultAsGlideString(result.SingleValue);
    }

    /// <summary>
    /// Builds the command arguments for JSON.MGET.
    /// </summary>
    private static GlideString[] BuildMGetArgs(GlideString[] keys, GlideString path)
    {
        // Command format: JSON.MGET key [key ...] path
        GlideString[] args = new GlideString[keys.Length + 2];
        args[0] = JsonMGet;
        for (int i = 0; i < keys.Length; i++)
        {
            args[i + 1] = keys[i];
        }
        args[keys.Length + 1] = path;
        return args;
    }

    /// <summary>
    /// Parses the result of JSON.MGET command into a string array.
    /// </summary>
    private static string?[] ParseMGetResult(object? result)
    {
        if (result is null)
        {
            return [];
        }

        if (result is object?[] array)
        {
            return array.Select(item => item?.ToString()).ToArray();
        }

        // Single result case
        return [result.ToString()];
    }

    /// <summary>
    /// Parses the result of JSON.MGET command into a GlideString array.
    /// </summary>
    private static GlideString?[] ParseMGetResultAsGlideString(object? result)
    {
        if (result is null)
        {
            return [];
        }

        if (result is object?[] array)
        {
            return array.Select(item => item is null ? null : (GlideString?)item.ToString()).ToArray();
        }

        // Single result case
        return [result.ToString()];
    }

    #endregion

    #region JSON.DEL

    /// <summary>
    /// Deletes the entire JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of elements deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\"}");
    /// long deleted = await GlideJson.DelAsync(client, "mykey");
    /// // deleted == 1
    /// </code>
    /// </example>
    public static async Task<long> DelAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonDel, key];
        object? result = await client.CustomCommand(args);
        return ParseDelResult(result);
    }

    /// <summary>
    /// Deletes the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of elements deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\",\"age\":30}");
    /// long deleted = await GlideJson.DelAsync(client, "mykey", "$.age");
    /// // deleted == 1
    /// </code>
    /// </example>
    public static async Task<long> DelAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonDel, key, path];
        object? result = await client.CustomCommand(args);
        return ParseDelResult(result);
    }

    /// <summary>
    /// Deletes the entire JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of elements deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static async Task<long> DelAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonDel, key];
        object? result = await client.CustomCommand(args);
        return ParseDelResult(result);
    }

    /// <summary>
    /// Deletes the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of elements deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static async Task<long> DelAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonDel, key, path];
        object? result = await client.CustomCommand(args);
        return ParseDelResult(result);
    }

    /// <summary>
    /// Deletes the entire JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of elements deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static async Task<long> DelAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonDel, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseDelResult(result.SingleValue);
    }

    /// <summary>
    /// Deletes the JSON value at the specified path in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of elements deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static async Task<long> DelAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonDel, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseDelResult(result.SingleValue);
    }

    /// <summary>
    /// Deletes the entire JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of elements deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static async Task<long> DelAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonDel, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseDelResult(result.SingleValue);
    }

    /// <summary>
    /// Deletes the JSON value at the specified path in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of elements deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static async Task<long> DelAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonDel, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseDelResult(result.SingleValue);
    }

    /// <summary>
    /// Parses the result of JSON.DEL command into a long.
    /// </summary>
    private static long ParseDelResult(object? result)
    {
        if (result is null)
        {
            return 0;
        }

        if (result is long longResult)
        {
            return longResult;
        }

        if (long.TryParse(result.ToString(), out long parsed))
        {
            return parsed;
        }

        return 0;
    }

    #endregion

    #region JSON.FORGET

    /// <summary>
    /// Deletes the entire JSON document stored at the key.
    /// This is an alias for <see cref="DelAsync(GlideClient, string)"/>.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of elements deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\"}");
    /// long deleted = await GlideJson.ForgetAsync(client, "mykey");
    /// // deleted == 1
    /// </code>
    /// </example>
    public static Task<long> ForgetAsync(GlideClient client, string key)
        => DelAsync(client, key);

    /// <summary>
    /// Deletes the JSON value at the specified path in the key.
    /// This is an alias for <see cref="DelAsync(GlideClient, string, string)"/>.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of elements deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static Task<long> ForgetAsync(GlideClient client, string key, string path)
        => DelAsync(client, key, path);

    /// <summary>
    /// Deletes the entire JSON document stored at the key.
    /// This is an alias for <see cref="DelAsync(GlideClient, GlideString)"/>.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of elements deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static Task<long> ForgetAsync(GlideClient client, GlideString key)
        => DelAsync(client, key);

    /// <summary>
    /// Deletes the JSON value at the specified path in the key.
    /// This is an alias for <see cref="DelAsync(GlideClient, GlideString, GlideString)"/>.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of elements deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static Task<long> ForgetAsync(GlideClient client, GlideString key, GlideString path)
        => DelAsync(client, key, path);

    /// <summary>
    /// Deletes the entire JSON document stored at the key (cluster client).
    /// This is an alias for <see cref="DelAsync(GlideClusterClient, string)"/>.
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of elements deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static Task<long> ForgetAsync(GlideClusterClient client, string key)
        => DelAsync(client, key);

    /// <summary>
    /// Deletes the JSON value at the specified path in the key (cluster client).
    /// This is an alias for <see cref="DelAsync(GlideClusterClient, string, string)"/>.
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of elements deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static Task<long> ForgetAsync(GlideClusterClient client, string key, string path)
        => DelAsync(client, key, path);

    /// <summary>
    /// Deletes the entire JSON document stored at the key (cluster client).
    /// This is an alias for <see cref="DelAsync(GlideClusterClient, GlideString)"/>.
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of elements deleted (1 if the key existed, 0 otherwise).</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static Task<long> ForgetAsync(GlideClusterClient client, GlideString key)
        => DelAsync(client, key);

    /// <summary>
    /// Deletes the JSON value at the specified path in the key (cluster client).
    /// This is an alias for <see cref="DelAsync(GlideClusterClient, GlideString, GlideString)"/>.
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of elements deleted.</returns>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static Task<long> ForgetAsync(GlideClusterClient client, GlideString key, GlideString path)
        => DelAsync(client, key, path);

    #endregion

    #region JSON.CLEAR

    /// <summary>
    /// Clears container values (arrays/objects) and sets numeric values to 0, booleans to false, and strings to empty at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of values cleared (0 if the key does not exist).</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"items\":[1,2,3],\"count\":5}");
    /// long cleared = await GlideJson.ClearAsync(client, "mykey");
    /// // cleared == 1 (root object cleared)
    /// </code>
    /// </example>
    public static async Task<long> ClearAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonClear, key];
        object? result = await client.CustomCommand(args);
        return ParseClearResult(result);
    }

    /// <summary>
    /// Clears container values (arrays/objects) and sets numeric values to 0, booleans to false, and strings to empty at the specified path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of values cleared.</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"items\":[1,2,3],\"count\":5}");
    /// long cleared = await GlideJson.ClearAsync(client, "mykey", "$.items");
    /// // cleared == 1 (array cleared to [])
    /// </code>
    /// </example>
    public static async Task<long> ClearAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonClear, key, path];
        object? result = await client.CustomCommand(args);
        return ParseClearResult(result);
    }

    /// <summary>
    /// Clears container values (arrays/objects) and sets numeric values to 0, booleans to false, and strings to empty at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of values cleared (0 if the key does not exist).</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static async Task<long> ClearAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonClear, key];
        object? result = await client.CustomCommand(args);
        return ParseClearResult(result);
    }

    /// <summary>
    /// Clears container values (arrays/objects) and sets numeric values to 0, booleans to false, and strings to empty at the specified path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of values cleared.</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static async Task<long> ClearAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonClear, key, path];
        object? result = await client.CustomCommand(args);
        return ParseClearResult(result);
    }

    /// <summary>
    /// Clears container values (arrays/objects) and sets numeric values to 0, booleans to false, and strings to empty at the root path (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of values cleared (0 if the key does not exist).</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static async Task<long> ClearAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonClear, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseClearResult(result.SingleValue);
    }

    /// <summary>
    /// Clears container values (arrays/objects) and sets numeric values to 0, booleans to false, and strings to empty at the specified path (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of values cleared.</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static async Task<long> ClearAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonClear, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseClearResult(result.SingleValue);
    }

    /// <summary>
    /// Clears container values (arrays/objects) and sets numeric values to 0, booleans to false, and strings to empty at the root path (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The number of values cleared (0 if the key does not exist).</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static async Task<long> ClearAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonClear, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseClearResult(result.SingleValue);
    }

    /// <summary>
    /// Clears container values (arrays/objects) and sets numeric values to 0, booleans to false, and strings to empty at the specified path (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The number of values cleared.</returns>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static async Task<long> ClearAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonClear, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return ParseClearResult(result.SingleValue);
    }

    /// <summary>
    /// Parses the result of JSON.CLEAR command into a long.
    /// </summary>
    private static long ParseClearResult(object? result)
    {
        if (result is null)
        {
            return 0;
        }

        if (result is long longResult)
        {
            return longResult;
        }

        if (long.TryParse(result.ToString(), out long parsed))
        {
            return parsed;
        }

        return 0;
    }

    #endregion

    #region JSON.TYPE

    /// <summary>
    /// Gets the type of the JSON value at the root path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>
    /// The type of the JSON value at the root path. Returns one of: "null", "boolean", "string",
    /// "number", "integer", "object", or "array". Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\",\"age\":30}");
    /// object? type = await GlideJson.TypeAsync(client, "mykey");
    /// // type == "object"
    /// </code>
    /// </example>
    public static async Task<object?> TypeAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonType, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the type of the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of type strings for all matching paths.
    /// When a legacy path is provided, returns a single type string.
    /// Returns one of: "null", "boolean", "string", "number", "integer", "object", or "array".
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\",\"age\":30}");
    /// // JSONPath returns array of types
    /// object? types = await GlideJson.TypeAsync(client, "mykey", "$.name");
    /// // types == ["string"]
    ///
    /// // Legacy path returns single type
    /// object? type = await GlideJson.TypeAsync(client, "mykey", ".name");
    /// // type == "string"
    /// </code>
    /// </example>
    public static async Task<object?> TypeAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonType, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the type of the JSON value at the root path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>
    /// The type of the JSON value at the root path. Returns one of: "null", "boolean", "string",
    /// "number", "integer", "object", or "array". Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static async Task<object?> TypeAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonType, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the type of the JSON value at the specified path in the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of type strings for all matching paths.
    /// When a legacy path is provided, returns a single type string.
    /// Returns one of: "null", "boolean", "string", "number", "integer", "object", or "array".
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static async Task<object?> TypeAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonType, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the type of the JSON value at the root path in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>
    /// The type of the JSON value at the root path. Returns one of: "null", "boolean", "string",
    /// "number", "integer", "object", or "array". Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static async Task<object?> TypeAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonType, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the type of the JSON value at the specified path in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of type strings for all matching paths.
    /// When a legacy path is provided, returns a single type string.
    /// Returns one of: "null", "boolean", "string", "number", "integer", "object", or "array".
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static async Task<object?> TypeAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonType, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the type of the JSON value at the root path in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>
    /// The type of the JSON value at the root path. Returns one of: "null", "boolean", "string",
    /// "number", "integer", "object", or "array". Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static async Task<object?> TypeAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonType, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the type of the JSON value at the specified path in the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of type strings for all matching paths.
    /// When a legacy path is provided, returns a single type string.
    /// Returns one of: "null", "boolean", "string", "number", "integer", "object", or "array".
    /// Returns null if the key does not exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static async Task<object?> TypeAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonType, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    #endregion

    #region JSON.NUMINCRBY

    /// <summary>
    /// Increments the numeric value at the specified path in the JSON document stored at the key by the specified amount.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The amount to increment by (can be negative for decrement).</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON array of new values (or null for non-numeric matches).
    /// When a legacy path is provided, returns the new numeric value as a string.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-numeric value with legacy path,
    /// or if the result overflows 64-bit IEEE double range.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.numincrby/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"count\":10,\"price\":5.5}");
    ///
    /// // Increment with JSONPath - returns array
    /// string? result = await GlideJson.NumIncrByAsync(client, "mykey", "$.count", 5);
    /// // result == "[15]"
    ///
    /// // Increment with legacy path - returns single value
    /// string? result2 = await GlideJson.NumIncrByAsync(client, "mykey", ".price", 2.5);
    /// // result2 == "8"
    ///
    /// // Decrement using negative value
    /// string? result3 = await GlideJson.NumIncrByAsync(client, "mykey", "$.count", -3);
    /// // result3 == "[12]"
    /// </code>
    /// </example>
    public static async Task<string?> NumIncrByAsync(GlideClient client, string key, string path, double value)
    {
        GlideString[] args = BuildNumIncrByArgs(key, path, value);
        object? result = await client.CustomCommand(args);
        return result?.ToString();
    }

    /// <summary>
    /// Increments the numeric value at the specified path in the JSON document stored at the key by the specified amount.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The amount to increment by (can be negative for decrement).</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON array of new values (or null for non-numeric matches).
    /// When a legacy path is provided, returns the new numeric value as a GlideString.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-numeric value with legacy path,
    /// or if the result overflows 64-bit IEEE double range.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.numincrby/"/>
    public static async Task<GlideString?> NumIncrByAsync(GlideClient client, GlideString key, GlideString path, double value)
    {
        GlideString[] args = BuildNumIncrByArgs(key, path, value);
        object? result = await client.CustomCommand(args);
        return result is null ? null : (GlideString?)result.ToString();
    }

    /// <summary>
    /// Increments the numeric value at the specified path in the JSON document stored at the key by the specified amount (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The amount to increment by (can be negative for decrement).</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON array of new values (or null for non-numeric matches).
    /// When a legacy path is provided, returns the new numeric value as a string.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-numeric value with legacy path,
    /// or if the result overflows 64-bit IEEE double range.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.numincrby/"/>
    public static async Task<string?> NumIncrByAsync(GlideClusterClient client, string key, string path, double value)
    {
        GlideString[] args = BuildNumIncrByArgs(key, path, value);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue?.ToString();
    }

    /// <summary>
    /// Increments the numeric value at the specified path in the JSON document stored at the key by the specified amount (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The amount to increment by (can be negative for decrement).</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON array of new values (or null for non-numeric matches).
    /// When a legacy path is provided, returns the new numeric value as a GlideString.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-numeric value with legacy path,
    /// or if the result overflows 64-bit IEEE double range.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.numincrby/"/>
    public static async Task<GlideString?> NumIncrByAsync(GlideClusterClient client, GlideString key, GlideString path, double value)
    {
        GlideString[] args = BuildNumIncrByArgs(key, path, value);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue is null ? null : (GlideString?)result.SingleValue.ToString();
    }

    /// <summary>
    /// Builds the command arguments for JSON.NUMINCRBY.
    /// </summary>
    private static GlideString[] BuildNumIncrByArgs(GlideString key, GlideString path, double value)
    {
        // Use G format for general number formatting that handles both integers and decimals appropriately
        return [JsonNumIncrBy, key, path, value.ToString("G", System.Globalization.CultureInfo.InvariantCulture)];
    }

    #endregion

    #region JSON.NUMMULTBY

    /// <summary>
    /// Multiplies the numeric value at the specified path in the JSON document stored at the key by the specified amount.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The amount to multiply by.</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON array of new values (or null for non-numeric matches).
    /// When a legacy path is provided, returns the new numeric value as a string.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-numeric value with legacy path,
    /// or if the result overflows 64-bit IEEE double range.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.nummultby/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"count\":10,\"price\":5.5}");
    ///
    /// // Multiply with JSONPath - returns array
    /// string? result = await GlideJson.NumMultByAsync(client, "mykey", "$.count", 2);
    /// // result == "[20]"
    ///
    /// // Multiply with legacy path - returns single value
    /// string? result2 = await GlideJson.NumMultByAsync(client, "mykey", ".price", 3);
    /// // result2 == "16.5"
    ///
    /// // Multiply by zero
    /// string? result3 = await GlideJson.NumMultByAsync(client, "mykey", "$.count", 0);
    /// // result3 == "[0]"
    /// </code>
    /// </example>
    public static async Task<string?> NumMultByAsync(GlideClient client, string key, string path, double value)
    {
        GlideString[] args = BuildNumMultByArgs(key, path, value);
        object? result = await client.CustomCommand(args);
        return result?.ToString();
    }

    /// <summary>
    /// Multiplies the numeric value at the specified path in the JSON document stored at the key by the specified amount.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The amount to multiply by.</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON array of new values (or null for non-numeric matches).
    /// When a legacy path is provided, returns the new numeric value as a GlideString.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-numeric value with legacy path,
    /// or if the result overflows 64-bit IEEE double range.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.nummultby/"/>
    public static async Task<GlideString?> NumMultByAsync(GlideClient client, GlideString key, GlideString path, double value)
    {
        GlideString[] args = BuildNumMultByArgs(key, path, value);
        object? result = await client.CustomCommand(args);
        return result is null ? null : (GlideString?)result.ToString();
    }

    /// <summary>
    /// Multiplies the numeric value at the specified path in the JSON document stored at the key by the specified amount (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The amount to multiply by.</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON array of new values (or null for non-numeric matches).
    /// When a legacy path is provided, returns the new numeric value as a string.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-numeric value with legacy path,
    /// or if the result overflows 64-bit IEEE double range.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.nummultby/"/>
    public static async Task<string?> NumMultByAsync(GlideClusterClient client, string key, string path, double value)
    {
        GlideString[] args = BuildNumMultByArgs(key, path, value);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue?.ToString();
    }

    /// <summary>
    /// Multiplies the numeric value at the specified path in the JSON document stored at the key by the specified amount (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The amount to multiply by.</param>
    /// <returns>
    /// When a JSONPath is provided, returns a JSON array of new values (or null for non-numeric matches).
    /// When a legacy path is provided, returns the new numeric value as a GlideString.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-numeric value with legacy path,
    /// or if the result overflows 64-bit IEEE double range.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.nummultby/"/>
    public static async Task<GlideString?> NumMultByAsync(GlideClusterClient client, GlideString key, GlideString path, double value)
    {
        GlideString[] args = BuildNumMultByArgs(key, path, value);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue is null ? null : (GlideString?)result.SingleValue.ToString();
    }

    /// <summary>
    /// Builds the command arguments for JSON.NUMMULTBY.
    /// </summary>
    private static GlideString[] BuildNumMultByArgs(GlideString key, GlideString path, double value)
    {
        // Use G format for general number formatting that handles both integers and decimals appropriately
        return [JsonNumMultBy, key, path, value.ToString("G", System.Globalization.CultureInfo.InvariantCulture)];
    }

    #endregion

    #region JSON.STRAPPEND

    /// <summary>
    /// Appends the specified string to the string value at the root path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="value">The JSON-encoded string to append (e.g., "\"foo\"" to append foo).</param>
    /// <returns>
    /// The new length of the string at the root path, or null if the root is not a string.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the root path does not point to a string value.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "\"hello\"");
    /// object? result = await GlideJson.StrAppendAsync(client, "mykey", "\" world\"");
    /// // result == 11 (length of "hello world")
    /// </code>
    /// </example>
    public static async Task<object?> StrAppendAsync(GlideClient client, string key, string value)
    {
        GlideString[] args = [JsonStrAppend, key, value];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Appends the specified string to the string value at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON-encoded string to append (e.g., "\"foo\"" to append foo).</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of new lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the new string length.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-string value with legacy path.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\",\"greeting\":\"Hello\"}");
    ///
    /// // Append with JSONPath - returns array of lengths
    /// object? result = await GlideJson.StrAppendAsync(client, "mykey", "$.greeting", "\" World\"");
    /// // result == [11] (array with new length)
    ///
    /// // Append with legacy path - returns single length
    /// object? result2 = await GlideJson.StrAppendAsync(client, "mykey", ".name", "\" Doe\"");
    /// // result2 == 8 (length of "John Doe")
    /// </code>
    /// </example>
    public static async Task<object?> StrAppendAsync(GlideClient client, string key, string path, string value)
    {
        GlideString[] args = [JsonStrAppend, key, path, value];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Appends the specified string to the string value at the root path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="value">The JSON-encoded string to append (e.g., "\"foo\"" to append foo).</param>
    /// <returns>
    /// The new length of the string at the root path, or null if the root is not a string.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the root path does not point to a string value.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static async Task<object?> StrAppendAsync(GlideClient client, GlideString key, GlideString value)
    {
        GlideString[] args = [JsonStrAppend, key, value];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Appends the specified string to the string value at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON-encoded string to append (e.g., "\"foo\"" to append foo).</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of new lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the new string length.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-string value with legacy path.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static async Task<object?> StrAppendAsync(GlideClient client, GlideString key, GlideString path, GlideString value)
    {
        GlideString[] args = [JsonStrAppend, key, path, value];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Appends the specified string to the string value at the root path in the JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="value">The JSON-encoded string to append (e.g., "\"foo\"" to append foo).</param>
    /// <returns>
    /// The new length of the string at the root path, or null if the root is not a string.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the root path does not point to a string value.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static async Task<object?> StrAppendAsync(GlideClusterClient client, string key, string value)
    {
        GlideString[] args = [JsonStrAppend, key, value];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Appends the specified string to the string value at the specified path in the JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON-encoded string to append (e.g., "\"foo\"" to append foo).</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of new lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the new string length.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-string value with legacy path.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static async Task<object?> StrAppendAsync(GlideClusterClient client, string key, string path, string value)
    {
        GlideString[] args = [JsonStrAppend, key, path, value];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Appends the specified string to the string value at the root path in the JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="value">The JSON-encoded string to append (e.g., "\"foo\"" to append foo).</param>
    /// <returns>
    /// The new length of the string at the root path, or null if the root is not a string.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the root path does not point to a string value.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static async Task<object?> StrAppendAsync(GlideClusterClient client, GlideString key, GlideString value)
    {
        GlideString[] args = [JsonStrAppend, key, value];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Appends the specified string to the string value at the specified path in the JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON-encoded string to append (e.g., "\"foo\"" to append foo).</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of new lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the new string length.
    /// </returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown if the path does not exist or points to a non-string value with legacy path.
    /// </exception>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static async Task<object?> StrAppendAsync(GlideClusterClient client, GlideString key, GlideString path, GlideString value)
    {
        GlideString[] args = [JsonStrAppend, key, path, value];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    #endregion

    #region JSON.STRLEN

    /// <summary>
    /// Gets the length of the string value at the root path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>
    /// The length of the string at the root path, or null if the key does not exist or the root is not a string.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "\"hello\"");
    /// object? result = await GlideJson.StrLenAsync(client, "mykey");
    /// // result == 5 (length of "hello")
    /// </code>
    /// </example>
    public static async Task<object?> StrLenAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonStrLen, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the length of the string value at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the string length.
    /// When the key does not exist, returns null.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"name\":\"John\",\"greeting\":\"Hello\"}");
    ///
    /// // Get length with JSONPath - returns array of lengths
    /// object? result = await GlideJson.StrLenAsync(client, "mykey", "$.greeting");
    /// // result == [5] (array with length of "Hello")
    ///
    /// // Get length with legacy path - returns single length
    /// object? result2 = await GlideJson.StrLenAsync(client, "mykey", ".name");
    /// // result2 == 4 (length of "John")
    /// </code>
    /// </example>
    public static async Task<object?> StrLenAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonStrLen, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the length of the string value at the root path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>
    /// The length of the string at the root path, or null if the key does not exist or the root is not a string.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static async Task<object?> StrLenAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonStrLen, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the length of the string value at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the string length.
    /// When the key does not exist, returns null.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static async Task<object?> StrLenAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonStrLen, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the length of the string value at the root path in the JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>
    /// The length of the string at the root path, or null if the key does not exist or the root is not a string.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static async Task<object?> StrLenAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonStrLen, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the length of the string value at the specified path in the JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the string length.
    /// When the key does not exist, returns null.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static async Task<object?> StrLenAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonStrLen, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the length of the string value at the root path in the JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>
    /// The length of the string at the root path, or null if the key does not exist or the root is not a string.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static async Task<object?> StrLenAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonStrLen, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the length of the string value at the specified path in the JSON document stored at the key (cluster client).
    /// </summary>
    /// <param name="client">The Glide cluster client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the string length.
    /// When the key does not exist, returns null.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static async Task<object?> StrLenAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonStrLen, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    #endregion
}

