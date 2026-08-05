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
    /// Throws if the certificate byte array is null, empty, or exceeds <see cref="ConnectionConfiguration.CertificateMaxSize"/>.
    /// </summary>
    /// <param name="data">The certificate or key byte array to validate.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    internal static void ThrowIfCertificateNotSupported(byte[] data, string paramName)
    {
        ArgumentNullException.ThrowIfNull(data, paramName);
        ArgumentOutOfRangeException.ThrowIfZero(data.Length, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(data.Length, ConnectionConfiguration.CertificateMaxSize, paramName);
    }

    /// <summary>
    /// Throws if the certificate file at the given path is empty or exceeds <see cref="ConnectionConfiguration.CertificateMaxSize"/>.
    /// </summary>
    /// <param name="path">The certificate or key file path to check.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    internal static void ThrowIfCertificateNotSupported(string path, string paramName)
    {
        var length = new FileInfo(path).Length;
        ArgumentOutOfRangeException.ThrowIfZero(length, paramName);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, (long)ConnectionConfiguration.CertificateMaxSize, paramName);
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
