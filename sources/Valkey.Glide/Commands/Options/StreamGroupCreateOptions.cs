// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the <c>XGROUP CREATE</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xgroup-create/"/>
public sealed class StreamGroupCreateOptions
{
    #region Public Properties

    /// <summary>
    /// Whether to create the stream if it does not already exist (MKSTREAM).
    /// </summary>
    public bool MakeStream { get; init; } = true;

    /// <summary>
    /// If specified, sets the group's entries-read counter to the given value (ENTRIESREAD).
    /// </summary>
    public long? EntriesRead { get; init; } = null;

    #endregion
}
