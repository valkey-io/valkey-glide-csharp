// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Errors;
using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

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

    private static readonly Func<string, ValkeyValue> ToOkConverter = _ => ValkeyValue.Ok;

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
    private static Cmd<string, ValkeyValue> Ok(RequestType request, GlideString[]? args = null)
        => new(request, args ?? [], false, ToOkConverter);

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
    /// Converts a keyword and items into a counted array: <c>keyword count item1 item2 ...</c>.
    /// </summary>
    private static GlideString[] ToArgs(GlideString keyword, IEnumerable<ValkeyValue> items)
        => [keyword, items.Count().ToGlideString(), .. items];


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
    /// Appends SetExpiryOptions arguments (PX/PXAT/KEEPTTL) to the args list.
    /// </summary>
    private static void AddExpiryArgs(List<GlideString> args, SetExpiryOptions options)
    {
        if (options.Duration.HasValue)
        {
            args.Add(ValkeyLiterals.PX);
            args.Add(ToMilliseconds(options.Duration.Value).ToGlideString());
        }
        else if (options.Timestamp.HasValue)
        {
            args.Add(ValkeyLiterals.PXAT);
            args.Add(options.Timestamp.Value.ToUnixTimeMilliseconds().ToGlideString());
        }
        else
        {
            args.Add(ValkeyLiterals.KEEPTTL);
        }
    }

    #region Collection Converters

    /// <summary>
    /// Converts the given objects to a <see cref="ValkeyKey"/> array.
    /// </summary>
    private static ValkeyKey[] ToValkeyKeyArray(IEnumerable<object> items)
        => [.. items.Cast<GlideString>().Select(gs => (ValkeyKey)gs.Bytes)];

    /// <summary>
    /// Converts the given objects to a <see cref="ValkeyKey"/> set.
    /// </summary>
    private static ISet<ValkeyKey> ToValkeyKeySet(IEnumerable<object> items)
        => new HashSet<ValkeyKey>(items.Cast<GlideString>().Select(gs => (ValkeyKey)gs.Bytes));

    /// <summary>
    /// Converts the given objects to a <see cref="ValkeyValue"/> array.
    /// </summary>
    private static ValkeyValue[] ToValkeyValueArray(object[] items)
        => [.. items.Cast<GlideString>().Select(gs => (ValkeyValue)gs)];

    /// <summary>
    /// Converts the given objects to a <see cref="ValkeyValue"/> set.
    /// </summary>
    private static ISet<ValkeyValue> ToValkeyValueSet(IEnumerable<object> items)
        => new HashSet<ValkeyValue>(items.Cast<GlideString>().Select(gs => (ValkeyValue)gs));

    /// <summary>
    /// Converts the given objects to an <see cref="IReadOnlySet{String}"/>.
    /// </summary>
    private static IReadOnlySet<string> ToReadOnlyStringSet(IEnumerable<object> items)
        => new HashSet<string>(items.Cast<GlideString>().Select(gs => gs.ToString()));

    #endregion
    #region Response Map Helpers

    /// <summary>
    /// Returns a required <see langword="bool"/> value from the given response dictionary.
    /// </summary>
    private static bool GetBool(Dictionary<GlideString, object> map, string key)
        => TryGetBool(map, key) ?? throw new RequestException($"Response missing required field '{key}'");

    /// <summary>
    /// Returns an optional <see langword="bool"/> value from the given response dictionary.
    /// </summary>
    private static bool? TryGetBool(Dictionary<GlideString, object> map, string key)
        => map.TryGetValue(key, out var value) ? ((GlideString)value).ToString() == "1" : null;

    /// <summary>
    /// Returns a required <see langword="char"/> value from the given response dictionary.
    /// </summary>
    private static char GetChar(Dictionary<GlideString, object> map, string key)
    {
        var s = GetString(map, key);
        return s.Length == 1 ? s[0] : throw new RequestException($"Response field '{key}' expected single character, got '{s}'");
    }

    /// <summary>
    /// Returns a required <see langword="double"/> value from the given response dictionary.
    /// </summary>
    private static double GetDouble(Dictionary<GlideString, object> map, string key)
        => TryGetDouble(map, key) ?? throw new RequestException($"Response missing required field '{key}'");

    /// <summary>
    /// Returns an optional <see langword="double"/> value from the given response dictionary.
    /// </summary>
    private static double? TryGetDouble(Dictionary<GlideString, object> map, string key)
        => map.TryGetValue(key, out var value)
            ? value switch
            {
                double d => d,
                GlideString gs => double.Parse(gs.ToString()),
                _ => throw new RequestException($"Response field '{key}' expected double or string, got {value.GetType()}"),
            } : null;

    /// <summary>
    /// Returns a required <see langword="long"/> value from the given response dictionary.
    /// </summary>
    private static long GetLong(Dictionary<GlideString, object> map, string key)
        => TryGetLong(map, key) ?? throw new RequestException($"Response missing required field '{key}'");

    /// <summary>
    /// Returns an optional <see langword="long"/> value from the given response dictionary.
    /// </summary>
    private static long? TryGetLong(Dictionary<GlideString, object> map, string key)
        => map.TryGetValue(key, out var value)
            ? value switch
            {
                long l => l,
                GlideString gs => long.Parse(gs.ToString()),
                _ => throw new RequestException($"Response field '{key}' expected long or string, got {value.GetType()}"),
            } : null;

    /// <summary>
    /// Returns a required <see langword="string"/> value from the given response dictionary.
    /// </summary>
    private static string GetString(Dictionary<GlideString, object> map, string key)
        => TryGetString(map, key) ?? throw new RequestException($"Response missing required field '{key}'");

    /// <summary>
    /// Returns an optional <see langword="string"/> value from the given response dictionary.
    /// </summary>
    private static string? TryGetString(Dictionary<GlideString, object> map, string key)
        => map.TryGetValue(key, out var value) ? ((GlideString)value).ToString() : null;

    /// <summary>
    /// Returns a required <see cref="TimeSpan"/> value from the given response dictionary.
    /// </summary>
    private static TimeSpan GetTimeSpan(Dictionary<GlideString, object> map, string key)
        // Expects a server duration string (e.g. "0.123 sec").
        => TimeSpan.FromSeconds(double.Parse(GetString(map, key).Replace(" sec", "")));

    /// <summary>
    /// Returns a required <see cref="ValkeyValue"/> from the given response dictionary.
    /// </summary>
    private static ValkeyValue GetValkeyValue(Dictionary<GlideString, object> map, string key)
    {
        var result = TryGetValkeyValue(map, key);
        return result != ValkeyValue.Null ? result : throw new RequestException($"Response missing required field '{key}'");
    }

    /// <summary>
    /// Returns an optional <see cref="ValkeyValue"/> from the given response dictionary.
    /// </summary>
    private static ValkeyValue TryGetValkeyValue(Dictionary<GlideString, object> map, string key)
        => map.TryGetValue(key, out var value) ? (GlideString)value : ValkeyValue.Null;

    /// <summary>
    /// Returns a required <see cref="ValkeyValue"/> array from the given response dictionary.
    /// </summary>
    private static ValkeyValue[] GetValkeyValues(Dictionary<GlideString, object> map, string key)
        => TryGetValkeyValues(map, key) ?? throw new RequestException($"Response missing required field '{key}'");

    /// <summary>
    /// Returns an optional <see cref="ValkeyValue"/> array from the given response dictionary.
    /// </summary>
    private static ValkeyValue[]? TryGetValkeyValues(Dictionary<GlideString, object> map, string key)
    {
        if (!map.TryGetValue(key, out var value))
        {
            return null;
        }

        IEnumerable<object> items = value switch
        {
            object[] arr => arr,
            HashSet<object> set => set,
            _ => throw new RequestException($"Response field '{key}' expected array, got {value.GetType()}"),
        };

        return [.. items.Cast<GlideString>().Select(gs => (ValkeyValue)gs)];
    }

    #endregion
}
