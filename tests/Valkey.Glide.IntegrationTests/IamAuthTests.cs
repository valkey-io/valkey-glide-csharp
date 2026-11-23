// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Assertions;

namespace Valkey.Glide.IntegrationTests;

// IAM authentication requires TLS (valkey-io/valkey-glide#7023).
public class IamAuthTests(TlsServerFixture tlsServerFixture) : IClassFixture<TlsServerFixture>
{
    private const string TestClusterName = "test-cluster";
    private const string TestRegion = "us-east-1";
    private const string DefaultUsername = "default";

    /// <summary>
    /// Refresh interval long enough that scheduled refreshes do not run during a test.
    /// </summary>
    private const uint LongRefreshIntervalSeconds = 3600;

    /// <summary>
    /// Bound on waiting for a provider invocation.
    /// </summary>
    private static readonly TimeSpan SignalTimeout = TimeSpan.FromSeconds(15);

    [Fact]
    public async Task IamAuthenticationWithMockCredentials_Standalone()
    {
        var credentials = CreateCredentials(refreshIntervalSeconds: 5);

        var server = tlsServerFixture.StandaloneServer;
        var config = CreateStandaloneConfig(server, credentials);

        await using var client = await GlideClient.CreateClient(config);

        await AssertConnected(client);

        // Verify connection after token refresh
        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        await AssertConnected(client);
    }

    [Fact]
    public async Task IamAuthenticationWithMockCredentials_Cluster()
    {
        var credentials = CreateCredentials(refreshIntervalSeconds: 5);

        var server = tlsServerFixture.ClusterServer;
        var config = CreateClusterConfig(server, credentials);

        await using var client = await GlideClusterClient.CreateClient(config);

        // Verify connection
        await AssertConnected(client);

        // Verify RefreshIamTokenAsync()
        await client.RefreshIamTokenAsync();

        // Test basic SET/GET operations
        await client.SetAsync("iam_test_key", "iam_test_value");
        var value = await client.GetAsync("iam_test_key");
        Assert.Equal("iam_test_value", value.ToString());

        // Verify operations still work after token refresh
        await client.SetAsync("iam_test_key2", "iam_test_value2");
        var value2 = await client.GetAsync("iam_test_key2");
        Assert.Equal("iam_test_value2", value2.ToString());
    }

