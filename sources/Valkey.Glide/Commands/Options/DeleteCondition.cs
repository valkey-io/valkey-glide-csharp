// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The condition for an operation to delete a key.
/// </summary>
/// <seealso href="https://valkey.io/commands/delex/"/>
public sealed class DeleteCondition
{
    #region Constructors

    private DeleteCondition(DeleteConditionType type, ValkeyValue value)
    {
        Type = type;
        Value = value;
    }

    #endregion
    #region Nested Types

    /// <summary>
    /// The delete condition options.
    /// </summary>
    internal enum DeleteConditionType
    {
        OnlyIfEqual,
        OnlyIfNotEqual,
    }

    #endregion
    #region Internal Properties

    internal DeleteConditionType Type { get; }
    internal ValkeyValue Value { get; }

    #endregion
    #region Public Methods

    /// <summary>
    /// Only delete the key if it exists and its value equals <paramref name="value"/> (IFEQ).
    /// </summary>
    /// <param name="value">The value to compare the current value against.</param>
    /// <remarks>
    /// <para>
    /// Not supported when compression is enabled.
    /// See <see href="https://glide.valkey.io/concepts/client-features/compression/#incompatible-commands">Valkey GLIDE – Data Compression</see>.
    /// </para>
    /// </remarks>
    public static DeleteCondition OnlyIfEqual(ValkeyValue value)
        => new(DeleteConditionType.OnlyIfEqual, value);

    /// <summary>
    /// Only delete the key if it exists and its value does not equal <paramref name="value"/> (IFNE).
    /// </summary>
    /// <param name="value">The value to compare the current value against.</param>
    /// <remarks>
    /// <para>
    /// Not supported when compression is enabled.
    /// See <see href="https://glide.valkey.io/concepts/client-features/compression/#incompatible-commands">Valkey GLIDE – Data Compression</see>.
    /// </para>
    /// </remarks>
    public static DeleteCondition OnlyIfNotEqual(ValkeyValue value)
        => new(DeleteConditionType.OnlyIfNotEqual, value);

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the condition type is unknown.</exception>
    internal GlideString[] ToArgs() => Type switch
    {
        DeleteConditionType.OnlyIfEqual => [ValkeyLiterals.IFEQ, Value],
        DeleteConditionType.OnlyIfNotEqual => [ValkeyLiterals.IFNE, Value],
        _ => throw new InvalidOperationException($"Unknown condition type: {Type}"),
    };

    #endregion
}
