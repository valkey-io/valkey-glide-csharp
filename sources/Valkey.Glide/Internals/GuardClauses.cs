// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Internals;

/// <summary>
/// Contains guard clauses for validating method parameters and enforcing preconditions.
/// </summary>
internal static class GuardClauses
{
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
    /// Throws if the byte array is null, empty, or exceeds <paramref name="maxLength"/>.
    /// </summary>
    /// <param name="data">The byte array to validate.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    /// <param name="maxLength">The maximum allowed byte array length.</param>
    internal static void ThrowIfBytesNotSupported(byte[] data, string paramName, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(data, paramName);
        ArgumentOutOfRangeException.ThrowIfZero(data.Length, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(data.Length, maxLength, paramName);
    }

    /// <summary>
    /// Throws if the file at the given path is empty or exceeds <paramref name="maxLength"/>.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    /// <param name="maxLength">The maximum allowed file length.</param>
    internal static void ThrowIfFileNotSupported(string path, string paramName, int maxLength)
    {
        var length = new FileInfo(path).Length;
        ArgumentOutOfRangeException.ThrowIfZero(length, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, maxLength, paramName);
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
