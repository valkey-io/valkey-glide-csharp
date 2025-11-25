// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(StreamConsumerGroupTests))]
[CollectionDefinition(DisableParallelization = true)]
public class StreamConsumerGroupTests
{
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamCreateConsumerGroupAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add an entry first
        await client.StreamAddAsync(key, "field1", "value1");

        // Create consumer group
        bool created = await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);
        Assert.True(created);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamCreateConsumerGroupAsync_NonExistentStreamWithoutMkstream(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Try to create group on non-existent stream without MKSTREAM - should error
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages, createStream: false));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamCreateConsumerGroupAsync_DuplicateGroup(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Create stream and group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Try to create same group again - should error with BUSYGROUP
        var exception = await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages));
        Assert.Contains("BUSYGROUP", exception.Message);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamCreateConsumerGroupAsync_WrongKeyType(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Set key as string
        await client.StringSetAsync(key, "not_a_stream");

        // Try to create group on string key - should error
        var exception = await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages, createStream: true));
        Assert.Contains("WRONGTYPE", exception.Message);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamCreateConsumerGroupAsync_WithMkstream(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Create group with MKSTREAM (stream doesn't exist yet)
        bool created = await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.NewMessages, createStream: true);
        Assert.True(created);

        // Verify stream was created by adding an entry
        ValkeyValue id = await client.StreamAddAsync(key, "field1", "value1");
        Assert.False(id.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamDeleteConsumerGroupAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Delete the group
        bool deleted = await client.StreamDeleteConsumerGroupAsync(key, "mygroup");
        Assert.True(deleted);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamDeleteConsumerGroupAsync_WrongKeyType(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Set key as string
        await client.StringSetAsync(key, "not_a_stream");

        // Try to delete group on string key - should error
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamDeleteConsumerGroupAsync(key, "mygroup"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamConsumerGroupSetPositionAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field2", "value2");

        // Create group starting from beginning
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Set position to second entry
        bool result = await client.StreamConsumerGroupSetPositionAsync(key, "mygroup", id2);
        Assert.True(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamConsumerGroupSetPositionAsync_ToEnd(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");

        // Create group
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Set position to end ($)
        bool result = await client.StreamConsumerGroupSetPositionAsync(key, "mygroup", StreamConstants.NewMessages);
        Assert.True(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamDeleteConsumerAsync_NonExistentConsumer(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Delete non-existent consumer - should return 0
        long pendingCount = await client.StreamDeleteConsumerAsync(key, "mygroup", "nonexistent");
        Assert.Equal(0, pendingCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_NewMessages(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Read new messages with >
        StreamEntry[] entries = await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);
        Assert.Equal(2, entries.Length);
        Assert.Equal("value1", entries[0].Values[0].Value.ToString());
        Assert.Equal("value2", entries[1].Values[0].Value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_WithCount(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamAddAsync(key, "field3", "value3");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Read only 2 messages
        StreamEntry[] entries = await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages, count: 2);
        Assert.Equal(2, entries.Length);
        Assert.Equal("value1", entries[0].Values[0].Value.ToString());
        Assert.Equal("value2", entries[1].Values[0].Value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_NoGroup(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry but don't create group
        await client.StreamAddAsync(key, "field1", "value1");

        // Try to read from non-existent group - should error with NOGROUP
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamReadGroupAsync(key, "nonexistent", "consumer1", StreamConstants.UndeliveredMessages));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_WrongKeyType(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Set key as string
        await client.StringSetAsync(key, "not_a_stream");

        // Try to read from string key - should error
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamReadGroupAsync_MultiStream(BaseClient client)
    {
        string key1 = "{StreamGroup}" + Guid.NewGuid();
        string key2 = "{StreamGroup}" + Guid.NewGuid();

        // Add entries to both streams
        await client.StreamAddAsync(key1, "field1", "value1");
        await client.StreamAddAsync(key2, "field2", "value2");

        // Create groups
        await client.StreamCreateConsumerGroupAsync(key1, "mygroup", StreamConstants.AllMessages);
        await client.StreamCreateConsumerGroupAsync(key2, "mygroup", StreamConstants.AllMessages);

        // Read from both streams
        StreamPosition[] positions = [new StreamPosition(key1, StreamConstants.UndeliveredMessages), new StreamPosition(key2, StreamConstants.UndeliveredMessages)];
        ValkeyStream[] streams = await client.StreamReadGroupAsync(positions, "mygroup", "consumer1");

        Assert.Equal(2, streams.Length);
        Assert.Single(streams[0].Entries);
        Assert.Single(streams[1].Entries);
        Assert.Equal("value1", streams[0].Entries[0].Values[0].Value.ToString());
        Assert.Equal("value2", streams[1].Entries[0].Values[0].Value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAcknowledgeAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Read messages
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Acknowledge both messages
        long ackCount = await client.StreamAcknowledgeAsync(key, "mygroup", [id1, id2]);
        Assert.Equal(2, ackCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAcknowledgeAsync_NonExistentMessage(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Try to acknowledge non-existent message
        long ackCount = await client.StreamAcknowledgeAsync(key, "mygroup", ["9999999999999-0"]);
        Assert.Equal(0, ackCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAcknowledgeAsync_WrongGroup(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        ValkeyValue id = await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Try to acknowledge with wrong group name - returns 0
        long ackCount = await client.StreamAcknowledgeAsync(key, "wronggroup", [id]);
        Assert.Equal(0, ackCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_Summary(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Read messages without acknowledging
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Get pending summary
        StreamPendingInfo info = await client.StreamPendingAsync(key, "mygroup");
        Assert.Equal(2, info.PendingMessageCount);
        Assert.Single(info.Consumers);
        Assert.Equal("consumer1", info.Consumers[0].Name.ToString());
        Assert.Equal(2, info.Consumers[0].PendingMessageCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_Detailed(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Read messages without acknowledging
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Get detailed pending messages
        StreamPendingMessageInfo[] messages = await client.StreamPendingMessagesAsync(key, "mygroup", 10, "consumer1");
        Assert.Equal(2, messages.Length);
        Assert.Equal(id1.ToString(), messages[0].MessageId.ToString());
        Assert.Equal("consumer1", messages[0].ConsumerName.ToString());
        Assert.Equal(1, messages[0].DeliveryCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_NoGroup(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry but don't create group
        await client.StreamAddAsync(key, "field1", "value1");

        // Try to get pending from non-existent group - should error
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamPendingAsync(key, "nonexistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingAsync_WrongKeyType(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Set key as string
        await client.StringSetAsync(key, "not_a_stream");

        // Try to get pending from string key - should error
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamPendingAsync(key, "mygroup"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamPendingMessagesAsync_WithMinIdle(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Read message
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Query with high minIdleTime - should return empty
        StreamPendingMessageInfo[] messages = await client.StreamPendingMessagesAsync(key, "mygroup", 10, "consumer1", minIdleTimeInMs: 999999);
        Assert.Empty(messages);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Consumer1 reads message
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Consumer2 claims the message
        StreamEntry[] claimed = await client.StreamClaimAsync(key, "mygroup", "consumer2", 0, [id1]);
        Assert.Single(claimed);
        Assert.Equal(id1.ToString(), claimed[0].Id.ToString());
        Assert.Equal("value1", claimed[0].Values[0].Value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_NoGroup(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry but don't create group
        ValkeyValue id = await client.StreamAddAsync(key, "field1", "value1");

        // Try to claim from non-existent group - should error
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamClaimAsync(key, "nonexistent", "consumer1", 0, [id]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_WrongKeyType(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Set key as string
        await client.StringSetAsync(key, "not_a_stream");

        // Try to claim from string key - should error
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.StreamClaimAsync(key, "mygroup", "consumer1", 0, ["1-0"]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimIdsOnlyAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Consumer1 reads messages
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Consumer2 claims the messages (IDs only)
        ValkeyValue[] claimedIds = await client.StreamClaimIdsOnlyAsync(key, "mygroup", "consumer2", 0, [id1, id2]);
        Assert.Equal(2, claimedIds.Length);
        Assert.Equal(id1.ToString(), claimedIds[0].ToString());
        Assert.Equal(id2.ToString(), claimedIds[1].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAutoClaimAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Consumer1 reads messages
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Consumer2 auto-claims pending messages
        StreamAutoClaimResult result = await client.StreamAutoClaimAsync(key, "mygroup", "consumer2", 0, StreamConstants.MinimumId);
        Assert.Equal("0-0", result.NextStartId.ToString());
        Assert.Equal(2, result.ClaimedEntries.Length);
        Assert.Equal("value1", result.ClaimedEntries[0].Values[0].Value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAutoClaimIdsOnlyAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Consumer1 reads messages
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Consumer2 auto-claims pending messages (IDs only)
        StreamAutoClaimIdsOnlyResult result = await client.StreamAutoClaimIdsOnlyAsync(key, "mygroup", "consumer2", 0, StreamConstants.MinimumId);
        Assert.Equal("0-0", result.NextStartId.ToString());
        Assert.Equal(2, result.ClaimedIds.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamAutoClaimAsync_WithCount(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamAddAsync(key, "field3", "value3");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Consumer1 reads messages
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Consumer2 auto-claims only 2 messages
        StreamAutoClaimResult result = await client.StreamAutoClaimAsync(key, "mygroup", "consumer2", 0, StreamConstants.MinimumId, count: 2);
        Assert.Equal(2, result.ClaimedEntries.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamGroupInfoAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create groups
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "group1", StreamConstants.AllMessages);
        await client.StreamCreateConsumerGroupAsync(key, "group2", StreamConstants.NewMessages);

        // Get group info
        StreamGroupInfo[] groups = await client.StreamGroupInfoAsync(key);
        Assert.Equal(2, groups.Length);
        Assert.Equal("group1", groups[0].Name);
        Assert.Equal("group2", groups[1].Name);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamConsumerInfoAsync_Basic(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries and create group
        await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamAddAsync(key, "field2", "value2");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Multiple consumers read messages
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages, count: 1);
        await client.StreamReadGroupAsync(key, "mygroup", "consumer2", StreamConstants.UndeliveredMessages, count: 1);

        // Get consumer info
        StreamConsumerInfo[] consumers = await client.StreamConsumerInfoAsync(key, "mygroup");
        Assert.Equal(2, consumers.Length);
        Assert.Equal("consumer1", consumers[0].Name);
        Assert.Equal(1, consumers[0].PendingMessageCount);
        Assert.Equal("consumer2", consumers[1].Name);
        Assert.Equal(1, consumers[1].PendingMessageCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_WithIdleTime(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Consumer1 reads message
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Consumer2 claims with IDLE parameter
        StreamEntry[] claimed = await client.StreamClaimAsync(key, "mygroup", "consumer2", 0, [id1], idleTimeInMs: 5000);
        Assert.Single(claimed);
        Assert.Equal(id1.ToString(), claimed[0].Id.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_WithRetryCount(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Consumer1 reads message
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Consumer2 claims with RETRYCOUNT parameter
        StreamEntry[] claimed = await client.StreamClaimAsync(key, "mygroup", "consumer2", 0, [id1], retryCount: 10);
        Assert.Single(claimed);

        // Verify retry count was set
        StreamPendingMessageInfo[] pending = await client.StreamPendingMessagesAsync(key, "mygroup", 10, "consumer2");
        Assert.Single(pending);
        Assert.Equal(10, pending[0].DeliveryCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimAsync_WithForce(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Claim message without reading it first (using FORCE)
        StreamEntry[] claimed = await client.StreamClaimAsync(key, "mygroup", "consumer1", 0, [id1], force: true);
        Assert.Single(claimed);
        Assert.Equal(id1.ToString(), claimed[0].Id.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamClaimIdsOnlyAsync_WithOptionalParams(BaseClient client)
    {
        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entry and create group
        ValkeyValue id1 = await client.StreamAddAsync(key, "field1", "value1");
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);

        // Consumer1 reads message
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // Consumer2 claims with optional parameters
        ValkeyValue[] claimedIds = await client.StreamClaimIdsOnlyAsync(key, "mygroup", "consumer2", 0, [id1], idleTimeInMs: 1000, retryCount: 5);
        Assert.Single(claimedIds);
        Assert.Equal(id1.ToString(), claimedIds[0].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StreamConsumerGroupSetPositionAsync_WithEntriesRead(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("7.0.0"), "ENTRIESREAD parameter requires server version 7.0.0 or higher");

        string key = "{StreamGroup}" + Guid.NewGuid();

        // Add entries
        ValkeyValue id1 = await client.StreamAddAsync(key, "field", "value1");
        ValkeyValue id2 = await client.StreamAddAsync(key, "field", "value2");
        ValkeyValue id3 = await client.StreamAddAsync(key, "field", "value3");

        // Create group and read all messages
        await client.StreamCreateConsumerGroupAsync(key, "mygroup", StreamConstants.AllMessages);
        await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);

        // No more new messages
        StreamEntry[] entries = await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);
        Assert.Empty(entries);

        // Reset position to id2 with entriesRead=10 (Valkey 7.0+)
        bool result = await client.StreamConsumerGroupSetPositionAsync(key, "mygroup", id2, entriesRead: 10);
        Assert.True(result);

        // Should now be able to read id3
        entries = await client.StreamReadGroupAsync(key, "mygroup", "consumer1", StreamConstants.UndeliveredMessages);
        Assert.Single(entries);
        Assert.Equal(id3.ToString(), entries[0].Id.ToString());

        StreamGroupInfo[] groups = await client.StreamGroupInfoAsync(key);
        // After reading one more message, it should be 11
        Assert.True(groups[0].EntriesRead >= 10L);
    }
}
