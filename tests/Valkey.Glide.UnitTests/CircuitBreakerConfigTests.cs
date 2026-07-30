// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Unit tests for <see cref="CircuitBreakerConfig"/>.
/// </summary>
public class CircuitBreakerConfigTests
{
    #region Constants

    private static readonly TimeSpan TooLarge =
        CircuitBreakerConfig.MaxTimeSpan + TimeSpan.FromMilliseconds(1);

    #endregion
    #region Default Values

    [Fact]
    public void DefaultConfig_AllPropertiesCorrect()
    {
        var config = new CircuitBreakerConfig();

        Assert.Equal(TimeSpan.FromSeconds(10), config.WindowSize);
        Assert.Equal(0.5f, config.FailureRateThreshold);
        Assert.Equal(50u, config.MinErrors);
        Assert.Equal(TimeSpan.FromSeconds(5), config.OpenTimeout);
        Assert.False(config.CountTimeouts);
        Assert.Equal(3u, config.ConsecutiveSuccesses);
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
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithWindowSize(TooLarge));
    }

    #endregion
    #region WithFailureRateThreshold

    [Fact]
    public void WithFailureRateThreshold_ValidValue_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithFailureRateThreshold(0.6f);

        Assert.Equal(0.6f, config.FailureRateThreshold);
    }

    [Fact]
    public void WithFailureRateThreshold_One_SetsProperty()
    {
        var config = new CircuitBreakerConfig()
            .WithFailureRateThreshold(1.0f);

        Assert.Equal(1.0f, config.FailureRateThreshold);
    }

    [Fact]
    public void WithFailureRateThreshold_Zero_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithFailureRateThreshold(0.0f));
    }

    [Fact]
    public void WithFailureRateThreshold_Negative_Throws()
    {
        var config = new CircuitBreakerConfig();
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithFailureRateThreshold(-0.5f));
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
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithOpenTimeout(TooLarge));
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
    #region ToFfi

    [Fact]
    public void ToFfi_DefaultConfig()
    {
        var ffi = new CircuitBreakerConfig().ToFfi();

        Assert.Equal(10_000u, ffi.WindowSizeMs);
        Assert.Equal(0.5f, ffi.FailureRateThreshold);
        Assert.Equal(50u, ffi.MinErrors);
        Assert.Equal(5_000u, ffi.OpenTimeoutMs);
        Assert.False(ffi.CountTimeouts);
        Assert.Equal(3u, ffi.ConsecutiveSuccesses);
    }

    [Fact]
    public void ToFfi_CustomConfig()
    {
        var ffi = new CircuitBreakerConfig()
            .WithWindowSize(TimeSpan.FromSeconds(30))
            .WithFailureRateThreshold(0.75f)
            .WithMinErrors(200)
            .WithOpenTimeout(TimeSpan.FromMilliseconds(2500))
            .WithCountTimeouts(true)
            .WithConsecutiveSuccesses(5)
            .ToFfi();

        Assert.Equal(30_000u, ffi.WindowSizeMs);
        Assert.Equal(0.75f, ffi.FailureRateThreshold);
        Assert.Equal(200u, ffi.MinErrors);
        Assert.Equal(2_500u, ffi.OpenTimeoutMs);
        Assert.True(ffi.CountTimeouts);
        Assert.Equal(5u, ffi.ConsecutiveSuccesses);
    }

    #endregion
}
