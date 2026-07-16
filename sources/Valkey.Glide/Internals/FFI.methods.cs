// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.CompilerServices;
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

    [LibraryImport("libglide_rs", EntryPoint = "command")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CommandFfi(IntPtr client, ulong index, IntPtr cmdInfo, IntPtr routeInfo);

    [LibraryImport("libglide_rs", EntryPoint = "batch")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void BatchFfi(IntPtr client, ulong index, IntPtr batch, [MarshalAs(UnmanagedType.U1)] bool raiseOnError, IntPtr opts);

    [LibraryImport("libglide_rs", EntryPoint = "free_response")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void FreeResponse(IntPtr responsePtr);

    [LibraryImport("libglide_rs", EntryPoint = "free_string")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void FreeString(IntPtr strPtr);

    [LibraryImport("libglide_rs", EntryPoint = "create_client")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CreateClientFfi(IntPtr config, IntPtr successCallback, IntPtr failureCallback, IntPtr pubsubCallback, IntPtr addressResolverCallback);

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

    [LibraryImport("libglide_rs", EntryPoint = "invoke_script")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void ScriptInvokeFfi(
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

    [LibraryImport("libglide_rs", EntryPoint = "get_statistics")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial Statistics GetStatisticsFfi();

    [LibraryImport("libglide_rs", EntryPoint = "get_cache_metrics")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void GetCacheMetricsFfi(IntPtr client, ulong index, uint metricsType);

    #region OpenTelemetry

    [LibraryImport("libglide_rs", EntryPoint = "init_otel")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr InitOpenTelemetryFfi(IntPtr config);

    [LibraryImport("libglide_rs", EntryPoint = "create_otel_span")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr CreateOpenTelemetrySpanFfi(uint requestType);

    [LibraryImport("libglide_rs", EntryPoint = "create_batch_otel_span")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr CreateBatchOpenTelemetrySpanFfi();

    [LibraryImport("libglide_rs", EntryPoint = "drop_otel_span")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void DropOpenTelemetrySpanFfi(IntPtr spanPtr);

    #endregion
    #region Monitor

    /// <summary>
    /// FFI callback delegate for monitor message reception matching the Rust FFI signature.
    /// </summary>
    /// <param name="timestamp">Unix timestamp when the command was processed.</param>
    /// <param name="db">Database number the command was issued against.</param>
    /// <param name="clientAddrPtr">Pointer to the client address that issued the command.</param>
    /// <param name="clientAddrLen">Length of the client address in bytes.</param>
    /// <param name="commandPtr">Pointer to the command name.</param>
    /// <param name="commandLen">Length of the command name in bytes.</param>
    /// <param name="argsCount">Number of command arguments.</param>
    /// <param name="argsPtrs">Pointer to an array of pointers to argument bytes.</param>
    /// <param name="argsLens">Pointer to an array of argument lengths.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void MonitorMessageCallback(
        double timestamp,
        ushort database,
        IntPtr clientAddrPtr,
        long clientAddrLen,
        IntPtr commandPtr,
        long commandLen,
        long argsCount,
        IntPtr argsPtrs,
        IntPtr argsLens);

    [LibraryImport("libglide_rs", EntryPoint = "create_monitor_client")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr CreateMonitorClientFfi(IntPtr config, IntPtr monitorCallback);

    [LibraryImport("libglide_rs", EntryPoint = "close_monitor_client")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void CloseMonitorClientFfi(IntPtr clientPtr);

    [LibraryImport("libglide_rs", EntryPoint = "free_monitor_connection_response")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void FreeMonitorConnectionResponseFfi(IntPtr responsePtr);

    #endregion
}
