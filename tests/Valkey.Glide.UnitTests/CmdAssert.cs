// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Assertions for <see cref="Cmd{R, T}"/> instances.
/// </summary>
internal static class CmdAssert
{
    /// <summary>
    /// Asserts that a command has the expected request type and arguments.
    /// These are what is sent to glide-core, which maps the request type to the command name.
    /// </summary>
    /// <typeparam name="R">The response type.</typeparam>
    /// <typeparam name="T">The converted response type.</typeparam>
    /// <param name="expectedRequest">The expected request type.</param>
    /// <param name="expectedArgs">The expected arguments, excluding the command name.</param>
    /// <param name="cmd">The command to check.</param>
    public static void Equal<R, T>(RequestType expectedRequest, GlideString[] expectedArgs, Cmd<R, T> cmd)
    {
        Assert.Equal(expectedRequest, cmd.Request);
        Assert.Equal(expectedArgs, cmd.Args);
    }
}
