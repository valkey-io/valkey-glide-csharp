// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClient : IScriptingAndFunctionStandaloneCommands
{
    // ===== Function Inspection =====

    /// <inheritdoc/>
    public async Task<LibraryInfo[]> FunctionListAsync(
        FunctionListQuery? query = null,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionListAsync(query));
    }

    /// <inheritdoc/>
    public async Task<FunctionStatsResult> FunctionStatsAsync(
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionStatsAsync());
    }

    // ===== Function Management =====

    /// <inheritdoc/>
    public async Task FunctionDeleteAsync(
        string libraryName,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FunctionDeleteAsync(libraryName));
    }

    /// <inheritdoc/>
    public async Task FunctionKillAsync(
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FunctionKillAsync());
    }

    // ===== Function Persistence =====

    /// <inheritdoc/>
    public async Task<byte[]> FunctionDumpAsync(
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionDumpAsync());
    }

    /// <inheritdoc/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FunctionRestoreAsync(payload, null));
    }

    /// <inheritdoc/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FunctionRestoreAsync(payload, policy));
    }
}
