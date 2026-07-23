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

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the given time span is negative.
    /// </summary>
    /// <param name="value">The time span value to validate.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is negative.</exception>
    public static void ThrowIfNegative(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
        {
            throw new ArgumentException("Time span cannot be negative.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the given value is negative.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The parameter name for the exception.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative.</exception>
    public static void ThrowIfNegative(long value, string name)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(name, value, "Value cannot be negative.");
        }
    }

    /// <summary>
    /// Throws a <see cref="NotImplementedException"/> if the stream trim mode is not supported.
    /// </summary>
    /// <param name="trimMode">The stream trim mode to validate.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="trimMode"/> is not <see cref="StreamTrimMode.KeepReferences"/>.</exception>
    public static void ThrowIfNotSupported(StreamTrimMode trimMode)
    {
        if (trimMode != StreamTrimMode.KeepReferences)
        {
            throw new NotImplementedException($"Stream trim mode {trimMode} is not supported by Valkey GLIDE");
        }
    }
}
