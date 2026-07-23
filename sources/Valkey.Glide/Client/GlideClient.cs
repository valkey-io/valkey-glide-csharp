// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;
using Valkey.Glide.Pipeline;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Pipeline.Options;

namespace Valkey.Glide;

/// <summary>
/// Client used for connection to standalone servers. Use <see cref="CreateClient"/> to request a client.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/client-initialization/">Valkey GLIDE – Client Initialization</seealso>
public partial class GlideClient :
    BaseClient,
    IGlideClient
{
    internal GlideClient() { }

    /// <summary>
    /// Creates a new <see cref="GlideClient" /> instance and establishes a connection to a standalone Valkey server.
    /// </summary>
    /// <param name="config">The configuration options for the client.</param>
    /// <returns>A task that resolves to a connected <see cref="GlideClient" /> instance.</returns>
    public static async Task<GlideClient> CreateClient(StandaloneClientConfiguration config)
        => await CreateClient(config, () => new GlideClient());

    /// <inheritdoc cref="IGenericCommands.CustomCommand(IEnumerable{GlideString})"/>
    public async Task<object?> CustomCommand(IEnumerable<GlideString> args)
        => await Command(Request.CustomCommand([.. args]));

    /// <inheritdoc cref="IGenericCommands.Exec(Batch, bool)"/>
    public async Task<object?[]?> Exec(Batch batch, bool raiseOnError)
        => await Batch(batch, raiseOnError);

    /// <inheritdoc cref="IGenericCommands.Exec(Batch, bool, BatchOptions)"/>
    public async Task<object?[]?> Exec(Batch batch, bool raiseOnError, BatchOptions options)
        => await Batch(batch, raiseOnError, options);

    /// <inheritdoc cref="BaseClient.GetServerVersionAsync()"/>
    protected override async Task<Version> GetServerVersionAsync()
    {
        if (_serverVersion == null)
        {
            try
            {
                var infoResponse = await Command(Request.Info([InfoOptions.Section.SERVER]));
                _serverVersion = ParseServerVersion(infoResponse) ?? DefaultServerVersion;
            }
            catch
            {
                _serverVersion = DefaultServerVersion;
            }
        }

        return _serverVersion;
    }
}
