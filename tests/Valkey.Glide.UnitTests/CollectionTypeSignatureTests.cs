// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection;

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Verifies that collection parameters on command interfaces use <c>IEnumerable&lt;T&gt;</c>
/// instead of concrete array or dictionary types.
/// </summary>
public class CollectionTypeSignatureTests
{
    #region Data

    /// <summary>
    /// Static dataset of (interfaceType, methodName, parameterName, expectedGenericType) tuples.
    /// Each entry identifies a method whose collection parameter must be <c>IEnumerable&lt;T&gt;</c>
    /// for the given <c>T</c>.
    /// </summary>
    public static TheoryData<Type, string, string, Type> CollectionParameterData => new()
    {
        // IGenericBaseCommands — ValkeyKey
        { typeof(IGenericBaseCommands), "KeyDeleteAsync", "keys", typeof(ValkeyKey) },
        { typeof(IGenericBaseCommands), "KeyUnlinkAsync", "keys", typeof(ValkeyKey) },
        { typeof(IGenericBaseCommands), "KeyExistsAsync", "keys", typeof(ValkeyKey) },
        { typeof(IGenericBaseCommands), "KeyTouchAsync", "keys", typeof(ValkeyKey) },
        // IGenericBaseCommands — ValkeyValue (nullable IEnumerable)
        { typeof(IGenericBaseCommands), "SortAsync", "get", typeof(ValkeyValue) },

        // IGenericCommands — GlideString
        { typeof(IGenericCommands), "CustomCommand", "args", typeof(GlideString) },

        // IListCommands — ValkeyKey
        { typeof(IListCommands), "ListLeftPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListRightPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListBlockingLeftPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListBlockingRightPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListBlockingPopAsync", "keys", typeof(ValkeyKey) },
        // IListCommands — ValkeyValue
        { typeof(IListCommands), "ListLeftPushAsync", "values", typeof(ValkeyValue) },
        { typeof(IListCommands), "ListRightPushAsync", "values", typeof(ValkeyValue) },

        // ISetCommands — ValkeyValue
        { typeof(ISetCommands), "SetAddAsync", "values", typeof(ValkeyValue) },
        { typeof(ISetCommands), "SetRemoveAsync", "values", typeof(ValkeyValue) },
        { typeof(ISetCommands), "SetContainsAsync", "values", typeof(ValkeyValue) },
        // ISetCommands — ValkeyKey
        { typeof(ISetCommands), "SetIntersectionLengthAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetUnionAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetIntersectAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetDifferenceAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetUnionStoreAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetIntersectStoreAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetDifferenceStoreAsync", "keys", typeof(ValkeyKey) },

        // ISortedSetCommands — SortedSetEntry
        { typeof(ISortedSetCommands), "SortedSetAddAsync", "values", typeof(SortedSetEntry) },
        // ISortedSetCommands — ValkeyValue
        { typeof(ISortedSetCommands), "SortedSetRemoveAsync", "members", typeof(ValkeyValue) },
        // ISortedSetCommands — ValkeyKey
        { typeof(ISortedSetCommands), "SortedSetBlockingPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISortedSetCommands), "SortedSetCombineAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISortedSetCommands), "SortedSetCombineWithScoresAsync", "keys", typeof(ValkeyKey) },

        // IGeospatialCommands — GeoEntry
        { typeof(IGeospatialCommands), "GeoAddAsync", "values", typeof(GeoEntry) },
        // IGeospatialCommands — ValkeyValue
        { typeof(IGeospatialCommands), "GeoHashAsync", "members", typeof(ValkeyValue) },
        { typeof(IGeospatialCommands), "GeoPositionAsync", "members", typeof(ValkeyValue) },

