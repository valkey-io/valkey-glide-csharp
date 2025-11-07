// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Statistics about loaded functions.
/// </summary>
/// <param name="engines">Engine statistics by engine name.</param>
/// <param name="runningScript">Information about the currently running script (null if none).</param>
public sealed class FunctionStatsResult(Dictionary<string, EngineStats> engines, RunningScriptInfo? runningScript = null)
{
    /// <summary>
    /// Gets engine statistics by engine name.
    /// </summary>
    public Dictionary<string, EngineStats> Engines { get; } = engines ?? throw new ArgumentNullException(nameof(engines));

    /// <summary>
    /// Gets information about the currently running script (null if none).
    /// </summary>
    public RunningScriptInfo? RunningScript { get; } = runningScript;
}
