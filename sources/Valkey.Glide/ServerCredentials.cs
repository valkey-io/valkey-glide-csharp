// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents the credentials for connecting to a server.
/// Supports both password-based and IAM authentication modes, which are mutually exclusive.
/// </summary>
public class ServerCredentials : IDisposable
{
    #region Fields

    private bool _disposed;

    #endregion
    #region Public Properties

    /// <summary>
    /// The username to use for authenticating connections.
    /// If not specified, "default" will be used.
    /// </summary>
    public string? Username { get; private set; }

    /// <summary>
    /// IAM authentication configuration to use for authenticating connections.
    /// Required for IAM authentication, must be <c>null</c> for password-based authentication.
    /// </summary>
    public IamAuthConfig? IamConfig { get; private set; }

    #endregion
    #region Internal Properties

    /// <summary>
    /// The password to use for authenticating connections.
    /// Required for password-based authentication, must be <c>null</c> for IAM authentication.
    /// </summary>
    internal string? Password { get; private set; }

    #endregion
    #region Constructors

    /// <summary>
    /// Creates server credentials for password-based authentication.
    /// </summary>
    /// <param name="username"><inheritdoc cref="Username" path="/summary" /></param>
    /// <param name="password"><inheritdoc cref="Password" path="/summary" /></param>
    public ServerCredentials(string? username, string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        Username = username;
        Password = password;
        IamConfig = null;
    }

    /// <summary>
    /// Creates server credentials for password-based authentication.
    /// </summary>
    /// <param name="password"><inheritdoc cref="Password" path="/summary" /></param>
    public ServerCredentials(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        Username = null;
        Password = password;
        IamConfig = null;
    }

    /// <summary>
    /// Creates server credentials for IAM authentication.
    /// </summary>
    /// <param name="username"><inheritdoc cref="Username" path="/summary" /></param>
    /// <param name="iamConfig"><inheritdoc cref="IamConfig" path="/summary" /></param>
    public ServerCredentials(string username, IamAuthConfig iamConfig)
    {
        ArgumentNullException.ThrowIfNull(username, nameof(username));
        ArgumentNullException.ThrowIfNull(iamConfig, nameof(iamConfig));

        Username = username;
        Password = null;
        IamConfig = iamConfig;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Returns true if this instance is configured for IAM authentication.
    /// </summary>
    public bool IsIamAuth()
        => IamConfig != null;

    /// <summary>
    /// Returns a string representation with sensitive data omitted.
    /// </summary>
    public override string ToString()
        // Omit sensitive data.
        => $"ServerCredentials {{ Username = {Username}, IsIamAuth = {IsIamAuth()} }}";

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            // Clear sensitive data.
            Username = null;
            Password = null;
            IamConfig = null;

            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    #endregion
}
