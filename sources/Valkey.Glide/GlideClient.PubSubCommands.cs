// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

public partial class GlideClient
{
    /// <inheritdoc/>
    /// No pub/sub routing for non-cluster clients.
    protected sealed override Route? PubSubRoute => null;

    /// <inheritdoc/>
    /// No pub/sub info routing for non-cluster clients.
    protected sealed override Route? PubSubInfoRoute => null;
}
