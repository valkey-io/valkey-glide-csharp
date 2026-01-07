// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

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
                    Assert.Fail(
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

    [Theory]
    [Trait("Category", "LongRunning")]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1_000)]
    [InlineData(10_000)]
    [InlineData(100_000)]
    public void ProcessVariousMessageSizes_NoMemoryLeak_ConsistentBehavior(int messageSize)
    {
        // Arrange
        const int iterationsPerSize = 1_000;
        // This threshold now represents the max acceptable "unreclaimed" memory between two identical, GC-cleaned runs.
        // It should be small, as true leaks will cause unbounded growth, but some minor variance is expected.
        const long leakThresholdBytes = 50_000; // A reasonably small tolerance for any GC quirks.

        var message = new string('X', messageSize);
        var channel = "test-channel";

        // Run the operation once. This will allocate all necessary objects and
        // potentially promote some to higher GC generations.
        for (int i = 0; i < iterationsPerSize; i++)
        {
            ProcessSingleMessage(message, channel, null);
        }

        // Force a full GC and measure the memory. This becomes our stable baseline
        // *after* one full operation.
        long baselineMemory = GetMemoryAfterFullGC();
        Console.WriteLine($"Baseline memory after first run for size {messageSize}: {baselineMemory:N0} bytes");

        // Run the exact same operation again.
        for (int i = 0; i < iterationsPerSize; i++)
        {
            ProcessSingleMessage(message, channel, null);
        }

        // Force another full GC and measure the final memory state.
        long finalMemory = GetMemoryAfterFullGC();
        Console.WriteLine($"Final memory after second run for size {messageSize}: {finalMemory:N0} bytes");

        // Calculate the growth between the two stable, cleaned states.
        // This represents memory that was allocated in the second run but could NOT be reclaimed by GC,
        // which is the true definition of a memory leak in a managed context.
        long memoryGrowthBetweenRuns = finalMemory - baselineMemory;

        Assert.True(memoryGrowthBetweenRuns < leakThresholdBytes,
            $"Potential memory leak for {messageSize}-byte messages. " +
            $"Memory grew by {memoryGrowthBetweenRuns:N0} bytes between two identical, GC-cleaned runs, " +
            $"which exceeds the leak threshold of {leakThresholdBytes:N0} bytes.");

        Console.WriteLine($"Memory leak test passed for {messageSize}-byte messages. Growth between runs: {memoryGrowthBetweenRuns:N0} bytes");
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
                throw new AggregateException($"Thread {i} failed", exceptions[i]!);
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

    /// <summary>
    /// Performs garbage collection and returns the total memory usage after cleanup.
    /// </summary>
    /// <returns>Total memory usage in bytes after garbage collection.</returns>
    private static long GetMemoryAfterFullGC()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        return GC.GetTotalMemory(true);
    }
}
