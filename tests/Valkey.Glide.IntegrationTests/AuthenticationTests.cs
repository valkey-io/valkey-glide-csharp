// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Client;
using static Valkey.Glide.TestUtils.Data;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for password and username/password authentication.
/// </summary>
[Collection(typeof(AuthenticationTests))]
[CollectionDefinition(DisableParallelization = true)]
public class AuthenticationTests(ServerFixture fixture) : IClassFixture<ServerFixture>
{
    #region Constants

    private const string Password = "PASSWORD";
    private const string Username = "USERNAME";
    private const string NonAsciiPassword = "NON_ASCII_PASSWORD-日本語";
    private const string NonAsciiUsername = "NON_ASCII_USERNAME-日本語";

    #endregion
    #region Tests

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Connect_WithPassword_Succeeds(bool clusterMode)
    {
        var server = fixture.GetServer(clusterMode);
        await server.SetAuthenticationAsync(Password);

        await using var client = await server.CreateClientAsync();
        await AssertConnected(client);

        await server.ClearAuthenticationAsync();
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Connect_WithUsernameAndPassword_Succeeds(bool clusterMode)
    {
        var server = fixture.GetServer(clusterMode);
        await server.SetAuthenticationAsync(Username, Password);

        await using var client = await server.CreateClientAsync();
        await AssertConnected(client);

        await server.ClearAuthenticationAsync();
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Connect_WithNonAsciiPassword_Succeeds(bool clusterMode)
    {
        var server = fixture.GetServer(clusterMode);
        await server.SetAuthenticationAsync(NonAsciiPassword);

        await using var client = await server.CreateClientAsync();
        await AssertConnected(client);

        await server.ClearAuthenticationAsync();
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task Connect_WithNonAsciiUsernameAndPassword_Succeeds(bool clusterMode)
    {
        var server = fixture.GetServer(clusterMode);
        await server.SetAuthenticationAsync(NonAsciiUsername, NonAsciiPassword);

        await using var client = await server.CreateClientAsync();
        await AssertConnected(client);

        await server.ClearAuthenticationAsync();
    }

    #endregion
}
