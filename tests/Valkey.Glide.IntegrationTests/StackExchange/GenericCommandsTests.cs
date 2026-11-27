// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER compatibility tests for generic commands.
/// Tests verify that Key-prefixed methods work identically to SER equivalents.
/// </summary>
public class GenericCommandsTests(GenericCommandsFixture fixture) : IClassFixture<GenericCommandsFixture>
{
    #region KeyDeleteAsync Tests

    [Fact]
    public async Task KeyDeleteAsync_SingleKey_DeletesKey()
    {
        var db = fixture.Database;
        string key = $"ser-delete-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyExistsAsync(key));

        Assert.True(await db.KeyDeleteAsync(key));
        Assert.False(await db.KeyExistsAsync(key));
    }

    [Fact]
    public async Task KeyDeleteAsync_MultipleKeys_DeletesKeys()
    {
        var db = fixture.Database;
        string key1 = $"ser-delete-multi-{Guid.NewGuid()}";
        string key2 = $"ser-delete-multi-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key1, "value1");
        _ = await db.StringSetAsync(key2, "value2");

        ValkeyKey[] keys = [key1, key2];
        long deleted = await db.KeyDeleteAsync(keys);
        Assert.Equal(2, deleted);
    }

    #endregion
    #region KeyExistsAsync Tests

