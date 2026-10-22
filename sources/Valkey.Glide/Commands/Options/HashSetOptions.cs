// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The options for an operation to set hash field value(s).
/// </summary>
/// <seealso href="https://valkey.io/commands/hsetex/"/>
public sealed class HashSetOptions
{
    #region Public Properties

    /// <summary>
    /// The condition on the hash key under which to set the field(s).
    /// </summary>
    /// <remarks>
    /// <para>Since Valkey 9.1.0.</para>
    /// </remarks>
    public HashSetKeyCondition KeyCondition { get; init; } = HashSetKeyCondition.Always;

    /// <summary>
    /// The condition on the hash field(s) under which to set the field(s).
    /// </summary>
    // TODO #604: Rename to FieldsCondition.
    public HashSetCondition Condition { get; init; } = HashSetCondition.Always;

    /// <summary>
    /// The expiry configuration for the field(s). If <see langword="null"/>, no expiry is set.
    /// </summary>
    public SetExpiryOptions? Expiry { get; init; }

    #endregion
}
