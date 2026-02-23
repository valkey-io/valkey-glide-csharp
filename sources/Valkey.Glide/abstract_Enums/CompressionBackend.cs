// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Compression backend types supported by Valkey GLIDE.
/// </summary>
public enum CompressionBackend : uint
{
    /// <summary>
    /// Zstandard (zstd) compression backend.
    /// Provides high compression ratios with good performance.
    /// Default compression level: 3
    /// </summary>
    Zstd = 0,

    /// <summary>
    /// LZ4 compression backend.
    /// Provides fast compression and decompression with moderate compression ratios.
    /// Default compression level: 0
    /// </summary>
    Lz4 = 1,
}
