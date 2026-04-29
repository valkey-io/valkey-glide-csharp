// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Controls the scope of information returned by FT.INFO.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
public enum FtInfoScope
{
    /// <summary>Returns per-node (local shard) index information.</summary>
    Local,
    /// <summary>Returns aggregated information from the primary coordinator.</summary>
    Primary,
    /// <summary>Returns cluster-wide aggregated index information.</summary>
    Cluster,
}

internal static class FtInfoScopeExtensions
{
    internal static GlideString ToLiteral(this FtInfoScope scope)
        => scope switch
        {
            FtInfoScope.Local => ValkeyLiterals.LOCAL,
            FtInfoScope.Primary => ValkeyLiterals.PRIMARY,
            FtInfoScope.Cluster => ValkeyLiterals.CLUSTER,
            _ => throw new ArgumentOutOfRangeException(nameof(scope)),
        };
}
