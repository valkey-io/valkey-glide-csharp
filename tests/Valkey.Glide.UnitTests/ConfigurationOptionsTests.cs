// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Security.Cryptography.X509Certificates;

namespace Valkey.Glide.UnitTests;

public class ConfigurationOptionsTests
{
    static readonly X509Certificate2 Certificate = CreateTestCertificate();
    static readonly byte[] CertificateData = Certificate.Export(X509ContentType.Cert);

    [Fact]
    public void TrustIssuer_WithNullPath_Throws()
    {
        var options = new ConfigurationOptions();
        var ex = Assert.Throws<ArgumentNullException>(() => options.TrustIssuer((string)null!));
    }

    [Fact]
    public void TrustIssuer_WithInvalidPath_Throws()
    {
        var options = new ConfigurationOptions();
        Assert.Throws<FileNotFoundException>(() => options.TrustIssuer("nonexistent.crt"));
    }

    [Fact]
    public void TrustIssuer_WithEmptyFile_Throws()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(tempFile, Array.Empty<byte>());
            var options = new ConfigurationOptions();
            Assert.Throws<ArgumentException>(() => options.TrustIssuer(tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void TrustIssuer_WithNullCertificate_Throws()
    {
        var options = new ConfigurationOptions();
        Assert.Throws<ArgumentNullException>(() =>options.TrustIssuer((X509Certificate2)null!));
    }

    [Fact]
    public void TrustIssuer_WithValidPath_AddsCertificate()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(tempFile, CertificateData);
            var options = new ConfigurationOptions();
            options.TrustIssuer(tempFile);

            Assert.Equivalent(new[] { CertificateData }, options._trustedIssuers);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void TrustIssuer_WithValidCertificate_AddsCertificate()
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
