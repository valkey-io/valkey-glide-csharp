// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Globalization;

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Sets the current culture and current UI culture for the lifetime of the scope,
/// then restores the previous cultures when disposed.
/// </summary>
public sealed class CultureScope : IDisposable
{
    #region Private Fields

    private readonly CultureInfo _previousCulture;
    private readonly CultureInfo _previousUICulture;

    #endregion
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureScope"/> class.
    /// </summary>
    /// <param name="culture">The culture to use within the scope.</param>
    public CultureScope(CultureInfo culture)
    {
        _previousCulture = CultureInfo.CurrentCulture;
        _previousUICulture = CultureInfo.CurrentUICulture;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    #endregion
    #region Public Methods

    /// <inheritdoc/>
    public void Dispose()
    {
        CultureInfo.CurrentCulture = _previousCulture;
        CultureInfo.CurrentUICulture = _previousUICulture;
    }

    #endregion
}
