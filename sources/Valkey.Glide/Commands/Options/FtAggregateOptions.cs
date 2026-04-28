// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Base interface for all FT.AGGREGATE pipeline clauses.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public interface IFtAggregateClause
{
    internal GlideString[] ToArgs();
}

/// <summary>
/// Filters results using a predicate expression applied post-query.
/// </summary>
public sealed class FtAggregateFilter(string expression) : IFtAggregateClause
{
    /// <summary>
    /// The filter expression.
    /// </summary>
    public string Expression { get; } = expression;

    GlideString[] IFtAggregateClause.ToArgs() => [ValkeyLiterals.FILTER, Expression];
}

/// <summary>
/// A reducer function applied within a GROUPBY clause.
/// </summary>
public sealed class FtAggregateReducer(FtReducerFunction function)
{
    /// <summary>
    /// The reduction function.
    /// </summary>
    public FtReducerFunction Function { get; } = function;

    /// <summary>
    /// Arguments for the reducer function.
    /// </summary>
    public string[] Args { get; init; } = [];

    /// <summary>
    /// Optional user-defined output property name.
    /// </summary>
    public string? Name { get; init; }

    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [ValkeyLiterals.REDUCE, Function.ToLiteral(), Args.Length.ToString()];
        foreach (var a in Args)
        {
            args.Add(a);
        }

        if (Name is not null)
        {
            args.Add(ValkeyLiterals.AS);
            args.Add(Name);
        }

        return [.. args];
    }
}

/// <summary>
/// Groups pipeline results by one or more properties.
/// </summary>
public sealed class FtAggregateGroupBy(params string[] properties) : IFtAggregateClause
{
    /// <summary>
    /// Fields to group by (e.g. "@condition").
    /// </summary>
    public string[] Properties { get; } = properties;

    /// <summary>
    /// Aggregate functions applied to each group.
    /// </summary>
    public FtAggregateReducer[] Reducers { get; init; } = [];

    GlideString[] IFtAggregateClause.ToArgs()
    {
        List<GlideString> args = [ValkeyLiterals.GROUPBY, Properties.Length.ToString()];
        foreach (var p in Properties)
        {
            args.Add(p);
        }

        foreach (var r in Reducers)
        {
            args.AddRange(r.ToArgs());
        }

        return [.. args];
    }
}

/// <summary>
/// A single sort property with optional direction for <see cref="FtAggregateSortBy"/>.
/// </summary>
public sealed class FtAggregateSortProperty(string property, SortOrder order = SortOrder.Default)
{
    /// <summary>
    /// The property name.
    /// </summary>
    public string Property { get; } = property;

    /// <summary>
    /// The sort direction.
    /// <see cref="SortOrder.Default"/> omits the direction and lets the server use its default (ascending).
    /// </summary>
    public SortOrder Order { get; } = order;
}

/// <summary>
/// Sorts the pipeline by a list of properties.
/// </summary>
public sealed class FtAggregateSortBy(FtAggregateSortProperty[] properties, int? max = null) : IFtAggregateClause
{
    /// <summary>
    /// Fields and their sort directions.
    /// </summary>
    public FtAggregateSortProperty[] Properties { get; } = properties;

    /// <summary>
    /// Optimizes sorting by only sorting the n-largest elements.
    /// </summary>
    public int? Max { get; } = max;

    GlideString[] IFtAggregateClause.ToArgs()
    {
        // Count is the total number of arguments: 1 per property name + 1 per explicit direction.
        int count = Properties.Length + Properties.Count(p => p.Order != SortOrder.Default);
        List<GlideString> args = [ValkeyLiterals.SORTBY, count.ToString()];
        foreach (var p in Properties)
        {
            args.Add(p.Property);
            if (p.Order != SortOrder.Default)
            {
                args.Add(p.Order.ToOrder().ToLiteral());
            }
        }

        if (Max.HasValue)
        {
            args.Add(ValkeyLiterals.MAX);
            args.Add(Max.Value.ToString());
        }

        return [.. args];
    }
}

