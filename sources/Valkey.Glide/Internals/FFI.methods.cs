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
    /// FFI callback delegate for PubSub message reception.
    /// </summary>
    /// <param name="clientId">The client ID that received the message.</param>
    /// <param name="messagePtr">Pointer to the PubSubMessageInfo structure.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void PubSubMessageCallback(ulong clientId, IntPtr messagePtr);
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
    public static partial void CreateClientFfi(IntPtr config, IntPtr successCallback, IntPtr failureCallback);

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
    public static extern void CreateClientFfi(IntPtr config, IntPtr successCallback, IntPtr failureCallback);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "close_client")]
    public static extern void CloseClientFfi(IntPtr client);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "register_pubsub_callback")]
    public static extern void RegisterPubSubCallbackFfi(IntPtr client, IntPtr callback);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_pubsub_message")]
    public static extern void FreePubSubMessageFfi(IntPtr messagePtr);
#endif
}
