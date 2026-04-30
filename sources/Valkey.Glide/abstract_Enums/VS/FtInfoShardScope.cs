// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Controls which shards participate in the FT.INFO query.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
public enum FtInfoShardScope
{
    /// <summary>Queries all shards (default).</summary>
    AllShards,
    /// <summary>Queries only a subset of shards.</summary>
    SomeShards,
}

internal static class FtInfoShardScopeExtensions
{
    internal static GlideString ToLiteral(this FtInfoShardScope scope)
        => scope switch
        {
            FtInfoShardScope.AllShards => ValkeyLiterals.ALLSHARDS,
            FtInfoShardScope.SomeShards => ValkeyLiterals.SOMESHARDS,
            _ => throw new ArgumentOutOfRangeException(nameof(scope)),
        };
}
