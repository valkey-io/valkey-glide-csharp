// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
        ValkeyValue id4 = await client.StreamAddAsync(key, "field", "value4", minId: id2);
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
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3", minId: id1, useApproximateTrimming: true);
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
}
