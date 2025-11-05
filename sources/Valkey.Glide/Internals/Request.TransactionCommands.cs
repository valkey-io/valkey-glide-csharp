// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    /// <summary>
    /// Creates a command to watch keys for conditional execution of a transaction.
    /// </summary>
    /// <param name="keys">The keys to watch.</param>
    /// <returns>A command that watches the specified keys.</returns>
    public static Cmd<string, string> Watch(ValkeyKey[] keys)
    {
        GlideString[] args = [.. keys.Select(k => (GlideString)k)];
        return new(RequestType.Watch, args, false, response => response);
    }

    /// <summary>
    /// Creates a command to flush all previously watched keys for a transaction.
    /// </summary>
    /// <returns>A command that unwatches all previously watched keys.</returns>
    public static Cmd<string, string> Unwatch()
        => new(RequestType.UnWatch, [], false, response => response);
}