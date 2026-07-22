// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for the scan commands.
/// </summary>
/// <seealso href="https://valkey.io/commands/scan/">Valkey commands - SCAN</seealso>
/// <seealso href="https://valkey.io/commands/sscan/">Valkey commands - SSCAN</seealso>
/// <seealso href="https://valkey.io/commands/zscan/">Valkey commands - ZSCAN</seealso>
public class ScanOptions
{
    #region Public Properties

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

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts the options to command arguments.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (!MatchPattern.IsNull)
        {
            args.Add(ValkeyLiterals.MATCH);
            args.Add(MatchPattern.ToGlideString());
        }

        if (Count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT);
            args.Add(Count.Value.ToGlideString());
        }

        if (Type.HasValue)
        {
            args.Add(ValkeyLiterals.TYPE);
            args.Add(ToType(Type.Value));
        }

        return [.. args];
    }

    /// <summary>
    /// Converts the ValkeyType enum to a string.
    /// </summary>
    private static GlideString ToType(ValkeyType type) => type switch
    {
        ValkeyType.String => "string",
        ValkeyType.List => "list",
        ValkeyType.Set => "set",
        ValkeyType.SortedSet => "zset",
        ValkeyType.Hash => "hash",
        ValkeyType.Stream => "stream",
        ValkeyType.Unknown or ValkeyType.None or _ => throw new ArgumentException($"Unsupported ValkeyType '{type}'")
    };

    #endregion
}
