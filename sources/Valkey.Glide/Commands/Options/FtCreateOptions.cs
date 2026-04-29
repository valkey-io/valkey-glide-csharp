// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Base interface for all index schema field types used in FT.CREATE.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public interface IField
{
    /// <summary>
    /// Returns the command arguments for this field definition.
    /// </summary>
    GlideString[] ToArgs();
}

/// <summary>
/// Represents a full-text search field in an index schema.
/// </summary>
/// <remarks>
/// Use the fluent builder methods (<see cref="WithAlias"/>, <see cref="AsSortable"/>,
/// <see cref="WithNoStem"/>, <see cref="WithWeight"/>, <see cref="WithSuffixTrie(bool?)"/>)
/// to configure the field, or set properties directly via object initializer.
/// </remarks>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public sealed class TextField(ValkeyValue name) : IField
{
    /// <summary>The field name.</summary>
    public ValkeyValue Name { get; } = name;

    /// <summary>Optional alias for the field.</summary>
    public ValkeyValue? Alias { get; set; }

    /// <summary>Whether the field is sortable.</summary>
    public bool Sortable { get; set; }

    /// <summary>Disables stemming when indexing the field.</summary>
    public bool NoStem { get; set; }

    /// <summary>The importance of this field when calculating result accuracy.</summary>
    public double? Weight { get; set; }

    /// <summary>
    /// Controls suffix trie behaviour for the field.
    /// <list type="bullet">
    ///   <item><description><see langword="true"/> — emit <c>WITHSUFFIXTRIE</c></description></item>
    ///   <item><description><see langword="false"/> — emit <c>NOSUFFIXTRIE</c></description></item>
    ///   <item><description><see langword="null"/> (default) — omit the option entirely</description></item>
    /// </list>
    /// </summary>
    public bool? SuffixTrie { get; set; }

    // ── Fluent builder methods ────────────────────────────────────────────────

    /// <summary>Sets the optional alias for the field.</summary>
    public TextField WithAlias(ValkeyValue alias) { Alias = alias; return this; }

    /// <summary>Marks the field as sortable.</summary>
    public TextField AsSortable() { Sortable = true; return this; }

    /// <summary>Disables stemming for the field.</summary>
    public TextField WithNoStem() { NoStem = true; return this; }

    /// <summary>Sets the field weight used when calculating result accuracy.</summary>
    public TextField WithWeight(double weight) { Weight = weight; return this; }

    /// <summary>
    /// Controls the suffix trie for the field.
    /// Pass <see langword="true"/> for <c>WITHSUFFIXTRIE</c>, <see langword="false"/> for
    /// <c>NOSUFFIXTRIE</c>, or <see langword="null"/> to omit the option.
    /// </summary>
    public TextField WithSuffixTrie(bool? include = true) { SuffixTrie = include; return this; }

    /// <inheritdoc/>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [(GlideString)Name];
        if (Alias is not null)
        {
            args.Add(ValkeyLiterals.AS);
            args.Add((GlideString)Alias.Value);
        }

        args.Add(FieldType.Text.ToLiteral());
        if (NoStem)
        {
            args.Add(ValkeyLiterals.NOSTEM);
        }

        if (Weight.HasValue)
        {
            args.Add(ValkeyLiterals.WEIGHT);
            args.Add(Weight.Value.ToString("G"));
        }

        if (SuffixTrie == true)
        {
            args.Add(ValkeyLiterals.WITHSUFFIXTRIE);
        }
        else if (SuffixTrie == false)
        {
            args.Add(ValkeyLiterals.NOSUFFIXTRIE);
        }

        if (Sortable)
        {
            args.Add(ValkeyLiterals.SORTABLE);
        }

        return [.. args];
    }
}

