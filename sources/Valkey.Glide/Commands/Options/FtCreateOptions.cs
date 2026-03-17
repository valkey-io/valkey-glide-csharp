// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Constants.Constants;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Represents the data type of a field in a vector search index schema.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum FieldType
{
    /// <summary>Full-text search field.</summary>
    Text,
    /// <summary>Tag field (delimited list of tags).</summary>
    Tag,
    /// <summary>Numeric field.</summary>
    Numeric,
    /// <summary>Vector field for similarity search.</summary>
    Vector,
}

/// <summary>
/// Represents the algorithm used for vector similarity search.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum VectorAlgorithm
{
    /// <summary>Brute force (flat) algorithm.</summary>
    FLAT,
    /// <summary>Hierarchical Navigable Small World algorithm.</summary>
    HNSW,
}

/// <summary>
/// Represents the distance metric used to measure similarity between vectors.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum DistanceMetric
{
    /// <summary>Euclidean distance.</summary>
    L2,
    /// <summary>Inner product.</summary>
    IP,
    /// <summary>Cosine distance.</summary>
    COSINE,
}

/// <summary>
/// Represents the vector data type for vector fields.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum VectorDataType
{
    /// <summary>32-bit floating point (default).</summary>
    FLOAT32,
}

/// <summary>
/// Represents the type of the index dataset.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public enum IndexDataType
{
    /// <summary>Data stored in hashes; field identifiers are field names within the hashes.</summary>
    HASH,
    /// <summary>Data stored as JSON documents; field identifiers are JSON Path expressions.</summary>
    JSON,
}

/// <summary>
/// Base interface for all index schema field types used in FT.CREATE.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public interface IField
{
    /// <summary>
    /// Returns the command arguments for this field definition.
    /// </summary>
    string[] ToArgs();
}

/// <summary>
/// Represents a full-text search field in an index schema.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public class TextField(string name) : IField
{
    /// <summary>The field name.</summary>
    public string Name { get; } = name;
    /// <summary>Optional alias for the field.</summary>
    public string? Alias { get; set; }
    /// <summary>Whether the field is sortable.</summary>
    public bool Sortable { get; set; }
    /// <summary>Disables stemming when indexing the field.</summary>
    public bool NoStem { get; set; }
    /// <summary>The importance of this field when calculating result accuracy.</summary>
    public double? Weight { get; set; }
    /// <summary>Keeps a suffix trie for the field. Mutually exclusive with <see cref="NoSuffixTrie"/>.</summary>
    public bool WithSuffixTrie { get; set; }
    /// <summary>Disables the suffix trie for the field. Mutually exclusive with <see cref="WithSuffixTrie"/>.</summary>
    public bool NoSuffixTrie { get; set; }

    /// <inheritdoc/>
    public string[] ToArgs()
    {
        if (WithSuffixTrie && NoSuffixTrie)
            throw new ArgumentException("WithSuffixTrie and NoSuffixTrie are mutually exclusive.");

        List<string> args = [Name];
        if (Alias is not null) { args.Add(AsKeyword); args.Add(Alias); }
        args.Add(nameof(FieldType.Text).ToUpper());
        if (NoStem) args.Add(NoStemKeyword);
        if (Weight.HasValue) { args.Add(WeightKeyword); args.Add(Weight.Value.ToString("G")); }
        if (WithSuffixTrie) args.Add(WithSuffixTrieKeyword);
        else if (NoSuffixTrie) args.Add(NoSuffixTrieKeyword);
        if (Sortable) args.Add(SortableKeyword);
        return [.. args];
    }
}

/// <summary>
/// Represents a tag field in an index schema.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public class TagField(string name) : IField
{
    /// <summary>The field name.</summary>
    public string Name { get; } = name;
    /// <summary>Optional alias for the field.</summary>
    public string? Alias { get; set; }
    /// <summary>Whether the field is sortable.</summary>
    public bool Sortable { get; set; }
    /// <summary>The tag separator character.</summary>
    public string? Separator { get; set; }
    /// <summary>Preserves original letter cases of tags.</summary>
    public bool CaseSensitive { get; set; }

    /// <inheritdoc/>
    public string[] ToArgs()
    {
        List<string> args = [Name];
        if (Alias is not null) { args.Add(AsKeyword); args.Add(Alias); }
        args.Add(nameof(FieldType.Tag).ToUpper());
        if (Separator is not null) { args.Add(SeparatorKeyword); args.Add(Separator); }
        if (CaseSensitive) args.Add(CaseSensitiveKeyword);
        if (Sortable) args.Add(SortableKeyword);
        return [.. args];
    }
}

