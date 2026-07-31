// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Security.Cryptography.X509Certificates;

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.UnitTests;

public class ConfigurationOptionsTests
{
    #region Constants

    private static readonly X509Certificate2 Certificate = CreateTestCertificate();
    private static readonly byte[] CertificateData = Certificate.Export(X509ContentType.Cert);

    #endregion
    #region TrustIssuer

    [Fact]
    public void TrustIssuer_WithPath_NullThrows()
    {
        var options = new ConfigurationOptions();
        var ex = Assert.Throws<ArgumentNullException>(() => options.TrustIssuer((string)null!));
    }

    [Fact]
    public void TrustIssuer_WithPath_NonExistentThrows()
    {
        var options = new ConfigurationOptions();
        _ = Assert.Throws<FileNotFoundException>(() => options.TrustIssuer("nonexistent.crt"));
    }

    [Fact]
    public void TrustIssuer_WithPath_EmptyThrows()
    {
        using var tempFile = new TempFile();
        var options = new ConfigurationOptions();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => options.TrustIssuer(tempFile.Path));
    }

    [Fact]
    public void TrustIssuer_WithPath_OversizedThrows()
    {
        using var tempFile = new TempFile();
        using (var fs = new FileStream(tempFile.Path, FileMode.Create))
        {
            fs.SetLength(GuardClauses.MaxDataSize + 1);
        }

        _ = Assert.Throws<ArgumentOutOfRangeException>(()
            => new ConfigurationOptions().TrustIssuer(tempFile.Path));
    }

    [Fact]
    public void TrustIssuer_WithPath_Succeeds()
    {
        using var tempFile = new TempFile(CertificateData);

        var options = new ConfigurationOptions();
        options.TrustIssuer(tempFile.Path);

        Assert.Equivalent(new[] { CertificateData }, options._trustedIssuers);
    }

    [Fact]
    public void TrustIssuer_WithPath_TraversalPathSucceeds()
    {
        // Create a temp file and construct a traversal path that resolves to it.
        using var tempFile = new TempFile(CertificateData);

        string dir = Path.GetDirectoryName(tempFile.Path)!;
        string fileName = Path.GetFileName(tempFile.Path);
        string traversalPath = Path.Combine(dir, "subdir", "..", fileName);

        var options = new ConfigurationOptions();
        options.TrustIssuer(traversalPath);

        Assert.Equivalent(new[] { CertificateData }, options._trustedIssuers);
    }

    [Fact]
    public void TrustIssuer_WithCertificate_NullThrows()
    {
        var options = new ConfigurationOptions();
        _ = Assert.Throws<ArgumentNullException>(() => options.TrustIssuer((X509Certificate2)null!));
    }

    [Fact]
    public void TrustIssuer_WithCertificate_Succeeds()
    {
        var options = new ConfigurationOptions();
        options.TrustIssuer(Certificate);

        Assert.Equivalent(new[] { CertificateData }, options._trustedIssuers);
    }

    #endregion
    #region SetUserPemCertificate

    [Fact]
    public void SetUserPemCertificate_WithCertAndKey_Succeeds()
    {
        using var certFile = new TempFile(CertificateData);
        using var keyFile = new TempFile(CertificateData);

        var options = new ConfigurationOptions();
        options.SetUserPemCertificate(certFile.Path, keyFile.Path);

        Assert.NotNull(options._clientCertificate);
        Assert.Equal(CertificateData, options._clientCertificate);
        Assert.Equal(CertificateData, options._clientKey);
        Assert.True(options.Ssl);
    }

    [Fact]
    public void SetUserPemCertificate_CertOnly_UsesCertAsKey()
    {
        using var certFile = new TempFile(CertificateData);

        var options = new ConfigurationOptions();
        options.SetUserPemCertificate(certFile.Path);

        Assert.NotNull(options._clientCertificate);
        Assert.Equal(CertificateData, options._clientCertificate);
        Assert.Equal(CertificateData, options._clientKey);
        Assert.True(options.Ssl);
    }

    [Fact]
    public void SetUserPemCertificate_NullPath_Throws()
        => _ = Assert.Throws<ArgumentNullException>(()
            => new ConfigurationOptions().SetUserPemCertificate(null!));

    [Fact]
    public void SetUserPemCertificate_NonExistentCertPath_Throws()
        => _ = Assert.Throws<FileNotFoundException>(()
            => new ConfigurationOptions().SetUserPemCertificate("nonexistent.crt"));

    [Fact]
    public void SetUserPemCertificate_NonExistentKeyPath_Throws()
    {
        using var certFile = new TempFile(CertificateData);
        _ = Assert.Throws<FileNotFoundException>(()
            => new ConfigurationOptions().SetUserPemCertificate(certFile.Path, "nonexistent.key"));
    }

    #endregion
    #region Helpers

    private static X509Certificate2 CreateTestCertificate()
    {
        // Create a self-signed certificate for testing
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var request = new CertificateRequest(
            "CN=Test CA",
            rsa,
            System.Security.Cryptography.HashAlgorithmName.SHA256,
            System.Security.Cryptography.RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(
                certificateAuthority: true,
                hasPathLengthConstraint: false,
                pathLengthConstraint: 0,
                critical: true));

        return request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddDays(365));
    }

    #endregion
}
