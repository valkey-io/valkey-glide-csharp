// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by Valkey GLIDE clients but NOT by StackExchange.Redis databases.

/// <summary>
/// Client-side cache commands for Valkey GLIDE clients.
/// These methods provide access to cache metrics when client-side caching is enabled.
/// </summary>
/// <seealso cref="ClientSideCacheConfig"/>
public partial interface IBaseClient
{
    /// <summary>
    /// Returns the cache hit rate as a value between 0.0 and 1.0.
    /// A higher hit rate indicates better cache performance.
    /// </summary>
    /// <returns>The cache hit rate (0.0 to 1.0).</returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown when client-side caching is not enabled or metrics collection is disabled.
    /// </exception>
    /// <example>
    /// <code>
    /// double hitRate = await client.GetCacheHitRateAsync();
    /// Console.WriteLine($"Cache hit rate: {hitRate:P2}");
    /// </code>
    /// </example>
    Task<double> GetCacheHitRateAsync();

    /// <summary>
    /// Returns the cache miss rate as a value between 0.0 and 1.0.
    /// A lower miss rate indicates better cache performance.
    /// </summary>
    /// <returns>The cache miss rate (0.0 to 1.0).</returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown when client-side caching is not enabled or metrics collection is disabled.
    /// </exception>
    /// <example>
    /// <code>
    /// double missRate = await client.GetCacheMissRateAsync();
    /// Console.WriteLine($"Cache miss rate: {missRate:P2}");
    /// </code>
    /// </example>
    Task<double> GetCacheMissRateAsync();

    /// <summary>
    /// Returns the current number of entries in the client-side cache.
    /// </summary>
    /// <returns>The number of cache entries.</returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown when client-side caching is not enabled.
    /// </exception>
    /// <example>
    /// <code>
    /// long entryCount = await client.GetCacheEntryCountAsync();
    /// Console.WriteLine($"Cache contains {entryCount} entries");
    /// </code>
    /// </example>
    Task<long> GetCacheEntryCountAsync();

    /// <summary>
    /// Returns the total number of cache evictions.
    /// Evictions occur when the cache reaches its maximum size and entries are removed
    /// based on the configured <see cref="EvictionPolicy"/>.
    /// </summary>
    /// <returns>The number of cache evictions.</returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown when client-side caching is not enabled or metrics collection is disabled.
    /// </exception>
    /// <example>
    /// <code>
    /// long evictions = await client.GetCacheEvictionsAsync();
    /// Console.WriteLine($"Cache has evicted {evictions} entries");
    /// </code>
    /// </example>
    Task<long> GetCacheEvictionsAsync();

    /// <summary>
    /// Returns the total number of cache expirations.
    /// Expirations occur when entries exceed their TTL (Time-To-Live).
    /// </summary>
    /// <returns>The number of cache expirations.</returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown when client-side caching is not enabled or metrics collection is disabled.
    /// </exception>
    /// <example>
    /// <code>
    /// long expirations = await client.GetCacheExpirationsAsync();
    /// Console.WriteLine($"Cache has expired {expirations} entries");
    /// </code>
    /// </example>
    Task<long> GetCacheExpirationsAsync();

    /// <summary>
    /// Returns the total number of cache lookups (hits + misses).
    /// </summary>
    /// <returns>The total number of cache lookups.</returns>
    /// <exception cref="Errors.RequestException">
    /// Thrown when client-side caching is not enabled or metrics collection is disabled.
    /// </exception>
    /// <example>
    /// <code>
    /// long totalLookups = await client.GetCacheTotalLookupsAsync();
    /// Console.WriteLine($"Cache has performed {totalLookups} lookups");
    /// </code>
    /// </example>
    Task<long> GetCacheTotalLookupsAsync();
}
