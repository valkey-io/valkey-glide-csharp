// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Contains guard clauses for validating method parameters and enforcing preconditions.
/// </summary>
internal static class GuardClauses
{
    #region Constants

    /// <summary>
    /// Maximum size for byte data (10 MB). Security measure to prevent
    /// excessive memory allocation from malformed or malicious data.
    /// </summary>
    /// <seealso href="https://github.com/valkey-io/valkey-glide-csharp/issues/226">#226</seealso>
    internal static readonly long MaxDataSize = 10 * 1024 * 1024;

    /// <summary>
    /// Maximum value for <see cref="uint"/> milliseconds
    /// </summary>
    internal static readonly TimeSpan MaxUintMilliseconds = TimeSpan.FromMilliseconds(uint.MaxValue);

    /// <summary>
    /// Maximum value for <see cref="uint"/> seconds
    /// </summary>
    internal static readonly TimeSpan MaxUintSeconds = TimeSpan.FromSeconds(uint.MaxValue);

    #endregion

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
    /// Throws an <see cref="ArgumentException"/> if the byte array is not supported.
    /// </summary>
    /// <param name="data">The byte array to validate.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    internal static void ThrowIfDataNotSupported(byte[] data, string paramName)
    {
        ArgumentNullException.ThrowIfNull(data, paramName);
        ArgumentOutOfRangeException.ThrowIfZero(data.Length, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(data.Length, MaxDataSize, paramName);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if file is not supported.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    internal static void ThrowIfFileNotSupported(string path, string paramName)
    {
        var fileLength = new FileInfo(path).Length;
        ArgumentOutOfRangeException.ThrowIfZero(fileLength, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(fileLength, MaxDataSize, paramName);
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
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the given value is not positive.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The parameter name for the exception.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is zero or negative.</exception>
    public static void ThrowIfNotPositive(long value, string name)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(name, value, "Value must be positive.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the given <see cref="TimeSpan"/>
    /// cannot be represented as a positive <see cref="uint"/> number of seconds.
    /// </summary>
    /// <param name="value">The time span value to validate.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    internal static void ThrowIfNotPositiveUintSeconds(TimeSpan value, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, TimeSpan.Zero, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxUintSeconds, paramName);
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

    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the given <see cref="TimeSpan"/>
    /// cannot be represented as a positive <see cref="uint"/> number of milliseconds.
    /// (<see cref="MaxUintMilliseconds"/>).
    /// </summary>
    /// <param name="value">The time span value to validate.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    internal static void ThrowIfNotUintMilliseconds(TimeSpan value, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, TimeSpan.Zero, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxUintMilliseconds, paramName);
    }
}
