// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for the <c>FAILOVER</c> command.
/// </summary>
/// <seealso href="https://valkey.io/commands/failover/"/>
public sealed class FailoverOptions
{
    #region Private Fields

    private string? _host;
    private int _port;
    private bool _force;
    private bool _abort;
    private TimeSpan? _timeout;

    #endregion
    #region Constructors & Builders

    private FailoverOptions() { }

    /// <summary>
    /// Creates options for an abort failover command.
    /// </summary>
    public static FailoverOptions Abort()
        => new() { _abort = true };

    /// <summary>
    /// Creates options for a failover with timeout.
    /// </summary>
    /// <param name="timeout">The maximum time to wait before aborting the failover.</param>
    public static FailoverOptions Timeout(TimeSpan timeout)
        => new() { _timeout = timeout };

    /// <summary>
    /// Creates options for a failover with a specified replica.
    /// </summary>
    /// <param name="host">The host of the target replica.</param>
    /// <param name="port">The port of the target replica.</param>
    public static FailoverOptions To(string host, int port)
        => new() { _host = host, _port = port };

    /// <summary>
    /// Creates options for a failover with a specified replica and timeout.
    /// </summary>
    /// <param name="host">The host of the target replica.</param>
    /// <param name="port">The port of the target replica.</param>
    /// <param name="timeout">The maximum time to wait before aborting.</param>
    public static FailoverOptions To(string host, int port, TimeSpan timeout)
        => new() { _host = host, _port = port, _timeout = timeout };

    /// <summary>
    /// Creates options for a forced failover with a specified replica and timeout.
    /// </summary>
    /// <param name="host">The host of the target replica.</param>
    /// <param name="port">The port of the target replica.</param>
    /// <param name="timeout">The maximum time before forcing the failover.</param>
    public static FailoverOptions Forced(string host, int port, TimeSpan timeout)
        => new() { _host = host, _port = port, _force = true, _timeout = timeout };

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts the options to command arguments.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        List<GlideString> args = [];

        if (_abort)
        {
            args.Add(ValkeyLiterals.ABORT);
        }
        else
        {
            if (_host is not null)
            {
                args.Add(ValkeyLiterals.TO);
                args.Add(_host);
                args.Add(_port.ToGlideString());

                if (_force)
                {
                    args.Add(ValkeyLiterals.FORCE);
                }
            }

            if (_timeout is not null)
            {
                args.Add(ValkeyLiterals.TIMEOUT);
                args.Add(ToGlideStringMilliseconds(_timeout.Value));
            }
        }

        return [.. args];
    }

    #endregion
}
