// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a subscription to a pub/sub channel with associated handlers and queues.
/// </summary>
internal sealed class Subscription
{
    private Action<ValkeyChannel, ValkeyValue>? _handlers;
    private readonly object _handlersLock = new();
    private ChannelMessageQueue? _queues;

    /// <summary>
    /// Initializes a new instance of the <see cref="Subscription"/> class.
    /// </summary>
    public Subscription() { }

    /// <summary>
    /// Adds a new message handler to this subscription.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    public void AddHandler(Action<ValkeyChannel, ValkeyValue> handler)
    {
        lock (_handlersLock)
        {
            _handlers += handler;
        }
    }

    /// <summary>
    /// Removes a handler from this subscription.
    /// </summary>
    /// <param name="handler">The specific handler to remove.</param>
    public void RemoveHandler(Action<ValkeyChannel, ValkeyValue> handler)
    {
        lock (_handlersLock)
        {
            _handlers -= handler;
        }
    }

    /// <summary>
    /// Adds a new message queue to this subscription.
    /// </summary>
    /// <param name="queue">The queue to add.</param>
    public void AddQueue(ChannelMessageQueue queue)
    {
        ChannelMessageQueue.Combine(ref _queues, queue);
    }

    /// <summary>
    /// Removes a queue from this subscription.
    /// </summary>
    /// <param name="queue">The queue to remove.</param>
    public void RemoveQueue(ChannelMessageQueue queue)
    {
        ChannelMessageQueue.Remove(ref _queues, queue);
    }

    /// <summary>
    /// Invokes all handlers and writes to all queues for a received message.
    /// </summary>
    /// <param name="channel">The channel on which the message was received.</param>
    /// <param name="message">The received message.</param>
    public void OnMessage(ValkeyChannel channel, ValkeyValue message)
    {
        // Invoke all registered handlers.
        var handlers = _handlers;
        if (handlers != null)
        {
            // Queue handler invocation to thread pool to avoid blocking message processing
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (var handler in handlers.GetInvocationList().Cast<Action<ValkeyChannel, ValkeyValue>>())
                {
                    try
                    {
                        handler(channel, message);
                    }

                    // Swallow handler exceptions to prevent breaking message processing
                    catch { }
                }
            });
        }

        // Write to all registered queues.
        var queues = Volatile.Read(ref _queues);
        if (queues != null)
        {
            ChannelMessageQueue.WriteAll(ref queues, channel, message);
        }
    }

    /// <summary>
    /// Returns true if this subscription is empty.
    /// </summary>
    /// <returns>True if empty, otherwise false.</returns>
    public bool IsEmpty()
    {
        return _handlers == null && _queues == null;
    }
}
