// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

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

    /// <summary>Standalone and cluster server addresses, so mapping tests run against both.</summary>
    public static TheoryData<bool> UseStandalone => [true, false];

    private static Address AddressFor(bool useStandalone)
        => useStandalone ? TestConfiguration.STANDALONE_ADDRESS : TestConfiguration.CLUSTER_ADDRESS;

    [Theory]
    [InlineData(true, ReadFromStrategy.Primary, null)]
    [InlineData(true, ReadFromStrategy.PreferReplica, null)]
    [InlineData(true, ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(true, ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    [InlineData(true, ReadFromStrategy.AzAffinityAllNodes, "us-east-1a")]
    [InlineData(false, ReadFromStrategy.Primary, null)]
    [InlineData(false, ReadFromStrategy.PreferReplica, null)]
    [InlineData(false, ReadFromStrategy.AzAffinity, "us-west-2a")]
    [InlineData(false, ReadFromStrategy.AzAffinityReplicasAndPrimary, "ap-south-1c")]
    [InlineData(false, ReadFromStrategy.AzAffinityAllNodes, "ap-south-1c")]
    public async Task ConfigurationOptions_ReadFrom_MapsToRawConfig(bool useStandalone, ReadFromStrategy strategy, string? az)
    {
        // Arrange
        Address address = AddressFor(useStandalone);
        var configOptions = new ConfigurationOptions
        {
            ReadFrom = az != null ? new ReadFrom(strategy, az) : new ReadFrom(strategy)
        };
        configOptions.EndPoints.Add(address.Host, address.Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        await using ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(strategy, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(az, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
    }

    [Theory]
    [MemberData(nameof(UseStandalone))]
    public async Task ConfigurationOptions_NullReadFrom_DefaultsToNull(bool useStandalone)
    {
        // Arrange
        Address address = AddressFor(useStandalone);
        var configOptions = new ConfigurationOptions { ReadFrom = null };
        configOptions.EndPoints.Add(address.Host, address.Port);
        configOptions.Ssl = TestConfiguration.TLS;

        // Act
        await using ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configOptions);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.False(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
    }

    [Theory]
    [InlineData(true, "readFrom=Primary", ReadFromStrategy.Primary, null)]
    [InlineData(true, "readFrom=AzAffinity,az=us-east-1a", ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData(false, "readFrom=Primary", ReadFromStrategy.Primary, null)]
    [InlineData(false, "readFrom=AzAffinity,az=eu-west-1b", ReadFromStrategy.AzAffinity, "eu-west-1b")]
    public async Task ConnectionString_ReadFrom_MapsToRawConfig(bool useStandalone, string readFromSegment, ReadFromStrategy strategy, string? az)
    {
        // Arrange
        Address address = AddressFor(useStandalone);
        string connectionString = $"{address},{readFromSegment},ssl={TestConfiguration.TLS}";

        // Act
        await using ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(strategy, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(az, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
    }

    [Theory]
    [InlineData("readFrom=Primary", ReadFromStrategy.Primary, null)]
    [InlineData("readFrom=PreferReplica", ReadFromStrategy.PreferReplica, null)]
    [InlineData("readFrom=AllNodes", ReadFromStrategy.AllNodes, null)]
    [InlineData("readFrom=AzAffinity,az=us-east-1a", ReadFromStrategy.AzAffinity, "us-east-1a")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary,az=eu-west-1b", ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b")]
    [InlineData("readFrom=AzAffinityAllNodes,az=ap-south-1c", ReadFromStrategy.AzAffinityAllNodes, "ap-south-1c")]
    public async Task EndToEnd_ReadFromConfiguration_FlowsFromConnectionStringToConnectionConfig(string readFromSegment, ReadFromStrategy strategy, string? az)
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_ADDRESS},ssl={TestConfiguration.TLS},{readFromSegment}";

        // Act
        await using var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.True(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
        Assert.Equal(strategy, connectionMultiplexer.RawConfig.ReadFrom.Value.Strategy);
        Assert.Equal(az, connectionMultiplexer.RawConfig.ReadFrom.Value.Az);
    }

    [Fact]
    public async Task EndToEnd_NoReadFromConfiguration_DefaultsToNull()
    {
        // Arrange
        string connectionString = $"{TestConfiguration.STANDALONE_ADDRESS},ssl={TestConfiguration.TLS}";

        // Act
        await using ConnectionMultiplexer connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(connectionString);

        // Assert
        Assert.NotNull(connectionMultiplexer);
        Assert.NotNull(connectionMultiplexer.RawConfig);
        Assert.False(connectionMultiplexer.RawConfig.ReadFrom.HasValue);
    }
}
