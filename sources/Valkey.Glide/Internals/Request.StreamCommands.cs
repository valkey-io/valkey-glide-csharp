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

    public static Cmd<object, StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, long? count, long? block)
    {
        List<GlideString> args = [];

        if (block.HasValue)
        {
            args.Add("BLOCK".ToGlideString());
            args.Add(block.Value.ToGlideString());
        }

        if (count.HasValue)
        {
            args.Add("COUNT".ToGlideString());
            args.Add(count.Value.ToGlideString());
        }

        args.Add("STREAMS".ToGlideString());
        args.Add(key.ToGlideString());
        args.Add(position.ToGlideString());

        return new Cmd<object, StreamEntry[]>(RequestType.XRead, [.. args], false, ConvertSingleStreamRead, allowConverterToHandleNull: true);
    }

    public static Cmd<object, ValkeyStream[]> StreamReadAsync(StreamPosition[] streamPositions, long? count, long? block)
    {
        List<GlideString> args = [];

        if (block.HasValue)
        {
            args.Add("BLOCK".ToGlideString());
            args.Add(block.Value.ToGlideString());
        }

        if (count.HasValue)
        {
            args.Add("COUNT".ToGlideString());
            args.Add(count.Value.ToGlideString());
        }

        args.Add("STREAMS".ToGlideString());
        foreach (var sp in streamPositions)
        {
            args.Add(sp.Key.ToGlideString());
        }
        foreach (var sp in streamPositions)
        {
            args.Add(sp.Position.ToGlideString());
        }

        return new(RequestType.XRead, [.. args], false, ConvertMultiStreamRead, allowConverterToHandleNull: true);
    }

    private static StreamEntry[] ConvertSingleStreamRead(object response)
    {
        if (response is null) return [];
        
        // Handle dictionary response (RESP3 format)
        // Structure: {stream_key => {entry_id => [field_value_pairs]}}
        if (response is Dictionary<GlideString, object> dict)
        {
            var allEntries = new List<StreamEntry>();
            
            foreach (var streamKvp in dict)
            {
                // streamKvp.Value should be a nested dictionary of {entry_id => field_values}
                if (streamKvp.Value is Dictionary<GlideString, object> entriesDict)
                {
                    foreach (var entryKvp in entriesDict)
                    {
                        var entryId = entryKvp.Key; // This is the entry ID like "1763338493936-0"
                        var outerArray = entryKvp.Value as object[];
                        
                        if (outerArray is null || outerArray.Length == 0) continue;
                        
                        // The value is an array containing a single element which is the field-value array
                        var fieldValues = outerArray[0] as object[];
                        if (fieldValues is null || fieldValues.Length == 0) continue;
                        
                        // Convert field-value array to NameValueEntry array
                        var values = new NameValueEntry[fieldValues.Length / 2];
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = new NameValueEntry(
                                (GlideString)fieldValues[i * 2],
                                (GlideString)fieldValues[i * 2 + 1]
                            );
                        }
                        
                        allEntries.Add(new StreamEntry(entryId, values));
                    }
                }
            }
            
            return [.. allEntries];
        }
        
        // Handle standalone response (array)
        var streams = (object[])response;
        if (streams.Length == 0) return [];
        
        var streamData = (object[])streams[0];
        if (streamData.Length < 2) return [];
        
        var entries = (object[])streamData[1];
        if (entries.Length == 0) return [];
        
        return ConvertStreamEntries(entries);
    }

    private static ValkeyStream[] ConvertMultiStreamRead(object response)
    {
        if (response is null) return [];
        
        // Handle dictionary response (RESP3 format)
        // Structure: {stream_key => {entry_id => [field_value_pairs]}}
        if (response is Dictionary<GlideString, object> dict)
        {
            var result = new List<ValkeyStream>();
            
            foreach (var streamKvp in dict)
            {
                var streamKey = new ValkeyKey(streamKvp.Key);
                var entries = new List<StreamEntry>();
                
                // streamKvp.Value should be a nested dictionary of {entry_id => field_values}
                if (streamKvp.Value is Dictionary<GlideString, object> entriesDict)
                {
                    foreach (var entryKvp in entriesDict)
                    {
                        var entryId = entryKvp.Key;
                        var outerArray = entryKvp.Value as object[];
                        
                        if (outerArray is null || outerArray.Length == 0) continue;
                        
                        // The value is an array containing a single element which is the field-value array
                        var fieldValues = outerArray[0] as object[];
                        if (fieldValues is null || fieldValues.Length == 0) continue;
                        
                        // Convert field-value array to NameValueEntry array
                        var values = new NameValueEntry[fieldValues.Length / 2];
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = new NameValueEntry(
                                (GlideString)fieldValues[i * 2],
                                (GlideString)fieldValues[i * 2 + 1]
                            );
                        }
                        
                        entries.Add(new StreamEntry(entryId, values));
                    }
                }
                
                result.Add(new ValkeyStream(streamKey, [.. entries]));
            }
            
            return [.. result];
        }
        
        // Handle standalone response (array)
        var streams = (object[])response;
        var resultArray = new ValkeyStream[streams.Length];
        
        for (int i = 0; i < streams.Length; i++)
        {
            var streamData = (object[])streams[i];
            var key = new ValkeyKey((GlideString)streamData[0]);
            var entries = streamData[1] as object[];
            
            resultArray[i] = new ValkeyStream(key, entries is null || entries.Length == 0 ? [] : ConvertStreamEntries(entries));
        }
        
        return resultArray;
    }

    private static StreamEntry[] ConvertStreamEntries(object[] entries)
    {
        var result = new StreamEntry[entries.Length];
        
        for (int i = 0; i < entries.Length; i++)
        {
            var entry = (object[])entries[i];
            var id = (GlideString)entry[0];
            var fields = entry[1] as object[];
            
            var values = fields is null ? [] : new NameValueEntry[fields.Length / 2];
            if (fields is not null)
            {
                for (int j = 0; j < values.Length; j++)
                {
                    values[j] = new NameValueEntry(
                        (GlideString)fields[j * 2],
                        (GlideString)fields[j * 2 + 1]
                    );
                }
            }
            
            result[i] = new StreamEntry(id, values);
        }
        
        return result;
    }
}
