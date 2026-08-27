// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for <see cref="ClusterValue{T}" />.
/// </summary>
public class ClusterValueTests
{
    #region Test Data

    // Single-value
    private const int SingleValue = 1;
    private const string SingleReference = "REFERENCE";

    // Multi-value
    private static readonly Dictionary<string, int> ValueEmpty = [];
    private static readonly Dictionary<string, int> ValueString = new() { ["node1"] = 1 };
    private static readonly Dictionary<string, int> ValueStrings = new() { ["node1"] = 1, ["node2"] = 2 };
    private static readonly Dictionary<gs, int> ValueGlideStrings = new() { ["node1"] = 1, ["node2"] = 2 };

    private static readonly Dictionary<string, string> ReferenceEmpty = [];
    private static readonly Dictionary<string, string> ReferenceString = new() { ["node1"] = "value1" };
    private static readonly Dictionary<string, string> ReferenceStrings = new() { ["node1"] = "value1", ["node2"] = "value2" };
    private static readonly Dictionary<gs, string> ReferenceGlideStrings = new() { ["node1"] = "value1", ["node2"] = "value2" };

    #endregion
    #region Tests

    [Fact]
    public void OfMultiValue_ValueType()
    {
        // Values
        AssertMultiValue(ClusterValue<int>.OfMultiValue(ValueEmpty), ValueEmpty);
        AssertMultiValue(ClusterValue<int>.OfMultiValue(ValueString), ValueString);
        AssertMultiValue(ClusterValue<int>.OfMultiValue(ValueStrings), ValueStrings);
        AssertMultiValue(ClusterValue<int>.OfMultiValue(ValueGlideStrings), ValueStrings);

        // References
        AssertMultiValue(ClusterValue<string>.OfMultiValue(ReferenceEmpty), ReferenceEmpty);
        AssertMultiValue(ClusterValue<string>.OfMultiValue(ReferenceString), ReferenceString);
        AssertMultiValue(ClusterValue<string>.OfMultiValue(ReferenceStrings), ReferenceStrings);
        AssertMultiValue(ClusterValue<string>.OfMultiValue(ReferenceGlideStrings), ReferenceStrings);
    }

    [Fact]
    public void OfSingleValue_ValueType()
    {
        // Values
        AssertSingleValue(ClusterValue<int>.OfSingleValue(SingleValue), SingleValue);
        AssertSingleValue(ClusterValue<int>.OfSingleValue(default), default);

        // References
        AssertSingleValue(ClusterValue<string?>.OfSingleValue(SingleReference), SingleReference);
        AssertSingleValue(ClusterValue<string?>.OfSingleValue(null), null);
    }

    [Fact]
    public void SingleValue_OnMultiValue_Throws()
    {
        _ = Assert.Throws<Exception>(() => _ = ClusterValue<int>.OfMultiValue(ValueStrings).SingleValue);
        _ = Assert.Throws<Exception>(() => _ = ClusterValue<string>.OfMultiValue(ReferenceStrings).SingleValue);
    }

    [Fact]
    public void MultiValue_OnSingleValue_Throws()
    {
        _ = Assert.Throws<Exception>(() => _ = ClusterValue<int>.OfSingleValue(SingleValue).MultiValue);
        _ = Assert.Throws<Exception>(() => _ = ClusterValue<string>.OfSingleValue(SingleReference).MultiValue);
    }

    [Fact]
    public void Of_CreatesValue()
    {
        AssertSingleValue(ClusterValue<int>.Of(SingleValue), SingleValue);
        AssertMultiValue(ClusterValue<int>.Of(ValueStrings), ValueStrings);
        AssertMultiValue(ClusterValue<int>.Of(ValueGlideStrings), ValueStrings);

        _ = Assert.Throws<ArgumentException>(() => ClusterValue<int>.Of(SingleReference));
    }

    #endregion
    #region Helpers

    private static void AssertMultiValue<T>(ClusterValue<T> value, Dictionary<string, T> expected)
    {
        Assert.True(value.HasMultiData);
        Assert.False(value.HasSingleData);
        Assert.Equivalent(expected, value.MultiValue);
    }

    private static void AssertSingleValue<T>(ClusterValue<T> value, T expected)
    {
        Assert.True(value.HasSingleData);
        Assert.False(value.HasMultiData);
        Assert.Equivalent(expected, value.SingleValue);
    }

    #endregion
}
