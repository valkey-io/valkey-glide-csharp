// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

public static partial class Ft
{
    #region Nested Interfaces

    /// <summary>
    /// Base interface for all aggregate command clauses.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public interface IAggregateClause
    {
        /// <summary>
        /// Converts to command arguments.
        /// </summary>
        internal GlideString[] ToArgs();
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Runs an aggregate command against an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to aggregate.</param>
    /// <param name="query">The search query expression.</param>
    /// <returns>An array of result row dictionaries</returns>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "index",
    ///     [new Ft.CreateTextField("name"), new Ft.CreateTagField("category")],
    ///     new Ft.CreateOptions { Prefixes = ["item:"] });
    /// await client.HashSetAsync("item:1", [new("name", "Widget"), new("category", "electronics")]);
    /// await client.HashSetAsync("item:2", [new("name", "Gadget"), new("category", "electronics")]);
    ///
    /// var rows = await Ft.AggregateAsync(client, "index", "*");
    ///
    /// Console.WriteLine($"Rows: {rows.Length}");                                                        // 2
    /// Console.WriteLine($"{rows[0]["name"]}: {rows[0]["category"]}");  // "Widget: electronics"
    /// Console.WriteLine($"{rows[1]["name"]}: {rows[1]["category"]}");  // "Gadget: electronics"
    /// </code>
    /// </example>
    public static Task<IDictionary<ValkeyValue, ValkeyValue>[]> AggregateAsync(BaseClient client, ValkeyKey index, ValkeyValue query)
        => client.Command(Request.FtAggregate(index, query));

    /// <summary>
    /// Runs an aggregate command against an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to aggregate.</param>
    /// <param name="query">The search query expression.</param>
    /// <param name="options">Additional options for the aggregation command.</param>
    /// <returns>An array of result row dictionaries</returns>    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "index",
    ///     [new Ft.CreateTextField("name"), new Ft.CreateTagField("category")],
    ///     new Ft.CreateOptions { Prefixes = ["item:"] });
    /// await client.HashSetAsync("item:1", [new("name", "Widget"), new("category", "electronics")]);
    /// await client.HashSetAsync("item:2", [new("name", "Gadget"), new("category", "electronics")]);
    /// await client.HashSetAsync("item:3", [new("name", "Wrench"), new("category", "hardware")]);
    ///
    /// var rows = await Ft.AggregateAsync(client, "index", "*",
    ///     new Ft.AggregateOptions
    ///     {
    ///         Clauses =
    ///         [
    ///             new Ft.AggregateGroupBy
    ///             {
    ///                 Fields = ["@category"],
    ///                 Reducers = [new Ft.AggregateReducer { Function = Ft.ReducerFunction.Count, Name = "count" }],
    ///             },
    ///             new Ft.AggregateSortBy { Expressions = [new Ft.AggregateSortExpression { Expression = "@count", Order = SortOrder.Descending }] },
    ///         ],
    ///     });
    ///
    /// Console.WriteLine($"Rows: {rows.Length}");                                                    // 2
    /// Console.WriteLine($"{rows[0]["category"]}: {rows[0]["count"]}");  // "electronics: 2"
    /// Console.WriteLine($"{rows[1]["category"]}: {rows[1]["count"]}");  // "hardware: 1"
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<IDictionary<ValkeyValue, ValkeyValue>[]> AggregateAsync(BaseClient client, ValkeyKey index, ValkeyValue query, AggregateOptions options)
        => client.Command(Request.FtAggregate(index, query, options));

    #endregion
    #region Nested Classes

    /// <summary>
    /// Limit results for an aggregate command (<c>LIMIT</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public sealed class AggregateLimit : IAggregateClause
    {
        /// <summary>
        /// Number of results to return (<c>count</c>).
        /// </summary>
        public required long Count { get; init; }

        /// <summary>
        /// Number of results to skip (<c>offset</c>).
        /// </summary>
        public long Offset { get; init; } = 0;

        /// <inheritdoc cref="IAggregateClause.ToArgs"/>
        GlideString[] IAggregateClause.ToArgs()
            => [ValkeyLiterals.LIMIT, Offset.ToGlideString(), Count.ToGlideString()];
    }

    /// <summary>
    /// Groups results by fields in an aggregate command (<c>GROUPBY</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public sealed class AggregateGroupBy : IAggregateClause
    {
        /// <summary>
        /// The fields to group by (<c>field</c>).
        /// </summary>
        public required IEnumerable<ValkeyValue> Fields { get; init; }

        /// <summary>
        /// Aggregate reducer functions applied to each group.
        /// </summary>
        public IEnumerable<AggregateReducer> Reducers { get; init; } = [];

        /// <inheritdoc cref="IAggregateClause.ToArgs"/>
        GlideString[] IAggregateClause.ToArgs()
        {
            List<GlideString> args = [ValkeyLiterals.GROUPBY];

            args.Add(Fields.Count().ToGlideString());
            foreach (var field in Fields)
            {
                args.Add(field);
            }

            foreach (var reducer in Reducers)
            {
                args.AddRange(reducer.ToArgs());
            }

            return [.. args];
        }
    }

    /// <summary>
    /// A reducer function for an aggregation group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public sealed class AggregateReducer
    {
        /// <summary>
        /// The reduction function to apply (<c>REDUCE</c>).
        /// </summary>
        public required ReducerFunction Function { get; init; }

        /// <summary>
        /// Arguments for the reducer function (<c>expression</c>).
        /// </summary>
        public IEnumerable<ValkeyValue> Expressions { get; init; } = [];

        /// <summary>
        /// An optional output property name (<c>AS</c>).
        /// </summary>
        public ValkeyValue Name { get; init; }

        /// <summary>
        /// Creates a <c>TOLIST</c> reducer, which collects all values for <paramref name="field"/>
        /// across the records in a group into a single array, removing duplicates and nil values.
        /// If a value is itself an array, it is flattened one level before being added. Values are
        /// returned in the order they were first encountered.
        /// </summary>
        /// <param name="field">The property whose values are collected into a list.</param>
        /// <param name="name">
        /// A user defined property name for the reducer. If not provided, the server generates a default name.
        /// </param>
        public static AggregateReducer ToList(ValkeyValue field, ValkeyValue name = default)
            => new() { Function = ReducerFunction.ToList, Expressions = [field], Name = name };

        /// <summary>
        /// Creates a <c>FIRST_VALUE</c> reducer, which returns the value of <paramref name="field"/>
        /// from a single record in the group. Without <paramref name="by"/>, the first record
        /// encountered wins. With <paramref name="by"/>, the record whose <paramref name="by"/>
        /// property is smallest (<see cref="SortOrder.Ascending"/>, the default) or largest
        /// (<see cref="SortOrder.Descending"/>) wins; ties keep the first-encountered record.
        /// </summary>
        /// <param name="field">The property whose value is returned.</param>
        /// <param name="by">The property used to choose which record's value of <paramref name="field"/> is returned.</param>
        /// <param name="order">The direction used to pick the winning record when <paramref name="by"/> is set.</param>
        /// <param name="name">
        /// A user defined property name for the reducer. If not provided, the server generates a default name.
        /// </param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="order"/> is specified without <paramref name="by"/>.</exception>
        public static AggregateReducer FirstValue(ValkeyValue field, ValkeyValue by = default, SortOrder order = SortOrder.Default, ValkeyValue name = default)
        {
            if (by.IsNull && order != SortOrder.Default)
            {
                throw new ArgumentException($"{nameof(order)} requires {nameof(by)} to be set.");
            }

            List<ValkeyValue> expressions = [field];
            if (!by.IsNull)
            {
                expressions.Add(ValkeyLiterals.BY);
                expressions.Add(by);
                switch (order)
                {
                    case SortOrder.Ascending:
                        expressions.Add(ValkeyLiterals.ASC);
                        break;
                    case SortOrder.Descending:
                        expressions.Add(ValkeyLiterals.DESC);
                        break;
                    case SortOrder.Default:
                    default:
                        break;
                }
            }

            return new() { Function = ReducerFunction.FirstValue, Expressions = expressions, Name = name };
        }

        /// <summary>
        /// Creates a <c>RANDOM_SAMPLE</c> reducer, which returns a uniform random sample of up to
        /// <paramref name="size"/> values of <paramref name="field"/> from the records in a group,
        /// using reservoir sampling.
        /// </summary>
        /// <param name="field">The property to sample values from.</param>
        /// <param name="size">
        /// The maximum number of values to sample. Must be a non-negative integer no greater than
        /// 1000; the server rejects values outside this range.
        /// </param>
        /// <param name="name">
        /// A user defined property name for the reducer. If not provided, the server generates a default name.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="size"/> is negative or greater than 1000.</exception>
        public static AggregateReducer RandomSample(ValkeyValue field, long size, ValkeyValue name = default)
        {
            if (size < 0 || size > 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "RANDOM_SAMPLE's size must be a non-negative integer no greater than 1000; the server rejects values outside this range.");
            }

            return new() { Function = ReducerFunction.RandomSample, Expressions = [field, size.ToGlideString()], Name = name };
        }

        /// <summary>
        /// Converts to command arguments.
        /// </summary>
        internal GlideString[] ToArgs()
        {
            List<GlideString> args = [ValkeyLiterals.REDUCE, ToLiteral(Function)];

            args.Add(Expressions.Count().ToGlideString());
            foreach (var expr in Expressions)
            {
                args.Add(expr);
            }

            if (!Name.IsNull)
            {
                args.Add(ValkeyLiterals.AS);
                args.Add(Name);
            }

            return [.. args];
        }
    }

