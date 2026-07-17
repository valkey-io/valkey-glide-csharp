// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Helper methods for building command options and configurations for testing.
/// </summary>
public static class Builders
{
    /// <summary>
    /// Builds and returns a <see cref="ClientSideCacheConfig"/> for testing.
    /// If not specified, uses sensible defaults (1024 KB, 1 minute TTL, server-assisted).
    /// </summary>
    public static ClientSideCacheConfig BuildClientSideCacheConfig(
        ulong maxCacheKb = 1024,
        TimeSpan? entryTtl = null)
        => new ClientSideCacheConfig(maxCacheKb, entryTtl ?? TimeSpan.FromMinutes(1))
            .WithServerAssisted();

    /// <summary>
    /// Builds and returns an <see cref="IamAuthConfig"/> for testing.
    /// If not specified, required arguments are populated with default values.
    /// </summary>
    public static IamAuthConfig BuildIamAuthConfig(
        string? clusterName = null,
        ServiceType serviceType = ServiceType.ElastiCache,
        string? region = null,
        uint? refreshIntervalSeconds = null)
        => new(clusterName ?? "CLUSTER_NAME", serviceType, region ?? "REGION", refreshIntervalSeconds);

    /// <summary>
    /// Builds and returns <see cref="MigrateOptions"/> for testing.
    /// If not specified, required arguments are populated with default values.
    /// </summary>
    public static MigrateOptions BuildMigrateOptions(
        Address? address = null,
        TimeSpan? timeout = null)
        => new(
            host: address?.Host ?? "HOST",
            port: address?.Port ?? 1234,
            destinationDb: 0,
            timeout: timeout ?? TimeSpan.FromSeconds(10));

    /// <summary>
    /// Builds and returns a <see cref="MonitorConfig"/> for testing.
    /// If not specified, required arguments are populated with default values.
    /// </summary>
    public static MonitorConfig BuildMonitorConfig(string? host = null, ushort? port = null)
        => new(host ?? "HOST", port ?? 1234);

    /// <summary>
    /// Builds and returns <see cref="ServerCredentials"/> for testing.
    /// If not specified, required arguments are populated with default values.
    /// </summary>
    public static ServerCredentials BuildServerCredentials(string? username = null, string? password = null)
        => username is not null
            ? new ServerCredentials(username, password ?? "PASSWORD")
            : new ServerCredentials(password ?? "PASSWORD");
}
