// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

using Valkey.Glide.Commands.Options;

public class GenericCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyDelete_KeyExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key first
        await client.SetAsync(key, value);
        Assert.True(await client.ExistsAsync(key));

        // Delete the key
        Assert.True(await client.DeleteAsync(key));
        Assert.False(await client.ExistsAsync(key));

        // Try to delete non-existent key
        Assert.False(await client.DeleteAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyDelete_MultipleKeys(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();

        // Set keys
        await client.SetAsync(key1, "value1");
        await client.SetAsync(key2, "value2");

        // Delete multiple keys (including non-existent)
        long deletedCount = await client.DeleteAsync([key1, key2, key3]);
        Assert.Equal(2, deletedCount);

        Assert.False(await client.ExistsAsync(key1));
        Assert.False(await client.ExistsAsync(key2));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyUnlink(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key first
        await client.SetAsync(key, value);
        Assert.True(await client.ExistsAsync(key));

        // Unlink the key
        Assert.True(await client.UnlinkAsync(key));
        Assert.False(await client.ExistsAsync(key));

        // Try to unlink non-existent key
        Assert.False(await client.UnlinkAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyExists_MultipleKeys(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();

        // Set some keys
        await client.SetAsync(key1, "value1");
        await client.SetAsync(key2, "value2");

        // Check existence
        long existingCount = await client.ExistsAsync([key1, key2, key3]);
        Assert.Equal(2, existingCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TimeToLiveAsync_ReturnsSentinelValues(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string nonExistentKey = Guid.NewGuid().ToString();

        // Set a key without expiry
        await client.SetAsync(key, "value");

        // Key exists but has no expiry -> HasTimeToLive is false
        var ttl = await client.TimeToLiveAsync(key);
        Assert.True(ttl.Exists);
        Assert.False(ttl.HasTimeToLive);

        // Non-existent key -> Exists is false
        ttl = await client.TimeToLiveAsync(nonExistentKey);
        Assert.False(ttl.Exists);

        // Set expiry and verify positive TTL
        Assert.True(await client.ExpireAsync(key, TimeSpan.FromSeconds(10)));
        ttl = await client.TimeToLiveAsync(key);
        Assert.True(ttl.HasTimeToLive);
        Assert.True(ttl.TimeToLive!.Value.TotalMilliseconds > 0 && ttl.TimeToLive.Value.TotalMilliseconds <= 10000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyExpire_TimeToLive(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key
        await client.SetAsync(key, value);

        // Set expiry with seconds precision (should use EXPIRE)
        Assert.True(await client.ExpireAsync(key, TimeSpan.FromSeconds(10)));

        // Check TTL - GLIDE-style returns TimeToLiveResult
        var ttl = await client.TimeToLiveAsync(key);
        Assert.True(ttl.HasTimeToLive);
        Assert.True(ttl.TimeToLive!.Value.TotalMilliseconds > 0 && ttl.TimeToLive.Value.TotalMilliseconds <= 10000);

        // Test with millisecond precision (should use PEXPIRE)
        Assert.True(await client.ExpireAsync(key, TimeSpan.FromMilliseconds(5500)));

        ttl = await client.TimeToLiveAsync(key);
        // Now with PTTL support, we should get millisecond precision
        Assert.True(ttl.HasTimeToLive);
        Assert.True(ttl.TimeToLive!.Value.TotalMilliseconds > 0 && ttl.TimeToLive.Value.TotalMilliseconds <= 5500);

        // Test with DateTimeOffset (should use EXPIREAT or PEXPIREAT based on precision)
        DateTimeOffset expireTime = DateTimeOffset.UtcNow.AddSeconds(15);
        Assert.True(await client.ExpireAsync(key, expireTime));

        ttl = await client.TimeToLiveAsync(key);
        Assert.True(ttl.TimeToLive!.Value.TotalMilliseconds > 10000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyExpireTime(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "PEXPIRETIME is supported since 7.0.0"
        );
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key
        await client.SetAsync(key, value);

        // Key without expiry should return null
        DateTimeOffset? expireTime = await client.ExpireTimeAsync(key);
        Assert.Null(expireTime);

        // Set expiry and check expire time
        DateTimeOffset futureTime = DateTimeOffset.UtcNow.AddSeconds(30);
        Assert.True(await client.ExpireAsync(key, futureTime));

        expireTime = await client.ExpireTimeAsync(key);
        _ = Assert.NotNull(expireTime);
        // Should be close to the set time (within a few seconds tolerance)
        Assert.True(Math.Abs((expireTime.Value - futureTime).TotalSeconds) < 5);

        // Non-existent key should return null
        string nonExistentKey = Guid.NewGuid().ToString();
        expireTime = await client.ExpireTimeAsync(nonExistentKey);
        Assert.Null(expireTime);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyEncoding(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a string key
        await client.SetAsync(key, value);

        // Get encoding for string key
        string? encoding = await client.ObjectEncodingAsync(key);
        Assert.NotNull(encoding);
        // String encoding can be "raw", "embstr", or "int" depending on the value
        Assert.Contains(encoding, new[] { "raw", "embstr", "int" });

        // Test with different data types
        string listKey = Guid.NewGuid().ToString();
        _ = await client.ListLeftPushAsync(listKey, "item");
        string? listEncoding = await client.ObjectEncodingAsync(listKey);
        Assert.NotNull(listEncoding);

        string setKey = Guid.NewGuid().ToString();
        _ = await client.SetAddAsync(setKey, "member");
        string? setEncoding = await client.ObjectEncodingAsync(setKey);
        Assert.NotNull(setEncoding);

        // Non-existent key should return null
        string nonExistentKey = Guid.NewGuid().ToString();
        string? nonExistentEncoding = await client.ObjectEncodingAsync(nonExistentKey);
        Assert.Null(nonExistentEncoding);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyFrequency(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a string key
        await client.SetAsync(key, value);

        try
        {
            // Get frequency for string key
            long? frequency = await client.ObjectFrequencyAsync(key);
            _ = Assert.NotNull(frequency);
            Assert.True(frequency >= 0);

            // Non-existent key should return null
            string nonExistentKey = Guid.NewGuid().ToString();
            long? nonExistentFrequency = await client.ObjectFrequencyAsync(nonExistentKey);
            Assert.Null(nonExistentFrequency);
        }
        catch (Errors.RequestException ex) when (ex.Message.Contains("LFU maxmemory policy is not selected"))
        {
            // This is expected when LFU eviction policy is not configured
            // The command implementation is correct, but server doesn't track frequency
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyIdleTime(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a string key
        await client.SetAsync(key, value);

        // Get idle time for string key
        TimeSpan? idleTime = await client.ObjectIdleTimeAsync(key);
        _ = Assert.NotNull(idleTime);
        Assert.True(idleTime.Value.TotalSeconds >= 0);

        // Wait a bit and check that idle time increases
        await Task.Delay(1000, TestContext.Current.CancellationToken);
        TimeSpan? idleTime2 = await client.ObjectIdleTimeAsync(key);
        _ = Assert.NotNull(idleTime2);
        Assert.True(idleTime2.Value.TotalSeconds >= idleTime.Value.TotalSeconds);

        // Access the key to reset idle time
        _ = await client.GetAsync(key);
        TimeSpan? idleTimeAfterAccess = await client.ObjectIdleTimeAsync(key);
        _ = Assert.NotNull(idleTimeAfterAccess);
        Assert.True(idleTimeAfterAccess.Value.TotalSeconds < idleTime2.Value.TotalSeconds);

        // Non-existent key should return null
        string nonExistentKey = Guid.NewGuid().ToString();
        TimeSpan? nonExistentIdleTime = await client.ObjectIdleTimeAsync(nonExistentKey);
        Assert.Null(nonExistentIdleTime);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyRefCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a string key
        await client.SetAsync(key, value);

        // Get reference count for string key
        long? refCount = await client.ObjectRefCountAsync(key);
        _ = Assert.NotNull(refCount);
        Assert.True(refCount >= 1);

        // Non-existent key should return null
        string nonExistentKey = Guid.NewGuid().ToString();
        long? nonExistentRefCount = await client.ObjectRefCountAsync(nonExistentKey);
        Assert.Null(nonExistentRefCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyType(BaseClient client)
    {
        string stringKey = Guid.NewGuid().ToString();
        string setKey = Guid.NewGuid().ToString();

        // Test string type
        await client.SetAsync(stringKey, "value");
        ValkeyType stringType = await client.TypeAsync(stringKey);
        Assert.Equal(ValkeyType.String, stringType);

        // Test set type
        _ = await client.SetAddAsync(setKey, "member");
        ValkeyType setType = await client.TypeAsync(setKey);
        Assert.Equal(ValkeyType.Set, setType);

        // Test non-existent key
        string nonExistentKey = Guid.NewGuid().ToString();
        ValkeyType noneType = await client.TypeAsync(nonExistentKey);
        Assert.Equal(ValkeyType.None, noneType);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyRename(BaseClient client)
    {
        string oldKey = "{prefix}-" + Guid.NewGuid().ToString();
        string newKey = "{prefix}-" + Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key
        await client.SetAsync(oldKey, value);

        // Rename the key
        await client.RenameAsync(oldKey, newKey);

        // Check that old key doesn't exist and new key exists
        Assert.False(await client.ExistsAsync(oldKey));
        Assert.True(await client.ExistsAsync(newKey));
        Assert.Equal(value, await client.GetAsync(newKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyRenameIfNotExists(BaseClient client)
    {
        string oldKey = "{prefix}-" + Guid.NewGuid().ToString();
        string newKey = "{prefix}-" + Guid.NewGuid().ToString();
        string existingKey = "{prefix}-" + Guid.NewGuid().ToString();
        string value = "test_value";
        string existingValue = "existing_value";

        // Set keys
        await client.SetAsync(oldKey, value);
        await client.SetAsync(existingKey, existingValue);

        // Rename to non-existing key should succeed
        Assert.True(await client.RenameIfNotExistsAsync(oldKey, newKey));

        // Check that old key doesn't exist and new key exists
        Assert.False(await client.ExistsAsync(oldKey));
        Assert.True(await client.ExistsAsync(newKey));
        Assert.Equal(value, await client.GetAsync(newKey));

        // Try to rename to existing key should fail
        Assert.False(await client.RenameIfNotExistsAsync(newKey, existingKey));

        // Both keys should still exist with original values
        Assert.True(await client.ExistsAsync(newKey));
        Assert.True(await client.ExistsAsync(existingKey));
        Assert.Equal(value, await client.GetAsync(newKey));
        Assert.Equal(existingValue, await client.GetAsync(existingKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyPersist(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key with expiry
        await client.SetAsync(key, value);
        _ = await client.ExpireAsync(key, TimeSpan.FromSeconds(10));

        // Verify it has TTL (HasTimeToLive means has expiry)
        var ttl = await client.TimeToLiveAsync(key);
        Assert.True(ttl.HasTimeToLive);

        // Persist the key
        Assert.True(await client.PersistAsync(key));

        // Verify TTL is removed (HasTimeToLive = false means no expiry)
        ttl = await client.TimeToLiveAsync(key);
        Assert.False(ttl.HasTimeToLive);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyDump_KeyRestore(BaseClient client)
    {
        string sourceKey = Guid.NewGuid().ToString();
        string destKey = Guid.NewGuid().ToString();
        string replaceKey = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key
        await client.SetAsync(sourceKey, value);

        // Dump the key
        byte[]? dumpData = await client.DumpAsync(sourceKey);
        Assert.NotNull(dumpData);
        Assert.NotEmpty(dumpData);

        // Restore to a new key
        await client.RestoreAsync(destKey, dumpData);

        // Verify the restored key
        Assert.Equal(value, await client.GetAsync(destKey));

        // Test RestoreOptions with Replace
        await client.SetAsync(replaceKey, "old_value");
        await client.RestoreAsync(replaceKey, dumpData, new RestoreOptions { Replace = true });
        Assert.Equal(value, await client.GetAsync(replaceKey));

        // Test RestoreOptions with TTL
        string ttlKey = Guid.NewGuid().ToString();
        await client.RestoreAsync(ttlKey, dumpData, new RestoreOptions { Ttl = TimeSpan.FromSeconds(30) });

        // Verify key exists and has TTL
        Assert.True(await client.ExistsAsync(ttlKey));
        var ttl = await client.TimeToLiveAsync(ttlKey);
        Assert.True(ttl.HasTimeToLive);

        // Test RestoreOptions with ExpireAt
        string expireAtKey = Guid.NewGuid().ToString();
        await client.RestoreAsync(expireAtKey, dumpData, new RestoreOptions { ExpireAt = DateTimeOffset.UtcNow.AddYears(10) });

        // Verify key exists and has TTL
        Assert.True(await client.ExistsAsync(expireAtKey));
        var expireAtTtl = await client.TimeToLiveAsync(expireAtKey);
        Assert.True(expireAtTtl.HasTimeToLive);

        // Test RestoreOptions with IDLETIME
        string idletimeKey = Guid.NewGuid().ToString();
        await client.RestoreAsync(idletimeKey, dumpData, new RestoreOptions { IdleTime = 1000 });
        Assert.Equal(value, await client.GetAsync(idletimeKey));

        // Test RestoreOptions with FREQ
        string freqKey = Guid.NewGuid().ToString();
        await client.RestoreAsync(freqKey, dumpData, new RestoreOptions { Frequency = 5 });
        Assert.Equal(value, await client.GetAsync(freqKey));

        // Test combined options
        string combinedKey = Guid.NewGuid().ToString();
        await client.RestoreAsync(combinedKey, dumpData, new RestoreOptions { Ttl = TimeSpan.FromMinutes(5), Replace = false, IdleTime = 500 });
        Assert.Equal(value, await client.GetAsync(combinedKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyTouch(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();

        // Set some keys
        await client.SetAsync(key1, "value1");
        await client.SetAsync(key2, "value2");

        // Touch single key
        Assert.True(await client.TouchAsync(key1));

        // Touch multiple keys (including non-existent)
        long touchedCount = await client.TouchAsync([key1, key2, key3]);
        Assert.Equal(2, touchedCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyCopy(BaseClient client)
    {
        string sourceKey = "{prefix}-" + Guid.NewGuid().ToString();
        string destKey = "{prefix}-" + Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key
        await client.SetAsync(sourceKey, value);

        // Copy the key
        Assert.True(await client.CopyAsync(sourceKey, destKey));

        // Verify both keys exist with same value
        Assert.Equal(value, await client.GetAsync(sourceKey));
        Assert.Equal(value, await client.GetAsync(destKey));

        // Test copy with replace
        await client.SetAsync(destKey, "new_value");
        Assert.True(await client.CopyAsync(sourceKey, destKey, replace: true));
        Assert.Equal(value, await client.GetAsync(destKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyRandom(BaseClient client)
    {
        // Test with empty database
        _ = await client.RandomKeyAsync();
        // May be null if database is empty, or return an existing key

        // Set some keys to ensure we have data
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();

        await client.SetAsync(key1, "value1");
        await client.SetAsync(key2, "value2");
        await client.SetAsync(key3, "value3");

        // Now we should get a random key
        ValkeyKey? randomKey = await client.RandomKeyAsync();
        _ = Assert.NotNull(randomKey);
        Assert.True(await client.ExistsAsync(randomKey.Value));

        // Call multiple times to verify it can return different keys
        HashSet<string> seenKeys = [];
        for (int i = 0; i < 10; i++)
        {
            ValkeyKey? key = await client.RandomKeyAsync();
            if (key.HasValue)
            {
                _ = seenKeys.Add(key.Value.ToString());
            }
        }

        // We should have seen at least one key
        Assert.NotEmpty(seenKeys);
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSort(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test with list
        _ = await client.ListLeftPushAsync(key, ["3", "1", "2"]);
        ValkeyValue[] result = await client.SortAsync(key);
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);

        // Test with descending order
        result = await client.SortAsync(key, order: Order.Descending);
        Assert.Equal(["3", "2", "1"], [.. result.Select(v => v.ToString())]);

        // Test with limit
        result = await client.SortAsync(key, skip: 1, take: 1);
        _ = Assert.Single(result);
        Assert.Equal("2", result[0].ToString());

        // Test alphabetic sort
        string alphaKey = Guid.NewGuid().ToString();
        _ = await client.ListLeftPushAsync(alphaKey, ["b", "a", "c"]);
        result = await client.SortAsync(alphaKey, sortType: SortType.Alphabetic);
        Assert.Equal(["a", "b", "c"], [.. result.Select(v => v.ToString())]);

        string userKey = Guid.NewGuid().ToString();

        // Test with BY pattern (skip for cluster clients as BY option is denied in cluster mode)
        if (client is not GlideClusterClient)
        {
            _ = await client.HashSetAsync("user:1", [new HashEntry("age", "30")]);
            _ = await client.HashSetAsync("user:2", [new HashEntry("age", "25")]);
            _ = await client.ListLeftPushAsync(userKey, ["2", "1"]);
            result = await client.SortAsync(userKey, by: "user:*->age");
            Assert.Equal(["2", "1"], [.. result.Select(v => v.ToString())]);

            // Test with GET pattern
            _ = await client.HashSetAsync("user:1", [new HashEntry("name", "Alice")]);
            _ = await client.HashSetAsync("user:2", [new HashEntry("name", "Bob")]);
            result = await client.SortAsync(userKey, by: "user:*->age", get: ["user:*->name"]);
            Assert.Equal(["Bob", "Alice"], [.. result.Select(v => v.ToString())]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortAndStore(BaseClient client)
    {
        string sourceKey = "{prefix}-" + Guid.NewGuid().ToString();
        string destKey = "{prefix}-" + Guid.NewGuid().ToString();

        // Test basic sort and store
        _ = await client.ListLeftPushAsync(sourceKey, ["3", "1", "2"]);
        long count = await client.SortAndStoreAsync(destKey, sourceKey);
        Assert.Equal(3, count);

        // Verify destination contains sorted values
        ValkeyValue[] result = await client.ListRangeAsync(destKey);
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);

        // Test with descending order
        string destKey2 = "{prefix}-" + Guid.NewGuid().ToString();
        count = await client.SortAndStoreAsync(destKey2, sourceKey, order: Order.Descending);
        Assert.Equal(3, count);
        result = await client.ListRangeAsync(destKey2);
        Assert.Equal(["3", "2", "1"], [.. result.Select(v => v.ToString())]);

        // Test with limit
        string destKey3 = "{prefix}-" + Guid.NewGuid().ToString();
        count = await client.SortAndStoreAsync(destKey3, sourceKey, skip: 1, take: 1);
        Assert.Equal(1, count);
        result = await client.ListRangeAsync(destKey3);
        _ = Assert.Single(result);
        Assert.Equal("2", result[0].ToString());

        // Test alphabetic sort
        string alphaKey = "{prefix}-" + Guid.NewGuid().ToString();
        string alphaDestKey = "{prefix}-" + Guid.NewGuid().ToString();
        _ = await client.ListLeftPushAsync(alphaKey, ["b", "a", "c"]);
        count = await client.SortAndStoreAsync(alphaDestKey, alphaKey, sortType: SortType.Alphabetic);
        Assert.Equal(3, count);
        result = await client.ListRangeAsync(alphaDestKey);
        Assert.Equal(["a", "b", "c"], [.. result.Select(v => v.ToString())]);

        // Test overwriting existing destination
        await client.SetAsync(destKey, "existing_value");
        count = await client.SortAndStoreAsync(destKey, sourceKey);
        Assert.Equal(3, count);
        // Destination should now be a list, not a string
        Assert.Equal(ValkeyType.List, await client.TypeAsync(destKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestWait(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key to create a write operation
        await client.SetAsync(key, value);

        // Test WAIT with different expected behavior for cluster vs standalone
        long replicaCount = await client.WaitAsync(1, TimeSpan.FromSeconds(2));
        if (client is GlideClusterClient)
        {
            Assert.True(replicaCount >= 1); // Cluster mode
        }
        else
        {
            Assert.True(replicaCount >= 0); // Standalone mode
        }

        // Test WAIT with 0 replicas (should return immediately)
        replicaCount = await client.WaitAsync(0, TimeSpan.FromSeconds(2));
        Assert.True(replicaCount >= 0);

        // Test WAIT with timeout 0 (should return immediately)
        replicaCount = await client.WaitAsync(1, TimeSpan.Zero);
        Assert.True(replicaCount >= 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestWait_NegativeTimeout(BaseClient client)
    {
        // Test negative timeout should throw exception
        var exception = await Assert.ThrowsAsync<Errors.RequestException>(
            () => client.WaitAsync(1, TimeSpan.FromMilliseconds(-1)));
        Assert.Contains("Timeout cannot be negative", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortAndStore_WithPatterns(GlideClient client)
    {
        // Test with BY and GET patterns (only for standalone clients)
        string userKey = "{prefix}-" + Guid.NewGuid().ToString();
        string destKey = "{prefix}-" + Guid.NewGuid().ToString();

        // Set up test data
        _ = await client.HashSetAsync("user:1", [new HashEntry("age", "30"), new HashEntry("name", "Alice")]);
        _ = await client.HashSetAsync("user:2", [new HashEntry("age", "25"), new HashEntry("name", "Bob")]);
        _ = await client.ListLeftPushAsync(userKey, ["2", "1"]);

        // Test with BY pattern
        long count = await client.SortAndStoreAsync(destKey, userKey, by: "user:*->age");
        Assert.Equal(2, count);
        ValkeyValue[] result = await client.ListRangeAsync(destKey);
        Assert.Equal(["2", "1"], [.. result.Select(v => v.ToString())]);

        // Test with GET pattern
        string destKey2 = "{prefix}-" + Guid.NewGuid().ToString();
        count = await client.SortAndStoreAsync(destKey2, userKey, by: "user:*->age", get: ["user:*->name"]);
        Assert.Equal(2, count);
        result = await client.ListRangeAsync(destKey2);
        Assert.Equal(["Bob", "Alice"], [.. result.Select(v => v.ToString())]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortWithOptions(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test with list using SortOptions
        _ = await client.ListLeftPushAsync(key, ["3", "1", "2"]);
        ValkeyValue[] result = await client.SortAsync(key, new SortOptions());
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);

        // Test with descending order
        result = await client.SortAsync(key, new SortOptions { Order = SortOrder.Descending });
        Assert.Equal(["3", "2", "1"], [.. result.Select(v => v.ToString())]);

        // Test with explicit ascending order
        result = await client.SortAsync(key, new SortOptions { Order = SortOrder.Ascending });
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);

        // Test with default order (should behave same as ascending - server default)
        result = await client.SortAsync(key, new SortOptions { Order = SortOrder.Default });
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);

        // Test with limit
        result = await client.SortAsync(key, new SortOptions { Skip = 1, Take = 1 });
        _ = Assert.Single(result);
        Assert.Equal("2", result[0].ToString());

        // Test alphabetic sort
        string alphaKey = Guid.NewGuid().ToString();
        _ = await client.ListLeftPushAsync(alphaKey, ["b", "a", "c"]);
        result = await client.SortAsync(alphaKey, new SortOptions { SortType = SortType.Alphabetic });
        Assert.Equal(["a", "b", "c"], [.. result.Select(v => v.ToString())]);

        // Test with null options (should use defaults)
        result = await client.SortAsync(key, null);
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);

        // Test combined options
        result = await client.SortAsync(key, new SortOptions
        {
            Order = SortOrder.Descending,
            Skip = 1,
            Take = 2
        });
        Assert.Equal(2, result.Length);
        Assert.Equal(["2", "1"], [.. result.Select(v => v.ToString())]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortAndStoreWithOptions(BaseClient client)
    {
        string sourceKey = "{prefix}-" + Guid.NewGuid().ToString();
        string destKey = "{prefix}-" + Guid.NewGuid().ToString();

        // Test basic sort and store with SortOptions
        _ = await client.ListLeftPushAsync(sourceKey, ["3", "1", "2"]);
        long count = await client.SortAndStoreAsync(destKey, sourceKey, new SortOptions());
        Assert.Equal(3, count);

        // Verify destination contains sorted values
        ValkeyValue[] result = await client.ListRangeAsync(destKey);
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);

        // Test with descending order
        string destKey2 = "{prefix}-" + Guid.NewGuid().ToString();
        count = await client.SortAndStoreAsync(destKey2, sourceKey, new SortOptions { Order = SortOrder.Descending });
        Assert.Equal(3, count);
        result = await client.ListRangeAsync(destKey2);
        Assert.Equal(["3", "2", "1"], [.. result.Select(v => v.ToString())]);

        // Test with limit
        string destKey3 = "{prefix}-" + Guid.NewGuid().ToString();
        count = await client.SortAndStoreAsync(destKey3, sourceKey, new SortOptions { Skip = 1, Take = 1 });
        Assert.Equal(1, count);
        result = await client.ListRangeAsync(destKey3);
        _ = Assert.Single(result);
        Assert.Equal("2", result[0].ToString());

        // Test alphabetic sort
        string alphaKey = "{prefix}-" + Guid.NewGuid().ToString();
        string alphaDestKey = "{prefix}-" + Guid.NewGuid().ToString();
        _ = await client.ListLeftPushAsync(alphaKey, ["b", "a", "c"]);
        count = await client.SortAndStoreAsync(alphaDestKey, alphaKey, new SortOptions { SortType = SortType.Alphabetic });
        Assert.Equal(3, count);
        result = await client.ListRangeAsync(alphaDestKey);
        Assert.Equal(["a", "b", "c"], [.. result.Select(v => v.ToString())]);

        // Test with null options (should use defaults)
        string destKey4 = "{prefix}-" + Guid.NewGuid().ToString();
        count = await client.SortAndStoreAsync(destKey4, sourceKey, null);
        Assert.Equal(3, count);
        result = await client.ListRangeAsync(destKey4);
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortWithOptions_WithPatterns(GlideClient client)
    {
        // Test with BY and GET patterns (only for standalone clients)
        string userKey = Guid.NewGuid().ToString();

        // Set up test data
        _ = await client.HashSetAsync("user:1", [new HashEntry("age", "30"), new HashEntry("name", "Alice")]);
        _ = await client.HashSetAsync("user:2", [new HashEntry("age", "25"), new HashEntry("name", "Bob")]);
        _ = await client.ListLeftPushAsync(userKey, ["2", "1"]);

        // Test with BY pattern using SortOptions
        ValkeyValue[] result = await client.SortAsync(userKey, new SortOptions { By = "user:*->age" });
        Assert.Equal(["2", "1"], [.. result.Select(v => v.ToString())]);

        // Test with GET pattern using SortOptions
        result = await client.SortAsync(userKey, new SortOptions
        {
            By = "user:*->age",
            Get = ["user:*->name"]
        });
        Assert.Equal(["Bob", "Alice"], [.. result.Select(v => v.ToString())]);
    }
}
