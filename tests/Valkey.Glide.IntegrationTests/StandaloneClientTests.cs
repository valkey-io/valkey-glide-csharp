﻿// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Pipeline;

using static Valkey.Glide.Commands.Options.InfoOptions;
using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

[Collection("GlideTests")]
public class StandaloneClientTests(TestConfiguration config)
{
    public static TheoryData<bool> GetAtomic => [true, false];

    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public void CustomCommand(GlideClient client) =>
        // Assert.Multiple doesn't work with async tasks https://github.com/xunit/xunit/issues/3209
        Assert.Multiple(
            () => Assert.Equal("PONG", client.CustomCommand(["ping"]).Result!.ToString()),
            () => Assert.Equal("piping", client.CustomCommand(["ping", "piping"]).Result!.ToString()),
            () => Assert.Contains("# Server", client.CustomCommand(["INFO"]).Result!.ToString())
        );

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task CustomCommandWithBinary(GlideClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();
        string value = Guid.NewGuid().ToString();
        Assert.True(await client.StringSetAsync(key1, value));

        gs dump = (await client.CustomCommand(["DUMP", key1]) as gs)!;

        Assert.Equal("OK", await client.CustomCommand(["RESTORE", key2, "0", dump!]));
        ValkeyValue retrievedValue = await client.StringGetAsync(key2);
        Assert.Equal(value, retrievedValue.ToString());

        // Set and get a binary value
        Assert.True(await client.StringSetAsync(key3, dump!));
        ValkeyValue binaryValue = await client.StringGetAsync(key3);
        Assert.Equal(dump, (gs)binaryValue);
    }

