// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Delegate for PubSub message callbacks.
/// </summary>
/// <param name="message">The received PubSub message.</param>
/// <param name="context">User-provided context object.</param>
public delegate void MessageCallback(PubSubMessage message, object? context);

/// <summary>
/// Handles routing of PubSub messages to callbacks or queues.
/// Provides error handling and recovery for callback exceptions.
/// </summary>
internal sealed class PubSubMessageHandler : IDisposable
{
    private readonly MessageCallback? _callback;
    private readonly object? _context;
    private PubSubMessageQueue? _queue;
    private readonly object _lock = new();
    private volatile bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubMessageHandler"/> class.
    /// </summary>
    /// <param name="callback">Optional callback to invoke when messages are received. If null, messages will be queued.</param>
    /// <param name="context">Optional context object to pass to the callback.</param>
    internal PubSubMessageHandler(MessageCallback? callback, object? context)
    {
        _callback = callback;
        _context = context;
        // Queue will be lazily initialized only when needed (i.e., when no callback is provided)
    }

    /// <summary>
    /// Process an incoming PubSub message by routing it to callback or queue.
    /// </summary>
    /// <param name="message">The message to process.</param>
    /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the handler has been disposed.</exception>
    internal void HandleMessage(PubSubMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ThrowIfDisposed();

        if (_callback != null)
        {
            // Route to callback with error handling
            InvokeCallbackSafely(message);
        }
        else
        {
            // Route to queue - initialize if needed
            var queue = GetOrCreateQueue();
            try
            {
                queue.EnqueueMessage(message);
            }
            catch (ObjectDisposedException)
            {
                // Queue was disposed, ignore the message
                Logger.Log(Level.Warn, "PubSubMessageHandler", $"Attempted to enqueue message to disposed queue for channel {message.Channel}");
            }
        }
    }

    /// <summary>
    /// Get the message queue for manual message retrieval.
    /// </summary>
    /// <returns>The message queue instance.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the handler has been disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a callback is configured.</exception>
    internal PubSubMessageQueue GetQueue()
    {
        ThrowIfDisposed();

        if (_callback != null)
        {
            throw new InvalidOperationException("Cannot access message queue when callback is configured. Use callback mode or queue mode, not both.");
        }

        return GetOrCreateQueue();
    }

    /// <summary>
    /// Gets or creates the message queue instance. Used internally for lazy initialization.
    /// </summary>
    /// <returns>The message queue instance.</returns>
    private PubSubMessageQueue GetOrCreateQueue()
    {
        if (_queue == null)
        {
            lock (_lock)
            {
                if (_queue == null && !_disposed)
                {
                    _queue = new PubSubMessageQueue();
                }
            }
        }

        return _queue ?? throw new ObjectDisposedException(nameof(PubSubMessageHandler));
    }

    /// <summary>
    /// Safely invoke the callback with proper error handling and recovery.
    /// </summary>
    /// <param name="message">The message to pass to the callback.</param>
    private void InvokeCallbackSafely(PubSubMessage message)
    {
        try
        {
            // Check if disposed before invoking callback to avoid race conditions
            if (_disposed)
            {
                return;
            }

            _callback!(message, _context);
        }
        catch (Exception ex)
        {
            // Log the error and continue processing subsequent messages
            // This ensures that callback exceptions don't break the message processing pipeline
            Logger.Log(Level.Error, "PubSubMessageHandler", $"Error in PubSub message callback for channel {message.Channel}. Message processing will continue.", ex);
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="PubSubMessageHandler"/>.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        // Dispose the message queue if it was created
        if (_queue != null)
        {
            try
            {
                _queue.Dispose();
            }
            catch (Exception ex)
            {
                // Log disposal errors but don't throw to ensure cleanup completes
                Logger.Log(Level.Warn, "PubSubMessageHandler", "Error during PubSub message queue disposal", ex);
            }
        }
    }

    /// <summary>
    /// Throws an ObjectDisposedException if the handler has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(PubSubMessageHandler));
        }
    }
}
