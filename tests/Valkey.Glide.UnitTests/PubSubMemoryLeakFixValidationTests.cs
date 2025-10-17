// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Runtime.InteropServices;

using Valkey.Glide.Internals;

using Xunit;

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
            string message = $"test-message-{i}";
            string channel = $"test-channel-{i % 10}";

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
        string message = "pattern test message";
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
                message.Length,
                channelPtr,
                channel.Length,
                patternPtr,
                pattern.Length);

            // Assert
            Assert.Equal(message, result.Message);
            Assert.Equal(channel, result.Channel);
            Assert.Equal(pattern, result.Pattern);
        }
        finally
        {
            // Clean up allocated memory
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
            Marshal.FreeHGlobal(patternPtr);
        }
    }

    [Fact]
    public void MarshalPubSubMessage_WithShardedMessages_HandlesCorrectly()
    {
        // Arrange
        string message = "sharded test message";
        string channel = "shard-channel";

        IntPtr messagePtr = Marshal.StringToHGlobalAnsi(message);
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);

        try
        {
            // Act
            PubSubMessage result = FFI.MarshalPubSubMessage(
                FFI.PushKind.PushSMessage,
                messagePtr,
                message.Length,
                channelPtr,
                channel.Length,
                IntPtr.Zero,
                0);

            // Assert
            Assert.Equal(message, result.Message);
            Assert.Equal(channel, result.Channel);
            Assert.Null(result.Pattern);
        }
        finally
        {
            // Clean up allocated memory
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
        }
    }

    /// <summary>
    /// Helper method to simulate processing a single PubSub message through the FFI marshaling layer.
    /// </summary>
    private static void ProcessSingleMessage(string message, string channel, string? pattern)
    {
        IntPtr messagePtr = Marshal.StringToHGlobalAnsi(message);
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);
        IntPtr patternPtr = pattern != null ? Marshal.StringToHGlobalAnsi(pattern) : IntPtr.Zero;

        try
        {
            FFI.PushKind pushKind = pattern != null ? FFI.PushKind.PushPMessage : FFI.PushKind.PushMessage;

            PubSubMessage result = FFI.MarshalPubSubMessage(
                pushKind,
                messagePtr,
                message.Length,
                channelPtr,
                channel.Length,
                patternPtr,
                pattern?.Length ?? 0);

            // Verify the message was marshaled correctly
            if (result.Message != message || result.Channel != channel || result.Pattern != pattern)
            {
                throw new InvalidOperationException("Message marshaling failed");
            }
        }
        finally
        {
            // Clean up allocated memory
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
            if (patternPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(patternPtr);
            }
        }
    }
}
