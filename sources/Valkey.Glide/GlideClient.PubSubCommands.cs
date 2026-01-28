// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClient
{
    /// <inheritdoc/>
    protected sealed override Route? PubSubRoute => null;

    /// <inheritdoc/>
    protected sealed override Route? PubSubInfoRoute => null;
}
