// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;
using System.Net;

namespace Valkey.Glide.Internals;

internal static class Utils
{
    public static (string host, ushort port) SplitEndpoint(EndPoint ep)
        => ep switch
        {
            DnsEndPoint dns => (dns.Host, (ushort)dns.Port),
            IPEndPoint ip => (ip.Address.ToString(), (ushort)ip.Port),
            _ => throw new ArgumentException($"Unsupported endpoint type: {ep.GetType()}"),
        };

    /// <summary>
    /// Formats a host and port as an address string.
    /// </summary>
    /// <param name="host">The hostname or IP address.</param>
    /// <param name="port">The port number.</param>
    /// <returns>
    /// A formatted address string (e.g. <c>127.0.0.1:6379</c> or <c>[::1]:6379</c>).
    /// </returns>
    public static string FormatAddress(string host, ushort port)
        => Format.ToString(Format.ParseEndPoint(host, port));

    public static void Requires<TException>(bool predicate, string message)
        where TException : Exception, new()
    {
        if (!predicate)
        {
            Debug.WriteLine(message);
            throw new TException();
        }
    }

    public static List<Tuple<string, KeyValuePair<string, string>>> ParseInfoResponse(string data)
    {
        string category = "miscellaneous";
        List<Tuple<string, KeyValuePair<string, string>>> list = [];
        using StringReader reader = new(data);
        while (reader.ReadLine() is string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("# "))
            {
                category = line[2..].Trim();
                continue;
            }

            int idx = line.IndexOf(':');
            if (idx < 0)
            {
                continue;
            }

            KeyValuePair<string, string> pair = new(
                line[..idx].Trim(),
                line[(idx + 1)..].Trim());
            list.Add(Tuple.Create(category, pair));
        }

        return list;
    }

    /// <summary>
    /// The default library name reported via <c>CLIENT SETINFO LIB-NAME</c> when no override is supplied.
    /// </summary>
    public const string DefaultLibraryName = "GlideC#";

    /// <summary>
    /// Composes the value reported via <c>CLIENT SETINFO LIB-NAME</c> from an optional library-name
    /// override and an optional client-info tag. Used by every connection type (standard, cluster,
    /// and MONITOR) so the composition stays identical across them.
    /// </summary>
    /// <param name="libraryName">Full override for the library name, or <see langword="null"/> to use <see cref="DefaultLibraryName"/>.</param>
    /// <param name="clientInfoTag">
    /// Tag appended in parentheses, e.g. <c>GlideC#(tag)</c>, or <see langword="null"/> for none.
    /// A non-null value — including an empty or whitespace-only one — is passed through as
    /// supplied. GLIDE core validates the effective library name before client creation and fails
    /// creation with a configuration error if it is malformed; validation is deliberately not
    /// duplicated here, per the standing rule that the client defers to the server.
    /// </param>
    /// <returns>The resolved library name to send to the server.</returns>
    public static string ResolveLibraryName(string? libraryName, string? clientInfoTag)
    {
        string baseName = libraryName ?? DefaultLibraryName;
        return clientInfoTag is null
            ? baseName
            : $"{baseName}({clientInfoTag})";
    }
}