        // IBitmapCommands — ValkeyKey
        { typeof(IBitmapCommands), "StringBitOperationAsync", "keys", typeof(ValkeyKey) },
        // IBitmapCommands — BitFieldOptions.IBitFieldSubCommand / BitFieldOptions.IBitFieldReadOnlySubCommand
        { typeof(IBitmapCommands), "StringBitFieldAsync", "subCommands", typeof(BitFieldOptions.IBitFieldSubCommand) },
        { typeof(IBitmapCommands), "StringBitFieldReadOnlyAsync", "subCommands", typeof(BitFieldOptions.IBitFieldReadOnlySubCommand) },

        // IHyperLogLogCommands — ValkeyValue
        { typeof(IHyperLogLogCommands), "HyperLogLogAddAsync", "elements", typeof(ValkeyValue) },
        // IHyperLogLogCommands — ValkeyKey
        { typeof(IHyperLogLogCommands), "HyperLogLogLengthAsync", "keys", typeof(ValkeyKey) },
        { typeof(IHyperLogLogCommands), "HyperLogLogMergeAsync", "sourceKeys", typeof(ValkeyKey) },

        // IHashCommands — ValkeyValue
        { typeof(IHashCommands), "HashGetAsync", "hashFields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashDeleteAsync", "hashFields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashPersistAsync", "fields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashExpireAsync", "fields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashPExpireAsync", "fields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashExpireAtAsync", "fields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashGetExAsync", "fields", typeof(ValkeyValue) },
        // IHashCommands — HashEntry
        { typeof(IHashCommands), "HashSetAsync", "hashFields", typeof(HashEntry) },

        // IStringCommands — ValkeyKey
        { typeof(IStringCommands), "StringGetAsync", "keys", typeof(ValkeyKey) },