/// <summary>
/// Represents a numeric field in an index schema.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public class NumericField(string name) : IField
{
    /// <summary>The field name.</summary>
    public string Name { get; } = name;
    /// <summary>Optional alias for the field.</summary>
    public string? Alias { get; set; }
    /// <summary>Whether the field is sortable.</summary>
    public bool Sortable { get; set; }

    /// <inheritdoc/>
    public string[] ToArgs()
    {
        List<string> args = [Name];
        if (Alias is not null) { args.Add(AsKeyword); args.Add(Alias); }
        args.Add(nameof(FieldType.Numeric).ToUpper());
        if (Sortable) args.Add(SortableKeyword);
        return [.. args];
    }
}

/// <summary>
/// Represents a vector field using the FLAT (brute force) algorithm.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public class VectorFieldFlat(string name, DistanceMetric distanceMetric, int dimensions) : IField
{
    /// <summary>The field name.</summary>
    public string Name { get; } = name;
    /// <summary>Optional alias for the field.</summary>
    public string? Alias { get; set; }
    /// <summary>The distance metric used in vector similarity search.</summary>
    public DistanceMetric DistanceMetric { get; } = distanceMetric;
    /// <summary>The number of dimensions in the vector.</summary>
    public int Dimensions { get; } = dimensions;
    /// <summary>The vector data type. Defaults to FLOAT32.</summary>
    public VectorDataType Type { get; set; } = VectorDataType.FLOAT32;
    /// <summary>Initial vector capacity in the index affecting memory allocation size.</summary>
    public int? InitialCap { get; set; }

    /// <inheritdoc/>
    public string[] ToArgs()
    {
        List<string> args = [Name];
        if (Alias is not null) { args.Add(AsKeyword); args.Add(Alias); }
        args.Add("VECTOR");
        args.Add(nameof(VectorAlgorithm.FLAT));

        List<string> attrs =
        [
            "DIM", Dimensions.ToString(),
            "DISTANCE_METRIC", DistanceMetric.ToString(),
            "TYPE", Type.ToString(),
        ];
        if (InitialCap.HasValue) { attrs.Add("INITIAL_CAP"); attrs.Add(InitialCap.Value.ToString()); }

        args.Add(attrs.Count.ToString());
        args.AddRange(attrs);
        return [.. args];
    }
}

/// <summary>
/// Represents a vector field using the HNSW algorithm.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public class VectorFieldHnsw(string name, DistanceMetric distanceMetric, int dimensions) : IField
{
    /// <summary>The field name.</summary>
    public string Name { get; } = name;
    /// <summary>Optional alias for the field.</summary>
    public string? Alias { get; set; }
    /// <summary>The distance metric used in vector similarity search.</summary>
    public DistanceMetric DistanceMetric { get; } = distanceMetric;
    /// <summary>The number of dimensions in the vector.</summary>
    public int Dimensions { get; } = dimensions;
    /// <summary>The vector data type. Defaults to FLOAT32.</summary>
    public VectorDataType Type { get; set; } = VectorDataType.FLOAT32;
    /// <summary>Initial vector capacity in the index affecting memory allocation size.</summary>
    public int? InitialCap { get; set; }
    /// <summary>Max number of outgoing edges per node per layer (M parameter). Default 16, max 512.</summary>
    public int? NumberOfEdges { get; set; }
    /// <summary>Vectors examined during index construction (EF_CONSTRUCTION). Default 200, max 4096.</summary>
    public int? VectorsExaminedOnConstruction { get; set; }
    /// <summary>Vectors examined during query operations (EF_RUNTIME). Default 10, max 4096.</summary>
    public int? VectorsExaminedOnRuntime { get; set; }

    /// <inheritdoc/>
    public string[] ToArgs()
    {
        List<string> args = [Name];
        if (Alias is not null) { args.Add(AsKeyword); args.Add(Alias); }
        args.Add("VECTOR");
        args.Add(nameof(VectorAlgorithm.HNSW));

        List<string> attrs =
        [
            "DIM", Dimensions.ToString(),
            "DISTANCE_METRIC", DistanceMetric.ToString(),
            "TYPE", Type.ToString(),
        ];
        if (InitialCap.HasValue) { attrs.Add("INITIAL_CAP"); attrs.Add(InitialCap.Value.ToString()); }
        if (NumberOfEdges.HasValue) { attrs.Add("M"); attrs.Add(NumberOfEdges.Value.ToString()); }
        if (VectorsExaminedOnConstruction.HasValue) { attrs.Add("EF_CONSTRUCTION"); attrs.Add(VectorsExaminedOnConstruction.Value.ToString()); }
        if (VectorsExaminedOnRuntime.HasValue) { attrs.Add("EF_RUNTIME"); attrs.Add(VectorsExaminedOnRuntime.Value.ToString()); }

        args.Add(attrs.Count.ToString());
        args.AddRange(attrs);
        return [.. args];
    }
}

