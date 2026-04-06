// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Interface for Valkey GLIDE cluster client.
/// </summary>
// NOTE: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.
public interface IGlideClusterClient :
    IBaseClient,
    IConnectionManagementClusterCommands,
    IGenericClusterCommands,
    IPubSubClusterCommands,
    IScriptingAndFunctionClusterCommands,
    IServerManagementClusterCommands,
    ITransactionClusterCommands
{
}
