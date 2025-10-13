// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;
using System.Threading.Tasks;

namespace Valkey.Glide.Internals;

/// <summary>
/// Manages PubSub callbacks and message routing between FFI and client instances.
/// </summary>
internal static class PubSubCallbackManager
{
    private static readonly FFI.PubSubMessageCallback NativeCallback = HandlePubSubMessage;
    private static readonly IntPtr NativeCallbackPtr = FFI.CreatePubSubCallbackPtr(NativeCallback);

    /// <summary>
    /// Registers a client for PubSub message callbacks.
    /// </summary>
    /// <param name="clientPtr">The client pointer address used as unique identifier.</param>
    /// <param name="client">The client instance to register.</param>
    internal static void RegisterClient(ulong clientPtr, BaseClient client) => ClientRegistry.RegisterClient(clientPtr, client);

    /// <summary>
    /// Unregisters a client from PubSub message callbacks.
    /// </summary>
    /// <param name="clientPtr">The client pointer address to unregister.</param>
    internal static void UnregisterClient(ulong clientPtr) => ClientRegistry.UnregisterClient(clientPtr);

    /// <summary>
    /// Gets the native callback pointer that can be passed to FFI functions.
    /// </summary>
    /// <returns>A function pointer for the native PubSub callback.</returns>
    internal static IntPtr GetNativeCallbackPtr() => NativeCallbackPtr;

    /// <summary>
    /// Native callback function that receives PubSub messages from the FFI layer.
    /// This function is called from native code and must handle all exceptions.
    /// The callback matches the Rust FFI signature for PubSubCallback.
    /// </summary>
    /// <param name="clientPtr">The client pointer address used as unique identifier.</param>
    /// <param name="pushKind">The type of push notification received.</param>
    /// <param name="messagePtr">Pointer to the raw message bytes.</param>
    /// <param name="messageLen">The length of the message data in bytes.</param>
    /// <param name="channelPtr">Pointer to the raw channel name bytes.</param>
    /// <param name="channelLen">The length of the channel name in bytes.</param>
    /// <param name="patternPtr">Pointer to the raw pattern bytes (null if no pattern).</param>
    /// <param name="patternLen">The length of the pattern in bytes (0 if no pattern).</param>
    private static void HandlePubSubMessage(
        ulong clientPtr,
        FFI.PushKind pushKind,
        IntPtr messagePtr,
        long messageLen,
        IntPtr channelPtr,
        long channelLen,
        IntPtr patternPtr,
        long patternLen)
    {
        DateTime callbackStartTime = DateTime.UtcNow;

        try
        {
            // Find the client instance using the ClientRegistry
            BaseClient? client = ClientRegistry.GetClient(clientPtr);
            if (client == null)
            {
                // Client not found or has been garbage collected
                // Log warning and return - no cleanup needed as FFI handles memory
                Logger.Log(Level.Warn, "PubSubCallback", $"PubSub message received for unknown client pointer: {clientPtr}");
                LogCallbackPerformance(callbackStartTime, clientPtr, "ClientNotFound");
                return;
            }

            // Only process actual message notifications, ignore subscription confirmations
            if (!IsMessageNotification(pushKind))
            {
                // Log subscription/unsubscription events for debugging
                Logger.Log(Level.Debug, "PubSubCallback", $"PubSub notification received: {pushKind} for client {clientPtr}");
                LogCallbackPerformance(callbackStartTime, clientPtr, "SubscriptionNotification");
                return;
            }

            // Marshal the message from FFI callback parameters
            PubSubMessage message;
            try
            {
                message = FFI.MarshalPubSubMessage(
                    pushKind,
                    messagePtr,
                    messageLen,
                    channelPtr,
                    channelLen,
                    patternPtr,
                    patternLen);
            }
            catch (Exception marshalEx)
            {
                Logger.Log(Level.Error, "PubSubCallback", $"Error marshaling PubSub message for client {clientPtr}: {marshalEx.Message}", marshalEx);
                LogCallbackPerformance(callbackStartTime, clientPtr, "MarshalingError");
                return;
            }

            // Process the message asynchronously to avoid blocking the FFI thread pool
            // Use Task.Run with proper error isolation to ensure callback exceptions don't crash the process
            _ = Task.Run(async () =>
            {
                DateTime processingStartTime = DateTime.UtcNow;

                try
                {
                    // Ensure we have a valid client reference before processing
                    // This prevents race conditions during client disposal
                    BaseClient? processingClient = ClientRegistry.GetClient(clientPtr);
                    if (processingClient == null)
                    {
                        Logger.Log(Level.Warn, "PubSubCallback", $"Client {clientPtr} was disposed before message processing could begin");
                        LogMessageProcessingPerformance(processingStartTime, clientPtr, message.Channel, "ClientDisposed");
                        return;
                    }

                    // Process the message through the client's handler
                    processingClient.HandlePubSubMessage(message);

                    LogMessageProcessingPerformance(processingStartTime, clientPtr, message.Channel, "Success");
                }
                catch (ObjectDisposedException)
                {
                    // Client was disposed during processing - this is expected during shutdown
                    Logger.Log(Level.Debug, "PubSubCallback", $"Client {clientPtr} was disposed during message processing");
                    LogMessageProcessingPerformance(processingStartTime, clientPtr, message?.Channel ?? "unknown", "ClientDisposed");
                }
                catch (Exception processingEx)
                {
                    // Isolate processing errors to prevent them from crashing the process
                    // Log detailed error information for debugging
                    Logger.Log(Level.Error, "PubSubCallback",
                        $"Error processing PubSub message for client {clientPtr}, channel '{message?.Channel}': {processingEx.Message}",
                        processingEx);
                    LogMessageProcessingPerformance(processingStartTime, clientPtr, message?.Channel ?? "unknown", "ProcessingError");
                }
            });

            LogCallbackPerformance(callbackStartTime, clientPtr, "Success");
        }
        catch (Exception ex)
        {
            // Log the error but don't let exceptions escape to native code
            // This is the final safety net to prevent FFI callback exceptions from crashing the process
            Logger.Log(Level.Error, "PubSubCallback", $"Critical error in PubSub FFI callback for client {clientPtr}: {ex.Message}", ex);
            LogCallbackPerformance(callbackStartTime, clientPtr, "CriticalError");
        }
    }

