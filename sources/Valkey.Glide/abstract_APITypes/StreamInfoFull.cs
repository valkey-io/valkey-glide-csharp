// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes detailed stream information retrieved using the <c>XINFO STREAM key FULL</c> command.
/// This includes consumer group details, PEL entries per consumer, and the full entries list.
/// </summary>
public readonly struct StreamInfoFull
{
    internal StreamInfoFull(
        long length,
        long radixTreeKeys,
        long radixTreeNodes,
        ValkeyValue lastGeneratedId,
        long maxDeletedEntryId,
        long entriesAdded,
        ValkeyValue recordedFirstEntryId,
        StreamEntry[] entries,
        StreamGroupFullInfo[] groups)
    {
        Length = length;
        RadixTreeKeys = radixTreeKeys;
        RadixTreeNodes = radixTreeNodes;
        LastGeneratedId = lastGeneratedId;
        MaxDeletedEntryId = maxDeletedEntryId;
        EntriesAdded = entriesAdded;
        RecordedFirstEntryId = recordedFirstEntryId;
        Entries = entries;
        Groups = groups;
    }

    /// <summary>
    /// The number of entries in the stream.
    /// </summary>
    public long Length { get; }

    /// <summary>
    /// The number of radix tree keys in the stream.
    /// </summary>
    public long RadixTreeKeys { get; }

    /// <summary>
    /// The number of radix tree nodes in the stream.
    /// </summary>
    public long RadixTreeNodes { get; }

    /// <summary>
    /// The last generated ID in the stream.
    /// </summary>
    public ValkeyValue LastGeneratedId { get; }

    /// <summary>
    /// The ID of the maximum deleted entry. Available since server 7.0.
    /// </summary>
    public long MaxDeletedEntryId { get; }

    /// <summary>
    /// The total number of entries added to the stream since creation. Available since server 7.0.
    /// </summary>
    public long EntriesAdded { get; }

    /// <summary>
    /// The recorded first entry ID. Available since server 7.0.
    /// </summary>
    public ValkeyValue RecordedFirstEntryId { get; }

    /// <summary>
    /// The stream entries.
    /// </summary>
    public StreamEntry[] Entries { get; }

    /// <summary>
    /// The consumer groups associated with the stream.
    /// </summary>
    public StreamGroupFullInfo[] Groups { get; }
}

/// <summary>
/// Describes a consumer group in the detailed stream info returned by <c>XINFO STREAM key FULL</c>.
/// </summary>
public readonly struct StreamGroupFullInfo
{
    internal StreamGroupFullInfo(
        string name,
        ValkeyValue lastDeliveredId,
        long? entriesRead,
        long pelCount,
        StreamConsumerFullInfo[] consumers,
        StreamPendingEntryInfo[] pendingEntries)
    {
        Name = name;
        LastDeliveredId = lastDeliveredId;
        EntriesRead = entriesRead;
        PelCount = pelCount;
        Consumers = consumers;
        PendingEntries = pendingEntries;
    }

    /// <summary>
    /// The name of the consumer group.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The ID of the last message delivered to the group.
    /// </summary>
    public ValkeyValue LastDeliveredId { get; }

    /// <summary>
    /// Total number of entries the group had read. Available since server 7.0.
    /// </summary>
    public long? EntriesRead { get; }

    /// <summary>
    /// The total number of pending entries (PEL count) for the group.
    /// </summary>
    public long PelCount { get; }

    /// <summary>
    /// The consumers within this group.
    /// </summary>
    public StreamConsumerFullInfo[] Consumers { get; }

    /// <summary>
    /// The group-level pending entries list (PEL).
    /// </summary>
    public StreamPendingEntryInfo[] PendingEntries { get; }
}

/// <summary>
/// Describes a consumer in the detailed stream info returned by <c>XINFO STREAM key FULL</c>.
/// </summary>
public readonly struct StreamConsumerFullInfo
{
    internal StreamConsumerFullInfo(
        string name,
        long seenTime,
        long activeTime,
        long pelCount,
        StreamPendingEntryInfo[] pendingEntries)
    {
        Name = name;
        SeenTime = seenTime;
        ActiveTime = activeTime;
        PelCount = pelCount;
        PendingEntries = pendingEntries;
    }

    /// <summary>
    /// The name of the consumer.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The last time the consumer was seen (Unix timestamp in milliseconds).
    /// </summary>
    public long SeenTime { get; }

    /// <summary>
    /// The last time the consumer was active (Unix timestamp in milliseconds). Available since server 7.2.
    /// </summary>
    public long ActiveTime { get; }

    /// <summary>
    /// The number of pending entries for this consumer.
    /// </summary>
    public long PelCount { get; }

    /// <summary>
    /// The consumer-level pending entries list (PEL).
    /// </summary>
    public StreamPendingEntryInfo[] PendingEntries { get; }
}

/// <summary>
/// Describes a pending entry in the PEL (Pending Entries List) returned by <c>XINFO STREAM key FULL</c>.
/// </summary>
public readonly struct StreamPendingEntryInfo
{
    internal StreamPendingEntryInfo(
        ValkeyValue entryId,
        string? consumerName,
        long deliveryTime,
        int deliveryCount)
    {
        EntryId = entryId;
        ConsumerName = consumerName;
        DeliveryTime = deliveryTime;
        DeliveryCount = deliveryCount;
    }

    /// <summary>
    /// The ID of the pending entry.
    /// </summary>
    public ValkeyValue EntryId { get; }

    /// <summary>
    /// The name of the consumer that owns this pending entry.
    /// This is <c>null</c> when the entry is nested under a specific consumer.
    /// </summary>
    public string? ConsumerName { get; }

    /// <summary>
    /// The delivery time of the entry (Unix timestamp in milliseconds).
    /// </summary>
    public long DeliveryTime { get; }

    /// <summary>
    /// The number of times this entry has been delivered.
    /// </summary>
    public int DeliveryCount { get; }
}
