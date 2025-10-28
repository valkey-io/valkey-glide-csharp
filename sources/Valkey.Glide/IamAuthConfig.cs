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
}
