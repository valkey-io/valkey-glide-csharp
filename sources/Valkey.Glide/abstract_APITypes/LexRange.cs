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
    /// A range spanning all elements.
    /// </summary>
    public static readonly LexRange All = new(LexBound.Min, LexBound.Max);

    #endregion
    #region Fields

    private readonly LexBound _min;
    private readonly LexBound _max;

    #endregion
    #region Constructors

    private LexRange(LexBound min, LexBound max)
    {
        _min = min;
        _max = max;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a lexicographic range from a minimum bound.
    /// </summary>
    /// <param name="min">The minimum lexicographic bound.</param>
    /// <returns>A <see cref="LexRange"/> from a minimum bound.</returns>
    public static LexRange From(LexBound min)
        => new(min, LexBound.Max);

    /// <summary>
    /// Creates a lexicographic range to a maximum bound.
    /// </summary>
    /// <param name="max">The maximum lexicographic bound.</param>
    /// <returns>A <see cref="LexRange"/> to a maximum bound.</returns>
    public static LexRange To(LexBound max)
        => new(LexBound.Min, max);

    /// <summary>
    /// Creates a lexicographic range between minimum and maximum bounds.
    /// </summary>
    /// <param name="min">The minimum lexicographic bound.</param>
    /// <param name="max">The maximum lexicographic bound.</param>
    /// <returns>A <see cref="LexRange"/> between minimum and maximum bounds.</returns>
    public static LexRange Between(LexBound min, LexBound max)
        => new(min, max);

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override bool IsUnbounded()
        => _min.IsMin && _max.IsMax;

    /// <inheritdoc/>
    internal override GlideString[] ToArgs()
        => [.. _min.ToArgs(), .. _max.ToArgs()];

    #endregion
}
