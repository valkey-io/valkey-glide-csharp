// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// End-to-end integration tests for ReadFrom configuration flowing from connection string to FFI layer.
/// These tests verify the complete configuration pipeline and error handling scenarios.
/// Implements task 12 from the AZ Affinity support implementation plan.
/// </summary>
[Collection(typeof(ReadFromEndToEndIntegrationTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ReadFromEndToEndIntegrationTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Connection String to FFI Layer Tests

    [Theory]
    [InlineData("Primary", ReadFromStrategy.Primary, null)]
    [InlineData("PreferReplica", ReadFromStrategy.PreferReplica, null)]
    [InlineData("AzAffinity", ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData("AzAffinityReplicasAndPrimary", ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    public async Task EndToEnd_ConnectionString_ReadFromConfigurationFlowsToFFILayer_Standalone(
        string strategyString, ReadFromStrategy expectedStrategy, string? expectedAz)
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={strategyString}";
        if (expectedAz != null)
        {
            connectionString += $",az={expectedAz}";
        }

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Parse the original configuration to verify ReadFrom was set correctly
        var parsedConfig = ConfigurationOptions.Parse(connectionString);
        Assert.NotNull(parsedConfig.ReadFrom);
        Assert.Equal(expectedStrategy, parsedConfig.ReadFrom.Value.Strategy);
        Assert.Equal(expectedAz, parsedConfig.ReadFrom.Value.Az);

        // Verify the configuration reaches the underlying client by testing functionality
        var database = connectionMultiplexer.GetDatabase();
        Assert.NotNull(database);

        // Test a basic operation to ensure the connection works with ReadFrom configuration
        await database.PingAsync();

        // Test data operations to verify the ReadFrom configuration is active
        string testKey = Guid.NewGuid().ToString();
        string testValue = "end-to-end-test";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    [Theory]
    [InlineData("Primary", ReadFromStrategy.Primary, null)]
    [InlineData("PreferReplica", ReadFromStrategy.PreferReplica, null)]
    [InlineData("AzAffinity", ReadFromStrategy.AzAffinity, "ap-south-1c")]
    [InlineData("AzAffinityReplicasAndPrimary", ReadFromStrategy.AzAffinityReplicasAndPrimary, "us-west-2b")]
    public async Task EndToEnd_ConnectionString_ReadFromConfigurationFlowsToFFILayer_Cluster(
        string strategyString, ReadFromStrategy expectedStrategy, string? expectedAz)
    {
        // Skip if no cluster hosts available
        if (TestConfiguration.CLUSTER_HOSTS.Count == 0)
        {
            return;
        }

        // Arrange
        string connectionString = $"{TestConfiguration.CLUSTER_HOSTS[0].host}:{TestConfiguration.CLUSTER_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={strategyString}";
        if (expectedAz != null)
        {
            connectionString += $",az={expectedAz}";
        }

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Parse the original configuration to verify ReadFrom was set correctly
        var parsedConfig = ConfigurationOptions.Parse(connectionString);
        Assert.NotNull(parsedConfig.ReadFrom);
        Assert.Equal(expectedStrategy, parsedConfig.ReadFrom.Value.Strategy);
        Assert.Equal(expectedAz, parsedConfig.ReadFrom.Value.Az);

        // Verify the configuration reaches the underlying client by testing functionality
        var database = connectionMultiplexer.GetDatabase();
        Assert.NotNull(database);

        // Test a basic operation to ensure the connection works with ReadFrom configuration
        await database.PingAsync();

        // Test data operations to verify the ReadFrom configuration is active
        string testKey = Guid.NewGuid().ToString();
        string testValue = "cluster-end-to-end-test";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task EndToEnd_ConnectionStringWithoutReadFrom_DefaultsToNullInFFILayer()
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Parse the original configuration to verify ReadFrom is null (default behavior)
        var parsedConfig = ConfigurationOptions.Parse(connectionString);
        Assert.Null(parsedConfig.ReadFrom);

        // Test a basic operation to ensure the connection works without ReadFrom configuration
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Test data operations to verify default behavior works
        string testKey = Guid.NewGuid().ToString();
        string testValue = "default-behavior-test";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    [Theory]
    [InlineData("primary")]
    [InlineData("PREFERREPLICA")]
    [InlineData("azaffinity")]
    [InlineData("AzAffinityReplicasAndPrimary")]
    public async Task EndToEnd_ConnectionString_CaseInsensitiveReadFromParsing(string strategyString)
    {
        // Arrange
        var expectedStrategy = strategyString.ToLowerInvariant() switch
        {
            "primary" => ReadFromStrategy.Primary,
            "preferreplica" => ReadFromStrategy.PreferReplica,
            "azaffinity" => ReadFromStrategy.AzAffinity,
            "azaffinityreplicasandprimary" => ReadFromStrategy.AzAffinityReplicasAndPrimary,
            _ => throw new ArgumentException($"Unexpected strategy: {strategyString}")
        };

        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={strategyString}";

        if (expectedStrategy is ReadFromStrategy.AzAffinity or ReadFromStrategy.AzAffinityReplicasAndPrimary)
        {
            connectionString += ",az=test-zone";
        }

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Parse the original configuration to verify ReadFrom was set correctly
        var parsedConfig = ConfigurationOptions.Parse(connectionString);
        Assert.NotNull(parsedConfig.ReadFrom);
        Assert.Equal(expectedStrategy, parsedConfig.ReadFrom.Value.Strategy);

        // Test basic functionality
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    #endregion

    #region Error Handling in Complete Pipeline Tests

    [Theory]
    [InlineData("InvalidStrategy")]
    [InlineData("Unknown")]
    [InlineData("PrimaryAndSecondary")]
    public async Task EndToEnd_ConnectionString_InvalidReadFromStrategy_ThrowsArgumentException(string invalidStrategy)
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={invalidStrategy}";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => ConnectionMultiplexer.ConnectAsync(connectionString));

        Assert.Contains("is not supported", exception.Message);
    }

    [Fact]
    public async Task EndToEnd_ConnectionString_EmptyReadFromStrategy_ThrowsArgumentException()
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += ",readFrom=";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => ConnectionMultiplexer.ConnectAsync(connectionString));

        Assert.Contains("cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData("AzAffinity")]
    [InlineData("AzAffinityReplicasAndPrimary")]
    public async Task EndToEnd_ConnectionString_AzAffinityWithoutAz_ThrowsArgumentException(string strategy)
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={strategy}";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => ConnectionMultiplexer.ConnectAsync(connectionString));

        Assert.Contains("Availability zone should be set when using", exception.Message);
    }

    [Theory]
    [InlineData("Primary")]
    [InlineData("PreferReplica")]
    public async Task EndToEnd_ConnectionString_NonAzStrategyWithAz_ThrowsArgumentException(string strategy)
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={strategy},az=us-east-1a";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => ConnectionMultiplexer.ConnectAsync(connectionString));

        Assert.Contains("Availability zone should not be set when using", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task EndToEnd_ConnectionString_EmptyOrWhitespaceAz_ThrowsArgumentException(string invalidAz)
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom=AzAffinity,az={invalidAz}";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => ConnectionMultiplexer.ConnectAsync(connectionString));

        Assert.Contains("Availability zone cannot be empty or whitespace", exception.Message);
    }

    #endregion

    #region ConfigurationOptions to FFI Layer Tests

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    public async Task EndToEnd_ConfigurationOptions_ReadFromConfigurationFlowsToFFILayer_Standalone(
        ReadFromStrategy strategy, string? az)
    {
        // Arrange
        var configOptions = new ConfigurationOptions();
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        if (az != null)
        {
            configOptions.ReadFrom = new ReadFrom(strategy, az);
        }
        else
        {
            configOptions.ReadFrom = new ReadFrom(strategy);
        }

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Verify ReadFrom configuration is set correctly
        Assert.NotNull(configOptions.ReadFrom);
        Assert.Equal(strategy, configOptions.ReadFrom.Value.Strategy);
        Assert.Equal(az, configOptions.ReadFrom.Value.Az);

        // Test a basic operation to ensure the connection works with ReadFrom configuration
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Test data operations to verify the ReadFrom configuration is active
        string testKey = Guid.NewGuid().ToString();
        string testValue = "config-options-test";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "ap-south-1c")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "us-west-2b")]
    public async Task EndToEnd_ConfigurationOptions_ReadFromConfigurationFlowsToFFILayer_Cluster(
        ReadFromStrategy strategy, string? az)
    {
        // Skip if no cluster hosts available
        if (TestConfiguration.CLUSTER_HOSTS.Count == 0)
        {
            return;
        }

        // Arrange
        var configOptions = new ConfigurationOptions();
        configOptions.EndPoints.Add(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        if (az != null)
        {
            configOptions.ReadFrom = new ReadFrom(strategy, az);
        }
        else
        {
            configOptions.ReadFrom = new ReadFrom(strategy);
        }

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Verify ReadFrom configuration is set correctly
        Assert.NotNull(configOptions.ReadFrom);
        Assert.Equal(strategy, configOptions.ReadFrom.Value.Strategy);
        Assert.Equal(az, configOptions.ReadFrom.Value.Az);

        // Test a basic operation to ensure the connection works with ReadFrom configuration
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Test data operations to verify the ReadFrom configuration is active
        string testKey = Guid.NewGuid().ToString();
        string testValue = "cluster-config-options-test";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task EndToEnd_ConfigurationOptions_NullReadFrom_DefaultsToNullInFFILayer()
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

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Verify ReadFrom is null (default behavior)
        Assert.Null(configOptions.ReadFrom);

        // Test a basic operation to ensure the connection works without ReadFrom configuration
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Test data operations to verify default behavior works
        string testKey = Guid.NewGuid().ToString();
        string testValue = "null-readfrom-test";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    #endregion

    #region Round-Trip Serialization Tests

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    public async Task EndToEnd_RoundTripSerialization_ParseToStringToParse_MaintainsConfigurationIntegrity(
        ReadFromStrategy strategy, string? az)
    {
        // Arrange
        var originalConfig = new ConfigurationOptions();
        originalConfig.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        originalConfig.Ssl = TestConfiguration.TLS;

        if (az != null)
        {
            originalConfig.ReadFrom = new ReadFrom(strategy, az);
        }
        else
        {
            originalConfig.ReadFrom = new ReadFrom(strategy);
        }

        // Act 1: Serialize to string
        string serializedConfig = originalConfig.ToString();

        // Act 2: Parse back from string
        var parsedConfig = ConfigurationOptions.Parse(serializedConfig);

        // Act 3: Connect using parsed configuration
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(parsedConfig);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Verify functional equivalence between original and parsed configurations
        Assert.Equal(originalConfig.ReadFrom?.Strategy, parsedConfig.ReadFrom?.Strategy);
        Assert.Equal(originalConfig.ReadFrom?.Az, parsedConfig.ReadFrom?.Az);

        // Test a basic operation to ensure the connection works with round-trip configuration
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Test data operations to verify the round-trip configuration is active
        string testKey = Guid.NewGuid().ToString();
        string testValue = "round-trip-test";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task EndToEnd_RoundTripSerialization_NullReadFrom_MaintainsConfigurationIntegrity()
    {
        // Arrange
        var originalConfig = new ConfigurationOptions
        {
            ReadFrom = null
        };
        originalConfig.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        originalConfig.Ssl = TestConfiguration.TLS;

        // Act 1: Serialize to string
        string serializedConfig = originalConfig.ToString();

        // Act 2: Parse back from string
        var parsedConfig = ConfigurationOptions.Parse(serializedConfig);

        // Act 3: Connect using parsed configuration
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(parsedConfig);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Verify functional equivalence between original and parsed configurations
        Assert.Null(originalConfig.ReadFrom);
        Assert.Null(parsedConfig.ReadFrom);

        // Test a basic operation to ensure the connection works with null ReadFrom
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Test data operations to verify null ReadFrom behavior works
        string testKey = Guid.NewGuid().ToString();
        string testValue = "null-round-trip-test";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    #endregion

    #region Configuration Pipeline Validation Tests

    [Fact]
    public async Task EndToEnd_ConfigurationPipeline_ValidationErrorPropagation()
    {
        // Test that validation errors propagate correctly through the entire configuration pipeline

        // Arrange: Create configuration with invalid ReadFrom combination
        var configOptions = new ConfigurationOptions();
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act & Assert: Test invalid assignment through property setter
        Assert.Throws<ArgumentException>(() =>
        {
            configOptions.ReadFrom = new ReadFrom(ReadFromStrategy.Primary, "invalid-az-for-primary");
        });

        // Test that the configuration remains in a valid state after failed assignment
        configOptions.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);
        Assert.NotNull(connectionMultiplexer);

        // Test basic functionality
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task EndToEnd_ConfigurationPipeline_ClonePreservesReadFromConfiguration()
    {
        // Arrange
        var originalConfig = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1a")
        };
        originalConfig.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        originalConfig.Ssl = TestConfiguration.TLS;

        // Act
        var clonedConfig = originalConfig.Clone();

        // Modify original to ensure independence
        originalConfig.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);

        // Connect using cloned configuration
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(clonedConfig);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Verify cloned configuration preserved the original ReadFrom settings
        Assert.NotNull(clonedConfig.ReadFrom);
        Assert.Equal(ReadFromStrategy.AzAffinity, clonedConfig.ReadFrom.Value.Strategy);
        Assert.Equal("us-east-1a", clonedConfig.ReadFrom.Value.Az);

        // Verify original configuration was modified independently
        Assert.NotNull(originalConfig.ReadFrom);
        Assert.Equal(ReadFromStrategy.Primary, originalConfig.ReadFrom.Value.Strategy);

        // Test basic functionality
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Cleanup
        connectionMultiplexer.Dispose();
    }

    #endregion

    #region Performance and Stress Tests

    [Fact]
    public async Task EndToEnd_PerformanceValidation_MultipleConnectionsWithDifferentReadFromStrategies()
    {
        // Test that multiple connections with different ReadFrom strategies can be created efficiently
        var tasks = new List<Task<ConnectionMultiplexer>>();

        var configurations = new[]
        {
            ("Primary", ReadFromStrategy.Primary, null),
            ("PreferReplica", ReadFromStrategy.PreferReplica, null),
            ("AzAffinity", ReadFromStrategy.AzAffinity, "us-east-1a"),
            ("AzAffinityReplicasAndPrimary", ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b"),
            ("PrimaryAndSecondary", ReadFromStrategy.PrimaryAndSecondary, null)
        };

        try
        {
            // Create multiple connections concurrently
            foreach (var (strategyName, strategy, az) in configurations)
            {
                string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
                connectionString += $",readFrom={strategyName}";
                if (az != null)
                {
                    connectionString += $",az={az}";
                }

                tasks.Add(ConnectionMultiplexer.ConnectAsync(connectionString));
            }

            var connections = await Task.WhenAll(tasks);

            // Verify all connections were created successfully
            Assert.Equal(configurations.Length, connections.Length);

            // Verify each connection has the correct ReadFrom configuration
            for (int i = 0; i < connections.Length; i++)
            {
                var connection = connections[i];
                var (strategyName, expectedStrategy, expectedAz) = configurations[i];

                Assert.NotNull(connection);

                // Parse the connection string to verify ReadFrom configuration
                string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
                connectionString += $",readFrom={strategyName}";
                if (expectedAz != null)
                {
                    connectionString += $",az={expectedAz}";
                }

                var parsedConfig = ConfigurationOptions.Parse(connectionString);
                Assert.NotNull(parsedConfig.ReadFrom);
                Assert.Equal(expectedStrategy, parsedConfig.ReadFrom.Value.Strategy);
                Assert.Equal(expectedAz, parsedConfig.ReadFrom.Value.Az);

                // Test basic functionality
                var database = connection.GetDatabase();
                await database.PingAsync();

                // Test data operations
                string testKey = $"perf-test-{i}";
                string testValue = $"value-{strategyName}";
                await database.StringSetAsync(testKey, testValue);
                string? retrievedValue = await database.StringGetAsync(testKey);
                Assert.Equal(testValue, retrievedValue);
                await database.KeyDeleteAsync(testKey);
            }

            // Cleanup
            foreach (var connection in connections)
            {
                connection.Dispose();
            }
        }
        catch
        {
            // Cleanup on failure
            foreach (var task in tasks.Where(t => t.IsCompletedSuccessfully))
            {
                task.Result.Dispose();
            }
            throw;
        }
    }

    #endregion

    #region Backward Compatibility Tests

    [Fact]
    public async Task EndToEnd_BackwardCompatibility_LegacyConfigurationWithoutReadFromStillWorks()
    {
        // Test that existing applications that don't use ReadFrom continue to work

        // Arrange: Create a legacy-style configuration without ReadFrom
        var legacyConfig = new ConfigurationOptions();
        legacyConfig.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        legacyConfig.Ssl = TestConfiguration.TLS;
        legacyConfig.ResponseTimeout = 5000;
        legacyConfig.ConnectTimeout = 5000;
        // Explicitly not setting ReadFrom to simulate legacy behavior

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(legacyConfig);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Verify ReadFrom is null (legacy behavior)
        Assert.Null(legacyConfig.ReadFrom);

        // Verify full functionality
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Test basic operations to ensure legacy behavior works
        string testKey = "legacy-test-key";
        string testValue = "legacy-test-value";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    [Fact]
    public async Task EndToEnd_BackwardCompatibility_LegacyConnectionStringWithoutReadFromStillWorks()
    {
        // Test that existing connection strings without ReadFrom continue to work

        // Arrange: Create a legacy-style connection string without ReadFrom
        string legacyConnectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS},connectTimeout=5000,responseTimeout=5000";

        // Act
        var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(legacyConnectionString);

        // Assert - Verify connection was created successfully
        Assert.NotNull(connectionMultiplexer);

        // Parse the connection string to verify ReadFrom is null (legacy behavior)
        var parsedConfig = ConfigurationOptions.Parse(legacyConnectionString);
        Assert.Null(parsedConfig.ReadFrom);

        // Verify full functionality
        var database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();

        // Test basic operations to ensure legacy behavior works
        string testKey = "legacy-connection-string-test-key";
        string testValue = "legacy-connection-string-test-value";
        await database.StringSetAsync(testKey, testValue);
        string? retrievedValue = await database.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue);

        // Cleanup
        await database.KeyDeleteAsync(testKey);
        connectionMultiplexer.Dispose();
    }

    #endregion

    #region FFI Layer Integration Tests

    [Fact]
    public async Task EndToEnd_FFILayerIntegration_ReadFromConfigurationReachesRustCore()
    {
        // Test that ReadFrom configuration actually reaches the Rust core (FFI layer)
        // by creating clients with different ReadFrom strategies and verifying they work

#pragma warning disable CS8619
        var testCases = new[]
        {
            new { Strategy = ReadFromStrategy.Primary, Az = (string?)null },
            new { Strategy = ReadFromStrategy.PreferReplica, Az = (string?)null },
            new { Strategy = ReadFromStrategy.AzAffinity, Az = "us-east-1a" },
            new { Strategy = ReadFromStrategy.AzAffinityReplicasAndPrimary, Az = "eu-west-1b" }
        };
#pragma warning restore CS8619

        foreach (var testCase in testCases)
        {
            // Arrange - Create configuration with ReadFrom
            var config = new ConfigurationOptions();
            config.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
            config.Ssl = TestConfiguration.TLS;

            if (testCase.Az != null)
            {
                config.ReadFrom = new ReadFrom(testCase.Strategy, testCase.Az);
            }
            else
            {
                config.ReadFrom = new ReadFrom(testCase.Strategy);
            }

            // Act - Create connection and perform operations
            var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(config);
            var database = connectionMultiplexer.GetDatabase();

            // Assert - Verify the connection works, indicating FFI layer received configuration
            await database.PingAsync();

            // Perform multiple operations to ensure the ReadFrom strategy is active
            var tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                string key = $"ffi-test-{testCase.Strategy}-{i}";
                string value = $"value-{i}";
                tasks.Add(Task.Run(async () =>
                {
                    await database.StringSetAsync(key, value);
                    string? retrievedValue = await database.StringGetAsync(key);
                    Assert.Equal(value, retrievedValue);
                    await database.KeyDeleteAsync(key);
                }));
            }

            await Task.WhenAll(tasks);

            // Cleanup
            connectionMultiplexer.Dispose();
        }
    }

    [Fact]
    public async Task EndToEnd_FFILayerIntegration_AzAffinitySettingsPassedToRustCore()
    {
        // Test that AZ affinity settings are properly passed to the Rust core
        // by creating connections with different AZ values and verifying they work

        var azValues = new[] { "us-east-1a", "us-west-2b", "eu-central-1c", "ap-south-1d" };

        foreach (string az in azValues)
        {
            // Test both AZ affinity strategies
            var strategies = new[] { ReadFromStrategy.AzAffinity, ReadFromStrategy.AzAffinityReplicasAndPrimary };

            foreach (var strategy in strategies)
            {
                // Arrange
                var config = new ConfigurationOptions();
                config.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
                config.Ssl = TestConfiguration.TLS;
                config.ReadFrom = new ReadFrom(strategy, az);

                // Act
                var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(config);
                var database = connectionMultiplexer.GetDatabase();

                // Assert - Verify the connection works with the specific AZ configuration
                await database.PingAsync();

                // Test data operations to ensure AZ configuration is active
                string testKey = $"az-test-{strategy}-{az.Replace("-", "")}";
                string testValue = $"az-value-{az}";
                await database.StringSetAsync(testKey, testValue);
                string? retrievedValue = await database.StringGetAsync(testKey);
                Assert.Equal(testValue, retrievedValue);

                // Cleanup
                await database.KeyDeleteAsync(testKey);
                connectionMultiplexer.Dispose();
            }
        }
    }

    [Fact]
    public async Task EndToEnd_FFILayerIntegration_ErrorHandlingInCompleteConfigurationPipeline()
    {
        // Test that error handling works correctly throughout the complete configuration pipeline
        // from connection string parsing to FFI layer

        var errorTestCases = new[]
        {
            new {
                ConnectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS},readFrom=InvalidStrategy",
                ExpectedErrorSubstring = "is not supported"
            },
            new {
                ConnectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS},readFrom=AzAffinity",
                ExpectedErrorSubstring = "Availability zone should be set"
            },
            new {
                ConnectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS},readFrom=Primary,az=invalid-az",
                ExpectedErrorSubstring = "Availability zone should not be set"
            }
        };

        foreach (var testCase in errorTestCases)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => ConnectionMultiplexer.ConnectAsync(testCase.ConnectionString));

            Assert.Contains(testCase.ExpectedErrorSubstring, exception.Message);
        }
    }

    [Fact]
    public async Task EndToEnd_FFILayerIntegration_ConcurrentConnectionsWithDifferentReadFromStrategies()
    {
        // Test that multiple concurrent connections with different ReadFrom strategies
        // can be created and used simultaneously, verifying FFI layer handles multiple configurations

        var connectionTasks = new List<Task<(ConnectionMultiplexer Connection, ReadFromStrategy Strategy, string? Az)>>();

        var configurations = new[]
        {
            (ReadFromStrategy.Primary, null),
            (ReadFromStrategy.PreferReplica, null),
            (ReadFromStrategy.AzAffinity, "us-east-1a"),
            (ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")
        };

        try
        {
            // Create multiple connections concurrently
            foreach (var (strategy, az) in configurations)
            {
                connectionTasks.Add(Task.Run(async () =>
                {
                    var config = new ConfigurationOptions();
                    config.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
                    config.Ssl = TestConfiguration.TLS;

                    if (az != null)
                    {
                        config.ReadFrom = new ReadFrom(strategy, az);
                    }
                    else
                    {
                        config.ReadFrom = new ReadFrom(strategy);
                    }

                    var connection = await ConnectionMultiplexer.ConnectAsync(config);
                    return (connection, strategy, az);
                }));
            }

            var connections = await Task.WhenAll(connectionTasks);

            // Test all connections concurrently
            var operationTasks = new List<Task>();

            for (int i = 0; i < connections.Length; i++)
            {
                var (connection, strategy, az) = connections[i];
                int connectionIndex = i;

                operationTasks.Add(Task.Run(async () =>
                {
                    var database = connection.GetDatabase();

                    // Test ping
                    await database.PingAsync();

                    // Test data operations
                    for (int j = 0; j < 3; j++)
                    {
                        string key = $"concurrent-test-{connectionIndex}-{j}";
                        string value = $"value-{strategy}-{az ?? "null"}-{j}";

                        await database.StringSetAsync(key, value);
                        string? retrievedValue = await database.StringGetAsync(key);
                        Assert.Equal(value, retrievedValue);
                        await database.KeyDeleteAsync(key);
                    }
                }));
            }

            await Task.WhenAll(operationTasks);

            // Cleanup
            foreach (var (connection, _, _) in connections)
            {
                connection.Dispose();
            }
        }
        catch
        {
            // Cleanup on failure
            foreach (var task in connectionTasks.Where(t => t.IsCompletedSuccessfully))
            {
                task.Result.Connection.Dispose();
            }
            throw;
        }
    }

    #endregion
}
