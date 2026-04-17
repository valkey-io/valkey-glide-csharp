// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Options for listing functions.
/// </summary>
public readonly struct FunctionListOptions
{
    /// <summary>
    /// Gets the library name filter (null for all libraries).
    /// </summary>
    public string? LibraryName { get; init; }

    /// <summary>
    /// Gets whether to include source code in results.
    /// </summary>
    public bool WithCode { get; init; }
}
