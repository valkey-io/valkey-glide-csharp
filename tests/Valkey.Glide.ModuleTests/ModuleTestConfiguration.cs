// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.ModuleTests;
using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

[assembly: AssemblyFixture(typeof(ModuleTestConfiguration))]

namespace Valkey.Glide.ModuleTests;

/// <summary>
/// Assembly fixture for the valkey-search 1.3 module-test harness. Connects to the Docker-based
/// standalone + cluster deployments <c>utils/run-modules-ci-locally.sh</c> starts with real
/// valkey-search modules loaded, via <c>MODULES_STANDALONE_ENDPOINT</c>/<c>MODULES_CLUSTER_ENDPOINTS</c>
/// -- never the main suite's own <c>ServerManager</c>-started servers, which don't load any module.
/// </summary>
/// <remarks>
/// Deliberately does not fall back to starting its own server the way
/// <c>Valkey.Glide.IntegrationTests.TestConfiguration</c> does: running this project without the
/// harness's endpoints set must fail loudly, not silently exercise the main suite's modules-free
/// server and report false negatives/positives for 1.3 features.
/// </remarks>
public sealed class ModuleTestConfiguration : IDisposable
{
    private const string StandaloneEndpointEnvVar = "MODULES_STANDALONE_ENDPOINT";
    private const string ClusterEndpointsEnvVar = "MODULES_CLUSTER_ENDPOINTS";

    public static Address? StandaloneAddress { get; private set; }

    public static IList<Address> ClusterAddresses { get; private set; } = [];

    public static bool HasCluster => ClusterAddresses.Count > 0;

    public ModuleTestConfiguration()
    {
        var standaloneEndpoint = Environment.GetEnvironmentVariable(StandaloneEndpointEnvVar);
        if (string.IsNullOrWhiteSpace(standaloneEndpoint))
        {
            throw new InvalidOperationException(
                $"{StandaloneEndpointEnvVar} is not set. The module-test harness never starts its own " +
                    "server -- run via utils/run-modules-ci-locally.sh, which starts Docker containers with " +
                    "real valkey-search modules loaded and exports this variable. Running this test " +
                    "project directly without that harness would otherwise silently fail to prove anything " +
                    "about search 1.3 support.");
        }

        StandaloneAddress = Address.FromHosts(standaloneEndpoint).First();

        var clusterEndpoints = Environment.GetEnvironmentVariable(ClusterEndpointsEnvVar);
        if (!string.IsNullOrWhiteSpace(clusterEndpoints))
        {
            ClusterAddresses = Address.FromHosts(clusterEndpoints);
        }
    }

    public static GlideClient CreateStandaloneClient()
    {
        // The assembly fixture constructor either sets this or throws before any test runs.
        Address address = StandaloneAddress
            ?? throw new InvalidOperationException($"{nameof(StandaloneAddress)} was never set; the fixture constructor should have thrown first.");

        return GlideClient.CreateClient(
            new StandaloneClientConfigurationBuilder()
                .WithAddress(address.Host, address.Port)
                .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
                .WithRequestTimeout(TimeSpan.FromSeconds(30))
                .Build())
            .GetAwaiter()
            .GetResult();
    }

    public static GlideClusterClient? CreateClusterClient()
    {
        if (!HasCluster)
        {
            return null;
        }

        Address address = ClusterAddresses.First();
        return GlideClusterClient.CreateClient(
            new ClusterClientConfigurationBuilder()
                .WithAddress(address.Host, address.Port)
                .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
                .WithRequestTimeout(TimeSpan.FromSeconds(30))
                .Build())
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Every client under test: the standalone module server, plus the cluster module deployment
    /// when its endpoints were provided.
    /// </summary>
    public static TheoryData<BaseClient> TestClients
    {
        get
        {
            if (field.Count == 0)
            {
                List<BaseClient> clients = [CreateStandaloneClient()];
                GlideClusterClient? cluster = CreateClusterClient();
                if (cluster is not null)
                {
                    clients.Add(cluster);
                }

                field = [.. clients];
                s_testClientsWereCreated = true;
            }

            return field;
        }
    } = [];

    private static bool s_testClientsWereCreated;

    public void Dispose()
    {
        // L6: Dispose previously read the lazy TestClients getter unconditionally, which (a)
        // creates clients from scratch just to immediately dispose them if no test ever touched
        // TestClients, and (b) if the cluster endpoint is unreachable, makes teardown itself
        // throw from inside CreateClusterClient() rather than cleanly disposing whatever was
        // already created. Only dispose what was actually created.
        if (!s_testClientsWereCreated)
        {
            return;
        }

        foreach (TheoryDataRow<BaseClient> row in TestClients)
        {
            row.Data.Dispose();
        }
    }
}
