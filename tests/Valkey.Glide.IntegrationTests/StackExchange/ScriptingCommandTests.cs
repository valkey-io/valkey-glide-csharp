// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.TestUtils.Data;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> scripting commands.
/// These tests verify the StackExchange.Redis-compatible API surface.
/// </summary>
public class ScriptingCommandTests(TestConfiguration config)
{
    #region Properties

    public TestConfiguration Config { get; } = config;

    #endregion
    #region ScriptEvaluateAsync (string script)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_SimpleScript_ReturnsResult(IDatabaseAsync db)
    {
        ValkeyResult result = await db.ScriptEvaluateAsync("return 'Hello, World!'");

        Assert.Equal("Hello, World!", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_WithKeys_AccessesKeys(IDatabaseAsync db)
    {
        ValkeyKey key = BuildKey("ser-script-key-");
        ValkeyValue value = BuildValue();
        _ = await db.StringSetAsync(key, value);

        ValkeyResult result = await db.ScriptEvaluateAsync(
            "return redis.call('GET', KEYS[1])",
            keys: [key]);

        Assert.Equal(value, (ValkeyValue)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_WithKeysAndValues_UsesArguments(IDatabaseAsync db)
    {
        ValkeyKey key = BuildKey("ser-script-kv-");
        ValkeyValue value = BuildValue();

        ValkeyResult result = await db.ScriptEvaluateAsync(
            "redis.call('SET', KEYS[1], ARGV[1]); return redis.call('GET', KEYS[1])",
            keys: [key],
            values: [value]);

        Assert.Equal(value, (ValkeyValue)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_ReturnsNumber(IDatabaseAsync db)
    {
        ValkeyResult result = await db.ScriptEvaluateAsync("return 42");

        Assert.Equal(42, (int)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_ReturnsArray(IDatabaseAsync db)
    {
        ValkeyResult result = await db.ScriptEvaluateAsync("return {'a', 'b', 'c'}");

        ValkeyResult[]? array = (ValkeyResult[]?)result;
        Assert.NotNull(array);
        Assert.Equal(3, array.Length);
        Assert.Equal("a", array[0].ToString());
        Assert.Equal("b", array[1].ToString());
        Assert.Equal("c", array[2].ToString());
    }

    #endregion
    #region ScriptEvaluateAsync (LuaScript)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_LuaScript_SimpleScript(IDatabaseAsync db)
    {
        LuaScript script = LuaScript.Prepare("return 'lua-script'");

        ValkeyResult result = await db.ScriptEvaluateAsync(script);

        Assert.Equal("lua-script", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_LuaScript_WithParameters(IDatabaseAsync db)
    {
        ValkeyKey key = BuildKey("ser-lua-param-");
        ValkeyValue value = BuildValue();
        LuaScript script = LuaScript.Prepare("redis.call('SET', @key, @value); return redis.call('GET', @key)");

        ValkeyResult result = await db.ScriptEvaluateAsync(script, new { key, value });

        Assert.Equal(value, (ValkeyValue)result);
    }

    #endregion
}
