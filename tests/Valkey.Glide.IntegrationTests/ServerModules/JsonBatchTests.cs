// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.IntegrationTests;
using Valkey.Glide.Pipeline;
using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.ServerModules;

/// <summary>
/// Integration tests for JSON batch commands via the GlideJsonBatch static methods.
/// These tests require a Valkey server with the JSON module enabled.
/// </summary>
[Collection("GlideTests")]
public class JsonBatchTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static string GetUniqueKey(string prefix = "json_batch") => $"{prefix}:{Guid.NewGuid()}";

    #region Standalone Batch Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task JsonBatch_SetAndGet_ReturnsExpectedResults(GlideClient client)
    {
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // Get returns the full JSON document
            Assert.Contains("John", results[1]?.ToString());
            Assert.Contains("30", results[1]?.ToString());
            // Get with path returns array of matched values
            Assert.Contains("John", results[2]?.ToString());
            // Type at root returns "object"
            Assert.Equal("object", results[3]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // NumIncrBy: 10 + 5 = 15
            Assert.Contains("15", results[0]?.ToString());
            // NumMultBy: 5 * 2 = 10
            Assert.Contains("10", results[1]?.ToString());
            // Final state: counter=15, multiplier=10
            Assert.Contains("15", results[2]?.ToString());
            Assert.Contains("10", results[2]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // ArrAppend with JSONPath returns array of new lengths: [5]
            Assert.NotNull(results[0]);
            // ArrLen with JSONPath returns array of lengths: [5]
            Assert.NotNull(results[1]);
            // ArrIndex with JSONPath returns array of indices: [2]
            Assert.NotNull(results[2]);
            // Get returns the updated JSON document
            Assert.Contains("4", results[3]?.ToString());
            Assert.Contains("5", results[3]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // ObjLen at root returns 3 (keys: a, b, c)
            Assert.Equal(3L, results[0]);
            // ObjLen with $ path returns array [3]
            Assert.NotNull(results[1]);
            // ObjKeys returns the keys
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
    public async Task JsonBatch_StringOperations_ReturnsExpectedResults(GlideClient client)
    {
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // StrLen with JSONPath returns array of lengths: [5]
            Assert.NotNull(results[0]);
            // StrAppend with JSONPath returns array of new lengths: [11]
            Assert.NotNull(results[1]);
            // StrLen after append with JSONPath returns array: [11]
            Assert.NotNull(results[2]);
            // Get returns "Hello World"
            Assert.Contains("Hello World", results[3]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // Del returns number of paths deleted (1)
            Assert.Equal(1L, results[0]);
            // Clear returns number of values cleared (1)
            Assert.Equal(1L, results[1]);
            // Get key1 should not contain "b" anymore
            Assert.DoesNotContain("\"b\"", results[2]?.ToString());
            Assert.Contains("\"a\"", results[2]?.ToString());
            // Get key2 should have empty array and num still 42
            Assert.Contains("[]", results[3]?.ToString());
            Assert.Contains("42", results[3]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // Toggle true -> false returns [false] or 0
            Assert.NotNull(results[0]);
            // Toggle false -> true returns [true] or 1
            Assert.NotNull(results[1]);
            // Final state: active=false, enabled=true
            Assert.Contains("false", results[2]?.ToString());
            Assert.Contains("true", results[2]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // DebugMemory returns memory usage in bytes (should be > 0)
            Assert.True((long)results[0]! > 0);
            // DebugMemory with path returns array
            Assert.NotNull(results[1]);
            // DebugFields returns field count (should be > 0)
            Assert.True((long)results[2]! > 0);
            // DebugFields with path returns array
            Assert.NotNull(results[3]);
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // MGet returns array of values for each key
            object?[]? mgetResult = results[0] as object?[];
            Assert.NotNull(mgetResult);
            Assert.Equal(3, mgetResult.Length);
            Assert.Contains("1", mgetResult[0]?.ToString());
            Assert.Contains("2", mgetResult[1]?.ToString());
            Assert.Contains("3", mgetResult[2]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            Assert.Null(results[2]); // NX fails because key exists
            // Final value should have a=2 (updated by XX)
            Assert.Contains("2", results[3]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // ArrInsert with JSONPath returns array of new lengths
            Assert.NotNull(results[0]);
            // ArrLen with JSONPath returns array of lengths
            Assert.NotNull(results[1]);
            // ArrTrim with JSONPath returns array of new lengths
            Assert.NotNull(results[2]);
            // ArrLen after trim with JSONPath returns array
            Assert.NotNull(results[3]);
            // Get returns trimmed array
            Assert.Contains("inserted", results[4]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // ArrPop with JSONPath returns array of popped values
            Assert.NotNull(results[0]);
            // ArrPop from index 0 with JSONPath returns array
            Assert.NotNull(results[1]);
            // ArrLen with JSONPath returns array of lengths
            Assert.NotNull(results[2]);
            // Get returns remaining array [2,3,4]
            Assert.Contains("2", results[3]?.ToString());
            Assert.Contains("3", results[3]?.ToString());
            Assert.Contains("4", results[3]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // Resp returns RESP representation of JSON
            Assert.NotNull(results[0]);
            Assert.NotNull(results[1]);
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            Assert.Equal("OK", results[0]?.ToString());
            // Each increment returns [1], [2], [3]
            Assert.Contains("1", results[1]?.ToString());
            Assert.Contains("2", results[2]?.ToString());
            Assert.Contains("3", results[3]?.ToString());
            // Final counter value = 3
            Assert.Contains("3", results[4]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // Get returns the full JSON document
            Assert.Contains("John", results[1]?.ToString());
            Assert.Contains("30", results[1]?.ToString());
            // Get with path returns array of matched values
            Assert.Contains("John", results[2]?.ToString());
            // Type at root returns "object"
            Assert.Equal("object", results[3]?.ToString());
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
        if (!await ModuleUtils.IsJsonModuleAvailableAsync(client))
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
            // ArrAppend with JSONPath returns array of new lengths
            Assert.NotNull(results[0]);
            // ArrLen with JSONPath returns array of lengths
            Assert.NotNull(results[1]);
            // ObjLen with JSONPath returns array of lengths
            Assert.NotNull(results[2]);
            // ObjKeys with JSONPath returns array of key arrays
            Assert.NotNull(results[3]);
            // Get returns updated document
            Assert.Contains("4", results[4]?.ToString());
        }
        finally
        {
            _ = await client.DeleteAsync(key);
        }
    }

    #endregion
}
