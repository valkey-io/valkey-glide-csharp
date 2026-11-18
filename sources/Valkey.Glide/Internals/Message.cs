// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Valkey.Glide.Internals;

/// <summary>
/// Reusable source of ValueTask. This object can be allocated once and then reused
/// to create multiple asynchronous operations, as long as each call to CreateTask
/// is awaited to completion before the next call begins.
/// </summary>
/// <param name="index">The index of the message in the external message array.</param>
/// <param name="container">The container that owns this message.</param>
internal class Message(int index, MessageContainer container) : INotifyCompletion
{
    private const int COMPLETION_STAGE_STARTED = 0;
    private const int COMPLETION_STAGE_NEXT_SHOULD_EXECUTE_CONTINUATION = 1;
    private const int COMPLETION_STAGE_CONTINUATION_EXECUTED = 2;

    private Action? _continuation = () => { };

    private int _completionState;
    private IntPtr _result = default;
    private Exception? _exception;

    /// <summary>
    /// Handle that prevents the client from being disposed until the request completes.
    /// The Rust core only stores this message's index, so the garbage collector could
    /// otherwise dispose of the client while the request is still waiting for a response.
    /// </summary>
    private IntPtr _clientHandle;

    /// <summary>
    /// This is the index of the message in an external array, that allows the user to
    /// know how to find the message and set its result.
    /// </summary>
    public int Index { get; } = index;

    // TODO #635: Return true once the response has arrived, so that awaiting does not grow the stack.
    public bool IsCompleted => _completionState == COMPLETION_STAGE_CONTINUATION_EXECUTED;

    private MessageContainer Container { get; } = container;

    /// <summary>
    /// Starts a new operation that completes when <see cref="SetResult(IntPtr)"/>
    /// or <see cref="SetException(Exception)"/> is called.
    /// </summary>
    /// <param name="client">The client associated with the operation.</param>
    public void SetupTask(object client)
    {
        _continuation = null;
        _completionState = COMPLETION_STAGE_STARTED;
        _result = default;
        _exception = null;
        _clientHandle = GCHandle.ToIntPtr(GCHandle.Alloc(client));
    }

    /// <summary>
    /// Completes the current operation successfully.
    /// The awaiting code then receives <paramref name="result"/>.
    /// </summary>
    /// <param name="result">The result to complete the operation with.</param>
    public void SetResult(IntPtr result)
    {
        _result = result;
        FinishSet();
    }

    /// <summary>
    /// Completes the current operation with an error.
    /// The awaiting code then throws <paramref name="exc"/>.
    /// </summary>
    /// <param name="exc">The exception to complete the operation with.</param>
    public void SetException(Exception exc)
    {
        _exception = exc;
        FinishSet();
    }

    public Message GetAwaiter() => this;

    public void OnCompleted(Action continuation)
    {
        _continuation = continuation;
        CheckRaceAndCallContinuation();
    }

    public IntPtr GetResult() => _exception is null ? _result : throw _exception;

    private void FinishSet()
    {
        CleanUp();
        CheckRaceAndCallContinuation();
    }

    private void CheckRaceAndCallContinuation()
    {
        if (Interlocked.CompareExchange(ref _completionState, COMPLETION_STAGE_NEXT_SHOULD_EXECUTE_CONTINUATION, COMPLETION_STAGE_STARTED) == COMPLETION_STAGE_NEXT_SHOULD_EXECUTE_CONTINUATION)
        {
            Debug.Assert(_continuation != null);
            _completionState = COMPLETION_STAGE_CONTINUATION_EXECUTED;
            try
            {
                _continuation();
            }
            finally
            {
                Container.ReturnFreeMessage(this);
            }
        }
    }

    /// <summary>
    /// Frees the handle that keeps the client from being garbage collected.
    /// </summary>
    private void CleanUp()
    {
        IntPtr handle = Interlocked.Exchange(ref _clientHandle, IntPtr.Zero);
        if (handle != IntPtr.Zero)
        {
            GCHandle.FromIntPtr(handle).Free();
        }
    }
}
