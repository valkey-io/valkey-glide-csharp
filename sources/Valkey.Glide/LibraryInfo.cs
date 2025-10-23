// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Information about a function library.
/// </summary>
/// <param name="name">The library name.</param>
/// <param name="engine">The engine type (e.g., "LUA").</param>
/// <param name="functions">The functions in the library.</param>
/// <param name="code">The library source code (null if not requested).</param>
public sealed class LibraryInfo(string name, string engine, FunctionInfo[] functions, string? code = null)
{
    /// <summary>
    /// Gets the library name.
    /// </summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    /// <summary>
    /// Gets the engine type (e.g., "LUA").
    /// </summary>
    public string Engine { get; } = engine ?? throw new ArgumentNullException(nameof(engine));

    /// <summary>
    /// Gets the functions in the library.
    /// </summary>
    public FunctionInfo[] Functions { get; } = functions ?? throw new ArgumentNullException(nameof(functions));

    /// <summary>
    /// Gets the library source code (null if not requested).
    /// </summary>
    public string? Code { get; } = code;
}
