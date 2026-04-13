// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Client;

namespace Valkey.Glide.IntegrationTests;

public class IamAuthTests
{
    private const string TestClusterName = "test-cluster";
    private const string TestRegion = "us-east-1";
    private const string DefaultUsername = "default";

    private static IamAuthConfig CreateIamConfig(uint refreshIntervalSeconds) =>
        new(TestClusterName, ServiceType.ElastiCache, TestRegion, refreshIntervalSeconds);

    private static ServerCredentials CreateCredentials(uint refreshIntervalSeconds) =>
        new(DefaultUsername, CreateIamConfig(refreshIntervalSeconds));

    private static ConnectionConfiguration.StandaloneClientConfiguration CreateStandaloneConfig(
        StandaloneServer server, ServerCredentials credentials) =>
        server.CreateConfigBuilder()
            .WithCredentials(credentials)
            .WithTls(false)
            .Build();

    private static ConnectionConfiguration.ClusterClientConfiguration CreateClusterConfig(
        ClusterServer server, ServerCredentials credentials) =>
        server.CreateConfigBuilder()
            .WithCredentials(credentials)
            .WithTls(false)
            .Build();

    [Fact]
    public async Task IamAuthenticationWithMockCredentials_Standalone()
    {
        var credentials = CreateCredentials(refreshIntervalSeconds: 5);

        using var server = new StandaloneServer();
        var config = CreateStandaloneConfig(server, credentials);

        await using var client = await GlideClient.CreateClient(config);

        await AssertConnected(client);

        // Verify connection after token refresh
        await Task.Delay(TimeSpan.FromSeconds(5));
        await AssertConnected(client);
    }

    [Fact]
    public async Task IamAuthenticationWithMockCredentials_Cluster()
    {
        var credentials = CreateCredentials(refreshIntervalSeconds: 5);

        using var server = new ClusterServer();
        var config = CreateClusterConfig(server, credentials);

        await using var client = await GlideClusterClient.CreateClient(config);

        // Verify connection
        await AssertConnected(client);

        // Verify RefreshIamTokenAsync()
        await client.RefreshIamTokenAsync();

        // Test basic SET/GET operations
        await client.StringSetAsync("iam_test_key", "iam_test_value");
        var value = await client.StringGetAsync("iam_test_key");
        Assert.Equal("iam_test_value", value.ToString());

        // Verify operations still work after token refresh
        await client.StringSetAsync("iam_test_key2", "iam_test_value2");
        var value2 = await client.StringGetAsync("iam_test_key2");
        Assert.Equal("iam_test_value2", value2.ToString());
    }

    [Fact]
    public async Task AutomaticIamTokenRefresh_Standalone()
    {
        var credentials = CreateCredentials(refreshIntervalSeconds: 2);

        using var server = new StandaloneServer();
        var config = CreateStandaloneConfig(server, credentials);

        await using var client = await GlideClient.CreateClient(config);

        // Verify initial connection
        await AssertConnected(client);

        // Verify RefreshIamTokenAsync()
        await client.RefreshIamTokenAsync();

        // Wait for automatic token refresh (3 seconds to ensure refresh happens)
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Verify client still works after automatic refresh
        await client.StringSetAsync("iam_auto_refresh_key", "iam_auto_refresh_value");
        var value = await client.StringGetAsync("iam_auto_refresh_key");
        Assert.Equal("iam_auto_refresh_value", value.ToString());
    }

    [Fact]
    public async Task AutomaticIamTokenRefresh_Cluster()
    {
        var credentials = CreateCredentials(refreshIntervalSeconds: 2);

        using var server = new ClusterServer();
        var config = CreateClusterConfig(server, credentials);

        await using var client = await GlideClusterClient.CreateClient(config);

        // Verify initial connection
        await AssertConnected(client);

        // Wait for automatic token refresh (3 seconds to ensure refresh happens)
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Verify client still works after automatic refresh
        await client.StringSetAsync("iam_auto_refresh_key", "iam_auto_refresh_value");
        var value = await client.StringGetAsync("iam_auto_refresh_key");
        Assert.Equal("iam_auto_refresh_value", value.ToString());
    }
}
