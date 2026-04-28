// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Sort order for FT.SEARCH SORTBY clause.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public enum FtSearchSortOrder
{
    /// <summary>Ascending order.</summary>
    Ascending,
    /// <summary>Descending order.</summary>
    Descending,
}

internal static class FtSearchSortOrderExtensions
{
    internal static GlideString ToLiteral(this FtSearchSortOrder order)
        => order switch
        {
            FtSearchSortOrder.Ascending => ValkeyLiterals.ASC,
            FtSearchSortOrder.Descending => ValkeyLiterals.DESC,
            _ => throw new ArgumentOutOfRangeException(nameof(order)),
        };
}
