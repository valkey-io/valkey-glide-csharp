// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Database memory overhead statistics from <see href="https://valkey.io/commands/memory-stats/">MEMORY STATS</see>.
/// </summary>
public class MemoryStatsDb
{
    /// <summary>
    /// Overhead of the main dictionary hashtable.
    /// </summary>
    public long OverheadHashtableMain { get; init; }

    /// <summary>
    /// Overhead of the expires dictionary hashtable.
    /// </summary>
    public long OverheadHashtableExpires { get; init; }
}

/// <summary>
/// Represents a <see href="https://valkey.io/commands/memory-stats/">MEMORY STATS</see> response.
/// </summary>
public class MemoryStats
{
    /// <summary>
    /// Per-database overhead keyed by database index.
    /// </summary>
    public Dictionary<int, MemoryStatsDb> Db { get; init; } = [];

    /// <summary>
    /// Bytes active (in use) by the allocator.
    /// </summary>
    public long AllocatorActive { get; init; }

    /// <summary>
    /// Bytes allocated by the allocator.
    /// </summary>
    public long AllocatorAllocated { get; init; }

    /// <summary>
    /// Bytes of allocator fragmentation.
    /// </summary>
    public long AllocatorFragmentationBytes { get; init; }

    /// <summary>
    /// Bytes resident (RSS) by the allocator.
    /// </summary>
    public long AllocatorResident { get; init; }

    /// <summary>
    /// Bytes of allocator RSS overhead.
    /// </summary>
    public long AllocatorRssBytes { get; init; }

    /// <summary>
    /// Memory used for AOF buffer in bytes.
    /// </summary>
    public long AofBuffer { get; init; }

    /// <summary>
    /// Memory used by normal clients in bytes.
    /// </summary>
    public long ClientsNormal { get; init; }

    /// <summary>
    /// Memory used by replica clients in bytes.
    /// </summary>
    public long ClientsSlaves { get; init; }

    /// <summary>
    /// Memory used to store dataset in bytes.
    /// </summary>
    public long DatasetBytes { get; init; }

    /// <summary>
    /// Bytes of overall fragmentation.
    /// </summary>
    public long FragmentationBytes { get; init; }

    /// <summary>
    /// Average bytes per key.
    /// </summary>
    public long KeysBytesPerKey { get; init; }

    /// <summary>
    /// Total number of keys across all databases.
    /// </summary>
    public long KeysCount { get; init; }

    /// <summary>
    /// Memory used by Lua caches in bytes.
    /// </summary>
    public long LuaCaches { get; init; }

    /// <summary>
    /// Total memory overhead in bytes.
    /// </summary>
    public long OverheadTotal { get; init; }

    /// <summary>
    /// Peak memory consumed by the server in bytes.
    /// </summary>
    public long PeakAllocated { get; init; }

    /// <summary>
    /// Memory used for replication backlog in bytes.
    /// </summary>
    public long ReplicationBacklog { get; init; }

    /// <summary>
    /// Bytes of RSS overhead.
    /// </summary>
    public long RssOverheadBytes { get; init; }

    /// <summary>
    /// Memory consumed at startup in bytes.
    /// </summary>
    public long StartupAllocated { get; init; }

    /// <summary>
    /// Total bytes allocated by the server.
    /// </summary>
    public long TotalAllocated { get; init; }

    /// <summary>
    /// Ratio of allocator fragmentation.
    /// </summary>
    public double AllocatorFragmentationRatio { get; init; }

    /// <summary>
    /// Ratio of allocator RSS overhead.
    /// </summary>
    public double AllocatorRssRatio { get; init; }

    /// <summary>
    /// Percentage of net memory used for dataset.
    /// </summary>
    public double DatasetPercentage { get; init; }

    /// <summary>
    /// Overall memory fragmentation ratio.
    /// </summary>
    public double Fragmentation { get; init; }

    /// <summary>
    /// Percentage of peak.allocated out of total.allocated.
    /// </summary>
    public double PeakPercentage { get; init; }

    /// <summary>
    /// Ratio of RSS overhead.
    /// </summary>
    public double RssOverheadRatio { get; init; }

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
}
