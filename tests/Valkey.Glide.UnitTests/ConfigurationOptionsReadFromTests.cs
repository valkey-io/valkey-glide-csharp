// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Xunit;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConfigurationOptionsReadFromTests
{
    [Theory]
    [InlineData("readFrom=Primary", ReadFromStrategy.Primary, null)]
    [InlineData("readFrom=PreferReplica", ReadFromStrategy.PreferReplica, null)]
    [InlineData("readFrom=primary", ReadFromStrategy.Primary, null)]
    [InlineData("readFrom=preferreplica", ReadFromStrategy.PreferReplica, null)]
    public void Parse_ValidReadFromWithoutAz_SetsCorrectStrategy(string connectionString, ReadFromStrategy expectedStrategy, string? expectedAz)
    {
        // Act
        var options = ConfigurationOptions.Parse(connectionString);

        // Assert
        Assert.NotNull(options.ReadFrom);
        Assert.Equal(expectedStrategy, options.ReadFrom.Value.Strategy);
        Assert.Equal(expectedAz, options.ReadFrom.Value.Az);
    }

    [Theory]
    [InlineData("readFrom=AzAffinity,az=us-east-1", ReadFromStrategy.AzAffinity, "us-east-1")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary,az=eu-west-1", ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1")]
    [InlineData("readFrom=azaffinity,az=us-west-2", ReadFromStrategy.AzAffinity, "us-west-2")]
    [InlineData("readFrom=azaffinityreplicasandprimary,az=ap-south-1", ReadFromStrategy.AzAffinityReplicasAndPrimary, "ap-south-1")]
    public void Parse_ValidReadFromWithAz_SetsCorrectStrategyAndAz(string connectionString, ReadFromStrategy expectedStrategy, string expectedAz)
    {
        // Act
        var options = ConfigurationOptions.Parse(connectionString);

        // Assert
        Assert.NotNull(options.ReadFrom);
        Assert.Equal(expectedStrategy, options.ReadFrom.Value.Strategy);
        Assert.Equal(expectedAz, options.ReadFrom.Value.Az);
    }

    [Fact]
    public void Parse_AzAndReadFromInDifferentOrder_ParsesCorrectly()
    {
        // Act
        var options1 = ConfigurationOptions.Parse("az=us-east-1,readFrom=AzAffinity");
        var options2 = ConfigurationOptions.Parse("readFrom=AzAffinityReplicasAndPrimary,az=eu-west-1");

        // Assert
        Assert.NotNull(options1.ReadFrom);
        Assert.Equal("us-east-1", options1.ReadFrom.Value.Az);
        Assert.Equal(ReadFromStrategy.AzAffinity, options1.ReadFrom.Value.Strategy);

        Assert.NotNull(options2.ReadFrom);
        Assert.Equal("eu-west-1", options2.ReadFrom.Value.Az);
        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, options2.ReadFrom.Value.Strategy);
    }

    [Theory]
    [InlineData("readFrom=")]
    [InlineData("readFrom= ")]
    [InlineData("readFrom=\t")]
    public void Parse_EmptyReadFromValue_ThrowsArgumentException(string connectionString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("requires a ReadFrom strategy value", exception.Message);
    }

    [Theory]
    [InlineData("readFrom=InvalidStrategy")]
    [InlineData("readFrom=Unknown")]
    [InlineData("readFrom=123")]
    public void Parse_InvalidReadFromStrategy_ThrowsArgumentException(string connectionString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("is not supported", exception.Message);
        Assert.Contains("Primary, PreferReplica, AzAffinity, AzAffinityReplicasAndPrimary", exception.Message);
    }

    [Theory]
    [InlineData("readFrom=AzAffinity")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary")]
    public void Parse_AzAffinityStrategiesWithoutAz_ThrowsArgumentException(string connectionString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("Availability zone should be set", exception.Message);
    }

    [Theory]
    [InlineData("readFrom=Primary,az=us-east-1")]
    [InlineData("readFrom=PreferReplica,az=eu-west-1")]
    public void Parse_NonAzAffinityStrategiesWithAz_ThrowsArgumentException(string connectionString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("should not be set", exception.Message);
    }

    [Theory]
    [InlineData("az=")]
    [InlineData("az= ")]
    [InlineData("az=\t")]
    public void Parse_EmptyAzValue_ThrowsArgumentException(string connectionString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void ReadFromProperty_SetValidConfiguration_DoesNotThrow()
    {
        // Arrange
        var options = new ConfigurationOptions();
        var readFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1");

        // Act & Assert
        options.ReadFrom = readFrom;
        Assert.Equal(ReadFromStrategy.AzAffinity, options.ReadFrom.Value.Strategy);
        Assert.Equal("us-east-1", options.ReadFrom.Value.Az);
    }

    [Fact]
    public void ReadFromProperty_SetAzAffinityWithoutAz_ThrowsArgumentException()
    {
        // Arrange
        var options = new ConfigurationOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            // This should throw because ReadFrom constructor validates AZ requirement
            var readFrom = new ReadFrom(ReadFromStrategy.AzAffinity);
            options.ReadFrom = readFrom;
        });
        Assert.Contains("Availability zone should be set", exception.Message);
    }

    [Fact]
    public void ReadFromProperty_SetPrimaryWithAz_ThrowsArgumentException()
    {
        // Arrange
        var options = new ConfigurationOptions();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            // This should throw because ReadFrom constructor validates AZ requirement
            var readFrom = new ReadFrom(ReadFromStrategy.Primary, "us-east-1");
            options.ReadFrom = readFrom;
        });
        Assert.Contains("could be set only when using", exception.Message);
    }

    [Fact]
    public void Parse_ComplexConnectionStringWithReadFrom_ParsesAllParameters()
    {
        // Arrange
        var connectionString = "localhost:6379,readFrom=AzAffinity,az=us-east-1,ssl=true,user=testuser,password=testpass";

        // Act
        var options = ConfigurationOptions.Parse(connectionString);

        // Assert
        Assert.NotNull(options.ReadFrom);
        Assert.Equal(ReadFromStrategy.AzAffinity, options.ReadFrom.Value.Strategy);
        Assert.Equal("us-east-1", options.ReadFrom.Value.Az);
        Assert.True(options.Ssl);
        Assert.Equal("testuser", options.User);
        Assert.Equal("testpass", options.Password);
        Assert.Single(options.EndPoints);
    }

    [Fact]
    public void Clone_WithReadFromSet_ClonesReadFromCorrectly()
    {
        // Arrange
        var original = ConfigurationOptions.Parse("readFrom=AzAffinity,az=us-east-1");

        // Act
        var cloned = original.Clone();

        // Assert
        Assert.NotNull(cloned.ReadFrom);
        Assert.Equal(original.ReadFrom.Value.Strategy, cloned.ReadFrom.Value.Strategy);
        Assert.Equal(original.ReadFrom.Value.Az, cloned.ReadFrom.Value.Az);
    }

    [Fact]
    public void ToString_WithReadFromAndAz_IncludesInConnectionString()
    {
        // Arrange
        var options = ConfigurationOptions.Parse("localhost:6379,readFrom=AzAffinity,az=us-east-1");

        // Act
        var connectionString = options.ToString();

        // Assert
        Assert.Contains("readFrom=AzAffinity", connectionString);
        Assert.Contains("az=us-east-1", connectionString);
    }

    [Fact]
    public void ToString_WithReadFromWithoutAz_IncludesOnlyReadFrom()
    {
        // Arrange
        var options = ConfigurationOptions.Parse("localhost:6379,readFrom=Primary");

        // Act
        var connectionString = options.ToString();

        // Assert
        Assert.Contains("readFrom=Primary", connectionString);
        Assert.DoesNotContain("az=", connectionString);
    }
}
