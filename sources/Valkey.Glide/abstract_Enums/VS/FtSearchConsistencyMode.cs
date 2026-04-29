// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Controls consistency requirements for an FT.SEARCH query (cluster mode).
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public enum FtSearchConsistencyMode
{
    /// <summary>Requires consistent results across shards.</summary>
    Consistent,
    /// <summary>Allows inconsistent (faster) results.</summary>
    Inconsistent,
}

internal static class FtSearchConsistencyModeExtensions
{
    internal static GlideString ToLiteral(this FtSearchConsistencyMode mode)
        => mode switch
        {
            FtSearchConsistencyMode.Consistent => ValkeyLiterals.CONSISTENT,
            FtSearchConsistencyMode.Inconsistent => ValkeyLiterals.INCONSISTENT,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
}
