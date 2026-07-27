// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a message received through the <see href="https://valkey.io/commands/monitor/">MONITOR</see> stream.
/// </summary>
/// <seealso href="https://valkey.io/commands/monitor/"/>
public sealed record MonitorMessage
{
    #region Public Properties

    /// <summary>
    /// The server timestamp when the command was processed.
    /// </summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>
    /// The database number the command was issued against.
    /// </summary>
    public required ushort Database { get; init; }

    /// <summary>
    /// The address of the client that issued the command.
    /// </summary>
    public required string ClientAddress { get; init; }

    /// <summary>
    /// The command name.
    /// </summary>
    public required string Command { get; init; }

    /// <summary>
    /// The command arguments.
    /// </summary>
    public required IReadOnlyList<string> Args { get; init; }

    #endregion
    #region Constructors & Builders

    internal MonitorMessage() { }

    #endregion
}
