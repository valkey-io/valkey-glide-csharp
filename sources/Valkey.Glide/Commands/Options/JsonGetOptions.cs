// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

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
/// var options = new JsonGetOptions
/// {
///     Indent = "  ",
///     Newline = "\n",
///     Space = " "
/// };
///
/// // Pretty print with tabs
/// var tabOptions = new JsonGetOptions { Indent = "\t", Newline = "\n" };
/// </code>
/// </example>
/// </remarks>
/// <seealso href="https://valkey.io/commands/json.get/">valkey.io</seealso>
public sealed class JsonGetOptions
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
