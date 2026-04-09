// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER compatibility tests for generic commands.
/// Tests verify that Key-prefixed methods work identically to SER equivalents
/// and that CommandFlags validation works correctly.
/// </summary>
public class GenericCommandsTests(GenericCommandsFixture fixture) : IClassFixture<GenericCommandsFixture>
{
    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion

    #region KeyDeleteAsync Tests

    [Fact]
    public async Task KeyDeleteAsync_SingleKey_DeletesKey()
    {
        var db = fixture.Database;
        string key = $"ser-delete-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyExistsAsync(key));

        Assert.True(await db.KeyDeleteAsync(key));
        Assert.False(await db.KeyExistsAsync(key));
    }

    [Fact]
    public async Task KeyDeleteAsync_SingleKey_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-delete-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        bool result = await db.KeyDeleteAsync(key, CommandFlags.None);
        Assert.True(result);
    }

    [Fact]
    public async Task KeyDeleteAsync_SingleKey_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyDeleteAsync("key", UnsupportedCommandFlag));

    [Fact]
    public async Task KeyDeleteAsync_MultipleKeys_DeletesKeys()
    {
        var db = fixture.Database;
        string key1 = $"ser-delete-multi-{Guid.NewGuid()}";
        string key2 = $"ser-delete-multi-{Guid.NewGuid()}";

        await db.StringSetAsync(key1, "value1");
        await db.StringSetAsync(key2, "value2");

        ValkeyKey[] keys = [key1, key2];
        long deleted = await db.KeyDeleteAsync(keys);
        Assert.Equal(2, deleted);
    }

    [Fact]
    public async Task KeyDeleteAsync_MultipleKeys_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key1 = $"ser-delete-multi-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key1, "value1");
        ValkeyKey[] keys = [key1];
        long deleted = await db.KeyDeleteAsync(keys, CommandFlags.None);
        Assert.Equal(1, deleted);
    }

    [Fact]
    public async Task KeyDeleteAsync_MultipleKeys_WithNonNoneCommandFlags_ThrowsNotImplementedException()
    {
        ValkeyKey[] keys = ["key1", "key2"];
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyDeleteAsync(keys, UnsupportedCommandFlag));
    }

    #endregion

    #region KeyExistsAsync Tests

    [Fact]
    public async Task KeyExistsAsync_SingleKey_ReturnsCorrectResult()
    {
        var db = fixture.Database;
        string key = $"ser-exists-{Guid.NewGuid()}";

        Assert.False(await db.KeyExistsAsync(key));

        await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyExistsAsync(key));
    }

    [Fact]
    public async Task KeyExistsAsync_SingleKey_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-exists-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        bool exists = await db.KeyExistsAsync(key, CommandFlags.None);
        Assert.True(exists);
    }

    [Fact]
    public async Task KeyExistsAsync_SingleKey_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyExistsAsync("key", UnsupportedCommandFlag));

    [Fact]
    public async Task KeyExistsAsync_MultipleKeys_ReturnsCount()
    {
        var db = fixture.Database;
        string key1 = $"ser-exists-multi-{Guid.NewGuid()}";
        string key2 = $"ser-exists-multi-{Guid.NewGuid()}";
        string key3 = $"ser-exists-multi-{Guid.NewGuid()}";

        await db.StringSetAsync(key1, "value1");
        await db.StringSetAsync(key2, "value2");

        ValkeyKey[] keys = [key1, key2, key3];
        long count = await db.KeyExistsAsync(keys);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task KeyExistsAsync_MultipleKeys_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key1 = $"ser-exists-multi-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key1, "value1");
        ValkeyKey[] keys = [key1];
        long count = await db.KeyExistsAsync(keys, CommandFlags.None);
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task KeyExistsAsync_MultipleKeys_WithNonNoneCommandFlags_ThrowsNotImplementedException()
    {
        ValkeyKey[] keys = ["key1", "key2"];
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyExistsAsync(keys, UnsupportedCommandFlag));
    }

    #endregion

    #region KeyExpireAsync Tests

    [Fact]
    public async Task KeyExpireAsync_WithTimeSpan_SetsExpiry()
    {
        var db = fixture.Database;
        string key = $"ser-expire-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10)));

        TimeSpan? ttl = await db.KeyTimeToLiveAsync(key);
        _ = Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalSeconds > 0 && ttl.Value.TotalSeconds <= 10);
    }

    [Fact]
    public async Task KeyExpireAsync_WithTimeSpan_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-expire-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        bool result = await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10), CommandFlags.None);
        Assert.True(result);
    }

    [Fact]
    public async Task KeyExpireAsync_WithTimeSpan_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyExpireAsync("key", TimeSpan.FromSeconds(10), UnsupportedCommandFlag));

    [Fact]
    public async Task KeyExpireAsync_WithDateTime_SetsExpiry()
    {
        var db = fixture.Database;
        string key = $"ser-expireat-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        DateTime expiry = DateTime.UtcNow.AddSeconds(10);
        Assert.True(await db.KeyExpireAsync(key, expiry));

        TimeSpan? ttl = await db.KeyTimeToLiveAsync(key);
        _ = Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalSeconds > 0);
    }

    [Fact]
    public async Task KeyExpireAsync_WithDateTime_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-expireat-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        DateTime expiry = DateTime.UtcNow.AddSeconds(10);
        bool result = await db.KeyExpireAsync(key, expiry, CommandFlags.None);
        Assert.True(result);
    }

    [Fact]
    public async Task KeyExpireAsync_WithDateTime_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyExpireAsync("key", DateTime.UtcNow.AddSeconds(10), UnsupportedCommandFlag));

    #endregion

    #region KeyTimeToLiveAsync Tests

    [Fact]
    public async Task KeyTimeToLiveAsync_ReturnsCorrectTTL()
    {
        var db = fixture.Database;
        string key = $"ser-ttl-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        Assert.Null(await db.KeyTimeToLiveAsync(key));

        _ = await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
        TimeSpan? ttl = await db.KeyTimeToLiveAsync(key);
        _ = Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalSeconds > 0 && ttl.Value.TotalSeconds <= 10);
    }

    [Fact]
    public async Task KeyTimeToLiveAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-ttl-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        _ = await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
        TimeSpan? ttl = await db.KeyTimeToLiveAsync(key, CommandFlags.None);
        _ = Assert.NotNull(ttl);
    }

    [Fact]
    public async Task KeyTimeToLiveAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyTimeToLiveAsync("key", UnsupportedCommandFlag));

    #endregion

    #region KeyTypeAsync Tests

    [Fact]
    public async Task KeyTypeAsync_ReturnsCorrectType()
    {
        var db = fixture.Database;
        string stringKey = $"ser-type-string-{Guid.NewGuid()}";
        string listKey = $"ser-type-list-{Guid.NewGuid()}";

        await db.StringSetAsync(stringKey, "value");
        Assert.Equal(ValkeyType.String, await db.KeyTypeAsync(stringKey));

        _ = await fixture.Client.ListLeftPushAsync(listKey, "item");
        Assert.Equal(ValkeyType.List, await db.KeyTypeAsync(listKey));
    }

    [Fact]
    public async Task KeyTypeAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-type-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        ValkeyType type = await db.KeyTypeAsync(key, CommandFlags.None);
        Assert.Equal(ValkeyType.String, type);
    }

    [Fact]
    public async Task KeyTypeAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyTypeAsync("key", UnsupportedCommandFlag));

    #endregion

    #region KeyRenameAsync Tests

    [Fact]
    public async Task KeyRenameAsync_RenamesKey()
    {
        var db = fixture.Database;
        string oldKey = $"ser-rename-old-{Guid.NewGuid()}";
        string newKey = $"ser-rename-new-{Guid.NewGuid()}";

        await db.StringSetAsync(oldKey, "value");
        await db.KeyRenameAsync(oldKey, newKey);

        Assert.False(await db.KeyExistsAsync(oldKey));
        Assert.True(await db.KeyExistsAsync(newKey));
    }

    [Fact]
    public async Task KeyRenameAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string oldKey = $"ser-rename-flags-old-{Guid.NewGuid()}";
        string newKey = $"ser-rename-flags-new-{Guid.NewGuid()}";

        await db.StringSetAsync(oldKey, "value");
        bool result = await db.KeyRenameAsync(oldKey, newKey, CommandFlags.None);
        Assert.True(result);
    }

    [Fact]
    public async Task KeyRenameAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyRenameAsync("oldKey", "newKey", UnsupportedCommandFlag));

    #endregion

    #region KeyPersistAsync Tests

    [Fact]
    public async Task KeyPersistAsync_RemovesExpiry()
    {
        var db = fixture.Database;
        string key = $"ser-persist-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        _ = await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
        _ = Assert.NotNull(await db.KeyTimeToLiveAsync(key));

        Assert.True(await db.KeyPersistAsync(key));
        Assert.Null(await db.KeyTimeToLiveAsync(key));
    }

    [Fact]
    public async Task KeyPersistAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-persist-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        _ = await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
        bool result = await db.KeyPersistAsync(key, CommandFlags.None);
        Assert.True(result);
    }

    [Fact]
    public async Task KeyPersistAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyPersistAsync("key", UnsupportedCommandFlag));

    #endregion

    #region KeyDumpAsync Tests

    [Fact]
    public async Task KeyDumpAsync_ReturnsSerialized()
    {
        var db = fixture.Database;
        string key = $"ser-dump-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        byte[]? dump = await db.KeyDumpAsync(key);
        Assert.NotNull(dump);
        Assert.NotEmpty(dump);
    }

    [Fact]
    public async Task KeyDumpAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-dump-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        byte[]? dump = await db.KeyDumpAsync(key, CommandFlags.None);
        Assert.NotNull(dump);
    }

    [Fact]
    public async Task KeyDumpAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyDumpAsync("key", UnsupportedCommandFlag));

    #endregion

    #region KeyTouchAsync Tests

    [Fact]
    public async Task KeyTouchAsync_SingleKey_TouchesKey()
    {
        var db = fixture.Database;
        string key = $"ser-touch-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyTouchAsync(key));
    }

    [Fact]
    public async Task KeyTouchAsync_SingleKey_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-touch-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        bool result = await db.KeyTouchAsync(key, CommandFlags.None);
        Assert.True(result);
    }

    [Fact]
    public async Task KeyTouchAsync_SingleKey_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyTouchAsync("key", UnsupportedCommandFlag));

    [Fact]
    public async Task KeyTouchAsync_MultipleKeys_TouchesKeys()
    {
        var db = fixture.Database;
        string key1 = $"ser-touch-multi-{Guid.NewGuid()}";
        string key2 = $"ser-touch-multi-{Guid.NewGuid()}";

        await db.StringSetAsync(key1, "value1");
        await db.StringSetAsync(key2, "value2");

        ValkeyKey[] keys = [key1, key2];
        long touched = await db.KeyTouchAsync(keys);
        Assert.Equal(2, touched);
    }

    [Fact]
    public async Task KeyTouchAsync_MultipleKeys_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key1 = $"ser-touch-multi-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key1, "value1");
        ValkeyKey[] keys = [key1];
        long touched = await db.KeyTouchAsync(keys, CommandFlags.None);
        Assert.Equal(1, touched);
    }

    [Fact]
    public async Task KeyTouchAsync_MultipleKeys_WithNonNoneCommandFlags_ThrowsNotImplementedException()
    {
        ValkeyKey[] keys = ["key1", "key2"];
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyTouchAsync(keys, UnsupportedCommandFlag));
    }

    #endregion

    #region KeyCopyAsync Tests

    [Fact]
    public async Task KeyCopyAsync_CopiesKey()
    {
        var db = fixture.Database;
        string sourceKey = $"ser-copy-src-{Guid.NewGuid()}";
        string destKey = $"ser-copy-dst-{Guid.NewGuid()}";

        await db.StringSetAsync(sourceKey, "value");
        Assert.True(await db.KeyCopyAsync(sourceKey, destKey));

        Assert.True(await db.KeyExistsAsync(sourceKey));
        Assert.True(await db.KeyExistsAsync(destKey));
        Assert.Equal("value", (await db.StringGetAsync(destKey)).ToString());
    }

    [Fact]
    public async Task KeyCopyAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string sourceKey = $"ser-copy-flags-src-{Guid.NewGuid()}";
        string destKey = $"ser-copy-flags-dst-{Guid.NewGuid()}";

        await db.StringSetAsync(sourceKey, "value");
        bool result = await db.KeyCopyAsync(sourceKey, destKey, false, CommandFlags.None);
        Assert.True(result);
    }

    [Fact]
    public async Task KeyCopyAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyCopyAsync("src", "dst", false, UnsupportedCommandFlag));

    #endregion

    #region KeyRandomAsync Tests

    [Fact]
    public async Task KeyRandomAsync_ReturnsKey()
    {
        var db = fixture.Database;
        string key = $"ser-random-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        string? randomKey = await db.KeyRandomAsync();
        Assert.NotNull(randomKey);
    }

    [Fact]
    public async Task KeyRandomAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-random-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "value");
        string? randomKey = await db.KeyRandomAsync(CommandFlags.None);
        Assert.NotNull(randomKey);
    }

    [Fact]
    public async Task KeyRandomAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.KeyRandomAsync(UnsupportedCommandFlag));

    #endregion

    #region SortAsync Tests

    [Fact]
    public async Task SortAsync_SortsElements()
    {
        var db = fixture.Database;
        string key = $"ser-sort-{Guid.NewGuid()}";

        _ = await fixture.Client.ListLeftPushAsync(key, ["3", "1", "2"]);
        ValkeyValue[] result = await db.SortAsync(key);
        Assert.Equal(new[] { "1", "2", "3" }, result.Select(v => v.ToString()).ToArray());
    }

    [Fact]
    public async Task SortAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-sort-flags-{Guid.NewGuid()}";

        _ = await fixture.Client.ListLeftPushAsync(key, ["3", "1", "2"]);
        ValkeyValue[] result = await db.SortAsync(key, flags: CommandFlags.None);
        Assert.Equal(3, result.Length);
    }

    [Fact]
    public async Task SortAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.SortAsync("key", flags: UnsupportedCommandFlag));

    #endregion
}

/// <summary>
/// Fixture class for <see cref="GenericCommandsTests" />.
/// </summary>
public class GenericCommandsFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer;
    private readonly ConnectionMultiplexer _connection;
    private readonly GlideClient _client;

    public IDatabase Database { get; }
    public GlideClient Client => _client;

    public GenericCommandsFixture()
    {
        _standaloneServer = new();
        var (host, port) = _standaloneServer.Addresses.First();

        ConfigurationOptions config = new();
        config.EndPoints.Add(host, port);
        _connection = ConnectionMultiplexer.Connect(config);

        Database = _connection.GetDatabase();

        // Create GLIDE client for operations not available on IDatabase
        var glideConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(host, port)
            .Build();
        _client = GlideClient.CreateClient(glideConfig).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _client.Dispose();
        _connection.Dispose();
        _standaloneServer.Dispose();
    }
}
