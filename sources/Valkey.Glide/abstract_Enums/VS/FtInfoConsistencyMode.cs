// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Controls consistency requirements for the FT.INFO query.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
public enum FtInfoConsistencyMode
{
    /// <summary>Requires consistent results across shards.</summary>
    Consistent,
    /// <summary>Allows inconsistent (faster) results.</summary>
    Inconsistent,
}

internal static class FtInfoConsistencyModeExtensions
{
    internal static GlideString ToLiteral(this FtInfoConsistencyMode mode)
        => mode switch
        {
            FtInfoConsistencyMode.Consistent => ValkeyLiterals.CONSISTENT,
            FtInfoConsistencyMode.Inconsistent => ValkeyLiterals.INCONSISTENT,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
}
