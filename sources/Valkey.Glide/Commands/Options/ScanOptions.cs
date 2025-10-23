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
    public string? MatchPattern { get; set; }

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
    internal string[] ToArgs()
    {
        List<string> args = [];

        if (MatchPattern != null)
        {
            args.Add(MatchKeyword);
            args.Add(MatchPattern);
        }

        if (Count.HasValue)
        {
            args.Add(CountKeyword);
            args.Add(Count.Value.ToString());
        }

        if (Type.HasValue)
        {
            args.Add(TypeKeyword);
            args.Add(MapValkeyTypeToString(Type.Value));
        }

        return [.. args];
    }

    private static string MapValkeyTypeToString(ValkeyType type) => type switch
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
