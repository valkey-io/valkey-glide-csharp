// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Pipeline;

namespace Valkey.Glide.ServerModules;

/// <summary>
/// Batch implementation for JSON module. Batches allow the execution of a group of commands in a
/// single step. See <see cref="Batch"/>.
/// </summary>
/// <remarks>
/// This class provides static methods for adding JSON commands to batches, following the same
/// pattern as the Java client's JsonBatch class.
/// </remarks>
/// <example>
/// <code>
/// Batch batch = new Batch(true);
/// GlideJsonBatch.Set(batch, "doc", ".", "{\"a\": 1.0, \"b\": 2}");
/// GlideJsonBatch.Get(batch, "doc");
/// object?[]? result = await client.Exec(batch, false);
/// // result[0] == "OK" (result of Set)
/// // result[1] == "{\"a\": 1.0, \"b\": 2}" (result of Get)
/// </code>
/// </example>
/// <seealso href="https://valkey.io/commands/?group=json">Valkey JSON Commands</seealso>
public static class GlideJsonBatch
{
    private const string JsonPrefix = "JSON.";
    private const string JsonSetCmd = JsonPrefix + "SET";
    private const string JsonGetCmd = JsonPrefix + "GET";
    private const string JsonMGetCmd = JsonPrefix + "MGET";
    private const string JsonDelCmd = JsonPrefix + "DEL";
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
    /// Sets the JSON value at the specified path stored at key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="value">The value to set (must be valid JSON).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>Command Response - "OK" if successful.</remarks>
    public static T Set<T>(T batch, GlideString key, GlideString path, GlideString value)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonSetCmd, key, path, value]);

    /// <summary>
    /// Adds a JSON.SET command to the batch with a condition.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="value">The value to set (must be valid JSON).</param>
    /// <param name="condition">The condition for setting (NX or XX).</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>Command Response - "OK" if successful, null if condition not met.</remarks>
    public static T Set<T>(T batch, GlideString key, GlideString path, GlideString value, GlideJson.SetCondition condition)
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
    /// Gets the entire JSON document stored at key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>Command Response - The JSON document, or null if key doesn't exist.</remarks>
    public static T Get<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonGetCmd, key]);

    /// <summary>
    /// Adds a JSON.GET command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Get<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonGetCmd, key, path]);

    /// <summary>
    /// Adds a JSON.GET command to the batch with multiple paths.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="paths">The paths within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Get<T>(T batch, GlideString key, GlideString[] paths)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonGetCmd, key];
        args.AddRange(paths);
        return batch.CustomCommand(args);
    }

    /// <summary>
    /// Adds a JSON.GET command to the batch with formatting options.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="options">Formatting options for the JSON output.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Get<T>(T batch, GlideString key, GlideJson.GetOptions options)
        where T : BaseBatch<T>
    {
        List<GlideString> args = [JsonGetCmd, key];
        args.AddRange(options.ToArgs());
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
    /// <param name="keys">The keys of the JSON documents.</param>
    /// <param name="path">The path within the JSON documents.</param>
    /// <returns>The batch for chaining.</returns>
    public static T MGet<T>(T batch, GlideString[] keys, GlideString path)
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
    /// Deletes the entire JSON document stored at key.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    /// <remarks>Command Response - Number of paths deleted.</remarks>
    public static T Del<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDelCmd, key]);

    /// <summary>
    /// Adds a JSON.DEL command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Del<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDelCmd, key, path]);

    #endregion

    #region JSON.CLEAR

    /// <summary>
    /// Adds a JSON.CLEAR command to the batch.
    /// Clears the JSON value at the root path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Clear<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonClearCmd, key]);

    /// <summary>
    /// Adds a JSON.CLEAR command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Clear<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonClearCmd, key, path]);

    #endregion

    #region JSON.TYPE

    /// <summary>
    /// Adds a JSON.TYPE command to the batch.
    /// Gets the type of the JSON value at the root.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Type<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonTypeCmd, key]);

    /// <summary>
    /// Adds a JSON.TYPE command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Type<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonTypeCmd, key, path]);

    #endregion

    #region JSON.NUMINCRBY

    /// <summary>
    /// Adds a JSON.NUMINCRBY command to the batch.
    /// Increments the numeric value at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="increment">The amount to increment by.</param>
    /// <returns>The batch for chaining.</returns>
    public static T NumIncrBy<T>(T batch, GlideString key, GlideString path, double increment)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonNumIncrByCmd, key, path, increment.ToString()]);

    #endregion

    #region JSON.NUMMULTBY

    /// <summary>
    /// Adds a JSON.NUMMULTBY command to the batch.
    /// Multiplies the numeric value at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="multiplier">The amount to multiply by.</param>
    /// <returns>The batch for chaining.</returns>
    public static T NumMultBy<T>(T batch, GlideString key, GlideString path, double multiplier)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonNumMultByCmd, key, path, multiplier.ToString()]);

    #endregion

    #region JSON.STRAPPEND

    /// <summary>
    /// Adds a JSON.STRAPPEND command to the batch.
    /// Appends a string to the string value at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="value">The JSON string to append (must be valid JSON string).</param>
    /// <returns>The batch for chaining.</returns>
    public static T StrAppend<T>(T batch, GlideString key, GlideString path, GlideString value)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonStrAppendCmd, key, path, value]);

    #endregion

    #region JSON.STRLEN

    /// <summary>
    /// Adds a JSON.STRLEN command to the batch.
    /// Gets the length of the string value at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T StrLen<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonStrLenCmd, key, path]);

    #endregion

    #region JSON.ARRAPPEND

    /// <summary>
    /// Adds a JSON.ARRAPPEND command to the batch.
    /// Appends values to the array at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="values">The JSON values to append.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ArrAppend<T>(T batch, GlideString key, GlideString path, GlideString[] values)
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
    /// Inserts values into the array at the specified path before the given index.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="index">The array index before which to insert.</param>
    /// <param name="values">The JSON values to insert.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ArrInsert<T>(T batch, GlideString key, GlideString path, long index, GlideString[] values)
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
    /// Searches for the first occurrence of a value in the array.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ArrIndex<T>(T batch, GlideString key, GlideString path, GlideString value)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrIndexCmd, key, path, value]);

    #endregion

    #region JSON.ARRLEN

    /// <summary>
    /// Adds a JSON.ARRLEN command to the batch.
    /// Gets the length of the array at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ArrLen<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrLenCmd, key, path]);

    #endregion

    #region JSON.ARRPOP

    /// <summary>
    /// Adds a JSON.ARRPOP command to the batch.
    /// Pops the last element from the array at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ArrPop<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrPopCmd, key, path]);

    /// <summary>
    /// Adds a JSON.ARRPOP command to the batch with an index.
    /// Pops the element at the specified index from the array.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="index">The index of the element to pop.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ArrPop<T>(T batch, GlideString key, GlideString path, long index)
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
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <param name="start">The start index (inclusive).</param>
    /// <param name="stop">The stop index (inclusive).</param>
    /// <returns>The batch for chaining.</returns>
    public static T ArrTrim<T>(T batch, GlideString key, GlideString path, long start, long stop)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonArrTrimCmd, key, path, start.ToString(), stop.ToString()]);

    #endregion

    #region JSON.OBJLEN

    /// <summary>
    /// Adds a JSON.OBJLEN command to the batch.
    /// Gets the number of keys in the object at the root.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ObjLen<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonObjLenCmd, key]);

    /// <summary>
    /// Adds a JSON.OBJLEN command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ObjLen<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonObjLenCmd, key, path]);

    #endregion

    #region JSON.OBJKEYS

    /// <summary>
    /// Adds a JSON.OBJKEYS command to the batch.
    /// Gets the keys in the object at the root.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ObjKeys<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonObjKeysCmd, key]);

    /// <summary>
    /// Adds a JSON.OBJKEYS command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T ObjKeys<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonObjKeysCmd, key, path]);

    #endregion

    #region JSON.TOGGLE

    /// <summary>
    /// Adds a JSON.TOGGLE command to the batch.
    /// Toggles the boolean value at the specified path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Toggle<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonToggleCmd, key, path]);

    #endregion

    #region JSON.DEBUG

    /// <summary>
    /// Adds a JSON.DEBUG MEMORY command to the batch.
    /// Reports memory usage in bytes of a JSON value.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T DebugMemory<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDebugCmd, "MEMORY", key]);

    /// <summary>
    /// Adds a JSON.DEBUG MEMORY command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T DebugMemory<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDebugCmd, "MEMORY", key, path]);

    /// <summary>
    /// Adds a JSON.DEBUG FIELDS command to the batch.
    /// Reports the number of fields in a JSON value.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T DebugFields<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDebugCmd, "FIELDS", key]);

    /// <summary>
    /// Adds a JSON.DEBUG FIELDS command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T DebugFields<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonDebugCmd, "FIELDS", key, path]);

    #endregion

    #region JSON.RESP

    /// <summary>
    /// Adds a JSON.RESP command to the batch.
    /// Returns the JSON value at the root in RESP format.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Resp<T>(T batch, GlideString key)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonRespCmd, key]);

    /// <summary>
    /// Adds a JSON.RESP command to the batch with a path.
    /// </summary>
    /// <typeparam name="T">The batch type.</typeparam>
    /// <param name="batch">The batch to add the command to.</param>
    /// <param name="key">The key of the JSON document.</param>
    /// <param name="path">The path within the JSON document.</param>
    /// <returns>The batch for chaining.</returns>
    public static T Resp<T>(T batch, GlideString key, GlideString path)
        where T : BaseBatch<T>
        => batch.CustomCommand([JsonRespCmd, key, path]);

    #endregion
}
