// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> scripting commands.
/// These tests verify the StackExchange.Redis-compatible API surface.
/// </summary>
public class ScriptingCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

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
        string key = $"ser-script-key-{Guid.NewGuid()}";
        _ = await db.StringSetAsync(key, "test-value");

        ValkeyResult result = await db.ScriptEvaluateAsync(
            "return redis.call('GET', KEYS[1])",
            keys: [(ValkeyKey)key]);

        Assert.Equal("test-value", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_WithKeysAndValues_UsesArguments(IDatabaseAsync db)
    {
        string key = $"ser-script-kv-{Guid.NewGuid()}";

        ValkeyResult result = await db.ScriptEvaluateAsync(
            "redis.call('SET', KEYS[1], ARGV[1]); return redis.call('GET', KEYS[1])",
            keys: [(ValkeyKey)key],
            values: [(ValkeyValue)"script-value"]);

        Assert.Equal("script-value", result.ToString());
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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_WithCommandFlags_ThrowsOnNonNone(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ScriptEvaluateAsync("return 1", null, null, CommandFlags.DemandMaster));

    #endregion

    #region ScriptEvaluateAsync (byte[] hash)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_WithHash_CommandFlags_ThrowsOnNonNone(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ScriptEvaluateAsync(new byte[20], null, null, CommandFlags.DemandMaster));

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
        string key = $"ser-lua-param-{Guid.NewGuid()}";
        LuaScript script = LuaScript.Prepare("redis.call('SET', @key, @value); return redis.call('GET', @key)");

        ValkeyResult result = await db.ScriptEvaluateAsync(script, new { key = (ValkeyKey)key, value = "param-value" });

        Assert.Equal("param-value", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_LuaScript_WithCommandFlags_ThrowsOnNonNone(IDatabaseAsync db)
    {
        LuaScript script = LuaScript.Prepare("return 1");

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ScriptEvaluateAsync(script, null, CommandFlags.DemandMaster));
    }

    #endregion

    #region ScriptEvaluateAsync (LoadedLuaScript)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_LoadedLuaScript_WithCommandFlags_ThrowsOnNonNone(IDatabaseAsync db)
    {
        // Create a LoadedLuaScript with a mock hash - the hash doesn't matter for this test
        // since we're testing that CommandFlags throws before execution
        LuaScript script = LuaScript.Prepare("return 1");
        byte[] mockHash = new byte[20];
        LoadedLuaScript loaded = new(script, mockHash, script.ExecutableScript);

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ScriptEvaluateAsync(loaded, null, CommandFlags.DemandMaster));
    }

    #endregion
}
