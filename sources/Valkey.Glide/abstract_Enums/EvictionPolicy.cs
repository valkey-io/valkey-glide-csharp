// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Eviction policy for client-side cache entries.
/// When the cache reaches its maximum size, it must evict existing entries to make room for new ones.
/// Values must match the corresponding enum in glide-core.
/// </summary>
public enum EvictionPolicy : uint
{
    /// <summary>
    /// Least Recently Used — evicts the least recently accessed entry.
    /// Best for recency-biased workloads like event streams and job queues.
    /// </summary>
    LRU = 0,

    /// <summary>
    /// Least Frequently Used — evicts the least frequently accessed entry.
    /// Best for frequency-biased workloads like user profiles and product catalogs.
    /// </summary>
    LFU = 1,
}
