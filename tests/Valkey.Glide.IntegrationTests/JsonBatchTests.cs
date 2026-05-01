// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Pipeline;
using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for JSON batch commands via the GlideJsonBatch static methods.
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
            _ = GlideJsonBatch.Set(batch, key, "$", jsonValue);
            _ = GlideJsonBatch.Get(batch, key);
            _ = GlideJsonBatch.Get(batch, key, "$.name");
            _ = GlideJsonBatch.Type(batch, key);
            _ = GlideJsonBatch.Type(batch, key, "$.age");

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
            _ = GlideJsonBatch.NumIncrBy(batch, key, "$.counter", 5);
            _ = GlideJsonBatch.NumMultBy(batch, key, "$.multiplier", 2);
            _ = GlideJsonBatch.Get(batch, key);

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
            _ = GlideJsonBatch.ArrAppend(batch, key, "$.items", ["4", "5"]);
            _ = GlideJsonBatch.ArrLen(batch, key, "$.items");
            _ = GlideJsonBatch.ArrIndex(batch, key, "$.items", "3");
            _ = GlideJsonBatch.Get(batch, key);

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
            _ = GlideJsonBatch.ObjLen(batch, key);
            _ = GlideJsonBatch.ObjLen(batch, key, "$");
            _ = GlideJsonBatch.ObjKeys(batch, key);
            _ = GlideJsonBatch.ObjKeys(batch, key, "$");

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
            _ = GlideJsonBatch.StrLen(batch, key, "$.greeting");
            _ = GlideJsonBatch.StrAppend(batch, key, "$.greeting", "\" World\"");
            _ = GlideJsonBatch.StrLen(batch, key, "$.greeting");
            _ = GlideJsonBatch.Get(batch, key, "$.greeting");

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
            _ = GlideJsonBatch.Del(batch, key1, "$.b");
            _ = GlideJsonBatch.Clear(batch, key2, "$.arr");
            _ = GlideJsonBatch.Get(batch, key1);
            _ = GlideJsonBatch.Get(batch, key2);

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
            _ = GlideJsonBatch.Toggle(batch, key, "$.active");
            _ = GlideJsonBatch.Toggle(batch, key, "$.enabled");
            _ = GlideJsonBatch.Get(batch, key);

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
            _ = GlideJsonBatch.DebugMemory(batch, key);
            _ = GlideJsonBatch.DebugMemory(batch, key, "$");
            _ = GlideJsonBatch.DebugFields(batch, key);
            _ = GlideJsonBatch.DebugFields(batch, key, "$");

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
            _ = GlideJsonBatch.MGet(batch, [(GlideString)key1, key2, key3], "$.value");

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
            _ = GlideJsonBatch.Set(batch, key, "$", "{\"a\":1}", GlideJson.SetCondition.OnlyIfDoesNotExist);
            _ = GlideJsonBatch.Set(batch, key, "$.a", "2", GlideJson.SetCondition.OnlyIfExists);
            _ = GlideJsonBatch.Set(batch, key, "$", "{\"b\":1}", GlideJson.SetCondition.OnlyIfDoesNotExist);
            _ = GlideJsonBatch.Get(batch, key);

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
            _ = GlideJsonBatch.ArrInsert(batch, key, "$.arr", 2, ["\"inserted\""]);
            _ = GlideJsonBatch.ArrLen(batch, key, "$.arr");
            _ = GlideJsonBatch.ArrTrim(batch, key, "$.arr", 0, 3);
            _ = GlideJsonBatch.ArrLen(batch, key, "$.arr");
            _ = GlideJsonBatch.Get(batch, key);

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
            _ = GlideJsonBatch.ArrPop(batch, key, "$.arr");
            _ = GlideJsonBatch.ArrPop(batch, key, "$.arr", 0);
            _ = GlideJsonBatch.ArrLen(batch, key, "$.arr");
            _ = GlideJsonBatch.Get(batch, key);

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
            _ = GlideJsonBatch.Resp(batch, key);
            _ = GlideJsonBatch.Resp(batch, key, "$");

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
            _ = GlideJsonBatch.Set(batch, key, "$", "{\"counter\":0}");
            _ = GlideJsonBatch.NumIncrBy(batch, key, "$.counter", 1);
            _ = GlideJsonBatch.NumIncrBy(batch, key, "$.counter", 1);
            _ = GlideJsonBatch.NumIncrBy(batch, key, "$.counter", 1);
            _ = GlideJsonBatch.Get(batch, key);

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
            _ = GlideJsonBatch.Set(batch, key, "$", jsonValue);
            _ = GlideJsonBatch.Get(batch, key);
            _ = GlideJsonBatch.Get(batch, key, "$.name");
            _ = GlideJsonBatch.Type(batch, key);

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
            _ = GlideJsonBatch.ArrAppend(batch, key, "$.arr", ["4"]);
            _ = GlideJsonBatch.ArrLen(batch, key, "$.arr");
            _ = GlideJsonBatch.ObjLen(batch, key, "$.obj");
            _ = GlideJsonBatch.ObjKeys(batch, key, "$.obj");
            _ = GlideJsonBatch.Get(batch, key);

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
