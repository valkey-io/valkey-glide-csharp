// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class PubSubMessageHandlerTests
{
    [Fact]
    public void Constructor_WithCallback_InitializesCorrectly()
    {
        // Arrange
        var callback = new MessageCallback((msg, ctx) => { });
        var context = new object();

        // Act
        using var handler = new PubSubMessageHandler(callback, context);

        // Assert - GetQueue should throw when callback is configured
        _ = Assert.Throws<InvalidOperationException>(handler.GetQueue);
    }

    [Fact]
    public void Constructor_WithoutCallback_InitializesCorrectly()
    {
        // Act
        using var handler = new PubSubMessageHandler(null, null);

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
        var context = new object();

        var callback = new MessageCallback((msg, ctx) =>
        {
            callbackInvoked = true;
            receivedMessage = msg;
            receivedContext = ctx;
        });

        using var handler = new PubSubMessageHandler(callback, context);
        PubSubMessage message = PubSubMessage.FromChannel("test-message", "test-channel");

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
        using var handler = new PubSubMessageHandler(null, null);
        PubSubMessage message = PubSubMessage.FromChannel("test-message", "test-channel");

        // Act
        handler.HandleMessage(message);

        // Assert
        PubSubMessageQueue queue = handler.GetQueue();
        Assert.Equal(1, queue.Count);
        Assert.True(queue.TryGetMessage(out PubSubMessage? queuedMessage));
        Assert.Equal(message, queuedMessage);
    }

    [Fact]
    public void HandleMessage_CallbackThrowsException_DoesNotPropagate()
    {
        // Arrange
        bool exceptionThrown = false;

        var callback = new MessageCallback((msg, ctx) =>
        {
            exceptionThrown = true;
            throw new InvalidOperationException("Test exception");
        });

        using var handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message = PubSubMessage.FromChannel("test-message", "test-channel");

        // Act & Assert - Exception should be caught and not propagate
        handler.HandleMessage(message);

        Assert.True(exceptionThrown);
    }

    [Fact]
    public void HandleMessage_MultipleMessages_InvokesCallbackInOrder()
    {
        // Arrange
        List<PubSubMessage> receivedMessages = [];
        var callback = new MessageCallback((msg, ctx) => receivedMessages.Add(msg));

        using var handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message1 = PubSubMessage.FromChannel("message1", "channel1");
        PubSubMessage message2 = PubSubMessage.FromChannel("message2", "channel2");
        PubSubMessage message3 = PubSubMessage.FromChannel("message3", "channel3");

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
        var callback = new MessageCallback((msg, ctx) => receivedMessage = msg);

        using var handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message = PubSubMessage.FromPattern("test-message", "test-channel", "test-pattern");

        // Act
        handler.HandleMessage(message);

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal(PubSubChannelMode.Pattern, receivedMessage.ChannelMode);
        Assert.Equal("test-message", receivedMessage.Message);
        Assert.Equal("test-channel", receivedMessage.Channel);
        Assert.Equal("test-pattern", receivedMessage.Pattern);
    }

    [Fact]
    public void HandleMessage_NullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        using var handler = new PubSubMessageHandler(null, null);

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => handler.HandleMessage(null!));
    }

    [Fact]
    public void HandleMessage_DisposedHandler_ThrowsObjectDisposedException()
    {
        // Arrange
        var handler = new PubSubMessageHandler(null, null);
        handler.Dispose();
        PubSubMessage message = PubSubMessage.FromChannel("test-message", "test-channel");

        // Act & Assert
        _ = Assert.Throws<ObjectDisposedException>(() => handler.HandleMessage(message));
    }

    [Fact]
    public void GetQueue_ReturnsValidQueue()
    {
        // Arrange
        using var handler = new PubSubMessageHandler(null, null);

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
        var handler = new PubSubMessageHandler(null, null);
        handler.Dispose();

        // Act & Assert
        _ = Assert.Throws<ObjectDisposedException>(handler.GetQueue);
    }

    [Fact]
    public void Dispose_MultipleCalls_DoesNotThrow()
    {
        // Arrange
        var handler = new PubSubMessageHandler(null, null);

        // Act & Assert - Should not throw
        handler.Dispose();
        handler.Dispose();
        handler.Dispose();
    }

    [Fact]
    public void HandleMessage_CallbackWithNullContext_WorksCorrectly()
    {
        // Arrange
        bool callbackInvoked = false;
        var receivedContext = new object(); // Initialize with non-null to verify it gets set to null

        var callback = new MessageCallback((msg, ctx) =>
        {
            callbackInvoked = true;
            receivedContext = ctx;
        });

        using var handler = new PubSubMessageHandler(callback, null);
        PubSubMessage message = PubSubMessage.FromChannel("test-message", "test-channel");

        // Act
        handler.HandleMessage(message);

        // Assert
        Assert.True(callbackInvoked);
        Assert.Null(receivedContext);
    }

    [Fact]
    public async Task HandleMessage_ConcurrentAccess_HandlesCorrectly()
    {
        // Arrange
        List<PubSubMessage> receivedMessages = [];
        var lockObject = new object();
        var callback = new MessageCallback((msg, ctx) =>
        {
            lock (lockObject)
            {
                receivedMessages.Add(msg);
            }
        });

        using var handler = new PubSubMessageHandler(callback, null);
        PubSubMessage[] messages =
        [
            PubSubMessage.FromChannel("message1", "channel1"),
            PubSubMessage.FromChannel("message2", "channel2"),
            PubSubMessage.FromChannel("message3", "channel3")
        ];

        // Act
        Task[] tasks = [.. messages.Select(msg => Task.Run(() => handler.HandleMessage(msg), TestContext.Current.CancellationToken))];
        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(3, receivedMessages.Count);
        Assert.Contains(messages[0], receivedMessages);
        Assert.Contains(messages[1], receivedMessages);
        Assert.Contains(messages[2], receivedMessages);
    }

    [Fact]
    public async Task HandleMessage_DisposedDuringCallback_HandlesGracefully()
    {
        // Arrange
        var callbackStarted = new ManualResetEventSlim(false);
        var disposeStarted = new ManualResetEventSlim(false);
        bool callbackCompleted = false;

        var callback = new MessageCallback((msg, ctx) =>
        {
            callbackStarted.Set();
            _ = disposeStarted.Wait(TimeSpan.FromSeconds(5)); // Wait for dispose to start
            Thread.Sleep(100); // Simulate some work
            callbackCompleted = true;
        });

        var handler = new PubSubMessageHandler(callback, null);
        var message = PubSubMessage.FromChannel("test-message", "test-channel");

        // Act
        Task handleTask = Task.Run(() => handler.HandleMessage(message), TestContext.Current.CancellationToken);
        _ = callbackStarted.Wait(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Task disposeTask = Task.Run(
            () =>
            {
                disposeStarted.Set();
                handler.Dispose();
            },
            TestContext.Current.CancellationToken);

        await Task.WhenAll(handleTask, disposeTask);

        // Assert
        Assert.True(callbackCompleted);
    }
}
