// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;
using System.Net;

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

internal partial class ValkeyServer(Database conn, EndPoint endpoint) : IServer
{
    #region Private Fields

    private readonly Database _conn = conn;

    #endregion Private Fields
    #region Public Properties

    /// <inheritdoc/>
    public EndPoint EndPoint { get; } = endpoint;

    /// <inheritdoc/>
    public bool IsConnected => true;

    /// <inheritdoc/>
    public Protocol Protocol => (long)Hello()["proto"] == 2 ? Protocol.Resp2 : Protocol.Resp3;

    /// <inheritdoc/>
    public Version Version => new(Hello()["version"].ToString()!);

    /// <inheritdoc/>
    public ServerType ServerType => Enum.Parse<ServerType>(Hello()["mode"].ToString()!, true);

    #endregion Public Properties
    #region Public Methods

    /// <summary>
    /// Run <c>HELLO</c> command.
    /// </summary>
    private Dictionary<GlideString, object> Hello()
        => (Dictionary<GlideString, object>)_conn.CustomCommand(["hello"]).GetAwaiter().GetResult()!;

    /// <inheritdoc/>
    public async Task<ValkeyResult> ExecuteAsync(string command, params object[] args)
        => await ExecuteAsync(command, args.ToList());

    /// <inheritdoc/>
    public async Task<ValkeyResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        object? res = await _conn.Command(Request.CustomCommand([command, .. args?.Select(static a => a.ToString()!) ?? []]), MakeRoute());
        return ValkeyResult.Create(res);
    }

    /// <inheritdoc/>
    public Task<string?> InfoRawAsync(ValkeyValue section = default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        InfoOptions.Section[] sections = section.Type == ValkeyValue.StorageType.Null ? [] :
            [Enum.Parse<InfoOptions.Section>(section.ToString(), true)];

        return _conn
            .Command(Request.Info(sections), MakeRoute())
            .ContinueWith(static task => (string?)task.Result);
    }

    /// <inheritdoc/>
    public Task<IGrouping<string, KeyValuePair<string, string>>[]> InfoAsync(ValkeyValue section = default, CommandFlags flags = CommandFlags.None)
        => InfoRawAsync(section, flags).ContinueWith(static t
            => Utils.ParseInfoResponse(t.Result!).GroupBy(static x => x.Item1, static x => x.Item2).ToArray());

    /// <inheritdoc/>
    public async Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        Stopwatch stopwatch = Stopwatch.StartNew();
        _ = await _conn.Command(Request.Ping(), MakeRoute());
        stopwatch.Stop();

        return stopwatch.Elapsed;
    }

    /// <inheritdoc/>
    public async Task<TimeSpan> PingAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        Stopwatch stopwatch = Stopwatch.StartNew();
        _ = await _conn.Command(Request.Ping(message), MakeRoute());
        stopwatch.Stop();

        return stopwatch.Elapsed;
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.Echo(message), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.ClientGetName(), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task<long> ClientIdAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.ClientId(), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task ClientKillAsync(EndPoint endpoint, CommandFlags flags = CommandFlags.None)
        => await ClientKillAsync(id: null, clientType: null, endpoint: endpoint, skipMe: true, flags: flags);

    /// <inheritdoc/>
    public async Task<long> ClientKillAsync(
        long? id = null,
        ClientType? clientType = null,
        EndPoint? endpoint = null,
        bool skipMe = true,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = new ClientFilterOptions().WithSkipMe(skipMe);

        if (id is not null)
        {
            _ = options.WithId(id.Value);
        }

        if (clientType is not null)
        {
            _ = options.WithType(clientType.Value);
        }

        if (endpoint is not null)
        {
            (string host, ushort port) = Utils.SplitEndpoint(endpoint);
            _ = options.WithAddress(host, port);
        }

        return await _conn.Command(Request.ClientKill(options), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task<long> ClientKillAsync(ClientKillFilter filter, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = new ClientFilterOptions();

        if (filter.Id is not null)
        {
            _ = options.WithId(filter.Id.Value);
        }

        if (filter.ClientType is not null)
        {
            _ = options.WithType(filter.ClientType.Value);
        }

        if (filter.Username is not null)
        {
            _ = options.WithUser(filter.Username);
        }

        if (filter.Endpoint is not null)
        {
            (string host, ushort port) = Utils.SplitEndpoint(filter.Endpoint);
            _ = options.WithAddress(host, port);
        }

        if (filter.ServerEndpoint is not null)
        {
            (string host, ushort port) = Utils.SplitEndpoint(filter.ServerEndpoint);
            _ = options.WithLocalAddress(host, port);
        }

        if (filter.SkipMe is not null)
        {
            _ = options.WithSkipMe(filter.SkipMe.Value);
        }

        if (filter.MaxAgeInSeconds is not null)
        {
            _ = options.WithMaxAge(TimeSpan.FromSeconds(filter.MaxAgeInSeconds.Value));
        }

        return await _conn.Command(Request.ClientKill(options), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.ConfigGet(pattern), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await _conn.Command(Request.ConfigResetStatistics(), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await _conn.Command(Request.ConfigRewrite(), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await _conn.Command(Request.ConfigSet(setting, value), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task<long> DatabaseSizeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.DatabaseSize(), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await _conn.Command(Request.FlushAllDatabases(), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task FlushDatabaseAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await _conn.Command(Request.FlushDatabase(), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task<DateTime> LastSaveAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return (await _conn.Command(Request.LastSave(), MakeRoute())).DateTime;
    }

    /// <inheritdoc/>
    public async Task SaveAsync(SaveType type, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = type switch
        {
            SaveType.BackgroundSave => await _conn.Command(Request.BackgroundSave(), MakeRoute()),
            SaveType.BackgroundRewriteAppendOnlyFile => await _conn.Command(Request.BgRewriteAof(), MakeRoute()),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"Unknown SaveType value '{type}'."),
        };
    }

    /// <inheritdoc/>
    public async Task<DateTime> TimeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return (await _conn.Command(Request.Time(), MakeRoute())).DateTime;
    }

    /// <inheritdoc/>
    public async Task<string> LolwutAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.Lolwut(), MakeRoute());
    }

    /// <inheritdoc/>
    public async Task<bool> ScriptExistsAsync(string script, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        if (string.IsNullOrEmpty(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        // Calculate SHA1 hash of the script
        using Script scriptObj = new(script);
        string hash = scriptObj.Hash;

        // Call SCRIPT EXISTS with the hash
        bool[] results = await _conn.Command(Request.ScriptExists([hash]), MakeRoute());
        return results.Length > 0 && results[0];
    }

    /// <inheritdoc/>
    public async Task<bool> ScriptExistsAsync(byte[] sha1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        if (sha1 == null || sha1.Length == 0)
        {
            throw new ArgumentException("SHA1 hash cannot be null or empty", nameof(sha1));
        }

        // Convert byte array to hex string
        string hash = BitConverter.ToString(sha1).Replace("-", "").ToLowerInvariant();

        // Call SCRIPT EXISTS with the hash
        bool[] results = await _conn.Command(Request.ScriptExists([hash]), MakeRoute());
        return results.Length > 0 && results[0];
    }

    /// <inheritdoc/>
    public async Task<byte[]> ScriptLoadAsync(string script, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        if (string.IsNullOrEmpty(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        // Use custom command to call SCRIPT LOAD
        ValkeyResult result = await ExecuteAsync("SCRIPT", ["LOAD", script], flags);
        string? hashString = (string?)result;

        if (string.IsNullOrEmpty(hashString))
        {
            throw new InvalidOperationException("SCRIPT LOAD returned null or empty hash");
        }

        // Convert hex string to byte array
        return Convert.FromHexString(hashString);
    }

    /// <inheritdoc/>
    public async Task<LoadedLuaScript> ScriptLoadAsync(LuaScript script, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

        // Load the executable script
        byte[] hash = await ScriptLoadAsync(script.ExecutableScript, flags);
        return new LoadedLuaScript(script, hash, script.ExecutableScript);
    }

    /// <inheritdoc/>
    public async Task ScriptFlushAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await _conn.Command(Request.ScriptFlush(), MakeRoute());
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<ValkeyKey> KeysAsync(
        ValkeyValue pattern = default,
        int pageSize = 250,
        long cursor = 0,
        int pageOffset = 0,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 0, nameof(pageSize));

        var options = new ScanOptions { MatchPattern = pattern, Count = pageSize };
        return ScanAsync(cursor.ToString(), options).SkipAsync(pageOffset);
    }

    /// <inheritdoc/>
    public async Task ReplicaOfAsync(EndPoint? master, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        if (master is null)
        {
            _ = await _conn.Command(Request.ReplicaOfNoOne(), MakeRoute());
        }
        else
        {
            (string host, ushort port) = Utils.SplitEndpoint(master);
            _ = await _conn.Command(Request.ReplicaOf(host, port), MakeRoute());
        }
    }

    #endregion Public Methods
    #region Private Methods

    private Route MakeRoute()
    {
        (string host, ushort port) = Utils.SplitEndpoint(EndPoint);
        return new ByAddressRoute(host, port);
    }

    private async IAsyncEnumerable<ValkeyKey> ScanAsync(string cursor, ScanOptions options)
    {
        var route = MakeRoute();

        do
        {
            (cursor, ValkeyKey[] keys) = await _conn.Command(Request.Scan(cursor, options), route);
            foreach (ValkeyKey key in keys)
            {
                yield return key;
            }
        } while (cursor != "0");
    }

    #endregion Private Methods
}
