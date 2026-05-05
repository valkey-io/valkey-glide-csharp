// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.ServerModules;

/// <summary>
/// Module for JSON commands - Object, Debug, and RESP operations.
/// </summary>
public static partial class GlideJson
{
    #region JSON.OBJLEN

    /// <summary>
    /// Gets the number of keys in the object at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// An array of key counts for each matching path, or <see langword="null"/> if the key does not exist.
    /// Elements are <see langword="null"/> for paths where the value is not an object.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.objlen/">Valkey commands – JSON.OBJLEN</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"obj\":{\"a\":1,\"b\":2},\"arr\":[1,2]}");
    /// var counts = await GlideJson.ObjLenAsync(client, "mykey", "$.*");
    /// // counts = [2, null] - obj has 2 keys, arr is not an object (null)
    ///
    /// var missing = await GlideJson.ObjLenAsync(client, "nonexistent", "$.*");
    /// // missing = null - key doesn't exist
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> ObjLenAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonObjLen, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
    }

    /// <summary>
    /// Gets the number of keys in the object at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The key count at the root path, or <see langword="null"/> if the key does not exist or root is not an object.</returns>
    /// <seealso href="https://valkey.io/commands/json.objlen/">Valkey commands – JSON.OBJLEN</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":1,\"b\":2}");
    /// var count = await GlideJson.ObjLenAsync(client, "mykey");  // 2
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?> ObjLenAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonObjLen, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return result is null ? null : (long)result;
    }

    #endregion

    #region JSON.OBJKEYS

    /// <summary>
    /// Gets the keys of the object at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// An array of arrays of key names for each matching path. Returns <see langword="null"/> for non-object matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.objkeys/">Valkey commands – JSON.OBJKEYS</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":{\"x\":1},\"b\":{\"y\":2}}");
    /// var keys = await GlideJson.ObjKeysAsync(client, "mykey", "$.*");  // [["x"], ["y"]]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<ValkeyValue[]?[]?> ObjKeysAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonObjKeys, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNestedValkeyValueArray(result);
    }

    private static ValkeyValue[]?[]? ConvertToNestedValkeyValueArray(object? result)
    {
        if (result is null)
            return null;
        if (result is object?[] arr)
        {
            return [.. arr.Select<object?, ValkeyValue[]?>(o =>
            {
                if (o is null)
                    return null;
                if (o is object?[] innerArr)
                    return [.. innerArr.Select(ToValkeyValue)];
                // Single value - wrap in array
                return [ToValkeyValue(o)];
            })];
        }
        // Single value (legacy path) - wrap in nested array
        return [[ToValkeyValue(result)]];
    }

    /// <summary>
    /// Gets the keys of the object at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>An array of key names at the root path, or <see langword="null"/> if the key does not exist or root is not an object.</returns>
    /// <seealso href="https://valkey.io/commands/json.objkeys/">Valkey commands – JSON.OBJKEYS</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":1,\"b\":2}");
    /// var keys = await GlideJson.ObjKeysAsync(client, "mykey");  // ["a", "b"]
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<string[]?> ObjKeysAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonObjKeys, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        if (result is null)
            return null;
        if (result is object?[] arr)
            return [.. arr.Select(o => o?.ToString() ?? string.Empty)];
        return [];
    }

    #endregion

    #region JSON.DEBUG MEMORY

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the specified path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// An array of memory sizes in bytes for each matching path, or <see langword="null"/> if the key does not exist.
    /// Elements are <see langword="null"/> for paths that don't exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.debug/">Valkey commands – JSON.DEBUG</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":\"hello\",\"b\":123}");
    /// var memory = await GlideJson.DebugMemoryAsync(client, "mykey", "$.*");
    /// // memory = [size1, size2] - memory usage for each matched path
    ///
    /// var missing = await GlideJson.DebugMemoryAsync(client, "nonexistent", "$.*");
    /// // missing = null - key doesn't exist
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> DebugMemoryAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonDebug, ValkeyLiterals.MEMORY, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The memory size in bytes at the root path, or <see langword="null"/> if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.debug/">Valkey commands – JSON.DEBUG</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":1}");
    /// var memory = await GlideJson.DebugMemoryAsync(client, "mykey");
    /// Console.WriteLine($"Memory usage: {memory} bytes");
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?> DebugMemoryAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonDebug, ValkeyLiterals.MEMORY, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return result is null ? null : (long)result;
    }

    #endregion

    #region JSON.DEBUG FIELDS

    /// <summary>
    /// Reports the number of fields in the JSON value at the specified path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// An array of field counts for each matching path, or <see langword="null"/> if the key does not exist.
    /// Elements are <see langword="null"/> for paths that don't exist.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.debug/">Valkey commands – JSON.DEBUG</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":{\"x\":1,\"y\":2},\"b\":123}");
    /// var fields = await GlideJson.DebugFieldsAsync(client, "mykey", "$.*");
    /// // fields = [2, 1] - a has 2 fields, b has 1 field
    ///
    /// var missing = await GlideJson.DebugFieldsAsync(client, "nonexistent", "$.*");
    /// // missing = null - key doesn't exist
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?[]?> DebugFieldsAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonDebug, ValkeyLiterals.FIELDS, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableLongArray(result);
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The field count at the root path, or <see langword="null"/> if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.debug/">Valkey commands – JSON.DEBUG</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":1,\"b\":2}");
    /// var fields = await GlideJson.DebugFieldsAsync(client, "mykey");  // 2
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<long?> DebugFieldsAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonDebug, ValkeyLiterals.FIELDS, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return result is null ? null : (long)result;
    }

    #endregion

    #region JSON.RESP

    /// <summary>
    /// Returns the JSON value at the specified path in RESP format.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>The JSON value in RESP format.</returns>
    /// <seealso href="https://valkey.io/commands/json.resp/">Valkey commands – JSON.RESP</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":1}");
    /// var resp = await GlideJson.RespAsync(client, "mykey", "$");
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<ValkeyResult> RespAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonResp, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Returns the JSON value at the root path in RESP format.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The JSON value in RESP format at the root path.</returns>
    /// <seealso href="https://valkey.io/commands/json.resp/">Valkey commands – JSON.RESP</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await GlideJson.SetAsync(client, "mykey", "$", "{\"a\":1}");
    /// var resp = await GlideJson.RespAsync(client, "mykey");
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<ValkeyResult> RespAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonResp, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    #endregion
}
