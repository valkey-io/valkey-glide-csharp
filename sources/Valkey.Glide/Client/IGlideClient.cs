// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Interface for Valkey GLIDE standalone client.
/// </summary>
public interface IGlideClient :
    IBaseClient,
    IConnectionManagementCommands,
    IGenericCommands,
    IScriptingAndFunctionStandaloneCommands,
    IServerManagementCommands
{
}
