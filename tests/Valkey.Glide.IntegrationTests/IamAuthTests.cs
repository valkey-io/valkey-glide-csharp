// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

public class IamAuthTests
{
    private const string TestClusterName = "test-cluster";
    private const string TestRegion = "us-east-1";
    private const string DefaultUsername = "default";

    [Fact]
    public async Task IamAuthenticationWithMockCredentials_Standalone()
    {
        // Create IAM configuration with 5 second refresh interval
        var iamConfig = new IamAuthConfig(
            TestClusterName,
            ServiceType.ElastiCache,
            TestRegion,
            refreshIntervalSeconds: 5
        );

        // Create server credentials with IAM config
        var credentials = new ServerCredentials(DefaultUsername, iamConfig);

        // Start server and create client with IAM authentication
        using var server = new StandaloneServer();
        var config = server.CreateConfigBuilder()
            .WithCredentials(credentials)
            .WithTls(false)
            .Build();

        await using var client = await GlideClient.CreateClient(config);

        // Verify connection with PING
        await client.PingAsync();

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
    public async Task IamAuthenticationWithMockCredentials_Cluster()
    {
        // Create IAM configuration with 5 second refresh interval
        var iamConfig = new IamAuthConfig(
            TestClusterName,
            ServiceType.ElastiCache,
            TestRegion,
            refreshIntervalSeconds: 5
        );

        // Create server credentials with IAM config
        var credentials = new ServerCredentials(DefaultUsername, iamConfig);

        // Start cluster and create client with IAM authentication
        using var server = new ClusterServer();
        var config = server.CreateConfigBuilder()
            .WithCredentials(credentials)
            .WithTls(false)
            .Build();

        await using var client = await GlideClusterClient.CreateClient(config);

        // Verify connection with PING
        await client.PingAsync();

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
        // Create IAM configuration with very short refresh interval (2 seconds)
        var iamConfig = new IamAuthConfig(
            TestClusterName,
            ServiceType.ElastiCache,
            TestRegion,
            refreshIntervalSeconds: 2
        );

        // Create server credentials with IAM config
        var credentials = new ServerCredentials(DefaultUsername, iamConfig);

        // Start server and create client with IAM authentication
        using var server = new StandaloneServer();
        var config = server.CreateConfigBuilder()
            .WithCredentials(credentials)
            .WithTls(false)
            .Build();

        await using var client = await GlideClient.CreateClient(config);

        // Verify initial connection with PING
        await client.PingAsync();

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

        // Create IAM configuration with very short refresh interval (2 seconds)
        var iamConfig = new IamAuthConfig(
            TestClusterName,
            ServiceType.ElastiCache,
            TestRegion,
            refreshIntervalSeconds: 2
        );

        // Create server credentials with IAM config
        var credentials = new ServerCredentials(DefaultUsername, iamConfig);

        // Start cluster and create client with IAM authentication
        using var server = new ClusterServer();
        var config = server.CreateConfigBuilder()
            .WithCredentials(credentials)
            .WithTls(false)
            .Build();

        await using var client = await GlideClusterClient.CreateClient(config);

        // Verify initial connection with PING
        await client.PingAsync();

        // Wait for automatic token refresh (3 seconds to ensure refresh happens)
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Verify client still works after automatic refresh
        await client.StringSetAsync("iam_auto_refresh_key", "iam_auto_refresh_value");
        var value = await client.StringGetAsync("iam_auto_refresh_key");
        Assert.Equal("iam_auto_refresh_value", value.ToString());
    }
}
