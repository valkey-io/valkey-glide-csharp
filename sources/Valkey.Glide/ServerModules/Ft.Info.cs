// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.ServerModules;

public static partial class Ft
{
    #region Public Methods

    /// <summary>
    /// Returns information about a search index from the local node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index.</param>
    /// <returns>An <see cref="InfoLocalResult"/> for the local node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "index", new Ft.CreateTextField("title"));
    ///
    /// var info = await Ft.InfoLocalAsync(client, "index");
    /// Console.WriteLine($"Index name: {info.IndexName}");  // "index"
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<InfoLocalResult> InfoLocalAsync(BaseClient client, ValkeyKey index)
        => client.Command(Request.FtInfoLocal(index));

    /// <summary>
    /// Returns information about a search index from the local node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
    /// <param name="client">The client to execute the command.</param>
    /// <param name="index">The name of the index.</param>
    /// <param name="options">Additional options for the info command.</param>
    /// <returns>An <see cref="InfoLocalResult"/> for the local node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(client, "index", new Ft.CreateTextField("title"));
    ///
    /// var options = new Ft.InfoOptions { SomeShards = true };
    /// var info = await Ft.InfoLocalAsync(client, "index", options);
    /// Console.WriteLine($"Index name: {info.IndexName}");  // "index"
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<InfoLocalResult> InfoLocalAsync(BaseClient client, ValkeyKey index, InfoOptions options)
        => client.Command(Request.FtInfoLocal(index, options));

    /// <summary>
    /// Returns information about a search index from all cluster nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
    /// <param name="client">The cluster client to execute the command.</param>
    /// <param name="index">The name of the index.</param>
    /// <returns>An <see cref="InfoClusterResult"/> for all cluster nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(clusterClient, "index", new Ft.CreateTextField("title"));
    ///
    /// var info = await Ft.InfoClusterAsync(clusterClient, "index");
    /// Console.WriteLine($"Index name: {info.IndexName}");  // "index"
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<InfoClusterResult> InfoClusterAsync(GlideClusterClient client, ValkeyKey index)
        => client.Command(Request.FtInfoCluster(index));

    /// <summary>
    /// Returns information about a search index from all cluster nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
    /// <param name="client">The cluster client to execute the command.</param>
    /// <param name="index">The name of the index.</param>
    /// <param name="options">Additional options for the info command.</param>
    /// <returns>An <see cref="InfoClusterResult"/> for all cluster nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(clusterClient, "index", new Ft.CreateTextField("title"));
    ///
    /// var options = new Ft.InfoOptions { SomeShards = true };
    /// var info = await Ft.InfoClusterAsync(clusterClient, "index", options);
    /// Console.WriteLine($"Index name: {info.IndexName}");  // "index"
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<InfoClusterResult> InfoClusterAsync(GlideClusterClient client, ValkeyKey index, InfoOptions options)
        => client.Command(Request.FtInfoCluster(index, options));

    /// <summary>
    /// Returns information about a search index from all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
    /// <param name="client">The cluster client to execute the command.</param>
    /// <param name="index">The name of the index.</param>
    /// <returns>An <see cref="InfoPrimaryResult"/> for all primary nodes</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(clusterClient, "index", new Ft.CreateTextField("title"));
    ///
    /// var info = await Ft.InfoPrimaryAsync(clusterClient, "index");
    /// Console.WriteLine($"Index name: {info.IndexName}, Docs: {info.NumDocs}");
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<InfoPrimaryResult> InfoPrimaryAsync(GlideClusterClient client, ValkeyKey index)
        => client.Command(Request.FtInfoPrimary(index));

    /// <summary>
    /// Returns information about a search index from all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
    /// <param name="client">The cluster client to execute the command.</param>
    /// <param name="index">The name of the index.</param>
    /// <param name="options">Additional options for the info command.</param>
    /// <returns>An <see cref="InfoPrimaryResult"/> for all primary nodes</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await Ft.CreateAsync(clusterClient, "index", new Ft.CreateTextField("title"));
    ///
    /// var options = new Ft.InfoOptions { SomeShards = true };
    /// var info = await Ft.InfoPrimaryAsync(clusterClient, "index", options);
    /// Console.WriteLine($"Index name: {info.IndexName}, Docs: {info.NumDocs}");
    /// </code>
    /// </example>
    /// </remarks>
    public static Task<InfoPrimaryResult> InfoPrimaryAsync(GlideClusterClient client, ValkeyKey index, InfoOptions options)
        => client.Command(Request.FtInfoPrimary(index, options));

    #endregion

    #region Nested Types

    /// <summary>
    /// The options for an operation to retrieve information about a search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
    public sealed class InfoOptions
    {
        /// <summary>
        /// Whether to allow partial results from available shards (<c>SOMESHARDS</c>).
        /// </summary>
        public bool SomeShards { get; init; }

