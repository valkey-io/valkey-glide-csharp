// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Temporary file that is deleted when disposed.
/// </summary>
public sealed class TempFile : IDisposable
{
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

    public void Dispose() => File.Delete(Path);
}
