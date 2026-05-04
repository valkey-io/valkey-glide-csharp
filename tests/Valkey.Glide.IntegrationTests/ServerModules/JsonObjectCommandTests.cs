// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.IntegrationTests;
using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.ServerModules;

/// <summary>
/// Integration tests for JSON object, boolean, debug, and RESP commands via the GlideJson module.
/// These tests require a Valkey server with the JSON module enabled.
/// </summary>
[Collection("GlideTests")]
public class JsonObjectCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static string GetUniqueKey(string prefix = "json") => $"{prefix}:{Guid.NewGuid()}";

    #region JSON.OBJLEN Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ObjLenAsync_ValidObject_ReturnsKeyCount(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":1,\"b\":2,\"c\":3}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ObjLenAsync(client, key, "$");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(3L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ObjLenAsync_EmptyObject_ReturnsZero(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ObjLenAsync(client, key, "$");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ObjKeysAsync(client, key, "$");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"active\":true}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ToggleAsync(client, key, "$.active");

        Assert.NotNull(result);
        bool[] arr = (bool[])result!;
        _ = Assert.Single(arr);
        Assert.False(arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ToggleAsync_FalseToTrue_ReturnsTrue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"active\":false}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ToggleAsync(client, key, "$.active");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.DebugMemoryAsync(client, key);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.DebugFieldsAsync(client, key);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.RespAsync(client, key);

        Assert.NotNull(result);
        // RESP format returns an array representation
        ValkeyResult[] arr = (ValkeyResult[])result!;
        Assert.NotNull(arr);
    }

    #endregion
}
