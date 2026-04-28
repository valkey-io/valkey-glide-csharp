// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Controls which shards participate in an FT.SEARCH query (cluster mode).
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public enum FtSearchShardScope
{
    /// <summary>Queries all shards (default). Terminates with timeout error if not all shards respond.</summary>
    AllShards,
    /// <summary>Queries only a subset of shards. Generates a best-effort reply if not all shards respond.</summary>
    SomeShards,
}

internal static class FtSearchShardScopeExtensions
{
    internal static GlideString ToLiteral(this FtSearchShardScope scope)
        => scope switch
        {
            FtSearchShardScope.AllShards => ValkeyLiterals.ALLSHARDS,
            FtSearchShardScope.SomeShards => ValkeyLiterals.SOMESHARDS,
            _ => throw new ArgumentOutOfRangeException(nameof(scope)),
        };
}
