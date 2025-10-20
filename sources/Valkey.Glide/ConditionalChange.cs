// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Conditional change options for commands that support conditional updates.
/// </summary>
public enum ConditionalChange
{
    /// <summary>
    /// Only add new elements. Don't update already existing elements.
    /// </summary>
    ONLY_IF_DOES_NOT_EXIST,

    /// <summary>
    /// Only update elements that already exist. Don't add new elements.
    /// </summary>
    ONLY_IF_EXISTS
}
