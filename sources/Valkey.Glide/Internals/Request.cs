// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<object?, object?> CustomCommand(GlideString[] args)
        => new(RequestType.CustomCommand, args, true, o => o);

    public static Cmd<object?, T> CustomCommand<T>(GlideString[] args, Func<object?, T> converter) where T : class?
        => new(RequestType.CustomCommand, args, true, converter);

    /// <summary>
    /// Create a Cmd which does not need type conversion
    /// </summary>
    private static Cmd<T, T> Simple<T>(RequestType request, GlideString[] args, bool isNullable = false)
        => new(request, args, isNullable, o => o);

    /// <summary>
    /// Create a Cmd which returns a Boolean value based on the response being 1 or not.
    /// </summary>
    /// <typeparam name="T">Any type that can be implicitly cast to a numeric value for comparison</typeparam>
    /// <param name="request">The request type</param>
    /// <param name="args">The command arguments</param>
    /// <returns>A command that converts the response to a boolean value (true if response equals 1)</returns>
    private static Cmd<T, bool> Boolean<T>(RequestType request, GlideString[] args)
        => new(request, args, false, response => Convert.ToInt64(response) == 1);

    private static readonly Func<string, bool> IsOkConverter = response => response == "OK";
    private static readonly Func<string, ValkeyValue> ToOkConverter = _ => ValkeyValue.Ok;

    /// <summary>
    /// Create a Cmd which returns a Boolean value based on the response being OK or not.
    /// </summary>
    /// <param name="request">The request type</param>
    /// <param name="args">The command arguments</param>
    /// <returns>A command that converts the response to a boolean value (true if response equals OK)</returns>
    private static Cmd<string, bool> OKToBool(RequestType request, GlideString[] args)
        => new(request, args, false, IsOkConverter);

    /// <summary>
    /// Create a Cmd which returns a Boolean value based on the response being OK or not, allowing null responses.
    /// </summary>
    /// <param name="request">The request type</param>
    /// <param name="args">The command arguments</param>
    /// <returns>A command that converts the response to a boolean value (true if response equals OK, false if null)</returns>
    private static Cmd<string?, bool> NullableOKToBool(RequestType request, GlideString[] args)
        => new(request, args, true, response => response == "OK", allowConverterToHandleNull: true);

    /// <summary>
    /// Create a Cmd which returns "OK" when it completes.
    /// </summary>
    /// <param name="request">The request type</param>
    /// <param name="args">The command arguments</param>
    /// <returns>A command that returns <see cref="ValkeyValue.Ok"/></returns>
    private static Cmd<string, ValkeyValue> Ok(RequestType request, GlideString[] args)
        => new(request, args, false, ToOkConverter);

    /// <summary>
    /// Create a Cmd which converts the response to a ValkeyValue.
    /// </summary>
    /// <param name="request">The request type</param>
    /// <param name="args">The command arguments</param>
    /// <param name="isNullable">Whether the response can be null</param>
    /// <returns>A command that converts the response to a ValkeyValue</returns>
    private static Cmd<GlideString, ValkeyValue> ToValkeyValue(RequestType request, GlideString[] args, bool isNullable = false)
        => new(request, args, isNullable, response => (ValkeyValue)response, allowConverterToHandleNull: true);

    /// <summary>
    /// Create a Cmd which converts an array of GlideStrings to an array of ValkeyValues.
    /// </summary>
    /// <param name="request">The request type</param>
    /// <param name="args">The command arguments</param>
    /// <returns>A command that converts an array to a ValkeyValue array</returns>
    private static Cmd<object[], ValkeyValue[]> ObjectArrayToValkeyValueArray(RequestType request, GlideString[] args)
        => new(request, args, false, set => [.. set.Cast<GlideString>().Select(gs => gs)]);

    /// <summary>
    /// Converts a <see cref="HashSet{Object}"/> response to an <see cref="ISet{ValkeyValue}"/>.
    /// </summary>
    private static ISet<ValkeyValue> ToValkeyValueSet(HashSet<object> set)
        => new HashSet<ValkeyValue>(set.Cast<GlideString>().Select(gs => (ValkeyValue)gs));

    /// <summary>
    /// Converts an object array to a <see cref="ValkeyKey"/> set and returns the result.
    /// </summary>
    /// <param name="objects">The object array to convert.</param>
    /// <returns>A converted <see cref="ValkeyKey"/> set.</returns>
    private static HashSet<ValkeyKey> ToValkeyKeySet(object[] objects)
        => [.. objects.Cast<GlideString>().Select(gs => (ValkeyKey)gs.Bytes)];

    /// <summary>
    /// Converts a <see cref="GlideString"/>-keyed dictionary to a <see cref="ValkeyKey"/>-keyed dictionary with <see langword="long"/> values.
    /// </summary>
    private static Dictionary<ValkeyKey, long> ToValkeyKeyLongDict(Dictionary<GlideString, object> dict)
    {
        Dictionary<ValkeyKey, long> result = [];

        foreach (var kvp in dict)
        {
            result[(ValkeyKey)kvp.Key.Bytes] = Convert.ToInt64(kvp.Value);
        }

        return result;
    }

    /// <summary>
    /// Converts the given time span to milliseconds.
    /// <param name="timeSpan">The time span to convert.</param>
    /// <returns>The number of milliseconds in the time span, rounded to the nearest integer.</returns>
    /// </summary>
    private static long ToMilliseconds(TimeSpan timeSpan)
        => timeSpan.Ticks / TimeSpan.TicksPerMillisecond;

    /// <summary>
    /// Converts the given time span to seconds.
    /// <param name="timeSpan">The time span to convert.</param>
    /// <returns>The number of seconds in the time span, as a floating-point number.</returns>
    /// </summary>
    private static double ToSeconds(TimeSpan timeSpan)
        => timeSpan.TotalSeconds;
}
