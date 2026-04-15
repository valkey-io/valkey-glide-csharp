// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Trimming options for stream commands (XADD, XTRIM).
/// </summary>
/// <seealso href="https://valkey.io/commands/xadd/"/>
/// <seealso href="https://valkey.io/commands/xtrim/"/>
public abstract class StreamTrimOptions
{
    /// <summary>
    /// If <c>true</c>, trims exactly. If <c>false</c>, trims approximately (~) for better performance.
    /// If <c>null</c>, the server default is used.
    /// </summary>
    public bool? Exact { get; init; }

    /// <summary>
    /// Maximum number of entries to trim per operation. Only applicable when <see cref="Exact"/> is <c>false</c>.
    /// </summary>
    public long? Limit { get; init; }

    internal abstract GlideString Method { get; }

    internal abstract GlideString Threshold { get; }

    internal GlideString[] ToArgs()
    {
        if (Limit.HasValue && Exact == true)
            throw new ArgumentException("LIMIT cannot be used with exact trimming.", nameof(Limit));

        List<GlideString> args = [Method];

        if (Exact == true)
            args.Add(ValkeyLiterals.ExactTrim.ToGlideString());
        else if (Exact == false)
            args.Add(ValkeyLiterals.ApproxTrim.ToGlideString());

        args.Add(Threshold);

        if (Limit.HasValue)
        {
            args.Add(ValkeyLiterals.LIMIT.ToGlideString());
            args.Add(Limit.Value.ToGlideString());
        }

        return [.. args];
    }

    /// <summary>
    /// Trim the stream to a maximum length.
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
        /// The minimum entry ID to keep. Entries with lower IDs are trimmed.
        /// </summary>
        public required ValkeyValue MinEntryId { get; init; }

        internal override GlideString Method => ValkeyLiterals.MINID.ToGlideString();

        internal override GlideString Threshold => MinEntryId.ToGlideString();
    }
}
