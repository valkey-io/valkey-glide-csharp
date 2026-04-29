// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for JSON commands via the GlideJson module.
/// These tests require a Valkey server with the JSON module enabled.
/// </summary>
[Collection("GlideTests")]
public class JsonCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    /// <summary>
    /// Checks if the JSON module is available on the server.
    /// Returns true if available, false otherwise.
    /// </summary>
    private static async Task<bool> IsJsonModuleAvailable(BaseClient client)
    {
        try
        {
            // Try a simple JSON.SET command to check if the module is loaded
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
            // Clean up the test key
            _ = await client.DeleteAsync(testKey);
            return true;
        }
        catch (Errors.RequestException)
        {
            // JSON module is not available
            return false;
        }
    }

    /// <summary>
    /// Skips the test if the JSON module is not available.
    /// </summary>
    private static async Task SkipIfJsonModuleNotAvailable(BaseClient client)
    {
        bool isAvailable = await IsJsonModuleAvailable(client);
        Assert.SkipWhen(!isAvailable, "JSON module is not available on the server");
    }

    /// <summary>
    /// Generates a unique key with an optional prefix for test isolation.
    /// </summary>
    private static string GetUniqueKey(string prefix = "json") => $"{prefix}:{Guid.NewGuid()}";

    /// <summary>
    /// Generates a unique key with hash tags for cluster mode compatibility.
    /// </summary>
    private static string GetUniqueClusterKey(string prefix = "json") => $"{{{prefix}}}:{Guid.NewGuid()}";

    #region JSON.SET Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_BasicSet_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            result = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
        }
        else
        {
            result = await GlideJson.SetAsync((GlideClusterClient)client, key, "$", jsonValue);
        }

        Assert.Equal("OK", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithGlideString_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            result = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);
        }
        else
        {
            result = await GlideJson.SetAsync((GlideClusterClient)client, key, path, jsonValue);
        }

        Assert.Equal("OK", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_OverwriteExistingValue_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string initialValue = "{\"name\":\"John\"}";
        string newValue = "{\"name\":\"Jane\",\"age\":25}";

        if (client is GlideClient standaloneClient)
        {
            // Set initial value
            string? result1 = await GlideJson.SetAsync(standaloneClient, key, "$", initialValue);
            Assert.Equal("OK", result1);

            // Overwrite with new value
            string? result2 = await GlideJson.SetAsync(standaloneClient, key, "$", newValue);
            Assert.Equal("OK", result2);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set initial value
            string? result1 = await GlideJson.SetAsync(clusterClient, key, "$", initialValue);
            Assert.Equal("OK", result1);

            // Overwrite with new value
            string? result2 = await GlideJson.SetAsync(clusterClient, key, "$", newValue);
            Assert.Equal("OK", result2);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithNxCondition_KeyDoesNotExist_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            result = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue, JsonSetCondition.OnlyIfDoesNotExist);
        }
        else
        {
            result = await GlideJson.SetAsync((GlideClusterClient)client, key, "$", jsonValue, JsonSetCondition.OnlyIfDoesNotExist);
        }

        Assert.Equal("OK", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithNxCondition_KeyExists_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string initialValue = "{\"name\":\"John\"}";
        string newValue = "{\"name\":\"Jane\"}";

        if (client is GlideClient standaloneClient)
        {
            // Set initial value
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", initialValue);

            // Try to set with NX condition - should return null since key exists
            string? result = await GlideJson.SetAsync(standaloneClient, key, "$", newValue, JsonSetCondition.OnlyIfDoesNotExist);
            Assert.Null(result);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set initial value
            _ = await GlideJson.SetAsync(clusterClient, key, "$", initialValue);

            // Try to set with NX condition - should return null since key exists
            string? result = await GlideJson.SetAsync(clusterClient, key, "$", newValue, JsonSetCondition.OnlyIfDoesNotExist);
            Assert.Null(result);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithXxCondition_KeyExists_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string initialValue = "{\"name\":\"John\"}";
        string newValue = "{\"name\":\"Jane\"}";

        if (client is GlideClient standaloneClient)
        {
            // Set initial value
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", initialValue);

            // Set with XX condition - should succeed since key exists
            string? result = await GlideJson.SetAsync(standaloneClient, key, "$", newValue, JsonSetCondition.OnlyIfExists);
            Assert.Equal("OK", result);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set initial value
            _ = await GlideJson.SetAsync(clusterClient, key, "$", initialValue);

            // Set with XX condition - should succeed since key exists
            string? result = await GlideJson.SetAsync(clusterClient, key, "$", newValue, JsonSetCondition.OnlyIfExists);
            Assert.Equal("OK", result);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithXxCondition_KeyDoesNotExist_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Try to set with XX condition on non-existent key - should return null
            result = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue, JsonSetCondition.OnlyIfExists);
        }
        else
        {
            // Try to set with XX condition on non-existent key - should return null
            result = await GlideJson.SetAsync((GlideClusterClient)client, key, "$", jsonValue, JsonSetCondition.OnlyIfExists);
        }

        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SetNestedPath_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string initialValue = "{\"person\":{\"name\":\"John\"}}";
        string newNameValue = "\"Jane\"";

        if (client is GlideClient standaloneClient)
        {
            // Set initial document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", initialValue);

            // Update nested path
            string? result = await GlideJson.SetAsync(standaloneClient, key, "$.person.name", newNameValue);
            Assert.Equal("OK", result);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set initial document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", initialValue);

            // Update nested path
            string? result = await GlideJson.SetAsync(clusterClient, key, "$.person.name", newNameValue);
            Assert.Equal("OK", result);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SetArrayValue_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "[1, 2, 3, 4, 5]";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            result = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
        }
        else
        {
            result = await GlideJson.SetAsync((GlideClusterClient)client, key, "$", jsonValue);
        }

        Assert.Equal("OK", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SetScalarValues_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string keyString = GetUniqueKey("string");
        string keyNumber = GetUniqueKey("number");
        string keyBool = GetUniqueKey("bool");
        string keyNull = GetUniqueKey("null");

        if (client is GlideClient standaloneClient)
        {
            // Set string value
            string? resultString = await GlideJson.SetAsync(standaloneClient, keyString, "$", "\"hello\"");
            Assert.Equal("OK", resultString);

            // Set number value
            string? resultNumber = await GlideJson.SetAsync(standaloneClient, keyNumber, "$", "42");
            Assert.Equal("OK", resultNumber);

            // Set boolean value
            string? resultBool = await GlideJson.SetAsync(standaloneClient, keyBool, "$", "true");
            Assert.Equal("OK", resultBool);

            // Set null value
            string? resultNull = await GlideJson.SetAsync(standaloneClient, keyNull, "$", "null");
            Assert.Equal("OK", resultNull);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set string value
            string? resultString = await GlideJson.SetAsync(clusterClient, keyString, "$", "\"hello\"");
            Assert.Equal("OK", resultString);

            // Set number value
            string? resultNumber = await GlideJson.SetAsync(clusterClient, keyNumber, "$", "42");
            Assert.Equal("OK", resultNumber);

            // Set boolean value
            string? resultBool = await GlideJson.SetAsync(clusterClient, keyBool, "$", "true");
            Assert.Equal("OK", resultBool);

            // Set null value
            string? resultNull = await GlideJson.SetAsync(clusterClient, keyNull, "$", "null");
            Assert.Equal("OK", resultNull);
        }
    }

    #endregion

    #region JSON.GET Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_EntireDocument_ReturnsDocument(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        // The result should contain the JSON document
        Assert.Contains("\"name\"", result);
        Assert.Contains("\"John\"", result);
        Assert.Contains("\"age\"", result);
        Assert.Contains("30", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithJsonPath_ReturnsArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.name"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, ["$.name"]);
        }

        Assert.NotNull(result);
        // JSONPath returns an array of matching values
        Assert.Equal("[\"John\"]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithLegacyPath_ReturnsSingleValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, [".name"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, [".name"]);
        }

        Assert.NotNull(result);
        // Legacy path returns the single matching value
        Assert.Equal("\"John\"", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithMultiplePaths_ReturnsObject(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.name", "$.age"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, ["$.name", "$.age"]);
        }

        Assert.NotNull(result);
        // Multiple paths return a JSON object with each path as a key
        Assert.Contains("$.name", result);
        Assert.Contains("$.age", result);
        Assert.Contains("\"John\"", result);
        Assert.Contains("30", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithJsonGetOptions_ReturnsFormattedJson(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";
        var options = new JsonGetOptions { Indent = "  ", Newline = "\n", Space = " " };

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, options);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, options);
        }

        Assert.NotNull(result);
        // The result should be formatted with newlines and indentation
        Assert.Contains("\n", result);
        Assert.Contains("  ", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithPathsAndOptions_ReturnsFormattedJson(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"age\":30}}";
        var options = new JsonGetOptions { Indent = "\t", Newline = "\n" };

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.person"], options);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, ["$.person"], options);
        }

        Assert.NotNull(result);
        // The result should be formatted with tabs and newlines
        Assert.Contains("\n", result);
        Assert.Contains("\t", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        string? result;
        if (client is GlideClient standaloneClient)
        {
            result = await GlideJson.GetAsync(standaloneClient, key);
        }
        else
        {
            result = await GlideJson.GetAsync((GlideClusterClient)client, key);
        }

        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_SetGetRoundTrip_ReturnsOriginalValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"active\":true,\"scores\":[1,2,3]}";

        if (client is GlideClient standaloneClient)
        {
            // Set the value
            string? setResult = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            Assert.Equal("OK", setResult);

            // Get the value back
            string? getResult = await GlideJson.GetAsync(standaloneClient, key);
            Assert.NotNull(getResult);

            // Verify the round trip preserves the data
            Assert.Contains("\"name\"", getResult);
            Assert.Contains("\"John\"", getResult);
            Assert.Contains("\"age\"", getResult);
            Assert.Contains("30", getResult);
            Assert.Contains("\"active\"", getResult);
            Assert.Contains("true", getResult);
            Assert.Contains("\"scores\"", getResult);
            Assert.Contains("[1,2,3]", getResult);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set the value
            string? setResult = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            Assert.Equal("OK", setResult);

            // Get the value back
            string? getResult = await GlideJson.GetAsync(clusterClient, key);
            Assert.NotNull(getResult);

            // Verify the round trip preserves the data
            Assert.Contains("\"name\"", getResult);
            Assert.Contains("\"John\"", getResult);
            Assert.Contains("\"age\"", getResult);
            Assert.Contains("30", getResult);
            Assert.Contains("\"active\"", getResult);
            Assert.Contains("true", getResult);
            Assert.Contains("\"scores\"", getResult);
            Assert.Contains("[1,2,3]", getResult);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithGlideString_ReturnsDocument(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        GlideString? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        string resultStr = result.ToString();
        Assert.Contains("\"name\"", resultStr);
        Assert.Contains("\"Jane\"", resultStr);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithGlideStringPaths_ReturnsValues(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        GlideString[] paths = ["$.name"];

        GlideString? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, paths);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, paths);
        }

        Assert.NotNull(result);
        Assert.Equal("[\"Jane\"]", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_NestedObject_ReturnsNestedValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"address\":{\"city\":\"NYC\",\"zip\":\"10001\"}}}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.person.address.city"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, ["$.person.address.city"]);
        }

        Assert.NotNull(result);
        Assert.Equal("[\"NYC\"]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_ArrayElement_ReturnsElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"numbers\":[10,20,30,40,50]}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.numbers[2]"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, ["$.numbers[2]"]);
        }

        Assert.NotNull(result);
        Assert.Equal("[30]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WildcardPath_ReturnsAllMatches(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"b\"},{\"name\":\"c\"}]}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.items[*].name"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.GetAsync(clusterClient, key, ["$.items[*].name"]);
        }

        Assert.NotNull(result);
        // Should return all matching names as an array
        Assert.Contains("\"a\"", result);
        Assert.Contains("\"b\"", result);
        Assert.Contains("\"c\"", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_ScalarValues_ReturnsCorrectTypes(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string keyString = GetUniqueKey("string");
        string keyNumber = GetUniqueKey("number");
        string keyBool = GetUniqueKey("bool");
        string keyNull = GetUniqueKey("null");

        if (client is GlideClient standaloneClient)
        {
            // Set and get string value
            _ = await GlideJson.SetAsync(standaloneClient, keyString, "$", "\"hello\"");
            string? resultString = await GlideJson.GetAsync(standaloneClient, keyString);
            Assert.Equal("\"hello\"", resultString);

            // Set and get number value
            _ = await GlideJson.SetAsync(standaloneClient, keyNumber, "$", "42");
            string? resultNumber = await GlideJson.GetAsync(standaloneClient, keyNumber);
            Assert.Equal("42", resultNumber);

            // Set and get boolean value
            _ = await GlideJson.SetAsync(standaloneClient, keyBool, "$", "true");
            string? resultBool = await GlideJson.GetAsync(standaloneClient, keyBool);
            Assert.Equal("true", resultBool);

            // Set and get null value
            _ = await GlideJson.SetAsync(standaloneClient, keyNull, "$", "null");
            string? resultNull = await GlideJson.GetAsync(standaloneClient, keyNull);
            Assert.Equal("null", resultNull);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set and get string value
            _ = await GlideJson.SetAsync(clusterClient, keyString, "$", "\"hello\"");
            string? resultString = await GlideJson.GetAsync(clusterClient, keyString);
            Assert.Equal("\"hello\"", resultString);

            // Set and get number value
            _ = await GlideJson.SetAsync(clusterClient, keyNumber, "$", "42");
            string? resultNumber = await GlideJson.GetAsync(clusterClient, keyNumber);
            Assert.Equal("42", resultNumber);

            // Set and get boolean value
            _ = await GlideJson.SetAsync(clusterClient, keyBool, "$", "true");
            string? resultBool = await GlideJson.GetAsync(clusterClient, keyBool);
            Assert.Equal("true", resultBool);

            // Set and get null value
            _ = await GlideJson.SetAsync(clusterClient, keyNull, "$", "null");
            string? resultNull = await GlideJson.GetAsync(clusterClient, keyNull);
            Assert.Equal("null", resultNull);
        }
    }

    #endregion

    #region JSON.MGET Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_MultipleExistingKeys_ReturnsAllValues(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        string key3 = GetUniqueClusterKey("mget");

        if (client is GlideClient standaloneClient)
        {
            // Set up JSON documents
            _ = await GlideJson.SetAsync(standaloneClient, key1, "$", "{\"name\":\"John\",\"age\":30}");
            _ = await GlideJson.SetAsync(standaloneClient, key2, "$", "{\"name\":\"Jane\",\"age\":25}");
            _ = await GlideJson.SetAsync(standaloneClient, key3, "$", "{\"name\":\"Bob\",\"age\":35}");

            // Get values from multiple keys with JSONPath
            string?[] results = await GlideJson.MGetAsync(standaloneClient, [key1, key2, key3], "$.name");

            Assert.Equal(3, results.Length);
            Assert.Equal("[\"John\"]", results[0]);
            Assert.Equal("[\"Jane\"]", results[1]);
            Assert.Equal("[\"Bob\"]", results[2]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set up JSON documents
            _ = await GlideJson.SetAsync(clusterClient, key1, "$", "{\"name\":\"John\",\"age\":30}");
            _ = await GlideJson.SetAsync(clusterClient, key2, "$", "{\"name\":\"Jane\",\"age\":25}");
            _ = await GlideJson.SetAsync(clusterClient, key3, "$", "{\"name\":\"Bob\",\"age\":35}");

            // Get values from multiple keys with JSONPath
            string?[] results = await GlideJson.MGetAsync(clusterClient, [key1, key2, key3], "$.name");

            Assert.Equal(3, results.Length);
            Assert.Equal("[\"John\"]", results[0]);
            Assert.Equal("[\"Jane\"]", results[1]);
            Assert.Equal("[\"Bob\"]", results[2]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_SomeNonExistentKeys_ReturnsNullForMissing(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        string nonExistentKey = GetUniqueClusterKey("mget_nonexistent");

        if (client is GlideClient standaloneClient)
        {
            // Set up JSON documents (only key1 and key2)
            _ = await GlideJson.SetAsync(standaloneClient, key1, "$", "{\"name\":\"John\"}");
            _ = await GlideJson.SetAsync(standaloneClient, key2, "$", "{\"name\":\"Jane\"}");

            // Get values including a non-existent key
            string?[] results = await GlideJson.MGetAsync(standaloneClient, [key1, nonExistentKey, key2], "$.name");

            Assert.Equal(3, results.Length);
            Assert.Equal("[\"John\"]", results[0]);
            Assert.Null(results[1]); // Non-existent key returns null
            Assert.Equal("[\"Jane\"]", results[2]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set up JSON documents (only key1 and key2)
            _ = await GlideJson.SetAsync(clusterClient, key1, "$", "{\"name\":\"John\"}");
            _ = await GlideJson.SetAsync(clusterClient, key2, "$", "{\"name\":\"Jane\"}");

            // Get values including a non-existent key
            string?[] results = await GlideJson.MGetAsync(clusterClient, [key1, nonExistentKey, key2], "$.name");

            Assert.Equal(3, results.Length);
            Assert.Equal("[\"John\"]", results[0]);
            Assert.Null(results[1]); // Non-existent key returns null
            Assert.Equal("[\"Jane\"]", results[2]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_WithJsonPath_ReturnsArrays(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");

        if (client is GlideClient standaloneClient)
        {
            // Set up JSON documents with arrays
            _ = await GlideJson.SetAsync(standaloneClient, key1, "$", "{\"items\":[1,2,3]}");
            _ = await GlideJson.SetAsync(standaloneClient, key2, "$", "{\"items\":[4,5,6]}");

            // Get values with JSONPath - should return arrays
            string?[] results = await GlideJson.MGetAsync(standaloneClient, [key1, key2], "$.items");

            Assert.Equal(2, results.Length);
            Assert.NotNull(results[0]);
            Assert.NotNull(results[1]);
            // JSONPath returns arrays of matching values
            Assert.Contains("[1,2,3]", results[0]);
            Assert.Contains("[4,5,6]", results[1]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set up JSON documents with arrays
            _ = await GlideJson.SetAsync(clusterClient, key1, "$", "{\"items\":[1,2,3]}");
            _ = await GlideJson.SetAsync(clusterClient, key2, "$", "{\"items\":[4,5,6]}");

            // Get values with JSONPath - should return arrays
            string?[] results = await GlideJson.MGetAsync(clusterClient, [key1, key2], "$.items");

            Assert.Equal(2, results.Length);
            Assert.NotNull(results[0]);
            Assert.NotNull(results[1]);
            // JSONPath returns arrays of matching values
            Assert.Contains("[1,2,3]", results[0]);
            Assert.Contains("[4,5,6]", results[1]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_WithLegacyPath_ReturnsSingleValues(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");

        if (client is GlideClient standaloneClient)
        {
            // Set up JSON documents
            _ = await GlideJson.SetAsync(standaloneClient, key1, "$", "{\"name\":\"John\"}");
            _ = await GlideJson.SetAsync(standaloneClient, key2, "$", "{\"name\":\"Jane\"}");

            // Get values with legacy path (no $ prefix)
            string?[] results = await GlideJson.MGetAsync(standaloneClient, [key1, key2], ".name");

            Assert.Equal(2, results.Length);
            Assert.Equal("\"John\"", results[0]);
            Assert.Equal("\"Jane\"", results[1]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set up JSON documents
            _ = await GlideJson.SetAsync(clusterClient, key1, "$", "{\"name\":\"John\"}");
            _ = await GlideJson.SetAsync(clusterClient, key2, "$", "{\"name\":\"Jane\"}");

            // Get values with legacy path (no $ prefix)
            string?[] results = await GlideJson.MGetAsync(clusterClient, [key1, key2], ".name");

            Assert.Equal(2, results.Length);
            Assert.Equal("\"John\"", results[0]);
            Assert.Equal("\"Jane\"", results[1]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_WithGlideString_ReturnsValues(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        GlideString key1 = GetUniqueClusterKey("mget");
        GlideString key2 = GetUniqueClusterKey("mget");
        GlideString path = "$.name";

        if (client is GlideClient standaloneClient)
        {
            // Set up JSON documents
            _ = await GlideJson.SetAsync(standaloneClient, key1, (GlideString)"$", (GlideString)"{\"name\":\"John\"}");
            _ = await GlideJson.SetAsync(standaloneClient, key2, (GlideString)"$", (GlideString)"{\"name\":\"Jane\"}");

            // Get values using GlideString overload
            GlideString?[] results = await GlideJson.MGetAsync(standaloneClient, [key1, key2], path);

            Assert.Equal(2, results.Length);
            Assert.NotNull(results[0]);
            Assert.NotNull(results[1]);
            Assert.Equal("[\"John\"]", results[0]!.ToString());
            Assert.Equal("[\"Jane\"]", results[1]!.ToString());
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set up JSON documents
            _ = await GlideJson.SetAsync(clusterClient, key1, (GlideString)"$", (GlideString)"{\"name\":\"John\"}");
            _ = await GlideJson.SetAsync(clusterClient, key2, (GlideString)"$", (GlideString)"{\"name\":\"Jane\"}");

            // Get values using GlideString overload
            GlideString?[] results = await GlideJson.MGetAsync(clusterClient, [key1, key2], path);

            Assert.Equal(2, results.Length);
            Assert.NotNull(results[0]);
            Assert.NotNull(results[1]);
            Assert.Equal("[\"John\"]", results[0]!.ToString());
            Assert.Equal("[\"Jane\"]", results[1]!.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_PathDoesNotExist_ReturnsNullForThoseKeys(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");

        if (client is GlideClient standaloneClient)
        {
            // Set up JSON documents with different structures
            _ = await GlideJson.SetAsync(standaloneClient, key1, "$", "{\"name\":\"John\"}");
            _ = await GlideJson.SetAsync(standaloneClient, key2, "$", "{\"age\":25}"); // No "name" field

            // Get values with legacy path - key2 doesn't have "name"
            string?[] results = await GlideJson.MGetAsync(standaloneClient, [key1, key2], ".name");

            Assert.Equal(2, results.Length);
            Assert.Equal("\"John\"", results[0]);
            Assert.Null(results[1]); // Path doesn't exist in key2
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set up JSON documents with different structures
            _ = await GlideJson.SetAsync(clusterClient, key1, "$", "{\"name\":\"John\"}");
            _ = await GlideJson.SetAsync(clusterClient, key2, "$", "{\"age\":25}"); // No "name" field

            // Get values with legacy path - key2 doesn't have "name"
            string?[] results = await GlideJson.MGetAsync(clusterClient, [key1, key2], ".name");

            Assert.Equal(2, results.Length);
            Assert.Equal("\"John\"", results[0]);
            Assert.Null(results[1]); // Path doesn't exist in key2
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_RootPath_ReturnsEntireDocuments(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");

        if (client is GlideClient standaloneClient)
        {
            // Set up JSON documents
            _ = await GlideJson.SetAsync(standaloneClient, key1, "$", "{\"a\":1}");
            _ = await GlideJson.SetAsync(standaloneClient, key2, "$", "{\"b\":2}");

            // Get entire documents with root path
            string?[] results = await GlideJson.MGetAsync(standaloneClient, [key1, key2], "$");

            Assert.Equal(2, results.Length);
            Assert.NotNull(results[0]);
            Assert.NotNull(results[1]);
            Assert.Contains("\"a\"", results[0]);
            Assert.Contains("1", results[0]);
            Assert.Contains("\"b\"", results[1]);
            Assert.Contains("2", results[1]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set up JSON documents
            _ = await GlideJson.SetAsync(clusterClient, key1, "$", "{\"a\":1}");
            _ = await GlideJson.SetAsync(clusterClient, key2, "$", "{\"b\":2}");

            // Get entire documents with root path
            string?[] results = await GlideJson.MGetAsync(clusterClient, [key1, key2], "$");

            Assert.Equal(2, results.Length);
            Assert.NotNull(results[0]);
            Assert.NotNull(results[1]);
            Assert.Contains("\"a\"", results[0]);
            Assert.Contains("1", results[0]);
            Assert.Contains("\"b\"", results[1]);
            Assert.Contains("2", results[1]);
        }
    }

    #endregion

    #region JSON.DEL Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_EntireDocument_ReturnsOne(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Delete entire document
            deleted = await GlideJson.DelAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Delete entire document
            deleted = await GlideJson.DelAsync(clusterClient, key);
        }

        Assert.Equal(1, deleted);

        // Verify the key no longer exists
        string? result;
        if (client is GlideClient standaloneClient2)
        {
            result = await GlideJson.GetAsync(standaloneClient2, key);
        }
        else
        {
            result = await GlideJson.GetAsync((GlideClusterClient)client, key);
        }
        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_WithPath_DeletesSpecificElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Delete specific path
            deleted = await GlideJson.DelAsync(standaloneClient, key, "$.age");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Delete specific path
            deleted = await GlideJson.DelAsync(clusterClient, key, "$.age");
        }

        Assert.Equal(1, deleted);

        // Verify the path was deleted but document still exists
        string? result;
        if (client is GlideClient standaloneClient2)
        {
            result = await GlideJson.GetAsync(standaloneClient2, key);
        }
        else
        {
            result = await GlideJson.GetAsync((GlideClusterClient)client, key);
        }
        Assert.NotNull(result);
        Assert.Contains("\"name\"", result);
        Assert.Contains("\"city\"", result);
        Assert.DoesNotContain("\"age\"", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_NonExistentKey_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            deleted = await GlideJson.DelAsync(standaloneClient, key);
        }
        else
        {
            deleted = await GlideJson.DelAsync((GlideClusterClient)client, key);
        }

        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_NonExistentPath_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Try to delete non-existent path
            deleted = await GlideJson.DelAsync(standaloneClient, key, "$.nonexistent");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Try to delete non-existent path
            deleted = await GlideJson.DelAsync(clusterClient, key, "$.nonexistent");
        }

        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_WithGlideString_DeletesDocument(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);

            // Delete entire document using GlideString overload
            deleted = await GlideJson.DelAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);

            // Delete entire document using GlideString overload
            deleted = await GlideJson.DelAsync(clusterClient, key);
        }

        Assert.Equal(1, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_WithGlideStringPath_DeletesSpecificElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        GlideString deletePath = "$.age";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);

            // Delete specific path using GlideString overload
            deleted = await GlideJson.DelAsync(standaloneClient, key, deletePath);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);

            // Delete specific path using GlideString overload
            deleted = await GlideJson.DelAsync(clusterClient, key, deletePath);
        }

        Assert.Equal(1, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_MultipleMatchingPaths_ReturnsCount(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"b\"},{\"name\":\"c\"}]}";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document with array of objects
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Delete all "name" fields using wildcard path
            deleted = await GlideJson.DelAsync(standaloneClient, key, "$.items[*].name");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document with array of objects
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Delete all "name" fields using wildcard path
            deleted = await GlideJson.DelAsync(clusterClient, key, "$.items[*].name");
        }

        // Should delete 3 "name" fields
        Assert.Equal(3, deleted);
    }

    #endregion

    #region JSON.FORGET Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_EntireDocument_ReturnsOne(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Forget (delete) entire document
            deleted = await GlideJson.ForgetAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Forget (delete) entire document
            deleted = await GlideJson.ForgetAsync(clusterClient, key);
        }

        Assert.Equal(1, deleted);

        // Verify the key no longer exists
        string? result;
        if (client is GlideClient standaloneClient2)
        {
            result = await GlideJson.GetAsync(standaloneClient2, key);
        }
        else
        {
            result = await GlideJson.GetAsync((GlideClusterClient)client, key);
        }
        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_WithPath_DeletesSpecificElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Forget specific path
            deleted = await GlideJson.ForgetAsync(standaloneClient, key, "$.city");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Forget specific path
            deleted = await GlideJson.ForgetAsync(clusterClient, key, "$.city");
        }

        Assert.Equal(1, deleted);

        // Verify the path was deleted but document still exists
        string? result;
        if (client is GlideClient standaloneClient2)
        {
            result = await GlideJson.GetAsync(standaloneClient2, key);
        }
        else
        {
            result = await GlideJson.GetAsync((GlideClusterClient)client, key);
        }
        Assert.NotNull(result);
        Assert.Contains("\"name\"", result);
        Assert.Contains("\"age\"", result);
        Assert.DoesNotContain("\"city\"", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_NonExistentKey_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            deleted = await GlideJson.ForgetAsync(standaloneClient, key);
        }
        else
        {
            deleted = await GlideJson.ForgetAsync((GlideClusterClient)client, key);
        }

        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_BehavesIdenticallyToDel(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string keyDel = GetUniqueKey("del");
        string keyForget = GetUniqueKey("forget");
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"items\":[1,2,3]}";

        if (client is GlideClient standaloneClient)
        {
            // Set up identical JSON documents
            _ = await GlideJson.SetAsync(standaloneClient, keyDel, "$", jsonValue);
            _ = await GlideJson.SetAsync(standaloneClient, keyForget, "$", jsonValue);

            // Delete using Del
            long deletedDel = await GlideJson.DelAsync(standaloneClient, keyDel, "$.age");
            // Delete using Forget
            long deletedForget = await GlideJson.ForgetAsync(standaloneClient, keyForget, "$.age");

            // Both should return the same count
            Assert.Equal(deletedDel, deletedForget);

            // Both documents should be in the same state
            string? resultDel = await GlideJson.GetAsync(standaloneClient, keyDel);
            string? resultForget = await GlideJson.GetAsync(standaloneClient, keyForget);

            Assert.NotNull(resultDel);
            Assert.NotNull(resultForget);
            Assert.Equal(resultDel, resultForget);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;

            // Set up identical JSON documents
            _ = await GlideJson.SetAsync(clusterClient, keyDel, "$", jsonValue);
            _ = await GlideJson.SetAsync(clusterClient, keyForget, "$", jsonValue);

            // Delete using Del
            long deletedDel = await GlideJson.DelAsync(clusterClient, keyDel, "$.age");
            // Delete using Forget
            long deletedForget = await GlideJson.ForgetAsync(clusterClient, keyForget, "$.age");

            // Both should return the same count
            Assert.Equal(deletedDel, deletedForget);

            // Both documents should be in the same state
            string? resultDel = await GlideJson.GetAsync(clusterClient, keyDel);
            string? resultForget = await GlideJson.GetAsync(clusterClient, keyForget);

            Assert.NotNull(resultDel);
            Assert.NotNull(resultForget);
            Assert.Equal(resultDel, resultForget);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_WithGlideString_DeletesDocument(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);

            // Forget entire document using GlideString overload
            deleted = await GlideJson.ForgetAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);

            // Forget entire document using GlideString overload
            deleted = await GlideJson.ForgetAsync(clusterClient, key);
        }

        Assert.Equal(1, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_WithGlideStringPath_DeletesSpecificElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        GlideString deletePath = "$.name";

        long deleted;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);

            // Forget specific path using GlideString overload
            deleted = await GlideJson.ForgetAsync(standaloneClient, key, deletePath);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);

            // Forget specific path using GlideString overload
            deleted = await GlideJson.ForgetAsync(clusterClient, key, deletePath);
        }

        Assert.Equal(1, deleted);
    }

    #endregion

    #region JSON.CLEAR Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_Array_EmptiesArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[1,2,3,4,5]}";

        long cleared;
        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document with array
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Clear the array
            cleared = await GlideJson.ClearAsync(standaloneClient, key, "$.items");

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.items"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document with array
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Clear the array
            cleared = await GlideJson.ClearAsync(clusterClient, key, "$.items");

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key, ["$.items"]);
        }

        Assert.Equal(1, cleared);
        Assert.NotNull(result);
        // Array should be empty
        Assert.Equal("[[]]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_Object_RemovesAllKeys(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"data\":{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}}";

        long cleared;
        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document with nested object
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Clear the nested object
            cleared = await GlideJson.ClearAsync(standaloneClient, key, "$.data");

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.data"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document with nested object
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Clear the nested object
            cleared = await GlideJson.ClearAsync(clusterClient, key, "$.data");

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key, ["$.data"]);
        }

        Assert.Equal(1, cleared);
        Assert.NotNull(result);
        // Object should be empty
        Assert.Equal("[{}]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_Number_SetsToZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";

        long cleared;
        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document with number
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Clear the number
            cleared = await GlideJson.ClearAsync(standaloneClient, key, "$.count");

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.count"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document with number
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Clear the number
            cleared = await GlideJson.ClearAsync(clusterClient, key, "$.count");

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key, ["$.count"]);
        }

        Assert.Equal(1, cleared);
        Assert.NotNull(result);
        // Number should be 0
        Assert.Equal("[0]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_Boolean_SetsToFalse(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"active\":true}";

        long cleared;
        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document with boolean
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Clear the boolean - Valkey JSON clears booleans to false
            cleared = await GlideJson.ClearAsync(standaloneClient, key, "$.active");

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.active"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document with boolean
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Clear the boolean
            cleared = await GlideJson.ClearAsync(clusterClient, key, "$.active");

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key, ["$.active"]);
        }

        // Valkey JSON clears booleans to false and returns 1
        Assert.Equal(1, cleared);
        Assert.NotNull(result);
        // Boolean should be set to false
        Assert.Equal("[false]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_String_NotCleared(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        long cleared;
        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document with string
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Clear the string - Valkey JSON clears strings to empty string
            cleared = await GlideJson.ClearAsync(standaloneClient, key, "$.name");

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.name"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document with string
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Clear the string
            cleared = await GlideJson.ClearAsync(clusterClient, key, "$.name");

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key, ["$.name"]);
        }

        // Valkey JSON clears strings to empty string and returns 1
        Assert.Equal(1, cleared);
        Assert.NotNull(result);
        // String should be cleared to empty string
        Assert.Equal("[\"\"]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_NonExistentKey_ThrowsException(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        // Valkey JSON throws NONEXISTENT error for non-existent keys
        if (client is GlideClient standaloneClient)
        {
            var ex = await Assert.ThrowsAsync<Errors.RequestException>(
                () => GlideJson.ClearAsync(standaloneClient, key));
            Assert.Contains("NONEXISTENT", ex.Message);
        }
        else
        {
            var ex = await Assert.ThrowsAsync<Errors.RequestException>(
                () => GlideJson.ClearAsync((GlideClusterClient)client, key));
            Assert.Contains("NONEXISTENT", ex.Message);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_WithGlideString_ClearsArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"items\":[1,2,3]}";
        GlideString clearPath = "$.items";

        long cleared;
        GlideString? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);

            // Clear using GlideString overload
            cleared = await GlideJson.ClearAsync(standaloneClient, key, clearPath);

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key, [clearPath]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);

            // Clear using GlideString overload
            cleared = await GlideJson.ClearAsync(clusterClient, key, clearPath);

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key, [clearPath]);
        }

        Assert.Equal(1, cleared);
        Assert.NotNull(result);
        Assert.Equal("[[]]", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_RootPath_ClearsEntireDocument(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"items\":[1,2,3],\"count\":5}";

        long cleared;
        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Clear at root path
            cleared = await GlideJson.ClearAsync(standaloneClient, key, "$");

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Clear at root path
            cleared = await GlideJson.ClearAsync(clusterClient, key, "$");

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key);
        }

        Assert.Equal(1, cleared);
        Assert.NotNull(result);
        // Root object should be empty
        Assert.Equal("{}", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_MultipleMatchingPaths_ReturnsCount(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"data\":[1,2]},{\"data\":[3,4]},{\"data\":[5,6]}]}";

        long cleared;
        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document with multiple arrays
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Clear all "data" arrays using wildcard path
            cleared = await GlideJson.ClearAsync(standaloneClient, key, "$.items[*].data");

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key, ["$.items[*].data"]);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document with multiple arrays
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Clear all "data" arrays using wildcard path
            cleared = await GlideJson.ClearAsync(clusterClient, key, "$.items[*].data");

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key, ["$.items[*].data"]);
        }

        // Should clear 3 arrays
        Assert.Equal(3, cleared);
        Assert.NotNull(result);
        // All arrays should be empty
        Assert.Equal("[[],[],[]]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_WithoutPath_ClearsRoot(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        long cleared;
        string? result;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Clear without specifying path (should clear root)
            cleared = await GlideJson.ClearAsync(standaloneClient, key);

            // Get the result
            result = await GlideJson.GetAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Clear without specifying path (should clear root)
            cleared = await GlideJson.ClearAsync(clusterClient, key);

            // Get the result
            result = await GlideJson.GetAsync(clusterClient, key);
        }

        Assert.Equal(1, cleared);
        Assert.NotNull(result);
        // Root object should be empty
        Assert.Equal("{}", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_NonExistentPath_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        long cleared;
        if (client is GlideClient standaloneClient)
        {
            // Set up JSON document
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);

            // Try to clear non-existent path
            cleared = await GlideJson.ClearAsync(standaloneClient, key, "$.nonexistent");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            // Set up JSON document
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);

            // Try to clear non-existent path
            cleared = await GlideJson.ClearAsync(clusterClient, key, "$.nonexistent");
        }

        Assert.Equal(0, cleared);
    }

    #endregion

    #region JSON.TYPE Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_ObjectType_ReturnsObject(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        Assert.Equal("object", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_ArrayType_ReturnsArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "[1, 2, 3, 4, 5]";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        Assert.Equal("array", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_StringType_ReturnsString(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "\"hello world\"";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        Assert.Equal("string", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NumberType_ReturnsNumberOrInteger(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string keyInteger = GetUniqueKey("integer");
        string keyFloat = GetUniqueKey("float");

        object? intResult;
        object? floatResult;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, keyInteger, "$", "42");
            _ = await GlideJson.SetAsync(standaloneClient, keyFloat, "$", "3.14");
            intResult = await GlideJson.TypeAsync(standaloneClient, keyInteger);
            floatResult = await GlideJson.TypeAsync(standaloneClient, keyFloat);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, keyInteger, "$", "42");
            _ = await GlideJson.SetAsync(clusterClient, keyFloat, "$", "3.14");
            intResult = await GlideJson.TypeAsync(clusterClient, keyInteger);
            floatResult = await GlideJson.TypeAsync(clusterClient, keyFloat);
        }

        Assert.NotNull(intResult);
        Assert.NotNull(floatResult);
        // Integer values may return "integer" or "number" depending on the JSON module version
        Assert.True(intResult.ToString() == "integer" || intResult.ToString() == "number",
            $"Expected 'integer' or 'number' but got '{intResult}'");
        Assert.Equal("number", floatResult.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_BooleanType_ReturnsBoolean(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string keyTrue = GetUniqueKey("true");
        string keyFalse = GetUniqueKey("false");

        object? trueResult;
        object? falseResult;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, keyTrue, "$", "true");
            _ = await GlideJson.SetAsync(standaloneClient, keyFalse, "$", "false");
            trueResult = await GlideJson.TypeAsync(standaloneClient, keyTrue);
            falseResult = await GlideJson.TypeAsync(standaloneClient, keyFalse);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, keyTrue, "$", "true");
            _ = await GlideJson.SetAsync(clusterClient, keyFalse, "$", "false");
            trueResult = await GlideJson.TypeAsync(clusterClient, keyTrue);
            falseResult = await GlideJson.TypeAsync(clusterClient, keyFalse);
        }

        Assert.NotNull(trueResult);
        Assert.NotNull(falseResult);
        Assert.Equal("boolean", trueResult.ToString());
        Assert.Equal("boolean", falseResult.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NullType_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "null";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        Assert.Equal("null", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithJsonPath_ReturnsArrayOfTypes(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"active\":true}";

        object? nameResult;
        object? ageResult;
        object? activeResult;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            nameResult = await GlideJson.TypeAsync(standaloneClient, key, "$.name");
            ageResult = await GlideJson.TypeAsync(standaloneClient, key, "$.age");
            activeResult = await GlideJson.TypeAsync(standaloneClient, key, "$.active");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            nameResult = await GlideJson.TypeAsync(clusterClient, key, "$.name");
            ageResult = await GlideJson.TypeAsync(clusterClient, key, "$.age");
            activeResult = await GlideJson.TypeAsync(clusterClient, key, "$.active");
        }

        // JSONPath returns an array of types
        Assert.NotNull(nameResult);
        Assert.NotNull(ageResult);
        Assert.NotNull(activeResult);

        // Results should be arrays containing the type strings
        if (nameResult is object?[] nameArray)
        {
            Assert.Single(nameArray);
            Assert.Equal("string", nameArray[0]?.ToString());
        }
        else
        {
            // Some implementations may return the type directly
            Assert.Equal("string", nameResult.ToString());
        }

        if (ageResult is object?[] ageArray)
        {
            Assert.Single(ageArray);
            Assert.True(ageArray[0]?.ToString() == "integer" || ageArray[0]?.ToString() == "number",
                $"Expected 'integer' or 'number' but got '{ageArray[0]}'");
        }
        else
        {
            Assert.True(ageResult.ToString() == "integer" || ageResult.ToString() == "number",
                $"Expected 'integer' or 'number' but got '{ageResult}'");
        }

        if (activeResult is object?[] activeArray)
        {
            Assert.Single(activeArray);
            Assert.Equal("boolean", activeArray[0]?.ToString());
        }
        else
        {
            Assert.Equal("boolean", activeResult.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithLegacyPath_ReturnsSingleType(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(standaloneClient, key, ".name");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(clusterClient, key, ".name");
        }

        Assert.NotNull(result);
        // Legacy path returns a single type string
        Assert.Equal("string", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        object? result;
        if (client is GlideClient standaloneClient)
        {
            result = await GlideJson.TypeAsync(standaloneClient, key);
        }
        else
        {
            result = await GlideJson.TypeAsync((GlideClusterClient)client, key);
        }

        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithGlideString_ReturnsType(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"name\":\"Jane\",\"items\":[1,2,3]}";

        object? rootResult;
        object? nameResult;
        object? itemsResult;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);
            rootResult = await GlideJson.TypeAsync(standaloneClient, key);
            nameResult = await GlideJson.TypeAsync(standaloneClient, key, (GlideString)"$.name");
            itemsResult = await GlideJson.TypeAsync(standaloneClient, key, (GlideString)"$.items");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);
            rootResult = await GlideJson.TypeAsync(clusterClient, key);
            nameResult = await GlideJson.TypeAsync(clusterClient, key, (GlideString)"$.name");
            itemsResult = await GlideJson.TypeAsync(clusterClient, key, (GlideString)"$.items");
        }

        Assert.NotNull(rootResult);
        Assert.Equal("object", rootResult.ToString());

        Assert.NotNull(nameResult);
        Assert.NotNull(itemsResult);

        // Check name type (should be string)
        if (nameResult is object?[] nameArray)
        {
            Assert.Single(nameArray);
            Assert.Equal("string", nameArray[0]?.ToString());
        }
        else
        {
            Assert.Equal("string", nameResult.ToString());
        }

        // Check items type (should be array)
        if (itemsResult is object?[] itemsArray)
        {
            Assert.Single(itemsArray);
            Assert.Equal("array", itemsArray[0]?.ToString());
        }
        else
        {
            Assert.Equal("array", itemsResult.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NestedTypes_ReturnsCorrectTypes(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"scores\":[90,85,92]},\"active\":true}";

        object? personResult;
        object? scoresResult;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            personResult = await GlideJson.TypeAsync(standaloneClient, key, "$.person");
            scoresResult = await GlideJson.TypeAsync(standaloneClient, key, "$.person.scores");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            personResult = await GlideJson.TypeAsync(clusterClient, key, "$.person");
            scoresResult = await GlideJson.TypeAsync(clusterClient, key, "$.person.scores");
        }

        Assert.NotNull(personResult);
        Assert.NotNull(scoresResult);

        // Check person type (should be object)
        if (personResult is object?[] personArray)
        {
            Assert.Single(personArray);
            Assert.Equal("object", personArray[0]?.ToString());
        }
        else
        {
            Assert.Equal("object", personResult.ToString());
        }

        // Check scores type (should be array)
        if (scoresResult is object?[] scoresArray)
        {
            Assert.Single(scoresArray);
            Assert.Equal("array", scoresArray[0]?.ToString());
        }
        else
        {
            Assert.Equal("array", scoresResult.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WildcardPath_ReturnsMultipleTypes(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"value\":\"text\"},{\"value\":42},{\"value\":true}]}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(standaloneClient, key, "$.items[*].value");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.TypeAsync(clusterClient, key, "$.items[*].value");
        }

        Assert.NotNull(result);

        // Wildcard path should return an array of types
        if (result is object?[] typesArray)
        {
            Assert.Equal(3, typesArray.Length);
            Assert.Equal("string", typesArray[0]?.ToString());
            Assert.True(typesArray[1]?.ToString() == "integer" || typesArray[1]?.ToString() == "number",
                $"Expected 'integer' or 'number' but got '{typesArray[1]}'");
            Assert.Equal("boolean", typesArray[2]?.ToString());
        }
    }

    #endregion

    #region JSON.NUMINCRBY Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_IncrementInteger_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, "$.count", 5);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(clusterClient, key, "$.count", 5);
        }

        Assert.NotNull(result);
        // JSONPath returns array of results
        Assert.Equal("[15]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_IncrementFloat_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"price\":10.5}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, "$.price", 2.5);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(clusterClient, key, "$.price", 2.5);
        }

        Assert.NotNull(result);
        // JSONPath returns array of results
        Assert.Equal("[13]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_DecrementWithNegativeValue_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":20}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, "$.count", -7);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(clusterClient, key, "$.count", -7);
        }

        Assert.NotNull(result);
        // JSONPath returns array of results
        Assert.Equal("[13]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_WithJsonPath_ReturnsArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":1,\"b\":2,\"nested\":{\"c\":3}}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Increment all numeric values at root level
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, "$.a", 10);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(clusterClient, key, "$.a", 10);
        }

        Assert.NotNull(result);
        // JSONPath returns array of results
        Assert.Equal("[11]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_WithLegacyPath_ReturnsSingleValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":100}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Legacy path (no $ prefix)
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, ".count", 25);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Legacy path (no $ prefix)
            result = await GlideJson.NumIncrByAsync(clusterClient, key, ".count", 25);
        }

        Assert.NotNull(result);
        // Legacy path returns single value (not array)
        Assert.Equal("125", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_NonNumericValue_ThrowsError(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Trying to increment a string value with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.NumIncrByAsync(standaloneClient, key, ".name", 5));
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Trying to increment a string value with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.NumIncrByAsync(clusterClient, key, ".name", 5));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_WithGlideString_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"value\":50}";
        GlideString incrPath = "$.value";

        GlideString? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, incrPath, 25);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);
            result = await GlideJson.NumIncrByAsync(clusterClient, key, incrPath, 25);
        }

        Assert.NotNull(result);
        Assert.Equal("[75]", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_MultipleNumericValues_ReturnsArrayOfResults(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"count\":1},{\"count\":2},{\"count\":3}]}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Increment all count values using wildcard
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, "$.items[*].count", 10);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Increment all count values using wildcard
            result = await GlideJson.NumIncrByAsync(clusterClient, key, "$.items[*].count", 10);
        }

        Assert.NotNull(result);
        // Should return array with all incremented values
        Assert.Contains("11", result);
        Assert.Contains("12", result);
        Assert.Contains("13", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_NonExistentPath_ThrowsError(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";

        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Trying to increment non-existent path with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.NumIncrByAsync(standaloneClient, key, ".nonexistent", 5));
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Trying to increment non-existent path with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.NumIncrByAsync(clusterClient, key, ".nonexistent", 5));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_IncrementByZero_ReturnsSameValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, "$.count", 0);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(clusterClient, key, "$.count", 0);
        }

        Assert.NotNull(result);
        Assert.Equal("[42]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_NegativeToPositive_ReturnsCorrectValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"value\":-10}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(standaloneClient, key, "$.value", 25);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumIncrByAsync(clusterClient, key, "$.value", 25);
        }

        Assert.NotNull(result);
        Assert.Equal("[15]", result);
    }

    #endregion

    #region JSON.NUMMULTBY Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyInteger_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(standaloneClient, key, "$.count", 3);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(clusterClient, key, "$.count", 3);
        }

        Assert.NotNull(result);
        // JSONPath returns array of results
        Assert.Equal("[30]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyFloat_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"price\":10.5}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(standaloneClient, key, "$.price", 2);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(clusterClient, key, "$.price", 2);
        }

        Assert.NotNull(result);
        // JSONPath returns array of results
        Assert.Equal("[21]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyByZero_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(standaloneClient, key, "$.count", 0);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(clusterClient, key, "$.count", 0);
        }

        Assert.NotNull(result);
        Assert.Equal("[0]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyByNegative_ReturnsNegativeValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(standaloneClient, key, "$.count", -3);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(clusterClient, key, "$.count", -3);
        }

        Assert.NotNull(result);
        // JSONPath returns array of results
        Assert.Equal("[-30]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_WithJsonPath_ReturnsArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":2,\"b\":3,\"nested\":{\"c\":4}}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(standaloneClient, key, "$.a", 5);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.NumMultByAsync(clusterClient, key, "$.a", 5);
        }

        Assert.NotNull(result);
        // JSONPath returns array of results
        Assert.Equal("[10]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_WithLegacyPath_ReturnsSingleValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":5}";

        string? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Legacy path (no $ prefix)
            result = await GlideJson.NumMultByAsync(standaloneClient, key, ".count", 4);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Legacy path (no $ prefix)
            result = await GlideJson.NumMultByAsync(clusterClient, key, ".count", 4);
        }

        Assert.NotNull(result);
        // Legacy path returns single value (not array)
        Assert.Equal("20", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_NonNumericValue_ThrowsError(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Trying to multiply a string value with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.NumMultByAsync(standaloneClient, key, ".name", 5));
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Trying to multiply a string value with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.NumMultByAsync(clusterClient, key, ".name", 5));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_WithGlideString_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"value\":7}";
        GlideString multPath = "$.value";

        GlideString? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);
            result = await GlideJson.NumMultByAsync(standaloneClient, key, multPath, 6);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);
            result = await GlideJson.NumMultByAsync(clusterClient, key, multPath, 6);
        }

        Assert.NotNull(result);
        Assert.Equal("[42]", result.ToString());
    }

    #endregion

    #region JSON.STRAPPEND Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_AppendToString_ReturnsNewLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"greeting\":\"Hello\"}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.StrAppendAsync(standaloneClient, key, "$.greeting", "\" World\"");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.StrAppendAsync(clusterClient, key, "$.greeting", "\" World\"");
        }

        Assert.NotNull(result);
        // JSONPath returns array of lengths
        // "Hello" (5) + " World" (6) = 11
        if (result is object?[] arr)
        {
            Assert.Single(arr);
            Assert.Equal(11L, Convert.ToInt64(arr[0]));
        }
        else
        {
            Assert.Equal(11L, Convert.ToInt64(result));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithJsonPath_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":\"foo\",\"nested\":{\"b\":\"bar\"}}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.StrAppendAsync(standaloneClient, key, "$.a", "\"baz\"");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.StrAppendAsync(clusterClient, key, "$.a", "\"baz\"");
        }

        Assert.NotNull(result);
        // "foo" (3) + "baz" (3) = 6
        if (result is object?[] arr)
        {
            Assert.Single(arr);
            Assert.Equal(6L, Convert.ToInt64(arr[0]));
        }
        else
        {
            Assert.Equal(6L, Convert.ToInt64(result));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithLegacyPath_ReturnsSingleLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Legacy path (no $ prefix)
            result = await GlideJson.StrAppendAsync(standaloneClient, key, ".name", "\" Doe\"");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Legacy path (no $ prefix)
            result = await GlideJson.StrAppendAsync(clusterClient, key, ".name", "\" Doe\"");
        }

        Assert.NotNull(result);
        // "John" (4) + " Doe" (4) = 8
        // Legacy path returns single value (not array)
        Assert.Equal(8L, Convert.ToInt64(result));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_NonStringValue_ThrowsError(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";

        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Trying to append to a number value with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.StrAppendAsync(standaloneClient, key, ".count", "\"test\""));
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Trying to append to a number value with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.StrAppendAsync(clusterClient, key, ".count", "\"test\""));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithGlideString_ReturnsNewLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"text\":\"Hello\"}";
        GlideString appendPath = "$.text";
        GlideString appendValue = "\" World\"";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);
            result = await GlideJson.StrAppendAsync(standaloneClient, key, appendPath, appendValue);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);
            result = await GlideJson.StrAppendAsync(clusterClient, key, appendPath, appendValue);
        }

        Assert.NotNull(result);
        // "Hello" (5) + " World" (6) = 11
        if (result is object?[] arr)
        {
            Assert.Single(arr);
            Assert.Equal(11L, Convert.ToInt64(arr[0]));
        }
        else
        {
            Assert.Equal(11L, Convert.ToInt64(result));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_AppendEmptyString_ReturnsSameLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"text\":\"Hello\"}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.StrAppendAsync(standaloneClient, key, "$.text", "\"\"");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.StrAppendAsync(clusterClient, key, "$.text", "\"\"");
        }

        Assert.NotNull(result);
        // "Hello" (5) + "" (0) = 5
        if (result is object?[] arr)
        {
            Assert.Single(arr);
            Assert.Equal(5L, Convert.ToInt64(arr[0]));
        }
        else
        {
            Assert.Equal(5L, Convert.ToInt64(result));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_RootStringValue_ReturnsNewLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        // Set a root-level string value
        string jsonValue = "\"Hello\"";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Append to root string (no path specified)
            result = await GlideJson.StrAppendAsync(standaloneClient, key, "\" World\"");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Append to root string (no path specified)
            result = await GlideJson.StrAppendAsync(clusterClient, key, "\" World\"");
        }

        Assert.NotNull(result);
        // "Hello" (5) + " World" (6) = 11
        Assert.Equal(11L, Convert.ToInt64(result));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_MultipleStringValues_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"bb\"},{\"name\":\"ccc\"}]}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Append to all name values using wildcard
            result = await GlideJson.StrAppendAsync(standaloneClient, key, "$.items[*].name", "\"x\"");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Append to all name values using wildcard
            result = await GlideJson.StrAppendAsync(clusterClient, key, "$.items[*].name", "\"x\"");
        }

        Assert.NotNull(result);
        // Should return array of new lengths: [2, 3, 4] (a+x=2, bb+x=3, ccc+x=4)
        if (result is object?[] arr)
        {
            Assert.Equal(3, arr.Length);
            Assert.Equal(2L, Convert.ToInt64(arr[0]));
            Assert.Equal(3L, Convert.ToInt64(arr[1]));
            Assert.Equal(4L, Convert.ToInt64(arr[2]));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_NonExistentPath_ThrowsError(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Trying to append to non-existent path with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.StrAppendAsync(standaloneClient, key, ".nonexistent", "\"test\""));
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Trying to append to non-existent path with legacy path should throw
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await GlideJson.StrAppendAsync(clusterClient, key, ".nonexistent", "\"test\""));
        }
    }

    #endregion

    #region JSON.STRLEN Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_GetStringLength_ReturnsLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"greeting\":\"Hello\"}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.StrLenAsync(standaloneClient, key, "$.greeting");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.StrLenAsync(clusterClient, key, "$.greeting");
        }

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        object?[] arr = (object?[])result;
        Assert.Single(arr);
        Assert.Equal(5L, Convert.ToInt64(arr[0])); // "Hello" has 5 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_WithJsonPath_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":\"foo\",\"nested\":{\"a\":\"hello\"}}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.StrLenAsync(standaloneClient, key, "$..a");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.StrLenAsync(clusterClient, key, "$..a");
        }

        Assert.NotNull(result);
        // JSONPath returns an array of lengths for all matching paths
        object?[] arr = (object?[])result;
        Assert.Equal(2, arr.Length);
        // "foo" has 3 characters, "hello" has 5 characters
        long[] lengths = arr.Select(x => Convert.ToInt64(x)).OrderBy(x => x).ToArray();
        Assert.Contains(3L, lengths);
        Assert.Contains(5L, lengths);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_WithLegacyPath_ReturnsSingleLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Legacy path (no $ prefix)
            result = await GlideJson.StrLenAsync(standaloneClient, key, ".name");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Legacy path (no $ prefix)
            result = await GlideJson.StrLenAsync(clusterClient, key, ".name");
        }

        Assert.NotNull(result);
        // Legacy path returns a single length value
        Assert.Equal(4L, Convert.ToInt64(result)); // "John" has 4 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        object? result;
        if (client is GlideClient standaloneClient)
        {
            result = await GlideJson.StrLenAsync(standaloneClient, key);
        }
        else
        {
            result = await GlideJson.StrLenAsync((GlideClusterClient)client, key);
        }

        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_WithGlideString_ReturnsLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        GlideString key = GetUniqueKey();
        GlideString path = "$";
        GlideString jsonValue = "{\"text\":\"Hello World\"}";
        GlideString strPath = "$.text";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, path, jsonValue);
            result = await GlideJson.StrLenAsync(standaloneClient, key, strPath);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, path, jsonValue);
            result = await GlideJson.StrLenAsync(clusterClient, key, strPath);
        }

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        object?[] arr = (object?[])result;
        Assert.Single(arr);
        Assert.Equal(11L, Convert.ToInt64(arr[0])); // "Hello World" has 11 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_EmptyString_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"empty\":\"\"}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            result = await GlideJson.StrLenAsync(standaloneClient, key, "$.empty");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            result = await GlideJson.StrLenAsync(clusterClient, key, "$.empty");
        }

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        object?[] arr = (object?[])result;
        Assert.Single(arr);
        Assert.Equal(0L, Convert.ToInt64(arr[0])); // Empty string has 0 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_RootStringValue_ReturnsLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "\"Hello World\"";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Get length of root string (no path specified)
            result = await GlideJson.StrLenAsync(standaloneClient, key);
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Get length of root string (no path specified)
            result = await GlideJson.StrLenAsync(clusterClient, key);
        }

        Assert.NotNull(result);
        Assert.Equal(11L, Convert.ToInt64(result)); // "Hello World" has 11 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_NonStringValue_ReturnsNullInArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Get length of non-string value with JSONPath
            result = await GlideJson.StrLenAsync(standaloneClient, key, "$.age");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Get length of non-string value with JSONPath
            result = await GlideJson.StrLenAsync(clusterClient, key, "$.age");
        }

        Assert.NotNull(result);
        // JSONPath returns an array with null for non-string matches
        object?[] arr = (object?[])result;
        Assert.Single(arr);
        Assert.Null(arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_MultipleStringValues_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"bb\"},{\"name\":\"ccc\"}]}";

        object? result;
        if (client is GlideClient standaloneClient)
        {
            _ = await GlideJson.SetAsync(standaloneClient, key, "$", jsonValue);
            // Get length of all name values using wildcard
            result = await GlideJson.StrLenAsync(standaloneClient, key, "$.items[*].name");
        }
        else
        {
            var clusterClient = (GlideClusterClient)client;
            _ = await GlideJson.SetAsync(clusterClient, key, "$", jsonValue);
            // Get length of all name values using wildcard
            result = await GlideJson.StrLenAsync(clusterClient, key, "$.items[*].name");
        }

        Assert.NotNull(result);
        // JSONPath returns an array of lengths for all matching paths
        object?[] arr = (object?[])result;
        Assert.Equal(3, arr.Length);
        Assert.Equal(1L, Convert.ToInt64(arr[0])); // "a" has 1 character
        Assert.Equal(2L, Convert.ToInt64(arr[1])); // "bb" has 2 characters
        Assert.Equal(3L, Convert.ToInt64(arr[2])); // "ccc" has 3 characters
    }

    #endregion
}

