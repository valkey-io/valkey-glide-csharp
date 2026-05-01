// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Pipeline;

namespace Valkey.Glide.ServerModules;

/// <summary>
/// Extension methods for adding JSON commands to batches.
/// Provides batch support for JSON module commands.
/// </summary>
/// <seealso href="https://valkey.io/commands/?group=json">Valkey JSON Commands</seealso>
/// <example>
/// <code>
/// var batch = new Batch(isAtomic: true);
/// batch.JsonSet("mykey", "$", "{\"name\":\"John\"}");
/// batch.JsonGet("mykey");
/// object?[] results = await client.Exec(batch, true);
/// // results[0] == "OK"
/// // results[1] == "{\"name\":\"John\"}"
/// </code>
/// </example>
public static class GlideJsonBatch
{
    private const string JsonPrefix = "JSON.";

    // Command constants
    private const string JsonSetCmd = JsonPrefix + "SET";
    private const string JsonGetCmd = JsonPrefix + "GET";
    private const string JsonMGetCmd = JsonPrefix + "MGET";
    private const string JsonDelCmd = JsonPrefix + "DEL";
    private const string JsonForgetCmd = JsonPrefix + "FORGET";
    private const string JsonClearCmd = JsonPrefix + "CLEAR";
    private const string JsonTypeCmd = JsonPrefix + "TYPE";
    private const string JsonNumIncrByCmd = JsonPrefix + "NUMINCRBY";
    private const string JsonNumMultByCmd = JsonPrefix + "NUMMULTBY";
    private const string JsonStrAppendCmd = JsonPrefix + "STRAPPEND";
    private const string JsonStrLenCmd = JsonPrefix + "STRLEN";
    private const string JsonArrAppendCmd = JsonPrefix + "ARRAPPEND";
    private const string JsonArrInsertCmd = JsonPrefix + "ARRINSERT";
    private const string JsonArrIndexCmd = JsonPrefix + "ARRINDEX";
    private const string JsonArrLenCmd = JsonPrefix + "ARRLEN";
    private const string JsonArrPopCmd = JsonPrefix + "ARRPOP";
    private const string JsonArrTrimCmd = JsonPrefix + "ARRTRIM";
    private const string JsonObjLenCmd = JsonPrefix + "OBJLEN";
    private const string JsonObjKeysCmd = JsonPrefix + "OBJKEYS";
    private const string JsonToggleCmd = JsonPrefix + "TOGGLE";
    private const string JsonDebugCmd = JsonPrefix + "DEBUG";
    private const string JsonRespCmd = JsonPrefix + "RESP";

    #region JSON.SET

    /// <summary>
    /// Adds a JSON.SET command to the batch.
    /// Sets the JSON value at the specified path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - "OK" if the value was set successfully, or null if the condition was not met.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static T JsonSet<T>(this T batch, GlideString key, GlideString path, GlideString value)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonSetCmd, key, path, value]);

