// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The options for an operation to add or update  sorted set members.
/// </summary>
/// <seealso href="https://valkey.io/commands/zadd/"/>
public readonly struct SortedSetAddOptions
{
    /// <summary>
    /// The condition under which to add or update  members.
    /// </summary>
    public SortedSetAddCondition Condition { get; init; }

    /// <summary>
    /// Whether to return the number of changed elements (added and
    /// updated) instead of the number of added elements only (CH).
    /// </summary>
    public bool Changed { get; init; }

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];
        args.AddRange(Condition.ToArgs());

        if (Changed)
        {
            args.Add(ValkeyLiterals.CH);
        }

        return [.. args];
    }
}
