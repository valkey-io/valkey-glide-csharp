// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;

namespace Valkey.Glide.Internals;

/// <summary>
/// Thread-safe registry for mapping FFI client pointers to C# client instances.
/// Uses WeakReference to prevent memory leaks from the registry holding strong references.
/// </summary>
internal static class ClientRegistry
{
    private static readonly ConcurrentDictionary<ulong, WeakReference<BaseClient>> _clients = new();

    /// <summary>
    /// Registers a client instance with the specified client pointer address.
    /// </summary>
    /// <param name="clientPtr">The client pointer address used as the unique identifier.</param>
    /// <param name="client">The client instance to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    internal static void RegisterClient(ulong clientPtr, BaseClient client)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        _clients[clientPtr] = new WeakReference<BaseClient>(client);
    }

    /// <summary>
    /// Retrieves a client instance by its pointer address.
    /// </summary>
    /// <param name="clientPtr">The client pointer address to look up.</param>
    /// <returns>The client instance if found and still alive, otherwise null.</returns>
    internal static BaseClient? GetClient(ulong clientPtr)
    {
        if (_clients.TryGetValue(clientPtr, out WeakReference<BaseClient>? clientRef) &&
            clientRef.TryGetTarget(out BaseClient? client))
        {
            return client;
        }

        // Client not found or has been garbage collected
        // Clean up the dead reference if it exists
        if (clientRef != null && !clientRef.TryGetTarget(out _))
        {
            _clients.TryRemove(clientPtr, out _);
        }

        return null;
    }

    /// <summary>
    /// Unregisters a client from the registry.
    /// </summary>
    /// <param name="clientPtr">The client pointer address to unregister.</param>
    /// <returns>True if the client was found and removed, false otherwise.</returns>
    internal static bool UnregisterClient(ulong clientPtr)
    {
        return _clients.TryRemove(clientPtr, out _);
    }

    /// <summary>
    /// Gets the current number of registered clients (including dead references).
    /// This is primarily for testing and diagnostics.
    /// </summary>
    internal static int Count => _clients.Count;

    /// <summary>
    /// Cleans up dead weak references from the registry.
    /// This method can be called periodically to prevent memory leaks from accumulated dead references.
    /// </summary>
    internal static void CleanupDeadReferences()
    {
        var keysToRemove = new List<ulong>();

        foreach (var kvp in _clients)
        {
            if (!kvp.Value.TryGetTarget(out _))
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            _clients.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Clears all registered clients from the registry.
    /// This is primarily for testing purposes.
    /// </summary>
    internal static void Clear()
    {
        _clients.Clear();
    }
}
