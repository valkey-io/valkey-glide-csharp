// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for the LOLWUT command.
/// </summary>
/// <seealso href="https://valkey.io/commands/lolwut/"/>
public class LolwutOptions
{
    /// <summary>
    /// The version of the generative computer art to display.
    /// </summary>
    /// <remarks>
    /// Versions 5 and 6 produce graphical output. On other versions, parameters may be ignored.
    /// </remarks>
    public int? Version { get; set; }

    /// <summary>
    /// Additional parameters for the art generation.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>For version 5: length of the line, number of squares per row, and number of squares per column.</item>
    ///   <item>For version 6: number of columns and number of lines.</item>
    /// </list>
    /// </remarks>
    public int[]? Parameters { get; set; }

    /// <summary>
    /// Converts the options to command arguments.
    /// </summary>
    internal string[] ToArgs()
    {
        List<string> args = [];
        if (Version is not null)
        {
            args.Add(ValkeyLiterals.VERSION.ToString());
            args.Add(Version.Value.ToString());
        }
        if (Parameters is not null)
        {
            foreach (int param in Parameters)
            {
                args.Add(param.ToString());
            }
        }
        return [.. args];
    }
}
