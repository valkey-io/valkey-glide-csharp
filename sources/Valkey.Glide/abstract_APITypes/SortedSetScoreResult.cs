// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The result of a sorted set operation containing a member and its score.
/// </summary>
public readonly struct SortedSetScoreResult : IEquatable<SortedSetScoreResult>
{
    #region Public Properties

    /// <summary>
    /// The sorted set member.
    /// </summary>
    public ValkeyValue Member { get; }

    /// <summary>
    /// The score associated with the sorted set member.
    /// </summary>
    public double Score { get; }

    #endregion
    #region Constructors

    /// <summary>
    /// Initializes a new <see cref="SortedSetScoreResult"/>.
    /// </summary>
    internal SortedSetScoreResult(ValkeyValue value, double score)
    {
        Member = value;
        Score = score;
    }

    #endregion
    #region Public Methods

    /// <inheritdoc/>
    public override string ToString()
        => $"{Member}: {Score}";

    /// <inheritdoc/>
    public bool Equals(SortedSetScoreResult other)
        => Member == other.Member
        && Score.Equals(other.Score);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is SortedSetScoreResult other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Member, Score);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(SortedSetScoreResult left, SortedSetScoreResult right)
        => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(SortedSetScoreResult left, SortedSetScoreResult right)
        => !left.Equals(right);

    #endregion
}
