// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.IntegrationTests;

using static Valkey.Glide.ConnectionConfiguration;

[assembly: AssemblyFixture(typeof(TestConfiguration))]

namespace Valkey.Glide.IntegrationTests;

public class TestConfiguration : IDisposable
{
    public static Cluster DefaultStandaloneServer { get; internal set; } = null!;
    public static Cluster DefaultClusterServer { get; internal set; } = null!;
    public static Version SERVER_VERSION { get; internal set; } = new();
    public static bool TLS { get; internal set; } = false;

    public static StandaloneClientConfigurationBuilder DefaultClientConfig() =>
        new StandaloneClientConfigurationBuilder()
            .WithAddress(DefaultStandaloneServer.Hosts[0].host, DefaultStandaloneServer.Hosts[0].port)
            .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
            .WithTls(TLS);

    public static ClusterClientConfigurationBuilder DefaultClusterClientConfig() =>
        new ClusterClientConfigurationBuilder()
            .WithAddress(DefaultClusterServer.Hosts[0].host, DefaultClusterServer.Hosts[0].port)
            .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
            .WithTls(TLS);

    public static GlideClient DefaultStandaloneClientWithExtraTimeout()
        => GlideClient.CreateClient(
                DefaultClientConfig()
                .WithRequestTimeout(TimeSpan.FromSeconds(1))
                .Build())
            .GetAwaiter()
            .GetResult();

    public static GlideClusterClient DefaultClusterClientWithExtraTimeout()
        => GlideClusterClient.CreateClient(
                DefaultClusterClientConfig()
                .WithRequestTimeout(TimeSpan.FromSeconds(1))
                .Build())
            .GetAwaiter()
            .GetResult();

    public static GlideClient DefaultStandaloneClient()
        => GlideClient.CreateClient(DefaultClientConfig().Build()).GetAwaiter().GetResult();

    public static GlideClusterClient DefaultClusterClient()
        => GlideClusterClient.CreateClient(DefaultClusterClientConfig().Build()).GetAwaiter().GetResult();

    public static TheoryData<BaseClient> TestClients
    {
        get
        {
            if (field.Count == 0)
            {
                field = [.. TestStandaloneClients.Select(d => (BaseClient)d.Data), .. TestClusterClients.Select(d => (BaseClient)d.Data)];
            }
            return field;
        }

        private set;
    } = [];

    public static TheoryData<GlideClient> TestStandaloneClients
    {
        get
        {
            if (field.Count == 0)
            {
                GlideClient resp2client = GlideClient.CreateClient(
                    DefaultClientConfig()
                    .WithRequestTimeout(TimeSpan.FromSeconds(1))
                    .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP2)
                    .Build()
                ).GetAwaiter().GetResult();
                resp2client.SetInfo("RESP2");
                GlideClient resp3client = GlideClient.CreateClient(
                    DefaultClientConfig()
                    .WithRequestTimeout(TimeSpan.FromSeconds(1))
                    .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
                    .Build()
                ).GetAwaiter().GetResult();
                resp3client.SetInfo("RESP3");
                field = [resp2client, resp3client];
            }
            return field;
        }

        private set;
    } = [];

    public static TheoryData<GlideClusterClient> TestClusterClients
    {
        get
        {
            if (field.Count == 0)
            {
                GlideClusterClient resp2client = GlideClusterClient.CreateClient(
                    DefaultClusterClientConfig()
                    .WithRequestTimeout(TimeSpan.FromSeconds(1))
                    .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP2)
                    .Build()
                ).GetAwaiter().GetResult();
                resp2client.SetInfo("RESP2");
                GlideClusterClient resp3client = GlideClusterClient.CreateClient(
                    DefaultClusterClientConfig()
                    .WithRequestTimeout(TimeSpan.FromSeconds(1))
                    .WithProtocolVersion(ConnectionConfiguration.Protocol.RESP3)
                    .Build()
                ).GetAwaiter().GetResult();
                resp3client.SetInfo("RESP3");
                field = [resp2client, resp3client];
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
        config.EndPoints.Add(DefaultStandaloneServer.Hosts[0].host, DefaultStandaloneServer.Hosts[0].port);
        config.Ssl = TLS;
        config.ResponseTimeout = 1000;
        return config;
    }

    public static ConfigurationOptions DefaultCompatibleClusterConfig()
    {
        ConfigurationOptions config = new();
        config.EndPoints.Add(DefaultClusterServer.Hosts[0].host, DefaultClusterServer.Hosts[0].port);
        config.Ssl = TLS;
        config.ResponseTimeout = 1000;
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
            return field;
        }

        private set;
    } = [];

    public static TheoryData<ConnectionMultiplexer> TestClusterConnections
    {
        get
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
            return field;
        }

        private set;
    } = [];

    public static List<TheoryDataRow<ConnectionMultiplexer, bool>> TestConnections
    {
        get
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

        if (Environment.GetEnvironmentVariable("cluster-endpoints") is { } || Environment.GetEnvironmentVariable("standalone-endpoints") is { })
        {
            string? clusterEndpoints = Environment.GetEnvironmentVariable("cluster-endpoints");
            DefaultClusterServer = new(clusterEndpoints is null ? [] : ParseHostsString(clusterEndpoints));
            string? standaloneEndpoints = Environment.GetEnvironmentVariable("standalone-endpoints");
            DefaultStandaloneServer = new(standaloneEndpoints is null ? [] : ParseHostsString(standaloneEndpoints));
            _startedServer = false;
        }
        else
        {
            _startedServer = true;
            DefaultClusterServer = new Cluster();
            DefaultClusterServer.Start(clusterMode: true, replicas: 1, tls: TLS);
            DefaultStandaloneServer = new Cluster();
            DefaultStandaloneServer.Start(clusterMode: false, replicas: 1, tls: TLS);
        }

        SERVER_VERSION = GetServerVersion();

        TestConsoleWriteLine($"Cluster hosts = {string.Join(", ", DefaultClusterServer.Hosts)}");
        TestConsoleWriteLine($"Standalone hosts = {string.Join(", ", DefaultStandaloneServer.Hosts)}");
        TestConsoleWriteLine($"Server version = {SERVER_VERSION}");
    }

    ~TestConfiguration() => Dispose();

    public void Dispose()
    {
        ResetTestClients();
        ResetTestConnections();
        if (_startedServer)
        {
            // Stop all
            DefaultStandaloneServer.Stop();
            DefaultClusterServer.Stop();
        }
    }

    private readonly bool _startedServer;

    private static void TestConsoleWriteLine(string message) => TestContext.Current.SendDiagnosticMessage(message);

    private static Version GetServerVersion()
    {
#pragma warning disable IDE0046 // Convert to conditional expression
        if (DefaultStandaloneServer.Hosts.Count > 0)
        {
            return DefaultStandaloneServer.ServerVersion;
        }
        if (DefaultClusterServer.Hosts.Count > 0)
        {
            return DefaultClusterServer.ServerVersion;
        }
        throw new Exception("No servers are given");
#pragma warning restore IDE0046 // Convert to conditional expression
    }

    private static List<(string host, ushort port)> ParseHostsString(string @string)
        => [.. @string.Split(',').Select(s => s.Split(':')).Select(s => (host: s[0], port: ushort.Parse(s[1])))];
}
