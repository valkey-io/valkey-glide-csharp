// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ClusterScriptOptionsTests
{
    [Fact]
    public void Constructor_CreatesInstanceWithNullProperties()
    {
        // Act
        var options = new ClusterScriptOptions();

        // Assert
        Assert.Null(options.Args);
        Assert.Null(options.Route);
    }

    [Fact]
    public void WithArgs_SetsArgsProperty()
    {
        // Arrange
        var options = new ClusterScriptOptions();
        string[] args = ["arg1", "arg2", "arg3"];

        // Act
        var result = options.WithArgs(args);

        // Assert
        Assert.Same(options, result); // Fluent interface returns same instance
        Assert.Equal(args, options.Args);
    }

    [Fact]
    public void WithArgs_WithParamsArray_SetsArgsProperty()
    {
        // Arrange
        var options = new ClusterScriptOptions();

        // Act
        var result = options.WithArgs("arg1", "arg2", "arg3");

        // Assert
        Assert.Same(options, result);
        Assert.NotNull(options.Args);
        Assert.Equal(["arg1", "arg2", "arg3"], options.Args);
    }

    [Fact]
    public void WithRoute_SetsRouteProperty()
    {
        // Arrange
        var options = new ClusterScriptOptions();
        var route = Route.AllPrimaries;

        // Act
        var result = options.WithRoute(route);

        // Assert
        Assert.Same(options, result); // Fluent interface returns same instance
        Assert.Same(route, options.Route);
    }

    [Fact]
    public void WithRoute_WithRandomRoute_SetsRouteProperty()
    {
        // Arrange
        var options = new ClusterScriptOptions();
        var route = Route.Random;

        // Act
        options.WithRoute(route);

        // Assert
        Assert.Same(route, options.Route);
    }

    [Fact]
    public void WithRoute_WithAllNodesRoute_SetsRouteProperty()
    {
        // Arrange
        var options = new ClusterScriptOptions();
        var route = Route.AllNodes;

        // Act
        options.WithRoute(route);

        // Assert
        Assert.Same(route, options.Route);
    }

    [Fact]
    public void WithRoute_WithSlotIdRoute_SetsRouteProperty()
    {
        // Arrange
        var options = new ClusterScriptOptions();
        var route = new Route.SlotIdRoute(1234, Route.SlotType.Primary);

        // Act
        options.WithRoute(route);

        // Assert
        Assert.Same(route, options.Route);
    }

    [Fact]
    public void WithRoute_WithSlotKeyRoute_SetsRouteProperty()
    {
        // Arrange
        var options = new ClusterScriptOptions();
        var route = new Route.SlotKeyRoute("mykey", Route.SlotType.Replica);

        // Act
        options.WithRoute(route);

        // Assert
        Assert.Same(route, options.Route);
    }

    [Fact]
    public void WithRoute_WithByAddressRoute_SetsRouteProperty()
    {
        // Arrange
        var options = new ClusterScriptOptions();
        var route = new Route.ByAddressRoute("localhost", 6379);

        // Act
        options.WithRoute(route);

        // Assert
        Assert.Same(route, options.Route);
    }

    [Fact]
    public void FluentBuilder_ChainsMultipleCalls()
    {
        // Arrange
        var route = Route.AllPrimaries;

        // Act
        var options = new ClusterScriptOptions()
            .WithArgs("arg1", "arg2", "arg3")
            .WithRoute(route);

        // Assert
        Assert.NotNull(options.Args);
        Assert.Equal(["arg1", "arg2", "arg3"], options.Args);
        Assert.Same(route, options.Route);
    }

    [Fact]
    public void WithArgs_WithEmptyArray_SetsEmptyArray()
    {
        // Arrange
        var options = new ClusterScriptOptions();

        // Act
        options.WithArgs([]);

        // Assert
        Assert.NotNull(options.Args);
        Assert.Empty(options.Args);
    }

    [Fact]
    public void WithArgs_OverwritesPreviousValue()
    {
        // Arrange
        var options = new ClusterScriptOptions()
            .WithArgs("arg1", "arg2");

        // Act
        options.WithArgs("arg3", "arg4");

        // Assert
        Assert.NotNull(options.Args);
        Assert.Equal(["arg3", "arg4"], options.Args);
    }

    [Fact]
    public void WithRoute_OverwritesPreviousValue()
    {
        // Arrange
        var route1 = Route.Random;
        var route2 = Route.AllPrimaries;
        var options = new ClusterScriptOptions()
            .WithRoute(route1);

        // Act
        options.WithRoute(route2);

        // Assert
        Assert.Same(route2, options.Route);
    }

    [Fact]
    public void PropertySetters_WorkDirectly()
    {
        // Arrange
        var options = new ClusterScriptOptions();
        string[] args = ["arg1"];
        var route = Route.AllNodes;

        // Act
        options.Args = args;
        options.Route = route;

        // Assert
        Assert.Equal(args, options.Args);
        Assert.Same(route, options.Route);
    }

    [Fact]
    public void PropertySetters_CanSetToNull()
    {
        // Arrange
        var options = new ClusterScriptOptions()
            .WithArgs("arg1")
            .WithRoute(Route.Random);

        // Act
        options.Args = null;
        options.Route = null;

        // Assert
        Assert.Null(options.Args);
        Assert.Null(options.Route);
    }

    [Fact]
    public void FluentBuilder_CanBuildWithOnlyArgs()
    {
        // Act
        var options = new ClusterScriptOptions()
            .WithArgs("arg1", "arg2");

        // Assert
        Assert.NotNull(options.Args);
        Assert.Equal(["arg1", "arg2"], options.Args);
        Assert.Null(options.Route);
    }

    [Fact]
    public void FluentBuilder_CanBuildWithOnlyRoute()
    {
        // Arrange
        var route = Route.AllPrimaries;

        // Act
        var options = new ClusterScriptOptions()
            .WithRoute(route);

        // Assert
        Assert.Null(options.Args);
        Assert.Same(route, options.Route);
    }
}
