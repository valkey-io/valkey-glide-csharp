// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Query parameters for listing functions.
/// </summary>
public sealed class FunctionListQuery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionListQuery"/> class.
    /// </summary>
    public FunctionListQuery()
    {
    }

    /// <summary>
    /// Gets or sets the library name filter (null for all libraries).
    /// </summary>
    public string? LibraryName { get; set; }

    /// <summary>
    /// Gets or sets whether to include source code in results.
    /// </summary>
    public bool WithCode { get; set; }

    /// <summary>
    /// Sets the library name filter.
    /// </summary>
    /// <param name="libraryName">The library name to filter by.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public FunctionListQuery ForLibrary(string libraryName)
    {
        LibraryName = libraryName;
        return this;
    }

    /// <summary>
    /// Includes source code in the results.
    /// </summary>
    /// <returns>This instance for fluent chaining.</returns>
    public FunctionListQuery IncludeCode()
    {
        WithCode = true;
        return this;
    }
}
