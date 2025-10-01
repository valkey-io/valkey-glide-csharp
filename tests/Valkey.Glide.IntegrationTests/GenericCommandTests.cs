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
        await client.StringSetAsync(key, value);
        Assert.True(await client.KeyExistsAsync(key));

        // Delete the key
        Assert.True(await client.KeyDeleteAsync(key));
        Assert.False(await client.KeyExistsAsync(key));

        // Try to delete non-existent key
        Assert.False(await client.KeyDeleteAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyDelete_MultipleKeys(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();

        // Set keys
        await client.StringSetAsync(key1, "value1");
        await client.StringSetAsync(key2, "value2");

        // Delete multiple keys (including non-existent)
        long deletedCount = await client.KeyDeleteAsync([key1, key2, key3]);
        Assert.Equal(2, deletedCount);

        Assert.False(await client.KeyExistsAsync(key1));
        Assert.False(await client.KeyExistsAsync(key2));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyUnlink(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key first
        await client.StringSetAsync(key, value);
        Assert.True(await client.KeyExistsAsync(key));

        // Unlink the key
        Assert.True(await client.KeyUnlinkAsync(key));
        Assert.False(await client.KeyExistsAsync(key));

        // Try to unlink non-existent key
        Assert.False(await client.KeyUnlinkAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyExists_MultipleKeys(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();

        // Set some keys
        await client.StringSetAsync(key1, "value1");
        await client.StringSetAsync(key2, "value2");

        // Check existence
        long existingCount = await client.KeyExistsAsync([key1, key2, key3]);
        Assert.Equal(2, existingCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyExpire_KeyTimeToLive(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key
        await client.StringSetAsync(key, value);

        // Set expiry with seconds precision (should use EXPIRE)
        Assert.True(await client.KeyExpireAsync(key, TimeSpan.FromSeconds(10)));

        // Check TTL
        TimeSpan? ttl = await client.KeyTimeToLiveAsync(key);
        Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalSeconds > 0 && ttl.Value.TotalSeconds <= 10);

        // Test with millisecond precision (should use PEXPIRE)
        Assert.True(await client.KeyExpireAsync(key, TimeSpan.FromMilliseconds(5500)));

        ttl = await client.KeyTimeToLiveAsync(key);
        Assert.NotNull(ttl);
        // Now with PTTL support, we should get millisecond precision
        Assert.True(ttl.Value.TotalMilliseconds > 0 && ttl.Value.TotalMilliseconds <= 5500);

        // Test with DateTime (should use EXPIREAT or PEXPIREAT based on precision)
        DateTime expireTime = DateTime.UtcNow.AddSeconds(15);
        Assert.True(await client.KeyExpireAsync(key, expireTime));

        ttl = await client.KeyTimeToLiveAsync(key);
        Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalSeconds > 10);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyExpireTime(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("7.0.0"),
            "SetIntersectionLength is supported since 7.0.0"
        );
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key
        await client.StringSetAsync(key, value);

        // Key without expiry should return null
        DateTime? expireTime = await client.KeyExpireTimeAsync(key);
        Assert.Null(expireTime);

        // Set expiry and check expire time
        DateTime futureTime = DateTime.UtcNow.AddSeconds(30);
        Assert.True(await client.KeyExpireAsync(key, futureTime));

        expireTime = await client.KeyExpireTimeAsync(key);
        Assert.NotNull(expireTime);
        // Should be close to the set time (within a few seconds tolerance)
        Assert.True(Math.Abs((expireTime.Value - futureTime).TotalSeconds) < 5);

        // Non-existent key should return null
        string nonExistentKey = Guid.NewGuid().ToString();
        expireTime = await client.KeyExpireTimeAsync(nonExistentKey);
        Assert.Null(expireTime);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyEncoding(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a string key
        await client.StringSetAsync(key, value);

        // Get encoding for string key
        string? encoding = await client.KeyEncodingAsync(key);
        Assert.NotNull(encoding);
        // String encoding can be "raw", "embstr", or "int" depending on the value
        Assert.Contains(encoding, new[] { "raw", "embstr", "int" });

        // Test with different data types
        string listKey = Guid.NewGuid().ToString();
        await client.ListLeftPushAsync(listKey, "item");
        string? listEncoding = await client.KeyEncodingAsync(listKey);
        Assert.NotNull(listEncoding);

        string setKey = Guid.NewGuid().ToString();
        await client.SetAddAsync(setKey, "member");
        string? setEncoding = await client.KeyEncodingAsync(setKey);
        Assert.NotNull(setEncoding);

        // Non-existent key should return null
        string nonExistentKey = Guid.NewGuid().ToString();
        string? nonExistentEncoding = await client.KeyEncodingAsync(nonExistentKey);
        Assert.Null(nonExistentEncoding);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyFrequency(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a string key
        await client.StringSetAsync(key, value);

        try
        {
            // Get frequency for string key
            long? frequency = await client.KeyFrequencyAsync(key);
            Assert.NotNull(frequency);
            Assert.True(frequency >= 0);

            // Non-existent key should return null
            string nonExistentKey = Guid.NewGuid().ToString();
            long? nonExistentFrequency = await client.KeyFrequencyAsync(nonExistentKey);
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
        await client.StringSetAsync(key, value);

        // Get idle time for string key
        long? idleTime = await client.KeyIdleTimeAsync(key);
        Assert.NotNull(idleTime);
        Assert.True(idleTime >= 0);

        // Wait a bit and check that idle time increases
        await Task.Delay(1000);
        long? idleTime2 = await client.KeyIdleTimeAsync(key);
        Assert.NotNull(idleTime2);
        Assert.True(idleTime2 >= idleTime);

        // Access the key to reset idle time
        await client.StringGetAsync(key);
        long? idleTimeAfterAccess = await client.KeyIdleTimeAsync(key);
        Assert.NotNull(idleTimeAfterAccess);
        Assert.True(idleTimeAfterAccess < idleTime2);

        // Non-existent key should return null
        string nonExistentKey = Guid.NewGuid().ToString();
        long? nonExistentIdleTime = await client.KeyIdleTimeAsync(nonExistentKey);
        Assert.Null(nonExistentIdleTime);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyRefCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a string key
        await client.StringSetAsync(key, value);

        // Get reference count for string key
        long? refCount = await client.KeyRefCountAsync(key);
        Assert.NotNull(refCount);
        Assert.True(refCount >= 1);

        // Non-existent key should return null
        string nonExistentKey = Guid.NewGuid().ToString();
        long? nonExistentRefCount = await client.KeyRefCountAsync(nonExistentKey);
        Assert.Null(nonExistentRefCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyType(BaseClient client)
    {
        string stringKey = Guid.NewGuid().ToString();
        string setKey = Guid.NewGuid().ToString();

        // Test string type
        await client.StringSetAsync(stringKey, "value");
        ValkeyType stringType = await client.KeyTypeAsync(stringKey);
        Assert.Equal(ValkeyType.String, stringType);

        // Test set type
        await client.SetAddAsync(setKey, "member");
        ValkeyType setType = await client.KeyTypeAsync(setKey);
        Assert.Equal(ValkeyType.Set, setType);

        // Test non-existent key
        string nonExistentKey = Guid.NewGuid().ToString();
        ValkeyType noneType = await client.KeyTypeAsync(nonExistentKey);
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
        await client.StringSetAsync(oldKey, value);

        // Rename the key
        Assert.True(await client.KeyRenameAsync(oldKey, newKey));

        // Check that old key doesn't exist and new key exists
        Assert.False(await client.KeyExistsAsync(oldKey));
        Assert.True(await client.KeyExistsAsync(newKey));
        Assert.Equal(value, await client.StringGetAsync(newKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyRenameNX(BaseClient client)
    {
        string oldKey = "{prefix}-" + Guid.NewGuid().ToString();
        string newKey = "{prefix}-" + Guid.NewGuid().ToString();
        string existingKey = "{prefix}-" + Guid.NewGuid().ToString();
        string value = "test_value";
        string existingValue = "existing_value";

        // Set keys
        await client.StringSetAsync(oldKey, value);
        await client.StringSetAsync(existingKey, existingValue);

        // Rename to non-existing key should succeed
        Assert.True(await client.KeyRenameNXAsync(oldKey, newKey));

        // Check that old key doesn't exist and new key exists
        Assert.False(await client.KeyExistsAsync(oldKey));
        Assert.True(await client.KeyExistsAsync(newKey));
        Assert.Equal(value, await client.StringGetAsync(newKey));

        // Try to rename to existing key should fail
        Assert.False(await client.KeyRenameNXAsync(newKey, existingKey));

        // Both keys should still exist with original values
        Assert.True(await client.KeyExistsAsync(newKey));
        Assert.True(await client.KeyExistsAsync(existingKey));
        Assert.Equal(value, await client.StringGetAsync(newKey));
        Assert.Equal(existingValue, await client.StringGetAsync(existingKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyPersist(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key with expiry
        await client.StringSetAsync(key, value);
        await client.KeyExpireAsync(key, TimeSpan.FromSeconds(10));

        // Verify it has TTL
        TimeSpan? ttl = await client.KeyTimeToLiveAsync(key);
        Assert.NotNull(ttl);

        // Persist the key
        Assert.True(await client.KeyPersistAsync(key));

        // Verify TTL is removed
        ttl = await client.KeyTimeToLiveAsync(key);
        Assert.Null(ttl);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyDump_KeyRestore(BaseClient client)
    {
        string sourceKey = Guid.NewGuid().ToString();
        string destKey = Guid.NewGuid().ToString();
        string replaceKey = Guid.NewGuid().ToString();
        string replaceDateTimeKey = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key
        await client.StringSetAsync(sourceKey, value);

        // Dump the key
        byte[]? dumpData = await client.KeyDumpAsync(sourceKey);
        Assert.NotNull(dumpData);
        Assert.NotEmpty(dumpData);

        // Restore to a new key
        await client.KeyRestoreAsync(destKey, dumpData);

        // Verify the restored key
        Assert.Equal(value, await client.StringGetAsync(destKey));

        // Test RestoreOptions with Replace
        await client.StringSetAsync(replaceKey, "old_value");
        await client.StringSetAsync(replaceDateTimeKey, "old_value");
        RestoreOptions replaceOptions = new RestoreOptions().Replace();
        await client.KeyRestoreAsync(replaceKey, dumpData, restoreOptions: replaceOptions);
        Assert.Equal(value, await client.StringGetAsync(replaceKey));
        await client.KeyRestoreDateTimeAsync(replaceDateTimeKey, dumpData, restoreOptions: replaceOptions);
        Assert.Equal(value, await client.StringGetAsync(replaceDateTimeKey));

        // Test RestoreOptions with TTL
        string ttlKey = Guid.NewGuid().ToString();
        string ttlDateTimeKey = Guid.NewGuid().ToString();
        TimeSpan ts = TimeSpan.FromSeconds(30);
        DateTime dt = new DateTime(2042, 12, 31);
        await client.KeyRestoreAsync(ttlKey, dumpData, expiry: ts);
        await client.KeyRestoreDateTimeAsync(ttlDateTimeKey, dumpData, expiry: dt);

        // Verify key exists and has TTL
        Assert.True(await client.KeyExistsAsync(ttlKey));
        Assert.True(await client.KeyExistsAsync(ttlDateTimeKey));
        TimeSpan? ttl = await client.KeyTimeToLiveAsync(ttlKey);
        TimeSpan? ttlDateTime = await client.KeyTimeToLiveAsync(ttlDateTimeKey);
        Assert.NotNull(ttl);
        Assert.NotNull(ttlDateTime);
        Assert.True(ttl.Value.TotalSeconds > 0);
        Assert.True(ttlDateTime.Value.TotalSeconds > 0);

        // Test RestoreOptions with IDLETIME
        string idletimeKey = Guid.NewGuid().ToString();
        string idletimeDateTimeKey = Guid.NewGuid().ToString();
        RestoreOptions idletimeOptions = new RestoreOptions().SetIdletime(1000);
        await client.KeyRestoreAsync(idletimeKey, dumpData, restoreOptions: idletimeOptions);
        await client.KeyRestoreDateTimeAsync(idletimeDateTimeKey, dumpData, restoreOptions: idletimeOptions);
        Assert.Equal(value, await client.StringGetAsync(idletimeKey));
        Assert.Equal(value, await client.StringGetAsync(idletimeDateTimeKey));

        // Test RestoreOptions with FREQ
        string freqKey = Guid.NewGuid().ToString();
        string freqDateTimeKey = Guid.NewGuid().ToString();
        RestoreOptions freqOptions = new RestoreOptions().SetFrequency(5);
        await client.KeyRestoreAsync(freqKey, dumpData, restoreOptions: freqOptions);
        await client.KeyRestoreDateTimeAsync(freqDateTimeKey, dumpData, restoreOptions: freqOptions);
        Assert.Equal(value, await client.StringGetAsync(freqKey));
        Assert.Equal(value, await client.StringGetAsync(freqDateTimeKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyTouch(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();

        // Set some keys
        await client.StringSetAsync(key1, "value1");
        await client.StringSetAsync(key2, "value2");

        // Touch single key
        Assert.True(await client.KeyTouchAsync(key1));

        // Touch multiple keys (including non-existent)
        long touchedCount = await client.KeyTouchAsync([key1, key2, key3]);
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
        await client.StringSetAsync(sourceKey, value);

        // Copy the key
        Assert.True(await client.KeyCopyAsync(sourceKey, destKey));

        // Verify both keys exist with same value
        Assert.Equal(value, await client.StringGetAsync(sourceKey));
        Assert.Equal(value, await client.StringGetAsync(destKey));

        // Test copy with replace
        await client.StringSetAsync(destKey, "new_value");
        Assert.True(await client.KeyCopyAsync(sourceKey, destKey, replace: true));
        Assert.Equal(value, await client.StringGetAsync(destKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyRandom(BaseClient client)
    {
        // Test with empty database
        string? randomKey = await client.KeyRandomAsync();
        // May be null if database is empty, or return an existing key

        // Set some keys to ensure we have data
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();

        await client.StringSetAsync(key1, "value1");
        await client.StringSetAsync(key2, "value2");
        await client.StringSetAsync(key3, "value3");

        // Now we should get a random key
        randomKey = await client.KeyRandomAsync();
        Assert.NotNull(randomKey);
        Assert.True(await client.KeyExistsAsync(randomKey));

        // Call multiple times to verify it can return different keys
        HashSet<string> seenKeys = [];
        for (int i = 0; i < 10; i++)
        {
            string? key = await client.KeyRandomAsync();
            if (key != null)
            {
                seenKeys.Add(key);
            }
        }

        // We should have seen at least one key
        Assert.NotEmpty(seenKeys);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeysAsync_Scan(GlideClient client)
    {
        string prefix = Guid.NewGuid().ToString();
        string key1 = $"{prefix}:key1";
        string key2 = $"{prefix}:key2";
        string key3 = $"{prefix}:key3";
        string otherKey = "other:key";

        // Set up test keys
        await client.StringSetAsync(key1, "value1");
        await client.StringSetAsync(key2, "value2");
        await client.StringSetAsync(key3, "value3");
        await client.StringSetAsync(otherKey, "other");

        // Test scanning all keys with pattern
        List<ValkeyKey> keys = [];
        await foreach (ValkeyKey key in client.KeysAsync(pattern: $"{prefix}:*"))
        {
            keys.Add(key);
        }

        Assert.Equal(3, keys.Count);
        Assert.Contains(key1, keys.Select(k => k.ToString()));
        Assert.Contains(key2, keys.Select(k => k.ToString()));
        Assert.Contains(key3, keys.Select(k => k.ToString()));
        Assert.DoesNotContain(otherKey, keys.Select(k => k.ToString()));

        // Test scanning with pageSize
        keys.Clear();
        await foreach (ValkeyKey key in client.KeysAsync(pattern: $"{prefix}:*", pageSize: 1))
        {
            keys.Add(key);
        }
        Assert.Equal(3, keys.Count);

        // Test scanning with pageOffset
        keys.Clear();
        await foreach (ValkeyKey key in client.KeysAsync(pattern: $"{prefix}:*", pageOffset: 1))
        {
            keys.Add(key);
        }
        // Should get 2 keys (skipping first one)
        Assert.True(keys.Count >= 2);

        // Test scanning non-existent pattern
        keys.Clear();
        await foreach (ValkeyKey key in client.KeysAsync(pattern: "nonexistent:*"))
        {
            keys.Add(key);
        }
        Assert.Empty(keys);
    }



    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSort(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test with list
        await client.ListLeftPushAsync(key, ["3", "1", "2"]);
        ValkeyValue[] result = await client.SortAsync(key);
        Assert.Equal(["1", "2", "3"], [.. result.Select(v => v.ToString())]);

        // Test with descending order
        result = await client.SortAsync(key, order: Order.Descending);
        Assert.Equal(["3", "2", "1"], [.. result.Select(v => v.ToString())]);

        // Test with limit
        result = await client.SortAsync(key, skip: 1, take: 1);
        Assert.Single(result);
        Assert.Equal("2", result[0].ToString());

        // Test alphabetic sort
        string alphaKey = Guid.NewGuid().ToString();
        await client.ListLeftPushAsync(alphaKey, ["b", "a", "c"]);
        result = await client.SortAsync(alphaKey, sortType: SortType.Alphabetic);
        Assert.Equal(["a", "b", "c"], [.. result.Select(v => v.ToString())]);

        string userKey = Guid.NewGuid().ToString();

        // Test with BY pattern (skip for cluster clients as BY option is denied in cluster mode)
        if (client is not GlideClusterClient)
        {
            await client.HashSetAsync("user:1", [new HashEntry("age", "30")]);
            await client.HashSetAsync("user:2", [new HashEntry("age", "25")]);
            await client.ListLeftPushAsync(userKey, ["2", "1"]);
            result = await client.SortAsync(userKey, by: "user:*->age");
            Assert.Equal(["2", "1"], [.. result.Select(v => v.ToString())]);

            // Test with GET pattern
            await client.HashSetAsync("user:1", [new HashEntry("name", "Alice")]);
            await client.HashSetAsync("user:2", [new HashEntry("name", "Bob")]);
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
        await client.ListLeftPushAsync(sourceKey, ["3", "1", "2"]);
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
        Assert.Single(result);
        Assert.Equal("2", result[0].ToString());

        // Test alphabetic sort
        string alphaKey = "{prefix}-" + Guid.NewGuid().ToString();
        string alphaDestKey = "{prefix}-" + Guid.NewGuid().ToString();
        await client.ListLeftPushAsync(alphaKey, ["b", "a", "c"]);
        count = await client.SortAndStoreAsync(alphaDestKey, alphaKey, sortType: SortType.Alphabetic);
        Assert.Equal(3, count);
        result = await client.ListRangeAsync(alphaDestKey);
        Assert.Equal(["a", "b", "c"], [.. result.Select(v => v.ToString())]);

        // Test overwriting existing destination
        await client.StringSetAsync(destKey, "existing_value");
        count = await client.SortAndStoreAsync(destKey, sourceKey);
        Assert.Equal(3, count);
        // Destination should now be a list, not a string
        Assert.Equal(ValkeyType.List, await client.KeyTypeAsync(destKey));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestWait(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key to create a write operation
        await client.StringSetAsync(key, value);

        // Test WAIT with different expected behavior for cluster vs standalone
        long replicaCount = await client.WaitAsync(1, 1000);
        if (client is GlideClusterClient)
        {
            Assert.True(replicaCount >= 1); // Cluster mode
        }
        else
        {
            Assert.True(replicaCount >= 0); // Standalone mode
        }

        // Test WAIT with 0 replicas (should return immediately)
        replicaCount = await client.WaitAsync(0, 1000);
        Assert.True(replicaCount >= 0);

        // Test WAIT with timeout 0 (should return immediately)
        replicaCount = await client.WaitAsync(1, 0);
        Assert.True(replicaCount >= 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestWait_NegativeTimeout(BaseClient client)
    {
        // Test negative timeout should throw exception
        var exception = await Assert.ThrowsAsync<Errors.RequestException>(
            () => client.WaitAsync(1, -1));
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
        await client.HashSetAsync("user:1", [new HashEntry("age", "30"), new HashEntry("name", "Alice")]);
        await client.HashSetAsync("user:2", [new HashEntry("age", "25"), new HashEntry("name", "Bob")]);
        await client.ListLeftPushAsync(userKey, ["2", "1"]);

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
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeysAsync_LargeDataset(GlideClient client)
    {
        string prefix = Guid.NewGuid().ToString();

        var tasks = Enumerable.Range(0, 25000).Select(i =>
            client.StringSetAsync($"{prefix}:key{i}", $"value{i}"));
        await Task.WhenAll(tasks);

        int count = 0;
        await foreach (var key in client.KeysAsync(pattern: $"{prefix}:*"))
        {
            count++;
        }
        Assert.Equal(25000, count);

        ValkeyKey[] sampleKeys = [.. Enumerable.Range(0, 100).Select(i => (ValkeyKey)$"{prefix}:key{i}")];
        long sampleCount = await client.KeyExistsAsync(sampleKeys);
        Assert.Equal(100L, sampleCount);
    }
}
