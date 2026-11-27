// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Static extension members for <see cref="Assert"/>.
/// </summary>
public static class AssertExtensions
{
    extension(Assert)
    {
        /// <summary>
        /// Asserts that the given result is a single value that is equal to the expected string.
        /// </summary>
        public static void Equal(string expected, ValkeyResult actual)
            => Assert.Equal(expected, (string?)actual);
        /// <summary>
        /// Asserts that the given result is a single value that is equal to the expected key.
        /// </summary>
        public static void Equal(ValkeyKey expected, ValkeyResult actual)
            => Assert.Equal(expected, (ValkeyKey)actual);

        /// <summary>
        /// Asserts that the given result is a single value that is equal to the   expected value.
        /// </summary>
        public static void Equal(ValkeyValue expected, ValkeyResult actual)
            => Assert.Equal(expected, (ValkeyValue)actual);

        /// <summary>
        /// Asserts that the given result is an array equal to the expected booleans.
        /// </summary>
        public static void Equal(bool[] expected, ValkeyResult actual)
            => Assert.Equal(expected, (bool[]?)actual);

        /// <summary>
        /// Asserts that the given result is an array equal to the expected strings.
        /// </summary>
        public static void Equal(string?[] expected, ValkeyResult actual)
            => Assert.Equal(expected, (string?[]?)actual);

        // TODO #678: Assert that the result is a set (Resp3Type is Set under RESP3).
        /// <summary>
        /// Asserts that the given result is an array containing exactly the expected strings, in any order.
        /// </summary>
        public static void Equal(ISet<string> expected, ValkeyResult actual)
            => Assert.Equal(expected, new HashSet<string>(((string?[]?)actual)!.Select(s => s!)));

        // TODO #678: Assert that the result is a map (Resp3Type is Map under RESP3).
        /// <summary>
        /// Asserts that the given result is a map equal to the expected string dictionary.
        /// </summary>
        public static void Equal(Dictionary<string, string> expected, ValkeyResult actual)
            => Assert.Equal(expected, actual.ToDictionary().ToDictionary(kv => kv.Key, kv => (string)kv.Value!));
    }
}