    [Fact]
    public async Task AutomaticIamTokenRefresh_Standalone()
    {
        var credentials = CreateCredentials(refreshIntervalSeconds: 2);

        var server = tlsServerFixture.StandaloneServer;
        var config = CreateStandaloneConfig(server, credentials);

        await using var client = await GlideClient.CreateClient(config);

        // Verify initial connection
        await AssertConnected(client);

        // Verify RefreshIamTokenAsync()
        await client.RefreshIamTokenAsync();

        // Wait for automatic token refresh (3 seconds to ensure refresh happens)
        await Task.Delay(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        // Verify client still works after automatic refresh
        await client.SetAsync("iam_auto_refresh_key", "iam_auto_refresh_value");
        var value = await client.GetAsync("iam_auto_refresh_key");
        Assert.Equal("iam_auto_refresh_value", value.ToString());
    }

    [Fact]
    public async Task AutomaticIamTokenRefresh_Cluster()
    {
        var credentials = CreateCredentials(refreshIntervalSeconds: 2);

        var server = tlsServerFixture.ClusterServer;
        var config = CreateClusterConfig(server, credentials);

        await using var client = await GlideClusterClient.CreateClient(config);

        // Verify initial connection
        await AssertConnected(client);

        // Wait for automatic token refresh (3 seconds to ensure refresh happens)
        await Task.Delay(TimeSpan.FromSeconds(3), TestContext.Current.CancellationToken);

        // Verify client still works after automatic refresh
        await client.SetAsync("iam_auto_refresh_key", "iam_auto_refresh_value");
        var value = await client.GetAsync("iam_auto_refresh_key");
        Assert.Equal("iam_auto_refresh_value", value.ToString());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CustomIamProviderOnStartupAndManualRefresh(bool clusterMode)
    {
        var provider = new RecordingCredentialsProvider();
        var credentials = CreateCredentials(LongRefreshIntervalSeconds, provider.GetCredentialsAsync);

        await using var client = await CreateClientAsync(clusterMode, credentials);

        // The provider supplies the credentials for the initial token.
        Assert.True(provider.Calls >= 1);
        await AssertConnected(client);

        // A manual refresh invokes the provider again.
        Task nextCall = provider.ArmNextCall();
        await client.RefreshIamTokenAsync();
        await nextCall.WaitAsync(SignalTimeout, TestContext.Current.CancellationToken);

        await AssertConnected(client);
    }

    [Fact]
    public async Task CustomIamProviderOnScheduledRefresh()
    {
        var provider = new RecordingCredentialsProvider();
        var credentials = CreateCredentials(refreshIntervalSeconds: 1, provider.GetCredentialsAsync);

        await using var client = await GlideClient.CreateClient(CreateStandaloneConfig(tlsServerFixture.StandaloneServer, credentials));

        // A scheduled refresh invokes the provider after startup.
        Task nextCall = provider.ArmNextCall();
        await nextCall.WaitAsync(SignalTimeout, TestContext.Current.CancellationToken);

        await client.SetAsync("iam_provider_key", "iam_provider_value");
        var value = await client.GetAsync("iam_provider_key");
        Assert.Equal("iam_provider_value", value.ToString());
    }

    [Fact]
    public async Task CustomIamProviderFailureOnStartup()
    {
        // Default-chain credentials are available from the environment, but must not be used.
        var provider = new RecordingCredentialsProvider { Fail = true };
        var credentials = CreateCredentials(LongRefreshIntervalSeconds, provider.GetCredentialsAsync);
        var config = CreateStandaloneConfig(tlsServerFixture.StandaloneServer, credentials);

        var exception = await Assert.ThrowsAsync<Errors.ConnectionException>(() => GlideClient.CreateClient(config));
        Assert.Equal(1, provider.Calls);
        AssertSanitizedProviderFailure(exception);
    }

    [Fact]
    public async Task CustomIamProviderFailureOnManualRefresh()
    {
        var provider = new RecordingCredentialsProvider();
        var credentials = CreateCredentials(LongRefreshIntervalSeconds, provider.GetCredentialsAsync);

        await using var client = await GlideClient.CreateClient(CreateStandaloneConfig(tlsServerFixture.StandaloneServer, credentials));

        provider.Fail = true;
        var exception = await Assert.ThrowsAsync<Errors.RequestException>(client.RefreshIamTokenAsync);
        AssertSanitizedProviderFailure(exception);

        // The provider is invoked again on the next refresh.
        provider.Fail = false;
        Task nextCall = provider.ArmNextCall();
        await client.RefreshIamTokenAsync();
        await nextCall.WaitAsync(SignalTimeout, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CustomIamProviderWithoutSessionTokenAndWithExpiry()
    {
        var provider = new RecordingCredentialsProvider(
            new AwsCredentials("test_access_key", "test_secret_key", sessionToken: null, DateTimeOffset.UtcNow.AddHours(1)));
        var credentials = CreateCredentials(LongRefreshIntervalSeconds, provider.GetCredentialsAsync);

        await using var client = await GlideClient.CreateClient(CreateStandaloneConfig(tlsServerFixture.StandaloneServer, credentials));

        Assert.True(provider.Calls >= 1);
        await AssertConnected(client);
    }

    /// <summary>
    /// Asserts that a provider failure surfaces only the binding's fixed message.
    /// </summary>
    private static void AssertSanitizedProviderFailure(Exception exception)
    {
        Assert.Contains("Custom credentials provider failed", exception.Message);
        Assert.DoesNotContain(RecordingCredentialsProvider.FailureMessage, exception.Message);
    }

    private static IamAuthConfig CreateIamConfig(uint refreshIntervalSeconds, GlideCredentialProvider? provider = null) =>
        new(TestClusterName, ServiceType.ElastiCache, TestRegion, refreshIntervalSeconds) { CredentialsProvider = provider };

    private static ServerCredentials CreateCredentials(uint refreshIntervalSeconds, GlideCredentialProvider? provider = null) =>
        new(DefaultUsername, CreateIamConfig(refreshIntervalSeconds, provider));

    private static ConnectionConfiguration.StandaloneClientConfiguration CreateStandaloneConfig(
        StandaloneServer server, ServerCredentials credentials) =>
        server.CreateConfigBuilder()
            .WithCredentials(credentials)
            .Build();

    private static ConnectionConfiguration.ClusterClientConfiguration CreateClusterConfig(
        ClusterServer server, ServerCredentials credentials) =>
        server.CreateConfigBuilder()
            .WithCredentials(credentials)
            .Build();

    private async Task<BaseClient> CreateClientAsync(bool clusterMode, ServerCredentials credentials) =>
        clusterMode
            ? await GlideClusterClient.CreateClient(CreateClusterConfig(tlsServerFixture.ClusterServer, credentials))
            : await GlideClient.CreateClient(CreateStandaloneConfig(tlsServerFixture.StandaloneServer, credentials));

    /// <summary>
    /// Credentials provider that records invocations and can be armed to signal the next one or to fail.
    /// </summary>
    private sealed class RecordingCredentialsProvider(AwsCredentials? credentials = null)
    {
        /// <summary>
        /// Message of the exception thrown by a failing provider. It must not reach the client.
        /// </summary>
        public const string FailureMessage = "PROVIDER_FAILURE_SECRET";

        private readonly AwsCredentials _credentials =
            credentials ?? new AwsCredentials("test_access_key", "test_secret_key", "test_session_token");

        private int _calls;
        private TaskCompletionSource? _nextCall;

        /// <summary>
        /// Number of provider invocations.
        /// </summary>
        public int Calls => Volatile.Read(ref _calls);

        /// <summary>
        /// Whether invocations fail.
        /// </summary>
        public bool Fail
        {
            get => Volatile.Read(ref field);
            set => Volatile.Write(ref field, value);
        }

        /// <summary>
        /// Returns a task that completes on the next provider invocation.
        /// </summary>
        public Task ArmNextCall()
        {
            var nextCall = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            Volatile.Write(ref _nextCall, nextCall);
            return nextCall.Task;
        }

        public Task<AwsCredentials> GetCredentialsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _ = Interlocked.Increment(ref _calls);
            _ = (Interlocked.Exchange(ref _nextCall, null)?.TrySetResult());

            return Fail
                ? Task.FromException<AwsCredentials>(new InvalidOperationException(FailureMessage))
                : Task.FromResult(_credentials);
        }
    }
}
