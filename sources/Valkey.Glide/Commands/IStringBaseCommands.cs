// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// String commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#string">Valkey – String Commands</seealso>
public interface IStringBaseCommands
{
    /// <summary>
    /// Returns the substring of the string value stored at key, determined by the offsets
    /// start and end (both are inclusive).
    /// Negative offsets can be used in order to provide an offset starting from the end of the string. So `-1` means the last
    /// character, `-2` the penultimate and so forth.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getrange/">valkey.io</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The starting offset.</param>
    /// <param name="end">The ending offset.</param>
    /// <returns>
    /// A substring extracted from the value stored at key.<br/>
    /// An empty string is returned if the key does not exist or if the start and end offsets are out of range.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "Hello World");
    /// var response = await client.StringGetRangeAsync("key", 0, 4);
    /// Console.WriteLine(response); // Output: "Hello"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end);

    /// <summary>
    /// Returns the longest common subsequence between the values at <paramref name="first"/> and <paramref name="second"/>,
    /// returning a string containing the common sequence.
    /// Note that this is different than the longest common string algorithm,
    /// since matching characters in the string does not need to be contiguous.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lcs/">valkey.io</seealso>
    /// <note>Since Valkey 7.0 and above.</note>
    /// <note>When in cluster mode, both <paramref name="first"/> and <paramref name="second"/> must map to the same hash slot.</note>
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <returns>A string (sequence of characters) of the LCS match.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key1", "abcdef");
    /// await client.SetAsync("key2", "acef");
    /// var response = await client.StringLongestCommonSubsequenceAsync("key1", "key2");
    /// Console.WriteLine(response); // Output: "acef"
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second);

    /// <summary>
    /// Returns the longest common subsequence between the values at <paramref name="first"/> and <paramref name="second"/>,
    /// returning the length of the common sequence.
    /// Note that this is different than the longest common string algorithm,
    /// since matching characters in the string does not need to be contiguous.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lcs/">valkey.io</seealso>
    /// <note>Since Valkey 7.0 and above.</note>
    /// <note>When in cluster mode, both <paramref name="first"/> and <paramref name="second"/> must map to the same hash slot.</note>
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <returns>The length of the LCS match.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key1", "abcdef");
    /// await client.SetAsync("key2", "acef");
    /// long length = await client.StringLongestCommonSubsequenceLengthAsync("key1", "key2");
    /// Console.WriteLine(length); // Output: 4
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second);

    /// <summary>
    /// Returns the longest common subsequence between the values at <paramref name="first"/> and <paramref name="second"/>,
    /// returning a list of all common sequences with their positions and match information.
    /// Note that this is different than the longest common string algorithm,
    /// since matching characters in the string does not need to be contiguous.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lcs/">valkey.io</seealso>
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <param name="minLength">Can be used to restrict the list of matches to the ones of a given minimum length. Defaults to 0.</param>
    /// <returns>The result of LCS algorithm, containing match positions and lengths based on the given parameters.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key1", "abcdefghijk");
    /// await client.SetAsync("key2", "b1fh");
    /// LCSMatchResult result = await client.StringLongestCommonSubsequenceWithMatchesAsync("key1", "key2");
    /// Console.WriteLine($"LCS Length: {result.LongestMatchLength}");
    /// foreach (var match in result.Matches)
    /// {
    ///     Console.WriteLine($"Match at positions: first[{match.FirstStringIndex}], second[{match.SecondStringIndex}], length: {match.Length}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0);
}
