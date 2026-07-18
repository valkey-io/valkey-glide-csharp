// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Database memory overhead statistics from <see href="https://valkey.io/commands/memory-stats/">MEMORY STATS</see>.
/// </summary>
public sealed record MemoryStatsDb
{
    #region Public Properties

    /// <summary>
    /// Overhead of the main dictionary hashtable.
    /// </summary>
    public required long OverheadHashtableMain { get; init; }

    /// <summary>
    /// Overhead of the expires dictionary hashtable.
    /// </summary>
    public required long OverheadHashtableExpires { get; init; }

    #endregion
    #region Constructors & Builders

    internal MemoryStatsDb() { }

    #endregion
}

/// <summary>
/// Represents a <see href="https://valkey.io/commands/memory-stats/">MEMORY STATS</see> response.
/// </summary>
public sealed record MemoryStats
{
    #region Public Properties

    /// <summary>
    /// Per-database overhead keyed by database index.
    /// </summary>
    public required Dictionary<int, MemoryStatsDb> Db { get; init; }

    /// <summary>
    /// Bytes active (in use) by the allocator.
    /// </summary>
    public required long AllocatorActive { get; init; }

    /// <summary>
    /// Bytes allocated by the allocator.
    /// </summary>
    public required long AllocatorAllocated { get; init; }

    /// <summary>
    /// Bytes of allocator fragmentation.
    /// </summary>
    public required long AllocatorFragmentationBytes { get; init; }

    /// <summary>
    /// Bytes resident (RSS) by the allocator.
    /// </summary>
    public required long AllocatorResident { get; init; }

    /// <summary>
    /// Bytes of allocator RSS overhead.
    /// </summary>
    public required long AllocatorRssBytes { get; init; }

    /// <summary>
    /// Memory used for AOF buffer in bytes.
    /// </summary>
    public required long AofBuffer { get; init; }

    /// <summary>
    /// Memory used by normal clients in bytes.
    /// </summary>
    public required long ClientsNormal { get; init; }

    /// <summary>
    /// Memory used by replica clients in bytes.
    /// </summary>
    public required long ClientsSlaves { get; init; }

    /// <summary>
    /// Memory used to store dataset in bytes.
    /// </summary>
    public required long DatasetBytes { get; init; }

    /// <summary>
    /// Bytes of overall fragmentation.
    /// </summary>
    public required long FragmentationBytes { get; init; }

    /// <summary>
    /// Average bytes per key.
    /// </summary>
    public required long KeysBytesPerKey { get; init; }

    /// <summary>
    /// Total number of keys across all databases.
    /// </summary>
    public required long KeysCount { get; init; }

    /// <summary>
    /// Memory used by Lua caches in bytes.
    /// </summary>
    public required long LuaCaches { get; init; }

    /// <summary>
    /// Total memory overhead in bytes.
    /// </summary>
    public required long OverheadTotal { get; init; }

    /// <summary>
    /// Peak memory consumed by the server in bytes.
    /// </summary>
    public required long PeakAllocated { get; init; }

    /// <summary>
    /// Memory used for replication backlog in bytes.
    /// </summary>
    public required long ReplicationBacklog { get; init; }

    /// <summary>
    /// Bytes of RSS overhead.
    /// </summary>
    public required long RssOverheadBytes { get; init; }

    /// <summary>
    /// Memory consumed at startup in bytes.
    /// </summary>
    public required long StartupAllocated { get; init; }

    /// <summary>
    /// Total bytes allocated by the server.
    /// </summary>
    public required long TotalAllocated { get; init; }

    /// <summary>
    /// Ratio of allocator fragmentation.
    /// </summary>
    public required double AllocatorFragmentationRatio { get; init; }

    /// <summary>
    /// Ratio of allocator RSS overhead.
    /// </summary>
    public required double AllocatorRssRatio { get; init; }

    /// <summary>
    /// Percentage of net memory used for dataset.
    /// </summary>
    public required double DatasetPercentage { get; init; }

    /// <summary>
    /// Overall memory fragmentation ratio.
    /// </summary>
    public required double Fragmentation { get; init; }

    /// <summary>
    /// Percentage of peak.allocated out of total.allocated.
    /// </summary>
    public required double PeakPercentage { get; init; }

    /// <summary>
    /// Ratio of RSS overhead.
    /// </summary>
    public required double RssOverheadRatio { get; init; }

    /// <summary>
    /// Memory used by cluster links in bytes.
    /// </summary>
    /// <note>Since Valkey 7.0.0.</note>
    public long? ClusterLinks { get; init; }

    /// <summary>
    /// Memory used by functions caches in bytes.
    /// </summary>
    /// <note>Since Valkey 7.0.0.</note>
    public long? FunctionsCaches { get; init; }

    /// <summary>
    /// Memory used by allocator muzzy pages in bytes.
    /// </summary>
    /// <note>Since Valkey 8.0.0.</note>
    public long? AllocatorMuzzy { get; init; }

    /// <summary>
    /// Count of db dictionaries currently rehashing.
    /// </summary>
    /// <note>Since Valkey 8.0.0.</note>
    public long? DbDictRehashingCount { get; init; }

    /// <summary>
    /// Overhead of db hashtable LUT in bytes.
    /// </summary>
    /// <note>Since Valkey 8.0.0.</note>
    public long? OverheadDbHashtableLut { get; init; }

    /// <summary>
    /// Overhead of db hashtable rehashing in bytes.
    /// </summary>
    /// <note>Since Valkey 8.0.0.</note>
    public long? OverheadDbHashtableRehashing { get; init; }

    #endregion
    #region Constructors & Builders

    internal MemoryStats() { }

    #endregion
}
