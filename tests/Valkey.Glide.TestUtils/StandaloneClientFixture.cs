// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Fixture with a standalone server and client for integration tests.
/// </summary>
public class StandaloneClientFixture : StandaloneServerFixture
{
    #region Public Properties

    public GlideClient Client { get; private set; } = null!;

    #endregion
    #region Public Methods

    /// <inheritdoc/>
    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        Client = await Server.CreateStandaloneClientAsync();
    }

    /// <inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        Client?.Dispose();

        await base.DisposeAsync();
    }

    #endregion
}