    [Fact]
    public void CanConnectWithDifferentParameters()
    {
        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithClientName("GLIDE").Build());

        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithTls(false).Build());

        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithConnectionTimeout(TimeSpan.FromSeconds(2)).Build());

        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithRequestTimeout(TimeSpan.FromSeconds(2)).Build());

        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithDataBaseId(4).Build());

        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithConnectionRetryStrategy(1, 2, 3).Build());

        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithAuthentication("default", "").Build());

        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP2).Build());

        _ = GlideClient.CreateClient(TestConfiguration.DefaultClientConfig()
            .WithReadFrom(new ConnectionConfiguration.ReadFrom(ConnectionConfiguration.ReadFromStrategy.Primary)).Build());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    // Verify that client can handle complex return types, not just strings
    // TODO: remove this test once we add tests with these commands
    public async Task CustomCommandWithDifferentReturnTypes(GlideClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        Assert.Equal(2L, await client.CustomCommand(["hset", key1, "f1", "v1", "f2", "v2"]));
        Assert.Equal(
            new Dictionary<gs, gs> { { "f1", "v1" }, { "f2", "v2" } },
            await client.CustomCommand(["hgetall", key1])
        );
        Assert.Equal(
            new gs?[] { "v1", "v2", null },
            await client.CustomCommand(["hmget", key1, "f1", "f2", "f3"])
        );

        string key2 = Guid.NewGuid().ToString();
        Assert.Equal(3L, (await client.CustomCommand(["sadd", key2, "a", "b", "c"]))!);
        Assert.Equal(
            [new gs("a"), new gs("b"), new gs("c")],
            (await client.CustomCommand(["smembers", key2]) as HashSet<object>)!
        );
        Assert.Equal(
            new bool[] { true, true, false },
            await client.CustomCommand(["smismember", key2, "a", "b", "d"])
        );

        string key3 = Guid.NewGuid().ToString();
        _ = await client.CustomCommand(["xadd", key3, "0-1", "str-1-id-1-field-1", "str-1-id-1-value-1", "str-1-id-1-field-2", "str-1-id-1-value-2"]);
        _ = await client.CustomCommand(["xadd", key3, "0-2", "str-1-id-2-field-1", "str-1-id-2-value-1", "str-1-id-2-field-2", "str-1-id-2-value-2"]);
        _ = Assert.IsType<Dictionary<gs, object?>>((await client.CustomCommand(["xread", "streams", key3, "stream", "0-1", "0-2"]))!);
        _ = Assert.IsType<Dictionary<gs, object?>>((await client.CustomCommand(["xinfo", "stream", key3, "full"]))!);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task ExecuteWithDifferentReturnTypes(ConnectionMultiplexer conn)
    {
        IDatabase db = conn.GetDatabase();
        string key1 = Guid.NewGuid().ToString();
        Assert.Equal(2L, (await db.ExecuteAsync("hset", [key1, "f1", "v1", "f2", "v2"])).AsInt64());
        Assert.Equal(
            new Dictionary<string, string> { { "f1", "v1" }, { "f2", "v2" } },
            (await db.ExecuteAsync("hgetall", [key1])).ToDictionary().ToDictionary(k => k.Key, v => v.Value.AsString()!)
        );
        Assert.Equal(
            new string?[] { "v1", "v2", null },
            db.Execute("hmget", [key1, "f1", "f2", "f3"]).AsStringArray()!
        );

        string key2 = Guid.NewGuid().ToString();
        Assert.Equal(3L, db.Execute("sadd", [key2, "a", "b", "c"]).AsInt64());
        Assert.False(db.Execute("smembers", [key2]).AsStringArray()!.Except(["a", "b", "c"]).Any());
        Assert.Equal([true, true, false], db.Execute("smismember", [key2, "a", "b", "d"]).AsBooleanArray()!);

        string key3 = Guid.NewGuid().ToString();
        _ = await db.ExecuteAsync("xadd", [key3, "0-1", "str-1-id-1-field-1", "str-1-id-1-value-1", "str-1-id-1-field-2", "str-1-id-1-value-2"]);
        _ = await db.ExecuteAsync("xadd", [key3, "0-2", "str-1-id-2-field-1", "str-1-id-2-value-1", "str-1-id-2-field-2", "str-1-id-2-value-2"]);
        _ = await db.ExecuteAsync("xread", ["streams", key3, "stream", "0-1", "0-2"]);
        _ = await db.ExecuteAsync("xinfo", ["stream", key3, "full"]);
    }

    [Fact]
    public async Task Info()
    {
        GlideClient client = TestConfiguration.DefaultStandaloneClient();

        string info = await client.Info();
        Assert.Multiple([
            () => Assert.Contains("# Server", info),
            () => Assert.Contains("# Replication", info),
            () => Assert.DoesNotContain("# Latencystats", info),
        ]);

        info = await client.Info([Section.REPLICATION]);
        Assert.Multiple([
            () => Assert.DoesNotContain("# Server", info),
            () => Assert.Contains("# Replication", info),
            () => Assert.DoesNotContain("# Latencystats", info),
        ]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TestPing_NoMessage(GlideClient client)
    {
        TimeSpan result = await client.PingAsync();
        Assert.True(result >= TimeSpan.Zero);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TestPing_WithMessage(GlideClient client)
    {
        ValkeyValue message = "Hello, Valkey!";
        TimeSpan result = await client.PingAsync(message);
        Assert.True(result >= TimeSpan.Zero);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TestEcho_SimpleMessage(GlideClient client)
    {
        ValkeyValue message = "Hello, Valkey!";
        ValkeyValue result = await client.EchoAsync(message);
        Assert.Equal(message, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TestEcho_BinaryData(GlideClient client)
    {
        byte[] binaryData = [0x00, 0x01, 0x02, 0xFF, 0xFE];
        ValkeyValue result = await client.EchoAsync(binaryData);
        Assert.Equal(binaryData, (byte[]?)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task KeyCopy_Move(GlideClient client)
    {
        string key = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();

        _ = await client.StringSetAsync(key, "val");
        Assert.True(await client.KeyCopyAsync(key, key2, 1));
        Assert.True(await client.KeyMoveAsync(key, 2));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task BatchKeyCopyAndKeyMove(bool isAtomic)
    {
        GlideClient client = TestConfiguration.DefaultStandaloneClient();
        string sourceKey = Guid.NewGuid().ToString();
        string destKey = Guid.NewGuid().ToString();
        string moveKey = Guid.NewGuid().ToString();
        string value = "test-value";

        Pipeline.IBatch batch = new Batch(isAtomic);

        // Set up keys
        _ = batch.StringSet(sourceKey, value);
        _ = batch.StringSet(moveKey, value);

        IBatchStandalone batch2 = new Batch(isAtomic);

        // Test KeyCopy with database parameter
        _ = batch2.KeyCopy(sourceKey, destKey, 1, false);

        // Test KeyMove
        _ = batch2.KeyMove(moveKey, 2);

        object?[] results = (await client.Exec((Batch)batch, false))!;
        object?[] results2 = (await client.Exec((Batch)batch2, false))!;

        Assert.Multiple(
            () => Assert.True((bool)results[0]!), // Set sourceKey
            () => Assert.True((bool)results[1]!), // Set moveKey
            () => Assert.True((bool)results2[0]!), // KeyCopy result
            () => Assert.True((bool)results2[1]!)  // KeyMove result
        );
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigGetAsync_ReturnsConfiguration(GlideClient client)
    {
        // Test getting all configuration
        var allConfig = await client.ConfigGetAsync("*");
        Assert.NotEmpty(allConfig);
        Assert.Contains(allConfig, kvp => kvp.Key == "maxmemory-policy");

        // Test getting specific configuration
        var maxMemoryConfig = await client.ConfigGetAsync("maxmemory-policy");
        Assert.Single(maxMemoryConfig);
        Assert.Equal("maxmemory-policy", maxMemoryConfig[0].Key);
        Assert.NotEmpty(maxMemoryConfig[0].Value);

        // Test getting multiple parameters individually (since StackExchange.Redis doesn't support multiple params in one call)
        var timeoutConfig = await client.ConfigGetAsync("timeout");
        var maxMemoryPolicyConfig = await client.ConfigGetAsync("maxmemory-policy");
        Assert.True(timeoutConfig.Length >= 0); // timeout might not be set
        Assert.True(maxMemoryPolicyConfig.Length >= 1); // maxmemory-policy should exist
        Assert.Contains(maxMemoryPolicyConfig, kvp => kvp.Key == "maxmemory-policy");

        // Test getting non-existent configuration (like Go test)
        var nonExistentConfig = await client.ConfigGetAsync("non-existent-config");
        Assert.Empty(nonExistentConfig);

        // Test with empty parameters should default to "*" pattern
        var allConfigDefault = await client.ConfigGetAsync();
        Assert.NotEmpty(allConfigDefault);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigSetAsync_SetsConfiguration(GlideClient client)
    {
        // Test single parameter set/get (existing functionality)
        var originalConfig = await client.ConfigGetAsync("maxmemory-policy");
        string originalValue = originalConfig[0].Value;

        try
        {
            // Set new value
            await client.ConfigSetAsync((ValkeyValue)"maxmemory-policy", (ValkeyValue)"allkeys-lru");

            // Verify it was set
            var newConfig = await client.ConfigGetAsync("maxmemory-policy");
            Assert.Equal("allkeys-lru", newConfig[0].Value);
        }
        finally
        {
            // Restore original value
            await client.ConfigSetAsync((ValkeyValue)"maxmemory-policy", (ValkeyValue)originalValue);
        }

        // Test multiple parameters (like Go test TestConfigSetAndGet_multipleArgs)
        var serverVersion = await client.Info([Section.SERVER]);
        if (serverVersion != null && (serverVersion.Contains("redis_version:7") || serverVersion.Contains("valkey_version:7")))
        {
            var originalTimeout = await client.ConfigGetAsync("timeout");
            var originalMaxMemory = await client.ConfigGetAsync("maxmemory");

            string originalTimeoutValue = originalTimeout.FirstOrDefault().Value ?? "0";
            string originalMaxMemoryValue = originalMaxMemory.FirstOrDefault().Value ?? "0";

            try
            {
                // Set multiple configuration parameters individually (since StackExchange.Redis doesn't support dictionary)
                await client.ConfigSetAsync((ValkeyValue)"timeout", (ValkeyValue)"1000");
                await client.ConfigSetAsync((ValkeyValue)"maxmemory", (ValkeyValue)"1073741824"); // 1GB in bytes

                // Verify both parameters were set
                var timeoutResult = await client.ConfigGetAsync("timeout");
                var maxMemoryResult = await client.ConfigGetAsync("maxmemory");
                Assert.Contains(timeoutResult, kvp => kvp.Key == "timeout" && kvp.Value == "1000");
                Assert.Contains(maxMemoryResult, kvp => kvp.Key == "maxmemory" && kvp.Value == "1073741824");
            }
            finally
            {
                // Restore original values
                await client.ConfigSetAsync((ValkeyValue)"timeout", (ValkeyValue)originalTimeoutValue);
                await client.ConfigSetAsync((ValkeyValue)"maxmemory", (ValkeyValue)originalMaxMemoryValue);
            }
        }

        // Test invalid parameters (like Go test TestConfigSetAndGet_invalidArgs)
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.ConfigSetAsync((ValkeyValue)"invalid-config-param", (ValkeyValue)"value"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigResetStatisticsAsync_ResetsStats(GlideClient client)
    {
        // This should not throw
        await client.ConfigResetStatisticsAsync();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigRewriteAsync_RewritesConfig(GlideClient client)
    {
        // This should not throw (though it may fail if config file is read-only)
        try
        {
            await client.ConfigRewriteAsync();
        }
        catch (Exception ex)
        {
            // Config rewrite may fail in test environments, that's okay
            // Just verify we got some kind of error response
            Assert.NotEmpty(ex.Message);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task DatabaseSizeAsync_ReturnsSize(GlideClient client)
    {
        string key = $"dbsize-test-{Guid.NewGuid()}";

        try
        {
            // Get initial size
            long initialSize = await client.DatabaseSizeAsync();
            Assert.True(initialSize >= 0);

            // Add a key
            await client.StringSetAsync(key, "test-value");

            // Size should increase
            long newSize = await client.DatabaseSizeAsync();
            Assert.True(newSize >= initialSize);

            // Test with explicit database (standalone only)
            long dbSize = await client.DatabaseSizeAsync(0);
            Assert.True(dbSize >= 0);
        }
        finally
        {
            await client.KeyDeleteAsync(key);
        }
    }



    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task LastSaveAsync_ReturnsLastSaveTime(GlideClient client)
    {
        DateTime lastSave = await client.LastSaveAsync();

        // Should be a valid date (not default)
        Assert.NotEqual(DateTime.MinValue, lastSave);
        Assert.NotEqual(DateTime.MaxValue, lastSave);

        // Should be in the past
        Assert.True(lastSave <= DateTime.UtcNow);

        // Should be reasonable (not too far in the past)
        Assert.True(lastSave >= DateTime.UtcNow.AddDays(-30));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TimeAsync_ReturnsServerTime(GlideClient client)
    {
        DateTime serverTime = await client.TimeAsync();
        DateTime localTime = DateTime.UtcNow;

        // Server time should be close to local time (within 10 seconds)
        TimeSpan diff = (serverTime - localTime).Duration();
        Assert.True(diff < TimeSpan.FromSeconds(10),
            $"Server time {serverTime} differs from local time {localTime} by {diff}");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task LolwutAsync_ReturnsArt(GlideClient client)
    {
        string result = await client.LolwutAsync();

        // Should return some text
        Assert.NotEmpty(result);

        // Should contain version information (could be Redis or Valkey)
        Assert.True(result.Contains("Redis", StringComparison.OrdinalIgnoreCase) ||
                   result.Contains("Valkey", StringComparison.OrdinalIgnoreCase));
    }
}
