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
        /// Asserts that the given result is a single value equal to the expected value.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual result.</param>
        public static void Equal(ValkeyValue expected, ValkeyResult actual)
            => Assert.Equal(expected, (ValkeyValue)actual);
    }
}
