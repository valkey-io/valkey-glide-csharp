// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide;

internal class Database : GlideClient, IDatabase
{
    public new async Task<string> InfoAsync() => await InfoAsync([]);

    public new async Task<string> InfoAsync(InfoOptions.Section[] sections)
        => IsCluster
            ? await Command(Request.Info(sections), Route.Random)
            : await base.InfoAsync(sections);

    public IBatch CreateBatch(object? asyncState = null)
    {
        GuardClauses.ThrowIfAsyncState(asyncState);
        return new ValkeyBatch(this);
    }

    public ITransaction CreateTransaction(object? asyncState = null)
    {
        GuardClauses.ThrowIfAsyncState(asyncState);
        return new ValkeyTransaction(this);
    }

    internal readonly bool IsCluster;

    protected Database(bool isCluster) { IsCluster = isCluster; }

    public static async Task<Database> Create(BaseClientConfiguration config)
        => await CreateClient(config, () => new Database(config is ClusterClientConfiguration));

    public ValkeyResult Execute(string command, params object[] args)
        => ExecuteAsync(command, args).GetAwaiter().GetResult();

    public ValkeyResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        => ExecuteAsync(command, args, flags).GetAwaiter().GetResult();

    public async Task<ValkeyResult> ExecuteAsync(string command, params object[] args)
        => await ExecuteAsync(command, args.ToList());

    public async Task<ValkeyResult> ExecuteAsync(string command, ICollection<object>? args, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        object? res = await Command(Request.CustomCommand([command, .. args?.Select(a => a.ToString()!) ?? []]));
        return ValkeyResult.Create(res);
    }
}
