// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the <c>XREADGROUP</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xreadgroup/"/>
public sealed class StreamReadGroupOptions : StreamReadOptions
{
    #region Public Properties

    /// <summary>
    /// If <see langword="true"/>, messages are not added to the Pending Entries List (PEL).
    /// This is equivalent to acknowledging the message when it is read.
    /// </summary>
    public bool NoAck { get; init; } = false;

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs()
    {
        List<GlideString> args = [.. base.ToArgs()];

        if (NoAck)
        {
            args.Add(ValkeyLiterals.NOACK);
        }

        return [.. args];
    }

    #endregion
}
