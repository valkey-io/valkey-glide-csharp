// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Supplies AWS credentials used to sign IAM authentication tokens.
/// </summary>
/// <param name="cancellationToken">
/// Signaled when the invocation times out or the client is disposed.
/// </param>
/// <returns>A task that completes with the AWS credentials to use for signing.</returns>
/// <remarks>
/// <para>
/// The provider is invoked whenever the client generates an IAM token: during client creation,
/// on each scheduled token refresh, and on each call to <see cref="BaseClient.RefreshIamTokenAsync"/>.
/// </para>
/// <para>
/// The provider runs on the .NET thread pool. Each invocation has nine seconds in total to return its
/// task and complete it. A call is rejected while the synchronous part of a previous invocation for the
/// same client is still running, so return the task promptly and perform slow work asynchronously.
/// </para>
/// <para>
/// Clients created from the same configuration share the provider and may invoke it concurrently, so
/// the provider must be thread-safe.
/// </para>
/// </remarks>
/// <seealso href="https://glide.valkey.io/how-to/security/iam-integration/">Valkey GLIDE – Configure AWS IAM Authentication</seealso>
public delegate Task<AwsCredentials> GlideCredentialProvider(CancellationToken cancellationToken);
