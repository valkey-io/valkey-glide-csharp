// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <summary>
/// Interface for Valkey GLIDE standalone client.
/// </summary>
public partial interface IGlideClient :
    IBaseClient,
    IGenericCommands,
    IScriptingAndFunctionStandaloneCommands,
    IServerManagementCommands
{
}
