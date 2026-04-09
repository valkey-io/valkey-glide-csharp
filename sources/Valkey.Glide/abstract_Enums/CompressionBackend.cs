// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Compression backend types supported by Valkey GLIDE.
/// Values must match the corresponding enum in glide-core.
/// </summary>
public enum CompressionBackend : uint
{
    /// <summary>
    /// Zstandard (zstd) compression backend.
    /// </summary>
    /// <seealso href="https://facebook.github.io/zstd/"/>
    Zstd = 0,

    /// <summary>
    /// LZ4 compression backend.
    /// </summary>
    /// <seealso href="https://lz4.org/"/>
    Lz4 = 1,
}
