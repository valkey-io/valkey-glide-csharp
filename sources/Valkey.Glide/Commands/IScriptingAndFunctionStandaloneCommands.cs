// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Scripting and function commands for standalone clients.
/// </summary>
/// <seealso href="https://valkey.io/commands#scripting">Valkey – Scripting and Function Commands</seealso>
public interface IScriptingAndFunctionStandaloneCommands : IScriptingAndFunctionBaseCommands
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

    // ===== Function Management =====

    /// <summary>
    /// Deletes a function library by name.
    /// </summary>
    /// <param name="libraryName">The name of the library to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the library does not exist.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FunctionDeleteAsync("mylib");
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionDeleteAsync(
        string libraryName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates a currently executing function that has not written data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if no function is running or if the function has written data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FunctionKillAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionKillAsync(
        CancellationToken cancellationToken = default);

    // ===== Function Persistence =====

    /// <summary>
    /// Creates a binary backup of all loaded functions.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A binary payload containing all loaded functions.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// byte[] backup = await client.FunctionDumpAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<byte[]> FunctionDumpAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup using default policy (APPEND).
    /// </summary>
    /// <param name="payload">The binary payload from FunctionDump.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if restoration fails (e.g., library conflict with APPEND policy).</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FunctionRestoreAsync(backup);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionRestoreAsync(
        byte[] payload,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup with specified policy.
    /// </summary>
    /// <param name="payload">The binary payload from FunctionDump.</param>
    /// <param name="policy">The restore policy (APPEND, FLUSH, or REPLACE).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if restoration fails.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FunctionRestoreAsync(backup, FunctionRestorePolicy.Replace);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        CancellationToken cancellationToken = default);
}
