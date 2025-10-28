// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class FunctionDataModelTests
{
    #region LibraryInfo Tests

    [Fact]
    public void LibraryInfo_Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        string name = "mylib";
        string engine = "LUA";
        FunctionInfo[] functions =
        [
            new FunctionInfo("func1", "Description 1", ["no-writes"]),
            new FunctionInfo("func2", null, ["allow-oom"])
        ];
        string code = "return 'hello'";

        // Act
        LibraryInfo libraryInfo = new(name, engine, functions, code);

        // Assert
        Assert.Equal(name, libraryInfo.Name);
        Assert.Equal(engine, libraryInfo.Engine);
        Assert.Equal(functions, libraryInfo.Functions);
        Assert.Equal(code, libraryInfo.Code);
    }

    [Fact]
    public void LibraryInfo_Constructor_WithoutCode_CreatesInstanceWithNullCode()
    {
        // Arrange
        string name = "mylib";
        string engine = "LUA";
        FunctionInfo[] functions = [new FunctionInfo("func1", null, [])];

        // Act
        LibraryInfo libraryInfo = new(name, engine, functions);

        // Assert
        Assert.Equal(name, libraryInfo.Name);
        Assert.Equal(engine, libraryInfo.Engine);
        Assert.Equal(functions, libraryInfo.Functions);
        Assert.Null(libraryInfo.Code);
    }

    [Fact]
    public void LibraryInfo_Constructor_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        string engine = "LUA";
        FunctionInfo[] functions = [new FunctionInfo("func1", null, [])];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LibraryInfo(null!, engine, functions));
    }

    [Fact]
    public void LibraryInfo_Constructor_WithNullEngine_ThrowsArgumentNullException()
    {
        // Arrange
        string name = "mylib";
        FunctionInfo[] functions = [new FunctionInfo("func1", null, [])];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LibraryInfo(name, null!, functions));
    }

    [Fact]
    public void LibraryInfo_Constructor_WithNullFunctions_ThrowsArgumentNullException()
    {
        // Arrange
        string name = "mylib";
        string engine = "LUA";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LibraryInfo(name, engine, null!));
    }

    #endregion

    #region FunctionInfo Tests

    [Fact]
    public void FunctionInfo_Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        string name = "myfunction";
        string description = "My function description";
        string[] flags = ["no-writes", "allow-oom"];

        // Act
        FunctionInfo functionInfo = new(name, description, flags);

        // Assert
        Assert.Equal(name, functionInfo.Name);
        Assert.Equal(description, functionInfo.Description);
        Assert.Equal(flags, functionInfo.Flags);
    }

    [Fact]
    public void FunctionInfo_Constructor_WithNullDescription_CreatesInstance()
    {
        // Arrange
        string name = "myfunction";
        string[] flags = ["no-writes"];

        // Act
        FunctionInfo functionInfo = new(name, null, flags);

        // Assert
        Assert.Equal(name, functionInfo.Name);
        Assert.Null(functionInfo.Description);
        Assert.Equal(flags, functionInfo.Flags);
    }

    [Fact]
    public void FunctionInfo_Constructor_WithEmptyFlags_CreatesInstance()
    {
        // Arrange
        string name = "myfunction";
        string description = "Description";
        string[] flags = [];

        // Act
        FunctionInfo functionInfo = new(name, description, flags);

        // Assert
        Assert.Equal(name, functionInfo.Name);
        Assert.Equal(description, functionInfo.Description);
        Assert.Empty(functionInfo.Flags);
    }

    [Fact]
    public void FunctionInfo_Constructor_WithNullName_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new FunctionInfo(null!, "description", ["no-writes"]));

    [Fact]
    public void FunctionInfo_Constructor_WithNullFlags_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new FunctionInfo("myfunction", "description", null!));

    #endregion

    #region FunctionStatsResult Tests

    [Fact]
    public void FunctionStatsResult_Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        Dictionary<string, EngineStats> engines = new()
        {
            ["LUA"] = new EngineStats("LUA", 5, 2)
        };
        RunningScriptInfo runningScript = new("myscript", "FCALL", ["arg1"], TimeSpan.FromSeconds(10));

        // Act
        FunctionStatsResult result = new(engines, runningScript);

        // Assert
        Assert.Equal(engines, result.Engines);
        Assert.Equal(runningScript, result.RunningScript);
    }

    [Fact]
    public void FunctionStatsResult_Constructor_WithoutRunningScript_CreatesInstanceWithNullRunningScript()
    {
        // Arrange
        Dictionary<string, EngineStats> engines = new()
        {
            ["LUA"] = new EngineStats("LUA", 5, 2)
        };

        // Act
        FunctionStatsResult result = new(engines);

        // Assert
        Assert.Equal(engines, result.Engines);
        Assert.Null(result.RunningScript);
    }

    [Fact]
    public void FunctionStatsResult_Constructor_WithNullEngines_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new FunctionStatsResult(null!));

    #endregion

    #region EngineStats Tests

    [Fact]
    public void EngineStats_Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        string language = "LUA";
        long functionCount = 10L;
        long libraryCount = 3L;

        // Act
        EngineStats stats = new(language, functionCount, libraryCount);

        // Assert
        Assert.Equal(language, stats.Language);
        Assert.Equal(functionCount, stats.FunctionCount);
        Assert.Equal(libraryCount, stats.LibraryCount);
    }

    [Fact]
    public void EngineStats_Constructor_WithZeroCounts_CreatesInstance()
    {
        // Arrange
        string language = "LUA";

        // Act
        EngineStats stats = new(language, 0, 0);

        // Assert
        Assert.Equal(language, stats.Language);
        Assert.Equal(0, stats.FunctionCount);
        Assert.Equal(0, stats.LibraryCount);
    }

    [Fact]
    public void EngineStats_Constructor_WithNullLanguage_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new EngineStats(null!, 5, 2));

    #endregion

    #region RunningScriptInfo Tests

    [Fact]
    public void RunningScriptInfo_Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        string name = "myscript";
        string command = "FCALL";
        string[] args = ["arg1", "arg2"];
        TimeSpan duration = TimeSpan.FromSeconds(5);

        // Act
        RunningScriptInfo info = new(name, command, args, duration);

        // Assert
        Assert.Equal(name, info.Name);
        Assert.Equal(command, info.Command);
        Assert.Equal(args, info.Args);
        Assert.Equal(duration, info.Duration);
    }

    [Fact]
    public void RunningScriptInfo_Constructor_WithEmptyArgs_CreatesInstance()
    {
        // Arrange
        string name = "myscript";
        string command = "FCALL";
        string[] args = [];
        TimeSpan duration = TimeSpan.FromSeconds(1);

        // Act
        RunningScriptInfo info = new(name, command, args, duration);

        // Assert
        Assert.Equal(name, info.Name);
        Assert.Equal(command, info.Command);
        Assert.Empty(info.Args);
        Assert.Equal(duration, info.Duration);
    }

    [Fact]
    public void RunningScriptInfo_Constructor_WithNullName_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new RunningScriptInfo(null!, "FCALL", ["arg1"], TimeSpan.FromSeconds(1)));

    [Fact]
    public void RunningScriptInfo_Constructor_WithNullCommand_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new RunningScriptInfo("myscript", null!, ["arg1"], TimeSpan.FromSeconds(1)));

    [Fact]
    public void RunningScriptInfo_Constructor_WithNullArgs_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => new RunningScriptInfo("myscript", "FCALL", null!, TimeSpan.FromSeconds(1)));

    #endregion

    #region FunctionListQuery Tests

    [Fact]
    public void FunctionListQuery_Constructor_CreatesInstanceWithDefaultValues()
    {
        // Act
        FunctionListQuery query = new();

        // Assert
        Assert.Null(query.LibraryName);
        Assert.False(query.WithCode);
    }

    [Fact]
    public void FunctionListQuery_ForLibrary_SetsLibraryName()
    {
        // Arrange
        FunctionListQuery query = new();
        string libraryName = "mylib";

        // Act
        FunctionListQuery result = query.ForLibrary(libraryName);

        // Assert
        Assert.Equal(libraryName, query.LibraryName);
        Assert.Same(query, result); // Verify fluent interface
    }

    [Fact]
    public void FunctionListQuery_IncludeCode_SetsWithCodeToTrue()
    {
        // Arrange
        FunctionListQuery query = new();

        // Act
        FunctionListQuery result = query.IncludeCode();

        // Assert
        Assert.True(query.WithCode);
        Assert.Same(query, result); // Verify fluent interface
    }

    [Fact]
    public void FunctionListQuery_FluentChaining_WorksCorrectly()
    {
        // Arrange
        string libraryName = "mylib";

        // Act
        FunctionListQuery query = new FunctionListQuery()
            .ForLibrary(libraryName)
            .IncludeCode();

        // Assert
        Assert.Equal(libraryName, query.LibraryName);
        Assert.True(query.WithCode);
    }

    [Fact]
    public void FunctionListQuery_PropertySetters_WorksCorrectly()
    {
        // Arrange
        FunctionListQuery query = new();
        string libraryName = "mylib";

        // Act
        query.LibraryName = libraryName;
        query.WithCode = true;

        // Assert
        Assert.Equal(libraryName, query.LibraryName);
        Assert.True(query.WithCode);
    }

    #endregion
}
