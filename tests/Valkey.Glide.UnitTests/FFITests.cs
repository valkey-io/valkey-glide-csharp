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
