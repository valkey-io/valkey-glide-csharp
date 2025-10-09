// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Valkey.Glide;

/// <summary>
/// Thread-safe queue for PubSub messages with async support.
/// Provides both blocking and non-blocking message retrieval methods.
/// </summary>
public sealed class PubSubMessageQueue : IDisposable
{
    private readonly ConcurrentQueue<PubSubMessage> _messages;
    private readonly SemaphoreSlim _messageAvailable;
    private readonly object _lock = new();
    private volatile bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubMessageQueue"/> class.
    /// </summary>
    public PubSubMessageQueue()
    {
        _messages = new ConcurrentQueue<PubSubMessage>();
        _messageAvailable = new SemaphoreSlim(0);
    }

    /// <summary>
    /// Gets the current number of queued messages.
    /// </summary>
    public int Count => _messages.Count;

    /// <summary>
    /// Try to get a message from the queue without blocking.
    /// </summary>
    /// <param name="message">The retrieved message, or null if no message is available.</param>
    /// <returns>true if a message was retrieved; otherwise, false.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the queue has been disposed.</exception>
    public bool TryGetMessage(out PubSubMessage? message)
    {
        ThrowIfDisposed();

        if (_messages.TryDequeue(out message))
        {
            // Consume one semaphore count since we dequeued a message
            _ = _messageAvailable.Wait(0);
            return true;
        }

        message = null;
        return false;
    }

    /// <summary>
    /// Asynchronously wait for and retrieve a message from the queue.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved message.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the queue has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    public async Task<PubSubMessage> GetMessageAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        // Wait for a message to be available
        await _messageAvailable.WaitAsync(cancellationToken).ConfigureAwait(false);

        // Check if disposed after waiting
        ThrowIfDisposed();

        // Try to dequeue the message
        if (_messages.TryDequeue(out PubSubMessage? message))
        {
            return message;
        }

        // This should not happen under normal circumstances, but handle it gracefully
        throw new InvalidOperationException("Message queue is in an inconsistent state");
    }

    /// <summary>
    /// Get an async enumerable for continuous message processing.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the enumeration.</param>
    /// <returns>An async enumerable that yields messages as they become available.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the queue has been disposed.</exception>
    public async IAsyncEnumerable<PubSubMessage> GetMessagesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!_disposed && !cancellationToken.IsCancellationRequested)
        {
            PubSubMessage message;
            try
            {
                message = await GetMessageAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
                // Queue was disposed, exit enumeration
                yield break;
            }
            catch (OperationCanceledException)
            {
                // Operation was cancelled, exit enumeration
                yield break;
            }

            yield return message;
        }
    }

    /// <summary>
    /// Enqueue a message to the queue.
    /// This method is intended for internal use by the PubSub message handler.
    /// </summary>
    /// <param name="message">The message to enqueue.</param>
    /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when the queue has been disposed.</exception>
    internal void EnqueueMessage(PubSubMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ThrowIfDisposed();

        _messages.Enqueue(message);
        _ = _messageAvailable.Release();
    }

    /// <summary>
    /// Releases all resources used by the <see cref="PubSubMessageQueue"/>.
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

        // Release all waiting threads
        try
        {
            // Release as many times as there are potentially waiting threads
            // This ensures all waiting GetMessageAsync calls will complete
            int releaseCount = Math.Max(1, _messageAvailable.CurrentCount + 10);
            _ = _messageAvailable.Release(releaseCount);
        }
        catch (SemaphoreFullException)
        {
            // Ignore if semaphore is already at maximum
        }

        _messageAvailable.Dispose();
    }

    /// <summary>
    /// Throws an ObjectDisposedException if the queue has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(PubSubMessageQueue));
        }
    }
}
