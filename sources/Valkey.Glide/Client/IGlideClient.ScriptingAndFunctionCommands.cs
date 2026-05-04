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
    /// <seealso href="https://valkey.io/commands/function-list/">Valkey commands – FUNCTION LIST</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="options">Optional query parameters to filter results.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>An array of <see cref="LibraryInfo"/> describing each loaded library.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var libraries = await client.FunctionListAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<LibraryInfo[]> FunctionListAsync(
        FunctionListOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns statistics about loaded functions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-stats/">Valkey commands – FUNCTION STATS</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="FunctionStatsResult"/> with engine statistics and information about any running script.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var stats = await client.FunctionStatsAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<FunctionStatsResult> FunctionStatsAsync(
        CancellationToken cancellationToken = default);
}
