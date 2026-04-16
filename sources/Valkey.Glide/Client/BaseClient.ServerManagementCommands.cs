// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public abstract Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters);

    /// <inheritdoc/>
    public abstract Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns);

    /// <inheritdoc/>
    public abstract Task FlushAllDatabasesAsync(FlushMode mode);

    /// <inheritdoc/>
    public abstract Task FlushDatabaseAsync(FlushMode mode);

    /// <inheritdoc/>
    public abstract Task<string> LolwutAsync(LolwutOptions options);
}
