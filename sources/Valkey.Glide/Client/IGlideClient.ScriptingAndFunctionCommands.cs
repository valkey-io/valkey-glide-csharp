// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Scripting and function commands for Valkey GLIDE standalone clients.
/// </summary>
/// <remarks>
/// These methods are GLIDE-specific and not available in StackExchange.Redis.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#scripting">Valkey – Scripting and Function Commands</seealso>
public partial interface IGlideClient
{
    // ===== Function Inspection =====

    /// <summary>
    /// Lists all loaded function libraries.
    /// </summary>
    /// <param name="query">Optional query parameters to filter results.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An array of library information.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// LibraryInfo[] libraries = await client.FunctionListAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<LibraryInfo[]> FunctionListAsync(
        FunctionListQuery? query = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns statistics about loaded functions.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Function statistics including engine stats and running script information.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// FunctionStatsResult stats = await client.FunctionStatsAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<FunctionStatsResult> FunctionStatsAsync(
        CancellationToken cancellationToken = default);
}
