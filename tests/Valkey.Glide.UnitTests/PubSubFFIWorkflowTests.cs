// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;

using Valkey.Glide.Internals;

using Xunit;

namespace Valkey.Glide.UnitTests;

public class PubSubFFIWorkflowTests
{
    [Fact]
    public void BaseClient_WithPubSubConfiguration_InitializesPubSubHandler()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel");

        // Note: The constructor doesn't directly accept pubSubSubscriptions parameter
        // This is just for demonstration of the configuration structure

        // Create a mock client to test initialization
        var mockClient = new TestableBaseClient();

        // Act
        mockClient.TestInitializePubSubHandler(pubSubConfig);

        // Assert
        Assert.True(mockClient.HasPubSubSubscriptions);
        Assert.NotNull(mockClient.PubSubQueue);
    }

    [Fact]
    public void BaseClient_WithoutPubSubConfiguration_DoesNotInitializePubSubHandler()
    {
        // Arrange
        var mockClient = new TestableBaseClient();

        // Act
        mockClient.TestInitializePubSubHandler(null);

        // Assert
        Assert.False(mockClient.HasPubSubSubscriptions);
        Assert.Null(mockClient.PubSubQueue);
    }

    [Fact]
    public void BaseClient_HandlePubSubMessage_RoutesToHandler()
    {
        // Arrange
        PubSubMessage? receivedMessage = null;
        MessageCallback callback = (msg, ctx) => receivedMessage = msg;

        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel")
            .WithCallback<StandalonePubSubSubscriptionConfig>(callback);

        var mockClient = new TestableBaseClient();
        mockClient.TestInitializePubSubHandler(pubSubConfig);

        var testMessage = new PubSubMessage("test message", "test-channel");

        // Act
        mockClient.HandlePubSubMessage(testMessage);

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal("test message", receivedMessage.Message);
        Assert.Equal("test-channel", receivedMessage.Channel);
    }

    [Fact]
    public void BaseClient_HandlePubSubMessage_WithoutCallback_QueuesToMessageQueue()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel");

        var mockClient = new TestableBaseClient();
        mockClient.TestInitializePubSubHandler(pubSubConfig);

        var testMessage = new PubSubMessage("queued message", "test-channel");

        // Act
        mockClient.HandlePubSubMessage(testMessage);

        // Assert
        Assert.NotNull(mockClient.PubSubQueue);
        bool hasMessage = mockClient.PubSubQueue.TryGetMessage(out PubSubMessage? queuedMessage);
        Assert.True(hasMessage);
        Assert.NotNull(queuedMessage);
        Assert.Equal("queued message", queuedMessage.Message);
        Assert.Equal("test-channel", queuedMessage.Channel);
    }

    [Fact]
    public void PubSubCallbackManager_RegisterUnregisterWorkflow_HandlesClientLifecycle()
    {
        // Arrange
        ulong clientId = 98765;
        var mockClient = new TestableBaseClient();

        // Act - Register
        PubSubCallbackManager.RegisterClient(clientId, mockClient);
        IntPtr callbackPtr = PubSubCallbackManager.GetNativeCallbackPtr();

        // Assert - Registration
        Assert.NotEqual(IntPtr.Zero, callbackPtr);

        // Act - Unregister
        PubSubCallbackManager.UnregisterClient(clientId);

        // Assert - No exceptions should be thrown during unregistration
        // The callback manager should handle missing clients gracefully
    }

    [Fact]
    public void BaseClient_CleanupPubSubResources_HandlesDisposal()
    {
        // Arrange
        var pubSubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel("test-channel");

        var mockClient = new TestableBaseClient();
        mockClient.TestInitializePubSubHandler(pubSubConfig);

        // Verify initialization
        Assert.True(mockClient.HasPubSubSubscriptions);

        // Act
        mockClient.TestCleanupPubSubResources();

        // Assert
        Assert.False(mockClient.HasPubSubSubscriptions);
        Assert.Null(mockClient.PubSubQueue);
    }

    // Testable version of BaseClient that exposes internal methods for testing
    private class TestableBaseClient : BaseClient
    {
        protected override Task InitializeServerVersionAsync()
        {
            return Task.CompletedTask;
        }

        public void TestInitializePubSubHandler(BasePubSubSubscriptionConfig? config)
        {
            // Use reflection to call the private method
            var method = typeof(BaseClient).GetMethod("InitializePubSubHandler",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(this, [config]);
        }

        public void TestCleanupPubSubResources()
        {
            // Use reflection to call the private method
            var method = typeof(BaseClient).GetMethod("CleanupPubSubResources",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(this, null);
        }
    }
}
