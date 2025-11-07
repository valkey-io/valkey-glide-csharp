// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Information about a currently running script.
/// </summary>
/// <param name="name">The script name.</param>
/// <param name="command">The command being executed.</param>
/// <param name="args">The command arguments.</param>
/// <param name="duration">The execution duration.</param>
public sealed class RunningScriptInfo(string name, string command, string[] args, TimeSpan duration)
{
    /// <summary>
    /// Gets the script name.
    /// </summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    /// Gets the command being executed.
    /// </summary>
    public string Command { get; } = command ?? throw new ArgumentNullException(nameof(command));

    /// <summary>
    /// Gets the command arguments.
    /// </summary>
    public string[] Args { get; } = args ?? throw new ArgumentNullException(nameof(args));

    /// <summary>
    /// Gets the execution duration.
    /// </summary>
    public TimeSpan Duration { get; } = duration;
}
