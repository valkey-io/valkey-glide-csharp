// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.IntegrationTests;
using Valkey.Glide.ServerModules;

namespace Valkey.Glide.IntegrationTests.ServerModules;

/// <summary>
/// Integration tests for JSON commands via the GlideJson module.
/// These tests require a Valkey server with the JSON module enabled.
/// </summary>
[Collection("GlideTests")]
public class JsonCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Verify the value was set by reading it back
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.False(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithGlideString_ReturnsOk(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        await GlideJson.SetAsync(client, key, path, jsonValue);

        // Verify the value was set by reading it back
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.False(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_OverwriteExistingValue_ReturnsOk(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string initialValue = "{\"name\":\"John\"}";
        string newValue = "{\"name\":\"Jane\",\"age\":25}";

        // Set initial value
        await GlideJson.SetAsync(client, key, "$", initialValue);

        // Overwrite with new value
        await GlideJson.SetAsync(client, key, "$", newValue);

        // Verify the new value was set
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.Contains("Jane", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithNxCondition_KeyDoesNotExist_ReturnsOk(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        bool wasSet = await GlideJson.SetAsync(client, key, "$", jsonValue, GlideJson.SetCondition.OnlyIfDoesNotExist);

        Assert.True(wasSet);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithNxCondition_KeyExists_ReturnsNull(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string initialValue = "{\"name\":\"John\"}";
        string newValue = "{\"name\":\"Jane\"}";            // Set initial value
        await GlideJson.SetAsync(client, key, "$", initialValue);

        // Try to set with condition
        bool wasSet = await GlideJson.SetAsync(client, key, "$", newValue, GlideJson.SetCondition.OnlyIfDoesNotExist);
        Assert.False(wasSet);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithXxCondition_KeyExists_ReturnsOk(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string initialValue = "{\"name\":\"John\"}";
        string newValue = "{\"name\":\"Jane\"}";            // Set initial value
        await GlideJson.SetAsync(client, key, "$", initialValue);

        // Try to set with condition
        bool wasSet = await GlideJson.SetAsync(client, key, "$", newValue, GlideJson.SetCondition.OnlyIfExists);
        Assert.True(wasSet);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithXxCondition_KeyDoesNotExist_ReturnsNull(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        bool wasSet = await GlideJson.SetAsync(client, key, "$", jsonValue, GlideJson.SetCondition.OnlyIfExists);

        Assert.False(wasSet);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SetNestedPath_ReturnsOk(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string initialValue = "{\"person\":{\"name\":\"John\"}}";
        string newNameValue = "\"Jane\"";            // Set initial value
        await GlideJson.SetAsync(client, key, "$", initialValue);

        // Try to set with condition
        await GlideJson.SetAsync(client, key, "$.person.name", newNameValue);

        // Verify the value was updated
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.Contains("Jane", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SetArrayValue_ReturnsOk(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "[1, 2, 3, 4, 5]";

        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Verify the value was set
        ValkeyValue result = await GlideJson.GetAsync(client, key);
        Assert.Contains("1", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SetScalarValues_ReturnsOk(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string keyString = GetUniqueKey("string");
        string keyNumber = GetUniqueKey("number");
        string keyBool = GetUniqueKey("bool");
        string keyNull = GetUniqueKey("null");
        // Set string value
        await GlideJson.SetAsync(client, keyString, "$", "\"hello\"");

        // Set number value
        await GlideJson.SetAsync(client, keyNumber, "$", "42");

        // Set boolean value
        await GlideJson.SetAsync(client, keyBool, "$", "true");

        // Set null value
        await GlideJson.SetAsync(client, keyNull, "$", "null");

        // Verify values were set
        Assert.Equal("\"hello\"", (await GlideJson.GetAsync(client, keyString)).ToString());
        Assert.Equal("42", (await GlideJson.GetAsync(client, keyNumber)).ToString());
        Assert.Equal("true", (await GlideJson.GetAsync(client, keyBool)).ToString());
        Assert.Equal("null", (await GlideJson.GetAsync(client, keyNull)).ToString());
    }

    #endregion

    #region JSON.GET Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_EntireDocument_ReturnsDocument(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.name"]);

        Assert.False(result.IsNull);
        // JSONPath returns an array of matching values
        Assert.Equal("[\"John\"]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithLegacyPath_ReturnsSingleValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, [".name"]);

        Assert.False(result.IsNull);
        // Legacy path returns the single matching value
        Assert.Equal("\"John\"", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithMultiplePaths_ReturnsObject(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.name", "$.age"]);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";
        var options = new GlideJson.GetOptions { Indent = "  ", Newline = "\n", Space = " " };

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, options);

        Assert.False(result.IsNull);
        // The result should be formatted with newlines and indentation
        Assert.Contains("\n", result.ToString());
        Assert.Contains("  ", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithPathsAndOptions_ReturnsFormattedJson(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"age\":30}}";
        var options = new GlideJson.GetOptions { Indent = "\t", Newline = "\n" };

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.person"], options);

        Assert.False(result.IsNull);
        // The result should be formatted with tabs and newlines
        Assert.Contains("\n", result.ToString());
        Assert.Contains("\t", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey("nonexistent");

        ValkeyValue result = await GlideJson.GetAsync(client, key);

        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_SetGetRoundTrip_ReturnsOriginalValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"active\":true,\"scores\":[1,2,3]}";
        // Set the value
        await GlideJson.SetAsync(client, key, "$", jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        await GlideJson.SetAsync(client, key, path, jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key);

        Assert.False(result.IsNull);
        string resultStr = result.ToString();
        Assert.Contains("\"name\"", resultStr);
        Assert.Contains("\"Jane\"", resultStr);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WithGlideStringPaths_ReturnsValues(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        ValkeyValue[] paths = ["$.name"];

        await GlideJson.SetAsync(client, key, path, jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, paths);

        Assert.False(result.IsNull);
        Assert.Equal("[\"Jane\"]", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_NestedObject_ReturnsNestedValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"address\":{\"city\":\"NYC\",\"zip\":\"10001\"}}}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.person.address.city"]);

        Assert.False(result.IsNull);
        Assert.Equal("[\"NYC\"]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_ArrayElement_ReturnsElement(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"numbers\":[10,20,30,40,50]}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.numbers[2]"]);

        Assert.False(result.IsNull);
        Assert.Equal("[30]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_WildcardPath_ReturnsAllMatches(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"b\"},{\"name\":\"c\"}]}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.items[*].name"]);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string keyString = GetUniqueKey("string");
        string keyNumber = GetUniqueKey("number");
        string keyBool = GetUniqueKey("bool");
        string keyNull = GetUniqueKey("null");
        // Set and get string value
        await GlideJson.SetAsync(client, keyString, "$", "\"hello\"");
        string? resultString = await GlideJson.GetAsync(client, keyString);
        Assert.Equal("\"hello\"", resultString);

        // Set and get number value
        await GlideJson.SetAsync(client, keyNumber, "$", "42");
        string? resultNumber = await GlideJson.GetAsync(client, keyNumber);
        Assert.Equal("42", resultNumber);

        // Set and get boolean value
        await GlideJson.SetAsync(client, keyBool, "$", "true");
        string? resultBool = await GlideJson.GetAsync(client, keyBool);
        Assert.Equal("true", resultBool);

        // Set and get null value
        await GlideJson.SetAsync(client, keyNull, "$", "null");
        string? resultNull = await GlideJson.GetAsync(client, keyNull);
        Assert.Equal("null", resultNull);

    }

    #endregion

    #region JSON.MGET Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task MGetAsync_MultipleExistingKeys_ReturnsAllValues(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        string key3 = GetUniqueClusterKey("mget");
        // Set up JSON documents
        await GlideJson.SetAsync(client, key1, "$", "{\"name\":\"John\",\"age\":30}");
        await GlideJson.SetAsync(client, key2, "$", "{\"name\":\"Jane\",\"age\":25}");
        await GlideJson.SetAsync(client, key3, "$", "{\"name\":\"Bob\",\"age\":35}");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        string nonExistentKey = GetUniqueClusterKey("mget_nonexistent");
        // Set up JSON documents (only key1 and key2)
        await GlideJson.SetAsync(client, key1, "$", "{\"name\":\"John\"}");
        await GlideJson.SetAsync(client, key2, "$", "{\"name\":\"Jane\"}");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        // Set up JSON documents with arrays
        await GlideJson.SetAsync(client, key1, "$", "{\"items\":[1,2,3]}");
        await GlideJson.SetAsync(client, key2, "$", "{\"items\":[4,5,6]}");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        // Set up JSON documents
        await GlideJson.SetAsync(client, key1, "$", "{\"name\":\"John\"}");
        await GlideJson.SetAsync(client, key2, "$", "{\"name\":\"Jane\"}");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        string path = "$.name";
        // Set up JSON documents
        await GlideJson.SetAsync(client, key1, "$", (GlideString)"{\"name\":\"John\"}");
        await GlideJson.SetAsync(client, key2, "$", (GlideString)"{\"name\":\"Jane\"}");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        // Set up JSON documents with different structures
        await GlideJson.SetAsync(client, key1, "$", "{\"name\":\"John\"}");
        await GlideJson.SetAsync(client, key2, "$", "{\"age\":25}"); // No "name" field

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        // Use hash tags for cluster compatibility
        string key1 = GetUniqueClusterKey("mget");
        string key2 = GetUniqueClusterKey("mget");
        // Set up JSON documents
        await GlideJson.SetAsync(client, key1, "$", "{\"a\":1}");
        await GlideJson.SetAsync(client, key2, "$", "{\"b\":2}");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, "$", jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, "$", jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey("nonexistent");

        long deleted = await GlideJson.DelAsync(client, key);

        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_NonExistentPath_ReturnsZero(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Try to delete non-existent path
        deleted = await GlideJson.DelAsync(client, key, "$.nonexistent");


        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_WithGlideString_DeletesDocument(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, path, jsonValue);

        // Delete entire document using GlideString overload
        deleted = await GlideJson.DelAsync(client, key);


        Assert.Equal(1, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_WithGlideStringPath_DeletesSpecificElement(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        string deletePath = "$.age";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, path, jsonValue);

        // Delete specific path using GlideString overload
        deleted = await GlideJson.DelAsync(client, key, deletePath);


        Assert.Equal(1, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DelAsync_MultipleMatchingPaths_ReturnsCount(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"b\"},{\"name\":\"c\"}]}";

        long deleted;
        // Set up JSON document with array of objects
        await GlideJson.SetAsync(client, key, "$", jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, "$", jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, "$", jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey("nonexistent");

        long deleted = await GlideJson.ForgetAsync(client, key);

        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_BehavesIdenticallyToDel(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string keyDel = GetUniqueKey("del");
        string keyForget = GetUniqueKey("forget");
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"items\":[1,2,3]}";
        // Set up identical JSON documents
        await GlideJson.SetAsync(client, keyDel, "$", jsonValue);
        await GlideJson.SetAsync(client, keyForget, "$", jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, path, jsonValue);

        // Forget entire document using GlideString overload
        deleted = await GlideJson.ForgetAsync(client, key);


        Assert.Equal(1, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ForgetAsync_WithGlideStringPath_DeletesSpecificElement(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"age\":25}";
        string deletePath = "$.name";

        long deleted;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, path, jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[1,2,3,4,5]}";

        long cleared;
        // Set up JSON document with array
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the array
        cleared = await GlideJson.ClearAsync(client, key, "$.items");

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.items"]);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
        // Array should be empty
        Assert.Equal("[[]]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_Object_RemovesAllKeys(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"data\":{\"name\":\"John\",\"age\":30,\"city\":\"NYC\"}}";

        long cleared;
        // Set up JSON document with nested object
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the nested object
        cleared = await GlideJson.ClearAsync(client, key, "$.data");

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.data"]);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
        // Object should be empty
        Assert.Equal("[{}]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_Number_SetsToZero(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";

        long cleared;
        // Set up JSON document with number
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the number
        cleared = await GlideJson.ClearAsync(client, key, "$.count");

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.count"]);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
        // Number should be 0
        Assert.Equal("[0]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_Boolean_SetsToFalse(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"active\":true}";

        long cleared;
        // Set up JSON document with boolean
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the boolean - Valkey JSON clears booleans to false
        cleared = await GlideJson.ClearAsync(client, key, "$.active");

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.active"]);


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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        long cleared;
        // Set up JSON document with string
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear the string - Valkey JSON clears strings to empty string
        cleared = await GlideJson.ClearAsync(client, key, "$.name");

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.name"]);


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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"items\":[1,2,3]}";
        GlideString clearPath = "$.items";

        long cleared;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, path, jsonValue);

        // Clear using GlideString overload
        cleared = await GlideJson.ClearAsync(client, key, clearPath);

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key, [clearPath]);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
        Assert.Equal("[[]]", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_RootPath_ClearsEntireDocument(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"items\":[1,2,3],\"count\":5}";

        long cleared;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear at root path
        cleared = await GlideJson.ClearAsync(client, key, "$");

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
        // Root object should be empty
        Assert.Equal("{}", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_MultipleMatchingPaths_ReturnsCount(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"data\":[1,2]},{\"data\":[3,4]},{\"data\":[5,6]}]}";

        long cleared;
        // Set up JSON document with multiple arrays
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear all "data" arrays using wildcard path
        cleared = await GlideJson.ClearAsync(client, key, "$.items[*].data");

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key, ["$.items[*].data"]);


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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        long cleared;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, "$", jsonValue);

        // Clear without specifying path (should clear root)
        cleared = await GlideJson.ClearAsync(client, key);

        // Get the result
        ValkeyValue result = await GlideJson.GetAsync(client, key);


        Assert.Equal(1, cleared);
        Assert.False(result.IsNull);
        // Root object should be empty
        Assert.Equal("{}", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ClearAsync_NonExistentPath_ReturnsZero(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        long cleared;
        // Set up JSON document
        await GlideJson.SetAsync(client, key, "$", jsonValue);

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        string? result = await GlideJson.TypeAsync(client, key);

        Assert.NotNull(result);
        Assert.Equal("object", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_ArrayType_ReturnsArray(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "[1, 2, 3, 4, 5]";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        string? result = await GlideJson.TypeAsync(client, key);

        Assert.NotNull(result);
        Assert.Equal("array", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_StringType_ReturnsString(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "\"hello world\"";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        string? result = await GlideJson.TypeAsync(client, key);

        Assert.NotNull(result);
        Assert.Equal("string", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NumberType_ReturnsNumberOrInteger(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string keyInteger = GetUniqueKey("integer");
        string keyFloat = GetUniqueKey("float");

        await GlideJson.SetAsync(client, keyInteger, "$", "42");
        await GlideJson.SetAsync(client, keyFloat, "$", "3.14");
        string? intResult = await GlideJson.TypeAsync(client, keyInteger);
        string? floatResult = await GlideJson.TypeAsync(client, keyFloat);


        Assert.NotNull(intResult);
        Assert.NotNull(floatResult);
        // Integer values may return "integer" or "number" depending on the JSON module version
        Assert.True(intResult == "integer" || intResult == "number",
            $"Expected 'integer' or 'number' but got '{intResult}'");
        Assert.Equal("number", floatResult);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_BooleanType_ReturnsBoolean(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string keyTrue = GetUniqueKey("true");
        string keyFalse = GetUniqueKey("false");

        await GlideJson.SetAsync(client, keyTrue, "$", "true");
        await GlideJson.SetAsync(client, keyFalse, "$", "false");
        string? trueResult = await GlideJson.TypeAsync(client, keyTrue);
        string? falseResult = await GlideJson.TypeAsync(client, keyFalse);


        Assert.NotNull(trueResult);
        Assert.NotNull(falseResult);
        Assert.Equal("boolean", trueResult);
        Assert.Equal("boolean", falseResult);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NullType_ReturnsNull(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "null";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        string? result = await GlideJson.TypeAsync(client, key);

        Assert.NotNull(result);
        Assert.Equal("null", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithJsonPath_ReturnsArrayOfTypes(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30,\"active\":true}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue[] nameResult = await GlideJson.TypeAsync(client, key, "$.name");
        ValkeyValue[] ageResult = await GlideJson.TypeAsync(client, key, "$.age");
        ValkeyValue[] activeResult = await GlideJson.TypeAsync(client, key, "$.active");

        // JSONPath returns an array of types
        // Results should be arrays containing the type strings
        _ = Assert.Single(nameResult);
        Assert.Equal("string", (string?)nameResult[0]);

        _ = Assert.Single(ageResult);
        string? ageType = (string?)ageResult[0];
        Assert.True(ageType == "integer" || ageType == "number",
            $"Expected 'integer' or 'number' but got '{ageType}'");

        _ = Assert.Single(activeResult);
        Assert.Equal("boolean", (string?)activeResult[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithLegacyPath_ReturnsSingleType(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue[] result = await GlideJson.TypeAsync(client, key, ".name");

        // Legacy path returns a single type string wrapped in array
        _ = Assert.Single(result);
        Assert.Equal("string", (string?)result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey("nonexistent");

        string? result = await GlideJson.TypeAsync(client, key);

        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WithGlideString_ReturnsType(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"name\":\"Jane\",\"items\":[1,2,3]}";

        await GlideJson.SetAsync(client, key, path, jsonValue);
        string? rootResult = await GlideJson.TypeAsync(client, key);
        ValkeyValue[] nameResult = await GlideJson.TypeAsync(client, key, "$.name");
        ValkeyValue[] itemsResult = await GlideJson.TypeAsync(client, key, "$.items");

        Assert.NotNull(rootResult);
        Assert.Equal("object", rootResult);

        // Check name type (should be string)
        _ = Assert.Single(nameResult);
        Assert.Equal("string", (string?)nameResult[0]);

        // Check items type (should be array)
        _ = Assert.Single(itemsResult);
        Assert.Equal("array", (string?)itemsResult[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_NestedTypes_ReturnsCorrectTypes(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"person\":{\"name\":\"John\",\"scores\":[90,85,92]},\"active\":true}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue[] personResult = await GlideJson.TypeAsync(client, key, "$.person");
        ValkeyValue[] scoresResult = await GlideJson.TypeAsync(client, key, "$.person.scores");

        // Check person type (should be object)
        _ = Assert.Single(personResult);
        Assert.Equal("object", (string?)personResult[0]);

        // Check scores type (should be array)
        _ = Assert.Single(scoresResult);
        Assert.Equal("array", (string?)scoresResult[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TypeAsync_WildcardPath_ReturnsMultipleTypes(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"value\":\"text\"},{\"value\":42},{\"value\":true}]}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue[] result = await GlideJson.TypeAsync(client, key, "$.items[*].value");

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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, "$.count", 5);

        Assert.False(result.IsNull);
        // JSONPath returns array of results
        Assert.Equal("[15]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_IncrementFloat_ReturnsNewValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"price\":10.5}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, "$.price", 2.5);

        Assert.False(result.IsNull);
        // JSONPath returns array of results
        Assert.Equal("[13]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_DecrementWithNegativeValue_ReturnsNewValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":20}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, "$.count", -7);

        Assert.False(result.IsNull);
        // JSONPath returns array of results
        Assert.Equal("[13]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_WithJsonPath_ReturnsArray(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":1,\"b\":2,\"nested\":{\"c\":3}}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Increment all numeric values at root level
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, "$.a", 10);


        Assert.False(result.IsNull);
        // JSONPath returns array of results
        Assert.Equal("[11]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_WithLegacyPath_ReturnsSingleValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":100}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Legacy path (no $ prefix)
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, ".count", 25);


        Assert.False(result.IsNull);
        // Legacy path returns single value (not array)
        Assert.Equal("125", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_NonNumericValue_ThrowsError(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";
        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to increment a string value with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.NumIncrByAsync(client, key, ".name", 5));

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_WithGlideString_ReturnsNewValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"value\":50}";
        GlideString incrPath = "$.value";

        await GlideJson.SetAsync(client, key, path, jsonValue);
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, incrPath, 25);

        Assert.False(result.IsNull);
        Assert.Equal("[75]", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_MultipleNumericValues_ReturnsArrayOfResults(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"count\":1},{\"count\":2},{\"count\":3}]}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Increment all count values using wildcard
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, "$.items[*].count", 10);


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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";
        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to increment non-existent path with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.NumIncrByAsync(client, key, ".nonexistent", 5));

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_IncrementByZero_ReturnsSameValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, "$.count", 0);

        Assert.False(result.IsNull);
        Assert.Equal("[42]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumIncrByAsync_NegativeToPositive_ReturnsCorrectValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"value\":-10}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumIncrByAsync(client, key, "$.value", 25);

        Assert.False(result.IsNull);
        Assert.Equal("[15]", result);
    }

    #endregion

    #region JSON.NUMMULTBY Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyInteger_ReturnsNewValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumMultByAsync(client, key, "$.count", 3);

        Assert.False(result.IsNull);
        // JSONPath returns array of results
        Assert.Equal("[30]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyFloat_ReturnsNewValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"price\":10.5}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumMultByAsync(client, key, "$.price", 2);

        Assert.False(result.IsNull);
        // JSONPath returns array of results
        Assert.Equal("[21]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyByZero_ReturnsZero(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumMultByAsync(client, key, "$.count", 0);

        Assert.False(result.IsNull);
        Assert.Equal("[0]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_MultiplyByNegative_ReturnsNegativeValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":10}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumMultByAsync(client, key, "$.count", -3);

        Assert.False(result.IsNull);
        // JSONPath returns array of results
        Assert.Equal("[-30]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_WithJsonPath_ReturnsArray(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":2,\"b\":3,\"nested\":{\"c\":4}}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        ValkeyValue result = await GlideJson.NumMultByAsync(client, key, "$.a", 5);

        Assert.False(result.IsNull);
        // JSONPath returns array of results
        Assert.Equal("[10]", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_WithLegacyPath_ReturnsSingleValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":5}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Legacy path (no $ prefix)
        ValkeyValue result = await GlideJson.NumMultByAsync(client, key, ".count", 4);


        Assert.False(result.IsNull);
        // Legacy path returns single value (not array)
        Assert.Equal("20", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_NonNumericValue_ThrowsError(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";
        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to multiply a string value with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.NumMultByAsync(client, key, ".name", 5));

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NumMultByAsync_WithGlideString_ReturnsNewValue(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"value\":7}";
        GlideString multPath = "$.value";

        await GlideJson.SetAsync(client, key, path, jsonValue);
        ValkeyValue result = await GlideJson.NumMultByAsync(client, key, multPath, 6);

        Assert.False(result.IsNull);
        Assert.Equal("[42]", result.ToString());
    }

    #endregion

    #region JSON.STRAPPEND Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_AppendToString_ReturnsNewLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"greeting\":\"Hello\"}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        long?[]? result = await GlideJson.StrAppendAsync(client, key, "$.greeting", "\" World\"");

        Assert.NotNull(result);
        // JSONPath returns array of lengths
        // "Hello" (5) + " World" (6) = 11
        _ = Assert.Single(result);
        Assert.Equal(11L, result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithJsonPath_ReturnsArrayOfLengths(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":\"foo\",\"nested\":{\"b\":\"bar\"}}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        long?[]? result = await GlideJson.StrAppendAsync(client, key, "$.a", "\"baz\"");

        Assert.NotNull(result);
        // "foo" (3) + "baz" (3) = 6
        _ = Assert.Single(result);
        Assert.Equal(6L, result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithLegacyPath_ReturnsSingleLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Legacy path (no $ prefix)
        long?[]? result = await GlideJson.StrAppendAsync(client, key, ".name", "\" Doe\"");

        Assert.NotNull(result);
        // "John" (4) + " Doe" (4) = 8
        // Legacy path returns single value wrapped in array
        _ = Assert.Single(result);
        Assert.Equal(8L, result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_NonStringValue_ThrowsError(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"count\":42}";
        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Trying to append to a number value with legacy path should throw
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await GlideJson.StrAppendAsync(client, key, ".count", "\"test\""));

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_WithGlideString_ReturnsNewLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"text\":\"Hello\"}";
        GlideString appendPath = "$.text";
        GlideString appendValue = "\" World\"";

        await GlideJson.SetAsync(client, key, path, jsonValue);
        long?[]? result = await GlideJson.StrAppendAsync(client, key, appendPath, appendValue);

        Assert.NotNull(result);
        // "Hello" (5) + " World" (6) = 11
        _ = Assert.Single(result);
        Assert.Equal(11L, result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_AppendEmptyString_ReturnsSameLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"text\":\"Hello\"}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        long?[]? result = await GlideJson.StrAppendAsync(client, key, "$.text", "\"\"");

        Assert.NotNull(result);
        // "Hello" (5) + "" (0) = 5
        _ = Assert.Single(result);
        Assert.Equal(5L, result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_RootStringValue_ReturnsNewLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        // Set a root-level string value
        string jsonValue = "\"Hello\"";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Append to root string (no path specified)
        long result = await GlideJson.StrAppendAsync(client, key, "\" World\"");


        // "Hello" (5) + " World" (6) = 11
        Assert.Equal(11L, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_MultipleStringValues_ReturnsArrayOfLengths(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"bb\"},{\"name\":\"ccc\"}]}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Append to all name values using wildcard
        long?[]? result = await GlideJson.StrAppendAsync(client, key, "$.items[*].name", "\"x\"");

        Assert.NotNull(result);
        // Should return array of new lengths: [2, 3, 4] (a+x=2, bb+x=3, ccc+x=4)
        Assert.Equal(3, result.Length);
        Assert.Equal(2L, result[0]);
        Assert.Equal(3L, result[1]);
        Assert.Equal(4L, result[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrAppendAsync_NonExistentPath_ThrowsError(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";
        await GlideJson.SetAsync(client, key, "$", jsonValue);
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
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"greeting\":\"Hello\"}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        long?[]? result = await GlideJson.StrLenAsync(client, key, "$.greeting");

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        _ = Assert.Single(result);
        Assert.Equal(5L, result[0]); // "Hello" has 5 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_WithJsonPath_ReturnsArrayOfLengths(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"a\":\"foo\",\"nested\":{\"a\":\"hello\"}}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        long?[]? result = await GlideJson.StrLenAsync(client, key, "$..a");

        Assert.NotNull(result);
        // JSONPath returns an array of lengths for all matching paths
        Assert.Equal(2, result.Length);
        // "foo" has 3 characters, "hello" has 5 characters
        long?[] lengths = [.. result.OrderBy(x => x)];
        Assert.Contains(3L, lengths);
        Assert.Contains(5L, lengths);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_WithLegacyPath_ReturnsSingleLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\"}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Legacy path (no $ prefix)
        long?[]? result = await GlideJson.StrLenAsync(client, key, ".name");

        Assert.NotNull(result);
        // Legacy path returns a single length value wrapped in array
        _ = Assert.Single(result);
        Assert.Equal(4L, result[0]); // "John" has 4 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey("nonexistent");

        long? result = await GlideJson.StrLenAsync(client, key);

        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_WithGlideString_ReturnsLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string path = "$";
        string jsonValue = "{\"text\":\"Hello World\"}";
        GlideString strPath = "$.text";

        await GlideJson.SetAsync(client, key, path, jsonValue);
        long?[]? result = await GlideJson.StrLenAsync(client, key, strPath);

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        _ = Assert.Single(result);
        Assert.Equal(11L, result[0]); // "Hello World" has 11 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_EmptyString_ReturnsZero(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"empty\":\"\"}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        long?[]? result = await GlideJson.StrLenAsync(client, key, "$.empty");

        Assert.NotNull(result);
        // JSONPath returns an array of lengths
        _ = Assert.Single(result);
        Assert.Equal(0L, result[0]); // Empty string has 0 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_RootStringValue_ReturnsLength(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "\"Hello World\"";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Get length of root string (no path specified)
        long? result = await GlideJson.StrLenAsync(client, key);


        _ = Assert.NotNull(result);
        Assert.Equal(11L, result); // "Hello World" has 11 characters
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_NonStringValue_ReturnsNullInArray(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"name\":\"John\",\"age\":30}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Get length of non-string value with JSONPath
        long?[]? result = await GlideJson.StrLenAsync(client, key, "$.age");

        Assert.NotNull(result);
        // JSONPath returns an array with null for non-string matches
        _ = Assert.Single(result);
        Assert.Null(result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StrLenAsync_MultipleStringValues_ReturnsArrayOfLengths(BaseClient client)
    {
        await ModuleUtils.SkipIfJsonModuleNotAvailableAsync(client);

        string key = GetUniqueKey();
        string jsonValue = "{\"items\":[{\"name\":\"a\"},{\"name\":\"bb\"},{\"name\":\"ccc\"}]}";

        await GlideJson.SetAsync(client, key, "$", jsonValue);
        // Get length of all name values using wildcard
        long?[]? result = await GlideJson.StrLenAsync(client, key, "$.items[*].name");

        Assert.NotNull(result);
        // JSONPath returns an array of lengths for all matching paths
        Assert.Equal(3, result.Length);
        Assert.Equal(1L, result[0]); // "a" has 1 character
        Assert.Equal(2L, result[1]); // "bb" has 2 characters
        Assert.Equal(3L, result[2]); // "ccc" has 3 characters
    }

    #endregion
}

