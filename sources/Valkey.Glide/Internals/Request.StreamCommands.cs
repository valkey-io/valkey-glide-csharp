// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue messageId, long? maxLength, bool useApproximateMaxLength, NameValueEntry[] streamPairs, long? limit, StreamTrimMode mode)
    {
        List<GlideString> args = [key.ToGlideString()];

        // Add MAXLEN trimming options if specified
        if (maxLength.HasValue)
        {
            if (useApproximateMaxLength)
            {
                args.Add("MAXLEN".ToGlideString());
                args.Add("~".ToGlideString());
            }
            else
            {
                args.Add("MAXLEN".ToGlideString());
            }
            args.Add(maxLength.Value.ToGlideString());

            // Add LIMIT if specified and approximate trimming is used
            if (limit.HasValue && useApproximateMaxLength)
            {
                args.Add("LIMIT".ToGlideString());
                args.Add(limit.Value.ToGlideString());
            }
        }

        // Add message ID
        args.Add(messageId.IsNull ? "*".ToGlideString() : messageId.ToGlideString());

        // Add field-value pairs
        foreach (var pair in streamPairs)
        {
            args.Add(pair.Name.ToGlideString());
            args.Add(pair.Value.ToGlideString());
        }

        return new(RequestType.XAdd, [.. args], false, response => (ValkeyValue)response);
    }
}
