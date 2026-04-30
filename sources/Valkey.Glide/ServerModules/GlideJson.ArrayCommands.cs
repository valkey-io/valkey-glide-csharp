// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules.Options;

namespace Valkey.Glide.ServerModules;

/// <summary>
/// Module for JSON commands - Array operations.
/// </summary>
public static partial class GlideJson
{
    #region JSON.ARRAPPEND

    /// <summary>
    /// Appends one or more values to the array at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="values">The JSON values to append to the array.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of new array lengths (or null for non-array matches).
    /// When a legacy path is provided, returns the new array length.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrappend/"/>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"arr\":[1,2,3]}");
    /// ValkeyResult result = await GlideJson.ArrAppendAsync(client, "mykey", "$.arr", "4", "5");
    /// // (long[])result == [5] (new length of the array)
    /// </code>
    /// </example>
    public static async Task<ValkeyResult> ArrAppendAsync(BaseClient client, ValkeyKey key, ValkeyValue path, params ValkeyValue[] values)
    {
        GlideString[] args = BuildArrAppendArgs(ToGlideString(key), ToGlideString(path), values.Select(v => ToGlideString(v)).ToArray());
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    private static GlideString[] BuildArrAppendArgs(GlideString key, GlideString path, GlideString[] values)
    {
        List<GlideString> args = [JsonArrAppend, key, path];
        args.AddRange(values);
        return [.. args];
    }

    #endregion

    #region JSON.ARRINSERT

    /// <summary>
    /// Inserts one or more values into the array at the specified path and index in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="index">The index before which to insert the values. Negative indices count from the end.</param>
    /// <param name="values">The JSON values to insert into the array.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of new array lengths (or null for non-array matches).
    /// When a legacy path is provided, returns the new array length.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrinsert/"/>
    public static async Task<ValkeyResult> ArrInsertAsync(BaseClient client, ValkeyKey key, ValkeyValue path, long index, params ValkeyValue[] values)
    {
        GlideString[] args = BuildArrInsertArgs(ToGlideString(key), ToGlideString(path), index, values.Select(v => ToGlideString(v)).ToArray());
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    private static GlideString[] BuildArrInsertArgs(GlideString key, GlideString path, long index, GlideString[] values)
    {
        List<GlideString> args = [JsonArrInsert, key, path, index.ToString()];
        args.AddRange(values);
        return [.. args];
    }

    #endregion

    #region JSON.ARRINDEX

    /// <summary>
    /// Searches for the first occurrence of a value in the array at the specified path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to search for.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of indices (or -1 if not found, null for non-array matches).
    /// When a legacy path is provided, returns the index or -1 if not found.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrindex/"/>
    public static async Task<ValkeyResult> ArrIndexAsync(BaseClient client, ValkeyKey key, ValkeyValue path, ValkeyValue value)
    {
        GlideString[] args = [JsonArrIndex, ToGlideString(key), ToGlideString(path), ToGlideString(value)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array at the specified path with range options.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to search for.</param>
    /// <param name="options">Range options for the search.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of indices (or -1 if not found, null for non-array matches).
    /// When a legacy path is provided, returns the index or -1 if not found.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrindex/"/>
    public static async Task<ValkeyResult> ArrIndexAsync(BaseClient client, ValkeyKey key, ValkeyValue path, ValkeyValue value, JsonArrIndexOptions options)
    {
        GlideString[] args = BuildArrIndexArgs(ToGlideString(key), ToGlideString(path), ToGlideString(value), options);
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    private static GlideString[] BuildArrIndexArgs(GlideString key, GlideString path, GlideString value, JsonArrIndexOptions options)
    {
        List<GlideString> args = [JsonArrIndex, key, path, value];
        args.AddRange(options.ToArgs());
        return [.. args];
    }

    #endregion

    #region JSON.ARRLEN

    /// <summary>
    /// Gets the length of the array at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of lengths (or null for non-array matches).
    /// When a legacy path is provided, returns the array length.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrlen/"/>
    public static async Task<ValkeyResult> ArrLenAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonArrLen, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Gets the length of the array at the root path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The array length at the root path.</returns>
    /// <seealso href="https://valkey.io/commands/json.arrlen/"/>
    public static async Task<ValkeyResult> ArrLenAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonArrLen, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    #endregion

    #region JSON.ARRPOP

    /// <summary>
    /// Pops an element from the array at the specified path and index in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="index">The index of the element to pop. Negative indices count from the end. Default is -1 (last element).</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of popped elements (or null for non-array/empty matches).
    /// When a legacy path is provided, returns the popped element.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrpop/"/>
    public static async Task<ValkeyResult> ArrPopAsync(BaseClient client, ValkeyKey key, ValkeyValue path, long index = -1)
    {
        GlideString[] args = [JsonArrPop, ToGlideString(key), ToGlideString(path), index.ToString()];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Pops the last element from the array at the root path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The popped element.</returns>
    /// <seealso href="https://valkey.io/commands/json.arrpop/"/>
    public static async Task<ValkeyResult> ArrPopAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonArrPop, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    #endregion

    #region JSON.ARRTRIM

    /// <summary>
    /// Trims the array at the specified path to contain only elements within the specified range.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="start">The start index (inclusive).</param>
    /// <param name="end">The end index (inclusive). Negative indices count from the end.</param>
    /// <returns>
    /// When a JSONPath is provided, returns an array of new array lengths (or null for non-array matches).
    /// When a legacy path is provided, returns the new array length.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrtrim/"/>
    public static async Task<ValkeyResult> ArrTrimAsync(BaseClient client, ValkeyKey key, ValkeyValue path, long start, long end)
    {
        GlideString[] args = [JsonArrTrim, ToGlideString(key), ToGlideString(path), start.ToString(), end.ToString()];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    #endregion
}
