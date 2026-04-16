// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Information about a function library.
/// </summary>
public sealed class LibraryInfo
{
    internal LibraryInfo(string name, string engine, FunctionInfo[] functions, string? code = null)
    {
        Name = name;
        Engine = engine;
        Functions = functions;
        Code = code;
    }

    /// <summary>
    /// Gets the library name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the engine type (e.g., "LUA").
    /// </summary>
    public string Engine { get; }

    /// <summary>
    /// Gets the functions in the library.
    /// </summary>
    public FunctionInfo[] Functions { get; }

    /// <summary>
    /// Gets the library source code (null if not requested).
    /// </summary>
    public string? Code { get; }
}
