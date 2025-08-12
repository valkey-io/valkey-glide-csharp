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

    [Fact]
    public void ReadFromProperty_SetNull_DoesNotThrow()
    {
        // Arrange
        var options = new ConfigurationOptions();

        // Act & Assert - Setting to null should not throw
        options.ReadFrom = null;
        Assert.Null(options.ReadFrom);
    }

    [Fact]
    public void Clone_WithNullReadFrom_ClonesCorrectly()
    {
        // Arrange
        var original = new ConfigurationOptions();
        original.ReadFrom = null;

        // Act
        var cloned = original.Clone();

        // Assert
        Assert.Null(cloned.ReadFrom);
    }

    [Theory]
    [InlineData("readFrom=AzAffinity,az=")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary,az= ")]
    public void Parse_AzAffinityWithEmptyOrWhitespaceAz_ThrowsSpecificException(string connectionString)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("Availability zone cannot be empty or whitespace", exception.Message);
    }

    #region ToString Serialization Tests

    [Theory]
    [InlineData(ReadFromStrategy.Primary, "readFrom=Primary")]
    [InlineData(ReadFromStrategy.PreferReplica, "readFrom=PreferReplica")]
    public void ToString_WithReadFromStrategyWithoutAz_IncludesCorrectFormat(ReadFromStrategy strategy, string expectedSubstring)
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(strategy);

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains(expectedSubstring, result);
    }

    [Theory]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a", "readFrom=AzAffinity,az=us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b", "readFrom=AzAffinityReplicasAndPrimary,az=eu-west-1b")]
    public void ToString_WithReadFromStrategyWithAz_IncludesCorrectFormat(ReadFromStrategy strategy, string az, string expectedSubstring)
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(strategy, az);

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains(expectedSubstring, result);
    }

    [Fact]
    public void ToString_WithPrimaryStrategy_DoesNotIncludeAz()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("readFrom=Primary", result);
        Assert.DoesNotContain("az=", result);
    }

    [Fact]
    public void ToString_WithPreferReplicaStrategy_DoesNotIncludeAz()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.PreferReplica);

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("readFrom=PreferReplica", result);
        Assert.DoesNotContain("az=", result);
    }

    [Theory]
    [InlineData("us-east-1a")]
    [InlineData("eu-west-1b")]
    [InlineData("ap-south-1c")]
    [InlineData("ca-central-1")]
    public void ToString_WithAzAffinityStrategy_IncludesCorrectAzFormat(string azValue)
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, azValue);

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("readFrom=AzAffinity", result);
        Assert.Contains($"az={azValue}", result);
    }

    [Theory]
    [InlineData("us-west-2a")]
    [InlineData("eu-central-1b")]
    [InlineData("ap-northeast-1c")]
    public void ToString_WithAzAffinityReplicasAndPrimaryStrategy_IncludesCorrectAzFormat(string azValue)
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, azValue);

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("readFrom=AzAffinityReplicasAndPrimary", result);
        Assert.Contains($"az={azValue}", result);
    }

    [Fact]
    public void ToString_WithNullReadFrom_DoesNotIncludeReadFromOrAz()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.ReadFrom = null;

        // Act
        var result = options.ToString();

        // Assert
        Assert.DoesNotContain("readFrom=", result);
        Assert.DoesNotContain("az=", result);
    }

    [Fact]
    public void ToString_WithComplexConfiguration_IncludesAllParameters()
    {
        // Arrange
        var options = new ConfigurationOptions();
        options.EndPoints.Add("localhost:6379");
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1a");
        options.Ssl = true;
        options.User = "testuser";
        options.Password = "testpass";

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("localhost:6379", result);
        Assert.Contains("readFrom=AzAffinity", result);
        Assert.Contains("az=us-east-1a", result);
        Assert.Contains("ssl=True", result);
        Assert.Contains("user=testuser", result);
        Assert.Contains("password=testpass", result);
    }

    #endregion

    #region Round-trip Parsing Tests

    [Theory]
    [InlineData("readFrom=Primary")]
    [InlineData("readFrom=PreferReplica")]
    [InlineData("readFrom=AzAffinity,az=us-east-1")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary,az=eu-west-1")]
    public void RoundTrip_ParseToStringToParse_PreservesReadFromConfiguration(string originalConnectionString)
    {
        // Act - First parse
        var options1 = ConfigurationOptions.Parse(originalConnectionString);

        // Act - ToString
        var serialized = options1.ToString();

        // Act - Second parse
        var options2 = ConfigurationOptions.Parse(serialized);

        // Assert
        Assert.Equal(options1.ReadFrom?.Strategy, options2.ReadFrom?.Strategy);
        Assert.Equal(options1.ReadFrom?.Az, options2.ReadFrom?.Az);
    }

    [Fact]
    public void RoundTrip_ComplexConfigurationWithReadFrom_PreservesAllSettings()
    {
        // Arrange
        var originalConnectionString = "localhost:6379,readFrom=AzAffinity,az=us-east-1,ssl=true,user=testuser";

        // Act - First parse
        var options1 = ConfigurationOptions.Parse(originalConnectionString);

        // Act - ToString
        var serialized = options1.ToString();

        // Act - Second parse
        var options2 = ConfigurationOptions.Parse(serialized);

        // Assert ReadFrom configuration
        Assert.Equal(options1.ReadFrom?.Strategy, options2.ReadFrom?.Strategy);
        Assert.Equal(options1.ReadFrom?.Az, options2.ReadFrom?.Az);

        // Assert other configuration is preserved
        Assert.Equal(options1.Ssl, options2.Ssl);
        Assert.Equal(options1.User, options2.User);
        Assert.Equal(options1.EndPoints.Count, options2.EndPoints.Count);
    }

    [Fact]
    public void RoundTrip_ProgrammaticallySetReadFrom_PreservesConfiguration()
    {
        // Arrange
        var options1 = new ConfigurationOptions();
        options1.EndPoints.Add("localhost:6379");
        options1.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, "ap-south-1");

        // Act - ToString
        var serialized = options1.ToString();

        // Act - Parse
        var options2 = ConfigurationOptions.Parse(serialized);

        // Assert
        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, options2.ReadFrom?.Strategy);
        Assert.Equal("ap-south-1", options2.ReadFrom?.Az);
    }

    #endregion

    #region Backward Compatibility Tests

    [Fact]
    public void ToString_ExistingConfigurationWithoutReadFrom_RemainsUnchanged()
    {
        // Arrange
        var options = ConfigurationOptions.Parse("localhost:6379,ssl=true,user=testuser");

        // Act
        var result = options.ToString();

        // Assert - Should not contain ReadFrom parameters
        Assert.DoesNotContain("readFrom=", result);
        Assert.DoesNotContain("az=", result);

        // Assert - Should contain existing parameters
        Assert.Contains("localhost:6379", result);
        Assert.Contains("ssl=True", result);
        Assert.Contains("user=testuser", result);
    }

    [Fact]
    public void ToString_DefaultConfigurationOptions_DoesNotIncludeReadFrom()
    {
        // Arrange
        var options = new ConfigurationOptions();

        // Act
        var result = options.ToString();

        // Assert
        Assert.DoesNotContain("readFrom=", result);
        Assert.DoesNotContain("az=", result);
    }

    [Fact]
    public void RoundTrip_LegacyConnectionString_RemainsCompatible()
    {
        // Arrange - Legacy connection string without ReadFrom
        var legacyConnectionString = "localhost:6379,ssl=true,connectTimeout=5000,user=admin,password=secret";

        // Act - Parse and serialize
        var options = ConfigurationOptions.Parse(legacyConnectionString);
        var serialized = options.ToString();
        var reparsed = ConfigurationOptions.Parse(serialized);

        // Assert - ReadFrom should be null (default behavior)
        Assert.Null(options.ReadFrom);
        Assert.Null(reparsed.ReadFrom);

        // Assert - Other settings preserved
        Assert.Equal(options.Ssl, reparsed.Ssl);
        Assert.Equal(options.User, reparsed.User);
        Assert.Equal(options.Password, reparsed.Password);
    }

    #endregion
}
