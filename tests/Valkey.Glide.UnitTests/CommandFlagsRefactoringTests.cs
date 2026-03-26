// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection;

using Valkey.Glide.Commands;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Property-based tests to verify the CommandFlags refactoring is complete and correct.
/// These tests validate that GLIDE interfaces are free of CommandFlags and that SER interfaces
/// provide matching overloads.
/// </summary>
public class CommandFlagsRefactoringTests
{
    #region Data

    /// <summary>
    /// All GLIDE command interfaces that should NOT have CommandFlags parameters.
    /// </summary>
    public static readonly Type[] GlideCommandInterfaces =
    [
        typeof(IStringCommands),
        typeof(IHashCommands),
        typeof(IListCommands),
        typeof(ISetCommands),
        typeof(ISortedSetCommands),
        typeof(IGenericBaseCommands),
        typeof(IGenericCommands),
        typeof(IGenericClusterCommands),
        typeof(IBitmapCommands),
        typeof(IHyperLogLogCommands),
        typeof(IStreamCommands),
        typeof(IGeospatialCommands),
        typeof(IConnectionManagementCommands),
        typeof(IConnectionManagementClusterCommands),
        typeof(IServerManagementCommands),
        typeof(IServerManagementClusterCommands),
        typeof(IScriptingAndFunctionBaseCommands),
        typeof(IScriptingAndFunctionStandaloneCommands),
        typeof(IScriptingAndFunctionClusterCommands),
        typeof(ITransactionBaseCommands),
        typeof(ITransactionCommands),
        typeof(ITransactionClusterCommands),
        typeof(IPubSubCommands),
        typeof(IPubSubClusterCommands),
    ];

    public static TheoryData<Type> GlideInterfaceData
    {
        get
        {
            var data = new TheoryData<Type>();
            foreach (var type in GlideCommandInterfaces)
            {
                data.Add(type);
            }
            return data;
        }
    }

    #endregion

    #region Tests

