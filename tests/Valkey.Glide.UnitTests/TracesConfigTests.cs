// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class TracesConfigTests
{
    #region Data

    // TODO #215: Move to TestUtils.Data folder.
    public static TheoryData<string> ValidEndpoints =>
        [
            "http://localhost:4321",                    // HTTP endpoint
            "https://otel-collector.example.com:4318",  // HTTPS endpoint
            "file:///tmp/traces.txt",                   // Unix-style file URI
            @"file://C:\Users\runner\traces.txt",       // Windows-style file URI
        ];

    // TODO #215: Move to TestUtils.Data folder.
    public static TheoryData<string> InvalidEndpoints =>
        [
            (string)null!,        // null
            "",                   // empty
            "\t",                 // whitespace only
            "not-a-url",          // no scheme
            "://missing-scheme",  // malformed scheme
            "just some text",     // plain text
        ];

    #endregion
    #region Tests

    [Theory]
    [MemberData(nameof(InvalidEndpoints))]
    public void WithEndpoint_WithInvalidEndpoint_ThrowsArgumentException(string endpoint)
    {
        var builder = TracesConfig.CreateBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithEndpoint(endpoint));
    }

    [Theory]
    [MemberData(nameof(ValidEndpoints))]
    public void WithEndpoint_WithValidEndpoint_Succeeds(string endpoint)
    {
        var config = TracesConfig.CreateBuilder()
            .WithEndpoint(endpoint)
            .Build();

        Assert.Equal(endpoint, config.Endpoint);
    }

    [Fact]
    public void WithSamplePercentage_WithInvalidPercentage_ThrowsArgumentException()
    {
        var builder = TracesConfig.CreateBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithSamplePercentage(101));
    }

    [Fact]
    public void Build_WithoutEndpoint_ThrowsInvalidOperationException()
    {
        var builder = TracesConfig.CreateBuilder();
        _ = Assert.Throws<InvalidOperationException>(builder.Build);
    }

    [Fact]
    public void Build_WithoutSamplePercentage_Succeeds()
    {
        var endpoint = ValidEndpoints.First();
        var config = TracesConfig.CreateBuilder()
            .WithEndpoint(endpoint)
            .Build();

        Assert.Equal(endpoint, config.Endpoint);
        Assert.Equal(TracesConfig.DefaultSamplePercentage, config.SamplePercentage);
    }

    [Fact]
    public void Build_WithSamplePercentage_Succeeds()
    {
        var endpoint = ValidEndpoints.First();
        var samplePercentage = 50u;

        var config = TracesConfig.CreateBuilder()
            .WithEndpoint(endpoint)
            .WithSamplePercentage(samplePercentage)
            .Build();

        Assert.Equal(endpoint, config.Endpoint);
        Assert.Equal(samplePercentage, config.SamplePercentage);
    }

    #endregion
}
