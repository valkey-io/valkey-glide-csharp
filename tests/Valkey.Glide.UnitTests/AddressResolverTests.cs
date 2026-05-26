// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class AddressResolverTests
{
    #region Constants

    private static readonly (string, ushort) Resolved = ("resolved-host", 9999);
    private static readonly AddressResolverDelegate Resolver = (host, port) => Resolved;

    #endregion

    #region Tests

    [Fact]
    public void WithAddressResolver_Standalone_SetsResolver()
    {
        var config = new StandaloneClientConfigurationBuilder().WithAddressResolver(Resolver).Build();
        Assert.Equal(Resolver, config.Request.AddressResolver);
        Assert.Equal(Resolved, config.Request.AddressResolver!("localhost", 6379));

    }

    [Fact]
    public void WithAddressResolver_Cluster_SetsResolver()
    {
        var config = new ClusterClientConfigurationBuilder().WithAddressResolver(Resolver).Build();
        Assert.Equal(Resolver, config.Request.AddressResolver);
        Assert.Equal(Resolved, config.Request.AddressResolver!("localhost", 6379));

    }

    [Fact]
    public void AddressResolver_Standalone_DefaultIsNull()
        => Assert.Null(new StandaloneClientConfigurationBuilder().Build().Request.AddressResolver);

    [Fact]
    public void AddressResolver_Cluster_DefaultIsNull()
        => Assert.Null(new ClusterClientConfigurationBuilder().Build().Request.AddressResolver);

    [Fact]
    public void AddressResolver_Standalone_SetToNull()
        => Assert.Null(new StandaloneClientConfigurationBuilder { AddressResolver = null }.Build().Request.AddressResolver);

    [Fact]
    public void AddressResolver_Cluster_SetToNull()
        => Assert.Null(new ClusterClientConfigurationBuilder { AddressResolver = null }.Build().Request.AddressResolver);

    #endregion
}
