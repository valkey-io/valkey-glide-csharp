// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XREAD command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xread/"/>
public class StreamReadOptions
{
    #region Public Properties

    /// <summary>
    /// If specified, the maximum number of entries to return per stream (COUNT).
    /// </summary>
    public int? Count { get; init; } = null;

    /// <summary>
    /// If set, the request will block for the specified duration or until new entries are available.
    /// A value of <see cref="TimeSpan.Zero"/> blocks indefinitely (BLOCK).
    /// </summary>
    public TimeSpan? Block { get; init; } = null;

    #endregion
}

/// <summary>
/// Optional arguments for the XREADGROUP command.
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
}
