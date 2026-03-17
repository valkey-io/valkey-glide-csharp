// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConnectionConfigurationTests
{
    // Authentication constants.
    private static readonly string Username = "testUsername";
    private static readonly string Password = "testPassword";

    // IAM authentication constants.
    private static readonly string ClusterName = "testClusterName";
    private static readonly string Region = "testRegion";
    private static readonly uint RefreshIntervalSeconds = 600;

    private static readonly IamAuthConfig IamAuthConfig = new(
        ClusterName,
        ServiceType.ElastiCache,
        Region,
        RefreshIntervalSeconds);
    private static readonly ServerCredentials ServerCredentials = new(Username, IamAuthConfig);

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
    public void WithAuthentication_UsernamePassword_Succeeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, Password);

        var authenticationInfo = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);
    }

    [Fact]
    public void WithAuthentication_UsernamePassword_NullPasswordThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (string)null!));
    }

    [Fact]
    public void WithAuthentication_PasswordOnly_Succeeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Password);

        var authenticationInfo = builder.Build()!.Request.AuthenticationInfo!.Value;
        Assert.Null(authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);
    }

    [Fact]
    public void WithAuthentication_PasswordOnly_NullThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(null!));
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig_Succeeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, IamAuthConfig);

        var authenticationInfo = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.ElastiCache, iamCredentials.ServiceType);
        Assert.True(iamCredentials.HasRefreshIntervalSeconds);
        Assert.Equal(600u, iamCredentials.RefreshIntervalSeconds);
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig_NullUsernameThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(null!, IamAuthConfig));
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig_NulIamAuthConfigThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username!, (IamAuthConfig)null!));
    }

    [Fact]
    public void WithAuthentication_MultipleCalls_LastWins()
    {
        // Use IamAuthConfig with different service type and no specified refresh interval.
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);

        // Password-based authentication last.
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, iamConfig)
            .WithAuthentication(Username, Password);

        var authenticationInfo1 = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo1.Username);
        Assert.Equal(Password, authenticationInfo1.Password);
        Assert.False(authenticationInfo1.HasIamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, Password)
            .WithAuthentication(Username, IamAuthConfig);

        var authenticationInfo2 = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo2.Username);
        Assert.Null(authenticationInfo2.Password);
        Assert.True(authenticationInfo2.HasIamCredentials);

        var iamCredentials = authenticationInfo2.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);
    }

    [Fact]
    public void WithCredentials_Succeeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithCredentials(ServerCredentials);

        var authenticationInfo = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(ClusterName, iamCredentials.ClusterName);
        Assert.Equal(Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.Equal(RefreshIntervalSeconds, iamCredentials.RefreshIntervalSeconds);
    }

    [Fact]
    public void WithCredentials()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithCredentials(null!));
    }

    [Fact]
    public void WithCredentials_MultipleCalls_LastWins()
    {
        // Use IamAuthConfig with different service type and no specified refresh interval.
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);

        var iamServerCredentials = new ServerCredentials(Username, iamConfig);
        var passwordServerCredentials = new ServerCredentials(Username, Password);

        // Password-based authentication last.
        var builder = new StandaloneClientConfigurationBuilder()
            .WithCredentials(iamServerCredentials)
            .WithCredentials(passwordServerCredentials);

        var authenticationInfo1 = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo1.Username);
        Assert.Equal(Password, authenticationInfo1.Password);
        Assert.False(authenticationInfo1.HasIamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder()
            .WithCredentials(passwordServerCredentials)
            .WithCredentials(iamServerCredentials);

        var authenticationInfo2 = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo2.Username);
        Assert.Null(authenticationInfo2.Password);
        Assert.True(authenticationInfo2.HasIamCredentials);

        var iamCredentials = authenticationInfo2.IamCredentials!;
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
        Assert.False(builder.Build().Request.RefreshTopologyFromInitialNodes);
    }

    [Fact]
    public void RefreshTopologyFromInitialNodes_True()
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithRefreshTopologyFromInitialNodes(true);
        Assert.True(builder.Build().Request.RefreshTopologyFromInitialNodes);
    }

    [Fact]
    public void RefreshTopologyFromInitialNodes_False()
    {
        var builder = new ClusterClientConfigurationBuilder()
            .WithRefreshTopologyFromInitialNodes(false);
        Assert.False(builder.Build().Request.RefreshTopologyFromInitialNodes);
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
    public void WithTrustedCertificate_ByteArray_Succeeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(CertificateData1);

        Assert.Equivalent(
            new List<byte[]> { CertificateData1 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_MultipleCertificatesSucceeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(CertificateData1)
            .WithTrustedCertificate(CertificateData2);

        Assert.Equivalent(
            new List<byte[]> { CertificateData1, CertificateData2 },
            builder.Build().Request.RootCertificates);
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
    public void WithTrustedCertificate_ByteArray_OversizedThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate(new byte[CertificateMaxSize]));
    }

    [Fact]
    public void WithTrustedCertificate_Path_Succeeds()
    {
        using var tempFile = new TempFile(CertificateData1);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(tempFile.Path);

        Assert.Equivalent(
            new List<byte[]> { CertificateData1 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_MultipleCertificatesSucceeds()
    {
        using var tempFile1 = new TempFile(CertificateData1);
        using var tempFile2 = new TempFile(CertificateData2);

        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(tempFile1.Path)
            .WithTrustedCertificate(tempFile2.Path);

        Assert.Equivalent(
            new List<byte[]> { CertificateData1, CertificateData2 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_TraversalPathSucceeds()
    {
        using var tempFile = new TempFile(CertificateData1);

        // Construct a traversal path that resolves to the temp file.
        string dir = Path.GetDirectoryName(tempFile.Path)!;
        string fileName = Path.GetFileName(tempFile.Path);
        string traversalPath = Path.Combine(dir, "subdir", "..", fileName);

        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(traversalPath);

        Assert.Equivalent(
            new List<byte[]> { CertificateData1 },
            builder.Build().Request.RootCertificates);
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

        Assert.Equivalent(
            new List<byte[]> { CertificateData1, CertificateData2 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_OversizedThrows()
    {
        using var tempFile = new TempFile();

        // Set temp file size so that it exceeds the maximum size.
        using (var fs = new FileStream(tempFile.Path, FileMode.Create))
        {
            fs.SetLength(CertificateMaxSize);
        }

        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate(tempFile.Path));
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

        Assert.Equal(
            interval.TotalMilliseconds,
            builder.Build().Request.PubSubReconciliationInterval!.Value.TotalMilliseconds);
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
