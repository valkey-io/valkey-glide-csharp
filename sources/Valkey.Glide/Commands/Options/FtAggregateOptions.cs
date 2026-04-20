// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Sort order for FT.AGGREGATE SORTBY clause.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public enum FtAggregateOrderBy
{
    /// <summary>Ascending order.</summary>
    ASC,
    /// <summary>Descending order.</summary>
    DESC,
}

/// <summary>
/// Base interface for all FT.AGGREGATE pipeline clauses.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public interface IFtAggregateClause
{
    /// <summary>Returns the command arguments for this clause.</summary>
    GlideString[] ToArgs();
}

/// <summary>
/// Limits the number of retained records in the pipeline.
/// </summary>
public class FtAggregateLimit(int offset, int count) : IFtAggregateClause
{
    /// <summary>Number of results to skip.</summary>
    public int Offset { get; } = offset;
    /// <summary>Number of results to return.</summary>
    public int Count { get; } = count;

    /// <inheritdoc/>
    public GlideString[] ToArgs() => [ValkeyLiterals.LIMIT, Offset.ToString(), Count.ToString()];
}

/// <summary>
/// Filters results using a predicate expression applied post-query.
/// </summary>
public class FtAggregateFilter(string expression) : IFtAggregateClause
{
    /// <summary>The filter expression.</summary>
    public string Expression { get; } = expression;

    /// <inheritdoc/>
    public GlideString[] ToArgs() => [ValkeyLiterals.FILTER, Expression];
}

/// <summary>
/// A reducer function applied within a GROUPBY clause.
/// </summary>
public class FtAggregateReducer(string function)
{
    /// <summary>The reduction function name (e.g. "COUNT", "SUM", "TOLIST").</summary>
    public string Function { get; } = function;
    /// <summary>Arguments for the reducer function.</summary>
    public string[] Args { get; set; } = [];
    /// <summary>Optional user-defined output property name.</summary>
    public string? Name { get; set; }

    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [ValkeyLiterals.REDUCE, Function, Args.Length.ToString()];
        foreach (var a in Args) args.Add(a);
        if (Name is not null) { args.Add(ValkeyLiterals.AS); args.Add(Name); }
        return [.. args];
    }
}

/// <summary>
/// Groups pipeline results by one or more properties.
/// </summary>
public class FtAggregateGroupBy(params string[] properties) : IFtAggregateClause
{
    /// <summary>Fields to group by (e.g. "@condition").</summary>
    public string[] Properties { get; } = properties;
    /// <summary>Aggregate functions applied to each group.</summary>
    public FtAggregateReducer[] Reducers { get; set; } = [];

    /// <inheritdoc/>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [ValkeyLiterals.GROUPBY, Properties.Length.ToString()];
        foreach (var p in Properties) args.Add(p);
        foreach (var r in Reducers) args.AddRange(r.ToArgs());
        return [.. args];
    }
}

/// <summary>
/// A single sort property with direction for <see cref="FtAggregateSortBy"/>.
/// </summary>
public class FtAggregateSortProperty(string property, FtAggregateOrderBy order)
{
    /// <summary>The property name.</summary>
    public string Property { get; } = property;
    /// <summary>The sort direction.</summary>
    public FtAggregateOrderBy Order { get; } = order;
}

/// <summary>
/// Sorts the pipeline by a list of properties.
/// </summary>
public class FtAggregateSortBy(params FtAggregateSortProperty[] properties) : IFtAggregateClause
{
    /// <summary>Fields and their sort directions.</summary>
    public FtAggregateSortProperty[] Properties { get; } = properties;
    /// <summary>Optimizes sorting by only sorting the n-largest elements.</summary>
    public int? Max { get; set; }

