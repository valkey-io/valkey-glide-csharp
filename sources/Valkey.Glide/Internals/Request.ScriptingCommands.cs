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
        var cmdArgs = new List<GlideString> { script };

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
    public static Cmd<object[], FunctionStatsResult> FunctionStatsAsync()
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
            var libArray = (object[])libObj;
            string? name = null;
            string? engine = null;
            string? code = null;
            var functions = new List<FunctionInfo>();

            for (int i = 0; i < libArray.Length; i += 2)
            {
                string key = ((GlideString)libArray[i]).ToString();
                object value = libArray[i + 1];

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
                        var funcArray = (object[])value;
                        foreach (object funcObj in funcArray)
                        {
                            var funcData = (object[])funcObj;
                            string? funcName = null;
                            string? funcDesc = null;
                            var funcFlags = new List<string>();

                            for (int j = 0; j < funcData.Length; j += 2)
                            {
                                string funcKey = ((GlideString)funcData[j]).ToString();
                                object funcValue = funcData[j + 1];

                                switch (funcKey)
                                {
                                    case "name":
                                        funcName = ((GlideString)funcValue).ToString();
                                        break;
                                    case "description":
                                        funcDesc = funcValue != null ? ((GlideString)funcValue).ToString() : null;
                                        break;
                                    case "flags":
                                        var flagsArray = (object[])funcValue;
                                        funcFlags.AddRange(flagsArray.Select(f => ((GlideString)f).ToString()));
                                        break;
                                    default:
                                        // Ignore unknown function properties
                                        break;
                                }
                            }

                            if (funcName != null)
                            {
                                functions.Add(new FunctionInfo(funcName, funcDesc, [.. funcFlags]));
                            }
                        }
                        break;
                    default:
                        // Ignore unknown library properties
                        break;
                }
            }

            if (name != null && engine != null)
            {
                libraries.Add(new LibraryInfo(name, engine, [.. functions], code));
            }
        }

        return [.. libraries];
    }

    private static FunctionStatsResult ParseFunctionStatsResponse(object[] response)
    {
        var engines = new Dictionary<string, EngineStats>();
        RunningScriptInfo? runningScript = null;

        for (int i = 0; i < response.Length; i += 2)
        {
            string key = ((GlideString)response[i]).ToString();
            object value = response[i + 1];

            switch (key)
            {
                case "running_script":
                    if (value != null)
                    {
                        var scriptData = (object[])value;
                        string? name = null;
                        string? command = null;
                        var args = new List<string>();
                        long durationMs = 0;

                        for (int j = 0; j < scriptData.Length; j += 2)
                        {
                            string scriptKey = ((GlideString)scriptData[j]).ToString();
                            object scriptValue = scriptData[j + 1];

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

                        if (name != null && command != null)
                        {
                            runningScript = new RunningScriptInfo(
                                name,
                                command,
                                [.. args],
                                TimeSpan.FromMilliseconds(durationMs));
                        }
                    }
                    break;
                case "engines":
                    var enginesData = (object[])value;
                    for (int j = 0; j < enginesData.Length; j += 2)
                    {
                        string engineName = ((GlideString)enginesData[j]).ToString();
                        var engineData = (object[])enginesData[j + 1];

                        string? language = null;
                        long functionCount = 0;
                        long libraryCount = 0;

                        for (int k = 0; k < engineData.Length; k += 2)
                        {
                            string engineKey = ((GlideString)engineData[k]).ToString();
                            object engineValue = engineData[k + 1];

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

                        language = engineName; // Engine name is the language
                        engines[engineName] = new EngineStats(language, functionCount, libraryCount);
                    }
                    break;
                default:
                    // Ignore unknown top-level properties
                    break;
            }
        }

        return new FunctionStatsResult(engines, runningScript);
    }
}
