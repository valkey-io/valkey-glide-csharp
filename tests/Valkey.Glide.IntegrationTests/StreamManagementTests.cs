// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(StreamManagementTests))]
[CollectionDefinition(DisableParallelization = true)]
public class StreamManagementTests
{
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
}
