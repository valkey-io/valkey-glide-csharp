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
    /// Includes clients with the specified connection type.
    /// </summary>
    public ClientType? Type { get; private set; }

    /// <summary>
    /// Excludes clients with the specified connection type.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public ClientType? NotType { get; private set; }

    /// <summary>
    /// Includes clients with the specified client IDs.
    /// </summary>
    public IReadOnlySet<long> Ids => _ids;

    /// <summary>
    /// Excludes clients with the specified client IDs.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public IReadOnlySet<long> NotIds => _notIds;

    /// <summary>
    /// Includes clients with the specified ACL username.
    /// </summary>
    public string? User { get; private set; }

    /// <summary>
    /// Excludes clients with the specified ACL username.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? NotUser { get; private set; }

    /// <summary>
    /// Includes clients with the specified address.
    /// </summary>
    public (string Host, ushort Port)? Address { get; private set; }

    /// <summary>
    /// Excludes clients with the specified address.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public (string Host, ushort Port)? NotAddress { get; private set; }

    /// <summary>
    /// Includes clients with the specified local address.
    /// </summary>
    public (string Host, ushort Port)? LocalAddress { get; private set; }

    /// <summary>
    /// Excludes clients with the specified local address.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public (string Host, ushort Port)? NotLocalAddress { get; private set; }

    /// <summary>
    /// Whether to skip the current connection.
    /// </summary>
    public bool? SkipMe { get; private set; }

    /// <summary>
    /// Includes clients older than the specified age.
    /// </summary>
    /// <remarks>Since Valkey 8.0.0.</remarks>
    public TimeSpan? MaxAge => _maxAgeSecs.HasValue ? TimeSpan.FromSeconds(_maxAgeSecs.Value) : null;

    /// <summary>
    /// Includes clients with the specified name.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? Name { get; private set; }

    /// <summary>
    /// Excludes clients with the specified name.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? NotName { get; private set; }

    /// <summary>
    /// Includes clients that have been idle for at least the specified time.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public TimeSpan? Idle => _idleSecs.HasValue ? TimeSpan.FromSeconds(_idleSecs.Value) : null;

    /// <summary>
    /// Includes clients with the specified flags.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public IReadOnlySet<ClientFlag> Flags => _flags;

    /// <summary>
    /// Excludes clients with the specified flags.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public IReadOnlySet<ClientFlag> NotFlags => _notFlags;

    /// <summary>
    /// Includes clients with the specified library name.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? LibraryName { get; private set; }

    /// <summary>
    /// Excludes clients with the specified library name.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? NotLibraryName { get; private set; }

    /// <summary>
    /// Includes clients with the specified library version.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? LibraryVersion { get; private set; }

    /// <summary>
    /// Excludes clients with the specified library version.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? NotLibraryVersion { get; private set; }

    /// <summary>
    /// Includes clients with the specified database ID.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public ushort? DatabaseId { get; private set; }

    /// <summary>
    /// Excludes clients with the specified database ID.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public ushort? NotDatabaseId { get; private set; }

    /// <summary>
    /// Includes clients with the specified capabilities.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public IReadOnlySet<ClientCapability> Capabilities => _capabilities;

    /// <summary>
    /// Excludes clients with the specified capabilities.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public IReadOnlySet<ClientCapability> NotCapabilities => _notCapabilities;

    /// <summary>
    /// Includes clients with the specified IP address.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Excludes clients with the specified IP address.
    /// </summary>
    /// <remarks>Since Valkey 9.0.0.</remarks>
    public string? NotIpAddress { get; private set; }

    #endregion
    #region Public Methods

    /// <inheritdoc cref="Type" />
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithType(ClientType type)
    {
        Type = type;
        return this;
    }

    /// <inheritdoc cref="NotType" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutType(ClientType type)
    {
        NotType = type;
        return this;
    }

    /// <inheritdoc cref="Ids" />
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithId(long id)
    {
        _ids.Clear();
        _ = _ids.Add(id);
        return this;
    }

    /// <inheritdoc cref="Ids" />
    /// <remarks>Since Valkey 8.1.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithIds(IEnumerable<long> ids)
    {
        _ids.Clear();
        _ids.UnionWith(ids);
        return this;
    }

    /// <inheritdoc cref="NotIds" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutId(long id)
    {
        _notIds.Clear();
        _ = _notIds.Add(id);
        return this;
    }

    /// <inheritdoc cref="NotIds" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutIds(IEnumerable<long> ids)
    {
        _notIds.Clear();
        _notIds.UnionWith(ids);
        return this;
    }

    /// <inheritdoc cref="User" />
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithUser(string username)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        User = username;
        return this;
    }

    /// <inheritdoc cref="NotUser" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutUser(string username)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        NotUser = username;
        return this;
    }

    /// <inheritdoc cref="Address" />
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithAddress(string host, ushort port)
    {
        ArgumentException.ThrowIfNullOrEmpty(host);
        Address = (host, port);
        return this;
    }

    /// <inheritdoc cref="NotAddress" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutAddress(string host, ushort port)
    {
        ArgumentException.ThrowIfNullOrEmpty(host);
        NotAddress = (host, port);
        return this;
    }

    /// <inheritdoc cref="LocalAddress" />
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithLocalAddress(string host, ushort port)
    {
        ArgumentException.ThrowIfNullOrEmpty(host);
        LocalAddress = (host, port);
        return this;
    }

    /// <inheritdoc cref="NotLocalAddress" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutLocalAddress(string host, ushort port)
    {
        ArgumentException.ThrowIfNullOrEmpty(host);
        NotLocalAddress = (host, port);
        return this;
    }

    /// <inheritdoc cref="SkipMe" />
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithSkipMe(bool skipMe)
    {
        SkipMe = skipMe;
        return this;
    }

    /// <inheritdoc cref="MaxAge" />
    /// <remarks>Since Valkey 8.0.0.</remarks>
    /// <param name="maxAge">The maximum connection age. Rounded to the nearest second.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxAge"/> is not positive.</exception>
    public ClientFilterOptions WithMaxAge(TimeSpan maxAge)
    {
        _maxAgeSecs = TimeUtils.ToPositiveULongSecs(maxAge, nameof(maxAge));
        return this;
    }

    /// <inheritdoc cref="Name" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
        return this;
    }

    /// <inheritdoc cref="NotName" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        NotName = name;
        return this;
    }

    /// <inheritdoc cref="Idle" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <param name="idle">The minimum idle time of connections to match. Rounded to the nearest second.</param>
    /// <returns>This instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="idle"/> is not positive.</exception>
    public ClientFilterOptions WithIdle(TimeSpan idle)
    {
        _idleSecs = TimeUtils.ToPositiveULongSecs(idle, nameof(idle));
        return this;
    }

    /// <inheritdoc cref="Flags" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithFlag(ClientFlag flag)
    {
        _flags.Clear();
        _ = _flags.Add(flag);
        return this;
    }

    /// <inheritdoc cref="Flags" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithFlag(char flag)
    {
        _flags.Clear();
        _ = _flags.Add((ClientFlag)flag);
        return this;
    }

    /// <inheritdoc cref="Flags" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithFlags(IEnumerable<ClientFlag> flags)
    {
        _flags.Clear();
        _flags.UnionWith(flags);
        return this;
    }

    /// <inheritdoc cref="Flags" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithFlags(string flags)
    {
        _flags.Clear();
        _flags.UnionWith(flags.Select(c => (ClientFlag)c));
        return this;
    }

    /// <inheritdoc cref="NotFlags" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutFlag(ClientFlag flag)
    {
        _notFlags.Clear();
        _ = _notFlags.Add(flag);
        return this;
    }

    /// <inheritdoc cref="NotFlags" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutFlag(char flag)
    {
        _notFlags.Clear();
        _ = _notFlags.Add((ClientFlag)flag);
        return this;
    }

    /// <inheritdoc cref="NotFlags" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutFlags(IEnumerable<ClientFlag> flags)
    {
        _notFlags.Clear();
        _notFlags.UnionWith(flags);
        return this;
    }

    /// <inheritdoc cref="NotFlags" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutFlags(string flags)
    {
        _notFlags.Clear();
        _notFlags.UnionWith(flags.Select(c => (ClientFlag)c));
        return this;
    }

    /// <inheritdoc cref="LibraryName" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithLibraryName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        LibraryName = name;
        return this;
    }

    /// <inheritdoc cref="NotLibraryName" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutLibraryName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        NotLibraryName = name;
        return this;
    }

    /// <inheritdoc cref="LibraryVersion" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithLibraryVersion(string version)
    {
        ArgumentException.ThrowIfNullOrEmpty(version);
        LibraryVersion = version;
        return this;
    }

    /// <inheritdoc cref="NotLibraryVersion" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutLibraryVersion(string version)
    {
        ArgumentException.ThrowIfNullOrEmpty(version);
        NotLibraryVersion = version;
        return this;
    }

    /// <inheritdoc cref="DatabaseId" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithDatabaseId(ushort id)
    {
        DatabaseId = id;
        return this;
    }

    /// <inheritdoc cref="NotDatabaseId" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutDatabaseId(ushort id)
    {
        NotDatabaseId = id;
        return this;
    }

    /// <inheritdoc cref="Capabilities" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithCapability(ClientCapability capability)
    {
        _capabilities.Clear();
        _ = _capabilities.Add(capability);
        return this;
    }

    /// <inheritdoc cref="NotCapabilities" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutCapability(ClientCapability capability)
    {
        _notCapabilities.Clear();
        _ = _notCapabilities.Add(capability);
        return this;
    }

    /// <inheritdoc cref="IpAddress" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithIpAddress(string address)
    {
        ArgumentException.ThrowIfNullOrEmpty(address);
        IpAddress = address;
        return this;
    }

    /// <inheritdoc cref="NotIpAddress" />
    /// <remarks>Since Valkey 9.0.0.</remarks>
    /// <returns>This instance for method chaining.</returns>
    public ClientFilterOptions WithoutIpAddress(string address)
    {
        ArgumentException.ThrowIfNullOrEmpty(address);
        NotIpAddress = address;
        return this;
    }

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (Type is not null)
        {
            args.Add(ValkeyLiterals.TYPE);
            args.Add(ValkeyLiterals.Get(Type.Value));
        }
        if (NotType is not null)
        {
            args.Add(ValkeyLiterals.NOT_TYPE);
            args.Add(ValkeyLiterals.Get(NotType.Value));
        }

        if (_ids.Count > 0)
        {
            args.Add(ValkeyLiterals.ID);
            args.AddRange(_ids.Select(id => id.ToGlideString()));
        }
        if (_notIds.Count > 0)
        {
            args.Add(ValkeyLiterals.NOT_ID);
            args.AddRange(_notIds.Select(id => id.ToGlideString()));
        }

        if (User is not null)
        {
            args.Add(ValkeyLiterals.USER);
            args.Add(User);
        }
        if (NotUser is not null)
        {
            args.Add(ValkeyLiterals.NOT_USER);
            args.Add(NotUser);
        }

        if (Address is not null)
        {
            args.Add(ValkeyLiterals.ADDR);
            args.Add(Utils.FormatAddress(Address.Value.Host, Address.Value.Port));
        }
        if (NotAddress is not null)
        {
            args.Add(ValkeyLiterals.NOT_ADDR);
            args.Add(Utils.FormatAddress(NotAddress.Value.Host, NotAddress.Value.Port));
        }

        if (LocalAddress is not null)
        {
            args.Add(ValkeyLiterals.LADDR);
            args.Add(Utils.FormatAddress(LocalAddress.Value.Host, LocalAddress.Value.Port));
        }
        if (NotLocalAddress is not null)
        {
            args.Add(ValkeyLiterals.NOT_LADDR);
            args.Add(Utils.FormatAddress(NotLocalAddress.Value.Host, NotLocalAddress.Value.Port));
        }

        if (SkipMe is not null)
        {
            args.Add(ValkeyLiterals.SKIPME);
            args.Add(SkipMe.Value ? ValkeyLiterals.yes : ValkeyLiterals.no);
        }

        if (_maxAgeSecs.HasValue)
        {
            args.Add(ValkeyLiterals.MAXAGE);
            args.Add(_maxAgeSecs.Value.ToGlideString());
        }

        if (Name is not null)
        {
            args.Add(ValkeyLiterals.NAME);
            args.Add(Name);
        }
        if (NotName is not null)
        {
            args.Add(ValkeyLiterals.NOT_NAME);
            args.Add(NotName);
        }

        if (_idleSecs.HasValue)
        {
            args.Add(ValkeyLiterals.IDLE);
            args.Add(_idleSecs.Value.ToGlideString());
        }

        if (_flags.Count > 0)
        {
            args.Add(ValkeyLiterals.FLAGS);
            args.Add(new string([.. _flags.Select(f => (char)f)]));
        }
        if (_notFlags.Count > 0)
        {
            args.Add(ValkeyLiterals.NOT_FLAGS);
            args.Add(new string([.. _notFlags.Select(f => (char)f)]));
        }

        if (LibraryName is not null)
        {
            args.Add(ValkeyLiterals.LIB_NAME);
            args.Add(LibraryName);
        }
        if (NotLibraryName is not null)
        {
            args.Add(ValkeyLiterals.NOT_LIB_NAME);
            args.Add(NotLibraryName);
        }

        if (LibraryVersion is not null)
        {
            args.Add(ValkeyLiterals.LIB_VER);
            args.Add(LibraryVersion);
        }
        if (NotLibraryVersion is not null)
        {
            args.Add(ValkeyLiterals.NOT_LIB_VER);
            args.Add(NotLibraryVersion);
        }

        if (DatabaseId is not null)
        {
            args.Add(ValkeyLiterals.DB);
            args.Add(DatabaseId.Value.ToGlideString());
        }
        if (NotDatabaseId is not null)
        {
            args.Add(ValkeyLiterals.NOT_DB);
            args.Add(NotDatabaseId.Value.ToGlideString());
        }

        if (_capabilities.Count > 0)
        {
            args.Add(ValkeyLiterals.CAPA);
            args.Add(new string([.. _capabilities.Select(c => (char)c)]));
        }
        if (_notCapabilities.Count > 0)
        {
            args.Add(ValkeyLiterals.NOT_CAPA);
            args.Add(new string([.. _notCapabilities.Select(c => (char)c)]));
        }

        if (IpAddress is not null)
        {
            args.Add(ValkeyLiterals.IP);
            args.Add(IpAddress);
        }
        if (NotIpAddress is not null)
        {
            args.Add(ValkeyLiterals.NOT_IP);
            args.Add(NotIpAddress);
        }

        return [.. args];
    }

    #endregion
    #region Private Fields

    // Use sorted sets to ensure deterministic behaviour.
    private readonly SortedSet<long> _ids = [];
    private readonly SortedSet<long> _notIds = [];
    private readonly SortedSet<ClientFlag> _flags = [];
    private readonly SortedSet<ClientFlag> _notFlags = [];
    private readonly SortedSet<ClientCapability> _capabilities = [];
    private readonly SortedSet<ClientCapability> _notCapabilities = [];

    private ulong? _maxAgeSecs;
    private ulong? _idleSecs;

    #endregion
}
