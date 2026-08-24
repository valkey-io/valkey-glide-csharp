// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A lexicographic sorted set range.
/// </summary>
/// <seealso href="https://valkey.io/commands/zlexcount/"/>
/// <seealso href="https://valkey.io/commands/zrangebylex/"/>
/// <seealso href="https://valkey.io/commands/zremrangebylex/"/>
public sealed class LexRange : Range
{
    #region Constants

    /// <summary>
    /// An ascending range spanning all elements (minimum to maximum).
    /// </summary>
    public static readonly LexRange MinToMax = new(LexBound.Min, LexBound.Max);

    /// <summary>
    /// A descending range spanning all elements (maximum to minimum).
    /// </summary>
    public static readonly LexRange MaxToMin = new(LexBound.Max, LexBound.Min);

    #endregion
    #region Fields

    private readonly LexBound _start;
    private readonly LexBound _stop;

    #endregion
    #region Constructors

    private LexRange(LexBound start, LexBound stop)
    {
        _start = start;
        _stop = stop;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a lexicographic range between two bounds.
    /// </summary>
    /// <param name="start">The start lexicographic bound.</param>
    /// <param name="stop">The stop lexicographic bound.</param>
    /// <returns>A <see cref="LexRange"/> between the two bounds.</returns>
    public static LexRange Between(LexBound start, LexBound stop) => new(start, stop);

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() => [.. _start.ToArgs(), .. _stop.ToArgs()];

    #endregion
}
