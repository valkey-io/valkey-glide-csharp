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
        ValkeyValue maxDeletedEntryId,
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
    public ValkeyValue MaxDeletedEntryId { get; }

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
