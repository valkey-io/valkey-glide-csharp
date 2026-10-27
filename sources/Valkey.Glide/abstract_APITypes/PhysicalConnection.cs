// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Valkey.Glide;

internal sealed class PhysicalConnection
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteCrlf(Span<byte> span, int offset)
    {
        span[offset++] = (byte)'\r';
        span[offset++] = (byte)'\n';
        return offset;
    }

    internal static int WriteRaw(Span<byte> span, long value, bool withLengthPrefix = false, int offset = 0)
    {
        if (value is >= 0 and <= 9)
        {
            if (withLengthPrefix)
            {
                span[offset++] = (byte)'1';
                offset = WriteCrlf(span, offset);
            }

            span[offset++] = (byte)('0' + (int)value);
        }
        else if (value is >= 10 and < 100)
        {
            if (withLengthPrefix)
            {
                span[offset++] = (byte)'2';
                offset = WriteCrlf(span, offset);
            }

            span[offset++] = (byte)('0' + ((int)value / 10));
            span[offset++] = (byte)('0' + ((int)value % 10));
        }
        else if (value is >= 100 and < 1000)
        {
            int v = (int)value;
            int units = v % 10;
            v /= 10;
            int tens = v % 10, hundreds = v / 10;
            if (withLengthPrefix)
            {
                span[offset++] = (byte)'3';
                offset = WriteCrlf(span, offset);
            }

            span[offset++] = (byte)('0' + hundreds);
            span[offset++] = (byte)('0' + tens);
            span[offset++] = (byte)('0' + units);
        }
        else if (value is < 0 and >= -9)
        {
            if (withLengthPrefix)
            {
                span[offset++] = (byte)'2';
                offset = WriteCrlf(span, offset);
            }

            span[offset++] = (byte)'-';
            span[offset++] = (byte)('0' - (int)value);
        }
        else if (value is <= -10 and > -100)
        {
            if (withLengthPrefix)
            {
                span[offset++] = (byte)'3';
                offset = WriteCrlf(span, offset);
            }

            value = -value;
            span[offset++] = (byte)'-';
            span[offset++] = (byte)('0' + ((int)value / 10));
            span[offset++] = (byte)('0' + ((int)value % 10));
        }
        else
        {
            // we're going to write it, but *to the wrong place*
            var availableChunk = span[offset..];
            var formattedLength = Format.FormatInt64(value, availableChunk);
            if (withLengthPrefix)
            {
                // now we know how large the prefix is: write the prefix, then write the value
                var prefixLength = Format.FormatInt32(formattedLength, availableChunk);
                offset += prefixLength;
                offset = WriteCrlf(span, offset);

                availableChunk = span[offset..];
                var finalLength = Format.FormatInt64(value, availableChunk);
                offset += finalLength;
                Debug.Assert(finalLength == formattedLength);
            }
            else
            {
                offset += formattedLength;
            }
        }

        return WriteCrlf(span, offset);
    }
}