        /// <summary>
        /// Whether to allow inconsistent results across shards (<c>INCONSISTENT</c>).
        /// </summary>
        public bool Inconsistent { get; init; }
    }

    /// <summary>
    /// The state of a search index.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ft.info/">Valkey commands – FT.INFO</seealso>
    public enum InfoState
    {
        /// <summary>
        /// The index is ready (<c>ready</c>).
        /// </summary>
        Ready,

        /// <summary>
        /// The index is backfilling existing data (<c>backfill_in_progress</c>).
        /// </summary>
        BackfillInProgress,

        /// <summary>
        /// The index backfill has been paused due to an out-of-memory condition (<c>backfill_paused_by_oom</c>).
        /// </summary>
        BackfillPausedByOom,
    }

    /// <summary>
    /// Abstract base class for search index information returned by <c>FT.INFO</c>.
    /// </summary>
    public abstract class InfoResult
    {
        /// <summary>
        /// The name of the index (<c>index_name</c>)
        /// </summary>
        public required string IndexName { get; init; }

        internal InfoResult() { }
    }

    /// <summary>
    /// Information about a search index from the local node.
    /// </summary>
    public sealed class InfoLocalResult : InfoResult
    {
        /// <summary>
        /// The data structure type for the index (<c>index_definition.key_type</c>).
        /// </summary>
        public required DataType KeyType { get; init; }

        /// <summary>
        /// The key prefixes included in the index (<c>index_definition.prefixes</c>).
        /// </summary>
        public required ValkeyValue[] Prefixes { get; init; }

        /// <summary>
        /// The field attributes defined in the index schema (<c>attributes</c>).
        /// </summary>
        public required InfoField[] Attributes { get; init; }

        /// <summary>
        /// The number of documents in the index (<c>num_docs</c>).
        /// </summary>
        public required long NumDocs { get; init; }

        /// <summary>
        /// The number of records in the index (<c>num_records</c>).
        /// </summary>
        public required long NumRecords { get; init; }

        /// <summary>
        /// The total number of term occurrences across all documents (<c>total_term_occurrences</c>).
        /// </summary>
        public required long TotalTermOccurrences { get; init; }

        /// <summary>
        /// The number of distinct terms in the index (<c>num_terms</c>).
        /// </summary>
        public required long NumTerms { get; init; }

        /// <summary>
        /// The number of hash indexing failures (<c>hash_indexing_failures</c>).
        /// </summary>
        public required long HashIndexingFailures { get; init; }

        /// <summary>
        /// Whether a backfill operation is currently in progress (<c>backfill_in_progress</c>).
        /// </summary>
        public required bool BackfillInProgress { get; init; }

        /// <summary>
        /// The percentage of backfill completion, from 0.0 to 1.0 (<c>backfill_complete_percent</c>).
        /// </summary>
        public required double BackfillCompletePercent { get; init; }

        /// <summary>
        /// The current size of the mutation queue (<c>mutation_queue_size</c>).
        /// </summary>
        public required long MutationQueueSize { get; init; }

        /// <summary>
        /// The recent delay of the mutation queue in seconds (<c>recent_mutations_queue_delay</c>).
        /// </summary>
        public required double RecentMutationsQueueDelay { get; init; }

        /// <summary>
        /// The current state of the index (<c>state</c>).
        /// </summary>
        public required InfoState State { get; init; }

        /// <summary>
        /// The punctuation characters used for tokenization (<c>punctuation</c>).
        /// </summary>
        public required ValkeyValue Punctuation { get; init; }

        /// <summary>
        /// The stop words configured for the index (<c>stopwords</c>).
        /// </summary>
        public required ValkeyValue[] StopWords { get; init; }

        /// <summary>
        /// Whether term offsets are stored in the index (<c>with_offsets</c>).
        /// </summary>
        public required bool WithOffsets { get; init; }

        /// <summary>
        /// The minimum word length for stemming (<c>min_stem_size</c>).
        /// </summary>
        public required long MinStemSize { get; init; }

        internal InfoLocalResult() { }
    }

    /// <summary>
    /// Information about a search index from all cluster nodes.
    /// </summary>
    public sealed class InfoClusterResult : InfoResult
    {
        /// <summary>
        /// Whether a backfill operation is currently in progress on any node (<c>backfill_in_progress</c>).
        /// </summary>
        public required bool BackfillInProgress { get; init; }

        /// <summary>
        /// The minimum backfill completion percentage across all nodes (<c>backfill_complete_percent_min</c>).
        /// </summary>
        public required double BackfillCompletePercentMin { get; init; }

        /// <summary>
        /// The maximum backfill completion percentage across all nodes (<c>backfill_complete_percent_max</c>).
        /// </summary>
        public required double BackfillCompletePercentMax { get; init; }

        /// <summary>
        /// The current state of the index across the cluster (<c>state</c>).
        /// </summary>
        public required InfoState State { get; init; }

        internal InfoClusterResult() { }
    }

