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
        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

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
        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

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

        // Track allocated memory for cleanup
        IntPtr[]? keyPtrs = null;
        IntPtr keysPtr = IntPtr.Zero;
        IntPtr keysLenPtr = IntPtr.Zero;
        IntPtr[]? argPtrs = null;
        IntPtr argsPtr = IntPtr.Zero;
        IntPtr argsLenPtr = IntPtr.Zero;

        try
        {
            // Prepare keys
            ulong keysCount = PrepareStringArrayForFFI(keys, out keyPtrs, out keysPtr, out keysLenPtr);

            // Prepare args
            ulong argsCount = PrepareStringArrayForFFI(args, out argPtrs, out argsPtr, out argsLenPtr);

            // Prepare route (null for now)
            IntPtr routePtr = IntPtr.Zero;
            ulong routeLen = 0;

            // Call FFI
            Message message = MessageContainer.GetMessageForCall();
            FFI.InvokeScriptFfi(
                ClientPointer,
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
            FreeScriptMemory(hashPtr, keyPtrs, keysPtr, keysLenPtr, argPtrs, argsPtr, argsLenPtr);
        }
    }

    /// <summary>
    /// Prepares string array for FFI by allocating unmanaged memory and marshalling data.
    /// </summary>
    /// <param name="items">Array of strings to prepare.</param>
    /// <param name="itemPtrs">Output array of pointers to individual string data.</param>
    /// <param name="itemsPtr">Output pointer to array of string pointers.</param>
    /// <param name="itemsLenPtr">Output pointer to array of string lengths.</param>
    /// <returns>Count of items prepared.</returns>
    private static ulong PrepareStringArrayForFFI(
        string[]? items,
        out IntPtr[]? itemPtrs,
        out IntPtr itemsPtr,
        out IntPtr itemsLenPtr)
    {
        itemPtrs = null;
        itemsPtr = IntPtr.Zero;
        itemsLenPtr = IntPtr.Zero;

        if (items == null || items.Length == 0)
        {
            return 0;
        }

        ulong count = (ulong)items.Length;
        itemPtrs = new IntPtr[items.Length];
        ulong[] itemLens = new ulong[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            byte[] itemBytes = System.Text.Encoding.UTF8.GetBytes(items[i]);
            itemPtrs[i] = Marshal.AllocHGlobal(itemBytes.Length);
            Marshal.Copy(itemBytes, 0, itemPtrs[i], itemBytes.Length);
            itemLens[i] = (ulong)itemBytes.Length;
        }

        itemsPtr = Marshal.AllocHGlobal(IntPtr.Size * items.Length);
        Marshal.Copy(itemPtrs, 0, itemsPtr, items.Length);

        itemsLenPtr = Marshal.AllocHGlobal(sizeof(ulong) * items.Length);
        Marshal.Copy(itemLens.Select(l => (long)l).ToArray(), 0, itemsLenPtr, items.Length);

        return count;
    }

    /// <summary>
    /// Frees all allocated memory for script invocation.
    /// </summary>
    private static void FreeScriptMemory(
        IntPtr hashPtr,
        IntPtr[]? keyPtrs,
        IntPtr keysPtr,
        IntPtr keysLenPtr,
        IntPtr[]? argPtrs,
        IntPtr argsPtr,
        IntPtr argsLenPtr)
    {
        // Free hash
        if (hashPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(hashPtr);
        }

        // Free individual key strings
        if (keyPtrs != null)
        {
            foreach (IntPtr ptr in keyPtrs)
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        // Free keys array and lengths
        if (keysPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(keysPtr);
        }
        if (keysLenPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(keysLenPtr);
        }

        // Free individual arg strings
        if (argPtrs != null)
        {
            foreach (IntPtr ptr in argPtrs)
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        // Free args array and lengths
        if (argsPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(argsPtr);
        }
        if (argsLenPtr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(argsLenPtr);
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

    // ===== StackExchange.Redis Compatibility Methods =====

    /// <inheritdoc/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(string script, ValkeyKey[]? keys = null, ValkeyValue[]? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");

        // Use the optimized InvokeScript path via Script object
        // Script constructor will validate the script parameter
        using Script scriptObj = new(script);

        // Convert keys and values to string arrays
        string[]? keyStrings = keys?.Select(k => k.ToString()).ToArray();
        string[]? valueStrings = values?.Select(v => v.ToString()).ToArray();

        // Use InvokeScriptInternalAsync for automatic EVALSHA→EVAL optimization
        return await InvokeScriptInternalAsync(scriptObj.Hash, keyStrings, valueStrings, null);
    }

    /// <inheritdoc/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(byte[] hash, ValkeyKey[]? keys = null, ValkeyValue[]? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");

        // Convert hash to hex string (lowercase)
        string hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        // Convert keys and values to string arrays
        string[]? keyStrings = keys?.Select(k => k.ToString()).ToArray();
        string[]? valueStrings = values?.Select(v => v.ToString()).ToArray();

        // Use InvokeScriptInternalAsync (will use EVALSHA directly, no fallback since we don't have source)
        return await InvokeScriptInternalAsync(hashString, keyStrings, valueStrings, null);
    }

    /// <inheritdoc/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");

        // Replace placeholders in the executable script with KEYS/ARGV references
        string executableScript = parameters != null
            ? ScriptParameterMapper.ReplacePlaceholders(script.ExecutableScript, script.Arguments, parameters)
            : script.ExecutableScript;

        // Extract parameters from the object
        (ValkeyKey[] keys, ValkeyValue[] args) = script.ExtractParametersInternal(parameters, null);

        // Convert to string arrays
        string[]? keyStrings = keys.Length > 0 ? [.. keys.Select(k => k.ToString())] : null;
        string[]? valueStrings = args.Length > 0 ? [.. args.Select(v => v.ToString())] : null;

        // Create a Script object from the executable script and use InvokeScript
        // This will automatically load the script if needed (EVALSHA with fallback to EVAL)
        using Script scriptObj = new(executableScript);
        return await InvokeScriptInternalAsync(scriptObj.Hash, keyStrings, valueStrings, null);
    }

    /// <inheritdoc/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");

        // Extract parameters from the object using the internal LuaScript
        (ValkeyKey[] keys, ValkeyValue[] args) = script.Script.ExtractParametersInternal(parameters, null);

        // Convert to string arrays
        string[]? keyStrings = keys.Length > 0 ? [.. keys.Select(k => k.ToString())] : null;
        string[]? valueStrings = args.Length > 0 ? [.. args.Select(v => v.ToString())] : null;

        // Convert the hash from byte[] to hex string
        // The hash in LoadedLuaScript is the hash of the script that was actually loaded on the server
        string hashString = BitConverter.ToString(script.Hash).Replace("-", "").ToLowerInvariant();

        // Use InvokeScriptInternalAsync with the hash from LoadedLuaScript
        // The script was already loaded on the server, so EVALSHA will work
        return await InvokeScriptInternalAsync(hashString, keyStrings, valueStrings, null);
    }

    // ===== Synchronous Wrappers =====

    /// <inheritdoc/>
    public ValkeyResult ScriptEvaluate(string script, ValkeyKey[]? keys = null, ValkeyValue[]? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        return ScriptEvaluateAsync(script, keys, values, flags).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public ValkeyResult ScriptEvaluate(byte[] hash, ValkeyKey[]? keys = null, ValkeyValue[]? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        return ScriptEvaluateAsync(hash, keys, values, flags).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public ValkeyResult ScriptEvaluate(LuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        return ScriptEvaluateAsync(script, parameters, flags).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public ValkeyResult ScriptEvaluate(LoadedLuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        return ScriptEvaluateAsync(script, parameters, flags).GetAwaiter().GetResult();
    }
}
