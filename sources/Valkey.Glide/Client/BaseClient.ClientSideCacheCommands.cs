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
        => GetCacheMetricAsync(CacheMetricsType.HitRate, v => Convert.ToDouble(v));

    /// <inheritdoc/>
    public Task<double> GetCacheMissRateAsync()
        => GetCacheMetricAsync(CacheMetricsType.MissRate, v => Convert.ToDouble(v));

    /// <inheritdoc/>
    public Task<long> GetCacheEntryCountAsync()
        => GetCacheMetricAsync(CacheMetricsType.EntryCount, v => Convert.ToInt64(v));

    /// <inheritdoc/>
    public Task<long> GetCacheEvictionsAsync()
        => GetCacheMetricAsync(CacheMetricsType.Evictions, v => Convert.ToInt64(v));

    /// <inheritdoc/>
    public Task<long> GetCacheExpirationsAsync()
        => GetCacheMetricAsync(CacheMetricsType.Expirations, v => Convert.ToInt64(v));

    /// <inheritdoc/>
    public Task<long> GetCacheTotalLookupsAsync()
        => GetCacheMetricAsync(CacheMetricsType.TotalLookups, v => Convert.ToInt64(v));
}