/// <summary>
/// Represents a tag field in an index schema.
/// </summary>
/// <remarks>
/// Use the fluent builder methods (<see cref="WithAlias"/>, <see cref="AsSortable"/>,
/// <see cref="WithSeparator"/>, <see cref="AsCaseSensitive"/>)
/// to configure the field, or set properties directly via object initializer.
/// </remarks>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public sealed class TagField(ValkeyValue name) : IField
{
    /// <summary>The field name.</summary>
    public ValkeyValue Name { get; } = name;

    /// <summary>Optional alias for the field.</summary>
    public ValkeyValue? Alias { get; set; }

    /// <summary>Whether the field is sortable.</summary>
    public bool Sortable { get; set; }

    /// <summary>The tag separator character.</summary>
    public string? Separator { get; set; }

    /// <summary>Preserves original letter cases of tags.</summary>
    public bool CaseSensitive { get; set; }

    // ── Fluent builder methods ────────────────────────────────────────────────

    /// <summary>Sets the optional alias for the field.</summary>
    public TagField WithAlias(ValkeyValue alias) { Alias = alias; return this; }

    /// <summary>Marks the field as sortable.</summary>
    public TagField AsSortable() { Sortable = true; return this; }

    /// <summary>Sets the tag separator character.</summary>
    public TagField WithSeparator(string separator) { Separator = separator; return this; }

    /// <summary>Preserves original letter cases of tags.</summary>
    public TagField AsCaseSensitive() { CaseSensitive = true; return this; }

    /// <inheritdoc/>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [(GlideString)Name];
        if (Alias is not null)
        {
            args.Add(ValkeyLiterals.AS);
            args.Add((GlideString)Alias.Value);
        }

        args.Add(FieldType.Tag.ToLiteral());
        if (Separator is not null)
        {
            args.Add(ValkeyLiterals.SEPARATOR);
            args.Add(Separator);
        }

        if (CaseSensitive)
        {
            args.Add(ValkeyLiterals.CASESENSITIVE);
        }

        if (Sortable)
        {
            args.Add(ValkeyLiterals.SORTABLE);
        }

        return [.. args];
    }
}

/// <summary>
/// Represents a numeric field in an index schema.
/// </summary>
/// <remarks>
/// Use the fluent builder methods (<see cref="WithAlias"/>, <see cref="AsSortable"/>)
/// to configure the field, or set properties directly via object initializer.
/// </remarks>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public sealed class NumericField(ValkeyValue name) : IField
{
    /// <summary>The field name.</summary>
    public ValkeyValue Name { get; } = name;

    /// <summary>Optional alias for the field.</summary>
    public ValkeyValue? Alias { get; set; }

    /// <summary>Whether the field is sortable.</summary>
    public bool Sortable { get; set; }

    // ── Fluent builder methods ────────────────────────────────────────────────

    /// <summary>Sets the optional alias for the field.</summary>
    public NumericField WithAlias(ValkeyValue alias) { Alias = alias; return this; }

    /// <summary>Marks the field as sortable.</summary>
    public NumericField AsSortable() { Sortable = true; return this; }

    /// <inheritdoc/>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [(GlideString)Name];
        if (Alias is not null)
        {
            args.Add(ValkeyLiterals.AS);
            args.Add((GlideString)Alias.Value);
        }

        args.Add(FieldType.Numeric.ToLiteral());
        if (Sortable)
        {
            args.Add(ValkeyLiterals.SORTABLE);
        }

        return [.. args];
    }
}

/// <summary>
/// Represents a vector field using the FLAT (brute force) algorithm.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public sealed class VectorFieldFlat(ValkeyValue name, DistanceMetric distanceMetric, int dimensions) : IField
{
    /// <summary>The field name.</summary>
    public ValkeyValue Name { get; } = name;

    /// <summary>Optional alias for the field.</summary>
    public ValkeyValue? Alias { get; set; }

    /// <summary>The distance metric used in vector similarity search.</summary>
    public DistanceMetric DistanceMetric { get; } = distanceMetric;

    /// <summary>The number of dimensions in the vector.</summary>
    public int Dimensions { get; } = dimensions;

    /// <summary>Initial vector capacity in the index affecting memory allocation size.</summary>
    public int? InitialCap { get; set; }

    /// <inheritdoc/>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [(GlideString)Name];
        if (Alias is not null)
        {
            args.Add(ValkeyLiterals.AS);
            args.Add((GlideString)Alias.Value);
        }

        args.Add(FieldType.Vector.ToLiteral());
        args.Add(VectorAlgorithm.Flat.ToLiteral());

        List<GlideString> attrs =
        [
            ValkeyLiterals.DIM, Dimensions.ToString(),
            ValkeyLiterals.DISTANCE_METRIC, DistanceMetric.ToLiteral(),
            ValkeyLiterals.TYPE, "FLOAT32", // Only one type currently supported; expose as a property when more are added
        ];
        if (InitialCap.HasValue)
        {
            attrs.Add(ValkeyLiterals.INITIAL_CAP);
            attrs.Add(InitialCap.Value.ToString());
        }

        args.Add(attrs.Count.ToString());
        args.AddRange(attrs);
        return [.. args];
    }
}

