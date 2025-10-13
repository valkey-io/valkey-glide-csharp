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
    /// <param name="clientPtr">The client pointer address used as unique identifier.</param>
    /// <param name="pushKind">The type of push notification received.</param>
    /// <param name="messagePtr">Pointer to the raw message bytes.</param>
    /// <param name="messageLen">The length of the message data in bytes.</param>
    /// <param name="channelPtr">Pointer to the raw channel name bytes.</param>
    /// <param name="channelLen">The length of the channel name in bytes.</param>
    /// <param name="patternPtr">Pointer to the raw pattern bytes (null if no pattern).</param>
    /// <param name="patternLen">The length of the pattern in bytes (0 if no pattern).</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void PubSubMessageCallback(
        ulong clientPtr,
        PushKind pushKind,
        IntPtr messagePtr,
        long messageLen,
        IntPtr channelPtr,
        long channelLen,
        IntPtr patternPtr,
        long patternLen);
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

    [LibraryImport("libglide_rs", EntryPoint = "register_pubsub_callback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void RegisterPubSubCallbackFfi(IntPtr client, IntPtr callback);

    [LibraryImport("libglide_rs", EntryPoint = "free_pubsub_message")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void FreePubSubMessageFfi(IntPtr messagePtr);


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

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "register_pubsub_callback")]
    public static extern void RegisterPubSubCallbackFfi(IntPtr client, IntPtr callback);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_pubsub_message")]
    public static extern void FreePubSubMessageFfi(IntPtr messagePtr);


#endif
}
