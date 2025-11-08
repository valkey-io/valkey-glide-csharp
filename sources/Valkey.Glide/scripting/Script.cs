// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Represents a Lua script with automatic SHA1 hash management and FFI integration.
/// Implements IDisposable to ensure proper cleanup of resources in the Rust core.
/// </summary>
/// <remarks>
/// The Script class stores Lua script code in the Rust core and manages its lifecycle.
/// When a Script is created, the code is sent to the Rust core which calculates and stores
/// the SHA1 hash. When disposed, the script is removed from the Rust core storage.
///
/// This class is thread-safe and can be safely accessed from multiple threads.
/// Multiple calls to Dispose are safe and will not cause errors.
/// </remarks>
public sealed class Script : IDisposable
{
    private readonly string _hash;
    private readonly object _lock = new();
    private bool _disposed;

    /// <summary>
    /// Creates a new Script instance and stores it in Rust core.
    /// </summary>
    /// <param name="code">The Lua script code.</param>
    /// <exception cref="ArgumentNullException">Thrown when code is null.</exception>
    /// <exception cref="ArgumentException">Thrown when code is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when script storage in Rust core fails.</exception>
    public Script(string code)
    {
        if (code == null)
        {
            throw new ArgumentNullException(nameof(code), "Script code cannot be null");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Script code cannot be empty or whitespace", nameof(code));
        }

        Code = code;
        _hash = Internals.FFI.StoreScript(code);
    }

    /// <summary>
    /// Gets the SHA1 hash of the script.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when accessing the hash after the script has been disposed.</exception>
    public string Hash
    {
        get
        {
            ThrowIfDisposed();
            return _hash;
        }
    }

    /// <summary>
    /// Gets the original Lua script code.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when accessing the code after the script has been disposed.</exception>
    internal string Code
    {
        get
        {
            ThrowIfDisposed();
            return field;
        }
    }

    /// <summary>
    /// Releases the script from Rust core storage.
    /// This method is thread-safe and can be called multiple times without error.
    /// </summary>
    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                Internals.FFI.DropScript(_hash);
            }
            catch
            {
                // Suppress exceptions during disposal to prevent issues in finalizer
                // The Rust core will handle cleanup even if this fails
            }
            finally
            {
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }

    /// <summary>
    /// Finalizer to ensure cleanup if Dispose is not called.
    /// </summary>
    ~Script()
    {
        Dispose();
    }

    /// <summary>
    /// Throws ObjectDisposedException if the script has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(Script), "Cannot access a disposed Script");
        }
    }
}
