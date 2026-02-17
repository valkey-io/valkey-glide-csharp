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
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task Callback_Channel_ReceivesMessage(bool isCluster)
    {
        var message = BuildChannelMessage();
        var received = new TaskCompletionSource<PubSubMessage>();

        using var subscriber = await BuildSubscriber(isCluster,
            channels: [message.Channel],
            callback: (msg, _) => received.TrySetResult(msg));
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message.Channel, message.Message);
        Assert.Equal(message, await received.Task.WaitAsync(MaxDuration));
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task Callback_Pattern_ReceivesMessage(bool isCluster)
    {
        var message = BuildPatternMessage();
        var received = new TaskCompletionSource<PubSubMessage>();

        using var subscriber = await BuildSubscriber(isCluster,
            patterns: [message.Pattern!],
            callback: (msg, _) => received.TrySetResult(msg));
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message.Channel, message.Message);
        Assert.Equal(message, await received.Task.WaitAsync(MaxDuration));
    }

    [Fact]
    public async Task Callback_ShardChannel_ReceivesMessage()
    {
        SkipUnlessShardedSupported();

        var message = BuildShardChannelMessage();
        var received = new TaskCompletionSource<PubSubMessage>();

        using var subscriber = await BuildClusterSubscriber(
            shardChannels: [message.Channel],
            callback: (msg, _) => received.TrySetResult(msg));
        using var publisher = BuildClusterClient();

        await publisher.SPublishAsync(message.Channel, message.Message);
        Assert.Equal(message, await received.Task.WaitAsync(MaxDuration));
    }

    [Fact]
    public async Task Callback_WithException_ContinuesProcessing()
    {
        var message = BuildChannelMessage();
        var channels = new string[] { message.Channel };

        // Setup callback that throws an exception on the first message, but succeeds on subsequent messages
        var receivedCount = 0;
        var succeededCount = 0;
        using var completed = new ManualResetEventSlim(false);

        using var subscriber = await BuildStandaloneSubscriber(
                channels: channels,
                callback: (message, context) =>
                {
                    int invocation = Interlocked.Increment(ref receivedCount);

                    // Throw exception on first message, succeed on subsequent messages
                    if (invocation == 1)
                        throw new InvalidOperationException("Test exception in callback");

                    Interlocked.Increment(ref succeededCount);

                    if (invocation >= 3)
                        completed.Set();
                });
        using var publisher = BuildStandaloneClient();

        // Publish multiple messages.
        await publisher.PublishAsync(message.Channel, message.Message);
        await publisher.PublishAsync(message.Channel, message.Message);
        await publisher.PublishAsync(message.Channel, message.Message);

        // Verify that all messages received despite exception.
        completed.Wait(MaxDuration);
        Assert.Equal(3, receivedCount);
        Assert.Equal(2, succeededCount);
    }

    [Fact]
    public async Task Callback_WithMultipleMessages_PreservesOrder()
    {
        var channel = $"test-channel-{Guid.NewGuid()}";

        int messageCount = 20;
        var expectedMessages = Enumerable.Range(0, messageCount).Select(i => $"Message-{i:D3}").ToList();

        // Setup callback to capture received messages.
        List<string> receivedMessages = [];
        using var completed = new ManualResetEventSlim(false);

        using var subscriber = await BuildStandaloneSubscriber(
            channels: [channel],
            callback: (msg, context) =>
            {
                lock (receivedMessages)
                {
                    receivedMessages.Add(msg.Message);
                    if (receivedMessages.Count >= messageCount)
                        completed.Set();
                }
            });
        using var publisher = BuildStandaloneClient();

        // Publish all messages.
        foreach (var msg in expectedMessages)
            await publisher.PublishAsync(channel, msg);


        // Verify that order is preserved.
        completed.Wait(MaxDuration);
        Assert.Equivalent(expectedMessages, receivedMessages);
    }
}
