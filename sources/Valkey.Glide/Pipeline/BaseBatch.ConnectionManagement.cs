// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> : IBatchConnectionManagementCommands where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchConnectionManagementCommands.ClientGetNameAsync()" />
    public T ClientGetNameAsync() => AddCmd(Request.ClientGetName());

    /// <inheritdoc cref="IBatchConnectionManagementCommands.ClientIdAsync()" />
    public T ClientIdAsync() => AddCmd(Request.ClientId());

    /// <inheritdoc cref="IBatchConnectionManagementCommands.ClientPauseAsync(TimeSpan)" />
    public T ClientPauseAsync(TimeSpan timeout) => AddCmd(Request.ClientPause(timeout));

    /// <inheritdoc cref="IBatchConnectionManagementCommands.ClientPauseWriteAsync(TimeSpan)" />
    public T ClientPauseWriteAsync(TimeSpan timeout) => AddCmd(Request.ClientPauseWrite(timeout));

    /// <inheritdoc cref="IBatchConnectionManagementCommands.ClientUnpauseAsync()" />
    public T ClientUnpauseAsync() => AddCmd(Request.ClientUnpause());

    /// <inheritdoc cref="IBatchConnectionManagementCommands.Echo(ValkeyValue)" />
    public T Echo(ValkeyValue message) => AddCmd(Request.Echo(message));

    /// <inheritdoc cref="IBatchConnectionManagementCommands.Ping()" />
    public T Ping() => AddCmd(Request.Ping());

    /// <inheritdoc cref="IBatchConnectionManagementCommands.Ping(ValkeyValue)" />
    public T Ping(ValkeyValue message) => AddCmd(Request.Ping(message));

    /// <inheritdoc cref="IBatchConnectionManagementCommands.ResetAsync()" />
    public T ResetAsync() => AddCmd(Request.Reset());

    /// <inheritdoc cref="IBatchConnectionManagementCommands.SelectAsync(long)" />
    public T SelectAsync(long index) => AddCmd(Request.Select(index));

    IBatch IBatchConnectionManagementCommands.ClientGetNameAsync() => ClientGetNameAsync();
    IBatch IBatchConnectionManagementCommands.ClientIdAsync() => ClientIdAsync();
    IBatch IBatchConnectionManagementCommands.ClientPauseAsync(TimeSpan timeout) => ClientPauseAsync(timeout);
    IBatch IBatchConnectionManagementCommands.ClientPauseWriteAsync(TimeSpan timeout) => ClientPauseWriteAsync(timeout);
    IBatch IBatchConnectionManagementCommands.ClientUnpauseAsync() => ClientUnpauseAsync();
    IBatch IBatchConnectionManagementCommands.Echo(ValkeyValue message) => Echo(message);
    IBatch IBatchConnectionManagementCommands.Ping() => Ping();
    IBatch IBatchConnectionManagementCommands.Ping(ValkeyValue message) => Ping(message);
    IBatch IBatchConnectionManagementCommands.ResetAsync() => ResetAsync();
    IBatch IBatchConnectionManagementCommands.SelectAsync(long index) => SelectAsync(index);
}
