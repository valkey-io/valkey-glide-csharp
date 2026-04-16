// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Information about a function.
/// </summary>
public sealed class FunctionInfo
{
    internal FunctionInfo(string name, string? description, string[] flags)
    {
        Name = name;
        Description = description;
        Flags = flags;
    }

    /// <summary>
    /// Gets the function name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the function description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the function flags (e.g., "no-writes", "allow-oom").
    /// </summary>
    public string[] Flags { get; }
}
