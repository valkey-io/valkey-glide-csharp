// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConnectionConfigurationTests
{
    // Authentication constants.
    private const string Username = "testUsername";
    private const string Password = "testPassword";
    private const string ClusterName = "testClusterName";
    private const string Region = "testRegion";
    private const uint RefreshIntervalSeconds = 600;

    // Certificate data constants.
    private static readonly byte[] CertificateData1 = [0x30, 0x82, 0x01, 0x00];
    private static readonly byte[] CertificateData2 = [0x30, 0x82, 0x02, 0x00];

    // Connection retry strategy constants.
    private static readonly uint NumberOfRetries = 3u;
    private static readonly uint Factor = 50u;
    private static readonly uint ExponentBase = 2u;
    private static readonly uint JitterPercent = 10u;

    // Authentication & Credentials
    // ----------------------------

    [Fact]
    public void WithAuthentication_UsernamePassword()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, Password);

        var config = builder.Build();
        var authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (string)null!));
    }

    [Fact]
    public void WithAuthentication_PasswordOnly()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Password);

        var config = builder.Build();
        var authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Null(authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(null!));
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, RefreshIntervalSeconds);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, iamConfig);

        var config = builder.Build();
        var authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
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
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, iamConfig)
            .WithAuthentication(Username, Password);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, Password)
            .WithAuthentication(Username, iamConfig);

        config = builder.Build();
        authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);
    }

    [Fact]
    public void WithCredentials()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);
        var credentials = new ServerCredentials(Username, iamConfig);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithCredentials(credentials);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);

        _ = Assert.Throws<ArgumentNullException>(() => builder.WithCredentials(null!));
    }

    [Fact]
    public void WithCredentials_MultipleCalls_LastWins()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);
        var iamServerCredentials = new ServerCredentials(Username, iamConfig);
        var passwordServerCredentials = new ServerCredentials(Username, Password);

        // Password-based authentication last.
        var builder = new StandaloneClientConfigurationBuilder()
            .WithCredentials(iamServerCredentials)
            .WithCredentials(passwordServerCredentials);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder()
            .WithCredentials(passwordServerCredentials)
            .WithCredentials(iamServerCredentials);

        config = builder.Build();
        authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
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
        var builder = new ClusterClientConfigurationBuilder();
        var config = builder.Build();
        Assert.False(config.Request.RefreshTopologyFromInitialNodes);
    }

    [Fact]
    public void RefreshTopologyFromInitialNodes_True()
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithRefreshTopologyFromInitialNodes(true);
        var config = builder.Build();
        Assert.True(config.Request.RefreshTopologyFromInitialNodes);
    }

    [Fact]
    public void RefreshTopologyFromInitialNodes_False()
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithRefreshTopologyFromInitialNodes(false);
        var config = builder.Build();
        Assert.False(config.Request.RefreshTopologyFromInitialNodes);
    }

    // TLS Configuration
    // -----------------

    [Fact]
    public void UseTls()
    {
        var builder = new StandaloneClientConfigurationBuilder();

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
        var builder = new StandaloneClientConfigurationBuilder();

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
        var builder = new StandaloneClientConfigurationBuilder();

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
        var builder = new StandaloneClientConfigurationBuilder();

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
        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(CertificateData1);
        var config = builder.Build();
        Assert.Equivalent(new List<byte[]> { CertificateData1 }, config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_NullThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate((byte[])null!));
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_EmptyThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate([]));
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_MultipleCertificates()
    {
        var config = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(CertificateData1)
            .WithTrustedCertificate(CertificateData2)
            .Build();

        Assert.Equivalent(
            new List<byte[]> { CertificateData1, CertificateData2 },
            config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path()
    {
        using var tempFile = new TempFile(CertificateData1);

        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(tempFile.Path);
        var config = builder.Build();

        Assert.Equivalent(new List<byte[]> { CertificateData1 }, config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_FileNotFoundThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<FileNotFoundException>(() => builder.WithTrustedCertificate("/nonexistent/path/cert.pem"));
    }

    [Fact]
    public void WithTrustedCertificate_Path_NullThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithTrustedCertificate((string)null!));
    }

    [Fact]
    public void WithTrustedCertificate_Path_MultipleCertificates()
    {
        using var tempFile1 = new TempFile(CertificateData1);
        using var tempFile2 = new TempFile(CertificateData2);

        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(tempFile1.Path)
            .WithTrustedCertificate(tempFile2.Path);
        var config = builder.Build();

        Assert.Equivalent(new List<byte[]> { CertificateData1, CertificateData2 }, config.Request.RootCertificates);
    }

    // Pub/Sub Reconciliation Interval
    // -------------------------------

    [Fact]
    public void PubSubReconciliationInterval_Default()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Null(builder.Build().Request.PubSubReconciliationInterval);
    }

    [Fact]
    public void PubSubReconciliationInterval_PositiveSucceeds()
    {
        var interval = TimeSpan.FromSeconds(30);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithPubSubReconciliationInterval(interval);

        Assert.Equal(interval.TotalMilliseconds, builder.Build().Request.PubSubReconciliationInterval!.Value.TotalMilliseconds);
    }

    [Fact]
    public void PubSubReconciliationInterval_NegativeThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithPubSubReconciliationInterval(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void PubSubReconciliationInterval_ZeroThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithPubSubReconciliationInterval(TimeSpan.Zero));
    }

    // Connection Retry Strategy
    // -------------------------

    [Fact]
    public void WithConnectionRetryStrategy_Standalone_NotSpecified()
    {
        var config = new StandaloneClientConfigurationBuilder().Build();
        Assert.Null(config.Request.RetryStrategy);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Standalone_RetryStrategy_NoJitter()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithConnectionRetryStrategy(new RetryStrategy(NumberOfRetries, Factor, ExponentBase));

        var strategy = builder.Build().Request.RetryStrategy!.Value;
        Assert.Equal(NumberOfRetries, strategy.NumberOfRetries);
        Assert.Equal(Factor, strategy.Factor);
        Assert.Equal(ExponentBase, strategy.ExponentBase);
        Assert.Equal(0u, strategy.JitterPercent);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Standalone_RetryStrategy_WithJitter()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithConnectionRetryStrategy(new RetryStrategy(NumberOfRetries, Factor, ExponentBase, JitterPercent));

        var strategy = builder.Build().Request.RetryStrategy!.Value;
        Assert.Equal(NumberOfRetries, strategy.NumberOfRetries);
        Assert.Equal(Factor, strategy.Factor);
        Assert.Equal(ExponentBase, strategy.ExponentBase);
        Assert.Equal(JitterPercent, strategy.JitterPercent);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Standalone_UintParams_NoJitter()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithConnectionRetryStrategy(NumberOfRetries, Factor, ExponentBase);

        var strategy = builder.Build().Request.RetryStrategy!.Value;
        Assert.Equal(NumberOfRetries, strategy.NumberOfRetries);
        Assert.Equal(Factor, strategy.Factor);
        Assert.Equal(ExponentBase, strategy.ExponentBase);
        Assert.Equal(0u, strategy.JitterPercent);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Standalone_UintParams_WithJitter()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithConnectionRetryStrategy(NumberOfRetries, Factor, ExponentBase, JitterPercent);

        var strategy = builder.Build().Request.RetryStrategy!.Value;
        Assert.Equal(NumberOfRetries, strategy.NumberOfRetries);
        Assert.Equal(Factor, strategy.Factor);
        Assert.Equal(ExponentBase, strategy.ExponentBase);
        Assert.Equal(JitterPercent, strategy.JitterPercent);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Cluster_NotSpecified()
    {
        var config = new ClusterClientConfigurationBuilder().Build();
        Assert.Null(config.Request.RetryStrategy);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Cluster_RetryStrategy_NoJitter()
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithConnectionRetryStrategy(new RetryStrategy(NumberOfRetries, Factor, ExponentBase));

        var strategy = builder.Build().Request.RetryStrategy!.Value;
        Assert.Equal(NumberOfRetries, strategy.NumberOfRetries);
        Assert.Equal(Factor, strategy.Factor);
        Assert.Equal(ExponentBase, strategy.ExponentBase);
        Assert.Equal(0u, strategy.JitterPercent);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Cluster_RetryStrategy_WithJitter()
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithConnectionRetryStrategy(new RetryStrategy(NumberOfRetries, Factor, ExponentBase, JitterPercent));

        var strategy = builder.Build().Request.RetryStrategy!.Value;
        Assert.Equal(NumberOfRetries, strategy.NumberOfRetries);
        Assert.Equal(Factor, strategy.Factor);
        Assert.Equal(ExponentBase, strategy.ExponentBase);
        Assert.Equal(JitterPercent, strategy.JitterPercent);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Cluster_UintParams_NoJitter()
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithConnectionRetryStrategy(NumberOfRetries, Factor, ExponentBase);

        var strategy = builder.Build().Request.RetryStrategy!.Value;
        Assert.Equal(NumberOfRetries, strategy.NumberOfRetries);
        Assert.Equal(Factor, strategy.Factor);
        Assert.Equal(ExponentBase, strategy.ExponentBase);
        Assert.Equal(0u, strategy.JitterPercent);
    }

    [Fact]
    public void WithConnectionRetryStrategy_Cluster_UintParams_WithJitter()
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithConnectionRetryStrategy(NumberOfRetries, Factor, ExponentBase, JitterPercent);

        var strategy = builder.Build().Request.RetryStrategy!.Value;
        Assert.Equal(NumberOfRetries, strategy.NumberOfRetries);
        Assert.Equal(Factor, strategy.Factor);
        Assert.Equal(ExponentBase, strategy.ExponentBase);
        Assert.Equal(JitterPercent, strategy.JitterPercent);
    }

}
