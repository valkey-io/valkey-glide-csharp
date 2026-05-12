// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

public class AddressResolverTests
{
    [Fact]
    public void StandaloneBuilder_WithAddressResolver_SetsResolver()
    {
        static (string, int) Resolver(string host, int port) => ("resolved-host", 9999);
        var config = new StandaloneClientConfigurationBuilder()
            .WithAddressResolver(Resolver)
            .Build();
        Assert.NotNull(config.Request.AddressResolver);
    }

    [Fact]
    public void ClusterBuilder_WithAddressResolver_SetsResolver()
    {
        static (string, int) Resolver(string host, int port) => (host, port);
        var config = new ClusterClientConfigurationBuilder()
            .WithAddressResolver(Resolver)
            .Build();
        Assert.NotNull(config.Request.AddressResolver);
    }

    [Fact]
    public void AddressResolver_DefaultIsNull()
    {
        var config = new StandaloneClientConfigurationBuilder().Build();
        Assert.Null(config.Request.AddressResolver);
    }

    [Fact]
    public void AddressResolver_InvokesCallback()
    {
        static (string, int) Resolver(string host, int port) => ("proxy.example.com", 7000);
        var config = new StandaloneClientConfigurationBuilder()
            .WithAddressResolver(Resolver)
            .Build();
        var (resolvedHost, resolvedPort) = config.Request.AddressResolver!("localhost", 6379);
        Assert.Equal("proxy.example.com", resolvedHost);
        Assert.Equal(7000, resolvedPort);
    }

    [Fact]
    public void AddressResolver_NullIsValid()
    {
        var builder = new StandaloneClientConfigurationBuilder();
        builder.AddressResolver = null;
        var config = builder.Build();
        Assert.Null(config.Request.AddressResolver);
    }
}