/// <summary>
/// Represents a vector field using the HNSW algorithm.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public sealed class VectorFieldHnsw(ValkeyValue name, DistanceMetric distanceMetric, int dimensions) : IField
{
    /// <summary>The field name.</summary>
    public ValkeyValue Name { get; } = name;

    /// <summary>Optional alias for the field.</summary>
    public ValkeyValue? Alias { get; set; }

    /// <summary>The distance metric used in vector similarity search.</summary>
    public DistanceMetric DistanceMetric { get; } = distanceMetric;

    /// <summary>The number of dimensions in the vector.</summary>
    public int Dimensions { get; } = dimensions;

    /// <summary>Initial vector capacity in the index affecting memory allocation size.</summary>
    public int? InitialCap { get; set; }

    /// <summary>Max number of outgoing edges per node per layer (M parameter).</summary>
    public int? NumberOfEdges { get; set; }

    /// <summary>Vectors examined during index construction (EF_CONSTRUCTION).</summary>
    public int? VectorsExaminedOnConstruction { get; set; }

    /// <summary>Vectors examined during query operations (EF_RUNTIME).</summary>
    public int? VectorsExaminedOnRuntime { get; set; }

    /// <inheritdoc/>
    public GlideString[] ToArgs()
    {
        List<GlideString> args = [(GlideString)Name];
        if (Alias is not null)
        {
            args.Add(ValkeyLiterals.AS);
            args.Add((GlideString)Alias.Value);
        }

        args.Add(FieldType.Vector.ToLiteral());
        args.Add(VectorAlgorithm.Hnsw.ToLiteral());

        List<GlideString> attrs =
        [
            ValkeyLiterals.DIM, Dimensions.ToString(),
            ValkeyLiterals.DISTANCE_METRIC, DistanceMetric.ToLiteral(),
            ValkeyLiterals.TYPE, "FLOAT32", // Only one type currently supported; expose as a property when more are added
        ];
        if (InitialCap.HasValue)
        {
            attrs.Add(ValkeyLiterals.INITIAL_CAP);
            attrs.Add(InitialCap.Value.ToString());
        }

        if (NumberOfEdges.HasValue)
        {
            attrs.Add(ValkeyLiterals.M);
            attrs.Add(NumberOfEdges.Value.ToString());
        }

        if (VectorsExaminedOnConstruction.HasValue)
        {
            attrs.Add(ValkeyLiterals.EF_CONSTRUCTION);
            attrs.Add(VectorsExaminedOnConstruction.Value.ToString());
        }

        if (VectorsExaminedOnRuntime.HasValue)
        {
            attrs.Add(ValkeyLiterals.EF_RUNTIME);
            attrs.Add(VectorsExaminedOnRuntime.Value.ToString());
        }

        args.Add(attrs.Count.ToString());
        args.AddRange(attrs);
        return [.. args];
    }
}

