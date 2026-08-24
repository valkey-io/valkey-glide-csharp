// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.IntegrationTests;

internal partial class BatchTestUtils
{
    public static List<TestInfo> CreateStreamTest(Pipeline.IBatch batch, bool isAtomic)
    {
        string prefix = "{streamKey}-";
        string atomicPrefix = isAtomic ? prefix : "";
        string key1 = $"{atomicPrefix}1-{Guid.NewGuid()}";

        string groupName = "mygroup";
        string consumer1 = "consumer1";
        string consumer2 = "consumer2";

        List<TestInfo> testData = [];

        // Test StreamAdd
        _ = batch.StreamAdd(key1, "field1", "value1");
        testData.Add(new(new ValkeyValue(""), "StreamAdd(key1, field1, value1)", true));

        _ = batch.StreamAdd(key1, [new NameValueEntry("field2", "value2"), new NameValueEntry("field3", "value3")]);
        testData.Add(new(new ValkeyValue(""), "StreamAdd(key1, multiple fields)", true));

        // Test StreamLength
        _ = batch.StreamLength(key1);
        testData.Add(new(2L, "StreamLength(key1)"));

        // Test StreamRead
        _ = batch.StreamRead(new StreamPosition(key1, StreamPosition.Beginning));
        testData.Add(new(Array.Empty<StreamEntry>(), "StreamRead(key1, 0-0)", true));

        // Test StreamTrim
        _ = batch.StreamTrim(key1, new StreamTrimOptions.MaxLen { MaxLength = 1 });
        testData.Add(new(1L, "StreamTrim(key1, maxLength: 1)"));

        // Test StreamCreateConsumerGroup
        _ = batch.StreamGroupCreate(key1, groupName, "0");
        testData.Add(new(ValkeyValue.Ok, "StreamGroupCreate(key1, mygroup, 0)"));

        // Add more entries for consumer group tests
        _ = batch.StreamAdd(key1, "field4", "value4");
        testData.Add(new(new ValkeyValue(""), "StreamAdd(key1, field4, value4)", true));

        _ = batch.StreamAdd(key1, "field5", "value5");
        testData.Add(new(new ValkeyValue(""), "StreamAdd(key1, field5, value5)", true));

        // Test StreamReadGroup
        _ = batch.StreamReadGroup(new StreamPosition(key1, StreamPosition.UndeliveredMessages), groupName, consumer1);
        testData.Add(new(Array.Empty<StreamEntry>(), "StreamReadGroup(key1, mygroup, consumer1, >)", true));

        // Test StreamPending
        _ = batch.StreamPending(key1, groupName);
        testData.Add(new(default(StreamPendingInfo), "StreamPending(key1, mygroup)", true));

        // Test StreamAcknowledge
        _ = batch.StreamAcknowledge(key1, groupName, ["0-0"]);
        testData.Add(new(0L, "StreamAcknowledge(key1, mygroup, [0-0])", true));

        // Test StreamClaim
        _ = batch.StreamClaim(key1, groupName, consumer2, ["0-0"], StreamClaimOptions.From(TimeSpan.Zero));
        testData.Add(new(Array.Empty<StreamEntry>(), "StreamClaim(key1, mygroup, consumer2, 0, [0-0])", true));

        // Test StreamGroupInfo
        _ = batch.StreamInfoGroups(key1);
        testData.Add(new(Array.Empty<StreamGroupInfo>(), "StreamInfoGroups(key1)", true));

        // Test StreamConsumerInfo
        _ = batch.StreamInfoConsumers(key1, groupName);
        testData.Add(new(Array.Empty<StreamConsumerInfo>(), "StreamInfoConsumers(key1, mygroup)", true));

        // Test StreamInfo
        _ = batch.StreamInfo(key1);
        testData.Add(new(default(StreamInfo), "StreamInfo(key1)", true));

        // Test StreamInfoFull
        _ = batch.StreamInfoFull(key1);
        testData.Add(new(default(StreamInfoFull), "StreamInfoFull(key1)", true));

        // Test StreamDelete (multi-ID)
        _ = batch.StreamDelete(key1, ["0-0"]);
        testData.Add(new(0L, "StreamDelete(key1, [0-0])", true));

        // Test StreamDelete (single-ID)
        _ = batch.StreamDelete(key1, "0-0");
        testData.Add(new(false, "StreamDelete(key1, 0-0)", true));

        // Test StreamDeleteConsumer
        _ = batch.StreamGroupDeleteConsumer(key1, groupName, consumer1);
        testData.Add(new(default(long), "StreamGroupDeleteConsumer(key1, mygroup, consumer1)", true));

        // Test StreamDeleteConsumerGroup
        _ = batch.StreamGroupDestroy(key1, groupName);
        testData.Add(new(true, "StreamGroupDestroy(key1, mygroup)"));

        return testData;
    }
}
