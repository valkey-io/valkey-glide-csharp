// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Minimal interface matching StackExchange.Redis <c>IRedisAsync</c> for connection-level operations.
/// </summary>
public interface IRedisAsync
{
    /// <summary>
    /// Ping the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <returns>The observed latency.</returns>
    Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None);
}
