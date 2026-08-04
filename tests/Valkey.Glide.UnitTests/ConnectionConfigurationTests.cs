// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class ConnectionConfigurationTests
{
    #region Constants

    // Authentication constants
    private static readonly string Username = "USERNAME";
    private static readonly string Password = "PASSWORD";
    private const uint RefreshInterval = 300;

    // Certificate constants
    private static readonly byte[] CertData1 = [0x30, 0x82, 0x01, 0x00];
    private static readonly byte[] CertData2 = [0x30, 0x82, 0x02, 0x00];
    private const string CertPath = "/path/cert.pem";
    private const string KeyPath = "/path/key.pem";

    // Connection retry strategy constants
    private static readonly uint NumberOfRetries = 3u;
    private static readonly uint Factor = 50u;
    private static readonly uint ExponentBase = 2u;
    private static readonly uint JitterPercent = 10u;

    // Address resolver constants.
    private static readonly (string, ushort) Resolved = ("resolved-host", 9999);
    private static readonly AddressResolverDelegate Resolver = (host, port) => Resolved;

    #endregion
    #region Authentication & Credentials Tests

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
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithAuthentication(Username, (string)null!));

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
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithAuthentication(null!));

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig_WithoutRefreshInterval_Succeeds()
    {
        using var iamAuthConfig = BuildIamAuthConfig(refreshIntervalSeconds: null);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, iamAuthConfig);

        var authenticationInfo = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(iamAuthConfig.ClusterName, iamCredentials.ClusterName);
        Assert.Equal(iamAuthConfig.Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.ElastiCache, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig_WithRefreshInterval_Succeeds()
    {
        using var iamAuthConfig = BuildIamAuthConfig(refreshIntervalSeconds: RefreshInterval);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithAuthentication(Username, iamAuthConfig);

        var authenticationInfo = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(iamAuthConfig.ClusterName, iamCredentials.ClusterName);
        Assert.Equal(iamAuthConfig.Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.ElastiCache, iamCredentials.ServiceType);
        Assert.True(iamCredentials.HasRefreshIntervalSeconds);
        Assert.Equal(iamAuthConfig.RefreshIntervalSeconds, RefreshInterval);
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig_NullUsernameThrows()
    {
        using var iamAuthConfig = BuildIamAuthConfig();
        var builder = new StandaloneClientConfigurationBuilder();
        _ = Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(null!, iamAuthConfig));
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig_NulIamAuthConfigThrows()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithAuthentication(Username!, (IamAuthConfig)null!));

    [Fact]
    public void WithAuthentication_MultipleCalls_LastWins()
    {
        // Use IamAuthConfig with different service type.
        using var iamConfig = BuildIamAuthConfig(serviceType: ServiceType.MemoryDB);

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
            .WithAuthentication(Username, iamConfig);

        var authenticationInfo2 = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo2.Username);
        Assert.Null(authenticationInfo2.Password);
        Assert.True(authenticationInfo2.HasIamCredentials);
        Assert.Equal(FFI.ServiceType.MemoryDB, authenticationInfo2.IamCredentials!.ServiceType);
    }

    [Fact]
    public void WithCredentials_WithoutRefreshInterval_Succeeds()
    {
        using var iamAuthConfig = BuildIamAuthConfig(refreshIntervalSeconds: null);
        using var serverCredentials = new ServerCredentials(Username, iamAuthConfig);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithCredentials(serverCredentials);

        var authenticationInfo = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(iamAuthConfig.ClusterName, iamCredentials.ClusterName);
        Assert.Equal(iamAuthConfig.Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.ElastiCache, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);
    }

    [Fact]
    public void WithCredentials_Succeeds()
    {
        using var iamAuthConfig = BuildIamAuthConfig(refreshIntervalSeconds: RefreshInterval);
        using var serverCredentials = new ServerCredentials(Username, iamAuthConfig);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithCredentials(serverCredentials);

        var authenticationInfo = builder.Build().Request.AuthenticationInfo!.Value;
        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Null(authenticationInfo.Password);
        Assert.True(authenticationInfo.HasIamCredentials);

        var iamCredentials = authenticationInfo.IamCredentials!;
        Assert.Equal(iamAuthConfig.ClusterName, iamCredentials.ClusterName);
        Assert.Equal(iamAuthConfig.Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.ElastiCache, iamCredentials.ServiceType);
        Assert.True(iamCredentials.HasRefreshIntervalSeconds);
        Assert.Equal(iamAuthConfig.RefreshIntervalSeconds, RefreshInterval);
    }

    [Fact]
    public void WithCredentials()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithCredentials(null!));

    [Fact]
    public void WithCredentials_MultipleCalls_LastWins()
    {
        // Use IamAuthConfig with different service type and no refresh interval.
        using var iamConfig = new IamAuthConfig(
            clusterName: "CLUSTER",
            serviceType: ServiceType.MemoryDB,
            region: "REGION",
            refreshIntervalSeconds: null);

        using var iamServerCredentials = new ServerCredentials(Username, iamConfig);
        using var passwordServerCredentials = new ServerCredentials(Username, Password);

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
        Assert.Equal(iamConfig.ClusterName, iamCredentials.ClusterName);
        Assert.Equal(iamConfig.Region, iamCredentials.Region);
        Assert.Equal(FFI.ServiceType.MemoryDB, iamCredentials.ServiceType);
        Assert.False(iamCredentials.HasRefreshIntervalSeconds);
    }

    #endregion
    #region Refresh Topology Configuration Tests

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

    #endregion
    #region Node Discovery Mode Tests

    [Fact]
    public void NodeDiscoveryMode_Default_IsStandard()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Equal(NodeDiscoveryMode.Standard, builder.Build().Request.NodeDiscoveryMode);
    }

    [Theory]
    [MemberData(nameof(Data.NodeDiscoveryModes), MemberType = typeof(Data))]
    public void WithNodeDiscoveryMode_SetsMode(NodeDiscoveryMode mode)
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithNodeDiscoveryMode(mode);
        Assert.Equal(mode, builder.Build().Request.NodeDiscoveryMode);
    }

    [Fact]
    public void NodeDiscoveryMode_Property_SetsMode()
    {
        var builder = new StandaloneClientConfigurationBuilder
        {
            NodeDiscoveryMode = NodeDiscoveryMode.DiscoverAll,
        };
        Assert.Equal(NodeDiscoveryMode.DiscoverAll, builder.NodeDiscoveryMode);
        Assert.Equal(NodeDiscoveryMode.DiscoverAll, builder.Build().Request.NodeDiscoveryMode);
    }

    [Theory]
    [MemberData(nameof(Data.NodeDiscoveryModes), MemberType = typeof(Data))]
    public void WithNodeDiscoveryMode_ToFfi_PassesModeToFfiLayer(NodeDiscoveryMode mode)
    {
        var config = new StandaloneClientConfigurationBuilder()
            .WithNodeDiscoveryMode(mode)
            .Build();

        using FFI.ConnectionConfig ffi = config.Request.ToFfi();
        Assert.Equal(mode, ffi.NodeDiscoveryMode);
    }

    [Fact]
    public void NodeDiscoveryMode_EnumValues_MatchFfiContract()
    {
        // These discriminants must stay aligned with the Rust FFI `NodeDiscoveryMode` enum and
        // the glide-core protobuf values.
        Assert.Equal(0u, (uint)NodeDiscoveryMode.Standard);
        Assert.Equal(1u, (uint)NodeDiscoveryMode.Static);
        Assert.Equal(2u, (uint)NodeDiscoveryMode.DiscoverAll);
    }

    #endregion
    #region TLS Tests

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
    public void WithTrustedCertificate_Bytes_Succeeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(CertData1);

        Assert.Equivalent(
            new List<byte[]> { CertData1 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Bytes_MultipleCertificatesSucceeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(CertData1)
            .WithTrustedCertificate(CertData2);

        Assert.Equivalent(
            new List<byte[]> { CertData1, CertData2 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Bytes_NullThrows()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithTrustedCertificate((byte[])null!));

    [Fact]
    public void WithTrustedCertificate_Bytes_EmptyThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithTrustedCertificate([]));

    [Fact]
    public void WithTrustedCertificate_Bytes_OversizedThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithTrustedCertificate(new byte[ConnectionConfiguration.CertificateMaxSize + 1]));

    [Fact]
    public void WithTrustedCertificate_Path_Succeeds()
    {
        using var tempFile = new TempFile(CertData1);
        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(tempFile.Path);

        Assert.Equivalent(
            new List<byte[]> { CertData1 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_MultipleCertificatesSucceeds()
    {
        using var tempFile1 = new TempFile(CertData1);
        using var tempFile2 = new TempFile(CertData2);

        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(tempFile1.Path)
            .WithTrustedCertificate(tempFile2.Path);

        Assert.Equivalent(
            new List<byte[]> { CertData1, CertData2 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_TraversalPathSucceeds()
    {
        using var tempFile = new TempFile(CertData1);

        // Construct a traversal path that resolves to the temp file.
        string dir = Path.GetDirectoryName(tempFile.Path)!;
        string fileName = Path.GetFileName(tempFile.Path);
        string traversalPath = Path.Combine(dir, "subdir", "..", fileName);

        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(traversalPath);

        Assert.Equivalent(
            new List<byte[]> { CertData1 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_FileNotFoundThrows()
        => _ = Assert.Throws<FileNotFoundException>(()
            => new StandaloneClientConfigurationBuilder().WithTrustedCertificate("/nonexistent/path/cert.pem"));

    [Fact]
    public void WithTrustedCertificate_Path_NullThrows()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithTrustedCertificate((string)null!));

    [Fact]
    public void WithTrustedCertificate_Path_MultipleCertificates()
    {
        using var tempFile1 = new TempFile(CertData1);
        using var tempFile2 = new TempFile(CertData2);

        var builder = new StandaloneClientConfigurationBuilder()
            .WithTrustedCertificate(tempFile1.Path)
            .WithTrustedCertificate(tempFile2.Path);

        Assert.Equivalent(
            new List<byte[]> { CertData1, CertData2 },
            builder.Build().Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_OversizedThrows()
    {
        using var tempFile = new TempFile();
        using (var fs = new FileStream(tempFile.Path, FileMode.Create))
        {
            fs.SetLength(ConnectionConfiguration.CertificateMaxSize + 1);
        }

        _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithTrustedCertificate(tempFile.Path));
    }

    #endregion
    #region Mutual TLS Tests

    [Fact]
    public void WithClientCertificate_Bytes_Succeeds()
    {
        var config = new StandaloneClientConfigurationBuilder()
            .WithClientCertificate(CertData1, CertData2)
            .Build();

        Assert.Equal(CertData1, config.Request.ClientCertificate);
        Assert.Equal(CertData2, config.Request.ClientKey);
        Assert.False(config.Request.CertReloadEnabled);
        Assert.Null(config.Request.CertReloadIntervalSeconds);
    }

    [Fact]
    public void WithClientCertificate_Bytes_NullCertThrows()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(null!, CertData1));

    [Fact]
    public void WithClientCertificate_Bytes_NullKeyThrows()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(CertData1, null!));

    [Fact]
    public void WithClientCertificate_Bytes_EmptyCertThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate([], CertData1));

    [Fact]
    public void WithClientCertificate_Bytes_EmptyKeyThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(CertData1, []));

    [Fact]
    public void WithClientCertificate_Bytes_OversizedCertThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(new byte[ConnectionConfiguration.CertificateMaxSize + 1], CertData1));

    [Fact]
    public void WithClientCertificate_Bytes_OversizedKeyThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(CertData1, new byte[ConnectionConfiguration.CertificateMaxSize + 1]));

    [Fact]
    public void WithClientCertificate_Paths_Succeeds()
    {
        var config = new StandaloneClientConfigurationBuilder()
            .WithClientCertificate(CertPath, KeyPath)
            .Build();

        Assert.Null(config.Request.ClientCertificate);
        Assert.Null(config.Request.ClientKey);
        Assert.Equal(CertPath, config.Request.ClientCertificatePath);
        Assert.Equal(KeyPath, config.Request.ClientKeyPath);
        Assert.True(config.Request.CertReloadEnabled);
        Assert.Null(config.Request.CertReloadIntervalSeconds);
    }

    [Fact]
    public void WithClientCertificate_Paths_NullCertPathThrows()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(null!, KeyPath));

    [Fact]
    public void WithClientCertificate_Paths_NullKeyPathThrows()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(CertPath, null!));

    [Fact]
    public void WithClientCertificate_Paths_EmptyCertPathThrows()
        => _ = Assert.Throws<ArgumentException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate("", KeyPath));

    [Fact]
    public void WithClientCertificate_Paths_EmptyKeyPathThrows()
        => _ = Assert.Throws<ArgumentException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(CertPath, ""));

    [Fact]
    public void WithClientCertificate_Paths_WithInterval_Succeeds()
    {
        var config = new StandaloneClientConfigurationBuilder()
            .WithClientCertificate(CertPath, KeyPath, TimeSpan.FromSeconds(60))
            .Build();

        Assert.Equal(CertPath, config.Request.ClientCertificatePath);
        Assert.Equal(KeyPath, config.Request.ClientKeyPath);
        Assert.True(config.Request.CertReloadEnabled);
        Assert.Equal(60u, config.Request.CertReloadIntervalSeconds);
    }

    [Fact]
    public void WithClientCertificate_Paths_WithInterval_ZeroThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(CertPath, KeyPath, TimeSpan.Zero));

    [Fact]
    public void WithClientCertificate_Paths_WithInterval_NegativeThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(CertPath, KeyPath, TimeSpan.FromSeconds(-1)));

    [Fact]
    public void WithClientCertificate_Paths_WithInterval_OverflowThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithClientCertificate(CertPath, KeyPath, TimeSpan.FromSeconds((double)uint.MaxValue + 1)));

    #endregion
    #region Pub/Sub Reconciliation Interval Tests

    [Fact]
    public void PubSubReconciliationInterval_Default()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Null(builder.Build().Request.PubSubReconciliationIntervalMs);
    }

    [Fact]
    public void PubSubReconciliationInterval_PositiveSucceeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithPubSubReconciliationInterval(TimeSpan.FromSeconds(30));
        Assert.Equal(30_000u, builder.Build().Request.PubSubReconciliationIntervalMs);
    }

    [Fact]
    public void PubSubReconciliationInterval_NegativeThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithPubSubReconciliationInterval(TimeSpan.FromSeconds(-1)));

    [Fact]
    public void PubSubReconciliationInterval_ZeroThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithPubSubReconciliationInterval(TimeSpan.Zero));

    [Fact]
    public void PubSubReconciliationInterval_OverflowThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithPubSubReconciliationInterval(TimeSpan.FromMilliseconds((double)uint.MaxValue + 1)));

    #endregion
    #region Request Timeout Tests

    [Fact]
    public void RequestTimeout_Default()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Null(builder.Build().Request.RequestTimeoutMs);
    }

    [Fact]
    public void RequestTimeout_PositiveSucceeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithRequestTimeout(TimeSpan.FromMilliseconds(500));
        Assert.Equal(500u, builder.Build().Request.RequestTimeoutMs);
    }

    [Fact]
    public void RequestTimeout_NegativeThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithRequestTimeout(TimeSpan.FromMilliseconds(-1)));

    [Fact]
    public void RequestTimeout_ZeroThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithRequestTimeout(TimeSpan.Zero));

    [Fact]
    public void RequestTimeout_OverflowThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithRequestTimeout(TimeSpan.FromMilliseconds((double)uint.MaxValue + 1)));

    #endregion
    #region Connection Timeout Tests

    [Fact]
    public void ConnectionTimeout_Default()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Null(builder.Build().Request.ConnectionTimeoutMs);
    }

    [Fact]
    public void ConnectionTimeout_PositiveSucceeds()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithConnectionTimeout(TimeSpan.FromMilliseconds(1000));
        Assert.Equal(1000u, builder.Build().Request.ConnectionTimeoutMs);
    }

    [Fact]
    public void ConnectionTimeout_NegativeThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithConnectionTimeout(TimeSpan.FromMilliseconds(-1)));

    [Fact]
    public void ConnectionTimeout_ZeroThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithConnectionTimeout(TimeSpan.Zero));

    [Fact]
    public void ConnectionTimeout_OverflowThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithConnectionTimeout(TimeSpan.FromMilliseconds((double)uint.MaxValue + 1)));

    #endregion
    #region Connection Retry Strategy Tests

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

    #endregion
    #region Inflight Requests Limit Tests

    [Fact]
    public void InflightRequestsLimit_Default_IsNull()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Null(builder.Build().Request.InflightRequestsLimit);
    }

    [Fact]
    public void WithInflightRequestsLimit_SetsValue()
    {
        var builder = new StandaloneClientConfigurationBuilder()
            .WithInflightRequestsLimit(500);
        Assert.Equal(500u, builder.Build().Request.InflightRequestsLimit);
    }

    [Fact]
    public void WithInflightRequestsLimit_ZeroThrows()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new StandaloneClientConfigurationBuilder().WithInflightRequestsLimit(0));

    #endregion
    #region Address Resolver Tests

    [Fact]
    public void WithAddressResolver_Standalone_SetsResolver()
    {
        var config = new StandaloneClientConfigurationBuilder().WithAddressResolver(Resolver).Build();
        Assert.Equal(Resolver, config.Request.AddressResolver);
        Assert.Equal(Resolved, config.Request.AddressResolver!("localhost", 6379));
    }

    [Fact]
    public void WithAddressResolver_Cluster_SetsResolver()
    {
        var config = new ClusterClientConfigurationBuilder().WithAddressResolver(Resolver).Build();
        Assert.Equal(Resolver, config.Request.AddressResolver);
        Assert.Equal(Resolved, config.Request.AddressResolver!("localhost", 6379));
    }

    [Fact]
    public void AddressResolver_Standalone_NotSet_IsNull()
        => Assert.Null(new StandaloneClientConfigurationBuilder().Build().Request.AddressResolver);

    [Fact]
    public void AddressResolver_Cluster_NotSet_IsNull()
        => Assert.Null(new ClusterClientConfigurationBuilder().Build().Request.AddressResolver);

    [Fact]
    public void AddressResolver_Standalone_SetToNull_IsNull()
        => Assert.Null(new StandaloneClientConfigurationBuilder { AddressResolver = null }.Build().Request.AddressResolver);

    [Fact]
    public void AddressResolver_Cluster_SetToNull_IsNull()
        => Assert.Null(new ClusterClientConfigurationBuilder { AddressResolver = null }.Build().Request.AddressResolver);

    #endregion
    #region Helpers

    /// <summary>
    /// Builds and returns a new IAM authentication configuration for testing.
    /// If required parameters are not specified, default values are used.
    /// </summary>
    private static IamAuthConfig BuildIamAuthConfig(
        string clusterName = "CLUSTER_NAME",
        ServiceType serviceType = ServiceType.ElastiCache,
        string region = "REGION",
        uint? refreshIntervalSeconds = null
    )
        => new(clusterName, serviceType, region, refreshIntervalSeconds);

    #endregion
}
