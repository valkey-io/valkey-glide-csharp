// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class ListCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;
    private static readonly TimeSpan BlockingTimeout = TimeSpan.FromSeconds(5);

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestLPush_LPop(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        Assert.Equal(2, await client.ListLeftPushAsync(key, ["test1", "test2"]));
        Assert.Equal(3, await client.ListLeftPushAsync(key, ["test3"]));

        ValkeyValue lPopResult1 = await client.ListLeftPopAsync(key);
        Assert.Equal("test3", lPopResult1.ToGlideString());

        ValkeyValue lPopResult2 = await client.ListLeftPopAsync(key);
        Assert.Equal("test2", lPopResult2.ToGlideString());

        ValkeyValue lPopResult3 = await client.ListLeftPopAsync("non-exist-key");
        Assert.Equal(ValkeyValue.Null, lPopResult3);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestLPopWithCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        Assert.Equal(4, await client.ListLeftPushAsync(key, ["test1", "test2", "test3", "test4"]));

        ValkeyValue[]? lPopResultWithCount = await client.ListLeftPopAsync(key, 2);
        Assert.Equal(["test4", "test3"], lPopResultWithCount!.ToGlideStrings());

        ValkeyValue[]? lPopResultWithCount2 = await client.ListLeftPopAsync(key, 10);
        Assert.Equal(["test2", "test1"], lPopResultWithCount2!.ToGlideStrings());

        ValkeyValue[]? lPopResultWithCount3 = await client.ListLeftPopAsync("non-exist-key", 10);
        Assert.Null(lPopResultWithCount3);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestRPush_RPop(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test RPUSH - elements should be added to the tail
        Assert.Equal(2, await client.ListRightPushAsync(key, ["test1", "test2"]));
        Assert.Equal(3, await client.ListRightPushAsync(key, ["test3"]));

        // Test RPOP - should remove from tail (last added)
        ValkeyValue rPopResult1 = await client.ListRightPopAsync(key);
        Assert.Equal("test3", rPopResult1.ToGlideString());

        ValkeyValue rPopResult2 = await client.ListRightPopAsync(key);
        Assert.Equal("test2", rPopResult2.ToGlideString());

        // Test RPOP on non-existent key
        ValkeyValue rPopResult3 = await client.ListRightPopAsync("non-exist-key");
        Assert.Equal(ValkeyValue.Null, rPopResult3);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestLPushSingleValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test LPUSH with single value
        Assert.Equal(1, await client.ListLeftPushAsync(key, "test1"));
        Assert.Equal(2, await client.ListLeftPushAsync(key, "test2"));
        Assert.Equal(3, await client.ListLeftPushAsync(key, "test3"));

        // Verify order by popping from left (should be test3, test2, test1)
        ValkeyValue lPopResult1 = await client.ListLeftPopAsync(key);
        Assert.Equal("test3", lPopResult1.ToGlideString());

        ValkeyValue lPopResult2 = await client.ListLeftPopAsync(key);
        Assert.Equal("test2", lPopResult2.ToGlideString());

        ValkeyValue lPopResult3 = await client.ListLeftPopAsync(key);
        Assert.Equal("test1", lPopResult3.ToGlideString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestRPushSingleValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test RPUSH with single value
        Assert.Equal(1, await client.ListRightPushAsync(key, "test1"));
        Assert.Equal(2, await client.ListRightPushAsync(key, "test2"));
        Assert.Equal(3, await client.ListRightPushAsync(key, "test3"));

        // Verify order by popping from right (should be test3, test2, test1)
        ValkeyValue rPopResult1 = await client.ListRightPopAsync(key);
        Assert.Equal("test3", rPopResult1.ToGlideString());

        ValkeyValue rPopResult2 = await client.ListRightPopAsync(key);
        Assert.Equal("test2", rPopResult2.ToGlideString());

        ValkeyValue rPopResult3 = await client.ListRightPopAsync(key);
        Assert.Equal("test1", rPopResult3.ToGlideString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestRPopWithCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Setup list: [test1, test2, test3, test4] (left to right)
        Assert.Equal(4, await client.ListRightPushAsync(key, ["test1", "test2", "test3", "test4"]));

        // Pop 2 elements from right (tail)
        ValkeyValue[]? rPopResultWithCount = await client.ListRightPopAsync(key, 2);
        Assert.Equal(["test4", "test3"], rPopResultWithCount!.ToGlideStrings());

        // Pop more elements than available
        ValkeyValue[]? rPopResultWithCount2 = await client.ListRightPopAsync(key, 10);
        Assert.Equal(["test2", "test1"], rPopResultWithCount2!.ToGlideStrings());

        // Pop from non-existent key
        ValkeyValue[]? rPopResultWithCount3 = await client.ListRightPopAsync("non-exist-key", 10);
        Assert.Null(rPopResultWithCount3);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListLength(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test length of non-existent list
        Assert.Equal(0, await client.ListLengthAsync("non-exist-key"));

        // Test length after adding elements
        Assert.Equal(3, await client.ListLeftPushAsync(key, ["test1", "test2", "test3"]));
        Assert.Equal(3, await client.ListLengthAsync(key));

        // Test length after adding more elements
        Assert.Equal(5, await client.ListRightPushAsync(key, ["test4", "test5"]));
        Assert.Equal(5, await client.ListLengthAsync(key));

        // Test length after removing elements
        await client.ListLeftPopAsync(key);
        Assert.Equal(4, await client.ListLengthAsync(key));

        await client.ListRightPopAsync(key);
        Assert.Equal(3, await client.ListLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListRemove(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Setup list with duplicate values: [a, b, a, c, a, b]
        await client.ListRightPushAsync(key, ["a", "b", "a", "c", "a", "b"]);

        // Test removing all occurrences (count = 0)
        Assert.Equal(3, await client.ListRemoveAsync(key, "a", 0));
        Assert.Equal(3, await client.ListLengthAsync(key)); // Should have [b, c, b]

        // Reset list
        await client.KeyDeleteAsync(key);
        await client.ListRightPushAsync(key, ["a", "b", "a", "c", "a", "b"]);

        // Test removing from head to tail (count > 0)
        Assert.Equal(2, await client.ListRemoveAsync(key, "a", 2));
        Assert.Equal(4, await client.ListLengthAsync(key)); // Should have [b, c, a, b]

        // Reset list
        await client.KeyDeleteAsync(key);
        await client.ListRightPushAsync(key, ["a", "b", "a", "c", "a", "b"]);

        // Test removing from tail to head (count < 0)
        Assert.Equal(2, await client.ListRemoveAsync(key, "a", -2));
        Assert.Equal(4, await client.ListLengthAsync(key)); // Should have [a, b, c, b]

        // Test removing non-existent value
        Assert.Equal(0, await client.ListRemoveAsync(key, "x", 0));

        // Test removing from non-existent key
        Assert.Equal(0, await client.ListRemoveAsync("non-exist-key", "a", 0));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListTrim(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Setup list: [0, 1, 2, 3, 4, 5]
        await client.ListRightPushAsync(key, ["0", "1", "2", "3", "4", "5"]);

        // Trim to keep elements from index 1 to 3
        await client.ListTrimAsync(key, 1, 3);
        Assert.Equal(3, await client.ListLengthAsync(key));

        // Verify remaining elements
        ValkeyValue[] remaining = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["1", "2", "3"], remaining.ToGlideStrings());

        // Test trim with negative indices
        await client.KeyDeleteAsync(key);
        await client.ListRightPushAsync(key, ["0", "1", "2", "3", "4", "5"]);

        // Keep last 3 elements
        await client.ListTrimAsync(key, -3, -1);
        Assert.Equal(3, await client.ListLengthAsync(key));

        ValkeyValue[] lastThree = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["3", "4", "5"], lastThree.ToGlideStrings());

        // Test trim on non-existent key (should not throw)
        await client.ListTrimAsync("non-exist-key", 0, 1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListRange(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test range on non-existent key
        ValkeyValue[] emptyResult = await client.ListRangeAsync("non-exist-key", 0, -1);
        Assert.Empty(emptyResult);

        // Setup list: [0, 1, 2, 3, 4, 5]
        await client.ListRightPushAsync(key, ["0", "1", "2", "3", "4", "5"]);

        // Test getting all elements (default parameters)
        ValkeyValue[] allElements = await client.ListRangeAsync(key);
        Assert.Equal(["0", "1", "2", "3", "4", "5"], allElements.ToGlideStrings());

        // Test getting all elements explicitly
        ValkeyValue[] allElementsExplicit = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["0", "1", "2", "3", "4", "5"], allElementsExplicit.ToGlideStrings());

        // Test getting subset
        ValkeyValue[] subset = await client.ListRangeAsync(key, 1, 3);
        Assert.Equal(["1", "2", "3"], subset.ToGlideStrings());

        // Test with negative indices
        ValkeyValue[] lastTwo = await client.ListRangeAsync(key, -2, -1);
        Assert.Equal(["4", "5"], lastTwo.ToGlideStrings());

        // Test with start > stop (should return empty)
        ValkeyValue[] invalidRange = await client.ListRangeAsync(key, 3, 1);
        Assert.Empty(invalidRange);

        // Test with out-of-bounds indices
        ValkeyValue[] outOfBounds = await client.ListRangeAsync(key, 10, 20);
        Assert.Empty(outOfBounds);

        // Test single element
        ValkeyValue[] singleElement = await client.ListRangeAsync(key, 2, 2);
        Assert.Equal(["2"], singleElement.ToGlideStrings());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListCommandsIntegration(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test comprehensive workflow combining all list commands

        // 1. Build list using both LPUSH and RPUSH
        Assert.Equal(2, await client.ListLeftPushAsync(key, ["left2", "left1"])); // [left1, left2]
        Assert.Equal(4, await client.ListRightPushAsync(key, ["right1", "right2"])); // [left1, left2, right1, right2]
        Assert.Equal(6, await client.ListLeftPushAsync(key, ["extra2", "extra1"])); // [extra1, extra2, left1, left2, right1, right2]

        // 2. Verify length
        Assert.Equal(6, await client.ListLengthAsync(key));

        // 3. Check full range
        ValkeyValue[] fullList = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["extra1", "extra2", "left1", "left2", "right1", "right2"], fullList.ToGlideStrings());

        // 4. Add duplicates and test removal
        await client.ListRightPushAsync(key, ["left1", "duplicate", "left1"]); // [extra1, extra2, left1, left2, right1, right2, left1, duplicate, left1]
        Assert.Equal(9, await client.ListLengthAsync(key));

        // Remove first 2 occurrences of "left1"
        Assert.Equal(2, await client.ListRemoveAsync(key, "left1", 2));
        Assert.Equal(7, await client.ListLengthAsync(key)); // [extra1, extra2, left2, right1, right2, duplicate, left1]

        // 5. Trim to middle section
        await client.ListTrimAsync(key, 2, 4); // Keep [left2, right1, right2]
        Assert.Equal(3, await client.ListLengthAsync(key));

        // 6. Verify final state
        ValkeyValue[] finalList = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["left2", "right1", "right2"], finalList.ToGlideStrings());

        // 7. Pop remaining elements
        ValkeyValue leftPop = await client.ListLeftPopAsync(key);
        Assert.Equal("left2", leftPop.ToGlideString());

        ValkeyValue rightPop = await client.ListRightPopAsync(key);
        Assert.Equal("right2", rightPop.ToGlideString());

        Assert.Equal(1, await client.ListLengthAsync(key));

        ValkeyValue[] lastElement = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["right1"], lastElement.ToGlideStrings());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListMultiPop(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LMPOP is supported since 7.0.0"
        );

        string key1 = $"{{listKey}}-multipop1-{Guid.NewGuid().ToString()}";
        string key2 = $"{{listKey}}-multipop2-{Guid.NewGuid().ToString()}";
        string key3 = $"{{listKey}}-multipop3-{Guid.NewGuid().ToString()}";

        // Test LMPOP with empty lists
        ListPopResult emptyResult = await client.ListLeftPopAsync([key1, key2], 2);
        Assert.True(emptyResult.IsNull);

        // Setup lists
        await client.ListRightPushAsync(key2, ["a", "b", "c", "d"]);
        await client.ListRightPushAsync(key3, ["x", "y", "z"]);

        // Test LMPOP LEFT - should pop from first non-empty list (key2)
        ListPopResult leftResult = await client.ListLeftPopAsync([key1, key2, key3], 2);
        Assert.False(leftResult.IsNull);
        Assert.Equal(key2, leftResult.Key);
        Assert.Equal(["a", "b"], leftResult.Values.ToGlideStrings());

        // Verify key2 has remaining elements
        Assert.Equal(2, await client.ListLengthAsync(key2));

        // Test LMPOP RIGHT - should pop from first non-empty list (key2)
        ListPopResult rightResult = await client.ListRightPopAsync([key1, key2, key3], 1);
        Assert.False(rightResult.IsNull);
        Assert.Equal(key2, rightResult.Key);
        Assert.Equal(["d"], rightResult.Values.ToGlideStrings());

        // Now key2 has only one element, test popping more than available
        ListPopResult moreResult = await client.ListLeftPopAsync([key1, key2, key3], 5);
        Assert.False(moreResult.IsNull);
        Assert.Equal(key2, moreResult.Key);
        Assert.Equal(["c"], moreResult.Values.ToGlideStrings());

        // key2 is now empty, should pop from key3
        ListPopResult nextResult = await client.ListRightPopAsync([key1, key2, key3], 2);
        Assert.False(nextResult.IsNull);
        Assert.Equal(key3, nextResult.Key);
        Assert.Equal(["z", "y"], nextResult.Values.ToGlideStrings());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListPushX(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test LPUSHX on non-existent key
        Assert.Equal(0, await client.ListLeftPushAsync(key, "test", When.Exists));
        Assert.Equal(0, await client.ListLengthAsync(key));

        // Test RPUSHX on non-existent key
        Assert.Equal(0, await client.ListRightPushAsync(key, "test", When.Exists));
        Assert.Equal(0, await client.ListLengthAsync(key));

        // Create the list first
        Assert.Equal(1, await client.ListRightPushAsync(key, "initial"));

        // Now LPUSHX should work
        Assert.Equal(2, await client.ListLeftPushAsync(key, "left", When.Exists));
        Assert.Equal(4, await client.ListLeftPushAsync(key, ["left2", "left3"], When.Exists));

        // And RPUSHX should work
        Assert.Equal(5, await client.ListRightPushAsync(key, "right", When.Exists));
        Assert.Equal(7, await client.ListRightPushAsync(key, ["right2", "right3"], When.Exists));

        // Verify final order
        ValkeyValue[] result = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["left3", "left2", "left", "initial", "right", "right2", "right3"], result.ToGlideStrings());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListGetByIndex(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        ValkeyValue nullResult = await client.ListGetByIndexAsync("non-exist-key", 0);
        Assert.Equal(ValkeyValue.Null, nullResult);

        // Setup list
        await client.ListRightPushAsync(key, ["zero", "one", "two", "three", "four"]);

        // Test positive indices
        Assert.Equal("zero", (await client.ListGetByIndexAsync(key, 0)).ToGlideString());
        Assert.Equal("two", (await client.ListGetByIndexAsync(key, 2)).ToGlideString());
        Assert.Equal("four", (await client.ListGetByIndexAsync(key, 4)).ToGlideString());

        // Test negative indices
        Assert.Equal("four", (await client.ListGetByIndexAsync(key, -1)).ToGlideString());
        Assert.Equal("three", (await client.ListGetByIndexAsync(key, -2)).ToGlideString());
        Assert.Equal("zero", (await client.ListGetByIndexAsync(key, -5)).ToGlideString());

        // Test out of range
        ValkeyValue outOfRange1 = await client.ListGetByIndexAsync(key, 10);
        Assert.Equal(ValkeyValue.Null, outOfRange1);

        ValkeyValue outOfRange2 = await client.ListGetByIndexAsync(key, -10);
        Assert.Equal(ValkeyValue.Null, outOfRange2);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListInsert(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test insert on non-existent key
        Assert.Equal(0, await client.ListInsertBeforeAsync("non-exist-key", "pivot", "value"));
        Assert.Equal(0, await client.ListInsertAfterAsync("non-exist-key", "pivot", "value"));

        // Setup list
        await client.ListRightPushAsync(key, ["a", "c", "e"]);

        // Test insert before
        Assert.Equal(4, await client.ListInsertBeforeAsync(key, "c", "b"));
        ValkeyValue[] afterBefore = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["a", "b", "c", "e"], afterBefore.ToGlideStrings());

        // Test insert after
        Assert.Equal(5, await client.ListInsertAfterAsync(key, "c", "d"));
        ValkeyValue[] afterAfter = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["a", "b", "c", "d", "e"], afterAfter.ToGlideStrings());

        // Test insert with non-existent pivot
        Assert.Equal(-1, await client.ListInsertBeforeAsync(key, "nonexistent", "x"));
        Assert.Equal(-1, await client.ListInsertAfterAsync(key, "nonexistent", "y"));

        // List should remain unchanged
        ValkeyValue[] unchanged = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["a", "b", "c", "d", "e"], unchanged.ToGlideStrings());

        // Test insert with duplicate values (should insert at first occurrence)
        await client.ListRightPushAsync(key, "c"); // Now: [a, b, c, d, e, c]
        Assert.Equal(7, await client.ListInsertBeforeAsync(key, "c", "before_first_c"));
        ValkeyValue[] withDuplicate = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["a", "b", "before_first_c", "c", "d", "e", "c"], withDuplicate.ToGlideStrings());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListMove(BaseClient client)
    {
        string source = $"{{listKey}}-movesrc-{Guid.NewGuid().ToString()}";
        string dest = $"{{listKey}}-movedst-{Guid.NewGuid().ToString()}";

        // Test move from non-existent source
        ValkeyValue nullMove = await client.ListMoveAsync("{listKey}-non-exist-source", dest, ListSide.Left, ListSide.Right);
        Assert.Equal(ValkeyValue.Null, nullMove);

        // Setup source list
        await client.ListRightPushAsync(source, ["a", "b", "c", "d"]);

        // Test LEFT to RIGHT move
        ValkeyValue moved1 = await client.ListMoveAsync(source, dest, ListSide.Left, ListSide.Right);
        Assert.Equal("a", moved1.ToGlideString());

        // Verify source and destination
        ValkeyValue[] sourceAfter1 = await client.ListRangeAsync(source, 0, -1);
        Assert.Equal(["b", "c", "d"], sourceAfter1.ToGlideStrings());

        ValkeyValue[] destAfter1 = await client.ListRangeAsync(dest, 0, -1);
        Assert.Equal(["a"], destAfter1.ToGlideStrings());

        // Test RIGHT to LEFT move
        ValkeyValue moved2 = await client.ListMoveAsync(source, dest, ListSide.Right, ListSide.Left);
        Assert.Equal("d", moved2.ToGlideString());

        ValkeyValue[] sourceAfter2 = await client.ListRangeAsync(source, 0, -1);
        Assert.Equal(["b", "c"], sourceAfter2.ToGlideStrings());

        ValkeyValue[] destAfter2 = await client.ListRangeAsync(dest, 0, -1);
        Assert.Equal(["d", "a"], destAfter2.ToGlideStrings());

        // Test LEFT to LEFT move
        ValkeyValue moved3 = await client.ListMoveAsync(source, dest, ListSide.Left, ListSide.Left);
        Assert.Equal("b", moved3.ToGlideString());

        ValkeyValue[] destAfter3 = await client.ListRangeAsync(dest, 0, -1);
        Assert.Equal(["b", "d", "a"], destAfter3.ToGlideStrings());

        // Test RIGHT to RIGHT move
        ValkeyValue moved4 = await client.ListMoveAsync(source, dest, ListSide.Right, ListSide.Right);
        Assert.Equal("c", moved4.ToGlideString());

        ValkeyValue[] destAfter4 = await client.ListRangeAsync(dest, 0, -1);
        Assert.Equal(["b", "d", "a", "c"], destAfter4.ToGlideStrings());

        // Source should now be empty
        Assert.Equal(0, await client.ListLengthAsync(source));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListPosition(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        string nonExistentKey = "non-exist-key-" + Guid.NewGuid();
        string nonExistentElement = "non-existent-element-" + Guid.NewGuid();
        long result = await client.ListPositionAsync(nonExistentKey, nonExistentElement);
        Assert.Equal(-1, result);

        // Setup list with duplicates
        await client.ListRightPushAsync(key, ["a", "b", "a", "c", "a", "d"]);

        // Test basic position (first occurrence)
        Assert.Equal(0, await client.ListPositionAsync(key, "a"));
        Assert.Equal(1, await client.ListPositionAsync(key, "b"));
        Assert.Equal(3, await client.ListPositionAsync(key, "c"));

        // Test with rank (nth occurrence)
        Assert.Equal(0, await client.ListPositionAsync(key, "a", 1)); // First occurrence
        Assert.Equal(2, await client.ListPositionAsync(key, "a", 2)); // Second occurrence
        Assert.Equal(4, await client.ListPositionAsync(key, "a", 3)); // Third occurrence

        // Test with negative rank (from end)
        Assert.Equal(4, await client.ListPositionAsync(key, "a", -1)); // Last occurrence
        Assert.Equal(2, await client.ListPositionAsync(key, "a", -2)); // Second to last
        Assert.Equal(0, await client.ListPositionAsync(key, "a", -3)); // Third to last

        // Test non-existent element
        Assert.Equal(-1, await client.ListPositionAsync(key, "nonexistent"));

        // Test with maxLength
        Assert.Equal(0, await client.ListPositionAsync(key, "a", 1, 3)); // Search only first 3 elements
        Assert.Equal(2, await client.ListPositionAsync(key, "a", 2, 3)); // Second 'a' is at index 2, but we only search first 3

        // Test rank beyond available occurrences
        Assert.Equal(-1, await client.ListPositionAsync(key, "a", 5)); // Only 3 'a's exist
        Assert.Equal(-1, await client.ListPositionAsync(key, "b", 2)); // Only 1 'b' exists
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListPositions(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        long[] emptyResult = await client.ListPositionsAsync("{listKey}-non-exist-key-" + Guid.NewGuid(), "{listKey}-non-existent-element-" + Guid.NewGuid(), 5);
        Assert.Empty(emptyResult);

        // Setup list with duplicates
        await client.ListRightPushAsync(key, ["a", "b", "a", "c", "a", "d", "a"]);

        // Test getting all positions
        long[] allPositions = await client.ListPositionsAsync(key, "a", 10);
        Assert.Equal([0, 2, 4, 6], allPositions);

        // Test limiting count
        long[] limitedPositions = await client.ListPositionsAsync(key, "a", 2);
        Assert.Equal([0, 2], limitedPositions);

        // Test with rank (starting from nth occurrence)
        long[] fromSecond = await client.ListPositionsAsync(key, "a", 2, 2);
        Assert.Equal([2, 4], fromSecond);

        // Test with negative rank (from end)
        long[] fromEnd = await client.ListPositionsAsync(key, "a", 2, -1);
        Assert.Equal([6, 4], fromEnd);

        // Test with maxLength
        long[] withMaxLen = await client.ListPositionsAsync(key, "a", 5, 1, 5);
        Assert.Equal([0, 2, 4], withMaxLen); // Only search first 5 elements

        // Test non-existent element
        long[] nonExistent = await client.ListPositionsAsync(key, "nonexistent", 5);
        Assert.Empty(nonExistent);

        // Test single occurrence element
        long[] singleOccurrence = await client.ListPositionsAsync(key, "b", 5);
        Assert.Equal([1], singleOccurrence);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListSetByIndex(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Setup list
        await client.ListRightPushAsync(key, ["zero", "one", "two", "three", "four"]);

        // Test setting by positive index
        await client.ListSetByIndexAsync(key, 0, "ZERO");
        Assert.Equal("ZERO", (await client.ListGetByIndexAsync(key, 0)).ToGlideString());

        await client.ListSetByIndexAsync(key, 2, "TWO");
        Assert.Equal("TWO", (await client.ListGetByIndexAsync(key, 2)).ToGlideString());

        // Test setting by negative index
        await client.ListSetByIndexAsync(key, -1, "FOUR");
        Assert.Equal("FOUR", (await client.ListGetByIndexAsync(key, -1)).ToGlideString());

        await client.ListSetByIndexAsync(key, -2, "THREE");
        Assert.Equal("THREE", (await client.ListGetByIndexAsync(key, -2)).ToGlideString());

        // Verify final state
        ValkeyValue[] finalState = await client.ListRangeAsync(key, 0, -1);
        Assert.Equal(["ZERO", "one", "TWO", "THREE", "FOUR"], finalState.ToGlideStrings());

        // Test error cases - out of range indices should throw
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.ListSetByIndexAsync(key, 10, "invalid"));

        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.ListSetByIndexAsync(key, -10, "invalid"));

        // Test on non-existent key should throw
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.ListSetByIndexAsync("non-exist-key", 0, "value"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListBlockingLeftPop(BaseClient client)
    {
        // Use hash tags to ensure keys map to same slot in cluster mode
        string key1 = $"{{testkey}}-{Guid.NewGuid()}";
        string key2 = $"{{testkey}}-{Guid.NewGuid()}";

        // Test with populated list
        await client.ListLeftPushAsync(key1, ["value1", "value2"]);

        ValkeyValue[]? result = await client.ListBlockingLeftPopAsync([key1, key2], BlockingTimeout);
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.Equal(key1, result[0].ToGlideString());
        Assert.Equal("value2", result[1].ToGlideString());

        // Test timeout with empty lists first
        ValkeyValue[]? timeoutResult = await client.ListBlockingLeftPopAsync([key2], BlockingTimeout);
        Assert.Null(timeoutResult);

        // Test with data available - push first, then pop
        string testKey = $"{{testkey}}-test-{Guid.NewGuid()}";
        await client.ListRightPushAsync(testKey, ["test1", "test2"]);

        ValkeyValue[]? result2 = await client.ListBlockingLeftPopAsync([testKey], BlockingTimeout);
        Assert.NotNull(result2);
        Assert.Equal(testKey, result2[0].ToGlideString());
        Assert.Equal("test1", result2[1].ToGlideString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListBlockingRightPop(BaseClient client)
    {
        // Use hash tags to ensure keys map to same slot in cluster mode
        string key1 = $"{{testkey}}-{Guid.NewGuid()}";
        string key2 = $"{{testkey}}-{Guid.NewGuid()}";

        // Test with populated list
        await client.ListRightPushAsync(key1, ["value1", "value2"]);

        ValkeyValue[]? result = await client.ListBlockingRightPopAsync([key1, key2], BlockingTimeout);
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.Equal(key1, result[0].ToGlideString());
        Assert.Equal("value2", result[1].ToGlideString());

        // Test timeout with empty lists first
        ValkeyValue[]? timeoutResult = await client.ListBlockingRightPopAsync([key2], BlockingTimeout);
        Assert.Null(timeoutResult);

        // Test with data available - push first, then pop
        string testKey = $"{{testkey}}-test-{Guid.NewGuid()}";
        await client.ListLeftPushAsync(testKey, ["test1", "test2"]);

        ValkeyValue[]? result2 = await client.ListBlockingRightPopAsync([testKey], BlockingTimeout);
        Assert.NotNull(result2);
        Assert.Equal(testKey, result2[0].ToGlideString());
        Assert.Equal("test1", result2[1].ToGlideString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListBlockingMove(BaseClient client)
    {
        // Use hash tags to ensure keys map to same slot in cluster mode
        string source = $"{{testkey}}-src-{Guid.NewGuid()}";
        string destination = $"{{testkey}}-dst-{Guid.NewGuid()}";
        string emptyKey = $"{{testkey}}-empty-{Guid.NewGuid()}";

        // Test with populated source list
        await client.ListLeftPushAsync(source, ["value1", "value2"]);

        ValkeyValue result = await client.ListBlockingMoveAsync(source, destination, ListSide.Left, ListSide.Right, BlockingTimeout);
        Assert.Equal("value2", result.ToGlideString());

        // Verify the move
        ValkeyValue[] sourceList = await client.ListRangeAsync(source, 0, -1);
        Assert.Equal(["value1"], sourceList.ToGlideStrings());

        ValkeyValue[] destList = await client.ListRangeAsync(destination, 0, -1);
        Assert.Equal(["value2"], destList.ToGlideStrings());

        // Test timeout with empty source first
        ValkeyValue timeoutResult = await client.ListBlockingMoveAsync(emptyKey, destination, ListSide.Left, ListSide.Right, BlockingTimeout);
        Assert.Equal(ValkeyValue.Null, timeoutResult);

        // Test with data available - push first, then move
        string testSource = $"{{testkey}}-test-src-{Guid.NewGuid()}";
        string testDest = $"{{testkey}}-test-dst-{Guid.NewGuid()}";
        await client.ListLeftPushAsync(testSource, "move_value");

        ValkeyValue result2 = await client.ListBlockingMoveAsync(testSource, testDest, ListSide.Left, ListSide.Right, BlockingTimeout);
        Assert.Equal("move_value", result2.ToGlideString());

        // Verify the move happened
        ValkeyValue[] testDestList = await client.ListRangeAsync(testDest, 0, -1);
        Assert.Equal(["move_value"], testDestList.ToGlideStrings());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestListBlockingPop(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "BLMPOP is supported since 7.0.0"
        );

        // Use hash tags to ensure keys map to same slot in cluster mode
        string key1 = $"{{testkey}}-{Guid.NewGuid()}";
        string key2 = $"{{testkey}}-{Guid.NewGuid()}";

        // Test with populated list - single element
        await client.ListLeftPushAsync(key1, ["value1", "value2", "value3"]);

        ListPopResult result = await client.ListBlockingPopAsync([key1, key2], ListSide.Left, BlockingTimeout);
        Assert.NotEqual(ListPopResult.Null, result);
        Assert.Equal(key1, result.Key.ToGlideString());
        Assert.Equal(["value3"], result.Values.ToGlideStrings());

        // Test with count
        ListPopResult resultWithCount = await client.ListBlockingPopAsync([key1], ListSide.Left, 2, BlockingTimeout);
        Assert.NotEqual(ListPopResult.Null, resultWithCount);
        Assert.Equal(key1, resultWithCount.Key.ToGlideString());
        Assert.Equal(["value2", "value1"], resultWithCount.Values.ToGlideStrings());

        // Test timeout with empty lists first
        ListPopResult timeoutResult = await client.ListBlockingPopAsync([key2], ListSide.Left, BlockingTimeout);
        Assert.True(timeoutResult.IsNull);

        // Test with data available - push first, then pop
        string testKey = $"{{testkey}}-test-{Guid.NewGuid()}";
        await client.ListRightPushAsync(testKey, ["test1", "test2"]);

        ListPopResult result2 = await client.ListBlockingPopAsync([testKey], ListSide.Left, BlockingTimeout);
        Assert.NotEqual(ListPopResult.Null, result2);
        Assert.Equal(testKey, result2.Key.ToGlideString());
        Assert.Equal(["test1"], result2.Values.ToGlideStrings());
    }
}
