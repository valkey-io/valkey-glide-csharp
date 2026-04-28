// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

using static Valkey.Glide.Internals.ResponseHandler;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    // ===== Script Execution =====

    /// <inheritdoc cref="IBaseClient.ScriptInvokeAsync(Script, CancellationToken)"/>
    public async Task<ValkeyResult> ScriptInvokeAsync(
        Script script,
        CancellationToken cancellationToken = default)
    {
        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

        return await ScriptInvokeInternalAsync(script.Hash, null, null);
    }

    /// <inheritdoc cref="IBaseClient.ScriptInvokeAsync(Script, ScriptOptions, CancellationToken)"/>
    public async Task<ValkeyResult> ScriptInvokeAsync(
        Script script,
        ScriptOptions options,
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

        return await ScriptInvokeInternalAsync(script.Hash, options.Keys, options.Args);
    }

    private async Task<ValkeyResult> ScriptInvokeInternalAsync(
        string hash,
        string[]? keys,
        string[]? args,
        Route? route = null)
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

            // Prepare route
            using FFI.Route? ffiRoute = route?.ToFfi();
            IntPtr routePtr = ffiRoute?.ToPtr() ?? IntPtr.Zero;

            // Call FFI
            Message message = MessageContainer.GetMessageForCall();
            FFI.ScriptInvokeFfi(
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
                routePtr != IntPtr.Zero ? 1UL : 0UL);

            // Wait for response
            IntPtr response = await message;
            try
            {
                return ResponseConverters.HandleServerValue<object?, ValkeyResult>(HandleResponse(response), true, ValkeyResult.Create, true);
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

    /// <inheritdoc cref="IBaseClient.ScriptExistsAsync(string, CancellationToken)"/>
    public async Task<bool> ScriptExistsAsync(
        string sha1Hash,
        CancellationToken cancellationToken = default)
    {
        bool[] results = await ScriptExistsAsync([sha1Hash], cancellationToken);
        return results[0];
    }

    /// <inheritdoc cref="IBaseClient.ScriptExistsAsync(IEnumerable{string}, CancellationToken)"/>
    public async Task<bool[]> ScriptExistsAsync(
        IEnumerable<string> sha1Hashes,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.ScriptExistsAsync([.. sha1Hashes]));
    }

    /// <inheritdoc cref="IBaseClient.ScriptFlushAsync(CancellationToken)"/>
    public async Task ScriptFlushAsync(
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptFlushAsync());
    }

    /// <inheritdoc cref="IBaseClient.ScriptFlushAsync(FlushMode, CancellationToken)"/>
    public async Task ScriptFlushAsync(
        FlushMode mode,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptFlushAsync(mode));
    }

    /// <inheritdoc/>
    public async Task<string?> ScriptShowAsync(
        string sha1Hash,
        CancellationToken cancellationToken = default)
    {
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
    public async Task ScriptKillAsync(
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptKillAsync());
    }

    // ===== Function Execution =====

    /// <inheritdoc cref="IBaseClient.FCallAsync(string, CancellationToken)"/>
    public async Task<ValkeyResult> FCallAsync(
        string function,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallAsync(function, null, null));
    }

    /// <inheritdoc cref="IBaseClient.FCallAsync(string, IEnumerable{string}, IEnumerable{string}, CancellationToken)"/>
    public async Task<ValkeyResult> FCallAsync(
        string function,
        IEnumerable<string> keys,
        IEnumerable<string> args,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallAsync(function, [.. keys], [.. args]));
    }

    /// <inheritdoc cref="IBaseClient.FCallReadOnlyAsync(string, CancellationToken)"/>
    public async Task<ValkeyResult> FCallReadOnlyAsync(
        string function,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallReadOnlyAsync(function, null, null));
    }

    /// <inheritdoc cref="IBaseClient.FCallReadOnlyAsync(string, IEnumerable{string}, IEnumerable{string}, CancellationToken)"/>
    public async Task<ValkeyResult> FCallReadOnlyAsync(
        string function,
        IEnumerable<string> keys,
        IEnumerable<string> args,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallReadOnlyAsync(function, [.. keys], [.. args]));
    }

    // ===== Function Management =====

    /// <inheritdoc/>
    public async Task<string> FunctionLoadAsync(
        string libraryCode,
        bool replace = false,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionLoadAsync(libraryCode, replace));
    }

    /// <inheritdoc cref="IBaseClient.FunctionFlushAsync(CancellationToken)"/>
    public async Task FunctionFlushAsync(
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionFlushAsync());
    }

    /// <inheritdoc cref="IBaseClient.FunctionFlushAsync(FlushMode, CancellationToken)"/>
    public async Task FunctionFlushAsync(
        FlushMode mode,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionFlushAsync(mode));
    }

    /// <inheritdoc/>
    public async Task FunctionDeleteAsync(
        string libraryName,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionDeleteAsync(libraryName));
    }

    /// <inheritdoc/>
    public async Task FunctionKillAsync(
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionKillAsync());
    }

    // ===== Function Persistence =====

    /// <inheritdoc/>
    public async Task<byte[]> FunctionDumpAsync(
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionDumpAsync());
    }

    /// <inheritdoc cref="IBaseClient.FunctionRestoreAsync(byte[], CancellationToken)"/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, null));
    }

    /// <inheritdoc cref="IBaseClient.FunctionRestoreAsync(byte[], FunctionRestorePolicy, CancellationToken)"/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, policy));
    }

    // ===== StackExchange.Redis Compatibility Methods =====

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}, IEnumerable{ValkeyValue})"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(
        string script, IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null)
    {
        // Use the optimized ScriptInvoke path via Script object
        // Script constructor will validate the script parameter
        using Script scriptObj = new(script);

        // Convert keys and values to string arrays
        string[]? keyStrings = keys?.Select(k => k.ToString()).ToArray();
        string[]? valueStrings = values?.Select(v => v.ToString()).ToArray();

        // Use ScriptInvokeInternalAsync for automatic EVALSHA→EVAL optimization
        return await ScriptInvokeInternalAsync(scriptObj.Hash, keyStrings, valueStrings);
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}, IEnumerable{ValkeyValue})"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null)
    {
        // Convert hash to hex string (lowercase)
        string hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        // Convert keys and values to string arrays
        string[]? keyStrings = keys?.Select(k => k.ToString()).ToArray();
        string[]? valueStrings = values?.Select(v => v.ToString()).ToArray();

        // Use ScriptInvokeInternalAsync (will use EVALSHA directly, no fallback since we don't have source)
        return await ScriptInvokeInternalAsync(hashString, keyStrings, valueStrings);
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LuaScript, object)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters = null)
    {
        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

        // Replace placeholders in the executable script with KEYS/ARGV references
        string executableScript = parameters != null
            ? ScriptParameterMapper.ReplacePlaceholders(script.ExecutableScript, script.Arguments, parameters)
            : script.ExecutableScript;

        // Extract parameters from the object
        (ValkeyKey[] keys, ValkeyValue[] args) = script.ExtractParametersInternal(parameters, null);

        // Convert to string arrays
        string[]? keyStrings = keys.Length > 0 ? [.. keys.Select(k => k.ToString())] : null;
        string[]? valueStrings = args.Length > 0 ? [.. args.Select(v => v.ToString())] : null;

        // Create a Script object from the executable script and use ScriptInvoke
        // This will automatically load the script if needed (EVALSHA with fallback to EVAL)
        using Script scriptObj = new(executableScript);
        return await ScriptInvokeInternalAsync(scriptObj.Hash, keyStrings, valueStrings);
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LoadedLuaScript, object)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters = null)
    {
        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

        // Extract parameters from the object using the internal LuaScript
        (ValkeyKey[] keys, ValkeyValue[] args) = script.Script.ExtractParametersInternal(parameters, null);

        // Convert to string arrays
        string[]? keyStrings = keys.Length > 0 ? [.. keys.Select(k => k.ToString())] : null;
        string[]? valueStrings = args.Length > 0 ? [.. args.Select(v => v.ToString())] : null;

        // Convert the hash from byte[] to hex string
        // The hash in LoadedLuaScript is the hash of the script that was actually loaded on the server
        string hashString = BitConverter.ToString(script.Hash).Replace("-", "").ToLowerInvariant();

        // Use ScriptInvokeInternalAsync with the hash from LoadedLuaScript
        // The script was already loaded on the server, so EVALSHA will work
        return await ScriptInvokeInternalAsync(hashString, keyStrings, valueStrings);
    }

    // ===== Synchronous Wrappers =====

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}, IEnumerable{ValkeyValue})"/>
    public ValkeyResult ScriptEvaluate(
        string script,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null)
    => ScriptEvaluateAsync(script, keys, values).GetAwaiter().GetResult();

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}, IEnumerable{ValkeyValue})"/>
    public ValkeyResult ScriptEvaluate(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null)
    => ScriptEvaluateAsync(hash, keys, values).GetAwaiter().GetResult();

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LuaScript, object)"/>
    public ValkeyResult ScriptEvaluate(LuaScript script, object? parameters = null)
        => ScriptEvaluateAsync(script, parameters).GetAwaiter().GetResult();

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LoadedLuaScript, object)"/>
    public ValkeyResult ScriptEvaluate(LoadedLuaScript script, object? parameters = null)
        => ScriptEvaluateAsync(script, parameters).GetAwaiter().GetResult();
}
