// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.ServerModules;

public static partial class GlideJson
{
    /// <summary>
    /// Specifies the condition for JSON.SET command execution.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/json.set/"/>
    public enum SetCondition
    {
        /// <summary>
        /// Always set the value (default behavior).
        /// </summary>
        None,

        /// <summary>
        /// Only set if the key/path does not already exist (NX).
        /// </summary>
        OnlyIfDoesNotExist,

        /// <summary>
        /// Only set if the key/path already exists (XX).
        /// </summary>
        OnlyIfExists
    }

    /// <summary>
    /// Options for formatting JSON.GET command output.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These options control the formatting of the JSON output returned by the JSON.GET command.
    /// </para>
    /// <example>
    /// <code>
    /// // Using object initializer
    /// var options = new GlideJson.GetOptions
    /// {
    ///     Indent = "  ",
    ///     Newline = "\n",
    ///     Space = " "
    /// };
    ///
    /// // Pretty print with tabs
    /// var tabOptions = new GlideJson.GetOptions { Indent = "\t", Newline = "\n" };
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.get/">valkey.io</seealso>
    public sealed class GetOptions
    {
        /// <summary>
        /// Sets an indentation string for nested levels.
        /// </summary>
        /// <remarks>
        /// When set, this string is used to indent nested JSON elements.
        /// Common values include spaces ("  ") or tabs ("\t").
        /// </remarks>
        public string? Indent { get; init; }

        /// <summary>
        /// Sets a string that's printed at the end of each line.
        /// </summary>
        /// <remarks>
        /// When set, this string is appended after each JSON element.
        /// Typically set to "\n" for line breaks.
        /// </remarks>
        public string? Newline { get; init; }

        /// <summary>
        /// Sets a string that's put between a key and a value.
        /// </summary>
        /// <remarks>
        /// When set, this string is inserted between object keys and their values.
        /// Typically set to " " for readability.
        /// </remarks>
        public string? Space { get; init; }

        /// <summary>
        /// Converts options to command arguments.
        /// </summary>
        /// <returns>An array of <see cref="GlideString"/> containing the command arguments.</returns>
        internal GlideString[] ToArgs()
        {
            List<GlideString> args = [];

            if (Indent is not null)
            {
                args.Add("INDENT");
                args.Add(Indent);
            }

            if (Newline is not null)
            {
                args.Add("NEWLINE");
                args.Add(Newline);
            }

            if (Space is not null)
            {
                args.Add("SPACE");
                args.Add(Space);
            }

            return [.. args];
        }
    }

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
    /// var fromStart = GlideJson.ArrIndexOptions.FromStart(2);
    /// var inRange = GlideJson.ArrIndexOptions.InRange(0, 5);
    ///
    /// // Using object initializer
    /// var options = new GlideJson.ArrIndexOptions { Start = 1, End = 10 };
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso href="https://valkey.io/commands/json.arrindex/">valkey.io</seealso>
    public sealed class ArrIndexOptions
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
        /// <returns>A new <see cref="ArrIndexOptions"/> instance.</returns>
        public static ArrIndexOptions FromStart(long start) => new() { Start = start };

        /// <summary>
        /// Creates options with a start and end index.
        /// </summary>
        /// <param name="start">The starting index (inclusive).</param>
        /// <param name="end">The ending index (exclusive).</param>
        /// <returns>A new <see cref="ArrIndexOptions"/> instance.</returns>
        public static ArrIndexOptions InRange(long start, long end) => new() { Start = start, End = end };

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
}
