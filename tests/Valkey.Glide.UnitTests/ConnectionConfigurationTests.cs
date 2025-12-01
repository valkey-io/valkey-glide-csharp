// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
    private static readonly byte[] CertificateData = [0x30, 0x82, 0x01, 0x00];

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

        // Password cannot be null.
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

        // Password cannot be null.
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

        // Username and IamAuthConfig cannot be null.
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

        // Credentials cannot be null.
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

    // ---------
    // TLS Tests
    // ---------

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTls_Default(object builderObj)
    {
        var builder = (dynamic)builderObj;
        var config = builder.Build();
        Assert.Null(config.Request.TlsMode);
    }

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTls_Enabled(object builderObj)
    {
        var builder = (dynamic)builderObj;
        builder.WithTls(true);
        var config = builder.Build();
        Assert.Equal(FFI.TlsMode.SecureTls, config.Request.TlsMode);
    }

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTls_Disabled(object builderObj)
    {
        var builder = (dynamic)builderObj;
        builder.WithTls(false);
        var config = builder.Build();
        Assert.Equal(FFI.TlsMode.NoTls, config.Request.TlsMode);
    }

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTls_NoParameter_Theory(object builderObj)
    {
        var builder = (dynamic)builderObj;
        builder.WithTls();
        var config = builder.Build();
        Assert.Equal(FFI.TlsMode.SecureTls, config.Request.TlsMode);
    }

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTrustedCertificate_ByteArray_Theory(object builderObj)
    {
        var builder = (dynamic)builderObj;
        builder.WithTrustedCertificate(CertificateData);
        var config = builder.Build();
        Assert.Equivalent(new List<byte[]> { CertificateData }, config.Request.RootCertificates);
    }

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTrustedCertificate_NullByteArray_Throws_Theory(object builderObj)
    {
        var builder = (dynamic)builderObj;
        Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate((byte[])null!));
    }

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTrustedCertificate_EmptyByteArray_Throws_Theory(object builderObj)
    {
        var builder = (dynamic)builderObj;
        Assert.Throws<ArgumentException>(() => builder.WithTrustedCertificate(Array.Empty<byte>()));
    }

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTrustedCertificate_FileNotFound_Throws_Theory(object builderObj)
    {
        var builder = (dynamic)builderObj;
        Assert.Throws<FileNotFoundException>(() => builder.WithTrustedCertificate("/nonexistent/path/cert.pem"));
    }

    [Theory]
    [ClassData(typeof(BuildersData))]
    public void WithTrustedCertificate_MultipleCertificates_Theory(object builderObj)
    {
        var builder = (dynamic)builderObj;
        builder.WithTrustedCertificate(CertificateData);
        builder.WithTrustedCertificate(CertificateData);
        var config = builder.Build();
        Assert.Equivalent(new List<byte[]> { CertificateData, CertificateData }, config.Request.RootCertificates);
    }

    /// <summary>
    /// Data for parameterized tests with both StandaloneClientConfigurationBuilder and ClusterClientConfigurationBuilder.
    /// </summary>
    private class BuildersData : TheoryData<object[]>
    {
        public BuildersData()
        {
            Add([new StandaloneClientConfigurationBuilder()]);
            Add([new ClusterClientConfigurationBuilder()]);
        }
    }
}
