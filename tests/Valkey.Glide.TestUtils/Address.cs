// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Represents a network address with a host and port.
/// </summary>
public record class Address(string Host, ushort Port)
{
    /// <summary>
    /// Parses a comma-separated list of hosts (e.g. "localhost:6379,localhost:6380") to addresses.
    /// </summary>
    public static IList<Address> FromHosts(string hosts)
    {
        List<Address> addresses = [];
        foreach (var host in hosts.Split(','))
        {
            var parts = host.Split(':');
            addresses.Add(new Address(parts[0], ushort.Parse(parts[1])));
        }

        return addresses;
    }
}
