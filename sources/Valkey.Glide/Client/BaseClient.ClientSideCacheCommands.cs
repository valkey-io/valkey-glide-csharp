// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.ResponseHandler;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    private async Task<T> GetCacheMetricAsync<T>(CacheMetricsType type, Func<object?, T> convert)
    {
        Message message = MessageContainer.GetMessageForCall();
        GetCacheMetricsFfi(ClientPointer, (ulong)message.Index, (uint)type);
        IntPtr response = await message;
        try
        {
            return convert(HandleResponse(response));
        }
        finally
        {
            FreeResponse(response);
        }
    }

    /// <inheritdoc/>
    public Task<double> GetCacheHitRateAsync()
        => GetCacheMetricAsync(CacheMetricsType.HitRate, Convert.ToDouble);

    /// <inheritdoc/>
    public Task<double> GetCacheMissRateAsync()
        => GetCacheMetricAsync(CacheMetricsType.MissRate, Convert.ToDouble);

    /// <inheritdoc/>
    public Task<long> GetCacheEntryCountAsync()
        => GetCacheMetricAsync(CacheMetricsType.EntryCount, Convert.ToInt64);

    /// <inheritdoc/>
    public Task<long> GetCacheEvictionsAsync()
        => GetCacheMetricAsync(CacheMetricsType.Evictions, Convert.ToInt64);

    /// <inheritdoc/>
    public Task<long> GetCacheExpirationsAsync()
        => GetCacheMetricAsync(CacheMetricsType.Expirations, Convert.ToInt64);

    /// <inheritdoc/>
    public Task<long> GetCacheTotalLookupsAsync()
        => GetCacheMetricAsync(CacheMetricsType.TotalLookups, Convert.ToInt64);
}
