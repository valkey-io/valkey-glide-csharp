// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

public class IamAuthTests
{
    private const string TestAccessKey = "test_access_key";
    private const string TestSecretKey = "test_secret_key";
    private const string TestSessionToken = "test_session_token";
    private const string TestClusterName = "test-cluster";
    private const string TestRegion = "us-east-1";
    private const string DefaultUsername = "default";

    [Fact]
    public async Task IamAuthenticationWithMockCredentials_Standalone()
    {
        // Save original AWS environment variables
        var originalAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var originalSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        var originalSessionToken = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

        try
        {
            // Set mock AWS credentials
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", TestAccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", TestSecretKey);
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", TestSessionToken);

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
        finally
        {
            // Restore original AWS environment variables
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", originalAccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", originalSecretKey);
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", originalSessionToken);
        }
    }

    [Fact]
    public async Task IamAuthenticationWithMockCredentials_Cluster()
    {
        // Save original AWS environment variables
        var originalAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var originalSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        var originalSessionToken = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

        try
        {
            // Set mock AWS credentials
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", TestAccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", TestSecretKey);
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", TestSessionToken);

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

            // Test basic SET/GET operations
            await client.StringSetAsync("iam_test_key", "iam_test_value");
            var value = await client.StringGetAsync("iam_test_key");
            Assert.Equal("iam_test_value", value.ToString());

            // Verify operations still work after token refresh
            await client.StringSetAsync("iam_test_key2", "iam_test_value2");
            var value2 = await client.StringGetAsync("iam_test_key2");
            Assert.Equal("iam_test_value2", value2.ToString());
        }
        finally
        {
            // Restore original AWS environment variables
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", originalAccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", originalSecretKey);
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", originalSessionToken);
        }
    }

    [Fact]
    public async Task AutomaticIamTokenRefresh_Standalone()
    {
        // Save original AWS environment variables
        var originalAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var originalSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        var originalSessionToken = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

        try
        {
            // Set mock AWS credentials
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", TestAccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", TestSecretKey);
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", TestSessionToken);

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

            // Wait for automatic token refresh (3 seconds to ensure refresh happens)
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Verify client still works after automatic refresh
            await client.StringSetAsync("iam_auto_refresh_key", "iam_auto_refresh_value");
            var value = await client.StringGetAsync("iam_auto_refresh_key");
            Assert.Equal("iam_auto_refresh_value", value.ToString());
        }
        finally
        {
            // Restore original AWS environment variables
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", originalAccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", originalSecretKey);
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", originalSessionToken);
        }
    }

    [Fact]
    public async Task AutomaticIamTokenRefresh_Cluster()
    {
        // Save original AWS environment variables
        var originalAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var originalSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        var originalSessionToken = Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN");

        try
        {
            // Set mock AWS credentials
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", TestAccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", TestSecretKey);
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", TestSessionToken);

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
        finally
        {
            // Restore original AWS environment variables
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", originalAccessKey);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", originalSecretKey);
            Environment.SetEnvironmentVariable("AWS_SESSION_TOKEN", originalSessionToken);
        }
    }
}
