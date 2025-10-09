// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Valkey.Glide.Internals;

/// <summary>
/// Manages PubSub callbacks and message routing between FFI and client instances.
/// </summary>
internal static class PubSubCallbackManager
{
    private static readonly ConcurrentDictionary<ulong, WeakReference<BaseClient>> _clients = new();
    private static readonly FFI.PubSubMessageCallback _nativeCallback = HandlePubSubMessage;
    private static readonly IntPtr _nativeCallbackPtr = FFI.CreatePubSubCallbackPtr(_nativeCallback);

    /// <summary>
    /// Registers a client for PubSub message callbacks.
    /// </summary>
    /// <param name="clientId">The unique client ID.</param>
    /// <param name="client">The client instance to register.</param>
    internal static void RegisterClient(ulong clientId, BaseClient client)
    {
        _clients[clientId] = new WeakReference<BaseClient>(client);
    }

    /// <summary>
    /// Unregisters a client from PubSub message callbacks.
    /// </summary>
    /// <param name="clientId">The unique client ID to unregister.</param>
    internal static void UnregisterClient(ulong clientId)
    {
        _clients.TryRemove(clientId, out _);
    }

    /// <summary>
    /// Gets the native callback pointer that can be passed to FFI functions.
    /// </summary>
    /// <returns>A function pointer for the native PubSub callback.</returns>
    internal static IntPtr GetNativeCallbackPtr()
    {
        return _nativeCallbackPtr;
    }

    /// <summary>
    /// Native callback function that receives PubSub messages from the FFI layer.
    /// This function is called from native code and must handle all exceptions.
    /// </summary>
    /// <param name="clientId">The client ID that received the message.</param>
    /// <param name="messagePtr">Pointer to the native PubSubMessageInfo structure.</param>
    private static void HandlePubSubMessage(ulong clientId, IntPtr messagePtr)
    {
        try
        {
            // Find the client instance
            if (!_clients.TryGetValue(clientId, out WeakReference<BaseClient>? clientRef) ||
                !clientRef.TryGetTarget(out BaseClient? client))
            {
                // Client not found or has been garbage collected
                // Free the message and return
                if (messagePtr != IntPtr.Zero)
                {
                    FFI.FreePubSubMessageFfi(messagePtr);
                }
                return;
            }

            // Marshal the message from native memory
            PubSubMessage message = FFI.MarshalPubSubMessage(messagePtr);

            // Route the message to the client's PubSub handler
            client.HandlePubSubMessage(message);
        }
        catch (Exception ex)
        {
            // Log the error but don't let exceptions escape to native code
            // In a production environment, this should use proper logging
            Console.Error.WriteLine($"Error handling PubSub message for client {clientId}: {ex}");
        }
        finally
        {
            // Always free the native message memory
            if (messagePtr != IntPtr.Zero)
            {
                try
                {
                    FFI.FreePubSubMessageFfi(messagePtr);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error freeing PubSub message memory: {ex}");
                }
            }
        }
    }
}
