// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE clients.
/// </summary>
public static class Client
{
    /// <summary>
    /// Asserts that a client is connected by sending a PING command.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertConnected(BaseClient client)
    {
        if (client is GlideClient standaloneClient)
            Assert.True(await standaloneClient.PingAsync() > TimeSpan.Zero);

        else if (client is GlideClusterClient clusterClient)
            Assert.True(await clusterClient.PingAsync() > TimeSpan.Zero);

        else
            Assert.Fail("Unknown client type.");
    }
}
