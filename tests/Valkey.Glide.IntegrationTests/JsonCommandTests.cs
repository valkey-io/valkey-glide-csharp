// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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

        ValkeyValue result = await GlideJson.SetAsync(client, key, "$", jsonValue);

        Assert.Equal("OK", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithGlideString_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        ValkeyValue result = await GlideJson.SetAsync(client, key, path, jsonValue);

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

        // Set initial value
        ValkeyValue result1 = await GlideJson.SetAsync(client, key, "$", initialValue);
        Assert.Equal("OK", result1);

        // Overwrite with new value
        ValkeyValue result2 = await GlideJson.SetAsync(client, key, "$", newValue);
        Assert.Equal("OK", result2);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithNxCondition_KeyDoesNotExist_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        ValkeyValue result = await GlideJson.SetAsync(client, key, "$", jsonValue, GlideJson.SetCondition.OnlyIfDoesNotExist);

        Assert.Equal("OK", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithNxCondition_KeyExists_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string initialValue = "{\"name\":\"John\"}";
        string newValue = "{\"name\":\"Jane\"}";            // Set initial value
        _ = await GlideJson.SetAsync(client, key, "$", initialValue);

        // Try to set with condition
        ValkeyValue result = await GlideJson.SetAsync(client, key, "$", newValue, GlideJson.SetCondition.OnlyIfDoesNotExist);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithXxCondition_KeyExists_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string initialValue = "{\"name\":\"John\"}";
        string newValue = "{\"name\":\"Jane\"}";            // Set initial value
        _ = await GlideJson.SetAsync(client, key, "$", initialValue);

        // Try to set with condition
        ValkeyValue result = await GlideJson.SetAsync(client, key, "$", newValue, GlideJson.SetCondition.OnlyIfExists);
        Assert.Equal("OK", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithXxCondition_KeyDoesNotExist_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        ValkeyValue result = await GlideJson.SetAsync(client, key, "$", jsonValue, GlideJson.SetCondition.OnlyIfExists);

        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SetNestedPath_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string initialValue = "{\"person\":{\"name\":\"John\"}}";
        string newNameValue = "\"Jane\"";            // Set initial value
        _ = await GlideJson.SetAsync(client, key, "$", initialValue);

        // Try to set with condition
        ValkeyValue result = await GlideJson.SetAsync(client, key, "$.person.name", newNameValue);
        Assert.Equal("OK", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SetArrayValue_ReturnsOk(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "[1, 2, 3, 4, 5]";

        ValkeyValue result = await GlideJson.SetAsync(client, key, "$", jsonValue);

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
        // Set string value
        string? resultString = await GlideJson.SetAsync(client, keyString, "$", "\"hello\"");
        Assert.Equal("OK", resultString);

        // Set number value
        string? resultNumber = await GlideJson.SetAsync(client, keyNumber, "$", "42");
        Assert.Equal("OK", resultNumber);

        // Set boolean value
        string? resultBool = await GlideJson.SetAsync(client, keyBool, "$", "true");
        Assert.Equal("OK", resultBool);

        // Set null value
        string? resultNull = await GlideJson.SetAsync(client, keyNull, "$", "null");
        Assert.Equal("OK", resultNull);

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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key);

        Assert.False(result.IsNull);
        // The result should contain the JSON document
        Assert.Contains("\"name\"", result.ToString());
        Assert.Contains("\"John\"", result.ToString());
        Assert.Contains("\"age\"", result.ToString());
        Assert.Contains("30", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithJsonPath_ReturnsArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key, ["$.name"]);

        Assert.False(result.IsNull);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key, [".name"]);

        Assert.False(result.IsNull);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key, ["$.name", "$.age"]);

        Assert.False(result.IsNull);
        // Multiple paths return a JSON object with each path as a key
        Assert.Contains("$.name", result.ToString());
        Assert.Contains("$.age", result.ToString());
        Assert.Contains("\"John\"", result.ToString());
        Assert.Contains("30", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithJsonGetOptions_ReturnsFormattedJson(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";
        var options = new GlideJson.GetOptions { Indent = "  ", Newline = "\n", Space = " " };

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key, options);

        Assert.False(result.IsNull);
        // The result should be formatted with newlines and indentation
        Assert.Contains("\n", result.ToString());
        Assert.Contains("  ", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithPathsAndOptions_ReturnsFormattedJson(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"age\":30}}";
        var options = new GlideJson.GetOptions { Indent = "\t", Newline = "\n" };

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key, ["$.person"], options);

        Assert.False(result.IsNull);
        // The result should be formatted with tabs and newlines
        Assert.Contains("\n", result.ToString());
        Assert.Contains("\t", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        ValkeyValue result = await GlideJson.GetAsync(client, key);

        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_SetGetRoundTrip_ReturnsOriginalValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"active\":true,\"scores\":[1,2,3]}";
        // Set the value
        string? setResult = await GlideJson.SetAsync(client, key, "$", jsonValue);
        Assert.Equal("OK", setResult);

        // Get the value back
        string? getResult = await GlideJson.GetAsync(client, key);
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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithGlideString_ReturnsDocument(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, path, jsonValue);
        result = await GlideJson.GetAsync(client, key);

        Assert.False(result.IsNull);
        string resultStr = result.ToString();
        Assert.Contains("\"name\"", resultStr);
        Assert.Contains("\"Jane\"", resultStr);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithGlideStringPaths_ReturnsValues(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        ValkeyValue[] paths = ["$.name"];

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, path, jsonValue);
        result = await GlideJson.GetAsync(client, key, paths);

        Assert.False(result.IsNull);
        Assert.Equal("[\"Jane\"]", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_NestedObject_ReturnsNestedValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"address\":{\"city\":\"NYC\",\"zip\":\"10001\"}}}";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key, ["$.person.address.city"]);

        Assert.False(result.IsNull);
        Assert.Equal("[\"NYC\"]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_ArrayElement_ReturnsElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"numbers\":[10,20,30,40,50]}";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key, ["$.numbers[2]"]);

        Assert.False(result.IsNull);
        Assert.Equal("[30]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WildcardPath_ReturnsAllMatches(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"b\"},{\"name\":\"c\"}]}";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.GetAsync(client, key, ["$.items[*].name"]);

        Assert.False(result.IsNull);
        // Should return all matching names as an array
        Assert.Contains("\"a\"", result.ToString());
        Assert.Contains("\"b\"", result.ToString());
        Assert.Contains("\"c\"", result.ToString());
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
        // Set and get string value
        _ = await GlideJson.SetAsync(client, keyString, "$", "\"hello\"");
        string? resultString = await GlideJson.GetAsync(client, keyString);
        Assert.Equal("\"hello\"", resultString);

        // Set and get number value
        _ = await GlideJson.SetAsync(client, keyNumber, "$", "42");
        string? resultNumber = await GlideJson.GetAsync(client, keyNumber);
        Assert.Equal("42", resultNumber);

        // Set and get boolean value
        _ = await GlideJson.SetAsync(client, keyBool, "$", "true");
        string? resultBool = await GlideJson.GetAsync(client, keyBool);
        Assert.Equal("true", resultBool);

        // Set and get null value
        _ = await GlideJson.SetAsync(client, keyNull, "$", "null");
        string? resultNull = await GlideJson.GetAsync(client, keyNull);
        Assert.Equal("null", resultNull);

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
        // Set up JSON documents
        _ = await GlideJson.SetAsync(client, key1, "$", "{\"name\":\"John\",\"age\":30}");
        _ = await GlideJson.SetAsync(client, key2, "$", "{\"name\":\"Jane\",\"age\":25}");
        _ = await GlideJson.SetAsync(client, key3, "$", "{\"name\":\"Bob\",\"age\":35}");

        // Get values from multiple keys with JSONPath
        ValkeyValue[] results = await GlideJson.MGetAsync(client, [key1, key2, key3], "$.name");

        Assert.Equal(3, results.Length);
        Assert.Equal("[\"John\"]", results[0]);
        Assert.Equal("[\"Jane\"]", results[1]);
        Assert.Equal("[\"Bob\"]", results[2]);

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
        // Set up JSON documents (only key1 and key2)
        _ = await GlideJson.SetAsync(client, key1, "$", "{\"name\":\"John\"}");
        _ = await GlideJson.SetAsync(client, key2, "$", "{\"name\":\"Jane\"}");

        // Get values including a non-existent key
        ValkeyValue[] results = await GlideJson.MGetAsync(client, [key1, nonExistentKey, key2], "$.name");

        Assert.Equal(3, results.Length);
        Assert.Equal("[\"John\"]", results[0]);
        Assert.True(results[1].IsNull); // Non-existent key returns null
        Assert.Equal("[\"Jane\"]", results[2]);

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_WithJsonPath_ReturnsArrays(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        // Set up JSON documents with arrays
        _ = await GlideJson.SetAsync(client, key1, "$", "{\"items\":[1,2,3]}");
        _ = await GlideJson.SetAsync(client, key2, "$", "{\"items\":[4,5,6]}");

        // Get values with JSONPath - should return arrays
        ValkeyValue[] results = await GlideJson.MGetAsync(client, [key1, key2], "$.items");

        Assert.Equal(2, results.Length);
        Assert.False(results[0].IsNull);
        Assert.False(results[1].IsNull);
        // JSONPath returns arrays of matching values
        Assert.Contains("[1,2,3]", results[0].ToString());
        Assert.Contains("[4,5,6]", results[1].ToString());

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_WithLegacyPath_ReturnsSingleValues(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        // Set up JSON documents
        _ = await GlideJson.SetAsync(client, key1, "$", "{\"name\":\"John\"}");
        _ = await GlideJson.SetAsync(client, key2, "$", "{\"name\":\"Jane\"}");

        // Get values with legacy path (no $ prefix)
        ValkeyValue[] results = await GlideJson.MGetAsync(client, [key1, key2], ".name");

        Assert.Equal(2, results.Length);
        Assert.Equal("\"John\"", results[0]);
        Assert.Equal("\"Jane\"", results[1]);

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_WithGlideString_ReturnsValues(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        string path = "$.name";
        // Set up JSON documents
        _ = await GlideJson.SetAsync(client, key1, "$", (GlideString)"{\"name\":\"John\"}");
        _ = await GlideJson.SetAsync(client, key2, "$", (GlideString)"{\"name\":\"Jane\"}");

        // Get values using GlideString overload
        ValkeyValue[] results = await GlideJson.MGetAsync(client, [key1, key2], path);

        Assert.Equal(2, results.Length);
        Assert.False(results[0].IsNull);
        Assert.False(results[1].IsNull);
        Assert.Equal("[\"John\"]", results[0].ToString());
        Assert.Equal("[\"Jane\"]", results[1].ToString());

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_PathDoesNotExist_ReturnsNullForThoseKeys(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        // Set up JSON documents with different structures
        _ = await GlideJson.SetAsync(client, key1, "$", "{\"name\":\"John\"}");
        _ = await GlideJson.SetAsync(client, key2, "$", "{\"age\":25}"); // No "name" field

        // Get values with legacy path - key2 doesn't have "name"
        ValkeyValue[] results = await GlideJson.MGetAsync(client, [key1, key2], ".name");

        Assert.Equal(2, results.Length);
        Assert.Equal("\"John\"", results[0]);
        Assert.True(results[1].IsNull); // Path doesn't exist in key2

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_RootPath_ReturnsEntireDocuments(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        // Set up JSON documents
        _ = await GlideJson.SetAsync(client, key1, "$", "{\"a\":1}");
        _ = await GlideJson.SetAsync(client, key2, "$", "{\"b\":2}");

        // Get entire documents with root path
        ValkeyValue[] results = await GlideJson.MGetAsync(client, [key1, key2], "$");

        Assert.Equal(2, results.Length);
        Assert.False(results[0].IsNull);
        Assert.False(results[1].IsNull);
        Assert.Contains("\"a\"", results[0].ToString());
        Assert.Contains("1", results[0].ToString());
        Assert.Contains("\"b\"", results[1].ToString());
        Assert.Contains("2", results[1].ToString());

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
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Delete entire document
        deleted = await GlideJson.DelAsync(client, key);


        Assert.Equal(1, deleted);

        // Verify the key no longer exists
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_WithPath_DeletesSpecificElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}";

        long deleted;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Delete specific path
        deleted = await GlideJson.DelAsync(client, key, "$.age");


        Assert.Equal(1, deleted);

        // Verify the path was deleted but document still exists
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.False(result.IsNull);
        Assert.Contains("\"name\"", result.ToString());
        Assert.Contains("\"city\"", result.ToString());
        Assert.DoesNotContain("\"age\"", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_NonExistentKey_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        long deleted = await GlideJson.DelAsync(client, key);

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
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Try to delete non-existent path
        deleted = await GlideJson.DelAsync(client, key, "$.nonexistent");


        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_WithGlideString_DeletesDocument(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        long deleted;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, path, jsonValue);

        // Delete entire document using GlideString overload
        deleted = await GlideJson.DelAsync(client, key);


        Assert.Equal(1, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_WithGlideStringPath_DeletesSpecificElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        string deletePath = "$.age";

        long deleted;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, path, jsonValue);

        // Delete specific path using GlideString overload
        deleted = await GlideJson.DelAsync(client, key, deletePath);


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
        // Set up JSON document with array of objects
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Delete all "name" fields using wildcard path
        deleted = await GlideJson.DelAsync(client, key, "$.items[*].name");


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
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Forget (delete) entire document
        deleted = await GlideJson.ForgetAsync(client, key);


        Assert.Equal(1, deleted);

        // Verify the key no longer exists
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_WithPath_DeletesSpecificElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}";

        long deleted;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Forget specific path
        deleted = await GlideJson.ForgetAsync(client, key, "$.city");


        Assert.Equal(1, deleted);

        // Verify the path was deleted but document still exists
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.False(result.IsNull);
        Assert.Contains("\"name\"", result.ToString());
        Assert.Contains("\"age\"", result.ToString());
        Assert.DoesNotContain("\"city\"", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_NonExistentKey_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        long deleted = await GlideJson.ForgetAsync(client, key);

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
        // Set up identical JSON documents
        _ = await GlideJson.SetAsync(client, keyDel, "$", jsonValue);
        _ = await GlideJson.SetAsync(client, keyForget, "$", jsonValue);

        // Delete using Del
        long deletedDel = await GlideJson.DelAsync(client, keyDel, "$.age");
        // Delete using Forget
        long deletedForget = await GlideJson.ForgetAsync(client, keyForget, "$.age");

        // Both should return the same count
        Assert.Equal(deletedDel, deletedForget);

        // Both documents should be in the same state
        string? resultDel = await GlideJson.GetAsync(client, keyDel);
        string? resultForget = await GlideJson.GetAsync(client, keyForget);

        Assert.NotNull(resultDel);
        Assert.NotNull(resultForget);
        Assert.Equal(resultDel, resultForget);

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_WithGlideString_DeletesDocument(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        long deleted;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, path, jsonValue);

        // Forget entire document using GlideString overload
        deleted = await GlideJson.ForgetAsync(client, key);


        Assert.Equal(1, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_WithGlideStringPath_DeletesSpecificElement(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        string deletePath = "$.name";

        long deleted;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, path, jsonValue);

        // Forget specific path using GlideString overload
        deleted = await GlideJson.ForgetAsync(client, key, deletePath);


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
        ValkeyValue result;
        // Set up JSON document with array
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the array
        cleared = await GlideJson.ClearAsync(client, key, "$.items");

        // Get the result
        result = await GlideJson.GetAsync(client, key, ["$.items"]);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
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
        ValkeyValue result;
        // Set up JSON document with nested object
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the nested object
        cleared = await GlideJson.ClearAsync(client, key, "$.data");

        // Get the result
        result = await GlideJson.GetAsync(client, key, ["$.data"]);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
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
        ValkeyValue result;
        // Set up JSON document with number
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the number
        cleared = await GlideJson.ClearAsync(client, key, "$.count");

        // Get the result
        result = await GlideJson.GetAsync(client, key, ["$.count"]);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
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
        ValkeyValue result;
        // Set up JSON document with boolean
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the boolean - Valkey JSON clears booleans to false
        cleared = await GlideJson.ClearAsync(client, key, "$.active");

        // Get the result
        result = await GlideJson.GetAsync(client, key, ["$.active"]);


        // Valkey JSON clears booleans to false and returns 1
        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
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
        ValkeyValue result;
        // Set up JSON document with string
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the string - Valkey JSON clears strings to empty string
        cleared = await GlideJson.ClearAsync(client, key, "$.name");

        // Get the result
        result = await GlideJson.GetAsync(client, key, ["$.name"]);


        // Valkey JSON clears strings to empty string and returns 1
        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
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
        var ex = await Assert.ThrowsAsync<Errors.RequestException>(
            () => GlideJson.ClearAsync(client, key));
        Assert.Contains("NONEXISTENT", ex.Message);

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_WithGlideString_ClearsArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"items\":[1,2,3]}";
        GlideString clearPath = "$.items";

        long cleared;
        ValkeyValue result;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, path, jsonValue);

        // Clear using GlideString overload
        cleared = await GlideJson.ClearAsync(client, key, clearPath);

        // Get the result
        result = await GlideJson.GetAsync(client, key, [clearPath]);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
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
        ValkeyValue result;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear at root path
        cleared = await GlideJson.ClearAsync(client, key, "$");

        // Get the result
        result = await GlideJson.GetAsync(client, key);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
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
        ValkeyValue result;
        // Set up JSON document with multiple arrays
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear all "data" arrays using wildcard path
        cleared = await GlideJson.ClearAsync(client, key, "$.items[*].data");

        // Get the result
        result = await GlideJson.GetAsync(client, key, ["$.items[*].data"]);


        // Should clear 3 arrays
        Assert.Equal(3, cleared);
        Assert.False(result.IsNull);
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
        ValkeyValue result;
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear without specifying path (should clear root)
        cleared = await GlideJson.ClearAsync(client, key);

        // Get the result
        result = await GlideJson.GetAsync(client, key);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
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
        // Set up JSON document
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Try to clear non-existent path
        cleared = await GlideJson.ClearAsync(client, key, "$.nonexistent");


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

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.TypeAsync(client, key);

        Assert.NotNull(result);
        Assert.Equal("object", (string?)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_ArrayType_ReturnsArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "[1, 2, 3, 4, 5]";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.TypeAsync(client, key);

        Assert.NotNull(result);
        Assert.Equal("array", (string?)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_StringType_ReturnsString(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "\"hello world\"";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.TypeAsync(client, key);

        Assert.NotNull(result);
        Assert.Equal("string", (string?)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NumberType_ReturnsNumberOrInteger(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string keyInteger = GetUniqueKey("integer");
        string keyFloat = GetUniqueKey("float");

        ValkeyResult intResult;
        ValkeyResult floatResult;
        _ = await GlideJson.SetAsync(client, keyInteger, "$", "42");
        _ = await GlideJson.SetAsync(client, keyFloat, "$", "3.14");
        intResult = await GlideJson.TypeAsync(client, keyInteger);
        floatResult = await GlideJson.TypeAsync(client, keyFloat);


        Assert.NotNull(intResult);
        Assert.NotNull(floatResult);
        // Integer values may return "integer" or "number" depending on the JSON module version
        string? intType = (string?)intResult;
        Assert.True(intType == "integer" || intType == "number",
            $"Expected 'integer' or 'number' but got '{intType}'");
        Assert.Equal("number", (string?)floatResult);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_BooleanType_ReturnsBoolean(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string keyTrue = GetUniqueKey("true");
        string keyFalse = GetUniqueKey("false");

        ValkeyResult trueResult;
        ValkeyResult falseResult;
        _ = await GlideJson.SetAsync(client, keyTrue, "$", "true");
        _ = await GlideJson.SetAsync(client, keyFalse, "$", "false");
        trueResult = await GlideJson.TypeAsync(client, keyTrue);
        falseResult = await GlideJson.TypeAsync(client, keyFalse);


        Assert.NotNull(trueResult);
        Assert.NotNull(falseResult);
        Assert.Equal("boolean", (string?)trueResult);
        Assert.Equal("boolean", (string?)falseResult);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NullType_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "null";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.TypeAsync(client, key);

        Assert.NotNull(result);
        Assert.Equal("null", (string?)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithJsonPath_ReturnsArrayOfTypes(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"active\":true}";

        ValkeyResult nameResult;
        ValkeyResult ageResult;
        ValkeyResult activeResult;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        nameResult = await GlideJson.TypeAsync(client, key, "$.name");
        ageResult = await GlideJson.TypeAsync(client, key, "$.age");
        activeResult = await GlideJson.TypeAsync(client, key, "$.active");


        // JSONPath returns an array of types
        Assert.NotNull(nameResult);
        Assert.NotNull(ageResult);
        Assert.NotNull(activeResult);

        // Results should be arrays containing the type strings
        // ValkeyResult wraps the array, so we need to access elements via indexer or cast to array
        if (nameResult.Length > 0)
        {
            Assert.Equal(1, nameResult.Length);
            Assert.Equal("string", (string?)nameResult[0]);
        }
        else
        {
            // Some implementations may return the type directly
            Assert.Equal("string", (string?)nameResult);
        }

        if (ageResult.Length > 0)
        {
            Assert.Equal(1, ageResult.Length);
            string? ageType = (string?)ageResult[0];
            Assert.True(ageType == "integer" || ageType == "number",
                $"Expected 'integer' or 'number' but got '{ageType}'");
        }
        else
        {
            string? ageType = (string?)ageResult;
            Assert.True(ageType == "integer" || ageType == "number",
                $"Expected 'integer' or 'number' but got '{ageType}'");
        }

        if (activeResult.Length > 0)
        {
            Assert.Equal(1, activeResult.Length);
            Assert.Equal("boolean", (string?)activeResult[0]);
        }
        else
        {
            Assert.Equal("boolean", (string?)activeResult);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithLegacyPath_ReturnsSingleType(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.TypeAsync(client, key, ".name");

        Assert.NotNull(result);
        // Legacy path returns a single type string
        Assert.Equal("string", (string?)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        ValkeyResult result = await GlideJson.TypeAsync(client, key);

        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithGlideString_ReturnsType(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"items\":[1,2,3]}";

        ValkeyResult rootResult;
        ValkeyResult nameResult;
        ValkeyResult itemsResult;
        _ = await GlideJson.SetAsync(client, key, path, jsonValue);
        rootResult = await GlideJson.TypeAsync(client, key);
        nameResult = await GlideJson.TypeAsync(client, key, "$.name");
        itemsResult = await GlideJson.TypeAsync(client, key, "$.items");


        Assert.NotNull(rootResult);
        Assert.Equal("object", (string?)rootResult);

        Assert.NotNull(nameResult);
        Assert.NotNull(itemsResult);

        // Check name type (should be string)
        if (nameResult.Length > 0)
        {
            Assert.Equal(1, nameResult.Length);
            Assert.Equal("string", (string?)nameResult[0]);
        }
        else
        {
            Assert.Equal("string", (string?)nameResult);
        }

        // Check items type (should be array)
        if (itemsResult.Length > 0)
        {
            Assert.Equal(1, itemsResult.Length);
            Assert.Equal("array", (string?)itemsResult[0]);
        }
        else
        {
            Assert.Equal("array", (string?)itemsResult);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NestedTypes_ReturnsCorrectTypes(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"scores\":[90,85,92]},\"active\":true}";

        ValkeyResult personResult;
        ValkeyResult scoresResult;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        personResult = await GlideJson.TypeAsync(client, key, "$.person");
        scoresResult = await GlideJson.TypeAsync(client, key, "$.person.scores");


        Assert.NotNull(personResult);
        Assert.NotNull(scoresResult);

        // Check person type (should be object)
        if (personResult.Length > 0)
        {
            Assert.Equal(1, personResult.Length);
            Assert.Equal("object", (string?)personResult[0]);
        }
        else
        {
            Assert.Equal("object", (string?)personResult);
        }

        // Check scores type (should be array)
        if (scoresResult.Length > 0)
        {
            Assert.Equal(1, scoresResult.Length);
            Assert.Equal("array", (string?)scoresResult[0]);
        }
        else
        {
            Assert.Equal("array", (string?)scoresResult);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WildcardPath_ReturnsMultipleTypes(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"value\":\"text\"},{\"value\":42},{\"value\":true}]}";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.TypeAsync(client, key, "$.items[*].value");

        Assert.NotNull(result);

        // Wildcard path should return an array of types
        Assert.Equal(3, result.Length);
        Assert.Equal("string", (string?)result[0]);
        string? secondType = (string?)result[1];
        Assert.True(secondType == "integer" || secondType == "number",
            $"Expected 'integer' or 'number' but got '{secondType}'");
        Assert.Equal("boolean", (string?)result[2]);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumIncrByAsync(client, key, "$.count", 5);

        Assert.False(result.IsNull);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumIncrByAsync(client, key, "$.price", 2.5);

        Assert.False(result.IsNull);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumIncrByAsync(client, key, "$.count", -7);

        Assert.False(result.IsNull);
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

        ValkeyValue result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Increment all numeric values at root level
        result = await GlideJson.NumIncrByAsync(client, key, "$.a", 10);


        Assert.False(result.IsNull);
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

        ValkeyValue result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Legacy path (no $ prefix)
        result = await GlideJson.NumIncrByAsync(client, key, ".count", 25);


        Assert.False(result.IsNull);
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
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to increment a string value with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.NumIncrByAsync(client, key, ".name", 5));

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_WithGlideString_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"value\":50}";
        GlideString incrPath = "$.value";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, path, jsonValue);
        result = await GlideJson.NumIncrByAsync(client, key, incrPath, 25);

        Assert.False(result.IsNull);
        Assert.Equal("[75]", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_MultipleNumericValues_ReturnsArrayOfResults(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"count\":1},{\"count\":2},{\"count\":3}]}";

        ValkeyValue result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Increment all count values using wildcard
        result = await GlideJson.NumIncrByAsync(client, key, "$.items[*].count", 10);


        Assert.False(result.IsNull);
        // Should return array with all incremented values
        Assert.Contains("11", result.ToString());
        Assert.Contains("12", result.ToString());
        Assert.Contains("13", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_NonExistentPath_ThrowsError(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to increment non-existent path with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.NumIncrByAsync(client, key, ".nonexistent", 5));

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_IncrementByZero_ReturnsSameValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumIncrByAsync(client, key, "$.count", 0);

        Assert.False(result.IsNull);
        Assert.Equal("[42]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_NegativeToPositive_ReturnsCorrectValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"value\":-10}";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumIncrByAsync(client, key, "$.value", 25);

        Assert.False(result.IsNull);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumMultByAsync(client, key, "$.count", 3);

        Assert.False(result.IsNull);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumMultByAsync(client, key, "$.price", 2);

        Assert.False(result.IsNull);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumMultByAsync(client, key, "$.count", 0);

        Assert.False(result.IsNull);
        Assert.Equal("[0]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyByNegative_ReturnsNegativeValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumMultByAsync(client, key, "$.count", -3);

        Assert.False(result.IsNull);
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

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.NumMultByAsync(client, key, "$.a", 5);

        Assert.False(result.IsNull);
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

        ValkeyValue result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Legacy path (no $ prefix)
        result = await GlideJson.NumMultByAsync(client, key, ".count", 4);


        Assert.False(result.IsNull);
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
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to multiply a string value with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.NumMultByAsync(client, key, ".name", 5));

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_WithGlideString_ReturnsNewValue(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"value\":7}";
        GlideString multPath = "$.value";

        ValkeyValue result; _ = await GlideJson.SetAsync(client, key, path, jsonValue);
        result = await GlideJson.NumMultByAsync(client, key, multPath, 6);

        Assert.False(result.IsNull);
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

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.StrAppendAsync(client, key, "$.greeting", "\" World\"");

        Assert.NotNull(result);
        // JSONPath returns array of lengths
        // "Hello" (5) + " World" (6) = 11
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(11L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithJsonPath_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":\"foo\",\"nested\":{\"b\":\"bar\"}}";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.StrAppendAsync(client, key, "$.a", "\"baz\"");

        Assert.NotNull(result);
        // "foo" (3) + "baz" (3) = 6
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(6L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithLegacyPath_ReturnsSingleLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        ValkeyResult result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Legacy path (no $ prefix)
        result = await GlideJson.StrAppendAsync(client, key, ".name", "\" Doe\"");


        Assert.NotNull(result);
        // "John" (4) + " Doe" (4) = 8
        // Legacy path returns single value (not array)
        Assert.Equal(8L, (long)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_NonStringValue_ThrowsError(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to append to a number value with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.StrAppendAsync(client, key, ".count", "\"test\""));

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithGlideString_ReturnsNewLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"text\":\"Hello\"}";
        GlideString appendPath = "$.text";
        GlideString appendValue = "\" World\"";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, path, jsonValue);
        result = await GlideJson.StrAppendAsync(client, key, appendPath, appendValue);

        Assert.NotNull(result);
        // "Hello" (5) + " World" (6) = 11
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(11L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_AppendEmptyString_ReturnsSameLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"text\":\"Hello\"}";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.StrAppendAsync(client, key, "$.text", "\"\"");

        Assert.NotNull(result);
        // "Hello" (5) + "" (0) = 5
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(5L, arr[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_RootStringValue_ReturnsNewLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        // Set a root-level string value
        string jsonValue = "\"Hello\"";

        ValkeyResult result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Append to root string (no path specified)
        result = await GlideJson.StrAppendAsync(client, key, "\" World\"");


        Assert.NotNull(result);
        // "Hello" (5) + " World" (6) = 11
        Assert.Equal(11L, (long)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_MultipleStringValues_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"bb\"},{\"name\":\"ccc\"}]}";

        ValkeyResult result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Append to all name values using wildcard
        result = await GlideJson.StrAppendAsync(client, key, "$.items[*].name", "\"x\"");


        Assert.NotNull(result);
        // Should return array of new lengths: [2, 3, 4] (a+x=2, bb+x=3, ccc+x=4)
        long[] arr = (long[])result!;
        Assert.Equal(3, arr.Length);
        Assert.Equal(2L, arr[0]);
        Assert.Equal(3L, arr[1]);
        Assert.Equal(4L, arr[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_NonExistentPath_ThrowsError(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to append to non-existent path with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.StrAppendAsync(client, key, ".nonexistent", "\"test\""));

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

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.StrLenAsync(client, key, "$.greeting");

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(5L, arr[0]); // "Hello" has 5 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_WithJsonPath_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":\"foo\",\"nested\":{\"a\":\"hello\"}}";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.StrLenAsync(client, key, "$..a");

        Assert.NotNull(result);
        // JSONPath returns an array of lengths for all matching paths
        long[] arr = (long[])result!;
        Assert.Equal(2, arr.Length);
        // "foo" has 3 characters, "hello" has 5 characters
        long[] lengths = [.. arr.OrderBy(x => x)];
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

        ValkeyResult result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Legacy path (no $ prefix)
        result = await GlideJson.StrLenAsync(client, key, ".name");


        Assert.NotNull(result);
        // Legacy path returns a single length value
        Assert.Equal(4L, (long)result); // "John" has 4 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey("nonexistent");

        ValkeyResult result = await GlideJson.StrLenAsync(client, key);

        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_WithGlideString_ReturnsLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"text\":\"Hello World\"}";
        GlideString strPath = "$.text";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, path, jsonValue);
        result = await GlideJson.StrLenAsync(client, key, strPath);

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(11L, arr[0]); // "Hello World" has 11 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_EmptyString_ReturnsZero(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"empty\":\"\"}";

        ValkeyResult result; _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        result = await GlideJson.StrLenAsync(client, key, "$.empty");

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        long[] arr = (long[])result!;
        _ = Assert.Single(arr);
        Assert.Equal(0L, arr[0]); // Empty string has 0 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_RootStringValue_ReturnsLength(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "\"Hello World\"";

        ValkeyResult result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Get length of root string (no path specified)
        result = await GlideJson.StrLenAsync(client, key);


        Assert.NotNull(result);
        Assert.Equal(11L, (long)result); // "Hello World" has 11 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_NonStringValue_ReturnsNullInArray(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        ValkeyResult result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Get length of non-string value with JSONPath
        result = await GlideJson.StrLenAsync(client, key, "$.age");


        Assert.NotNull(result);
        // JSONPath returns an array with null for non-string matches
        ValkeyResult[] arr = (ValkeyResult[])result!;
        _ = Assert.Single(arr);
        Assert.True(arr[0].IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_MultipleStringValues_ReturnsArrayOfLengths(BaseClient client)
    {
        await SkipIfJsonModuleNotAvailable(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"bb\"},{\"name\":\"ccc\"}]}";

        ValkeyResult result;
        _ = await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Get length of all name values using wildcard
        result = await GlideJson.StrLenAsync(client, key, "$.items[*].name");


        Assert.NotNull(result);
        // JSONPath returns an array of lengths for all matching paths
        long[] arr = (long[])result!;
        Assert.Equal(3, arr.Length);
        Assert.Equal(1L, arr[0]); // "a" has 1 character
        Assert.Equal(2L, arr[1]); // "bb" has 2 characters
        Assert.Equal(3L, arr[2]); // "ccc" has 3 characters
    }

    #endregion
}