    /// <summary>
    /// Reducer functions for an aggregate command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public enum ReducerFunction
    {
        /// <summary>
        /// Counts the number of records in each group (<c>COUNT</c>).
        /// </summary>
        Count,

        /// <summary>
        /// Counts the number of distinct values of an expression (<c>COUNT_DISTINCT</c>).
        /// </summary>
        CountDistinct,

        /// <summary>
        /// Sums the values of an expression (<c>SUM</c>).
        /// </summary>
        Sum,

        /// <summary>
        /// Returns the minimum value of an expression (<c>MIN</c>).
        /// </summary>
        Min,

        /// <summary>
        /// Returns the maximum value of an expression (<c>MAX</c>).
        /// </summary>
        Max,

        /// <summary>
        /// Returns the average value of an expression (<c>AVG</c>).
        /// </summary>
        Avg,

        /// <summary>
        /// Returns the standard deviation of the values of an expression (<c>STDDEV</c>).
        /// </summary>
        Stddev,

        /// <summary>
        /// Collects all values into a single array (<c>TOLIST</c>) (valkey-search 1.3.0). Produced
        /// by <see cref="AggregateReducer.ToList"/>; not constructible via the public
        /// <see cref="AggregateReducer"/> object initializer directly, since the factory also sets
        /// <see cref="AggregateReducer.Expressions"/> to the shape TOLIST requires.
        /// </summary>
        ToList,

        /// <summary>
        /// Returns a single record's value, optionally chosen by another property
        /// (<c>FIRST_VALUE</c>) (valkey-search 1.3.0). Produced by
        /// <see cref="AggregateReducer.FirstValue"/>.
        /// </summary>
        FirstValue,

        /// <summary>
        /// Returns a uniform random sample of values (<c>RANDOM_SAMPLE</c>) (valkey-search 1.3.0).
        /// Produced by <see cref="AggregateReducer.RandomSample"/>.
        /// </summary>
        RandomSample,
    }

