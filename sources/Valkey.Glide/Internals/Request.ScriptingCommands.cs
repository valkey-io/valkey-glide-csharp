// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    // ===== Script Execution =====

    /// <summary>
    /// Creates a command to execute a script using EVALSHA.
    /// </summary>
    public static Cmd<object?, ValkeyResult> EvalShaAsync(string hash, string[]? keys = null, string[]? args = null)
    {
        var cmdArgs = new List<GlideString> { hash };

        int numKeys = keys?.Length ?? 0;
        cmdArgs.Add(numKeys.ToString());

        if (keys != null)
        {
            cmdArgs.AddRange(keys.Select(k => (GlideString)k));
        }

        if (args != null)
        {
            cmdArgs.AddRange(args.Select(a => (GlideString)a));
        }

        return new(RequestType.EvalSha, [.. cmdArgs], true, o => ValkeyResult.Create(o), allowConverterToHandleNull: true);
    }

    /// <summary>
    /// Creates a command to execute a script using EVAL.
    /// </summary>
    public static Cmd<object?, ValkeyResult> EvalAsync(string script, string[]? keys = null, string[]? args = null)
    {
        List<GlideString> cmdArgs = [script];

        int numKeys = keys?.Length ?? 0;
        cmdArgs.Add(numKeys.ToString());

        if (keys != null)
        {
            cmdArgs.AddRange(keys.Select(k => (GlideString)k));
        }

        if (args != null)
        {
            cmdArgs.AddRange(args.Select(a => (GlideString)a));
        }

        return new(RequestType.Eval, [.. cmdArgs], true, o => ValkeyResult.Create(o), allowConverterToHandleNull: true);
    }

    // ===== Script Management =====

    /// <summary>
    /// Creates a command to check if scripts exist in the cache.
    /// </summary>
    public static Cmd<object[], bool[]> ScriptExistsAsync(string[] sha1Hashes)
    {
        var cmdArgs = sha1Hashes.Select(h => (GlideString)h).ToArray();
        return new(RequestType.ScriptExists, cmdArgs, false, arr => [.. arr.Select(o => Convert.ToInt64(o) == 1)]);
    }

    /// <summary>
    /// Creates a command to flush all scripts from the cache.
    /// </summary>
    public static Cmd<string, string> ScriptFlushAsync()
        => OK(RequestType.ScriptFlush, []);

    /// <summary>
    /// Creates a command to flush all scripts from the cache with specified mode.
    /// </summary>
    public static Cmd<string, string> ScriptFlushAsync(FlushMode mode)
        => OK(RequestType.ScriptFlush, [mode == FlushMode.Sync ? "SYNC" : "ASYNC"]);

    /// <summary>
    /// Creates a command to get the source code of a cached script.
    /// </summary>
    public static Cmd<GlideString?, string?> ScriptShowAsync(string sha1Hash)
        => new(RequestType.ScriptShow, [sha1Hash], true, gs => gs?.ToString());

    /// <summary>
    /// Creates a command to kill a currently executing script.
    /// </summary>
    public static Cmd<string, string> ScriptKillAsync()
        => OK(RequestType.ScriptKill, []);

    // ===== Function Execution =====

    /// <summary>
    /// Creates a command to execute a function.
    /// </summary>
    public static Cmd<object?, ValkeyResult> FCallAsync(string function, string[]? keys = null, string[]? args = null)
    {
        var cmdArgs = new List<GlideString> { function };

        int numKeys = keys?.Length ?? 0;
        cmdArgs.Add(numKeys.ToString());

        if (keys != null)
        {
            cmdArgs.AddRange(keys.Select(k => (GlideString)k));
        }

        if (args != null)
        {
            cmdArgs.AddRange(args.Select(a => (GlideString)a));
        }

        return new(RequestType.FCall, [.. cmdArgs], true, o => ValkeyResult.Create(o), allowConverterToHandleNull: true);
    }

    /// <summary>
    /// Creates a command to execute a function in read-only mode.
    /// </summary>
    public static Cmd<object?, ValkeyResult> FCallReadOnlyAsync(string function, string[]? keys = null, string[]? args = null)
    {
        var cmdArgs = new List<GlideString> { function };

        int numKeys = keys?.Length ?? 0;
        cmdArgs.Add(numKeys.ToString());

        if (keys != null)
        {
            cmdArgs.AddRange(keys.Select(k => (GlideString)k));
        }

        if (args != null)
        {
            cmdArgs.AddRange(args.Select(a => (GlideString)a));
        }

        return new(RequestType.FCallReadOnly, [.. cmdArgs], true, o => ValkeyResult.Create(o), allowConverterToHandleNull: true);
    }

    // ===== Function Management =====

    /// <summary>
    /// Creates a command to load a function library.
    /// </summary>
    public static Cmd<GlideString, string> FunctionLoadAsync(string libraryCode, bool replace)
    {
        var cmdArgs = new List<GlideString>();
        if (replace)
        {
            cmdArgs.Add("REPLACE");
        }
        cmdArgs.Add(libraryCode);

        return new(RequestType.FunctionLoad, [.. cmdArgs], false, gs => gs.ToString());
    }

    /// <summary>
    /// Creates a command to flush all functions.
    /// </summary>
    public static Cmd<string, string> FunctionFlushAsync()
        => OK(RequestType.FunctionFlush, []);

    /// <summary>
    /// Creates a command to flush all functions with specified mode.
    /// </summary>
    public static Cmd<string, string> FunctionFlushAsync(FlushMode mode)
        => OK(RequestType.FunctionFlush, [mode == FlushMode.Sync ? "SYNC" : "ASYNC"]);

    // ===== Function Inspection =====

    /// <summary>
    /// Creates a command to list all loaded function libraries.
    /// </summary>
    public static Cmd<object[], LibraryInfo[]> FunctionListAsync(FunctionListQuery? query = null)
    {
        var cmdArgs = new List<GlideString>();

        if (query?.LibraryName != null)
        {
            cmdArgs.Add("LIBRARYNAME");
            cmdArgs.Add(query.LibraryName);
        }

        if (query?.WithCode == true)
        {
            cmdArgs.Add("WITHCODE");
        }

        return new(RequestType.FunctionList, [.. cmdArgs], false, ParseFunctionListResponse);
    }

    /// <summary>
    /// Creates a command to get function statistics.
    /// </summary>
    public static Cmd<object, FunctionStatsResult> FunctionStatsAsync()
        => new(RequestType.FunctionStats, [], false, ParseFunctionStatsResponse);

    /// <summary>
    /// Creates a command to delete a function library.
    /// </summary>
    public static Cmd<string, string> FunctionDeleteAsync(string libraryName)
        => OK(RequestType.FunctionDelete, [libraryName]);

    /// <summary>
    /// Creates a command to kill a currently executing function.
    /// </summary>
    public static Cmd<string, string> FunctionKillAsync()
        => OK(RequestType.FunctionKill, []);

    /// <summary>
    /// Creates a command to dump all functions to a binary payload.
    /// </summary>
    public static Cmd<GlideString, byte[]> FunctionDumpAsync()
        => new(RequestType.FunctionDump, [], false, gs => gs.Bytes);

    /// <summary>
    /// Creates a command to restore functions from a binary payload.
    /// </summary>
    public static Cmd<string, string> FunctionRestoreAsync(byte[] payload, FunctionRestorePolicy? policy = null)
    {
        var cmdArgs = new List<GlideString> { payload };

        if (policy.HasValue)
        {
            cmdArgs.Add(policy.Value switch
            {
                FunctionRestorePolicy.Append => "APPEND",
                FunctionRestorePolicy.Flush => "FLUSH",
                FunctionRestorePolicy.Replace => "REPLACE",
                _ => throw new ArgumentException($"Unknown policy: {policy.Value}", nameof(policy))
            });
        }

        return OK(RequestType.FunctionRestore, [.. cmdArgs]);
    }

    // ===== Response Parsers =====

    private static LibraryInfo[] ParseFunctionListResponse(object[] response)
    {
        var libraries = new List<LibraryInfo>();

        foreach (object libObj in response)
        {
            string? name = null;
            string? engine = null;
            string? code = null;
            var functions = new List<FunctionInfo>();

            // Handle both RESP2 (array) and RESP3 (dictionary) formats
            if (libObj is Dictionary<GlideString, object> libDict)
            {
                // RESP3 format - dictionary
                foreach (var kvp in libDict)
                {
                    string key = kvp.Key.ToString();
                    object value = kvp.Value;
                    ProcessLibraryField(key, value, ref name, ref engine, ref code, functions);
                }
            }
            else
            {
                // RESP2 format - array
                var libArray = (object[])libObj;
                for (int i = 0; i < libArray.Length; i += 2)
                {
                    string key = ((GlideString)libArray[i]).ToString();
                    object value = libArray[i + 1];
                    ProcessLibraryField(key, value, ref name, ref engine, ref code, functions);
                }
            }

            if (name != null && engine != null)
            {
                libraries.Add(new LibraryInfo(name, engine, [.. functions], code));
            }
        }

        return [.. libraries];
    }

    private static void ProcessLibraryField(string key, object value, ref string? name, ref string? engine, ref string? code, List<FunctionInfo> functions)
    {
        switch (key)
        {
            case "library_name":
                name = ((GlideString)value).ToString();
                break;
            case "engine":
                engine = ((GlideString)value).ToString();
                break;
            case "library_code":
                code = ((GlideString)value).ToString();
                break;
            case "functions":
                ParseFunctions(value, functions);
                break;
            default:
                // Ignore unknown library properties
                break;
        }
    }

    private static void ParseFunctions(object value, List<FunctionInfo> functions)
    {
        // Handle both array and potential dictionary formats for functions
        if (value is object[] funcArray)
        {
            foreach (object funcObj in funcArray)
            {
                string? funcName = null;
                string? funcDesc = null;
                var funcFlags = new List<string>();

                if (funcObj is Dictionary<GlideString, object> funcDict)
                {
                    // RESP3 format
                    foreach (var kvp in funcDict)
                    {
                        ProcessFunctionField(kvp.Key.ToString(), kvp.Value, ref funcName, ref funcDesc, funcFlags);
                    }
                }
                else
                {
                    // RESP2 format
                    var funcData = (object[])funcObj;
                    for (int j = 0; j < funcData.Length; j += 2)
                    {
                        string funcKey = ((GlideString)funcData[j]).ToString();
                        object funcValue = funcData[j + 1];
                        ProcessFunctionField(funcKey, funcValue, ref funcName, ref funcDesc, funcFlags);
                    }
                }

                if (funcName != null)
                {
                    functions.Add(new FunctionInfo(funcName, funcDesc, [.. funcFlags]));
                }
            }
        }
    }

    private static void ProcessFunctionField(string funcKey, object funcValue, ref string? funcName, ref string? funcDesc, List<string> funcFlags)
    {
        switch (funcKey)
        {
            case "name":
                funcName = ((GlideString)funcValue).ToString();
                break;
            case "description":
                funcDesc = funcValue != null ? ((GlideString)funcValue).ToString() : null;
                break;
            case "flags":
                if (funcValue is object[] flagsArray)
                {
                    funcFlags.AddRange(flagsArray.Select(f => ((GlideString)f).ToString()));
                }
                break;
            default:
                // Ignore unknown function properties
                break;
        }
    }

    private static FunctionStatsResult ParseFunctionStatsResponse(object response)
    {
        // The response is a map of node addresses to their stats
        // For standalone mode, there's only one node
        // We extract the first node's stats

        object? nodeData = null;

        // Handle both RESP2 (array) and RESP3 (dictionary) at top level
        if (response is Dictionary<GlideString, object> responseDict)
        {
            // RESP3 format - dictionary of node addresses
            // Get the first (and typically only) node's data
            nodeData = responseDict.Values.FirstOrDefault();
        }
        else if (response is object[] responseArray && responseArray.Length >= 2)
        {
            // RESP2 format - array of [nodeAddr, nodeData, ...]
            // Get the first node's data (at index 1)
            nodeData = responseArray[1];
        }

        if (nodeData == null)
        {
            return new FunctionStatsResult([], null);
        }

        // Now parse the node's stats
        var engines = new Dictionary<string, EngineStats>();
        RunningScriptInfo? runningScript = null;

        if (nodeData is Dictionary<GlideString, object> nodeDict)
        {
            // RESP3 format
            foreach (var kvp in nodeDict)
            {
                string key = kvp.Key.ToString();
                object value = kvp.Value;
                ProcessStatsField(key, value, ref runningScript, engines);
            }
        }
        else if (nodeData is object[] nodeArray)
        {
            // RESP2 format
            for (int i = 0; i < nodeArray.Length; i += 2)
            {
                string key = ((GlideString)nodeArray[i]).ToString();
                object value = nodeArray[i + 1];
                ProcessStatsField(key, value, ref runningScript, engines);
            }
        }

        return new FunctionStatsResult(engines, runningScript);
    }

    private static void ProcessStatsField(string key, object value, ref RunningScriptInfo? runningScript, Dictionary<string, EngineStats> engines)
    {
        switch (key)
        {
            case "running_script":
                if (value != null)
                {
                    runningScript = ParseRunningScript(value);
                }
                break;
            case "engines":
                ParseEngines(value, engines);
                break;
            default:
                // Ignore unknown top-level properties
                break;
        }
    }

    private static RunningScriptInfo? ParseRunningScript(object value)
    {
        string? name = null;
        string? command = null;
        var args = new List<string>();
        long durationMs = 0;

        if (value is Dictionary<GlideString, object> scriptDict)
        {
            // RESP3 format
            foreach (var kvp in scriptDict)
            {
                ProcessRunningScriptField(kvp.Key.ToString(), kvp.Value, ref name, ref command, args, ref durationMs);
            }
        }
        else
        {
            // RESP2 format
            var scriptData = (object[])value;
            for (int j = 0; j < scriptData.Length; j += 2)
            {
                string scriptKey = ((GlideString)scriptData[j]).ToString();
                object scriptValue = scriptData[j + 1];
                ProcessRunningScriptField(scriptKey, scriptValue, ref name, ref command, args, ref durationMs);
            }
        }

        if (name != null && command != null)
        {
            return new RunningScriptInfo(name, command, [.. args], TimeSpan.FromMilliseconds(durationMs));
        }

        return null;
    }

    private static void ProcessRunningScriptField(string scriptKey, object scriptValue, ref string? name, ref string? command, List<string> args, ref long durationMs)
    {
        switch (scriptKey)
        {
            case "name":
                name = ((GlideString)scriptValue).ToString();
                break;
            case "command":
                var cmdArray = (object[])scriptValue;
                command = ((GlideString)cmdArray[0]).ToString();
                args.AddRange(cmdArray.Skip(1).Select(a => ((GlideString)a).ToString()));
                break;
            case "duration_ms":
                durationMs = Convert.ToInt64(scriptValue);
                break;
            default:
                // Ignore unknown script properties
                break;
        }
    }

    private static void ParseEngines(object value, Dictionary<string, EngineStats> engines)
    {
        if (value is Dictionary<GlideString, object> enginesDict)
        {
            // RESP3 format
            foreach (var kvp in enginesDict)
            {
                string engineName = kvp.Key.ToString();
                ParseEngineData(engineName, kvp.Value, engines);
            }
        }
        else
        {
            // RESP2 format
            var enginesData = (object[])value;
            for (int j = 0; j < enginesData.Length; j += 2)
            {
                string engineName = ((GlideString)enginesData[j]).ToString();
                ParseEngineData(engineName, enginesData[j + 1], engines);
            }
        }
    }

    private static void ParseEngineData(string engineName, object value, Dictionary<string, EngineStats> engines)
    {
        string? language = null;
        long functionCount = 0;
        long libraryCount = 0;

        if (value is Dictionary<GlideString, object> engineDict)
        {
            // RESP3 format
            foreach (var kvp in engineDict)
            {
                ProcessEngineField(kvp.Key.ToString(), kvp.Value, ref functionCount, ref libraryCount);
            }
        }
        else
        {
            // RESP2 format
            var engineData = (object[])value;
            for (int k = 0; k < engineData.Length; k += 2)
            {
                string engineKey = ((GlideString)engineData[k]).ToString();
                object engineValue = engineData[k + 1];
                ProcessEngineField(engineKey, engineValue, ref functionCount, ref libraryCount);
            }
        }

        language = engineName; // Engine name is the language
        engines[engineName] = new EngineStats(language, functionCount, libraryCount);
    }

    private static void ProcessEngineField(string engineKey, object engineValue, ref long functionCount, ref long libraryCount)
    {
        switch (engineKey)
        {
            case "libraries_count":
                libraryCount = Convert.ToInt64(engineValue);
                break;
            case "functions_count":
                functionCount = Convert.ToInt64(engineValue);
                break;
            default:
                // Ignore unknown engine properties
                break;
        }
    }

}
