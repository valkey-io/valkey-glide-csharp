// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents full stream consumer information from a <c>XINFO STREAM</c> response.
/// </summary>
/// <seealso href="https://valkey.io/commands/xinfo-stream/"/>
public readonly struct StreamConsumerInfoFull
{
    #region Public Properties

    /// <summary>
    /// The name of the consumer (<c>name</c>).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The last time the consumer was seen (<c>seen-time</c>).
    /// </summary>
    public DateTimeOffset SeenTime { get; }

    /// <summary>
    /// The last time the consumer was active (<c>active-time</c>),
    /// or <see langword="null"/> if not specified.
    /// </summary>
    /// <remarks>Since Valkey 7.2.0.</remarks>
    public DateTimeOffset? ActiveTime { get; }

    /// <summary>
    /// The number of entries in the consumer's pending entries list (<c>pel-count</c>).
    /// </summary>
    public long PelCount { get; }

    /// <summary>
    /// The consumer's pending entries list (<c>pending</c>).
    /// </summary>
    public StreamPendingEntry[] PendingEntries { get; }

    #endregion
    #region Constructors

    internal StreamConsumerInfoFull(
        string name, 
        DateTimeOffset seenTime, 
        DateTimeOffset? activeTime, 
        long pelCount, 
        StreamPendingEntry[] pendingEntries)
    {
        Name = name;
        SeenTime = seenTime;
        ActiveTime = activeTime;
        PelCount = pelCount;
        PendingEntries = pendingEntries;
    }

    #endregion
}
