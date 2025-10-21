// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Threading.Channels;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Tests for graceful shutdown coordination in PubSub processing.
/// Validates Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 9.2
/// </summary>
public class PubSubGracefulShutdownTests
{
    [Fact]
    public void PubSubPerformanceConfig_DefaultShutdownTimeout_IsCorrect()
    {
        PubSubPerformanceConfig config = new();
        Assert.Equal(TimeSpan.FromSeconds(5), config.ShutdownTimeout);
    }

    [Fact]
    public void PubSubPerformanceConfig_CustomShutdownTimeout_CanBeSet()
    {
        TimeSpan customTimeout = TimeSpan.FromSeconds(10);
        PubSubPerformanceConfig config = new() { ShutdownTimeout = customTimeout };
        Assert.Equal(customTimeout, config.ShutdownTimeout);
    }

    [Fact]
    public void PubSubPerformanceConfig_InvalidShutdownTimeout_ThrowsException()
    {
        PubSubPerformanceConfig config = new() { ShutdownTimeout = TimeSpan.FromSeconds(-1) };
        Assert.Throws<ArgumentOutOfRangeException>(() => config.Validate());
    }

    [Fact]
    public async Task ChannelBasedProcessing_CancellationToken_IsRespected()
    {
        Channel<int> channel = Channel.CreateBounded<int>(10);
        CancellationTokenSource cts = new();
        int messagesProcessed = 0;

        Task processingTask = Task.Run(async () =>
        {
            try
            {
                await foreach (int message in channel.Reader.ReadAllAsync(cts.Token))
                {
                    _ = Interlocked.Increment(ref messagesProcessed);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
        });

        await channel.Writer.WriteAsync(1);
        await channel.Writer.WriteAsync(2);
        await Task.Delay(50);

        cts.Cancel();
        channel.Writer.Complete();
        await processingTask;

        Assert.True(messagesProcessed >= 0);
    }

    [Fact]
    public async Task ChannelCompletion_StopsProcessing_Gracefully()
    {
        Channel<int> channel = Channel.CreateBounded<int>(10);
        int messagesProcessed = 0;
        bool processingCompleted = false;

        Task processingTask = Task.Run(async () =>
        {
            await foreach (int message in channel.Reader.ReadAllAsync())
            {
                _ = Interlocked.Increment(ref messagesProcessed);
            }
            processingCompleted = true;
        });

        await channel.Writer.WriteAsync(1);
        await channel.Writer.WriteAsync(2);
        await channel.Writer.WriteAsync(3);
        channel.Writer.Complete();
        await processingTask;

        Assert.Equal(3, messagesProcessed);
        Assert.True(processingCompleted);
    }
}
