// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for IAM authentication with AWS services.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/security/iam-integration/">Valkey GLIDE – Configure AWS IAM Authentication</seealso>
public sealed class IamAuthConfig : IDisposable
{
    #region Private Fields

    private bool _disposed;
    private GlideCredentialProvider? _credentialsProvider;

    #endregion
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="IamAuthConfig"/> instance.
    /// </summary>
    /// <param name="clusterName"><inheritdoc cref="ClusterName" path="/summary" /></param>
    /// <param name="serviceType"><inheritdoc cref="ServiceType" path="/summary" /></param>
    /// <param name="region"><inheritdoc cref="Region" path="/summary" /></param>
    /// <param name="refreshIntervalSeconds"><inheritdoc cref="RefreshIntervalSeconds" path="/summary" /></param>
    public IamAuthConfig(
        string clusterName,
        ServiceType serviceType,
        string region,
        uint? refreshIntervalSeconds = null)
    {
        ArgumentNullException.ThrowIfNull(clusterName, nameof(clusterName));
        ArgumentNullException.ThrowIfNull(region, nameof(region));

        ClusterName = clusterName;
        ServiceType = serviceType;
        Region = region;
        RefreshIntervalSeconds = refreshIntervalSeconds;
    }

    #endregion
    #region Public Properties

    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string ClusterName
    {
        get { ThrowIfDisposed(); return field; }
        private set;
    }

    /// <summary>
    /// The AWS service type.
    /// </summary>
    public ServiceType ServiceType { get; }

    /// <summary>
    /// The AWS region where the cluster is located.
    /// </summary>
    public string Region
    {
        get { ThrowIfDisposed(); return field; }
        private set;
    }

    /// <summary>
    /// Optional refresh interval in seconds.
    /// </summary>
    public uint? RefreshIntervalSeconds
    {
        get { ThrowIfDisposed(); return field; }
        private set;
    }

    /// <summary>
    /// Optional provider of the AWS credentials used to sign IAM tokens.
    /// If <see langword="null"/>, credentials are resolved from the default AWS credential chain.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The client configuration captures the provider when it is built, so disposing this instance
    /// afterwards does not affect clients created from that configuration.
    /// </para>
    /// <para>
    /// If the provider fails, client creation throws <see cref="Errors.ConnectionException"/> and
    /// <see cref="BaseClient.RefreshIamTokenAsync"/> throws <see cref="Errors.RequestException"/>.
    /// See <see cref="GlideCredentialProvider"/> for invocation, timeout, and concurrency behavior.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var iamConfig = new IamAuthConfig("my-cache", ServiceType.ElastiCache, "us-east-1")
    /// {
    ///     CredentialsProvider = _ => Task.FromResult(
    ///         new AwsCredentials(
    ///             "access-key-id", "secret-access-key", "session-token", DateTimeOffset.UtcNow.AddHours(1))),
    /// };
    /// </code>
    /// </example>
    public GlideCredentialProvider? CredentialsProvider
    {
        get { ThrowIfDisposed(); return _credentialsProvider; }
        init => _credentialsProvider = value;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Returns a string representation with sensitive data omitted.
    /// </summary>
    public override string ToString()
        => $"IamAuthConfig {{ ServiceType = {ServiceType} }}";

    /// <summary>
    /// Clears sensitive data.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            ClusterName = string.Empty;
            Region = string.Empty;
            RefreshIntervalSeconds = null;
            _credentialsProvider = null;

            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Throws <see cref="ObjectDisposedException"/> if the object is disposed.
    /// </summary>
    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_disposed, this);

    #endregion
}
