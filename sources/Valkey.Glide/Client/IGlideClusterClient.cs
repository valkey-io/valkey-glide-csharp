// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Interface for Valkey GLIDE cluster client.
/// </summary>
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
