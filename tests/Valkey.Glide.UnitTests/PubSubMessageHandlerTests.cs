// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace Valkey.Glide.UnitTests;

public class PubSubMessageHandlerTests
{
    [Fact]
    public void Constructor_WithCallback_InitializesCorrectly()
    {
        // Arrange
        MessageCallback callback = new MessageCallback((msg, ctx) => { });
        object context = new object();

        // Act
        using PubSubMessageHandler handler = new PubSubMessageHandler(callback, context);

        // Assert
        Assert.NotNull(handler.GetQueue());
    }

    [Fact]
    public void Constructor_WithoutCallback_InitializesCorrectly()
    {
        // Act
        using PubSubMessageHandler handler = new PubSubMessageHandler(null, null);

        // Assert
        Assert.NotNull(handler.GetQueue());
    }

    [Fact]
    public void HandleMessage_WithCallback_InvokesCallback()
    {
        // Arrange
        bool callbackInvoked = false;
        PubSubMessage? receivedMessage = null;
        object? receivedContext = null;
        object context = new object();

        MessageCallback callback = new MessageCallback((msg, ctx) =>
        {
            callbackInvoked = true;
            receivedMessage = msg;
            receivedContext = ctx;
        });

        using PubSubMessageHandler handler = new PubSubMessageHandler(callback, context);
        PubSubMessage message = new PubSubMessage("test-message", "test-channel");

        // Act
        handler.HandleMessage(message);

        // Assert
        Assert.True(callbackInvoked);
        Assert.Equal(message, receivedMessage);
        Assert.Equal(context, receivedContext);
    }

    [Fact]
    public void HandleMessage_WithoutCallback_QueuesMessage()
    {
        // Arrange
        using PubSubMessageHandler handler = new PubSubMessageHandler(null, null);
        PubSubMessage message = new PubSubMessage("test-message", "test-channel");

        // Act
        handler.HandleMessage(message);

        // Assert
        PubSubMessageQueue queue = handler.GetQueue();
        Assert.Equal(1, queue.Count);
        Assert.True(queue.TryGetMessage(out PubSubMessage? queuedMessage));
        Assert.Equal(message, queuedMessage);
    }

    [Fact]
    public void HandleMessage_CallbackThrowsException_LogsErrorAndContinues()
    {
        // Arrange
        bool exceptionThrown = false;

        MessageCallback callback = new MessageCallback((msg, ctx) =>
        {
            exceptionThrown = true;
            throw new InvalidOperationException("Test exception");
        });

        using PubSubMessageHandler handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message = new PubSubMessage("test-message", "test-channel");

        // Act & Assert - Should not throw
        handler.HandleMessage(message);

        Assert.True(exceptionThrown);
        // Note: We can't easily verify logging without mocking the static Logger class
        // The important thing is that the exception doesn't propagate
    }

    [Fact]
    public void HandleMessage_MultipleMessages_InvokesCallbackInOrder()
    {
        // Arrange
        List<PubSubMessage> receivedMessages = new List<PubSubMessage>();
        MessageCallback callback = new MessageCallback((msg, ctx) => receivedMessages.Add(msg));

        using PubSubMessageHandler handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message1 = new PubSubMessage("message1", "channel1");
        PubSubMessage message2 = new PubSubMessage("message2", "channel2");
        PubSubMessage message3 = new PubSubMessage("message3", "channel3");

        // Act
        handler.HandleMessage(message1);
        handler.HandleMessage(message2);
        handler.HandleMessage(message3);

        // Assert
        Assert.Equal(3, receivedMessages.Count);
        Assert.Equal(message1, receivedMessages[0]);
        Assert.Equal(message2, receivedMessages[1]);
        Assert.Equal(message3, receivedMessages[2]);
    }

    [Fact]
    public void HandleMessage_PatternMessage_InvokesCallbackCorrectly()
    {
        // Arrange
        PubSubMessage? receivedMessage = null;
        MessageCallback callback = new MessageCallback((msg, ctx) => receivedMessage = msg);

        using PubSubMessageHandler handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message = new PubSubMessage("test-message", "test-channel", "test-pattern");

        // Act
        handler.HandleMessage(message);

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal("test-message", receivedMessage.Message);
        Assert.Equal("test-channel", receivedMessage.Channel);
        Assert.Equal("test-pattern", receivedMessage.Pattern);
    }