    /// <summary>
    /// Property 1: GLIDE interfaces are free of CommandFlags.
    /// Verifies that no method in any GLIDE command interface has a CommandFlags parameter.
    /// </summary>
    /// <remarks>
    /// Tag: Feature: remove-command-flags-from-glide-interfaces, Property 1: GLIDE interfaces are free of CommandFlags
    /// Validates: Requirements 1.1, 6.1, 6.2
    /// </remarks>
    [Theory]
    [MemberData(nameof(GlideInterfaceData))]
    public void GlideInterface_HasNoCommandFlagsParameters(Type interfaceType)
    {
        var methods = interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            var hasCommandFlags = parameters.Any(p => p.ParameterType == typeof(CommandFlags));

            Assert.False(hasCommandFlags,
                $"Method {interfaceType.Name}.{method.Name} should not have a CommandFlags parameter. " +
                $"CommandFlags should only exist in SER compatibility interfaces (IDatabaseAsync, IDatabase).");
        }
    }

    /// <summary>
    /// Property 1 (aggregate): All GLIDE interfaces are free of CommandFlags.
    /// Single test that checks all interfaces at once for a comprehensive view.
    /// </summary>
    /// <remarks>
    /// Tag: Feature: remove-command-flags-from-glide-interfaces, Property 1: GLIDE interfaces are free of CommandFlags
    /// Validates: Requirements 1.1, 6.1, 6.2
    /// </remarks>
    [Fact]
    public void AllGlideInterfaces_AreCommandFlagsFree()
    {
        var violations = new List<string>();

        foreach (var interfaceType in GlideCommandInterfaces)
        {
            var methods = interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Any(p => p.ParameterType == typeof(CommandFlags)))
                {
                    violations.Add($"{interfaceType.Name}.{method.Name}");
                }
            }
        }

        Assert.Empty(violations);
    }

    /// <summary>
    /// Property 2: SER interfaces provide CommandFlags overloads.
    /// Verifies that IDatabaseAsync has methods with CommandFlags parameters for SER compatibility.
    /// </summary>
    /// <remarks>
    /// Tag: Feature: remove-command-flags-from-glide-interfaces, Property 2: SER interfaces provide matching CommandFlags overloads
    /// Validates: Requirements 3.1, 3.2, 5.3
    /// </remarks>
    [Fact]
    public void IDatabaseAsync_HasCommandFlagsOverloads()
    {
        var databaseAsyncType = typeof(IDatabaseAsync);
        var methods = databaseAsyncType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        var methodsWithCommandFlags = methods
            .Where(m => m.GetParameters().Any(p => p.ParameterType == typeof(CommandFlags)))
            .ToList();

        // Verify that IDatabaseAsync has CommandFlags overloads
        Assert.NotEmpty(methodsWithCommandFlags);

        // Verify key command categories have CommandFlags overloads
        var methodNames = methodsWithCommandFlags.Select(m => m.Name).ToHashSet();

        // String commands
        Assert.Contains("StringSetAsync", methodNames);
        Assert.Contains("StringGetAsync", methodNames);

        // Hash commands
        Assert.Contains("HashGetAsync", methodNames);
        Assert.Contains("HashSetAsync", methodNames);

        // List commands
        Assert.Contains("ListLeftPushAsync", methodNames);
        Assert.Contains("ListRightPushAsync", methodNames);

        // Set commands
        Assert.Contains("SetAddAsync", methodNames);
        Assert.Contains("SetRemoveAsync", methodNames);

        // Sorted set commands
        Assert.Contains("SortedSetAddAsync", methodNames);

        // Generic commands
        Assert.Contains("KeyDeleteAsync", methodNames);
        Assert.Contains("KeyExistsAsync", methodNames);

        // Server management commands
        Assert.Contains("EchoAsync", methodNames);

        // Connection management commands
        Assert.Contains("ClientIdAsync", methodNames);
        Assert.Contains("ClientGetNameAsync", methodNames);

        // Scripting commands
        Assert.Contains("ScriptEvaluateAsync", methodNames);

        // Transaction commands
        Assert.Contains("WatchAsync", methodNames);
        Assert.Contains("UnwatchAsync", methodNames);
    }

    /// <summary>
    /// Property 3: SER overloads reject non-None CommandFlags.
    /// Verifies that Database CommandFlags overloads throw NotImplementedException for non-None flags.
    /// </summary>
    /// <remarks>
    /// Tag: Feature: remove-command-flags-from-glide-interfaces, Property 3: SER overloads reject non-None CommandFlags
    /// Validates: Requirements 4.2, 5.2
    /// </remarks>
    [Theory]
    [InlineData(CommandFlags.DemandMaster)]
    [InlineData(CommandFlags.DemandReplica)]
    [InlineData(CommandFlags.PreferReplica)]
    [InlineData(CommandFlags.FireAndForget)]
    [InlineData(CommandFlags.NoRedirect)]
    [InlineData(CommandFlags.NoScriptCache)]
    public void GuardClauses_ThrowsForNonNoneCommandFlags(CommandFlags flags)
    {
        var exception = Assert.Throws<NotImplementedException>(() =>
            Internals.GuardClauses.ThrowIfCommandFlags(flags));

        Assert.Contains("Command flags are not supported by GLIDE", exception.Message);
    }

    /// <summary>
    /// Property 3 (None case): SER overloads accept CommandFlags.None.
    /// Verifies that GuardClauses does not throw for CommandFlags.None.
    /// Note: PreferMaster has the same value (0) as None, so it's also accepted.
    /// </summary>
    /// <remarks>
    /// Tag: Feature: remove-command-flags-from-glide-interfaces, Property 3: SER overloads reject non-None CommandFlags
    /// Validates: Requirements 4.2, 5.2
    /// </remarks>
    [Fact]
    public void GuardClauses_DoesNotThrowForNoneCommandFlags()
    {
        // CommandFlags.None = 0
        var exceptionNone = Record.Exception(() =>
            Internals.GuardClauses.ThrowIfCommandFlags(CommandFlags.None));
        Assert.Null(exceptionNone);

        // CommandFlags.PreferMaster = 0 (same as None)
        var exceptionPreferMaster = Record.Exception(() =>
            Internals.GuardClauses.ThrowIfCommandFlags(CommandFlags.PreferMaster));
        Assert.Null(exceptionPreferMaster);
    }

    #endregion
}
