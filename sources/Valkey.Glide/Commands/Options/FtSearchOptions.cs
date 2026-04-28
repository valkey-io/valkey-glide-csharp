// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Specifies a field to return in FT.SEARCH results.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchReturnField(string fieldIdentifier, string? alias = null)
{
    /// <summary>
    /// The field name to return.
    /// </summary>
    public string FieldIdentifier { get; init; } = fieldIdentifier;

    /// <summary>
    /// Optional alias to override the field name in the result.
    /// </summary>
    public string? Alias { get; init; } = alias;
}

/// <summary>
/// A key/value pair passed as a query parameter for FT.SEARCH.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchParam(string key, string value)
{
    /// <summary>
    /// The parameter key.
    /// </summary>
    public string Key { get; init; } = key;

    /// <summary>
    /// The parameter value.
    /// </summary>
    public string Value { get; init; } = value;
}

/// <summary>
/// Specifies the SORTBY clause for FT.SEARCH, bundling the field name with
/// optional sort direction and WITHSORTKEYS flag so that invalid combinations
/// (e.g., <c>WithSortKeys</c> without a field) are unrepresentable.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchSortBy(string fieldName, FtSearchSortOrder? order = null, bool withSortKeys = false)
{
    /// <summary>
    /// The field name to sort results by.
    /// </summary>
    public string FieldName { get; init; } = fieldName;

    /// <summary>
    /// Optional sort direction (ASC / DESC).
    /// </summary>
    public FtSearchSortOrder? Order { get; init; } = order;

    /// <summary>
    /// When <see langword="true"/>, augments each result with the sort key value.
    /// </summary>
    public bool WithSortKeys { get; init; } = withSortKeys;
}

/// <summary>
/// Optional arguments for the FT.SEARCH command.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.search/">valkey.io</seealso>
public sealed class FtSearchOptions
{
    /// <summary>
    /// Fields to return. If empty, all fields are returned.
    /// </summary>
    public FtSearchReturnField[]? ReturnFields { get; set; }

    /// <summary>
    /// Overrides the module timeout.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Key/value pairs referenced from within the query expression.
    /// </summary>
    public FtSearchParam[]? Params { get; set; }

    /// <summary>
    /// Pagination. Only keys satisfying offset and count are returned.
    /// </summary>
    public FtLimit? Limit { get; set; }

    /// <summary>
    /// Returns only document IDs without field content.
    /// </summary>
    public bool NoContent { get; set; }

    /// <summary>
    /// Sets the query dialect version.
    /// </summary>
    public int? Dialect { get; set; }

    /// <summary>
    /// Disables stemming on text terms in the query.
    /// </summary>
    public bool Verbatim { get; set; }

    /// <summary>
    /// Requires proximity matching of text terms to be in order.
    /// </summary>
    public bool InOrder { get; set; }

    /// <summary>
    /// Sets the slop value for proximity matching of text terms.
    /// </summary>
    public int? Slop { get; set; }

    /// <summary>
    /// Sort configuration. Bundles the field name, direction, and WITHSORTKEYS flag.
    /// </summary>
    public FtSearchSortBy? SortBy { get; set; }

    /// <summary>
    /// Controls shard participation in cluster mode.
    /// </summary>
    public FtSearchShardScope? ShardScope { get; set; }

    /// <summary>
    /// Controls consistency requirements in cluster mode.
    /// </summary>
    public FtSearchConsistencyMode? Consistency { get; set; }

    /// <summary>
    /// Returns the command arguments for these options.
    /// </summary>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [];
        if (ShardScope.HasValue)
        {
            args.Add(ShardScope.Value.ToLiteral());
        }

        if (Consistency.HasValue)
        {
            args.Add(Consistency.Value.ToLiteral());
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

        if (Slop.HasValue)
        {
            args.Add(ValkeyLiterals.SLOP);
            args.Add(Slop.Value.ToString());
        }

        if (ReturnFields is { Length: > 0 })
        {
            List<GlideString> fieldArgs = [];
            foreach (var rf in ReturnFields)
            {
                fieldArgs.Add(rf.FieldIdentifier);
                if (rf.Alias is not null)
                {
                    fieldArgs.Add(ValkeyLiterals.AS);
                    fieldArgs.Add(rf.Alias);
                }
            }

            args.Add(ValkeyLiterals.RETURN);
            args.Add(fieldArgs.Count.ToString());
            args.AddRange(fieldArgs);
        }

        if (SortBy is not null)
        {
            args.Add(ValkeyLiterals.SORTBY);
            args.Add(SortBy.FieldName);
            if (SortBy.Order.HasValue)
            {
                args.Add(SortBy.Order.Value.ToLiteral());
            }

            if (SortBy.WithSortKeys && !NoContent)
            {
                args.Add(ValkeyLiterals.WITHSORTKEYS);
            }
        }

        if (Timeout.HasValue)
        {
            args.Add(ValkeyLiterals.TIMEOUT);
            args.Add(((long)Timeout.Value.TotalMilliseconds).ToString());
        }

        if (Params is { Length: > 0 })
        {
            args.Add(ValkeyLiterals.PARAMS);
            args.Add((Params.Length * 2).ToString());
            foreach (var p in Params)
            {
                args.Add(p.Key);
                args.Add(p.Value);
            }
        }

        if (Limit is not null)
        {
            args.AddRange(Limit.ToArgs());
        }

        if (Dialect.HasValue)
        {
            args.Add(ValkeyLiterals.DIALECT);
            args.Add(Dialect.Value.ToString());
        }

        return [.. args];
    }
}
