// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

public static partial class Ft
{
    #region Nested Enums

    /// <summary>
    /// Controls how a VSIM pre-filter (see <see cref="HybridVsimFilter"/>) is applied against the
    /// vector search. Only meaningful inside a counted VSIM FILTER block.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    public enum HybridFilterPolicy
    {
        /// <summary>
        /// Evaluates the filter against every candidate as it is produced (brute-force), rather
        /// than pre-restricting the vector index's own search space. <c>EF_RUNTIME</c> on the same
        /// VSIM arm is rejected when this policy is set.
        /// </summary>
        Adhoc,

        /// <summary>
        /// Evaluates the filter in batches drawn from the vector index. Required for
        /// <see cref="HybridVsimFilter.BatchSize"/> to be set.
        /// </summary>
        Batches,
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Combines a text/tag/numeric query (the SEARCH arm) with a vector similarity search
    /// (the VSIM arm) in a single query, fusing both arms' results into one ranked reply.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to query.</param>
    /// <param name="searchArm">The text/tag/numeric query arm. See <see cref="HybridSearchArm"/>.</param>
    /// <param name="vsimArm">The vector similarity search arm. See <see cref="HybridVsimArm"/>.</param>
    /// <returns>
    /// An array of result row dictionaries, after both arms' results have been fused. With no LOAD
    /// clause (the default), each row contains just the document key and the fused score.
    /// </returns>
    public static Task<IDictionary<ValkeyValue, ValkeyValue>[]> HybridAsync(BaseClient client, ValkeyKey index, HybridSearchArm searchArm, HybridVsimArm vsimArm)
        => client.Command(Request.FtHybrid(index, searchArm, vsimArm));

    /// <summary>
    /// Combines a text/tag/numeric query (the SEARCH arm) with a vector similarity search
    /// (the VSIM arm) in a single query, fusing both arms' results into one ranked reply.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to query.</param>
    /// <param name="searchArm">The text/tag/numeric query arm. See <see cref="HybridSearchArm"/>.</param>
    /// <param name="vsimArm">The vector similarity search arm. See <see cref="HybridVsimArm"/>.</param>
    /// <param name="options">
    /// Options covering COMBINE, LOAD, FILTER/LIMIT/GROUPBY/SORTBY/APPLY clauses, LOCALONLY,
    /// TIMEOUT, PARAMS and DIALECT. For <c>WITHCURSOR</c>, use <see cref="HybridWithCursorAsync"/> instead.
    /// </param>
    /// <returns>
    /// An array of result row dictionaries, after both arms' results have been fused and any
    /// clauses in <paramref name="options"/> applied.
    /// </returns>
    public static Task<IDictionary<ValkeyValue, ValkeyValue>[]> HybridAsync(BaseClient client, ValkeyKey index, HybridSearchArm searchArm, HybridVsimArm vsimArm, HybridOptions options)
        => client.Command(Request.FtHybrid(index, searchArm, vsimArm, options));

    /// <summary>
    /// Runs <see cref="HybridAsync(BaseClient, ValkeyKey, HybridSearchArm, HybridVsimArm, HybridOptions)"/> with <see cref="HybridOptions.WithCursorOptions"/> set,
    /// returning the first page of rows and the cursor id directly, rather than through the
    /// plain-row shape <see cref="HybridAsync(BaseClient, ValkeyKey, HybridSearchArm, HybridVsimArm, HybridOptions)"/> returns.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index to query.</param>
    /// <param name="searchArm">The text/tag/numeric query arm. See <see cref="HybridSearchArm"/>.</param>
    /// <param name="vsimArm">The vector similarity search arm. See <see cref="HybridVsimArm"/>.</param>
    /// <param name="options">Options; <see cref="HybridOptions.WithCursorOptions"/> must be set.</param>
    /// <returns>A <see cref="CursorResult"/> with the first page of rows and the cursor id.</returns>
    /// <remarks>
    /// Routed to the PRIMARY of <paramref name="index"/>'s slot in cluster mode, regardless of the
    /// client's read-from configuration -- a cursor is a per-process, node-local resource.
    /// </remarks>
    public static Task<CursorResult> HybridWithCursorAsync(BaseClient client, ValkeyKey index, HybridSearchArm searchArm, HybridVsimArm vsimArm, HybridOptions options)
        => client.Command(Request.FtHybridWithCursor(index, searchArm, vsimArm, options), PrimaryRoute(index));

    #endregion
    #region Nested Types

    /// <summary>
    /// The SEARCH arm of an FT.HYBRID query: an ordinary (non-vector) FT.SEARCH-style text/tag/
    /// numeric query, run alongside the VSIM arm and fused with it. A vector query (e.g. a
    /// <c>=>[KNN ...]</c> clause) is rejected here -- use <see cref="HybridVsimArm"/> for the
    /// vector half instead.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    public sealed class HybridSearchArm
    {
        /// <summary>
        /// The text/tag/numeric query, in the same language FT.SEARCH uses. Must not be a vector
        /// query; the server rejects that combination.
        /// </summary>
        public required ValkeyValue Query { get; init; }

        /// <summary>
        /// The scoring function used to rank this arm's results, or <see langword="null"/> for
        /// server default (valkey-search 1.3.0). <c>"BM25STD"</c> is the only value currently
        /// registered and selectable; any other value is rejected by the server.
        /// </summary>
        public ValkeyValue Scorer { get; init; }

        /// <summary>
        /// Exposes this arm's score under the given column name, reachable from an
        /// <see cref="AggregateApply"/>, <see cref="AggregateSortExpression"/>,
        /// <see cref="AggregateGroupBy"/>/<see cref="AggregateReducer"/> expression, or
        /// <see cref="HybridFunction"/>'s <c>expr</c>. This alias cannot itself be named inside a
        /// <see cref="LoadField"/>/<see cref="HybridOptions.LoadFields"/> entry -- the server
        /// rejects it outright, since it's already present in the default reply with no LOAD needed.
        /// </summary>
        public ValkeyValue YieldScoreAs { get; init; }

        /// <summary>
        /// Converts a <see cref="string"/> to a <see cref="HybridSearchArm"/>.
        /// </summary>
        /// <param name="query">The query expression.</param>
        public static implicit operator HybridSearchArm(string query)
            => new() { Query = query };

        /// <summary>
        /// Converts a <see cref="ValkeyValue"/> to a <see cref="HybridSearchArm"/>.
        /// </summary>
        /// <param name="query">The query value.</param>
        public static implicit operator HybridSearchArm(ValkeyValue query)
            => new() { Query = query };

        /// <summary>
        /// Converts to command arguments for the SEARCH clause.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <see cref="Query"/> contains a vector query clause (<c>=&gt;[KNN</c>), which the server rejects on the SEARCH arm.</exception>
        internal GlideString[] ToArgs()
        {
            if (Query.Type == ValkeyValue.StorageType.String && Query.ToString().Contains("=>[KNN", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"{nameof(Query)} must not be a vector query (a '=>[KNN ...]' clause) -- the server rejects a vector " +
                        $"query on the SEARCH arm. Use {nameof(HybridVsimArm)} for the vector half instead.");
            }

            List<GlideString> args = [ValkeyLiterals.SEARCH, Query];
            if (!Scorer.IsNull)
            {
                EnsureSupportedScorer(Scorer);
                args.Add(ValkeyLiterals.SCORER);
                args.Add((GlideString)Scorer);
            }

            if (!YieldScoreAs.IsNull)
            {
                args.Add(ValkeyLiterals.YIELD_SCORE_AS);
                args.Add((GlideString)YieldScoreAs);
            }

            return [.. args];
        }
    }

    /// <summary>
    /// A pre-filter on the VSIM arm's vector search, written in the same query language the SEARCH
    /// arm uses (e.g. <c>@title:hello</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    /// <param name="expression">The filter expression.</param>
    [method: SetsRequiredMembers]
    public sealed class HybridVsimFilter(ValkeyValue expression)
    {
        /// <summary>
        /// The filter expression, in the same query language FT.SEARCH uses.
        /// </summary>
        public required ValkeyValue Expression { get; init; } = expression;

        /// <summary>
        /// Controls how the filter is applied. Setting this forces the wire-level counted VSIM
        /// FILTER form (required for the server to recognize POLICY at all).
        /// </summary>
        public HybridFilterPolicy? Policy { get; init; }

        /// <summary>
        /// Tunes how many candidates <see cref="HybridFilterPolicy.Batches"/> draws per batch.
        /// Requires <see cref="Policy"/> to be <see cref="HybridFilterPolicy.Batches"/>.
        /// </summary>
        public long? BatchSize { get; init; }

        /// <summary>
        /// Converts to command arguments for the VSIM FILTER clause.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <see cref="BatchSize"/> is set without <see cref="Policy"/> being <see cref="HybridFilterPolicy.Batches"/>.</exception>
        internal GlideString[] ToArgs()
        {
            if (BatchSize.HasValue && Policy != HybridFilterPolicy.Batches)
            {
                throw new ArgumentException($"{nameof(BatchSize)} requires {nameof(Policy)}={nameof(HybridFilterPolicy)}.{nameof(HybridFilterPolicy.Batches)}.");
            }

            List<GlideString> subArgs = [];
            if (Policy.HasValue)
            {
                subArgs.Add(ValkeyLiterals.POLICY);
                subArgs.Add(Policy.Value == HybridFilterPolicy.Adhoc ? ValkeyLiterals.ADHOC : ValkeyLiterals.BATCHES);
            }

            if (BatchSize.HasValue)
            {
                subArgs.Add(ValkeyLiterals.BATCH_SIZE);
                subArgs.Add(BatchSize.Value.ToGlideString());
            }

            if (subArgs.Count == 0)
            {
                // Uncounted extension: a bare expression, no POLICY/BATCH_SIZE possible in this form.
                return [ValkeyLiterals.FILTER, Expression];
            }

            long tokenCount = 1 + subArgs.Count;
            return [ValkeyLiterals.FILTER, tokenCount.ToGlideString(), Expression, .. subArgs];
        }
    }

    /// <summary>
    /// The VSIM arm of an FT.HYBRID query: a vector similarity search against a vector field, run
    /// alongside the SEARCH arm and fused with it.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    public sealed class HybridVsimArm
    {
        /// <summary>
        /// The vector field's identifier, without the leading <c>@</c> (added automatically).
        /// </summary>
        public required ValkeyValue Field { get; init; }

        /// <summary>
        /// The query vector, either as raw bytes/any <see cref="ValkeyValue"/> (an internal PARAMS
        /// entry is generated automatically) or as an explicit <c>$name</c> string referencing an
        /// entry the caller has placed in <see cref="HybridOptions.Params"/>. A string starting
        /// with <c>$</c> is always treated as a parameter reference, never as literal vector bytes
        /// -- a raw vector's packed binary representation can legitimately start with the byte
        /// <c>0x24</c> (<c>$</c>) by coincidence, so only an actual string value is checked.
        /// </summary>
        public required ValkeyValue Vector { get; init; }

        /// <summary>
        /// The number of nearest neighbours to return, or <see langword="null"/> for server default
        /// (optional on the wire -- omitting the whole KNN block, which happens when
        /// <see cref="K"/>, <see cref="EfRuntime"/> and <see cref="ShardKRatio"/> are all unset,
        /// defaults to 10, the same default RediSearch documents for FT.HYBRID's VSIM clause).
        /// </summary>
        public long? K { get; init; }

        /// <summary>
        /// HNSW search-time effort parameter. Only valid when the KNN block is written.
        /// </summary>
        public long? EfRuntime { get; init; }

        /// <summary>
        /// Tunes how much of <see cref="K"/> each shard returns during a cluster fanout.
        /// </summary>
        public double? ShardKRatio { get; init; }

        /// <summary>
        /// Exposes this arm's score (the vector distance) under the given column name. See
        /// <see cref="HybridSearchArm.YieldScoreAs"/>'s remarks for the LOAD-clause restriction
        /// that applies identically here.
        /// </summary>
        public ValkeyValue YieldScoreAs { get; init; }

        /// <summary>
        /// A pre-filter on the vector search. See <see cref="HybridVsimFilter"/>.
        /// </summary>
        public HybridVsimFilter? Filter { get; init; }

        /// <summary>
        /// <see langword="true"/> if <see cref="Vector"/> is a <c>$name</c> parameter reference
        /// rather than raw vector bytes.
        /// </summary>
        internal bool IsParamReference => Vector.Type == ValkeyValue.StorageType.String && Vector.ToString().StartsWith('$');

        /// <summary>
        /// Converts to command arguments for the VSIM clause.
        /// </summary>
        /// <param name="paramRef">
        /// The <c>$</c>-prefixed parameter name to reference on the wire. Supplied by
        /// <see cref="Request.FtHybrid"/>/<see cref="Request.FtHybridWithCursor"/> when
        /// <see cref="Vector"/> was given as raw bytes rather than an explicit <c>$name</c> string.
        /// </param>
        /// <exception cref="ArgumentException">Thrown if <see cref="EfRuntime"/> is set while <see cref="Filter"/>'s policy is <see cref="HybridFilterPolicy.Adhoc"/>.</exception>
        internal GlideString[] ToArgs(GlideString paramRef)
        {
            if (EfRuntime.HasValue && Filter is { Policy: HybridFilterPolicy.Adhoc })
            {
                throw new ArgumentException(
                    $"{nameof(EfRuntime)} is not supported with {nameof(Filter)}.{nameof(HybridVsimFilter.Policy)}={nameof(HybridFilterPolicy)}.{nameof(HybridFilterPolicy.Adhoc)} " +
                        "(the server rejects this combination: \"EF_RUNTIME is irrelevant for POLICY ADHOC\").");
            }

            GlideString fieldRef = $"@{Field}";
            List<GlideString> args = [ValkeyLiterals.VSIM, fieldRef, paramRef];

            if (K.HasValue || EfRuntime.HasValue || ShardKRatio.HasValue)
            {
                List<GlideString> subArgs = [];
                if (K.HasValue)
                {
                    subArgs.Add(ValkeyLiterals.K);
                    subArgs.Add(K.Value.ToGlideString());
                }

                if (EfRuntime.HasValue)
                {
                    subArgs.Add(ValkeyLiterals.EF_RUNTIME);
                    subArgs.Add(EfRuntime.Value.ToGlideString());
                }

                if (ShardKRatio.HasValue)
                {
                    subArgs.Add(ValkeyLiterals.SHARD_K_RATIO);
                    subArgs.Add(ShardKRatio.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                }

                args.Add(ValkeyLiterals.KNN);
                args.Add(subArgs.Count.ToGlideString());
                args.AddRange(subArgs);
            }

            if (!YieldScoreAs.IsNull)
            {
                args.Add(ValkeyLiterals.YIELD_SCORE_AS);
                args.Add((GlideString)YieldScoreAs);
            }

            if (Filter is not null)
            {
                args.AddRange(Filter.ToArgs());
            }

            return [.. args];
        }
    }

    /// <summary>
    /// Base class for the COMBINE clause of FT.HYBRID, controlling how the SEARCH and VSIM arms'
    /// per-document scores are fused into one ranking. See <see cref="HybridRrf"/>,
    /// <see cref="HybridLinear"/> and <see cref="HybridFunction"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    public abstract class HybridCombine
    {
        /// <summary>
        /// Converts to command arguments for the COMBINE clause.
        /// </summary>
        internal abstract GlideString[] ToArgs();
    }

    /// <summary>
    /// Fuses the two arms' results using Reciprocal Rank Fusion: each arm's rank (not its raw
    /// score) contributes <c>1 / (constant + rank)</c> to the fused score.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    public sealed class HybridRrf : HybridCombine
    {
        /// <summary>
        /// The RRF constant. Must be non-negative (the server rejects a negative value -- it
        /// places a pole at <c>rank == -constant</c>).
        /// </summary>
        public double? Constant { get; init; }

        /// <summary>
        /// How many of each arm's results take part in fusion.
        /// </summary>
        public long? Window { get; init; }

        /// <summary>
        /// Exposes the fused score under the given column name instead of the default <c>__score</c>.
        /// </summary>
        public ValkeyValue YieldScoreAs { get; init; }

        /// <inheritdoc cref="HybridCombine.ToArgs"/>
        /// <exception cref="ArgumentException">Thrown if <see cref="Constant"/> is negative.</exception>
        internal override GlideString[] ToArgs()
        {
            if (Constant is < 0)
            {
                throw new ArgumentException($"{nameof(Constant)} must not be negative.");
            }

            List<GlideString> subArgs = [];
            if (Constant.HasValue)
            {
                subArgs.Add(ValkeyLiterals.CONSTANT);
                subArgs.Add(Constant.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            if (Window.HasValue)
            {
                subArgs.Add(ValkeyLiterals.WINDOW);
                subArgs.Add(Window.Value.ToGlideString());
            }

            if (!YieldScoreAs.IsNull)
            {
                subArgs.Add(ValkeyLiterals.YIELD_SCORE_AS);
                subArgs.Add((GlideString)YieldScoreAs);
            }

            return [ValkeyLiterals.COMBINE, ValkeyLiterals.RRF, subArgs.Count.ToGlideString(), .. subArgs];
        }
    }

    /// <summary>
    /// Fuses the two arms' results linearly: <c>alpha * search_score + beta * vsim_score</c>.
    /// Writing neither <see cref="Alpha"/> nor <see cref="Beta"/> defaults both to 0.3/0.7
    /// respectively; writing exactly one without the other is rejected by the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    public sealed class HybridLinear : HybridCombine
    {
        /// <summary>
        /// The SEARCH arm's weight. Defaults (server-side) to <c>0.3</c> when neither this nor
        /// <see cref="Beta"/> is set.
        /// </summary>
        public double? Alpha { get; init; }

        /// <summary>
        /// The VSIM arm's weight. Defaults (server-side) to <c>0.7</c> when neither this nor
        /// <see cref="Alpha"/> is set.
        /// </summary>
        public double? Beta { get; init; }

        /// <summary>
        /// How many of each arm's results take part in fusion.
        /// </summary>
        public long? Window { get; init; }

        /// <summary>
        /// Exposes the fused score under the given column name instead of the default <c>__score</c>.
        /// </summary>
        public ValkeyValue YieldScoreAs { get; init; }

        /// <inheritdoc cref="HybridCombine.ToArgs"/>
        /// <exception cref="ArgumentException">Thrown if exactly one of <see cref="Alpha"/>/<see cref="Beta"/> is set without the other.</exception>
        internal override GlideString[] ToArgs()
        {
            if (Alpha.HasValue != Beta.HasValue)
            {
                throw new ArgumentException($"{nameof(Alpha)} and {nameof(Beta)} must both be set, or both left unset -- the server rejects writing exactly one of them.");
            }

            List<GlideString> subArgs = [];
            if (Alpha.HasValue)
            {
                subArgs.Add(ValkeyLiterals.ALPHA);
                subArgs.Add(Alpha.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            if (Beta.HasValue)
            {
                subArgs.Add(ValkeyLiterals.BETA);
                subArgs.Add(Beta.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            if (Window.HasValue)
            {
                subArgs.Add(ValkeyLiterals.WINDOW);
                subArgs.Add(Window.Value.ToGlideString());
            }

            if (!YieldScoreAs.IsNull)
            {
                subArgs.Add(ValkeyLiterals.YIELD_SCORE_AS);
                subArgs.Add((GlideString)YieldScoreAs);
            }

            return [ValkeyLiterals.COMBINE, ValkeyLiterals.LINEAR, subArgs.Count.ToGlideString(), .. subArgs];
        }
    }

    /// <summary>
    /// Fuses the two arms' results using a caller-supplied expression. <see cref="Expr"/> is a raw
    /// string written in the same expression language <see cref="AggregateApply.Expression"/>
    /// uses -- the server compiles it against a context where each arm's score is reachable via
    /// its own <c>YIELD_SCORE_AS</c> alias (if any), a positional default <c>@__arm&lt;i&gt;_score</c>,
    /// and (for this SEARCH+VSIM two-arm shape) <c>@__search_score</c> / <c>@__vector_score</c> specifically.
    /// This is a valkey-search extension not present in RediSearch's FT.HYBRID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    public sealed class HybridFunction : HybridCombine
    {
        /// <summary>
        /// The fusion expression, as a raw string (e.g. <c>"@__search_score + @__vector_score"</c>
        /// or, with explicit arm aliases, <c>"@ss + @vs"</c>).
        /// </summary>
        public required ValkeyValue Expr { get; init; }

        /// <summary>
        /// How many of each arm's results take part in fusion. If left unset, the server defaults
        /// this to the widest window allowed rather than the usual RRF/LINEAR default, since a
        /// caller-written expression typically expects to see every matching candidate.
        /// </summary>
        public long? Window { get; init; }

        /// <summary>
        /// Exposes the fused score under the given column name instead of the default <c>__score</c>.
        /// </summary>
        public ValkeyValue YieldScoreAs { get; init; }

        /// <inheritdoc cref="HybridCombine.ToArgs"/>
        internal override GlideString[] ToArgs()
        {
            List<GlideString> subArgs = [ValkeyLiterals.EXPR, (GlideString)Expr];
            if (Window.HasValue)
            {
                subArgs.Add(ValkeyLiterals.WINDOW);
                subArgs.Add(Window.Value.ToGlideString());
            }

            if (!YieldScoreAs.IsNull)
            {
                subArgs.Add(ValkeyLiterals.YIELD_SCORE_AS);
                subArgs.Add((GlideString)YieldScoreAs);
            }

            return [ValkeyLiterals.COMBINE, ValkeyLiterals.FUNCTION, subArgs.Count.ToGlideString(), .. subArgs];
        }
    }

    /// <summary>
    /// Options for the FT.HYBRID command, applied to the aggregate-suffix tail that follows the
    /// mandatory SEARCH, VSIM and optional COMBINE clauses.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.hybrid/">Valkey commands – FT.HYBRID</seealso>
    public sealed class HybridOptions
    {
        /// <summary>
        /// Controls how the SEARCH and VSIM arms' scores are fused. See <see cref="HybridRrf"/>,
        /// <see cref="HybridLinear"/> and <see cref="HybridFunction"/>. If omitted, the server's
        /// own default fusion method applies.
        /// </summary>
        public HybridCombine? Combine { get; init; }

        /// <summary>
        /// Loads every field declared in the index, plus both arms' score aliases automatically.
        /// Takes priority over <see cref="LoadFields"/> when set.
        /// </summary>
        public bool LoadAll { get; init; }

        /// <summary>
        /// Loads only the fields passed in this list, reusing <see cref="LoadField"/> from the
        /// FT.AGGREGATE options (the LOAD clause's grammar is byte-identical between the two
        /// commands). Must NOT include either arm's <c>YieldScoreAs</c> alias -- the server rejects
        /// it outright even though the alias is otherwise freely reachable. With no LOAD clause at
        /// all (<see cref="LoadAll"/> false and this unset), FT.HYBRID's default reply is the
        /// document key and the fused score.
        /// </summary>
        public IEnumerable<LoadField>? LoadFields { get; init; }

        /// <summary>
        /// FILTER, LIMIT, GROUPBY, SORTBY and APPLY clauses, reusing <see cref="IAggregateClause"/>
        /// and its subclasses verbatim from the FT.AGGREGATE options (FT.HYBRID forwards this whole
        /// tail to the exact same aggregate pipeline parser FT.AGGREGATE uses). Every arm alias is
        /// freely reachable from these clauses -- only the LOAD clause has the restriction described
        /// on <see cref="LoadFields"/>. FT.HYBRID also applies its own default LIMIT of 10 rows when
        /// no <see cref="AggregateLimit"/> clause is given here -- FT.AGGREGATE has no such default
        /// and returns everything.
        /// </summary>
        public IEnumerable<IAggregateClause> Clauses { get; init; } = [];

        /// <summary>
        /// Requests a paged reply. See <see cref="WithCursor"/>.
        /// </summary>
        public WithCursor? WithCursorOptions { get; init; }

        /// <summary>
        /// Restricts the query to the local shard rather than fanning out across the cluster. This
        /// is a valkey-search extension not present in RediSearch's FT.HYBRID.
        /// </summary>
        public bool LocalOnly { get; init; }

        /// <summary>
        /// Overrides the timeout parameter of the module, in milliseconds.
        /// </summary>
        public TimeSpan? Timeout { get; init; }

        /// <summary>
        /// Param key/value pairs supplying a VSIM arm's <c>$name</c> vector reference, or any
        /// aggregate clause's (FILTER, APPLY) expression text. The SEARCH arm's query text is not
        /// substituted -- same as FT.SEARCH's own non-vector predicates, a bare <c>$name</c> there
        /// either fails to parse or matches the literal text, never a param value. If
        /// <see cref="HybridVsimArm.Vector"/> in the same call is given raw vector bytes rather
        /// than an explicit <c>$name</c> string, an internally generated entry is merged into this
        /// mapping automatically -- a name collision with a caller-supplied entry here throws
        /// <see cref="ArgumentException"/> rather than silently overwriting either one.
        /// </summary>
        public IDictionary<ValkeyValue, ValkeyValue> Params { get; init; } = new Dictionary<ValkeyValue, ValkeyValue>();

        /// <summary>
        /// The query dialect version to use, or <see langword="null"/> for server default.
        /// FT.HYBRID is fixed at dialect 2 server-side; passing any other value is rejected by the server.
        /// </summary>
        public int? Dialect { get; init; }
    }

    #endregion
}
