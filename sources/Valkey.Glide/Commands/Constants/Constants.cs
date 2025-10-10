// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Constants;

/// <summary>
/// Common constants used throughout Valkey GLIDE.
/// </summary>
public static class Constants
{
    public const string LimitKeyword = "LIMIT";
    public const string ReplaceKeyword = "REPLACE";
    public const string DbKeyword = "DB";
    public const string AbsttlKeyword = "ABSTTL";
    public const string IdletimeKeyword = "IDLETIME";
    public const string FreqKeyword = "FREQ";
    public const string WithScoresKeyword = "WITHSCORES";
    public const string ReverseKeyword = "REV";
    public const string ByLexKeyword = "BYLEX";
    public const string ByScoreKeyword = "BYSCORE";
    public const string MatchKeyword = "MATCH";
    public const string CountKeyword = "COUNT";
    public const string LeftKeyword = "LEFT";
    public const string RightKeyword = "RIGHT";
    public const string BeforeKeyword = "BEFORE";
    public const string AfterKeyword = "AFTER";
    public const string RankKeyword = "RANK";
    public const string MaxLenKeyword = "MAXLEN";
    public const string WithValuesKeyword = "WITHVALUES";

    /// <summary>
    /// Expiry keywords.
    /// </summary>
    public const string PersistKeyword = "PERSIST";
    public const string ExpiryKeyword = "EX";
    public const string ExpiryAtKeyword = "EXAT";

    /// <summary>
    /// Keywords for the LCS command.
    /// </summary>
    public const string LenKeyword = "LEN";
    public const string IdxKeyword = "IDX";
    public const string MinMatchLenKeyword = "MINMATCHLEN";
    public const string WithMatchLenKeyword = "WITHMATCHLEN";

    /// <summary>
    /// Keywords for sorted set conditional operations.
    /// </summary>
    public const string ExistsKeyword = "XX";
    public const string NotExistsKeyword = "NX";
    public const string GreaterThanKeyword = "GT";
    public const string LessThanKeyword = "LT";

    /// <summary>
    /// Keywords for sorted set operations.
    /// </summary>
    public const string WeightsKeyword = "WEIGHTS";
    public const string AggregateKeyword = "AGGREGATE";
    public const string MinKeyword = "MIN";
    public const string MaxKeyword = "MAX";

    /// <summary>
    /// The highest bound in the sorted set for lexicographical operations.
    /// </summary>
    public const string PositiveInfinity = "+";

    /// <summary>
    /// The lowest bound in the sorted set for lexicographical operations.
    /// </summary>
    public const string NegativeInfinity = "-";

    /// <summary>
    /// The highest bound in the sorted set for score operations.
    /// </summary>
    public const string PositiveInfinityScore = "+inf";

    /// <summary>
    /// The lowest bound in the sorted set for score operations.
    /// </summary>
    public const string NegativeInfinityScore = "-inf";

    /// <summary>
    /// Keywords for SORT command.
    /// </summary>
    public const string AlphaKeyword = "ALPHA";
    public const string AscKeyword = "ASC";
    public const string DescKeyword = "DESC";
    public const string ByKeyword = "BY";
    public const string GetKeyword = "GET";
    public const string StoreKeyword = "STORE";
}
