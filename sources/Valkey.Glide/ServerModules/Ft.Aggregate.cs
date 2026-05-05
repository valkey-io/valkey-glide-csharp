// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

public static partial class Ft
{
    #region Public Methods

    /// <summary>
    /// Runs an aggregate command against an index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to aggregate.</param>
    /// <param name="query">The search query expression.</param>
    /// <returns>An array of result row dictionaries</returns>
    /// <remarks>
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
    /// </remarks>
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

    #region Nested Types

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
    /// Options for an aggregate command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.aggregate/">Valkey commands – FT.AGGREGATE</seealso>
    public sealed class AggregateOptions
    {
        #region Constants

        /// <summary>
        /// Indicates that that no fields should be loaded (<c>LOAD 0</c>).
        /// </summary>
        public static readonly IEnumerable<ValkeyValue> LoadNone = [];

        /// <summary>
        /// Indicates that that all fields should be loaded (<c>LOAD *</c>).
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
            _ => throw new ArgumentOutOfRangeException(nameof(function)),
        };

    #endregion
}
