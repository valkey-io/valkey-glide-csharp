// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection;

using Xunit;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConnectionMultiplexerReadFromMappingTests
{
    [Fact]
    public void CreateClientConfigBuilder_WithReadFromPrimary_MapsCorrectly()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);
        var clusterBuilder = InvokeCreateClientConfigBuilder<ClusterClientConfigurationBuilder>(options);

        // Assert
        var standaloneConfig = standaloneBuilder.Build();
        var clusterConfig = clusterBuilder.Build();

        Assert.Equal(ReadFromStrategy.Primary, standaloneConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Null(standaloneConfig.Request.ReadFrom!.Value.Az);

        Assert.Equal(ReadFromStrategy.Primary, clusterConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Null(clusterConfig.Request.ReadFrom!.Value.Az);
    }

    [Fact]
    public void CreateClientConfigBuilder_WithReadFromPreferReplica_MapsCorrectly()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.PreferReplica);

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);
        var clusterBuilder = InvokeCreateClientConfigBuilder<ClusterClientConfigurationBuilder>(options);

        // Assert
        var standaloneConfig = standaloneBuilder.Build();
        var clusterConfig = clusterBuilder.Build();

        Assert.Equal(ReadFromStrategy.PreferReplica, standaloneConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Null(standaloneConfig.Request.ReadFrom!.Value.Az);

        Assert.Equal(ReadFromStrategy.PreferReplica, clusterConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Null(clusterConfig.Request.ReadFrom!.Value.Az);
    }

    [Fact]
    public void CreateClientConfigBuilder_WithReadFromAzAffinity_MapsCorrectly()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1a");

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);
        var clusterBuilder = InvokeCreateClientConfigBuilder<ClusterClientConfigurationBuilder>(options);

        // Assert
        var standaloneConfig = standaloneBuilder.Build();
        var clusterConfig = clusterBuilder.Build();

        Assert.Equal(ReadFromStrategy.AzAffinity, standaloneConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Equal("us-east-1a", standaloneConfig.Request.ReadFrom!.Value.Az);

        Assert.Equal(ReadFromStrategy.AzAffinity, clusterConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Equal("us-east-1a", clusterConfig.Request.ReadFrom!.Value.Az);
    }

    [Fact]
    public void CreateClientConfigBuilder_WithReadFromAzAffinityReplicasAndPrimary_MapsCorrectly()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b");

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);
        var clusterBuilder = InvokeCreateClientConfigBuilder<ClusterClientConfigurationBuilder>(options);

        // Assert
        var standaloneConfig = standaloneBuilder.Build();
        var clusterConfig = clusterBuilder.Build();

        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, standaloneConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Equal("eu-west-1b", standaloneConfig.Request.ReadFrom!.Value.Az);

        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, clusterConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Equal("eu-west-1b", clusterConfig.Request.ReadFrom!.Value.Az);
    }

    [Fact]
    public void CreateClientConfigBuilder_WithNullReadFrom_HandlesCorrectly()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = null;

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);
        var clusterBuilder = InvokeCreateClientConfigBuilder<ClusterClientConfigurationBuilder>(options);

        // Assert
        var standaloneConfig = standaloneBuilder.Build();
        var clusterConfig = clusterBuilder.Build();

        Assert.Null(standaloneConfig.Request.ReadFrom);
        Assert.Null(clusterConfig.Request.ReadFrom);
    }

    [Fact]
    public void CreateClientConfigBuilder_ReadFromFlowsToConnectionConfig()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "ap-south-1");

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);
        var standaloneConfig = standaloneBuilder.Build();

        // Assert - Verify ReadFrom flows through to ConnectionConfig
        var connectionConfig = standaloneConfig.ToRequest();
        Assert.NotNull(connectionConfig.ReadFrom);
        Assert.Equal(ReadFromStrategy.AzAffinity, connectionConfig.ReadFrom.Value.Strategy);
        Assert.Equal("ap-south-1", connectionConfig.ReadFrom.Value.Az);
    }

    [Fact]
    public void CreateClientConfigBuilder_ReadFromFlowsToFfiLayer()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.PreferReplica);

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);
        var standaloneConfig = standaloneBuilder.Build();

        // Assert - Verify ReadFrom flows through to FFI layer
        var connectionConfig = standaloneConfig.ToRequest();

        // We can't directly access the FFI structure, but we can verify it's properly set in the ConnectionConfig
        // The ToFfi() method will properly marshal the ReadFrom to the FFI layer
        using var ffiConfig = connectionConfig.ToFfi();

        // The fact that ToFfi() doesn't throw and the connectionConfig has the correct ReadFrom
        // indicates that the mapping is working correctly
        Assert.NotNull(connectionConfig.ReadFrom);
        Assert.Equal(ReadFromStrategy.PreferReplica, connectionConfig.ReadFrom.Value.Strategy);
        Assert.Null(connectionConfig.ReadFrom.Value.Az);
    }

    [Theory]
    [InlineData(ReadFromStrategy.Primary, null)]
    [InlineData(ReadFromStrategy.PreferReplica, null)]
    [InlineData(ReadFromStrategy.AzAffinity, "us-west-2")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-central-1")]
    public void CreateClientConfigBuilder_AllReadFromStrategies_MapCorrectly(ReadFromStrategy strategy, string? az)
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = az != null ? new ReadFrom(strategy, az) : new ReadFrom(strategy);

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);
        var clusterBuilder = InvokeCreateClientConfigBuilder<ClusterClientConfigurationBuilder>(options);

        // Assert
        var standaloneConfig = standaloneBuilder.Build();
        var clusterConfig = clusterBuilder.Build();

        // Verify standalone configuration
        Assert.Equal(strategy, standaloneConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Equal(az, standaloneConfig.Request.ReadFrom!.Value.Az);

        // Verify cluster configuration
        Assert.Equal(strategy, clusterConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Equal(az, clusterConfig.Request.ReadFrom!.Value.Az);
    }

    [Fact]
    public void CreateClientConfigBuilder_WithComplexConfiguration_MapsReadFromCorrectly()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.EndPoints.Add("localhost:6379");
        options.Ssl = true;
        options.User = "testuser";
        options.Password = "testpass";
        options.ClientName = "TestClient";
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1a");

        // Act
        var standaloneBuilder = InvokeCreateClientConfigBuilder<StandaloneClientConfigurationBuilder>(options);

        // Assert
        var standaloneConfig = standaloneBuilder.Build();

        // Verify ReadFrom is correctly mapped alongside other configuration
        Assert.Equal(ReadFromStrategy.AzAffinity, standaloneConfig.Request.ReadFrom!.Value.Strategy);
        Assert.Equal("us-east-1a", standaloneConfig.Request.ReadFrom!.Value.Az);

        // Verify other configuration is also correctly mapped (basic checks)
        Assert.NotNull(standaloneConfig.Request.TlsMode);
        Assert.NotNull(standaloneConfig.Request.AuthenticationInfo);
        Assert.Equal("TestClient", standaloneConfig.Request.ClientName);
    }

    /// <summary>
    /// Helper method to invoke the private CreateClientConfigBuilder method using reflection
    /// </summary>
    private static T InvokeCreateClientConfigBuilder<T>(ConfigurationOptions configuration)
        where T : ClientConfigurationBuilder<T>, new()
    {
        var method = typeof(ConnectionMultiplexer).GetMethod("CreateClientConfigBuilder",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        var genericMethod = method.MakeGenericMethod(typeof(T));
        var result = genericMethod.Invoke(null, [configuration]);

        Assert.NotNull(result);
        return (T)result;
    }
}
