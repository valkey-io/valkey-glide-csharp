// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Reducer functions available for use in a <see cref="FtAggregateReducer"/>.
/// </summary>
/// <seealso href="https://valkey.io/commands/ft.aggregate/">valkey.io</seealso>
public enum FtReducerFunction
{
    /// <summary>Counts the number of records in each group.</summary>
    Count,
    /// <summary>Counts the number of distinct values of an expression.</summary>
    CountDistinct,
    /// <summary>Sums the numeric values of an expression.</summary>
    Sum,
    /// <summary>Returns the minimum numeric value of an expression.</summary>
    Min,
    /// <summary>Returns the maximum numeric value of an expression.</summary>
    Max,
    /// <summary>Returns the average numeric value of an expression.</summary>
    Avg,
    /// <summary>Returns the standard deviation of the numeric values of an expression.</summary>
    Stddev,
}

internal static class FtReducerFunctionExtensions
{
    internal static GlideString ToLiteral(this FtReducerFunction function)
        => function switch
        {
            FtReducerFunction.Count => "COUNT",
            FtReducerFunction.CountDistinct => "COUNT_DISTINCT",
            FtReducerFunction.Sum => "SUM",
            FtReducerFunction.Min => "MIN",
            FtReducerFunction.Max => "MAX",
            FtReducerFunction.Avg => "AVG",
            FtReducerFunction.Stddev => "STDDEV",
            _ => throw new ArgumentOutOfRangeException(nameof(function)),
        };
}
