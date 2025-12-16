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
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, Password);

        var config = builder.Build();
        var authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (string)null!));
    }

    [Fact]
    public void WithAuthentication_PasswordOnly()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Password);

        var config = builder.Build();
        var authenticationInfo = config!.Request.AuthenticationInfo!.Value;

        Assert.Null(authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(null!));
    }

    [Fact]
    public void WithAuthentication_UsernameIamAuthConfig()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.ElastiCache, Region, RefreshIntervalSeconds);
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, iamConfig);

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

        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(null!, iamConfig));
        Assert.Throws<ArgumentNullException>(() => builder.WithAuthentication(Username, (IamAuthConfig)null!));
    }

    [Fact]
    public void WithAuthentication_MultipleCalls_LastWins()
    {
        // Password-based authentication last.
        var builder = new StandaloneClientConfigurationBuilder();
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);
        builder.WithAuthentication(Username, iamConfig);
        builder.WithAuthentication(Username, Password);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder();
        builder.WithAuthentication(Username, Password);
        builder.WithAuthentication(Username, iamConfig);

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
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithCredentials(credentials);

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

        Assert.Throws<ArgumentNullException>(() => builder.WithCredentials(null!));
    }

    [Fact]
    public void WithCredentials_MultipleCalls_LastWins()
    {
        var iamConfig = new IamAuthConfig(ClusterName, ServiceType.MemoryDB, Region);
        var iamServerCredentials = new ServerCredentials(Username, iamConfig);
        var passwordServerCredentials = new ServerCredentials(Username, Password);

        // Password-based authentication last.
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithCredentials(iamServerCredentials);
        builder.WithCredentials(passwordServerCredentials);

        var config = builder.Build();
        var authenticationInfo = config.Request.AuthenticationInfo!.Value;

        Assert.Equal(Username, authenticationInfo.Username);
        Assert.Equal(Password, authenticationInfo.Password);
        Assert.False(authenticationInfo.HasIamCredentials);

        // IAM authentication last.
        builder = new StandaloneClientConfigurationBuilder();
        builder.WithCredentials(passwordServerCredentials);
        builder.WithCredentials(iamServerCredentials);

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
        var builder = new ClusterClientConfigurationBuilder();
        builder.WithRefreshTopologyFromInitialNodes(true);
        var config = builder.Build();
        Assert.True(config.Request.RefreshTopologyFromInitialNodes);
    }

    [Fact]
    public void RefreshTopologyFromInitialNodes_False()
    {
        var builder = new ClusterClientConfigurationBuilder();
        builder.WithRefreshTopologyFromInitialNodes(false);
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
        builder.WithTls(true);
        Assert.True(builder.UseTls);

        // Disable TLS.
        builder.WithTls(false);
        Assert.False(builder.UseTls);

        // Enable TLS (no parameter).
        builder.WithTls();
        Assert.True(builder.UseTls);

        // Disable TLS when insecure TLS enabled.
        builder.WithInsecureTls();
        builder.WithTls(false);
        Assert.False(builder.UseTls);
    }

    [Fact]
    public void UseInsecureTls()
    {
        var builder = new StandaloneClientConfigurationBuilder();

        // Default configuration.
        Assert.False(builder.UseInsecureTls);

        // Configure insecure TLS without TLS enabled.
        Assert.Throws<ArgumentException>(() => builder.UseInsecureTls = true);
        Assert.Throws<ArgumentException>(() => builder.UseInsecureTls = false);

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
        Assert.Throws<ArgumentException>(() => builder.WithInsecureTls());
        Assert.Throws<ArgumentException>(() => builder.WithInsecureTls(true));
        Assert.Throws<ArgumentException>(() => builder.WithInsecureTls(false));

        builder.WithTls();

        // Enable insecure TLS.
        builder.WithInsecureTls(true);
        Assert.True(builder.UseInsecureTls);

        // Disable insecure TLS.
        builder.WithInsecureTls(false);
        Assert.False(builder.UseInsecureTls);

        // Enable insecure TLS (no parameter).
        builder.WithInsecureTls(true);
        Assert.True(builder.UseInsecureTls);

        // Disable TLS when insecure TLS enabled.
        builder.WithTls(false);
        Assert.False(builder.UseInsecureTls);
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithTrustedCertificate(CertificateData1);
        var config = builder.Build();
        Assert.Equivalent(new List<byte[]> { CertificateData1 }, config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_NullThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate((byte[])null!));
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_EmptyThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate([]));
    }

    [Fact]
    public void WithTrustedCertificate_ByteArray_MultipleCertificates()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithTrustedCertificate(CertificateData1);
        builder.WithTrustedCertificate(CertificateData2);
        var config = builder.Build();

        Assert.Equivalent(new List<byte[]> { CertificateData1, CertificateData2 }, config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path()
    {
        using var tempFile = new TempFile(CertificateData1);

        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithTrustedCertificate(tempFile.Path);
        var config = builder.Build();

        Assert.Equivalent(new List<byte[]> { CertificateData1 }, config.Request.RootCertificates);
    }

    [Fact]
    public void WithTrustedCertificate_Path_FileNotFoundThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Throws<FileNotFoundException>(() => builder.WithTrustedCertificate("/nonexistent/path/cert.pem"));
    }

    [Fact]
    public void WithTrustedCertificate_Path_NullThrows()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.WithTrustedCertificate((string)null!));
    }

    [Fact]
    public void WithTrustedCertificate_Path_MultipleCertificates()
    {
        using var tempFile1 = new TempFile(CertificateData1);
        using var tempFile2 = new TempFile(CertificateData2);

        var builder = new StandaloneClientConfigurationBuilder();
        builder.WithTrustedCertificate(tempFile1.Path);
        builder.WithTrustedCertificate(tempFile2.Path);
        var config = builder.Build();

        Assert.Equivalent(new List<byte[]> { CertificateData1, CertificateData2 }, config.Request.RootCertificates);
    }
}
