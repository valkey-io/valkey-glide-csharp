// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

internal static class StreamTrimModeExtensions
{
    internal static GlideString? ToKeyword(this StreamTrimMode trimMode)
        => trimMode switch
        {
            StreamTrimMode.KeepReferences => null,
            StreamTrimMode.DeleteReferences => ValkeyLiterals.StreamDeleteReferencesTrim.ToGlideString(),
            StreamTrimMode.Acknowledged => ValkeyLiterals.StreamAcknowledgedTrim.ToGlideString(),
            _ => throw new ArgumentOutOfRangeException(nameof(trimMode), trimMode, null)
        };
}
