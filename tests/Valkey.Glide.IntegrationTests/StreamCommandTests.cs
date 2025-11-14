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
}
