// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide;

internal partial class Database : GlideClient, IDatabase
{
    internal readonly bool IsCluster;

    protected Database(bool isCluster) { IsCluster = isCluster; }

    public static async Task<Database> Create(BaseClientConfiguration config)
        => await CreateClient(config, () => new Database(config is ClusterClientConfiguration));

    public new async Task<string> InfoAsync() => await InfoAsync([]);

    public new async Task<string> InfoAsync(IEnumerable<InfoOptions.Section> sections)
        => IsCluster
            ? await Command(Request.Info([.. sections]), Route.Random)
            : await base.InfoAsync(sections);

    public IBatch CreateBatch()
        => new ValkeyBatch(this);

    public ITransaction CreateTransaction()
        => new ValkeyTransaction(this);

    public async Task<ValkeyResult> ExecuteAsync(string command, params object[] args)
        => await ExecuteAsync(command, args.ToList());

    public async Task<ValkeyResult> ExecuteAsync(string command, ICollection<object>? args, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var res = await CustomCommand(ToExecuteArgs(command, args));
        return ValkeyResult.Create(res);
    }

    /// <summary>
    /// Converts a StackExchange.Redis <c>ExecuteAsync</c> command name and arguments to binary-safe <see cref="GlideString"/> arguments.
    /// </summary>
    /// <remarks>
    /// Matches StackExchange.Redis: keys, channels, values, and byte arrays keep their raw bytes,
    /// and numbers are formatted using the invariant culture.
    /// </remarks>
    /// <param name="command">The command name.</param>
    /// <param name="args">The command arguments.</param>
    /// <exception cref="InvalidCastException">Thrown if an argument is not a supported type.</exception>
    internal static GlideString[] ToExecuteArgs(string command, IEnumerable<object>? args)
        => [command, .. args?.Select(ToExecuteArg) ?? []];

    /// <summary>
    /// Converts a single <c>ExecuteAsync</c> argument to a binary-safe <see cref="GlideString"/>.
    /// </summary>
    /// <param name="arg">The argument to convert.</param>
    /// <exception cref="InvalidCastException">Thrown if <paramref name="arg"/> is not a supported type.</exception>
    private static GlideString ToExecuteArg(object? arg)
    {
        switch (arg)
        {
            case ValkeyKey key:
                return key;
            case ValkeyChannel channel:
                return new GlideString((byte[]?)channel ?? []);
            default:
                var value = ValkeyValue.TryParse(arg, out bool valid);
                return valid ? value : throw new InvalidCastException($"Unable to parse value: '{arg}'");
        }
    }
}
