// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;

using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Represents a returned value object from a server with cluster-mode enabled. The response type
/// may depend on the submitted <see cref="Route"/>.<br />
/// See also <seealso href="https://valkey.io/docs/topics/cluster-spec/">Valkey cluster specification</seealso>.
/// </summary>
/// <remarks>
/// <see langword="ClusterValue" /> stores values in a union-like object. It contains a single-value or
/// multi-value response from the server. If the command's routing is to a single node use
/// <see cref="ClusterValue{T}.SingleValue" /> to return a response of type <typeparamref name="T" />.
/// Otherwise, use <see cref="ClusterValue{T}.MultiValue" /> to return a <see langword="Dictionary" />
/// of <c>address: nodeResponse</c> where <c>address</c> is of type <see langword="string" /> and
/// <c>nodeResponse</c> is of type <typeparamref name="T" />.
/// </remarks>
/// <typeparam name="T">The wrapped response type</typeparam>
public sealed class ClusterValue<T>
{
    #region Private Fields

    private Dictionary<string, T>? _multiValue = null;

    #endregion
    #region Constructors & Builders

    private ClusterValue() { }

    /// <summary>
    /// Builds a cluster value from the given object.
    /// </summary>
    /// <param name="obj">The response to wrap.</param>
    /// <exception cref="ArgumentException">Thrown if type is not supported.</exception>
    internal static ClusterValue<T> Of(object obj)
        => obj switch
        {
            Dictionary<string, T> dict => OfMultiValue(dict),
            Dictionary<GlideString, T> dictGs => OfMultiValue(dictGs),
            T value => OfSingleValue(value),
            _ => throw new ArgumentException(
                $"Cannot build {nameof(ClusterValue<>)}<{typeof(T).Name}> from a value of type '{obj?.GetType().Name ?? "null"}'.",
                nameof(obj)),
        };

    /// <summary>
    /// Builds a cluster value from the given value.
    /// </summary>
    /// <param name="obj">The value to wrap.</param>
    internal static ClusterValue<T> OfSingleValue(T obj)
        => new() { SingleValue = obj };

    /// <summary>
    /// Builds a cluster value from the given values.
    /// </summary>
    /// <param name="obj">The values to wrap, indexed by node address.</param>
    internal static ClusterValue<T> OfMultiValue(Dictionary<string, T> obj)
        => new() { _multiValue = obj };

    /// <summary>
    /// Builds a cluster value from the given values.
    /// </summary>
    /// <param name="obj">The values to wrap, indexed by node address.</param>
    internal static ClusterValue<T> OfMultiValue(Dictionary<GlideString, T> obj)
        => new() { _multiValue = obj.DownCastKeys() };

    #endregion
    #region Public Methods

    /// <summary>
    /// Returns whether multiple values are stored in this object.
    /// Should be called prior to <see cref="MultiValue" />.
    /// </summary>
    public bool HasMultiData => _multiValue != null;

    /// <summary>
    /// Returns multiple value if they are stored in this object.
    /// Values are indexed by node address.
    /// </summary>
    /// <exception cref="Exception">Thrown when <see cref="HasMultiData" /> is <see langword="false" />.</exception>
    public Dictionary<string, T> MultiValue
        => HasMultiData ? _multiValue! : throw new Exception("No multi value stored");

    /// <summary>
    /// Returns whether a single value is stored in this object.
    /// Should be called prior to <see cref="SingleValue" />.
    /// </summary>
    public bool HasSingleData => !HasMultiData;

    /// <summary>
    /// Returns a single value if one is stored in this object.
    /// </summary>
    /// <returns>The single value response</returns>
    /// <exception cref="Exception">Thrown when <see cref="HasSingleData" /> is <see langword="false" />.</exception>
    [AllowNull]
    public T SingleValue
    {
        get => HasSingleData ? field : throw new Exception("No single value stored");
        private set;
    } = default!;

    #endregion
}
