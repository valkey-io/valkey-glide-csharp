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
    public static async Task<ValkeyResult> ObjLenAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonObjLen, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Gets the number of keys in the object at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The key count at the root path.</returns>
    /// <seealso href="https://valkey.io/commands/json.objlen/"/>
    public static async Task<ValkeyResult> ObjLenAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonObjLen, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
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
    public static async Task<ValkeyResult> ObjKeysAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonObjKeys, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Gets the keys of the object at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>An array of key names at the root path.</returns>
    /// <seealso href="https://valkey.io/commands/json.objkeys/"/>
    public static async Task<ValkeyResult> ObjKeysAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonObjKeys, ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
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
    public static async Task<ValkeyResult> ToggleAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonToggle, ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
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
    public static async Task<ValkeyResult> DebugMemoryAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonDebug, "MEMORY", ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Reports the memory usage in bytes of the JSON value at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The memory size in bytes at the root path.</returns>
    /// <seealso href="https://valkey.io/commands/json.debug-memory/"/>
    public static async Task<ValkeyResult> DebugMemoryAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonDebug, "MEMORY", ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
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
    public static async Task<ValkeyResult> DebugFieldsAsync(BaseClient client, ValkeyKey key, ValkeyValue path)
    {
        GlideString[] args = [JsonDebug, "FIELDS", ToGlideString(key), ToGlideString(path)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
    }

    /// <summary>
    /// Reports the number of fields in the JSON value at the root path.
    /// </summary>
    /// <param name="client">The Glide client to use for the command.</param>
    /// <param name="key">The key where the JSON document is stored.</param>
    /// <returns>The field count at the root path.</returns>
    /// <seealso href="https://valkey.io/commands/json.debug-fields/"/>
    public static async Task<ValkeyResult> DebugFieldsAsync(BaseClient client, ValkeyKey key)
    {
        GlideString[] args = [JsonDebug, "FIELDS", ToGlideString(key)];
        object? result = await ExecuteCommandAsync(client, args);
        return ValkeyResult.Create(result);
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
