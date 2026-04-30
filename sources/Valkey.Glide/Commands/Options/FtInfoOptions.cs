// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

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
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];
        if (Scope.HasValue)
        {
            args.Add(Scope.Value.ToLiteral());
        }

        if (ShardScope.HasValue)
        {
            args.Add(ShardScope.Value.ToLiteral());
        }

        if (Consistency.HasValue)
        {
            args.Add(Consistency.Value.ToLiteral());
        }

        return [.. args];
    }
}
