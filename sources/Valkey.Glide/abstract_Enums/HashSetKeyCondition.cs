// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.ComponentModel;

namespace Valkey.Glide;

/// <summary>
/// The condition on the hash key for an operation to set the value and expiry for hash fields.
/// </summary>
/// <seealso href="https://valkey.io/commands/hsetex/"/>
public enum HashSetKeyCondition
{
    /// <summary>
    /// Always set the value and expiry for the hash fields.
    /// </summary>
    Always,

    /// <summary>
    /// Only set the value and expiry if the hash key already exists (XX).
    /// </summary>
    OnlyIfExists,

    /// <summary>
    /// Only set the value and expiry if the hash key does not already exist (NX).
    /// </summary>
    OnlyIfNotExists,

    // TODO #671: Remove deprecated value.
    /// <inheritdoc cref="OnlyIfNotExists"/>
    [Obsolete("Use " + nameof(OnlyIfNotExists) + " instead. See https://github.com/valkey-io/valkey-glide-csharp/issues/671")]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    OnlyIfDoesNotExist = OnlyIfNotExists,
}

internal static class HashSetKeyConditionExtensions
{
    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    /// <param name="condition">The condition to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="condition"/> is not a supported <see cref="HashSetKeyCondition"/> value.</exception>
    internal static GlideString[] ToArgs(this HashSetKeyCondition condition)
        => condition switch
        {
            HashSetKeyCondition.Always => [],
            HashSetKeyCondition.OnlyIfExists => [ValkeyLiterals.XX],
            HashSetKeyCondition.OnlyIfNotExists => [ValkeyLiterals.NX],
            _ => throw new ArgumentOutOfRangeException(nameof(condition)),
        };
}