/// <summary>
/// Applies a 1-to-1 transformation on properties and stores the result as a new property.
/// </summary>
public sealed class FtAggregateApply(string expression, string name) : IFtAggregateClause
{
    /// <summary>
    /// The transformation expression.
    /// </summary>
    public string Expression { get; } = expression;

    /// <summary>
    /// The output property name.
    /// </summary>
    public string Name { get; } = name;

    GlideString[] IFtAggregateClause.ToArgs() => [ValkeyLiterals.APPLY, Expression, ValkeyLiterals.AS, Name];
}

/// <summary>
/// A key/value pair passed as a query parameter for FT.AGGREGATE.
/// </summary>
public sealed class FtAggregateParam(string key, string value)
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
/// Fluent builder for <see cref="FtAggregateOptions"/>.
/// Covers both scalar options and the ordered pipeline of clauses in a single chain.
/// </summary>
/// <remarks>
/// The object-initializer style and the builder style are interchangeable — both
/// produce an identical <see cref="FtAggregateOptions"/> instance.
/// <code>
/// // Builder style
/// FtAggregateOptions options = new FtAggregateOptionsBuilder()
///     .WithField("@f1").WithField("@f2")
///     .Verbatim()
///     .Timeout(TimeSpan.FromSeconds(3))
///     .Dialect(2)
///     .GroupBy(["@condition"], new FtAggregateReducer(FtReducerFunction.Count) { Name = "count" })
///     .SortBy([new FtAggregateSortProperty("@score", SortOrder.Descending)], max: 10)
///     .Filter("@score > 5")
///     .Limit(0, 10)
///     .Build();
///
/// // Object-initializer style (equivalent)
/// FtAggregateOptions options = new FtAggregateOptions
/// {
///     LoadFields = ["@f1", "@f2"],
///     Verbatim = true,
///     Timeout = TimeSpan.FromSeconds(3),
///     Dialect = 2,
///     Clauses =
///     [
///         new FtAggregateGroupBy("@condition") { Reducers = [new FtAggregateReducer(FtReducerFunction.Count) { Name = "count" }] },
///         new FtAggregateSortBy([new FtAggregateSortProperty("@score", SortOrder.Descending)], 10),
///         new FtAggregateFilter("@score > 5"),
///         new FtLimit(0, 10),
///     ]
/// };
/// </code>
/// </remarks>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public sealed class FtAggregateOptionsBuilder
{
    private bool _loadAll;
    private readonly List<string> _loadFields = [];
    private TimeSpan? _timeout;
    private readonly List<FtAggregateParam> _params = [];
    private bool _verbatim;
    private bool _inOrder;
    private int? _slop;
    private int? _dialect;
    private readonly List<IFtAggregateClause> _clauses = [];

    // ── Field loading ─────────────────────────────────────────────────────────

    /// <summary>
    /// Sets LOAD * — loads all fields declared in the index.
    /// Clears any fields previously added via <see cref="WithField"/> or <see cref="WithFields"/>.
    /// </summary>
    public FtAggregateOptionsBuilder WithAllFields()
    {
        _loadAll = true;
        _loadFields.Clear();
        return this;
    }

    /// <summary>
    /// Adds a single field to the LOAD list.
    /// Clears the LOAD * flag if <see cref="WithAllFields"/> was previously called.
    /// </summary>
    public FtAggregateOptionsBuilder WithField(string field)
    {
        _loadAll = false;
        _loadFields.Add(field);
        return this;
    }

    /// <summary>
    /// Replaces the LOAD field list with <paramref name="fields"/>.
    /// Clears the LOAD * flag if <see cref="WithAllFields"/> was previously called.
    /// </summary>
    public FtAggregateOptionsBuilder WithFields(IEnumerable<string> fields)
    {
        _loadAll = false;
        _loadFields.Clear();
        _loadFields.AddRange(fields);
        return this;
    }

    // ── Scalar options ────────────────────────────────────────────────────────

    /// <summary>
    /// Sets the module timeout.
    /// </summary>
    public FtAggregateOptionsBuilder Timeout(TimeSpan timeout)
    {
        _timeout = timeout;
        return this;
    }

    /// <summary>
    /// Adds a single PARAMS key/value pair.
    /// </summary>
    public FtAggregateOptionsBuilder WithParam(FtAggregateParam param)
    {
        _params.Add(param);
        return this;
    }

    /// <summary>
    /// Replaces the PARAMS list with <paramref name="parameters"/>.
    /// </summary>
    public FtAggregateOptionsBuilder WithParams(IEnumerable<FtAggregateParam> parameters)
    {
        _params.Clear();
        _params.AddRange(parameters);
        return this;
    }

    /// <summary>
    /// Enables VERBATIM — disables stemming on term searches.
    /// </summary>
    public FtAggregateOptionsBuilder Verbatim()
    {
        _verbatim = true;
        return this;
    }

    /// <summary>
    /// Enables INORDER — requires proximity matching of terms to be in order.
    /// </summary>
    public FtAggregateOptionsBuilder InOrder()
    {
        _inOrder = true;
        return this;
    }

    /// <summary>
    /// Sets the SLOP value for proximity matching.
    /// </summary>
    public FtAggregateOptionsBuilder Slop(int slop)
    {
        _slop = slop;
        return this;
    }

    /// <summary>
    /// Sets the query DIALECT version.
    /// </summary>
    public FtAggregateOptionsBuilder Dialect(int dialect)
    {
        _dialect = dialect;
        return this;
    }

    // ── Pipeline clauses ──────────────────────────────────────────────────────

    /// <summary>
    /// Appends a single pipeline clause.
    /// </summary>
    public FtAggregateOptionsBuilder WithClause(IFtAggregateClause clause)
    {
        _clauses.Add(clause);
        return this;
    }

    /// <summary>
    /// Replaces the pipeline clause list with <paramref name="clauses"/>.
    /// </summary>
    public FtAggregateOptionsBuilder WithClauses(IEnumerable<IFtAggregateClause> clauses)
    {
        _clauses.Clear();
        _clauses.AddRange(clauses);
        return this;
    }

    /// <summary>
    /// Appends a GROUPBY clause with the given properties and reducers.
    /// </summary>
    public FtAggregateOptionsBuilder GroupBy(IEnumerable<string> properties, params FtAggregateReducer[] reducers)
    {
        _clauses.Add(new FtAggregateGroupBy([.. properties]) { Reducers = reducers });
        return this;
    }

    /// <summary>
    /// Appends a SORTBY clause.
    /// </summary>
    /// <param name="properties">Fields and their sort directions.</param>
    /// <param name="max">When set, optimizes sorting by only retaining the top <paramref name="max"/> elements.</param>
    public FtAggregateOptionsBuilder SortBy(IEnumerable<FtAggregateSortProperty> properties, int? max = null)
    {
        _clauses.Add(new FtAggregateSortBy([.. properties], max));
        return this;
    }

    /// <summary>
    /// Appends a FILTER clause.
    /// </summary>
    public FtAggregateOptionsBuilder Filter(string expression)
    {
        _clauses.Add(new FtAggregateFilter(expression));
        return this;
    }

    /// <summary>
    /// Appends an APPLY clause.
    /// </summary>
    public FtAggregateOptionsBuilder Apply(string expression, string name)
    {
        _clauses.Add(new FtAggregateApply(expression, name));
        return this;
    }

    /// <summary>
    /// Appends a LIMIT clause.
    /// </summary>
    public FtAggregateOptionsBuilder Limit(long offset, long count)
    {
        _clauses.Add(new FtLimit(offset, count));
        return this;
    }

    /// <summary>
    /// Executes <paramref name="configure"/> only when <paramref name="condition"/> is
    /// <see langword="true"/>. Useful for conditionally including steps without breaking the chain.
    /// </summary>
    public FtAggregateOptionsBuilder When(bool condition, Func<FtAggregateOptionsBuilder, FtAggregateOptionsBuilder> configure)
        => condition ? configure(this) : this;

    // ── Build ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Constructs the <see cref="FtAggregateOptions"/> from the accumulated state.
    /// </summary>
    public FtAggregateOptions Build() => new()
    {
        LoadAll = _loadAll,
        LoadFields = _loadFields.Count > 0 ? [.. _loadFields] : null,
        Timeout = _timeout,
        Params = _params.Count > 0 ? [.. _params] : null,
        Clauses = _clauses.Count > 0 ? [.. _clauses] : null,
        Verbatim = _verbatim,
        InOrder = _inOrder,
        Slop = _slop,
        Dialect = _dialect,
    };
}

