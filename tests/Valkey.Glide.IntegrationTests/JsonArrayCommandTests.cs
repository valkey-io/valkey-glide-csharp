// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ServerModules;
using Valkey.Glide.ServerModules.Options;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for JSON array commands via the GlideJson module.
/// These tests require a Valkey server with the JSON module enabled.
/// </summary>
[Collection("GlideTests")]
public class JsonArrayCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    /// <summary>
    /// Checks if the JSON module is available on the server.
    /// </summary>
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

    #region JSON.ARRAPPEND Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrAppendAsync_BasicAppend_ReturnsNewLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrAppendAsync(standaloneClient, key, "$.arr", "4", "5");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrAppendAsync(clusterClient, key, "$.arr", "4", "5");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(5L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrAppendAsync_MultipleArrays_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":[1],\"b\":[2,3]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrAppendAsync(standaloneClient, key, "$.*", "99");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrAppendAsync(clusterClient, key, "$.*", "99");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Equal(2, arr.Length);
    }

    #endregion

    #region JSON.ARRINSERT Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrInsertAsync_InsertAtIndex_ReturnsNewLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrInsertAsync(standaloneClient, key, "$.arr", 1, "99");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrInsertAsync(clusterClient, key, "$.arr", 1, "99");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(4L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrInsertAsync_NegativeIndex_InsertsFromEnd(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrInsertAsync(standaloneClient, key, "$.arr", -1, "99");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrInsertAsync(clusterClient, key, "$.arr", -1, "99");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(4L, arr[0]);
    }

    #endregion

    #region JSON.ARRINDEX Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrIndexAsync_ValueExists_ReturnsIndex(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3,4,5]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrIndexAsync(standaloneClient, key, "$.arr", "3");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrIndexAsync(clusterClient, key, "$.arr", "3");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(2L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrIndexAsync_ValueNotFound_ReturnsMinusOne(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrIndexAsync(standaloneClient, key, "$.arr", "99");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrIndexAsync(clusterClient, key, "$.arr", "99");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(-1L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrIndexAsync_WithRangeOptions_SearchesInRange(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3,2,5]}";
        var options = JsonArrIndexOptions.FromStart(2);

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrIndexAsync(standaloneClient, key, "$.arr", "2", options);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrIndexAsync(clusterClient, key, "$.arr", "2", options);
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(3L, arr[0]);
    }

    #endregion

    #region JSON.ARRLEN Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrLenAsync_ValidArray_ReturnsLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3,4,5]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrLenAsync(standaloneClient, key, "$.arr");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrLenAsync(clusterClient, key, "$.arr");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(5L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrLenAsync_EmptyArray_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrLenAsync(standaloneClient, key, "$.arr");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrLenAsync(clusterClient, key, "$.arr");
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(0L, arr[0]);
    }

    #endregion

    #region JSON.ARRPOP Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrPopAsync_PopLastElement_ReturnsElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrPopAsync(standaloneClient, key, "$.arr");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrPopAsync(clusterClient, key, "$.arr");
        }

        Assert.NotNull(result);
        ValkeyResult[] arr = (ValkeyResult[])result;
        Assert.Single(arr);
        Assert.Equal("3", (string?)arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrPopAsync_PopAtIndex_ReturnsElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrPopAsync(standaloneClient, key, "$.arr", 0);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrPopAsync(clusterClient, key, "$.arr", 0);
        }

        Assert.NotNull(result);
        ValkeyResult[] arr = (ValkeyResult[])result;
        Assert.Single(arr);
        Assert.Equal("1", (string?)arr[0]);
    }

    #endregion

    #region JSON.ARRTRIM Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrTrimAsync_TrimArray_ReturnsNewLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3,4,5]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrTrimAsync(standaloneClient, key, "$.arr", 1, 3);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrTrimAsync(clusterClient, key, "$.arr", 1, 3);
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(3L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ArrTrimAsync_TrimToEmpty_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"arr\":[1,2,3]}";

        ValkeyResult result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.ArrTrimAsync(standaloneClient, key, "$.arr", 5, 10);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.ArrTrimAsync(clusterClient, key, "$.arr", 5, 10);
        }

        Assert.NotNull(result);
        long[] arr = (long[])result;
        Assert.Single(arr);
        Assert.Equal(0L, arr[0]);
    }

    #endregion
}
