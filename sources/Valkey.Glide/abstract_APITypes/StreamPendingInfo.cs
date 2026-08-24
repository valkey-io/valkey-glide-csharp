// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A summary from a <c>XPENDING</c> response.
/// </summary>
/// <seealso href="https://valkey.io/commands/xpending/"/>
public readonly struct StreamPendingInfo
{
    #region Public Properties

    /// <summary>
    /// The number of pending messages. A pending message is a message that has been consumed but not yet acknowledged.
    /// </summary>
    public int PendingMessageCount { get; }

    /// <summary>
    /// The lowest message ID in the set of pending messages.
    /// </summary>
    public ValkeyValue LowestPendingMessageId { get; }

    /// <summary>
    /// The highest message ID in the set of pending messages.
    /// </summary>
    public ValkeyValue HighestPendingMessageId { get; }

    /// <summary>
    /// An array of consumers within the consumer group that have pending messages.
    /// </summary>
    public StreamConsumer[] Consumers { get; }

    #endregion
    #region Constructors

    internal StreamPendingInfo(int pendingMessageCount, ValkeyValue lowestId, ValkeyValue highestId, StreamConsumer[] consumers)
    {
        PendingMessageCount = pendingMessageCount;
        LowestPendingMessageId = lowestId;
        HighestPendingMessageId = highestId;
        Consumers = consumers;
    }

    #endregion
}
