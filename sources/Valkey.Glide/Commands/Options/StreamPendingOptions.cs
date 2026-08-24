// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the <c>XPENDING</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xpending/"/>
public sealed class StreamPendingOptions
{
    #region Public Properties

    /// <summary>
    /// The maximum number of messages to return.
    /// </summary>
    public required int Count { get; init; }

    /// <summary>
    /// The start of the ID range to query.
    /// </summary>
    public StreamIdBound Start { get; init; } = StreamIdBound.Min;

    /// <summary>
    /// The end of the ID range to query.
    /// </summary>
    public StreamIdBound End { get; init; } = StreamIdBound.Max;

    /// <summary>
    /// If specified, restricts the results to pending entries owned by a single consumer.
    /// </summary>
    public ValkeyValue ConsumerName { get; init; } = ValkeyValue.Null;

    /// <summary>
    /// If specified, restricts the results to entries idle for at least this long (IDLE).
    /// </summary>
    public TimeSpan? MinIdleTime { get; init; } = null;

    #endregion
    #region Internal Methods

    /// <summary>
    /// Builds the command arguments for these options.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (MinIdleTime.HasValue)
        {
            args.Add(ValkeyLiterals.IDLE);
            args.Add(ToULongMs(MinIdleTime.Value, nameof(MinIdleTime)).ToGlideString());
        }

        args.Add(Start.Value);
        args.Add(End.Value);
        args.Add(Count.ToGlideString());

        if (!ConsumerName.IsNull)
        {
            args.Add(ConsumerName);
        }

        return [.. args];
    }

    #endregion
}
