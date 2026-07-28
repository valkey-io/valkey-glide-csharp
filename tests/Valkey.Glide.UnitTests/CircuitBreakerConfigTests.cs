// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for <see cref="CircuitBreakerConfig"/>.
/// </summary>
public class CircuitBreakerConfigTests
{
    #region Default Values

    [Fact]
    public void DefaultConfig_AllPropertiesAreUnset()
    {
        var config = new CircuitBreakerConfig();

        Assert.Null(config.WindowSize);
        Assert.Null(config.FailureRateThreshold);
        Assert.Null(config.MinErrors);
        Assert.Null(config.OpenTimeout);
        Assert.False(config.CountTimeouts);
        Assert.Null(config.ConsecutiveSuccesses);
    }

    #endregion
    #region WithWindowSize

    [Fact]
    public void WithWindowSize_ValidValue_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithWindowSize(TimeSpan.FromSeconds(15));

        Assert.Equal(TimeSpan.FromSeconds(15), config.WindowSize);
    }

    [Fact]
    public void WithWindowSize_Zero_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithWindowSize(TimeSpan.Zero));
    }

    [Fact]
    public void WithWindowSize_Negative_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithWindowSize(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void WithWindowSize_ExceedsMax_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithWindowSize(TimeSpan.FromDays(50)));
    }

    #endregion
    #region WithFailureRateThreshold

    [Fact]
    public void WithFailureRateThreshold_ValidValue_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithFailureRateThreshold(0.6);

        Assert.Equal(0.6, config.FailureRateThreshold);
    }

    [Fact]
    public void WithFailureRateThreshold_One_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithFailureRateThreshold(1.0);

        Assert.Equal(1.0, config.FailureRateThreshold);
    }

    [Fact]
    public void WithFailureRateThreshold_Zero_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithFailureRateThreshold(0.0));
    }

    [Fact]
    public void WithFailureRateThreshold_Negative_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithFailureRateThreshold(-0.5));
    }

    [Fact]
    public void WithFailureRateThreshold_AboveOne_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithFailureRateThreshold(1.1));
    }

    #endregion
    #region WithMinErrors

    [Fact]
    public void WithMinErrors_ValidValue_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithMinErrors(100);

        Assert.Equal(100u, config.MinErrors);
    }

    [Fact]
    public void WithMinErrors_Zero_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithMinErrors(0));
    }

    #endregion
    #region WithOpenTimeout

    [Fact]
    public void WithOpenTimeout_ValidValue_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithOpenTimeout(TimeSpan.FromSeconds(10));

        Assert.Equal(TimeSpan.FromSeconds(10), config.OpenTimeout);
    }

    [Fact]
    public void WithOpenTimeout_Zero_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithOpenTimeout(TimeSpan.Zero));
    }

    [Fact]
    public void WithOpenTimeout_Negative_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithOpenTimeout(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void WithOpenTimeout_ExceedsMax_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithOpenTimeout(TimeSpan.FromDays(50)));
    }

    #endregion
    #region WithCountTimeouts

    [Fact]
    public void WithCountTimeouts_True_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithCountTimeouts(true);

        Assert.True(config.CountTimeouts);
    }

    [Fact]
    public void WithCountTimeouts_DefaultParam_SetsTrue()
    {
        var config = new CircuitBreakerConfig()
            .WithCountTimeouts();

        Assert.True(config.CountTimeouts);
    }

    [Fact]
    public void WithCountTimeouts_False_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithCountTimeouts(false);

        Assert.False(config.CountTimeouts);
    }

    #endregion
    #region WithConsecutiveSuccesses

    [Fact]
    public void WithConsecutiveSuccesses_ValidValue_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithConsecutiveSuccesses(5);

        Assert.Equal(5u, config.ConsecutiveSuccesses);
    }

    [Fact]
    public void WithConsecutiveSuccesses_Zero_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithConsecutiveSuccesses(0));
    }

    #endregion
}
