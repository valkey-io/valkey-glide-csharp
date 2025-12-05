// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Temporary file that is deleted when disposed.
/// </summary>
public sealed class TempFile : IDisposable
{
    private bool _disposed;

    public string Path { get; }

    public byte[] Bytes
    {
        get => File.ReadAllBytes(Path);
        set => File.WriteAllBytes(Path, value);
    }

    public string Text
    {
        get => File.ReadAllText(Path);
        set => File.WriteAllText(Path, value);
    }

    public string[] Lines
    {
        get => File.ReadAllLines(Path);
        set => File.WriteAllLines(Path, value);
    }

    public TempFile()
    {
        Path = System.IO.Path.GetTempFileName();
    }

    public TempFile(byte[] contents) : this()
    {
        File.WriteAllBytes(Path, contents);
    }

    public TempFile(string contents) : this()
    {
        File.WriteAllText(Path, contents);
    }

    ~TempFile()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        File.Delete(Path);
        GC.SuppressFinalize(this);
    }
}
