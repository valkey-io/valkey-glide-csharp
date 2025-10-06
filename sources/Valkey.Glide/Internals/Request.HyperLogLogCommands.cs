// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    /// <summary>
    /// Creates a command to add one element to a HyperLogLog data structure.
    /// </summary>
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <param name="element">The element to add.</param>
    /// <returns>A command that adds the element to the HyperLogLog and returns true if altered.</returns>
    public static Cmd<bool, bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element)
        => Simple<bool>(RequestType.PfAdd, [key.ToGlideString(), element.ToGlideString()]);

    /// <summary>
    /// Creates a command to add multiple elements to a HyperLogLog data structure.
    /// </summary>
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <param name="elements">The elements to add.</param>
    /// <returns>A command that adds the elements to the HyperLogLog and returns true if altered.</returns>
    public static Cmd<bool, bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue[] elements)
    {
        GlideString[] args = new GlideString[elements.Length + 1];
        args[0] = key.ToGlideString();
        for (int i = 0; i < elements.Length; i++)
        {
            args[i + 1] = elements[i].ToGlideString();
        }
        return Simple<bool>(RequestType.PfAdd, args);
    }

    /// <summary>
    /// Creates a command to get the cardinality of a HyperLogLog data structure.
    /// </summary>
    /// <param name="key">The key of the HyperLogLog.</param>
    /// <returns>A command that returns the approximated cardinality of the HyperLogLog.</returns>
    public static Cmd<long, long> HyperLogLogLengthAsync(ValkeyKey key)
        => Simple<long>(RequestType.PfCount, [key.ToGlideString()]);

    /// <summary>
    /// Creates a command to get the cardinality of the union of multiple HyperLogLog data structures.
    /// </summary>
    /// <param name="keys">The keys of the HyperLogLogs.</param>
    /// <returns>A command that returns the approximated cardinality of the union of HyperLogLogs.</returns>
    public static Cmd<long, long> HyperLogLogLengthAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.PfCount, keys.ToGlideStrings());
}