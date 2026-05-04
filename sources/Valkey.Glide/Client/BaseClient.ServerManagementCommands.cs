// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})"/>
    public abstract Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters);

    /// <inheritdoc cref="IBaseClient.ConfigGetAsync(IEnumerable{ValkeyValue})"/>
    public abstract Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns);

    /// <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)"/>
    public abstract Task FlushAllDatabasesAsync(FlushMode mode);

    /// <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)"/>
    public abstract Task FlushDatabaseAsync(FlushMode mode);

    /// <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)"/>
    public abstract Task<string> LolwutAsync(LolwutOptions options);
}
