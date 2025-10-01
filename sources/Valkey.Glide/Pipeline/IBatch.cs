// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

using static Valkey.Glide.Commands.Options.InfoOptions;

namespace Valkey.Glide.Pipeline;

// BaseBatch was split into two types, one for docs, another for the impl. This also ease the testing.
internal interface IBatch : IBatchSetCommands, IBatchStringCommands, IBatchListCommands, IBatchSortedSetCommands, IBatchGenericCommands, IBatchConnectionManagementCommands, IBatchHashCommands, IBatchServerManagementCommands
{
    // inherit all docs except `remarks` section which stores en example (not relevant for batch)
    // and returns section, because we customize it.

    /// <inheritdoc cref="IGenericCommands.CustomCommand(GlideString[])" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericCommands.CustomCommand(GlideString[])" /></returns>
    IBatch CustomCommand(GlideString[] args);

    /// <inheritdoc cref="IServerManagementCommands.InfoAsync()" path="/summary" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.InfoAsync()" /></returns>
    IBatch Info();

    /// <inheritdoc cref="IServerManagementCommands.InfoAsync(Section[])" path="/summary" />
    /// <inheritdoc cref="IServerManagementCommands.InfoAsync(Section[])" path="/param" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.InfoAsync(Section[])" /></returns>
    IBatch Info(Section[] sections);
}
