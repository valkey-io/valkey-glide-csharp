// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a command received through the <c>MONITOR</c> stream.
/// </summary>
/// <seealso href="https://valkey.io/commands/monitor/">Valkey commands - MONITOR</seealso>
public sealed class MonitorMessage
{
    #region Public Properties

    /// <summary>
    /// The server timestamp when the command was processed.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// The database number the command was issued against.
    /// </summary>
    public long Database { get; }

    /// <summary>
    /// The address of the client that issued the command.
    /// </summary>
    public string ClientAddress { get; }

    /// <summary>
    /// The command name.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// The command arguments.
    /// </summary>
    public IReadOnlyList<string> Args { get; }

    #endregion
    #region Constructors

    internal MonitorMessage(DateTimeOffset timestamp, long database, string clientAddress, string command, IReadOnlyList<string> args)
    {
        Timestamp = timestamp;
        Database = database;
        ClientAddress = clientAddress;
        Command = command;
        Args = args;
    }

    #endregion
}
