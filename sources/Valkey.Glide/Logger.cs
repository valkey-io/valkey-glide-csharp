// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

namespace Valkey.Glide;

/// <summary>
/// Specifies the severity level for log messages.
/// </summary>
public enum Level
{
    /// <summary>
    /// Error level - logs only error messages indicating failures.
    /// </summary>
    Error = 0,

    /// <summary>
    /// Warning level - logs warnings and errors.
    /// </summary>
    Warn = 1,

    /// <summary>
    /// Info level - logs informational messages, warnings, and errors.
    /// </summary>
    Info = 2,

    /// <summary>
    /// Debug level - logs debug information and all higher severity messages.
    /// </summary>
    Debug = 3,

    /// <summary>
    /// Trace level - logs all messages including detailed trace information.
    /// </summary>
    Trace = 4,

    /// <summary>
    /// Off - disables all logging.
    /// </summary>
    Off = 5,
}

/// <summary>
/// A singleton class that allows logging which is consistent with logs from the internal GLIDE core.
/// The logger can be set up in 2 ways:
/// <list type="number">
///   <item>
///     By calling <see cref="Init(Level, string?)" />, which configures the logger only if it wasn't previously configured.
///   </item>
///   <item>
///      By calling <see cref="SetLoggerConfig(Level, string?)" />, which replaces the existing configuration, and means that
///      new logs will not be saved with the logs that were sent before the call.
///   </item>
/// </list>
/// If none of these functions are called, the first log attempt will initialize a new logger with default configuration.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/monitoring/logging/">Valkey GLIDE – Logging</seealso>
public class Logger
{
    #region private fields

    private static Level? s_loggerLevel = null;
    #endregion private fields

    #region public methods
    /// <summary>
    /// Initialize a logger if it wasn't initialized before - this method is meant to be used when there is no intention to
    /// replace an existing logger.<br />
    /// The logger will filter all logs with a level lower than the given level,
    /// If given a <paramref name="filename"/> argument, will write the logs to files postfixed with <paramref name="filename"/>.
    /// If <paramref name="filename"/> isn't provided, the logs will be written to the console.
    /// </summary>
    /// <param name="level">
    /// Set the logger level to one of <c>[ERROR, WARN, INFO, DEBUG, TRACE, OFF]</c>.
    /// If log level isn't provided, the logger will be configured with default configuration.
    /// To turn off logging completely, set the level to <see cref="Level.Off"/>.
    /// </param>
    /// <param name="filename">
    /// If provided the target of the logs will be the file mentioned.<br />
    /// Otherwise, logs will be printed to the console.
    /// </param>
    public static void Init(Level level, string? filename = null) => SetLoggerConfig(level, filename);

    /// <summary>
    /// Logs the provided message if the provided log level is lower then the logger level.
    /// </summary>
    /// <param name="logLevel">The log level of the provided message.</param>
    /// <param name="logIdentifier">The log identifier should give the log a context.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="error">The exception or error to log.</param>
    public static void Log(Level logLevel, string logIdentifier, string message, Exception? error = null)
    {
        if (s_loggerLevel is null)
        {
            SetLoggerConfig(logLevel);
        }
        if (!(logLevel <= s_loggerLevel))
        {
            return;
        }
        if (error is not null)
        {
            message += $": {error}";
        }
        log(Convert.ToInt32(logLevel), logIdentifier, message);
    }

    /// <summary>
    /// Creates a new logger instance and configure it with the provided log level and file name.
    /// </summary>
    /// <param name="level">
    /// Set the logger level to one of <c>[ERROR, WARN, INFO, DEBUG, TRACE, OFF]</c>.
    /// If log level isn't provided, the logger will be configured with default configuration.
    /// To turn off logging completely, set the level to <see cref="Level.Off"/>.
    /// </param>
    /// <param name="filename">
    /// If provided the target of the logs will be the file mentioned.<br />
    /// Otherwise, logs will be printed to the console.
    /// </param>
    /// <exception cref="InvalidOperationException">Thrown when the native logger fails to initialize.</exception>
    public static void SetLoggerConfig(Level level, string? filename = null)
    {
        IntPtr errorPtr = InitInternalLogger(Convert.ToInt32(level), filename, out Level resolvedLevel);
        if (errorPtr != IntPtr.Zero)
        {
            string? errorMessage = Marshal.PtrToStringUTF8(errorPtr);
            FreeString(errorPtr);
            throw new InvalidOperationException($"Failed to initialize logger: {errorMessage}");
        }
        s_loggerLevel = resolvedLevel;
    }

    #endregion public methods

    #region FFI function declaration
    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "log")]
    private static extern void log(
        int logLevel,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string logIdentifier,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string message);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "init_logger")]
    private static extern IntPtr InitInternalLogger(
        int level,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? filename,
        out Level levelOut);

    [DllImport("libglide_rs", CallingConvention = CallingConvention.Cdecl, EntryPoint = "free_string")]
    private static extern void FreeString(IntPtr strPtr);

    #endregion
}
