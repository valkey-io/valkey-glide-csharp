// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;

using Valkey.Glide;

using Xunit;

namespace Valkey.Glide.UnitTests;

public class PubSubMessageQueueTests
{
    [Fact]
    public void Constructor_InitializesEmptyQueue()
    {
        // Arrange & Act
        using var queue = new PubSubMessageQueue();

        // Assert
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void TryGetMessage_EmptyQueue_ReturnsFalse()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();

        // Act
        bool result = queue.TryGetMessage(out PubSubMessage? message);

        // Assert
        Assert.False(result);
        Assert.Null(message);
    }

    [Fact]
    public void TryGetMessage_WithMessage_ReturnsTrue()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        var testMessage = new PubSubMessage("test-message", "test-channel");
        queue.EnqueueMessage(testMessage);

        // Act
        bool result = queue.TryGetMessage(out PubSubMessage? message);

        // Assert
        Assert.True(result);
        Assert.NotNull(message);
        Assert.Equal("test-message", message.Message);
        Assert.Equal("test-channel", message.Channel);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void EnqueueMessage_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => queue.EnqueueMessage(null!));
    }

    [Fact]
    public void EnqueueMessage_ValidMessage_IncreasesCount()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        var testMessage = new PubSubMessage("test-message", "test-channel");

        // Act
        queue.EnqueueMessage(testMessage);

        // Assert
        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void EnqueueMessage_MultipleMessages_MaintainsOrder()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        var message1 = new PubSubMessage("message1", "channel1");
        var message2 = new PubSubMessage("message2", "channel2");
        var message3 = new PubSubMessage("message3", "channel3");

        // Act
        queue.EnqueueMessage(message1);
        queue.EnqueueMessage(message2);
        queue.EnqueueMessage(message3);

        // Assert
        Assert.Equal(3, queue.Count);

        Assert.True(queue.TryGetMessage(out PubSubMessage? retrievedMessage1));
        Assert.Equal("message1", retrievedMessage1!.Message);

        Assert.True(queue.TryGetMessage(out PubSubMessage? retrievedMessage2));
        Assert.Equal("message2", retrievedMessage2!.Message);

        Assert.True(queue.TryGetMessage(out PubSubMessage? retrievedMessage3));
        Assert.Equal("message3", retrievedMessage3!.Message);

        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public async Task GetMessageAsync_WithMessage_ReturnsImmediately()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        var testMessage = new PubSubMessage("test-message", "test-channel");
        queue.EnqueueMessage(testMessage);

        // Act
        PubSubMessage result = await queue.GetMessageAsync();

        // Assert
        Assert.Equal("test-message", result.Message);
        Assert.Equal("test-channel", result.Channel);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public async Task GetMessageAsync_EmptyQueue_WaitsForMessage()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        var testMessage = new PubSubMessage("test-message", "test-channel");

        // Act
        Task<PubSubMessage> getMessageTask = queue.GetMessageAsync();

        // Ensure the task is waiting
        await Task.Delay(50);
        Assert.False(getMessageTask.IsCompleted);

        // Enqueue a message
        queue.EnqueueMessage(testMessage);

        // Wait for the task to complete
        PubSubMessage result = await getMessageTask;

        // Assert
        Assert.Equal("test-message", result.Message);
        Assert.Equal("test-channel", result.Channel);
    }

    [Fact]
    public async Task GetMessageAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        using CancellationTokenSource cts = new();

        // Act
        Task<PubSubMessage> getMessageTask = queue.GetMessageAsync(cts.Token);

        // Cancel after a short delay
        cts.CancelAfter(50);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => getMessageTask);
    }

    [Fact]
    public async Task GetMessagesAsync_YieldsMessages()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        var message1 = new PubSubMessage("message1", "channel1");
        var message2 = new PubSubMessage("message2", "channel2");
        var message3 = new PubSubMessage("message3", "channel3");

        queue.EnqueueMessage(message1);
        queue.EnqueueMessage(message2);
        queue.EnqueueMessage(message3);

        // Act
        List<PubSubMessage> messages = [];
        using CancellationTokenSource cts = new();

        await foreach (PubSubMessage message in queue.GetMessagesAsync(cts.Token))
        {
            messages.Add(message);
            if (messages.Count == 3)
            {
                cts.Cancel(); // Stop enumeration after 3 messages
            }
        }

        // Assert
        Assert.Equal(3, messages.Count);
        Assert.Equal("message1", messages[0].Message);
        Assert.Equal("message2", messages[1].Message);
        Assert.Equal("message3", messages[2].Message);
    }

    [Fact]
    public async Task GetMessagesAsync_WithCancellation_StopsEnumeration()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        var message1 = new PubSubMessage("message1", "channel1");
        queue.EnqueueMessage(message1);

        using CancellationTokenSource cts = new();
        cts.CancelAfter(100); // Cancel after 100ms

        // Act
        List<PubSubMessage> messages = [];
        await foreach (PubSubMessage message in queue.GetMessagesAsync(cts.Token))
        {
            messages.Add(message);
            // Don't add more messages, so enumeration will wait and then be cancelled
        }

        // Assert
        Assert.Single(messages);
        Assert.Equal("message1", messages[0].Message);
    }

    [Fact]
    public void TryGetMessage_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var queue = new PubSubMessageQueue();
        queue.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => queue.TryGetMessage(out _));
    }

    [Fact]
    public async Task GetMessageAsync_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var queue = new PubSubMessageQueue();
        queue.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => queue.GetMessageAsync());
    }

    [Fact]
    public void EnqueueMessage_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var queue = new PubSubMessageQueue();
        var testMessage = new PubSubMessage("test-message", "test-channel");
        queue.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => queue.EnqueueMessage(testMessage));
    }

    [Fact]
    public async Task GetMessagesAsync_AfterDispose_StopsEnumeration()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        var message1 = new PubSubMessage("message1", "channel1");
        queue.EnqueueMessage(message1);

        // Act
        List<PubSubMessage> messages = [];
        await foreach (PubSubMessage message in queue.GetMessagesAsync())
        {
            messages.Add(message);
            queue.Dispose(); // Dispose after getting first message
        }

        // Assert
        Assert.Single(messages);
        Assert.Equal("message1", messages[0].Message);
    }

    [Fact]
    public void Dispose_MultipleCalls_DoesNotThrow()
    {
        // Arrange
        var queue = new PubSubMessageQueue();

        // Act & Assert
        queue.Dispose();
        queue.Dispose(); // Should not throw
        queue.Dispose(); // Should not throw
    }

    [Fact]
    public async Task ConcurrentAccess_MultipleThreads_ThreadSafe()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        const int messageCount = 1000;
        const int producerThreads = 5;
        const int consumerThreads = 5;

        ConcurrentBag<string> producedMessages = [];
        ConcurrentBag<string> consumedMessages = [];
        List<Task> tasks = [];

        // Producer tasks
        for (int i = 0; i < producerThreads; i++)
        {
            int threadId = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < messageCount / producerThreads; j++)
                {
                    string messageContent = $"thread-{threadId}-message-{j}";
                    var message = new PubSubMessage(messageContent, $"channel-{threadId}");
                    queue.EnqueueMessage(message);
                    producedMessages.Add(messageContent);
                }
            }));
        }

        // Consumer tasks
        for (int i = 0; i < consumerThreads; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                int messagesConsumed = 0;
                while (messagesConsumed < messageCount / consumerThreads)
                {
                    try
                    {
                        PubSubMessage message = await queue.GetMessageAsync();
                        consumedMessages.Add(message.Message);
                        messagesConsumed++;
                    }
                    catch (ObjectDisposedException)
                    {
                        // Queue was disposed, exit
                        break;
                    }
                }
            }));
        }

        // Act
        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(messageCount, producedMessages.Count);
        Assert.Equal(messageCount, consumedMessages.Count);
        Assert.Equal(0, queue.Count);

        // Verify all produced messages were consumed
        HashSet<string> producedSet = [.. producedMessages];
        HashSet<string> consumedSet = [.. consumedMessages];
        Assert.Equal(producedSet, consumedSet);
    }

    [Fact]
    public async Task ConcurrentTryGetMessage_MultipleThreads_ThreadSafe()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();
        const int messageCount = 100;
        const int consumerThreads = 10;

        // Enqueue messages
        for (int i = 0; i < messageCount; i++)
        {
            var message = new PubSubMessage($"message-{i}", $"channel-{i}");
            queue.EnqueueMessage(message);
        }

        ConcurrentBag<string> consumedMessages = [];
        List<Task> tasks = [];

        // Consumer tasks using TryGetMessage
        for (int i = 0; i < consumerThreads; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                while (queue.TryGetMessage(out PubSubMessage? message))
                {
                    if (message != null)
                    {
                        consumedMessages.Add(message.Message);
                    }
                }
            }));
        }

        // Act
        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(messageCount, consumedMessages.Count);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public async Task DisposeDuringAsyncOperation_CancelsWaitingOperations()
    {
        // Arrange
        using var queue = new PubSubMessageQueue();

        // Start a task that will wait for a message
        Task<PubSubMessage> waitingTask = queue.GetMessageAsync();

        // Ensure the task is waiting
        await Task.Delay(50);
        Assert.False(waitingTask.IsCompleted);

        // Act
        queue.Dispose();

        // Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => waitingTask);
    }
}
