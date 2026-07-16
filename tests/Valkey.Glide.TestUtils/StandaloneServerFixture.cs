// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Fixture with a standalone server only for integration tests.
/// </summary>
public class StandaloneServerFixture : IAsyncLifetime
{
    #region Public Properties

    public StandaloneServer Server { get; private set; } = null!;

    #endregion
    #region Public Methods

    /// <inheritdoc/>
    public virtual ValueTask InitializeAsync()
    {
        Server = new();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual ValueTask DisposeAsync()
    {
        Server.Dispose();
        return ValueTask.CompletedTask;
    }

    #endregion
}
