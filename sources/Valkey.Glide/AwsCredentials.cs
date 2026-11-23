// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Globalization;

namespace Valkey.Glide;

/// <summary>
/// AWS credentials returned by a <see cref="GlideCredentialProvider"/>.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/security/iam-integration/">Valkey GLIDE – Configure AWS IAM Authentication</seealso>
public sealed class AwsCredentials
{
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="AwsCredentials"/> instance.
    /// </summary>
    /// <param name="accessKeyId"><inheritdoc cref="AccessKeyId" path="/summary" /></param>
    /// <param name="secretAccessKey"><inheritdoc cref="SecretAccessKey" path="/summary" /></param>
    /// <param name="sessionToken"><inheritdoc cref="SessionToken" path="/summary" /></param>
    /// <param name="expiresAt"><inheritdoc cref="ExpiresAt" path="/summary" /></param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="accessKeyId"/> or <paramref name="secretAccessKey"/> is <see langword="null"/>,
    /// empty, or consists only of white-space characters.
    /// </exception>
    public AwsCredentials(
        string accessKeyId,
        string secretAccessKey,
        string? sessionToken = null,
        DateTimeOffset? expiresAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessKeyId);
        ArgumentException.ThrowIfNullOrWhiteSpace(secretAccessKey);

        AccessKeyId = accessKeyId;
        SecretAccessKey = secretAccessKey;
        SessionToken = sessionToken;
        ExpiresAt = expiresAt;
    }

    #endregion
    #region Public Properties

    /// <summary>
    /// The AWS access key ID.
    /// </summary>
    public string AccessKeyId { get; }

    /// <summary>
    /// The AWS secret access key.
    /// </summary>
    public string SecretAccessKey { get; }

    /// <summary>
    /// The optional AWS session token for temporary credentials.
    /// </summary>
    public string? SessionToken { get; }

    /// <summary>
    /// The optional time at which the credentials expire.
    /// </summary>
    /// <remarks>
    /// The expiry is passed to the AWS SDK as credential metadata. It does not change when IAM tokens are refreshed,
    /// which follows <see cref="IamAuthConfig.RefreshIntervalSeconds"/>. Leave it <see langword="null"/> if unknown.
    /// </remarks>
    public DateTimeOffset? ExpiresAt { get; }

    #endregion
    #region Public Methods

    /// <summary>
    /// Returns a string representation with sensitive data omitted.
    /// </summary>
    public override string ToString()
        => $"AwsCredentials {{ HasSessionToken = {SessionToken is not null}, ExpiresAt = {ExpiresAt?.ToString("O", CultureInfo.InvariantCulture) ?? "null"} }}";

    #endregion
}
