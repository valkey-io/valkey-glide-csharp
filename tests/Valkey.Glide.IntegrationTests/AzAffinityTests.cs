// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text.RegularExpressions;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.Commands.Options.InfoOptions;
using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Route;

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(AzAffinityTests))]
[CollectionDefinition(DisableParallelization = true)]
public class AzAffinityTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static readonly Regex GetCallsRegex = new(@"cmdstat_get:calls=(\d+)", RegexOptions.Compiled);

    private static async Task<GlideClusterClient> CreateAzTestClient(ReadFromStrategy strategy, string az, ConnectionConfiguration.Protocol protocol)
    {
        ClusterClientConfiguration config = TestConfiguration.DefaultClusterClientConfig()
            .WithReadFrom(new(strategy, az))
            .WithRequestTimeout(TimeSpan.FromSeconds(2))
            .WithProtocolVersion(protocol)
            .Build();
        return await GlideClusterClient.CreateClient(config);
    }

    private static async Task<GlideClusterClient> CreateConfigClient(ConnectionConfiguration.Protocol protocol)
        => await GlideClusterClient.CreateClient(
            TestConfiguration.DefaultClusterClientConfig().WithProtocolVersion(protocol).Build());

    /// <summary>Sets the <c>availability-zone</c> config on the nodes selected by <paramref name="route"/>.</summary>
    private static async Task SetAvailabilityZone(GlideClusterClient client, string az, Route route)
        => _ = await client.CustomCommand(["config", "set", "availability-zone", az], route);

    /// <summary>Resets command stats on all nodes.</summary>
    private static async Task ResetStats(GlideClusterClient client)
        => _ = await client.CustomCommand(["config", "resetstat"], AllNodes);

    private static async Task<int> GetReplicaCountInCluster(GlideClusterClient client)
    {
        ClusterValue<string> clusterInfo = await client.InfoAsync([Section.REPLICATION], new SlotKeyRoute("_", SlotType.Primary));
        foreach (string line in clusterInfo.SingleValue.Split('\n'))
        {
            string[] parts = line.Split(':', 2);
            if (parts.Length == 2 && parts[0].Trim() == "connected_slaves")
            {
                return int.Parse(parts[1].Trim());
            }
        }

        throw new Exception("Can't get replica count");
    }

    /// <summary>Parses the <c>cmdstat_get:calls</c> value from a node's INFO output, or 0 if absent.</summary>
    private static int GetCalls(string infoValue)
    {
        Match m = GetCallsRegex.Match(infoValue);
        return m.Success ? int.Parse(m.Groups[1].Value) : 0;
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestRoutingWithAzAffinityStrategyTo1Replica(ConnectionConfiguration.Protocol protocol)
    {
        Skip.IfAzAffinityNotSupported();

        await using GlideClusterClient configClient = await CreateConfigClient(protocol);
        string az = Data.AvailabilityZone;
        const int nGetCalls = 3;
        string key = Guid.NewGuid().ToString();

        // Reset the availability zone for all nodes
        await SetAvailabilityZone(configClient, "", AllNodes);
        await ResetStats(configClient);
        await SetAvailabilityZone(configClient, az, new SlotKeyRoute(key, SlotType.Replica));

        await using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinity, az, protocol);

        for (int i = 0; i < nGetCalls; i++)
        {
            _ = await azTestClient.GetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.SERVER, Section.COMMANDSTATS], AllNodes);

        int changedAzCount = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            int calls = GetCalls(value);
            if (value.Contains($"availability_zone:{az}"))
            {
                changedAzCount++;
                if (value.Contains("role:slave") && calls > 0)
                {
                    Assert.Equal(nGetCalls, calls);
                }
            }
            else if (calls > 0)
            {
                Assert.Fail($"Non AZ replica got {calls} get calls");
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
        Skip.IfAzAffinityNotSupported();

        await using GlideClusterClient configClient = await CreateConfigClient(protocol);
        string az = Data.AvailabilityZone;
        string key = Guid.NewGuid().ToString();

        // Reset the availability zone for all nodes
        await SetAvailabilityZone(configClient, "", AllNodes);
        await ResetStats(configClient);

        // Get Replica Count for current cluster
        int nReplicas = await GetReplicaCountInCluster(configClient);
        int nCallsPerReplica = 5;
        int nGetCalls = nCallsPerReplica * nReplicas;

        // Setting AZ for all Nodes
        await SetAvailabilityZone(configClient, az, AllNodes);

        await using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinity, az, protocol);

        ClusterValue<object?> azGetResult = await azTestClient.CustomCommand(["config", "get", "availability-zone"], AllNodes);
        foreach (object? value in azGetResult.MultiValue.Values)
        {
            if (value is object[] configArray && configArray.Length >= 2)
            {
                Assert.Equal(az, configArray[1]?.ToString());
            }
        }

        // Execute GET commands
        for (int i = 0; i < nGetCalls; i++)
        {
            _ = await azTestClient.GetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.ALL], AllNodes);

        // Check that all replicas have the same number of GET calls
        foreach (string value in infoResult.MultiValue.Values)
        {
            if (value.Contains("role:slave") && GetCalls(value) > 0)
            {
                Assert.Equal(nCallsPerReplica, GetCalls(value));
            }
        }
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestAzAffinityNonExistingAz(ConnectionConfiguration.Protocol protocol)
    {
        Skip.IfAzAffinityNotSupported();

        const int nGetCalls = 3;

        await using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinity, Data.NonExistingAvailabilityZone, protocol);

        // Reset stats
        await ResetStats(azTestClient);

        // Execute GET commands
        for (int i = 0; i < nGetCalls; i++)
        {
            _ = await azTestClient.GetAsync("foo");
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.COMMANDSTATS], AllNodes);

        // We expect the calls to be distributed evenly among the replicas
        foreach (string value in infoResult.MultiValue.Values)
        {
            if (value.Contains("role:slave") && GetCalls(value) > 0)
            {
                Assert.Equal(1, GetCalls(value));
            }
        }
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestAzAffinityReplicasAndPrimaryRoutesToPrimary(ConnectionConfiguration.Protocol protocol)
    {
        Skip.IfAzAffinityNotSupported();

        await using GlideClusterClient configClient = await CreateConfigClient(protocol);
        string az = Data.AvailabilityZone;
        string otherAz = Data.OtherAvailabilityZone;
        int nReplicas = await GetReplicaCountInCluster(configClient);
        string key = Guid.NewGuid().ToString();

        // Reset stats and set all nodes to otherAz
        await ResetStats(configClient);
        await SetAvailabilityZone(configClient, otherAz, AllNodes);

        // Set primary which holds the key to az
        await SetAvailabilityZone(configClient, az, new SlotKeyRoute(key, SlotType.Primary));

        // Verify primary AZ
        ClusterValue<object?> primaryAzResult = await configClient.CustomCommand(["config", "get", "availability-zone"], new SlotKeyRoute(key, SlotType.Primary));
        if (primaryAzResult.SingleValue is object[] primaryConfigArray && primaryConfigArray.Length >= 2)
        {
            Assert.Equal(az, primaryConfigArray[1]?.ToString());
        }

        await using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinityReplicasAndPrimary, az, protocol);

        // Execute GET commands
        for (int i = 0; i < nReplicas; i++)
        {
            _ = await azTestClient.GetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.ALL], AllNodes);

        // Check that only the primary in the specified AZ handled all GET calls
        foreach (string value in infoResult.MultiValue.Values)
        {
            int calls = GetCalls(value);
            if (value.Contains(az))
            {
                if (value.Contains("role:slave") && calls > 0)
                {
                    Assert.Fail($"Replica node got GET {calls} calls when shouldn't be");
                }

                if (value.Contains("role:master"))
                {
                    if (calls > 0)
                    {
                        Assert.Equal(nReplicas, calls);
                    }
                    else
                    {
                        Assert.Fail("Primary node didn't get GET calls");
                    }
                }
            }
        }
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestAzAffinityAllNodesSplitsBetweenPrimaryAndReplica(ConnectionConfiguration.Protocol protocol)
    {
        Skip.IfAzAffinityNotSupported();

        await using GlideClusterClient configClient = await CreateConfigClient(protocol);
        string az = Data.AvailabilityZone;
        string otherAz = Data.OtherAvailabilityZone;
        const int nGetCalls = 4;
        const int nodesInSameAz = 2; // one primary + one replica
        int callsPerNode = nGetCalls / nodesInSameAz;
        string key = Guid.NewGuid().ToString();

        // Reset stats and place every node outside the client's AZ...
        await ResetStats(configClient);
        await SetAvailabilityZone(configClient, otherAz, AllNodes);

        // ...then move exactly the primary and replica owning the key into the client's AZ.
        await SetAvailabilityZone(configClient, az, new SlotKeyRoute(key, SlotType.Primary));
        await SetAvailabilityZone(configClient, az, new SlotKeyRoute(key, SlotType.Replica));

        await using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinityAllNodes, az, protocol);

        for (int i = 0; i < nGetCalls; i++)
        {
            _ = await azTestClient.GetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.ALL], AllNodes);

        int matchingNodeCount = 0;
        int totalGetCalls = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            int calls = GetCalls(value);
            totalGetCalls += calls;

            bool inAz = value.Contains($"availability_zone:{az}");
            if (inAz)
            {
                if (calls > 0)
                {
                    Assert.Equal(callsPerNode, calls);
                    matchingNodeCount++;
                }
            }
            else if (calls > 0)
            {
                Assert.Fail($"Out-of-AZ node received {calls} GET calls when it shouldn't");
            }
        }

        // Both the in-AZ primary and the in-AZ replica should evenly split the GET calls,
        // and no reads should have landed outside the AZ.
        Assert.Equal(nodesInSameAz, matchingNodeCount);
        Assert.Equal(nGetCalls, totalGetCalls);
    }

    [Theory]
    [InlineData(ConnectionConfiguration.Protocol.RESP2)]
    [InlineData(ConnectionConfiguration.Protocol.RESP3)]
    public async Task TestAzAffinityAllNodesFallsBackToAllNodesWhenNoInAzNode(ConnectionConfiguration.Protocol protocol)
    {
        Skip.IfAzAffinityNotSupported();

        await using GlideClusterClient configClient = await CreateConfigClient(protocol);
        string key = Guid.NewGuid().ToString();

        // Clear any AZ so that the non-existing AZ matches nothing, triggering the all-nodes fallback.
        await SetAvailabilityZone(configClient, "", AllNodes);
        await ResetStats(configClient);

        int nReplicas = await GetReplicaCountInCluster(configClient);
        int nodesInShard = nReplicas + 1; // primary + replicas
        Assert.True(nodesInShard > 1, "shard must have at least one replica for this test");

        // nGetCalls == nodesInShard gives exactly one GET per shard node under round-robin.
        int nGetCalls = nodesInShard;

        // Use a client AZ that no node belongs to, forcing the all-nodes fallback.
        await using GlideClusterClient azTestClient = await CreateAzTestClient(ReadFromStrategy.AzAffinityAllNodes, Data.NonExistingAvailabilityZone, protocol);

        for (int i = 0; i < nGetCalls; i++)
        {
            _ = await azTestClient.GetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.InfoAsync([Section.ALL], AllNodes);

        int nodesWithGets = 0;
        int totalGetCalls = 0;
        bool primaryReceivedGets = false;
        bool replicaReceivedGets = false;
        foreach (string value in infoResult.MultiValue.Values)
        {
            int calls = GetCalls(value);
            if (calls == 0)
            {
                continue;
            }

            totalGetCalls += calls;
            nodesWithGets++;

            if (value.Contains("role:master"))
            {
                primaryReceivedGets = true;
            }

            if (value.Contains("role:slave"))
            {
                replicaReceivedGets = true;
            }
        }

        // Under the all-nodes fallback, every node in the shard (primary + replicas) receives traffic.
        Assert.Equal(nodesInShard, nodesWithGets);
        Assert.True(primaryReceivedGets, "primary must receive GET calls");
        Assert.True(replicaReceivedGets, "at least one replica must receive GET calls");
        Assert.Equal(nGetCalls, totalGetCalls);
    }
}
