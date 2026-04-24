// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Specifies the condition for JSON.SET command execution.
/// </summary>
/// <seealso href="https://valkey.io/commands/json.set/"/>
public enum JsonSetCondition
{
    /// <summary>
    /// Always set the value (default behavior).
    /// </summary>
    None,

    /// <summary>
    /// Only set if the key/path does not already exist (NX).
    /// </summary>
    OnlyIfDoesNotExist,

    /// <summary>
    /// Only set if the key/path already exists (XX).
    /// </summary>
    OnlyIfExists
}
