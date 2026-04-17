// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Information about a currently running script.
/// </summary>
public sealed class RunningScriptInfo
{
    internal RunningScriptInfo(string name, string command, string[] args, TimeSpan duration)
    {
        Name = name;
        Command = command;
        Args = args;
        Duration = duration;
    }

    /// <summary>
    /// Gets the script name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the command being executed.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Gets the command arguments.
    /// </summary>
    public string[] Args { get; }

    /// <summary>
    /// Gets the execution duration.
    /// </summary>
    public TimeSpan Duration { get; }
}