    /// <summary>
    /// Sort options for an aggregate command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public sealed class AggregateSortBy : IAggregateClause
    {
        /// <summary>
        /// The sort expressions and their directions.
        /// </summary>
        public required IEnumerable<AggregateSortExpression> Expressions { get; init; }

        /// <summary>
        /// Optimizes sorting by only retaining the top <c>n</c> elements (<c>MAX num</c>).
        /// When <see langword="null"/>, the option is omitted.
        /// </summary>
        public long? Max { get; init; }

        /// <inheritdoc cref="IAggregateClause.ToArgs"/>
        GlideString[] IAggregateClause.ToArgs()
        {
            int nargs = Expressions.Count() + Expressions.Count(e => e.Order != SortOrder.Default);
            List<GlideString> args = [ValkeyLiterals.SORTBY, nargs.ToGlideString()];

            foreach (var expr in Expressions)
            {
                args.Add(expr.Expression);
                switch (expr.Order)
                {
                    case SortOrder.Ascending:
                        args.Add(ValkeyLiterals.ASC);
                        break;
                    case SortOrder.Descending:
                        args.Add(ValkeyLiterals.DESC);
                        break;
                    case SortOrder.Default:
                    default:
                        break;
                }
            }

            if (Max.HasValue)
            {
                args.Add(ValkeyLiterals.MAX);
                args.Add(Max.Value.ToGlideString());
            }

            return [.. args];
        }
    }

