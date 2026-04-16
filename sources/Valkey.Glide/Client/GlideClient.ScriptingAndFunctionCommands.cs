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
}
