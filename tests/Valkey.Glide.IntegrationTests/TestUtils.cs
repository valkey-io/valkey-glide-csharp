// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

internal static class TestUtils
{
    private static readonly GlideString[] ClientListCommandArgs = ["CLIENT", "LIST"];

    public static async Task<int> GetConnectionCount(GlideClient client)
    {
        var result = await client.CustomCommand(ClientListCommandArgs);
        return result!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    public static async Task<int> GetConnectionCount(GlideClusterClient client)
    {
        var result = await client.CustomCommand(ClientListCommandArgs, new Route.AllPrimariesRoute());
        return result!.MultiValue.Values.Sum(nodeResult =>
            nodeResult!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
    }
}
