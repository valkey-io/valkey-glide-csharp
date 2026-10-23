// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The condition on the hash fields for an operation to set their value and expiry.
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

internal static class HashSetConditionExtensions
{
    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    /// <param name="condition">The condition to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="condition"/> is not a supported <see cref="HashSetCondition"/> value.</exception>
    internal static GlideString[] ToArgs(this HashSetCondition condition)
        => condition switch
        {
            HashSetCondition.Always => [],
            HashSetCondition.OnlyIfNoneExist => [ValkeyLiterals.FNX],
            HashSetCondition.OnlyIfAllExist => [ValkeyLiterals.FXX],
            _ => throw new ArgumentOutOfRangeException(nameof(condition)),
        };
}
