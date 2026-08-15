// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Consumer group information from a <c>XINFO GROUPS</c> response.
/// </summary>
/// <seealso href="https://valkey.io/commands/xinfo-groups/"/>
public readonly struct StreamGroupInfo
{
    #region Public Properties

    /// <summary>
    /// The name of the consumer group (<c>name</c>).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The number of consumers in the consumer group (<c>consumers</c>).
    /// </summary>
    public int ConsumerCount { get; }

    /// <summary>
    /// The total number of pending messages for the consumer group (<c>pending</c>). A pending message is
    /// one that has been received by a consumer but not yet acknowledged.
    /// </summary>
    public int PendingMessageCount { get; }

    /// <summary>
    /// The ID of the last entry delivered to the group (<c>last-delivered-id</c>).
    /// </summary>
    public string? LastDeliveredId { get; }

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

    #endregion
    #region Constructors

    internal StreamGroupInfo(string name, int consumerCount, int pendingMessageCount, string? lastDeliveredId, long? entriesRead, long? lag)
    {
        Name = name;
        ConsumerCount = consumerCount;
        PendingMessageCount = pendingMessageCount;
        LastDeliveredId = lastDeliveredId;
        EntriesRead = entriesRead;
        Lag = lag;
    }

    #endregion
}
