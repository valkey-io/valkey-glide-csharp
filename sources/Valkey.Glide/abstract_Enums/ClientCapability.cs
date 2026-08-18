// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Client capabilities.
/// </summary>
/// <seealso href="https://valkey.io/commands/client-capa/" />
/// <seealso href="https://valkey.io/commands/client-kill/" />
/// <seealso href="https://valkey.io/commands/client-list/" />
public enum ClientCapability
{
    /// <summary>
    /// Client can handle redirect messages.
    /// </summary>
    Redirect = 'r',
}
