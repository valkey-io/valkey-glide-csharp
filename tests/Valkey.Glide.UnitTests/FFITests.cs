// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

// TODO #650: Add layout tests for the remaining FFI structs.

/// <summary>
/// Tests that FFI structs marshal to the layout of their Rust <c>#[repr(C)]</c> counterparts.
/// </summary>
public class FFITests
{
    #region Tests

    [Fact]
    public void IamCredentials_MatchesRustLayout()
    {
        // Rust `IamCredentials` struct in `rust/src/ffi.rs` on 64-bit platforms:
        // - cluster_name: *const c_char (0)
        // - region: *const c_char (8)
        // - service_type: ServiceType (16)
        // - has_refresh_interval_seconds: bool (20)
        // - refresh_interval_seconds: u32 (24)

        // With refresh interval.
        var withInterval = ToBytes(new FFI.IamCredentials("cluster", "region", (FFI.ServiceType)ServiceType.ElastiCache, 100));

        Assert.Equal(32, withInterval.Length);
        Assert.Equal(1, withInterval[20]);
        Assert.Equal(100u, BitConverter.ToUInt32(withInterval, 24));

        // Without refresh interval.
        var withoutInterval = ToBytes(new FFI.IamCredentials("cluster", "region", (FFI.ServiceType)ServiceType.ElastiCache, null));

        Assert.Equal(32, withoutInterval.Length);
        Assert.Equal(0, withoutInterval[20]);
    }

    [Fact]
    public void RetryStrategy_MatchesRustLayout()
    {
        // Rust `ConnectionRetryStrategy` struct in `rust/src/ffi.rs`:
        // - exponent_base: u32 (0)
        // - factor: u32 (4)
        // - number_of_retries: u32 (8)
        // - has_jitter_percent: bool (12)
        // - jitter_percent: u32 (16)

        // With jitter.
        var withJitter = ToBytes(new ConnectionConfiguration.RetryStrategy(numberOfRetries: 7, factor: 100, exponentBase: 2, jitterPercent: 20));

        Assert.Equal(20, withJitter.Length);
        Assert.Equal(2u, BitConverter.ToUInt32(withJitter, 0));
        Assert.Equal(100u, BitConverter.ToUInt32(withJitter, 4));
        Assert.Equal(7u, BitConverter.ToUInt32(withJitter, 8));
        Assert.Equal(1, withJitter[12]);
        Assert.Equal(20u, BitConverter.ToUInt32(withJitter, 16));

        // Without jitter.
        var withoutJitter = ToBytes(new ConnectionConfiguration.RetryStrategy(numberOfRetries: 7, factor: 100, exponentBase: 2));

        Assert.Equal(20, withoutJitter.Length);
        Assert.Equal(0, withoutJitter[12]);
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Marshals <paramref name="value"/> to unmanaged memory and returns a copy of its bytes.
    /// </summary>
    private static byte[] ToBytes<T>(T value) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(value, ptr, false);
            try
            {
                byte[] bytes = new byte[size];
                Marshal.Copy(ptr, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.DestroyStructure<T>(ptr);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    #endregion
}
