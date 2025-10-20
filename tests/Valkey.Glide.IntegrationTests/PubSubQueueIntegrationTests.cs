// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// End-to-end integration tests for PubSub queue-based message retrieval.
/// These tests verify the complete message flow from PUBLISH commands through the server,
/// Rust core, FFI boundary, and into C# message queues (without callbacks).
/// Tests the alternative PubSub usage pattern where users manually poll for messages.
/// </summary>
public class PubSubQueueIntegrationTests : IDisposable
{
    private readonly List<BaseClient> _testClients = [];

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        foreach (BaseClient client in _testClients)
        {
            try
            {
                client.Dispose();
            }
            catch
            {
                // Ignore disposal errors in tests
            }
        }
        _testClients.Clear();
    }

    [Fact]
    public async Task QueueBasedRetrieval_WithStandaloneClient_ReceivesMessages()
    {
        // Arrange
        string testChannel = $"queue-test-{Guid.NewGuid()}";
        string testMessage = "Hello from queue test!";

        // Create subscription config WITHOUT callback - messages go to queue
        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);
        // Note: No .WithCallback() - this enables queue mode

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClient publisherClient = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        // Wait for subscription to be established
        await Task.Delay(1000);

        // Publish message through the server
        object? publishResult = await publisherClient.CustomCommand(["PUBLISH", testChannel, testMessage]);
        long numReceivers = Convert.ToInt64(publishResult);
        Assert.Equal(1L, numReceivers);

        // Get the message queue and retrieve message
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        // Wait for message to arrive in queue
        await Task.Delay(500);

        // Assert
        Assert.True(queue.Count > 0, "Queue should contain at least one message");
        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.True(hasMessage, "Should successfully retrieve message from queue");
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
        Assert.Null(receivedMessage.Pattern);
    }

    [Fact]
    public async Task QueueBasedRetrieval_WithClusterClient_ReceivesMessages()
    {
        // Skip if no cluster hosts available
        if (TestConfiguration.CLUSTER_HOSTS.Count == 0)
        {
            return;
        }

        // Arrange
        string testChannel = $"cluster-queue-{Guid.NewGuid()}";
        string testMessage = "Cluster queue message";

        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel);
        // No callback - queue mode

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClusterClientConfig().Build();

        // Act
        GlideClusterClient subscriberClient = await GlideClusterClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClusterClient publisherClient = await GlideClusterClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        await Task.Delay(1000);

        ClusterValue<object?> publishResult = await publisherClient.CustomCommand(["PUBLISH", testChannel, testMessage]);
        long numReceivers = Convert.ToInt64(publishResult.SingleValue);
        Assert.Equal(1L, numReceivers);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);
        await Task.Delay(500);

        // Assert
        Assert.True(queue.Count > 0);
        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.True(hasMessage);
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
    }

    [Fact]
    public async Task QueueBasedRetrieval_WithPatternSubscription_ReceivesMatchingMessages()
    {
        // Arrange
        string testPattern = "queue.pattern.*";
        string testChannel = $"queue.pattern.{Guid.NewGuid()}";
        string testMessage = "Pattern queue message";

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithPattern(testPattern);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClient publisherClient = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        await Task.Delay(1000);

        await publisherClient.CustomCommand(["PUBLISH", testChannel, testMessage]);
        await Task.Delay(500);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        // Assert
        Assert.True(queue.Count > 0);
        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.True(hasMessage);
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
        Assert.Equal(testPattern, receivedMessage.Pattern);
    }

    [Fact]
    public async Task QueueBasedRetrieval_WithMultipleMessages_PreservesOrder()
    {
        // Arrange
        string testChannel = $"queue-order-{Guid.NewGuid()}";
        int messageCount = 10;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClient publisherClient = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        await Task.Delay(1000);

        // Publish multiple messages in order
        for (int i = 0; i < messageCount; i++)
        {
            await publisherClient.CustomCommand(["PUBLISH", testChannel, $"Message-{i:D3}"]);
        }

        // Wait for all messages to arrive
        await Task.Delay(1000);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        // Assert
        Assert.True(queue.Count >= messageCount, $"Queue should contain at least {messageCount} messages");

        List<string> receivedMessages = [];
        for (int i = 0; i < messageCount; i++)
        {
            bool hasMessage = queue.TryGetMessage(out PubSubMessage? message);
            Assert.True(hasMessage, $"Should retrieve message {i}");
            Assert.NotNull(message);
            receivedMessages.Add(message.Message);
        }

        // Verify order is preserved
        for (int i = 0; i < messageCount; i++)
        {
            Assert.Equal($"Message-{i:D3}", receivedMessages[i]);
        }
    }

    [Fact]
    public async Task QueueBasedRetrieval_GetMessageAsync_BlocksUntilMessageAvailable()
    {
        // Arrange
        string testChannel = $"queue-async-{Guid.NewGuid()}";
        string testMessage = "Async message";

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClient publisherClient = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        await Task.Delay(1000);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        // Start waiting for message (should block)
        Task<PubSubMessage> getMessageTask = Task.Run(async () =>
        {
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(10));
            return await queue.GetMessageAsync(cts.Token);
        });

        // Give the task time to start waiting
        await Task.Delay(100);

        // Verify task is still waiting
        Assert.False(getMessageTask.IsCompleted, "GetMessageAsync should be waiting for message");

        // Now publish the message
        await publisherClient.CustomCommand(["PUBLISH", testChannel, testMessage]);

        // Wait for message to be received
        PubSubMessage receivedMessage = await getMessageTask;

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
    }

    [Fact]
    public async Task QueueBasedRetrieval_GetMessageAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        string testChannel = $"queue-cancel-{Guid.NewGuid()}";

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        await Task.Delay(1000);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(500));

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await queue.GetMessageAsync(cts.Token);
        });
    }

    [Fact]
    public async Task QueueBasedRetrieval_TryGetMessage_ReturnsFalseWhenEmpty()
    {
        // Arrange
        string testChannel = $"queue-empty-{Guid.NewGuid()}";

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        await Task.Delay(1000);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        // Assert
        Assert.Equal(0, queue.Count);
        bool hasMessage = queue.TryGetMessage(out PubSubMessage? message);
        Assert.False(hasMessage);
        Assert.Null(message);
    }

    [Fact]
    public async Task QueueBasedRetrieval_WithMultipleChannels_ReceivesAllMessages()
    {
        // Arrange
        string channel1 = $"queue-multi-1-{Guid.NewGuid()}";
        string channel2 = $"queue-multi-2-{Guid.NewGuid()}";
        string message1 = "Message from channel 1";
        string message2 = "Message from channel 2";

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(channel1)
            .WithChannel(channel2);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClient publisherClient = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        await Task.Delay(1000);

        // Publish to both channels
        await publisherClient.CustomCommand(["PUBLISH", channel1, message1]);
        await publisherClient.CustomCommand(["PUBLISH", channel2, message2]);

        await Task.Delay(500);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        // Assert
        Assert.True(queue.Count >= 2, "Queue should contain messages from both channels");

        HashSet<string> receivedChannels = [];
        HashSet<string> receivedMessages = [];

        for (int i = 0; i < 2; i++)
        {
            bool hasMessage = queue.TryGetMessage(out PubSubMessage? message);
            Assert.True(hasMessage);
            Assert.NotNull(message);
            receivedChannels.Add(message.Channel);
            receivedMessages.Add(message.Message);
        }

        Assert.Contains(channel1, receivedChannels);
        Assert.Contains(channel2, receivedChannels);
        Assert.Contains(message1, receivedMessages);
        Assert.Contains(message2, receivedMessages);
    }

    [Fact]
    public async Task QueueBasedRetrieval_WithHighVolume_HandlesAllMessages()
    {
        // Arrange
        string testChannel = $"queue-volume-{Guid.NewGuid()}";
        int messageCount = 100;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClient publisherClient = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        await Task.Delay(1000);

        // Publish many messages rapidly
        for (int i = 0; i < messageCount; i++)
        {
            await publisherClient.CustomCommand(["PUBLISH", testChannel, $"Volume-{i}"]);
        }

        // Wait for messages to arrive
        await Task.Delay(2000);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        // Assert
        Assert.True(queue.Count >= messageCount, $"Queue should contain all {messageCount} messages");

        HashSet<string> receivedMessages = [];
        for (int i = 0; i < messageCount; i++)
        {
            bool hasMessage = queue.TryGetMessage(out PubSubMessage? message);
            Assert.True(hasMessage, $"Should retrieve message {i}");
            Assert.NotNull(message);
            receivedMessages.Add(message.Message);
        }

        Assert.Equal(messageCount, receivedMessages.Count);
    }

    [Fact]
    public async Task QueueBasedRetrieval_MixedCallbackAndQueue_ThrowsInvalidOperationException()
    {
        // Arrange
        string testChannel = $"queue-mixed-{Guid.NewGuid()}";

        // Create config WITH callback
        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                // Callback mode
            });

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        await Task.Delay(1000);

        // Assert - Should throw when trying to get queue in callback mode
        Assert.Throws<InvalidOperationException>(() =>
        {
            PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        });
    }

    [Fact]
    public async Task QueueBasedRetrieval_WithUnicodeAndSpecialCharacters_PreservesContent()
    {
        // Arrange
        string testChannel = $"queue-unicode-{Guid.NewGuid()}";
        string[] testMessages = [
            "Simple ASCII",
            "Unicode: 你好世界 🌍",
            "Special chars: !@#$%^&*()",
            "Emoji: 🎉🚀💻",
            "Mixed: Hello世界!🌟"
        ];

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClient publisherClient = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        await Task.Delay(1000);

        foreach (string message in testMessages)
        {
            await publisherClient.CustomCommand(["PUBLISH", testChannel, message]);
        }

        await Task.Delay(1000);

        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        // Assert
        Assert.True(queue.Count >= testMessages.Length);

        List<string> receivedMessages = [];
        for (int i = 0; i < testMessages.Length; i++)
        {
            bool hasMessage = queue.TryGetMessage(out PubSubMessage? message);
            Assert.True(hasMessage);
            Assert.NotNull(message);
            receivedMessages.Add(message.Message);
        }

        foreach (string expectedMessage in testMessages)
        {
            Assert.Contains(expectedMessage, receivedMessages);
        }
    }
}
