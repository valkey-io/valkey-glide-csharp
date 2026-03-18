// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for IAM authentication with AWS services.
/// </summary>
/// <param name="clusterName">The name of the cluster.</param>
/// <param name="serviceType">The AWS service type.</param>
/// <param name="region">The AWS region where the cluster is located.</param>
/// <param name="refreshIntervalSeconds">Optional refresh interval in seconds (must be between 10 and 3600 if specified).</param>
public class IamAuthConfig(string clusterName, ServiceType serviceType, string region, uint? refreshIntervalSeconds = null)
{
    private const uint MinRefreshIntervalSeconds = 10;
    private const uint MaxRefreshIntervalSeconds = 3600;

    private uint? _refreshIntervalSeconds = ValidateRefreshInterval(refreshIntervalSeconds);

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
    /// Optional refresh interval in seconds. Must be between 10 and 3600 if specified.
    /// </summary>
    public uint? RefreshIntervalSeconds
    {
        get => _refreshIntervalSeconds;
        set => _refreshIntervalSeconds = ValidateRefreshInterval(value);
    }

    /// <summary>
    /// Returns a safe string representation that omits sensitive fields (cluster name, region).
    /// </summary>
    public override string ToString() =>
        $"IamAuthConfig {{ ServiceType = {ServiceType} }}";

    private static uint? ValidateRefreshInterval(uint? value)
    {
        if (value.HasValue && (value.Value < MinRefreshIntervalSeconds || value.Value > MaxRefreshIntervalSeconds))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Refresh interval must be between {MinRefreshIntervalSeconds} and {MaxRefreshIntervalSeconds} seconds.");
        }
        return value;
    }
}
