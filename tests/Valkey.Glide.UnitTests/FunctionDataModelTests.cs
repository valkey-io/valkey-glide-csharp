// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class FunctionDataModelTests
{
    #region FunctionListOptions Tests

    [Fact]
    public void FunctionListOptions_DefaultValues_AreCorrect()
    {
        // Act
        FunctionListOptions options = new();

        // Assert
        Assert.Null(options.LibraryName);
        Assert.False(options.WithCode);
    }

    [Fact]
    public void FunctionListOptions_InitSyntax_SetsProperties()
    {
        // Arrange
        string libraryName = "mylib";

        // Act
        FunctionListOptions options = new()
        {
            LibraryName = libraryName,
            WithCode = true
        };

        // Assert
        Assert.Equal(libraryName, options.LibraryName);
        Assert.True(options.WithCode);
    }

    #endregion
}
