// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection;

using Valkey.Glide.Commands;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Bug condition exploration tests that verify affected methods return <see cref="Task"/>
/// (void) instead of <see cref="Task{TResult}"/> with "OK" strings.
/// </summary>
/// <remarks>
/// <b>Validates: Requirements 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8</b>
/// </remarks>
public class OkStringReturnTypeTests
{
    #region Helpers

    /// <summary>
    /// Gets the return type of a named method from an interface type.
    /// When multiple overloads exist, returns the first match (all overloads share the same return type for affected methods).
    /// </summary>
    private static Type GetMethodReturnType<TInterface>(string methodName)
    {
        MethodInfo? method = typeof(TInterface).GetMethod(methodName);
        Assert.NotNull(method);
        return method.ReturnType;
    }

    /// <summary>
    /// Asserts that all overloads of a method on the given interface return <see cref="Task"/> (void).
    /// </summary>
    private static void AssertAllOverloadsReturnTask<TInterface>(string methodName)
    {
        MethodInfo[] methods = [.. typeof(TInterface)
            .GetMethods()
            .Where(m => m.Name == methodName)];

        Assert.NotEmpty(methods);

        foreach (MethodInfo method in methods)
        {
            Assert.Equal(
                typeof(Task),
                method.ReturnType);
        }
    }

    #endregion

    #region Tests

    [Theory]
    [InlineData(nameof(IScriptingAndFunctionBaseCommands.ScriptFlushAsync))]
    [InlineData(nameof(IScriptingAndFunctionBaseCommands.ScriptKillAsync))]
    [InlineData(nameof(IScriptingAndFunctionBaseCommands.FunctionFlushAsync))]
    public void BaseCommands_VoidMethods_ShouldReturnTask(string methodName)
    {
        AssertAllOverloadsReturnTask<IScriptingAndFunctionBaseCommands>(methodName);
    }

    [Theory]
    [InlineData(nameof(IScriptingAndFunctionStandaloneCommands.FunctionDeleteAsync))]
    [InlineData(nameof(IScriptingAndFunctionStandaloneCommands.FunctionKillAsync))]
    [InlineData(nameof(IScriptingAndFunctionStandaloneCommands.FunctionRestoreAsync))]
    public void StandaloneCommands_VoidMethods_ShouldReturnTask(string methodName)
    {
        AssertAllOverloadsReturnTask<IScriptingAndFunctionStandaloneCommands>(methodName);
    }

    [Fact]
    public void ServerManagementCommands_SelectAsync_ShouldReturnTask()
    {
        AssertAllOverloadsReturnTask<IServerManagementCommands>(
            nameof(IServerManagementCommands.SelectAsync));
    }

    [Fact]
    public void ServerManagementClusterCommands_SelectAsync_ShouldReturnTask()
    {
        AssertAllOverloadsReturnTask<IServerManagementClusterCommands>(
            nameof(IServerManagementClusterCommands.SelectAsync));
    }

    [Theory]
    [InlineData(nameof(IScriptingAndFunctionClusterCommands.ScriptFlushAsync))]
    [InlineData(nameof(IScriptingAndFunctionClusterCommands.ScriptKillAsync))]
    [InlineData(nameof(IScriptingAndFunctionClusterCommands.FunctionDeleteAsync))]
    [InlineData(nameof(IScriptingAndFunctionClusterCommands.FunctionFlushAsync))]
    [InlineData(nameof(IScriptingAndFunctionClusterCommands.FunctionKillAsync))]
    [InlineData(nameof(IScriptingAndFunctionClusterCommands.FunctionRestoreAsync))]
    public void ClusterCommands_VoidMethods_ShouldReturnTask(string methodName)
    {
        AssertAllOverloadsReturnTask<IScriptingAndFunctionClusterCommands>(methodName);
    }

    #endregion
}
