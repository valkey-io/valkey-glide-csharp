// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.RegularExpressions;

using static Valkey.Glide.Commands.Options.InfoOptions;
using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Route;

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(AzAffinityTests))]
[CollectionDefinition(DisableParallelization = true)]
public class AzAffinityTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static async Task<GlideClusterClient> CreateAzTestClient(ReadFromStrategy strategy, string az, ConnectionConfiguration.Protocol protocol)
    {
        ClusterClientConfiguration config = TestConfiguration.DefaultClusterClientConfig()
            .WithReadFrom(new(strategy, az))
            .WithRequestTimeout(TimeSpan.FromSeconds(2))
            .WithProtocolVersion(protocol)
            .Build();
        return await GlideClusterClient.CreateClient(config);
    }

    private static async Task<int> GetReplicaCountInCluster(GlideClusterClient client)
    {
        ClusterValue<string> clusterInfo = await client.InfoAsync([Section.REPLICATION], new SlotKeyRoute("_", SlotType.Primary));
        foreach (string line in clusterInfo.SingleValue!.Split('\n'))
        {
            string[] parts = line.Split(':', 2);
            if (parts.Length == 2 && parts[0].Trim() == "connected_slaves")
            {
                return int.Parse(parts[1].Trim());
            }
        }
        throw new Exception("Can't get replica count");
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestRoutingWithAzAffinityStrategyTo1Replica(ConnectionConfiguration.Protocol protocol)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("8.0.0"), "AZ affinity requires server version 8.0.0 or higher");

        using GlideClusterClient configClient = await GlideClusterClient.CreateClient(
            TestConfiguration.DefaultClusterClientConfig().WithProtocolVersion(protocol).Build());
        const string az = "us-east-1a";
        const int nGetCalls = 3;
        string key = Guid.NewGuid().ToString();

        // Reset the availability zone for all nodes
        await configClient.CustomCommand(["config", "set", "availability-zone", ""], AllNodes);
        await configClient.CustomCommand(["config", "resetstat"], AllNodes);
        await configClient.CustomCommand(["config", "set", "availability-zone", az], new SlotKeyRoute(key, SlotType.Replica));

        using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinity, az, protocol);

        for (int i = 0; i < nGetCalls; i++)
        {
            await azTestClient.StringGetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.SERVER, Section.COMMANDSTATS], AllNodes);
        azTestClient.Dispose();

        int changedAzCount = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            Match m = Regex.Match(value, @"cmdstat_get:calls=(\d+)");
            if (value.Contains($"availability_zone:{az}"))
            {
                changedAzCount++;
                if (value.Contains("role:slave") && m.Success)
                {
                    Assert.Equal(nGetCalls, int.Parse(m.Groups[1].Value));
                }
            }
            else
            {
                if (m.Success)
                {
                    Assert.Fail($"Non AZ replica got {m.Groups[1].Value} get calls");
                }
            }
        }

        // Check that the other replicas have no availability zone set
        Assert.Equal(1, changedAzCount);
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestRoutingBySlotToReplicaWithAzAffinityStrategyToAllReplicas(ConnectionConfiguration.Protocol protocol)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("8.0.0"), "AZ affinity requires server version 8.0.0 or higher");

        using GlideClusterClient configClient = await GlideClusterClient.CreateClient(
            TestConfiguration.DefaultClusterClientConfig().WithProtocolVersion(protocol).Build());
        const string az = "us-east-1a";
        string key = Guid.NewGuid().ToString();

        // Reset the availability zone for all nodes
        await configClient.CustomCommand(["config", "set", "availability-zone", ""], AllNodes);
        await configClient.CustomCommand(["config", "resetstat"], AllNodes);

        // Get Replica Count for current cluster
        ClusterValue<string> clusterInfo = await configClient.InfoAsync([Section.REPLICATION], new SlotKeyRoute(key, SlotType.Primary));
        int nReplicas = await GetReplicaCountInCluster(configClient);
        int nCallsPerReplica = 5;
        int nGetCalls = nCallsPerReplica * nReplicas;

        // Setting AZ for all Nodes
        await configClient.CustomCommand(["config", "set", "availability-zone", az], AllNodes);

        using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinity, az, protocol);

        ClusterValue<object?> azGetResult = await azTestClient.CustomCommand(["config", "get", "availability-zone"], AllNodes);
        foreach (object? value in azGetResult.MultiValue.Values)
        {
            object[]? configArray = value as object[];
            if (configArray != null && configArray.Length >= 2)
            {
                Assert.Equal(az, configArray[1]?.ToString());
            }
        }

        // Execute GET commands
        for (int i = 0; i < nGetCalls; i++)
        {
            await azTestClient.StringGetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.ALL], AllNodes);
        azTestClient.Dispose();

        // Check that all replicas have the same number of GET calls
        foreach (string value in infoResult.MultiValue.Values)
        {
            Match m = Regex.Match(value, @"cmdstat_get:calls=(\d+)");
            if (value.Contains("role:slave") && m.Success)
            {
                Assert.Equal(nCallsPerReplica, int.Parse(m.Groups[1].Value));
            }
        }
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestAzAffinityNonExistingAz(ConnectionConfiguration.Protocol protocol)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("8.0.0"), "AZ affinity requires server version 8.0.0 or higher");

        const int nGetCalls = 3;

        using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinity, "non-existing-az", protocol);

        // Reset stats
        await azTestClient.CustomCommand(["config", "resetstat"], AllNodes);

        // Execute GET commands
        for (int i = 0; i < nGetCalls; i++)
        {
            await azTestClient.StringGetAsync("foo");
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.COMMANDSTATS], AllNodes);
        azTestClient.Dispose();

        // We expect the calls to be distributed evenly among the replicas
        foreach (string value in infoResult.MultiValue.Values)
        {
            Match m = Regex.Match(value, @"cmdstat_get:calls=(\d+)");
            if (value.Contains("role:slave") && m.Success)
            {
                Assert.Equal(1, int.Parse(m.Groups[1].Value));
            }
        }
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestAzAffinityReplicasAndPrimaryRoutesToPrimary(ConnectionConfiguration.Protocol protocol)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("8.0.0"), "AZ affinity requires server version 8.0.0 or higher");

        using GlideClusterClient configClient = await GlideClusterClient.CreateClient(
            TestConfiguration.DefaultClusterClientConfig().WithProtocolVersion(protocol).Build());
        const string az = "us-east-1a";
        const string otherAz = "us-east-1b";
        int nReplicas = await GetReplicaCountInCluster(configClient);
        string key = Guid.NewGuid().ToString();

        // Reset stats and set all nodes to otherAz
        await configClient.CustomCommand(["config", "resetstat"], AllNodes);
        await configClient.CustomCommand(["config", "set", "availability-zone", otherAz], AllNodes);

        // Set primary which holds the key to az
        await configClient.CustomCommand(["config", "set", "availability-zone", az], new SlotKeyRoute(key, SlotType.Primary));

        // Verify primary AZ
        ClusterValue<object?> primaryAzResult = await configClient.CustomCommand(["config", "get", "availability-zone"], new SlotKeyRoute(key, SlotType.Primary));
        object[]? primaryConfigArray = primaryAzResult.SingleValue as object[];
        if (primaryConfigArray != null && primaryConfigArray.Length >= 2)
        {
            Assert.Equal(az, primaryConfigArray[1]?.ToString());
        }

        using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinityReplicasAndPrimary, az, protocol);

        // Execute GET commands
        for (int i = 0; i < nReplicas; i++)
        {
            await azTestClient.StringGetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.ALL], AllNodes);
        azTestClient.Dispose();

        // Check that only the primary in the specified AZ handled all GET calls
        foreach (string value in infoResult.MultiValue.Values)
        {
            Match m = Regex.Match(value, @"cmdstat_get:calls=(\d+)");
            if (value.Contains(az))
            {
                if (value.Contains("role:slave") && m.Success)
                {
                    Assert.Fail($"Replica node got GET {m.Groups[1].Value} calls when shouldn't be");
                }
                if (value.Contains("role:master"))
                {
                    if (m.Success)
                    {
                        Assert.Equal(nReplicas, int.Parse(m.Groups[1].Value));
                    }
                    else
                    {
                        Assert.Fail($"Primary node didn't get GET calls");
                    }
                }
            }
        }
    }
}
