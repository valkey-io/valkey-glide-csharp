// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync.PublishAsync(ValkeyChannel, ValkeyValue, CommandFlags)" />.
/// </summary>
public class PublishCommandTests(PublishCommandFixture fixture) : IClassFixture<PublishCommandFixture>
{
    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion
    #region Tests

    [Fact]
    public async Task PublishAsync_ReturnsLong()
    {
        var db = fixture.Database;
        var channel = ValkeyChannel.Literal($"test-channel-{Guid.NewGuid()}");

        long result = await db.PublishAsync(channel, "hello");

        // No subscribers, so result should be 0.
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task PublishAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.PublishAsync(
                ValkeyChannel.Literal("test-channel"),
                "hello",
                UnsupportedCommandFlag));

    [Fact]
    public async Task PublishAsync_NullOrEmptyChannel_Throws()
    {
        var db = fixture.Database;
        var emptyChannel = new ValkeyChannel((byte[]?)null, ValkeyChannel.PatternMode.Literal);

        _ = await Assert.ThrowsAsync<ArgumentException>(
            () => db.PublishAsync(emptyChannel, "hello"));
    }

    #endregion
}

/// <summary>
/// Fixture class for <see cref="PublishCommandTests" />.
/// </summary>
public class PublishCommandFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer;
    private readonly ConnectionMultiplexer _connection;

    public IDatabase Database { get; }

    public PublishCommandFixture()
    {
        _standaloneServer = new();
        var (host, port) = _standaloneServer.Addresses.First();

        ConfigurationOptions config = new();
        config.EndPoints.Add(host, port);
        _connection = ConnectionMultiplexer.Connect(config);

        Database = _connection.GetDatabase();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _standaloneServer.Dispose();
    }
}
