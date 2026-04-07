// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// NOTE: Methods should only be added to this interface if they are implemented by both Valkey GLIDE clients
// and StackExchange.Redis databases.

/// <summary>
/// Connection management commands for cluster clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#connection">Valkey – Connection Management Commands</seealso>
public interface IConnectionManagementClusterCommands : IConnectionManagementBaseCommands
{
}
