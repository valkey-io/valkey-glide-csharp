// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class SimpleConfigTest
{
    [Fact]
    public void CanCreateStandaloneConfig()
    {
        var config = new StandalonePubSubSubscriptionConfig();
        Assert.NotNull(config);
    }
}
