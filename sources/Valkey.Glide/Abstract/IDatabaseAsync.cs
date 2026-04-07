// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Describes functionality that is common to both standalone and cluster servers.<br />
/// See also <see cref="GlideClient" /> and <see cref="GlideClusterClient" />.
/// </summary>
// ATTENTION: Methods should only be added to this interface if they are implemented by StackExchange.Redis
// databases but NOT by Valkey GLIDE clients. Methods implemented by both should be added to the corresponding
// Commands interface instead.
public partial interface IDatabaseAsync :
    IRedisAsync,
    IBitmapBaseCommands,
    IConnectionManagementCommands,
    IGeospatialBaseCommands,
    IGenericCommands,
    IGenericBaseCommands,
    IHashBaseCommands,
    IHyperLogLogBaseCommands,
    IListBaseCommands,
    IScriptingAndFunctionBaseCommands,
    IServerManagementCommands,
    ISetBaseCommands,
    ISortedSetBaseCommands,
    IStreamBaseCommands,
    IStringBaseCommands
{
    /// <summary>
    /// Execute an arbitrary command against the server; this is primarily intended for executing modules,
    /// but may also be used to provide access to new features that lack a direct API.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="args">The arguments to pass for the command.</param>
    /// <returns>A dynamic representation of the command's result.</returns>
    /// <remarks>This API should be considered an advanced feature; inappropriate use can be harmful.</remarks>
    Task<ValkeyResult> ExecuteAsync(string command, params object[] args);

    /// <summary>
    /// Execute an arbitrary command against the server; this is primarily intended for executing modules,
    /// but may also be used to provide access to new features that lack a direct API.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <param name="args">The arguments to pass for the command.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <returns>A dynamic representation of the command's result.</returns>
    /// <remarks>This API should be considered an advanced feature; inappropriate use can be harmful.</remarks>
    Task<ValkeyResult> ExecuteAsync(string command, ICollection<object>? args, CommandFlags flags = CommandFlags.None);
}
