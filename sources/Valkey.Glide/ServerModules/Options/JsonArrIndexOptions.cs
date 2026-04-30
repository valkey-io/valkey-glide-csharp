// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.ServerModules.Options;

/// <summary>
/// Options for JSON.ARRINDEX command specifying search range.
/// </summary>
/// <remarks>
/// <para>
/// These options control the search range for the JSON.ARRINDEX command.
/// The start index is inclusive and the end index is exclusive.
/// </para>
/// <example>
/// <code>
/// // Using factory methods (recommended)
/// var fromStart = JsonArrIndexOptions.FromStart(2);
/// var inRange = JsonArrIndexOptions.InRange(0, 5);
///
/// // Using object initializer
/// var options = new JsonArrIndexOptions { Start = 1, End = 10 };
/// </code>
/// </example>
/// </remarks>
/// <seealso href="https://valkey.io/commands/json.arrindex/">valkey.io</seealso>
public sealed class JsonArrIndexOptions
{
    /// <summary>
    /// The start index (inclusive). Defaults to 0.
    /// </summary>
    /// <remarks>
    /// Negative values indicate offsets from the end of the array.
    /// </remarks>
    public long Start { get; init; } = 0;

    /// <summary>
    /// The end index (exclusive). If null, searches to end of array.
    /// </summary>
    /// <remarks>
    /// Negative values indicate offsets from the end of the array.
    /// When null, the search continues to the end of the array.
    /// </remarks>
    public long? End { get; init; }

    /// <summary>
    /// Creates options with a start index.
    /// </summary>
    /// <param name="start">The starting index (inclusive).</param>
    /// <returns>A new <see cref="JsonArrIndexOptions"/> instance.</returns>
    public static JsonArrIndexOptions FromStart(long start) => new() { Start = start };

    /// <summary>
    /// Creates options with a start and end index.
    /// </summary>
    /// <param name="start">The starting index (inclusive).</param>
    /// <param name="end">The ending index (exclusive).</param>
    /// <returns>A new <see cref="JsonArrIndexOptions"/> instance.</returns>
    public static JsonArrIndexOptions InRange(long start, long end) => new() { Start = start, End = end };

    /// <summary>
    /// Converts options to command arguments.
    /// </summary>
    /// <returns>An array of <see cref="GlideString"/> containing the command arguments.</returns>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [Start.ToString()];

        if (End.HasValue)
        {
            args.Add(End.Value.ToString());
        }

        return [.. args];
    }
}
