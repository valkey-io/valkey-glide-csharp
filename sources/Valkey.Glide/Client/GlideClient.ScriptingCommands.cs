// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClient
{
    // ===== Function Inspection =====

    /// <inheritdoc/>
    public async Task<LibraryInfo[]> FunctionListAsync(
        FunctionListQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionListAsync(query));
    }

    /// <inheritdoc/>
    public async Task<FunctionStatsResult> FunctionStatsAsync(
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionStatsAsync());
    }

    // ===== Function Management =====

    /// <inheritdoc/>
    public async Task FunctionDeleteAsync(
        string libraryName,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionDeleteAsync(libraryName));
    }

    /// <inheritdoc/>
    public async Task FunctionKillAsync(
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionKillAsync());
    }

    // ===== Function Persistence =====

    /// <inheritdoc/>
    public async Task<byte[]> FunctionDumpAsync(
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionDumpAsync());
    }

    /// <inheritdoc/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, null));
    }

    /// <inheritdoc/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, policy));
    }
}
