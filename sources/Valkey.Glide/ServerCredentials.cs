// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents the credentials for connecting to a server.
/// Supports both password-based and IAM authentication modes, which are mutually exclusive.
/// </summary>
public class ServerCredentials
{
    /// <summary>
    /// The username that will be used for authenticating connections to the servers.
    /// If not supplied, "default" will be used.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// The password that will be used for authenticating connections to the servers.
    /// Required for password-based authentication, must be null for IAM authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// IAM authentication configuration.
    /// Required for IAM authentication, must be null for password-based authentication.
    /// </summary>
    public IamAuthConfig? IamConfig { get; set; }

    /// <summary>
    /// Creates server credentials for password-based authentication.
    /// </summary>
    /// <param name="username">The username for authentication. If null, "default" will be used.</param>
    /// <param name="password">The password for authentication.</param>
    public ServerCredentials(string? username, string password)
    {
        Username = username;
        Password = password ?? throw new ArgumentNullException(nameof(password));
        IamConfig = null;
    }

    /// <summary>
    /// Creates server credentials for password-based authentication.
    /// Username "default" will be used.
    /// </summary>
    /// <param name="password">The password for authentication.</param>
    public ServerCredentials(string password)
    {
        Username = null;
        Password = password ?? throw new ArgumentNullException(nameof(password));
        IamConfig = null;
    }

    /// <summary>
    /// Creates server credentials for IAM authentication.
    /// </summary>
    /// <param name="username">The username for authentication.</param>
    /// <param name="iamConfig">The IAM authentication configuration.</param>
    public ServerCredentials(string username, IamAuthConfig iamConfig)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        IamConfig = iamConfig ?? throw new ArgumentNullException(nameof(iamConfig));
        Password = null;
    }

    /// <summary>
    /// Returns true if this instance is configured for IAM authentication.
    /// </summary>
    public bool IsIamAuth() => IamConfig != null;
}
