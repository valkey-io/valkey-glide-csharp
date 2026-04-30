// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The condition for an operation to set the value of a key.
/// </summary>
/// <seealso href="https://valkey.io/commands/set/"/>
public sealed class SetCondition
{
    #region Public Properties

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

    #region Internal Properties

    internal SetConditionType Type { get; }
    internal ValkeyValue? ComparisonValue { get; }

    #endregion

    #region Constructors

    private SetCondition(SetConditionType type, ValkeyValue? comparisonValue = null)
    {
        Type = type;
        ComparisonValue = comparisonValue;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Only set the value if the current value equals <paramref name="comparisonValue"/> (IFEQ).
    /// </summary>
    /// <note>Since Valkey 8.1.0.</note>
    public static SetCondition OnlyIfEqual(ValkeyValue comparisonValue)
        => new(SetConditionType.OnlyIfEqual, comparisonValue);

    #endregion

    #region Internal Methods

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal GlideString[] ToArgs() => Type switch
    {
        SetConditionType.Always => [],
        SetConditionType.OnlyIfExists => [ValkeyLiterals.XX],
        SetConditionType.OnlyIfDoesNotExist => [ValkeyLiterals.NX],
        SetConditionType.OnlyIfEqual => [ValkeyLiterals.IFEQ, ComparisonValue!.Value.ToGlideString()],
        _ => throw new InvalidOperationException($"Unknown condition type: {Type}"),
    };

    #endregion

    /// <summary>
    /// The set condition options.
    /// </summary>
    internal enum SetConditionType { Always, OnlyIfExists, OnlyIfDoesNotExist, OnlyIfEqual }
}
