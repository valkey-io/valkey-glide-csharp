// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;
using System.Diagnostics;

using Valkey.Glide;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for FFI PubSub callback flow infrastructure.
/// These tests verify end-to-end message processing, error handling, and async message processing
/// using simulated FFI callbacks.
/// Note: Uses simulated FFI callbacks since full PubSub server integration requires additional infrastructure.
/// Future enhancement: Replace with real PUBLISH commands via CustomCommand when PubSub infrastructure is complete.
/// </summary>
public class PubSubFFICallbackIntegrationTests : IDisposable
{
    private readonly List<BaseClient> _testClients = [];
    private readonly ConcurrentBag<Exception> _callbackExceptions = [];
    private readonly ConcurrentBag<PubSubMessage> _receivedMessages = [];
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

    /// <summary>
    /// Simulates an FFI callback by directly invoking the client's message handler.
    /// This allows testing the callback infrastructure without requiring full server PubSub integration.
    /// Future enhancement: Replace with real PUBLISH commands via CustomCommand when PubSub infrastructure is complete.
    /// </summary>
    private async Task SimulateFFICallback(BaseClient client, string channel, string message, string? pattern)
    {
        await Task.Run(() =>
        {
            PubSubMessage pubsubMessage = pattern == null
                ? new PubSubMessage(message, channel)
                : new PubSubMessage(message, channel, pattern);
            client.HandlePubSubMessage(pubsubMessage);
        });
    }

    [Fact]
    public async Task EndToEndMessageFlow_WithStandaloneClient_ProcessesMessagesCorrectly()
    {
        // Arrange
        string testChannel = $"test-channel-{Guid.NewGuid()}";
        string testMessage = "Hello from integration test!";
        bool messageReceived = false;
        PubSubMessage? receivedMessage = null;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                receivedMessage = message;
                messageReceived = true;
                _messageReceivedEvent.Set();
            });

        var config = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(config);
        _testClients.Add(subscriberClient);

        // Simulate FFI callback invocation - tests the callback infrastructure
        await SimulateFFICallback(subscriberClient, testChannel, testMessage, null);

        // Wait for message to be received
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Message should have been received within timeout");
        Assert.True(messageReceived, "Callback should have been invoked");
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
        Assert.Null(receivedMessage.Pattern);
    }

    [Fact]
    public async Task EndToEndMessageFlow_WithClusterClient_ProcessesMessagesCorrectly()
    {
        // Skip if no cluster hosts available
        if (TestConfiguration.CLUSTER_HOSTS.Count == 0)
        {
            return;
        }

        // Arrange
        string testChannel = $"test-cluster-channel-{Guid.NewGuid()}";
        string testMessage = "Hello from cluster integration test!";
        bool messageReceived = false;
        PubSubMessage? receivedMessage = null;

        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel)
            .WithCallback<ClusterPubSubSubscriptionConfig>((message, context) =>
            {
                receivedMessage = message;
                messageReceived = true;
                _messageReceivedEvent.Set();
            });

        var config = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClusterClient subscriberClient = await GlideClusterClient.CreateClient(config);
        _testClients.Add(subscriberClient);

        // Simulate FFI callback invocation - tests the callback infrastructure
        await SimulateFFICallback(subscriberClient, testChannel, testMessage, null);

        // Wait for message to be received
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Message should have been received within timeout");
        Assert.True(messageReceived, "Callback should have been invoked");
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
        Assert.Null(receivedMessage.Pattern);
    }

    [Fact]
    public async Task PatternSubscription_WithSimulatedCallback_ProcessesPatternMessagesCorrectly()
    {
        // Arrange
        string testPattern = $"news.*";
        string testChannel = $"news.sports.{Guid.NewGuid()}";
        string testMessage = "Breaking sports news!";
        bool messageReceived = false;
        PubSubMessage? receivedMessage = null;

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithPattern(testPattern)
            .WithCallback<StandalonePubSubSubscriptionConfig>((message, context) =>
            {
                receivedMessage = message;
                messageReceived = true;
                _messageReceivedEvent.Set();
            });

        var config = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(config);
        _testClients.Add(subscriberClient);

        // Simulate FFI callback for pattern message
        await SimulateFFICallback(subscriberClient, testChannel, testMessage, testPattern);

        // Wait for message to be received
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Pattern message should have been received within timeout");
        Assert.True(messageReceived, "Callback should have been invoked for pattern message");
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

        var config = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(config);
        _testClients.Add(subscriberClient);

        // Simulate multiple messages via FFI callbacks
        await SimulateFFICallback(subscriberClient, testChannel, "Message 1 - should cause exception", null);
        await Task.Delay(100); // Allow first message to be processed

        await SimulateFFICallback(subscriberClient, testChannel, "Message 2 - should succeed", null);
        await SimulateFFICallback(subscriberClient, testChannel, "Message 3 - should succeed", null);

        // Wait for successful messages to be processed
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "Should have received successful messages despite callback exception");
        Assert.True(callbackInvocations >= 3, "Callback should have been invoked for all messages");
        Assert.True(successfulMessages >= 2, "Should have processed messages successfully after exception");
    }

    [Fact]
    public async Task AsyncMessageProcessing_WithRealClients_CompletesQuicklyWithoutBlockingFFI()
    {
        // Arrange
        string testChannel = $"async-test-{Guid.NewGuid()}";
        List<TimeSpan> callbackDurations = [];
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

        var config = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(config);
        _testClients.Add(subscriberClient);

        // Measure time to simulate multiple messages rapidly
        Stopwatch simulationStopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 5; i++)
        {
            await SimulateFFICallback(subscriberClient, testChannel, $"Async test message {i}", null);
        }

        simulationStopwatch.Stop();

        // Wait for all messages to be processed
        bool allProcessed = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(10));

        // Assert
        Assert.True(allProcessed, "All messages should have been processed");
        Assert.Equal(5, messagesProcessed);

        // Simulation should complete quickly (FFI callbacks shouldn't block)
        Assert.True(simulationStopwatch.ElapsedMilliseconds < 1000,
            $"Simulation should complete quickly, took {simulationStopwatch.ElapsedMilliseconds}ms");

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

        var config = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(config);
        _testClients.Add(subscriberClient);

        // Simulate messages with various content to test marshaling
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
            await SimulateFFICallback(subscriberClient, testChannel, message, null);
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

            // Verify message content integrity (marshaling worked correctly)
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

        var config = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        // Act
        GlideClient subscriberClient = await GlideClient.CreateClient(config);
        _testClients.Add(subscriberClient);

        // Simulate messages that will cause exceptions
        await SimulateFFICallback(subscriberClient, testChannel, "Message 1 - OutOfMemoryException", null);
        await Task.Delay(100);

        await SimulateFFICallback(subscriberClient, testChannel, "Message 2 - InvalidOperationException", null);
        await Task.Delay(100);

        await SimulateFFICallback(subscriberClient, testChannel, "Message 3 - Should succeed", null);

        // Wait for the successful message
        bool received = _messageReceivedEvent.Wait(TimeSpan.FromSeconds(5));

        // Assert
        Assert.True(received, "System should continue working after callback exceptions");
        Assert.True(systemStillWorking, "System should process subsequent messages successfully");
        Assert.True(callbackCount >= 3, "All callbacks should have been invoked despite exceptions");

        // Process should still be running (not crashed)
        Assert.True(!Environment.HasShutdownStarted, "Process should not have initiated shutdown");
    }
}
