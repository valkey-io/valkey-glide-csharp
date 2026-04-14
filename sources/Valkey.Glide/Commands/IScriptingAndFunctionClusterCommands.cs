// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Scripting commands for cluster clients (StackExchange.Redis compatibility).
/// </summary>
/// <seealso href="https://valkey.io/commands/#scripting">Valkey – Scripting and Function Commands</seealso>
public interface IScriptingAndFunctionClusterCommands : IScriptingAndFunctionBaseCommands
{
    // Script management methods with routing (ScriptInvokeAsync, ScriptExistsAsync, ScriptFlushAsync, ScriptKillAsync)
    // have been moved to IGlideClusterClient as they are not in StackExchange.Redis.
    // Function methods with routing have also been moved to IGlideClusterClient.
}
