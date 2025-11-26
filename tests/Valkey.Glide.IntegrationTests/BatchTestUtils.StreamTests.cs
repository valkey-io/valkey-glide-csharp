// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

internal partial class BatchTestUtils
{
    public static List<TestInfo> CreateStreamTest(Pipeline.IBatch batch, bool isAtomic)
    {
        List<TestInfo> testData = [];
        string prefix = "{streamKey}-";
        string atomicPrefix = isAtomic ? prefix : "";
        string key1 = $"{atomicPrefix}1-{Guid.NewGuid()}";
        string key2 = $"{atomicPrefix}2-{Guid.NewGuid()}";
        string groupName = "mygroup";
        string consumer1 = "consumer1";
        string consumer2 = "consumer2";

        // Test StreamAdd
        _ = batch.StreamAdd(key1, "field1", "value1");
        testData.Add(new(new ValkeyValue(""), "StreamAdd(key1, field1, value1)", true));

        _ = batch.StreamAdd(key1, [new NameValueEntry("field2", "value2"), new NameValueEntry("field3", "value3")]);
        testData.Add(new(new ValkeyValue(""), "StreamAdd(key1, multiple fields)", true));

        // Test StreamLength
        _ = batch.StreamLength(key1);
        testData.Add(new(2L, "StreamLength(key1)"));

        // Test StreamRange
        _ = batch.StreamRange(key1);
        testData.Add(new(Array.Empty<StreamEntry>(), "StreamRange(key1)", true));

        // Test StreamRead
        _ = batch.StreamRead(key1, "0-0", count: 10);
        testData.Add(new(Array.Empty<StreamEntry>(), "StreamRead(key1, 0-0, count: 10)", true));

        // Test StreamTrim
        _ = batch.StreamTrim(key1, maxLength: 1);
        testData.Add(new(1L, "StreamTrim(key1, maxLength: 1)"));

        // Test StreamCreateConsumerGroup
        _ = batch.StreamCreateConsumerGroup(key1, groupName, "0");
        testData.Add(new(true, "StreamCreateConsumerGroup(key1, mygroup, 0)"));

        // Add more entries for consumer group tests
        _ = batch.StreamAdd(key1, "field4", "value4");
        testData.Add(new(new ValkeyValue(""), "StreamAdd(key1, field4, value4)", true));

        _ = batch.StreamAdd(key1, "field5", "value5");
        testData.Add(new(new ValkeyValue(""), "StreamAdd(key1, field5, value5)", true));

        // Test StreamReadGroup
        _ = batch.StreamReadGroup(key1, groupName, consumer1, ">", count: 2);
        testData.Add(new(Array.Empty<StreamEntry>(), "StreamReadGroup(key1, mygroup, consumer1, >, count: 2)", true));

        // Test StreamAcknowledge - need to get IDs from previous read, so we'll use dummy IDs
        _ = batch.StreamAcknowledge(key1, groupName, ["0-0"]);
        testData.Add(new(0L, "StreamAcknowledge(key1, mygroup, [0-0])", true));

        // Test StreamClaim
        _ = batch.StreamClaim(key1, groupName, consumer2, 0, ["0-0"]);
        testData.Add(new(Array.Empty<StreamEntry>(), "StreamClaim(key1, mygroup, consumer2, 0, [0-0])", true));

        // Test StreamGroupInfo
        _ = batch.StreamGroupInfo(key1);
        testData.Add(new(Array.Empty<StreamGroupInfo>(), "StreamGroupInfo(key1)", true));

        // Test StreamConsumerInfo
        _ = batch.StreamConsumerInfo(key1, groupName);
        testData.Add(new(Array.Empty<StreamConsumerInfo>(), "StreamConsumerInfo(key1, mygroup)", true));

        // Test StreamDelete
        _ = batch.StreamDelete(key1, ["0-0"]);
        testData.Add(new(0L, "StreamDelete(key1, [0-0])", true));

        // Test StreamDeleteConsumer
        _ = batch.StreamDeleteConsumer(key1, groupName, consumer1);
        testData.Add(new(0L, "StreamDeleteConsumer(key1, mygroup, consumer1)", true));

        // Test StreamDeleteConsumerGroup
        _ = batch.StreamDeleteConsumerGroup(key1, groupName);
        testData.Add(new(true, "StreamDeleteConsumerGroup(key1, mygroup)"));

        return testData;
    }
}
