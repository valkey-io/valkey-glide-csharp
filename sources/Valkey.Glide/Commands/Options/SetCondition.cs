// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The condition for an operation to set the value of a key.
/// </summary>
/// <seealso href="https://valkey.io/commands/set/"/>
public sealed class SetCondition
{
    #region Public Fields

    /// <summary>
    /// Always set the value for the key.
    /// </summary>
    public static readonly SetCondition Always = new(SetConditionType.Always);

    /// <summary>
    /// Only set the value if the key already exists (XX).
    /// </summary>
    public static readonly SetCondition OnlyIfExists = new(SetConditionType.OnlyIfExists);

    /// <summary>
    /// Only set the value if the key does not already exist (NX).
    /// </summary>
    public static readonly SetCondition OnlyIfDoesNotExist = new(SetConditionType.OnlyIfDoesNotExist);

    #endregion
    #region Constructors

    private SetCondition(SetConditionType type, ValkeyValue? comparisonValue = null)
    {
        Type = type;
        ComparisonValue = comparisonValue;
    }

    #endregion
    #region Nested Types

    /// <summary>
    /// The set condition options.
    /// </summary>
    internal enum SetConditionType
    {
        Always,
        OnlyIfExists,
        OnlyIfDoesNotExist,
        OnlyIfEqual,
        OnlyIfNotEqual,
    }

    #endregion
    #region Internal Properties

    internal SetConditionType Type { get; }
    internal ValkeyValue? ComparisonValue { get; }

    #endregion
    #region Public Methods

    /// <summary>
    /// Only set the value if the key exists and its value equals <paramref name="comparisonValue"/> (IFEQ).
    /// </summary>
    /// <param name="comparisonValue">The value to compare the current value against.</param>
    /// <remarks>
    /// <para>Since Valkey 8.1.0.</para>
    /// <para>
    /// Not supported when compression is enabled.
    /// See <see href="https://glide.valkey.io/concepts/client-features/compression/#incompatible-commands">Valkey GLIDE – Data Compression</see>.
    /// </para>
    /// </remarks>
    public static SetCondition OnlyIfEqual(ValkeyValue comparisonValue)
        => new(SetConditionType.OnlyIfEqual, comparisonValue);

    /// <summary>
    /// Only set the value if the key does not exist or if its value does not equal <paramref name="comparisonValue"/> (IFNE).
    /// </summary>
    /// <param name="comparisonValue">The value to compare the current value against.</param>
    /// <remarks>
    /// <para>Since Valkey 9.2.0.</para>
    /// <para>
    /// Not supported when compression is enabled.
    /// See <see href="https://glide.valkey.io/concepts/client-features/compression/#incompatible-commands">Valkey GLIDE – Data Compression</see>.
    /// </para>
    /// </remarks>
    public static SetCondition OnlyIfNotEqual(ValkeyValue comparisonValue)
        => new(SetConditionType.OnlyIfNotEqual, comparisonValue);

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the condition type is unknown.</exception>
    internal GlideString[] ToArgs() => Type switch
    {
        SetConditionType.Always => [],
        SetConditionType.OnlyIfExists => [ValkeyLiterals.XX],
        SetConditionType.OnlyIfDoesNotExist => [ValkeyLiterals.NX],
        SetConditionType.OnlyIfEqual => [ValkeyLiterals.IFEQ, ComparisonValue!.Value],
        SetConditionType.OnlyIfNotEqual => [ValkeyLiterals.IFNE, ComparisonValue!.Value],
        _ => throw new InvalidOperationException($"Unknown condition type: {Type}"),
    };

    #endregion
}
