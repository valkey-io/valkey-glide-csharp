// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The condition for the set expiry operation.
/// </summary>
public enum SetExpiryCondition
{
    /// <summary>
    /// Always set the expiry.
    /// </summary>
    Always,

    /// <summary>
    /// Only set the expiry if none of the specified keys or fields exist (FNX).
    /// </summary>
    OnlyIfNoneExist,

    /// <summary>
    /// Only set the expiry if all of the specified keys or fields exist (FXX).
    /// </summary>
    OnlyIfAllExist,
}
