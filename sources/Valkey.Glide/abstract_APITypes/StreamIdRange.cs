// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A stream entry ID range for XRANGE and XREVRANGE queries.
/// </summary>
/// <seealso href="https://valkey.io/commands/xrange/"/>
/// <seealso href="https://valkey.io/commands/xrevrange/"/>
public sealed class StreamIdRange
{
    #region Constants

    /// <summary>
    /// A range spanning all stream entries (minimum to maximum).
    /// </summary>
    public static readonly StreamIdRange All = new(StreamIdBound.Min, StreamIdBound.Max);

    #endregion
    #region Public Properties

    /// <summary>
    /// The start bound of the range.
    /// </summary>
    public StreamIdBound Start { get; }

    /// <summary>
    /// The end bound of the range.
    /// </summary>
    public StreamIdBound End { get; }

    #endregion
    #region Constructors

    private StreamIdRange(StreamIdBound start, StreamIdBound end)
    {
        Start = start;
        End = end;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a range from the given start ID to the maximum stream ID.
    /// </summary>
    /// <param name="start">The start stream ID bound.</param>
    /// <returns>A <see cref="StreamIdRange"/> from <paramref name="start"/> to the maximum stream ID.</returns>
    public static StreamIdRange From(StreamIdBound start)
        => new(start, StreamIdBound.Max);

    /// <inheritdoc cref="From(StreamIdBound)"/>
    public static StreamIdRange From(ValkeyValue start)
        => From(new StreamIdBound(start));

    /// <summary>
    /// Creates a range from the minimum stream ID to the given end ID.
    /// </summary>
    /// <param name="end">The end stream ID bound.</param>
    /// <returns>A <see cref="StreamIdRange"/> from the minimum stream ID to <paramref name="end"/>.</returns>
    public static StreamIdRange To(StreamIdBound end)
        => new(StreamIdBound.Min, end);

    /// <inheritdoc cref="To(StreamIdBound)"/>
    public static StreamIdRange To(ValkeyValue end)
        => To(new StreamIdBound(end));

    /// <summary>
    /// Creates a range between two stream ID bounds.
    /// </summary>
    /// <param name="start">The start stream ID bound.</param>
    /// <param name="end">The end stream ID bound.</param>
    /// <returns>A <see cref="StreamIdRange"/> from <paramref name="start"/> to <paramref name="end"/>.</returns>
    public static StreamIdRange Between(StreamIdBound start, StreamIdBound end)
        => new(start, end);

    /// <inheritdoc cref="Between(StreamIdBound, StreamIdBound)"/>
    public static StreamIdRange Between(ValkeyValue start, ValkeyValue end)
        => Between(new StreamIdBound(start), new StreamIdBound(end));

    #endregion
}
