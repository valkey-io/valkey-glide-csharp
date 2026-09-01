// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.ConfigGetAsync(IEnumerable{ValkeyValue})"/>
    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns)
        => await Command(Request.ConfigGet(patterns));

    /// <inheritdoc cref="IBaseClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})"/>
    public async Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters)
        => _ = await Command(Request.ConfigSet(parameters));

    /// <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)"/>
    public async Task FlushAllDatabasesAsync(FlushMode mode)
        => _ = await Command(Request.FlushAllDatabases(mode));

    /// <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)"/>
    public async Task FlushDatabaseAsync(FlushMode mode)
        => _ = await Command(Request.FlushDatabase(mode));

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync()"/>
    public async Task<long> LatencyResetAsync()
        => await Command(Request.LatencyReset([]));

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync(ValkeyValue)"/>
    public async Task<long> LatencyResetAsync(ValkeyValue @event)
        => await Command(Request.LatencyReset([@event]));

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync(IEnumerable{ValkeyValue})"/>
    public async Task<long> LatencyResetAsync(IEnumerable<ValkeyValue> events)
        => await Command(Request.LatencyReset(events));

    /// <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)"/>
    public async Task<string> LolwutAsync(LolwutOptions options)
        => await Command(Request.Lolwut(options));

    /// <inheritdoc cref="IBaseClient.SaveAsync()"/>
    public async Task SaveAsync()
        => _ = await Command(Request.Save());
}
