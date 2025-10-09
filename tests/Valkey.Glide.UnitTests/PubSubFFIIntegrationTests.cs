// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Runtime.InteropServices;
using Valkey.Glide.Internals;
using Xunit;

namespace Valkey.Glide.UnitTests;

public class PubSubFFIIntegrationTests
{
    [Fact]
    public void MarshalPubSubMessage_WithValidExactChannelMessage_ReturnsCorrectMessage()
    {
        // Arrange
        var messageInfo = new FFI.PubSubMessageInfo
        {
            Message = "test message",
            Channel = "test-channel",
            Pattern = null
        };

        IntPtr messagePtr = Marshal.AllocHGlobal(Marshal.SizeOf<FFI.PubSubMessageInfo>());
        try
        {
            Marshal.StructureToPtr(messageInfo, messagePtr, false);

            // Act
            PubSubMessage result = FFI.MarshalPubSubMessage(messagePtr);

            // Assert
            Assert.Equal("test message", result.Message);
            Assert.Equal("test-channel", result.Channel);
            Assert.Null(result.Pattern);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithValidPatternMessage_ReturnsCorrectMessage()
    {
        // Arrange
        var messageInfo = new FFI.PubSubMessageInfo
        {
            Message = "pattern message",
            Channel = "news.sports",
            Pattern = "news.*"
        };

        IntPtr messagePtr = Marshal.AllocHGlobal(Marshal.SizeOf<FFI.PubSubMessageInfo>());
        try
        {
            Marshal.StructureToPtr(messageInfo, messagePtr, false);

            // Act
            PubSubMessage result = FFI.MarshalPubSubMessage(messagePtr);

            // Assert
            Assert.Equal("pattern message", result.Message);
            Assert.Equal("news.sports", result.Channel);
            Assert.Equal("news.*", result.Pattern);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithNullPointer_ThrowsArgumentException()
    {
        // Act & Assert
        ArgumentException ex = Assert.Throws<ArgumentException>(() => FFI.MarshalPubSubMessage(IntPtr.Zero));
        Assert.Contains("Invalid PubSub message pointer", ex.Message);
    }

    [Fact]
    public void MarshalPubSubMessage_WithEmptyMessage_ThrowsArgumentException()
    {
        // Arrange
        var messageInfo = new FFI.PubSubMessageInfo
        {
            Message = "",
            Channel = "test-channel",
            Pattern = null
        };

        IntPtr messagePtr = Marshal.AllocHGlobal(Marshal.SizeOf<FFI.PubSubMessageInfo>());
        try
        {
            Marshal.StructureToPtr(messageInfo, messagePtr, false);

            // Act & Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() => FFI.MarshalPubSubMessage(messagePtr));
            Assert.Contains("PubSub message content cannot be null or empty", ex.Message);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithEmptyChannel_ThrowsArgumentException()
    {
        // Arrange
        var messageInfo = new FFI.PubSubMessageInfo
        {
            Message = "test message",
            Channel = "",
            Pattern = null
        };

        IntPtr messagePtr = Marshal.AllocHGlobal(Marshal.SizeOf<FFI.PubSubMessageInfo>());
        try
        {
            Marshal.StructureToPtr(messageInfo, messagePtr, false);

            // Act & Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() => FFI.MarshalPubSubMessage(messagePtr));
            Assert.Contains("PubSub message channel cannot be null or empty", ex.Message);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
        }
    }

    [Fact]
    public void CreatePubSubCallbackPtr_WithValidCallback_ReturnsNonZeroPointer()
    {
        // Arrange
        FFI.PubSubMessageCallback callback = (clientId, messagePtr) => { };

        // Act
        IntPtr result = FFI.CreatePubSubCallbackPtr(callback);

        // Assert
        Assert.NotEqual(IntPtr.Zero, result);
    }

    [Fact]
    public void PubSubCallbackManager_RegisterAndUnregisterClient_WorksCorrectly()
    {
        // This test verifies the basic functionality of the callback manager
        // without actually invoking native callbacks

        // Arrange
        ulong clientId = 12345;
        var mockClient = new MockBaseClient();

        // Act - Register
        PubSubCallbackManager.RegisterClient(clientId, mockClient);

        // Act - Get callback pointer (should not throw)
        IntPtr callbackPtr = PubSubCallbackManager.GetNativeCallbackPtr();

        // Assert
        Assert.NotEqual(IntPtr.Zero, callbackPtr);

        // Act - Unregister
        PubSubCallbackManager.UnregisterClient(clientId);

        // No exception should be thrown
    }

    // Mock class for testing
    private class MockBaseClient : BaseClient
    {
        protected override Task InitializeServerVersionAsync()
        {
            return Task.CompletedTask;
        }

        internal override void HandlePubSubMessage(PubSubMessage message)
        {
            // Mock implementation
        }
    }
}
