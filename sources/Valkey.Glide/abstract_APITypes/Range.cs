// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Abstract base class for a sorted set range.
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
public abstract class Range
{
    #region Internal Methods

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal abstract GlideString[] ToArgs();

    /// <summary>
    /// Returns <see langword="true"/> if this range is unbounded.
    /// </summary>
    internal abstract bool IsUnbounded();

    #endregion
}
