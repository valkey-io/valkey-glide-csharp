// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for JSON object, boolean, debug, and RESP commands via the GlideJson module.
/// These tests require a Valkey server with the JSON module enabled.
/// </summary>
[Collection("GlideTests")]
public class JsonObjectCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static async Task<bool> IsJsonModuleAvailable(BaseClient client)
    {
        try
        {
            string testKey = $"__json_module_check_{Guid.NewGuid()}";
            GlideString[] args = ["JSON.SET", testKey, "$", "{}"];
            if (client is GlideClient standaloneClient)
            {
                _ = await standaloneClient.CustomCommand(args);
            }
            else if (client is GlideClusterClient clusterClient)
            {
                _ = await clusterClient.CustomCommand(args);
            }
            _ = await client.DeleteAsync(testKey);
            return true;
        }
        catch (Errors.RequestException)
        {
            return false;
        }
    }

    private static async Task SkipIfJsonModuleNotAvailable(BaseClient client)
    {
        bool isAvailable = await IsJsonModuleAvailable(client);
        Assert.SkipWhen(!isAvailable, "JSON module is not available on the server");
    }

    private static string GetUniqueKey(string prefix = "json") => $"{prefix}:{Guid.NewGuid()}";

    #region JSON.OBJLEN Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ObjLenAsync_ValidObject_ReturnsKeyCount(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":1,\"b\":2,\"c\":3}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ObjLenAsync(standaloneClient, key, "$");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ObjLenAsync(clusterClient, key, "$");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(3L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ObjLenAsync_EmptyObject_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ObjLenAsync(standaloneClient, key, "$");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ObjLenAsync(clusterClient, key, "$");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(0L, arr[0]);
    }

    #endregion

    #region JSON.OBJKEYS Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ObjKeysAsync_ValidObject_ReturnsKeys(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ObjKeysAsync(standaloneClient, key, "$");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ObjKeysAsync(clusterClient, key, "$");
        }

        Assert.NotNull(result);
        // JSONPath returns array of arrays
        ValkeyResult[] outerArr = (ValkeyResult[])result!;
        _ = Assert.Single(outerArr);
        ValkeyResult[] keys = (ValkeyResult[])outerArr[0]!;
        Assert.Equal(2, keys.Length);
    }

    #endregion

    #region JSON.TOGGLE Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ToggleAsync_TrueToFalse_ReturnsFalse(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"active\":true}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ToggleAsync(standaloneClient, key, "$.active");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ToggleAsync(clusterClient, key, "$.active");
        }

        Assert.NotNull(result);
        bool[] arr = (bool[])result!;
        _ = Assert.Single(arr);
        Assert.False(arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ToggleAsync_FalseToTrue_ReturnsTrue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"active\":false}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ToggleAsync(standaloneClient, key, "$.active");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ToggleAsync(clusterClient, key, "$.active");
        }

        Assert.NotNull(result);
        bool[] arr = (bool[])result!;
        _ = Assert.Single(arr);
        Assert.True(arr[0]);
    }

    #endregion

    #region JSON.DEBUG MEMORY Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DebugMemoryAsync_ValidDocument_ReturnsMemorySize(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.DebugMemoryAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.DebugMemoryAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        long memorySize = (long)result;
        Assert.True(memorySize > 0);
    }

    #endregion

    #region JSON.DEBUG FIELDS Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DebugFieldsAsync_ValidDocument_ReturnsFieldCount(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.DebugFieldsAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.DebugFieldsAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        long fieldCount = (long)result;
        Assert.True(fieldCount > 0);
    }

    #endregion

    #region JSON.RESP Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task RespAsync_ValidDocument_ReturnsRespFormat(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.RespAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.RespAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        // RESP format returns an array representation
        ValkeyResult[] arr = (ValkeyResult[])result!;
        Assert.NotNull(arr);
    }

    #endregion
}
