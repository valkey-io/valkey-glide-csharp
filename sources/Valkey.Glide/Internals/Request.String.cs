// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Commands.Constants.Constants;
using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, ValkeyValue> Get(GlideString key)
        => ToValkeyValue(RequestType.Get, [key], isNullable: true);

    public static Cmd<object[], ValkeyValue[]> Get(IEnumerable<ValkeyKey> keys)
        => new(RequestType.MGet, keys.ToGlideStrings(), false, array =>
            [.. array.Select(item => item is null ? ValkeyValue.Null : (ValkeyValue)(GlideString)item)]);

    public static Cmd<string?, bool> Set(ValkeyKey key, ValkeyValue value, SetOptions options)
        => NullableOKToBool(RequestType.Set, [key.ToGlideString(), value.ToGlideString(), .. options.ToArgs()]);

    public static Cmd<GlideString, ValkeyValue> GetSet(ValkeyKey key, ValkeyValue value, SetOptions options)
        => ToValkeyValue(RequestType.Set, [key.ToGlideString(), value.ToGlideString(), .. options.ToArgs(), ValkeyLiterals.GET.ToGlideString()], isNullable: true);

    public static Cmd<string, bool> Set(KeyValuePair<ValkeyKey, ValkeyValue>[] values)
        => OKToBool(RequestType.MSet, [.. values.SelectMany(kvp => new[] { kvp.Key.ToGlideString(), kvp.Value.ToGlideString() })]);

    public static Cmd<bool, bool> SetIfNotExists(KeyValuePair<ValkeyKey, ValkeyValue>[] values)
        => Simple<bool>(RequestType.MSetNX, [.. values.SelectMany(kvp => new[] { kvp.Key.ToGlideString(), kvp.Value.ToGlideString() })]);

    public static Cmd<long, ValkeyValue> SetRange(GlideString key, long offset, GlideString value)
        => new(RequestType.SetRange, [key, offset.ToGlideString(), value], false, response => (ValkeyValue)response);

    public static Cmd<GlideString, ValkeyValue> GetRange(GlideString key, long start, long end)
        => ToValkeyValue(RequestType.GetRange, [key, start.ToGlideString(), end.ToGlideString()], isNullable: true);

    public static Cmd<long, long> Length(GlideString key)
        => Simple<long>(RequestType.Strlen, [key]);

    public static Cmd<long, long> Append(ValkeyKey key, ValkeyValue value)
        => Simple<long>(RequestType.Append, [key.ToGlideString(), value.ToGlideString()]);

    public static Cmd<long, long> Decrement(ValkeyKey key)
        => Simple<long>(RequestType.Decr, [key.ToGlideString()]);

    public static Cmd<long, long> DecrementBy(ValkeyKey key, long decrement)
        => Simple<long>(RequestType.DecrBy, [key.ToGlideString(), decrement.ToGlideString()]);

    public static Cmd<long, long> Increment(ValkeyKey key)
        => Simple<long>(RequestType.Incr, [key.ToGlideString()]);

    public static Cmd<long, long> IncrementBy(ValkeyKey key, long increment)
        => Simple<long>(RequestType.IncrBy, [key.ToGlideString(), increment.ToGlideString()]);

    public static Cmd<double, double> IncrementByFloat(ValkeyKey key, double increment)
        => Simple<double>(RequestType.IncrByFloat, [key.ToGlideString(), increment.ToString(System.Globalization.CultureInfo.InvariantCulture).ToGlideString()]);

    public static Cmd<GlideString, ValkeyValue> GetDelete(ValkeyKey key)
        => ToValkeyValue(RequestType.GetDel, [key.ToGlideString()], isNullable: true);

    public static Cmd<GlideString, ValkeyValue> GetExpiry(ValkeyKey key, GetExpiryOptions options)
        => ToValkeyValue(RequestType.GetEx, [key.ToGlideString(), .. options.ToArgs()], isNullable: true);

    public static Cmd<GlideString, string?> LongestCommonSubsequence(ValkeyKey first, ValkeyKey second)
        => new(RequestType.LCS, [first.ToGlideString(), second.ToGlideString()], true, response => response?.ToString());

    public static Cmd<long, long> LongestCommonSubsequenceLength(ValkeyKey first, ValkeyKey second)
        => Simple<long>(RequestType.LCS, [first.ToGlideString(), second.ToGlideString(), LenKeyword.ToGlideString()]);

    public static Cmd<object, LCSMatchResult> LongestCommonSubsequenceWithMatches(ValkeyKey first, ValkeyKey second, long minLength = 0)
    {
        List<GlideString> args =
        [
            first.ToGlideString(),
            second.ToGlideString(),
            IdxKeyword.ToGlideString(),
            MinMatchLenKeyword.ToGlideString(),
            minLength.ToGlideString(),
            WithMatchLenKeyword.ToGlideString()
        ];

        return new(RequestType.LCS, [.. args], false, ConvertLCSMatchResult);
    }

    private static LCSMatchResult ConvertLCSMatchResult(object response) =>
        // Handle dictionary response (expected format)
        response is Dictionary<GlideString, object> dictResponse
            ? ConvertLCSMatchResultFromDictionary(dictResponse)
            : LCSMatchResult.Null;

    private static LCSMatchResult ConvertLCSMatchResultFromDictionary(Dictionary<GlideString, object> response)
    {
        List<LCSMatchResult.LCSMatch> matches = [];
        long totalLength = 0;

        // Extract length
        if (response.TryGetValue("len", out object? lengthValue))
        {
            totalLength = lengthValue is long l ? l : 0;
        }

        // Extract matches
        if (response.TryGetValue("matches", out object? matchesValue) && matchesValue is object[] matchesArray)
        {
            foreach (object matchObj in matchesArray)
            {
                if (matchObj is object[] matchArray && matchArray.Length >= 3)
                {
                    object[]? firstRange = matchArray[0] as object[];
                    object[]? secondRange = matchArray[1] as object[];
                    object matchLength = matchArray[2];

                    if (firstRange?.Length >= 2 && secondRange?.Length >= 2)
                    {
                        long firstStart = Convert.ToInt64(firstRange[0]);
                        long secondStart = Convert.ToInt64(secondRange[0]);
                        long length = Convert.ToInt64(matchLength);

                        matches.Add(new LCSMatchResult.LCSMatch(firstStart, secondStart, length));
                    }
                }
            }
        }

        return new LCSMatchResult([.. matches], totalLength);
    }
}
