// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Exception types for the Valkey GLIDE client.
/// </summary>
public static class Errors
{
    /// <summary>
    /// Base class for errors.
    /// </summary>
    public abstract class GlideException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlideException"/> class.
        /// </summary>
        public GlideException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlideException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public GlideException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlideException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public GlideException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// An error on Valkey service-side that was reported during a request.
    /// </summary>
    public sealed class RequestException : GlideException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestException"/> class.
        /// </summary>
        public RequestException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RequestException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public RequestException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// An error returned by the Valkey server during script or function execution.
    /// This includes Lua comtion errors, runtime errors, and script/function management errors.
    /// </summary>
    public sealed class ValkeyServerException : GlideException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValkeyServerException"/> class.
        /// </summary>
        public ValkeyServerException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValkeyServerException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ValkeyServerException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValkeyServerException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ValkeyServerException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// An error on Valkey service-side that is thrown when a transaction is aborted
    /// </summary>
    public sealed class ExecAbortException : GlideException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecAbortException"/> class.
        /// </summary>
        public ExecAbortException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecAbortException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ExecAbortException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecAbortException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ExecAbortException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// A timeout from Glide to Valkey service that is thrown when a request times out.
    /// </summary>
    public sealed class TimeoutException : GlideException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutException"/> class.
        /// </summary>
        public TimeoutException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TimeoutException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public TimeoutException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// A connection problem between Glide and Valkey.<br />
    /// That error is thrown when a connection disconnects. These errors can be temporary, as the client will attempt to reconnect.
    /// </summary>
    public sealed class ConnectionException : GlideException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        public ConnectionException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConnectionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// An errors that is thrown when a request cannot be completed in current configuration settings.
    /// </summary>
    public sealed class ConfigurationError : GlideException
    {
        // TODO set HelpLink with link to wiki

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationError"/> class.
        /// </summary>
        public ConfigurationError() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationError"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConfigurationError(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationError"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ConfigurationError(string message, Exception innerException) : base(message, innerException) { }
    }

    internal static GlideException Create(RequestErrorType type, string message) => type switch
    {
        RequestErrorType.Unspecified => new RequestException(message),
        RequestErrorType.ExecAbort => new ExecAbortException(message),
        RequestErrorType.Timeout => new TimeoutException(message),
        RequestErrorType.Disconnect => new ConnectionException(message),
        _ => new RequestException(message),
    };
}

internal enum RequestErrorType : uint
{
    Unspecified = 0,
    ExecAbort = 1,
    Timeout = 2,
    Disconnect = 3,
}
