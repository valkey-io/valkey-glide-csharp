// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

public class OpenTelemetryTests
{
    private static readonly string Endpoint = "http://localhost:4321";

    [Fact]
    public void Init_WithValidConfig_Succeeds()
    {
        var tracesConfig = TracesConfig.CreateBuilder()
            .WithEndpoint(Endpoint)
            .Build();
        var config = OpenTelemetryConfig.CreateBuilder()
            .WithTraces(tracesConfig)
            .Build();

        OpenTelemetry.Init(config);

        Assert.True(OpenTelemetry.IsInitialized());
    }
}
