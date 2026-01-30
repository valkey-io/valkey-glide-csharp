// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Valkey.Glide;

/// <summary>
/// Represents a message that is broadcast via publish/subscribe.
/// Compatible with StackExchange.Redis <c>ChannelMessage</c>.
/// </summary>
public readonly struct ChannelMessage
{
    private readonly ChannelMessageQueue _queue;

    /// <inheritdoc/>
    public override string ToString() => ((string?)Channel) + ":" + ((string?)Message);

    /// <inheritdoc/>
    public override int GetHashCode() => Channel.GetHashCode() ^ Message.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChannelMessage cm
        && cm.Channel == Channel && cm.Message == Message;

    internal ChannelMessage(ChannelMessageQueue queue, in ValkeyChannel channel, in ValkeyValue value)
    {
        _queue = queue;
        Channel = channel;
        Message = value;
    }

    /// <summary>
    /// The channel that the subscription was created from.
    /// </summary>
    public ValkeyChannel SubscriptionChannel => _queue.Channel;

    /// <summary>
    /// The channel that the message was broadcast to.
    /// </summary>
    public ValkeyChannel Channel { get; }

    /// <summary>
    /// The value that was broadcast.
    /// </summary>
    public ValkeyValue Message { get; }

    /// <summary>
    /// Checks if 2 messages are .Equal().
    /// </summary>
    public static bool operator ==(ChannelMessage left, ChannelMessage right) => left.Equals(right);

    /// <summary>
    /// Checks if 2 messages are not .Equal().
    /// </summary>
    public static bool operator !=(ChannelMessage left, ChannelMessage right) => !left.Equals(right);
}

/// <summary>
/// Represents a message queue of ordered pub/sub notifications.
/// Compatible with StackExchange.Redis <c>ChannelMessageQueue</c>.
/// </summary>
public sealed class ChannelMessageQueue : IAsyncEnumerable<ChannelMessage>
{
    private readonly Channel<ChannelMessage> _queue;

    /// <summary>
    /// The Channel that was subscribed for this queue.
    /// </summary>
    public ValkeyChannel Channel { get; }
    private ISubscriber? _parent;

    /// <inheritdoc/>
    public override string? ToString() => (string?)Channel;

    /// <summary>
    /// An awaitable task the indicates completion of the queue (including drain of data).
    /// </summary>
    public Task Completion => _queue.Reader.Completion;

    internal ChannelMessageQueue(in ValkeyChannel channel, ISubscriber parent)
    {
        Channel = channel;
        _parent = parent;
        _queue = System.Threading.Channels.Channel.CreateUnbounded<ChannelMessage>(s_ChannelOptions);
    }

    private static readonly UnboundedChannelOptions s_ChannelOptions = new()
    {
        SingleWriter = true,
        SingleReader = false,
        AllowSynchronousContinuations = false,
    };

    internal void Write(in ValkeyChannel channel, in ValkeyValue value)
    {
        var writer = _queue.Writer;
        writer.TryWrite(new ChannelMessage(this, channel, value));
    }

    /// <summary>
    /// Consume a message from the channel.
    /// </summary>
    public ValueTask<ChannelMessage> ReadAsync(CancellationToken cancellationToken = default)
        => _queue.Reader.ReadAsync(cancellationToken);

    /// <summary>
    /// Attempt to synchronously consume a message from the channel.
    /// </summary>
    public bool TryRead(out ChannelMessage item) => _queue.Reader.TryRead(out item);

    /// <summary>
    /// Attempt to query the backlog length of the queue.
    /// </summary>
    public bool TryGetCount(out int count)
    {
        var reader = _queue.Reader;
        if (reader.CanCount)
        {
            count = reader.Count;
            return true;
        }

        count = default;
        return false;
    }

