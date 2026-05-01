// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Pipeline;
using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for JSON batch commands via the GlideJsonBatch extension methods.
/// These tests require a Valkey server with the JSON module enabled.
/// </summary>
[Collection("GlideTests")]
public class JsonBatchTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    /// <summary>
    /// Checks if the JSON module is available on the server.
    /// </summary>
    private static async Task<bool> IsJsonModuleAvailable(GlideClient client)
    {
        try
        {
            string testKey = $"__json_batch_module_check_{Guid.NewGuid()}";
            GlideString[] args = ["JSON.SET", testKey, "$", "{}"];
            _ = await client.CustomCommand(args);
            _ = await client.DeleteAsync(testKey);
            return true;
        }
        catch (Errors.RequestException)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if the JSON module is available on the cluster server.
    /// </summary>
    private static async Task<bool> IsJsonModuleAvailable(GlideClusterClient client)
    {
        try
        {
            string testKey = $"__json_batch_module_check_{Guid.NewGuid()}";
            GlideString[] args = ["JSON.SET", testKey, "$", "{}"];
            _ = await client.CustomCommand(args);
            _ = await client.DeleteAsync(testKey);
            return true;
        }
        catch (Errors.RequestException)
        {
            return false;
        }
    }

    private static string GetUniqueKey(string prefix = "json_batch") => $"{prefix}:{Guid.NewGuid()}";

    #region Standalone Batch Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_SetAndGet_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        try
        {
            Batch batch = new(false);
            _ = batch.JsonSet(key, "$", jsonValue);
            _ = batch.JsonGet(key);
            _ = batch.JsonGet(key, "$.name");
            _ = batch.JsonType(key);
            _ = batch.JsonType(key, "$.age");

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(5, results.Length);
            Assert.Equal("OK", results[0]?.ToString());
            Assert.NotNull(results[1]);
            Assert.NotNull(results[2]);
            Assert.NotNull(results[3]);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_NumericOperations_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();
        string jsonValue = "{\"counter\":10,\"multiplier\":5}";

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

            Batch batch = new(false);
            _ = batch.JsonNumIncrBy(key, "$.counter", 5);
            _ = batch.JsonNumMultBy(key, "$.multiplier", 2);
            _ = batch.JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(3, results.Length);
            Assert.NotNull(results[0]);
            Assert.NotNull(results[1]);
            Assert.NotNull(results[2]);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_ArrayOperations_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[1,2,3]}";

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

            Batch batch = new(false);
            _ = batch.JsonArrAppend(key, "$.items", "4", "5");
            _ = batch.JsonArrLen(key, "$.items");
            _ = batch.JsonArrIndex(key, "$.items", "3");
            _ = batch.JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_ObjectOperations_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":1,\"b\":2,\"c\":3}";

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

            Batch batch = new(false);
            _ = batch.JsonObjLen(key);
            _ = batch.JsonObjLen(key, "$");
            _ = batch.JsonObjKeys(key);
            _ = batch.JsonObjKeys(key, "$");

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_StringOperations_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();
        string jsonValue = "{\"greeting\":\"Hello\"}";

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

            Batch batch = new(false);
            _ = batch.JsonStrLen(key, "$.greeting");
            _ = batch.JsonStrAppend(key, "$.greeting", "\" World\"");
            _ = batch.JsonStrLen(key, "$.greeting");
            _ = batch.JsonGet(key, "$.greeting");

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_DeleteAndClear_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key1 = GetUniqueKey("del1");
        string key2 = GetUniqueKey("del2");

        try
        {
            _ = await GlideJson.SetAsync(client, key1, "$", "{\"a\":1,\"b\":{\"c\":2}}");
            _ = await GlideJson.SetAsync(client, key2, "$", "{\"arr\":[1,2,3],\"num\":42}");

            Batch batch = new(false);
            _ = batch.JsonDel(key1, "$.b");
            _ = batch.JsonClear(key2, "$.arr");
            _ = batch.JsonGet(key1);
            _ = batch.JsonGet(key2);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync([key1, key2]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_Toggle_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();
        string jsonValue = "{\"active\":true,\"enabled\":false}";

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

            Batch batch = new(false);
            _ = batch.JsonToggle(key, "$.active");
            _ = batch.JsonToggle(key, "$.enabled");
            _ = batch.JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(3, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_DebugOperations_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":1,\"b\":{\"c\":2,\"d\":3}}";

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

            Batch batch = new(false);
            _ = batch.JsonDebugMemory(key);
            _ = batch.JsonDebugMemory(key, "$");
            _ = batch.JsonDebugFields(key);
            _ = batch.JsonDebugFields(key, "$");

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_MGet_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key1 = GetUniqueKey("mget1");
        string key2 = GetUniqueKey("mget2");
        string key3 = GetUniqueKey("mget3");

        try
        {
            _ = await GlideJson.SetAsync(client, key1, "$", "{\"value\":1}");
            _ = await GlideJson.SetAsync(client, key2, "$", "{\"value\":2}");
            _ = await GlideJson.SetAsync(client, key3, "$", "{\"value\":3}");

            Batch batch = new(false);
            _ = batch.JsonMGet([(GlideString)key1, key2, key3], "$.value");

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            _ = Assert.Single(results);
            Assert.NotNull(results[0]);
        }
        finally
        {
            _ = await client.DeleteAsync([key1, key2, key3]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_SetWithCondition_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();

        try
        {
            Batch batch = new(false);
            _ = batch.JsonSet(key, "$", "{\"a\":1}", GlideJson.SetCondition.OnlyIfDoesNotExist);
            _ = batch.JsonSet(key, "$.a", "2", GlideJson.SetCondition.OnlyIfExists);
            _ = batch.JsonSet(key, "$", "{\"b\":1}", GlideJson.SetCondition.OnlyIfDoesNotExist);
            _ = batch.JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
            Assert.Equal("OK", results[0]?.ToString());
            Assert.Equal("OK", results[1]?.ToString());
            Assert.Null(results[2]);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_ArrInsertAndTrim_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", "{\"arr\":[1,2,3,4,5]}");

            Batch batch = new(false);
            _ = batch.JsonArrInsert(key, "$.arr", 2, "\"inserted\"");
            _ = batch.JsonArrLen(key, "$.arr");
            _ = batch.JsonArrTrim(key, "$.arr", 0, 3);
            _ = batch.JsonArrLen(key, "$.arr");
            _ = batch.JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(5, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_ArrPop_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", "{\"arr\":[1,2,3,4,5]}");

            Batch batch = new(false);
            _ = batch.JsonArrPop(key, "$.arr");
            _ = batch.JsonArrPop(key, "$.arr", 0);
            _ = batch.JsonArrLen(key, "$.arr");
            _ = batch.JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_Resp_ReturnsExpectedResults(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", "{\"a\":1,\"b\":\"hello\"}");

            Batch batch = new(false);
            _ = batch.JsonResp(key);
            _ = batch.JsonResp(key, "$");

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(2, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_Chaining_WorksCorrectly(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();

        try
        {
            Batch batch = new Batch(false)
                .JsonSet(key, "$", "{\"x\":1}")
                .JsonGet(key)
                .JsonNumIncrBy(key, "$.x", 10)
                .JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
            Assert.Equal("OK", results[0]?.ToString());
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_AtomicTransaction_WorksCorrectly(GlideClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();

        try
        {
            Batch batch = new(true);
            _ = batch.JsonSet(key, "$", "{\"counter\":0}");
            _ = batch.JsonNumIncrBy(key, "$.counter", 1);
            _ = batch.JsonNumIncrBy(key, "$.counter", 1);
            _ = batch.JsonNumIncrBy(key, "$.counter", 1);
            _ = batch.JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(5, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    #endregion

    #region Cluster Batch Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonClusterBatch_SetAndGet_ReturnsExpectedResults(GlideClusterClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        try
        {
            ClusterBatch batch = new(false);
            _ = batch.JsonSet(key, "$", jsonValue);
            _ = batch.JsonGet(key);
            _ = batch.JsonGet(key, "$.name");
            _ = batch.JsonType(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(4, results.Length);
            Assert.Equal("OK", results[0]?.ToString());
            Assert.NotNull(results[1]);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonClusterBatch_ArrayAndObjectOps_ReturnsExpectedResults(GlideClusterClient client)
    {
        if (!await IsJsonModuleAvailable(client))
        {
            Assert.Skip("JSON module is not available");
            return;
        }

        string key = GetUniqueKey();

        try
        {
            _ = await GlideJson.SetAsync(client, key, "$", "{\"arr\":[1,2,3],\"obj\":{\"a\":1}}");

            ClusterBatch batch = new(false);
            _ = batch.JsonArrAppend(key, "$.arr", "4");
            _ = batch.JsonArrLen(key, "$.arr");
            _ = batch.JsonObjLen(key, "$.obj");
            _ = batch.JsonObjKeys(key, "$.obj");
            _ = batch.JsonGet(key);

            object?[]? results = await client.Exec(batch, true);

            Assert.NotNull(results);
            Assert.Equal(5, results.Length);
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    #endregion
}