    [Fact]
    public async Task KeyExistsAsync_SingleKey_ReturnsCorrectResult()
    {
        var db = fixture.Database;
        string key = $"ser-exists-{Guid.NewGuid()}";

        Assert.False(await db.KeyExistsAsync(key));

        _ = await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyExistsAsync(key));
    }

    [Fact]
    public async Task KeyExistsAsync_MultipleKeys_ReturnsCount()
    {
        var db = fixture.Database;
        string key1 = $"ser-exists-multi-{Guid.NewGuid()}";
        string key2 = $"ser-exists-multi-{Guid.NewGuid()}";
        string key3 = $"ser-exists-multi-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key1, "value1");
        _ = await db.StringSetAsync(key2, "value2");

        ValkeyKey[] keys = [key1, key2, key3];
        long count = await db.KeyExistsAsync(keys);
        Assert.Equal(2, count);
    }

    #endregion
    #region KeyExpireAsync Tests

    [Fact]
    public async Task KeyExpireAsync_WithTimeSpan_SetsExpiry()
    {
        var db = fixture.Database;
        string key = $"ser-expire-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10)));

        TimeSpan? ttl = await db.KeyTimeToLiveAsync(key);
        _ = Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalSeconds is > 0 and <= 10);
    }

    [Fact]
    public async Task KeyExpireAsync_WithNegativeTimeSpan_DeletesKey()
    {
        var db = fixture.Database;
        string key = $"ser-expire-negative-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyExpireAsync(key, TimeSpan.FromSeconds(-5)));
        Assert.False(await db.KeyExistsAsync(key));
    }

    [Fact]
    public async Task KeyExpireAsync_WithDateTime_SetsExpiry()
    {
        var db = fixture.Database;
        string key = $"ser-expireat-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        DateTime expiry = DateTime.UtcNow.AddSeconds(10);
        Assert.True(await db.KeyExpireAsync(key, expiry));

        TimeSpan? ttl = await db.KeyTimeToLiveAsync(key);
        _ = Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalSeconds > 0);
    }

    #endregion
    #region KeyTimeToLiveAsync Tests

    [Fact]
    public async Task KeyTimeToLiveAsync_ReturnsCorrectTTL()
    {
        var db = fixture.Database;
        string key = $"ser-ttl-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        Assert.Null(await db.KeyTimeToLiveAsync(key));

        _ = await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
        TimeSpan? ttl = await db.KeyTimeToLiveAsync(key);
        _ = Assert.NotNull(ttl);
        Assert.True(ttl.Value.TotalSeconds is > 0 and <= 10);
    }

    [Fact]
    public async Task KeyTimeToLiveAsync_ReturnsNullForNonExistentKey()
    {
        var db = fixture.Database;
        string nonExistentKey = $"ser-ttl-nonexistent-{Guid.NewGuid()}";

        // Non-existent key should return null (sentinel -2 converted to null)
        Assert.Null(await db.KeyTimeToLiveAsync(nonExistentKey));
    }

    [Fact]
    public async Task KeyTimeToLiveAsync_ReturnsNullForKeyWithNoExpiry()
    {
        var db = fixture.Database;
        string key = $"ser-ttl-noexpiry-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");

        // Key exists but has no expiry should return null (sentinel -1 converted to null)
        Assert.Null(await db.KeyTimeToLiveAsync(key));
    }

    #endregion
    #region KeyTypeAsync Tests

    [Fact]
    public async Task KeyTypeAsync_ReturnsCorrectType()
    {
        var db = fixture.Database;
        string stringKey = $"ser-type-string-{Guid.NewGuid()}";
        string listKey = $"ser-type-list-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(stringKey, "value");
        Assert.Equal(ValkeyType.String, await db.KeyTypeAsync(stringKey));

        _ = await fixture.Client.ListLeftPushAsync(listKey, "item");
        Assert.Equal(ValkeyType.List, await db.KeyTypeAsync(listKey));
    }

    #endregion
    #region KeyRenameAsync Tests

    [Fact]
    public async Task KeyRenameAsync_RenamesKey()
    {
        var db = fixture.Database;
        string oldKey = $"ser-rename-old-{Guid.NewGuid()}";
        string newKey = $"ser-rename-new-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(oldKey, "value");
        _ = await db.KeyRenameAsync(oldKey, newKey);

        Assert.False(await db.KeyExistsAsync(oldKey));
        Assert.True(await db.KeyExistsAsync(newKey));
    }

    [Fact]
    public async Task KeyRenameAsync_WithWhenAlways_RenamesKey()
    {
        var db = fixture.Database;
        string oldKey = $"ser-rename-when-always-old-{Guid.NewGuid()}";
        string newKey = $"ser-rename-when-always-new-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(oldKey, "value");
        bool result = await db.KeyRenameAsync(oldKey, newKey, When.Always);
        Assert.True(result);
        Assert.False(await db.KeyExistsAsync(oldKey));
        Assert.True(await db.KeyExistsAsync(newKey));
    }

    [Fact]
    public async Task KeyRenameAsync_WithWhenNotExists_RenamesKeyIfNotExists()
    {
        var db = fixture.Database;
        string oldKey = $"ser-rename-when-notexists-old-{Guid.NewGuid()}";
        string newKey = $"ser-rename-when-notexists-new-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(oldKey, "value");
        bool result = await db.KeyRenameAsync(oldKey, newKey, When.NotExists);
        Assert.True(result);
        Assert.False(await db.KeyExistsAsync(oldKey));
        Assert.True(await db.KeyExistsAsync(newKey));
    }

    [Fact]
    public async Task KeyRenameAsync_WithWhenNotExists_ReturnsFalseIfExists()
    {
        var db = fixture.Database;
        string oldKey = $"ser-rename-when-notexists-fail-old-{Guid.NewGuid()}";
        string newKey = $"ser-rename-when-notexists-fail-new-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(oldKey, "value1");
        _ = await db.StringSetAsync(newKey, "value2");
        bool result = await db.KeyRenameAsync(oldKey, newKey, When.NotExists);
        Assert.False(result);
        // Both keys should still exist
        Assert.True(await db.KeyExistsAsync(oldKey));
        Assert.True(await db.KeyExistsAsync(newKey));
    }

    [Fact]
    public async Task KeyRenameAsync_WithWhenExists_ThrowsArgumentException()
        => _ = await Assert.ThrowsAsync<ArgumentException>(
            () => fixture.Database.KeyRenameAsync("oldKey", "newKey", When.Exists));

    #endregion
    #region KeyPersistAsync Tests

    [Fact]
    public async Task KeyPersistAsync_RemovesExpiry()
    {
        var db = fixture.Database;
        string key = $"ser-persist-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        _ = await db.KeyExpireAsync(key, TimeSpan.FromSeconds(10));
        _ = Assert.NotNull(await db.KeyTimeToLiveAsync(key));

        Assert.True(await db.KeyPersistAsync(key));
        Assert.Null(await db.KeyTimeToLiveAsync(key));
    }

    #endregion
    #region KeyDumpAsync Tests

    [Fact]
    public async Task KeyDumpAsync_ReturnsSerialized()
    {
        var db = fixture.Database;
        string key = $"ser-dump-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        byte[]? dump = await db.KeyDumpAsync(key);
        Assert.NotNull(dump);
        Assert.NotEmpty(dump);
    }

    #endregion
    #region KeyRestoreAsync Tests

    [Fact]
    public async Task KeyRestoreAsync_WithTimeSpan_RestoresKey()
    {
        var db = fixture.Database;
        string sourceKey = $"ser-restore-src-{Guid.NewGuid()}";
        string destKey = $"ser-restore-dst-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(sourceKey, "value");
        byte[]? dump = await db.KeyDumpAsync(sourceKey);
        Assert.NotNull(dump);

        await db.KeyRestoreAsync(destKey, dump, TimeSpan.FromSeconds(10));
        Assert.True(await db.KeyExistsAsync(destKey));
        Assert.Equal("value", (await db.StringGetAsync(destKey)).ToString());
    }

    [Fact]
    public async Task KeyRestoreAsync_WithDateTime_RestoresKey()
    {
        var db = fixture.Database;
        string sourceKey = $"ser-restore-dt-src-{Guid.NewGuid()}";
        string destKey = $"ser-restore-dt-dst-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(sourceKey, "value");
        byte[]? dump = await db.KeyDumpAsync(sourceKey);
        Assert.NotNull(dump);

        DateTime expiry = DateTime.UtcNow.AddSeconds(10);
        await db.KeyRestoreAsync(destKey, dump, expiry);
        Assert.True(await db.KeyExistsAsync(destKey));
        Assert.Equal("value", (await db.StringGetAsync(destKey)).ToString());
    }

    #endregion
    #region KeyTouchAsync Tests

    [Fact]
    public async Task KeyTouchAsync_SingleKey_TouchesKey()
    {
        var db = fixture.Database;
        string key = $"ser-touch-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        Assert.True(await db.KeyTouchAsync(key));
    }

    [Fact]
    public async Task KeyTouchAsync_MultipleKeys_TouchesKeys()
    {
        var db = fixture.Database;
        string key1 = $"ser-touch-multi-{Guid.NewGuid()}";
        string key2 = $"ser-touch-multi-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key1, "value1");
        _ = await db.StringSetAsync(key2, "value2");

        ValkeyKey[] keys = [key1, key2];
        long touched = await db.KeyTouchAsync(keys);
        Assert.Equal(2, touched);
    }

    #endregion
    #region KeyCopyAsync Tests

    [Fact]
    public async Task KeyCopyAsync_CopiesKey()
    {
        var db = fixture.Database;
        string sourceKey = $"ser-copy-src-{Guid.NewGuid()}";
        string destKey = $"ser-copy-dst-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(sourceKey, "value");
        Assert.True(await db.KeyCopyAsync(sourceKey, destKey));

        Assert.True(await db.KeyExistsAsync(sourceKey));
        Assert.True(await db.KeyExistsAsync(destKey));
        Assert.Equal("value", (await db.StringGetAsync(destKey)).ToString());
    }

    #endregion
    #region KeyIdleTimeAsync Tests

    [Fact]
    public async Task KeyIdleTimeAsync_ReturnsIdleTime()
    {
        var db = fixture.Database;
        string key = $"ser-idletime-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        TimeSpan? idleTime = await db.KeyIdleTimeAsync(key);
        _ = Assert.NotNull(idleTime);
        Assert.True(idleTime.Value.TotalSeconds >= 0);
    }

    [Fact]
    public async Task KeyIdleTimeAsync_NonExistentKey_ReturnsNull()
    {
        var db = fixture.Database;
        string key = $"ser-idletime-nonexistent-{Guid.NewGuid()}";

        TimeSpan? idleTime = await db.KeyIdleTimeAsync(key);
        Assert.Null(idleTime);
    }

    #endregion
    #region KeyRandomAsync Tests

    [Fact]
    public async Task KeyRandomAsync_ReturnsKey()
    {
        var db = fixture.Database;
        string key = $"ser-random-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value");
        ValkeyKey randomKey = await db.KeyRandomAsync();
        Assert.False(randomKey.IsNull);
    }

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

    #endregion
    #region SortAndStoreAsync Tests

    [Fact]
    public async Task SortAndStoreAsync_StoresSortedElements()
    {
        var db = fixture.Database;
        string sourceKey = $"ser-sortstore-src-{Guid.NewGuid()}";
        string destKey = $"ser-sortstore-dst-{Guid.NewGuid()}";

        _ = await fixture.Client.ListLeftPushAsync(sourceKey, ["3", "1", "2"]);
        long count = await db.SortAndStoreAsync(destKey, sourceKey);
        Assert.Equal(3, count);

        ValkeyValue[] result = await fixture.Client.ListRangeAsync(destKey);
        Assert.Equal(new[] { "1", "2", "3" }, result.Select(v => v.ToString()).ToArray());
    }

    #endregion
    #region ExecuteAsync Tests

    [Fact]
    public async Task ExecuteAsync_WithKeyAndValue_ReturnsResult()
    {
        var db = fixture.Database;
        ValkeyKey key = $"ser-execute-{Guid.NewGuid()}";
        ValkeyValue value = "value";

        Assert.Equal("OK", await db.ExecuteAsync("SET", key, value));
        Assert.Equal(value, await db.ExecuteAsync("GET", key));
    }

    [Fact]
    public async Task ExecuteAsync_WithChannel_ReturnsResult()
    {
        var db = fixture.Database;
        var channel = ValkeyChannel.Literal($"ser-execute-channel-{Guid.NewGuid()}");

        // No subscribers, so PUBLISH reports zero receivers.
        Assert.Equal(0L, await db.ExecuteAsync("PUBLISH", channel, "message"));
    }

    [Fact]
    public async Task ExecuteAsync_UnsupportedArg_ThrowsInvalidCastException()
        => await Assert.ThrowsAsync<InvalidCastException>(() => fixture.Database.ExecuteAsync("ECHO", DateTime.UtcNow));

    #endregion
}

/// <summary>
/// Fixture class for <see cref="GenericCommandsTests" />.
/// </summary>
public class GenericCommandsFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer;
    private readonly ConnectionMultiplexer _connection;

    public GenericCommandsFixture()
    {
        _standaloneServer = new();
        var (host, port) = _standaloneServer.Address;

        ConfigurationOptions config = new();
        config.EndPoints.Add(host, port);
        _connection = ConnectionMultiplexer.Connect(config);

        Database = _connection.GetDatabase();

        // Create GLIDE client for operations not available on IDatabase
        var glideConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(host, port)
            .Build();
        Client = GlideClient.CreateClient(glideConfig).GetAwaiter().GetResult();
    }

    public IDatabase Database { get; }
    public GlideClient Client { get; }

    public void Dispose()
    {
        Client.Dispose();
        _connection.Dispose();
        _standaloneServer.Dispose();
    }
}
