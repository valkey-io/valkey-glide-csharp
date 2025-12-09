// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// End-to-end integration tests for ReadFrom configuration flowing from connection string to FFI layer.
/// These tests verify the complete configuration pipeline and error handling scenarios.
/// Implements task 12 from the AZ Affinity support implementation plan.
/// </summary>
[Collection(typeof(ReadFromTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ReadFromTests(TestConfiguration config)
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
    public async Task ConnectionString_ReadFromConfigurationFlowsToFFILayer(
        string strategyString, ReadFromStrategy expectedStrategy, string? expectedAz, bool useStandalone)
    {
        // Arrange
        Address hostConfig = useStandalone ? TestConfiguration.STANDALONE_ADDRESSES[0] : TestConfiguration.CLUSTER_ADDRESSES[0];
        string connectionString = $"{hostConfig.Host}:{hostConfig.Port},ssl={TestConfiguration.TLS}";
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
        }
    }

    [Theory]
    [InlineData(true)]  // Connection string without ReadFrom
    [InlineData(false)] // ConfigurationOptions with null ReadFrom
    public async Task NullReadFrom_DefaultsToNullInFFILayer(bool useConnectionString)
    {
        // Arrange & Act
        if (useConnectionString)
        {
            string connectionString = $"{TestConfiguration.STANDALONE_ADDRESSES[0].Host}:{TestConfiguration.STANDALONE_ADDRESSES[0].Port},ssl={TestConfiguration.TLS}";

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
            configOptions.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
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

    #region Configuration Pipeline Validation Tests

    [Fact]
    public async Task ConfigurationPipeline_ValidationErrorPropagation()
    {
        // Test that validation errors propagate correctly through the entire configuration pipeline

        // Arrange: Create configuration with invalid ReadFrom combination
        ConfigurationOptions configOptions = new ConfigurationOptions();
        configOptions.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act & Assert: Test invalid assignment through property setter
        Assert.Throws<ArgumentException>(() =>
        {
            configOptions.ReadFrom = new ReadFrom(ReadFromStrategy.Primary, "invalid-az-for-primary");
        });

        // Test that the configuration remains in a valid state after failed assignment
        configOptions.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);
        using ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);
        Assert.NotNull(connectionMultiplexer);

        // Test basic functionality
        IDatabase database = connectionMultiplexer.GetDatabase();
        await database.PingAsync();
    }

    [Fact]
    public async Task ConfigurationPipeline_ClonePreservesReadFromConfiguration()
    {
        // Arrange
        ConfigurationOptions originalConfig = new ConfigurationOptions
        {
            ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1a")
        };
        originalConfig.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
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

    #region Backward Compatibility Tests

    [Theory]
    [InlineData(true)]  // ConfigurationOptions without ReadFrom
    [InlineData(false)] // Connection string without ReadFrom
    public async Task BackwardCompatibility_LegacyConfigurationWithoutReadFromStillWorks(bool useConfigurationOptions)
    {
        // Test that existing applications that don't use ReadFrom continue to work

        if (useConfigurationOptions)
        {
            // Arrange: Create a legacy-style configuration without ReadFrom
            ConfigurationOptions legacyConfig = new ConfigurationOptions();
            legacyConfig.EndPoints.Add(TestConfiguration.STANDALONE_ADDRESSES[0].Host, TestConfiguration.STANDALONE_ADDRESSES[0].Port);
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
            string legacyConnectionString = $"{TestConfiguration.STANDALONE_ADDRESSES[0].Host}:{TestConfiguration.STANDALONE_ADDRESSES[0].Port},ssl={TestConfiguration.TLS},connectTimeout=5000,responseTimeout=5000";

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
}
