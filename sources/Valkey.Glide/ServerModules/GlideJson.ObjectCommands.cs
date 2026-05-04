// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.ServerModules;

/// <summary>
/// Module for JSON commands - Object, Boolean, Debug, and RESP operations.
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
    /// An array of key counts for each matching path. Returns null for non-object matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.objlen/"/>
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
    /// <returns>The key count at the root path, or null if the key does not exist or root is not an object.</returns>
    /// <seealso href="https://valkey.io/commands/json.objlen/"/>
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
    /// An array of arrays of key names for each matching path. Returns null for non-object matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.objkeys/"/>
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
            return arr.Select(o =>
            {
                if (o is null)
                    return null;
                if (o is object?[] innerArr)
                    return innerArr.Select(ToValkeyValue).ToArray();
                // Single value - wrap in array
                return new ValkeyValue[] { ToValkeyValue(o) };
            }).ToArray();
        }
        // Single value (legacy path) - wrap in nested array
        return [[ToValkeyValue(result)]];
    }

    /// <summary>
    /// Gets the keys of the object at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>An array of key names at the root path, or null if the key does not exist or root is not an object.</returns>
    /// <seealso href="https://valkey.io/commands/json.objkeys/"/>
    public static async Task<string[]?> ObjKeysAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonObjKeys, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        if (result is null)
            return null;
        if (result is object?[] arr)
            return arr.Select(o => o?.ToString() ?? string.Empty).ToArray();
        return [];
    }

    #endregion

    #region JSON.TOGGLE

    /// <summary>
    /// Toggles the boolean value at the specified path in the JSON document stored at the key.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <param name="path">The JSONPath or legacy path within the JSON document.</param>
    /// <returns>
    /// An array of toggled boolean values for each matching path. Returns null for non-boolean matches.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.toggle/"/>
    public static async Task<bool?[]?> ToggleAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonToggle, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ConvertToNullableBoolArray(result);
    }

    private static bool?[]? ConvertToNullableBoolArray(object? result)
    {
        if (result is null)
            return null;
        if (result is object?[] arr)
            return arr.Select(o => o is null ? (bool?)null : Convert.ToBoolean(o)).ToArray();
        // Single value (legacy path) - wrap in array for consistent return type
        return [Convert.ToBoolean(result)];
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
    /// An array of memory sizes in bytes for each matching path.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.debug/"/>
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
    /// <returns>The memory size in bytes at the root path, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.debug/"/>
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
    /// An array of field counts for each matching path.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.debug/"/>
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
    /// <returns>The field count at the root path, or null if the key does not exist.</returns>
    /// <seealso href="https://valkey.io/commands/json.debug/"/>
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
    /// <seealso href="https://valkey.io/commands/json.resp/"/>
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
    /// <seealso href="https://valkey.io/commands/json.resp/"/>
    public static async Task<ValkeyResult> RespAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonResp, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    #endregion
}