    /// <summary>
    /// A sort expression for an aggregate command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public sealed class AggregateSortExpression
    {
        /// <summary>
        /// The expression to sort by (<c>expression</c>).
        /// </summary>
        public required ValkeyValue Expression { get; init; }

        /// <summary>
        /// The sort direction (<c>ASC</c>/<c>DESC</c>).
        /// </summary>
        public SortOrder Order { get; init; } = SortOrder.Default;

        /// <summary>
        /// Converts a <see cref="string"/> to an <see cref="AggregateSortExpression"/>.
        /// </summary>
        /// <param name="expression">The sort expression.</param>
        public static implicit operator AggregateSortExpression(string expression)
            => new() { Expression = expression };

        /// <summary>
        /// Converts a <see cref="ValkeyValue"/> to an <see cref="AggregateSortExpression"/>.
        /// </summary>
        /// <param name="expression">The sort expression.</param>
        public static implicit operator AggregateSortExpression(ValkeyValue expression)
            => new() { Expression = expression };
    }

    /// <summary>
    /// A filter expression for an aggregate command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    /// <remarks>
    /// For FT.HYBRID, a <see cref="AggregateFilter"/> placed as the first clause after the VSIM arm
    /// is claimed by the server's VSIM parser as a VSIM pre-filter instead of an aggregate FILTER
    /// clause -- use <see cref="Ft.HybridOptions.Clauses"/> only after at least one other clause,
    /// or see <see cref="Ft.HybridVsimFilter"/> for an explicit VSIM pre-filter.
    /// </remarks>
    /// <param name="expression">The filter expression.</param>
    [method: SetsRequiredMembers]
    public sealed class AggregateFilter(ValkeyValue expression) : IAggregateClause
    {
        /// <summary>
        /// The filter expression (<c>expression</c>).
        /// </summary>
        public required ValkeyValue Expression { get; init; } = expression;

        /// <inheritdoc cref="IAggregateClause.ToArgs"/>
        GlideString[] IAggregateClause.ToArgs()
            => [ValkeyLiterals.FILTER, Expression];
    }

    /// <summary>
    /// A transformation expression for an aggregate command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public sealed class AggregateApply : IAggregateClause
    {
        /// <summary>
        /// The transformation expression (<c>expression</c>).
        /// </summary>
        public required ValkeyValue Expression { get; init; }

        /// <summary>
        /// The output field name (<c>AS field</c>).
        /// </summary>
        public required ValkeyValue Name { get; init; }

        /// <inheritdoc cref="IAggregateClause.ToArgs"/>
        GlideString[] IAggregateClause.ToArgs()
            => [ValkeyLiterals.APPLY, Expression, ValkeyLiterals.AS, Name];
    }

    /// <summary>
    /// A single entry in the <c>LOAD</c> clause of FT.AGGREGATE (or FT.HYBRID), optionally renaming
    /// the loaded field via <c>AS</c>. Modelled on <see cref="SearchReturnField"/> (the equivalent
    /// concept for FT.SEARCH's <c>RETURN</c> option).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    /// <remarks>
    /// <c>AS alias</c> in the <c>LOAD</c> clause is a compatibility fix introduced in valkey-search 1.3.0,
    /// gated by the <c>ft_aggregate_load_as</c> compatibility flag. Below
    /// <c>search.emulate-release 1.3.0</c>, the legacy branch does not recognize <c>AS</c> as a rename
    /// keyword at all: it is consumed as just another LOAD field token (i.e. treated as the
    /// identifier of a field literally named <c>AS</c>) rather than a rename directive. Since no
    /// schema ever has a field actually named <c>AS</c>, this mechanism is what causes the server
    /// to reject the whole command outright ("field AS does not exist") rather than silently
    /// ignoring the alias. Requires <c>search.emulate-release</c> &gt;= 1.3.0 to take effect.
    /// </remarks>
    public sealed class LoadField
    {
        /// <summary>
        /// The identifier for the field to load.
        /// </summary>
        public required ValkeyValue FieldIdentifier { get; init; }