/// <summary>
/// Optional arguments for the FT.CREATE command.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.create/">valkey.io</seealso>
public class FtCreateOptions
{
    /// <summary>The index data type. If not set, a HASH index is created.</summary>
    public IndexDataType? DataType { get; set; }
    /// <summary>Key prefixes to index.</summary>
    public string[]? Prefixes { get; set; }
    /// <summary>Default score for documents in the index. Default is 1.0.</summary>
    public double? Score { get; set; }
    /// <summary>Default language for documents in the index.</summary>
    public string? Language { get; set; }
    /// <summary>Skips scanning and indexing existing documents on index creation.</summary>
    public bool SkipInitialScan { get; set; }
    /// <summary>Minimum word length to stem.</summary>
    public int? MinStemSize { get; set; }
    /// <summary>Stores term offsets. Mutually exclusive with <see cref="NoOffsets"/>.</summary>
    public bool WithOffsets { get; set; }
    /// <summary>Does not store term offsets. Mutually exclusive with <see cref="WithOffsets"/>.</summary>
    public bool NoOffsets { get; set; }
    /// <summary>Disables stop-word filtering. Mutually exclusive with <see cref="StopWords"/>.</summary>
    public bool NoStopWords { get; set; }
    /// <summary>Custom list of stop words. Mutually exclusive with <see cref="NoStopWords"/>.</summary>
    public string[]? StopWords { get; set; }
    /// <summary>Custom set of punctuation characters for tokenization.</summary>
    public string? Punctuation { get; set; }

    /// <summary>
    /// Returns the command arguments for these options.
    /// </summary>
    public string[] ToArgs()
    {
        if (WithOffsets && NoOffsets)
            throw new ArgumentException("WithOffsets and NoOffsets are mutually exclusive.");
        if (NoStopWords && StopWords is { Length: > 0 })
            throw new ArgumentException("NoStopWords and StopWords are mutually exclusive.");

        List<string> args = [];
        if (DataType.HasValue) { args.Add(OnKeyword); args.Add(DataType.Value.ToString().ToUpper()); }
        if (Prefixes is { Length: > 0 }) { args.Add(PrefixKeyword); args.Add(Prefixes.Length.ToString()); args.AddRange(Prefixes); }
        if (Score.HasValue) { args.Add(ScoreKeyword); args.Add(Score.Value.ToString("G")); }
        if (Language is not null) { args.Add(LanguageKeyword); args.Add(Language); }
        if (SkipInitialScan) args.Add(SkipInitialScanKeyword);
        if (MinStemSize.HasValue) { args.Add(MinStemSizeKeyword); args.Add(MinStemSize.Value.ToString()); }
        if (WithOffsets) args.Add(WithOffsetsKeyword);
        else if (NoOffsets) args.Add(NoOffsetsKeyword);
        if (NoStopWords) args.Add(NoStopWordsKeyword);
        else if (StopWords is { Length: > 0 }) { args.Add(StopWordsKeyword); args.Add(StopWords.Length.ToString()); args.AddRange(StopWords); }
        if (Punctuation is not null) { args.Add(PunctuationKeyword); args.Add(Punctuation); }
        return [.. args];
    }
}
