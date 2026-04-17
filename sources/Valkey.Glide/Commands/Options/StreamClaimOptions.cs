// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XCLAIM command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xclaim/"/>
public sealed class StreamClaimOptions
{
    #region Public Properties

    /// <summary>
    /// Set the idle time (last delivery time) of the message. Equivalent to the IDLE option.
    /// </summary>
    public TimeSpan? Idle { get; init; } = null;

    /// <summary>
    /// Set the idle time to a specific Unix timestamp. Equivalent to the TIME option.
    /// </summary>
    public DateTimeOffset? IdleUnix { get; init; } = null;

    /// <summary>
    /// Set the retry counter to the specified value. Equivalent to the RETRYCOUNT option.
    /// </summary>
    public int? RetryCount { get; init; } = null;

    /// <summary>
    /// Create a PEL entry even if the message is not already assigned to a consumer. Equivalent to the FORCE option.
    /// </summary>
    public bool Force { get; init; } = false;

    #endregion
}