    /// <summary>
    /// Logs performance metrics for FFI callback execution.
    /// This helps monitor callback performance and identify potential bottlenecks.
    /// </summary>
    /// <param name="startTime">The time when the callback started executing.</param>
    /// <param name="clientPtr">The client pointer for context.</param>
    /// <param name="result">The result of the callback execution.</param>
    private static void LogCallbackPerformance(DateTime startTime, ulong clientPtr, string result)
    {
        TimeSpan duration = DateTime.UtcNow - startTime;

        // Log warning if callback takes too long (potential FFI thread pool starvation)
        if (duration.TotalMilliseconds > 10) // 10ms threshold for FFI callback
        {
            Logger.Log(Level.Warn, "PubSubCallback",
                $"PubSub FFI callback took {duration.TotalMilliseconds:F2}ms for client {clientPtr} (result: {result}). " +
                "Long callback durations can block the FFI thread pool.");
        }
        else if (duration.TotalMilliseconds > 1) // 1ms threshold for info logging
        {
            Logger.Log(Level.Info, "PubSubCallback",
                $"PubSub FFI callback completed in {duration.TotalMilliseconds:F2}ms for client {clientPtr} (result: {result})");
        }
        else
        {
            Logger.Log(Level.Debug, "PubSubCallback",
                $"PubSub FFI callback completed in {duration.TotalMilliseconds:F2}ms for client {clientPtr} (result: {result})");
        }
    }

    /// <summary>
    /// Logs performance metrics for async message processing.
    /// This helps monitor message processing performance and identify processing bottlenecks.
    /// </summary>
    /// <param name="startTime">The time when message processing started.</param>
    /// <param name="clientPtr">The client pointer for context.</param>
    /// <param name="channel">The channel name for context.</param>
    /// <param name="result">The result of the message processing.</param>
    private static void LogMessageProcessingPerformance(DateTime startTime, ulong clientPtr, string channel, string result)
    {
        TimeSpan duration = DateTime.UtcNow - startTime;

        // Log warning if message processing takes too long
        if (duration.TotalMilliseconds > 100) // 100ms threshold for message processing
        {
            Logger.Log(Level.Warn, "PubSubCallback",
                $"PubSub message processing took {duration.TotalMilliseconds:F2}ms for client {clientPtr}, channel '{channel}' (result: {result}). " +
                "Long processing times may indicate callback or queue performance issues.");
        }
        else if (duration.TotalMilliseconds > 10) // 10ms threshold for info logging
        {
            Logger.Log(Level.Info, "PubSubCallback",
                $"PubSub message processing completed in {duration.TotalMilliseconds:F2}ms for client {clientPtr}, channel '{channel}' (result: {result})");
        }
        else
        {
            Logger.Log(Level.Debug, "PubSubCallback",
                $"PubSub message processing completed in {duration.TotalMilliseconds:F2}ms for client {clientPtr}, channel '{channel}' (result: {result})");
        }
    }

    /// <summary>
    /// Determines if the push notification is an actual message that should be processed.
    /// </summary>
    /// <param name="pushKind">The type of push notification.</param>
    /// <returns>True if this is a message notification, false for subscription confirmations.</returns>
    private static bool IsMessageNotification(FFI.PushKind pushKind) =>
        pushKind switch
        {
            FFI.PushKind.PushMessage => true,      // Regular channel message
            FFI.PushKind.PushPMessage => true,     // Pattern-based message
            FFI.PushKind.PushSMessage => true,     // Sharded channel message
            _ => false                             // All other types are confirmations/notifications
        };
}
