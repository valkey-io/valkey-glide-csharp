// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// A cursor used to iterate through data returned by cluster scan requests.
/// </summary>
/// <remarks>
/// Cursors hold external resources. These resources can be released immediately
/// by calling <see cref="Dispose"/>. Disposing a cursor invalidates it from being
/// used in subsequent cluster scan requests.
/// </remarks>
/// <seealso cref="Commands.IGenericClusterCommands.ScanAsync"/>
public class ClusterScanCursor : IDisposable
{
    private const string InitialCursorId = "0";
    private const string FinishedCursorId = "finished";
    private bool _isDisposed;

    /// <summary>
    /// The cursor ID for this scan iteration.
    /// </summary>
    public string CursorId { get; internal set; }

    /// <summary>
    /// Indicates whether this cursor represents the end of the scan.
    /// </summary>
    public bool IsFinished => CursorId == FinishedCursorId;

    /// <summary>
    /// Creates a cursor to start a new cluster scan.
    /// </summary>
    /// <returns>A cursor to start a new cluster scan.</returns>
    public static ClusterScanCursor InitialCursor() => new(InitialCursorId);

    /// <summary>
    /// Releases resources associated with this cursor.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(obj: this);
    }

    /// <summary>
    /// Creates a new cursor with the specified cursor ID.
    /// </summary>
    /// <param name="cursorId">The cursor ID.</param>
    internal ClusterScanCursor(string cursorId)
    {
        CursorId = cursorId;
    }

    /// <summary>
    /// Releases resources associated with this cursor.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (CursorId != InitialCursorId && CursorId != FinishedCursorId)
        {
            var cursorPtr = Marshal.StringToHGlobalAnsi(CursorId);
            FFI.RemoveClusterScanCursorFfi(cursorPtr);
            Marshal.FreeHGlobal(cursorPtr);
        }

        _isDisposed = true;
    }

    ~ClusterScanCursor()
    {
        Dispose(disposing: false);
    }
}
