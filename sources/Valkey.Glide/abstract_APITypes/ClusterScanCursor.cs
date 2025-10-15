// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A cursor used to iterate through data returned by cluster SCAN requests.
/// </summary>
public class ClusterScanCursor
{
    private const string FinishedCursorId = "finished";

    /// <summary>
    /// The cursor ID for this scan iteration.
    /// </summary>
    public string CursorId { get; internal set; }

    /// <summary>
    /// Indicates whether this cursor represents the end of the scan.
    /// </summary>
    public bool IsFinished => CursorId == FinishedCursorId;

    /// <summary>
    /// Creates a new ClusterScanCursor with the specified cursor ID.
    /// </summary>
    /// <param name="cursorId">The cursor ID.</param>
    internal ClusterScanCursor(string cursorId)
    {
        CursorId = cursorId;
    }

    /// <summary>
    /// Creates an initial cursor to start a new cluster scan.
    /// </summary>
    /// <returns>A new ClusterScanCursor for starting a scan.</returns>
    public static ClusterScanCursor InitialCursor() => new("0");

    /// <summary>
    /// Releases resources associated with this cursor.
    /// </summary>
    ~ClusterScanCursor()
    {
        // TODO: Release native cursor resources when FFI integration is added.
    }
}
