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

    internal abstract string Method { get; }

    internal abstract string Threshold { get; }

    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [(GlideString)Method];

        if (Exact == true)
            args.Add((GlideString)"=");
        else if (Exact == false)
            args.Add((GlideString)"~");

        args.Add((GlideString)Threshold);

        if (Limit.HasValue)
        {
            args.Add((GlideString)"LIMIT");
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

        internal override string Method => "MAXLEN";

        internal override string Threshold => MaxLength.ToString();
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

        internal override string Method => "MINID";

        internal override string Threshold => MinEntryId.ToString();
    }
}