    /// <summary>
    /// Adds a JSON.SET command to the batch with a condition.
    /// Sets the JSON value at the specified path in the key only if the condition is met.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to set (must be valid JSON-encoded string).</param>
    /// <param name="condition">The condition for setting the value (NX or XX).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - "OK" if the value was set successfully, or null if the condition was not met.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public static T JsonSet<T>(this T batch, GlideString key, GlideString path, GlideString value, GlideJson.SetCondition condition)
        where T : BaseBatch<T>
    {
        GlideString[] args = condition switch
        {
            GlideJson.SetCondition.None => [JsonSetCmd, key, path, value],
            GlideJson.SetCondition.OnlyIfDoesNotExist => [JsonSetCmd, key, path, value, "NX"],
            GlideJson.SetCondition.OnlyIfExists => [JsonSetCmd, key, path, value, "XX"],
            _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, "Invalid SetCondition value")
        };
        return batch.CustomCommand(args);
    }

    #endregion

    #region JSON.GET

    /// <summary>
    /// Adds a JSON.GET command to the batch.
    /// Gets the entire JSON document stored at the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The JSON document, or null if the key does not exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static T JsonGet<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonGetCmd, key]);

    /// <summary>
    /// Adds a JSON.GET command to the batch with paths.
    /// Gets the JSON value(s) at the specified path(s) in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The JSON value(s), or null if the key does not exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static T JsonGet<T>(this T batch, GlideString key, params GlideString[] paths)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonGetCmd, key];
        args.AddRange(paths);
        return batch.CustomCommand(args);
    }

    /// <summary>
    /// Adds a JSON.GET command to the batch with formatting options.
    /// Gets the entire JSON document stored at the key with formatting.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="options">Formatting options for the JSON output.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The formatted JSON document, or null if the key does not exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static T JsonGet<T>(this T batch, GlideString key, GlideJson.GetOptions options)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonGetCmd, key];
        args.AddRange(options.ToArgs());
        return batch.CustomCommand(args);
    }

    /// <summary>
    /// Adds a JSON.GET command to the batch with paths and formatting options.
    /// Gets the JSON value(s) at the specified path(s) in the key with formatting.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="paths">The JSONPath or legacy path(s) within the JSON document.</param>
    /// <param name="options">Formatting options for the JSON output.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The formatted JSON value(s), or null if the key does not exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.get/"/>
    public static T JsonGet<T>(this T batch, GlideString key, GlideString[] paths, GlideJson.GetOptions options)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonGetCmd, key];
        args.AddRange(options.ToArgs());
        args.AddRange(paths);
        return batch.CustomCommand(args);
    }

    #endregion

    #region JSON.MGET

    /// <summary>
    /// Adds a JSON.MGET command to the batch.
    /// Gets the JSON values at the specified path from multiple keys.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="keys">The keys where the JSON documents are stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON documents.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - An array of JSON values, one for each key. Returns null for keys that don't exist or don't have the path.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.mget/"/>
    public static T JsonMGet<T>(this T batch, GlideString[] keys, GlideString path)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonMGetCmd];
        args.AddRange(keys);
        args.Add(path);
        return batch.CustomCommand(args);
    }

    #endregion

    #region JSON.DEL

    /// <summary>
    /// Adds a JSON.DEL command to the batch.
    /// Deletes the entire JSON document stored at the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The number of paths deleted (1 if the key existed, 0 otherwise).
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static T JsonDel<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDelCmd, key]);

    /// <summary>
    /// Adds a JSON.DEL command to the batch with a path.
    /// Deletes the JSON value at the specified path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The number of paths deleted.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.del/"/>
    public static T JsonDel<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDelCmd, key, path]);

    #endregion


    #region JSON.FORGET

    /// <summary>
    /// Adds a JSON.FORGET command to the batch.
    /// Alias for JSON.DEL. Deletes the entire JSON document stored at the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The number of paths deleted (1 if the key existed, 0 otherwise).
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static T JsonForget<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonForgetCmd, key]);

    /// <summary>
    /// Adds a JSON.FORGET command to the batch with a path.
    /// Alias for JSON.DEL. Deletes the JSON value at the specified path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The number of paths deleted.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.forget/"/>
    public static T JsonForget<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonForgetCmd, key, path]);

    #endregion

    #region JSON.CLEAR

    /// <summary>
    /// Adds a JSON.CLEAR command to the batch.
    /// Clears the JSON value at the root path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The number of values cleared.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static T JsonClear<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonClearCmd, key]);

    /// <summary>
    /// Adds a JSON.CLEAR command to the batch with a path.
    /// Clears the JSON value at the specified path in the key (sets arrays to empty, objects to empty, numbers to 0).
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The number of values cleared.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.clear/"/>
    public static T JsonClear<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonClearCmd, key, path]);

    #endregion

    #region JSON.TYPE

    /// <summary>
    /// Adds a JSON.TYPE command to the batch.
    /// Gets the type of the JSON value at the root path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The type string of the root value ("null", "boolean", "string", "number", "integer", "object", "array").
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static T JsonType<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonTypeCmd, key]);

    /// <summary>
    /// Adds a JSON.TYPE command to the batch with a path.
    /// Gets the type of the JSON value at the specified path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of type strings (or null for non-existent paths).
    /// When a legacy path is provided, returns the type string.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.type/"/>
    public static T JsonType<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonTypeCmd, key, path]);

    #endregion

    #region JSON.NUMINCRBY

    /// <summary>
    /// Adds a JSON.NUMINCRBY command to the batch.
    /// Increments the numeric value at the specified path in the key by the given amount.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="increment">The amount to increment by.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns a JSON string array of new values (or "null" for non-numeric matches).
    /// When a legacy path is provided, returns the new value as a string.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.numincrby/"/>
    public static T JsonNumIncrBy<T>(this T batch, GlideString key, GlideString path, double increment)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonNumIncrByCmd, key, path, increment.ToString()]);

    #endregion

    #region JSON.NUMMULTBY

    /// <summary>
    /// Adds a JSON.NUMMULTBY command to the batch.
    /// Multiplies the numeric value at the specified path in the key by the given amount.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="multiplier">The amount to multiply by.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns a JSON string array of new values (or "null" for non-numeric matches).
    /// When a legacy path is provided, returns the new value as a string.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.nummultby/"/>
    public static T JsonNumMultBy<T>(this T batch, GlideString key, GlideString path, double multiplier)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonNumMultByCmd, key, path, multiplier.ToString()]);

    #endregion

    #region JSON.STRAPPEND

    /// <summary>
    /// Adds a JSON.STRAPPEND command to the batch.
    /// Appends a string to the string value at the root path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="value">The JSON string value to append (must be a valid JSON string, e.g., "\"suffix\"").</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The new string length.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static T JsonStrAppend<T>(this T batch, GlideString key, GlideString value)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonStrAppendCmd, key, value]);

    /// <summary>
    /// Adds a JSON.STRAPPEND command to the batch with a path.
    /// Appends a string to the string value at the specified path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON string value to append (must be a valid JSON string, e.g., "\"suffix\"").</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of new string lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the new string length.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.strappend/"/>
    public static T JsonStrAppend<T>(this T batch, GlideString key, GlideString path, GlideString value)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonStrAppendCmd, key, path, value]);

    #endregion

    #region JSON.STRLEN

    /// <summary>
    /// Adds a JSON.STRLEN command to the batch.
    /// Gets the length of the string value at the root path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The string length.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static T JsonStrLen<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonStrLenCmd, key]);

    /// <summary>
    /// Adds a JSON.STRLEN command to the batch with a path.
    /// Gets the length of the string value at the specified path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of string lengths (or null for non-string matches).
    /// When a legacy path is provided, returns the string length.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.strlen/"/>
    public static T JsonStrLen<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonStrLenCmd, key, path]);

    #endregion


    #region JSON.ARRAPPEND

    /// <summary>
    /// Adds a JSON.ARRAPPEND command to the batch.
    /// Appends one or more values to the JSON array at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="values">The JSON values to append (must be valid JSON-encoded strings).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of new array lengths (or null for non-array matches).
    /// When a legacy path is provided, returns the new array length.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrappend/"/>
    public static T JsonArrAppend<T>(this T batch, GlideString key, GlideString path, params GlideString[] values)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonArrAppendCmd, key, path];
        args.AddRange(values);
        return batch.CustomCommand(args);
    }

    #endregion

    #region JSON.ARRINSERT

    /// <summary>
    /// Adds a JSON.ARRINSERT command to the batch.
    /// Inserts one or more values into the array at the specified path before the given index.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="index">The array index before which values are inserted.</param>
    /// <param name="values">The JSON values to insert (must be valid JSON-encoded strings).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of new array lengths (or null for non-array matches).
    /// When a legacy path is provided, returns the new array length.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrinsert/"/>
    public static T JsonArrInsert<T>(this T batch, GlideString key, GlideString path, long index, params GlideString[] values)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonArrInsertCmd, key, path, index.ToString()];
        args.AddRange(values);
        return batch.CustomCommand(args);
    }

    #endregion

    #region JSON.ARRINDEX

    /// <summary>
    /// Adds a JSON.ARRINDEX command to the batch.
    /// Searches for the first occurrence of a scalar JSON value in the arrays at the path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The scalar value to search for (must be a valid JSON-encoded string).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of indices (-1 if not found, null for non-array matches).
    /// When a legacy path is provided, returns the index (-1 if not found).
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrindex/"/>
    public static T JsonArrIndex<T>(this T batch, GlideString key, GlideString path, GlideString value)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrIndexCmd, key, path, value]);

    /// <summary>
    /// Adds a JSON.ARRINDEX command to the batch with range options.
    /// Searches for the first occurrence of a scalar JSON value in the arrays at the path within the specified range.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The scalar value to search for (must be a valid JSON-encoded string).</param>
    /// <param name="options">The range options for the search.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of indices (-1 if not found, null for non-array matches).
    /// When a legacy path is provided, returns the index (-1 if not found).
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrindex/"/>
    public static T JsonArrIndex<T>(this T batch, GlideString key, GlideString path, GlideString value, GlideJson.ArrIndexOptions options)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonArrIndexCmd, key, path, value];
        args.AddRange(options.ToArgs());
        return batch.CustomCommand(args);
    }

    #endregion

    #region JSON.ARRLEN

    /// <summary>
    /// Adds a JSON.ARRLEN command to the batch.
    /// Gets the length of the array at the root of the JSON document.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The array length, or null if the key doesn't exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrlen/"/>
    public static T JsonArrLen<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrLenCmd, key]);

    /// <summary>
    /// Adds a JSON.ARRLEN command to the batch with a path.
    /// Gets the length of the array at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of lengths (or null for non-array matches).
    /// When a legacy path is provided, returns the array length.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrlen/"/>
    public static T JsonArrLen<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrLenCmd, key, path]);

    #endregion

    #region JSON.ARRPOP

    /// <summary>
    /// Adds a JSON.ARRPOP command to the batch.
    /// Pops the last element from the array at the root of the JSON document.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The popped JSON value, or null if the array is empty.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrpop/"/>
    public static T JsonArrPop<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrPopCmd, key]);

    /// <summary>
    /// Adds a JSON.ARRPOP command to the batch with a path.
    /// Pops the last element from the array at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of popped values (or null for non-array/empty matches).
    /// When a legacy path is provided, returns the popped value.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrpop/"/>
    public static T JsonArrPop<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrPopCmd, key, path]);

    /// <summary>
    /// Adds a JSON.ARRPOP command to the batch with a path and index.
    /// Pops the element at the specified index from the array at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="index">The index of the element to pop (negative indices count from the end).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of popped values (or null for non-array/empty matches).
    /// When a legacy path is provided, returns the popped value.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrpop/"/>
    public static T JsonArrPop<T>(this T batch, GlideString key, GlideString path, long index)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrPopCmd, key, path, index.ToString()]);

    #endregion

    #region JSON.ARRTRIM

    /// <summary>
    /// Adds a JSON.ARRTRIM command to the batch.
    /// Trims the array at the specified path to contain only elements within the specified range.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="start">The start index (inclusive).</param>
    /// <param name="stop">The stop index (inclusive).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of new array lengths (or null for non-array matches).
    /// When a legacy path is provided, returns the new array length.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrtrim/"/>
    public static T JsonArrTrim<T>(this T batch, GlideString key, GlideString path, long start, long stop)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrTrimCmd, key, path, start.ToString(), stop.ToString()]);

    #endregion


    #region JSON.OBJLEN

    /// <summary>
    /// Adds a JSON.OBJLEN command to the batch.
    /// Gets the number of keys in the JSON object at the root of the document.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The number of keys in the object, or null if the key doesn't exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.objlen/"/>
    public static T JsonObjLen<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonObjLenCmd, key]);

    /// <summary>
    /// Adds a JSON.OBJLEN command to the batch with a path.
    /// Gets the number of keys in the JSON object at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of lengths (or null for non-object matches).
    /// When a legacy path is provided, returns the number of keys.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.objlen/"/>
    public static T JsonObjLen<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonObjLenCmd, key, path]);

    #endregion

    #region JSON.OBJKEYS

    /// <summary>
    /// Adds a JSON.OBJKEYS command to the batch.
    /// Gets the keys in the JSON object at the root of the document.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - An array of keys in the object, or null if the key doesn't exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.objkeys/"/>
    public static T JsonObjKeys<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonObjKeysCmd, key]);

    /// <summary>
    /// Adds a JSON.OBJKEYS command to the batch with a path.
    /// Gets the keys in the JSON object at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of arrays of keys (or null for non-object matches).
    /// When a legacy path is provided, returns an array of keys.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.objkeys/"/>
    public static T JsonObjKeys<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonObjKeysCmd, key, path]);

    #endregion

    #region JSON.TOGGLE

    /// <summary>
    /// Adds a JSON.TOGGLE command to the batch.
    /// Toggles the boolean value at the specified path in the key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of new boolean values (or null for non-boolean matches).
    /// When a legacy path is provided, returns the new boolean value as a string ("true" or "false").
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.toggle/"/>
    public static T JsonToggle<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonToggleCmd, key, path]);

    #endregion

    #region JSON.DEBUG

    /// <summary>
    /// Adds a JSON.DEBUG MEMORY command to the batch.
    /// Reports memory usage in bytes of a JSON object at the root of the document.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The total memory usage in bytes, or null if the key doesn't exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.debug/"/>
    public static T JsonDebugMemory<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDebugCmd, "MEMORY", key]);

    /// <summary>
    /// Adds a JSON.DEBUG MEMORY command to the batch with a path.
    /// Reports memory usage in bytes of a JSON object at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of memory usage values.
    /// When a legacy path is provided, returns the memory usage.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.debug/"/>
    public static T JsonDebugMemory<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDebugCmd, "MEMORY", key, path]);

    /// <summary>
    /// Adds a JSON.DEBUG FIELDS command to the batch.
    /// Reports the number of fields at the root of the JSON document.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The total number of fields, or null if the key doesn't exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.debug/"/>
    public static T JsonDebugFields<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDebugCmd, "FIELDS", key]);

    /// <summary>
    /// Adds a JSON.DEBUG FIELDS command to the batch with a path.
    /// Reports the number of fields at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of field counts.
    /// When a legacy path is provided, returns the field count.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.debug/"/>
    public static T JsonDebugFields<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDebugCmd, "FIELDS", key, path]);

    #endregion

    #region JSON.RESP

    /// <summary>
    /// Adds a JSON.RESP command to the batch.
    /// Returns the JSON value at the root of the document in RESP format.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - The JSON value in RESP format, or null if the key doesn't exist.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.resp/"/>
    public static T JsonResp<T>(this T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonRespCmd, key]);

    /// <summary>
    /// Adds a JSON.RESP command to the batch with a path.
    /// Returns the JSON value at the specified path in RESP format.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>
    /// Command Response - When a JSONPath is provided, returns an array of RESP values.
    /// When a legacy path is provided, returns the RESP value.
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.resp/"/>
    public static T JsonResp<T>(this T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonRespCmd, key, path]);

    #endregion
}
