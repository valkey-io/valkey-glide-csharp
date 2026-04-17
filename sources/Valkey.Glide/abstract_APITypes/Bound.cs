// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Abstract base class for a sorted set bound.
/// </summary>
/// <seealso href="https://valkey.io/commands/zcount/"/>
/// <seealso href="https://valkey.io/commands/zlexcount/"/>
/// <seealso href="https://valkey.io/commands/zrange/"/>
/// <seealso href="https://valkey.io/commands/zrangebylex/"/>
/// <seealso href="https://valkey.io/commands/zrangebyscore/"/>
/// <seealso href="https://valkey.io/commands/zrangestore/"/>
/// <seealso href="https://valkey.io/commands/zremrangebylex/"/>
/// <seealso href="https://valkey.io/commands/zremrangebyrank/"/>
/// <seealso href="https://valkey.io/commands/zremrangebyscore/"/>
public abstract class Bound : IEquatable<Bound>
{
    #region Internal Methods

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal abstract GlideString[] ToArgs();

    #endregion
    #region Public Methods

    /// <inheritdoc/>
    public abstract bool Equals(Bound? other);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => Equals(obj as Bound);

    /// <inheritdoc/>
    public abstract override int GetHashCode();

    /// <summary>
    /// Determines whether two <see cref="Bound"/> instances are equal.
    /// </summary>
    public static bool operator ==(Bound? left, Bound? right)
        => Equals(left, right);

    /// <summary>
    /// Determines whether two <see cref="Bound"/> instances are not equal.
    /// </summary>
    public static bool operator !=(Bound? left, Bound? right)
        => !Equals(left, right);

    #endregion
}
