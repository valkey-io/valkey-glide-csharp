// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// A cursor used to iterate through data returned by cluster scan requests.
/// </summary>
/// <seealso cref="Commands.IGenericClusterCommands.ScanAsync"/>
public class ClusterScanCursor
{
    private const string InitialCursorId = "0";
    private const string FinishedCursorId = "finished";

    /// <summary>
    /// The cursor ID for this scan iteration.
    /// </summary>
    public string CursorId { get; }

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
    /// Creates a new cursor with the specified cursor ID.
    /// </summary>
    /// <param name="cursorId">The cursor ID.</param>
    internal ClusterScanCursor(string cursorId)
    {
        ArgumentNullException.ThrowIfNull(cursorId);
        CursorId = cursorId;
    }
}
