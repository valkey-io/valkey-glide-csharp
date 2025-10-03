// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

internal class BatchTestUtils
{
    public static List<TestInfo> CreateStringTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];
        string prefix = "{stringKey}-";
        string atomicPrefix = isAtomic ? prefix : "";
        string key1 = $"{atomicPrefix}1-{Guid.NewGuid()}";
        string key2 = $"{atomicPrefix}2-{Guid.NewGuid()}";
        string nonExistingKey = $"{atomicPrefix}nonexisting-{Guid.NewGuid()}";

        string value1 = $"value-1-{Guid.NewGuid()}";
        string value2 = "test-value";

        // Use IBatch interface directly - no casting needed
        _ = batch.StringSet(key1, value1);
        testData.Add(new(true, "StringSet(key1, value1)"));
        _ = batch.StringSet(key2, value2);
        testData.Add(new(true, "StringSet(key2, value2)"));
        _ = batch.StringGet(key1);
        testData.Add(new(new ValkeyValue(value1), "StringGet(key1)"));
        _ = batch.StringGet(key2);
        testData.Add(new(new ValkeyValue(value2), "StringGet(key2)"));
        _ = batch.StringLength(key1);
        testData.Add(new((long)value1.Length, "StringLength(key1)"));
        _ = batch.StringLength(key2);
        testData.Add(new((long)value2.Length, "StringLength(key2)"));
        _ = batch.StringLength(nonExistingKey);
        testData.Add(new(0L, "StringLength(nonExistingKey)"));

        // StringAppend tests
        string appendValue = "-appended";
        _ = batch.StringAppend(key1, appendValue);
        testData.Add(new((long)(value1.Length + appendValue.Length), "StringAppend(key1, appendValue)"));

        _ = batch.StringGet(key1);
        testData.Add(new(new ValkeyValue(value1 + appendValue), "StringGet(key1) after append"));

        // Append to non-existing key (should create it)
        _ = batch.StringAppend(nonExistingKey, "new-value");
        testData.Add(new(9L, "StringAppend(nonExistingKey, new-value)"));

        _ = batch.StringGet(nonExistingKey);
        testData.Add(new(new ValkeyValue("new-value"), "StringGet(nonExistingKey) after append"));

        // Append empty string
        _ = batch.StringAppend(key2, "");
        testData.Add(new((long)value2.Length, "StringAppend(key2, empty-string)"));

        _ = batch.StringGet(key2);
        testData.Add(new(new ValkeyValue(value2), "StringGet(key2) after append empty string"));

        // Increment/Decrement tests
        string numKey1 = $"{atomicPrefix}num1-{Guid.NewGuid()}";
        string numKey2 = $"{atomicPrefix}num2-{Guid.NewGuid()}";
        string numKey3 = $"{atomicPrefix}num3-{Guid.NewGuid()}";
        string numKey4 = $"{atomicPrefix}num4-{Guid.NewGuid()}";
        string floatKey1 = $"{atomicPrefix}float1-{Guid.NewGuid()}";
        string floatKey2 = $"{atomicPrefix}float2-{Guid.NewGuid()}";

        // Set initial values
        _ = batch.StringSet(numKey1, "10");
        testData.Add(new(true, "StringSet(numKey1, 10)"));

        _ = batch.StringSet(numKey2, "20");
        testData.Add(new(true, "StringSet(numKey2, 20)"));

        _ = batch.StringSet(floatKey1, "10.5");
        testData.Add(new(true, "StringSet(floatKey1, 10.5)"));

        // Test StringIncrement (by 1)
        _ = batch.StringIncrement(numKey1);
        testData.Add(new(11L, "StringIncrement(numKey1)"));

        _ = batch.StringGet(numKey1);
        testData.Add(new(new ValkeyValue("11"), "StringGet(numKey1) after increment"));

        // Test StringIncrement with amount
        _ = batch.StringIncrement(numKey2, 5);
        testData.Add(new(25L, "StringIncrement(numKey2, 5)"));

        _ = batch.StringGet(numKey2);
        testData.Add(new(new ValkeyValue("25"), "StringGet(numKey2) after increment by 5"));

        // Test StringIncrement with negative amount
        _ = batch.StringIncrement(numKey2, -3);
        testData.Add(new(22L, "StringIncrement(numKey2, -3)"));

        _ = batch.StringGet(numKey2);
        testData.Add(new(new ValkeyValue("22"), "StringGet(numKey2) after increment by -3"));

        // Test StringIncrement on non-existent key
        _ = batch.StringIncrement(numKey3);
        testData.Add(new(1L, "StringIncrement(numKey3) non-existent key"));

        _ = batch.StringGet(numKey3);
        testData.Add(new(new ValkeyValue("1"), "StringGet(numKey3) after increment non-existent key"));

        // Test StringIncrement with float
        _ = batch.StringIncrement(floatKey1, 0.5);
        testData.Add(new(11.0, "StringIncrement(floatKey1, 0.5)"));

        _ = batch.StringGet(floatKey1);
        testData.Add(new(new ValkeyValue("11"), "StringGet(floatKey1) after increment by 0.5"));

        // Test StringIncrement with float on non-existent key
        _ = batch.StringIncrement(floatKey2, 0.5);
        testData.Add(new(0.5, "StringIncrement(floatKey2, 0.5) non-existent key"));

        _ = batch.StringGet(floatKey2);
        testData.Add(new(new ValkeyValue("0.5"), "StringGet(floatKey2) after increment non-existent key"));

        // Test StringDecrement (by 1)
        _ = batch.StringDecrement(numKey1);
        testData.Add(new(10L, "StringDecrement(numKey1)"));

        _ = batch.StringGet(numKey1);
        testData.Add(new(new ValkeyValue("10"), "StringGet(numKey1) after decrement"));

        // Test StringDecrement with amount
        _ = batch.StringDecrement(numKey2, 2);
        testData.Add(new(20L, "StringDecrement(numKey2, 2)"));

        _ = batch.StringGet(numKey2);
        testData.Add(new(new ValkeyValue("20"), "StringGet(numKey2) after decrement by 2"));

        // Test StringDecrement with negative amount
        _ = batch.StringDecrement(numKey2, -5);
        testData.Add(new(25L, "StringDecrement(numKey2, -5)"));

        _ = batch.StringGet(numKey2);
        testData.Add(new(new ValkeyValue("25"), "StringGet(numKey2) after decrement by -5"));

        // Test StringDecrement on non-existent key
        _ = batch.StringDecrement(numKey4);
        testData.Add(new(-1L, "StringDecrement(numKey4) non-existent key"));

        _ = batch.StringGet(numKey4);
        testData.Add(new(new ValkeyValue("-1"), "StringGet(numKey4) after decrement non-existent key"));

        // StringGetRange tests
        string rangeKey = $"{atomicPrefix}range-{Guid.NewGuid()}";
        string rangeValue = "Hello World";
        _ = batch.StringSet(rangeKey, rangeValue);
        testData.Add(new(true, "StringSet(rangeKey, Hello World)"));

        _ = batch.StringGetRange(rangeKey, 0, 4);
        testData.Add(new(new ValkeyValue("Hello"), "StringGetRange(rangeKey, 0, 4)"));

        _ = batch.StringGetRange(rangeKey, 6, -1);
        testData.Add(new(new ValkeyValue("World"), "StringGetRange(rangeKey, 6, -1)"));

        _ = batch.StringGetRange(rangeKey, -5, -1);
        testData.Add(new(new ValkeyValue("World"), "StringGetRange(rangeKey, -5, -1)"));

        string nonExistingKey2 = $"{atomicPrefix}nonexisting2-{Guid.NewGuid()}";
        _ = batch.StringGetRange(nonExistingKey2, 0, 5);
        testData.Add(new(new ValkeyValue(""), "StringGetRange(nonExistingKey2, 0, 5)"));

        // StringSetRange tests
        string setRangeKey = $"{atomicPrefix}setrange-{Guid.NewGuid()}";
        _ = batch.StringSet(setRangeKey, "Hello World");
        testData.Add(new(true, "StringSet(setRangeKey, Hello World)"));

        _ = batch.StringSetRange(setRangeKey, 6, "Redis");
        testData.Add(new((ValkeyValue)11L, "StringSetRange(setRangeKey, 6, Redis)"));

        _ = batch.StringGet(setRangeKey);
        testData.Add(new(new ValkeyValue("Hello Redis"), "StringGet(setRangeKey) after setrange"));

        // StringSetRange on non-existent key (should create it with null padding)
        string setRangeKey2 = $"{atomicPrefix}setrange2-{Guid.NewGuid()}";
        _ = batch.StringSetRange(setRangeKey2, 5, "test");
        testData.Add(new((ValkeyValue)9L, "StringSetRange(setRangeKey2, 5, test) non-existent key"));

        _ = batch.StringGet(setRangeKey2);
        testData.Add(new(new ValkeyValue("\0\0\0\0\0test"), "StringGet(setRangeKey2) after setrange non-existent", true));

        // Multiple key StringSet and StringGet tests
        string multiKey1 = $"{atomicPrefix}multi1-{Guid.NewGuid()}";
        string multiKey2 = $"{atomicPrefix}multi2-{Guid.NewGuid()}";
        string multiKey3 = $"{atomicPrefix}multi3-{Guid.NewGuid()}";

        KeyValuePair<ValkeyKey, ValkeyValue>[] multiKeyValues = [
            new(multiKey1, "value1"),
            new(multiKey2, "value2"),
            new(multiKey3, "value3")
        ];

        _ = batch.StringSet(multiKeyValues);
        testData.Add(new(true, "StringSet(multiKeyValues)"));

        string nonExistingKey3 = $"{atomicPrefix}nonexisting3-{Guid.NewGuid()}";
        ValkeyKey[] multiKeys = [multiKey1, multiKey2, multiKey3, nonExistingKey3];
        _ = batch.StringGet(multiKeys);
        testData.Add(new(Array.Empty<ValkeyValue>(), "StringGet(multiKeys)", true));

        // StringGetDelete tests
        string getDelKey = $"{atomicPrefix}getdel-{Guid.NewGuid()}";
        _ = batch.StringSet(getDelKey, "delete-me");
        testData.Add(new(true, "StringSet(getDelKey, delete-me)"));

        _ = batch.StringGetDelete(getDelKey);
        testData.Add(new(new ValkeyValue("delete-me"), "StringGetDelete(getDelKey)"));

        _ = batch.StringGet(getDelKey);
        testData.Add(new(ValkeyValue.Null, "StringGet(getDelKey) after delete"));

        string nonExistingKey4 = $"{atomicPrefix}nonexisting4-{Guid.NewGuid()}";
        _ = batch.StringGetDelete(nonExistingKey4);
        testData.Add(new(ValkeyValue.Null, "StringGetDelete(nonExistingKey4)"));

        // StringGetSetExpiry tests (TimeSpan)
        string getSetExpiryKey1 = $"{atomicPrefix}getsetexpiry1-{Guid.NewGuid()}";
        _ = batch.StringSet(getSetExpiryKey1, "expire-me");
        testData.Add(new(true, "StringSet(getSetExpiryKey1, expire-me)"));

        _ = batch.StringGetSetExpiry(getSetExpiryKey1, TimeSpan.FromSeconds(60));
        testData.Add(new(new ValkeyValue("expire-me"), "StringGetSetExpiry(getSetExpiryKey1, 60s)"));

        _ = batch.StringGetSetExpiry(getSetExpiryKey1, null);
        testData.Add(new(new ValkeyValue("expire-me"), "StringGetSetExpiry(getSetExpiryKey1, null) - remove expiry"));

        string nonExistingKey5 = $"{atomicPrefix}nonexisting5-{Guid.NewGuid()}";
        _ = batch.StringGetSetExpiry(nonExistingKey5, TimeSpan.FromSeconds(30));
        testData.Add(new(ValkeyValue.Null, "StringGetSetExpiry(nonExistingKey5, 30s)"));

        // StringGetSetExpiry tests (DateTime)
        string getSetExpiryKey2 = $"{atomicPrefix}getsetexpiry2-{Guid.NewGuid()}";
        _ = batch.StringSet(getSetExpiryKey2, "expire-me-abs");
        testData.Add(new(true, "StringSet(getSetExpiryKey2, expire-me-abs)"));

        DateTime futureTime = DateTime.UtcNow.AddMinutes(5);
        _ = batch.StringGetSetExpiry(getSetExpiryKey2, futureTime);
        testData.Add(new(new ValkeyValue("expire-me-abs"), "StringGetSetExpiry(getSetExpiryKey2, futureTime)"));

        if (TestConfiguration.SERVER_VERSION >= new Version("7.0.0"))
        {
            string lcsKey1 = $"{prefix}lcs1-{Guid.NewGuid()}";
            string lcsKey2 = $"{prefix}lcs2-{Guid.NewGuid()}";

            _ = batch.StringSet(lcsKey1, "abcdef");
            testData.Add(new(true, "StringSet(lcsKey1, abcdef)"));

            _ = batch.StringSet(lcsKey2, "acef");
            testData.Add(new(true, "StringSet(lcsKey2, acef)"));

            _ = batch.StringLongestCommonSubsequence(lcsKey1, lcsKey2);
            testData.Add(new("acef", "StringLongestCommonSubsequence(lcsKey1, lcsKey2)"));

            _ = batch.StringLongestCommonSubsequenceLength(lcsKey1, lcsKey2);
            testData.Add(new(4L, "StringLongestCommonSubsequenceLength(lcsKey1, lcsKey2)"));

            _ = batch.StringLongestCommonSubsequenceWithMatches(lcsKey1, lcsKey2);
            testData.Add(new(LCSMatchResult.Null, "StringLongestCommonSubsequenceWithMatches(lcsKey1, lcsKey2)", true));

            _ = batch.StringLongestCommonSubsequenceWithMatches(lcsKey1, lcsKey2, 2);
            testData.Add(new(LCSMatchResult.Null, "StringLongestCommonSubsequenceWithMatches(lcsKey1, lcsKey2, minLength=2)", true));

            // Test LCS with non-existent keys
            string nonExistingKey6 = $"{prefix}nonexisting6-{Guid.NewGuid()}";
            _ = batch.StringLongestCommonSubsequence(lcsKey1, nonExistingKey6);
            testData.Add(new("", "StringLongestCommonSubsequence(lcsKey1, nonExistingKey6)"));

            string nonExistingKey7 = $"{prefix}nonexisting7-{Guid.NewGuid()}";
            _ = batch.StringLongestCommonSubsequenceLength(nonExistingKey7, lcsKey2);
            testData.Add(new(0L, "StringLongestCommonSubsequenceLength(nonExistingKey7, lcsKey2)"));
        }

        return testData;
    }

    public static List<TestInfo> CreateSetTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];
        string prefix = "{setKey}-";
        string atomicPrefix = isAtomic ? prefix : "";
        string key1 = $"{atomicPrefix}1-{Guid.NewGuid()}";
        string key2 = $"{atomicPrefix}2-{Guid.NewGuid()}";
        string key3 = $"{atomicPrefix}3-{Guid.NewGuid()}";
        string key4 = $"{atomicPrefix}4-{Guid.NewGuid()}";
        string key5 = $"{atomicPrefix}5-{Guid.NewGuid()}";
        string destKey = $"{atomicPrefix}dest-{Guid.NewGuid()}";

        _ = batch.SetAdd(key1, "a");
        testData.Add(new(true, "SetAdd(key1, a)"));

        _ = batch.SetAdd(key1, ["b", "c"]);
        testData.Add(new(2L, "SetAdd(key1, [b, c])"));

        _ = batch.SetLength(key1);
        testData.Add(new(3L, "SetLength(key1)"));

        _ = batch.SetMembers(key1);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetMembers(key1)", true));

        _ = batch.SetRemove(key1, "a");
        testData.Add(new(true, "SetRemove(key1, a)"));

        _ = batch.SetRemove(key1, "x");
        testData.Add(new(false, "SetRemove(key1, x)"));

        _ = batch.SetAdd(key1, ["d", "e"]);
        testData.Add(new(2L, "SetAdd(key1, [d, e])"));

        _ = batch.SetRemove(key1, ["b", "d", "nonexistent"]);
        testData.Add(new(2L, "SetRemove(key1, [b, d, nonexistent])"));

        _ = batch.SetLength(key1);
        testData.Add(new(2L, "SetLength(key1) after multiple remove"));

        _ = batch.SetAdd(key2, "c");
        testData.Add(new(true, "SetAdd(key2, c)"));

        _ = batch.SetAdd(key3, "z");
        testData.Add(new(true, "SetAdd(key3, z)"));

        _ = batch.SetAdd(key4, ["x", "y"]);
        testData.Add(new(2L, "SetAdd(key4, [x, y])"));

        _ = batch.SetAdd(key5, ["c", "f"]);
        testData.Add(new(2L, "SetAdd(key5, [c, f])"));

        // Add data to prefixed keys for multi-key operations
        _ = batch.SetAdd(prefix + key1, ["c", "e"]);
        testData.Add(new(2L, "SetAdd(prefix+key1, [c, e])"));

        _ = batch.SetAdd(prefix + key2, ["c"]);
        testData.Add(new(1L, "SetAdd(prefix+key2, [c])"));

        _ = batch.SetAdd(prefix + key5, ["c", "f"]);
        testData.Add(new(2L, "SetAdd(prefix+key5, [c, f])"));

        // Multi-key operations: always use prefix to ensure same hash slot
        _ = batch.SetUnion(prefix + key1, prefix + key2);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetUnion(prefix+key1, prefix+key2)", true));

        _ = batch.SetUnion([prefix + key1, prefix + key2, prefix + key5]);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetUnion([prefix+key1, prefix+key2, prefix+key5])", true));

        _ = batch.SetIntersect(prefix + key1, prefix + key2);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetIntersect(prefix+key1, prefix+key2)", true));

        _ = batch.SetIntersect([prefix + key1, prefix + key2]);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetIntersect([prefix+key1, prefix+key2])", true));

        _ = batch.SetDifference(prefix + key1, prefix + key2);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetDifference(prefix+key1, prefix+key2)", true));

        _ = batch.SetDifference([prefix + key1, prefix + key2]);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetDifference([prefix+key1, prefix+key2])", true));

        if (TestConfiguration.SERVER_VERSION >= new Version("7.0.0"))
        {
            _ = batch.SetIntersectionLength([prefix + key1, prefix + key2]);
            testData.Add(new(1L, "SetIntersectionLength([prefix+key1, prefix+key2])"));

            _ = batch.SetIntersectionLength([prefix + key1, prefix + key2], 5);
            testData.Add(new(1L, "SetIntersectionLength([prefix+key1, prefix+key2], limit=5)"));

            _ = batch.SetIntersectionLength([prefix + key1, prefix + key5], 1);
            testData.Add(new(1L, "SetIntersectionLength([prefix+key1, prefix+key5], limit=1)"));
        }

        _ = batch.SetUnionStore(prefix + destKey, prefix + key1, prefix + key2);
        testData.Add(new(2L, "SetUnionStore(prefix+destKey, prefix+key1, prefix+key2)"));

        _ = batch.SetUnionStore(prefix + destKey, [prefix + key1, prefix + key2]);
        testData.Add(new(2L, "SetUnionStore(prefix+destKey, [prefix+key1, prefix+key2])"));

        _ = batch.SetIntersectStore(prefix + destKey, prefix + key1, prefix + key2);
        testData.Add(new(1L, "SetIntersectStore(prefix+destKey, prefix+key1, prefix+key2)"));

        _ = batch.SetIntersectStore(prefix + destKey, [prefix + key1, prefix + key2]);
        testData.Add(new(1L, "SetIntersectStore(prefix+destKey, [prefix+key1, prefix+key2])"));

        _ = batch.SetDifferenceStore(prefix + destKey, prefix + key1, prefix + key2);
        testData.Add(new(1L, "SetDifferenceStore(prefix+destKey, prefix+key1, prefix+key2)"));

        _ = batch.SetDifferenceStore(prefix + destKey, [prefix + key1, prefix + key2]);
        testData.Add(new(1L, "SetDifferenceStore(prefix+destKey, [prefix+key1, prefix+key2])"));

        _ = batch.SetPop(key3);
        testData.Add(new(new gs("z"), "SetPop(key3)"));

        _ = batch.SetLength(key3);
        testData.Add(new(0L, "SetLength(key3) after pop"));

        _ = batch.SetPop(key4, 1);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetPop(key4, 1)", true));

        _ = batch.SetLength(key4);
        testData.Add(new(1L, "SetLength(key4) after pop with count"));

        _ = batch.SetContains(key1, "c");
        testData.Add(new(true, "SetContains(key1, c)"));

        _ = batch.SetContains(key1, "nonexistent");
        testData.Add(new(false, "SetContains(key1, nonexistent)"));

        _ = batch.SetContains(key1, ["c", "e", "nonexistent"]);
        testData.Add(new(new bool[] { true, true, false }, "SetContains(key1, [c, e, nonexistent])"));

        _ = batch.SetRandomMember(key1);
        testData.Add(new(new ValkeyValue(""), "SetRandomMember(key1)", true)); // Can't predict exact value

        _ = batch.SetRandomMembers(key1, 1);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SetRandomMembers(key1, 1)", true)); // Can't predict exact values

        _ = batch.SetMove(prefix + key1, prefix + key2, "e");
        testData.Add(new(true, "SetMove(prefix+key1, prefix+key2, e)"));

        _ = batch.SetMove(prefix + key1, prefix + key2, "nonexistent");
        testData.Add(new(false, "SetMove(prefix+key1, prefix+key2, nonexistent)"));

        _ = batch.SetScan(key1, 0);
        testData.Add(new((0L, Array.Empty<ValkeyValue>()), "SetScan(key1, 0)", true)); // Test tuple types

        return testData;
    }

    public static List<TestInfo> CreateServerManagementTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];
        string prefix = "{serverKey}-";
        string atomicPrefix = isAtomic ? prefix : "";
        string testKey = $"{atomicPrefix}test-{Guid.NewGuid()}";

        // Set up some test data
        _ = batch.StringSet(testKey, "test-value");
        testData.Add(new(true, "StringSet(testKey, test-value)"));

        // ConfigGet tests
        _ = batch.ConfigGetAsync("*");
        testData.Add(new(Array.Empty<KeyValuePair<string, string>>(), "ConfigGetAsync(*)", true));

        _ = batch.ConfigGetAsync("maxmemory");
        testData.Add(new(Array.Empty<KeyValuePair<string, string>>(), "ConfigGetAsync(maxmemory)", true));

        // ConfigSet and ConfigGet combination
        _ = batch.ConfigSetAsync((ValkeyValue)"maxmemory-policy", (ValkeyValue)"allkeys-lru");
        testData.Add(new(ValkeyValue.Null, "ConfigSetAsync(maxmemory-policy, allkeys-lru)", true));

        _ = batch.ConfigGetAsync("maxmemory-policy");
        testData.Add(new(Array.Empty<KeyValuePair<string, string>>(), "ConfigGetAsync(maxmemory-policy)", true));

        // ConfigResetStatistics
        _ = batch.ConfigResetStatisticsAsync();
        testData.Add(new(ValkeyValue.Null, "ConfigResetStatisticsAsync()", true));

        // DatabaseSize
        _ = batch.DatabaseSizeAsync();
        testData.Add(new(1L, "DatabaseSizeAsync()", true));

        // LastSave
        _ = batch.LastSaveAsync();
        testData.Add(new(DateTime.MinValue, "LastSaveAsync()", true));

        // Time
        _ = batch.TimeAsync();
        testData.Add(new(DateTime.MinValue, "TimeAsync()", true));

        // Lolwut
        _ = batch.LolwutAsync();
        testData.Add(new("", "LolwutAsync()", true));

        return testData;
    }

    public static List<TestInfo> CreateGenericTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];
        string prefix = "{genericKey}-";
        string atomicPrefix = isAtomic ? "{genericKey}-" : "";
        string genericKey1 = $"{atomicPrefix}generic1-{Guid.NewGuid()}";
        string genericKey2 = $"{atomicPrefix}generic2-{Guid.NewGuid()}";
        string genericKey3 = $"{atomicPrefix}generic3-{Guid.NewGuid()}";

        _ = batch.StringSet(genericKey1, "value1");
        testData.Add(new(true, "StringSet(genericKey1, value1)"));

        _ = batch.StringSet(genericKey2, "value2");
        testData.Add(new(true, "StringSet(genericKey2, value2)"));

        _ = batch.KeyExists(genericKey1);
        testData.Add(new(true, "KeyExists(genericKey1)"));

        _ = batch.KeyExists([genericKey1, genericKey2, genericKey3]);
        testData.Add(new(2L, "KeyExists([genericKey1, genericKey2, genericKey3])"));

        _ = batch.KeyType(genericKey1);
        testData.Add(new(ValkeyType.String, "KeyType(genericKey1)"));

        _ = batch.KeyExpire(genericKey1, TimeSpan.FromSeconds(60));
        testData.Add(new(true, "KeyExpire(genericKey1, 60s)"));

        _ = batch.KeyTimeToLive(genericKey1);
        testData.Add(new(TimeSpan.FromSeconds(60), "KeyTimeToLive(genericKey1)", true));

        _ = batch.KeyPersist(genericKey1);
        testData.Add(new(true, "KeyPersist(genericKey1)"));

        _ = batch.KeyTimeToLive(genericKey1);
        testData.Add(new(null, "KeyTimeToLive(genericKey1) after persist"));

        _ = batch.KeyExpire(genericKey1, TimeSpan.FromSeconds(120));
        testData.Add(new(true, "KeyExpire(genericKey1, 120s)"));

        if (TestConfiguration.SERVER_VERSION > new Version("7.0.0")) // KeyExpireTime added in 7.0.0
        {
            _ = batch.KeyExpireTime(genericKey1);
            testData.Add(new(DateTime.UtcNow.AddSeconds(120), "KeyExpireTime(genericKey1)", true));
        }

        _ = batch.KeyEncoding(genericKey1);
        testData.Add(new("embstr", "KeyEncoding(genericKey1)", true));

        // KeyFrequency requires LFU maxmemory policy to be configured
        // Since we can't guarantee this in test environment, we skip this test in batch mode
        // The functionality is tested in integration tests with proper exception handling
        // _ = batch.KeyFrequency(genericKey1);
        // testData.Add(new(1L, "KeyFrequency(genericKey1)", true));

        _ = batch.KeyIdleTime(genericKey1);
        testData.Add(new(0L, "KeyIdleTime(genericKey1)", true));

        _ = batch.KeyRefCount(genericKey1);
        testData.Add(new(1L, "KeyRefCount(genericKey1)", true));

        _ = batch.KeyRandom();
        testData.Add(new(genericKey1, "KeyRandom()", true));

        _ = batch.KeyTouch(genericKey1);
        testData.Add(new(true, "KeyTouch(genericKey1)"));

        _ = batch.KeyTouch([genericKey1, genericKey2, genericKey3]);
        testData.Add(new(2L, "KeyTouch([genericKey1, genericKey2, genericKey3])"));

        _ = batch.StringSet(prefix + genericKey2, "value2");
        testData.Add(new(true, "StringSet(prefix + genericKey2, value2)"));

        string renamedKey = $"{prefix}renamed-{Guid.NewGuid()}";
        _ = batch.KeyRename(prefix + genericKey2, renamedKey);
        testData.Add(new(true, "KeyRename(prefix + genericKey2, renamedKey)"));

        _ = batch.KeyExists(prefix + genericKey2);
        testData.Add(new(false, "KeyExists(prefix + genericKey2) after rename"));

        _ = batch.KeyExists(renamedKey);
        testData.Add(new(true, "KeyExists(renamedKey) after rename"));

        string renameNXKey = $"{prefix}renamenx-{Guid.NewGuid()}";
        _ = batch.KeyRenameNX(renamedKey, renameNXKey);
        testData.Add(new(true, "KeyRenameNX(renamedKey, renameNXKey)"));

        _ = batch.KeyExists(renamedKey);
        testData.Add(new(false, "KeyExists(renamedKey) after renamenx"));

        _ = batch.KeyExists(renameNXKey);
        testData.Add(new(true, "KeyExists(renameNXKey) after renamenx"));

        _ = batch.StringSet(prefix + genericKey1, "value1");
        testData.Add(new(true, "StringSet(prefix + genericKey1, value1)"));

        string copiedKey = $"{prefix}copied-{Guid.NewGuid()}";
        _ = batch.KeyCopy(prefix + genericKey1, copiedKey);
        testData.Add(new(true, "KeyCopy(genericKey1, copiedKey)"));

        _ = batch.KeyExists(copiedKey);
        testData.Add(new(true, "KeyExists(copiedKey) after copy"));

        _ = batch.KeyDelete(copiedKey);
        testData.Add(new(true, "KeyDelete(copiedKey)"));

        _ = batch.KeyUnlink([genericKey1, renamedKey, genericKey3]);
        testData.Add(new(1L, "KeyUnlink([genericKey1, renamedKey, genericKey3])"));

        // WAIT command tests
        _ = batch.StringSet(prefix + "waitkey", "value");
        testData.Add(new(true, "StringSet(prefix + waitkey, value)"));

        _ = batch.Wait(0, 1000);
        testData.Add(new(0L, "Wait(0, 1000)", true));

        _ = batch.Wait(1, 0);
        testData.Add(new(0L, "Wait(1, 0)", true));

        return testData;
    }

    public static List<TestInfo> CreateSortedSetTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];
        string prefix = "{sortedSetKey}-";
        string atomicPrefix = isAtomic ? prefix : "";
        string key1 = $"{atomicPrefix}1-{Guid.NewGuid()}";
        string key2 = $"{atomicPrefix}2-{Guid.NewGuid()}";

        // Test single member add
        _ = batch.SortedSetAdd(key1, "member1", 10.5);
        testData.Add(new(true, "SortedSetAdd(key1, member1, 10.5)"));

        // Test multiple members add
        SortedSetEntry[] entries =
        [
            new("member2", 8.2),
            new("member3", 15.0)
        ];
        _ = batch.SortedSetAdd(key1, entries);
        testData.Add(new(2L, "SortedSetAdd(key1, [member2:8.2, member3:15.0])"));

        // Test add with NX (should not add existing member)
        _ = batch.SortedSetAdd(key1, "member1", 20.0, SortedSetWhen.NotExists);
        testData.Add(new(false, "SortedSetAdd(key1, member1, 20.0, NotExists)"));

        // Test add with XX (should update existing member)
        _ = batch.SortedSetAdd(key1, "member2", 12.0, SortedSetWhen.Exists);
        testData.Add(new(false, "SortedSetAdd(key1, member2, 12.0, Exists)"));

        // Test add new member with NX
        _ = batch.SortedSetAdd(key2, "newMember", 7.5, SortedSetWhen.NotExists);
        testData.Add(new(true, "SortedSetAdd(key2, newMember, 7.5, NotExists)"));

        // Test single member remove
        _ = batch.SortedSetRemove(key1, "member1");
        testData.Add(new(true, "SortedSetRemove(key1, member1)"));

        // Test multiple member remove
        _ = batch.SortedSetRemove(key1, ["member2", "member3"]);
        testData.Add(new(2L, "SortedSetRemove(key1, [member2, member3])"));

        // Test remove non-existent member
        _ = batch.SortedSetRemove(key1, "nonexistent");
        testData.Add(new(false, "SortedSetRemove(key1, nonexistent)"));

        // Add some test data for length, count, and range operations
        _ = batch.SortedSetAdd(key1, "testMember1", 1.0);
        testData.Add(new(true, "SortedSetAdd(key1, testMember1, 1.0)"));

        _ = batch.SortedSetAdd(key1, "testMember2", 2.0);
        testData.Add(new(true, "SortedSetAdd(key1, testMember2, 2.0)"));

        _ = batch.SortedSetAdd(key1, "testMember3", 3.0);
        testData.Add(new(true, "SortedSetAdd(key1, testMember3, 3.0)"));

        // Test SortedSetLength (uses ZCARD)
        _ = batch.SortedSetLength(key1);
        testData.Add(new(3L, "SortedSetLength(key1)"));

        _ = batch.SortedSetLength(key2);
        testData.Add(new(1L, "SortedSetLength(key2)"));

        // Test SortedSetLength with range (uses ZCOUNT)
        _ = batch.SortedSetLength(key1, 1.5, 2.5);
        testData.Add(new(1L, "SortedSetLength(key1, 1.5, 2.5)"));

        // Test SortedSetCard (ZCARD)
        _ = batch.SortedSetCard(key1);
        testData.Add(new(3L, "SortedSetCard(key1)"));

        _ = batch.SortedSetCard(key2);
        testData.Add(new(1L, "SortedSetCard(key2)"));

        // Test SortedSetCount
        _ = batch.SortedSetCount(key1);
        testData.Add(new(3L, "SortedSetCount(key1) - all elements"));

        _ = batch.SortedSetCount(key1, 1.5, 2.5);
        testData.Add(new(1L, "SortedSetCount(key1, 1.5, 2.5)"));

        _ = batch.SortedSetCount(key1, 1.0, 3.0, Exclude.Start);
        testData.Add(new(2L, "SortedSetCount(key1, 1.0, 3.0, Exclude.Start)"));

        // Test SortedSetRangeByRank
        // key1 has testMember1 (1.0), testMember2 (2.0), testMember3 (3.0)
        _ = batch.SortedSetRangeByRank(key1);
        testData.Add(new(new ValkeyValue[] { "testMember1", "testMember2", "testMember3" }, "SortedSetRangeByRank(key1) - all elements"));

        _ = batch.SortedSetRangeByRank(key1, 0, 1);
        testData.Add(new(new ValkeyValue[] { "testMember1", "testMember2" }, "SortedSetRangeByRank(key1, 0, 1)"));

        _ = batch.SortedSetRangeByRank(key1, 0, 1, Order.Descending);
        testData.Add(new(new ValkeyValue[] { "testMember3", "testMember2" }, "SortedSetRangeByRank(key1, 0, 1, Descending)"));

        // Test SortedSetRangeByRankWithScores
        _ = batch.SortedSetRangeByRankWithScores(key1);
        testData.Add(new(new SortedSetEntry[]
        {
            new("testMember1", 1.0),
            new("testMember2", 2.0),
            new("testMember3", 3.0)
        }, "SortedSetRangeByRankWithScores(key1) - all elements"));

        _ = batch.SortedSetRangeByRankWithScores(key1, 0, 1);
        testData.Add(new(new SortedSetEntry[]
        {
            new("testMember1", 1.0),
            new("testMember2", 2.0)
        }, "SortedSetRangeByRankWithScores(key1, 0, 1)"));

        // Test SortedSetRangeByScore
        _ = batch.SortedSetRangeByScore(key1, 1.0, 3.0);
        testData.Add(new(new ValkeyValue[] { "testMember1", "testMember2", "testMember3" }, "SortedSetRangeByScore(key1, 1.0, 3.0)"));

        _ = batch.SortedSetRangeByScore(key1, 1.0, 3.0, Exclude.None, Order.Descending);
        testData.Add(new(new ValkeyValue[] { "testMember3", "testMember2", "testMember1" }, "SortedSetRangeByScore(key1, 1.0, 3.0, Descending)"));

        // Test SortedSetRangeByScoreWithScores
        _ = batch.SortedSetRangeByScoreWithScores(key1, 1.0, 3.0);
        testData.Add(new(new SortedSetEntry[]
        {
            new("testMember1", 1.0),
            new("testMember2", 2.0),
            new("testMember3", 3.0)
        }, "SortedSetRangeByScoreWithScores(key1, 1.0, 3.0)"));

        _ = batch.SortedSetRangeByScoreWithScores(key1, 1.0, 3.0, skip: 1, take: 1);
        testData.Add(new(new SortedSetEntry[]
        {
            new("testMember2", 2.0)
        }, "SortedSetRangeByScoreWithScores(key1, 1.0, 3.0, skip: 1, take: 1)"));

        // Add members with same score for lexicographical ordering tests
        _ = batch.SortedSetAdd(key2, "apple", 0.0);
        testData.Add(new(true, "SortedSetAdd(key2, apple, 0.0)"));

        _ = batch.SortedSetAdd(key2, "banana", 0.0);
        testData.Add(new(true, "SortedSetAdd(key2, banana, 0.0)"));

        _ = batch.SortedSetAdd(key2, "cherry", 0.0);
        testData.Add(new(true, "SortedSetAdd(key2, cherry, 0.0)"));

        // Test SortedSetRangeByValue
        // key2 has newMember (7.5), apple (0.0), banana (0.0), cherry (0.0) - lexicographically ordered for same scores
        _ = batch.SortedSetRangeByValue(key2, "a", "c", Exclude.None, 0, -1);
        testData.Add(new(new ValkeyValue[] { "apple", "banana" }, "SortedSetRangeByValue(key2, 'a', 'c', Exclude.None, 0, -1)"));

        _ = batch.SortedSetRangeByValue(key2, "b", "d", Exclude.None, skip: 1, take: 1);
        testData.Add(new(new ValkeyValue[] { "cherry" }, "SortedSetRangeByValue(key2, 'b', 'd', Exclude.None, skip: 1, take: 1)"));

        // Test SortedSetRangeByValue
        _ = batch.SortedSetRangeByValue(key2, order: Order.Descending);
        testData.Add(new(new ValkeyValue[] { "newMember", "cherry", "banana", "apple" }, "SortedSetRangeByValue(key2, order: Descending)"));

        _ = batch.SortedSetRangeByValue(key2, "a", "c", order: Order.Ascending);
        testData.Add(new(new ValkeyValue[] { "apple", "banana" }, "SortedSetRangeByValue(key2, 'a', 'c', order: Ascending)"));

        // Test new sorted set commands
        string key3 = $"{atomicPrefix}3-{Guid.NewGuid()}";
        string destKey = $"{atomicPrefix}dest-{Guid.NewGuid()}";

        // Test SortedSetIncrement
        _ = batch.SortedSetIncrement(key1, "testMember1", 5.0);
        testData.Add(new(6.0, "SortedSetIncrement(key1, testMember1, 5.0)"));

        // Test combine operations - use prefixed keys to ensure same hash slot for cluster mode
        string combineKey1 = $"{prefix}combine1-{Guid.NewGuid()}";
        string combineKey3 = $"{prefix}combine3-{Guid.NewGuid()}";
        string combineDestKey = $"{prefix}combineDest-{Guid.NewGuid()}";

        // Setup data for combine operations
        _ = batch.SortedSetAdd(combineKey1, [
            new SortedSetEntry("testMember1", 6.0), // After increment
            new SortedSetEntry("testMember2", 2.0),
            new SortedSetEntry("testMember3", 3.0)
        ]);
        testData.Add(new(3L, "SortedSetAdd(combineKey1, test data for combine)"));

        _ = batch.SortedSetAdd(combineKey3, [
            new SortedSetEntry("testMember2", 25.0),
            new SortedSetEntry("testMember4", 40.0)
        ]);
        testData.Add(new(2L, "SortedSetAdd(combineKey3, test data for combine)"));

        _ = batch.SortedSetCombine(SetOperation.Union, [combineKey1, combineKey3]);
        testData.Add(new(new ValkeyValue[] { "testMember2", "testMember3", "testMember1", "testMember4" }, "SortedSetCombine(Union, [combineKey1, combineKey3])"));

        _ = batch.SortedSetCombineWithScores(SetOperation.Intersect, [combineKey1, combineKey3]);
        testData.Add(new(new SortedSetEntry[] { new("testMember2", 27.0) }, "SortedSetCombineWithScores(Intersect, [combineKey1, combineKey3])"));

        _ = batch.SortedSetCombineAndStore(SetOperation.Union, combineDestKey, combineKey1, combineKey3);
        testData.Add(new(4L, "SortedSetCombineAndStore(Union, combineDestKey, combineKey1, combineKey3)"));

        // Test SortedSetIntersectionLength (ZINTERCARD) - requires Redis 7.0+
        if (TestConfiguration.SERVER_VERSION >= new Version("7.0.0"))
        {
            _ = batch.SortedSetIntersectionLength([combineKey1, combineKey3]);
            testData.Add(new(1L, "SortedSetIntersectionLength([combineKey1, combineKey3])"));
        }

        // Test SortedSetLengthByValue
        _ = batch.SortedSetLengthByValue(key2, "a", "c");
        testData.Add(new(2L, "SortedSetLengthByValue(key2, a, c)"));

        // Test scores retrieval
        // testMember1 was incremented from 1.0 to 6.0, testMember2 is 2.0, nonexistent should be null
        _ = batch.SortedSetScores(key1, ["testMember1", "testMember2", "nonexistent"]);
        testData.Add(new(new double?[] { 6.0, 2.0, null }, "SortedSetScores(key1, [testMember1, testMember2, nonexistent])"));

        // Test pop operations
        // Test pop operations - add data first to prevent blocking
        string popKey = $"{atomicPrefix}pop-{Guid.NewGuid()}";
        _ = batch.SortedSetAdd(popKey, [
            new SortedSetEntry("member1", 1.0),
            new SortedSetEntry("member2", 2.0)
        ]);
        testData.Add(new(2L, "SortedSetAdd(popKey, test data for pop)"));

        // Test SortedSetPop (ZMPOP) - requires Redis 7.0+
        if (TestConfiguration.SERVER_VERSION >= new Version("7.0.0"))
        {
            _ = batch.SortedSetPop([popKey], 1);
            testData.Add(new(new SortedSetPopResult(popKey, [
                new SortedSetEntry("member1", 1.0)
            ]), "SortedSetPop([popKey], 1)"));

            // Test blocking commands with data present to prevent actual blocking
            string blockingKey = $"{atomicPrefix}blocking-{Guid.NewGuid()}";
            _ = batch.SortedSetAdd(blockingKey, [
                new SortedSetEntry("block1", 10.0),
                new SortedSetEntry("block2", 20.0)
            ]);
            testData.Add(new(2L, "SortedSetAdd(blockingKey, test data for blocking)"));

            // Test SortedSetBlockingPop (single key, single element)
            _ = batch.SortedSetBlockingPop(blockingKey, Order.Ascending, 0.1);
            testData.Add(new(new SortedSetEntry("block1", 10.0), "SortedSetBlockingPop(blockingKey, Ascending, 0.1s)"));

            // Test SortedSetBlockingPop (single key, multiple elements)
            _ = batch.SortedSetBlockingPop(blockingKey, 1, Order.Descending, 0.1);
            testData.Add(new(new SortedSetEntry[] { new("block2", 20.0) }, "SortedSetBlockingPop(blockingKey, 1, Descending, 0.1s)"));

            // Test SortedSetBlockingPop (multi-key, multiple elements)
            _ = batch.SortedSetBlockingPop([blockingKey], 1, Order.Descending, 0.1);
            testData.Add(new(SortedSetPopResult.Null, "SortedSetBlockingPop([blockingKey], 1, Descending, 0.1s) - should be null"));
        }

        // Test sorted set commands
        string newKey = $"{prefix}new-{Guid.NewGuid()}";
        string newDestKey = $"{prefix}newdest-{Guid.NewGuid()}";

        // Setup data
        _ = batch.SortedSetAdd(newKey, [
            new SortedSetEntry("newMember1", 10.5),
            new SortedSetEntry("newMember2", 8.2),
            new SortedSetEntry("newMember3", 15.0)
        ]);
        testData.Add(new(3L, "SortedSetAdd(newKey, test data for new commands)"));

        // Test SortedSetPop (min - default)
        _ = batch.SortedSetPop(newKey);
        testData.Add(new(new SortedSetEntry("newMember2", 8.2), "SortedSetPop(newKey) - min"));

        // Test SortedSetPop (max)
        _ = batch.SortedSetPop(newKey, Order.Descending);
        testData.Add(new(new SortedSetEntry("newMember3", 15.0), "SortedSetPop(newKey, Order.Descending) - max"));

        // Test SortedSetRandomMember
        _ = batch.SortedSetRandomMember(newKey);
        testData.Add(new(new ValkeyValue("newMember1"), "SortedSetRandomMember(newKey)"));

        // Setup data
        _ = batch.SortedSetAdd(newKey, [
            new SortedSetEntry("newMember1", 10.5),
            new SortedSetEntry("newMember2", 8.2),
            new SortedSetEntry("newMember3", 15.0)
        ]);
        testData.Add(new(2L, "SortedSetAdd(newKey, test data for new commands)"));

        // Test SortedSetRank
        _ = batch.SortedSetRank(newKey, "newMember1");
        testData.Add(new(1L, "SortedSetRank(newKey, newMember1)"));

        // Test SortedSetRank with descending order (reverse rank)
        _ = batch.SortedSetRank(newKey, "newMember1", Order.Descending);
        testData.Add(new(1L, "SortedSetRank(newKey, newMember1, Order.Descending)"));

        // Test SortedSetScore
        _ = batch.SortedSetScore(newKey, "newMember1");
        testData.Add(new(10.5, "SortedSetScore(newKey, newMember1)"));

        // Test SortedSetRandomMembers
        _ = batch.SortedSetRandomMembers(newKey, 2);
        testData.Add(new(Array.Empty<ValkeyValue>(), "SortedSetRandomMembers(newKey, 2)", true));

        // Test SortedSetRandomMembersWithScores
        _ = batch.SortedSetRandomMembersWithScores(newKey, 2);
        testData.Add(new(Array.Empty<SortedSetEntry>(), "SortedSetRandomMembersWithScores(newKey, 2)", true));

        // Test SortedSetRangeAndStore (should copy all 3 elements from newKey to newDestKey)
        _ = batch.SortedSetRangeAndStore(newKey, newDestKey, 0, 9);
        testData.Add(new(3L, "SortedSetRangeAndStore(newKey, newDestKey, 0, 9)"));

        // Test SortedSetRemoveRangeByRank (should remove all 3 elements)
        _ = batch.SortedSetRemoveRangeByRank(newKey, 0, 9);
        testData.Add(new(3L, "SortedSetRemoveRangeByRank(newKey, 0, 9)"));

        // Setup data for union operations
        string unionKey1 = $"{prefix}union1-{Guid.NewGuid()}";
        string unionKey2 = $"{prefix}union2-{Guid.NewGuid()}";

        _ = batch.SortedSetAdd(unionKey1, [
            new SortedSetEntry("common", 1.0),
            new SortedSetEntry("unique1", 2.0)
        ]);
        testData.Add(new(2L, "SortedSetAdd(unionKey1, union test data)"));

        _ = batch.SortedSetAdd(unionKey2, [
            new SortedSetEntry("common", 3.0),
            new SortedSetEntry("unique2", 4.0)
        ]);
        testData.Add(new(2L, "SortedSetAdd(unionKey2, union test data)"));

        // Test SortedSetCombine with Union
        _ = batch.SortedSetCombine(SetOperation.Union, [unionKey1, unionKey2]);
        testData.Add(new(new ValkeyValue[] { "common", "unique1", "unique2" }, "SortedSetCombine(SetOperation.Union, [unionKey1, unionKey2])"));

        // Test SortedSetCombineAndStore with Union
        string unionDestKey = $"{prefix}uniondest-{Guid.NewGuid()}";
        _ = batch.SortedSetCombineAndStore(SetOperation.Union, unionDestKey, unionKey1, unionKey2);
        testData.Add(new(3L, "SortedSetCombineAndStore(SetOperation.Union, unionDestKey, unionKey1, unionKey2)"));

        return testData;
    }

    public static List<TestInfo> CreateListTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];
        string prefix = "{listKey}-";
        string atomicPrefix = isAtomic ? prefix : "";
        string key1 = $"{atomicPrefix}1-{Guid.NewGuid()}";
        string key2 = $"{atomicPrefix}2-{Guid.NewGuid()}";
        string key3 = $"{atomicPrefix}3-{Guid.NewGuid()}";
        string key4 = $"{atomicPrefix}4-{Guid.NewGuid()}";
        string key5 = $"{atomicPrefix}5-{Guid.NewGuid()}";

        string value1 = $"value-1-{Guid.NewGuid()}";
        string value2 = $"value-2-{Guid.NewGuid()}";
        string value3 = $"value-3-{Guid.NewGuid()}";
        string value4 = $"value-4-{Guid.NewGuid()}";

        // Test LPUSH and LPOP
        _ = batch.ListLeftPush(key1, [value1, value2]);
        testData.Add(new(2L, "ListLeftPush(key1, [value1, value2])"));

        _ = batch.ListLeftPush(key1, value3);
        testData.Add(new(3L, "ListLeftPush(key1, value3)"));

        _ = batch.ListLeftPush(key1, [value4]);
        testData.Add(new(4L, "ListLeftPush(key1, [value4])"));

        _ = batch.ListLeftPop(key1);
        testData.Add(new(new ValkeyValue(value4), "ListLeftPop(key1)"));

        _ = batch.ListLeftPop(key1);
        testData.Add(new(new ValkeyValue(value3), "ListLeftPop(key1) second"));

        _ = batch.ListLeftPop(key1);
        testData.Add(new(new ValkeyValue(value2), "ListLeftPop(key1) third"));

        _ = batch.ListLeftPush(key2, [value1, value2, value3, value4]);
        testData.Add(new(4L, "ListLeftPush(key2, [value1, value2, value3, value4])"));

        _ = batch.ListLeftPop(key2, 2);
        testData.Add(new(ValkeyValue.EmptyArray, "ListLeftPop(key2, 2)", true));

        _ = batch.ListLeftPop(key2, 10);
        testData.Add(new(Array.Empty<ValkeyValue>(), "ListLeftPop(key2, 10)", true));

        _ = batch.ListLeftPop(key3);
        testData.Add(new(ValkeyValue.Null, "ListLeftPop(key3) non-existent"));

        _ = batch.ListLeftPop(key3, 5);
        testData.Add(new(null, "ListLeftPop(key3, 5) non-existent"));

        // Test RPUSH and RPOP
        _ = batch.ListRightPush(key4, [value1, value2]);
        testData.Add(new(2L, "ListRightPush(key4, [value1, value2])"));

        _ = batch.ListRightPush(key4, [value3]);
        testData.Add(new(3L, "ListRightPush(key4, [value3])"));

        _ = batch.ListRightPop(key4);
        testData.Add(new(new ValkeyValue(value3), "ListRightPop(key4)"));

        _ = batch.ListRightPop(key4);
        testData.Add(new(new ValkeyValue(value2), "ListRightPop(key4) second"));

        _ = batch.ListRightPush(key5, [value1, value2, value3, value4]);
        testData.Add(new(4L, "ListRightPush(key5, [value1, value2, value3, value4])"));

        _ = batch.ListRightPop(key5, 2);
        testData.Add(new(ValkeyValue.EmptyArray, "ListRightPop(key5, 2)", true));

        _ = batch.ListRightPop(key5, 10);
        testData.Add(new(Array.Empty<ValkeyValue>(), "ListRightPop(key5, 10)", true));

        _ = batch.ListRightPop(key3);
        testData.Add(new(ValkeyValue.Null, "ListRightPop(key3) non-existent"));

        _ = batch.ListRightPop(key3, 5);
        testData.Add(new(null, "ListRightPop(key3, 5) non-existent"));

        // Test LLEN (List Length)
        _ = batch.ListLength(key1);
        testData.Add(new(1L, "ListLength(key1) after pops"));

        _ = batch.ListLength(key3);
        testData.Add(new(0L, "ListLength(key3) non-existent"));

        // Setup list for LREM, LTRIM, and LRANGE tests
        string testKey = $"{atomicPrefix}test-{Guid.NewGuid()}";
        _ = batch.ListRightPush(testKey, ["a", "b", "a", "c", "a"]);
        testData.Add(new(5L, "ListRightPush(testKey, [a, b, a, c, a])"));

        // Test LREM (List Remove)
        _ = batch.ListRemove(testKey, "a", 2);
        testData.Add(new(2L, "ListRemove(testKey, a, 2) - remove first 2 occurrences"));

        _ = batch.ListLength(testKey);
        testData.Add(new(3L, "ListLength(testKey) after remove"));

        // Setup another list for more LREM tests
        string remKey = $"{atomicPrefix}rem-{Guid.NewGuid()}";
        _ = batch.ListRightPush(remKey, ["x", "y", "x", "z", "x"]);
        testData.Add(new(5L, "ListRightPush(remKey, [x, y, x, z, x])"));

        _ = batch.ListRemove(remKey, "x", 0);
        testData.Add(new(3L, "ListRemove(remKey, x, 0) - remove all occurrences"));

        _ = batch.ListRemove(remKey, "nonexistent", 1);
        testData.Add(new(0L, "ListRemove(remKey, nonexistent, 1) - remove non-existent"));

        // Test LRANGE (List Range)
        string rangeKey = $"{atomicPrefix}range-{Guid.NewGuid()}";
        _ = batch.ListRightPush(rangeKey, ["0", "1", "2", "3", "4", "5"]);
        testData.Add(new(6L, "ListRightPush(rangeKey, [0, 1, 2, 3, 4, 5])"));

        _ = batch.ListRange(rangeKey, 0, -1);
        testData.Add(new(Array.Empty<ValkeyValue>(), "ListRange(rangeKey, 0, -1) - all elements", true));

        _ = batch.ListRange(rangeKey, 1, 3);
        testData.Add(new(Array.Empty<ValkeyValue>(), "ListRange(rangeKey, 1, 3) - subset", true));

        _ = batch.ListRange(rangeKey, -2, -1);
        testData.Add(new(Array.Empty<ValkeyValue>(), "ListRange(rangeKey, -2, -1) - last two", true));

        _ = batch.ListRange(key3, 0, -1);
        testData.Add(new(Array.Empty<ValkeyValue>(), "ListRange(key3, 0, -1) - non-existent", true));

        // Test LTRIM (List Trim)
        string trimKey = $"{atomicPrefix}trim-{Guid.NewGuid()}";
        _ = batch.ListRightPush(trimKey, ["a", "b", "c", "d", "e", "f"]);
        testData.Add(new(6L, "ListRightPush(trimKey, [a, b, c, d, e, f])"));

        _ = batch.ListTrim(trimKey, 1, 4);
        testData.Add(new("OK", "ListTrim(trimKey, 1, 4) - keep middle elements", true));

        _ = batch.ListLength(trimKey);
        testData.Add(new(4L, "ListLength(trimKey) after trim"));

        _ = batch.ListRange(trimKey, 0, -1);
        testData.Add(new(Array.Empty<ValkeyValue>(), "ListRange(trimKey, 0, -1) - after trim", true));

        // Test LTRIM with negative indices
        string trimKey2 = $"{atomicPrefix}trim2-{Guid.NewGuid()}";
        _ = batch.ListRightPush(trimKey2, ["1", "2", "3", "4", "5"]);
        testData.Add(new(5L, "ListRightPush(trimKey2, [1, 2, 3, 4, 5])"));

        _ = batch.ListTrim(trimKey2, -3, -1);
        testData.Add(new("OK", "ListTrim(trimKey2, -3, -1) - keep last 3", true));

        _ = batch.ListLength(trimKey2);
        testData.Add(new(3L, "ListLength(trimKey2) after negative trim"));

        // Test LMPOP (ListLeftPop with multiple keys)
        if (TestConfiguration.SERVER_VERSION >= new Version("7.0.0"))
        {
            string lmpopKey1 = $"{prefix}lmpop1-{Guid.NewGuid()}";
            string lmpopKey2 = $"{prefix}lmpop2-{Guid.NewGuid()}";
            string lmpopKey3 = $"{prefix}lmpop3-{Guid.NewGuid()}";

            _ = batch.ListRightPush(lmpopKey2, ["lmpop1", "lmpop2", "lmpop3"]);
            testData.Add(new(3L, "ListRightPush(lmpopKey2, [lmpop1, lmpop2, lmpop3])"));

            // This should pop 2 elements from lmpopKey2 (lmpop1, lmpop2)
            _ = batch.ListLeftPop([lmpopKey1, lmpopKey2], 2);
            testData.Add(new(new ListPopResult(lmpopKey2, ["lmpop1", "lmpop2"]), "ListLeftPop([lmpopKey1, lmpopKey2], 2)"));

            // This should pop 1 element from lmpopKey2 (lmpop3)
            _ = batch.ListRightPop([lmpopKey1, lmpopKey2], 1);
            testData.Add(new(new ListPopResult(lmpopKey2, ["lmpop3"]), "ListRightPop([lmpopKey1, lmpopKey2], 1)"));

            // Test LMPOP with empty keys - should return null
            _ = batch.ListLeftPop([lmpopKey1, lmpopKey3], 1);
            testData.Add(new(ListPopResult.Null, "ListLeftPop([lmpopKey1, lmpopKey3], 1) - empty keys"));
        }

        // Test LPUSHX and RPUSHX (When.Exists)
        string pushxKey = $"{atomicPrefix}pushx-{Guid.NewGuid()}";

        _ = batch.ListLeftPush(pushxKey, "test", When.Exists);
        testData.Add(new(0L, "ListLeftPush(pushxKey, test, When.Exists) - key doesn't exist"));

        _ = batch.ListRightPush(pushxKey, "test", When.Exists);
        testData.Add(new(0L, "ListRightPush(pushxKey, test, When.Exists) - key doesn't exist"));

        _ = batch.ListRightPush(pushxKey, "initial");
        testData.Add(new(1L, "ListRightPush(pushxKey, initial)"));

        _ = batch.ListLeftPush(pushxKey, "left", When.Exists);
        testData.Add(new(2L, "ListLeftPush(pushxKey, left, When.Exists) - key exists"));

        _ = batch.ListRightPush(pushxKey, "right", When.Exists);
        testData.Add(new(3L, "ListRightPush(pushxKey, right, When.Exists) - key exists"));

        // Test LINDEX (ListGetByIndex)
        string indexKey = $"{atomicPrefix}index-{Guid.NewGuid()}";
        _ = batch.ListRightPush(indexKey, ["idx0", "idx1", "idx2", "idx3"]);
        testData.Add(new(4L, "ListRightPush(indexKey, [idx0, idx1, idx2, idx3])"));

        _ = batch.ListGetByIndex(indexKey, 0);
        testData.Add(new(new ValkeyValue("idx0"), "ListGetByIndex(indexKey, 0)"));

        _ = batch.ListGetByIndex(indexKey, -1);
        testData.Add(new(new ValkeyValue("idx3"), "ListGetByIndex(indexKey, -1)"));

        _ = batch.ListGetByIndex(indexKey, 10);
        testData.Add(new(ValkeyValue.Null, "ListGetByIndex(indexKey, 10) - out of range"));

        // Test LINSERT (ListInsertBefore/After)
        string insertKey = $"{atomicPrefix}insert-{Guid.NewGuid()}";
        _ = batch.ListRightPush(insertKey, ["a", "c", "e"]);
        testData.Add(new(3L, "ListRightPush(insertKey, [a, c, e])"));

        _ = batch.ListInsertBefore(insertKey, "c", "b");
        testData.Add(new(4L, "ListInsertBefore(insertKey, c, b)"));

        _ = batch.ListInsertAfter(insertKey, "c", "d");
        testData.Add(new(5L, "ListInsertAfter(insertKey, c, d)"));

        _ = batch.ListInsertBefore(insertKey, "nonexistent", "x");
        testData.Add(new(-1L, "ListInsertBefore(insertKey, nonexistent, x)"));

        // Test LMOVE (ListMove)
        string moveSource = $"{prefix}movesrc-{Guid.NewGuid()}";
        string moveDest = $"{prefix}movedst-{Guid.NewGuid()}";

        _ = batch.ListRightPush(moveSource, ["move1", "move2", "move3"]);
        testData.Add(new(3L, "ListRightPush(moveSource, [move1, move2, move3])"));

        _ = batch.ListMove(moveSource, moveDest, ListSide.Left, ListSide.Right);
        testData.Add(new(new ValkeyValue("move1"), "ListMove(moveSource, moveDest, Left, Right)"));

        _ = batch.ListLength(moveSource);
        testData.Add(new(2L, "ListLength(moveSource) after move"));

        _ = batch.ListLength(moveDest);
        testData.Add(new(1L, "ListLength(moveDest) after move"));

        // Test LPOS (ListPosition/ListPositions)
        string posKey = $"{atomicPrefix}pos-{Guid.NewGuid()}";
        _ = batch.ListRightPush(posKey, ["a", "b", "a", "c", "a"]);
        testData.Add(new(5L, "ListRightPush(posKey, [a, b, a, c, a])"));

        _ = batch.ListPosition(posKey, "a");
        testData.Add(new(0L, "ListPosition(posKey, a) - first occurrence"));

        _ = batch.ListPosition(posKey, "a", 2);
        testData.Add(new(2L, "ListPosition(posKey, a, rank=2) - second occurrence"));

        _ = batch.ListPosition(posKey, "nonexistent");
        testData.Add(new(-1L, "ListPosition(posKey, nonexistent)"));

        _ = batch.ListPositions(posKey, "a", 10);
        testData.Add(new(Array.Empty<long>(), "ListPositions(posKey, a, count=10)"));

        // Test LSET (ListSetByIndex)
        string setKey = $"{atomicPrefix}set-{Guid.NewGuid()}";
        _ = batch.ListRightPush(setKey, ["set0", "set1", "set2"]);
        testData.Add(new(3L, "ListRightPush(setKey, [set0, set1, set2])"));

        _ = batch.ListSetByIndex(setKey, 1, "newvalue");
        testData.Add(new("OK", "ListSetByIndex(setKey, 1, newvalue)", true));

        _ = batch.ListGetByIndex(setKey, 1);
        testData.Add(new(new ValkeyValue("newvalue"), "ListGetByIndex(setKey, 1) after set"));

        // Test blocking list operations - reuse existing keys that already have data
        string blockingKey1 = $"{atomicPrefix}blocking1-{Guid.NewGuid()}";

        // Push data right before BLPOP so it has something to pop
        _ = batch.ListRightPush(blockingKey1, "block1");
        testData.Add(new(1L, "ListRightPush(blockingKey1, block1)"));

        _ = batch.ListBlockingLeftPop([blockingKey1], TimeSpan.FromMilliseconds(100));
        testData.Add(new(new ValkeyValue[] { blockingKey1, "block1" }, "ListBlockingLeftPop([blockingKey1], 100ms)"));

        // Push data right before BRPOP so it has something to pop
        _ = batch.ListRightPush(blockingKey1, "block2");
        testData.Add(new(1L, "ListRightPush(blockingKey1, block2)"));

        _ = batch.ListBlockingRightPop([blockingKey1], TimeSpan.FromMilliseconds(100));
        testData.Add(new(new ValkeyValue[] { blockingKey1, "block2" }, "ListBlockingRightPop([blockingKey1], 100ms)"));

        // Reuse moveSource which already has data from the previous ListMove test
        _ = batch.ListBlockingMove(moveSource, moveDest, ListSide.Left, ListSide.Right, TimeSpan.FromMilliseconds(100));
        testData.Add(new(new ValkeyValue("move2"), "ListBlockingMove(moveSource, moveDest, Left, Right, 100ms)"));

        if (TestConfiguration.SERVER_VERSION >= new Version("7.0.0"))
        {
            // Push more data to moveSource for the blocking pop test
            _ = batch.ListRightPush(moveSource, "move3");
            testData.Add(new(2L, "ListRightPush(moveSource, move3)"));

            _ = batch.ListBlockingPop([moveSource], ListSide.Right, TimeSpan.FromMilliseconds(100));
            testData.Add(new(new ListPopResult(moveSource, ["move3"]), "ListBlockingPop([moveSource], Right, 100ms)"));
        }

        return testData;
    }

    public static List<TestInfo> CreateConnectionManagementTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];

        _ = batch.Ping();
        testData.Add(new(TimeSpan.Zero, "Ping()", true));

        ValkeyValue pingMessage = "Hello Valkey!";
        _ = batch.Ping(pingMessage);
        testData.Add(new(TimeSpan.Zero, "Ping(message)", true));

        ValkeyValue echoMessage = "Echo test message";
        _ = batch.Echo(echoMessage);
        testData.Add(new(echoMessage, "Echo(message)"));

        _ = batch.Echo("");
        testData.Add(new(new ValkeyValue(""), "Echo(empty)"));

        // CLIENT GETNAME - should return null initially (no name set)
        _ = batch.ClientGetNameAsync();
        testData.Add(new(ValkeyValue.Null, "ClientGetNameAsync()"));

        // CLIENT ID - should return a positive long value
        _ = batch.ClientIdAsync();
        testData.Add(new(1L, "ClientIdAsync()", true)); // Check type only since ID is dynamic

        // SELECT - 9.0.0 allows both standalone and cluster, else just run standalone
        if (batch is Pipeline.Batch || TestConfiguration.SERVER_VERSION >= new Version("9.0.0"))
        {
            _ = batch.SelectAsync(0); // Select database 0 (default)
            testData.Add(new("OK", "SelectAsync(0)"));
        }

        return testData;
    }

    public static List<TestInfo> CreateHashTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];
        string prefix = "{hashKey}-";
        string atomicPrefix = isAtomic ? prefix : "";
        string key1 = $"{atomicPrefix}1-{Guid.NewGuid()}";
        string key2 = $"{atomicPrefix}2-{Guid.NewGuid()}";
        string nonExistingKey = $"{atomicPrefix}nonexisting-{Guid.NewGuid()}";

        string field1 = "field1";
        string field2 = "field2";
        string value1 = "value1";
        string value2 = "value2";

        // HashSet and HashGet tests
        _ = batch.HashSet(key1, field1, value1);
        testData.Add(new(true, "HashSet(key1, field1, value1)"));

        _ = batch.HashGet(key1, field1);
        testData.Add(new(new ValkeyValue(value1), "HashGet(key1, field1)"));

        _ = batch.HashSet(key1, field2, value2);
        testData.Add(new(true, "HashSet(key1, field2, value2)"));

        // HashIncrement tests (long)
        _ = batch.HashSet(key1, "counter", "10");
        testData.Add(new(true, "HashSet(key1, counter, 10)"));

        _ = batch.HashIncrement(key1, "counter", 5);
        testData.Add(new(15L, "HashIncrement(key1, counter, 5)"));

        _ = batch.HashIncrement(key1, "counter");
        testData.Add(new(16L, "HashIncrement(key1, counter) default"));

        // HashIncrement tests (double)
        _ = batch.HashSet(key1, "float_counter", "10.5");
        testData.Add(new(true, "HashSet(key1, float_counter, 10.5)"));

        _ = batch.HashIncrement(key1, "float_counter", 2.5);
        testData.Add(new(13.0, "HashIncrement(key1, float_counter, 2.5)"));

        // HashKeys test
        _ = batch.HashKeys(key1);
        testData.Add(new(Array.Empty<ValkeyValue>(), "HashKeys(key1)", true));

        // HashLength test
        _ = batch.HashLength(key1);
        testData.Add(new(4L, "HashLength(key1)"));

        // HashExists test
        _ = batch.HashExists(key1, field1);
        testData.Add(new(true, "HashExists(key1, field1)"));

        _ = batch.HashExists(key1, "nonexistent");
        testData.Add(new(false, "HashExists(key1, nonexistent)"));

        // HashStringLength test
        _ = batch.HashStringLength(key1, field1);
        testData.Add(new((long)value1.Length, "HashStringLength(key1, field1)"));

        _ = batch.HashStringLength(key1, "nonexistent");
        testData.Add(new(0L, "HashStringLength(key1, nonexistent)"));

        // HashDelete test
        _ = batch.HashDelete(key1, field2);
        testData.Add(new(true, "HashDelete(key1, field2)"));

        _ = batch.HashExists(key1, field2);
        testData.Add(new(false, "HashExists(key1, field2) after delete"));

        // Test with non-existing key
        _ = batch.HashGet(nonExistingKey, field1);
        testData.Add(new(ValkeyValue.Null, "HashGet(nonExistingKey, field1)"));

        _ = batch.HashLength(nonExistingKey);
        testData.Add(new(0L, "HashLength(nonExistingKey)"));

        _ = batch.HashKeys(nonExistingKey);
        testData.Add(new(Array.Empty<ValkeyValue>(), "HashKeys(nonExistingKey)"));

        // HashScan tests
        _ = batch.HashScan(key1, 0, "field*", 10);
        testData.Add(new((0L, new HashEntry[] { new("field1", "value1") }), "HashScan(key1, 0, field*, 10)"));

        // HashScanNoValues tests
        _ = batch.HashScanNoValues(key1, 0, "field*", 10);
        testData.Add(new((0L, new ValkeyValue[] { "field1" }), "HashScanNoValues(key1, 0, field*, 10)"));

        // HashGetAll test
        _ = batch.HashGetAll(key1);
        testData.Add(new(new HashEntry[] {
            new("field1", value1),
            new("counter", "16"),
            new("float_counter", "13")
        }, "HashGetAll(key1)"));

        // HashValues test
        _ = batch.HashValues(key1);
        testData.Add(new(new ValkeyValue[] { value1, "16", "13" }, "HashValues(key1)"));

        // HashRandomField tests
        _ = batch.HashRandomField(key1);
        testData.Add(new(new ValkeyValue("field1"), "HashRandomField(key1)"));

        _ = batch.HashRandomFields(key1, 2);
        testData.Add(new(new ValkeyValue[] { "field1", "counter" }, "HashRandomFields(key1, 2)"));

        _ = batch.HashRandomFieldsWithValues(key1, 2);
        testData.Add(new(new HashEntry[] {
            new("field1", value1),
            new("counter", "16")
        }, "HashRandomFieldsWithValues(key1, 2)"));

        // Multi-field operations
        HashEntry[] multiEntries = [
            new HashEntry("multi1", "value1"),
            new HashEntry("multi2", "value2")
        ];
        _ = batch.HashSet(key2, multiEntries);
        testData.Add(new("OK", "HashSet(key2, multiEntries)"));

        _ = batch.HashGet(key2, ["multi1", "multi2"]);
        testData.Add(new(new ValkeyValue[] { "value1", "value2" }, "HashGet(key2, [multi1, multi2])"));

        _ = batch.HashDelete(key2, ["multi1", "multi2"]);
        testData.Add(new(2L, "HashDelete(key2, [multi1, multi2])"));

        return testData;
    }

    public static TheoryData<BatchTestData> GetTestClientWithAtomic =>
        [.. TestConfiguration.TestClients.SelectMany(r => new[] { true, false }.SelectMany(isAtomic =>
            new BatchTestData[] {
                new("String commands", r.Data, CreateStringTest, isAtomic),
                new("Set commands", r.Data, CreateSetTest, isAtomic),
                new("Generic commands", r.Data, CreateGenericTest, isAtomic),
                new("Hash commands", r.Data, CreateHashTest, isAtomic),
                new("List commands", r.Data, CreateListTest, isAtomic),
                new("Sorted Set commands", r.Data, CreateSortedSetTest, isAtomic),
                new("Connection Management commands", r.Data, CreateConnectionManagementTest, isAtomic),
                new("Server Management commands", r.Data, CreateServerManagementTest, isAtomic)
            }))];
}

internal delegate List<TestInfo> BatchTestDataProvider(Pipeline.IBatch batch, bool isAtomic);

internal record BatchTestData(string TestName, BaseClient Client, BatchTestDataProvider TestDataProvider, bool IsAtomic)
{
    public string TestName = TestName;
    public BaseClient Client = Client;
    public BatchTestDataProvider TestDataProvider = TestDataProvider;
    public bool IsAtomic = IsAtomic;

    public override string? ToString() => $"{TestName} {Client} IsAtomic = {IsAtomic}";
}

internal record TestInfo(object? Expected, string TestName, bool CheckTypeOnly = false)
{
    public object? ExpectedValue = Expected;
    public string TestName = TestName;
    public bool CheckTypeOnly = CheckTypeOnly;
}
