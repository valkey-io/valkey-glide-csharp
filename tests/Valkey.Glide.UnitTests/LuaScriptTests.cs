// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class LuaScriptTests
{
    [Fact]
    public void Prepare_WithValidScript_CreatesLuaScript()
    {
        // Arrange
        string script = "return redis.call('GET', @key)";

        // Act
        LuaScript luaScript = LuaScript.Prepare(script);

        // Assert
        Assert.NotNull(luaScript);
        Assert.Equal(script, luaScript.OriginalScript);
        Assert.NotNull(luaScript.ExecutableScript);
    }

    [Fact]
    public void Prepare_WithNullScript_ThrowsArgumentException()
    {
        // Act & Assert
        ArgumentException ex = Assert.Throws<ArgumentException>(() => LuaScript.Prepare(null!));
        Assert.Contains("Script cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Prepare_WithEmptyScript_ThrowsArgumentException()
    {
        // Act & Assert
        ArgumentException ex = Assert.Throws<ArgumentException>(() => LuaScript.Prepare(""));
        Assert.Contains("Script cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Prepare_ExtractsParametersCorrectly()
    {
        // Arrange
        string script = "return redis.call('SET', @key, @value)";

        // Act
        LuaScript luaScript = LuaScript.Prepare(script);

        // Assert
        Assert.Equal(2, luaScript.Arguments.Length);
        Assert.Equal("key", luaScript.Arguments[0]);
        Assert.Equal("value", luaScript.Arguments[1]);
    }

    [Fact]
    public void Prepare_WithDuplicateParameters_ExtractsUniqueParameters()
    {
        // Arrange
        string script = "return redis.call('SET', @key, @value) .. redis.call('GET', @key)";

        // Act
        LuaScript luaScript = LuaScript.Prepare(script);

        // Assert
        Assert.Equal(2, luaScript.Arguments.Length);
        Assert.Equal("key", luaScript.Arguments[0]);
        Assert.Equal("value", luaScript.Arguments[1]);
    }

    [Fact]
    public void Prepare_WithNoParameters_ReturnsScriptWithEmptyArguments()
    {
        // Arrange
        string script = "return 'hello'";

        // Act
        LuaScript luaScript = LuaScript.Prepare(script);

        // Assert
        Assert.Empty(luaScript.Arguments);
        Assert.Equal(script, luaScript.OriginalScript);
    }

    [Fact]
    public void Prepare_CachesScripts()
    {
        // Arrange
        string script = "return redis.call('GET', @key)";
        LuaScript.PurgeCache(); // Ensure clean state

        // Act
        LuaScript first = LuaScript.Prepare(script);
        LuaScript second = LuaScript.Prepare(script);

        // Assert
        Assert.Same(first, second); // Should return the same cached instance
    }

    [Fact]
    public void PurgeCache_ClearsCache()
    {
        // Arrange
        string script = "return redis.call('GET', @key)";
        LuaScript.Prepare(script);
        int countBefore = LuaScript.GetCachedScriptCount();

        // Act
        LuaScript.PurgeCache();
        int countAfter = LuaScript.GetCachedScriptCount();

        // Assert
        Assert.True(countBefore > 0);
        Assert.Equal(0, countAfter);
    }

    [Fact]
    public void GetCachedScriptCount_ReturnsCorrectCount()
    {
        // Arrange
        LuaScript.PurgeCache();
        string script1 = "return redis.call('GET', @key)";
        string script2 = "return redis.call('SET', @key, @value)";

        // Act
        LuaScript.Prepare(script1);
        int countAfterFirst = LuaScript.GetCachedScriptCount();
        LuaScript.Prepare(script2);
        int countAfterSecond = LuaScript.GetCachedScriptCount();

        // Assert
        Assert.Equal(1, countAfterFirst);
        Assert.Equal(2, countAfterSecond);
    }

    [Fact]
    public void Prepare_WithWeakReferences_AllowsGarbageCollection()
    {
        // Arrange
        LuaScript.PurgeCache();
        string script = "return redis.call('GET', @key)";

        // Act
        LuaScript.Prepare(script);
        // Don't keep a strong reference
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // The cache entry still exists but the weak reference may have been collected
        int count = LuaScript.GetCachedScriptCount();

        // Assert
        Assert.Equal(1, count); // Cache entry exists
        // Note: We can't reliably test if the weak reference was collected
        // as it depends on GC behavior
    }

    [Fact]
    public void ExtractParametersInternal_WithNullParameters_ReturnsEmptyArrays()
    {
        // Arrange
        string script = "return redis.call('GET', @key)";
        LuaScript luaScript = LuaScript.Prepare(script);

        // Act
        (ValkeyKey[] keys, ValkeyValue[] args) = luaScript.ExtractParametersInternal(null, null);

        // Assert
        Assert.Empty(keys);
        Assert.Empty(args);
    }

    [Fact]
    public void ExtractParametersInternal_WithValidParameters_ExtractsCorrectly()
    {
        // Arrange
        string script = "return redis.call('SET', @key, @value)";
        LuaScript luaScript = LuaScript.Prepare(script);
        object parameters = new { key = new ValkeyKey("mykey"), value = "myvalue" };

        // Act
        (ValkeyKey[] keys, ValkeyValue[] args) = luaScript.ExtractParametersInternal(parameters, null);

        // Assert
        Assert.Single(keys);
        Assert.Equal("mykey", (string?)keys[0]);
        Assert.Single(args);
        Assert.Equal("myvalue", (string?)args[0]);
    }

    [Fact]
    public void ExtractParametersInternal_WithMissingParameter_ThrowsArgumentException()
    {
        // Arrange
        string script = "return redis.call('SET', @key, @value)";
        LuaScript luaScript = LuaScript.Prepare(script);
        object parameters = new { key = new ValkeyKey("mykey") }; // Missing 'value'

        // Act & Assert
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => luaScript.ExtractParametersInternal(parameters, null));
        Assert.Contains("missing required property or field: value", ex.Message);
    }

    [Fact]
    public void ExtractParametersInternal_WithInvalidParameterType_ThrowsArgumentException()
    {
        // Arrange
        string script = "return redis.call('SET', @key, @value)";
        LuaScript luaScript = LuaScript.Prepare(script);
        object parameters = new { key = new ValkeyKey("mykey"), value = new object() }; // Invalid type

        // Act & Assert
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => luaScript.ExtractParametersInternal(parameters, null));
        Assert.Contains("has an invalid type", ex.Message);
    }

    [Fact]
    public void ExtractParametersInternal_WithKeyPrefix_AppliesPrefixToKeys()
    {
        // Arrange
        string script = "return redis.call('GET', @key)";
        LuaScript luaScript = LuaScript.Prepare(script);
        object parameters = new { key = new ValkeyKey("mykey") };
        ValkeyKey prefix = new("prefix:");

        // Act
        (ValkeyKey[] keys, ValkeyValue[] args) = luaScript.ExtractParametersInternal(parameters, prefix);

        // Assert
        Assert.Single(keys);
        Assert.Equal("prefix:mykey", (string?)keys[0]);
    }

    [Fact]
    public void ExtractParametersInternal_WithMultipleParameters_ExtractsInCorrectOrder()
    {
        // Arrange
        string script = "return redis.call('SET', @key1, @value1) .. redis.call('SET', @key2, @value2)";
        LuaScript luaScript = LuaScript.Prepare(script);
        object parameters = new
        {
            key1 = new ValkeyKey("key1"),
            value1 = "val1",
            key2 = new ValkeyKey("key2"),
            value2 = "val2"
        };

        // Act
        (ValkeyKey[] keys, ValkeyValue[] args) = luaScript.ExtractParametersInternal(parameters, null);

        // Assert
        Assert.Equal(2, keys.Length);
        Assert.Equal("key1", (string?)keys[0]);
        Assert.Equal("key2", (string?)keys[1]);
        Assert.Equal(2, args.Length);
        Assert.Equal("val1", (string?)args[0]);
        Assert.Equal("val2", (string?)args[1]);
    }

    [Fact]
    public void ExtractParametersInternal_WithNumericParameters_ExtractsCorrectly()
    {
        // Arrange
        string script = "return redis.call('INCRBY', @key, @amount)";
        LuaScript luaScript = LuaScript.Prepare(script);
        object parameters = new { key = new ValkeyKey("counter"), amount = 42 };

        // Act
        (ValkeyKey[] keys, ValkeyValue[] args) = luaScript.ExtractParametersInternal(parameters, null);

        // Assert
        Assert.Single(keys);
        Assert.Equal("counter", (string?)keys[0]);
        Assert.Single(args);
        Assert.Equal(42, (int)args[0]);
    }

    [Fact]
    public void ExtractParametersInternal_WithBooleanParameters_ExtractsCorrectly()
    {
        // Arrange
        string script = "return @flag";
        LuaScript luaScript = LuaScript.Prepare(script);
        object parameters = new { flag = true };

        // Act
        (ValkeyKey[] keys, ValkeyValue[] args) = luaScript.ExtractParametersInternal(parameters, null);

        // Assert
        Assert.Empty(keys);
        Assert.Single(args);
        Assert.True((bool)args[0]);
    }

    [Fact]
    public void ExtractParametersInternal_WithByteArrayParameters_ExtractsCorrectly()
    {
        // Arrange
        string script = "return @data";
        LuaScript luaScript = LuaScript.Prepare(script);
        byte[] data = [1, 2, 3, 4, 5];
        object parameters = new { data };

        // Act
        (ValkeyKey[] keys, ValkeyValue[] args) = luaScript.ExtractParametersInternal(parameters, null);

        // Assert
        Assert.Empty(keys);
        Assert.Single(args);
        Assert.Equal(data, (byte[]?)args[0]);
    }

    [Fact]
    public void Prepare_WithComplexScript_ExtractsAllParameters()
    {
        // Arrange
        string script = @"
            local key1 = @key1
            local key2 = @key2
            local value = @value
            local ttl = @ttl
            redis.call('SET', key1, value)
            redis.call('EXPIRE', key1, ttl)
            return redis.call('GET', key2)
        ";

        // Act
        LuaScript luaScript = LuaScript.Prepare(script);

        // Assert
        Assert.Equal(4, luaScript.Arguments.Length);
        Assert.Contains("key1", luaScript.Arguments);
        Assert.Contains("key2", luaScript.Arguments);
        Assert.Contains("value", luaScript.Arguments);
        Assert.Contains("ttl", luaScript.Arguments);
    }

    [Fact]
    public void Prepare_WithUnderscoresInParameterNames_ExtractsCorrectly()
    {
        // Arrange
        string script = "return redis.call('GET', @my_key_name)";

        // Act
        LuaScript luaScript = LuaScript.Prepare(script);

        // Assert
        Assert.Single(luaScript.Arguments);
        Assert.Equal("my_key_name", luaScript.Arguments[0]);
    }

    [Fact]
    public void Prepare_WithNumbersInParameterNames_ExtractsCorrectly()
    {
        // Arrange
        string script = "return redis.call('GET', @key1) .. redis.call('GET', @key2)";

        // Act
        LuaScript luaScript = LuaScript.Prepare(script);

        // Assert
        Assert.Equal(2, luaScript.Arguments.Length);
        Assert.Equal("key1", luaScript.Arguments[0]);
        Assert.Equal("key2", luaScript.Arguments[1]);
    }
}
