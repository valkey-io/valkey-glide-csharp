// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Trimming options for stream commands (XADD, XTRIM).
/// </summary>
/// <seealso href="https://valkey.io/commands/xadd/"/>
/// <seealso href="https://valkey.io/commands/xtrim/"/>
public abstract class StreamTrimOptions
{
    #region Public Properties

    /// <summary>
    /// If <see langword="true"/>, trims exactly. If <see langword="false"/>, trims approximately (~) for better performance.
    /// If <see langword="null"/>, the server default is used.
    /// </summary>
    public bool? Exact { get; init; } = null;

    /// <summary>
    /// Maximum number of entries to trim per operation. Only applicable when <see cref="Exact"/> is <see langword="false"/>.
    /// </summary>
    public long? Limit { get; init; } = null;

    /// <summary>
    /// Controls how consumer group PEL references are handled for trimmed entries.
    /// Requires Valkey 8.2 or later when set to a non-default value.
    /// </summary>
    public StreamTrimMode Mode { get; init; } = StreamTrimMode.KeepReferences;

    #endregion
    #region Internal Methods

    /// <summary>
    /// The trimming method keyword (e.g., MAXLEN or MINID).
    /// </summary>
    internal abstract GlideString Method { get; }

    /// <summary>
    /// The trimming threshold value (e.g., the max length or the minimum entry ID).
    /// </summary>
    internal abstract GlideString Threshold { get; }

    /// <inheritdoc/>
    internal GlideString[] ToArgs()
    {
        if (Limit.HasValue && Exact != false)
        {
            throw new ArgumentException("Limit can only be used when Exact is false.", nameof(Limit));
        }

        List<GlideString> args = [Method];

        if (Exact.HasValue)
        {
            args.Add(Exact.Value
                ? ValkeyLiterals.StreamExactTrim
                : ValkeyLiterals.StreamApproxTrim);
        }

        args.Add(Threshold);

        if (Limit.HasValue)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add(Limit.Value.ToGlideString());
        }

        GlideString? trimModeKeyword = Mode.ToKeyword();
        if (trimModeKeyword is not null)
        {
            args.Add(trimModeKeyword);
        }

        return [.. args];
    }

    #endregion

    /// <summary>
    /// Trim the stream to a maximum number of entries.
    /// </summary>
    public sealed class MaxLen : StreamTrimOptions
    {
        /// <summary>
        /// The maximum number of entries to keep.
        /// </summary>
        public required long MaxLength { get; init; }

        internal override GlideString Method => ValkeyLiterals.MAXLEN.ToGlideString();
        internal override GlideString Threshold => MaxLength.ToGlideString();
    }

    /// <summary>
    /// Trim entries with IDs lower than the specified threshold.
    /// </summary>
    public sealed class MinId : StreamTrimOptions
    {
        /// <summary>
        /// The minimum entry ID to keep. Entries with IDs lower than this value are trimmed.
        /// </summary>
        public required ValkeyValue MinEntryId { get; init; }

        internal override GlideString Method => ValkeyLiterals.MINID.ToGlideString();
        internal override GlideString Threshold => MinEntryId.ToGlideString();
    }
}
