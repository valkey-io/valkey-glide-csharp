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
}