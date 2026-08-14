// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Information about a consumer group within a stream.
/// </summary>
/// <seealso href="https://valkey.io/commands/xinfo-stream/"/>
public readonly struct StreamGroupInfoFull
{
    #region Public Properties

    /// <summary>
    /// The name of the consumer group (<c>name</c>).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The ID of the last entry delivered to the group (<c>last-delivered-id</c>).
    /// </summary>
    public ValkeyValue LastDeliveredId { get; }

    /// <summary>
    /// The number of entries the group has read (<c>entries-read</c>),
    /// or <see langword="null"/> if not specified.
    /// </summary>
    /// <remarks>Since Valkey 7.0.0.</remarks>
    public long? EntriesRead { get; }

    /// <summary>
    /// The number of entries in the stream still waiting to be delivered to the
    /// group's consumers (<c>lag</c>), or <see langword="null"/> if not specified.
    /// </summary>
    /// <remarks>Since Valkey 7.0.0.</remarks>
    public long? Lag { get; }

    /// <summary>
    /// The number of entries in the group's pending entries list (<c>pel-count</c>).
    /// </summary>
    public long PelCount { get; }

    /// <summary>
    /// The group's pending entries list (<c>pending</c>).
    /// </summary>
    public StreamPendingEntryInfo[] PendingEntries { get; }

    /// <summary>
    /// The consumers in the group (<c>consumers</c>).
    /// </summary>
    public StreamConsumerFullInfo[] Consumers { get; }

    #endregion
    #region Constructors

    internal StreamGroupInfoFull(
        string name,
        ValkeyValue lastDeliveredId,
        long? entriesRead,
        long? lag,
        long pelCount,
        StreamPendingEntryInfo[] pendingEntries,
        StreamConsumerFullInfo[] consumers)
    {
        Name = name;
        LastDeliveredId = lastDeliveredId;
        EntriesRead = entriesRead;
        Lag = lag;
        PelCount = pelCount;
        PendingEntries = pendingEntries;
        Consumers = consumers;
    }

    #endregion
}
