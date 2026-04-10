// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER compatibility tests for bitmap commands.
/// Tests verify that String-prefixed methods work identically to SER equivalents
/// and that CommandFlags validation works correctly.
/// </summary>
public class BitmapCommandsTests(BitmapCommandsFixture fixture) : IClassFixture<BitmapCommandsFixture>
{
    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion

    #region StringGetBitAsync Tests

    [Fact]
    public async Task StringGetBitAsync_ReturnsCorrectBitValue()
    {
        var db = fixture.Database;
        string key = $"ser-getbit-{Guid.NewGuid()}";

        // Set a string value - ASCII 'A' is 01000001 in binary
        await db.StringSetAsync(key, "A");

        // Test bit positions in 'A' (01000001)
        Assert.False(await db.StringGetBitAsync(key, 0));
        Assert.True(await db.StringGetBitAsync(key, 1));
        Assert.False(await db.StringGetBitAsync(key, 2));
        Assert.True(await db.StringGetBitAsync(key, 7));
    }

    [Fact]
    public async Task StringGetBitAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-getbit-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "A");

        // CommandFlags.None should be accepted without error
        bool bit = await db.StringGetBitAsync(key, 1, CommandFlags.None);
        Assert.True(bit);
    }

    [Fact]
    public async Task StringGetBitAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.StringGetBitAsync("key", 0, UnsupportedCommandFlag));

    #endregion

    #region StringSetBitAsync Tests

    [Fact]
    public async Task StringSetBitAsync_SetsAndReturnsOriginalValue()
    {
        var db = fixture.Database;
        string key = $"ser-setbit-{Guid.NewGuid()}";

        // Set bit 1 to true (original should be false)
        bool originalBit = await db.StringSetBitAsync(key, 1, true);
        Assert.False(originalBit);

        // Verify bit is now set
        bool currentBit = await db.StringGetBitAsync(key, 1);
        Assert.True(currentBit);
    }

    [Fact]
    public async Task StringSetBitAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-setbit-flags-{Guid.NewGuid()}";

        // CommandFlags.None should be accepted without error
        bool originalBit = await db.StringSetBitAsync(key, 0, true, CommandFlags.None);
        Assert.False(originalBit);
    }

    [Fact]
    public async Task StringSetBitAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.StringSetBitAsync("key", 0, true, UnsupportedCommandFlag));

    #endregion

    #region StringBitCountAsync Tests

    [Fact]
    public async Task StringBitCountAsync_CountsSetBits()
    {
        var db = fixture.Database;
        string key = $"ser-bitcount-{Guid.NewGuid()}";

        // Set string to "A" (ASCII 65 = 01000001 in binary = 2 bits set)
        await db.StringSetAsync(key, "A");

        // Count all bits
        long count = await db.StringBitCountAsync(key);
        Assert.Equal(2, count);

        // Count bits in byte range
        long countRange = await db.StringBitCountAsync(key, 0, 0);
        Assert.Equal(2, countRange);
    }

    [Fact]
    public async Task StringBitCountAsync_WithStringIndexType_WorksCorrectly()
    {
        var db = fixture.Database;
        string key = $"ser-bitcount-indextype-{Guid.NewGuid()}";

        // Set string to "AB" (2 bytes)
        await db.StringSetAsync(key, "AB");

        // Count bits using byte index type (default)
        long countByte = await db.StringBitCountAsync(key, 0, 0, StringIndexType.Byte);
        Assert.Equal(2, countByte); // 'A' has 2 bits set
    }

    [Fact]
    public async Task StringBitCountAsync_WithBitIndexType_WorksCorrectly()
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("7.0.0"),
            "BIT index type for BITCOUNT requires server version 7.0 or higher"
        );

        var db = fixture.Database;
        string key = $"ser-bitcount-bit-{Guid.NewGuid()}";

        // Set multiple bits
        _ = await db.StringSetBitAsync(key, 0, true);
        _ = await db.StringSetBitAsync(key, 1, true);
        _ = await db.StringSetBitAsync(key, 8, true);

        // Count bits 0-7 using bit indexing
        long count = await db.StringBitCountAsync(key, 0, 7, StringIndexType.Bit);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task StringBitCountAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-bitcount-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "A");

        // CommandFlags.None should be accepted without error
        long count = await db.StringBitCountAsync(key, 0, -1, StringIndexType.Byte, CommandFlags.None);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task StringBitCountAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.StringBitCountAsync("key", 0, -1, StringIndexType.Byte, UnsupportedCommandFlag));

    #endregion

    #region StringBitPositionAsync Tests

    [Fact]
    public async Task StringBitPositionAsync_FindsFirstSetBit()
    {
        var db = fixture.Database;
        string key = $"ser-bitpos-{Guid.NewGuid()}";

        // Set string to "A" (ASCII 65 = 01000001 in binary)
        await db.StringSetAsync(key, "A");

        // Find first set bit (should be at position 1)
        long pos1 = await db.StringBitPositionAsync(key, true);
        Assert.Equal(1, pos1);

        // Find first unset bit (should be at position 0)
        long pos0 = await db.StringBitPositionAsync(key, false);
        Assert.Equal(0, pos0);
    }

    [Fact]
    public async Task StringBitPositionAsync_WithStringIndexType_WorksCorrectly()
    {
        var db = fixture.Database;
        string key = $"ser-bitpos-indextype-{Guid.NewGuid()}";

        // Set string to "AB"
        await db.StringSetAsync(key, "AB");

        // Find first set bit starting from second byte using byte indexing
        long pos = await db.StringBitPositionAsync(key, true, 1, 1, StringIndexType.Byte);
        Assert.Equal(9, pos); // First set bit in 'B' at position 9
    }

    [Fact]
    public async Task StringBitPositionAsync_WithBitIndexType_WorksCorrectly()
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("7.0.0"),
            "BIT index type for BITPOS requires server version 7.0 or higher"
        );

        var db = fixture.Database;
        string key = $"ser-bitpos-bit-{Guid.NewGuid()}";

        // Set multiple bits
        _ = await db.StringSetBitAsync(key, 1, true);
        _ = await db.StringSetBitAsync(key, 9, true);

        // Find first set bit starting from bit 8 using bit indexing
        long pos = await db.StringBitPositionAsync(key, true, 8, -1, StringIndexType.Bit);
        Assert.Equal(9, pos);
    }

    [Fact]
    public async Task StringBitPositionAsync_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string key = $"ser-bitpos-flags-{Guid.NewGuid()}";

        await db.StringSetAsync(key, "A");

        // CommandFlags.None should be accepted without error
        long pos = await db.StringBitPositionAsync(key, true, 0, -1, StringIndexType.Byte, CommandFlags.None);
        Assert.Equal(1, pos);
    }

    [Fact]
    public async Task StringBitPositionAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.StringBitPositionAsync("key", true, 0, -1, StringIndexType.Byte, UnsupportedCommandFlag));

    #endregion

    #region StringBitOperationAsync Tests

    [Fact]
    public async Task StringBitOperationAsync_TwoKeys_PerformsCorrectOperation()
    {
        var db = fixture.Database;
        string keyPrefix = $"{{ser-bitop-{Guid.NewGuid()}}}";
        string key1 = $"{keyPrefix}:key1";
        string key2 = $"{keyPrefix}:key2";
        string result = $"{keyPrefix}:result";

        // Set key1 to "A" (01000001) and key2 to "B" (01000010)
        await db.StringSetAsync(key1, "A");
        await db.StringSetAsync(key2, "B");

        // Perform AND operation using two-key overload
        long size = await db.StringBitOperationAsync(Bitwise.And, result, key1, key2);
        Assert.Equal(1, size);

        // Verify result: A AND B = 01000001 AND 01000010 = 01000000 = '@'
        ValkeyValue resultValue = await db.StringGetAsync(result);
        Assert.Equal("@", resultValue.ToString());
    }

    [Fact]
    public async Task StringBitOperationAsync_WithDefaultSecond_WorksCorrectly()
    {
        var db = fixture.Database;
        string keyPrefix = $"{{ser-bitop-default-{Guid.NewGuid()}}}";
        string key1 = $"{keyPrefix}:key1";
        string result = $"{keyPrefix}:result";

        // Set key1 to "A" (01000001)
        await db.StringSetAsync(key1, "A");

        // Perform NOT operation with second = default (only first key used)
        long size = await db.StringBitOperationAsync(Bitwise.Not, result, key1, default);
        Assert.Equal(1, size);

        // Verify result: NOT A = NOT 01000001 = 10111110 = 190
        ValkeyValue resultValue = await db.StringGetAsync(result);
        byte[] resultBytes = resultValue!;
        Assert.Equal(190, resultBytes[0]);
    }

    [Fact]
    public async Task StringBitOperationAsync_MultipleKeys_PerformsCorrectOperation()
    {
        var db = fixture.Database;
        string keyPrefix = $"{{ser-bitop-multi-{Guid.NewGuid()}}}";
        string key1 = $"{keyPrefix}:key1";
        string key2 = $"{keyPrefix}:key2";
        string key3 = $"{keyPrefix}:key3";
        string result = $"{keyPrefix}:result";

        // Set keys with different bit patterns
        await db.StringSetAsync(key1, "A"); // 01000001
        await db.StringSetAsync(key2, "B"); // 01000010
        await db.StringSetAsync(key3, "D"); // 01000100

        // Perform OR operation on multiple keys using IEnumerable overload
        ValkeyKey[] keys = [key1, key2, key3];
        long size = await db.StringBitOperationAsync(Bitwise.Or, result, keys);
        Assert.Equal(1, size);

        // Verify result: A OR B OR D = 01000001 OR 01000010 OR 01000100 = 01000111 = 'G'
        ValkeyValue resultValue = await db.StringGetAsync(result);
        Assert.Equal("G", resultValue.ToString());
    }

    [Fact]
    public async Task StringBitOperationAsync_TwoKeys_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string keyPrefix = $"{{ser-bitop-flags-{Guid.NewGuid()}}}";
        string key1 = $"{keyPrefix}:key1";
        string key2 = $"{keyPrefix}:key2";
        string result = $"{keyPrefix}:result";

        await db.StringSetAsync(key1, "A");
        await db.StringSetAsync(key2, "B");

        // CommandFlags.None should be accepted without error
        long size = await db.StringBitOperationAsync(Bitwise.And, result, key1, key2, CommandFlags.None);
        Assert.Equal(1, size);
    }

    [Fact]
    public async Task StringBitOperationAsync_TwoKeys_WithNonNoneCommandFlags_ThrowsNotImplementedException()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.StringBitOperationAsync(Bitwise.And, "result", "key1", "key2", UnsupportedCommandFlag));

    [Fact]
    public async Task StringBitOperationAsync_MultipleKeys_WithCommandFlagsNone_Succeeds()
    {
        var db = fixture.Database;
        string keyPrefix = $"{{ser-bitop-multi-flags-{Guid.NewGuid()}}}";
        string key1 = $"{keyPrefix}:key1";
        string key2 = $"{keyPrefix}:key2";
        string result = $"{keyPrefix}:result";

        await db.StringSetAsync(key1, "A");
        await db.StringSetAsync(key2, "B");

        // CommandFlags.None should be accepted without error
        ValkeyKey[] keys = [key1, key2];
        long size = await db.StringBitOperationAsync(Bitwise.Or, result, keys, CommandFlags.None);
        Assert.Equal(1, size);
    }

    [Fact]
    public async Task StringBitOperationAsync_MultipleKeys_WithNonNoneCommandFlags_ThrowsNotImplementedException()
    {
        ValkeyKey[] keys = ["key1", "key2"];
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.StringBitOperationAsync(Bitwise.And, "result", keys, UnsupportedCommandFlag));
    }

    #endregion
}

/// <summary>
/// Fixture class for <see cref="BitmapCommandsTests" />.
/// </summary>
public class BitmapCommandsFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer;
    private readonly ConnectionMultiplexer _connection;
    private readonly GlideClient _client;

    public IDatabase Database { get; }
    public GlideClient Client => _client;

    public BitmapCommandsFixture()
    {
        _standaloneServer = new();
        var (host, port) = _standaloneServer.Addresses.First();

        ConfigurationOptions config = new();
        config.EndPoints.Add(host, port);
        _connection = ConnectionMultiplexer.Connect(config);

        Database = _connection.GetDatabase();

        // Create GLIDE client for equivalence tests
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
