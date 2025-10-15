// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

public class BitmapCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetBit_ReturnsCorrectBitValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Set a string value - ASCII 'A' is 01000001 in binary
        await client.StringSetAsync(key, "A");
        
        // Test bit positions in 'A' (01000001)
        bool bit0 = await client.StringGetBitAsync(key, 0); // Should be false (0)
        bool bit1 = await client.StringGetBitAsync(key, 1); // Should be true (1)
        bool bit2 = await client.StringGetBitAsync(key, 2); // Should be false (0)
        bool bit6 = await client.StringGetBitAsync(key, 6); // Should be false (0)
        bool bit7 = await client.StringGetBitAsync(key, 7); // Should be true (1)
        
        Assert.False(bit0);
        Assert.True(bit1);
        Assert.False(bit2);
        Assert.False(bit6);
        Assert.True(bit7);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetBit_NonExistentKey_ReturnsFalse(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Test bit on non-existent key
        bool bit = await client.StringGetBitAsync(key, 0);
        
        Assert.False(bit);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetBit_OffsetBeyondString_ReturnsFalse(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Set a short string
        await client.StringSetAsync(key, "A");
        
        // Test bit beyond the string length
        bool bit = await client.StringGetBitAsync(key, 100);
        
        Assert.False(bit);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetBit_SetsAndReturnsOriginalValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Set bit 1 to true (original should be false)
        bool originalBit = await client.StringSetBitAsync(key, 1, true);
        Assert.False(originalBit);
        
        // Verify bit is now set
        bool currentBit = await client.StringGetBitAsync(key, 1);
        Assert.True(currentBit);
        
        // Set bit 1 to false (original should be true)
        bool originalBit2 = await client.StringSetBitAsync(key, 1, false);
        Assert.True(originalBit2);
        
        // Verify bit is now cleared
        bool currentBit2 = await client.StringGetBitAsync(key, 1);
        Assert.False(currentBit2);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetBit_NonExistentKey_CreatesKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Set bit on non-existent key
        bool originalBit = await client.StringSetBitAsync(key, 0, true);
        Assert.False(originalBit);
        
        // Verify key was created and bit is set
        bool currentBit = await client.StringGetBitAsync(key, 0);
        Assert.True(currentBit);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetSetBit_CombinedOperations_WorksTogether(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Initially all bits should be 0
        bool bit0 = await client.StringGetBitAsync(key, 0);
        bool bit1 = await client.StringGetBitAsync(key, 1);
        Assert.False(bit0);
        Assert.False(bit1);
        
        // Set bit 0 to 1
        bool originalBit0 = await client.StringSetBitAsync(key, 0, true);
        Assert.False(originalBit0); // Was 0
        
        // Set bit 1 to 1
        bool originalBit1 = await client.StringSetBitAsync(key, 1, true);
        Assert.False(originalBit1); // Was 0
        
        // Verify both bits are now set
        bool newBit0 = await client.StringGetBitAsync(key, 0);
        bool newBit1 = await client.StringGetBitAsync(key, 1);
        Assert.True(newBit0);
        Assert.True(newBit1);
        
        // Clear bit 0
        bool clearBit0 = await client.StringSetBitAsync(key, 0, false);
        Assert.True(clearBit0); // Was 1
        
        // Verify bit 0 is cleared but bit 1 remains set
        bool finalBit0 = await client.StringGetBitAsync(key, 0);
        bool finalBit1 = await client.StringGetBitAsync(key, 1);
        Assert.False(finalBit0);
        Assert.True(finalBit1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_CountsSetBits(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Set string to "A" (ASCII 65 = 01000001 in binary = 2 bits set)
        await client.StringSetAsync(key, "A");
        
        // Count all bits
        long count = await client.StringBitCountAsync(key);
        Assert.Equal(2, count);
        
        // Count bits in byte range
        long countRange = await client.StringBitCountAsync(key, 0, 0);
        Assert.Equal(2, countRange);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_NonExistentKey_ReturnsZero(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        long count = await client.StringBitCountAsync(key);
        Assert.Equal(0, count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_WithBitIndex_CountsCorrectly(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Set multiple bits
        await client.StringSetBitAsync(key, 0, true);  // bit 0
        await client.StringSetBitAsync(key, 1, true);  // bit 1
        await client.StringSetBitAsync(key, 8, true);  // bit 8 (second byte)
        
        // Count all bits
        long totalCount = await client.StringBitCountAsync(key);
        Assert.Equal(3, totalCount);
        
        // Count bits 0-7 (first byte) using bit indexing
        long firstByteCount = await client.StringBitCountAsync(key, 0, 7, StringIndexType.Bit);
        Assert.Equal(2, firstByteCount);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPosition_FindsFirstSetBit(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Set string to "A" (ASCII 65 = 01000001 in binary)
        await client.StringSetAsync(key, "A");
        
        // Find first set bit (should be at position 1)
        long pos1 = await client.StringBitPositionAsync(key, true);
        Assert.Equal(1, pos1);
        
        // Find first unset bit (should be at position 0)
        long pos0 = await client.StringBitPositionAsync(key, false);
        Assert.Equal(0, pos0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPosition_NonExistentKey_ReturnsMinusOne(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Search for set bit in non-existent key
        long pos = await client.StringBitPositionAsync(key, true);
        Assert.Equal(-1, pos);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPosition_WithRange_FindsInRange(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        
        // Set multiple bits: bit 1 and bit 9
        await client.StringSetBitAsync(key, 1, true);
        await client.StringSetBitAsync(key, 9, true);
        
        // Find first set bit in entire string
        long pos1 = await client.StringBitPositionAsync(key, true);
        Assert.Equal(1, pos1);
        
        // Find first set bit starting from bit 8 using bit indexing
        long pos2 = await client.StringBitPositionAsync(key, true, 8, -1, StringIndexType.Bit);
        Assert.Equal(9, pos2);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitOperation_And_PerformsCorrectOperation(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string key1 = keyPrefix + ":key1";
        string key2 = keyPrefix + ":key2";
        string result = keyPrefix + ":result";
        
        // Set key1 to "A" (01000001) and key2 to "B" (01000010)
        await client.StringSetAsync(key1, "A");
        await client.StringSetAsync(key2, "B");
        
        // Perform AND operation
        long size = await client.StringBitOperationAsync(Bitwise.And, result, key1, key2);
        Assert.Equal(1, size);
        
        // Verify result: A AND B = 01000001 AND 01000010 = 01000000 = '@'
        ValkeyValue resultValue = await client.StringGetAsync(result);
        Assert.Equal("@", resultValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitOperation_Or_PerformsCorrectOperation(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string key1 = keyPrefix + ":key1";
        string key2 = keyPrefix + ":key2";
        string result = keyPrefix + ":result";
        
        // Set key1 to "A" (01000001) and key2 to "B" (01000010)
        await client.StringSetAsync(key1, "A");
        await client.StringSetAsync(key2, "B");
        
        // Perform OR operation
        long size = await client.StringBitOperationAsync(Bitwise.Or, result, key1, key2);
        Assert.Equal(1, size);
        
        // Verify result: A OR B = 01000001 OR 01000010 = 01000011 = 'C'
        ValkeyValue resultValue = await client.StringGetAsync(result);
        Assert.Equal("C", resultValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitOperation_MultipleKeys_PerformsCorrectOperation(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string key1 = keyPrefix + ":key1";
        string key2 = keyPrefix + ":key2";
        string key3 = keyPrefix + ":key3";
        string result = keyPrefix + ":result";
        
        // Set keys with different bit patterns
        await client.StringSetAsync(key1, "A"); // 01000001
        await client.StringSetAsync(key2, "B"); // 01000010
        await client.StringSetAsync(key3, "D"); // 01000100
        
        // Perform OR operation on multiple keys
        ValkeyKey[] keys = [key1, key2, key3];
        long size = await client.StringBitOperationAsync(Bitwise.Or, result, keys);
        Assert.Equal(1, size);
        
        // Verify result: A OR B OR D = 01000001 OR 01000010 OR 01000100 = 01000111 = 'G'
        ValkeyValue resultValue = await client.StringGetAsync(result);
        Assert.Equal("G", resultValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitOperation_Xor_PerformsCorrectOperation(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string key1 = keyPrefix + ":key1";
        string key2 = keyPrefix + ":key2";
        string result = keyPrefix + ":result";
        
        // Set key1 to "A" (01000001) and key2 to "B" (01000010)
        await client.StringSetAsync(key1, "A");
        await client.StringSetAsync(key2, "B");
        
        // Perform XOR operation
        long size = await client.StringBitOperationAsync(Bitwise.Xor, result, key1, key2);
        Assert.Equal(1, size);
        
        // Verify result: A XOR B = 01000001 XOR 01000010 = 00000011 = ASCII 3
        ValkeyValue resultValue = await client.StringGetAsync(result);
        byte[] resultBytes = resultValue;
        Assert.Equal(3, resultBytes[0]); // XOR of 65 (A) and 66 (B) is 3
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitOperation_Not_PerformsCorrectOperation(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string key1 = keyPrefix + ":key1";
        string result = keyPrefix + ":result";
        
        // Set key1 to "A" (01000001)
        await client.StringSetAsync(key1, "A");
        
        // Perform NOT operation (NOT only takes one key)
        ValkeyKey[] keys = [key1];
        long size = await client.StringBitOperationAsync(Bitwise.Not, result, keys);
        Assert.Equal(1, size);
        
        // Verify result: NOT A = NOT 01000001 = 10111110 = '¾' (ASCII 190)
        ValkeyValue resultValue = await client.StringGetAsync(result);
        byte[] resultBytes = resultValue;
        Assert.Equal(190, resultBytes[0]); // NOT of 65 (A) is 190
    }
}