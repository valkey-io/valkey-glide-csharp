// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Provides offset/count pagination for FT commands (LIMIT clause).
/// </summary>
/// <remarks>
/// <paramref name="offset"/> defaults to <c>0</c> (start from the first result).
/// <paramref name="count"/> defaults to <c>-1</c> (return all results).
/// </remarks>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public sealed class FtLimit(long offset = 0, long count = -1) : IFtAggregateClause
{
    /// <summary>
    /// Number of results to skip.
    /// </summary>
    public long Offset { get; init; } = offset;

    /// <summary>
    /// Number of results to return. <c>-1</c> returns all results.
    /// </summary>
    public long Count { get; init; } = count;

    internal GlideString[] ToArgs() => [ValkeyLiterals.LIMIT, Offset.ToGlideString(), Count.ToGlideString()];

    GlideString[] IFtAggregateClause.ToArgs() => ToArgs();
}
