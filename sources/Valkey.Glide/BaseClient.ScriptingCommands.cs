// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

using static Valkey.Glide.Internals.ResponseHandler;

namespace Valkey.Glide;

public abstract partial class BaseClient : IScriptingAndFunctionBaseCommands
{
    // ===== Script Execution =====

    /// <inheritdoc/>
    public async Task<ValkeyResult> InvokeScriptAsync(
        Script script,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await InvokeScriptInternalAsync(script.Hash, null, null, null);
    }

    /// <inheritdoc/>
    public async Task<ValkeyResult> InvokeScriptAsync(
        Script script,
        ScriptOptions options,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await InvokeScriptInternalAsync(script.Hash, options.Keys, options.Args, null);
    }

    private async Task<ValkeyResult> InvokeScriptInternalAsync(
        string hash,
        string[]? keys,
        string[]? args,
        Route? route)
    {
        // Convert hash to C string
        IntPtr hashPtr = Marshal.StringToHGlobalAnsi(hash);

        try
        {
            // Prepare keys
            IntPtr keysPtr = IntPtr.Zero;
            IntPtr keysLenPtr = IntPtr.Zero;
            ulong keysCount = 0;

            if (keys != null && keys.Length > 0)
            {
                keysCount = (ulong)keys.Length;
                IntPtr[] keyPtrs = new IntPtr[keys.Length];
                ulong[] keyLens = new ulong[keys.Length];

                for (int i = 0; i < keys.Length; i++)
                {
                    byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(keys[i]);
                    keyPtrs[i] = Marshal.AllocHGlobal(keyBytes.Length);
                    Marshal.Copy(keyBytes, 0, keyPtrs[i], keyBytes.Length);
                    keyLens[i] = (ulong)keyBytes.Length;
                }

                keysPtr = Marshal.AllocHGlobal(IntPtr.Size * keys.Length);
                Marshal.Copy(keyPtrs, 0, keysPtr, keys.Length);

                keysLenPtr = Marshal.AllocHGlobal(sizeof(ulong) * keys.Length);
                Marshal.Copy(keyLens.Select(l => (long)l).ToArray(), 0, keysLenPtr, keys.Length);
            }

            // Prepare args
            IntPtr argsPtr = IntPtr.Zero;
            IntPtr argsLenPtr = IntPtr.Zero;
            ulong argsCount = 0;

            if (args != null && args.Length > 0)
            {
                argsCount = (ulong)args.Length;
                IntPtr[] argPtrs = new IntPtr[args.Length];
                ulong[] argLens = new ulong[args.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    byte[] argBytes = System.Text.Encoding.UTF8.GetBytes(args[i]);
                    argPtrs[i] = Marshal.AllocHGlobal(argBytes.Length);
                    Marshal.Copy(argBytes, 0, argPtrs[i], argBytes.Length);
                    argLens[i] = (ulong)argBytes.Length;
                }

                argsPtr = Marshal.AllocHGlobal(IntPtr.Size * args.Length);
                Marshal.Copy(argPtrs, 0, argsPtr, args.Length);

                argsLenPtr = Marshal.AllocHGlobal(sizeof(ulong) * args.Length);
                Marshal.Copy(argLens.Select(l => (long)l).ToArray(), 0, argsLenPtr, args.Length);
            }

            // Prepare route (null for now)
            IntPtr routePtr = IntPtr.Zero;
            ulong routeLen = 0;

            // Call FFI
            Message message = _messageContainer.GetMessageForCall();
            FFI.InvokeScriptFfi(
                _clientPointer,
                (ulong)message.Index,
                hashPtr,
                keysCount,
                keysPtr,
                keysLenPtr,
                argsCount,
                argsPtr,
                argsLenPtr,
                routePtr,
                routeLen);

            // Wait for response
            IntPtr response = await message;
            try
            {
                return ResponseConverters.HandleServerValue<object?, ValkeyResult>(HandleResponse(response), true, o => ValkeyResult.Create(o), true);
            }
            finally
            {
                FFI.FreeResponse(response);
            }
        }
        finally
        {
            // Free allocated memory
            if (hashPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(hashPtr);
            }
            // TODO: Free keys and args memory
        }
    }

    // ===== Script Management =====

    /// <inheritdoc/>
    public async Task<bool[]> ScriptExistsAsync(
        string[] sha1Hashes,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ScriptExistsAsync(sha1Hashes));
    }

    /// <inheritdoc/>
    public async Task<string> ScriptFlushAsync(
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ScriptFlushAsync());
    }

    /// <inheritdoc/>
    public async Task<string> ScriptFlushAsync(
        FlushMode mode,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ScriptFlushAsync(mode));
    }

    /// <inheritdoc/>
    public async Task<string?> ScriptShowAsync(
        string sha1Hash,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        try
        {
            return await Command(Request.ScriptShowAsync(sha1Hash));
        }
        catch (Errors.RequestException ex) when (ex.Message.Contains("NoScriptError"))
        {
            // Return null when script doesn't exist
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<string> ScriptKillAsync(
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ScriptKillAsync());
    }

    // ===== Function Execution =====

    /// <inheritdoc/>
    public async Task<ValkeyResult> FCallAsync(
        string function,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.FCallAsync(function, null, null));
    }

    /// <inheritdoc/>
    public async Task<ValkeyResult> FCallAsync(
        string function,
        string[] keys,
        string[] args,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.FCallAsync(function, keys, args));
    }

    /// <inheritdoc/>
    public async Task<ValkeyResult> FCallReadOnlyAsync(
        string function,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.FCallReadOnlyAsync(function, null, null));
    }

    /// <inheritdoc/>
    public async Task<ValkeyResult> FCallReadOnlyAsync(
        string function,
        string[] keys,
        string[] args,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.FCallReadOnlyAsync(function, keys, args));
    }

    // ===== Function Management =====

    /// <inheritdoc/>
    public async Task<string> FunctionLoadAsync(
        string libraryCode,
        bool replace = false,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.FunctionLoadAsync(libraryCode, replace));
    }

    /// <inheritdoc/>
    public async Task<string> FunctionFlushAsync(
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.FunctionFlushAsync());
    }

    /// <inheritdoc/>
    public async Task<string> FunctionFlushAsync(
        FlushMode mode,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.FunctionFlushAsync(mode));
    }
}
