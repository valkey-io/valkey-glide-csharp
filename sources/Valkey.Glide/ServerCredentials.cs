// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents the credentials for connecting to a server.
/// Supports both password-based and IAM authentication modes, which are mutually exclusive.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/security/authentication"/>
public sealed class ServerCredentials : IDisposable
{
    #region Fields

    private bool _disposed;

    #endregion
    #region Public Properties

    /// <summary>
    /// The username to use for authenticating connections.
    /// If not specified, "default" will be used.
    /// </summary>
    public string? Username
    {
        get { ThrowIfDisposed(); return field; }
        private set;
    }

    /// <summary>
    /// IAM authentication configuration to use for authenticating connections.
    /// Required for IAM authentication, must be <c>null</c> for password-based authentication.
    /// It is not owned by the <see cref="ServerCredentials"/> instance - the caller is responsible for disposing it.
    /// </summary>
    public IamAuthConfig? IamAuthConfig
    {
        get { ThrowIfDisposed(); return field; }
        private set;
    }

    #endregion
    #region Internal Properties

    /// <summary>
    /// The password to use for authenticating connections.
    /// Required for password-based authentication, must be <c>null</c> for IAM authentication.
    /// </summary>
    internal char[]? Password
    {
        get { ThrowIfDisposed(); return field; }
        private set;
    }

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
        Password = password.ToCharArray();
        IamAuthConfig = null;
    }

    /// <summary>
    /// Creates server credentials for password-based authentication.
    /// </summary>
    /// <param name="password"><inheritdoc cref="Password" path="/summary" /></param>
    public ServerCredentials(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        Username = null;
        Password = password.ToCharArray();
        IamAuthConfig = null;
    }

    /// <summary>
    /// Creates server credentials for IAM authentication.
    /// </summary>
    /// <param name="username"><inheritdoc cref="Username" path="/summary" /></param>
    /// <param name="iamConfig"><inheritdoc cref="IamAuthConfig" path="/summary" /></param>
    public ServerCredentials(string username, IamAuthConfig iamConfig)
    {
        ArgumentNullException.ThrowIfNull(username, nameof(username));
        ArgumentNullException.ThrowIfNull(iamConfig, nameof(iamConfig));

        Username = username;
        Password = null;
        IamAuthConfig = iamConfig;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Returns true if this instance is configured for IAM authentication.
    /// </summary>
    public bool IsIamAuth()
    {
        ThrowIfDisposed();
        return IamAuthConfig != null;
    }

    /// <summary>
    /// Returns a string representation with sensitive data omitted.
    /// </summary>
    public override string ToString()
    {
        ThrowIfDisposed();
        return $"ServerCredentials {{ Username = {Username}, IsIamAuth = {IsIamAuth()} }}";
    }

    /// <summary>
    /// Clears sensitive data.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            Username = null;

            if (Password != null)
            {
                Array.Clear(Password);
                Password = null;
            }

            IamAuthConfig = null;

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
