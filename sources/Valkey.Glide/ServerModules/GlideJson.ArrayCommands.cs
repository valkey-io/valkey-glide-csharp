// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
    /// An array of new array lengths for each matching path. Returns <see langword="null"/> for non-array matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrappend/">Valkey commands – JSON.ARRAPPEND</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"arr\":[1,2,3]}");
    /// var lengths = await GlideJson.ArrAppendAsync(client, "mykey", "$.arr", ["4", "5"]);  // [5]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> ArrAppendAsync(BaseClient client, ValkeyKey key, ValkeyValue path, IEnumerable<ValkeyValue> values)
    {
        GlideString[] args = BuildArrAppendArgs(ToGlideString(key), ToGlideString(path), [.. values.Select(v => ToGlideString(v))]);
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
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
    /// An array of new array lengths for each matching path. Returns <see langword="null"/> for non-array matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrinsert/">Valkey commands – JSON.ARRINSERT</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"arr\":[1,3]}");
    /// var lengths = await GlideJson.ArrInsertAsync(client, "mykey", "$.arr", 1, ["2"]);  // [3]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> ArrInsertAsync(BaseClient client, ValkeyKey key, ValkeyValue path, long index, IEnumerable<ValkeyValue> values)
    {
        GlideString[] args = BuildArrInsertArgs(ToGlideString(key), ToGlideString(path), index, [.. values.Select(v => ToGlideString(v))]);
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
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
    /// An array of indices for each matching path. Returns -1 if not found, <see langword="null"/> for non-array matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrindex/">Valkey commands – JSON.ARRINDEX</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"arr\":[1,2,3,2]}");
    /// var indices = await GlideJson.ArrIndexAsync(client, "mykey", "$.arr", "2");  // [1]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> ArrIndexAsync(BaseClient client, ValkeyKey key, ValkeyValue path, ValkeyValue value)
    {
        GlideString[] args = [JsonArrIndex, ToGlideString(key), ToGlideString(path), ToGlideString(value)];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array at the specified path with range options.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <param name="value">The JSON value to search for.</param>
    /// <param name="range">Range options for the search.</param>
    /// <returns>
    /// An array of indices for each matching path. Returns -1 if not found, <see langword="null"/> for non-array matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrindex/">Valkey commands – JSON.ARRINDEX</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"arr\":[1,2,3,2]}");
    /// var range = GlideJson.ArrIndexRange.Between(2, 4);
    /// var indices = await GlideJson.ArrIndexAsync(client, "mykey", "$.arr", "2", range);  // [3]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> ArrIndexAsync(BaseClient client, ValkeyKey key, ValkeyValue path, ValkeyValue value, ArrIndexRange range)
    {
        GlideString[] args = BuildArrIndexArgs(ToGlideString(key), ToGlideString(path), ToGlideString(value), range);
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
    }

    private static GlideString[] BuildArrIndexArgs(GlideString key, GlideString path, GlideString value, ArrIndexRange range)
    {
        List<GlideString> args = [JsonArrIndex, key, path, value];
        args.AddRange(range.ToArgs());
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
    /// An array of lengths for each matching path. Returns <see langword="null"/> for non-array matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrlen/">Valkey commands – JSON.ARRLEN</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"arr\":[1,2,3]}");
    /// var lengths = await GlideJson.ArrLenAsync(client, "mykey", "$.arr");  // [3]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> ArrLenAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonArrLen, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
    }

    /// <summary>
    /// Gets the length of the array at the root path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The array length at the root path, or <see langword="null"/> if the key does not exist or root is not an array.</returns>
    /// <seealso href="https://valkey.io/commands/json.arrlen/">Valkey commands – JSON.ARRLEN</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "[1,2,3]");
    /// var length = await GlideJson.ArrLenAsync(client, "mykey");  // 3
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?> ArrLenAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonArrLen, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return result is null ? null : (long)result;
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
    /// An array of popped elements (as JSON strings) for each matching path. Returns <see langword="null"/> for non-array/empty matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrpop/">Valkey commands – JSON.ARRPOP</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"arr\":[1,2,3]}");
    /// var popped = await GlideJson.ArrPopAsync(client, "mykey", "$.arr", -1);  // ["3"]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<ValkeyValue?[]?> ArrPopAsync(BaseClient client, ValkeyKey key, ValkeyValue path, long index = -1)
    {
        GlideString[] args = [JsonArrPop, ToGlideString(key), ToGlideString(path), index.ToString()];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableValkeyValueArray(result);
    }

    private static ValkeyValue?[]? ConvertToNullableValkeyValueArray(object? result)
    {
        if (result is null)
            return null;
        if (result is object?[] arr)
            return [.. arr.Select(o => o is null ? (ValkeyValue?)null : ToValkeyValue(o))];
        // Single value (legacy path) - wrap in array for consistent return type
        return [ToValkeyValue(result)];
    }

    /// <summary>
    /// Pops the last element from the array at the root path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The popped element as a JSON string, or <see cref="ValkeyValue.Null"/> if the array is empty or key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.arrpop/">Valkey commands – JSON.ARRPOP</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "[1,2,3]");
    /// var popped = await GlideJson.ArrPopAsync(client, "mykey");  // "3"
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<ValkeyValue> ArrPopAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonArrPop, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ToValkeyValue(result);
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
    /// An array of new array lengths for each matching path. Returns <see langword="null"/> for non-array matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.arrtrim/">Valkey commands – JSON.ARRTRIM</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"arr\":[1,2,3,4,5]}");
    /// var lengths = await GlideJson.ArrTrimAsync(client, "mykey", "$.arr", 1, 3);  // [3]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> ArrTrimAsync(BaseClient client, ValkeyKey key, ValkeyValue path, long start, long end)
    {
        GlideString[] args = [JsonArrTrim, ToGlideString(key), ToGlideString(path), start.ToString(), end.ToString()];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
    }

    #endregion
}