        // IStreamCommands — StreamPosition
        { typeof(IStreamCommands), "StreamReadAsync", "streamPositions", typeof(StreamPosition) },
        { typeof(IStreamCommands), "StreamReadGroupAsync", "streamPositions", typeof(StreamPosition) },
        // IStreamCommands — ValkeyValue
        { typeof(IStreamCommands), "StreamAcknowledgeAsync", "messageIds", typeof(ValkeyValue) },
        { typeof(IStreamCommands), "StreamClaimAsync", "messageIds", typeof(ValkeyValue) },
        { typeof(IStreamCommands), "StreamClaimIdsOnlyAsync", "messageIds", typeof(ValkeyValue) },
        { typeof(IStreamCommands), "StreamDeleteAsync", "messageIds", typeof(ValkeyValue) },
    };

    /// <summary>
    /// Dataset for methods whose collection parameter is
    /// <c>IDictionary&lt;ValkeyValue, ValkeyValue&gt;</c>
    /// (formerly <c>Dictionary&lt;ValkeyValue, ValkeyValue&gt;</c>).
    /// </summary>
    public static TheoryData<Type, string, string> DictionaryValkeyValueParameterData => new()
    {
        { typeof(IHashCommands), "HashSetExAsync", "fieldValueMap" },
    };

    /// <summary>
    /// Dataset for methods whose collection parameter is
    /// <c>IDictionary&lt;ValkeyKey, ValkeyValue&gt;</c>
    /// (formerly <c>KeyValuePair&lt;ValkeyKey, ValkeyValue&gt;[]</c>).
    /// </summary>
    public static TheoryData<Type, string, string> DictionaryValkeyKeyParameterData => new()
    {
        { typeof(IStringCommands), "StringSetAsync", "values" },
    };

    #endregion
    #region Tests

    /// <summary>
    /// Validates: Requirements 1.1–1.48
    /// For each method identified by the bug condition, the collection parameter type
    /// must be <c>IEnumerable&lt;T&gt;</c> (or nullable <c>IEnumerable&lt;T&gt;?</c>).
    /// </summary>
    [Theory]
    [MemberData(nameof(CollectionParameterData))]
    public void CollectionParameter_ShouldBeIEnumerable(
        Type interfaceType, string methodName, string parameterName, Type expectedElementType)
    {
        // Find all overloads with the named parameter
        MethodInfo[] methods = interfaceType.GetMethods()
            .Where(m => m.Name == methodName &&
                        m.GetParameters().Any(p => p.Name == parameterName))
            .ToArray();

        Assert.True(methods.Length > 0,
            $"No method '{methodName}' with parameter '{parameterName}' found on {interfaceType.Name}");

        foreach (MethodInfo method in methods)
        {
            ParameterInfo param = method.GetParameters().First(p => p.Name == parameterName);
            Type paramType = param.ParameterType;

            // Handle nullable IEnumerable<T>? (e.g., SortAsync get parameter)
            Type underlying = Nullable.GetUnderlyingType(paramType) ?? paramType;

            Assert.True(
                underlying.IsGenericType &&
                underlying.GetGenericTypeDefinition() == typeof(IEnumerable<>),
                $"{interfaceType.Name}.{methodName} parameter '{parameterName}' " +
                $"should be IEnumerable<{expectedElementType.Name}> but is {paramType.Name}");

            Type actualElement = underlying.GetGenericArguments()[0];
            Assert.Equal(expectedElementType, actualElement);
        }
    }

    /// <summary>
    /// Validates: Requirement 1.45 (HashSetExAsync)
    /// The dictionary parameter must be <c>IDictionary&lt;ValkeyValue, ValkeyValue&gt;</c>.
    /// </summary>
    [Theory]
    [MemberData(nameof(DictionaryValkeyValueParameterData))]
    public void DictionaryParameter_ShouldBeIDictionaryValkeyValue(
        Type interfaceType, string methodName, string parameterName)
    {
        MethodInfo[] methods = interfaceType.GetMethods()
            .Where(m => m.Name == methodName &&
                        m.GetParameters().Any(p => p.Name == parameterName))
            .ToArray();

        Assert.True(methods.Length > 0,
            $"No method '{methodName}' with parameter '{parameterName}' found on {interfaceType.Name}");

        foreach (MethodInfo method in methods)
        {
            ParameterInfo param = method.GetParameters().First(p => p.Name == parameterName);
            Type paramType = param.ParameterType;

            Type expectedType = typeof(IDictionary<ValkeyValue, ValkeyValue>);

            Assert.True(
                paramType.IsGenericType &&
                paramType.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                paramType.GetGenericArguments()[0] == typeof(ValkeyValue) &&
                paramType.GetGenericArguments()[1] == typeof(ValkeyValue),
                $"{interfaceType.Name}.{methodName} parameter '{parameterName}' " +
                $"should be {expectedType.Name} but is {paramType.Name}");
        }
    }

    /// <summary>
    /// Validates: Requirement 1.46 (StringSetAsync)
    /// The dictionary parameter must be <c>IDictionary&lt;ValkeyKey, ValkeyValue&gt;</c>.
    /// </summary>
    [Theory]
    [MemberData(nameof(DictionaryValkeyKeyParameterData))]
    public void DictionaryParameter_ShouldBeIDictionaryValkeyKey(
        Type interfaceType, string methodName, string parameterName)
    {
        MethodInfo[] methods = interfaceType.GetMethods()
            .Where(m => m.Name == methodName &&
                        m.GetParameters().Any(p => p.Name == parameterName))
            .ToArray();

        Assert.True(methods.Length > 0,
            $"No method '{methodName}' with parameter '{parameterName}' found on {interfaceType.Name}");

        foreach (MethodInfo method in methods)
        {
            ParameterInfo param = method.GetParameters().First(p => p.Name == parameterName);
            Type paramType = param.ParameterType;

            Type expectedType = typeof(IDictionary<ValkeyKey, ValkeyValue>);

            Assert.True(
                paramType.IsGenericType &&
                paramType.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                paramType.GetGenericArguments()[0] == typeof(ValkeyKey) &&
                paramType.GetGenericArguments()[1] == typeof(ValkeyValue),
                $"{interfaceType.Name}.{methodName} parameter '{parameterName}' " +
                $"should be {expectedType.Name} but is {paramType.Name}");
        }
    }

    #endregion
}
