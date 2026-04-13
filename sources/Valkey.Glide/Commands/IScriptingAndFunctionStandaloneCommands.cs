// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Scripting and function commands for standalone clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#scripting">Valkey – Scripting and Function Commands</seealso>
public interface IScriptingAndFunctionStandaloneCommands : IScriptingAndFunctionBaseCommands
{
    // This interface currently has no additional script methods beyond the base interface.
    // Function-related methods have been moved to IGlideClient as they are not in StackExchange.Redis.
}