        /// <summary>
        /// The alias to emit the field's column under. Requires <c>search.emulate-release</c> &gt;= 1.3.0 to take effect.
        /// </summary>
        public ValkeyValue Alias { get; init; }

        /// <summary>
        /// Creates a new <see cref="LoadField"/> with the specified field identifier and optional alias.
        /// </summary>
        /// <param name="fieldIdentifier">The identifier for the field to load.</param>
        /// <param name="alias">An optional alias to emit the field's column under.</param>
        [SetsRequiredMembers]
        public LoadField(ValkeyValue fieldIdentifier, ValkeyValue alias = default)
        {
            FieldIdentifier = fieldIdentifier;
            Alias = alias;
        }

        /// <summary>
        /// Converts a <see cref="string"/> to a <see cref="LoadField"/>.
        /// </summary>
        /// <param name="fieldIdentifier">The field identifier.</param>
        public static implicit operator LoadField(string fieldIdentifier)
            => new(fieldIdentifier);

        /// <summary>
        /// Converts a <see cref="ValkeyValue"/> to a <see cref="LoadField"/>.
        /// </summary>
        /// <param name="fieldIdentifier">The field identifier.</param>
        public static implicit operator LoadField(ValkeyValue fieldIdentifier)
            => new(fieldIdentifier);

        /// <summary>
        /// Converts to command arguments for this LOAD clause entry.
        /// </summary>
        internal GlideString[] ToArgs()
            => Alias.IsNull ? [FieldIdentifier] : [FieldIdentifier, ValkeyLiterals.AS, Alias];
    }

    /// <summary>
    /// The <c>WITHCURSOR</c> option, requesting a paged reply instead of the whole result set at once.
    /// Shared between FT.AGGREGATE and FT.HYBRID -- the option's grammar (<c>WITHCURSOR [COUNT n]
    /// [MAXIDLE ms]</c>) and reply shape are identical regardless of which command opened the cursor.
    /// A paged query, and every subsequent <see cref="CursorReadAsync"/>/<see cref="CursorDelAsync"/>
    /// against the cursor it opens, is pinned to the PRIMARY of the index's slot regardless of the
    /// client's read-from configuration -- a cursor is a per-process, node-local resource, and this
    /// is a deliberate, documented exception to GLIDE's normal read routing.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.cursor/">Valkey commands – FT.CURSOR</seealso>
    public sealed class WithCursor
    {
        /// <summary>
        /// The number of rows to return per page, or <see langword="null"/> for server default.
        /// </summary>
        public long? Count { get; init; }

        /// <summary>
        /// How long (in milliseconds) the cursor may sit idle between reads before the server
        /// discards it, or <see langword="null"/> for server default.
        /// </summary>
        public long? MaxIdle { get; init; }

        /// <summary>
        /// Converts to command arguments for the WITHCURSOR option.
        /// </summary>
        internal GlideString[] ToArgs()
        {
            List<GlideString> args = [ValkeyLiterals.WITHCURSOR];
            if (Count.HasValue)
            {
                args.Add(ValkeyLiterals.COUNT);
                args.Add(Count.Value.ToGlideString());
            }

            if (MaxIdle.HasValue)
            {
                args.Add(ValkeyLiterals.MAXIDLE);
                args.Add(MaxIdle.Value.ToGlideString());
            }

            return [.. args];
        }
    }

