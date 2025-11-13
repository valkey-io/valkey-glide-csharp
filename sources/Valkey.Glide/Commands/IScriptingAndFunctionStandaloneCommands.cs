// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Scripting and function commands specific to standalone clients.
/// </summary>
public interface IScriptingAndFunctionStandaloneCommands : IScriptingAndFunctionBaseCommands
{
    // ===== Function Inspection =====

    /// <summary>
    /// Lists all loaded function libraries.
    /// </summary>
    /// <param name="query">Optional query parameters to filter results.</param>
    /// <param name="flags">The flags to use for this operation.</param>
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
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns statistics about loaded functions.
    /// </summary>
    /// <param name="flags">The flags to use for this operation.</param>
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
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    // ===== Function Management =====

    /// <summary>
    /// Deletes a function library by name.
    /// </summary>
    /// <param name="libraryName">The name of the library to delete.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>"OK" if the library was deleted.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the library does not exist.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// string result = await client.FunctionDeleteAsync("mylib");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> FunctionDeleteAsync(
        string libraryName,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates a currently executing function that has not written data.
    /// </summary>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>"OK" if the function was killed.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if no function is running or if the function has written data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// string result = await client.FunctionKillAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> FunctionKillAsync(
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    // ===== Function Persistence =====

    /// <summary>
    /// Creates a binary backup of all loaded functions.
    /// </summary>
    /// <param name="flags">The flags to use for this operation.</param>
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
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup using default policy (APPEND).
    /// </summary>
    /// <param name="payload">The binary payload from FunctionDump.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>"OK" if the functions were restored.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if restoration fails (e.g., library conflict with APPEND policy).</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// string result = await client.FunctionRestoreAsync(backup);
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> FunctionRestoreAsync(
        byte[] payload,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup with specified policy.
    /// </summary>
    /// <param name="payload">The binary payload from FunctionDump.</param>
    /// <param name="policy">The restore policy (APPEND, FLUSH, or REPLACE).</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>"OK" if the functions were restored.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if restoration fails.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// string result = await client.FunctionRestoreAsync(backup, FunctionRestorePolicy.Replace);
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);
}
