// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class AwsCredentialsTests
{
    #region Constants

    private const string AccessKeyId = "ACCESS_KEY_ID";
    private const string SecretAccessKey = "SECRET_ACCESS_KEY";
    private const string SessionToken = "SESSION_TOKEN";

    #endregion
    #region Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\n")]
    public void RejectsBlankRequiredKeys(string blank)
    {
        var accessKeyIdException = Assert.ThrowsAny<ArgumentException>(
            () => new AwsCredentials(blank, SecretAccessKey));
        Assert.Equal("accessKeyId", accessKeyIdException.ParamName);

        var secretAccessKeyException = Assert.ThrowsAny<ArgumentException>(
            () => new AwsCredentials(AccessKeyId, blank));
        Assert.Equal("secretAccessKey", secretAccessKeyException.ParamName);
    }

    [Fact]
    public void StoresOptionalValuesAndOmitsSecretsFromToString()
    {
        var expiresAt = new DateTimeOffset(2030, 1, 2, 3, 4, 5, TimeSpan.FromHours(-8));
        var credentials = new AwsCredentials(AccessKeyId, SecretAccessKey, SessionToken, expiresAt);

        Assert.Equal(AccessKeyId, credentials.AccessKeyId);
        Assert.Equal(SecretAccessKey, credentials.SecretAccessKey);
        Assert.Equal(SessionToken, credentials.SessionToken);
        Assert.Equal(expiresAt, credentials.ExpiresAt);
        Assert.Equal(expiresAt.Offset, credentials.ExpiresAt!.Value.Offset);

        var result = credentials.ToString();

        Assert.DoesNotContain(AccessKeyId, result);
        Assert.DoesNotContain(SecretAccessKey, result);
        Assert.DoesNotContain(SessionToken, result);

        // Optional values default to null.
        var minimal = new AwsCredentials(AccessKeyId, SecretAccessKey);

        Assert.Null(minimal.SessionToken);
        Assert.Null(minimal.ExpiresAt);
    }

    #endregion
}
