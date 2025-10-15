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
}