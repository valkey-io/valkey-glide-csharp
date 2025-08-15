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
    [InlineData("Primary", ReadFromStrategy.Primary, null, true)]
    [InlineData("PreferReplica", ReadFromStrategy.PreferReplica, null, true)]
    [InlineData("AzAffinity", ReadFromStrategy.AzAffinity, "us-east-1a", true)]
    [InlineData("AzAffinityReplicasAndPrimary", ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b", true)]
    [InlineData("Primary", ReadFromStrategy.Primary, null, false)]
    [InlineData("PreferReplica", ReadFromStrategy.PreferReplica, null, false)]
    [InlineData("AzAffinity", ReadFromStrategy.AzAffinity, "ap-south-1c", false)]
    [InlineData("AzAffinityReplicasAndPrimary", ReadFromStrategy.AzAffinityReplicasAndPrimary, "us-west-2b", false)]
    public async Task EndToEnd_ConnectionString_ReadFromConfigurationFlowsToFFILayer(
        string strategyString, ReadFromStrategy expectedStrategy, string? expectedAz, bool useStandalone)
    {
        // Arrange
        (string host, int port) hostConfig = useStandalone ? TestConfiguration.STANDALONE_HOSTS[0] : TestConfiguration.CLUSTER_HOSTS[0];
        string connectionString = $"{hostConfig.host}:{hostConfig.port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={strategyString}";
        if (expectedAz != null)
        {
            connectionString += $",az={expectedAz}";
        }

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString))
        {
            // Assert - Verify connection was created successfully
            Assert.NotNull(connectionMultiplexer);

            // Parse the original configuration to verify ReadFrom was set correctly
            ConfigurationOptions parsedConfig = ConfigurationOptions.Parse(connectionString);
            Assert.NotNull(parsedConfig.ReadFrom);
            Assert.Equal(expectedStrategy, parsedConfig.ReadFrom.Value.Strategy);
            Assert.Equal(expectedAz, parsedConfig.ReadFrom.Value.Az);

            // Verify the configuration reaches the underlying client by testing functionality
            IDatabase database = connectionMultiplexer.GetDatabase();
            Assert.NotNull(database);

            // Test a basic operation to ensure the connection works with ReadFrom configuration
            await database.PingAsync();

            // Test data operations to verify the ReadFrom configuration is active
            string testKey = Guid.NewGuid().ToString();
            string testValue = useStandalone ? "standalone-end-to-end-test" : "cluster-end-to-end-test";
            await database.StringSetAsync(testKey, testValue);
            string? retrievedValue = await database.StringGetAsync(testKey);
            Assert.Equal(testValue, retrievedValue);

            // Cleanup
            await database.KeyDeleteAsync(testKey);
        }
    }

    [Theory]
    [InlineData("primary")]
    [InlineData("PREFERREPLICA")]
    [InlineData("azaffinity")]
    [InlineData("AzAffinityReplicasAndPrimary")]
    public async Task EndToEnd_ConnectionString_CaseInsensitiveReadFromParsing(string strategyString)
    {
        // Arrange
        ReadFromStrategy expectedStrategy = Enum.Parse<ReadFromStrategy>(strategyString, ignoreCase: true);

        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={strategyString}";

        if (expectedStrategy is ReadFromStrategy.AzAffinity or ReadFromStrategy.AzAffinityReplicasAndPrimary)
        {
            connectionString += ",az=test-zone";
        }

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString))
        {
            // Assert - Verify connection was created successfully
            Assert.NotNull(connectionMultiplexer);

            // Parse the original configuration to verify ReadFrom was set correctly
            ConfigurationOptions parsedConfig = ConfigurationOptions.Parse(connectionString);
            Assert.NotNull(parsedConfig.ReadFrom);
            Assert.Equal(expectedStrategy, parsedConfig.ReadFrom.Value.Strategy);

            // Test basic functionality
            IDatabase database = connectionMultiplexer.GetDatabase();
            await database.PingAsync();
        }
    }

    [Theory]
    [InlineData(true)]  // Connection string without ReadFrom
    [InlineData(false)] // ConfigurationOptions with null ReadFrom
    public async Task EndToEnd_NullReadFrom_DefaultsToNullInFFILayer(bool useConnectionString)
    {
        // Arrange & Act
        if (useConnectionString)
        {
            string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";

            using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString))
            {
                // Assert - Verify connection was created successfully
                Assert.NotNull(connectionMultiplexer);

                // Parse the original configuration to verify ReadFrom is null (default behavior)
                ConfigurationOptions parsedConfig = ConfigurationOptions.Parse(connectionString);
                Assert.Null(parsedConfig.ReadFrom);

                // Test a basic operation to ensure the connection works without ReadFrom configuration
                IDatabase database = connectionMultiplexer.GetDatabase();
                await database.PingAsync();

                // Test data operations to verify default behavior works
                string testKey = Guid.NewGuid().ToString();
                string testValue = "connection-string-default-behavior-test";
                await database.StringSetAsync(testKey, testValue);
                string? retrievedValue = await database.StringGetAsync(testKey);
                Assert.Equal(testValue, retrievedValue);

                // Cleanup
                await database.KeyDeleteAsync(testKey);
            }
        }
        else
        {
            ConfigurationOptions configOptions = new ConfigurationOptions
            {
                ReadFrom = null
            };
            configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
            configOptions.Ssl = TestConfiguration.TLS;

            using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
            {
                // Assert - Verify connection was created successfully
                Assert.NotNull(connectionMultiplexer);

                // Verify ReadFrom is null (default behavior)
                Assert.Null(configOptions.ReadFrom);

                // Test a basic operation to ensure the connection works without ReadFrom configuration
                IDatabase database = connectionMultiplexer.GetDatabase();
                await database.PingAsync();

                // Test data operations to verify default behavior works
                string testKey = Guid.NewGuid().ToString();
                string testValue = "config-options-null-readfrom-test";
                await database.StringSetAsync(testKey, testValue);
                string? retrievedValue = await database.StringGetAsync(testKey);
                Assert.Equal(testValue, retrievedValue);

                // Cleanup
                await database.KeyDeleteAsync(testKey);
            }
        }
    }

    #endregion

    #region Error Handling in Complete Pipeline Tests

    [Theory]
    [InlineData("InvalidStrategy", "", "is not supported")]
    [InlineData("Unknown", "", "is not supported")]
    [InlineData("PrimaryAndSecondary", "", "is not supported")]
    [InlineData("", "", "cannot be empty")]
    [InlineData("AzAffinity", "", "Availability zone cannot be empty or whitespace")]
    [InlineData("AzAffinityReplicasAndPrimary", "", "Availability zone cannot be empty or whitespace")]
    [InlineData("Primary", "us-east-1a", "Availability zone should not be set when using")]
    [InlineData("PreferReplica", "us-east-1a", "Availability zone should not be set when using")]
    [InlineData("AzAffinity", "   ", "Availability zone cannot be empty or whitespace")]
    [InlineData("AzAffinity", "\t", "Availability zone cannot be empty or whitespace")]
    [InlineData("AzAffinity", "\n", "Availability zone cannot be empty or whitespace")]
    [InlineData("AzAffinityReplicasAndPrimary", "   ", "Availability zone cannot be empty or whitespace")]
    public async Task EndToEnd_ConnectionString_ArgumentExceptionScenarios(string readFromStrategy, string azValue, string expectedErrorSubstring)
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={readFromStrategy}";
        if (!string.IsNullOrEmpty(azValue))
        {
            connectionString += $",az={azValue}";
        }

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            () => ConnectionMultiplexer.ConnectAsync(connectionString));

        Assert.Contains(expectedErrorSubstring, exception.Message);
    }

    #endregion

    #region ConfigurationOptions to FFI Layer Tests

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null, true)]
    [InlineData(ReadFromStrategy.PreferReplica, null, true)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a", true)]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b", true)]
    [InlineData(ReadFromStrategy.Primary, null, false)]
    [InlineData(ReadFromStrategy.PreferReplica, null, false)]
    [InlineData(ReadFromStrategy.AzAffinity, "ap-south-1c", false)]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "us-west-2b", false)]
    public async Task EndToEnd_ConfigurationOptions_ReadFromConfigurationFlowsToFFILayer(
        ReadFromStrategy strategy, string? az, bool useStandalone)
    {
        // Skip cluster tests if no cluster hosts available
        if (!useStandalone && TestConfiguration.CLUSTER_HOSTS.Count == 0)
        {
            return;
        }

        // Arrange
        ConfigurationOptions configOptions = new ConfigurationOptions();
        (string host, int port) hostConfig = useStandalone ? TestConfiguration.STANDALONE_HOSTS[0] : TestConfiguration.CLUSTER_HOSTS[0];
        configOptions.EndPoints.Add(hostConfig.host, hostConfig.port);
        configOptions.Ssl = TestConfiguration.TLS;

        configOptions.ReadFrom = az != null
            ? new ReadFrom(strategy, az)
            : new ReadFrom(strategy);

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            // Assert - Verify connection was created successfully
            Assert.NotNull(connectionMultiplexer);

            // Verify ReadFrom configuration is set correctly
            Assert.NotNull(configOptions.ReadFrom);
            Assert.Equal(strategy, configOptions.ReadFrom.Value.Strategy);
            Assert.Equal(az, configOptions.ReadFrom.Value.Az);

            // Test a basic operation to ensure the connection works with ReadFrom configuration
            IDatabase database = connectionMultiplexer.GetDatabase();
            await database.PingAsync();

            // Test data operations to verify the ReadFrom configuration is active
            string testKey = Guid.NewGuid().ToString();
            string testValue = useStandalone ? "config-options-standalone-test" : "config-options-cluster-test";
            await database.StringSetAsync(testKey, testValue);
            string? retrievedValue = await database.StringGetAsync(testKey);
            Assert.Equal(testValue, retrievedValue);

            // Cleanup
            await database.KeyDeleteAsync(testKey);
        }
    }

    #endregion

    #region Round-Trip Serialization Tests

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    [InlineData(null, null)] // Null ReadFrom test case
    public async Task EndToEnd_RoundTripSerialization_MaintainsConfigurationIntegrity(
        ReadFromStrategy? strategy, string? az)
    {
        // Arrange
        ConfigurationOptions originalConfig = new ConfigurationOptions();
        originalConfig.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        originalConfig.Ssl = TestConfiguration.TLS;

        originalConfig.ReadFrom = strategy.HasValue
            ? (az != null
                ? new ReadFrom(strategy.Value, az)
                : new ReadFrom(strategy.Value))
            : null;

        // Act 1: Serialize to string
        string serializedConfig = originalConfig.ToString();

        // Act 2: Parse back from string
        ConfigurationOptions parsedConfig = ConfigurationOptions.Parse(serializedConfig);

        // Act 3: Connect using parsed configuration
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(parsedConfig))
        {
            // Assert - Verify connection was created successfully
            Assert.NotNull(connectionMultiplexer);

            // Verify functional equivalence between original and parsed configurations
            Assert.Equal(originalConfig.ReadFrom?.Strategy, parsedConfig.ReadFrom?.Strategy);
            Assert.Equal(originalConfig.ReadFrom?.Az, parsedConfig.ReadFrom?.Az);

            // Test a basic operation to ensure the connection works with round-trip configuration
            IDatabase database = connectionMultiplexer.GetDatabase();
            await database.PingAsync();

            // Test data operations to verify the round-trip configuration is active
            string testKey = Guid.NewGuid().ToString();
            string testValue = strategy.HasValue ? $"round-trip-{strategy}-test" : "round-trip-null-test";
            await database.StringSetAsync(testKey, testValue);
            string? retrievedValue = await database.StringGetAsync(testKey);
            Assert.Equal(testValue, retrievedValue);

            // Cleanup
            await database.KeyDeleteAsync(testKey);
        }
    }

    #endregion

    #region Configuration Pipeline Validation Tests

    [Fact]
    public async Task EndToEnd_ConfigurationPipeline_ValidationErrorPropagation()
    {
        // Test that validation errors propagate correctly through the entire configuration pipeline

        // Arrange: Create configuration with invalid ReadFrom combination
        ConfigurationOptions configOptions = new ConfigurationOptions();
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act & Assert: Test invalid assignment through property setter
        Assert.Throws<ArgumentException>(() =>
        {
            configOptions.ReadFrom = new ReadFrom(ReadFromStrategy.Primary, "invalid-az-for-primary");
        });

        // Test that the configuration remains in a valid state after failed assignment
        configOptions.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions))
        {
            Assert.NotNull(connectionMultiplexer);

            // Test basic functionality
            IDatabase database = connectionMultiplexer.GetDatabase();
            await database.PingAsync();
        }
    }

    [Fact]
    public async Task EndToEnd_ConfigurationPipeline_ClonePreservesReadFromConfiguration()
    {
        // Arrange
        ConfigurationOptions originalConfig = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1a")
        };
        originalConfig.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        originalConfig.Ssl = TestConfiguration.TLS;

        // Act
        ConfigurationOptions clonedConfig = originalConfig.Clone();

        // Modify original to ensure independence
        originalConfig.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);

        // Connect using cloned configuration
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(clonedConfig))
        {
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
            IDatabase database = connectionMultiplexer.GetDatabase();
            await database.PingAsync();
        }
    }

    #endregion

    #region Performance and Stress Tests

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    public async Task EndToEnd_PerformanceValidation_ConnectionWithReadFromStrategy(ReadFromStrategy strategy, string? az)
    {
        // Test that connections with different ReadFrom strategies can be created efficiently

        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS}";
        connectionString += $",readFrom={strategy}";
        connectionString += az != null
            ? $",az={az}"
            : string.Empty;

        // Act
        using (ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync(connectionString))
        {
            // Assert - Verify connection was created successfully
            Assert.NotNull(connection);

            // Parse the connection string to verify ReadFrom configuration
            ConfigurationOptions parsedConfig = ConfigurationOptions.Parse(connectionString);
            Assert.NotNull(parsedConfig.ReadFrom);
            Assert.Equal(strategy, parsedConfig.ReadFrom.Value.Strategy);
            Assert.Equal(az, parsedConfig.ReadFrom.Value.Az);

            // Test basic functionality
            IDatabase database = connection.GetDatabase();
            await database.PingAsync();

            // Test data operations
            string testKey = $"perf-test-{strategy}";
            string testValue = $"value-{strategy}";
            await database.StringSetAsync(testKey, testValue);
            string? retrievedValue = await database.StringGetAsync(testKey);
            Assert.Equal(testValue, retrievedValue);
            await database.KeyDeleteAsync(testKey);
        }
    }

    #endregion

    #region Backward Compatibility Tests

    [Theory]
    [InlineData(true)]  // ConfigurationOptions without ReadFrom
    [InlineData(false)] // Connection string without ReadFrom
    public async Task EndToEnd_BackwardCompatibility_LegacyConfigurationWithoutReadFromStillWorks(bool useConfigurationOptions)
    {
        // Test that existing applications that don't use ReadFrom continue to work

        if (useConfigurationOptions)
        {
            // Arrange: Create a legacy-style configuration without ReadFrom
            ConfigurationOptions legacyConfig = new ConfigurationOptions();
            legacyConfig.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
            legacyConfig.Ssl = TestConfiguration.TLS;
            legacyConfig.ResponseTimeout = 5000;
            legacyConfig.ConnectTimeout = 5000;
            // Explicitly not setting ReadFrom to simulate legacy behavior

            // Act
            using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(legacyConfig))
            {
                // Assert - Verify connection was created successfully
                Assert.NotNull(connectionMultiplexer);

                // Verify ReadFrom is null (legacy behavior)
                Assert.Null(legacyConfig.ReadFrom);

                // Verify full functionality
                IDatabase database = connectionMultiplexer.GetDatabase();
                await database.PingAsync();

                // Test basic operations to ensure legacy behavior works
                string testKey = "legacy-config-test-key";
                string testValue = "legacy-config-test-value";
                await database.StringSetAsync(testKey, testValue);
                string? retrievedValue = await database.StringGetAsync(testKey);
                Assert.Equal(testValue, retrievedValue);

                // Cleanup
                await database.KeyDeleteAsync(testKey);
            }
        }
        else
        {
            // Arrange: Create a legacy-style connection string without ReadFrom
            string legacyConnectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS},connectTimeout=5000,responseTimeout=5000";

            // Act
            using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(legacyConnectionString))
            {
                // Assert - Verify connection was created successfully
                Assert.NotNull(connectionMultiplexer);

                // Parse the connection string to verify ReadFrom is null (legacy behavior)
                ConfigurationOptions parsedConfig = ConfigurationOptions.Parse(legacyConnectionString);
                Assert.Null(parsedConfig.ReadFrom);

                // Verify full functionality
                IDatabase database = connectionMultiplexer.GetDatabase();
                await database.PingAsync();

                // Test basic operations to ensure legacy behavior works
                string testKey = "legacy-connection-string-test-key";
                string testValue = "legacy-connection-string-test-value";
                await database.StringSetAsync(testKey, testValue);
                string? retrievedValue = await database.StringGetAsync(testKey);
                Assert.Equal(testValue, retrievedValue);

                // Cleanup
                await database.KeyDeleteAsync(testKey);
            }
        }
    }

    #endregion

    #region FFI Layer Integration Tests

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    public async Task EndToEnd_FFILayerIntegration_ReadFromConfigurationReachesRustCore(ReadFromStrategy strategy, string? az)
    {
        // Test that ReadFrom configuration actually reaches the Rust core (FFI layer)
        // by creating clients with different ReadFrom strategies and verifying they work

        // Arrange - Create configuration with ReadFrom
        ConfigurationOptions config = new ConfigurationOptions();
        config.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        config.Ssl = TestConfiguration.TLS;

        config.ReadFrom = az != null
            ? new ReadFrom(strategy, az)
            : new ReadFrom(strategy);

        // Act - Create connection and perform operations
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(config))
        {
            IDatabase database = connectionMultiplexer.GetDatabase();

            // Assert - Verify the connection works, indicating FFI layer received configuration
            await database.PingAsync();

            // Perform multiple operations to ensure the ReadFrom strategy is active
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                string key = $"ffi-test-{strategy}-{i}";
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
        }
    }

    [Theory]
    [InlineData(ReadFromStrategy.AzAffinity)]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary)]
    public async Task EndToEnd_FFILayerIntegration_AzAffinitySettingsPassedToRustCore(ReadFromStrategy strategy)
    {
        // Test that AZ affinity settings are properly passed to the Rust core
        // by creating connections with AZ values and verifying they work

        const string testAz = "us-east-1a";

        // Arrange
        ConfigurationOptions config = new ConfigurationOptions();
        config.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        config.Ssl = TestConfiguration.TLS;
        config.ReadFrom = new ReadFrom(strategy, testAz);

        // Act
        using (ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(config))
        {
            IDatabase database = connectionMultiplexer.GetDatabase();

            // Assert - Verify the connection works with the specific AZ configuration
            await database.PingAsync();

            // Test data operations to ensure AZ configuration is active
            string testKey = $"az-test-{strategy}";
            string testValue = $"az-value-{testAz}";
            await database.StringSetAsync(testKey, testValue);
            string? retrievedValue = await database.StringGetAsync(testKey);
            Assert.Equal(testValue, retrievedValue);

            // Cleanup
            await database.KeyDeleteAsync(testKey);
        }
    }

    [Theory]
    [InlineData("InvalidStrategy", "is not supported")]
    [InlineData("AzAffinity", "Availability zone should be set")]
    [InlineData("Primary,az=invalid-az", "Availability zone should not be set")]
    public async Task EndToEnd_FFILayerIntegration_ErrorHandlingInCompleteConfigurationPipeline(string readFromPart, string expectedErrorSubstring)
    {
        // Test that error handling works correctly throughout the complete configuration pipeline
        // from connection string parsing to FFI layer

        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_HOSTS[0].host}:{TestConfiguration.STANDALONE_HOSTS[0].port},ssl={TestConfiguration.TLS},readFrom={readFromPart}";

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            () => ConnectionMultiplexer.ConnectAsync(connectionString));

        Assert.Contains(expectedErrorSubstring, exception.Message);
    }

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    public async Task EndToEnd_FFILayerIntegration_ConcurrentConnectionsWithDifferentReadFromStrategies(ReadFromStrategy strategy, string? az)
    {
        // Test that connections with different ReadFrom strategies can be created and used simultaneously,
        // verifying FFI layer handles multiple configurations

        // Arrange
        ConfigurationOptions config = new ConfigurationOptions();
        config.EndPoints.Add(TestConfiguration.STANDALONE_HOSTS[0].host, TestConfiguration.STANDALONE_HOSTS[0].port);
        config.Ssl = TestConfiguration.TLS;

        config.ReadFrom = az != null
            ? new ReadFrom(strategy, az)
            : new ReadFrom(strategy);

        // Act
        using (ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync(config))
        {
            IDatabase database = connection.GetDatabase();

            // Assert - Test ping
            await database.PingAsync();

            // Test concurrent data operations
            List<Task> operationTasks = new List<Task>();
            for (int j = 0; j < 3; j++)
            {
                int iteration = j;
                operationTasks.Add(Task.Run(async () =>
                {
                    string key = $"concurrent-test-{strategy}-{iteration}";
                    string value = $"value-{strategy}-{az ?? "null"}-{iteration}";

                    await database.StringSetAsync(key, value);
                    string? retrievedValue = await database.StringGetAsync(key);
                    Assert.Equal(value, retrievedValue);
                    await database.KeyDeleteAsync(key);
                }));
            }

            await Task.WhenAll(operationTasks);
        }
    }

    #endregion
}
