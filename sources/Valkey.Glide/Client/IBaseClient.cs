// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Interface for Valkey GLIDE client.
/// </summary>
// NOTE: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.
public partial interface IBaseClient :
    IDisposable,
    IAsyncDisposable,
    IBitmapBaseCommands,
    IGenericBaseCommands,
    IGeospatialBaseCommands,
    IHashBaseCommands,
    IHyperLogLogBaseCommands,
    IListBaseCommands,
    IPubSubBaseCommands,
    IScriptingAndFunctionBaseCommands,
    ISetBaseCommands,
    ISortedSetBaseCommands,
    IStreamBaseCommands,
    IStringBaseCommands
{
    /// <summary>
    /// Gets the name of the current connection.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname"/>
    /// <returns>
    /// The name of the client connection as a <see cref="ValkeyValue"/>.
    /// If no name is assigned, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ClientGetNameAsync();
    /// Console.WriteLine($"Connection name: {result}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ClientGetNameAsync();
}
