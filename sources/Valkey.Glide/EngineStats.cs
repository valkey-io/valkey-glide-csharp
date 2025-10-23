// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Statistics for a specific engine.
/// </summary>
/// <param name="language">The engine language (e.g., "LUA").</param>
/// <param name="functionCount">The number of loaded functions.</param>
/// <param name="libraryCount">The number of loaded libraries.</param>
public sealed class EngineStats(string language, long functionCount, long libraryCount)
{
    /// <summary>
    /// Gets the engine language (e.g., "LUA").
    /// </summary>
    public string Language { get; } = language ?? throw new ArgumentNullException(nameof(language));

    /// <summary>
    /// Gets the number of loaded functions.
    /// </summary>
    public long FunctionCount { get; } = functionCount;

    /// <summary>
    /// Gets the number of loaded libraries.
    /// </summary>
    public long LibraryCount { get; } = libraryCount;
}