    /// <inheritdoc/>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [ValkeyLiterals.SORTBY, (Properties.Length * 2).ToString()];
        foreach (var p in Properties)
        {
            args.Add(p.Property);
            args.Add(p.Order switch
            {
                FtAggregateOrderBy.ASC => ValkeyLiterals.ASC,
                FtAggregateOrderBy.DESC => ValkeyLiterals.DESC,
                _ => p.Order.ToString(),
            });
        }
        if (Max.HasValue) { args.Add(ValkeyLiterals.MAX); args.Add(Max.Value.ToString()); }
        return [.. args];
    }
}

/// <summary>
/// Applies a 1-to-1 transformation on properties and stores the result as a new property.
/// </summary>
public class FtAggregateApply(string expression, string name) : IFtAggregateClause
{
    /// <summary>The transformation expression.</summary>
    public string Expression { get; } = expression;
    /// <summary>The output property name.</summary>
    public string Name { get; } = name;

    /// <inheritdoc/>
    public GlideString[] ToArgs() => [ValkeyLiterals.APPLY, Expression, ValkeyLiterals.AS, Name];
}

/// <summary>
/// A key/value pair passed as a query parameter for FT.AGGREGATE.
/// </summary>
public class FtAggregateParam(string key, string value)
{
    /// <summary>The parameter key.</summary>
    public string Key { get; } = key;
    /// <summary>The parameter value.</summary>
    public string Value { get; } = value;
}

/// <summary>
/// Optional arguments for the FT.AGGREGATE command.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public class FtAggregateOptions
{
    /// <summary>Loads all fields declared in the index. Mutually exclusive with <see cref="LoadFields"/>.</summary>
    public bool LoadAll { get; set; }
    /// <summary>Loads only the specified fields. Mutually exclusive with <see cref="LoadAll"/>.</summary>
    public string[]? LoadFields { get; set; }
    /// <summary>Overrides the module timeout in milliseconds.</summary>
    public int? Timeout { get; set; }
    /// <summary>Key/value pairs referenced from within the query expression.</summary>
    public FtAggregateParam[]? Params { get; set; }
    /// <summary>Pipeline clauses (FILTER, LIMIT, GROUPBY, SORTBY, APPLY) applied in order.</summary>
    public IFtAggregateClause[]? Clauses { get; set; }
    /// <summary>Disables stemming on term searches.</summary>
    public bool Verbatim { get; set; }
    /// <summary>Requires proximity matching of terms to be in order.</summary>
    public bool InOrder { get; set; }
    /// <summary>Sets the slop value for proximity matching.</summary>
    public int? Slop { get; set; }
    /// <summary>Sets the query dialect version.</summary>
    public int? Dialect { get; set; }

    /// <summary>
    /// Returns the command arguments for these options.
    /// </summary>
    public GlideString[] ToArgs()
    {
        if (LoadAll && LoadFields is { Length: > 0 })
            throw new ArgumentException("LoadAll and LoadFields are mutually exclusive.");

        List<GlideString> args = [];
        if (Verbatim) args.Add(ValkeyLiterals.VERBATIM);
        if (InOrder) args.Add(ValkeyLiterals.INORDER);
        if (Slop.HasValue) { args.Add(ValkeyLiterals.SLOP); args.Add(Slop.Value.ToString()); }
        if (LoadAll) { args.Add(ValkeyLiterals.LOAD); args.Add("*"); }
        else if (LoadFields is { Length: > 0 }) { args.Add(ValkeyLiterals.LOAD); args.Add(LoadFields.Length.ToString()); foreach (var f in LoadFields) args.Add(f); }
        if (Timeout.HasValue) { args.Add(ValkeyLiterals.TIMEOUT); args.Add(Timeout.Value.ToString()); }
        if (Params is { Length: > 0 })
        {
            args.Add(ValkeyLiterals.PARAMS);
            args.Add((Params.Length * 2).ToString());
            foreach (var p in Params) { args.Add(p.Key); args.Add(p.Value); }
        }
        if (Clauses is not null)
        {
            foreach (var clause in Clauses) args.AddRange(clause.ToArgs());
        }
        if (Dialect.HasValue) { args.Add(ValkeyLiterals.DIALECT); args.Add(Dialect.Value.ToString()); }
        return [.. args];
    }
}
