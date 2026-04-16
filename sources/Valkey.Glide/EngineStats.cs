// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Statistics for a specific engine.
/// </summary>
public sealed class EngineStats
{
    internal EngineStats(string language, long functionCount, long libraryCount)
    {
        Language = language;
        FunctionCount = functionCount;
        LibraryCount = libraryCount;
    }

    /// <summary>
    /// Gets the engine language (e.g., "LUA").
    /// </summary>
    public string Language { get; }

    /// <summary>
    /// Gets the number of loaded functions.
    /// </summary>
    public long FunctionCount { get; }

    /// <summary>
    /// Gets the number of loaded libraries.
    /// </summary>
    public long LibraryCount { get; }
}
