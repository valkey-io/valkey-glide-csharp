// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for client-side caching with TTL-based expiration.
/// <para />
/// Configures a local cache that stores read command responses on the client side
/// to reduce network round-trips and server load. The cache uses Time-To-Live (TTL)
/// based expiration, where entries are automatically removed after a specified duration.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>
///     Valkey GLIDE supports TTL-based caching only. Invalidation-based client-side
///     caching (where the server notifies clients of key changes) is not supported.
///     This means cached values may become stale if updated on the server before the TTL expires.
///   </item>
///   <item>
///     Valkey GLIDE client-side cache supports lazy eviction only. Expired entries
///     are removed only when accessed after their TTL has expired. There is no proactive
///     background cleanup of expired entries.
///   </item>
///   <item>
///     Only read commands that retrieve entire values are cached
///     (GET, HGETALL, SMEMBERS).
///   </item>
/// </list>
/// In order for two clients to share the same cache, they must be created with the same
/// <see cref="ClientSideCacheConfig"/> instance.
/// <list type="bullet">
///   <item>Clients with different <see cref="ClientSideCacheConfig"/> instances will have separate caches, even if the configurations are identical.</item>
///   <item>Clients using different databases cannot share the same cache.</item>
///   <item>Clients using different ACL users cannot share the same cache.</item>
/// </list>
/// </remarks>
/// <seealso cref="EvictionPolicy"/>
public sealed class ClientSideCacheConfig
{
    /// <summary>
    /// A unique identifier for the cache instance.
    /// Multiple clients can share the same cache by using the same <see cref="ClientSideCacheConfig"/> instance.
    /// </summary>
    // Internal for testing.
    internal string CacheId { get; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// The maximum size of the cache in kilobytes (KB).
    /// This limits the total memory used by cached keys and values.
    /// When this limit is reached, entries are evicted based on the <see cref="EvictionPolicy"/>.
    /// </summary>
    public ulong MaxCacheKb { get; }

    /// <summary>
    /// The Time-To-Live for cached entries.
    /// After this duration, entries automatically expire and are removed from the cache.
    /// <see cref="TimeSpan.Zero"/> means no expiration is applied (entries remain until evicted).
    /// </summary>
    public TimeSpan EntryTtl { get; }

    /// <summary>
    /// The policy for evicting entries when the cache reaches its maximum size.
    /// Defaults to <see cref="Valkey.Glide.EvictionPolicy.LRU"/>.
    /// </summary>
    public EvictionPolicy EvictionPolicy { get; private set; } = EvictionPolicy.LRU;

    /// <summary>
    /// Whether collection of cache metrics (hit/miss rates, evictions, etc.) is enabled.
    /// </summary>
    public bool EnableMetrics { get; private set; }

    /// <summary>
    /// Creates a new <see cref="ClientSideCacheConfig"/> with an auto-generated unique cache ID.
    /// </summary>
    /// <param name="maxCacheKb">Maximum size of the cache in kilobytes (KB). Must be positive.</param>
    /// <param name="entryTtl">Time-To-Live for cached entries. Use <see cref="TimeSpan.Zero"/> to disable expiration.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxCacheKb"/> is zero or <paramref name="entryTtl"/> is negative.</exception>
    /// <example>
    /// <code>
    /// var cache = new ClientSideCacheConfig(1024, TimeSpan.FromMinutes(1))
    ///     .WithEvictionPolicy(EvictionPolicy.LRU)
    ///     .WithMetrics(true);
    ///
    /// var config = new StandaloneClientConfigurationBuilder()
    ///     .WithAddress("localhost", 6379)
    ///     .WithClientSideCache(cache)
    ///     .Build();
    /// </code>
    /// </example>
    public ClientSideCacheConfig(ulong maxCacheKb, TimeSpan entryTtl)
    {
        if (maxCacheKb == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxCacheKb), "maxCacheKb must be positive.");
        }

        if (entryTtl < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(entryTtl), "entryTtl must not be negative.");
        }

        MaxCacheKb = maxCacheKb;
        EntryTtl = entryTtl;
    }

    /// <summary>
    /// Sets the eviction policy for the cache.
    /// </summary>
    /// <param name="policy">The eviction policy to use.</param>
    /// <returns>This instance for method chaining.</returns>
    public ClientSideCacheConfig WithEvictionPolicy(EvictionPolicy policy)
    {
        EvictionPolicy = policy;
        return this;
    }

    /// <summary>
    /// Enables or disables cache metrics collection.
    /// </summary>
    /// <param name="enable">Whether to enable metrics.</param>
    /// <returns>This instance for method chaining.</returns>
    public ClientSideCacheConfig WithMetrics(bool enable = true)
    {
        EnableMetrics = enable;
        return this;
    }

    /// <summary>
    /// Converts to the FFI representation for marshalling to Rust core.
    /// </summary>
    internal Internals.FFI.ClientSideCacheConfig ToFfi() => new(
        CacheId,
        MaxCacheKb,
        (ulong)EntryTtl.TotalMilliseconds,
        EvictionPolicy,
        EnableMetrics
    );
}