    /// <summary>
    /// Options for an aggregate command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public sealed class AggregateOptions
    {
        #region Constants

        /// <summary>
        /// Indicates that no fields should be loaded (<c>LOAD 0</c>).
        /// </summary>
        public static readonly IEnumerable<ValkeyValue> LoadNone = [];

        /// <summary>
        /// Indicates that all fields should be loaded (<c>LOAD *</c>).
        /// </summary>
        public static readonly IEnumerable<ValkeyValue>? LoadAll = null;

        #endregion
        #region Public Properties

        /// <summary>
        /// Requires proximity matching of text terms to be in order (<c>INORDER</c>).
        /// </summary>
        public bool InOrder { get; init; }

        /// <summary>
        /// Fields to load from the document hash.
        /// </summary>
        public IEnumerable<ValkeyValue>? LoadFields { get; init; } = LoadNone;

        /// <summary>
        /// Fields to load from the document hash, optionally renamed via <see cref="LoadField.Alias"/>
        /// (valkey-search 1.3.0). Takes priority over <see cref="LoadFields"/> when set.
        /// </summary>
        public IEnumerable<LoadField>? LoadFieldEntries { get; init; }

        /// <summary>
        /// Key/value pairs referenced from within the query expression (<c>PARAMS</c>).
        /// </summary>
        public IDictionary<ValkeyValue, ValkeyValue> Params { get; init; }
            = new Dictionary<ValkeyValue, ValkeyValue>();

        /// <summary>
        /// Sets the slop value for proximity matching of text terms (<c>SLOP</c>).
        /// </summary>
        public long? Slop { get; init; }

        /// <summary>
        /// Timeout for the aggregate operation,
        /// or <see langword="null"/> to use the server default.
        /// </summary>
        public TimeSpan? Timeout { get; init; }

        /// <summary>
        /// Disables stemming on text terms in the query (<c>VERBATIM</c>).
        /// </summary>
        public bool Verbatim { get; init; }

        /// <summary>
        /// The aggregate clauses to apply.
        /// </summary>
        public IEnumerable<IAggregateClause> Clauses { get; init; } = [];

        /// <summary>
        /// The query dialect version to use, or <see langword="null"/> for server default.
        /// The only supported dialect is <c>2</c>.
        /// </summary>
        public int? Dialect { get; init; }

        /// <summary>
        /// Whether to expose the relevance score as a pipeline field named <c>__score</c>
        /// (<c>ADDSCORES</c>) (valkey-search 1.3.0). <c>__score</c> can then be referenced like any
        /// other pipeline field in a <see cref="LoadField"/>, <see cref="AggregateApply"/> expression,
        /// or an <see cref="AggregateSortExpression"/>.
        /// </summary>
        public bool AddScores { get; init; }

        /// <summary>
        /// The scoring function used to compute <c>__score</c> and rank results, or
        /// <see langword="null"/> for server default (valkey-search 1.3.0). <c>"BM25STD"</c> is the
        /// only value currently registered and selectable; any other value is rejected by the server.
        /// Has no visible effect unless combined with <see cref="AddScores"/>.
        /// </summary>
        public ValkeyValue Scorer { get; init; }

        /// <summary>
        /// Requests a paged reply instead of the whole result set at once. See <see cref="WithCursor"/>.
        /// A paged query, and every subsequent FT.CURSOR READ/DEL against the cursor it opens, always
        /// reads from the PRIMARY of the index's slot, regardless of the client's read-from
        /// configuration.
        /// </summary>
        public WithCursor? WithCursorOptions { get; init; }

        #endregion
    }

    #endregion
    #region Private Methods

    private static GlideString ToLiteral(ReducerFunction function)
        => function switch
        {
            ReducerFunction.Count => ValkeyLiterals.COUNT,
            ReducerFunction.CountDistinct => ValkeyLiterals.COUNT_DISTINCT,
            ReducerFunction.Sum => ValkeyLiterals.SUM,
            ReducerFunction.Min => ValkeyLiterals.MIN,
            ReducerFunction.Max => ValkeyLiterals.MAX,
            ReducerFunction.Avg => ValkeyLiterals.AVG,
            ReducerFunction.Stddev => ValkeyLiterals.STDDEV,
            ReducerFunction.ToList => ValkeyLiterals.TOLIST,
            ReducerFunction.FirstValue => ValkeyLiterals.FIRST_VALUE,
            ReducerFunction.RandomSample => ValkeyLiterals.RANDOM_SAMPLE,
            _ => throw new ArgumentOutOfRangeException(nameof(function)),
        };

    #endregion
}
