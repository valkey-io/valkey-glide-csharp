// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for ConnectionMultiplexer ReadFrom mapping functionality.
/// These tests verify that ReadFrom configuration flows correctly from ConfigurationOptions
/// through to the ClientConfigurationBuilder and ConnectionConfig levels.
/// </summary>
[Collection(typeof(ConnectionMultiplexerReadFromMappingTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ConnectionMultiplexerReadFromMappingTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Fact]
    public async Task ConfigurationOptions_ReadFromPrimary_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.Primary)
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.Primary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromPreferReplica_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.PreferReplica)
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.PreferReplica, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromAzAffinity_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "us-east-1a";
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, testAz)
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.AzAffinity, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromAzAffinityReplicasAndPrimary_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "eu-west-1b";
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, testAz)
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromPrimary_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.Primary)
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.Primary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromPreferReplica_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.PreferReplica)
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.PreferReplica, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromAzAffinity_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "us-west-2a";
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, testAz)
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.AzAffinity, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromAzAffinityReplicasAndPrimary_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "ap-south-1c";
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, testAz)
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_NullReadFrom_DefaultsToNullInStandaloneClient()
    {
        // Arrange
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = null
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.False(connectionMultiplexer.RawConfig.ReadFrom.HasValue);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConfigurationOptions_NullReadFrom_DefaultsToNullInClusterClient()
    {
        // Arrange
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = null
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.False(connectionMultiplexer.RawConfig.ReadFrom.HasValue);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConnectionString_ReadFromPrimary_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},readFrom=Primary,ssl={TestConfiguration.TLS}";

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.Primary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConnectionString_ReadFromAzAffinity_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "us-east-1a";
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},readFrom=AzAffinity,az={testAz},ssl={TestConfiguration.TLS}";

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.AzAffinity, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task ConnectionString_ReadFromAzAffinity_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "eu-west-1b";
        string connectionString = $"{TestConfiguration.CLUSTER_HOSTS[0].host}:{TestConfiguration.CLUSTER_HOSTS[0].port},readFrom=AzAffinity,az={testAz},ssl={TestConfiguration.TLS}";

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(ReadFromStrategy.AzAffinity, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public void ClientConfigurationBuilder_ReadFromConfiguration_FlowsToConnectionConfig()
    {
        // Arrange
        const string testAz = "us-west-2b";
        var readFromConfig = new ReadFrom(ReadFromStrategy.AzAffinity, testAz);

        // Act - Test Standalone Configuration
        var standaloneBuilder = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port)
            .WithReadFrom(readFromConfig);
        var standaloneConfig = standaloneBuilder.Build();

        // Assert - Standalone
        Assert.NotNull(standaloneConfig);
        var standaloneConnectionConfig = standaloneConfig.ToRequest();
        Assert.NotNull(standaloneConnectionConfig.ReadFrom);
        Assert.Equal(ReadFromStrategy.AzAffinity, standaloneConnectionConfig.ReadFrom.Value.Strategy);
        Assert.Equal(testAz, standaloneConnectionConfig.ReadFrom.Value.Az);

        // Act - Test Cluster Configuration
        var clusterBuilder = new ClusterClientConfigurationBuilder()
            .WithAddress(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port)
            .WithReadFrom(readFromConfig);
        var clusterConfig = clusterBuilder.Build();

        // Assert - Cluster
        Assert.NotNull(clusterConfig);
        var clusterConnectionConfig = clusterConfig.ToRequest();
        Assert.NotNull(clusterConnectionConfig.ReadFrom);
        Assert.Equal(ReadFromStrategy.AzAffinity, clusterConnectionConfig.ReadFrom.Value.Strategy);
        Assert.Equal(testAz, clusterConnectionConfig.ReadFrom.Value.Az);
    }

    [Fact]
    public void ClientConfigurationBuilder_NullReadFrom_FlowsToConnectionConfig()
    {
        // Act - Test Standalone Configuration
        var standaloneBuilder = new StandaloneClientConfigurationBuilder()
            .WithAddress(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        var standaloneConfig = standaloneBuilder.Build();

        // Assert - Standalone
        Assert.NotNull(standaloneConfig);
        var standaloneConnectionConfig = standaloneConfig.ToRequest();
        Assert.Null(standaloneConnectionConfig.ReadFrom);

        // Act - Test Cluster Configuration
        var clusterBuilder = new ClusterClientConfigurationBuilder()
            .WithAddress(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port);
        var clusterConfig = clusterBuilder.Build();

        // Assert - Cluster
        Assert.NotNull(clusterConfig);
        var clusterConnectionConfig = clusterConfig.ToRequest();
        Assert.Null(clusterConnectionConfig.ReadFrom);
    }

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PrimaryAndSecondary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    public async Task EndToEnd_ReadFromConfiguration_FlowsFromConnectionStringToConnectionConfig(ReadFromStrategy strategy, string? az)
    {
        // Arrange
        string connectionString = TestConfiguration.STANDALONE_HOSTS[0].host + ":" + TestConfiguration.STANDALONE_HOSTS[0].port;
        connectionString += ",ssl=" + TestConfiguration.TLS;

        switch (strategy)
        {
            case ReadFromStrategy.Primary:
                connectionString += ",readFrom=Primary";
                break;
            case ReadFromStrategy.PrimaryAndSecondary:
                connectionString += ",readFrom=PrimaryAndSecondary";
                break;
            case ReadFromStrategy.PreferReplica:
                connectionString += ",readFrom=PreferReplica";
                break;
            case ReadFromStrategy.AzAffinity:
                connectionString += $",readFrom=AzAffinity,az={az}";
                break;
            case ReadFromStrategy.AzAffinityReplicasAndPrimary:
                connectionString += $",readFrom=AzAffinityReplicasAndPrimary,az={az}";
                break;
            default:
                throw new ArgumentException("Invalid ReadFromStrategy for this test");
        }

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(strategy, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(az, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task EndToEnd_NoReadFromConfiguration_DefaultsToNull()
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.False(connectionMultiplexer.RawConfig.ReadFrom.HasValue);

        // Cleanup
        connectionMultiplexer.Dispose();
    }
}
