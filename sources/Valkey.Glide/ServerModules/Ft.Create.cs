// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

public static partial class Ft
{
    #region Public Methods

    /// <summary>
    /// Creates a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the new index.</param>
    /// <param name="field">The field definition for the index schema.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var field = new Ft.CreateTextField("title");
    /// await Ft.CreateAsync(client, "index", field);
    /// </code>
    /// </example>
    /// </remarks>
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
    /// <remarks>
    /// <example>
    /// <code>
    /// var field = new Ft.CreateTextField("title");
    /// var options = new Ft.CreateOptions { Prefixes = ["doc:"] };
    /// await Ft.CreateAsync(client, "index", field, options);
    /// </code>
    /// </example>
    /// </remarks>
    public static Task CreateAsync(BaseClient client, ValkeyKey index, CreateField field, CreateOptions options)
        => client.Command(Request.FtCreate(index, [field], options));

    /// <summary>
    /// Creates a new search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the new index.</param>
    /// <param name="schema">The field definitions for the index schema.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var fields = new Ft.CreateField[]
    /// {
    ///     new Ft.CreateTextField("title"),
    ///     new Ft.CreateTagField("category"),
    ///     new Ft.CreateNumericField("price"),
    /// }
    /// await Ft.CreateAsync(client, "index", fields);
    /// </code>
    /// </example>
    /// </remarks>
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
    /// <remarks>
    /// <example>
    /// <code>
    /// var fields = new Ft.CreateField[]
    /// {
    ///     new Ft.CreateTextField("title"),
    ///     new Ft.CreateTagField("category"),
    ///     new Ft.CreateNumericField("price"),
    /// }
    /// var options = new Ft.CreateOptions
    /// {
    ///     DataType = Ft.DataType.Hash,
    ///     Prefixes = ["doc:"],
    /// }
    /// await Ft.CreateAsync(client, "index", fields, options);
    /// </code>
    /// </example>
    /// </remarks>
    public static Task CreateAsync(BaseClient client, ValkeyKey index, IEnumerable<CreateField> schema, CreateOptions options)
        => client.Command(Request.FtCreate(index, schema, options));

    #endregion

    #region Nested Types

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
        /// The data structure type for search index (<c>ON HASH</c>/<c>ON JSON</c>).
        /// </summary>
        public DataType DataType { get; init; } = DataType.Hash;

        /// <summary>
        /// Key prefixes to include in the index (<c>PREFIX</c>).
        /// </summary>
        public IEnumerable<ValkeyValue> Prefixes { get; init; } = [];

        /// <summary>
        /// Whether to skip indexing existing documents on creation (<c>SKIPINITIALSCAN</c>).
        /// </summary>
        public bool SkipInitialScan { get; init; }

        /// <summary>
        /// The minimum word length for stemming
        /// or <see langword="null"/> for server default (<c>MINSTEMSIZE</c>).
        /// </summary>
        public long? MinStemSize { get; init; }

        /// <summary>
        /// Whether to disable term offset storage (<c>NOOFFSETS</c>).
        /// </summary>
        public bool NoOffsets { get; init; }

        /// <summary>
        /// Custom stop words, <see cref="NoStopWords"/> to disable,
        /// or <see langword="null"/> for server default (<c>STOPWORDS</c>).
        /// </summary>
        public IEnumerable<ValkeyValue>? StopWords { get; init; }

        /// <summary>
        /// Custom punctuation characters to use for tokenization,
        /// or <see cref="ValkeyValue.Null"/> for server default (<c>PUNCTUATION</c>).
        /// </summary>
        public ValkeyValue Punctuation { get; init; }

        #endregion
    }

    /// <summary>
    /// Abstract base class for a new search index field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public abstract class CreateField
    {
        /// <summary>
        /// The field identifier: a hash field name (<c>ON HASH</c>) or a JSON path (<c>ON JSON</c>).
        /// </summary>
        public required ValkeyValue Identifier { get; init; }

        /// <summary>
        /// An optional alias used as the field name in query results (<c>AS field-alias</c>).
        /// </summary>
        public ValkeyValue Alias { get; init; }
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
        /// Whether to enable the suffix trie optimization for this field (<c>NOSUFFIXTRIE</c>/<c>NOSUFFIXTRIE</c>).
        /// </summary>
        public bool WithSuffixTrie { get; init; } = true;
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
        /// or <see langword="null"/> for server default (<c>SEPARATOR</c>).
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
        /// The number of dimensions in the vector (<c>DIM</c>).
        /// </summary>
        public required long Dimensions { get; init; }

        /// <summary>
        /// The distance metric for vector similarity (<c>DISTANCE_METRIC</c>).
        /// </summary>
        public required DistanceMetric DistanceMetric { get; init; }

        /// <summary>
        /// Initial vector capacity, or <see langword="null"/> for server default (<c>INITIAL_CAP</c>).
        /// </summary>
        public long? InitialCap { get; init; }
    }

    /// <summary>
    /// A new vector search index field using the brute-force algorithm (<c>VECTOR FLAT</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public sealed class CreateVectorFieldFlat : CreateVectorField
    {
    }

    /// <summary>
    /// A new vector search index field using the Hierarchical Navigable Small World algorithm (<c>VECTOR HNSW</c>).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.create/">Valkey commands – FT.CREATE</seealso>
    public sealed class CreateVectorFieldHnsw : CreateVectorField
    {
        /// <summary>
        /// Maximum outgoing edges per graph node,
        /// or <see langword="null"/> for server default (<c>M</c>).
        /// </summary>
        public long? NumberOfEdges { get; init; }

        /// <summary>
        /// Vectors examined during index construction,
        /// or <see langword="null"/> for server default (<c>EF_CONSTRUCTION</c>).
        /// </summary>
        public long? VectorsExaminedOnConstruction { get; init; }

        /// <summary>
        /// Vectors examined during queries,
        /// or <see langword="null"/> for server default (<c>EF_RUNTIME</c>).
        /// </summary>
        public long? VectorsExaminedOnRuntime { get; init; }
    }

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

    #endregion
}