/// <summary>
/// Fluent builder for <see cref="FtCreateOptions"/>.
/// </summary>
/// <remarks>
/// The object-initializer style and the builder style are interchangeable — both
/// produce an identical <see cref="FtCreateOptions"/> instance.
/// <code>
/// // Builder style
/// FtCreateOptions options = new FtCreateOptionsBuilder()
///     .OnHash()
///     .WithPrefix("doc:")
///     .WithStopWords("the", "a")
///     .Build();
///
/// // Object-initializer style (equivalent)
/// FtCreateOptions options = new FtCreateOptions
/// {
///     DataType = IndexDataType.Hash,
///     Prefixes = ["doc:"],
///     StopWords = ["the", "a"],
/// };
/// </code>
/// </remarks>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public sealed class FtCreateOptionsBuilder
{
    private IndexDataType? _dataType;
    private readonly List<ValkeyValue> _prefixes = [];
    private double? _score;
    private string? _language;
    private bool _skipInitialScan;
    private int? _minStemSize;
    private bool? _offsets;
    private bool _noStopWords;
    private ValkeyValue[]? _stopWords;
    private ValkeyValue? _punctuation;

    /// <summary>Sets the index data type to HASH.</summary>
    public FtCreateOptionsBuilder OnHash() { _dataType = IndexDataType.Hash; return this; }

    /// <summary>Sets the index data type to JSON.</summary>
    public FtCreateOptionsBuilder OnJson() { _dataType = IndexDataType.Json; return this; }

    /// <summary>Sets the index data type.</summary>
    public FtCreateOptionsBuilder On(IndexDataType dataType) { _dataType = dataType; return this; }

    /// <summary>Adds a key prefix to index.</summary>
    public FtCreateOptionsBuilder WithPrefix(ValkeyValue prefix) { _prefixes.Add(prefix); return this; }

    /// <summary>Replaces the prefix list.</summary>
    public FtCreateOptionsBuilder WithPrefixes(IEnumerable<ValkeyValue> prefixes) { _prefixes.Clear(); _prefixes.AddRange(prefixes); return this; }

    /// <summary>Sets the default document score.</summary>
    public FtCreateOptionsBuilder WithScore(double score) { _score = score; return this; }

    /// <summary>Sets the default language for documents.</summary>
    public FtCreateOptionsBuilder WithLanguage(string language) { _language = language; return this; }

    /// <summary>Skips scanning and indexing existing documents on index creation.</summary>
    public FtCreateOptionsBuilder SkipInitialScan() { _skipInitialScan = true; return this; }

    /// <summary>Sets the minimum word length to stem.</summary>
    public FtCreateOptionsBuilder WithMinStemSize(int size) { _minStemSize = size; return this; }

    /// <summary>
    /// Controls term offset storage.
    /// Pass <see langword="true"/> for <c>WITHOFFSETS</c>, <see langword="false"/> for
    /// <c>NOOFFSETS</c>, or <see langword="null"/> to omit the option.
    /// </summary>
    public FtCreateOptionsBuilder WithOffsets(bool? include = true) { _offsets = include; return this; }

    /// <summary>Disables stop-word filtering. Clears any stop words set via <see cref="WithStopWords(string[])"/>.</summary>
    public FtCreateOptionsBuilder WithNoStopWords() { _noStopWords = true; _stopWords = null; return this; }

    /// <summary>
    /// Sets a custom stop-word list. Clears the <c>NOSTOPWORDS</c> flag if
    /// <see cref="WithNoStopWords"/> was previously called.
    /// </summary>
    public FtCreateOptionsBuilder WithStopWords(params ValkeyValue[] stopWords) { _noStopWords = false; _stopWords = stopWords; return this; }

    /// <summary>Sets custom punctuation characters for tokenization.</summary>
    public FtCreateOptionsBuilder WithPunctuation(ValkeyValue punctuation) { _punctuation = punctuation; return this; }

    /// <summary>
    /// Executes <paramref name="configure"/> only when <paramref name="condition"/> is
    /// <see langword="true"/>. Useful for conditionally including options without breaking the chain.
    /// </summary>
    public FtCreateOptionsBuilder When(bool condition, Func<FtCreateOptionsBuilder, FtCreateOptionsBuilder> configure)
        => condition ? configure(this) : this;

    /// <summary>Constructs the <see cref="FtCreateOptions"/> from the accumulated state.</summary>
    public FtCreateOptions Build() => new()
    {
        DataType = _dataType,
        Prefixes = _prefixes.Count > 0 ? [.. _prefixes] : null,
        Score = _score,
        Language = _language,
        SkipInitialScan = _skipInitialScan,
        MinStemSize = _minStemSize,
        Offsets = _offsets,
        NoStopWords = _noStopWords,
        StopWords = _stopWords,
        Punctuation = _punctuation,
    };
}

