﻿// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

// check https://stackoverflow.com/a/77455034 if you're getting analyzer error (using is unnecessary)
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;

namespace Valkey.Glide.Internals;

internal partial class FFI
{
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

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "store_script")]
    public static extern IntPtr StoreScriptFfi(IntPtr scriptPtr, UIntPtr scriptLen);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "drop_script")]
    public static extern IntPtr DropScriptFfi(IntPtr hashPtr, UIntPtr hashLen);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_script_hash_buffer")]
    public static extern void FreeScriptHashBuffer(IntPtr hashBuffer);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_drop_script_error")]
    public static extern void FreeDropScriptError(IntPtr errorBuffer);
#endif
}
