// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Options for parameterized script execution.
/// </summary>
public sealed class ScriptOptions
{
    /// <summary>
    /// Gets or sets the keys to pass to the script (KEYS array).
    /// </summary>
    public string[]? Keys { get; set; }

    /// <summary>
    /// Gets or sets the arguments to pass to the script (ARGV array).
    /// </summary>
    public string[]? Args { get; set; }

    /// <summary>
    /// Creates a new ScriptOptions instance.
    /// </summary>
    public ScriptOptions()
    {
    }

    /// <summary>
    /// Sets the keys for the script.
    /// </summary>
    /// <param name="keys">The keys to pass to the script.</param>
    /// <returns>This ScriptOptions instance for method chaining.</returns>
    public ScriptOptions WithKeys(params string[] keys)
    {
        Keys = keys;
        return this;
    }

    /// <summary>
    /// Sets the arguments for the script.
    /// </summary>
    /// <param name="args">The arguments to pass to the script.</param>
    /// <returns>This ScriptOptions instance for method chaining.</returns>
    public ScriptOptions WithArgs(params string[] args)
    {
        Args = args;
        return this;
    }
}
