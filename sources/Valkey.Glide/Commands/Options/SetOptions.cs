// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The options for an operation to set the value of a key.
/// </summary>
/// <seealso href="https://valkey.io/commands/set/"/>
public sealed class SetOptions
{
    #region Public Properties

    /// <summary>
    /// The condition under which to set the value.
    /// </summary>
    public SetCondition Condition { get; init; } = SetCondition.Always;

    /// <summary>
    /// The expiry configuration for the key. If <see langword="null"/>, no expiry is set.
    /// </summary>
    public SetExpiryOptions? Expiry { get; init; }

    #endregion
}
