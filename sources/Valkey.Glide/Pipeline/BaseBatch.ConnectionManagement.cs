// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> : IBatchConnectionManagementCommands where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchConnectionManagementCommands.Ping()" />
    public T Ping() => AddCmd(Request.Ping());

    /// <inheritdoc cref="IBatchConnectionManagementCommands.Ping(ValkeyValue)" />
    public T Ping(ValkeyValue message) => AddCmd(Request.Ping(message));

    /// <inheritdoc cref="IBatchConnectionManagementCommands.Echo(ValkeyValue)" />
    public T Echo(ValkeyValue message) => AddCmd(Request.Echo(message));

    /// <inheritdoc cref="IBatchConnectionManagementCommands.ClientGetNameAsync()" />
    public T ClientGetNameAsync() => AddCmd(Request.ClientGetName());

    /// <inheritdoc cref="IBatchConnectionManagementCommands.ClientIdAsync()" />
    public T ClientIdAsync() => AddCmd(Request.ClientId());

    // <inheritdoc cref="IBatchConnectionManagementCommands.SelectAsync(long)" />
    // TODO: Re-enable when https://github.com/valkey-io/valkey-glide/issues/4691 is resolved.
    // public T SelectAsync(long index) => AddCmd(Request.Select(index));

    IBatch IBatchConnectionManagementCommands.Ping() => Ping();
    IBatch IBatchConnectionManagementCommands.Ping(ValkeyValue message) => Ping(message);
    IBatch IBatchConnectionManagementCommands.Echo(ValkeyValue message) => Echo(message);
    IBatch IBatchConnectionManagementCommands.ClientGetNameAsync() => ClientGetNameAsync();
    IBatch IBatchConnectionManagementCommands.ClientIdAsync() => ClientIdAsync();

    // TODO: Re-enable when https://github.com/valkey-io/valkey-glide/issues/4691 is resolved.
    // IBatch IBatchConnectionManagementCommands.SelectAsync(long index) => SelectAsync(index);
}
