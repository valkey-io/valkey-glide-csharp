// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Options for cluster script execution with routing support.
/// </summary>
public sealed class ClusterScriptOptions
{
    /// <summary>
    /// Gets or sets the arguments to pass to the script (ARGV array).
    /// </summary>
    public string[]? Args { get; set; }

    /// <summary>
    /// Gets or sets the routing configuration for cluster execution.
    /// </summary>
    public Route? Route { get; set; }

    /// <summary>
    /// Creates a new ClusterScriptOptions instance.
    /// </summary>
    public ClusterScriptOptions()
    {
    }

    /// <summary>
    /// Sets the arguments for the script.
    /// </summary>
    /// <param name="args">The arguments to pass to the script.</param>
    /// <returns>This ClusterScriptOptions instance for method chaining.</returns>
    public ClusterScriptOptions WithArgs(params string[] args)
    {
        Args = args;
        return this;
    }

    /// <summary>
    /// Sets the routing configuration.
    /// </summary>
    /// <param name="route">The routing configuration for cluster execution.</param>
    /// <returns>This ClusterScriptOptions instance for method chaining.</returns>
    public ClusterScriptOptions WithRoute(Route route)
    {
        Route = route;
        return this;
    }
}
