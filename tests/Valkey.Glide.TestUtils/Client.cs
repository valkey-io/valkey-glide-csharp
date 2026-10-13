// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE clients.
/// </summary>
public static class Client
{
    /// <summary>
    /// Creates a client for the given configuration.
    /// </summary>
    /// <param name="config">The client configuration.</param>
    public static async Task<BaseClient> CreateClient(BaseClientConfiguration config)
        => config switch
        {
            StandaloneClientConfiguration standalone => await GlideClient.CreateClient(standalone),
            ClusterClientConfiguration cluster => await GlideClusterClient.CreateClient(cluster),
            _ => throw new ArgumentException($"Unknown configuration type: {config.GetType().Name}")
        };

    /// <summary>
    /// Returns the value of the given <c>CLIENT INFO</c> command field.
    /// For cluster clients, the command is sent to a random node.
    /// </summary>
    /// <param name="client">A client that is connected to the server.</param>
    /// <param name="field">The name of the <c>CLIENT INFO</c> field.</param>
    /// <returns>The value of the field.</returns>
    public static async Task<string> GetClientInfo(BaseClient client, string field)
    {
        GlideString[] infoCommand = ["CLIENT", "INFO"];
        object? result = client is GlideClusterClient clusterClient
            ? (await clusterClient.CustomCommand(infoCommand, Route.Random)).SingleValue
            : await ((GlideClient)client).CustomCommand(infoCommand);

        Dictionary<string, string> fields = result!.ToString()!
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(static pair => pair.Split('=', 2))
            .Where(static parts => parts.Length == 2)
            .ToDictionary(static parts => parts[0], static parts => parts[1]);

        return Assert.Contains(field, fields);
    }

    /// <summary>
    /// Returns the total number of client connections to a server.
    /// </summary>
    /// <param name="client">A client that is connected to the server.</param>
    /// <returns>A task that resolves to the total number of client connections.</returns>
    public static async Task<int> GetConnectionCount(BaseClient client)
    {
        GlideString[] clientListCommandArgs = ["CLIENT", "LIST"];
        if (client is GlideClient standaloneClient)
        {
            object? result = await standaloneClient.CustomCommand(clientListCommandArgs);
            return result!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        }
        else if (client is GlideClusterClient clusterClient)
        {
            ClusterValue<object?> result = await clusterClient.CustomCommand(clientListCommandArgs, new Route.AllPrimariesRoute());
            return result!.MultiValue.Values.Sum(static nodeResult =>
                nodeResult!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
        }
        else
        {
            throw new ArgumentException("Unknown client type.");
        }
    }

    /// <summary>
    /// Returns the value of the given <c>INFO</c> command field.
    /// For cluster clients, the command is sent to a random node.
    /// </summary>
    /// <param name="client">A client that is connected to the server.</param>
    /// <param name="field">The name of the <c>INFO</c> field.</param>
    /// <returns>The value of the field.</returns>
    public static async Task<string> GetInfo(BaseClient client, string field)
    {
        string info = client is GlideClient standaloneClient
            ? await standaloneClient.InfoAsync()
            : (await ((GlideClusterClient)client).InfoAsync(Route.Random)).SingleValue;

        Dictionary<string, string> fields = info
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(static line => !line.StartsWith('#'))
            .Select(static line => line.Split(':', 2))
            .Where(static parts => parts.Length == 2)
            .ToDictionary(static parts => parts[0], static parts => parts[1]);

        return Assert.Contains(field, fields);
    }

    /// <summary>
    /// Returns the Valkey server version.
    /// </summary>
    /// <param name="client">A client that is connected to the server.</param>
    /// <returns>The server version.</returns>
    public static Version GetVersion(BaseClient client)
    {
        string info =
            client is GlideClient standaloneClient
            ? standaloneClient.InfoAsync().GetAwaiter().GetResult()
            : ((GlideClusterClient)client).InfoAsync(Route.Random).GetAwaiter().GetResult().SingleValue;

        string[] lines = info.Split();
        string versionLine = lines.FirstOrDefault(static l => l.Contains("valkey_version")) ?? lines.First(static l => l.Contains("redis_version"));
        return new(versionLine.Split(':')[1]);
    }
}
