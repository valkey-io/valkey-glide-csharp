// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(StreamCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class StreamCommandTests
{
    private static void AssertIsValidMessageId(ValkeyValue messageId)
    {
        Assert.False(messageId.IsNull);
        Assert.Contains("-", messageId.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_SingleFieldValue(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();

        // Add entry with auto-generated ID
        var messageId = await client.StreamAddAsync(key, "field1", "value1");
        AssertIsValidMessageId(messageId);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_MultipleFieldValues(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();

        NameValueEntry[] entries = [
            new NameValueEntry("field1", "value1"),
            new NameValueEntry("field2", "value2"),
            new NameValueEntry("field3", "value3")
        ];

        var messageId = await client.StreamAddAsync(key, entries);
        AssertIsValidMessageId(messageId);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_WithMaxLength(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();

        // Add 5 entries with maxLength of 3
        for (int i = 0; i < 5; i++)
        {
            _ = await client.StreamAddAsync(key, "field", $"value{i}", new StreamAddOptions { Trim = new StreamTrimOptions.MaxLen { MaxLength = 3 } });
        }

        // Verify stream was trimmed to maxLength of 3
        long length = await client.StreamLengthAsync(key);
        Assert.Equal(3, length);

        // Add another entry with maxLength of 3
        ValkeyValue lastId = await client.StreamAddAsync(key, "field", "final", new StreamAddOptions { Trim = new StreamTrimOptions.MaxLen { MaxLength = 3 } });
        AssertIsValidMessageId(lastId);

        // Verify stream is still trimmed to 3
        length = await client.StreamLengthAsync(key);
        Assert.Equal(3, length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_SingleStream(BaseClient client)
    {
        string key = "{StreamRead}" + Guid.NewGuid();

        // Add some entries
        _ = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");

        // Read from beginning
        var entries = await client.StreamReadAsync(new StreamPosition(key, StreamPosition.Beginning));
        Assert.Equal(2, entries.Length);
        Assert.Equal("value1", entries[0]["field1"].ToString());
        Assert.Equal("value2", entries[1]["field2"].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_MultipleStreams(BaseClient client)
    {
        string key1 = "{StreamRead}" + Guid.NewGuid();
        string key2 = "{StreamRead}" + Guid.NewGuid();

        // Add entries to both streams
        _ = await client.StreamAddAsync(key1, "field", "stream1_value");
        _ = await client.StreamAddAsync(key2, "field", "stream2_value");

        // Read from both streams
        StreamPosition[] positions = [
            new StreamPosition(key1, StreamPosition.Beginning),
            new StreamPosition(key2, StreamPosition.Beginning)
        ];

        ValkeyStream[] streams = await client.StreamReadAsync(positions);
        Assert.Equal(2, streams.Length);
        _ = Assert.Single(streams[0].Entries);
        _ = Assert.Single(streams[1].Entries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_NoMakeStream_StreamDoesNotExist(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();

        // Try to add to non-existent stream with NOMKSTREAM - should return null
        var options = new StreamAddOptions { MakeStream = false };
        var messageId = await client.StreamAddAsync(key, "field", "value", options);
        Assert.True(messageId.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_NoMakeStream_StreamExists(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();

        // Create stream first
        _ = await client.StreamAddAsync(key, "field", "value1");

        // Add to existing stream with NOMKSTREAM - should succeed
        var options = new StreamAddOptions { MakeStream = false };
        var messageId = await client.StreamAddAsync(key, "field", "value2", options);
        AssertIsValidMessageId(messageId);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamTrimAsync_ByMinId(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();

        // Add 3 entries
        _ = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        _ = await client.StreamAddAsync(key, "field", "value3");

        // Add another entry and trim by minId separately
        ValkeyValue id4 = await client.StreamAddAsync(key, "field", "value4");
        Assert.False(id4.IsNull);

        // Trim entries older than id2
        _ = await client.StreamTrimAsync(key, new StreamTrimOptions.MinId { MinEntryId = id2 });

        // Verify entries - should have exactly id2, id3, id4 (id1 should be trimmed)
        var entries = await client.StreamReadAsync(new StreamPosition(key, StreamPosition.Beginning));
        Assert.Equal(3, entries.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamTrimAsync_ByMinIdApproximate(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();

        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        _ = await client.StreamAddAsync(key, "field", "value2");

        // Add entry and trim with approximate MINID trimming separately
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");
        Assert.False(id3.IsNull);

        // Trim with approximate minId
        _ = await client.StreamTrimAsync(key, new StreamTrimOptions.MinId { MinEntryId = id1, Exact = false });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_WithTimestampAutoSequence(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("7.0.0"), "Timestamp-* format requires server version 7.0.0 or higher");

        string key = "{StreamAdd}" + Guid.NewGuid();

        // Use <ms>-* format to auto-generate sequence number for specific timestamp
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1", new StreamAddOptions { Id = $"{timestamp}-*" });
        Assert.False(id1.IsNull);
        Assert.StartsWith($"{timestamp}-", id1.ToString());

        // Add another with same timestamp - should get incremented sequence
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2", new StreamAddOptions { Id = $"{timestamp}-*" });
        Assert.False(id2.IsNull);
        Assert.StartsWith($"{timestamp}-", id2.ToString());

        // Verify IDs are different (different sequences)
        Assert.NotEqual(id1.ToString(), id2.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_WithExplicitId(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();

        // Add with explicit ID
        ValkeyValue id = await client.StreamAddAsync(key, "field", "value", new StreamAddOptions { Id = "1000000000000-0" });
        Assert.Equal("1000000000000-0", id.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_AllEntries(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field", "value1");
        _ = await client.StreamAddAsync(key, "field", "value2");
        _ = await client.StreamAddAsync(key, "field", "value3");

        // Read all entries
        var entries = await client.StreamRangeAsync(key);
        Assert.Equal(3, entries.Length);
        Assert.Equal("value1", entries[0]["field"].ToString());
        Assert.Equal("value2", entries[1]["field"].ToString());
        Assert.Equal("value3", entries[2]["field"].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_WithRange(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();

        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        _ = await client.StreamAddAsync(key, "field", "value3");

        // Read from id1 to id2
        var entries = await client.StreamRangeAsync(key, new StreamRangeOptions { Range = StreamIdRange.Between(id1, id2) });
        Assert.Equal(2, entries.Length);
        Assert.Equal(id1.ToString(), entries[0].Id.ToString());
        Assert.Equal(id2.ToString(), entries[1].Id.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_WithCount(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field", "value1");
        _ = await client.StreamAddAsync(key, "field", "value2");
        _ = await client.StreamAddAsync(key, "field", "value3");

        // Read only 2 entries
        var entries = await client.StreamRangeAsync(key, new StreamRangeOptions { Count = 2 });
        Assert.Equal(2, entries.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_Descending(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field", "value1");
        _ = await client.StreamAddAsync(key, "field", "value2");
        _ = await client.StreamAddAsync(key, "field", "value3");

        // Read in ascending order first to verify entries exist
        var ascEntries = await client.StreamRangeAsync(key);
        Assert.Equal(3, ascEntries.Length);

        // Read in descending order (most recent first) - library swaps minId/maxId for XREVRANGE
        var entries = await client.StreamRangeAsync(key, new StreamRangeOptions { Order = Order.Descending });
        Assert.Equal(3, entries.Length);
        Assert.Equal("value3", entries[0]["field"].ToString());
        Assert.Equal("value2", entries[1]["field"].ToString());
        Assert.Equal("value1", entries[2]["field"].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamLengthAsync_And_StreamTrimAsync(BaseClient client)
    {
        string key = "{StreamLen}" + Guid.NewGuid();
        string key2 = "{StreamLen}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");
        _ = await client.StreamAddAsync(key, "field3", "value3");

        // Verify length
        long length = await client.StreamLengthAsync(key);
        Assert.Equal(3, length);

        // Trim to 2 entries
        long trimmed = await client.StreamTrimAsync(key, new StreamTrimOptions.MaxLen { MaxLength = 2 });
        Assert.Equal(1, trimmed);

        // Verify new length
        length = await client.StreamLengthAsync(key);
        Assert.Equal(2, length);

        // Trim to 1 entry with approximate trimming
        trimmed = await client.StreamTrimAsync(key, new StreamTrimOptions.MaxLen { MaxLength = 1, Exact = false });
        Assert.True(trimmed >= 0); // Approximate trimming may trim 0 or more

        // Verify new length is consistent with trimmed count
        length = await client.StreamLengthAsync(key);
        Assert.Equal(2, length + trimmed);

        // Key does not exist - returns 0
        length = await client.StreamLengthAsync(key2);
        Assert.Equal(0, length);

        trimmed = await client.StreamTrimAsync(key2, new StreamTrimOptions.MaxLen { MaxLength = 1 });
        Assert.Equal(0, trimmed);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_DuplicateFieldNames(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        string field = "myfield";

        // Add entry with duplicate field names
        NameValueEntry[] entries = [
            new NameValueEntry(field, "value1"),
            new NameValueEntry(field, "value2")
        ];

        ValkeyValue streamId = await client.StreamAddAsync(key, entries);
        Assert.False(streamId.IsNull);

        // Read back - entry should exist
        StreamEntry[] result = await client.StreamRangeAsync(key);
        _ = Assert.Single(result);
        Assert.Equal(streamId.ToString(), result[0].Id.ToString());

        // Verify that duplicate fields are preserved
        Assert.Equal(2, result[0].Values.Length);
        Assert.Equal(field, result[0].Values[0].Name.ToString());
        Assert.Equal("value1", result[0].Values[0].Value.ToString());
        Assert.Equal(field, result[0].Values[1].Name.ToString());
        Assert.Equal("value2", result[0].Values[1].Value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamDeleteAsync_Standalone(BaseClient client)
    {
        string key = "{StreamDel}" + Guid.NewGuid();

        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");

        // Delete two entries
        long deleted = await client.StreamDeleteAsync(key, [id1, id2]);
        Assert.Equal(2, deleted);

        // Verify only one entry remains
        var entries = await client.StreamRangeAsync(key);
        _ = Assert.Single(entries);
        Assert.Equal(id3.ToString(), entries[0].Id.ToString());

        // Try to delete non-existent ID
        deleted = await client.StreamDeleteAsync(key, ["999-999"]);
        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamDeleteAsync_SingleId(BaseClient client)
    {
        string key = "{StreamDel}" + Guid.NewGuid();

        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        _ = await client.StreamAddAsync(key, "field", "value2");

        // Delete single entry
        bool deleted = await client.StreamDeleteAsync(key, id1);
        Assert.True(deleted);
        Assert.Equal(1, await client.StreamLengthAsync(key));

        // Try to delete non-existent ID
        deleted = await client.StreamDeleteAsync(key, "999-999");
        Assert.False(deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamLengthAsync_Basic(BaseClient client)
    {
        string key = "{StreamMgmt}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");
        _ = await client.StreamAddAsync(key, "field3", "value3");

        // Get length
        long length = await client.StreamLengthAsync(key);
        Assert.Equal(3, length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamDeleteAsync_Basic(BaseClient client)
    {
        string key = "{StreamMgmt}" + Guid.NewGuid();

        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field2", "value2");
        _ = await client.StreamAddAsync(key, "field3", "value3");

        // Delete two entries
        long deleted = await client.StreamDeleteAsync(key, [id1, id2]);
        Assert.Equal(2, deleted);

        // Verify length
        long length = await client.StreamLengthAsync(key);
        Assert.Equal(1, length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamTrimAsync_MaxLength(BaseClient client)
    {
        string key = "{StreamMgmt}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");
        _ = await client.StreamAddAsync(key, "field3", "value3");
        _ = await client.StreamAddAsync(key, "field4", "value4");
        _ = await client.StreamAddAsync(key, "field5", "value5");

        // Trim to 2 entries
        long trimmed = await client.StreamTrimAsync(key, new StreamTrimOptions.MaxLen { MaxLength = 2 });
        Assert.Equal(3, trimmed);

        // Verify length
        long length = await client.StreamLengthAsync(key);
        Assert.Equal(2, length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamTrimAsync_MinId(BaseClient client)
    {
        string key = "{StreamMgmt}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field3", "value3");
        _ = await client.StreamAddAsync(key, "field4", "value4");

        // Trim entries before id3
        long trimmed = await client.StreamTrimAsync(key, new StreamTrimOptions.MinId { MinEntryId = id3 });
        Assert.Equal(2, trimmed);

        // Verify length
        long length = await client.StreamLengthAsync(key);
        Assert.Equal(2, length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoAsync_Basic(BaseClient client)
    {
        string key = "{StreamMgmt}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");
        _ = await client.StreamAddAsync(key, "field3", "value3");

        // Create a consumer group
        await client.StreamGroupCreateAsync(key, "mygroup", "0");

        // Get stream info
        StreamInfo info = await client.StreamInfoAsync(key);
        Assert.Equal(3, info.Length);
        Assert.Equal(1, info.ConsumerGroupCount);
        Assert.False(info.LastGeneratedId.IsNull);
        Assert.Equal("value1", info.FirstEntry.Values[0].Value.ToString());
        Assert.Equal("value3", info.LastEntry.Values[0].Value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_NonStreamKey_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.SetAsync(key, "not a stream");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamReadAsync(new StreamPosition(key, StreamPosition.Beginning)));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_GroupDoesNotExist_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        _ = await client.StreamAddAsync(key, "field", "value");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamReadGroupAsync(new StreamPosition(key, StreamPosition.UndeliveredMessages), "nonexistent", "consumer"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_NonStreamKey_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.SetAsync(key, "not a stream");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamReadGroupAsync(new StreamPosition(key, StreamPosition.UndeliveredMessages), "group", "consumer"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAcknowledgeAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.SetAsync(key, "not a stream");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamAcknowledgeAsync(key, "group", "1-0"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAcknowledgeAsync_NonExistentGroup_ReturnsFalse(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        ValkeyValue id = await client.StreamAddAsync(key, "field", "value");
        bool acknowledged = await client.StreamAcknowledgeAsync(key, "nonexistent", id);
        Assert.False(acknowledged);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_NonExistentKey_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamPendingAsync(key, "group"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_NonExistentGroup_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        _ = await client.StreamAddAsync(key, "field", "value");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamPendingAsync(key, "nonexistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.SetAsync(key, "not a stream");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamPendingAsync(key, "group"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_NonExistentGroup_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        ValkeyValue id = await client.StreamAddAsync(key, "field", "value");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamClaimAsync(key, "nonexistent", "consumer", [id], StreamClaimOptions.From(TimeSpan.Zero)));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.SetAsync(key, "not a stream");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamClaimAsync(key, "group", "consumer", ["1-0"], StreamClaimOptions.From(TimeSpan.Zero)));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoAsync_NonExistentKey_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamInfoAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.SetAsync(key, "not a stream");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamInfoAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_WithRangeBounds(BaseClient client)
    {
        string key = "{StreamPending}" + Guid.NewGuid();
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");
        await client.StreamGroupCreateAsync(key, "mygroup", "0");
        _ = await client.StreamReadGroupAsync(new StreamPosition(key, StreamPosition.UndeliveredMessages), "mygroup", "consumer1");

        var messages = await client.StreamPendingAsync(key, "mygroup", new StreamPendingOptions { Count = 10, ConsumerName = "consumer1" });

        Assert.Equal(3, messages.Length);
        Assert.Equal(id1.ToString(), messages[0].MessageId.ToString());
        Assert.Equal(id2.ToString(), messages[1].MessageId.ToString());
        Assert.Equal(id3.ToString(), messages[2].MessageId.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_WithSpecificRange(BaseClient client)
    {
        string key = "{StreamPending}" + Guid.NewGuid();
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        _ = await client.StreamAddAsync(key, "field", "value3");
        await client.StreamGroupCreateAsync(key, "mygroup", "0");
        _ = await client.StreamReadGroupAsync(new StreamPosition(key, StreamPosition.UndeliveredMessages), "mygroup", "consumer1");

        StreamPendingMessageInfo[] messages = await client.StreamPendingAsync(key, "mygroup", new StreamPendingOptions
        {
            Count = 10,
            ConsumerName = "consumer1",
            Start = StreamIdBound.Inclusive(id1),
            End = StreamIdBound.Inclusive(id2),
        });

        Assert.Equal(2, messages.Length);
        Assert.Equal(id1.ToString(), messages[0].MessageId.ToString());
        Assert.Equal(id2.ToString(), messages[1].MessageId.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_VerifyIdleTimeAndDeliveryCount(BaseClient client)
    {
        string key = "{StreamPending}" + Guid.NewGuid();
        _ = await client.StreamAddAsync(key, "field", "value");
        await client.StreamGroupCreateAsync(key, "mygroup", "0");
        _ = await client.StreamReadGroupAsync(new StreamPosition(key, StreamPosition.UndeliveredMessages), "mygroup", "consumer1");

        StreamPendingMessageInfo[] messages = await client.StreamPendingAsync(key, "mygroup", new StreamPendingOptions { Count = 10, ConsumerName = "consumer1" });

        _ = Assert.Single(messages);
        Assert.True(messages[0].IdleTimeInMilliseconds >= 0);
        Assert.Equal(1, messages[0].DeliveryCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAutoClaimAsync_DeletedEntriesDetection(BaseClient client)
    {
        string key = "{StreamAutoClaim}" + Guid.NewGuid();
        _ = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        _ = await client.StreamAddAsync(key, "field", "value3");
        await client.StreamGroupCreateAsync(key, "mygroup", "0");
        _ = await client.StreamReadGroupAsync(new StreamPosition(key, StreamPosition.UndeliveredMessages), "mygroup", "consumer1");
        _ = await client.StreamDeleteAsync(key, [id2]);

        StreamAutoClaimResult result = await client.StreamAutoClaimAsync(key, "mygroup", "consumer2", StreamAutoClaimOptions.FromStart(TimeSpan.Zero));

        Assert.Equal(2, result.ClaimedEntries.Length);
        Assert.Contains(id2, result.DeletedIds.ToHashSet());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamGroupCreateAsync_WithEntriesRead(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("7.0.0"), "ENTRIESREAD parameter requires server version 7.0.0 or higher");

        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field", "value1");
        _ = await client.StreamAddAsync(key, "field", "value2");
        _ = await client.StreamAddAsync(key, "field", "value3");

        // Create group with entriesRead parameter (Valkey 7.0+)
        await client.StreamGroupCreateAsync(key, "mygroup", "0", new StreamGroupCreateOptions { EntriesRead = 10 });

        // Verify group was created by checking group info
        StreamGroupInfo[] groups = await client.StreamInfoGroupsAsync(key);
        _ = Assert.Single(groups);
        Assert.Equal("mygroup", groups[0].Name);
        Assert.Equal(10L, groups[0].EntriesRead);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_WithPlusPosition(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("7.0.0"), "+ position in XREAD requires server version 7.0.0 or higher");

        string key = "{StreamRead}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field", "value1");
        _ = await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue lastId = await client.StreamAddAsync(key, "field", "value3");

        // Read from "+" (maximum ID) - returns only the last entry
        var entries = await client.StreamReadAsync(new StreamPosition(key, "+"));
        _ = Assert.Single(entries);
        Assert.Equal("value3", entries[0]["field"].ToString());
        Assert.Equal(lastId.ToString(), entries[0].Id.ToString());

        // Add another entry
        ValkeyValue newId = await client.StreamAddAsync(key, "field", "value4");

        // Read from "+" again - should now return the new last entry
        entries = await client.StreamReadAsync(new StreamPosition(key, "+"));
        _ = Assert.Single(entries);
        Assert.Equal("value4", entries[0]["field"].ToString());
        Assert.Equal(newId.ToString(), entries[0].Id.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_EmptyStream(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();
        _ = await client.StreamAddAsync(key, "field", "value");
        _ = await client.StreamTrimAsync(key, new StreamTrimOptions.MaxLen { MaxLength = 0 });

        var entries = await client.StreamRangeAsync(key);
        Assert.Empty(entries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_NoNewEntries(BaseClient client)
    {
        string key = "{StreamRead}" + Guid.NewGuid();
        ValkeyValue id = await client.StreamAddAsync(key, "field", "value");

        var entries = await client.StreamReadAsync(new StreamPosition(key, id));
        Assert.Empty(entries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_MultipleStreams_EmptyResult(BaseClient client)
    {
        string key1 = "{StreamRead}" + Guid.NewGuid();
        string key2 = "{StreamRead}" + Guid.NewGuid();
        ValkeyValue id1 = await client.StreamAddAsync(key1, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key2, "field", "value2");

        StreamPosition[] positions = [new StreamPosition(key1, id1), new StreamPosition(key2, id2)];
        ValkeyStream[] streams = await client.StreamReadAsync(positions);
        Assert.Empty(streams);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_MultipleFieldsPerEntry(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();
        NameValueEntry[] entries = [
            new NameValueEntry("field1", "value1"),
            new NameValueEntry("field2", "value2"),
            new NameValueEntry("field3", "value3")
        ];
        _ = await client.StreamAddAsync(key, entries);

        StreamEntry[] result = await client.StreamRangeAsync(key);
        _ = Assert.Single(result);
        Assert.Equal(3, result[0].Values.Length);
        Assert.Equal("value1", result[0]["field1"].ToString());
        Assert.Equal("value2", result[0]["field2"].ToString());
        Assert.Equal("value3", result[0]["field3"].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_DuplicateFieldNames(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();
        NameValueEntry[] entries = [
            new NameValueEntry("field", "value1"),
            new NameValueEntry("field", "value2"),
            new NameValueEntry("field", "value3")
        ];
        _ = await client.StreamAddAsync(key, entries);

        StreamEntry[] result = await client.StreamRangeAsync(key);
        _ = Assert.Single(result);
        Assert.Equal(3, result[0].Values.Length);
        Assert.Equal("field", result[0].Values[0].Name.ToString());
        Assert.Equal("value1", result[0].Values[0].Value.ToString());
        Assert.Equal("field", result[0].Values[1].Name.ToString());
        Assert.Equal("value2", result[0].Values[1].Value.ToString());
        Assert.Equal("field", result[0].Values[2].Name.ToString());
        Assert.Equal("value3", result[0].Values[2].Value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoFullAsync_Basic(BaseClient client)
    {
        string key = "{StreamInfoFull}" + Guid.NewGuid();

        // Add entries
        _ = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");
        _ = await client.StreamAddAsync(key, "field3", "value3");

        // Get full stream info
        StreamInfoFull info = await client.StreamInfoFullAsync(key);
        Assert.Equal(3, info.Length);
        Assert.True(info.RadixTreeKeys > 0);
        Assert.True(info.RadixTreeNodes > 0);
        Assert.False(info.LastGeneratedId.IsNull);
        Assert.Equal(3, info.Entries.Length);
        Assert.Equal("value1", info.Entries[0]["field1"].ToString());
        Assert.Equal("value3", info.Entries[2]["field3"].ToString());
        Assert.Empty(info.Groups);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoFullAsync_WithConsumerGroup(BaseClient client)
    {
        string key = "{StreamInfoFull}" + Guid.NewGuid();

        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");

        // Create a consumer group and read messages to create pending entries
        await client.StreamGroupCreateAsync(key, "mygroup", "0");
        _ = await client.StreamReadGroupAsync(new StreamPosition(key, StreamPosition.UndeliveredMessages), "mygroup", "consumer1");

        // Get full stream info
        StreamInfoFull info = await client.StreamInfoFullAsync(key);
        Assert.Equal(2, info.Length);
        Assert.Equal(2, info.Entries.Length);

        // Verify group info
        _ = Assert.Single(info.Groups);
        StreamGroupInfoFull group = info.Groups[0];
        Assert.Equal("mygroup", group.Name);
        Assert.False(group.LastDeliveredId.IsNull);
        Assert.Equal(2, group.PelCount);

        // Verify group-level PEL
        Assert.Equal(2, group.PendingEntries.Length);
        Assert.Equal(id1.ToString(), group.PendingEntries[0].EntryId.ToString());
        Assert.Equal("consumer1", group.PendingEntries[0].Consumer);
        Assert.True(group.PendingEntries[0].DeliveryTime > DateTimeOffset.UnixEpoch);
        Assert.Equal(1, group.PendingEntries[0].DeliveryCount);

        // Verify consumer info
        _ = Assert.Single(group.Consumers);
        StreamConsumerInfoFull consumer = group.Consumers[0];
        Assert.Equal("consumer1", consumer.Name);
        Assert.True(consumer.SeenTime > DateTimeOffset.UnixEpoch);
        Assert.Equal(2, consumer.PelCount);

        // Verify consumer-level PEL
        Assert.Equal(2, consumer.PendingEntries.Length);
        Assert.True(consumer.PendingEntries[0].DeliveryTime > DateTimeOffset.UnixEpoch);
        Assert.Equal(1, consumer.PendingEntries[0].DeliveryCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoFullAsync_WithCount(BaseClient client)
    {
        string key = "{StreamInfoFull}" + Guid.NewGuid();

        // Add entries and create group with pending messages
        _ = await client.StreamAddAsync(key, "field1", "value1");
        _ = await client.StreamAddAsync(key, "field2", "value2");
        _ = await client.StreamAddAsync(key, "field3", "value3");
        await client.StreamGroupCreateAsync(key, "mygroup", "0");
        _ = await client.StreamReadGroupAsync(new StreamPosition(key, StreamPosition.UndeliveredMessages), "mygroup", "consumer1");

        // Get full stream info with count=1 to limit PEL entries
        StreamInfoFull info = await client.StreamInfoFullAsync(key, count: 1);
        Assert.Equal(3, info.Length);

        // PEL entries should be limited by count
        var group = Assert.Single(info.Groups);
        _ = Assert.Single(group.PendingEntries);

        var consumer = Assert.Single(group.Consumers);
        _ = Assert.Single(consumer.PendingEntries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoFullAsync_NonExistentKey_ThrowsError(BaseClient client)
    {
        string key = "{StreamInfoFull}" + Guid.NewGuid();
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamInfoFullAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoFullAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamInfoFull}" + Guid.NewGuid();
        await client.SetAsync(key, "not a stream");
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.StreamInfoFullAsync(key));
    }
}
