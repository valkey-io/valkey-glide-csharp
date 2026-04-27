// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Sort order for FT.SEARCH SORTBY clause.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public enum FtSearchSortOrder
{
    /// <summary>Ascending order.</summary>
    ASC,
    /// <summary>Descending order.</summary>
    DESC,
}

/// <summary>
/// Controls which shards participate in an FT.SEARCH query (cluster mode).
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public enum FtSearchShardScope
{
    /// <summary>Queries all shards (default). Terminates with timeout error if not all shards respond.</summary>
    ALLSHARDS,
    /// <summary>Queries only a subset of shards. Generates a best-effort reply if not all shards respond.</summary>
    SOMESHARDS,
}

/// <summary>
/// Controls consistency requirements for an FT.SEARCH query (cluster mode).
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public enum FtSearchConsistencyMode
{
    /// <summary>Requires consistent results across shards.</summary>
    CONSISTENT,
    /// <summary>Allows inconsistent (faster) results.</summary>
    INCONSISTENT,
}

/// <summary>
/// Specifies a field to return in FT.SEARCH results.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchReturnField(string fieldIdentifier)
{
    /// <summary>The field name to return.</summary>
    public string FieldIdentifier { get; } = fieldIdentifier;
    /// <summary>Optional alias to override the field name in the result.</summary>
    public string? Alias { get; set; }
}

/// <summary>
/// Provides pagination for FT.SEARCH results.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchLimit(int offset, int count)
{
    /// <summary>Number of results to skip.</summary>
    public int Offset { get; } = offset;
    /// <summary>Number of results to return.</summary>
    public int Count { get; } = count;
}

/// <summary>
/// A key/value pair passed as a query parameter for FT.SEARCH.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchParam(string key, string value)
{
    /// <summary>The parameter key.</summary>
    public string Key { get; } = key;
    /// <summary>The parameter value.</summary>
    public string Value { get; } = value;
}

/// <summary>
/// Optional arguments for the FT.SEARCH command.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchOptions
{
    /// <summary>Fields to return. If empty, all fields are returned.</summary>
    public FtSearchReturnField[]? ReturnFields { get; set; }
    /// <summary>Overrides the module timeout.</summary>
    public TimeSpan? Timeout { get; set; }
    /// <summary>Key/value pairs referenced from within the query expression.</summary>
    public FtSearchParam[]? Params { get; set; }
    /// <summary>Pagination. Only keys satisfying offset and count are returned.</summary>
    public FtSearchLimit? Limit { get; set; }
    /// <summary>Returns only document IDs without field content.</summary>
    public bool NoContent { get; set; }
    /// <summary>Sets the query dialect version.</summary>
    public int? Dialect { get; set; }
    /// <summary>Disables stemming on text terms in the query.</summary>
    public bool Verbatim { get; set; }
    /// <summary>Requires proximity matching of text terms to be in order.</summary>
    public bool InOrder { get; set; }
    /// <summary>Sets the slop value for proximity matching of text terms.</summary>
    public int? Slop { get; set; }
    /// <summary>Field name to sort results by.</summary>
    public string? SortBy { get; set; }
    /// <summary>Sort direction. Only used when <see cref="SortBy"/> is set.</summary>
    public FtSearchSortOrder? SortByOrder { get; set; }
    /// <summary>Augments output with the sort key value when <see cref="SortBy"/> is set.</summary>
    public bool WithSortKeys { get; set; }
    /// <summary>Controls shard participation in cluster mode.</summary>
    public FtSearchShardScope? ShardScope { get; set; }
    /// <summary>Controls consistency requirements in cluster mode.</summary>
    public FtSearchConsistencyMode? Consistency { get; set; }

    /// <summary>
    /// Returns the command arguments for these options.
    /// </summary>
    public GlideString[] ToArgs()
    {
        if (WithSortKeys && SortBy is null)
        {
            throw new ArgumentException("WithSortKeys requires SortBy to be set.");
        }

        if (SortByOrder.HasValue && SortBy is null)
        {
            throw new ArgumentException("SortByOrder requires SortBy to be set.");
        }

        List<GlideString> args = [];
        if (ShardScope.HasValue)
        {
            args.Add(ShardScope.Value switch
            {
                FtSearchShardScope.ALLSHARDS => ValkeyLiterals.ALLSHARDS,
                FtSearchShardScope.SOMESHARDS => ValkeyLiterals.SOMESHARDS,
                _ => ShardScope.Value.ToString(),
            });
        }

        if (Consistency.HasValue)
        {
            args.Add(Consistency.Value switch
            {
                FtSearchConsistencyMode.CONSISTENT => ValkeyLiterals.CONSISTENT,
                FtSearchConsistencyMode.INCONSISTENT => ValkeyLiterals.INCONSISTENT,
                _ => Consistency.Value.ToString(),
            });
        }

        if (NoContent)
        {
            args.Add(ValkeyLiterals.NOCONTENT);
        }

        if (Verbatim)
        {
            args.Add(ValkeyLiterals.VERBATIM);
        }

        if (InOrder)
        {
            args.Add(ValkeyLiterals.INORDER);
        }

        if (Slop.HasValue) { args.Add(ValkeyLiterals.SLOP); args.Add(Slop.Value.ToString()); }
        if (ReturnFields is { Length: > 0 })
        {
            List<GlideString> fieldArgs = [];
            foreach (var rf in ReturnFields)
            {
                fieldArgs.Add(rf.FieldIdentifier);
                if (rf.Alias is not null) { fieldArgs.Add(ValkeyLiterals.AS); fieldArgs.Add(rf.Alias); }
            }
            args.Add(ValkeyLiterals.RETURN);
            args.Add(fieldArgs.Count.ToString());
            args.AddRange(fieldArgs);
        }
        if (SortBy is not null)
        {
            args.Add(ValkeyLiterals.SORTBY);
            args.Add(SortBy);
            if (SortByOrder.HasValue)
            {
                args.Add(SortByOrder.Value switch
                {
                    FtSearchSortOrder.ASC => ValkeyLiterals.ASC,
                    FtSearchSortOrder.DESC => ValkeyLiterals.DESC,
                    _ => SortByOrder.Value.ToString(),
                });
            }
        }
        // Server ignores WITHSORTKEYS when NOCONTENT is set (sort keys are not
        // returned), so we silently strip it to avoid a Rust-core parse error.
        if (WithSortKeys && !NoContent)
        {
            args.Add(ValkeyLiterals.WITHSORTKEYS);
        }

        if (Timeout.HasValue) { args.Add(ValkeyLiterals.TIMEOUT); args.Add(((long)Timeout.Value.TotalMilliseconds).ToString()); }
        if (Params is { Length: > 0 })
        {
            args.Add(ValkeyLiterals.PARAMS);
            args.Add((Params.Length * 2).ToString());
            foreach (var p in Params) { args.Add(p.Key); args.Add(p.Value); }
        }
        if (Limit is not null) { args.Add(ValkeyLiterals.LIMIT); args.Add(Limit.Offset.ToString()); args.Add(Limit.Count.ToString()); }
        if (Dialect.HasValue) { args.Add(ValkeyLiterals.DIALECT); args.Add(Dialect.Value.ToString()); }
        return [.. args];
    }
}
