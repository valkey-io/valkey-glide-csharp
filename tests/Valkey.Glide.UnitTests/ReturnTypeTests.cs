// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection;

using Valkey.Glide.Commands;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Tests verifying that GLIDE-native interface methods have the correct return types
/// after the bool-to-void return type changes, and that SER-compatible overloads
/// preserve their <see cref="Task{Boolean}"/> return types.
/// </summary>
public class ReturnTypeTests
{
    #region Data

    /// <summary>
    /// GLIDE-native interface methods that should return <see cref="Task"/> (void).
    /// Each entry: (interface type, method name, parameter types).
    /// </summary>
    public static TheoryData<Type, string, Type[]> GlideNativeVoidMethods => new()
    {
        // KeyRenameAsync(ValkeyKey, ValkeyKey) -> Task
        { typeof(IGenericBaseCommands), nameof(IGenericBaseCommands.KeyRenameAsync), new[] { typeof(ValkeyKey), typeof(ValkeyKey) } },

        // StringSetAsync(ValkeyKey, ValkeyValue) -> Task
        { typeof(IStringCommands), nameof(IStringCommands.StringSetAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue) } },

        // StringSetAsync(IEnumerable<KVP>) -> Task (unconditional multi-key)
        { typeof(IStringCommands), nameof(IStringCommands.StringSetAsync), new[] { typeof(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>>) } },

        // StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?) -> Task
        { typeof(IStreamCommands), nameof(IStreamCommands.StreamCreateConsumerGroupAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(ValkeyValue?) } },

        // StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?) -> Task
        { typeof(IStreamCommands), nameof(IStreamCommands.StreamCreateConsumerGroupAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(ValkeyValue?), typeof(bool), typeof(long?) } },

        // StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?) -> Task
        { typeof(IStreamCommands), nameof(IStreamCommands.StreamConsumerGroupSetPositionAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(ValkeyValue), typeof(long?) } },
    };

    /// <summary>
    /// GLIDE-native interface methods that should return <see cref="Task{Boolean}"/>.
    /// These are conditional operations where the bool carries meaningful information.
    /// </summary>
    public static TheoryData<Type, string, Type[]> GlideNativeBoolMethods => new()
    {
        // StringSetAsync(ValkeyKey, ValkeyValue, When) -> Task<bool> (conditional single-key)
        { typeof(IStringCommands), nameof(IStringCommands.StringSetAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(When) } },

        // StringSetAsync(IEnumerable<KVP>, When) -> Task<bool> (conditional multi-key)
        { typeof(IStringCommands), nameof(IStringCommands.StringSetAsync), new[] { typeof(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>>), typeof(When) } },
    };

    /// <summary>
    /// SER-compatible interface methods that must continue to return <see cref="Task{Boolean}"/>.
    /// Each entry: (interface type, method name, parameter types).
    /// </summary>
    public static TheoryData<Type, string, Type[]> SerCompatibleBoolMethods => new()
    {
        // KeyRenameAsync(ValkeyKey, ValkeyKey, CommandFlags) -> Task<bool>
        { typeof(IDatabaseAsync), nameof(IDatabaseAsync.KeyRenameAsync), new[] { typeof(ValkeyKey), typeof(ValkeyKey), typeof(CommandFlags) } },

        // StringSetAsync(ValkeyKey, ValkeyValue, CommandFlags) -> Task<bool>
        { typeof(IDatabaseAsync), nameof(IDatabaseAsync.StringSetAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },

        // StringSetAsync(IEnumerable<KVP>, When, CommandFlags) -> Task<bool>
        { typeof(IDatabaseAsync), nameof(IDatabaseAsync.StringSetAsync), new[] { typeof(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>>), typeof(When), typeof(CommandFlags) } },

        // StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, CommandFlags) -> Task<bool>
        { typeof(IDatabaseAsync), nameof(IDatabaseAsync.StreamCreateConsumerGroupAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(ValkeyValue?), typeof(CommandFlags) } },

        // StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?, CommandFlags) -> Task<bool>
        { typeof(IDatabaseAsync), nameof(IDatabaseAsync.StreamCreateConsumerGroupAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(ValkeyValue?), typeof(bool), typeof(long?), typeof(CommandFlags) } },

        // StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?, CommandFlags) -> Task<bool>
        { typeof(IDatabaseAsync), nameof(IDatabaseAsync.StreamConsumerGroupSetPositionAsync), new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(ValkeyValue), typeof(long?), typeof(CommandFlags) } },
    };

    #endregion

    #region GLIDE-Native Return Type Tests

    [Theory]
    [MemberData(nameof(GlideNativeVoidMethods))]
    public void GlideNativeMethod_UnconditionalOperation_ReturnsTask(Type interfaceType, string methodName, Type[] parameterTypes)
    {
        MethodInfo? method = interfaceType.GetMethod(methodName, parameterTypes);

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method.ReturnType);
    }

    [Theory]
    [MemberData(nameof(GlideNativeBoolMethods))]
    public void GlideNativeMethod_ConditionalOperation_ReturnsTaskBool(Type interfaceType, string methodName, Type[] parameterTypes)
    {
        MethodInfo? method = interfaceType.GetMethod(methodName, parameterTypes);

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<bool>), method.ReturnType);
    }

    #endregion

    #region SER-Compatible Return Type Tests

    [Theory]
    [MemberData(nameof(SerCompatibleBoolMethods))]
    public void SerCompatibleMethod_AllFourMethodGroups_ReturnsTaskBool(Type interfaceType, string methodName, Type[] parameterTypes)
    {
        MethodInfo? method = interfaceType.GetMethod(methodName, parameterTypes);

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<bool>), method.ReturnType);
    }

    #endregion

    #region StringSetAsync New Overload Tests

    [Fact]
    public void StringSetAsync_UnconditionalMultiKey_ExistsOnInterface()
    {
        // Verify the new unconditional multi-key overload exists and returns Task
        MethodInfo? method = typeof(IStringCommands).GetMethod(
            nameof(IStringCommands.StringSetAsync),
            [typeof(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>>)]);

        Assert.NotNull(method);
        Assert.Equal(typeof(Task), method.ReturnType);
    }

    [Fact]
    public void StringSetAsync_ConditionalSingleKey_ExistsOnInterface()
    {
        // Verify the new conditional single-key overload exists and returns Task<bool>
        MethodInfo? method = typeof(IStringCommands).GetMethod(
            nameof(IStringCommands.StringSetAsync),
            [typeof(ValkeyKey), typeof(ValkeyValue), typeof(When)]);

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<bool>), method.ReturnType);
    }

    [Fact]
    public void StringSetAsync_ConditionalMultiKey_WhenParameterIsRequired()
    {
        // Verify the When parameter on the multi-key conditional overload has no default value
        MethodInfo? method = typeof(IStringCommands).GetMethod(
            nameof(IStringCommands.StringSetAsync),
            [typeof(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>>), typeof(When)]);

        Assert.NotNull(method);
        ParameterInfo whenParam = method.GetParameters()[1];
        Assert.Equal("when", whenParam.Name);
        Assert.False(whenParam.HasDefaultValue, "The 'when' parameter should not have a default value");
    }

    #endregion

    #region Unchanged Method Preservation Tests

    [Fact]
    public void KeyRenameNXAsync_StillReturnsBool()
    {
        // KeyRenameNXAsync should NOT be changed — it has meaningful bool semantics
        MethodInfo? method = typeof(IGenericBaseCommands).GetMethod(
            nameof(IGenericBaseCommands.KeyRenameNXAsync),
            [typeof(ValkeyKey), typeof(ValkeyKey)]);

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<bool>), method.ReturnType);
    }

    [Fact]
    public void StreamDeleteConsumerGroupAsync_StillReturnsBool()
    {
        // StreamDeleteConsumerGroupAsync should NOT be changed — XGROUP DESTROY returns true/false
        MethodInfo? method = typeof(IStreamCommands).GetMethod(
            nameof(IStreamCommands.StreamDeleteConsumerGroupAsync),
            [typeof(ValkeyKey), typeof(ValkeyValue)]);

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<bool>), method.ReturnType);
    }

    [Fact]
    public void StreamCreateConsumerAsync_StillReturnsBool()
    {
        // StreamCreateConsumerAsync should NOT be changed — XGROUP CREATECONSUMER returns true/false
        MethodInfo? method = typeof(IStreamCommands).GetMethod(
            nameof(IStreamCommands.StreamCreateConsumerAsync),
            [typeof(ValkeyKey), typeof(ValkeyValue), typeof(ValkeyValue)]);

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<bool>), method.ReturnType);
    }

    #endregion
}
