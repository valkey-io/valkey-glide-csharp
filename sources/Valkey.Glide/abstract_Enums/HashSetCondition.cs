// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The condition on the hash fields for an operation to set the value and expiry for hash fields.
/// </summary>
/// <seealso href="https://valkey.io/commands/hsetex/"/>
// TODO #604: Rename to HashSetFieldsCondition.
public enum HashSetCondition
{
    /// <summary>
    /// Always set the value and expiry for the hash fields.
    /// </summary>
    Always,

    /// <summary>
    /// Only set the value and expiry if none of the hash fields exist (FNX).
    /// </summary>
    OnlyIfNoneExist,

    /// <summary>
    /// Only set the value and expiry if all of the hash fields exist (FXX).
    /// </summary>
    OnlyIfAllExist,
}
