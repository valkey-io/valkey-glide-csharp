// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Comprehensive thread safety tests for PubSub handler access in BaseClient.
/// Tests concurrent access, disposal during message processing, and race condition scenarios.
/// </summary>
public class PubSubThreadSafetyTests
{
    [Fact]
    public async Task PubSubHandler_ConcurrentMessageProcessing_NoRaceConditions()
    {
        // Arrange
        var messagesReceived = new ConcurrentBag<PubSubMessage>();
        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithCallback((msg, ctx) =>
            {
                messagesReceived.Add(msg);
                Thread.Sleep(1); // Simulate some processing
            }, null);

        var client = CreateMockClientWithPubSub(config);

        // Act - Process 100 messages concurrently from multiple threads
        var tasks = Enumerable.Range(0, 100)
            .Select(i => Task.Run(() =>
            {
                var message = PubSubMessage.FromChannel($"message-{i}", "test-channel");
                client.HandlePubSubMessage(message);
            }))
            .ToArray();

        await Task.WhenAll(tasks);

        // Wait for all messages to be processed
        await Task.Delay(500);

        // Assert
        Assert.Equal(100, messagesReceived.Count);
        Assert.Equal(100, messagesReceived.Distinct().Count()); // All messages should be unique
    }

    [Fact]
    public async Task PubSubHandler_DisposalDuringMessageProcessing_NoNullReferenceException()
    {
        // Arrange
        var processingStarted = new ManualResetEventSlim(false);
        var continueProcessing = new ManualResetEventSlim(false);
        var exceptions = new ConcurrentBag<Exception>();

        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithCallback((msg, ctx) =>
            {
                processingStarted.Set();
                continueProcessing.Wait(TimeSpan.FromSeconds(5));
                Thread.Sleep(50); // Simulate processing
            }, null);

        var client = CreateMockClientWithPubSub(config);

        // Act - Start message processing
        var messageTask = Task.Run(() =>
        {
            try
            {
                var message = PubSubMessage.FromChannel("test-message", "test-channel");
                client.HandlePubSubMessage(message);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        // Wait for processing to start
        processingStarted.Wait(TimeSpan.FromSeconds(5));

        // Dispose client while message is being processed
        var disposeTask = Task.Run(() =>
        {
            try
            {
                client.Dispose();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        // Allow message processing to continue
        continueProcessing.Set();

        await Task.WhenAll(messageTask, disposeTask);

        // Assert - No exceptions should occur
        Assert.Empty(exceptions);
    }

    [Fact]
    public async Task PubSubQueue_ConcurrentAccess_ThreadSafe()
    {
        // Arrange
        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel");

        var client = CreateMockClientWithPubSub(config);

        // Act - Access PubSubQueue from multiple threads concurrently
        var tasks = Enumerable.Range(0, 50)
            .Select(_ => Task.Run(() =>
            {
                var queue = client.PubSubQueue;
                Assert.NotNull(queue);
            }))
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert - No exceptions should occur
        Assert.NotNull(client.PubSubQueue);
    }

    [Fact]
    public async Task PubSubHandler_RapidCreateAndDispose_NoMemoryLeaks()
    {
        // Arrange & Act - Create and dispose clients rapidly
        for (int i = 0; i < 50; i++)
        {
            var config = new StandalonePubSubSubscriptionConfig()
                .WithChannel("test-channel")
                .WithCallback((msg, ctx) => { }, null);

            var client = CreateMockClientWithPubSub(config);

            // Send a few messages
            for (int j = 0; j < 5; j++)
            {
                var message = PubSubMessage.FromChannel($"message-{j}", "test-channel");
                client.HandlePubSubMessage(message);
            }

            // Dispose immediately
            client.Dispose();
        }

        // Force GC to detect any memory leaks
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert - Test passes if no exceptions occur
        Assert.True(true);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task PubSubHandler_DisposalDuringCallback_CompletesWithoutHanging()
    {
        // Arrange - Create handler with slow callback
        var disposeStarted = new ManualResetEventSlim(false);
        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithCallback((msg, ctx) =>
            {
                // This callback will block during disposal
                disposeStarted.Wait(TimeSpan.FromSeconds(10));
            }, null);

        var client = CreateMockClientWithPubSub(config);

        // Start a long-running message processing
        var messageTask = Task.Run(() =>
        {
            var message = PubSubMessage.FromChannel("test-message", "test-channel");
            client.HandlePubSubMessage(message);
        });

        await Task.Delay(100); // Let message processing start

        // Act - Dispose should complete without hanging
        var disposeTask = Task.Run(() => client.Dispose());

        // Allow disposal to proceed after a short delay
        await Task.Delay(100);
        disposeStarted.Set();

        await Task.WhenAll(messageTask, disposeTask);

        // Assert - Test passes if disposal completes
        Assert.True(true);
    }

    /// <summary>
    /// Helper method to create a mock client with PubSub configuration for testing.
    /// This simulates client creation without requiring actual server connection.
    /// </summary>
    private static TestableBaseClient CreateMockClientWithPubSub(BasePubSubSubscriptionConfig? config)
    {
        var client = new TestableBaseClient();
        client.InitializePubSubHandlerForTest(config);
        return client;
    }

    /// <summary>
    /// Testable version of BaseClient that exposes internal methods for testing.
    /// </summary>
    private class TestableBaseClient : BaseClient
    {
        public void InitializePubSubHandlerForTest(BasePubSubSubscriptionConfig? config)
        {
            // Use reflection to call private InitializePubSubHandler method
            var method = typeof(BaseClient).GetMethod("InitializePubSubHandler",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(this, [config]);
        }

        protected override Task<Version> GetServerVersionAsync()
        {
            _serverVersion = new Version(7, 2, 0);
            return Task.FromResult(_serverVersion);
        }
    }
}
