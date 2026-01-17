// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.IntegrationTests;
using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

[assembly: AssemblyFixture(typeof(TestConfiguration))]

namespace Valkey.Glide.IntegrationTests;

public class TestConfiguration : IDisposable
{
    // Default test timeout - higher on Windows due to WSL overhead.
    // TODO #184: Verify whether this is necessary on Windows.
    // public readonly static int DEFAULT_TIMEOUT_MS = OperatingSystem.IsWindows() ? 120_000 : 60_000;
    public readonly static int DEFAULT_TIMEOUT_MS = 60_000;
    public readonly static TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT_MS);

    // Addresses for the standalone and cluster servers.
    public static IList<Address> STANDALONE_ADDRESSES = [];
    public static IList<Address> CLUSTER_ADDRESSES = [];

    // First address for standalone and cluster servers, for convenience.
    public static Address STANDALONE_ADDRESS => STANDALONE_ADDRESSES.First();
    public static Address CLUSTER_ADDRESS => CLUSTER_ADDRESSES.First();

    // Environment variable names for providing server endpoints.
    private static readonly string StandaloneEndpointsEnvVar = "standalone-endpoints";
    private static readonly string ClusterEndpointsEnvVar = "cluster-endpoints";

    private static readonly object LockObject = new object();
    private const string DefaultServerGroupName = "cluster";
    public static Version SERVER_VERSION { get; internal set; } = new();
    public static bool TLS { get; internal set; } = false;

    // Version check helper methods for test skipping
    public static bool IsVersionLessThan(string version) => SERVER_VERSION < new Version(version);
    public static bool IsVersionLessThan(int major, int minor = 0, int build = 0) => SERVER_VERSION < new Version(major, minor, build);
    public static bool IsVersionAtLeast(string version) => SERVER_VERSION >= new Version(version);
    public static bool IsVersionAtLeast(int major, int minor = 0, int build = 0) => SERVER_VERSION >= new Version(major, minor, build);

    public static StandaloneClientConfigurationBuilder DefaultClientConfig() =>
        new StandaloneClientConfigurationBuilder()
            .WithAddress(STANDALONE_ADDRESS.Host, STANDALONE_ADDRESS.Port)
            .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
            .WithRequestTimeout(DEFAULT_TIMEOUT)
            .WithTls(TLS);

    public static ClusterClientConfigurationBuilder DefaultClusterClientConfig() =>
        new ClusterClientConfigurationBuilder()
            .WithAddress(CLUSTER_ADDRESS.Host, CLUSTER_ADDRESS.Port)
            .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
            .WithRequestTimeout(DEFAULT_TIMEOUT)
            .WithTls(TLS);

    public static StandaloneClientConfigurationBuilder DefaultClientConfigLowTimeout() =>
        new StandaloneClientConfigurationBuilder()
            .WithAddress(STANDALONE_ADDRESS.Host, STANDALONE_ADDRESS.Port)
            .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
            .WithRequestTimeout(TimeSpan.FromMilliseconds(250))
            .WithTls(TLS);

    public static GlideClient DefaultStandaloneClient()
        => GlideClient.CreateClient(DefaultClientConfig().Build()).GetAwaiter().GetResult();

    public static GlideClient LowTimeoutStandaloneClient()
        => GlideClient.CreateClient(DefaultClientConfigLowTimeout().Build()).GetAwaiter().GetResult();

    public static GlideClusterClient DefaultClusterClient()
        => GlideClusterClient.CreateClient(DefaultClusterClientConfig().Build()).GetAwaiter().GetResult();

    public static TheoryData<BaseClient> TestClients
    {
        get
        {
            lock (LockObject)
            {
                if (field.Count == 0)
                {
                    field = [.. TestStandaloneClients.Select(d => (BaseClient)d.Data), .. TestClusterClients.Select(d => (BaseClient)d.Data)];
                }
            }
            return field;
        }

        private set;
    } = [];

    public static TheoryData<GlideClient> TestStandaloneClients
    {
        get
        {
            lock (LockObject)
            {
                if (field.Count == 0)
                {
                    GlideClient resp2client = GlideClient.CreateClient(
                        DefaultClientConfig()
                        .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP2)
                        .WithRequestTimeout(DEFAULT_TIMEOUT)
                        .Build()
                    ).GetAwaiter().GetResult();
                    resp2client.SetInfo("RESP2");
                    GlideClient resp3client = GlideClient.CreateClient(
                        DefaultClientConfig()
                        .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
                        .WithRequestTimeout(DEFAULT_TIMEOUT)
                        .Build()
                    ).GetAwaiter().GetResult();
                    resp3client.SetInfo("RESP3");
                    field = [resp2client, resp3client];
                }
            }
            return field;
        }

        private set;
    } = [];

    public static TheoryData<GlideClusterClient> TestClusterClients
    {
        get
        {
            lock (LockObject)
            {
                if (field.Count == 0)
                {
                    GlideClusterClient resp2client = GlideClusterClient.CreateClient(
                        DefaultClusterClientConfig()
                        .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP2)
                        .WithRequestTimeout(DEFAULT_TIMEOUT)
                        .Build()
                    ).GetAwaiter().GetResult();
                    resp2client.SetInfo("RESP2");
                    GlideClusterClient resp3client = GlideClusterClient.CreateClient(
                        DefaultClusterClientConfig()
                        .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
                        .WithRequestTimeout(DEFAULT_TIMEOUT)
                        .Build()
                    ).GetAwaiter().GetResult();
                    resp3client.SetInfo("RESP3");
                    field = [resp2client, resp3client];
                }
            }
            return field;
        }

        private set;
    } = [];

    public static void ResetTestClients()
    {
        foreach (TheoryDataRow<BaseClient> data in TestClients)
        {
            data.Data.Dispose();
        }
        TestClients = [];
        TestClusterClients = [];
        TestStandaloneClients = [];
    }

    #region SER COMPAT
    public static ConfigurationOptions DefaultCompatibleConfig()
    {
        ConfigurationOptions config = new();
        config.EndPoints.Add(STANDALONE_ADDRESS.Host, STANDALONE_ADDRESS.Port);
        config.Ssl = TLS;
        config.ResponseTimeout = DEFAULT_TIMEOUT_MS;
        return config;
    }

    public static ConfigurationOptions DefaultCompatibleClusterConfig()
    {
        ConfigurationOptions config = new();
        config.EndPoints.Add(CLUSTER_ADDRESS.Host, CLUSTER_ADDRESS.Port);
        config.Ssl = TLS;
        config.ResponseTimeout = DEFAULT_TIMEOUT_MS;
        return config;
    }

    public static ConnectionMultiplexer DefaultCompatibleConnection()
        => ConnectionMultiplexer.Connect(DefaultCompatibleConfig());

    public static ConnectionMultiplexer DefaultCompatibleClusterConnection()
        => ConnectionMultiplexer.Connect(DefaultCompatibleClusterConfig());

    public static TheoryData<ConnectionMultiplexer> TestStandaloneConnections
    {
        get
        {
            lock (LockObject)
            {
                if (field.Count == 0)
                {
                    ConfigurationOptions resp2conf = DefaultCompatibleConfig();
                    resp2conf.Protocol = Protocol.Resp2;
                    ConnectionMultiplexer resp2Conn = ConnectionMultiplexer.Connect(resp2conf);
                    (resp2Conn.GetDatabase() as Database)!.SetInfo("RESP2");
                    ConfigurationOptions resp3conf = DefaultCompatibleConfig();
                    resp3conf.Protocol = Protocol.Resp3;
                    ConnectionMultiplexer resp3Conn = ConnectionMultiplexer.Connect(resp3conf);
                    (resp3Conn.GetDatabase() as Database)!.SetInfo("RESP3");

                    field = [resp2Conn, resp3Conn];
                }
            }
            return field;
        }

        private set;
    } = [];

    public static TheoryData<ConnectionMultiplexer> TestClusterConnections
    {
        get
        {
            lock (LockObject)
            {
                if (field.Count == 0)
                {
                    ConfigurationOptions resp2conf = DefaultCompatibleClusterConfig();
                    resp2conf.Protocol = Protocol.Resp2;
                    ConnectionMultiplexer resp2Conn = ConnectionMultiplexer.Connect(resp2conf);
                    (resp2Conn.GetDatabase() as Database)!.SetInfo("RESP2");
                    ConfigurationOptions resp3conf = DefaultCompatibleClusterConfig();
                    resp3conf.Protocol = Protocol.Resp3;
                    ConnectionMultiplexer resp3Conn = ConnectionMultiplexer.Connect(resp3conf);
                    (resp3Conn.GetDatabase() as Database)!.SetInfo("RESP3");

                    field = [resp2Conn, resp3Conn];
                }
            }
            return field;
        }

        private set;
    } = [];

    public static List<TheoryDataRow<ConnectionMultiplexer, bool>> TestConnections
    {
        get
        {
            lock (LockObject)
            {
                if (field.Count == 0)
                {
#pragma warning disable xUnit1046 // Avoid using TheoryDataRow arguments that are not serializable
                    field = [
                        .. TestStandaloneConnections.Select(d => new TheoryDataRow<ConnectionMultiplexer, bool>(d.Data, false)),
                        .. TestClusterConnections.Select(d => new TheoryDataRow<ConnectionMultiplexer, bool>(d.Data, true))
                    ];
#pragma warning restore xUnit1046 // Avoid using TheoryDataRow arguments that are not serializable
                }
            }
            return field;
        }

        private set;
    } = [];

    public static void ResetTestConnections()
    {
        TestConnections.ForEach(test => test.Data.Item1.Dispose());
        TestConnections = [];
        TestClusterConnections = [];
        TestStandaloneConnections = [];
    }
    #endregion

    public TestConfiguration()
    {
        TLS = Environment.GetEnvironmentVariable("tls") == "true";

        var standaloneEndpoints = Environment.GetEnvironmentVariable(StandaloneEndpointsEnvVar);
        var clusterEndpoints = Environment.GetEnvironmentVariable(ClusterEndpointsEnvVar);

        if (standaloneEndpoints is not null || clusterEndpoints is not null)
        {
            _startedServer = false;

            if (standaloneEndpoints is not null)
                STANDALONE_ADDRESSES = Address.FromHosts(standaloneEndpoints);

            if (clusterEndpoints is not null)
                CLUSTER_ADDRESSES = Address.FromHosts(clusterEndpoints);
        }
        else
        {
            _startedServer = true;

            // Stop all if weren't stopped on previous test run
            ServerManager.StopServer(DefaultServerGroupName, keepLogs: false);

            // TODO #184
            // Delete dirs if stop failed due to https://github.com/valkey-io/valkey-glide/issues/849
            // Not using `Directory.Exists` before deleting, because another process may delete the dir while IT is running.
            try
            {
                Directory.Delete(ServerManager.GetServerDirectory(), true);
            }
            catch (DirectoryNotFoundException) { }

            // Start standalone and cluster servers.
            CLUSTER_ADDRESSES = ServerManager.StartServer(DefaultServerGroupName, useClusterMode: true, useTls: TLS);
            STANDALONE_ADDRESSES = ServerManager.StartServer(DefaultServerGroupName, useClusterMode: false, useTls: TLS);
        }

        // Get server version
        SERVER_VERSION = GetServerVersion();

        TestConsoleWriteLine($"Cluster hosts = {string.Join(", ", CLUSTER_ADDRESSES)}");
        TestConsoleWriteLine($"Standalone hosts = {string.Join(", ", STANDALONE_ADDRESSES)}");
        TestConsoleWriteLine($"Server version = {SERVER_VERSION}");
    }

    ~TestConfiguration() => Dispose();

    public void Dispose()
    {
        ResetTestClients();
        ResetTestConnections();
        if (_startedServer)
        {
            ServerManager.StopServer(DefaultServerGroupName, keepLogs: true);
        }
    }

    private readonly bool _startedServer;

    private static void TestConsoleWriteLine(string message) =>
        TestContext.Current.SendDiagnosticMessage(message);


    private static Version GetServerVersion()
    {
        Exception? err = null;
        if (STANDALONE_ADDRESSES.Count > 0)
        {
            GlideClient client = DefaultStandaloneClient();
            try
            {
                return Client.GetVersion(client);
            }
            catch (Exception e)
            {
                err = e;
            }
        }
        if (CLUSTER_ADDRESSES.Count > 0)
        {
            GlideClusterClient client = DefaultClusterClient();
            try
            {
                return Client.GetVersion(client);
            }
            catch (Exception e)
            {
                if (err is not null)
                {
                    TestConsoleWriteLine(err.ToString());
                }
                TestConsoleWriteLine(e.ToString());
                throw;
            }
        }
        throw new Exception("No servers are given");
    }
}
