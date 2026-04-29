// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

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
    /// object? result = await GlideJson.ArrAppendAsync(client, "mykey", "$.arr", "4", "5");
    /// // result == [5] (new length of the array)
    /// </code>
    /// </example>
    public static async Task<object?> ArrAppendAsync(GlideClient client, string key, string path, params string[] values)
    {
        GlideString[] args = BuildArrAppendArgs(key, path, values.Select(v => (GlideString)v).ToArray());
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Appends one or more values to the array at the specified path in the JSON document stored at the key.
    /// </summary>
    public static async Task<object?> ArrAppendAsync(GlideClient client, GlideString key, GlideString path, params GlideString[] values)
    {
        GlideString[] args = BuildArrAppendArgs(key, path, values);
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Appends one or more values to the array at the specified path in the JSON document stored at the key (cluster client).
    /// </summary>
    public static async Task<object?> ArrAppendAsync(GlideClusterClient client, string key, string path, params string[] values)
    {
        GlideString[] args = BuildArrAppendArgs(key, path, values.Select(v => (GlideString)v).ToArray());
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Appends one or more values to the array at the specified path in the JSON document stored at the key (cluster client).
    /// </summary>
    public static async Task<object?> ArrAppendAsync(GlideClusterClient client, GlideString key, GlideString path, params GlideString[] values)
    {
        GlideString[] args = BuildArrAppendArgs(key, path, values);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    public static async Task<object?> ArrInsertAsync(GlideClient client, string key, string path, long index, params string[] values)
    {
        GlideString[] args = BuildArrInsertArgs(key, path, index, values.Select(v => (GlideString)v).ToArray());
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Inserts one or more values into the array at the specified path and index in the JSON document stored at the key.
    /// </summary>
    public static async Task<object?> ArrInsertAsync(GlideClient client, GlideString key, GlideString path, long index, params GlideString[] values)
    {
        GlideString[] args = BuildArrInsertArgs(key, path, index, values);
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Inserts one or more values into the array at the specified path and index (cluster client).
    /// </summary>
    public static async Task<object?> ArrInsertAsync(GlideClusterClient client, string key, string path, long index, params string[] values)
    {
        GlideString[] args = BuildArrInsertArgs(key, path, index, values.Select(v => (GlideString)v).ToArray());
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Inserts one or more values into the array at the specified path and index (cluster client).
    /// </summary>
    public static async Task<object?> ArrInsertAsync(GlideClusterClient client, GlideString key, GlideString path, long index, params GlideString[] values)
    {
        GlideString[] args = BuildArrInsertArgs(key, path, index, values);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    public static async Task<object?> ArrIndexAsync(GlideClient client, string key, string path, string value)
    {
        GlideString[] args = [JsonArrIndex, key, path, value];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array at the specified path with range options.
    /// </summary>
    public static async Task<object?> ArrIndexAsync(GlideClient client, string key, string path, string value, JsonArrIndexOptions options)
    {
        GlideString[] args = BuildArrIndexArgs(key, path, value, options);
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array at the specified path.
    /// </summary>
    public static async Task<object?> ArrIndexAsync(GlideClient client, GlideString key, GlideString path, GlideString value)
    {
        GlideString[] args = [JsonArrIndex, key, path, value];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array at the specified path with range options.
    /// </summary>
    public static async Task<object?> ArrIndexAsync(GlideClient client, GlideString key, GlideString path, GlideString value, JsonArrIndexOptions options)
    {
        GlideString[] args = BuildArrIndexArgs(key, path, value, options);
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array (cluster client).
    /// </summary>
    public static async Task<object?> ArrIndexAsync(GlideClusterClient client, string key, string path, string value)
    {
        GlideString[] args = [JsonArrIndex, key, path, value];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array with range options (cluster client).
    /// </summary>
    public static async Task<object?> ArrIndexAsync(GlideClusterClient client, string key, string path, string value, JsonArrIndexOptions options)
    {
        GlideString[] args = BuildArrIndexArgs(key, path, value, options);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array (cluster client).
    /// </summary>
    public static async Task<object?> ArrIndexAsync(GlideClusterClient client, GlideString key, GlideString path, GlideString value)
    {
        GlideString[] args = [JsonArrIndex, key, path, value];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Searches for the first occurrence of a value in the array with range options (cluster client).
    /// </summary>
    public static async Task<object?> ArrIndexAsync(GlideClusterClient client, GlideString key, GlideString path, GlideString value, JsonArrIndexOptions options)
    {
        GlideString[] args = BuildArrIndexArgs(key, path, value, options);
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    public static async Task<object?> ArrLenAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonArrLen, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the length of the array at the root path in the JSON document stored at the key.
    /// </summary>
    public static async Task<object?> ArrLenAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonArrLen, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the length of the array at the specified path in the JSON document stored at the key.
    /// </summary>
    public static async Task<object?> ArrLenAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonArrLen, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the length of the array at the root path in the JSON document stored at the key.
    /// </summary>
    public static async Task<object?> ArrLenAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonArrLen, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the length of the array at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ArrLenAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonArrLen, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the length of the array at the root path (cluster client).
    /// </summary>
    public static async Task<object?> ArrLenAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonArrLen, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the length of the array at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ArrLenAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonArrLen, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the length of the array at the root path (cluster client).
    /// </summary>
    public static async Task<object?> ArrLenAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonArrLen, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    public static async Task<object?> ArrPopAsync(GlideClient client, string key, string path, long index = -1)
    {
        GlideString[] args = [JsonArrPop, key, path, index.ToString()];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Pops the last element from the array at the root path in the JSON document stored at the key.
    /// </summary>
    public static async Task<object?> ArrPopAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonArrPop, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Pops an element from the array at the specified path and index.
    /// </summary>
    public static async Task<object?> ArrPopAsync(GlideClient client, GlideString key, GlideString path, long index = -1)
    {
        GlideString[] args = [JsonArrPop, key, path, index.ToString()];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Pops the last element from the array at the root path.
    /// </summary>
    public static async Task<object?> ArrPopAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonArrPop, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Pops an element from the array at the specified path and index (cluster client).
    /// </summary>
    public static async Task<object?> ArrPopAsync(GlideClusterClient client, string key, string path, long index = -1)
    {
        GlideString[] args = [JsonArrPop, key, path, index.ToString()];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Pops the last element from the array at the root path (cluster client).
    /// </summary>
    public static async Task<object?> ArrPopAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonArrPop, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Pops an element from the array at the specified path and index (cluster client).
    /// </summary>
    public static async Task<object?> ArrPopAsync(GlideClusterClient client, GlideString key, GlideString path, long index = -1)
    {
        GlideString[] args = [JsonArrPop, key, path, index.ToString()];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Pops the last element from the array at the root path (cluster client).
    /// </summary>
    public static async Task<object?> ArrPopAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonArrPop, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    public static async Task<object?> ArrTrimAsync(GlideClient client, string key, string path, long start, long end)
    {
        GlideString[] args = [JsonArrTrim, key, path, start.ToString(), end.ToString()];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Trims the array at the specified path to contain only elements within the specified range.
    /// </summary>
    public static async Task<object?> ArrTrimAsync(GlideClient client, GlideString key, GlideString path, long start, long end)
    {
        GlideString[] args = [JsonArrTrim, key, path, start.ToString(), end.ToString()];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Trims the array at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ArrTrimAsync(GlideClusterClient client, string key, string path, long start, long end)
    {
        GlideString[] args = [JsonArrTrim, key, path, start.ToString(), end.ToString()];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Trims the array at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ArrTrimAsync(GlideClusterClient client, GlideString key, GlideString path, long start, long end)
    {
        GlideString[] args = [JsonArrTrim, key, path, start.ToString(), end.ToString()];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    #endregion
}
