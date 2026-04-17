// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.ResponseHandler;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<double> GetCacheHitRateAsync()
    {
        Message message = MessageContainer.GetMessageForCall();
        GetCacheMetricsFfi(ClientPointer, (ulong)message.Index, (uint)CacheMetricsType.HitRate);
        IntPtr response = await message;
        try
        {
            object? value = HandleResponse(response);
            return value is double d ? d : Convert.ToDouble(value);
        }
        finally
        {
            FreeResponse(response);
        }
    }

    /// <inheritdoc/>
    public async Task<double> GetCacheMissRateAsync()
    {
        Message message = MessageContainer.GetMessageForCall();
        GetCacheMetricsFfi(ClientPointer, (ulong)message.Index, (uint)CacheMetricsType.MissRate);
        IntPtr response = await message;
        try
        {
            object? value = HandleResponse(response);
            return value is double d ? d : Convert.ToDouble(value);
        }
        finally
        {
            FreeResponse(response);
        }
    }

    /// <inheritdoc/>
    public async Task<long> GetCacheEntryCountAsync()
    {
        Message message = MessageContainer.GetMessageForCall();
        GetCacheMetricsFfi(ClientPointer, (ulong)message.Index, (uint)CacheMetricsType.EntryCount);
        IntPtr response = await message;
        try
        {
            object? value = HandleResponse(response);
            return value is long l ? l : Convert.ToInt64(value);
        }
        finally
        {
            FreeResponse(response);
        }
    }

    /// <inheritdoc/>
    public async Task<long> GetCacheEvictionsAsync()
    {
        Message message = MessageContainer.GetMessageForCall();
        GetCacheMetricsFfi(ClientPointer, (ulong)message.Index, (uint)CacheMetricsType.Evictions);
        IntPtr response = await message;
        try
        {
            object? value = HandleResponse(response);
            return value is long l ? l : Convert.ToInt64(value);
        }
        finally
        {
            FreeResponse(response);
        }
    }

    /// <inheritdoc/>
    public async Task<long> GetCacheExpirationsAsync()
    {
        Message message = MessageContainer.GetMessageForCall();
        GetCacheMetricsFfi(ClientPointer, (ulong)message.Index, (uint)CacheMetricsType.Expirations);
        IntPtr response = await message;
        try
        {
            object? value = HandleResponse(response);
            return value is long l ? l : Convert.ToInt64(value);
        }
        finally
        {
            FreeResponse(response);
        }
    }

    /// <inheritdoc/>
    public async Task<long> GetCacheTotalLookupsAsync()
    {
        Message message = MessageContainer.GetMessageForCall();
        GetCacheMetricsFfi(ClientPointer, (ulong)message.Index, (uint)CacheMetricsType.TotalLookups);
        IntPtr response = await message;
        try
        {
            object? value = HandleResponse(response);
            return value is long l ? l : Convert.ToInt64(value);
        }
        finally
        {
            FreeResponse(response);
        }
    }
}
