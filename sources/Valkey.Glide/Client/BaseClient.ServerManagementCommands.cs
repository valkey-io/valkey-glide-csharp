// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

// TODO #462: Consolidate no-route overloads into BaseClient (glide-core default routing matches).
public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.ConfigGetAsync(IEnumerable{ValkeyValue})"/>
    public abstract Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns);

    /// <inheritdoc cref="IBaseClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})"/>
    public abstract Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters);

    /// <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)"/>
    public abstract Task FlushAllDatabasesAsync(FlushMode mode);

    /// <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)"/>
    public abstract Task FlushDatabaseAsync(FlushMode mode);

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync()"/>
    public abstract Task<long> LatencyResetAsync();

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync(ValkeyValue)"/>
    public abstract Task<long> LatencyResetAsync(ValkeyValue @event);

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync(IEnumerable{ValkeyValue})"/>
    public abstract Task<long> LatencyResetAsync(IEnumerable<ValkeyValue> events);

    // TODO #475: Add parameterless LolwutAsync() here

    /// <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)"/>
    public abstract Task<string> LolwutAsync(LolwutOptions options);

    /// <inheritdoc cref="IBaseClient.SaveAsync()"/>
    public abstract Task SaveAsync();
}
