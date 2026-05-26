// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for building client configurations.
/// </summary>
public static class Config
{
    #region Constants

    /// <summary>
    /// Timeout for client connection and reconnection attempts.
    /// Use a longer timeout to allow for slower connections in CI environments.
    /// </summary>
    private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Retry strategy for client connections.
    /// Allow retries for transient connection timeouts in CI environments.
    /// </summary>
    private static readonly RetryStrategy RetryStrategy = new(numberOfRetries: 5, factor: 100, exponentBase: 2);

    #endregion
    #region Public Methods

    /// <summary>
    /// Builds a standalone client configuration builder with the given addresses and options.
    /// </summary>
    public static StandaloneClientConfigurationBuilder BuildStandaloneConfig(
        IEnumerable<Address> addresses,
        bool useTls = false,
        TimeSpan? connectionTimeout = null,
        RetryStrategy? retryStrategy = null,
        byte[]? trustedCertificate = null,
        string? password = null,
        AddressResolverDelegate? addressResolver = null)
    {
        StandaloneClientConfigurationBuilder builder = new()
        {
            UseTls = useTls,
            ConnectionTimeout = connectionTimeout ?? ConnectionTimeout,
            ConnectionRetryStrategy = retryStrategy ?? RetryStrategy,
        };

        foreach ((string host, ushort port) in addresses)
        {
            _ = builder.WithAddress(host, port);
        }

        if (trustedCertificate is not null)
        {
            _ = builder.WithTrustedCertificate(trustedCertificate);
        }

        if (password is not null)
        {
            _ = builder.WithAuthentication(password);
        }

        if (addressResolver is not null)
        {
            _ = builder.WithAddressResolver(addressResolver);
        }

        return builder;
    }

    /// <summary>
    /// Builds a cluster client configuration builder with the given addresses and options.
    /// </summary>
    public static ClusterClientConfigurationBuilder BuildClusterConfig(
        IEnumerable<Address> addresses,
        bool useTls = false,
        TimeSpan? connectionTimeout = null,
        RetryStrategy? retryStrategy = null,
        byte[]? trustedCertificate = null,
        string? password = null,
        AddressResolverDelegate? addressResolver = null)
    {
        ClusterClientConfigurationBuilder builder = new()
        {
            UseTls = useTls,
            ConnectionTimeout = connectionTimeout ?? ConnectionTimeout,
            ConnectionRetryStrategy = retryStrategy ?? RetryStrategy,
        };

        foreach ((string host, ushort port) in addresses)
        {
            _ = builder.WithAddress(host, port);
        }

        if (trustedCertificate is not null)
        {
            _ = builder.WithTrustedCertificate(trustedCertificate);
        }

        if (password is not null)
        {
            _ = builder.WithAuthentication(password);
        }

        if (addressResolver is not null)
        {
            _ = builder.WithAddressResolver(addressResolver);
        }

        return builder;
    }

    /// <summary>
    /// Builds a client configuration for the given mode, addresses, and options.
    /// </summary>
    public static BaseClientConfiguration BuildConfig(
        bool clusterMode,
        IEnumerable<Address> addresses,
        bool useTls = false,
        TimeSpan? connectionTimeout = null,
        RetryStrategy? retryStrategy = null,
        byte[]? trustedCertificate = null,
        string? password = null,
        AddressResolverDelegate? addressResolver = null)
        => clusterMode
            ? BuildClusterConfig(addresses, useTls, connectionTimeout, retryStrategy, trustedCertificate, password, addressResolver).Build()
            : BuildStandaloneConfig(addresses, useTls, connectionTimeout, retryStrategy, trustedCertificate, password, addressResolver).Build();

    #endregion
}
