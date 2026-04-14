// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the XREAD command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xread/"/>
public class StreamReadOptions
{
    /// <summary>
    /// The maximum number of entries to return per stream.
    /// Equivalent to the COUNT option.
    /// </summary>
    public int? Count { get; init; }

    /// <summary>
    /// If set, the request will block for the specified duration or until the server has the required
    /// number of entries. A value of <see cref="TimeSpan.Zero"/> blocks indefinitely.
    /// Equivalent to the BLOCK option.
    /// </summary>
    public TimeSpan? Block { get; init; }
}

/// <summary>
/// Optional arguments for the XREADGROUP command.
/// </summary>
/// <seealso href="https://valkey.io/commands/xreadgroup/"/>
public class StreamReadGroupOptions : StreamReadOptions
{
    /// <summary>
    /// If <c>true</c>, messages are not added to the Pending Entries List (PEL).
    /// This is equivalent to acknowledging the message when it is read.
    /// Equivalent to the NOACK option.
    /// </summary>
    public bool NoAck { get; init; }
}
