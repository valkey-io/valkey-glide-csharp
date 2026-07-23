// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> stream commands — verifies SER API compatibility.
/// </summary>
public class StreamCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region StreamAddAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_Legacy_SingleField(IDatabaseAsync db)
    {
        string key = $"ser-xadd-{Guid.NewGuid()}";
        ValkeyValue id = await db.StreamAddAsync(key, "field", "value");
        Assert.False(id.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_Legacy_MultipleFields(IDatabaseAsync db)
    {
        string key = $"ser-xadd-multi-{Guid.NewGuid()}";
        NameValueEntry[] entries = [new("field1", "value1"), new("field2", "value2")];
        ValkeyValue id = await db.StreamAddAsync(key, entries);
        Assert.False(id.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_WithMaxLength(IDatabaseAsync db)
    {
        string key = $"ser-xadd-trim-{Guid.NewGuid()}";

        for (int i = 0; i < 5; i++)
        {
            _ = await db.StreamAddAsync(key, "field", $"value{i}", maxLength: 3);
        }

        long length = await db.StreamLengthAsync(key);
        Assert.Equal(3, length);
    }

    #endregion
    #region StreamRangeAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_WithMinIdMaxId(IDatabaseAsync db)
    {
        string key = $"ser-xrange-{Guid.NewGuid()}";
        ValkeyValue id1 = await db.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await db.StreamAddAsync(key, "field", "value2");
        _ = await db.StreamAddAsync(key, "field", "value3");

        var entries = await db.StreamRangeAsync(key, minId: id1, maxId: id2);
        Assert.Equal(2, entries.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_Descending(IDatabaseAsync db)
    {
        string key = $"ser-xrevrange-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value1");
        _ = await db.StreamAddAsync(key, "field", "value2");
        _ = await db.StreamAddAsync(key, "field", "value3");

        var entries = await db.StreamRangeAsync(key, messageOrder: Order.Descending);
        Assert.Equal(3, entries.Length);
        Assert.Equal("value3", entries[0]["field"].ToString());
        Assert.Equal("value1", entries[2]["field"].ToString());
    }

    #endregion
    #region StreamReadAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_SingleStream(IDatabaseAsync db)
    {
        string key = $"ser-xread-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value1");
        _ = await db.StreamAddAsync(key, "field", "value2");

        var entries = await db.StreamReadAsync(key, "0-0");
        Assert.Equal(2, entries.Length);
    }

    #endregion
    #region StreamGroupAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamCreateConsumerGroupAsync_ReturnsBool(IDatabaseAsync db)
    {
        string key = $"ser-xgroup-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value");

        bool result = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        Assert.True(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamConsumerGroupSetPositionAsync_ReturnsBool(IDatabaseAsync db)
    {
        string key = $"ser-xgroupsetid-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");

        bool result = await db.StreamConsumerGroupSetPositionAsync(key, "mygroup", "0-0");
        Assert.True(result);
    }

    #endregion
    #region StreamReadGroupAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_NoGroup(IDatabaseAsync db)
    {
        var key = $"ser-xreadgroup-nogroup-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field1", "value1");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(
            () => db.StreamReadGroupAsync(key, "nonexistent", "consumer1", ">"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_WrongKeyType(IDatabaseAsync db)
    {
        var key = $"ser-xreadgroup-wrongtype-{Guid.NewGuid()}";
        _ = await db.StringSetAsync(key, "not_a_stream");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(
            () => db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_SingleStream(IDatabaseAsync db)
    {
        var key = $"ser-xreadgroup-single-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, [new NameValueEntry("a1", "b1"), new("a2", "b2")]);
        _ = await db.StreamAddAsync(key, [new NameValueEntry("c1", "d1"), new("c2", "d2"), new("c3", "d3")]);
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");

        var entries = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">");
        Assert.Equal(2, entries.Length);
        Assert.Equivalent(new NameValueEntry[] { new("a1", "b1"), new("a2", "b2") }, entries[0].Values);
        Assert.Equivalent(new NameValueEntry[] { new("c1", "d1"), new("c2", "d2"), new("c3", "d3") }, entries[1].Values);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_MultiStream(IDatabaseAsync db)
    {
        var key1 = $"ser-xreadgroup-ms1-{Guid.NewGuid()}";
        var key2 = $"ser-xreadgroup-ms2-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key1, [new NameValueEntry("f1", "v1"), new("f2", "v2")]);
        _ = await db.StreamAddAsync(key2, [new NameValueEntry("f3", "v3"), new("f4", "v4")]);
        _ = await db.StreamCreateConsumerGroupAsync(key1, "mygroup", "0");
        _ = await db.StreamCreateConsumerGroupAsync(key2, "mygroup", "0");

        var streams = await db.StreamReadGroupAsync([new StreamPosition(key1, ">"), new StreamPosition(key2, ">")], "mygroup", "consumer1");
        Assert.Equal(2, streams.Length);

        var stream0 = streams[0];
        Assert.Equal(key1, stream0.Key);
        Assert.Equivalent(new NameValueEntry[] { new("f1", "v1"), new("f2", "v2") }, Assert.Single(stream0.Entries).Values);

        var stream1 = streams[1];
        Assert.Equal(key2, stream1.Key);
        Assert.Equivalent(new NameValueEntry[] { new("f3", "v3"), new("f4", "v4") }, Assert.Single(stream1.Entries).Values);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_WithCount(IDatabaseAsync db)
    {
        var key = $"ser-xreadgroup-count-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field1", "value1");
        _ = await db.StreamAddAsync(key, "field2", "value2");
        _ = await db.StreamAddAsync(key, "field3", "value3");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");

        var entries = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">", count: 2);
        Assert.Equal(2, entries.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_WithNoAck(IDatabaseAsync db)
    {
        var key = $"ser-xreadgroup-noack-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");

        _ = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">", noAck: true);

        var pending = await db.StreamPendingAsync(key, "mygroup");
        Assert.Equal(0, pending.PendingMessageCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_WithPosition(IDatabaseAsync db)
    {
        var key = $"ser-xreadgroup-pos-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field1", "value1");
        var id2 = await db.StreamAddAsync(key, "field2", "value2");
        _ = await db.StreamAddAsync(key, "field3", "value3");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", id2);

        var entries = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">");
        Assert.Equivalent(new NameValueEntry[] { new("field3", "value3") }, Assert.Single(entries).Values);
    }

    #endregion
    #region StreamClaimIdsOnlyAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimIdsOnlyAsync_SERNaming(IDatabaseAsync db)
    {
        string key = $"ser-xclaim-idsonly-{Guid.NewGuid()}";
        ValkeyValue id1 = await db.StreamAddAsync(key, "field", "value1");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        _ = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">", count: 1);

        ValkeyValue[] claimedIds = await db.StreamClaimIdsOnlyAsync(key, "mygroup", "consumer2", 0, [id1]);
        _ = Assert.Single(claimedIds);
        Assert.Equal(id1.ToString(), claimedIds[0].ToString());
    }

    #endregion
    #region StreamAutoClaimIdsOnlyAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamAutoClaimIdsOnlyAsync_SERNaming(IDatabaseAsync db)
    {
        string key = $"ser-xautoclaim-idsonly-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value1");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        _ = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">", count: 1);

        StreamAutoClaimJustIdResult result = await db.StreamAutoClaimIdsOnlyAsync(key, "mygroup", "consumer2", 0, "0-0");
        _ = Assert.Single(result.ClaimedIds);
    }

    #endregion
    #region StreamTrimAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamTrimAsync_WithTrimMode(IDatabaseAsync db)
    {
        string key = $"ser-xtrim-{Guid.NewGuid()}";
        for (int i = 0; i < 5; i++)
        {
            _ = await db.StreamAddAsync(key, "field", $"value{i}");
        }

        long trimmed = await db.StreamTrimAsync(key, maxLength: 3);
        Assert.Equal(2, trimmed);
        Assert.Equal(3, await db.StreamLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamTrimByMinIdAsync_WithTrimMode(IDatabaseAsync db)
    {
        string key = $"ser-xtrim-minid-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await db.StreamAddAsync(key, "field", "value2");
        _ = await db.StreamAddAsync(key, "field", "value3");

        long trimmed = await db.StreamTrimByMinIdAsync(key, id2);
        Assert.True(trimmed >= 1);
    }

    #endregion
    #region StreamInfoAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoAsync_ReturnsInfo(IDatabaseAsync db)
    {
        string key = $"ser-xinfo-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value");

        StreamInfo info = await db.StreamInfoAsync(key);
        Assert.Equal(1, info.Length);
    }

    #endregion
    #region StreamPendingMessagesAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_WithConsumer(IDatabaseAsync db)
    {
        string key = $"ser-xpending-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value1");
        _ = await db.StreamAddAsync(key, "field", "value2");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        _ = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">", count: 2);

        StreamPendingMessageInfo[] messages = await db.StreamPendingMessagesAsync(key, "mygroup", 10, "consumer1");
        Assert.Equal(2, messages.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_WithoutConsumer_AllConsumers(IDatabaseAsync db)
    {
        string key = $"ser-xpending-all-{Guid.NewGuid()}";
        _ = await db.StreamAddAsync(key, "field", "value1");
        _ = await db.StreamAddAsync(key, "field", "value2");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        _ = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">", count: 1);
        _ = await db.StreamReadGroupAsync(key, "mygroup", "consumer2", ">", count: 1);

        // Query without consumer filter — should return messages from all consumers
        StreamPendingMessageInfo[] messages = await db.StreamPendingMessagesAsync(key, "mygroup", 10, default);
        Assert.Equal(2, messages.Length);
    }

    #endregion
    #region StreamDeleteAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamDeleteAsync_RemovesEntries(IDatabaseAsync db)
    {
        string key = $"ser-xdel-{Guid.NewGuid()}";
        ValkeyValue id1 = await db.StreamAddAsync(key, "field", "value1");
        _ = await db.StreamAddAsync(key, "field", "value2");

        long deleted = await db.StreamDeleteAsync(key, [id1]);
        Assert.Equal(1, deleted);
        Assert.Equal(1, await db.StreamLengthAsync(key));
    }

    #endregion
    #region StreamAcknowledgeAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StreamAcknowledgeAsync_AcksMessage(IDatabaseAsync db)
    {
        string key = $"ser-xack-{Guid.NewGuid()}";
        ValkeyValue id1 = await db.StreamAddAsync(key, "field", "value1");
        _ = await db.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        _ = await db.StreamReadGroupAsync(key, "mygroup", "consumer1", ">", count: 1);

        long acked = await db.StreamAcknowledgeAsync(key, "mygroup", id1);
        Assert.Equal(1, acked);

        StreamPendingInfo pending = await db.StreamPendingAsync(key, "mygroup");
        Assert.Equal(0, pending.PendingMessageCount);
    }

    #endregion
}
