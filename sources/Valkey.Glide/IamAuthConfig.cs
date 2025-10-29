// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for IAM authentication with AWS services.
/// </summary>
/// <param name="clusterName">The name of the cluster.</param>
/// <param name="serviceType">The AWS service type.</param>
/// <param name="region">The AWS region where the cluster is located.</param>
/// <param name="refreshIntervalSeconds">Optional refresh interval in seconds.</param>
public class IamAuthConfig(string clusterName, ServiceType serviceType, string region, int? refreshIntervalSeconds = null)
{
    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string ClusterName { get; set; } = clusterName ?? throw new ArgumentNullException(nameof(clusterName));

    /// <summary>
    /// The AWS service type.
    /// </summary>
    public ServiceType ServiceType { get; set; } = serviceType;

    /// <summary>
    /// The AWS region where the cluster is located.
    /// </summary>
    public string Region { get; set; } = region ?? throw new ArgumentNullException(nameof(region));

    /// <summary>
    /// Optional refresh interval in seconds.
    /// </summary>
    public int? RefreshIntervalSeconds { get; set; } = refreshIntervalSeconds;
}
