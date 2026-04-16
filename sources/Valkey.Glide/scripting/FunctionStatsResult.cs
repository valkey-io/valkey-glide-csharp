// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Statistics about loaded functions.
/// </summary>
public sealed class FunctionStatsResult
{
    internal FunctionStatsResult(Dictionary<string, EngineStats> engines, RunningScriptInfo? runningScript = null)
    {
        Engines = engines;
        RunningScript = runningScript;
    }

    /// <summary>
    /// Gets engine statistics by engine name.
    /// </summary>
    public Dictionary<string, EngineStats> Engines { get; }

    /// <summary>
    /// Gets information about the currently running script (null if none).
    /// </summary>
    public RunningScriptInfo? RunningScript { get; }
}
