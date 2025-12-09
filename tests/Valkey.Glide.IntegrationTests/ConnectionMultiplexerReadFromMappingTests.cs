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
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.Primary)
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.Primary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromPreferReplica_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.PreferReplica)
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.PreferReplica, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromAzAffinity_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "us-east-1a";
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, testAz)
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.AzAffinity, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromAzAffinityReplicasAndPrimary_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "eu-west-1b";
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, testAz)
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromPrimary_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.Primary)
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_ADDRESSES[0].Host, TestConfiguration.CLUSTER_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.Primary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromPreferReplica_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.PreferReplica)
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_ADDRESSES[0].Host, TestConfiguration.CLUSTER_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.PreferReplica, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromAzAffinity_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "us-west-2a";
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, testAz)
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_ADDRESSES[0].Host, TestConfiguration.CLUSTER_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.AzAffinity, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_ReadFromAzAffinityReplicasAndPrimary_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "ap-south-1c";
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, testAz)
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_ADDRESSES[0].Host, TestConfiguration.CLUSTER_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_NullReadFrom_DefaultsToNullInStandaloneClient()
    {
        // Arrange
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = null
        };
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.False(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        }
    }

    [Fact]
    public async Task ConfigurationOptions_NullReadFrom_DefaultsToNullInClusterClient()
    {
        // Arrange
        ConfigurationOptions configOptions = new ConfigurationOptions
        {
            ReadFrom = null
        };
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_ADDRESSES[0].Host, TestConfiguration.CLUSTER_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.False(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        }
    }

    [Fact]
    public async Task ConnectionString_ReadFromPrimary_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_ADDRESSES[0].Host}:{TestConfiguration.STANDALONE_ADDRESSES[0].Port},readFrom=Primary,ssl={TestConfiguration.TLS}";

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.Primary, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Null(connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConnectionString_ReadFromAzAffinity_MapsToStandaloneClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "us-east-1a";
        string connectionString = $"{TestConfiguration.STANDALONE_ADDRESSES[0].Host}:{TestConfiguration.STANDALONE_ADDRESSES[0].Port},readFrom=AzAffinity,az={testAz},ssl={TestConfiguration.TLS}";

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.AzAffinity, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task ConnectionString_ReadFromAzAffinity_MapsToClusterClientConfigurationBuilder()
    {
        // Arrange
        const string testAz = "eu-west-1b";
        string connectionString = $"{TestConfiguration.CLUSTER_ADDRESSES[0].Host}:{TestConfiguration.CLUSTER_ADDRESSES[0].Port},readFrom=AzAffinity,az={testAz},ssl={TestConfiguration.TLS}";

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(ReadFromStrategy.AzAffinity, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Equal(testAz, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    public async Task EndToEnd_ReadFromConfiguration_FlowsFromConnectionStringToConnectionConfig(ReadFromStrategy strategy, string? az)
    {
        // Arrange
        string connectionString = TestConfiguration.STANDALONE_ADDRESSES[0].Host + ":" + TestConfiguration.STANDALONE_ADDRESSES[0].Port;
        connectionString += ",ssl=" + TestConfiguration.TLS;

        switch (strategy)
        {
            case ReadFromStrategy.Primary:
                connectionString += ",readFrom=Primary";
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
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
            Assert.Equal(strategy, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
            Assert.Equal(az, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
        }
    }

    [Fact]
    public async Task EndToEnd_NoReadFromConfiguration_DefaultsToNull()
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_ADDRESSES[0].Host}:{TestConfiguration.STANDALONE_ADDRESSES[0].Port},ssl={TestConfiguration.TLS}";

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString))
        {
            // Assert
            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(connectionMultiplexer.RawConfig);
            Assert.False(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        }
    }
}
