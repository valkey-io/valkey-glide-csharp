// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Server management commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IServerManagementCommands" />
/// <seealso cref="IServerManagementClusterCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IServerManagementCommands.EchoAsync(ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.EchoAsync(ValkeyValue, Route)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigGetAsync(ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigResetStatisticsAsync()"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ConfigResetStatisticsAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigResetStatisticsAsync(Route)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ConfigResetStatisticsAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigRewriteAsync()"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ConfigRewriteAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigRewriteAsync(Route)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ConfigRewriteAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigSetAsync(ValkeyValue, ValkeyValue, Route)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.LastSaveAsync()"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<DateTime> LastSaveAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.LastSaveAsync(Route)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.TimeAsync()"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<DateTime> TimeAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.TimeAsync(Route)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.LolwutAsync()"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string> LolwutAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.LolwutAsync(Route)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.SelectAsync(long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task SelectAsync(long index, CommandFlags flags);
}
