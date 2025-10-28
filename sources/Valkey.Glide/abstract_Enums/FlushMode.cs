// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Flush mode for script and function cache operations.
/// </summary>
public enum FlushMode
{
    /// <summary>
    /// Flush synchronously - waits for flush to complete before returning.
    /// </summary>
    Sync,

    /// <summary>
    /// Flush asynchronously - returns immediately while flush continues in background.
    /// </summary>
    Async
}
