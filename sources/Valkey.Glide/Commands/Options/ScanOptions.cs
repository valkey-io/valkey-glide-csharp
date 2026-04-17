// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Constants.Constants;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for the scan commands.
/// </summary>
public class ScanOptions
{
    /// <summary>
    /// Pattern to filter keys against.
    /// </summary>
    public ValkeyValue MatchPattern { get; set; } = ValkeyValue.Null;

    /// <summary>
    /// Hint for the number of keys to return per iteration.
    /// </summary>
    public long? Count { get; set; }

    /// <summary>
    /// Type to filter keys against.
    /// </summary>
    public ValkeyType? Type { get; set; }

    /// <summary>
    /// Converts the options to an array of string arguments for scan commands.
    /// </summary>
    /// <returns>Array of string arguments.</returns>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (!MatchPattern.IsNull)
        {
            args.Add(MatchKeyword.ToGlideString());
            args.Add(MatchPattern.ToGlideString());
        }

        if (Count.HasValue)
        {
            args.Add(CountKeyword.ToGlideString());
            args.Add(Count.Value.ToGlideString());
        }

        if (Type.HasValue)
        {
            args.Add(TypeKeyword.ToGlideString());
            args.Add(MapValkeyTypeToString(Type.Value));
        }

        return [.. args];
    }

    // TODO - use ToArgs instead
    private static GlideString MapValkeyTypeToString(ValkeyType type) => type switch
    {
        ValkeyType.String => "string",
        ValkeyType.List => "list",
        ValkeyType.Set => "set",
        ValkeyType.SortedSet => "zset",
        ValkeyType.Hash => "hash",
        ValkeyType.Stream => "stream",
        ValkeyType.Unknown or ValkeyType.None or _ => throw new ArgumentException($"Unsupported ValkeyType for SCAN: {type}")
    };
}
