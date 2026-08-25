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
}
