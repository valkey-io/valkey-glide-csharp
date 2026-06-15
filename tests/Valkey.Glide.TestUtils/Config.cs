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
    /// Use a longer timeout to allow for slower connection in CI environments.
    /// </summary>
    private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Timeout for command requests.
    /// Use a longer timeout to allow for slower execution in CI environments.
    /// </summary>
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Retry strategy for client connections.
    /// Allow retries for transient connection timeouts in CI environments.
    /// </summary>
    private static readonly RetryStrategy RetryStrategy = new(numberOfRetries: 5, factor: 100, exponentBase: 2);

    #endregion
    #region Public Methods

    /// <summary>
    /// Builds a standalone client configuration builder with the given address and options.
    /// </summary>
    public static StandaloneClientConfigurationBuilder BuildStandaloneConfig(
        Address address,
        AddressResolverDelegate? addressResolver = null,
        bool useTls = false,
        TimeSpan? connectionTimeout = null,
        TimeSpan? requestTimeout = null,
        RetryStrategy? retryStrategy = null,
        byte[]? trustedCertificate = null,
        string? password = null)
    {
        StandaloneClientConfigurationBuilder builder = new()
        {
            UseTls = useTls,
            ConnectionTimeout = connectionTimeout ?? ConnectionTimeout,
            RequestTimeout = requestTimeout ?? RequestTimeout,
            ConnectionRetryStrategy = retryStrategy ?? RetryStrategy,
        };

        _ = builder.WithAddress(address.Host, address.Port);

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
    /// Builds a cluster client configuration builder with the given address and options.
    /// </summary>
    public static ClusterClientConfigurationBuilder BuildClusterConfig(
        Address address,
        AddressResolverDelegate? addressResolver = null,
        bool useTls = false,
        TimeSpan? connectionTimeout = null,
        TimeSpan? requestTimeout = null,
        RetryStrategy? retryStrategy = null,
        byte[]? trustedCertificate = null,
        string? password = null)
    {
        ClusterClientConfigurationBuilder builder = new()
        {
            UseTls = useTls,
            ConnectionTimeout = connectionTimeout ?? ConnectionTimeout,
            RequestTimeout = requestTimeout ?? RequestTimeout,
            ConnectionRetryStrategy = retryStrategy ?? RetryStrategy,
        };

        _ = builder.WithAddress(address.Host, address.Port);

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
    /// Builds a client configuration for the given mode, address, and options.
    /// </summary>
    public static BaseClientConfiguration BuildConfig(
        bool clusterMode,
        Address address,
        AddressResolverDelegate? addressResolver = null,
        bool useTls = false,
        TimeSpan? connectionTimeout = null,
        TimeSpan? requestTimeout = null,
        RetryStrategy? retryStrategy = null,
        byte[]? trustedCertificate = null,
        string? password = null)
        => clusterMode
            ? BuildClusterConfig(address, addressResolver, useTls, connectionTimeout, requestTimeout, retryStrategy, trustedCertificate, password).Build()
            : BuildStandaloneConfig(address, addressResolver, useTls, connectionTimeout, requestTimeout, retryStrategy, trustedCertificate, password).Build();

    #endregion
}
