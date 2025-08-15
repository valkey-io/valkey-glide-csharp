// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Xunit;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConfigurationOptionsReadFromTests
{
    [Theory]
    [InlineData("readFrom=Primary", ReadFromStrategy.Primary, null)]
    [InlineData("readFrom=PreferReplica", ReadFromStrategy.PreferReplica, null)]
    [InlineData("readFrom=AzAffinity,Az=us-east-1", ReadFromStrategy.AzAffinity, "us-east-1")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary,Az=us-east-1", ReadFromStrategy.AzAffinityReplicasAndPrimary, "us-east-1")]
    [InlineData("readFrom=primary", ReadFromStrategy.Primary, null)]
    [InlineData("readFrom=azaffinity,Az=us-east-1", ReadFromStrategy.AzAffinity, "us-east-1")]
    [InlineData("READFrom=PRIMARY", ReadFromStrategy.Primary, null)]
    [InlineData("READFrom=AZAFFINITY,AZ=us-east-1", ReadFromStrategy.AzAffinity, "us-east-1")]
    public void Parse_ValidReadFromWithoutAz_SetsCorrectStrategy(string connectionString, ReadFromStrategy expectedStrategy, string? expectedAz)
    {
        // Act
        ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);

        // Assert
        Assert.NotNull(options.ReadFrom);
        Assert.Equal(expectedStrategy, options.ReadFrom.Value.Strategy);
        Assert.Equal(expectedAz, options.ReadFrom.Value.Az);
    }

    [Theory]
    [InlineData("readFrom=AzAffinity,az=us-east-1", ReadFromStrategy.AzAffinity, "us-east-1")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary,az=eu-west-1", ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1")]
    public void Parse_ValidReadFromWithAz_SetsCorrectStrategyAndAz(string connectionString, ReadFromStrategy expectedStrategy, string expectedAz)
    {
        // Act
        ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);

        // Assert
        Assert.NotNull(options.ReadFrom);
        Assert.Equal(expectedStrategy, options.ReadFrom.Value.Strategy);
        Assert.Equal(expectedAz, options.ReadFrom.Value.Az);
    }

    [Theory]
    [InlineData("readFrom=")]
    [InlineData("readFrom= ")]
    [InlineData("readFrom=\t")]
    public void Parse_EmptyReadFromValue_ThrowsArgumentException(string connectionString)
    {
        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("ReadFrom strategy cannot be empty", exception.Message);
    }

    [Fact]
    public void Parse_InvalidReadFromStrategy_ThrowsArgumentException()
    {
        // Arrange
        string connectionString = "readFrom=InvalidStrategy";

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("is not supported", exception.Message);
        Assert.Contains("Primary, PreferReplica, AzAffinity, AzAffinityReplicasAndPrimary", exception.Message);
    }

    [Theory]
    [InlineData("readFrom=AzAffinity")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary")]
    public void Parse_AzAffinityStrategiesWithoutAz_ThrowsArgumentException(string connectionString)
    {
        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("Availability zone cannot be empty or whitespace", exception.Message);
    }

    [Theory]
    [InlineData("az=")]
    [InlineData("az= ")]
    [InlineData("az=\t")]
    public void Parse_EmptyAzValue_ThrowsArgumentException(string connectionString)
    {
        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("cannot be empty or whitespace", exception.Message.ToLower());
    }

    [Fact]
    public void ReadFromProperty_SetValidConfiguration_DoesNotThrow()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        ReadFrom readFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1");

        // Act & Assert
        options.ReadFrom = readFrom;
        Assert.Equal(ReadFromStrategy.AzAffinity, options.ReadFrom.Value.Strategy);
        Assert.Equal("us-east-1", options.ReadFrom.Value.Az);
    }

    [Fact]
    public void ReadFromProperty_SetAzAffinityWithoutAz_ThrowsArgumentException()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            // This should throw because ReadFrom constructor validates AZ requirement
            ReadFrom readFrom = new ReadFrom(ReadFromStrategy.AzAffinity);
            options.ReadFrom = readFrom;
        });
        Assert.Contains("Availability zone should be set", exception.Message);
    }

    [Fact]
    public void ReadFromProperty_SetPrimaryWithAz_ThrowsArgumentException()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            // This should throw because ReadFrom constructor validates AZ requirement
            ReadFrom readFrom = new ReadFrom(ReadFromStrategy.Primary, "us-east-1");
            options.ReadFrom = readFrom;
        });
        Assert.Contains("could be set only when using", exception.Message);
    }

    [Fact]
    public void Parse_ComplexConnectionStringWithReadFrom_ParsesAllParameters()
    {
        // Arrange
        string connectionString = "localhost:6379,readFrom=AzAffinity,az=us-east-1,ssl=true,user=testuser,password=testpass";

        // Act
        ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);

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
        ConfigurationOptions original = ConfigurationOptions.Parse("readFrom=AzAffinity,az=us-east-1");

        // Act
        ConfigurationOptions cloned = original.Clone();

        // Assert
        Assert.NotNull(cloned.ReadFrom);
        Assert.Equal(original.ReadFrom.Value.Strategy, cloned.ReadFrom.Value.Strategy);
        Assert.Equal(original.ReadFrom.Value.Az, cloned.ReadFrom.Value.Az);
    }

    [Fact]
    public void ToString_WithReadFromAndAz_IncludesInConnectionString()
    {
        // Arrange
        ConfigurationOptions options = ConfigurationOptions.Parse("localhost:6379,readFrom=AzAffinity,az=us-east-1");

        // Act
        string connectionString = options.ToString();

        // Assert
        Assert.Contains("readFrom=AzAffinity", connectionString);
        Assert.Contains("az=us-east-1", connectionString);
    }

    [Fact]
    public void ToString_WithReadFromWithoutAz_IncludesOnlyReadFrom()
    {
        // Arrange
        ConfigurationOptions options = ConfigurationOptions.Parse("localhost:6379,readFrom=Primary");

        // Act
        string connectionString = options.ToString();

        // Assert
        Assert.Contains("readFrom=Primary", connectionString);
        Assert.DoesNotContain("az=", connectionString);
    }

    [Fact]
    public void ReadFromProperty_SetNull_DoesNotThrow()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();

        // Act & Assert - Setting to null should not throw
        options.ReadFrom = null;
        Assert.Null(options.ReadFrom);
    }

    [Fact]
    public void Clone_WithNullReadFrom_ClonesCorrectly()
    {
        // Arrange
        ConfigurationOptions original = new ConfigurationOptions();
        original.ReadFrom = null;

        // Act
        ConfigurationOptions cloned = original.Clone();

        // Assert
        Assert.Null(cloned.ReadFrom);
    }

    [Theory]
    [InlineData("readFrom=AzAffinity,az=")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary,az= ")]
    public void Parse_AzAffinityWithEmptyOrWhitespaceAz_ThrowsSpecificException(string connectionString)
    {
        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => ConfigurationOptions.Parse(connectionString));
        Assert.Contains("Availability zone cannot be empty or whitespace", exception.Message);
    }

    [Theory]
    [InlineData(ReadFromStrategy.Primary, "readFrom=Primary")]
    [InlineData(ReadFromStrategy.PreferReplica, "readFrom=PreferReplica")]
    public void ToString_WithReadFromStrategyWithoutAz_IncludesCorrectFormat(ReadFromStrategy strategy, string expectedSubstring)
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(strategy);

        // Act
        string result = options.ToString();

        // Assert
        Assert.Contains(expectedSubstring, result);
    }

    [Theory]
    [InlineData(ReadFromStrategy.AzAffinity, "us-east-1a", "readFrom=AzAffinity,az=us-east-1a")]
    [InlineData(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1b", "readFrom=AzAffinityReplicasAndPrimary,az=eu-west-1b")]
    public void ToString_WithReadFromStrategyWithAz_IncludesCorrectFormat(ReadFromStrategy strategy, string az, string expectedSubstring)
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(strategy, az);

        // Act
        string result = options.ToString();

        // Assert
        Assert.Contains(expectedSubstring, result);
    }



    [Theory]
    [InlineData("us-east-1a")]
    [InlineData("eu-west-1b")]
    public void ToString_WithAzAffinityStrategy_IncludesCorrectAzFormat(string azValue)
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, azValue);

        // Act
        string result = options.ToString();

        // Assert
        Assert.Contains("readFrom=AzAffinity", result);
        Assert.Contains($"az={azValue}", result);
    }

    [Theory]
    [InlineData("us-west-2a")]
    [InlineData("eu-central-1b")]
    public void ToString_WithAzAffinityReplicasAndPrimaryStrategy_IncludesCorrectAzFormat(string azValue)
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, azValue);

        // Act
        string result = options.ToString();

        // Assert
        Assert.Contains("readFrom=AzAffinityReplicasAndPrimary", result);
        Assert.Contains($"az={azValue}", result);
    }

    [Fact]
    public void ToString_WithNullReadFrom_DoesNotIncludeReadFromOrAz()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        options.ReadFrom = null;

        // Act
        string result = options.ToString();

        // Assert
        Assert.DoesNotContain("readFrom=", result);
        Assert.DoesNotContain("az=", result);
    }

    [Fact]
    public void ToString_WithComplexConfiguration_IncludesAllParameters()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        options.EndPoints.Add("localhost:6379");
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1a");
        options.Ssl = true;
        options.User = "testuser";
        options.Password = "testpass";

        // Act
        string result = options.ToString();

        // Assert
        Assert.Contains("localhost:6379", result);
        Assert.Contains("readFrom=AzAffinity", result);
        Assert.Contains("az=us-east-1a", result);
        Assert.Contains("ssl=True", result);
        Assert.Contains("user=testuser", result);
        Assert.Contains("password=testpass", result);
    }

    [Theory]
    [InlineData("readFrom=Primary")]
    [InlineData("readFrom=PreferReplica")]
    [InlineData("readFrom=AzAffinity,az=us-east-1")]
    [InlineData("readFrom=AzAffinityReplicasAndPrimary,az=eu-west-1")]
    public void RoundTrip_ParseToStringToParse_PreservesReadFromConfiguration(string originalConnectionString)
    {
        // Act - First parse
        ConfigurationOptions options1 = ConfigurationOptions.Parse(originalConnectionString);

        // Act - ToString
        string serialized = options1.ToString();

        // Act - Second parse
        ConfigurationOptions options2 = ConfigurationOptions.Parse(serialized);

        // Assert
        Assert.Equal(options1.ReadFrom?.Strategy, options2.ReadFrom?.Strategy);
        Assert.Equal(options1.ReadFrom?.Az, options2.ReadFrom?.Az);
    }

    [Fact]
    public void RoundTrip_ComplexConfigurationWithReadFrom_PreservesAllSettings()
    {
        // Arrange
        string originalConnectionString = "localhost:6379,readFrom=AzAffinity,az=us-east-1,ssl=true,user=testuser";

        // Act - First parse
        ConfigurationOptions options1 = ConfigurationOptions.Parse(originalConnectionString);

        // Act - ToString
        string serialized = options1.ToString();

        // Act - Second parse
        ConfigurationOptions options2 = ConfigurationOptions.Parse(serialized);

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
        ConfigurationOptions options1 = new ConfigurationOptions();
        options1.EndPoints.Add("localhost:6379");
        options1.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, "ap-south-1");

        // Act - ToString
        string serialized = options1.ToString();

        // Act - Parse
        ConfigurationOptions options2 = ConfigurationOptions.Parse(serialized);

        // Assert
        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, options2.ReadFrom?.Strategy);
        Assert.Equal("ap-south-1", options2.ReadFrom?.Az);
    }

    [Fact]
    public void ToString_ExistingConfigurationWithoutReadFrom_RemainsUnchanged()
    {
        // Arrange
        ConfigurationOptions options = ConfigurationOptions.Parse("localhost:6379,ssl=true,user=testuser");

        // Act
        string result = options.ToString();

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
        ConfigurationOptions options = new ConfigurationOptions();

        // Act
        string result = options.ToString();

        // Assert
        Assert.DoesNotContain("readFrom=", result);
        Assert.DoesNotContain("az=", result);
    }

    [Fact]
    public void RoundTrip_LegacyConnectionString_RemainsCompatible()
    {
        // Arrange - Legacy connection string without ReadFrom
        string legacyConnectionString = "localhost:6379,ssl=true,connectTimeout=5000,user=admin,password=secret";

        // Act - Parse and serialize
        ConfigurationOptions options = ConfigurationOptions.Parse(legacyConnectionString);
        string serialized = options.ToString();
        ConfigurationOptions reparsed = ConfigurationOptions.Parse(serialized);

        // Assert - ReadFrom should be null (default behavior)
        Assert.Null(options.ReadFrom);
        Assert.Null(reparsed.ReadFrom);

        // Assert - Other settings preserved
        Assert.Equal(options.Ssl, reparsed.Ssl);
        Assert.Equal(options.User, reparsed.User);
        Assert.Equal(options.Password, reparsed.Password);
    }

    [Fact]
    public void ReadFromProperty_SetValidPrimaryStrategy_DoesNotThrow()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        ReadFrom readFrom = new ReadFrom(ReadFromStrategy.Primary);

        // Act & Assert
        options.ReadFrom = readFrom;
        Assert.Equal(ReadFromStrategy.Primary, options.ReadFrom.Value.Strategy);
        Assert.Null(options.ReadFrom.Value.Az);
    }

    [Fact]
    public void ReadFromProperty_SetValidPreferReplicaStrategy_DoesNotThrow()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        ReadFrom readFrom = new ReadFrom(ReadFromStrategy.PreferReplica);

        // Act & Assert
        options.ReadFrom = readFrom;
        Assert.Equal(ReadFromStrategy.PreferReplica, options.ReadFrom.Value.Strategy);
        Assert.Null(options.ReadFrom.Value.Az);
    }

    [Fact]
    public void ReadFromProperty_SetValidAzAffinityStrategy_DoesNotThrow()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        ReadFrom readFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-east-1");

        // Act & Assert
        options.ReadFrom = readFrom;
        Assert.Equal(ReadFromStrategy.AzAffinity, options.ReadFrom.Value.Strategy);
        Assert.Equal("us-east-1", options.ReadFrom.Value.Az);
    }

    [Fact]
    public void ReadFromProperty_SetValidAzAffinityReplicasAndPrimaryStrategy_DoesNotThrow()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        ReadFrom readFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, "eu-west-1");

        // Act & Assert
        options.ReadFrom = readFrom;
        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, options.ReadFrom.Value.Strategy);
        Assert.Equal("eu-west-1", options.ReadFrom.Value.Az);
    }



    [Fact]
    public void ReadFromProperty_SetMultipleTimes_UpdatesCorrectly()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();

        // Act & Assert - Set Primary first
        options.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);
        Assert.Equal(ReadFromStrategy.Primary, options.ReadFrom.Value.Strategy);
        Assert.Null(options.ReadFrom.Value.Az);

        // Act & Assert - Change to AzAffinity
        options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-west-2");
        Assert.Equal(ReadFromStrategy.AzAffinity, options.ReadFrom.Value.Strategy);
        Assert.Equal("us-west-2", options.ReadFrom.Value.Az);

        // Act & Assert - Change back to null
        options.ReadFrom = null;
        Assert.Null(options.ReadFrom);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ReadFromProperty_SetAzAffinityWithEmptyOrWhitespaceAz_ThrowsArgumentException(string azValue)
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, azValue);
        });
        Assert.Contains("Availability zone cannot be empty or whitespace", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void ReadFromProperty_SetAzAffinityReplicasAndPrimaryWithEmptyOrWhitespaceAz_ThrowsArgumentException(string azValue)
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
        {
            options.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, azValue);
        });
        Assert.Contains("Availability zone cannot be empty or whitespace", exception.Message);
    }





    [Fact]
    public void Clone_ModifyingClonedReadFrom_DoesNotAffectOriginal()
    {
        // Arrange
        ConfigurationOptions original = new ConfigurationOptions();
        original.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);

        // Act
        ConfigurationOptions cloned = original.Clone();
        cloned.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinity, "us-west-2");

        // Assert - Original should remain unchanged
        Assert.NotNull(original.ReadFrom);
        Assert.Equal(ReadFromStrategy.Primary, original.ReadFrom.Value.Strategy);
        Assert.Null(original.ReadFrom.Value.Az);

        // Assert - Cloned should have new value
        Assert.NotNull(cloned.ReadFrom);
        Assert.Equal(ReadFromStrategy.AzAffinity, cloned.ReadFrom.Value.Strategy);
        Assert.Equal("us-west-2", cloned.ReadFrom.Value.Az);
    }

    [Fact]
    public void Clone_WithComplexConfigurationIncludingReadFrom_PreservesAllSettings()
    {
        // Arrange
        ConfigurationOptions original = new ConfigurationOptions();
        original.EndPoints.Add("localhost:6379");
        original.ReadFrom = new ReadFrom(ReadFromStrategy.AzAffinityReplicasAndPrimary, "ap-south-1");
        original.Ssl = true;
        original.User = "testuser";
        original.Password = "testpass";
        original.ConnectTimeout = 5000;
        original.ResponseTimeout = 3000;

        // Act
        ConfigurationOptions cloned = original.Clone();

        // Assert ReadFrom configuration
        Assert.NotNull(cloned.ReadFrom);
        Assert.Equal(ReadFromStrategy.AzAffinityReplicasAndPrimary, cloned.ReadFrom.Value.Strategy);
        Assert.Equal("ap-south-1", cloned.ReadFrom.Value.Az);

        // Assert other configuration is preserved
        Assert.Equal(original.Ssl, cloned.Ssl);
        Assert.Equal(original.User, cloned.User);
        Assert.Equal(original.Password, cloned.Password);
        Assert.Equal(original.ConnectTimeout, cloned.ConnectTimeout);
        Assert.Equal(original.ResponseTimeout, cloned.ResponseTimeout);
        Assert.Equal(original.EndPoints.Count, cloned.EndPoints.Count);
    }

    [Fact]
    public void ReadFromProperty_DefaultValue_IsNull()
    {
        // Arrange & Act
        ConfigurationOptions options = new ConfigurationOptions();

        // Assert
        Assert.Null(options.ReadFrom);
    }

    [Fact]
    public void ReadFromProperty_AfterSettingToNonNull_CanBeSetBackToNull()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        options.ReadFrom = new ReadFrom(ReadFromStrategy.Primary);

        // Act
        options.ReadFrom = null;

        // Assert
        Assert.Null(options.ReadFrom);
    }

    [Fact]
    public void ReadFromProperty_NullValue_DoesNotAffectToString()
    {
        // Arrange
        ConfigurationOptions options = new ConfigurationOptions();
        options.EndPoints.Add("localhost:6379");
        options.ReadFrom = null;

        // Act
        string result = options.ToString();

        // Assert
        Assert.DoesNotContain("readFrom=", result);
        Assert.DoesNotContain("az=", result);
        Assert.Contains("localhost:6379", result);
    }

    [Fact]
    public void ReadFromProperty_NullValue_DoesNotAffectClone()
    {
        // Arrange
        ConfigurationOptions original = new ConfigurationOptions();
        original.EndPoints.Add("localhost:6379");
        original.Ssl = true;
        original.ReadFrom = null;

        // Act
        ConfigurationOptions cloned = original.Clone();

        // Assert
        Assert.Null(cloned.ReadFrom);
        Assert.Equal(original.Ssl, cloned.Ssl);
        Assert.Equal(original.EndPoints.Count, cloned.EndPoints.Count);
    }


}
