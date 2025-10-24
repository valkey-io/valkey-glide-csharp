// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

[Collection("GlideTests")]
public class ScriptingCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_SimpleScript_ReturnsExpectedResult(BaseClient client)
    {
        // Test simple script execution
        using var script = new Script("return 'Hello, World!'");
        ValkeyResult result = await client.InvokeScriptAsync(script);

        Assert.NotNull(result);
        Assert.Equal("Hello, World!", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_WithKeysAndArgs_ReturnsExpectedResult(BaseClient client)
    {
        // Test script with keys and arguments
        using var script = new Script("return KEYS[1] .. ':' .. ARGV[1]");
        var options = new ScriptOptions()
            .WithKeys("mykey")
            .WithArgs("myvalue");

        ValkeyResult result = await client.InvokeScriptAsync(script, options);

        Assert.NotNull(result);
        Assert.Equal("mykey:myvalue", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_EVALSHAOptimization_UsesEVALSHAFirst(BaseClient client)
    {
        // Test that EVALSHA is used first (optimization)
        // First execution should use EVALSHA and fallback to EVAL
        using var script = new Script("return 'test'");
        ValkeyResult result1 = await client.InvokeScriptAsync(script);
        Assert.Equal("test", result1.ToString());

        // Second execution should use EVALSHA successfully (script is now cached)
        ValkeyResult result2 = await client.InvokeScriptAsync(script);
        Assert.Equal("test", result2.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_NOSCRIPTFallback_AutomaticallyUsesEVAL(BaseClient client)
    {
        // Flush scripts to ensure NOSCRIPT error
        await client.ScriptFlushAsync();

        // This should trigger NOSCRIPT and automatically fallback to EVAL
        using var script = new Script("return 'fallback test'");
        ValkeyResult result = await client.InvokeScriptAsync(script);

        Assert.NotNull(result);
        Assert.Equal("fallback test", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_ScriptError_ThrowsException(BaseClient client)
    {
        // Test script execution error
        using var script = new Script("return redis.call('INVALID_COMMAND')");

        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.InvokeScriptAsync(script));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptExistsAsync_CachedScript_ReturnsTrue(BaseClient client)
    {
        // Load a script and verify it exists
        using var script = new Script("return 'exists test'");
        await client.InvokeScriptAsync(script);

        bool[] exists = await client.ScriptExistsAsync([script.Hash]);

        Assert.Single(exists);
        Assert.True(exists[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptExistsAsync_NonCachedScript_ReturnsFalse(BaseClient client)
    {
        // Flush scripts first
        await client.ScriptFlushAsync();

        // Create a script but don't execute it
        using var script = new Script("return 'not cached'");

        bool[] exists = await client.ScriptExistsAsync([script.Hash]);

        Assert.Single(exists);
        Assert.False(exists[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptExistsAsync_MultipleScripts_ReturnsCorrectStatus(BaseClient client)
    {
        // Flush scripts first
        await client.ScriptFlushAsync();

        using var script1 = new Script("return 'script1'");
        using var script2 = new Script("return 'script2'");

        // Execute only script1
        await client.InvokeScriptAsync(script1);

        bool[] exists = await client.ScriptExistsAsync([script1.Hash, script2.Hash]);

        Assert.Equal(2, exists.Length);
        Assert.True(exists[0]);  // script1 is cached
        Assert.False(exists[1]); // script2 is not cached
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptFlushAsync_SyncMode_RemovesAllScripts(BaseClient client)
    {
        // Load a script
        using var script = new Script("return 'flush test'");
        await client.InvokeScriptAsync(script);

        // Verify it exists
        bool[] existsBefore = await client.ScriptExistsAsync([script.Hash]);
        Assert.True(existsBefore[0]);

        // Flush with SYNC mode
        string result = await client.ScriptFlushAsync(FlushMode.Sync);
        Assert.Equal("OK", result);

        // Verify it no longer exists
        bool[] existsAfter = await client.ScriptExistsAsync([script.Hash]);
        Assert.False(existsAfter[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptFlushAsync_AsyncMode_RemovesAllScripts(BaseClient client)
    {
        // Load a script
        using var script = new Script("return 'async flush test'");
        await client.InvokeScriptAsync(script);

        // Flush with ASYNC mode
        string result = await client.ScriptFlushAsync(FlushMode.Async);
        Assert.Equal("OK", result);

        // Wait a bit for async flush to complete
        await Task.Delay(100);

        // Verify it no longer exists
        bool[] existsAfter = await client.ScriptExistsAsync([script.Hash]);
        Assert.False(existsAfter[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptFlushAsync_DefaultMode_RemovesAllScripts(BaseClient client)
    {
        // Load a script
        using var script = new Script("return 'default flush test'");
        await client.InvokeScriptAsync(script);

        // Flush with default mode (SYNC)
        string result = await client.ScriptFlushAsync();
        Assert.Equal("OK", result);

        // Verify it no longer exists
        bool[] existsAfter = await client.ScriptExistsAsync([script.Hash]);
        Assert.False(existsAfter[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptShowAsync_CachedScript_ReturnsSourceCode(BaseClient client)
    {
        // Load a script
        string scriptCode = "return 'show test'";
        using var script = new Script(scriptCode);
        await client.InvokeScriptAsync(script);

        // Get the source code
        string? source = await client.ScriptShowAsync(script.Hash);

        Assert.NotNull(source);
        Assert.Equal(scriptCode, source);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptShowAsync_NonCachedScript_ReturnsNull(BaseClient client)
    {
        // Flush scripts first
        await client.ScriptFlushAsync();

        // Create a script but don't execute it
        using var script = new Script("return 'not cached'");

        // Try to get source code
        string? source = await client.ScriptShowAsync(script.Hash);

        Assert.Null(source);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptKillAsync_NoScriptRunning_ThrowsException(BaseClient client)
    {
        // Try to kill when no script is running
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.ScriptKillAsync());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_MultipleKeys_WorksCorrectly(BaseClient client)
    {
        // Test script with multiple keys
        // Use hash tags to ensure keys hash to same slot in cluster mode
        using var script = new Script("return #KEYS");
        var options = new ScriptOptions()
            .WithKeys("{key}1", "{key}2", "{key}3");

        ValkeyResult result = await client.InvokeScriptAsync(script, options);

        Assert.NotNull(result);
        Assert.Equal(3, (long)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_MultipleArgs_WorksCorrectly(BaseClient client)
    {
        // Test script with multiple arguments
        using var script = new Script("return #ARGV");
        var options = new ScriptOptions()
            .WithArgs("arg1", "arg2", "arg3", "arg4");

        ValkeyResult result = await client.InvokeScriptAsync(script, options);

        Assert.NotNull(result);
        Assert.Equal(4, (long)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_ReturnsInteger_ConvertsCorrectly(BaseClient client)
    {
        // Test script returning integer
        using var script = new Script("return 42");
        ValkeyResult result = await client.InvokeScriptAsync(script);

        Assert.NotNull(result);
        Assert.Equal(42, (long)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_ReturnsArray_ConvertsCorrectly(BaseClient client)
    {
        // Test script returning array
        using var script = new Script("return {'a', 'b', 'c'}");
        ValkeyResult result = await client.InvokeScriptAsync(script);

        Assert.NotNull(result);
        string?[]? arr = (string?[]?)result;
        Assert.NotNull(arr);
        Assert.Equal(3, arr.Length);
        Assert.Equal("a", arr[0]);
        Assert.Equal("b", arr[1]);
        Assert.Equal("c", arr[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_ReturnsNil_HandlesCorrectly(BaseClient client)
    {
        // Test script returning nil
        using var script = new Script("return nil");
        ValkeyResult result = await client.InvokeScriptAsync(script);

        Assert.NotNull(result);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_AccessesRedisData_WorksCorrectly(BaseClient client)
    {
        // Set up test data
        string key = Guid.NewGuid().ToString();
        string value = "test value";
        await client.StringSetAsync(key, value);

        // Script that reads the data
        using var script = new Script("return redis.call('GET', KEYS[1])");
        var options = new ScriptOptions().WithKeys(key);

        ValkeyResult result = await client.InvokeScriptAsync(script, options);

        Assert.NotNull(result);
        Assert.Equal(value, result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task InvokeScriptAsync_ModifiesRedisData_WorksCorrectly(BaseClient client)
    {
        // Script that sets a value
        string key = Guid.NewGuid().ToString();
        string value = "script value";

        using var script = new Script("return redis.call('SET', KEYS[1], ARGV[1])");
        var options = new ScriptOptions()
            .WithKeys(key)
            .WithArgs(value);

        ValkeyResult result = await client.InvokeScriptAsync(script, options);

        Assert.NotNull(result);
        Assert.Equal("OK", result.ToString());

        // Verify the value was set
        ValkeyValue retrievedValue = await client.StringGetAsync(key);
        Assert.Equal(value, retrievedValue.ToString());
    }

    // ===== Function Execution Tests =====

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionLoadAsync_ValidLibraryCode_ReturnsLibraryName(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "testlib_load";
        string funcName = "testfunc_load";

        // Load a simple function library
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'Hello from function' end)";

        string libraryName = await client.FunctionLoadAsync(libraryCode);

        Assert.Equal(libName, libraryName);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionLoadAsync_WithReplace_ReplacesExistingLibrary(BaseClient client)
    {
        // Flush all functions first (use routing for cluster clients)
        if (client is GlideClusterClient clusterClient)
        {
            await clusterClient.FunctionFlushAsync(Route.AllPrimaries);
        }
        else
        {
            await client.FunctionFlushAsync();
        }

        // Use hardcoded unique library name per test
        string libName = "replacelib";
        string funcName = "func_replace";

        // Load initial library (use routing for cluster clients)
        string libraryCode1 = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'version 1' end)";
        if (client is GlideClusterClient clusterClient1)
        {
            await clusterClient1.FunctionLoadAsync(libraryCode1, false, Route.AllPrimaries);
        }
        else
        {
            await client.FunctionLoadAsync(libraryCode1);
        }

        // Replace with new version (use routing for cluster clients)
        string libraryCode2 = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'version 2' end)";
        string libraryName;
        if (client is GlideClusterClient clusterClient2)
        {
            ClusterValue<string> loadResult = await clusterClient2.FunctionLoadAsync(libraryCode2, replace: true, Route.AllPrimaries);
            libraryName = loadResult.HasSingleData ? loadResult.SingleValue : loadResult.MultiValue.Values.First();
        }
        else
        {
            libraryName = await client.FunctionLoadAsync(libraryCode2, replace: true);
        }

        Assert.Equal(libName, libraryName);

        // Verify the new version is loaded (use routing for cluster clients)
        ValkeyResult result;
        if (client is GlideClusterClient clusterClient3)
        {
            ClusterValue<ValkeyResult> callResult = await clusterClient3.FCallAsync(funcName, Route.Random);
            result = callResult.HasSingleData ? callResult.SingleValue : callResult.MultiValue.Values.First();
        }
        else
        {
            result = await client.FCallAsync(funcName);
        }
        Assert.Equal("version 2", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionLoadAsync_WithoutReplace_ThrowsErrorForExistingLibrary(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "conflictlib";
        string funcName = "func_conflict";

        // Load initial library
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'test' end)";
        await client.FunctionLoadAsync(libraryCode);

        // Try to load again without replace flag
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FunctionLoadAsync(libraryCode, replace: false));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionLoadAsync_InvalidCode_ThrowsException(BaseClient client)
    {
        // Try to load invalid Lua code
        string invalidCode = @"#!lua name=invalidlib
this is not valid lua code";

        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FunctionLoadAsync(invalidCode));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_ExecutesLoadedFunction_ReturnsResult(BaseClient client)
    {
        // Flush all functions first (use routing for cluster clients)
        if (client is GlideClusterClient clusterClient)
        {
            await clusterClient.FunctionFlushAsync(Route.AllPrimaries);
        }
        else
        {
            await client.FunctionFlushAsync();
        }

        // Use hardcoded unique library name per test
        string libName = "execlib";
        string funcName = "greet";

        // Load function (use routing for cluster clients)
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'Hello, World!' end)";
        if (client is GlideClusterClient clusterClient2)
        {
            await clusterClient2.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);
        }
        else
        {
            await client.FunctionLoadAsync(libraryCode);
        }

        // Execute the function (use routing for cluster clients)
        ValkeyResult result;
        if (client is GlideClusterClient clusterClient3)
        {
            ClusterValue<ValkeyResult> callResult = await clusterClient3.FCallAsync(funcName, Route.Random);
            result = callResult.HasSingleData ? callResult.SingleValue : callResult.MultiValue.Values.First();
        }
        else
        {
            result = await client.FCallAsync(funcName);
        }

        Assert.NotNull(result);
        Assert.Equal("Hello, World!", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_WithKeysAndArgs_PassesParametersCorrectly(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "paramlib";
        string funcName = "concat";

        // Load function
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args)
    return keys[1] .. ':' .. args[1]
end)";
        await client.FunctionLoadAsync(libraryCode);

        // Execute with keys and args
        ValkeyResult result = await client.FCallAsync(funcName, ["mykey"], ["myvalue"]);

        Assert.NotNull(result);
        Assert.Equal("mykey:myvalue", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_NonExistentFunction_ThrowsException(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Try to call non-existent function
        string funcName = "nonexistent";

        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FCallAsync(funcName));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallReadOnlyAsync_ExecutesFunction_ReturnsResult(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "readonlylib";
        string funcName = "readonly_func";

        // Load function
        string libraryCode = $@"#!lua name={libName}
redis.register_function{{
    function_name='{funcName}',
    callback=function(keys, args) return 'Read-only result' end,
    flags={{'no-writes'}}
}}";
        await client.FunctionLoadAsync(libraryCode);

        // Execute in read-only mode
        ValkeyResult result = await client.FCallReadOnlyAsync(funcName);

        Assert.NotNull(result);
        Assert.Equal("Read-only result", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallReadOnlyAsync_WithKeysAndArgs_PassesParametersCorrectly(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "readonlyparamlib";
        string funcName = "readonly_concat";

        // Load function
        string libraryCode = $@"#!lua name={libName}
redis.register_function{{
    function_name='{funcName}',
    callback=function(keys, args)
        return keys[1] .. ':' .. args[1]
    end,
    flags={{'no-writes'}}
}}";
        await client.FunctionLoadAsync(libraryCode);

        // Execute with keys and args
        ValkeyResult result = await client.FCallReadOnlyAsync(funcName, ["key1"], ["value1"]);

        Assert.NotNull(result);
        Assert.Equal("key1:value1", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionFlushAsync_RemovesAllFunctions(BaseClient client)
    {
        // Flush all functions first (use routing for cluster clients)
        if (client is GlideClusterClient clusterClient)
        {
            await clusterClient.FunctionFlushAsync(Route.AllPrimaries);
        }
        else
        {
            await client.FunctionFlushAsync();
        }

        // Use hardcoded unique library name per test
        string libName = "flushlib";
        string funcName = "flushfunc";

        // Load a function (use routing for cluster clients)
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'test' end)";
        if (client is GlideClusterClient clusterClient2)
        {
            await clusterClient2.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);
        }
        else
        {
            await client.FunctionLoadAsync(libraryCode);
        }

        // Verify function exists by calling it (use routing for cluster clients)
        ValkeyResult resultBefore;
        if (client is GlideClusterClient clusterClient3)
        {
            ClusterValue<ValkeyResult> callResult = await clusterClient3.FCallAsync(funcName, Route.Random);
            resultBefore = callResult.HasSingleData ? callResult.SingleValue : callResult.MultiValue.Values.First();
        }
        else
        {
            resultBefore = await client.FCallAsync(funcName);
        }
        Assert.Equal("test", resultBefore.ToString());

        // Flush all functions (use routing for cluster clients)
        string flushResult;
        if (client is GlideClusterClient clusterClient4)
        {
            ClusterValue<string> flushResultValue = await clusterClient4.FunctionFlushAsync(Route.AllPrimaries);
            flushResult = flushResultValue.HasSingleData ? flushResultValue.SingleValue : flushResultValue.MultiValue.Values.First();
        }
        else
        {
            flushResult = await client.FunctionFlushAsync();
        }
        Assert.Equal("OK", flushResult);

        // Verify function no longer exists (use routing for cluster clients)
        if (client is GlideClusterClient clusterClient5)
        {
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await clusterClient5.FCallAsync(funcName, Route.Random));
        }
        else
        {
            await Assert.ThrowsAsync<Errors.RequestException>(async () =>
                await client.FCallAsync(funcName));
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionFlushAsync_SyncMode_RemovesAllFunctions(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "flushsynclib";
        string funcName = "flushsyncfunc";

        // Load a function
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'test' end)";
        await client.FunctionLoadAsync(libraryCode);

        // Flush with SYNC mode
        string result = await client.FunctionFlushAsync(FlushMode.Sync);
        Assert.Equal("OK", result);

        // Verify function no longer exists
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FCallAsync(funcName));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionFlushAsync_AsyncMode_RemovesAllFunctions(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "flushasynclib";
        string funcName = "flushasyncfunc";

        // Load a function
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'test' end)";
        await client.FunctionLoadAsync(libraryCode);

        // Flush with ASYNC mode
        string result = await client.FunctionFlushAsync(FlushMode.Async);
        Assert.Equal("OK", result);

        // Wait a bit for async flush to complete
        await Task.Delay(100);

        // Verify function no longer exists
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FCallAsync(funcName));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_FunctionError_ThrowsException(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "errorlib";
        string funcName = "errorfunc";

        // Load function with error
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args)
    error('Intentional error')
end)";
        await client.FunctionLoadAsync(libraryCode);

        // Execute function that throws error
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FCallAsync(funcName));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_AccessesRedisData_WorksCorrectly(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Set up test data
        string key = Guid.NewGuid().ToString();
        string value = "function test value";
        await client.StringSetAsync(key, value);

        // Use hardcoded unique library name per test
        string libName = "getlib";
        string funcName = "getvalue";

        // Load function that reads data
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args)
    return redis.call('GET', keys[1])
end)";
        await client.FunctionLoadAsync(libraryCode);

        // Execute function
        ValkeyResult result = await client.FCallAsync(funcName, [key], []);

        Assert.NotNull(result);
        Assert.Equal(value, result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_ModifiesRedisData_WorksCorrectly(BaseClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "setlib";
        string funcName = "setvalue";

        // Load function that sets data
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args)
    return redis.call('SET', keys[1], args[1])
end)";
        await client.FunctionLoadAsync(libraryCode);

        // Execute function to set value
        string key = Guid.NewGuid().ToString();
        string value = "function set value";
        ValkeyResult result = await client.FCallAsync(funcName, [key], [value]);

        Assert.NotNull(result);
        Assert.Equal("OK", result.ToString());

        // Verify the value was set
        ValkeyValue retrievedValue = await client.StringGetAsync(key);
        Assert.Equal(value, retrievedValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_ReturnsInteger_ConvertsCorrectly(BaseClient client)
    {
        // Flush all functions first (use routing for cluster clients)
        if (client is GlideClusterClient clusterClient)
        {
            await clusterClient.FunctionFlushAsync(Route.AllPrimaries);
        }
        else
        {
            await client.FunctionFlushAsync();
        }

        // Use hardcoded unique library name per test
        string libName = "intlib";
        string funcName = "returnint";

        // Load function returning integer (use routing for cluster clients)
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 42 end)";
        if (client is GlideClusterClient clusterClient2)
        {
            await clusterClient2.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);
        }
        else
        {
            await client.FunctionLoadAsync(libraryCode);
        }

        ValkeyResult result;
        if (client is GlideClusterClient clusterClient3)
        {
            ClusterValue<ValkeyResult> callResult = await clusterClient3.FCallAsync(funcName, Route.Random);
            result = callResult.HasSingleData ? callResult.SingleValue : callResult.MultiValue.Values.First();
        }
        else
        {
            result = await client.FCallAsync(funcName);
        }

        Assert.NotNull(result);
        Assert.Equal(42, (long)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_ReturnsArray_ConvertsCorrectly(BaseClient client)
    {
        // Flush all functions first (use routing for cluster clients)
        if (client is GlideClusterClient clusterClient)
        {
            await clusterClient.FunctionFlushAsync(Route.AllPrimaries);
        }
        else
        {
            await client.FunctionFlushAsync();
        }

        // Use hardcoded unique library name per test
        string libName = "arraylib";
        string funcName = "returnarray";

        // Load function returning array (use routing for cluster clients)
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return {{'a', 'b', 'c'}} end)";
        if (client is GlideClusterClient clusterClient2)
        {
            await clusterClient2.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);
        }
        else
        {
            await client.FunctionLoadAsync(libraryCode);
        }

        ValkeyResult result;
        if (client is GlideClusterClient clusterClient3)
        {
            ClusterValue<ValkeyResult> callResult = await clusterClient3.FCallAsync(funcName, Route.Random);
            result = callResult.HasSingleData ? callResult.SingleValue : callResult.MultiValue.Values.First();
        }
        else
        {
            result = await client.FCallAsync(funcName);
        }

        Assert.NotNull(result);
        string?[]? arr = (string?[]?)result;
        Assert.NotNull(arr);
        Assert.Equal(3, arr.Length);
        Assert.Equal("a", arr[0]);
        Assert.Equal("b", arr[1]);
        Assert.Equal("c", arr[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_ReturnsNil_HandlesCorrectly(BaseClient client)
    {
        // Skip for cluster clients - nil handling with routing needs investigation
        Assert.SkipWhen(client is GlideClusterClient, "Nil handling with cluster routing needs investigation");

        // Flush all functions first
        await client.FunctionFlushAsync();

        // Use hardcoded unique library name per test
        string libName = "nillib";
        string funcName = "returnnil";

        // Load function returning nil
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return nil end)";
        await client.FunctionLoadAsync(libraryCode);

        ValkeyResult result = await client.FCallAsync(funcName);

        Assert.NotNull(result);
        Assert.True(result.IsNull);
    }

    // ===== Standalone-Specific Function Tests =====

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionListAsync_ReturnsAllLibraries(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load multiple libraries
        string lib1Code = @"#!lua name=testlib1
redis.register_function('func1', function(keys, args) return 'result1' end)";
        string lib2Code = @"#!lua name=testlib2
redis.register_function('func2', function(keys, args) return 'result2' end)";

        await client.FunctionLoadAsync(lib1Code);
        await client.FunctionLoadAsync(lib2Code);

        // List all libraries
        LibraryInfo[] libraries = await client.FunctionListAsync();

        Assert.NotNull(libraries);
        Assert.True(libraries.Length >= 2);
        Assert.Contains(libraries, lib => lib.Name == "testlib1");
        Assert.Contains(libraries, lib => lib.Name == "testlib2");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionListAsync_WithLibraryNameFilter_ReturnsMatchingLibrary(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load multiple libraries
        string lib1Code = @"#!lua name=filterlib1
redis.register_function('func1', function(keys, args) return 'result1' end)";
        string lib2Code = @"#!lua name=filterlib2
redis.register_function('func2', function(keys, args) return 'result2' end)";

        await client.FunctionLoadAsync(lib1Code);
        await client.FunctionLoadAsync(lib2Code);

        // List with filter
        var query = new FunctionListQuery().ForLibrary("filterlib1");
        LibraryInfo[] libraries = await client.FunctionListAsync(query);

        Assert.NotNull(libraries);
        Assert.Single(libraries);
        Assert.Equal("filterlib1", libraries[0].Name);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionListAsync_WithCodeFlag_IncludesSourceCode(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load a library
        string libCode = @"#!lua name=codelib
redis.register_function('codefunc', function(keys, args) return 'result' end)";
        await client.FunctionLoadAsync(libCode);

        // List with code
        var query = new FunctionListQuery().IncludeCode();
        LibraryInfo[] libraries = await client.FunctionListAsync(query);

        Assert.NotNull(libraries);
        var lib = libraries.FirstOrDefault(l => l.Name == "codelib");
        Assert.NotNull(lib);
        Assert.NotNull(lib.Code);
        Assert.Contains("codefunc", lib.Code);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionStatsAsync_ReturnsStatistics(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load a library
        string libCode = @"#!lua name=statslib
redis.register_function('statsfunc', function(keys, args) return 'result' end)";
        string libName = await client.FunctionLoadAsync(libCode);
        Assert.Equal("statslib", libName);

        // Verify the function was loaded
        var libraries = await client.FunctionListAsync();
        Assert.NotEmpty(libraries);
        Assert.Contains(libraries, lib => lib.Name == "statslib");

        // Get stats
        FunctionStatsResult stats = await client.FunctionStatsAsync();

        Assert.NotNull(stats);
        Assert.NotNull(stats.Engines);
        Assert.True(stats.Engines.Count > 0);

        // Check LUA engine stats
        Assert.True(stats.Engines.ContainsKey("LUA"));
        EngineStats luaStats = stats.Engines["LUA"];
        Assert.NotNull(luaStats);
        Assert.Equal(1, luaStats.FunctionCount);
        Assert.Equal(1, luaStats.LibraryCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionDeleteAsync_RemovesLibrary(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load a library
        string libCode = @"#!lua name=deletelib
redis.register_function('deletefunc', function(keys, args) return 'result' end)";
        await client.FunctionLoadAsync(libCode);

        // Verify it exists
        var libraries = await client.FunctionListAsync(new FunctionListQuery().ForLibrary("deletelib"));
        Assert.Single(libraries);

        // Delete the library
        string result = await client.FunctionDeleteAsync("deletelib");
        Assert.Equal("OK", result);

        // Verify it no longer exists
        libraries = await client.FunctionListAsync(new FunctionListQuery().ForLibrary("deletelib"));
        Assert.Empty(libraries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionDeleteAsync_NonExistentLibrary_ThrowsException(GlideClient client)
    {
        // Try to delete non-existent library
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FunctionDeleteAsync("nonexistentlib"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionKillAsync_NoFunctionRunning_ThrowsException(GlideClient client)
    {
        // Try to kill when no function is running
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FunctionKillAsync());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionDumpAsync_CreatesBackup(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load a library
        string libCode = @"#!lua name=dumplib
redis.register_function('dumpfunc', function(keys, args) return 'result' end)";
        await client.FunctionLoadAsync(libCode);

        // Dump functions
        byte[] backup = await client.FunctionDumpAsync();

        Assert.NotNull(backup);
        Assert.True(backup.Length > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionRestoreAsync_WithAppendPolicy_RestoresFunctions(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load and dump a library
        string libCode = @"#!lua name=restorelib1
redis.register_function('restorefunc1', function(keys, args) return 'result1' end)";
        await client.FunctionLoadAsync(libCode);
        byte[] backup = await client.FunctionDumpAsync();

        // Flush and restore with APPEND (default)
        await client.FunctionFlushAsync();
        string result = await client.FunctionRestoreAsync(backup);
        Assert.Equal("OK", result);

        // Verify library was restored
        var libraries = await client.FunctionListAsync(new FunctionListQuery().ForLibrary("restorelib1"));
        Assert.Single(libraries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionRestoreAsync_WithFlushPolicy_DeletesExistingFunctions(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load two libraries
        string lib1Code = @"#!lua name=flushlib1
redis.register_function('flushfunc1', function(keys, args) return 'result1' end)";
        string lib2Code = @"#!lua name=flushlib2
redis.register_function('flushfunc2', function(keys, args) return 'result2' end)";

        await client.FunctionLoadAsync(lib1Code);
        byte[] backup = await client.FunctionDumpAsync();

        await client.FunctionLoadAsync(lib2Code);

        // Restore with FLUSH policy
        string result = await client.FunctionRestoreAsync(backup, FunctionRestorePolicy.Flush);
        Assert.Equal("OK", result);

        // Verify only lib1 exists
        var libraries = await client.FunctionListAsync();
        Assert.Single(libraries);
        Assert.Equal("flushlib1", libraries[0].Name);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionRestoreAsync_WithReplacePolicy_OverwritesConflictingFunctions(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load a library
        string lib1Code = @"#!lua name=replacelib
redis.register_function('replacefunc', function(keys, args) return 'version1' end)";
        await client.FunctionLoadAsync(lib1Code);
        byte[] backup = await client.FunctionDumpAsync();

        // Load a different version of the same library
        string lib2Code = @"#!lua name=replacelib
redis.register_function('replacefunc', function(keys, args) return 'version2' end)";
        await client.FunctionLoadAsync(lib2Code, replace: true);

        // Restore with REPLACE policy
        string result = await client.FunctionRestoreAsync(backup, FunctionRestorePolicy.Replace);
        Assert.Equal("OK", result);

        // Verify the function was replaced (should return version1)
        ValkeyResult funcResult = await client.FCallAsync("replacefunc");
        Assert.Equal("version1", funcResult.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionRestoreAsync_ConflictingLibraryWithAppend_ThrowsException(GlideClient client)
    {
        // Flush all functions first
        await client.FunctionFlushAsync();

        // Load a library
        string libCode = @"#!lua name=conflictlib
redis.register_function('conflictfunc', function(keys, args) return 'result' end)";
        await client.FunctionLoadAsync(libCode);
        byte[] backup = await client.FunctionDumpAsync();

        // Try to restore with APPEND policy (should fail because library already exists)
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FunctionRestoreAsync(backup, FunctionRestorePolicy.Append));
    }

    // ===== Cluster-Specific Function Tests =====

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_WithAllPrimariesRouting_ExecutesOnAllPrimaries(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_allprimaries_lib";
        string funcName = "cluster_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'Hello from primary' end)";
        ClusterValue<string> loadResult = await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Verify load succeeded (may be single or multi-value depending on cluster configuration)
        if (loadResult.HasMultiData)
        {
            Assert.True(loadResult.MultiValue.Count > 0);
            Assert.All(loadResult.MultiValue.Values, name => Assert.Equal(libName, name));
        }
        else
        {
            Assert.Equal(libName, loadResult.SingleValue);
        }

        // Execute function on all primaries
        ClusterValue<ValkeyResult> result = await client.FCallAsync(funcName, Route.AllPrimaries);

        // Verify execution (may be single or multi-value depending on cluster configuration)
        if (result.HasMultiData)
        {
            Assert.True(result.MultiValue.Count > 0);
            Assert.All(result.MultiValue.Values, r => Assert.Equal("Hello from primary", r.ToString()));
        }
        else
        {
            Assert.Equal("Hello from primary", result.SingleValue.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_WithAllNodesRouting_ExecutesOnAllNodes(GlideClusterClient client)
    {
        // Flush all functions first (must use AllPrimaries since replicas are read-only)
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_allnodes_lib";
        string funcName = "cluster_allnodes_func";

        // Load function on all primaries (can't load on replicas - they're read-only)
        string libraryCode = $@"#!lua name={libName}
redis.register_function{{
    function_name='{funcName}',
    callback=function(keys, args) return 'Hello from node' end,
    flags={{'no-writes'}}
}}";
        ClusterValue<string> loadResult = await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Verify load succeeded (may be single or multi-value depending on cluster configuration)
        if (loadResult.HasMultiData)
        {
            Assert.True(loadResult.MultiValue.Count > 0);
        }
        else
        {
            Assert.Equal(libName, loadResult.SingleValue);
        }

        // Execute read-only function on all nodes
        ClusterValue<ValkeyResult> result = await client.FCallReadOnlyAsync(funcName, Route.AllNodes);

        // Verify execution (may be single or multi-value depending on cluster configuration)
        if (result.HasMultiData)
        {
            Assert.True(result.MultiValue.Count > 0);
            Assert.All(result.MultiValue.Values, r => Assert.Equal("Hello from node", r.ToString()));
        }
        else
        {
            Assert.Equal("Hello from node", result.SingleValue.ToString());
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FCallAsync_WithRandomRouting_ExecutesOnSingleNode(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_random_lib";
        string funcName = "cluster_random_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'Random node result' end)";
        await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Execute function on random node
        ClusterValue<ValkeyResult> result = await client.FCallAsync(funcName, Route.Random);

        // Verify execution on single node
        Assert.True(result.HasSingleData);
        Assert.Equal("Random node result", result.SingleValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionLoadAsync_WithRouting_LoadsOnSpecifiedNodes(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_load_lib";
        string funcName = "cluster_load_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'Loaded' end)";
        ClusterValue<string> result = await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Verify load succeeded (may be single or multi-value depending on cluster configuration)
        if (result.HasMultiData)
        {
            Assert.All(result.MultiValue.Values, name => Assert.Equal(libName, name));
        }
        else
        {
            Assert.Equal(libName, result.SingleValue);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionDeleteAsync_WithRouting_DeletesFromSpecifiedNodes(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_delete_lib";
        string funcName = "cluster_delete_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'test' end)";
        await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Verify function exists by calling it
        ClusterValue<ValkeyResult> callResult = await client.FCallAsync(funcName, Route.Random);
        Assert.Equal("test", callResult.SingleValue.ToString());

        // Delete function from all primaries
        ClusterValue<string> deleteResult = await client.FunctionDeleteAsync(libName, Route.AllPrimaries);

        // Verify delete succeeded (may be single or multi-value depending on cluster configuration)
        if (deleteResult.HasMultiData)
        {
            Assert.All(deleteResult.MultiValue.Values, r => Assert.Equal("OK", r));
        }
        else
        {
            Assert.Equal("OK", deleteResult.SingleValue);
        }

        // Verify function no longer exists
        await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.FCallAsync(funcName, Route.Random));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionListAsync_WithRouting_ReturnsLibrariesFromSpecifiedNodes(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_list_lib";
        string funcName = "cluster_list_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'test' end)";
        await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // List functions from all primaries
        ClusterValue<LibraryInfo[]> result = await client.FunctionListAsync(null, Route.AllPrimaries);

        // Verify list returned (may be single or multi-value depending on cluster configuration)
        if (result.HasMultiData)
        {
            Assert.True(result.MultiValue.Count > 0);
            // Verify each node has the library
            foreach (var (node, libraries) in result.MultiValue)
            {
                Assert.NotEmpty(libraries);
                Assert.Contains(libraries, lib => lib.Name == libName);
            }
        }
        else
        {
            Assert.NotEmpty(result.SingleValue);
            Assert.Contains(result.SingleValue, lib => lib.Name == libName);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionStatsAsync_WithRouting_ReturnsPerNodeStats(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_stats_lib";
        string funcName = "cluster_stats_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'test' end)";
        await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Get stats from all primaries
        ClusterValue<FunctionStatsResult> result = await client.FunctionStatsAsync(Route.AllPrimaries);

        // Verify stats returned (may be single or multi-value depending on cluster configuration)
        if (result.HasMultiData)
        {
            Assert.True(result.MultiValue.Count > 0);
            // Verify each node has stats
            foreach (var (node, stats) in result.MultiValue)
            {
                Assert.NotNull(stats);
                Assert.NotNull(stats.Engines);
                // Engines should contain LUA if available
                if (stats.Engines.Count > 0)
                {
                    Assert.Contains("LUA", stats.Engines.Keys);
                }
            }
        }
        else
        {
            Assert.NotNull(result.SingleValue);
            Assert.NotNull(result.SingleValue.Engines);
            // Engines should contain LUA if available
            if (result.SingleValue.Engines.Count > 0)
            {
                Assert.Contains("LUA", result.SingleValue.Engines.Keys);
            }
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionDumpAsync_WithRouting_CreatesBackupFromSpecifiedNode(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_dump_lib";
        string funcName = "cluster_dump_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'test' end)";
        await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Dump functions from random node
        ClusterValue<byte[]> result = await client.FunctionDumpAsync(Route.Random);

        // Verify dump succeeded on single node
        Assert.True(result.HasSingleData);
        Assert.NotNull(result.SingleValue);
        Assert.NotEmpty(result.SingleValue);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionRestoreAsync_WithRouting_RestoresToSpecifiedNodes(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_restore_lib";
        string funcName = "cluster_restore_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'restored' end)";
        await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Dump functions from random node
        ClusterValue<byte[]> dumpResult = await client.FunctionDumpAsync(Route.Random);
        byte[] backup = dumpResult.SingleValue;

        // Flush all functions
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Restore functions to all primaries
        ClusterValue<string> restoreResult = await client.FunctionRestoreAsync(backup, Route.AllPrimaries);

        // Verify restore succeeded (may be single or multi-value depending on cluster configuration)
        if (restoreResult.HasMultiData)
        {
            Assert.All(restoreResult.MultiValue.Values, r => Assert.Equal("OK", r));
        }
        else
        {
            Assert.Equal("OK", restoreResult.SingleValue);
        }

        // Verify function is restored by calling it
        ClusterValue<ValkeyResult> callResult = await client.FCallAsync(funcName, Route.Random);
        Assert.Equal("restored", callResult.SingleValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FunctionRestoreAsync_WithReplacePolicy_ReplacesExistingFunctions(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_replace_lib";
        string funcName = "cluster_replace_func";

        // Load initial function
        string libraryCode1 = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'version 1' end)";
        await client.FunctionLoadAsync(libraryCode1, false, Route.AllPrimaries);

        // Dump functions
        ClusterValue<byte[]> dumpResult = await client.FunctionDumpAsync(Route.Random);
        byte[] backup = dumpResult.SingleValue;

        // Load different version
        string libraryCode2 = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'version 2' end)";
        await client.FunctionLoadAsync(libraryCode2, true, Route.AllPrimaries);

        // Restore with REPLACE policy
        ClusterValue<string> restoreResult = await client.FunctionRestoreAsync(
            backup,
            FunctionRestorePolicy.Replace,
            Route.AllPrimaries);

        // Verify restore succeeded (may be single or multi-value depending on cluster configuration)
        if (restoreResult.HasMultiData)
        {
            Assert.All(restoreResult.MultiValue.Values, r => Assert.Equal("OK", r));
        }
        else
        {
            Assert.Equal("OK", restoreResult.SingleValue);
        }

        // Verify original version is restored
        ClusterValue<ValkeyResult> callResult = await client.FCallAsync(funcName, Route.Random);
        Assert.Equal("version 1", callResult.SingleValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task ClusterValue_MultiNodeResults_HandlesCorrectly(GlideClusterClient client)
    {

        // Flush all functions first
        await client.FunctionFlushAsync(Route.AllPrimaries);

        // Use hardcoded unique library name per test
        string libName = "cluster_multinode_lib";
        string funcName = "cluster_multinode_func";

        // Load function on all primaries
        string libraryCode = $@"#!lua name={libName}
redis.register_function('{funcName}', function(keys, args) return 'multi-node result' end)";
        ClusterValue<string> loadResult = await client.FunctionLoadAsync(libraryCode, false, Route.AllPrimaries);

        // Test ClusterValue properties (may be single or multi-value depending on cluster configuration)
        if (loadResult.HasMultiData)
        {
            Assert.False(loadResult.HasSingleData);
            Assert.NotNull(loadResult.MultiValue);
            Assert.True(loadResult.MultiValue.Count > 0);

            // Verify each node address is a key in the dictionary
            foreach (var (nodeAddress, libraryName) in loadResult.MultiValue)
            {
                Assert.NotNull(nodeAddress);
                Assert.NotEmpty(nodeAddress);
                Assert.Equal(libName, libraryName);
            }
        }
        else
        {
            Assert.True(loadResult.HasSingleData);
            Assert.False(loadResult.HasMultiData);
            Assert.Equal(libName, loadResult.SingleValue);
        }
    }
}
