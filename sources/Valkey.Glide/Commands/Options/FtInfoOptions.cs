// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Controls the scope of information returned by FT.INFO.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
public enum FtInfoScope
{
    /// <summary>Returns per-node (local shard) index information.</summary>
    LOCAL,
    /// <summary>Returns aggregated information from the primary coordinator.</summary>
    PRIMARY,
    /// <summary>Returns cluster-wide aggregated index information.</summary>
    CLUSTER,
}

/// <summary>
/// Controls which shards participate in the FT.INFO query.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
public enum FtInfoShardScope
{
    /// <summary>Queries all shards (default).</summary>
    ALLSHARDS,
    /// <summary>Queries only a subset of shards.</summary>
    SOMESHARDS,
}

/// <summary>
/// Controls consistency requirements for the FT.INFO query.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
public enum FtInfoConsistencyMode
{
    /// <summary>Requires consistent results across shards.</summary>
    CONSISTENT,
    /// <summary>Allows inconsistent (faster) results.</summary>
    INCONSISTENT,
}

/// <summary>
/// Optional arguments for the FT.INFO command.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.info/">valkey.io</seealso>
public sealed class FtInfoOptions
{
    /// <summary>Controls the scope of information returned.</summary>
    public FtInfoScope? Scope { get; set; }
    /// <summary>Controls which shards participate in the query.</summary>
    public FtInfoShardScope? ShardScope { get; set; }
    /// <summary>Controls consistency requirements.</summary>
    public FtInfoConsistencyMode? Consistency { get; set; }

    /// <summary>
    /// Returns the command arguments for these options.
    /// </summary>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [];
        if (Scope.HasValue)
            args.Add(Scope.Value switch
            {
                FtInfoScope.LOCAL => ValkeyLiterals.LOCAL,
                FtInfoScope.PRIMARY => ValkeyLiterals.PRIMARY,
                FtInfoScope.CLUSTER => ValkeyLiterals.CLUSTER,
                _ => Scope.Value.ToString(),
            });
        if (ShardScope.HasValue)
            args.Add(ShardScope.Value switch
            {
                FtInfoShardScope.ALLSHARDS => ValkeyLiterals.ALLSHARDS,
                FtInfoShardScope.SOMESHARDS => ValkeyLiterals.SOMESHARDS,
                _ => ShardScope.Value.ToString(),
            });
        if (Consistency.HasValue)
            args.Add(Consistency.Value switch
            {
                FtInfoConsistencyMode.CONSISTENT => ValkeyLiterals.CONSISTENT,
                FtInfoConsistencyMode.INCONSISTENT => ValkeyLiterals.INCONSISTENT,
                _ => Consistency.Value.ToString(),
            });
        return [.. args];
    }
}
