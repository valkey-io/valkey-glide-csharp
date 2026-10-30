// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Indicates the status of a condition as part of a transaction.
/// </summary>
/// <param name="condition">The condition to track.</param>
public sealed class ConditionResult(Condition condition)
{
    internal readonly Condition Condition = condition;

    /// <summary>
    /// Indicates whether the condition was satisfied.
    /// </summary>
    public bool WasSatisfied { internal set; get; }
}
