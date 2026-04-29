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
    /// When a JSONPath is provided, returns an array of key counts (or null for non-object matches).
    /// When a legacy path is provided, returns the key count.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.objlen/"/>
    public static async Task<object?> ObjLenAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonObjLen, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the number of keys in the object at the root path.
    /// </summary>
    public static async Task<object?> ObjLenAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonObjLen, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the number of keys in the object at the specified path.
    /// </summary>
    public static async Task<object?> ObjLenAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonObjLen, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the number of keys in the object at the root path.
    /// </summary>
    public static async Task<object?> ObjLenAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonObjLen, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the number of keys in the object at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ObjLenAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonObjLen, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the number of keys in the object at the root path (cluster client).
    /// </summary>
    public static async Task<object?> ObjLenAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonObjLen, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the number of keys in the object at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ObjLenAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonObjLen, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the number of keys in the object at the root path (cluster client).
    /// </summary>
    public static async Task<object?> ObjLenAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonObjLen, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    /// When a JSONPath is provided, returns an array of arrays of key names (or null for non-object matches).
    /// When a legacy path is provided, returns an array of key names.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.objkeys/"/>
    public static async Task<object?> ObjKeysAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonObjKeys, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the keys of the object at the root path.
    /// </summary>
    public static async Task<object?> ObjKeysAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonObjKeys, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the keys of the object at the specified path.
    /// </summary>
    public static async Task<object?> ObjKeysAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonObjKeys, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the keys of the object at the root path.
    /// </summary>
    public static async Task<object?> ObjKeysAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonObjKeys, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Gets the keys of the object at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ObjKeysAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonObjKeys, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the keys of the object at the root path (cluster client).
    /// </summary>
    public static async Task<object?> ObjKeysAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonObjKeys, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the keys of the object at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ObjKeysAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonObjKeys, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Gets the keys of the object at the root path (cluster client).
    /// </summary>
    public static async Task<object?> ObjKeysAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonObjKeys, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    /// When a JSONPath is provided, returns an array of toggled boolean values (or null for non-boolean matches).
    /// When a legacy path is provided, returns the toggled boolean value.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.toggle/"/>
    public static async Task<object?> ToggleAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonToggle, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Toggles the boolean value at the specified path.
    /// </summary>
    public static async Task<object?> ToggleAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonToggle, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Toggles the boolean value at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ToggleAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonToggle, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Toggles the boolean value at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> ToggleAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonToggle, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    /// When a JSONPath is provided, returns an array of memory sizes in bytes.
    /// When a legacy path is provided, returns the memory size in bytes.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.debug-memory/"/>
    public static async Task<object?> DebugMemoryAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonDebug, "MEMORY", key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the root path.
    /// </summary>
    public static async Task<object?> DebugMemoryAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonDebug, "MEMORY", key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the specified path.
    /// </summary>
    public static async Task<object?> DebugMemoryAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonDebug, "MEMORY", key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the root path.
    /// </summary>
    public static async Task<object?> DebugMemoryAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonDebug, "MEMORY", key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> DebugMemoryAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonDebug, "MEMORY", key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the root path (cluster client).
    /// </summary>
    public static async Task<object?> DebugMemoryAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonDebug, "MEMORY", key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> DebugMemoryAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonDebug, "MEMORY", key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the root path (cluster client).
    /// </summary>
    public static async Task<object?> DebugMemoryAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonDebug, "MEMORY", key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    /// When a JSONPath is provided, returns an array of field counts.
    /// When a legacy path is provided, returns the field count.
    /// </returns>
    /// <seealso href="https://valkey.io/commands/json.debug-fields/"/>
    public static async Task<object?> DebugFieldsAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonDebug, "FIELDS", key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the root path.
    /// </summary>
    public static async Task<object?> DebugFieldsAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonDebug, "FIELDS", key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the specified path.
    /// </summary>
    public static async Task<object?> DebugFieldsAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonDebug, "FIELDS", key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the root path.
    /// </summary>
    public static async Task<object?> DebugFieldsAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonDebug, "FIELDS", key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> DebugFieldsAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonDebug, "FIELDS", key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the root path (cluster client).
    /// </summary>
    public static async Task<object?> DebugFieldsAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonDebug, "FIELDS", key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the specified path (cluster client).
    /// </summary>
    public static async Task<object?> DebugFieldsAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonDebug, "FIELDS", key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the root path (cluster client).
    /// </summary>
    public static async Task<object?> DebugFieldsAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonDebug, "FIELDS", key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
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
    public static async Task<object?> RespAsync(GlideClient client, string key, string path)
    {
        GlideString[] args = [JsonResp, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Returns the JSON value at the root path in RESP format.
    /// </summary>
    public static async Task<object?> RespAsync(GlideClient client, string key)
    {
        GlideString[] args = [JsonResp, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Returns the JSON value at the specified path in RESP format.
    /// </summary>
    public static async Task<object?> RespAsync(GlideClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonResp, key, path];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Returns the JSON value at the root path in RESP format.
    /// </summary>
    public static async Task<object?> RespAsync(GlideClient client, GlideString key)
    {
        GlideString[] args = [JsonResp, key];
        object? result = await client.CustomCommand(args);
        return result;
    }

    /// <summary>
    /// Returns the JSON value at the specified path in RESP format (cluster client).
    /// </summary>
    public static async Task<object?> RespAsync(GlideClusterClient client, string key, string path)
    {
        GlideString[] args = [JsonResp, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Returns the JSON value at the root path in RESP format (cluster client).
    /// </summary>
    public static async Task<object?> RespAsync(GlideClusterClient client, string key)
    {
        GlideString[] args = [JsonResp, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Returns the JSON value at the specified path in RESP format (cluster client).
    /// </summary>
    public static async Task<object?> RespAsync(GlideClusterClient client, GlideString key, GlideString path)
    {
        GlideString[] args = [JsonResp, key, path];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    /// <summary>
    /// Returns the JSON value at the root path in RESP format (cluster client).
    /// </summary>
    public static async Task<object?> RespAsync(GlideClusterClient client, GlideString key)
    {
        GlideString[] args = [JsonResp, key];
        ClusterValue<object?> result = await client.CustomCommand(args);
        return result.SingleValue;
    }

    #endregion
}