    private Delegate? _onMessageHandler;
    private void AssertOnMessage(Delegate handler)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        if (Interlocked.CompareExchange(ref _onMessageHandler, handler, null) != null)
            throw new InvalidOperationException("Only a single " + nameof(OnMessage) + " is allowed");
    }

    /// <summary>
    /// Create a message loop that processes messages sequentially.
    /// </summary>
    public void OnMessage(Action<ChannelMessage> handler)
    {
        AssertOnMessage(handler);
        ThreadPool.QueueUserWorkItem(state => ((ChannelMessageQueue)state!).OnMessageSyncImpl(), this);
    }

    private async Task OnMessageSyncImpl()
    {
        var handler = (Action<ChannelMessage>?)_onMessageHandler;
        while (!Completion.IsCompleted)
        {
            ChannelMessage next;
            try
            {
                if (!TryRead(out next))
                    next = await ReadAsync().ConfigureAwait(false);
            }
            catch (ChannelClosedException) { break; }
            catch { break; }

            try { handler?.Invoke(next); }
            catch { }
        }
    }

    internal static void Combine(ref ChannelMessageQueue? head, ChannelMessageQueue queue)
    {
        if (queue != null)
        {
            ChannelMessageQueue? old;
            do
            {
                old = Volatile.Read(ref head);
                queue._next = old;
            }
            while (Interlocked.CompareExchange(ref head, queue, old) != old);
        }
    }

    /// <summary>
    /// Create a message loop that processes messages sequentially.
    /// </summary>
    public void OnMessage(Func<ChannelMessage, Task> handler)
    {
        AssertOnMessage(handler);
        ThreadPool.QueueUserWorkItem(state => ((ChannelMessageQueue)state!).OnMessageAsyncImpl(), this);
    }

    internal static void Remove(ref ChannelMessageQueue? head, ChannelMessageQueue queue)
    {
        if (queue is null) return;

        bool found;
        do
        {
            var current = Volatile.Read(ref head);
            if (current == null) return;
            if (current == queue)
            {
                found = true;
                if (Interlocked.CompareExchange(ref head, Volatile.Read(ref current._next), current) == current)
                {
                    return;
                }
            }
            else
            {
                ChannelMessageQueue? previous = current;
                current = Volatile.Read(ref previous._next);
                found = false;
                do
                {
                    if (current == queue)
                    {
                        found = true;
                        if (Interlocked.CompareExchange(ref previous._next, Volatile.Read(ref current._next), current) == current)
                        {
                            return;
                        }
                        else
                        {
                            break;
                        }
                    }
                    previous = current;
                    current = Volatile.Read(ref previous!._next);
                }
                while (current != null);
            }
        }
        while (found);
    }

    internal static int Count(ref ChannelMessageQueue? head)
    {
        var current = Volatile.Read(ref head);
        int count = 0;
        while (current != null)
        {
            count++;
            current = Volatile.Read(ref current._next);
        }
        return count;
    }

    internal static void WriteAll(ref ChannelMessageQueue head, in ValkeyChannel channel, in ValkeyValue message)
    {
        var current = Volatile.Read(ref head);
        while (current != null)
        {
            current.Write(channel, message);
            current = Volatile.Read(ref current._next);
        }
    }

    private ChannelMessageQueue? _next;

    private async Task OnMessageAsyncImpl()
    {
        var handler = (Func<ChannelMessage, Task>?)_onMessageHandler;
        while (!Completion.IsCompleted)
        {
            ChannelMessage next;
            try
            {
                if (!TryRead(out next))
                    next = await ReadAsync().ConfigureAwait(false);
            }
            catch (ChannelClosedException) { break; }
            catch { break; }

            try
            {
                var task = handler?.Invoke(next);
                if (task != null && task.Status != TaskStatus.RanToCompletion)
                    await task.ConfigureAwait(false);
            }
            catch { }
        }
    }

    internal static void MarkAllCompleted(ref ChannelMessageQueue? head)
    {
        var current = Interlocked.Exchange(ref head, null);
        while (current != null)
        {
            current.MarkCompleted();
            current = Volatile.Read(ref current._next);
        }
    }

    private void MarkCompleted(Exception? error = null)
    {
        _parent = null;
        _queue.Writer.TryComplete(error);
    }

    internal void UnsubscribeImpl(Exception? error = null, CommandFlags flags = CommandFlags.None)
    {
        var parent = _parent;
        _parent = null;
        parent?.Unsubscribe(Channel, null, flags);
        _queue.Writer.TryComplete(error);
    }

    internal async Task UnsubscribeAsyncImpl(Exception? error = null, CommandFlags flags = CommandFlags.None)
    {
        var parent = _parent;
        _parent = null;
        if (parent != null)
        {
            await parent.UnsubscribeAsync(Channel, null, flags).ConfigureAwait(false);
        }
        _queue.Writer.TryComplete(error);
    }

    /// <summary>
    /// Stop receiving messages on this channel.
    /// </summary>
    public void Unsubscribe(CommandFlags flags = CommandFlags.None) => UnsubscribeImpl(null, flags);

    /// <summary>
    /// Stop receiving messages on this channel.
    /// </summary>
    public Task UnsubscribeAsync(CommandFlags flags = CommandFlags.None) => UnsubscribeAsyncImpl(null, flags);

    /// <inheritdoc/>
    public async IAsyncEnumerator<ChannelMessage> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        while (await _queue.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
        {
            while (_queue.Reader.TryRead(out var item))
            {
                yield return item;
            }
        }
    }
}
