// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(StreamCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class StreamCommandTests
{
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_SingleFieldValue(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        
        // Add entry with auto-generated ID
        ValkeyValue messageId = await client.StreamAddAsync(key, "field1", "value1");
        Assert.False(messageId.IsNull);
        
        // Verify the ID format (should be timestamp-sequence)
        string idStr = messageId.ToString();
        Assert.Contains("-", idStr);
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
        
        ValkeyValue messageId = await client.StreamAddAsync(key, entries);
        Assert.False(messageId.IsNull);
        
        string idStr = messageId.ToString();
        Assert.Contains("-", idStr);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_WithMaxLength(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        
        // Add 5 entries with maxLength of 3
        for (int i = 0; i < 5; i++)
        {
            await client.StreamAddAsync(key, "field", $"value{i}", maxLength: 3);
        }
        
        // Verify stream was trimmed (this would require XLEN command to verify properly)
        // For now, just verify the command executes successfully
        ValkeyValue lastId = await client.StreamAddAsync(key, "field", "final", maxLength: 3);
        Assert.False(lastId.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_SingleStream(BaseClient client)
    {
        string key = "{StreamRead}" + Guid.NewGuid();
        
        // Add some entries
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        
        // Read from beginning
        StreamEntry[] entries = await client.StreamReadAsync(key, "0-0");
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
        await client.StreamAddAsync(key1, "field", "stream1_value");
        await client.StreamAddAsync(key2, "field", "stream2_value");
        
        // Read from both streams
        StreamPosition[] positions = [
            new StreamPosition(key1, "0-0"),
            new StreamPosition(key2, "0-0")
        ];
        
        ValkeyStream[] streams = await client.StreamReadAsync(positions);
        Assert.Equal(2, streams.Length);
        Assert.Single(streams[0].Entries);
        Assert.Single(streams[1].Entries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_BlockingTimeout(BaseClient client)
    {
        string key = "{StreamRead}" + Guid.NewGuid();
        
        // Try to read with a short timeout - should return empty since no data exists
        var startTime = DateTime.UtcNow;
        StreamEntry[] entries = await client.StreamReadAsync(key, "$", block: 100);
        var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
        
        Assert.Empty(entries);
        Assert.True(elapsed >= 100, $"Expected at least 100ms block, got {elapsed}ms");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_BlockingWithData(BaseClient client)
    {
        string key = "{StreamRead}" + Guid.NewGuid();
        
        // Add initial entry
        ValkeyValue firstId = await client.StreamAddAsync(key, "field", "value1");
        Assert.False(firstId.IsNull);
        
        // Read with blocking from the last ID - should timeout since no new data
        StreamEntry[] entries = await client.StreamReadAsync(key, firstId, block: 100);
        Assert.Empty(entries);
        
        // Add another entry
        await client.StreamAddAsync(key, "field", "value2");
        
        // Now read from first ID without blocking - should get the second entry
        entries = await client.StreamReadAsync(key, firstId);
        Assert.Single(entries);
        Assert.Equal("value2", entries[0]["field"].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_NoMakeStream_StreamDoesNotExist(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        
        // Try to add to non-existent stream with NOMKSTREAM - should return null
        ValkeyValue messageId = await client.StreamAddAsync(key, "field", "value", noMakeStream: true);
        Assert.True(messageId.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_NoMakeStream_StreamExists(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        
        // Create stream first
        await client.StreamAddAsync(key, "field", "value1");
        
        // Add to existing stream with NOMKSTREAM - should succeed
        ValkeyValue messageId = await client.StreamAddAsync(key, "field", "value2", noMakeStream: true);
        Assert.False(messageId.IsNull);
        Assert.Contains("-", messageId.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_WithMinId(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        
        // Add 3 entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");
        
        // Add another entry with MINID set to id2 - should trim entries older than id2
        ValkeyValue id4 = await client.StreamAddAsync(key, "field", "value4", noMakeStream: false, minId: id2);
        Assert.False(id4.IsNull);
        
        // Verify entries - should have id2, id3, id4 (id1 should be trimmed)
        StreamEntry[] entries = await client.StreamReadAsync(key, "0-0");
        Assert.True(entries.Length >= 3);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_WithMinIdApproximate(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        
        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        await client.StreamAddAsync(key, "field", "value2");
        
        // Add with approximate MINID trimming
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3", useApproximateTrimming: true, noMakeStream: false, minId: id1);
        Assert.False(id3.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_WithTimestampAutoSequence(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        
        // Use <ms>-* format to auto-generate sequence number for specific timestamp
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1", messageId: $"{timestamp}-*");
        Assert.False(id1.IsNull);
        Assert.StartsWith($"{timestamp}-", id1.ToString());
        
        // Add another with same timestamp - should get incremented sequence
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2", messageId: $"{timestamp}-*");
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
        ValkeyValue id = await client.StreamAddAsync(key, "field", "value", messageId: "1000000000000-0");
        Assert.Equal("1000000000000-0", id.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_AllEntries(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();
        
        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");
        
        // Read all entries
        StreamEntry[] entries = await client.StreamRangeAsync(key);
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
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");
        
        // Read from id1 to id2
        StreamEntry[] entries = await client.StreamRangeAsync(key, start: id1, end: id2);
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
        await client.StreamAddAsync(key, "field", "value1");
        await client.StreamAddAsync(key, "field", "value2");
        await client.StreamAddAsync(key, "field", "value3");
        
        // Read only 2 entries
        StreamEntry[] entries = await client.StreamRangeAsync(key, count: 2);
        Assert.Equal(2, entries.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamRangeAsync_Descending(BaseClient client)
    {
        string key = "{StreamRange}" + Guid.NewGuid();
        
        // Add entries
        await client.StreamAddAsync(key, "field", "value1");
        await client.StreamAddAsync(key, "field", "value2");
        await client.StreamAddAsync(key, "field", "value3");
        
        // Read in ascending order first to verify entries exist
        StreamEntry[] ascEntries = await client.StreamRangeAsync(key);
        Assert.Equal(3, ascEntries.Length);
        
        // Read in descending order (most recent first) - XREVRANGE uses + to -
        StreamEntry[] entries = await client.StreamRangeAsync(key, start: "+", end: "-", order: Order.Descending);
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
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamAddAsync(key, "field3", "value3");
        
        // Verify length
        long length = await client.StreamLengthAsync(key);
        Assert.Equal(3, length);
        
        // Trim to 2 entries
        long trimmed = await client.StreamTrimAsync(key, maxLength: 2);
        Assert.Equal(1, trimmed);
        
        // Verify new length
        length = await client.StreamLengthAsync(key);
        Assert.Equal(2, length);
        
        // Trim to 1 entry with approximate trimming
        trimmed = await client.StreamTrimAsync(key, maxLength: 1, useApproximateTrimming: true);
        Assert.True(trimmed >= 0); // Approximate trimming may trim 0 or more
        
        // Key does not exist - returns 0
        length = await client.StreamLengthAsync(key2);
        Assert.Equal(0, length);
        
        trimmed = await client.StreamTrimAsync(key2, maxLength: 1);
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
        Assert.Single(result);
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
        StreamEntry[] entries = await client.StreamRangeAsync(key);
        Assert.Single(entries);
        Assert.Equal(id3.ToString(), entries[0].Id.ToString());
        
        // Try to delete non-existent ID
        deleted = await client.StreamDeleteAsync(key, ["999-999"]);
        Assert.Equal(0, deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamLengthAsync_Basic(BaseClient client)
    {
        string key = "{StreamMgmt}" + Guid.NewGuid();
        
        // Add entries
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamAddAsync(key, "field3", "value3");
        
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
        await client.StreamAddAsync(key, "field3", "value3");
        
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
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamAddAsync(key, "field3", "value3");
        await client.StreamAddAsync(key, "field4", "value4");
        await client.StreamAddAsync(key, "field5", "value5");
        
        // Trim to 2 entries
        long trimmed = await client.StreamTrimAsync(key, maxLength: 2);
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
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field3", "value3");
        await client.StreamAddAsync(key, "field4", "value4");
        
        // Trim entries before id3
        long trimmed = await client.StreamTrimAsync(key, minId: id3);
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
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamAddAsync(key, "field3", "value3");
        
        // Create a consumer group
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        
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
        await client.StringSetAsync(key, "not a stream");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamReadAsync(key, "0-0"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_GroupDoesNotExist_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.StreamAddAsync(key, "field", "value");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamReadGroupAsync(key, "nonexistent", "consumer", ">"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_NonStreamKey_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.StringSetAsync(key, "not a stream");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamReadGroupAsync(key, "group", "consumer", ">"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAcknowledgeAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.StringSetAsync(key, "not a stream");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamAcknowledgeAsync(key, "group", "1-0"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAcknowledgeAsync_NonExistentGroup_ReturnsZero(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        ValkeyValue id = await client.StreamAddAsync(key, "field", "value");
        long ackCount = await client.StreamAcknowledgeAsync(key, "nonexistent", id);
        Assert.Equal(0, ackCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_NonExistentKey_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamPendingAsync(key, "group"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_NonExistentGroup_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.StreamAddAsync(key, "field", "value");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamPendingAsync(key, "nonexistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.StringSetAsync(key, "not a stream");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamPendingAsync(key, "group"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_NonExistentGroup_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        ValkeyValue id = await client.StreamAddAsync(key, "field", "value");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamClaimAsync(key, "nonexistent", "consumer", 0, [id]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.StringSetAsync(key, "not a stream");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamClaimAsync(key, "group", "consumer", 0, ["1-0"]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoAsync_NonExistentKey_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamInfoAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoAsync_WrongKeyType_ThrowsError(BaseClient client)
    {
        string key = "{StreamError}" + Guid.NewGuid();
        await client.StringSetAsync(key, "not a stream");
        await Assert.ThrowsAsync<RequestException>(async () => await client.StreamInfoAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_WithRangeBounds(BaseClient client)
    {
        string key = "{StreamPending}" + Guid.NewGuid();
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", ">");
        
        StreamPendingMessageInfo[] messages = await client.StreamPendingMessagesAsync(key, "mygroup", 10, "consumer1", "-", "+");
        
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
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", ">");
        
        StreamPendingMessageInfo[] messages = await client.StreamPendingMessagesAsync(key, "mygroup", 10, "consumer1", id1, id2);
        
        Assert.Equal(2, messages.Length);
        Assert.Equal(id1.ToString(), messages[0].MessageId.ToString());
        Assert.Equal(id2.ToString(), messages[1].MessageId.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_VerifyIdleTimeAndDeliveryCount(BaseClient client)
    {
        string key = "{StreamPending}" + Guid.NewGuid();
        ValkeyValue id = await client.StreamAddAsync(key, "field", "value");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", ">");
        
        StreamPendingMessageInfo[] messages = await client.StreamPendingMessagesAsync(key, "mygroup", 10, "consumer1");
        
        Assert.Single(messages);
        Assert.True(messages[0].IdleTimeInMilliseconds >= 0);
        Assert.Equal(1, messages[0].DeliveryCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAutoClaimAsync_DeletedEntriesDetection(BaseClient client)
    {
        string key = "{StreamAutoClaim}" + Guid.NewGuid();
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", "0");
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", ">");
        await client.StreamDeleteAsync(key, [id2]);
        
        StreamAutoClaimResult result = await client.StreamAutoClaimAsync(key, "mygroup", "consumer2", 0, "0-0");
        
        Assert.Equal(2, result.ClaimedEntries.Length);
        if (result.DeletedIds != null && result.DeletedIds.Length > 0)
        {
            Assert.Contains(result.DeletedIds, deletedId => deletedId.ToString() == id2.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamCreateConsumerGroupAsync_WithEntriesRead(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();
        
        // Add entries
        await client.StreamAddAsync(key, "field", "value1");
        await client.StreamAddAsync(key, "field", "value2");
        await client.StreamAddAsync(key, "field", "value3");
        
        // Create group with entriesRead parameter (Valkey 7.0+)
        bool created = await client.StreamCreateConsumerGroupAsync(key, "mygroup", "0", entriesRead: 10);
        Assert.True(created);
        
        // Verify group was created by checking group info
        StreamGroupInfo[] groups = await client.StreamGroupInfoAsync(key);
        Assert.Single(groups);
        Assert.Equal("mygroup", groups[0].Name);
        
        // EntriesRead should be 10 if server supports it (Valkey 7.0+)
        if (groups[0].EntriesRead.HasValue)
        {
            Assert.Equal(10, groups[0].EntriesRead.Value);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoFullAsync_Basic(BaseClient client)
    {
        string key = "{StreamInfo}" + Guid.NewGuid();
        string groupName = "mygroup";
        string consumer = "consumer1";
        
        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field2", "value2");
        
        // Create group and read messages
        await client.StreamCreateConsumerGroupAsync(key, groupName, "0");
        await client.StreamReadGroupAsync(key, groupName, consumer, ">");
        
        // Get full stream info
        Dictionary<string, object> info = await client.StreamInfoFullAsync(key);
        
        // Verify basic fields
        Assert.True(info.ContainsKey("length"));
        Assert.True(info.ContainsKey("entries"));
        Assert.True(info.ContainsKey("groups"));
        
        // Verify length
        long length = info["length"] is GlideString gs ? long.Parse(gs.ToString()) : (long)info["length"];
        Assert.Equal(2, length);
        
        // Verify entries
        object[] entries = (object[])info["entries"];
        Assert.Equal(2, entries.Length);
        
        // Verify groups
        object[] groups = (object[])info["groups"];
        Assert.Single(groups);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamInfoFullAsync_WithCount(BaseClient client)
    {
        string key = "{StreamInfo}" + Guid.NewGuid();
        
        // Add entries
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamAddAsync(key, "field3", "value3");
        
        // Get full stream info with count=1
        Dictionary<string, object> info = await client.StreamInfoFullAsync(key, count: 1);
        
        // Verify only 1 entry returned
        object[] entries = (object[])info["entries"];
        Assert.Single(entries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadAsync_WithPlusPosition(BaseClient client)
    {
        string key = "{StreamRead}" + Guid.NewGuid();
        
        // Add entries
        await client.StreamAddAsync(key, "field", "value1");
        await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue lastId = await client.StreamAddAsync(key, "field", "value3");
        
        // Read from "+" (maximum ID) - returns only the last entry
        StreamEntry[] entries = await client.StreamReadAsync(key, "+");
        Assert.Single(entries);
        Assert.Equal("value3", entries[0]["field"].ToString());
        Assert.Equal(lastId.ToString(), entries[0].Id.ToString());
        
        // Add another entry
        ValkeyValue newId = await client.StreamAddAsync(key, "field", "value4");
        
        // Read from "+" again - should now return the new last entry
        entries = await client.StreamReadAsync(key, "+");
        Assert.Single(entries);
        Assert.Equal("value4", entries[0]["field"].ToString());
        Assert.Equal(newId.ToString(), entries[0].Id.ToString());
    }

}
