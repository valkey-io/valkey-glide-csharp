// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection;

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Preservation property tests that verify existing behavior is maintained during the
/// IEnumerable generalization. These tests PASS on unfixed code, establishing a baseline.
/// After the fix, they must continue to pass — confirming no regressions.
/// </summary>
/// <remarks>
/// <b>Validates: Requirements 3.1–3.8</b>
/// </remarks>
public class CollectionTypePreservationTests
{
    #region Data

    /// <summary>
    /// Single-value overloads that must continue to exist alongside multi-value overloads.
    /// Format: (interfaceType, methodName, singleValueParamTypes) — the exact parameter
    /// type list that identifies the single-value overload.
    /// </summary>
    public static TheoryData<Type, string, Type[]> SingleValueOverloadData => new()
    {
        // IGenericBaseCommands single-key overloads
        { typeof(IGenericBaseCommands), "KeyDeleteAsync", new[] { typeof(ValkeyKey), typeof(CommandFlags) } },
        { typeof(IGenericBaseCommands), "KeyUnlinkAsync", new[] { typeof(ValkeyKey), typeof(CommandFlags) } },
        { typeof(IGenericBaseCommands), "KeyExistsAsync", new[] { typeof(ValkeyKey), typeof(CommandFlags) } },
        { typeof(IGenericBaseCommands), "KeyTouchAsync", new[] { typeof(ValkeyKey), typeof(CommandFlags) } },

        // ISetCommands single-value overloads
        { typeof(ISetCommands), "SetAddAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },
        { typeof(ISetCommands), "SetRemoveAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },
        { typeof(ISetCommands), "SetContainsAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },

        // IListCommands single-value overloads
        { typeof(IListCommands), "ListLeftPushAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(When), typeof(CommandFlags) } },
        { typeof(IListCommands), "ListRightPushAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(When), typeof(CommandFlags) } },

        // ISortedSetCommands single-value overloads
        { typeof(ISortedSetCommands), "SortedSetAddAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(double), typeof(SortedSetWhen), typeof(CommandFlags) } },
        { typeof(ISortedSetCommands), "SortedSetRemoveAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },

        // IGeospatialCommands single-value overloads
        { typeof(IGeospatialCommands), "GeoAddAsync", new[] { typeof(ValkeyKey), typeof(GeoEntry), typeof(CommandFlags) } },
        { typeof(IGeospatialCommands), "GeoHashAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },
        { typeof(IGeospatialCommands), "GeoPositionAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },

        // IHyperLogLogCommands single-value overloads
        { typeof(IHyperLogLogCommands), "HyperLogLogAddAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },
        { typeof(IHyperLogLogCommands), "HyperLogLogLengthAsync", new[] { typeof(ValkeyKey), typeof(CommandFlags) } },

        // IHashCommands single-value overloads
        { typeof(IHashCommands), "HashGetAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },
        { typeof(IHashCommands), "HashDeleteAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },

        // IStringCommands single-value overloads
        { typeof(IStringCommands), "StringSetAsync", new[] { typeof(ValkeyKey), typeof(ValkeyValue), typeof(CommandFlags) } },
        { typeof(IStringCommands), "StringGetAsync", new[] { typeof(ValkeyKey), typeof(CommandFlags) } },

        // IBitmapCommands single-value overloads (two-key form)
        { typeof(IBitmapCommands), "StringBitOperationAsync", new[] { typeof(Bitwise), typeof(ValkeyKey), typeof(ValkeyKey), typeof(ValkeyKey), typeof(CommandFlags) } },

        // IHyperLogLogCommands two-key merge overload
        { typeof(IHyperLogLogCommands), "HyperLogLogMergeAsync", new[] { typeof(ValkeyKey), typeof(ValkeyKey), typeof(ValkeyKey), typeof(CommandFlags) } },
    };

    /// <summary>
    /// Return types for all 48 multi-value methods that are being generalized.
    /// Format: (interfaceType, methodName, collectionParamName, expectedReturnType).
    /// </summary>
    public static TheoryData<Type, string, string, Type> ReturnTypeData => new()
    {
        // IGenericBaseCommands
        { typeof(IGenericBaseCommands), "KeyDeleteAsync", "keys", typeof(Task<long>) },
        { typeof(IGenericBaseCommands), "KeyUnlinkAsync", "keys", typeof(Task<long>) },
        { typeof(IGenericBaseCommands), "KeyExistsAsync", "keys", typeof(Task<long>) },
        { typeof(IGenericBaseCommands), "KeyTouchAsync", "keys", typeof(Task<long>) },
        { typeof(IGenericBaseCommands), "SortAsync", "get", typeof(Task<ValkeyValue[]>) },

        // IGenericCommands
        { typeof(IGenericCommands), "CustomCommand", "args", typeof(Task<object?>) },

        // IListCommands
        { typeof(IListCommands), "ListLeftPopAsync", "keys", typeof(Task<ListPopResult>) },
        { typeof(IListCommands), "ListLeftPushAsync", "values", typeof(Task<long>) },
        { typeof(IListCommands), "ListRightPushAsync", "values", typeof(Task<long>) },
        { typeof(IListCommands), "ListRightPopAsync", "keys", typeof(Task<ListPopResult>) },
        { typeof(IListCommands), "ListBlockingLeftPopAsync", "keys", typeof(Task<ValkeyValue[]?>) },
        { typeof(IListCommands), "ListBlockingRightPopAsync", "keys", typeof(Task<ValkeyValue[]?>) },
        { typeof(IListCommands), "ListBlockingPopAsync", "keys", typeof(Task<ListPopResult>) },

        // ISetCommands
        { typeof(ISetCommands), "SetAddAsync", "values", typeof(Task<long>) },
        { typeof(ISetCommands), "SetRemoveAsync", "values", typeof(Task<long>) },
        { typeof(ISetCommands), "SetIntersectionLengthAsync", "keys", typeof(Task<long>) },
        { typeof(ISetCommands), "SetUnionAsync", "keys", typeof(Task<ValkeyValue[]>) },
        { typeof(ISetCommands), "SetIntersectAsync", "keys", typeof(Task<ValkeyValue[]>) },
        { typeof(ISetCommands), "SetDifferenceAsync", "keys", typeof(Task<ValkeyValue[]>) },
        { typeof(ISetCommands), "SetUnionStoreAsync", "keys", typeof(Task<long>) },
        { typeof(ISetCommands), "SetIntersectStoreAsync", "keys", typeof(Task<long>) },
        { typeof(ISetCommands), "SetDifferenceStoreAsync", "keys", typeof(Task<long>) },
        { typeof(ISetCommands), "SetContainsAsync", "values", typeof(Task<bool[]>) },

        // ISortedSetCommands
        { typeof(ISortedSetCommands), "SortedSetAddAsync", "values", typeof(Task<long>) },
        { typeof(ISortedSetCommands), "SortedSetRemoveAsync", "members", typeof(Task<long>) },
        { typeof(ISortedSetCommands), "SortedSetBlockingPopAsync", "keys", typeof(Task<SortedSetPopResult>) },
        { typeof(ISortedSetCommands), "SortedSetCombineAsync", "keys", typeof(Task<ValkeyValue[]>) },
        { typeof(ISortedSetCommands), "SortedSetCombineWithScoresAsync", "keys", typeof(Task<SortedSetEntry[]>) },

        // IGeospatialCommands
        { typeof(IGeospatialCommands), "GeoAddAsync", "values", typeof(Task<long>) },
        { typeof(IGeospatialCommands), "GeoHashAsync", "members", typeof(Task<string?[]>) },
        { typeof(IGeospatialCommands), "GeoPositionAsync", "members", typeof(Task<GeoPosition?[]>) },

        // IBitmapCommands
        { typeof(IBitmapCommands), "StringBitOperationAsync", "keys", typeof(Task<long>) },
        { typeof(IBitmapCommands), "StringBitFieldAsync", "subCommands", typeof(Task<long[]>) },
        { typeof(IBitmapCommands), "StringBitFieldReadOnlyAsync", "subCommands", typeof(Task<long[]>) },

        // IHyperLogLogCommands
        { typeof(IHyperLogLogCommands), "HyperLogLogAddAsync", "elements", typeof(Task<bool>) },
        { typeof(IHyperLogLogCommands), "HyperLogLogLengthAsync", "keys", typeof(Task<long>) },
        { typeof(IHyperLogLogCommands), "HyperLogLogMergeAsync", "sourceKeys", typeof(Task) },

        // IHashCommands
        { typeof(IHashCommands), "HashGetAsync", "hashFields", typeof(Task<ValkeyValue[]>) },
        { typeof(IHashCommands), "HashSetAsync", "hashFields", typeof(Task) },
        { typeof(IHashCommands), "HashDeleteAsync", "hashFields", typeof(Task<long>) },
        { typeof(IHashCommands), "HashPersistAsync", "fields", typeof(Task<long[]>) },
        { typeof(IHashCommands), "HashExpireAsync", "fields", typeof(Task<long[]>) },
        { typeof(IHashCommands), "HashPExpireAsync", "fields", typeof(Task<long[]>) },
        { typeof(IHashCommands), "HashExpireAtAsync", "fields", typeof(Task<long[]>) },
        { typeof(IHashCommands), "HashGetExAsync", "fields", typeof(Task<ValkeyValue[]?>) },
        { typeof(IHashCommands), "HashSetExAsync", "fieldValueMap", typeof(Task<long>) },

        // IStringCommands
        { typeof(IStringCommands), "StringSetAsync", "values", typeof(Task<bool>) },
        { typeof(IStringCommands), "StringGetAsync", "keys", typeof(Task<ValkeyValue[]>) },
    };

    /// <summary>
    /// Pub/sub methods that use IEnumerable&lt;string&gt; and must remain unchanged.
    /// Format: (interfaceType, methodName, parameterName).
    /// </summary>
    public static TheoryData<Type, string, string> PubSubIEnumerableData => new()
    {
        // IPubSubCommands
        { typeof(IPubSubCommands), "SubscribeAsync", "channels" },
        { typeof(IPubSubCommands), "SubscribeLazyAsync", "channels" },
        { typeof(IPubSubCommands), "PSubscribeAsync", "patterns" },
        { typeof(IPubSubCommands), "PSubscribeLazyAsync", "patterns" },
        { typeof(IPubSubCommands), "UnsubscribeAsync", "channels" },
        { typeof(IPubSubCommands), "UnsubscribeLazyAsync", "channels" },
        { typeof(IPubSubCommands), "PUnsubscribeAsync", "patterns" },
        { typeof(IPubSubCommands), "PUnsubscribeLazyAsync", "patterns" },
        { typeof(IPubSubCommands), "PubSubNumSubAsync", "channels" },

        // IPubSubClusterCommands
        { typeof(IPubSubClusterCommands), "SSubscribeAsync", "shardedChannels" },
        { typeof(IPubSubClusterCommands), "SSubscribeLazyAsync", "shardedChannels" },
        { typeof(IPubSubClusterCommands), "SUnsubscribeAsync", "shardedChannels" },
        { typeof(IPubSubClusterCommands), "SUnsubscribeLazyAsync", "shardedChannels" },
        { typeof(IPubSubClusterCommands), "PubSubShardNumSubAsync", "shardedChannels" },
    };

    /// <summary>
    /// Expected public method counts for each of the 10 command interfaces.
    /// These counts are observed on unfixed code and must remain stable after the fix.
    /// </summary>
    public static TheoryData<Type, int> MethodCountData => new()
    {
        { typeof(IGenericBaseCommands), 31 },
        { typeof(IGenericCommands), 4 },
        { typeof(IListCommands), 28 },
        { typeof(ISetCommands), 27 },
        { typeof(ISortedSetCommands), 41 },
        { typeof(IGeospatialCommands), 14 },
        { typeof(IBitmapCommands), 8 },
        { typeof(IHyperLogLogCommands), 6 },
        { typeof(IHashCommands), 30 },
        { typeof(IStringCommands), 19 },
    };

    #endregion

    #region Tests

    /// <summary>
    /// Verifies that single-value overloads exist with their original parameter types.
    /// These must remain alongside the multi-value overloads after generalization.
    /// </summary>
    [Theory]
    [MemberData(nameof(SingleValueOverloadData))]
    public void SingleValueOverload_Exists_WithOriginalParameterTypes(
        Type interfaceType, string methodName, Type[] expectedParamTypes)
    {
        var method = interfaceType.GetMethod(
            methodName,
            BindingFlags.Public | BindingFlags.Instance,
            binder: null,
            types: expectedParamTypes,
            modifiers: null);

        Assert.NotNull(method);
    }

    /// <summary>
    /// Verifies that return types of all 48 multi-value methods remain unchanged.
    /// </summary>
    [Theory]
    [MemberData(nameof(ReturnTypeData))]
    public void MultiValueMethod_ReturnType_IsPreserved(
        Type interfaceType, string methodName, string parameterName, Type expectedReturnType)
    {
        var method = FindMethodWithParameter(interfaceType, methodName, parameterName);
        Assert.NotNull(method);

        Assert.Equal(expectedReturnType, method!.ReturnType);
    }

    /// <summary>
    /// Verifies that pub/sub interfaces retain IEnumerable&lt;string&gt; parameters
    /// and are not accidentally changed during the generalization.
    /// </summary>
    [Theory]
    [MemberData(nameof(PubSubIEnumerableData))]
    public void PubSubMethod_Parameter_UsesIEnumerableString(
        Type interfaceType, string methodName, string parameterName)
    {
        var method = FindMethodWithParameter(interfaceType, methodName, parameterName);
        Assert.NotNull(method);

        var parameter = method!.GetParameters().First(p => p.Name == parameterName);
        Assert.Equal(typeof(IEnumerable<string>), parameter.ParameterType);
    }

    /// <summary>
    /// Verifies that the total number of public methods on each command interface
    /// has not changed — no methods accidentally added or removed.
    /// </summary>
    [Theory]
    [MemberData(nameof(MethodCountData))]
    public void CommandInterface_MethodCount_IsPreserved(Type interfaceType, int expectedCount)
    {
        var actualCount = interfaceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Length;

        Assert.Equal(expectedCount, actualCount);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Finds a method overload on the given interface that has a parameter with the specified name.
    /// </summary>
    private static MethodInfo? FindMethodWithParameter(Type interfaceType, string methodName, string parameterName)
    {
        return interfaceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name == methodName)
            .FirstOrDefault(m => m.GetParameters().Any(p => p.Name == parameterName));
    }

    #endregion
}
