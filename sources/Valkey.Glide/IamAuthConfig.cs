// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for IAM authentication with AWS services.
/// </summary>
public class IamAuthConfig
{
    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string ClusterName { get; set; } = string.Empty;

    /// <summary>
    /// The AWS service type.
    /// </summary>
    public ServiceType ServiceType { get; set; }

    /// <summary>
    /// The AWS region where the cluster is located.
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Optional refresh interval in seconds.
    /// </summary>
    public int? RefreshIntervalSeconds { get; set; }

    /// <summary>
    /// Creates IAM authentication configuration.
    /// </summary>
    /// <param name="clusterName">The name of the cluster.</param>
    /// <param name="serviceType">The AWS service type.</param>
    /// <param name="region">The AWS region where the cluster is located.</param>
    /// <param name="refreshIntervalSeconds">Optional refresh interval in seconds.</param>
    public IamAuthConfig(string clusterName, ServiceType serviceType, string region, int? refreshIntervalSeconds = null)
    {
        ClusterName = clusterName;
        ServiceType = serviceType;
        Region = region;
        RefreshIntervalSeconds = refreshIntervalSeconds;
    }
}
