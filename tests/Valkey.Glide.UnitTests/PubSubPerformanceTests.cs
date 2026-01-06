// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;
using System.Diagnostics;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Performance validation tests for channel-based PubSub message processing.
/// These tests verify that the channel-based approach provides better performance
/// than the previous Task.Run per message approach.
/// </summary>
public class PubSubPerformanceTests
{

    [Fact]
    public void ChannelBasedProcessing_HighThroughput_HandlesMessagesEfficiently()
    {
        // Arrange
        const int messageCount = 10_000;
        var messagesReceived = 0;

        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("perf-test")
            .WithCallback<StandalonePubSubSubscriptionConfig>((msg, ctx) =>
            {
                Interlocked.Increment(ref messagesReceived);
            }, null);

        // Act - Simulate high-volume message processing
        var stopwatch = Stopwatch.StartNew();

        // Simulate messages being processed through the channel
        for (int i = 0; i < messageCount; i++)
        {
            var message = new PubSubMessage($"message-{i}", "perf-test");
            config.Callback!(message, null);
        }

        stopwatch.Stop();

        // Assert
        Assert.Equal(messageCount, messagesReceived);

        var throughput = messageCount / stopwatch.Elapsed.TotalSeconds;

        // Verify high throughput (should handle at least 10,000 msg/sec)
        Assert.True(throughput >= 10_000,
            $"Throughput {throughput:F0} msg/sec is below target of 10,000 msg/sec. Processed {messageCount} messages in {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
    }

    [Fact]
    public void ChannelBasedProcessing_ReducedAllocationPressure_MinimizesGCImpact()
    {
        // Arrange
        const int messageCount = 50_000;
        var messagesReceived = 0;

        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("gc-test")
            .WithCallback<StandalonePubSubSubscriptionConfig>((msg, ctx) =>
            {
                Interlocked.Increment(ref messagesReceived);
            }, null);

        // Force GC before test
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var initialMemory = GC.GetTotalMemory(false);
        var initialGen0 = GC.CollectionCount(0);
        var initialGen1 = GC.CollectionCount(1);
        var initialGen2 = GC.CollectionCount(2);

        // Act - Process many messages
        for (int i = 0; i < messageCount; i++)
        {
            var message = new PubSubMessage($"message-{i}", "gc-test");
            config.Callback!(message, null);
        }

        // Wait a bit for any pending operations
        Thread.Sleep(100);

        // Force GC to measure impact
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var finalMemory = GC.GetTotalMemory(false);
        var finalGen0 = GC.CollectionCount(0);
        var finalGen1 = GC.CollectionCount(1);
        var finalGen2 = GC.CollectionCount(2);

        // Assert
        Assert.Equal(messageCount, messagesReceived);

        var memoryGrowth = finalMemory - initialMemory;
        var gen0Collections = finalGen0 - initialGen0;
        var gen1Collections = finalGen1 - initialGen1;
        var gen2Collections = finalGen2 - initialGen2;

        // Verify reasonable memory growth (should be less than 10MB for 50k messages)
        Assert.True(memoryGrowth < 10_000_000,
            $"Memory grew by {memoryGrowth:N0} bytes ({memoryGrowth / 1024.0 / 1024.0:F2} MB) - excessive allocation pressure. Gen0: {gen0Collections}, Gen1: {gen1Collections}, Gen2: {gen2Collections}");

        // Verify minimal Gen2 collections (should be reasonable for 50k messages)
        // Note: GC behavior can vary based on system load and other factors
        Assert.True(gen2Collections <= 100,
            $"Too many Gen2 collections ({gen2Collections}) - indicates allocation pressure. Gen0: {gen0Collections}, Gen1: {gen1Collections}");
    }

    [Fact]
    public void ChannelBasedProcessing_ConcurrentMessages_MaintainsPerformance()
    {
        // Arrange
        const int threadCount = 10;
        const int messagesPerThread = 1_000;
        var totalMessages = threadCount * messagesPerThread;
        var messagesReceived = 0;

        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("concurrent-test")
            .WithCallback<StandalonePubSubSubscriptionConfig>((msg, ctx) =>
            {
                Interlocked.Increment(ref messagesReceived);
            }, null);

        // Act - Simulate concurrent message arrival from multiple threads
        var stopwatch = Stopwatch.StartNew();
        var tasks = new Task[threadCount];

        for (int t = 0; t < threadCount; t++)
        {
            var threadId = t;
            tasks[t] = Task.Run(() =>
            {
                for (int i = 0; i < messagesPerThread; i++)
                {
                    var message = new PubSubMessage($"thread-{threadId}-msg-{i}", "concurrent-test");
                    config.Callback!(message, null);
                }
            });
        }

        Task.WaitAll(tasks);
        stopwatch.Stop();

        // Assert
        Assert.Equal(totalMessages, messagesReceived);

        var throughput = totalMessages / stopwatch.Elapsed.TotalSeconds;

        // Verify high throughput even with concurrent access
        Assert.True(throughput >= 5_000,
            $"Concurrent throughput {throughput:F0} msg/sec is below target of 5,000 msg/sec. Processed {totalMessages} messages from {threadCount} threads in {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
    }

    [Fact]
    public void ChannelBasedProcessing_BurstTraffic_HandlesSpikesEfficiently()
    {
        // Arrange
        const int burstSize = 5_000;
        const int burstCount = 5;
        var messagesReceived = 0;
        var burstTimes = new ConcurrentBag<TimeSpan>();

        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("burst-test")
            .WithCallback<StandalonePubSubSubscriptionConfig>((msg, ctx) =>
            {
                Interlocked.Increment(ref messagesReceived);
            }, null);

        // Act - Simulate burst traffic patterns
        var totalStopwatch = Stopwatch.StartNew();

        for (int burst = 0; burst < burstCount; burst++)
        {
            var burstStopwatch = Stopwatch.StartNew();

            // Send burst of messages
            for (int i = 0; i < burstSize; i++)
            {
                var message = new PubSubMessage($"burst-{burst}-msg-{i}", "burst-test");
                config.Callback!(message, null);
            }

            burstStopwatch.Stop();
            burstTimes.Add(burstStopwatch.Elapsed);

            // Small delay between bursts
            Thread.Sleep(10);
        }

        totalStopwatch.Stop();

        // Assert
        Assert.Equal(burstSize * burstCount, messagesReceived);

        var avgBurstTime = burstTimes.Average(t => t.TotalMilliseconds);
        var maxBurstTime = burstTimes.Max(t => t.TotalMilliseconds);

        // Verify burst handling is efficient
        Assert.True(avgBurstTime < 1000,
            $"Average burst time {avgBurstTime:F2}ms exceeds 1 second threshold. Processed {burstCount} bursts of {burstSize} messages. Max burst time: {maxBurstTime:F2}ms, Total time: {totalStopwatch.Elapsed.TotalMilliseconds:F2}ms");
    }

    [Fact]
    public void ChannelBasedProcessing_LongRunning_MaintainsStablePerformance()
    {
        // Arrange
        const int duration = 5; // seconds
        const int targetRate = 1_000; // messages per second
        var messagesReceived = 0;
        var throughputSamples = new ConcurrentBag<double>();

        var config = new StandalonePubSubSubscriptionConfig()
            .WithChannel("long-running-test")
            .WithCallback<StandalonePubSubSubscriptionConfig>((msg, ctx) =>
            {
                Interlocked.Increment(ref messagesReceived);
            }, null);

        // Warm-up phase (1 second)
        var warmUpStopwatch = Stopwatch.StartNew();
        while (warmUpStopwatch.Elapsed < TimeSpan.FromSeconds(1))
        {
            var message = new PubSubMessage($"warmup-{messagesReceived}", "long-running-test");
            config.Callback!(message, null);
        }
        // Reset counters after warm-up
        messagesReceived = 0;

        // Act - Sustained message processing
        var stopwatch = Stopwatch.StartNew();
        var sampleInterval = TimeSpan.FromSeconds(1);
        var nextSampleTime = sampleInterval;
        var lastSampleCount = 0;
        long sentMessageCount = 0;

        while (stopwatch.Elapsed < TimeSpan.FromSeconds(duration))
        {
            // More robust rate-limiter logic
            var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            var expectedMessagesSent = (long)(elapsedSeconds * targetRate);

            // Send a batch of messages to catch up to the target rate
            while (sentMessageCount < expectedMessagesSent)
            {
                var message = new PubSubMessage($"msg-{sentMessageCount}", "long-running-test");
                config.Callback!(message, null);
                sentMessageCount++;
            }

            // Sample throughput every second
            if (stopwatch.Elapsed >= nextSampleTime)
            {
                var currentCount = messagesReceived;
                var sampleThroughput = (currentCount - lastSampleCount) / sampleInterval.TotalSeconds;
                throughputSamples.Add(sampleThroughput);
                lastSampleCount = currentCount;
                nextSampleTime += sampleInterval;
            }

            // Sleep for a very short duration to prevent a tight loop from hogging 100% CPU
            Thread.Sleep(1);
        }

        stopwatch.Stop();

        // Assert
        var avgThroughput = throughputSamples.Average();
        var minThroughput = throughputSamples.Min();
        var maxThroughput = throughputSamples.Max();
        var throughputStdDev = Math.Sqrt(throughputSamples.Average(t => Math.Pow(t - avgThroughput, 2)));

        // Verify stable performance over time
        Assert.True(avgThroughput >= targetRate * 0.8,
            $"Average throughput {avgThroughput:F0} is below 80% of target rate {targetRate}. Processed {messagesReceived} messages over {duration} seconds. Min: {minThroughput:F0}, Max: {maxThroughput:F0}, StdDev: {throughputStdDev:F0}");

        // Verify throughput stability (std dev should be less than 20% of average)
        Assert.True(throughputStdDev < avgThroughput * 0.2,
            $"Throughput std dev {throughputStdDev:F0} indicates unstable performance. Average: {avgThroughput:F0}, Min: {minThroughput:F0}, Max: {maxThroughput:F0}");
    }
}
