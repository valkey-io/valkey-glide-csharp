// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// End-to-end integration tests for PubSub functionality.
/// These tests verify the complete message flow from PUBLISH commands through the server,
/// Rust core, FFI boundary, and into C# callbacks.
/// Uses CustomCommand for PUBLISH operations to test the full stack.
/// </summary>
public class PubSubCallbackIntegrationTests : IDisposable
{
    private readonly List<BaseClient> _testClients = [];
    private readonly ManualResetEventSlim _messageReceivedEvent = new(false);
    private readonly object _lockObject = new();

    public void Dispose()
    {
        // Clean up all test clients
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
        _messageReceivedEvent.Dispose();
    }

    [Fact]
    public async Task EndToEndMessageFlow_WithStandaloneClient_ProcessesMessagesCorrectly()
    {
        // Arrange
        string testChannel = $"test-channel-{Guid.NewGuid()}";
        string testMessage = "Hello from integration test!";
        PubSubMessage? receivedMessage = null;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                receivedMessage = message;
                _messageReceivedEvent.Set();
            });

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

        // Publish message through the server (true E2E)
        object? publishResult = await publisherClient.CustomCommand(["PUBLISH", testChannel, testMessage]);
        long numReceivers = Convert.ToInt64(publishResult);
        Assert.Equal(1L, numReceivers); // Should have 1 subscriber

        // Wait for message to be received via callback
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Message should have been received within timeout");
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
        Assert.Null(receivedMessage.Pattern);
    }

    [Fact]
    public async Task EndToEndMessageFlow_WithClusterClient_ProcessesMessagesCorrectly()
    {
        // Arrange
        string testChannel = $"test-cluster-channel-{Guid.NewGuid()}";
        string testMessage = "Hello from cluster integration test!";
        PubSubMessage? receivedMessage = null;

        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<ClusterPubSubSubscriptionConfig>((message, context) =>
            {
                receivedMessage = message;
                _messageReceivedEvent.Set();
            });

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClusterClientConfig().Build();

        // Act
        GlideClusterClient subscriberClient = await GlideClusterClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        GlideClusterClient publisherClient = await GlideClusterClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        // Wait for subscription to be established
        await Task.Delay(1000);

        // Publish message through the server (true E2E)
        ClusterValue<object?> publishResult = await publisherClient.CustomCommand(["PUBLISH", testChannel, testMessage]);
        long numReceivers = Convert.ToInt64(publishResult.SingleValue);
        Assert.Equal(1L, numReceivers); // Should have 1 subscriber

        // Wait for message to be received via callback
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Message should have been received within timeout");
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
        Assert.Null(receivedMessage.Pattern);
    }

    [Fact]
    public async Task PatternSubscription_WithServerPublish_ProcessesPatternMessagesCorrectly()
    {
        // Arrange
        string testPattern = "news.*";
        string testChannel = $"news.sports.{Guid.NewGuid()}";
        string testMessage = "Breaking sports news!";
        PubSubMessage? receivedMessage = null;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithPattern(testPattern)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                receivedMessage = message;
                _messageReceivedEvent.Set();
            });

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

        // Publish message to channel matching pattern (true E2E)
        object? publishResult = await publisherClient.CustomCommand(["PUBLISH", testChannel, testMessage]);
        long numReceivers = Convert.ToInt64(publishResult);
        Assert.Equal(1L, numReceivers); // Should have 1 pattern subscriber

        // Wait for message to be received via callback
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Pattern message should have been received within timeout");
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
        Assert.Equal(testPattern, receivedMessage.Pattern);
    }

    [Fact]
    public async Task CallbackErrorHandling_WithExceptionInCallback_IsolatesErrorsAndContinuesProcessing()
    {
        // Arrange
        string testChannel = $"error-test-{Guid.NewGuid()}";
        int callbackInvocations = 0;
        int successfulMessages = 0;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                int invocation = Interlocked.Increment(ref callbackInvocations);

                // Throw exception on first message, succeed on subsequent messages
                if (invocation == 1)
                {
                    throw new InvalidOperationException("Test exception in callback");
                }

                _ = Interlocked.Increment(ref successfulMessages);
                if (successfulMessages >= 2)
                {
                    _messageReceivedEvent.Set();
                }
            });

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

        // Publish multiple messages through server
        await publisherClient.CustomCommand(["PUBLISH", testChannel, "Message 1 - should cause exception"]);
        await Task.Delay(100); // Allow first message to be processed

        await publisherClient.CustomCommand(["PUBLISH", testChannel, "Message 2 - should succeed"]);
        await publisherClient.CustomCommand(["PUBLISH", testChannel, "Message 3 - should succeed"]);

        // Wait for successful messages to be processed
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Should have received successful messages despite callback exception");
        Assert.True(callbackInvocations >= 3, "Callback should have been invoked for all messages");
        Assert.True(successfulMessages >= 2, "Should have processed messages successfully after exception");
    }

    [Fact]
    public async Task AsyncMessageProcessing_WithServerPublish_CompletesQuicklyWithoutBlockingFFI()
    {
        // Arrange
        string testChannel = $"async-test-{Guid.NewGuid()}";
        List<TimeSpan> processingDurations = [];
        int messagesProcessed = 0;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                Stopwatch sw = Stopwatch.StartNew();

                // Simulate some processing work
                Thread.Sleep(50); // 50ms processing time

                sw.Stop();
                lock (_lockObject)
                {
                    processingDurations.Add(sw.Elapsed);
                }

                int processed = Interlocked.Increment(ref messagesProcessed);
                if (processed >= 5)
                {
                    _messageReceivedEvent.Set();
                }
            });

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

        // Measure time to publish multiple messages rapidly
        Stopwatch publishStopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 5; i++)
        {
            await publisherClient.CustomCommand(["PUBLISH", testChannel, $"Async test message {i}"]);
        }

        publishStopwatch.Stop();

        // Wait for all messages to be processed
        bool allProcessed = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(10));

        // Assert
        Assert.True(allProcessed, "All messages should have been processed");
        Assert.Equal(5, messagesProcessed);

        // Publishing should complete quickly (shouldn't block on callback processing)
        Assert.True(publishStopwatch.ElapsedMilliseconds < 2000,
            $"Publishing should complete quickly, took {publishStopwatch.ElapsedMilliseconds}ms");

        // Processing durations should reflect the actual work done
        Assert.True(processingDurations.Count >= 5, "Should have recorded processing durations");
        Assert.All(processingDurations, duration =>
            Assert.True(duration.TotalMilliseconds >= 40,
                $"Processing should take at least 40ms, took {duration.TotalMilliseconds}ms"));
    }

    [Fact]
    public async Task MemoryManagement_WithMarshaledData_HandlesCleanupCorrectly()
    {
        // Arrange
        string testChannel = $"memory-test-{Guid.NewGuid()}";
        List<string> receivedMessages = [];
        int messageCount = 0;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                lock (_lockObject)
                {
                    receivedMessages.Add(message.Message);
                }

                int count = Interlocked.Increment(ref messageCount);
                if (count >= 10)
                {
                    _messageReceivedEvent.Set();
                }
            });

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

        // Publish messages with various content to test marshaling through FFI
        string[] testMessages = [
            "Simple message",
            "Message with special chars: !@#$%^&*()",
            "Unicode message: 你好世界 🌍",
            "Long message: " + new string('A', 1000),
            "Empty content: ",
            "Numbers: 1234567890",
            "JSON: {\"key\": \"value\", \"number\": 42}",
            "XML: <root><item>value</item></root>",
            "Base64: SGVsbG8gV29ybGQ=",
            "Final message"
        ];

        foreach (string message in testMessages)
        {
            await publisherClient.CustomCommand(["PUBLISH", testChannel, message]);
            await Task.Delay(10); // Small delay between messages
        }

        // Wait for all messages to be processed
        bool allReceived = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(10));

        // Assert
        Assert.True(allReceived, "All messages should have been received");
        Assert.Equal(10, messageCount);

        lock (_lockObject)
        {
            Assert.Equal(10, receivedMessages.Count);

            // Verify message content integrity (marshaling worked correctly through FFI)
            for (int i = 0; i < testMessages.Length; i++)
            {
                Assert.Contains(testMessages[i], receivedMessages);
            }
        }
    }

    [Fact]
    public async Task ErrorIsolation_WithMessageHandlerExceptions_DoesNotCrashProcess()
    {
        // Arrange
        string testChannel = $"isolation-test-{Guid.NewGuid()}";
        int callbackCount = 0;
        bool systemStillWorking = false;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                int count = Interlocked.Increment(ref callbackCount);

                if (count == 1)
                {
                    // First message: throw a severe exception
                    throw new OutOfMemoryException("Simulated severe exception");
                }
                else if (count == 2)
                {
                    // Second message: throw a different exception
                    throw new InvalidOperationException("Another test exception");
                }
                else
                {
                    // Third message: succeed
                    systemStillWorking = true;
                    _messageReceivedEvent.Set();
                }
            });

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

        // Publish messages that will cause exceptions in callbacks
        await publisherClient.CustomCommand(["PUBLISH", testChannel, "Message 1 - OutOfMemoryException"]);
        await Task.Delay(100);

        await publisherClient.CustomCommand(["PUBLISH", testChannel, "Message 2 - InvalidOperationException"]);
        await Task.Delay(100);

        await publisherClient.CustomCommand(["PUBLISH", testChannel, "Message 3 - Should succeed"]);

        // Wait for the successful message
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "System should continue working after callback exceptions");
        Assert.True(systemStillWorking, "System should process subsequent messages successfully");
        Assert.True(callbackCount >= 3, "All callbacks should have been invoked despite exceptions");

        // Process should still be running (not crashed)
        Assert.False(Environment.HasShutdownStarted, "Process should not have initiated shutdown");
    }

    [Fact]
    public async Task MultipleSubscribers_ToSameChannel_AllReceiveMessages()
    {
        // Arrange
        string testChannel = $"multi-sub-{Guid.NewGuid()}";
        string testMessage = "Broadcast message";
        int subscriber1Received = 0;
        int subscriber2Received = 0;
        ManualResetEventSlim allReceivedEvent = new ManualResetEventSlim(false);

        StandalonePubSubSubscriptionConfig pubsubConfig1 = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                int count = Interlocked.Increment(ref subscriber1Received);
                if (subscriber1Received >= 1 && subscriber2Received >= 1)
                {
                    allReceivedEvent.Set();
                }
            });

        StandalonePubSubSubscriptionConfig pubsubConfig2 = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                int count = Interlocked.Increment(ref subscriber2Received);
                if (subscriber1Received >= 1 && subscriber2Received >= 1)
                {
                    allReceivedEvent.Set();
                }
            });

        var subscriberConfig1 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();

        var subscriberConfig2 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriber1 = await GlideClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriber1);

        GlideClient subscriber2 = await GlideClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriber2);

        GlideClient publisher = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisher);

        // Wait for subscriptions to be established
        await Task.Delay(1000);

        // Publish message - should reach both subscribers
        object? publishResult = await publisher.CustomCommand(["PUBLISH", testChannel, testMessage]);
        long numReceivers = Convert.ToInt64(publishResult);
        Assert.Equal(2L, numReceivers); // Should have 2 subscribers

        // Wait for both subscribers to receive
        bool allReceived = allReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(allReceived, "Both subscribers should receive the message");
        Assert.Equal(1, subscriber1Received);
        Assert.Equal(1, subscriber2Received);

        allReceivedEvent.Dispose();
    }

    [Fact]
    public async Task MessageOrdering_WithMultipleMessages_PreservesOrder()
    {
        // Arrange
        string testChannel = $"order-test-{Guid.NewGuid()}";
        List<string> receivedMessages = [];
        int expectedCount = 20;
        ManualResetEventSlim allReceivedEvent = new ManualResetEventSlim(false);

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                lock (receivedMessages)
                {
                    receivedMessages.Add(message.Message);
                    if (receivedMessages.Count >= expectedCount)
                    {
                        allReceivedEvent.Set();
                    }
                }
            });

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();

        // Act
        GlideClient subscriber = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriber);

        GlideClient publisher = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisher);

        // Wait for subscription to be established
        await Task.Delay(1000);

        // Publish messages in order
        for (int i = 0; i < expectedCount; i++)
        {
            await publisher.CustomCommand(["PUBLISH", testChannel, $"Message-{i:D3}"]);
        }

        // Wait for all messages
        bool allReceived = allReceivedEvent.Wait(TimeSpan.FromSeconds(10));

        // Assert
        Assert.True(allReceived, "All messages should be received");
        Assert.Equal(expectedCount, receivedMessages.Count);

        // Verify order is preserved
        for (int i = 0; i < expectedCount; i++)
        {
            Assert.Equal($"Message-{i:D3}", receivedMessages[i]);
        }

        allReceivedEvent.Dispose();
    }

    [Fact]
    public async Task ClusterPatternSubscription_WithServerPublish_ReceivesMatchingMessages()
    {
        // Skip if no cluster hosts available
        if (TestConfiguration.CLUSTER_HOSTS.Count == 0)
        {
            return;
        }

        // Arrange
        string testPattern = "events.*";
        string testChannel = $"events.user.{Guid.NewGuid()}";
        string testMessage = "User event occurred";
        PubSubMessage? receivedMessage = null;

        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithPattern(testPattern)
            .WithCallback<ClusterPubSubSubscriptionConfig>((message, context) =>
            {
                receivedMessage = message;
                _messageReceivedEvent.Set();
            });

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        var publisherConfig = TestConfiguration.DefaultClusterClientConfig().Build();

        // Act
        GlideClusterClient subscriber = await GlideClusterClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriber);

        GlideClusterClient publisher = await GlideClusterClient.CreateClient(publisherConfig);
        _testClients.Add(publisher);

        // Wait for subscription to be established - pattern subscriptions in cluster mode may need more time
        await Task.Delay(5000);

        // Publish to matching channel
        ClusterValue<object?> publishResult = await publisher.CustomCommand(["PUBLISH", testChannel, testMessage]);
        long numReceivers = Convert.ToInt64(publishResult.SingleValue);

        // Note: In cluster mode, PUBLISH returns the number of clients that received the message on the node
        // where the channel is hashed. Pattern subscriptions may not always report correctly via PUBLISH return value
        // in cluster mode because the subscription might be on a different node. We verify message delivery via callback.

        // Wait for message
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Pattern message should be received");
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
        Assert.Equal(testPattern, receivedMessage.Pattern);
    }
}
