// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Contains guard clauses for validating method parameters and enforcing preconditions.
/// </summary>
internal static class GuardClauses
{
    /// <summary>
    /// Validates that the When parameter is either Always or NotExists.
    /// </summary>
    /// <param name="when">The When enum value to validate</param>
    /// <exception cref="ArgumentException">Thrown when the When parameter is not Always or NotExists</exception>
    public static void WhenAlwaysOrNotExists(When when)
    {
        switch (when)
        {
            case When.Always:
            case When.NotExists:
                break;
            case When.Exists:
            default:
                throw new ArgumentException(when + " is not valid in this context; the permitted values are: Always, NotExists");
        }
    }

    /// <summary>
    /// Throws a <see cref="NotImplementedException"/> if command flags are specified.
    /// </summary>
    /// <param name="flags">The command flags to validate.</param>
    /// <exception cref="NotImplementedException">Thrown when <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    public static void ThrowIfCommandFlags(CommandFlags flags)
    {
        if (flags != CommandFlags.None)
        {
            throw new NotImplementedException("Command flags are not supported by GLIDE");
        }
    }

    /// <summary>
    /// Throws a <see cref="NotImplementedException"/> if async state is specified.
    /// </summary>
    /// <param name="asyncState">The async state to validate.</param>
    /// <exception cref="NotImplementedException">Thrown when <paramref name="asyncState"/> is not null.</exception>
    public static void ThrowIfAsyncState(object? asyncState)
    {
        if (asyncState is not null)
        {
            throw new NotImplementedException("Async state is not supported by GLIDE");
        }
    }
}