    /// <summary>
    /// Information about a search index from all primary nodes.
    /// </summary>
    public sealed class InfoPrimaryResult : InfoResult
    {
        /// <summary>
        /// The total number of documents across all primary nodes (<c>num_docs</c>).
        /// </summary>
        public required long NumDocs { get; init; }

        /// <summary>
        /// The total number of records across all primary nodes (<c>num_records</c>).
        /// </summary>
        public required long NumRecords { get; init; }

        /// <summary>
        /// The total number of hash indexing failures across all primary nodes (<c>hash_indexing_failures</c>).
        /// </summary>
        public required long HashIndexingFailures { get; init; }

        internal InfoPrimaryResult() { }
    }

    /// <summary>
    /// Abstract base class for information about a field in a search index.
    /// </summary>
    public abstract class InfoField
    {
        /// <summary>
        /// The field identifier: hash field name or JSON path (<c>identifier</c>).
        /// </summary>
        public required ValkeyValue Identifier { get; init; }

        /// <summary>
        /// The field attribute name used in queries (<c>attribute</c>).
        /// </summary>
        public required ValkeyValue Attribute { get; init; }

        /// <summary>
        /// The memory used by user-indexed data for this field in bytes (<c>user_indexed_memory</c>).
        /// </summary>
        public required long UserIndexedMemory { get; init; }

        internal InfoField() { }
    }

    /// <summary>
    /// Information about a text field in a search index (<c>TEXT</c>).
    /// </summary>
    public sealed class InfoTextField : InfoField
    {
        /// <summary>
        /// Whether the suffix trie optimization is enabled for this field (<c>WITH_SUFFIX_TRIE</c>).
        /// </summary>
        public required bool WithSuffixTrie { get; init; }

        /// <summary>
        /// Whether stemming is disabled for this field (<c>NO_STEM</c>).
        /// </summary>
        public required bool NoStem { get; init; }

        internal InfoTextField() { }
    }

    /// <summary>
    /// Information about a tag field in a search index (<c>TAG</c>).
    /// </summary>
    public sealed class InfoTagField : InfoField
    {
        /// <summary>
        /// The separator character used for splitting tag values (<c>SEPARATOR</c>).
        /// </summary>
        public required char Separator { get; init; }

        /// <summary>
        /// Whether tag comparisons are case-sensitive (<c>CASESENSITIVE</c>).
        /// </summary>
        public required bool CaseSensitive { get; init; }

        /// <summary>
        /// The number of keys that have this tag attribute present (<c>SIZE</c>).
        /// </summary>
        public required long Size { get; init; }

        internal InfoTagField() { }
    }

    /// <summary>
    /// Information about a numeric field in a search index (<c>NUMERIC</c>).
    /// </summary>
    public sealed class InfoNumericField : InfoField
    {
        internal InfoNumericField() { }
    }

    /// <summary>
    /// Abstract base class for information about a vector field in a search index.
    /// </summary>
    public abstract class InfoVectorField : InfoField
    {
        /// <summary>
        /// The current capacity of the vector index (<c>index.capacity</c>).
        /// </summary>
        public required long Capacity { get; init; }

        /// <summary>
        /// The number of dimensions in the vector (<c>index.dimensions</c>).
        /// </summary>
        public required long Dimensions { get; init; }

        /// <summary>
        /// The distance metric used for vector similarity (<c>index.distance_metric</c>).
        /// </summary>
        public required DistanceMetric DistanceMetric { get; init; }

        /// <summary>
        /// The number of vectors currently stored in the index (<c>index.size</c>).
        /// </summary>
        public required long Size { get; init; }

        internal InfoVectorField() { }
    }

    /// <summary>
    /// Information about a vector field in a search index that uses the brute-force algorithm algorithm (<c>HNSW</c>).
    /// </summary>
    public sealed class InfoVectorFieldFlat : InfoVectorField
    {
        /// <summary>
        /// The block size for the FLAT index (<c>index.algorithm.block_size</c>).
        /// </summary>
        public long? BlockSize { get; init; }

        internal InfoVectorFieldFlat() { }
    }

    /// <summary>
    /// Information about a vector field in a search index that uses the Hierarchical Navigable Small World algorithm (<c>HNSW</c>).
    /// </summary>
    public sealed class InfoVectorFieldHnsw : InfoVectorField
    {
        /// <summary>
        /// The maximum number of outgoing edges per graph node (<c>index.algorithm.m</c>).
        /// </summary>
        public long? M { get; init; }

        /// <summary>
        /// The number of vectors examined during index construction (<c>index.algorithm.ef_construction</c>).
        /// </summary>
        public long? EfConstruction { get; init; }

        /// <summary>
        /// The number of vectors examined during queries (<c>index.algorithm.ef_runtime</c>).
        /// </summary>
        public long? EfRuntime { get; init; }

        internal InfoVectorFieldHnsw() { }
    }

    #endregion
}
