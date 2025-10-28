// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Information about a function.
/// </summary>
/// <param name="name">The function name.</param>
/// <param name="description">The function description.</param>
/// <param name="flags">The function flags (e.g., "no-writes", "allow-oom").</param>
public sealed class FunctionInfo(string name, string? description, string[] flags)
{
    /// <summary>
    /// Gets the function name.
    /// </summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    /// Gets the function description.
    /// </summary>
    public string? Description { get; } = description;

    /// <summary>
    /// Gets the function flags (e.g., "no-writes", "allow-oom").
    /// </summary>
    public string[] Flags { get; } = flags ?? throw new ArgumentNullException(nameof(flags));
}
