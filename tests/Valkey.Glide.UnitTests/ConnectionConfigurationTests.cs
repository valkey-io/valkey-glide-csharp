// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConnectionConfigurationTests
{
    // Test constants
    private const string Username = "testUsername";
    private const string Password = "testPassword";
    private const string ClusterName = "testClusterName";
    private const string Region = "testRegion";
    private const uint RefreshIntervalSeconds = 600;

    private static readonly byte[] CertificateData1 = [0x30, 0x82, 0x01, 0x00];
    private static readonly byte[] CertificateData2 = [0x30, 0x82, 0x02, 0x00];


    // Authentication & Credentials
    // ----------------------------

    [Fact]
    public void WithAuthentication_UsernamePassword()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithAuthentication(Username, Password);

        StandaloneClientConfiguration config = builder.Build();
        FFI.AuthenticationInfo authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (string)null!));
    }

    [Fact]
    public void WithAuthentication_PasswordOnly()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithAuthentication(Password);

        StandaloneClientConfiguration config = builder.Build();
        FFI.AuthenticationInfo authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Null(authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(null!));
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig()
    {
        IamAuthConfig iamConfig = new(ClusterName, ServiceType.ElastiCache, Region, RefreshIntervalSeconds);
        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithAuthentication(Username, iamConfig);

        StandaloneClientConfiguration config = builder.Build();
        FFI.AuthenticationInfo authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        FFI.IamCredentials iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.ElastiCache, iamCredentials.ServiceType);
        Assert.True(iamCredentials.HasRefreshIntervalSeconds);
        Assert.Equal(600u, iamCredentials.RefreshIntervalSeconds);

        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(null!, iamConfig));
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (IamAuthConfig)null!));
    }

    [Fact]
    public void WithAuthentication_MultipleCalls_LastWins()
    {
        // Password-based authentication last.
        StandaloneClientConfigurationBuilder builder = new();
        IamAuthConfig iamConfig = new(ClusterName, ServiceType.MemoryDB, Region);
        _ = builder.WithAuthentication(Username, iamConfig);
        _ = builder.WithAuthentication(Username, Password);

        StandaloneClientConfiguration config = builder.Build();
        FFI.AuthenticationInfo authenticationInfo = config.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder();
        _ = builder.WithAuthentication(Username, Password);
        _ = builder.WithAuthentication(Username, iamConfig);

        config = builder.Build();
        authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        FFI.IamCredentials iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);
    }

    [Fact]
    public void WithCredentials()
    {
        IamAuthConfig iamConfig = new(ClusterName, ServiceType.MemoryDB, Region);
        ServerCredentials credentials = new(Username, iamConfig);
        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithCredentials(credentials);

        StandaloneClientConfiguration config = builder.Build();
        FFI.AuthenticationInfo authenticationInfo = config.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        FFI.IamCredentials iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);

        _ = Assert.Throws<ArgumentNullException>(() => builder.WithCredentials(null!));
    }

    [Fact]
    public void WithCredentials_MultipleCalls_LastWins()
    {
        IamAuthConfig iamConfig = new(ClusterName, ServiceType.MemoryDB, Region);
        ServerCredentials iamServerCredentials = new(Username, iamConfig);
        ServerCredentials passwordServerCredentials = new(Username, Password);

        // Password-based authentication last.
        StandaloneClientConfigurationBuilder builder = new StandaloneClientConfigurationBuilder();
        _ = builder.WithCredentials(iamServerCredentials);
        _ = builder.WithCredentials(passwordServerCredentials);

        StandaloneClientConfiguration config = builder.Build();
        FFI.AuthenticationInfo authenticationInfo = config.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        // IAM authentication last.
        builder = new();
        _ = builder.WithCredentials(passwordServerCredentials);
        _ = builder.WithCredentials(iamServerCredentials);

        config = builder.Build();
        authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        FFI.IamCredentials iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);
    }

    // Refresh Topology Configuration
    // ------------------------------

    [Fact]
    public void RefreshTopologyFromInitialNodes_Default()
    {
        ClusterClientConfigurationBuilder builder = new();
        ClusterClientConfiguration config = builder.Build();
        Assert.False(config.Request.RefreshTopologyFromInitialNodes);
    }

    [Fact]
    public void RefreshTopologyFromInitialNodes_True()
    {
        ClusterClientConfigurationBuilder builder = new();
        _ = builder.WithRefreshTopologyFromInitialNodes(true);
        ClusterClientConfiguration config = builder.Build();
        Assert.True(config.Request.RefreshTopologyFromInitialNodes);
    }

    [Fact]
    public void RefreshTopologyFromInitialNodes_False()
    {
        ClusterClientConfigurationBuilder builder = new();
        _ = builder.WithRefreshTopologyFromInitialNodes(false);
        ClusterClientConfiguration config = builder.Build();
        Assert.False(config.Request.RefreshTopologyFromInitialNodes);
    }

    // TLS Configuration
    // -----------------

    [Fact]
    public void UseTls()
    {
        StandaloneClientConfigurationBuilder builder = new();

        // Default configuration.
        Assert.False(builder.UseTls);

        // Enable TLS.
        builder.UseTls = true;
        Assert.True(builder.UseTls);

        // Disable TLS.
        builder.UseTls = false;
        Assert.False(builder.UseTls);

        // Enable TLS (no parameter).
        builder.UseTls = true;
        Assert.True(builder.UseTls);

        // Disable insecure TLS.
        builder.UseInsecureTls = true;
        builder.UseTls = false;
        Assert.False(builder.UseTls);
    }

    [Fact]
    public void WithTls()
    {
        StandaloneClientConfigurationBuilder builder = new();

        // Default configuration.
        Assert.False(builder.UseTls);

        // Enable TLS.
        _ = builder.WithTls(true);
        Assert.True(builder.UseTls);

        // Disable TLS.
        _ = builder.WithTls(false);
        Assert.False(builder.UseTls);

        // Enable TLS (no parameter).
        _ = builder.WithTls();
        Assert.True(builder.UseTls);

        // Disable TLS when insecure TLS enabled.
        _ = builder.WithInsecureTls();
        _ = builder.WithTls(false);
        Assert.False(builder.UseTls);
    }

    [Fact]
    public void UseInsecureTls()
    {
        StandaloneClientConfigurationBuilder builder = new();

        // Default configuration.
        Assert.False(builder.UseInsecureTls);

        // Configure insecure TLS without TLS enabled.
        _ = Assert.Throws<ArgumentException>(() => builder.UseInsecureTls = true);
        _ = Assert.Throws<ArgumentException>(() => builder.UseInsecureTls = false);

        builder.UseTls = true;

        // Enable insecure TLS.
        builder.UseInsecureTls = true;
        Assert.True(builder.UseInsecureTls);

        // Disable insecure TLS.
        builder.UseInsecureTls = false;
        Assert.False(builder.UseInsecureTls);

        // Enable insecure TLS (no parameter).
        builder.UseInsecureTls = true;
        Assert.True(builder.UseInsecureTls);

        // Disable TLS when insecure TLS enabled.
        builder.UseTls = false;
        Assert.False(builder.UseInsecureTls);
    }

    [Fact]
    public void WithInsecureTls()
    {
        StandaloneClientConfigurationBuilder builder = new();

        // Default configuration.
        Assert.False(builder.UseInsecureTls);

        // Configure insecure TLS without TLS enabled.
        _ = Assert.Throws<ArgumentException>(() => builder.WithInsecureTls());
        _ = Assert.Throws<ArgumentException>(() => builder.WithInsecureTls(true));
        _ = Assert.Throws<ArgumentException>(() => builder.WithInsecureTls(false));

        _ = builder.WithTls();

        // Enable insecure TLS.
        _ = builder.WithInsecureTls(true);
        Assert.True(builder.UseInsecureTls);

        // Disable insecure TLS.
        _ = builder.WithInsecureTls(false);
        Assert.False(builder.UseInsecureTls);

        // Enable insecure TLS (no parameter).
        _ = builder.WithInsecureTls(true);
        Assert.True(builder.UseInsecureTls);

        // Disable TLS when insecure TLS enabled.
        _ = builder.WithTls(false);
        Assert.False(builder.UseInsecureTls);
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithTrustedCertificate(CertificateData1);
        StandaloneClientConfiguration config = builder.Build();
        Assert.Equivalent(new List<byte[]> { CertificateData1 }, config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_NullThrows()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate((byte[])null!));
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_EmptyThrows()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate([]));
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_MultipleCertificates()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithTrustedCertificate(CertificateData1);
        _ = builder.WithTrustedCertificate(CertificateData2);
        StandaloneClientConfiguration config = builder.Build();

        Assert.Equivalent(new List<byte[]> { CertificateData1, CertificateData2 }, config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path()
    {
        using TempFile tempFile = new(CertificateData1);

        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithTrustedCertificate(tempFile.Path);
        StandaloneClientConfiguration config = builder.Build();

        Assert.Equivalent(new List<byte[]> { CertificateData1 }, config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_FileNotFoundThrows()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = Assert.Throws<FileNotFoundException>(() => builder.WithTrustedCertificate("/nonexistent/path/cert.pem"));
    }

    [Fact]
    public void WithTrustedCertificate_Path_NullThrows()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithTrustedCertificate((string)null!));
    }

    [Fact]
    public void WithTrustedCertificate_Path_MultipleCertificates()
    {
        using TempFile tempFile1 = new(CertificateData1);
        using TempFile tempFile2 = new(CertificateData2);

        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithTrustedCertificate(tempFile1.Path);
        _ = builder.WithTrustedCertificate(tempFile2.Path);
        StandaloneClientConfiguration config = builder.Build();

        Assert.Equivalent(new List<byte[]> { CertificateData1, CertificateData2 }, config.Request.RootCertificates);
    }

    // Pub/Sub Reconciliation Interval
    // -------------------------------

    [Fact]
    public void PubSubReconciliationInterval_Default()
    {
        StandaloneClientConfigurationBuilder builder = new();
        Assert.Null(builder.Build().Request.PubSubReconciliationInterval);
    }

    [Fact]
    public void PubSubReconciliationInterval_PositiveSucceeds()
    {
        TimeSpan interval = TimeSpan.FromSeconds(30);
        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithPubSubReconciliationInterval(interval);

        Assert.Equal(interval.TotalMilliseconds, builder.Build().Request.PubSubReconciliationInterval!.Value.TotalMilliseconds);
    }

    [Fact]
    public void PubSubReconciliationInterval_NegativeThrows()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = Assert.Throws<ArgumentException>(() => builder.WithPubSubReconciliationInterval(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void PubSubReconciliationInterval_ZeroThrows()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = Assert.Throws<ArgumentException>(() => builder.WithPubSubReconciliationInterval(TimeSpan.Zero));
    }

    // Connection Retry Strategy
    // -------------------------

    [Fact]
    public void WithConnectionRetryStrategy_Standalone_SetsValues()
    {
        StandaloneClientConfigurationBuilder builder = new();
        _ = builder.WithConnectionRetryStrategy(5, 100, 2);

        StandaloneClientConfiguration config = builder.Build();
        RetryStrategy strategy = config.Request.RetryStrategy!.Value;

        Assert.Equal(5u, strategy.NumberOfRetries);
        Assert.Equal(100u, strategy.Factor);
        Assert.Equal(2u, strategy.ExponentBase);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Cluster_SetsValues()
    {
        ClusterClientConfigurationBuilder builder = new();
        _ = builder.WithConnectionRetryStrategy(3, 50, 2);

        ClusterClientConfiguration config = builder.Build();
        RetryStrategy strategy = config.Request.RetryStrategy!.Value;

        Assert.Equal(3u, strategy.NumberOfRetries);
        Assert.Equal(50u, strategy.Factor);
        Assert.Equal(2u, strategy.ExponentBase);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Cluster_WithJitter_SetsValues()
    {
        ClusterClientConfigurationBuilder builder = new();
        _ = builder.WithConnectionRetryStrategy(new RetryStrategy(4, 200, 3, jitterPercent: 10));

        ClusterClientConfiguration config = builder.Build();
        RetryStrategy strategy = config.Request.RetryStrategy!.Value;

        Assert.Equal(4u, strategy.NumberOfRetries);
        Assert.Equal(200u, strategy.Factor);
        Assert.Equal(3u, strategy.ExponentBase);
        Assert.Equal(10u, strategy.JitterPercent);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Cluster_Default_IsNull()
    {
        ClusterClientConfigurationBuilder builder = new();
        ClusterClientConfiguration config = builder.Build();
        Assert.Null(config.Request.RetryStrategy);
    }
}
