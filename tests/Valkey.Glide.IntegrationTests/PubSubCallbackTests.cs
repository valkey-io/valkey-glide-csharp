// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub callback.
/// </summary>
[Collection(typeof(PubSubCallbackTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubCallbackTests
{
    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Callback_ChannelMode_ReceivesMessage(bool isCluster, PubSubChannelMode channelMode)
    {
        var message = BuildMessage(channelMode);

        // Build subscriber with callback that captures received message.
        var received = new TaskCompletionSource<PubSubMessage>();
        using var subscriber = await BuildSubscriber(
            isCluster,
            message: message,
            callback: (msg, ctx) => received.SetResult(msg));

        // Publish message and verify receipt via callback.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, message);
        Assert.Equal(message, await received.Task.WaitAsync(MaxDuration));
    }

    [Theory]
    [MemberData(nameof(ClusterAndSubscribeModeData))]
    public static async Task Callback_SubscribeMode_ReceivesMessage(bool isCluster, SubscribeMode subscribeMode)
    {
        var message = BuildMessage();

        // Build subscriber with callback that captures received message.
        var received = new TaskCompletionSource<PubSubMessage>();
        using var subscriber = await BuildSubscriber(
            isCluster,
            message,
            subscribeMode: subscribeMode,
            callback: (msg, ctx) => received.SetResult(msg));

        // Publish message and verify receipt via callback.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, message);
        Assert.Equal(message, await received.Task.WaitAsync(MaxDuration));
    }


    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Callback_WithException_ContinuesProcessing(bool isCluster)
    {
        // Build messages.
        var message = BuildMessage();

        // Setup callback that throws an exception on the first message, but succeeds on subsequent messages
        var receivedCount = 0;
        var succeededCount = 0;
        using var completed = new ManualResetEventSlim(false);

        using var subscriber = await BuildSubscriber(
                isCluster,
                message,
                callback: (msg, context) =>
                {
                    int invocation = Interlocked.Increment(ref receivedCount);

                    // Throw exception on first message, succeed on subsequent messages
                    if (invocation == 1)
                        throw new InvalidOperationException("Test exception in callback");

                    Interlocked.Increment(ref succeededCount);

                    if (invocation >= 3)
                        completed.Set();
                });

        using var publisher = BuildPublisher(isCluster);

        // Publish multiple messages.
        await PublishAsync(publisher, message);
        await PublishAsync(publisher, message);
        await PublishAsync(publisher, message);

        // Verify that all messages received despite exception.
        completed.Wait(MaxDuration);
        Assert.Equal(3, receivedCount);
        Assert.Equal(2, succeededCount);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Callback_WithMultipleMessages_PreservesOrder(bool isCluster)
    {
        int messageCount = 20;
        var messages = Enumerable.Range(0, messageCount).Select(i => BuildMessage()).ToList();

        // Setup callback to capture received messages.
        List<PubSubMessage> receivedMessages = [];
        using var completed = new ManualResetEventSlim(false);

        using var subscriber = await BuildSubscriber(
            isCluster,
            messages,
            callback: (msg, context) =>
            {
                lock (receivedMessages)
                {
                    receivedMessages.Add(msg);
                    if (receivedMessages.Count >= messageCount)
                        completed.Set();
                }
            });

        using var publisher = BuildPublisher(isCluster);

        // Publish all messages and verify they are received in order.
        await PublishAsync(publisher, messages);
        completed.Wait(MaxDuration);
        Assert.Equivalent(messages, receivedMessages);
    }
}

