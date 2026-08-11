// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.Internals;

/// <summary>
/// Internal helpers for <see cref="ReadFromStrategy"/>.
/// </summary>
internal static class ReadFromStrategyExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> if the strategy requires an Availability Zone (AZ).
    /// </summary>
    internal static bool IsAzReadFromStrategy(this ReadFromStrategy strategy) =>
        strategy is ReadFromStrategy.AzAffinity or ReadFromStrategy.AzAffinityReplicasAndPrimary;
}
