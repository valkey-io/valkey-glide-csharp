// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection;

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Bug condition exploration tests that verify collection parameters use IEnumerable&lt;T&gt;
/// instead of concrete array types. On unfixed code, these tests FAIL — confirming the bug exists.
/// </summary>
/// <remarks>
/// <b>Validates: Requirements 1.1–1.48, 2.1–2.48</b>
/// </remarks>
public class CollectionTypeSignatureTests
{
    #region Data

    /// <summary>
    /// Dataset of (interfaceType, methodName, parameterName, expectedGenericType) tuples
    /// covering all 48 methods from the design document.
    /// </summary>
    public static TheoryData<Type, string, string, Type> CollectionParameterData => new()
    {
        // ValkeyKey[] → IEnumerable<ValkeyKey> (23 methods)
        { typeof(IGenericBaseCommands), "KeyDeleteAsync", "keys", typeof(ValkeyKey) },
        { typeof(IGenericBaseCommands), "KeyUnlinkAsync", "keys", typeof(ValkeyKey) },
        { typeof(IGenericBaseCommands), "KeyExistsAsync", "keys", typeof(ValkeyKey) },
        { typeof(IGenericBaseCommands), "KeyTouchAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListLeftPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListRightPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListBlockingLeftPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListBlockingRightPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(IListCommands), "ListBlockingPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetIntersectionLengthAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetUnionAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetIntersectAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetDifferenceAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetUnionStoreAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetIntersectStoreAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISetCommands), "SetDifferenceStoreAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISortedSetCommands), "SortedSetBlockingPopAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISortedSetCommands), "SortedSetCombineAsync", "keys", typeof(ValkeyKey) },
        { typeof(ISortedSetCommands), "SortedSetCombineWithScoresAsync", "keys", typeof(ValkeyKey) },
        { typeof(IBitmapCommands), "StringBitOperationAsync", "keys", typeof(ValkeyKey) },
        { typeof(IHyperLogLogCommands), "HyperLogLogLengthAsync", "keys", typeof(ValkeyKey) },
        { typeof(IHyperLogLogCommands), "HyperLogLogMergeAsync", "sourceKeys", typeof(ValkeyKey) },
        { typeof(IStringCommands), "StringGetAsync", "keys", typeof(ValkeyKey) },

        // ValkeyValue[] → IEnumerable<ValkeyValue> (16 methods)
        { typeof(IListCommands), "ListLeftPushAsync", "values", typeof(ValkeyValue) },
        { typeof(IListCommands), "ListRightPushAsync", "values", typeof(ValkeyValue) },
        { typeof(ISetCommands), "SetAddAsync", "values", typeof(ValkeyValue) },
        { typeof(ISetCommands), "SetRemoveAsync", "values", typeof(ValkeyValue) },
        { typeof(ISetCommands), "SetContainsAsync", "values", typeof(ValkeyValue) },
        { typeof(ISortedSetCommands), "SortedSetRemoveAsync", "members", typeof(ValkeyValue) },
        { typeof(IGeospatialCommands), "GeoHashAsync", "members", typeof(ValkeyValue) },
        { typeof(IGeospatialCommands), "GeoPositionAsync", "members", typeof(ValkeyValue) },
        { typeof(IHyperLogLogCommands), "HyperLogLogAddAsync", "elements", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashGetAsync", "hashFields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashDeleteAsync", "hashFields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashPersistAsync", "fields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashExpireAsync", "fields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashPExpireAsync", "fields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashExpireAtAsync", "fields", typeof(ValkeyValue) },
        { typeof(IHashCommands), "HashGetExAsync", "fields", typeof(ValkeyValue) },

        // SortedSetEntry[] → IEnumerable<SortedSetEntry>
        { typeof(ISortedSetCommands), "SortedSetAddAsync", "values", typeof(SortedSetEntry) },

        // GeoEntry[] → IEnumerable<GeoEntry>
        { typeof(IGeospatialCommands), "GeoAddAsync", "values", typeof(GeoEntry) },

        // HashEntry[] → IEnumerable<HashEntry>
        { typeof(IHashCommands), "HashSetAsync", "hashFields", typeof(HashEntry) },

        // IBitFieldSubCommand[] → IEnumerable<IBitFieldSubCommand>
        { typeof(IBitmapCommands), "StringBitFieldAsync", "subCommands", typeof(BitFieldOptions.IBitFieldSubCommand) },

        // IBitFieldReadOnlySubCommand[] → IEnumerable<IBitFieldReadOnlySubCommand>
        { typeof(IBitmapCommands), "StringBitFieldReadOnlyAsync", "subCommands", typeof(BitFieldOptions.IBitFieldReadOnlySubCommand) },

        // KeyValuePair<ValkeyKey,ValkeyValue>[] → IEnumerable<KeyValuePair<ValkeyKey,ValkeyValue>>
        { typeof(IStringCommands), "StringSetAsync", "values", typeof(KeyValuePair<ValkeyKey, ValkeyValue>) },

        // GlideString[] → IEnumerable<GlideString>
        { typeof(IGenericCommands), "CustomCommand", "args", typeof(GlideString) },
    };

    /// <summary>
    /// Special cases: Dictionary and nullable array parameters.
    /// </summary>
    public static TheoryData<Type, string, string, Type> SpecialCollectionParameterData => new()
    {
        // Dictionary<ValkeyValue,ValkeyValue> → IEnumerable<KeyValuePair<ValkeyValue,ValkeyValue>>
        { typeof(IHashCommands), "HashSetExAsync", "fieldValueMap", typeof(KeyValuePair<ValkeyValue, ValkeyValue>) },

        // ValkeyValue[]? → IEnumerable<ValkeyValue>?
        { typeof(IGenericBaseCommands), "SortAsync", "get", typeof(ValkeyValue) },
    };

    #endregion

    #region Tests

    /// <summary>
    /// Verifies that each collection parameter uses IEnumerable&lt;T&gt; for the expected generic type T.
    /// On unfixed code, this test FAILS because parameters are concrete array types.
    /// </summary>
    [Theory]
    [MemberData(nameof(CollectionParameterData))]
    public void CollectionParameter_ShouldBeIEnumerable(
        Type interfaceType, string methodName, string parameterName, Type expectedGenericType)
    {
        var method = FindMethodWithParameter(interfaceType, methodName, parameterName);
        Assert.NotNull(method);

        var parameter = method!.GetParameters().First(p => p.Name == parameterName);
        var paramType = parameter.ParameterType;

        var expectedType = typeof(IEnumerable<>).MakeGenericType(expectedGenericType);

        Assert.True(
            paramType == expectedType,
            $"{interfaceType.Name}.{methodName} parameter '{parameterName}' is " +
            $"'{FormatTypeName(paramType)}' instead of 'IEnumerable<{expectedGenericType.Name}>'");
    }

    /// <summary>
    /// Verifies special collection parameters: Dictionary → IEnumerable&lt;KeyValuePair&gt; and
    /// nullable array → nullable IEnumerable.
    /// On unfixed code, this test FAILS because parameters are concrete types.
    /// </summary>
    [Theory]
    [MemberData(nameof(SpecialCollectionParameterData))]
    public void SpecialCollectionParameter_ShouldBeIEnumerable(
        Type interfaceType, string methodName, string parameterName, Type expectedGenericType)
    {
        var method = FindMethodWithParameter(interfaceType, methodName, parameterName);
        Assert.NotNull(method);

        var parameter = method!.GetParameters().First(p => p.Name == parameterName);
        var paramType = parameter.ParameterType;

        // For nullable parameters (like SortAsync's "get"), unwrap the nullable wrapper
        // by checking if the underlying type is IEnumerable<T>
        var typeToCheck = Nullable.GetUnderlyingType(paramType) ?? paramType;

        var expectedType = typeof(IEnumerable<>).MakeGenericType(expectedGenericType);

        Assert.True(
            typeToCheck == expectedType,
            $"{interfaceType.Name}.{methodName} parameter '{parameterName}' is " +
            $"'{FormatTypeName(paramType)}' instead of 'IEnumerable<{expectedGenericType.Name}>'");
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Finds a method overload on the given interface that has a parameter with the specified name.
    /// This handles overloaded methods by selecting the correct overload.
    /// </summary>
    private static MethodInfo? FindMethodWithParameter(Type interfaceType, string methodName, string parameterName)
    {
        return interfaceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name == methodName)
            .FirstOrDefault(m => m.GetParameters().Any(p => p.Name == parameterName));
    }

    /// <summary>
    /// Formats a type name for readable assertion messages.
    /// </summary>
    private static string FormatTypeName(Type type)
    {
        if (!type.IsGenericType)
            return type.IsArray ? $"{type.GetElementType()!.Name}[]" : type.Name;

        var genericDef = type.GetGenericTypeDefinition();
        var args = string.Join(", ", type.GetGenericArguments().Select(FormatTypeName));
        var baseName = genericDef.Name[..genericDef.Name.IndexOf('`')];
        return $"{baseName}<{args}>";
    }

    #endregion
}
