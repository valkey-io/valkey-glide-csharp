// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Helper methods for building command options in tests.
/// </summary>
public static class Options
{
    /// <summary>
    /// Builds and returns migrate options for testing.
    /// If not specified, required arguments are populated w ith default values.
    /// </summary>
    /// <param name="address">The destination address.</param>
    public static MigrateOptions BuildMigrateOptions(Address? address = null)
        => new(address?.Host ?? "localhost", address?.Port ?? 1234, 0, TimeSpan.FromSeconds(10));
}
