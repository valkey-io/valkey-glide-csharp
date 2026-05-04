// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Errors;

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
        await client.SetAsync(key, "A");

        // Test bit positions in 'A' (01000001)
        Assert.False(await client.GetBitAsync(key, 0));
        Assert.True(await client.GetBitAsync(key, 1));
        Assert.False(await client.GetBitAsync(key, 2));
        Assert.False(await client.GetBitAsync(key, 6));
        Assert.True(await client.GetBitAsync(key, 7));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetBit_NonExistentKey_ReturnsFalse(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test bit on non-existent key
        bool bit = await client.GetBitAsync(key, 0);

        Assert.False(bit);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetBit_OffsetBeyondString_ReturnsFalse(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set a short string
        await client.SetAsync(key, "A");

        // Test bit beyond the string length
        bool bit = await client.GetBitAsync(key, 100);

        Assert.False(bit);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetBit_NegativeOffset_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set a string
        await client.SetAsync(key, "A");

        // Test negative offset - should throw an exception
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.GetBitAsync(key, -1));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetBit_SetsAndReturnsOriginalValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set bit 1 to true (original should be false)
        bool originalBit = await client.SetBitAsync(key, 1, true);
        Assert.False(originalBit);

        // Verify bit is now set
        bool currentBit = await client.GetBitAsync(key, 1);
        Assert.True(currentBit);

        // Set bit 1 to false (original should be true)
        bool originalBit2 = await client.SetBitAsync(key, 1, false);
        Assert.True(originalBit2);

        // Verify bit is now cleared
        bool currentBit2 = await client.GetBitAsync(key, 1);
        Assert.False(currentBit2);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetBit_NonExistentKey_CreatesKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set bit on non-existent key
        bool originalBit = await client.SetBitAsync(key, 0, true);
        Assert.False(originalBit);

        // Verify key was created and bit is set
        bool currentBit = await client.GetBitAsync(key, 0);
        Assert.True(currentBit);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetBit_NegativeOffset_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test negative offset - should throw an exception
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.SetBitAsync(key, -1, true));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetSetBit_CombinedOperations_WorksTogether(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Initially all bits should be 0
        bool bit0 = await client.GetBitAsync(key, 0);
        bool bit1 = await client.GetBitAsync(key, 1);
        Assert.False(bit0);
        Assert.False(bit1);

        // Set bit 0 to 1
        bool originalBit0 = await client.SetBitAsync(key, 0, true);
        Assert.False(originalBit0); // Was 0

        // Set bit 1 to 1
        bool originalBit1 = await client.SetBitAsync(key, 1, true);
        Assert.False(originalBit1); // Was 0

        // Verify both bits are now set
        bool newBit0 = await client.GetBitAsync(key, 0);
        bool newBit1 = await client.GetBitAsync(key, 1);
        Assert.True(newBit0);
        Assert.True(newBit1);

        // Clear bit 0
        bool clearBit0 = await client.SetBitAsync(key, 0, false);
        Assert.True(clearBit0); // Was 1

        // Verify bit 0 is cleared but bit 1 remains set
        bool finalBit0 = await client.GetBitAsync(key, 0);
        bool finalBit1 = await client.GetBitAsync(key, 1);
        Assert.False(finalBit0);
        Assert.True(finalBit1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_CountsSetBits(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set string to "A" (ASCII 65 = 01000001 in binary = 2 bits set)
        await client.SetAsync(key, "A");

        // Count all bits
        long count = await client.BitCountAsync(key);
        Assert.Equal(2, count);

        // Count bits in byte range
        long countRange = await client.BitCountAsync(key, 0, 0);
        Assert.Equal(2, countRange);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_NonExistentKey_ReturnsZero(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        long count = await client.BitCountAsync(key);
        Assert.Equal(0, count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_WithBitIndex_CountsCorrectly(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("7.0.0"),
            "BIT index type for BITCOUNT requires server version 7.0 or higher"
        );

        string key = Guid.NewGuid().ToString();

        // Set multiple bits
        _ = await client.SetBitAsync(key, 0, true);  // bit 0
        _ = await client.SetBitAsync(key, 1, true);  // bit 1
        _ = await client.SetBitAsync(key, 8, true);  // bit 8 (second byte)

        // Count all bits
        long totalCount = await client.BitCountAsync(key);
        Assert.Equal(3, totalCount);

        // Count bits 0-7 (first byte) using bit indexing
        long firstByteCount = await client.BitCountAsync(key, 0, 7, BitmapIndexType.Bit);
        Assert.Equal(2, firstByteCount);
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPosition_FindsFirstSetBit(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set string to "A" (ASCII 65 = 01000001 in binary)
        await client.SetAsync(key, "A");

        // Find first set bit (should be at position 1)
        long pos1 = await client.BitPosAsync(key, true);
        Assert.Equal(1, pos1);

        // Find first unset bit (should be at position 0)
        long pos0 = await client.BitPosAsync(key, false);
        Assert.Equal(0, pos0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPosition_NonExistentKey_ReturnsMinusOne(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Search for set bit in non-existent key
        long pos = await client.BitPosAsync(key, true);
        Assert.Equal(-1, pos);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPosition_WithRange_FindsInRange(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("7.0.0"),
            "BIT index type for BITPOS requires server version 7.0 or higher"
        );

        string key = Guid.NewGuid().ToString();

        // Set multiple bits: bit 1 and bit 9
        _ = await client.SetBitAsync(key, 1, true);
        _ = await client.SetBitAsync(key, 9, true);

        // Find first set bit in entire string
        long pos1 = await client.BitPosAsync(key, true);
        Assert.Equal(1, pos1);

        // Find first set bit starting from bit 8 using bit indexing
        long pos2 = await client.BitPosAsync(key, true, 8, -1, BitmapIndexType.Bit);
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
        await client.SetAsync(key1, "A");
        await client.SetAsync(key2, "B");

        // Perform AND operation
        long size = await client.BitOpAsync(Bitwise.And, result, [key1, key2]);
        Assert.Equal(1, size);

        // Verify result: A AND B = 01000001 AND 01000010 = 01000000 = '@'
        ValkeyValue resultValue = await client.GetAsync(result);
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
        await client.SetAsync(key1, "A");
        await client.SetAsync(key2, "B");

        // Perform OR operation
        long size = await client.BitOpAsync(Bitwise.Or, result, [key1, key2]);
        Assert.Equal(1, size);

        // Verify result: A OR B = 01000001 OR 01000010 = 01000011 = 'C'
        ValkeyValue resultValue = await client.GetAsync(result);
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
        await client.SetAsync(key1, "A"); // 01000001
        await client.SetAsync(key2, "B"); // 01000010
        await client.SetAsync(key3, "D"); // 01000100

        // Perform OR operation on multiple keys
        ValkeyKey[] keys = [key1, key2, key3];
        long size = await client.BitOpAsync(Bitwise.Or, result, keys);
        Assert.Equal(1, size);

        // Verify result: A OR B OR D = 01000001 OR 01000010 OR 01000100 = 01000111 = 'G'
        ValkeyValue resultValue = await client.GetAsync(result);
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
        await client.SetAsync(key1, "A");
        await client.SetAsync(key2, "B");

        // Perform XOR operation
        long size = await client.BitOpAsync(Bitwise.Xor, result, [key1, key2]);
        Assert.Equal(1, size);

        // Verify result: A XOR B = 01000001 XOR 01000010 = 00000011 = ASCII 3
        ValkeyValue resultValue = await client.GetAsync(result);
        byte[] resultBytes = resultValue!;
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
        await client.SetAsync(key1, "A");

        // Perform NOT operation (NOT only takes one key)
        ValkeyKey[] keys = [key1];
        long size = await client.BitOpAsync(Bitwise.Not, result, keys);
        Assert.Equal(1, size);

        // Verify result: NOT A = NOT 01000001 = 10111110 = '¾' (ASCII 190)
        ValkeyValue resultValue = await client.GetAsync(result);
        byte[] resultBytes = resultValue!;
        Assert.Equal(190, resultBytes[0]); // NOT of 65 (A) is 190
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitField_GetSetIncrBy_WorksCorrectly(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value "A" (ASCII 65 = 01000001)
        await client.SetAsync(key, "A");

        var subCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            // Get 8 unsigned bits at offset 0
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)),
            // Set 8 unsigned bits at offset 0 to 66 (ASCII 'B')
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 66),
            // Increment by 1
            new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 1)
        };

        long?[] results = await client.BitFieldAsync(key, subCommands);

        Assert.Equal(3, results.Length);
        Assert.Equal(65, results[0]); // Original value 'A'
        Assert.Equal(65, results[1]); // Old value before SET
        Assert.Equal(67, results[2]); // New value after INCRBY (66 + 1 = 67 = 'C')

        // Verify final value
        ValkeyValue finalValue = await client.GetAsync(key);
        Assert.Equal("C", finalValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitFieldReadOnly_Get_WorksCorrectly(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value "A" (ASCII 65 = 01000001)
        await client.SetAsync(key, "A");

        var readOnlyCommands = new BitFieldOptions.IBitFieldReadOnlySubCommand[]
        {
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)),
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(4), new BitFieldOptions.BitOffset(0)),
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(4), new BitFieldOptions.BitOffset(4))
        };

        long[] results = await client.BitFieldReadOnlyAsync(key, readOnlyCommands);

        Assert.Equal(3, results.Length);
        Assert.Equal(65, results[0]);  // Full 8 bits: 01000001 = 65
        Assert.Equal(4, results[1]);   // First 4 bits: 0100 = 4
        Assert.Equal(1, results[2]);   // Next 4 bits: 0001 = 1
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitField_OverflowControl_WorksCorrectly(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        var subCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            // Set overflow to WRAP first
            new BitFieldOptions.BitFieldOverflow(BitFieldOptions.OverflowType.Wrap),
            // Set u8 at offset 0 to 255 (max value)
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 255),
            // Increment u8 at offset 0 by 1 (255 + 1 = 0 with wrap)
            new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 1)
        };

        long?[] results = await client.BitFieldAsync(key, subCommands);

        Assert.Equal(2, results.Length);
        Assert.Equal(0, results[0]);   // SET returns old value (0)
        Assert.Equal(0, results[1]);   // 255 + 1 = 0 (wrapped)
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitField_OverflowSat_WorksCorrectly(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        var subCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldOverflow(BitFieldOptions.OverflowType.Sat),
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 250),
            new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 10)
        };

        long?[] results = await client.BitFieldAsync(key, subCommands);

        Assert.Equal(2, results.Length);
        Assert.Equal(0, results[0]);   // SET returns old value (0)
        Assert.Equal(255, results[1]); // 250 + 10 = 255 (saturated at max)
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitField_OverflowFail_WorksCorrectly(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        var subCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldOverflow(BitFieldOptions.OverflowType.Fail),
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 255),
            new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 1)
        };

        long?[] results = await client.BitFieldAsync(key, subCommands);

        Assert.Equal(2, results.Length);
        Assert.Equal(0, results[0]);   // SET returns old value (0)
        Assert.Null(results[1]);       // 255 + 1 = null (fail) - null is preserved, not converted to 0
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitField_OffsetMultiplier_WorksCorrectly(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        var subCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            // Set first i8 at offset #0 (0 * 8 = bit 0)
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Signed(8), new BitFieldOptions.BitOffsetMultiplier(0), 100),
            // Set second i8 at offset #1 (1 * 8 = bit 8)
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Signed(8), new BitFieldOptions.BitOffsetMultiplier(1), -50),
            // Get both values back
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Signed(8), new BitFieldOptions.BitOffsetMultiplier(0)),
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Signed(8), new BitFieldOptions.BitOffsetMultiplier(1))
        };

        long?[] results = await client.BitFieldAsync(key, subCommands);

        Assert.Equal(4, results.Length);
        Assert.Equal(0, results[0]);   // SET returns old value (0)
        Assert.Equal(0, results[1]);   // SET returns old value (0)
        Assert.Equal(100, results[2]); // First i8 value
        Assert.Equal(-50, results[3]); // Second i8 value
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitField_WithReadOnlyAndMixedOperations_WorksCorrectly(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value "A" (ASCII 65 = 01000001)
        await client.SetAsync(key, "A");

        // Test with only GET operations
        var readOnlySubCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)),
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(4), new BitFieldOptions.BitOffset(0)),
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(4), new BitFieldOptions.BitOffset(4))
        };

        long?[] results = await client.BitFieldAsync(key, readOnlySubCommands);

        Assert.Equal(3, results.Length);
        Assert.Equal(65, results[0]);  // Full 8 bits: 01000001 = 65
        Assert.Equal(4, results[1]);   // First 4 bits: 0100 = 4
        Assert.Equal(1, results[2]);   // Next 4 bits: 0001 = 1

        // Test with mixed operations
        var mixedSubCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)),
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 100)
        };

        long?[] mixedResults = await client.BitFieldAsync(key, mixedSubCommands);

        Assert.Equal(2, mixedResults.Length);
        Assert.Equal(65, mixedResults[0]); // Original value 'A'
        Assert.Equal(65, mixedResults[1]); // Old value before SET
    }

    #region BitOffsetOptions Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_WithBitOffsetOptions_DefaultValues(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set string to "AB" (ASCII 65 = 01000001, 66 = 01000010 = 4 bits set total)
        await client.SetAsync(key, "AB");

        // Use BitOffsetOptions with default values (Start=0, End=-1, IndexType=Byte)
        long count = await client.BitCountAsync(key, new BitOffsetOptions());
        Assert.Equal(4, count);

        // Null options should also use defaults
        long countNull = await client.BitCountAsync(key, null);
        Assert.Equal(4, countNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_WithBitOffsetOptions_ByteRange(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set string to "AB" (ASCII 65 = 01000001, 66 = 01000010)
        await client.SetAsync(key, "AB");

        // Count bits in first byte only using BitOffsetOptions
        var options = new BitOffsetOptions { Start = 0, End = 0, IndexType = BitmapIndexType.Byte };
        long count = await client.BitCountAsync(key, options);
        Assert.Equal(2, count); // 'A' has 2 bits set
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_WithBitOffsetOptions_BitRange(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("7.0.0"),
            "BIT index type for BITCOUNT requires server version 7.0 or higher"
        );

        string key = Guid.NewGuid().ToString();

        // Set multiple bits
        _ = await client.SetBitAsync(key, 0, true);  // bit 0
        _ = await client.SetBitAsync(key, 1, true);  // bit 1
        _ = await client.SetBitAsync(key, 8, true);  // bit 8 (second byte)

        // Count bits 0-7 (first byte) using BitOffsetOptions with bit indexing
        var options = new BitOffsetOptions { Start = 0, End = 7, IndexType = BitmapIndexType.Bit };
        long count = await client.BitCountAsync(key, options);
        Assert.Equal(2, count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitCount_WithBitOffsetOptions_NegativeEnd(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set string to "ABC" (3 bytes)
        await client.SetAsync(key, "ABC");

        // Count bits from start to second-to-last byte using negative end
        var options = new BitOffsetOptions { Start = 0, End = -2, IndexType = BitmapIndexType.Byte };
        long count = await client.BitCountAsync(key, options);

        // 'A' (01000001) + 'B' (01000010) = 2 + 2 = 4 bits
        Assert.Equal(4, count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPos_WithBitOffsetOptions_DefaultValues(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set string to "A" (ASCII 65 = 01000001)
        await client.SetAsync(key, "A");

        // Find first set bit using BitOffsetOptions with default values
        long pos = await client.BitPosAsync(key, true, new BitOffsetOptions());
        Assert.Equal(1, pos); // First set bit at position 1

        // Null options should also use defaults
        long posNull = await client.BitPosAsync(key, true, null);
        Assert.Equal(1, posNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPos_WithBitOffsetOptions_ByteRange(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set a two-byte string where first byte has no set bits and second byte has set bits
        // Using "AB" - 'A' (01000001) has bit 1 set, 'B' (01000010) has bit 1 set
        await client.SetAsync(key, "AB");

        // Find first set bit starting from second byte (byte index 1)
        // 'B' = 01000010, first set bit is at position 1 within the byte = bit 9 overall
        var options = new BitOffsetOptions { Start = 1, End = 1, IndexType = BitmapIndexType.Byte };
        long pos = await client.BitPosAsync(key, true, options);
        Assert.Equal(9, pos); // First set bit in 'B' at position 9 (bit 1 of second byte)
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPos_WithBitOffsetOptions_BitRange(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("7.0.0"),
            "BIT index type for BITPOS requires server version 7.0 or higher"
        );

        string key = Guid.NewGuid().ToString();

        // Set multiple bits: bit 1 and bit 9
        _ = await client.SetBitAsync(key, 1, true);
        _ = await client.SetBitAsync(key, 9, true);

        // Find first set bit starting from bit 8 using BitOffsetOptions
        var options = new BitOffsetOptions { Start = 8, End = -1, IndexType = BitmapIndexType.Bit };
        long pos = await client.BitPosAsync(key, true, options);
        Assert.Equal(9, pos);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPos_WithBitOffsetOptions_FindUnsetBit(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set a string where first byte is all 1s (0xFF = 255) and second byte has zeros
        // Using byte array to ensure exact binary content
        byte[] data = [0xFF, 0x00]; // First byte all 1s, second byte all 0s
        await client.SetAsync(key, data);

        // Find first unset bit using BitOffsetOptions
        var options = new BitOffsetOptions { Start = 0, End = -1, IndexType = BitmapIndexType.Byte };
        long pos = await client.BitPosAsync(key, false, options);
        Assert.Equal(8, pos); // First unset bit at position 8 (first bit of second byte)
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitPos_WithBitOffsetOptions_NotFound(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set string to all zeros
        await client.SetAsync(key, "\x00\x00");

        // Search for set bit - should return -1
        var options = new BitOffsetOptions { Start = 0, End = -1, IndexType = BitmapIndexType.Byte };
        long pos = await client.BitPosAsync(key, true, options);
        Assert.Equal(-1, pos);
    }

    #endregion
    #region Additional Edge Case Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public Task BitOp_EmptyKeys_ThrowsArgumentException(BaseClient client)
        => _ = Assert.ThrowsAsync<RequestException>(() =>
            client.BitOpAsync(Bitwise.And, Guid.NewGuid().ToString(), []));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitOp_Not_WithMultipleKeys_ThrowsServerError(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string key1 = keyPrefix + ":key1";
        string key2 = keyPrefix + ":key2";
        string result = keyPrefix + ":result";

        await client.SetAsync(key1, "A");
        await client.SetAsync(key2, "B");

        // NOT with multiple keys should throw server error
        _ = await Assert.ThrowsAsync<RequestException>(async () =>
            await client.BitOpAsync(Bitwise.Not, result, [key1, key2]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitField_OverflowFail_PreservesNullNotZero(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        var subCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldOverflow(BitFieldOptions.OverflowType.Fail),
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 255),
            new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 1)
        };

        long?[] results = await client.BitFieldAsync(key, subCommands);

        Assert.Equal(2, results.Length);
        Assert.Equal(0, results[0]);

        // Must be null, not 0 - null indicates overflow failure
        Assert.Null(results[1]);

        // Verify the value wasn't changed (still 255)
        var getCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0))
        };
        long?[] getResults = await client.BitFieldAsync(key, getCommands);
        Assert.Equal(255, getResults[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task BitField_MultipleOverflowFails_AllPreserveNull(BaseClient client)
    {
        // Test multiple overflow failures in a single BITFIELD command
        string key = Guid.NewGuid().ToString();

        var subCommands = new BitFieldOptions.IBitFieldSubCommand[]
        {
            new BitFieldOptions.BitFieldOverflow(BitFieldOptions.OverflowType.Fail),
            // Set two u8 fields to max value
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 255),
            new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(8), 255),
            // Try to increment both - both will overflow
            new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0), 1),
            new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(8), 1)
        };

        long?[] results = await client.BitFieldAsync(key, subCommands);

        Assert.Equal(4, results.Length);
        Assert.Equal(0, results[0]);   // First SET returns old value
        Assert.Equal(0, results[1]);   // Second SET returns old value
        Assert.Null(results[2]);       // First INCRBY overflow - must be null
        Assert.Null(results[3]);       // Second INCRBY overflow - must be null
    }

    #endregion
}
