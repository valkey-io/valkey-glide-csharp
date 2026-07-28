// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Contains guard clauses for validating method parameters and enforcing preconditions.
/// </summary>
internal static class GuardClauses
{
    /// <summary>
    /// Throws a <see cref="NotImplementedException"/> if async state is specified.
    /// </summary>
    /// <param name="asyncState">The async state to validate.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="asyncState"/> is not null.</exception>
    public static void ThrowIfAsyncState(object? asyncState)
    {
        if (asyncState is not null)
        {
            throw new NotImplementedException("Async state is not supported by Valkey GLIDE");
        }
    }

    /// <summary>
    /// Throws a <see cref="NotImplementedException"/> if command flags are specified.
    /// </summary>
    /// <param name="flags">The command flags to validate.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    public static void ThrowIfCommandFlags(CommandFlags flags)
    {
        if (flags != CommandFlags.None)
        {
            throw new NotImplementedException($"Command flag {flags} is not supported by Valkey GLIDE");
        }
    }

}
