// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

///  <summary>
/// Integration tests for circuit breaker functionality.
/// </summary>
[Collection("GlideTests")]
public class CircuitBreakerTests(TestConfiguration config)
{
    #region Public Properties

    public TestConfiguration Config { get; } = config;

    #endregion
    #region Tests

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task CircuitBreaker_DefaultConfig_ConnectsSuccessfully(bool useCluster)
    {
        using var client = await BuildClientAsync(useCluster, new CircuitBreakerConfig());
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task CircuitBreaker_CustomConfig_ConnectsSuccessfully(bool useCluster)
    {
        var cb = new CircuitBreakerConfig()
            .WithWindowSize(TimeSpan.FromSeconds(15))
            .WithFailureRateThreshold(0.6)
            .WithMinErrors(100)
            .WithOpenTimeout(TimeSpan.FromSeconds(10))
            .WithCountTimeouts(true)
            .WithConsecutiveSuccesses(5);

        using var client = await BuildClientAsync(useCluster, cb);
        await AssertConnected(client);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task CircuitBreaker_DoesNotInterfereWithNormalOperation(bool useCluster)
    {
        using var client = await BuildClientAsync(useCluster, new CircuitBreakerConfig());

        for (int i = 0; i < 100; i++)
        {
            await client.SetAsync($"cb_key_{i}", $"value_{i}");
        }

        for (int i = 0; i < 100; i++)
        {
            var result = await client.GetAsync($"cb_key_{i}");
            Assert.Equal($"value_{i}", result);
        }
    }

    [Fact]
    public async Task CircuitBreakerConfig_TripsOnTimeouts_RejectsWithCircuitBreakerException()
    {
        using var server = new StandaloneServer();

        // Configure circuit breaker to count timeouts and with low minimum errors.
        const int minErrors = 5;
        var cb = new CircuitBreakerConfig()
            .WithMinErrors(minErrors)
            .WithCountTimeouts(true);

        // Configure client with short request timeout.
        var requestTimeout = TimeSpan.FromMilliseconds(100);
        await using var client = await GlideClient.CreateClient(
            server.CreateConfigBuilder()
                .WithRequestTimeout(requestTimeout)
                .WithCircuitBreaker(cb)
                .Build());

        await AssertConnected(client);

        // Use a separate client to pause the server for longer than the request timeout.
        await using var adminClient = await GlideClient.CreateClient(
            server.CreateConfigBuilder().Build());

        var pauseDuration = TimeSpan.FromSeconds(5);
        await adminClient.ClientPauseAsync(pauseDuration);

        // All commands with timeout because the server is paused.
        for (int i = 0; i < minErrors; i++)
        {
            _ = await Assert.ThrowsAsync<Errors.TimeoutException>(
                () => client.SetAsync($"cb_trip_{i}", "value"));
        }

        _ = await Assert.ThrowsAsync<Errors.CircuitBreakerException>(client.PingAsync);
    }

    #endregion
    #region Helpers

    private static async Task<BaseClient> BuildClientAsync(bool useCluster, CircuitBreakerConfig cb)
        => useCluster
            ? await GlideClusterClient.CreateClient(
                TestConfiguration.DefaultClusterClientConfig()
                    .WithCircuitBreaker(cb)
                    .Build())
            : await GlideClient.CreateClient(
                TestConfiguration.DefaultClientConfig()
                    .WithCircuitBreaker(cb)
                    .Build());

    #endregion
}
