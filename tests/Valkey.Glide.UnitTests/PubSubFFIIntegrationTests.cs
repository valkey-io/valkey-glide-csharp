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
        string message = "test message";
        string channel = "test-channel";

        IntPtr messagePtr = Marshal.StringToHGlobalAnsi(message);
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);

        try
        {
            // Act
            PubSubMessage result = FFI.MarshalPubSubMessage(
                FFI.PushKind.PushMessage,
                messagePtr,
                (ulong)message.Length,
                channelPtr,
                (ulong)channel.Length,
                IntPtr.Zero,
                0);

            // Assert
            Assert.Equal("test message", result.Message);
            Assert.Equal("test-channel", result.Channel);
            Assert.Null(result.Pattern);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithValidPatternMessage_ReturnsCorrectMessage()
    {
        // Arrange
        string message = "pattern message";
        string channel = "news.sports";
        string pattern = "news.*";

        IntPtr messagePtr = Marshal.StringToHGlobalAnsi(message);
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);
        IntPtr patternPtr = Marshal.StringToHGlobalAnsi(pattern);

        try
        {
            // Act
            PubSubMessage result = FFI.MarshalPubSubMessage(
                FFI.PushKind.PushPMessage,
                messagePtr,
                (ulong)message.Length,
                channelPtr,
                (ulong)channel.Length,
                patternPtr,
                (ulong)pattern.Length);

            // Assert
            Assert.Equal("pattern message", result.Message);
            Assert.Equal("news.sports", result.Channel);
            Assert.Equal("news.*", result.Pattern);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
            Marshal.FreeHGlobal(patternPtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithNullMessagePointer_ThrowsArgumentException()
    {
        // Arrange
        string channel = "test-channel";
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);

        try
        {
            // Act & Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() =>
                FFI.MarshalPubSubMessage(
                    FFI.PushKind.PushMessage,
                    IntPtr.Zero,
                    0,
                    channelPtr,
                    (ulong)channel.Length,
                    IntPtr.Zero,
                    0));
            Assert.Contains("Invalid message data", ex.Message);
        }
        finally
        {
            Marshal.FreeHGlobal(channelPtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithEmptyMessage_ThrowsArgumentException()
    {
        // Arrange
        string message = "";
        string channel = "test-channel";

        IntPtr messagePtr = Marshal.StringToHGlobalAnsi(message);
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);

        try
        {
            // Act & Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() =>
                FFI.MarshalPubSubMessage(
                    FFI.PushKind.PushMessage,
                    messagePtr,
                    (ulong)message.Length,
                    channelPtr,
                    (ulong)channel.Length,
                    IntPtr.Zero,
                    0));
            Assert.Contains("PubSub message content cannot be null or empty after marshaling", ex.Message);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithEmptyChannel_ThrowsArgumentException()
    {
        // Arrange
        string message = "test message";
        string channel = "";

        IntPtr messagePtr = Marshal.StringToHGlobalAnsi(message);
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);

        try
        {
            // Act & Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() =>
                FFI.MarshalPubSubMessage(
                    FFI.PushKind.PushMessage,
                    messagePtr,
                    (ulong)message.Length,
                    channelPtr,
                    (ulong)channel.Length,
                    IntPtr.Zero,
                    0));
            Assert.Contains("Invalid channel data: length is zero", ex.Message);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithShardedMessage_ReturnsCorrectMessage()
    {
        // Arrange
        string message = "sharded message";
        string channel = "shard-channel";

        IntPtr messagePtr = Marshal.StringToHGlobalAnsi(message);
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);

        try
        {
            // Act
            PubSubMessage result = FFI.MarshalPubSubMessage(
                FFI.PushKind.PushSMessage,
                messagePtr,
                (ulong)message.Length,
                channelPtr,
                (ulong)channel.Length,
                IntPtr.Zero,
                0);

            // Assert
            Assert.Equal("sharded message", result.Message);
            Assert.Equal("shard-channel", result.Channel);
            Assert.Null(result.Pattern);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
        }
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