/// <summary>
/// Optional arguments for the FT.AGGREGATE command.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public sealed class FtAggregateOptions
{
    /// <summary>
    /// Loads all fields declared in the index. Mutually exclusive with <see cref="LoadFields"/>.
    /// </summary>
    public bool LoadAll { get; init; }

    /// <summary>
    /// Loads only the specified fields. Mutually exclusive with <see cref="LoadAll"/>.
    /// </summary>
    public IEnumerable<string>? LoadFields { get; init; }

    /// <summary>
    /// Overrides the module timeout.
    /// </summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>
    /// Key/value pairs referenced from within the query expression.
    /// </summary>
    public IEnumerable<FtAggregateParam>? Params { get; init; }

    /// <summary>
    /// Pipeline clauses (FILTER, LIMIT, GROUPBY, SORTBY, APPLY) applied in order.
    /// </summary>
    public IEnumerable<IFtAggregateClause>? Clauses { get; init; }

    /// <summary>
    /// Disables stemming on term searches.
    /// </summary>
    public bool Verbatim { get; init; }

    /// <summary>
    /// Requires proximity matching of terms to be in order.
    /// </summary>
    public bool InOrder { get; init; }

    /// <summary>
    /// Sets the slop value for proximity matching.
    /// </summary>
    public int? Slop { get; init; }

    /// <summary>
    /// Sets the query dialect version.
    /// </summary>
    public int? Dialect { get; init; }

    /// <summary>
    /// Returns the command arguments for these options.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        string[] loadFields = LoadFields?.ToArray() ?? [];

        if (LoadAll && loadFields.Length > 0)
        {
            throw new ArgumentException("LoadAll and LoadFields are mutually exclusive.");
        }

        List<GlideString> args = [];

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

        if (LoadAll)
        {
            args.Add(ValkeyLiterals.LOAD);
            args.Add("*");
        }
        else if (loadFields.Length > 0)
        {
            args.Add(ValkeyLiterals.LOAD);
            args.Add(loadFields.Length.ToString());
            foreach (var f in loadFields)
            {
                args.Add(f);
            }
        }

        if (Timeout.HasValue)
        {
            args.Add(ValkeyLiterals.TIMEOUT);
            args.Add(((long)Timeout.Value.TotalMilliseconds).ToString());
        }

        FtAggregateParam[] parameters = Params?.ToArray() ?? [];
        if (parameters.Length > 0)
        {
            args.Add(ValkeyLiterals.PARAMS);
            args.Add((parameters.Length * 2).ToString());
            foreach (var p in parameters)
            {
                args.Add(p.Key);
                args.Add(p.Value);
            }
        }

        if (Clauses is not null)
        {
            foreach (var clause in Clauses)
            {
                args.AddRange(clause.ToArgs());
            }
        }

        if (Dialect.HasValue)
        {
            args.Add(ValkeyLiterals.DIALECT);
            args.Add(Dialect.Value.ToString());
        }

        return [.. args];
    }
}
