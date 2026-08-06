// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Filter options for client connection commands.
/// </summary>
/// <seealso href="https://valkey.io/commands/client-kill/" />
/// <seealso href="https://valkey.io/commands/client-list/" />
public class ClientFilterOptions
{
    #region Public Properties

    /// <summary>
    /// The client IDs to filter by.
    /// </summary>
    public IReadOnlySet<long> Ids => _ids;

    /// <summary>
    /// The type of client connections to filter by.
    /// </summary>
    public ClientType? Type { get; private set; }

    /// <summary>
    /// The ACL username to filter by.
    /// </summary>
    public string? User { get; private set; }

    /// <summary>
    /// The client address host to filter by.
    /// </summary>
    public string? AddressHost { get; private set; }

    /// <summary>
    /// The client address port to filter by.
    /// </summary>
    public ushort? AddressPort { get; private set; }

    /// <summary>
    /// The server-side local address host to filter by.
    /// </summary>
    public string? LocalAddressHost { get; private set; }

    /// <summary>
    /// The server-side local address port to filter by.
    /// </summary>
    public ushort? LocalAddressPort { get; private set; }

    /// <summary>
    /// Whether to skip the current connection.
    /// </summary>
    public bool? SkipMe { get; private set; }

    /// <summary>
    /// The maximum connection age to filter by.
    /// </summary>
    public TimeSpan? MaxAge => _maxAgeSecs.HasValue ? TimeSpan.FromSeconds(_maxAgeSecs.Value) : null;

    #endregion
    #region Public Methods

    /// <summary>
    /// Filters by client ID.
    /// </summary>
    /// <param name="id">The client ID to filter by.</param>
    public ClientFilterOptions WithId(long id)
    {
        _ = _ids.Add(id);
        return this;
    }

    /// <summary>
    /// Filters by one or more client IDs.
    /// </summary>
    /// <param name="ids">The client IDs to filter by.</param>
    /// <remarks>
    /// <note>Since Valkey 8.1.0.</note>
    /// </remarks>
    public ClientFilterOptions WithIds(IEnumerable<long> ids)
    {
        _ids.UnionWith(ids);
        return this;
    }

    /// <summary>
    /// Filters by client connection type.
    /// </summary>
    /// <param name="type">The type of client connections to filter by.</param>
    public ClientFilterOptions WithType(ClientType type)
    {
        Type = type;
        return this;
    }

    /// <summary>
    /// Filters by authenticated ACL username.
    /// </summary>
    /// <param name="username">The ACL username to filter by.</param>
    public ClientFilterOptions WithUser(string username)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        User = username;
        return this;
    }

    /// <summary>
    /// Filters by client address.
    /// </summary>
    /// <param name="host">The hostname or IP address.</param>
    /// <param name="port">The port number.</param>
    public ClientFilterOptions WithAddress(string host, ushort port)
    {
        ArgumentException.ThrowIfNullOrEmpty(host);
        AddressHost = host;
        AddressPort = port;
        return this;
    }

    /// <summary>
    /// Filters by server-side local (bind) address.
    /// </summary>
    /// <param name="host">The local hostname or IP address.</param>
    /// <param name="port">The local port number.</param>
    public ClientFilterOptions WithLocalAddress(string host, ushort port)
    {
        ArgumentException.ThrowIfNullOrEmpty(host);
        LocalAddressHost = host;
        LocalAddressPort = port;
        return this;
    }

    /// <summary>
    /// Sets whether the current connection should be excluded.
    /// </summary>
    /// <param name="skipMe">If <see langword="true"/>, the calling client is excluded; if <see langword="false"/>, it is included.</param>
    public ClientFilterOptions WithSkipMe(bool skipMe)
    {
        SkipMe = skipMe;
        return this;
    }

    /// <summary>
    /// Filters by maximum connection age.
    /// </summary>
    /// <param name="maxAge">The minimum age of connections to match. Rounded to the nearest second.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxAge"/> is negative.</exception>
    /// <remarks>
    /// <note>Since Valkey 8.0.0.</note>
    /// </remarks>
    public ClientFilterOptions WithMaxAge(TimeSpan maxAge)
    {
        _maxAgeSecs = TimeUtils.ToULongSecs(maxAge, nameof(maxAge));
        return this;
    }

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts to <c>CLIENT KILL</c> command arguments.
    /// </summary>
    internal GlideString[] ToClientKillArgs()
    {
        List<GlideString> args = BuildCommonArgs();

        if (_maxAgeSecs.HasValue)
        {
            args.Add(ValkeyLiterals.MAXAGE);
            args.Add(_maxAgeSecs.Value.ToGlideString());
        }

        return [.. args];
    }

    // TODO #414: Add ToClientListArgs() (MAXAGE in milliseconds) when CLIENT LIST is implemented.

    #endregion
    #region Private Methods

    /// <summary>
    /// Converts command arguments that are common to both <c>CLIENT KILL</c> and <c>CLIENT LIST</c>.
    /// </summary>
    private List<GlideString> BuildCommonArgs()
    {
        List<GlideString> args = [];

        foreach (long id in Ids)
        {
            args.Add(ValkeyLiterals.ID);
            args.Add(id.ToGlideString());
        }

        if (Type is not null)
        {
            args.Add(ValkeyLiterals.TYPE);
            args.Add(ValkeyLiterals.Get(Type.Value));
        }

        if (User is not null)
        {
            args.Add(ValkeyLiterals.USER);
            args.Add(User);
        }

        if (AddressHost is not null)
        {
            args.Add(ValkeyLiterals.ADDR);
            args.Add($"{AddressHost}:{AddressPort}");
        }

        if (LocalAddressHost is not null)
        {
            args.Add(ValkeyLiterals.LADDR);
            args.Add($"{LocalAddressHost}:{LocalAddressPort}");
        }

        if (SkipMe is not null)
        {
            args.Add(ValkeyLiterals.SKIPME);
            args.Add(SkipMe.Value ? ValkeyLiterals.yes : ValkeyLiterals.no);
        }

        return args;
    }

    #endregion
    #region Private Fields

    // Use sorted set to ensure deterministic behaviour.
    private readonly SortedSet<long> _ids = [];

    private ulong? _maxAgeSecs;

    #endregion
}
