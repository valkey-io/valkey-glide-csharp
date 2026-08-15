// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Full stream information from a <c>XINFO STREAM</c> response.
/// </summary>
/// <seealso href="https://valkey.io/commands/xinfo-stream/"/>
public readonly struct StreamInfoFull
{
    #region Public Properties

    /// <summary>
    /// The number of entries in the stream (<c>length</c>).
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// The number of radix tree keys in the stream (<c>radix-tree-keys</c>).
    /// </summary>
    public int RadixTreeKeys { get; }

    /// <summary>
    /// The number of radix tree nodes in the stream (<c>radix-tree-nodes</c>).
    /// </summary>
    public int RadixTreeNodes { get; }

    /// <summary>
    /// The last generated id (<c>last-generated-id</c>),
    /// or <see cref="ValkeyValue.Null"/> if not specified.
    /// </summary>
    public ValkeyValue LastGeneratedId { get; }

    /// <summary>
    /// The maximal entry ID that was deleted from the stream (<c>max-deleted-entry-id</c>),
    /// or <see cref="ValkeyValue.Null"/> if not specified.
    /// </summary>
    /// <remarks>Since Valkey 7.0.0.</remarks>
    public ValkeyValue MaxDeletedEntryId { get; }

    /// <summary>
    /// The count of all entries added to the stream during its lifetime
    /// (<c>entries-added</c>), or <c>-1</c> if not specified.
    /// </summary>
    /// <remarks>Since Valkey 7.0.0.</remarks>
    public long EntriesAdded { get; }

    /// <summary>
    /// The first id recorded for the stream (<c>recorded-first-entry-id</c>),
    /// or <see cref="ValkeyValue.Null"/> if not specified.
    /// </summary>
    /// <remarks>Since Valkey 7.0.0.</remarks>
    public ValkeyValue RecordedFirstEntryId { get; }

    /// <summary>
    /// The stream entries (<c>entries</c>).
    /// </summary>
    public StreamEntry[] Entries { get; }

    /// <summary>
    /// The consumer groups defined for the stream (<c>groups</c>).
    /// </summary>
    public StreamGroupInfoFull[] Groups { get; }

    #endregion
    #region Constructors

    internal StreamInfoFull(
        int length,
        int radixTreeKeys,
        int radixTreeNodes,
        ValkeyValue lastGeneratedId,
        ValkeyValue maxDeletedEntryId,
        long entriesAdded,
        ValkeyValue recordedFirstEntryId,
        StreamEntry[] entries,
        StreamGroupInfoFull[] groups)
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

    #endregion
}
