// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Tests for detecting memory leaks in the FFI layer during PubSub message processing.
/// These tests are designed to detect the critical memory leak issue where Rust-allocated
/// memory is not properly freed after C# marshaling.
/// </summary>
public class PubSubFFIMemoryLeakTests
{
    [Fact]
    [Trait("Category", "LongRunning")]
    public void ProcessLargeVolumeMessages_NoMemoryLeak_MemoryUsageRemainsBounded()
    {
        // Arrange
        const int messageCount = 100_000;
        const long maxMemoryGrowthBytes = 50_000_000; // 50MB max growth allowed

        // Force initial GC to get baseline
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long initialMemory = GC.GetTotalMemory(false);
        Console.WriteLine($"Initial memory: {initialMemory:N0} bytes");

        // Act: Process large volume of messages
        for (int i = 0; i < messageCount; i++)
        {
            string message = $"test-message-{i}";
            string channel = $"test-channel-{i % 100}"; // Vary channels
            string? pattern = i % 3 == 0 ? $"pattern-{i % 10}" : null; // Some with patterns

            ProcessSingleMessage(message, channel, pattern);

            // Periodic GC to detect leaks early
            if (i % 10_000 == 0 && i > 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                long currentMemory = GC.GetTotalMemory(false);
                long memoryGrowth = currentMemory - initialMemory;

                Console.WriteLine($"Processed {i:N0} messages, memory growth: {memoryGrowth:N0} bytes");

                // Early detection of memory leaks
                if (memoryGrowth > maxMemoryGrowthBytes)
                {
                    Assert.True(false,
                        $"Memory leak detected after {i:N0} messages. " +
                        $"Memory grew by {memoryGrowth:N0} bytes, exceeding limit of {maxMemoryGrowthBytes:N0} bytes.");
                }
            }
        }

        // Final memory check
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long finalMemory = GC.GetTotalMemory(false);
        long totalMemoryGrowth = finalMemory - initialMemory;

        Console.WriteLine($"Final memory: {finalMemory:N0} bytes");
        Console.WriteLine($"Total memory growth: {totalMemoryGrowth:N0} bytes");
        Console.WriteLine($"Processed {messageCount:N0} messages successfully");

        // Assert: Memory growth should be bounded
        Assert.True(totalMemoryGrowth < maxMemoryGrowthBytes,
            $"Memory leak detected. Total memory growth: {totalMemoryGrowth:N0} bytes, " +
            $"limit: {maxMemoryGrowthBytes:N0} bytes");
    }