/// <summary>
/// Optional arguments for the FT.CREATE command.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public sealed class FtCreateOptions
{
    /// <summary>The index data type. If not set, a HASH index is created.</summary>
    public IndexDataType? DataType { get; init; }

    /// <summary>Key prefixes to index.</summary>
    public ValkeyValue[]? Prefixes { get; init; }

    /// <summary>Default score for documents in the index. Default is 1.0.</summary>
    public double? Score { get; init; }

    /// <summary>Default language for documents in the index.</summary>
    public string? Language { get; init; }

    /// <summary>Skips scanning and indexing existing documents on index creation.</summary>
    public bool SkipInitialScan { get; init; }

    /// <summary>Minimum word length to stem.</summary>
    public int? MinStemSize { get; init; }

    /// <summary>
    /// Controls term offset storage.
    /// <list type="bullet">
    ///   <item><description><see langword="true"/> — emit <c>WITHOFFSETS</c></description></item>
    ///   <item><description><see langword="false"/> — emit <c>NOOFFSETS</c></description></item>
    ///   <item><description><see langword="null"/> (default) — omit the option entirely</description></item>
    /// </list>
    /// </summary>
    public bool? Offsets { get; init; }

    /// <summary>Disables stop-word filtering. Mutually exclusive with <see cref="StopWords"/>.</summary>
    public bool NoStopWords { get; init; }

    /// <summary>Custom list of stop words. Mutually exclusive with <see cref="NoStopWords"/>.</summary>
    public ValkeyValue[]? StopWords { get; init; }

    /// <summary>Custom set of punctuation characters for tokenization.</summary>
    public ValkeyValue? Punctuation { get; init; }

    /// <summary>
    /// Returns the command arguments for these options.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        if (NoStopWords && StopWords is { Length: > 0 })
        {
            throw new ArgumentException("NoStopWords and StopWords are mutually exclusive.");
        }

        List<GlideString> args = [];
        if (DataType.HasValue)
        {
            args.Add(ValkeyLiterals.ON);
            args.Add(DataType.Value.ToLiteral());
        }

        if (Prefixes is { Length: > 0 })
        {
            args.Add(ValkeyLiterals.PREFIX);
            args.Add(Prefixes.Length.ToString());
            foreach (var p in Prefixes)
            {
                args.Add((GlideString)p);
            }
        }

        if (Score.HasValue)
        {
            args.Add(ValkeyLiterals.SCORE);
            args.Add(Score.Value.ToString("G"));
        }

        if (Language is not null)
        {
            args.Add(ValkeyLiterals.LANGUAGE);
            args.Add(Language);
        }

        if (SkipInitialScan)
        {
            args.Add(ValkeyLiterals.SKIPINITIALSCAN);
        }

        if (MinStemSize.HasValue)
        {
            args.Add(ValkeyLiterals.MINSTEMSIZE);
            args.Add(MinStemSize.Value.ToString());
        }

        if (Offsets == true)
        {
            args.Add(ValkeyLiterals.WITHOFFSETS);
        }
        else if (Offsets == false)
        {
            args.Add(ValkeyLiterals.NOOFFSETS);
        }

        if (NoStopWords)
        {
            args.Add(ValkeyLiterals.NOSTOPWORDS);
        }
        else if (StopWords is { Length: > 0 })
        {
            args.Add(ValkeyLiterals.STOPWORDS);
            args.Add(StopWords.Length.ToString());
            foreach (var sw in StopWords)
            {
                args.Add((GlideString)sw);
            }
        }

        if (Punctuation is not null)
        {
            args.Add(ValkeyLiterals.PUNCTUATION);
            args.Add((GlideString)Punctuation.Value);
        }

        return [.. args];
    }
}
