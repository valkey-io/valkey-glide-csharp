// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Security.Cryptography.X509Certificates;

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.UnitTests;

public class ConfigurationOptionsTests
{
    private static readonly X509Certificate2 Certificate = CreateTestCertificate();
    private static readonly byte[] CertificateData = Certificate.Export(X509ContentType.Cert);

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
        _ = Assert.Throws<ArgumentException>(() => options.TrustIssuer(tempFile.Path));
    }

    [Fact]
    public void TrustIssuer_WithCertificate_NullThrows()
    {
        var options = new ConfigurationOptions();
        _ = Assert.Throws<ArgumentNullException>(() => options.TrustIssuer((X509Certificate2)null!));
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

    // Security Hardening — Bug Condition Exploration Tests
    // ----------------------------------------------------

    [Fact]
    public void TrustIssuer_WithPath_OversizedFileThrows()
    {
        // Create a file just over 10 MB — should be rejected.
        const long oversizedLength = 10 * 1024 * 1024 + 1;
        using var tempFile = new TempFile();
        using (var fs = new FileStream(tempFile.Path, FileMode.Create))
        {
            fs.SetLength(oversizedLength);
        }

        var options = new ConfigurationOptions();
        _ = Assert.Throws<ArgumentException>(() => options.TrustIssuer(tempFile.Path));
    }

    [Fact]
    public void TrustIssuer_WithPath_TraversalPathCanonicalized()
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
    public void TrustIssuer_WithPath_ExactlyMaxSizeSucceeds()
    {
        // A file at exactly 10 MB should be accepted.
        const long exactMaxSize = 10 * 1024 * 1024;
        using var tempFile = new TempFile();
        using (var fs = new FileStream(tempFile.Path, FileMode.Create))
        {
            fs.SetLength(exactMaxSize);
        }

        var options = new ConfigurationOptions();
        options.TrustIssuer(tempFile.Path);

        _ = Assert.Single(options._trustedIssuers);
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
