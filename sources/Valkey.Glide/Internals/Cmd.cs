// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using static Valkey.Glide.Errors;
using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal interface ICmd
{
    /// <summary>
    /// Convert to an FFI-ready struct.
    /// </summary>
    Cmd ToFfi();

    /// <summary>
    /// Get untyped converted (used for batch).
    /// </summary>
    Func<object?, object?> GetConverter();
}

internal class Cmd<R, T>(
    RequestType request,
    GlideString[] args,
    bool isNullable,
    Func<R, T> converter,
    bool allowConverterToHandleNull = false) : ICmd
{
    public readonly bool IsNullable = isNullable;
    public readonly bool AllowConverterToHandleNull = allowConverterToHandleNull;
    public readonly Func<R, T> Converter = converter;
    public readonly RequestType Request = request;
    public readonly GlideString[] Args = args;

    public Func<object?, object?> GetConverter() => value =>
    {
        if (value is null)
        {
            if (AllowConverterToHandleNull)
            {
                return Converter(default!);
            }

            if (IsNullable)
            {
                return null;
            }

            throw new RequestException($"Unexpected return type from Glide: got null expected {typeof(T).GetRealTypeName()}");
        }

        if (value is RequestException)
        {
            return value;
        }

        Debug.Assert(
            value!.GetType() == typeof(R) || typeof(R).IsAssignableFrom(value!.GetType()),
            $"Unexpected return type from Glide: got {value?.GetType().GetRealTypeName()} expected {typeof(R).GetRealTypeName()}");

        return Converter((R)value!);
    };

    public Cmd ToFfi()
        => new(Request, Args);

    public new string ToString()
        => $"{Request} [{string.Join(' ', Args?.ToStrings() ?? [])}]";

    /// <summary>
    /// Convert a command to one which handles a multi-node cluster value.
    /// </summary>
    public Cmd<Dictionary<GlideString, object>, Dictionary<string, T>> ToMultiNodeValue()
        => new(Request, Args, IsNullable, map => ResponseConverters.HandleMultiNodeValue(map, Converter));

    /// <summary>
    /// Convert a command to one which handles a <see cref="ClusterValue{T}" />.
    /// </summary>
    public Cmd<object, ClusterValue<T>> ToClusterValue()
        => new(
            Request,
            Args,
            IsNullable,
            value => value is Dictionary<GlideString, object> dict
                ? ClusterValue<T>.OfMultiValue(dict.ConvertValues(Converter))
                : value is Dictionary<string, object> stringDict
                    ? ClusterValue<T>.OfMultiValue(stringDict.ConvertValues(Converter))
                    : ClusterValue<T>.OfSingleValue(Converter((R)value)),
            AllowConverterToHandleNull);

    /// <summary>
    /// Convert a command to one which handles a <see cref="ClusterValue{T}" /> for the given route.
    /// </summary>
    /// <param name="route">The route for the command.</param>
    public Cmd<object, ClusterValue<T>> ToClusterValue(Route route)
        => route is Route.SingleNodeRoute
            ? new(Request, Args, IsNullable, value => ClusterValue<T>.OfSingleValue(Converter((R)value)), AllowConverterToHandleNull)
            : ToClusterValue();
}
