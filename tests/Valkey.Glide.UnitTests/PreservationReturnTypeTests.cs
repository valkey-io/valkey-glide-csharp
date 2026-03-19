// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection;

using Valkey.Glide.Commands;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Preservation property tests that verify methods NOT affected by the OK-string-return bugfix
/// retain their original return types. These tests must pass on both unfixed and fixed code.
/// </summary>
/// <remarks>
/// <b>Validates: Requirements 3.1, 3.2, 3.3</b>
/// </remarks>
public class PreservationReturnTypeTests
{
    #region Helpers

    /// <summary>
    /// Gets the return type of a named method from an interface type.
    /// </summary>
    private static Type GetMethodReturnType<TInterface>(string methodName)
    {
        MethodInfo? method = typeof(TInterface).GetMethod(methodName);
        Assert.NotNull(method);
        return method.ReturnType;
    }

    /// <summary>
    /// Asserts that all overloads of a method on the given interface return the expected type.
    /// </summary>
    private static void AssertAllOverloadsReturn<TInterface>(string methodName, Type expectedReturnType)
    {
        MethodInfo[] methods = [.. typeof(TInterface)
            .GetMethods()
            .Where(m => m.Name == methodName)];

        Assert.NotEmpty(methods);

        foreach (MethodInfo method in methods)
        {
            Assert.Equal(expectedReturnType, method.ReturnType);
        }
    }

    /// <summary>
    /// Gets the generic type definition of a static method's return type from the Request class.
    /// </summary>
    private static Type GetRequestBuilderReturnType(string methodName)
    {
        MethodInfo? method = typeof(Request)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == methodName);

        Assert.NotNull(method);
        return method.ReturnType;
    }

    #endregion

    #region Preservation — Data-Returning Methods Remain Task<string> or Task<string?>

    [Fact]
    public void Preservation_FunctionLoadAsync_ReturnsTaskString()
    {
        Type returnType = GetMethodReturnType<IScriptingAndFunctionBaseCommands>(
            nameof(IScriptingAndFunctionBaseCommands.FunctionLoadAsync));

        Assert.Equal(typeof(Task<string>), returnType);
    }

    [Fact]
    public void Preservation_ScriptShowAsync_ReturnsTaskNullableString()
    {
        Type returnType = GetMethodReturnType<IScriptingAndFunctionBaseCommands>(
            nameof(IScriptingAndFunctionBaseCommands.ScriptShowAsync));

        Assert.Equal(typeof(Task<string?>), returnType);
    }

    [Fact]
    public void Preservation_InfoAsync_Standalone_ReturnsTaskString()
    {
        AssertAllOverloadsReturn<IServerManagementCommands>(
            nameof(IServerManagementCommands.InfoAsync),
            typeof(Task<string>));
    }

    [Fact]
    public void Preservation_LolwutAsync_Standalone_ReturnsTaskString()
    {
        Type returnType = GetMethodReturnType<IServerManagementCommands>(
            nameof(IServerManagementCommands.LolwutAsync));

        Assert.Equal(typeof(Task<string>), returnType);
    }

    #endregion

    #region Preservation — Void-Returning Methods Remain Task

    [Theory]
    [InlineData(nameof(IServerManagementCommands.ConfigResetStatisticsAsync))]
    [InlineData(nameof(IServerManagementCommands.ConfigRewriteAsync))]
    [InlineData(nameof(IServerManagementCommands.ConfigSetAsync))]
    [InlineData(nameof(IServerManagementCommands.FlushAllDatabasesAsync))]
    [InlineData(nameof(IServerManagementCommands.FlushDatabaseAsync))]
    public void Preservation_VoidMethods_Standalone_RemainTask(string methodName)
    {
        AssertAllOverloadsReturn<IServerManagementCommands>(methodName, typeof(Task));
    }

    [Theory]
    [InlineData(nameof(IServerManagementClusterCommands.ConfigResetStatisticsAsync))]
    [InlineData(nameof(IServerManagementClusterCommands.ConfigRewriteAsync))]
    [InlineData(nameof(IServerManagementClusterCommands.ConfigSetAsync))]
    [InlineData(nameof(IServerManagementClusterCommands.FlushAllDatabasesAsync))]
    [InlineData(nameof(IServerManagementClusterCommands.FlushDatabaseAsync))]
    public void Preservation_VoidMethods_Cluster_RemainTask(string methodName)
    {
        AssertAllOverloadsReturn<IServerManagementClusterCommands>(methodName, typeof(Task));
    }

    #endregion

    #region Preservation — Bool-Returning Methods Remain Task<bool>

    [Fact]
    public void Preservation_StringSetAsync_ReturnsTaskBool()
    {
        AssertAllOverloadsReturn<IStringCommands>(
            nameof(IStringCommands.StringSetAsync),
            typeof(Task<bool>));
    }

    [Fact]
    public void Preservation_KeyRenameAsync_ReturnsTaskBool()
    {
        Type returnType = GetMethodReturnType<IGenericBaseCommands>(
            nameof(IGenericBaseCommands.KeyRenameAsync));

        Assert.Equal(typeof(Task<bool>), returnType);
    }

    #endregion

    #region Preservation — Request Builder Return Types for Internal Cleanup Targets

    [Fact]
    public void Preservation_RequestHashSetAsync_ReturnsCmdStringObjectNullable()
    {
        // HashSetAsync(ValkeyKey, HashEntry[]) — the HMSet builder
        Type returnType = GetRequestBuilderReturnType(nameof(Request.HashSetAsync));

        Assert.Equal(typeof(Cmd<string, object?>), returnType);
    }

    [Fact]
    public void Preservation_RequestHyperLogLogMergeAsync_ReturnsCmdStringObjectNullable()
    {
        MethodInfo[] methods = [.. typeof(Request)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == nameof(Request.HyperLogLogMergeAsync))];

        Assert.NotEmpty(methods);

        foreach (MethodInfo method in methods)
        {
            Assert.Equal(typeof(Cmd<string, object?>), method.ReturnType);
        }
    }

    [Fact]
    public void Preservation_RequestKeyRestoreAsync_ReturnsCmdStringObjectNullable()
    {
        Type returnType = GetRequestBuilderReturnType(nameof(Request.KeyRestoreAsync));

        Assert.Equal(typeof(Cmd<string, object?>), returnType);
    }

    [Fact]
    public void Preservation_RequestClientSetName_ReturnsCmdStringObjectNullable()
    {
        Type returnType = GetRequestBuilderReturnType(nameof(Request.ClientSetName));

        Assert.Equal(typeof(Cmd<string, object?>), returnType);
    }

    #endregion
}
