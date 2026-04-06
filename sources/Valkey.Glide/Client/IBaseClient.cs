// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Interface for Valkey GLIDE client.
/// </summary>
public interface IBaseClient :
    IDisposable,
    IAsyncDisposable,
    IBitmapCommands,
    IGenericBaseCommands,
    IGeospatialCommands,
    IHashCommands,
    IHyperLogLogCommands,
    IListCommands,
    IPubSubCommands,
    IScriptingAndFunctionBaseCommands,
    ISetCommands,
    ISortedSetCommands,
    IStreamCommands,
    IStringCommands
{
}
