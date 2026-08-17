// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A pending entry from a <c>XINFO STREAM</c> response.
/// </summary>
/// <seealso href="https://valkey.io/commands/xinfo-stream/"/>
public readonly struct StreamPendingEntry
{
    #region Public Properties

    /// <summary>
    /// The ID of the pending entry.
    /// </summary>
    public ValkeyValue EntryId { get; }

    /// <summary>
    /// The name of the consumer that owns this pending entry (<c>consumer</c>).
    /// </summary>
    /// <remarks>
    /// For a consumer's PEL the name is not carried on the wire and is taken from the owning consumer.
    /// </remarks>
    public string Consumer { get; }

    /// <summary>
    /// The time the entry was last delivered to a consumer.
    /// </summary>
    public DateTimeOffset DeliveryTime { get; }

    /// <summary>
    /// The number of times this entry has been delivered to a consumer.
    /// </summary>
    public int DeliveryCount { get; }

    #endregion
    #region Constructors

    internal StreamPendingEntry(
        ValkeyValue entryId,
        string consumer,
        DateTimeOffset deliveryTime,
        int deliveryCount)
    {
        EntryId = entryId;
        Consumer = consumer;
        DeliveryTime = deliveryTime;
        DeliveryCount = deliveryCount;
    }

    #endregion
}
