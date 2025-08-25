// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Options.InfoOptions;
using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Route;

namespace Valkey.Glide.IntegrationTests;

public class AzAffinityTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static GlideClusterClient CreateAzTestClient(string az)
    {
        ClusterClientConfiguration config = TestConfiguration.DefaultClusterClientConfig()
            .WithReadFrom(new(ReadFromStrategy.AzAffinity, az))
            .WithRequestTimeout(TimeSpan.FromSeconds(2))
            .Build();
        return GlideClusterClient.CreateClient(config).GetAwaiter().GetResult();
    }

    private static GlideClusterClient CreateAzAffinityReplicasAndPrimaryTestClient(string az)
    {
        ClusterClientConfiguration config = TestConfiguration.DefaultClusterClientConfig()
            .WithReadFrom(new(ReadFromStrategy.AzAffinityReplicasAndPrimary, az))
            .WithRequestTimeout(TimeSpan.FromSeconds(2))
            .Build();
        return GlideClusterClient.CreateClient(config).GetAwaiter().GetResult();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestRoutingWithAzAffinityStrategyTo1Replica(GlideClusterClient configClient)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("8.0.0"), "AZ affinity requires server version 8.0.0 or higher");

        const string az = "us-east-1a";
        const int getCalls = 3;
        string key = Guid.NewGuid().ToString();
        string getCmdStat = $"cmdstat_get:calls={getCalls}";

        // Reset the availability zone for all nodes
        await configClient.CustomCommand(["config", "set", "availability-zone", ""], AllNodes);
        await configClient.CustomCommand(["config", "resetstat"], AllNodes);

        // 12182 is the slot of "foo"
        await configClient.CustomCommand(["config", "set", "availability-zone", az], new SlotKeyRoute(key, SlotType.Replica));

        using GlideClusterClient azTestClient = CreateAzTestClient(az);

        for (int i = 0; i < getCalls; i++)
        {
            await azTestClient.StringGetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.Info([Section.SERVER, Section.COMMANDSTATS], AllNodes);

        // Check that only the replica with az has all the GET calls
        int matchingEntriesCount = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            if (value.Contains(getCmdStat) && value.Contains(az))
            {
                matchingEntriesCount++;
            }
        }
        Assert.Equal(1, matchingEntriesCount);

        // Check that the other replicas have no availability zone set
        int changedAzCount = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            if (value.Contains($"availability_zone:{az}"))
            {
                changedAzCount++;
            }
        }
        Assert.Equal(1, changedAzCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestRoutingBySlotToReplicaWithAzAffinityStrategyToAllReplicas(GlideClusterClient configClient)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("8.0.0"), "AZ affinity requires server version 8.0.0 or higher");

        const string az = "us-east-1a";
        string key = Guid.NewGuid().ToString();

        // Reset the availability zone for all nodes
        await configClient.CustomCommand(["config", "set", "availability-zone", ""], AllNodes);
        await configClient.CustomCommand(["config", "resetstat"], AllNodes);

        // Get Replica Count for current cluster
        ClusterValue<string> clusterInfo = await configClient.Info([Section.REPLICATION], new SlotKeyRoute(key, SlotType.Primary));
        int nReplicas = 0;
        foreach (string line in clusterInfo.SingleValue!.Split('\n'))
        {
            string[] parts = line.Split(':', 2);
            if (parts.Length == 2 && parts[0].Trim() == "connected_slaves")
            {
                nReplicas = int.Parse(parts[1].Trim());
                break;
            }
        }

        int nGetCalls = 3 * nReplicas;
        string getCmdStat = "cmdstat_get:calls=3";

        // Setting AZ for all Nodes
        await configClient.CustomCommand(["config", "set", "availability-zone", az], AllNodes);

        using GlideClusterClient azTestClient = CreateAzTestClient(az);

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

        ClusterValue<string> infoResult = await azTestClient.Info([Section.ALL], AllNodes);

        // Check that all replicas have the same number of GET calls
        int matchingEntriesCount = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            if (value.Contains(getCmdStat) && value.Contains(az))
            {
                matchingEntriesCount++;
            }
        }
        Assert.Equal(nReplicas, matchingEntriesCount);
    }

    [Fact]
    public async Task TestAzAffinityNonExistingAz()
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("8.0.0"), "AZ affinity requires server version 8.0.0 or higher");

        const int nGetCalls = 3;
        const int nReplicaCalls = 1;
        string getCmdStat = $"cmdstat_get:calls={nReplicaCalls}";

        using GlideClusterClient azTestClient = CreateAzTestClient("non-existing-az");

        // Reset stats
        await azTestClient.CustomCommand(["config", "resetstat"], AllNodes);

        // Execute GET commands
        for (int i = 0; i < nGetCalls; i++)
        {
            await azTestClient.StringGetAsync("foo");
        }

        ClusterValue<string> infoResult = await azTestClient.Info([Section.COMMANDSTATS], AllNodes);

        // We expect the calls to be distributed evenly among the replicas
        int matchingEntriesCount = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            if (value.Contains(getCmdStat))
            {
                matchingEntriesCount++;
            }
        }
        Assert.Equal(3, matchingEntriesCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestAzAffinityReplicasAndPrimaryRoutesToPrimary(GlideClusterClient configClient)
    {
        Assert.SkipWhen(TestConfiguration.SERVER_VERSION < new Version("8.0.0"), "AZ affinity requires server version 8.0.0 or higher");

        const string az = "us-east-1a";
        const string otherAz = "us-east-1b";
        const int nGetCalls = 4;
        string key = Guid.NewGuid().ToString();
        string getCmdStat = $"cmdstat_get:calls={nGetCalls}";

        // Reset stats and set all nodes to otherAz
        await configClient.CustomCommand(["config", "resetstat"], AllNodes);
        await configClient.CustomCommand(["config", "set", "availability-zone", otherAz], AllNodes);

        // Set primary for slot 12182 to az
        await configClient.CustomCommand(["config", "set", "availability-zone", az], new SlotKeyRoute(key, SlotType.Primary));

        // Verify primary AZ
        ClusterValue<object?> primaryAzResult = await configClient.CustomCommand(["config", "get", "availability-zone"], new SlotKeyRoute(key, SlotType.Primary));
        object[]? primaryConfigArray = primaryAzResult.SingleValue as object[];
        if (primaryConfigArray != null && primaryConfigArray.Length >= 2)
        {
            Assert.Equal(az, primaryConfigArray[1]?.ToString());
        }

        using GlideClusterClient azTestClient = CreateAzAffinityReplicasAndPrimaryTestClient(az);

        // Execute GET commands
        for (int i = 0; i < nGetCalls; i++)
        {
            await azTestClient.StringGetAsync(key);
        }

        ClusterValue<string> infoResult = await azTestClient.Info([Section.ALL], AllNodes);

        // Check that only the primary in the specified AZ handled all GET calls
        int matchingEntriesCount = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            if (value.Contains(getCmdStat) && value.Contains(az) && value.Contains("role:master"))
            {
                matchingEntriesCount++;
            }
        }
        Assert.Equal(1, matchingEntriesCount);

        // Verify total GET calls
        int totalGetCalls = 0;
        foreach (string value in infoResult.MultiValue.Values)
        {
            if (value.Contains("cmdstat_get:calls="))
            {
                int startIndex = value.IndexOf("cmdstat_get:calls=") + "cmdstat_get:calls=".Length;
                int endIndex = value.IndexOf(',', startIndex);
                if (endIndex == -1) endIndex = value.Length;
                int calls = int.Parse(value.Substring(startIndex, endIndex - startIndex));
                totalGetCalls += calls;
            }
        }
        Assert.Equal(nGetCalls, totalGetCalls);
    }
}
