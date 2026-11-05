// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

public static partial class Ft
{
    #region Nested Enums

    /// <summary>
    /// The distance metric used to measure similarity in a vector search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public enum DistanceMetric
    {
        /// <summary>
        /// Cosine distance.
        /// </summary>
        Cosine,

        /// <summary>
        /// Euclidean (L2) distance.
        /// </summary>
        Euclidean,

        /// <summary>
        /// Inner product distance.
        /// </summary>
        InnerProduct,
    }

    /// <summary>
    /// The data type used to store documents in a search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public enum DataType
    {
        /// <summary>
        /// Data stored in hashes. Field identifiers are hash field names.
        /// </summary>
        Hash,

        /// <summary>
        /// Data stored as JSON. Field identifiers are JSON paths.
        /// </summary>
        Json,
    }

    /// <summary>
    /// The element type and encoding of a vector search index field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public enum VectorType
    {
        /// <summary>
        /// 4 bytes per element, IEEE 754 single precision (<c>FLOAT32</c>).
        /// </summary>
        Float32,

        /// <summary>
        /// 2 bytes per element, IEEE 754 half precision (<c>FLOAT16</c>) (valkey-search 1.3.0).
        /// Callers must pass already-encoded bytes; GLIDE does not encode vectors on the client's behalf.
        /// </summary>
        Float16,

        /// <summary>
        /// 2 bytes per element, truncated IEEE 754 single precision: the top 16 bits of a FLOAT32
        /// (sign, 8-bit exponent, 7-bit mantissa) (<c>BFLOAT16</c>) (valkey-search 1.3.0).
        /// Callers must pass already-encoded bytes; GLIDE does not encode vectors on the client's behalf.
        /// The engine may refuse to create or load a BFLOAT16 index on a host whose CPU lacks a SIMD
        /// BF16 code path; this is a server-side capability check, not a GLIDE-side restriction.
        /// </summary>
        Bfloat16,
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the new index.</param>
    /// <param name="field">The field definition for the index schema.</param>
    /// <example>
    /// <code>
    /// var field = new Ft.CreateTextField("title");
    /// await Ft.CreateAsync(client, "index", field);
    /// </code>
    /// </example>
    public static Task CreateAsync(BaseClient client, ValkeyKey index, CreateField field)
        => client.Command(Request.FtCreate(index, [field]));

    /// <summary>
    /// Creates a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the new index.</param>
    /// <param name="field">The field definition for the index schema.</param>
    /// <param name="options">Options for index creation.</param>
    /// <example>
    /// <code>
    /// var field = new Ft.CreateTextField("title");
    /// var options = new Ft.CreateOptions { Prefixes = ["doc:"] };
    /// await Ft.CreateAsync(client, "index", field, options);
    /// </code>
    /// </example>
    public static Task CreateAsync(BaseClient client, ValkeyKey index, CreateField field, CreateOptions options)
        => client.Command(Request.FtCreate(index, [field], options));

    /// <summary>
    /// Creates a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the new index.</param>
    /// <param name="schema">The field definitions for the index schema.</param>
    /// <example>
    /// <code>
    /// var fields = new Ft.CreateField[]
    /// {
    ///     new Ft.CreateTextField("title"),
    ///     new Ft.CreateTagField("category"),
    ///     new Ft.CreateNumericField("price"),
    /// };
    /// await Ft.CreateAsync(client, "index", fields);
    /// </code>
    /// </example>
    public static Task CreateAsync(BaseClient client, ValkeyKey index, IEnumerable<CreateField> schema)
        => client.Command(Request.FtCreate(index, schema));

    /// <summary>
    /// Creates a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the new index.</param>
    /// <param name="schema">The field definitions for the index schema.</param>
    /// <param name="options">Additional options for index creation.</param>
    /// <example>
    /// <code>
    /// var fields = new Ft.CreateField[]
    /// {
    ///     new Ft.CreateTextField("title"),
    ///     new Ft.CreateTagField("category"),
    ///     new Ft.CreateNumericField("price"),
    /// };
    /// var options = new Ft.CreateOptions
    /// {
    ///     DataType = Ft.DataType.Hash,
    ///     Prefixes = ["doc:"],
    /// };
    /// await Ft.CreateAsync(client, "index", fields, options);
    /// </code>
    /// </example>
    public static Task CreateAsync(BaseClient client, ValkeyKey index, IEnumerable<CreateField> schema, CreateOptions options)
        => client.Command(Request.FtCreate(index, schema, options));

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts the given <see cref="DistanceMetric"/> to the corresponding command arguments.
    /// </summary>
    /// <param name="metric">The distance metric to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="metric"/> is not a supported <see cref="DistanceMetric"/> value.</exception>
    internal static GlideString[] ToArgs(this DistanceMetric metric) => metric switch
    {
        DistanceMetric.Cosine => [ValkeyLiterals.COSINE],
        DistanceMetric.Euclidean => [ValkeyLiterals.L2],
        DistanceMetric.InnerProduct => [ValkeyLiterals.IP],
        _ => throw new ArgumentOutOfRangeException(nameof(metric)),
    };

    /// <summary>
    /// Converts the given <see cref="DataType"/> to the corresponding command arguments.
    /// </summary>
    /// <param name="dataType">The data type to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="dataType"/> is not a supported <see cref="DataType"/> value.</exception>
    internal static GlideString[] ToArgs(this DataType dataType) => dataType switch
    {
        DataType.Hash => [ValkeyLiterals.HASH],
        DataType.Json => [ValkeyLiterals.JSON],
        _ => throw new ArgumentOutOfRangeException(nameof(dataType)),
    };

    /// <summary>
    /// Converts to a command argument.
    /// </summary>
    /// <param name="type">The vector type to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="type"/> is not a supported <see cref="VectorType"/> value.</exception>
    internal static GlideString ToArg(this VectorType type) => type switch
    {
        VectorType.Float32 => ValkeyLiterals.FLOAT32,
        VectorType.Float16 => ValkeyLiterals.FLOAT16,
        VectorType.Bfloat16 => ValkeyLiterals.BFLOAT16,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    #endregion
    #region Nested Classes

    /// <summary>
    /// The options for an operation to create a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public sealed class CreateOptions
    {
        #region Constants

        /// <summary>
        /// Disables stop-word filtering (<c>STOPWORDS 0</c>).
        /// </summary>
        public static readonly IEnumerable<ValkeyValue> NoStopWords = [];

        #endregion
        #region Properties

        /// <summary>
        /// The data structure type for the index (<c>ON HASH</c>/<c>ON JSON</c>).
        /// </summary>
        public DataType DataType { get; init; } = DataType.Hash;

        /// <summary>
        /// Key prefixes to include in the index (<c>PREFIX prefix</c>).
        /// </summary>
        public IEnumerable<ValkeyValue> Prefixes { get; init; } = [];

        /// <summary>
        /// Whether to skip indexing existing documents on creation (<c>SKIPINITIALSCAN</c>).
        /// </summary>
        public bool SkipInitialScan { get; init; }

        /// <summary>
        /// The minimum word length for stemming,
        /// or <see langword="null"/> for server default (<c>MINSTEMSIZE min_stem_size</c>).
        /// </summary>
        public long? MinStemSize { get; init; }

        /// <summary>
        /// Whether to disable term offset storage (<c>NOOFFSETS</c>).
        /// </summary>
        public bool NoOffsets { get; init; }

        /// <summary>
        /// Custom stop words, <see cref="NoStopWords"/> to disable,
        /// or <see langword="null"/> for server default (<c>STOPWORDS count word</c>).
        /// </summary>
        public IEnumerable<ValkeyValue>? StopWords { get; init; }

        /// <summary>
        /// Custom punctuation characters to use for tokenization,
        /// or <see cref="ValkeyValue.Null"/> for server default (<c>PUNCTUATION punctuation</c>).
        /// </summary>
        public ValkeyValue Punctuation { get; init; }

        /// <summary>
        /// The default relevance score for documents in the index,
        /// or <see langword="null"/> for server default (1.0) (<c>SCORE score</c>).
        /// </summary>
        public double? Score { get; init; }

        /// <summary>
        /// The default stemming language for documents in the index,
        /// or <see langword="null"/> for server default (<c>LANGUAGE language</c>).
        /// Passed through verbatim; the server rejects an unrecognized value.
        /// </summary>
        public ValkeyValue Language { get; init; }

        /// <summary>
        /// Whether to explicitly store term offsets (<c>WITHOFFSETS</c>).
        /// Mutually exclusive with <see cref="NoOffsets"/>.
        /// </summary>
        public bool WithOffsets { get; init; }

        /// <summary>
        /// A filter expression limiting which documents the index applies to (<c>FILTER expression</c>).
        /// A document that doesn't match this expression is excluded from the index even if its key
        /// matches a configured prefix (valkey-search 1.3.0). The expression must not be empty.
        /// </summary>
        public ValkeyValue Filter { get; init; }

        /// <summary>
        /// The name of a document field whose numeric value is used as the document's custom score,
        /// multiplying into the final relevance score (<c>SCORE_FIELD field</c>) (valkey-search 1.3.0).
        /// The field must already exist in the document; it is not itself a schema definition.
        /// <c>FT.INFO</c> only reports this option back under <c>index_definition.score_field</c> when
        /// <c>search.emulate-release</c> is at least 1.3.0.
        /// </summary>
        public ValkeyValue ScoreField { get; init; }

        /// <summary>
        /// Emits <c>NOHL</c> for wire compatibility with RediSearch clients. valkey-search does not
        /// support highlighting, so this is a no-op on this engine.
        /// </summary>
        public bool NoHighlight { get; init; }

        #endregion
    }

    /// <summary>
    /// Abstract base class for a new search index field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public abstract class CreateField
    {
        /// <summary>
        /// The field identifier: a hash field name or a JSON path (<c>field-identifier</c>).
        /// </summary>
        public required ValkeyValue Identifier { get; init; }

        /// <summary>
        /// An optional alias used as the field name in query results (<c>AS field-alias</c>).
        /// </summary>
        public ValkeyValue Alias { get; init; }

        /// <summary>
        /// Whether the field value can be used for sorting (<c>SORTABLE</c>).
        /// Applies to TEXT, TAG and NUMERIC fields.
        /// </summary>
        public bool Sortable { get; init; }

        /// <summary>
        /// Whether to keep the sorted value in its original (un-normalized) form, e.g. preserving case
        /// that would otherwise be lowercased for sorting (<c>UNF</c>) (valkey-search 1.3.0).
        /// Applies to TEXT, TAG and NUMERIC fields. Requires <see cref="Sortable"/> to be set;
        /// otherwise <c>UNF</c> is omitted.
        /// </summary>
        public bool Unf { get; init; }
    }

    /// <summary>
    /// A new full-text search index field (<c>TEXT</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public sealed class CreateTextField : CreateField
    {
        /// <summary>
        /// Creates a new <see cref="CreateTextField"/> with the specified identifier and optional alias.
        /// </summary>
        /// <param name="identifier">The field identifier.</param>
        /// <param name="alias">An optional alias for the field.</param>
        [SetsRequiredMembers]
        public CreateTextField(ValkeyValue identifier, ValkeyValue alias = default)
        {
            Identifier = identifier;
            Alias = alias;
        }

        /// <summary>
        /// Whether to disable stemming for this field (<c>NOSTEM</c>).
        /// </summary>
        public bool NoStem { get; init; }

        /// <summary>
        /// Whether to enable the suffix trie optimization for this field (<c>WITHSUFFIXTRIE</c>/<c>NOSUFFIXTRIE</c>).
        /// </summary>
        public bool WithSuffixTrie { get; init; } = true;

        /// <summary>
        /// Declares the importance of this field when calculating result accuracy,
        /// or <see langword="null"/> for server default (1.0) (<c>WEIGHT weight</c>).
        /// </summary>
        public double? Weight { get; init; }
    }

    /// <summary>
    /// A new tag search index field (<c>TAG</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public sealed class CreateTagField : CreateField
    {
        /// <summary>
        /// Creates a new <see cref="CreateTagField"/> with the specified identifier and optional alias.
        /// </summary>
        /// <param name="identifier">The field identifier.</param>
        /// <param name="alias">An optional alias for the field.</param>
        [SetsRequiredMembers]
        public CreateTagField(ValkeyValue identifier, ValkeyValue alias = default)
        {
            Identifier = identifier;
            Alias = alias;
        }

        /// <summary>
        /// The separator character for splitting tag values,
        /// or <see langword="null"/> for server default (<c>SEPARATOR sep</c>).
        /// </summary>
        public char? Separator { get; init; }

        /// <summary>
        /// Whether tag comparisons are case-sensitive (<c>CASESENSITIVE</c>).
        /// </summary>
        public bool CaseSensitive { get; init; }
    }

    /// <summary>
    /// A new numeric search index field (<c>NUMERIC</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public sealed class CreateNumericField : CreateField
    {
        /// <summary>
        /// Creates a new <see cref="CreateNumericField"/> with the specified identifier and optional alias.
        /// </summary>
        /// <param name="identifier">The field identifier.</param>
        /// <param name="alias">An optional alias for the field.</param>
        [SetsRequiredMembers]
        public CreateNumericField(ValkeyValue identifier, ValkeyValue alias = default)
        {
            Identifier = identifier;
            Alias = alias;
        }
    }

    /// <summary>
    /// Abstract base class for a new vector search index field (<c>VECTOR</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public abstract class CreateVectorField : CreateField
    {
        /// <summary>
        /// The number of dimensions in the vector (<c>DIM number</c>).
        /// </summary>
        public required long Dimensions { get; init; }

        /// <summary>
        /// The distance metric for vector similarity (<c>DISTANCE_METRIC</c>).
        /// </summary>
        public required DistanceMetric DistanceMetric { get; init; }

        /// <summary>
        /// Initial vector capacity, or <see langword="null"/> for server default (<c>INITIAL_CAP size</c>).
        /// </summary>
        public long? InitialCap { get; init; }

        /// <summary>
        /// The element type and encoding of the vector (<c>TYPE</c>).
        /// Defaults to <see cref="VectorType.Float32"/>.
        /// </summary>
        public VectorType Type { get; init; } = VectorType.Float32;
    }

    /// <summary>
    /// A new vector search index field using the brute-force algorithm (<c>VECTOR FLAT</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public sealed class CreateVectorFieldFlat : CreateVectorField;

    /// <summary>
    /// A new vector search index field using the Hierarchical Navigable Small World algorithm (<c>VECTOR HNSW</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public sealed class CreateVectorFieldHnsw : CreateVectorField
    {
        /// <summary>
        /// Maximum outgoing edges per graph node,
        /// or <see langword="null"/> for server default (<c>M number</c>).
        /// </summary>
        public long? NumberOfEdges { get; init; }

        /// <summary>
        /// Vectors examined during index construction,
        /// or <see langword="null"/> for server default (<c>EF_CONSTRUCTION number</c>).
        /// </summary>
        public long? VectorsExaminedOnConstruction { get; init; }

        /// <summary>
        /// Vectors examined during queries,
        /// or <see langword="null"/> for server default (<c>EF_RUNTIME number</c>).
        /// </summary>
        public long? VectorsExaminedOnRuntime { get; init; }
    }

    #endregion
}
