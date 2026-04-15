// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Simple validation tests to verify the memory leak fix in FFI message processing.
/// These tests validate that the MarshalPubSubMessage function works correctly
/// without causing memory leaks.
/// </summary>
public class PubSubMemoryLeakFixValidationTests
{
    [Fact]
    public void MarshalPubSubMessage_ProcessMultipleMessages_NoMemoryLeak()
    {
        // Arrange
        const int messageCount = 10_000;

        // Force initial GC to get baseline
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long initialMemory = GC.GetTotalMemory(false);

        // Act: Process multiple messages
        for (int i = 0; i < messageCount; i++)
        {
            byte[] message = Encoding.UTF8.GetBytes($"test-message-{i}");
            byte[] channel = Encoding.UTF8.GetBytes($"test-channel-{i % 10}");

            ProcessSingleMessage(message, channel, null);
        }

        // Force GC to clean up any leaked memory
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long finalMemory = GC.GetTotalMemory(false);
        long memoryGrowth = finalMemory - initialMemory;

        Console.WriteLine($"Processed {messageCount:N0} messages");
        Console.WriteLine($"Memory growth: {memoryGrowth:N0} bytes");

        // Assert: Memory growth should be reasonable (less than 5MB for 10k messages)
        Assert.True(memoryGrowth < 5_000_000,
            $"Excessive memory growth detected: {memoryGrowth:N0} bytes for {messageCount} messages");
    }

    [Fact]
    public void MarshalPubSubMessage_WithPatternMessages_HandlesCorrectly()
    {
        // Arrange
        byte[] message = Encoding.UTF8.GetBytes("pattern test message");
        byte[] channel = Encoding.UTF8.GetBytes("news.sports");
        byte[] pattern = Encoding.UTF8.GetBytes("news.*");

        IntPtr messagePtr = AllocNative(message);
        IntPtr channelPtr = AllocNative(channel);
        IntPtr patternPtr = AllocNative(pattern);

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
            Assert.Equal(message, result.Message);
            Assert.Equal(channel, result.Channel);
            Assert.Equal(pattern, result.Pattern);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
            Marshal.FreeHGlobal(patternPtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithShardedMessages_HandlesCorrectly()
    {
        // Arrange
        byte[] message = Encoding.UTF8.GetBytes("sharded test message");
        byte[] channel = Encoding.UTF8.GetBytes("shard-channel");

        IntPtr messagePtr = AllocNative(message);
        IntPtr channelPtr = AllocNative(channel);

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
            Assert.Equal(message, result.Message);
            Assert.Equal(channel, result.Channel);
            Assert.Null(result.Pattern);
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
        }
    }

    /// <summary>
    /// Allocates native memory and copies the byte array into it, simulating
    /// the Rust-allocated buffers that the real FFI callback receives.
    /// </summary>
    private static IntPtr AllocNative(byte[] data)
    {
        IntPtr ptr = Marshal.AllocHGlobal(data.Length);
        Marshal.Copy(data, 0, ptr, data.Length);
        return ptr;
    }

    /// <summary>
    /// Helper method to simulate processing a single PubSub message through the FFI marshaling layer.
    /// </summary>
    private static void ProcessSingleMessage(byte[] message, byte[] channel, byte[]? pattern)
    {
        IntPtr messagePtr = AllocNative(message);
        IntPtr channelPtr = AllocNative(channel);
        IntPtr patternPtr = pattern != null ? AllocNative(pattern) : IntPtr.Zero;

        try
        {
            FFI.PushKind pushKind = pattern != null ? FFI.PushKind.PushPMessage : FFI.PushKind.PushMessage;

            PubSubMessage result = FFI.MarshalPubSubMessage(
                pushKind,
                messagePtr,
                (ulong)message.Length,
                channelPtr,
                (ulong)channel.Length,
                patternPtr,
                (ulong)(pattern?.Length ?? 0));

            // Verify the message was marshaled correctly
            if (!result.Message.Equals(message) || !result.Channel.Equals(channel) || !Equals(result.Pattern, pattern))
            {
                throw new InvalidOperationException("Message marshaling failed");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
            if (patternPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(patternPtr);
            }
        }
    }
}
