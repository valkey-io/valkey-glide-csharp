// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for JSON array commands via the GlideJson module.
/// These tests require a Valkey server with the JSON module enabled.
/// </summary>
[Collection("GlideTests")]
public class JsonArrayCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static string GetUniqueKey(string prefix = "json") => $"{prefix}:{Guid.NewGuid()}";

    #region JSON.ARRAPPEND Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrAppendAsync_BasicAppend_ReturnsNewLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrAppendAsync(client, key, "$.arr", "4", "5");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(5L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrAppendAsync_MultipleArrays_ReturnsArrayOfLengths(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":[1],\"b\":[2,3]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrAppendAsync(client, key, "$.*", "99");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        Assert.Equal(2, arr.Length);
    }

    #endregion

    #region JSON.ARRINSERT Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrInsertAsync_InsertAtIndex_ReturnsNewLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrInsertAsync(client, key, "$.arr", 1, "99");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(4L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrInsertAsync_NegativeIndex_InsertsFromEnd(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrInsertAsync(client, key, "$.arr", -1, "99");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(4L, arr[0]);
    }

    #endregion

    #region JSON.ARRINDEX Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrIndexAsync_ValueExists_ReturnsIndex(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3,4,5]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrIndexAsync(client, key, "$.arr", "3");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(2L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrIndexAsync_ValueNotFound_ReturnsMinusOne(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrIndexAsync(client, key, "$.arr", "99");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(-1L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrIndexAsync_WithRangeOptions_SearchesInRange(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3,2,5]}";
        var options = GlideJson.ArrIndexOptions.FromStart(2);

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrIndexAsync(client, key, "$.arr", "2", options);

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(3L, arr[0]);
    }

    #endregion

    #region JSON.ARRLEN Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrLenAsync_ValidArray_ReturnsLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3,4,5]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrLenAsync(client, key, "$.arr");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(5L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrLenAsync_EmptyArray_ReturnsZero(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrLenAsync(client, key, "$.arr");

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(0L, arr[0]);
    }

    #endregion

    #region JSON.ARRPOP Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrPopAsync_PopLastElement_ReturnsElement(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrPopAsync(client, key, "$.arr");

        Assert.NotNull(result);
        ValkeyResult[] arr = (ValkeyResult[])result!;
        _ = Assert.Single(arr);
        Assert.Equal("3", (string?)arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrPopAsync_PopAtIndex_ReturnsElement(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrPopAsync(client, key, "$.arr", 0);

        Assert.NotNull(result);
        ValkeyResult[] arr = (ValkeyResult[])result!;
        _ = Assert.Single(arr);
        Assert.Equal("1", (string?)arr[0]);
    }

    #endregion

    #region JSON.ARRTRIM Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrTrimAsync_TrimArray_ReturnsNewLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3,4,5]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrTrimAsync(client, key, "$.arr", 1, 3);

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(3L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrTrimAsync_TrimToEmpty_ReturnsZero(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyResult result = await GlideJson.ArrTrimAsync(client, key, "$.arr", 5, 10);

        Assert.NotNull(result);
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(0L, arr[0]);
    }

    #endregion
}
