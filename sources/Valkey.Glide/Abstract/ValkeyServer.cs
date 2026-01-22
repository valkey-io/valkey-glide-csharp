// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Net;

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

internal class ValkeyServer(Database conn, EndPoint endpoint) : IServer
{
    private readonly Database _conn = conn;

    /// <summary>
    /// Run <c>HELLO</c> command.
    /// </summary>
    private Dictionary<GlideString, object> Hello()
        => (Dictionary<GlideString, object>)_conn.CustomCommand(["hello"]).GetAwaiter().GetResult()!;

    public ValkeyResult Execute(string command, params object[] args)
        => ExecuteAsync(command, args).GetAwaiter().GetResult();

    public async Task<ValkeyResult> ExecuteAsync(string command, params object[] args)
        => await ExecuteAsync(command, args.ToList());

    public ValkeyResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        => ExecuteAsync(command, args, flags).GetAwaiter().GetResult();

    public async Task<ValkeyResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        object? res = await _conn.Command(Request.CustomCommand([command, .. args?.Select(a => a.ToString()!) ?? []]), MakeRoute());
        return ValkeyResult.Create(res);
    }

    private Route MakeRoute()
    {
        (string host, ushort port) = Utils.SplitEndpoint(EndPoint);
        return new ByAddressRoute(host, port);
    }

    public EndPoint EndPoint { get; } = endpoint;

    public bool IsConnected => true;

    public Protocol Protocol => (long)Hello()["proto"] == 2 ? Protocol.Resp2 : Protocol.Resp3;

    public Version Version => new(Hello()["version"].ToString()!);

    public ServerType ServerType => Enum.Parse<ServerType>(Hello()["mode"].ToString()!, true);

    public Task<string?> InfoRawAsync(ValkeyValue section = default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        InfoOptions.Section[] sections = section.Type == ValkeyValue.StorageType.Null ? [] :
            [Enum.Parse<InfoOptions.Section>(section.ToString(), true)];

        return _conn
            .Command(Request.Info(sections), MakeRoute())
            .ContinueWith(task => (string?)task.Result);
    }

    public Task<IGrouping<string, KeyValuePair<string, string>>[]> InfoAsync(ValkeyValue section = default, CommandFlags flags = CommandFlags.None)
        => InfoRawAsync(section, flags).ContinueWith(t
            => Utils.ParseInfoResponse(t.Result!).GroupBy(x => x.Item1, x => x.Item2).ToArray());

    public string? InfoRaw(ValkeyValue section = default, CommandFlags flags = CommandFlags.None)
        => InfoRawAsync(section, flags).GetAwaiter().GetResult();

    public async Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.Ping(), MakeRoute());
    }

    public async Task<TimeSpan> PingAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.Ping(message), MakeRoute());
    }

    public async Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.Echo(message), MakeRoute());
    }

    public async Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.ClientGetName(), MakeRoute());
    }

    public async Task<long> ClientIdAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.ClientId(), MakeRoute());
    }

    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.ConfigGetAsync(pattern), MakeRoute());
    }

    public async Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await _conn.Command(Request.ConfigResetStatisticsAsync(), MakeRoute());
    }

    public async Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await _conn.Command(Request.ConfigRewriteAsync(), MakeRoute());
    }

    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await _conn.Command(Request.ConfigSetAsync(setting, value), MakeRoute());
    }

    public async Task<long> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.DatabaseSizeAsync(database), MakeRoute());
    }

    public async Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await _conn.Command(Request.FlushAllDatabasesAsync(), MakeRoute());
    }

    public async Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await _conn.Command(Request.FlushDatabaseAsync(database), MakeRoute());
    }

    public async Task<DateTime> LastSaveAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.LastSaveAsync(), MakeRoute());
    }

    public async Task<DateTime> TimeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.TimeAsync(), MakeRoute());
    }

    public async Task<string> LolwutAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.Command(Request.LolwutAsync(), MakeRoute());
    }

    public async Task<bool> ScriptExistsAsync(string script, CommandFlags flags = CommandFlags.None)
    {
        if (string.IsNullOrEmpty(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        GuardClauses.ThrowIfCommandFlags(flags);

        // Calculate SHA1 hash of the script
        using Script scriptObj = new(script);
        string hash = scriptObj.Hash;

        // Call SCRIPT EXISTS with the hash
        bool[] results = await _conn.Command(Request.ScriptExistsAsync([hash]), MakeRoute());
        return results.Length > 0 && results[0];
    }

    public async Task<bool> ScriptExistsAsync(byte[] sha1, CommandFlags flags = CommandFlags.None)
    {
        if (sha1 == null || sha1.Length == 0)
        {
            throw new ArgumentException("SHA1 hash cannot be null or empty", nameof(sha1));
        }

        GuardClauses.ThrowIfCommandFlags(flags);

        // Convert byte array to hex string
        string hash = BitConverter.ToString(sha1).Replace("-", "").ToLowerInvariant();

        // Call SCRIPT EXISTS with the hash
        bool[] results = await _conn.Command(Request.ScriptExistsAsync([hash]), MakeRoute());
        return results.Length > 0 && results[0];
    }

    public async Task<byte[]> ScriptLoadAsync(string script, CommandFlags flags = CommandFlags.None)
    {
        if (string.IsNullOrEmpty(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        GuardClauses.ThrowIfCommandFlags(flags);

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

    public async Task<LoadedLuaScript> ScriptLoadAsync(LuaScript script, CommandFlags flags = CommandFlags.None)
    {
        if (script == null)
        {
            throw new ArgumentNullException(nameof(script));
        }

        GuardClauses.ThrowIfCommandFlags(flags);

        // Load the executable script
        byte[] hash = await ScriptLoadAsync(script.ExecutableScript, flags);
        return new LoadedLuaScript(script, hash, script.ExecutableScript);
    }

    public async Task ScriptFlushAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await _conn.Command(Request.ScriptFlushAsync(), MakeRoute());
    }
}
