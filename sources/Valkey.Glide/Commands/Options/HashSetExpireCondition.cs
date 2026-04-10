// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The condition for an operation to the set the values and expiry for hash fields.
/// </summary>
/// <seealso href="https://valkey.io/commands/hsetex/"/>
public enum HashSetExpireCondition
{
    /// <summary>
    /// Always set the values and expiry for the hash fields.
    /// </summary>
    Always,

    /// <summary>
    /// Only set the values and expiry if none of the hash fields exist (FNX).
    /// </summary>
    OnlyIfNoneExist,

    /// <summary>
    /// Only set the values and expiry if all of the hash fields exist (FXX).
    /// </summary>
    OnlyIfAllExist,
}
