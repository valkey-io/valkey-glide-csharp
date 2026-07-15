// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for a <see cref="MonitorClient"/>.
/// </summary>
/// <seealso href="https://valkey.io/commands/monitor/">Valkey commands - MONITOR</seealso>
public sealed class MonitorConfig(string host, ushort port) : IDisposable
{
    #region Private Fields

    private bool _disposed;

    #endregion
    #region Public Properties

    /// <summary>
    /// The server host.
    /// </summary>
    public string Host { get; } = host ?? throw new ArgumentNullException(nameof(host));

    /// <summary>
    /// The server port.
    /// </summary>
    public ushort Port { get; } = port;

    /// <summary>
    /// Whether to use TLS for the connection.
    /// </summary>
    public bool UseTls { get; private set; } = false;

    /// <summary>
    /// The database number to select.
    /// </summary>
    public uint DatabaseId { get; private set; } = 0;

    /// <summary>
    /// The username for authentication.
    /// </summary>
    public string? Username
    {
        get { ObjectDisposedException.ThrowIf(_disposed, this); return field; }
        private set;
    } = null;

    #endregion
    #region Internal Properties

    /// <summary>
    /// The password for authentication.
    /// </summary>
    internal char[]? Password
    {
        get { ObjectDisposedException.ThrowIf(_disposed, this); return field; }
        private set;
    } = null;

    #endregion
    #region Public Methods

    /// <summary>
    /// Enables or disables TLS for the connection.
    /// </summary>
    /// <param name="enable">Whether to enable TLS.</param>
    /// <returns>This instance for method chaining.</returns>
    public MonitorConfig WithTls(bool enable = true)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        UseTls = enable;
        return this;
    }

    /// <summary>
    /// Configures password-only authentication with the server.
    /// </summary>
    /// <param name="password">The password to authenticate with.</param>
    /// <returns>This instance for method chaining.</returns>
    public MonitorConfig WithAuth(string password)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        ClearPassword();

        Username = null;
        Password = password.ToCharArray();

        return this;
    }

    /// <summary>
    /// Configures username and password authentication with the server.
    /// </summary>
    /// <param name="username">The username to authenticate with.</param>
    /// <param name="password">The password to authenticate with.</param>
    /// <returns>This instance for method chaining.</returns>
    public MonitorConfig WithAuth(string username, string password)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(username, nameof(username));
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        ClearPassword();

        Username = username;
        Password = password.ToCharArray();

        return this;
    }

    /// <summary>
    /// Sets the database number to select.
    /// </summary>
    /// <param name="databaseId">The database number.</param>
    /// <returns>This instance for method chaining.</returns>
    public MonitorConfig WithDatabaseId(uint databaseId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        DatabaseId = databaseId;
        return this;
    }

    /// <summary>
    /// Returns a string representation with sensitive data omitted.
    /// </summary>
    public override string ToString()
        => $"MonitorConfig {{ Host = {Host}, Port = {Port}, UseTls = {UseTls}, DatabaseId = {DatabaseId} }}";

    /// <summary>
    /// Clears sensitive data.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            ClearPassword();
            Username = null;
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    #endregion
    #region Private Methods

    private void ClearPassword()
    {
        if (Password is not null)
        {
            Array.Clear(Password);
            Password = null;
        }
    }

    #endregion
}