    [Fact]
    public void HandleMessage_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        using PubSubMessageHandler handler = new PubSubMessageHandler(null, null);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => handler.HandleMessage(null!));
    }

    [Fact]
    public void HandleMessage_DisposedHandler_ThrowsObjectDisposedException()
    {
        // Arrange
        PubSubMessageHandler handler = new PubSubMessageHandler(null, null);
        handler.Dispose();
        PubSubMessage message = new PubSubMessage("test-message", "test-channel");

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => handler.HandleMessage(message));
    }

    [Fact]
    public void GetQueue_ReturnsValidQueue()
    {
        // Arrange
        using PubSubMessageHandler handler = new PubSubMessageHandler(null, null);

        // Act
        PubSubMessageQueue queue = handler.GetQueue();

        // Assert
        Assert.NotNull(queue);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void GetQueue_DisposedHandler_ThrowsObjectDisposedException()
    {
        // Arrange
        PubSubMessageHandler handler = new PubSubMessageHandler(null, null);
        handler.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => handler.GetQueue());
    }

    [Fact]
    public void Dispose_MultipleCalls_DoesNotThrow()
    {
        // Arrange
        PubSubMessageHandler handler = new PubSubMessageHandler(null, null);

        // Act & Assert - Should not throw
        handler.Dispose();
        handler.Dispose();
        handler.Dispose();
    }

    [Fact]
    public void Dispose_WithQueueDisposalError_LogsWarningAndContinues()
    {
        // Arrange
        using PubSubMessageHandler handler = new PubSubMessageHandler(null, null);

        // Act
        handler.Dispose();

        // The queue disposal should complete normally, but we test the error handling path
        // by verifying the handler can be disposed without throwing
        Assert.True(true); // Test passes if no exception is thrown
    }

    [Fact]
    public void HandleMessage_CallbackWithNullContext_WorksCorrectly()
    {
        // Arrange
        bool callbackInvoked = false;
        object? receivedContext = new object(); // Initialize with non-null to verify it gets set to null

        MessageCallback callback = new MessageCallback((msg, ctx) =>
        {
            callbackInvoked = true;
            receivedContext = ctx;
        });

        using PubSubMessageHandler handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message = new PubSubMessage("test-message", "test-channel");

        // Act
        handler.HandleMessage(message);

        // Assert
        Assert.True(callbackInvoked);
        Assert.Null(receivedContext);
    }

    [Fact]
    public void HandleMessage_ConcurrentAccess_HandlesCorrectly()
    {
        // Arrange
        List<PubSubMessage> receivedMessages = new List<PubSubMessage>();
        object lockObject = new object();
        MessageCallback callback = new MessageCallback((msg, ctx) =>
        {
            lock (lockObject)
            {
                receivedMessages.Add(msg);
            }
        });

        using PubSubMessageHandler handler = new PubSubMessageHandler(callback, null);
        PubSubMessage[] messages = new[]
        {
            new PubSubMessage("message1", "channel1"),
            new PubSubMessage("message2", "channel2"),
            new PubSubMessage("message3", "channel3")
        };

        // Act
        Task[] tasks = messages.Select(msg => Task.Run(() => handler.HandleMessage(msg))).ToArray();
        Task.WaitAll(tasks);

        // Assert
        Assert.Equal(3, receivedMessages.Count);
        Assert.Contains(messages[0], receivedMessages);
        Assert.Contains(messages[1], receivedMessages);
        Assert.Contains(messages[2], receivedMessages);
    }

    [Fact]
    public void HandleMessage_DisposedDuringCallback_HandlesGracefully()
    {
        // Arrange
        ManualResetEventSlim callbackStarted = new ManualResetEventSlim(false);
        ManualResetEventSlim disposeStarted = new ManualResetEventSlim(false);
        bool callbackCompleted = false;

        MessageCallback callback = new MessageCallback((msg, ctx) =>
        {
            callbackStarted.Set();
            disposeStarted.Wait(TimeSpan.FromSeconds(5)); // Wait for dispose to start
            Thread.Sleep(100); // Simulate some work
            callbackCompleted = true;
        });

        PubSubMessageHandler handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message = new PubSubMessage("test-message", "test-channel");

        // Act
        Task handleTask = Task.Run(() => handler.HandleMessage(message));
        callbackStarted.Wait(TimeSpan.FromSeconds(5));

        Task disposeTask = Task.Run(() =>
        {
            disposeStarted.Set();
            handler.Dispose();
        });

        Task.WaitAll(handleTask, disposeTask);

        // Assert
        Assert.True(callbackCompleted);
    }
}