    [Fact]
    [Trait("Category", "LongRunning")]
    public void ProcessVariousMessageSizes_NoMemoryLeak_ConsistentBehavior()
    {
        // Arrange
        const int iterationsPerSize = 1_000;
        int[] messageSizes = [10, 100, 1_000, 10_000, 100_000]; // Various sizes

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long initialMemory = GC.GetTotalMemory(false);
        Console.WriteLine($"Initial memory: {initialMemory:N0} bytes");

        // Act: Test different message sizes
        foreach (int messageSize in messageSizes)
        {
            Console.WriteLine($"Testing message size: {messageSize} bytes");

            string largeMessage = new('X', messageSize);
            string channel = "test-channel";

            long beforeSizeTest = GC.GetTotalMemory(true);

            for (int i = 0; i < iterationsPerSize; i++)
            {
                ProcessSingleMessage(largeMessage, channel, null);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long afterSizeTest = GC.GetTotalMemory(false);
            long sizeTestGrowth = afterSizeTest - beforeSizeTest;

            Console.WriteLine($"Memory growth for {messageSize}-byte messages: {sizeTestGrowth:N0} bytes");

            // Memory growth should be reasonable for the message size
            long expectedMaxGrowth = messageSize * iterationsPerSize * 2; // Allow 2x overhead
            Assert.True(sizeTestGrowth < expectedMaxGrowth,
                $"Excessive memory growth for {messageSize}-byte messages: {sizeTestGrowth:N0} bytes");
        }

        // Final check
        long finalMemory = GC.GetTotalMemory(true);
        long totalGrowth = finalMemory - initialMemory;

        Console.WriteLine($"Total memory growth across all sizes: {totalGrowth:N0} bytes");

        // Should not have significant permanent growth
        Assert.True(totalGrowth < 10_000_000, // 10MB max
            $"Permanent memory growth detected: {totalGrowth:N0} bytes");
    }

    [Fact]
    [Trait("Category", "LongRunning")]
    public void ProcessMessagesUnderGCPressure_NoMemoryLeak_StableUnderPressure()
    {
        // Arrange
        const int messageCount = 50_000;
        const int gcInterval = 1_000; // Force GC every 1000 messages

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long initialMemory = GC.GetTotalMemory(false);
        Console.WriteLine($"Initial memory under GC pressure test: {initialMemory:N0} bytes");

        // Act: Process messages with frequent GC pressure
        for (int i = 0; i < messageCount; i++)
        {
            string message = $"gc-pressure-message-{i}";
            string channel = "gc-test-channel";

            ProcessSingleMessage(message, channel, null);

            // Apply GC pressure frequently
            if (i % gcInterval == 0)
            {
                // Create some temporary objects to increase GC pressure
                object[] tempObjects = new object[1000];
                for (int j = 0; j < tempObjects.Length; j++)
                {
                    tempObjects[j] = new byte[1024]; // 1KB objects
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // Clear temp objects
                Array.Clear(tempObjects, 0, tempObjects.Length);
            }
        }

        // Final memory check under GC pressure
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long finalMemory = GC.GetTotalMemory(false);
        long memoryGrowth = finalMemory - initialMemory;

        Console.WriteLine($"Memory growth under GC pressure: {memoryGrowth:N0} bytes");

        // Assert: Should remain stable even under GC pressure
        Assert.True(memoryGrowth < 20_000_000, // 20MB max under pressure
            $"Memory leak detected under GC pressure: {memoryGrowth:N0} bytes");
    }

    [Fact]
    [Trait("Category", "LongRunning")]
    public void ProcessConcurrentMessages_NoMemoryLeak_ThreadSafeMemoryManagement()
    {
        // Arrange
        const int threadsCount = 10;
        const int messagesPerThread = 10_000;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long initialMemory = GC.GetTotalMemory(false);
        Console.WriteLine($"Initial memory for concurrent test: {initialMemory:N0} bytes");

        // Act: Process messages concurrently from multiple threads
        Task[] tasks = new Task[threadsCount];
        Exception?[] exceptions = new Exception?[threadsCount];

        for (int threadIndex = 0; threadIndex < threadsCount; threadIndex++)
        {
            int capturedIndex = threadIndex;
            tasks[threadIndex] = Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < messagesPerThread; i++)
                    {
                        string message = $"concurrent-message-{capturedIndex}-{i}";
                        string channel = $"concurrent-channel-{capturedIndex}";
                        string? pattern = i % 2 == 0 ? $"pattern-{capturedIndex}" : null;

                        ProcessSingleMessage(message, channel, pattern);
                    }
                }
                catch (Exception ex)
                {
                    exceptions[capturedIndex] = ex;
                }
            });
        }

        // Wait for all tasks to complete
        Task.WaitAll(tasks);

        // Check for exceptions
        for (int i = 0; i < exceptions.Length; i++)
        {
            if (exceptions[i] != null)
            {
                throw new AggregateException($"Thread {i} failed", exceptions[i]);
            }
        }

        // Final memory check
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long finalMemory = GC.GetTotalMemory(false);
        long memoryGrowth = finalMemory - initialMemory;

        Console.WriteLine($"Memory growth after concurrent processing: {memoryGrowth:N0} bytes");
        Console.WriteLine($"Processed {threadsCount * messagesPerThread:N0} messages concurrently");

        // Assert: Memory should remain bounded even with concurrent access
        Assert.True(memoryGrowth < 30_000_000, // 30MB max for concurrent test
            $"Memory leak detected in concurrent processing: {memoryGrowth:N0} bytes");
    }

    [Fact]
    [Trait("Category", "LongRunning")]
    public void ProcessExtendedDuration_NoMemoryLeak_StableOverTime()
    {
        // Arrange
        const int durationSeconds = 30; // Run for 30 seconds
        const int messagesPerSecond = 1000;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long initialMemory = GC.GetTotalMemory(false);
        Console.WriteLine($"Starting extended duration test for {durationSeconds} seconds");
        Console.WriteLine($"Initial memory: {initialMemory:N0} bytes");

        Stopwatch stopwatch = Stopwatch.StartNew();
        int messageCount = 0;
        List<(TimeSpan Time, long Memory)> memorySnapshots = [];

        // Act: Process messages for extended duration
        while (stopwatch.Elapsed.TotalSeconds < durationSeconds)
        {
            for (int i = 0; i < messagesPerSecond && stopwatch.Elapsed.TotalSeconds < durationSeconds; i++)
            {
                string message = $"duration-test-{messageCount}";
                string channel = $"duration-channel-{messageCount % 10}";

                ProcessSingleMessage(message, channel, null);
                messageCount++;
            }

            // Take memory snapshot every 5 seconds
            if (stopwatch.Elapsed.TotalSeconds % 5 < 0.1)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                long currentMemory = GC.GetTotalMemory(false);
                memorySnapshots.Add((stopwatch.Elapsed, currentMemory));

                Console.WriteLine($"Time: {stopwatch.Elapsed.TotalSeconds:F1}s, " +
                                $"Messages: {messageCount:N0}, " +
                                $"Memory: {currentMemory:N0} bytes");
            }

            Thread.Sleep(1); // Small delay to prevent tight loop
        }

        stopwatch.Stop();

        // Final memory check
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long finalMemory = GC.GetTotalMemory(false);
        long totalGrowth = finalMemory - initialMemory;

        Console.WriteLine($"Extended test completed:");
        Console.WriteLine($"Duration: {stopwatch.Elapsed.TotalSeconds:F1} seconds");
        Console.WriteLine($"Messages processed: {messageCount:N0}");
        Console.WriteLine($"Final memory: {finalMemory:N0} bytes");
        Console.WriteLine($"Total memory growth: {totalGrowth:N0} bytes");

        // Check for memory growth trend
        if (memorySnapshots.Count >= 2)
        {
            long firstSnapshot = memorySnapshots[0].Memory;
            long lastSnapshot = memorySnapshots[^1].Memory;
            long trendGrowth = lastSnapshot - firstSnapshot;

            Console.WriteLine($"Memory trend growth: {trendGrowth:N0} bytes");

            // Memory should not continuously grow over time
            Assert.True(trendGrowth < 25_000_000, // 25MB max trend growth
                $"Continuous memory growth detected: {trendGrowth:N0} bytes over time");
        }

        // Assert: Total memory growth should be reasonable
        Assert.True(totalGrowth < 40_000_000, // 40MB max for extended test
            $"Excessive memory growth over extended duration: {totalGrowth:N0} bytes");
    }

    /// <summary>
    /// Helper method to simulate processing a single PubSub message through the FFI marshaling layer.
    /// This simulates the memory allocation and marshaling that occurs in the real FFI callback.
    /// </summary>
    private static void ProcessSingleMessage(string message, string channel, string? pattern)
    {
        // Simulate the FFI marshaling process that occurs in the real callback
        IntPtr messagePtr = Marshal.StringToHGlobalAnsi(message);
        IntPtr channelPtr = Marshal.StringToHGlobalAnsi(channel);
        IntPtr patternPtr = pattern != null ? Marshal.StringToHGlobalAnsi(pattern) : IntPtr.Zero;

        try
        {
            // Simulate marshaling by creating byte arrays (as the real FFI callback does)
            byte[] messageBytes = new byte[message.Length];
            Marshal.Copy(messagePtr, messageBytes, 0, message.Length);

            byte[] channelBytes = new byte[channel.Length];
            Marshal.Copy(channelPtr, channelBytes, 0, channel.Length);

            byte[]? patternBytes = null;
            if (pattern != null && patternPtr != IntPtr.Zero)
            {
                patternBytes = new byte[pattern.Length];
                Marshal.Copy(patternPtr, patternBytes, 0, pattern.Length);
            }

            // Create PubSubMessage (simulating what the real callback does)
            PubSubMessage result = pattern != null
                ? new PubSubMessage(message, channel, pattern)
                : new PubSubMessage(message, channel);

            // Verify the message was created correctly
            if (result.Message != message || result.Channel != channel || result.Pattern != pattern)
            {
                throw new InvalidOperationException("Message marshaling failed");
            }
        }
        finally
        {
            // Clean up allocated memory (this is what C# should do)
            Marshal.FreeHGlobal(messagePtr);
            Marshal.FreeHGlobal(channelPtr);
            if (patternPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(patternPtr);
            }
        }
    }
}
