// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for a <see href="https://valkey.io/commands/monitor/">MONITOR</see> connection.
/// </summary>
/// <seealso href="https://valkey.io/commands/monitor/"/>
/// <param name="host">The server host to connect to.</param>
/// <param name="port">The server port to connect to.</param>
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
    public ushort Database { get; private set; } = 0;

    /// <summary>
    /// The username for authentication.
    /// </summary>
    public string? Username
    {
        get { ObjectDisposedException.ThrowIf(_disposed, this); return field; }
        private set;
    } = null;

    /// <summary>
    /// Full override for the library name reported via <c>CLIENT SETINFO LIB-NAME</c> for the
    /// MONITOR connection. Defaults to <c>GlideC#</c>. When set, this replaces the default entirely.
    /// </summary>
    /// <seealso cref="ClientInfoTag"/>
    public string? LibraryName
    {
        get => field ?? Internals.Utils.DefaultLibraryName;
        private set;
    } = null;

    /// <summary>
    /// A tag appended in parentheses to the library name reported via <c>CLIENT SETINFO LIB-NAME</c>
    /// for the MONITOR connection, e.g. <c>GlideC#(my-framework:1.0)</c>, preserving the GLIDE identity.
    /// <para/>
    /// A <see langword="null"/> value is treated as absent: the base library name is reported
    /// unchanged, with no <c>()</c> suffix. Any other value — including empty or whitespace-only —
    /// is passed through as supplied. Character validation is performed entirely by the GLIDE
    /// core, which permits only printable ASCII from <c>!</c> through <c>~</c> (excluding space)
    /// plus at most one matched, non-empty trailing <c>(tag)</c> group; a malformed value fails
    /// client creation with a configuration error.
    /// </summary>
    /// <seealso cref="LibraryName"/>
    public string? ClientInfoTag { get; private set; } = null;

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
    /// <param name="database">The database number.</param>
    /// <returns>This instance for method chaining.</returns>
    public MonitorConfig WithDatabase(ushort database)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Database = database;
        return this;
    }

    /// <summary>
    /// Sets a full override for the library name reported via <c>CLIENT SETINFO LIB-NAME</c>.
    /// </summary>
    /// <param name="libraryName">The library name, or <see langword="null"/> to use the default (<c>GlideC#</c>).</param>
    /// <returns>This instance for method chaining.</returns>
    public MonitorConfig WithLibraryName(string? libraryName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        LibraryName = libraryName;
        return this;
    }

    /// <summary>
    /// Sets a tag appended in parentheses to the library name reported via <c>CLIENT SETINFO LIB-NAME</c>.
    /// </summary>
    /// <param name="clientInfoTag">The tag, or <see langword="null"/> for none.</param>
    /// <returns>This instance for method chaining.</returns>
    public MonitorConfig WithClientInfoTag(string? clientInfoTag)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ClientInfoTag = clientInfoTag;
        return this;
    }

    /// <summary>
    /// Returns a string representation with sensitive data omitted.
    /// </summary>
    public override string ToString()
        => $"MonitorConfig {{ Host = {Host}, Port = {Port}, UseTls = {UseTls}, Database = {Database} }}";

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
