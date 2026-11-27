// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

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
        => Assert.Equal("Hello, World!", await db.ScriptEvaluateAsync("return 'Hello, World!'"));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_WithKeys_AccessesKeys(IDatabaseAsync db)
    {
        var key = BuildKey("ser-script-key-");
        var value = BuildValue();
        _ = await db.StringSetAsync(key, value);

        Assert.Equal(
            value,
            await db.ScriptEvaluateAsync(
                "return redis.call('GET', KEYS[1])",
                keys: [key]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_WithKeysAndValues_UsesArguments(IDatabaseAsync db)
    {
        var key = BuildKey("ser-script-kv-");
        var value = BuildValue();

        Assert.Equal(
            value,
            await db.ScriptEvaluateAsync(
                "redis.call('SET', KEYS[1], ARGV[1]); return redis.call('GET', KEYS[1])",
                keys: [key],
                values: [value]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_ReturnsNumber(IDatabaseAsync db)
        => Assert.Equal(42, await db.ScriptEvaluateAsync("return 42"));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_ReturnsArray(IDatabaseAsync db)
        => Assert.Equal(["a", "b", "c"], await db.ScriptEvaluateAsync("return {'a', 'b', 'c'}"));

    #endregion
    #region ScriptEvaluateAsync (LuaScript)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_LuaScript_SimpleScript(IDatabaseAsync db)
    {
        var script = LuaScript.Prepare("return 'lua-script'");
        Assert.Equal("lua-script", await db.ScriptEvaluateAsync(script));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_LuaScript_WithParameters(IDatabaseAsync db)
    {
        ValkeyKey key = BuildKey("ser-lua-param-");
        ValkeyValue value = BuildValue();
        LuaScript script = LuaScript.Prepare("redis.call('SET', @key, @value); return redis.call('GET', @key)");

        Assert.Equal(value, await db.ScriptEvaluateAsync(script, new { key, value }));
    }

    #endregion
}
