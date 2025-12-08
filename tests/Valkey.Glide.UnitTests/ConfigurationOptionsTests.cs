// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Security.Cryptography.X509Certificates;

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.UnitTests;

public class ConfigurationOptionsTests
{
    static readonly X509Certificate2 Certificate = CreateTestCertificate();
    static readonly byte[] CertificateData = Certificate.Export(X509ContentType.Cert);

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
        Assert.Throws<FileNotFoundException>(() => options.TrustIssuer("nonexistent.crt"));
    }

    [Fact]
    public void TrustIssuer_WithPath_EmptyThrows()
    {
        using var tempFile = new TempFile();

        var options = new ConfigurationOptions();
        Assert.Throws<ArgumentException>(() => options.TrustIssuer(tempFile.Path));
    }

    [Fact]
    public void TrustIssuer_WithCertificate_NullThrows()
    {
        var options = new ConfigurationOptions();
        Assert.Throws<ArgumentNullException>(() => options.TrustIssuer((X509Certificate2)null!));
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
    public void TrustIssuer_WithCertificate_Succeeds()
    {
        var options = new ConfigurationOptions();
        options.TrustIssuer(Certificate);

        Assert.Equivalent(new[] { CertificateData }, options._trustedIssuers);
    }

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
}
