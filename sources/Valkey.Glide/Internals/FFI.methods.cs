// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

// check https://stackoverflow.com/a/77455034 if you're getting analyzer error (using is unnecessary)
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;

namespace Valkey.Glide.Internals;

internal partial class FFI
{
    /// <summary>
    /// FFI callback delegate for PubSub message reception matching the Rust FFI signature.
    /// </summary>
    /// <param name="pushKind">The type of push notification received.</param>
    /// <param name="messagePtr">Pointer to the raw message bytes.</param>
    /// <param name="messageLen">The length of the message data in bytes.</param>
    /// <param name="channelPtr">Pointer to the raw channel name bytes.</param>
    /// <param name="channelLen">The length of the channel name in bytes.</param>
    /// <param name="patternPtr">Pointer to the raw pattern bytes (null if no pattern).</param>
    /// <param name="patternLen">The length of the pattern in bytes (0 if no pattern).</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void PubSubMessageCallback(
        uint pushKind,
        IntPtr messagePtr,
        ulong messageLen,
        IntPtr channelPtr,
        ulong channelLen,
        IntPtr patternPtr,
        ulong patternLen);

#if NET8_0_OR_GREATER
    [LibraryImport("libglide_rs", EntryPoint = "command")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CommandFfi(IntPtr client, ulong index, IntPtr cmdInfo, IntPtr routeInfo);

    [LibraryImport("libglide_rs", EntryPoint = "batch")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BatchFfi(IntPtr client, ulong index, IntPtr batch, [MarshalAs(UnmanagedType.U1)] bool raiseOnError, IntPtr opts);

    [LibraryImport("libglide_rs", EntryPoint = "free_response")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void FreeResponse(IntPtr response);

    [LibraryImport("libglide_rs", EntryPoint = "create_client")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CreateClientFfi(IntPtr config, IntPtr successCallback, IntPtr failureCallback, IntPtr pubsubCallback);

    [LibraryImport("libglide_rs", EntryPoint = "close_client")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CloseClientFfi(IntPtr client);

    [LibraryImport("libglide_rs", EntryPoint = "store_script")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr StoreScriptFfi(IntPtr scriptPtr, UIntPtr scriptLen);

    [LibraryImport("libglide_rs", EntryPoint = "drop_script")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr DropScriptFfi(IntPtr hashPtr, UIntPtr hashLen);

    [LibraryImport("libglide_rs", EntryPoint = "free_script_hash_buffer")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void FreeScriptHashBuffer(IntPtr hashBuffer);

    [LibraryImport("libglide_rs", EntryPoint = "free_drop_script_error")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void FreeDropScriptError(IntPtr errorBuffer);

    [LibraryImport("libglide_rs", EntryPoint = "invoke_script")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void InvokeScriptFfi(
        IntPtr client,
        ulong index,
        IntPtr hash,
        ulong keysCount,
        IntPtr keys,
        IntPtr keysLen,
        ulong argsCount,
        IntPtr args,
        IntPtr argsLen,
        IntPtr routeInfo,
        ulong routeInfoLen);

    [LibraryImport("libglide_rs", EntryPoint = "request_cluster_scan")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void RequestClusterScanFfi(IntPtr client, ulong index, IntPtr cursor, ulong argCount, IntPtr args, IntPtr argLengths);

    [LibraryImport("libglide_rs", EntryPoint = "remove_cluster_scan_cursor")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void RemoveClusterScanCursorFfi(IntPtr cursorId);

    [LibraryImport("libglide_rs", EntryPoint = "update_connection_password")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void UpdateConnectionPasswordFfi(IntPtr client, ulong index, IntPtr password, [MarshalAs(UnmanagedType.U1)] bool immediateAuth);

    [LibraryImport("libglide_rs", EntryPoint = "refresh_iam_token")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void RefreshIamTokenFfi(IntPtr client, ulong index);

    [LibraryImport("libglide_rs", EntryPoint = "init_otel")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr InitOpenTelemetryFfi(IntPtr config);

    [LibraryImport("libglide_rs", EntryPoint = "create_otel_span")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr CreateOpenTelemetrySpanFfi(uint requestType);

    [LibraryImport("libglide_rs", EntryPoint = "create_otel_span_with_parent")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr CreateOpenTelemetrySpanWithParentFfi(uint requestType, IntPtr parentSpanPtr);

    [LibraryImport("libglide_rs", EntryPoint = "create_batch_otel_span")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr CreateBatchOpenTelemetrySpanFfi();

    [LibraryImport("libglide_rs", EntryPoint = "create_batch_otel_span_with_parent")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr CreateBatchOpenTelemetrySpanWithParentFfi(IntPtr parentSpanPtr);

    [LibraryImport("libglide_rs", EntryPoint = "drop_otel_span")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DropOpenTelemetrySpanFfi(IntPtr spanPtr);
#else
    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "command")]
    public static extern void CommandFfi(IntPtr client, ulong index, IntPtr cmdInfo, IntPtr routeInfo);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "batch")]
    public static extern void BatchFfi(IntPtr client, ulong index, IntPtr batch, [MarshalAs(UnmanagedType.U1)] bool raiseOnError, IntPtr opts);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_response")]
    public static extern void FreeResponse(IntPtr response);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "create_client")]
    public static extern void CreateClientFfi(IntPtr config, IntPtr successCallback, IntPtr failureCallback, IntPtr pubsubCallback);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "close_client")]
    public static extern void CloseClientFfi(IntPtr client);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "store_script")]
    public static extern IntPtr StoreScriptFfi(IntPtr scriptPtr, UIntPtr scriptLen);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "drop_script")]
    public static extern IntPtr DropScriptFfi(IntPtr hashPtr, UIntPtr hashLen);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_script_hash_buffer")]
    public static extern void FreeScriptHashBuffer(IntPtr hashBuffer);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_drop_script_error")]
    public static extern void FreeDropScriptError(IntPtr errorBuffer);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "invoke_script")]
    public static extern void InvokeScriptFfi(
        IntPtr client,
        ulong index,
        IntPtr hash,
        ulong keysCount,
        IntPtr keys,
        IntPtr keysLen,
        ulong argsCount,
        IntPtr args,
        IntPtr argsLen,
        IntPtr routeInfo,
        ulong routeInfoLen);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "request_cluster_scan")]
    public static extern void RequestClusterScanFfi(IntPtr client, ulong index, IntPtr cursor, ulong argCount, IntPtr args, IntPtr argLengths);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "remove_cluster_scan_cursor")]
    public static extern void RemoveClusterScanCursorFfi(IntPtr cursorId);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "update_connection_password")]
    public static extern void UpdateConnectionPasswordFfi(IntPtr client, ulong index, IntPtr password, [MarshalAs(UnmanagedType.U1)] bool immediateAuth);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "refresh_iam_token")]
    public static extern void RefreshIamTokenFfi(IntPtr client, ulong index);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "init_otel")]
    public static extern IntPtr InitOpenTelemetryFfi(IntPtr config);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "create_otel_span")]
    public static extern IntPtr CreateOpenTelemetrySpanFfi(uint requestType);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "create_otel_span_with_parent")]
    public static extern IntPtr CreateOpenTelemetrySpanWithParentFfi(uint requestType, IntPtr parentSpanPtr);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "create_batch_otel_span")]
    public static extern IntPtr CreateBatchOpenTelemetrySpanFfi();

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "create_batch_otel_span_with_parent")]
    public static extern IntPtr CreateBatchOpenTelemetrySpanWithParentFfi(IntPtr parentSpanPtr);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "drop_otel_span")]
    public static extern void DropOpenTelemetrySpanFfi(IntPtr spanPtr);
#endif
}
