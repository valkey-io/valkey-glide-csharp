// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(StreamBackwardCompatibilityTests))]
[CollectionDefinition(DisableParallelization = true)]
public class StreamBackwardCompatibilityTests
{
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_LegacyOverload_SingleField(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        ValkeyValue messageId = await client.StreamAddAsync(key, "field", "value", null, null, false, CommandFlags.None);
        Assert.False(messageId.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAddAsync_LegacyOverload_MultipleFields(BaseClient client)
    {
        string key = "{StreamAdd}" + Guid.NewGuid();
        NameValueEntry[] entries = [new NameValueEntry("field1", "value1"), new NameValueEntry("field2", "value2")];
        ValkeyValue messageId = await client.StreamAddAsync(key, entries, null, null, false, CommandFlags.None);
        Assert.False(messageId.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamCreateConsumerGroupAsync_LegacyOverload(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();
        await client.StreamAddAsync(key, "field", "value");
        bool created = await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages, CommandFlags.None);
        Assert.True(created);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_LegacyOverload_SingleStream(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();
        await client.StreamAddAsync(key, "field", "value");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);
        StreamEntry[] entries = await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages, null, CommandFlags.None);
        Assert.Single(entries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_LegacyOverload_MultipleStreams(BaseClient client)
    {
        string key1 = "{StreamGroup}" + Guid.NewGuid();
        string key2 = "{StreamGroup}" + Guid.NewGuid();
        await client.StreamAddAsync(key1, "field", "value1");
        await client.StreamAddAsync(key2, "field", "value2");
        await client.StreamCreateConsumerGroupAsync(key1, "mygroup", StreamConstants.AllMessages);
        await client.StreamCreateConsumerGroupAsync(key2, "mygroup", StreamConstants.AllMessages);
        StreamPosition[] positions = [new StreamPosition(key1, StreamConstants.UndeliveredMessages), new StreamPosition(key2, StreamConstants.UndeliveredMessages)];
        ValkeyStream[] streams = await client.StreamReadGroupAsync(positions, "mygroup", "consumer1", null, CommandFlags.None);
        Assert.Equal(2, streams.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_LegacyOverload(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();
        await client.StreamAddAsync(key, "field", "value");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);
        StreamPendingMessageInfo[] messages = await client.StreamPendingMessagesAsync(key, "mygroup", 10, "consumer1", StreamConstants.ReadMinValue, StreamConstants.ReadMaxValue, CommandFlags.None);
        Assert.Single(messages);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamTrimAsync_LegacyOverload(BaseClient client)
    {
        string key = "{StreamTrim}" + Guid.NewGuid();
        await client.StreamAddAsync(key, "field", "value1");
        await client.StreamAddAsync(key, "field", "value2");
        await client.StreamAddAsync(key, "field", "value3");
        long trimmed = await client.StreamTrimAsync(key, 2, false, CommandFlags.None);
        Assert.Equal(1, trimmed);
        long length = await client.StreamLengthAsync(key);
        Assert.Equal(2, length);
    }
}
