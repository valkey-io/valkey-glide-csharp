// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A stream entry ID bound for range queries.
/// </summary>
/// <seealso href="https://valkey.io/commands/xrange/"/>
/// <seealso href="https://valkey.io/commands/xrevrange/"/>
public sealed class StreamIdBound
{
    #region Constants

    /// <summary>
    /// The minimum stream ID sentinel value (<c>"-"</c>).
    /// </summary>
    public static readonly StreamIdBound Min = new(ValkeyLiterals.StreamMinId);

    /// <summary>
    /// The maximum stream ID sentinel value (<c>"+"</c>).
    /// </summary>
    public static readonly StreamIdBound Max = new(ValkeyLiterals.StreamMaxId);

    #endregion
    #region Public Properties

    /// <summary>
    /// The stream ID bound.
    /// </summary>
    public ValkeyValue Value { get; init; }

    #endregion
    #region Constructors

    /// <summary>
    /// Creates an inclusive stream ID bound.
    /// </summary>
    /// <param name="id">The stream entry ID.</param>
    public StreamIdBound(ValkeyValue id)
    {
        Value = id;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates an inclusive stream ID bound.
    /// </summary>
    /// <param name="id">The stream entry ID.</param>
    /// <returns>An inclusive <see cref="StreamIdBound"/>.</returns>
    public static StreamIdBound Inclusive(ValkeyValue id) => new(id);

    /// <summary>
    /// Creates an exclusive stream ID bound.
    /// </summary>
    /// <param name="id">The stream entry ID.</param>
    /// <returns>An exclusive <see cref="StreamIdBound"/>.</returns>
    public static StreamIdBound Exclusive(ValkeyValue id) => new(ValkeyLiterals.RangeExclusive + id);

    /// <summary>
    /// Converts a <see cref="ValkeyValue"/> to an inclusive stream ID bound.
    /// </summary>
    public static implicit operator StreamIdBound(ValkeyValue id) => new(id);

    #endregion
}
