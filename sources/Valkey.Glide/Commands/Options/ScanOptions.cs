// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Constants.Constants;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for the SCAN command.
/// </summary>
public class ScanOptions
{
    /// <summary>
    /// Pattern to match keys against.
    /// </summary>
    public string? MatchPattern { get; set; }

    /// <summary>
    /// Hint for the number of keys to return per iteration.
    /// </summary>
    public long? Count { get; set; }

    /// <summary>
    /// Filter keys by their data type.
    /// </summary>
    public ValkeyType? Type { get; set; }

    /// <summary>
    /// Converts the options to an array of string arguments.
    /// </summary>
    /// <returns>Array of string arguments for the SCAN command.</returns>
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
