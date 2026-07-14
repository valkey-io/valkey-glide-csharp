// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Commands.Options;
/// <summary>
/// Options for the MIGRATE command.
/// </summary>
/// <remarks>
/// <para>See <a href="https://valkey.io/commands/migrate/">valkey.io</a></para>
/// </remarks>
/// <param name="host">The host address of the destination server.</param>
/// <param name="port">The port number of the destination server.</param>
/// <param name="destinationDb">The database number on the destination server.</param>
/// <param name="timeout">The timeout for the migration.</param>
public sealed class MigrateOptions(string host, ushort port, int destinationDb, TimeSpan timeout) : IDisposable
{
    #region Private Fields

    ///  <summary>
    /// Indicates whether the object has been disposed.
    /// </summary>
    private bool _disposed;

    #endregion
    #region Public Properties

    /// <summary>
    /// The host address of the destination server.
    /// </summary>
    public string Host { get; } = host;

    /// <summary>
    /// The port number of the destination server.
    /// </summary>
    public ushort Port { get; } = port;

    /// <summary>
    /// The database number on the destination server.
    /// </summary>
    public int DestinationDb { get; } = destinationDb;

    /// <summary>
    /// The timeout for the migration.
    /// </summary>
    public TimeSpan Timeout { get; } = timeout;

    /// <summary>
    /// When <see langword="true"/>, do not remove the key from the local instance.
    /// </summary>
    public bool Copy { get; private set; } = false;

    /// <summary>
    /// When <see langword="true"/>, replace existing key on the remote instance.
    /// </summary>
    public bool Replace { get; private set; } = false;

    /// <summary>
    /// The username to authenticate with the destination server.
    /// </summary>
    public string? Username { get; private set; }

    #endregion
    #region Internal Properties

    /// <summary>
    /// The password to authenticate with the destination server.
    /// </summary>
    internal char[]? Password { get; private set; }

    #endregion
    #region Public Methods

    /// <summary>
    /// Configures the migration to retain the specified key(s) on the source server.
    /// </summary>
    /// <returns>This instance for fluent chaining.</returns>
    public MigrateOptions WithCopy()
    {
        Copy = true;
        return this;
    }

    /// <summary>
    /// Configures the migration to overwrite the specified key(s) on the destination server.
    /// </summary>
    /// <returns>This instance for fluent chaining.</returns>
    public MigrateOptions WithReplace()
    {
        Replace = true;
        return this;
    }

    /// <summary>
    /// Configures password-only authentication (AUTH) with the destination server.
    /// </summary>
    /// <param name="password">The password to authenticate with.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public MigrateOptions WithAuth(string password)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        ClearPassword();

        Username = null;
        Password = password.ToCharArray();

        return this;
    }

    /// <summary>
    /// Configures username and password authentication (AUTH2) with the destination server.
    /// </summary>
    /// <param name="username">The username to authenticate with.</param>
    /// <param name="password">The password to authenticate with.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public MigrateOptions WithAuth(string username, string password)
    {
        ArgumentNullException.ThrowIfNull(username, nameof(username));
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        ClearPassword();

        Username = username;
        Password = password.ToCharArray();

        return this;
    }

    /// <summary>
    /// Returns a string representation with sensitive data omitted.
    /// </summary>
    public override string ToString()
        => $"MigrateOptions {{ Host = {Host}, Port = {Port}, DestinationDb = {DestinationDb}, Timeout = {Timeout}, Copy = {Copy}, Replace = {Replace} }}";

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
    #region Internal Methods

    /// <summary>
    /// Converts the options to command arguments.
    /// </summary>
    internal GlideString[] ToArgs(GlideString[] keys)
    {
        if (keys.Length == 0)
        {
            throw new ArgumentException("Keys must not be empty.", nameof(keys));
        }

        bool isMultiKey = keys.Length > 1;

        List<GlideString> args =
        [
            Host,
            Port.ToGlideString(),
            isMultiKey ? "" : keys.First(),
            DestinationDb.ToGlideString(),
            ToMilliseconds(Timeout).ToGlideString()
        ];

        if (Copy)
        {
            args.Add(ValkeyLiterals.COPY);
        }

        if (Replace)
        {
            args.Add(ValkeyLiterals.REPLACE);
        }

        if (Username is not null && Password is not null)
        {
            args.Add(ValkeyLiterals.AUTH2);
            args.Add(Username);
            args.Add(new string(Password));
        }
        else if (Password is not null)
        {
            args.Add(ValkeyLiterals.AUTH);
            args.Add(new string(Password));
        }

        if (isMultiKey)
        {
            args.Add(ValkeyLiterals.KEYS);
            args.AddRange(keys);
        }

        return [.. args];
    }

    #endregion
}
