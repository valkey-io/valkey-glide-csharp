// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Constants;

/// <summary>
/// Common constants used throughout Valkey GLIDE.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The LIMIT keyword for limiting the number of returned elements.
    /// </summary>
    public const string LimitKeyword = "LIMIT";

    /// <summary>
    /// The REPLACE keyword for replacing existing values.
    /// </summary>
    public const string ReplaceKeyword = "REPLACE";

    /// <summary>
    /// The DB keyword for specifying a database index.
    /// </summary>
    public const string DbKeyword = "DB";

    /// <summary>
    /// The ABSTTL keyword for specifying absolute TTL in milliseconds.
    /// </summary>
    public const string AbsttlKeyword = "ABSTTL";

    /// <summary>
    /// The IDLETIME keyword for specifying idle time in seconds.
    /// </summary>
    public const string IdletimeKeyword = "IDLETIME";

    /// <summary>
    /// The FREQ keyword for specifying frequency counter value.
    /// </summary>
    public const string FreqKeyword = "FREQ";

    /// <summary>
    /// The WITHSCORES keyword for including scores in sorted set results.
    /// </summary>
    public const string WithScoresKeyword = "WITHSCORES";

    /// <summary>
    /// The REV keyword for reversing the order of results.
    /// </summary>
    public const string ReverseKeyword = "REV";

    /// <summary>
    /// The BYLEX keyword for lexicographical ordering in sorted sets.
    /// </summary>
    public const string ByLexKeyword = "BYLEX";

    /// <summary>
    /// The BYSCORE keyword for score-based ordering in sorted sets.
    /// </summary>
    public const string ByScoreKeyword = "BYSCORE";

    /// <summary>
    /// The MATCH keyword for pattern matching in scan operations.
    /// </summary>
    public const string MatchKeyword = "MATCH";

    /// <summary>
    /// The COUNT keyword for specifying the number of elements to return.
    /// </summary>
    public const string CountKeyword = "COUNT";

    /// <summary>
    /// The TYPE keyword for filtering by data type.
    /// </summary>
    public const string TypeKeyword = "TYPE";

    /// <summary>
    /// The LEFT keyword for specifying left-side operations on lists.
    /// </summary>
    public const string LeftKeyword = "LEFT";

    /// <summary>
    /// The RIGHT keyword for specifying right-side operations on lists.
    /// </summary>
    public const string RightKeyword = "RIGHT";

    /// <summary>
    /// The BEFORE keyword for inserting before a pivot element.
    /// </summary>
    public const string BeforeKeyword = "BEFORE";

    /// <summary>
    /// The AFTER keyword for inserting after a pivot element.
    /// </summary>
    public const string AfterKeyword = "AFTER";

    /// <summary>
    /// The RANK keyword for specifying the rank of an element.
    /// </summary>
    public const string RankKeyword = "RANK";

    /// <summary>
    /// The MAXLEN keyword for specifying maximum length constraints.
    /// </summary>
    public const string MaxLenKeyword = "MAXLEN";

    /// <summary>
    /// The WITHVALUES keyword for including values in hash scan results.
    /// </summary>
    public const string WithValuesKeyword = "WITHVALUES";

    /// <summary>
    /// The PERSIST keyword for removing the time to live associated with a key.
    /// </summary>
    public const string PersistKeyword = "PERSIST";

    /// <summary>
    /// The EX keyword for setting the specified expire time in seconds.
    /// </summary>
    public const string ExKeyword = "EX";

    /// <summary>
    /// The PX keyword for setting the specified expire time in milliseconds.
    /// </summary>
    public const string PxKeyword = "PX";

    /// <summary>
    /// The EXAT keyword for setting the specified Unix time at which the key will expire, in seconds.
    /// </summary>
    public const string ExAtKeyword = "EXAT";

    /// <summary>
    /// The PXAT keyword for setting the specified Unix time at which the key will expire, in milliseconds.
    /// </summary>
    public const string PxAtKeyword = "PXAT";

    /// <summary>
    /// The STREAMS keyword for stream commands.
    /// </summary>
    public const string StreamsKeyword = "STREAMS";

    /// <summary>
    /// The KEEPTTL keyword for retaining the time to live associated with the key.
    /// </summary>
    public const string KeepTtlKeyword = "KEEPTTL";

    /// <summary>
    /// The NX keyword for only setting the key if it does not already exist.
    /// </summary>
    public const string NxKeyword = "NX";

    /// <summary>
    /// The XX keyword for only setting the key if it already exists.
    /// </summary>
    public const string XxKeyword = "XX";

    /// <summary>
    /// The GT keyword for only updating existing elements if the new score is greater than the current score.
    /// </summary>
    public const string GtKeyword = "GT";

    /// <summary>
    /// The LT keyword for only updating existing elements if the new score is less than the current score.
    /// </summary>
    public const string LtKeyword = "LT";

    /// <summary>
    /// The LEN keyword for the LCS command to return only the length of the longest common subsequence.
    /// </summary>
    public const string LenKeyword = "LEN";

    /// <summary>
    /// The IDX keyword for the LCS command to return match positions.
    /// </summary>
    public const string IdxKeyword = "IDX";

    /// <summary>
    /// The MINMATCHLEN keyword for the LCS command to restrict matches to a given minimal length.
    /// </summary>
    public const string MinMatchLenKeyword = "MINMATCHLEN";

    /// <summary>
    /// The WITHMATCHLEN keyword for the LCS command to include match lengths in results.
    /// </summary>
    public const string WithMatchLenKeyword = "WITHMATCHLEN";

    /// <summary>
    /// The XX keyword for sorted set operations to only update elements that already exist.
    /// </summary>
    public const string ExistsKeyword = "XX";

    /// <summary>
    /// The NX keyword for sorted set operations to only add new elements.
    /// </summary>
    public const string NotExistsKeyword = "NX";

    /// <summary>
    /// The GT keyword for sorted set operations to only update existing elements if the new score is greater.
    /// </summary>
    public const string GreaterThanKeyword = "GT";

    /// <summary>
    /// The LT keyword for sorted set operations to only update existing elements if the new score is less.
    /// </summary>
    public const string LessThanKeyword = "LT";

    /// <summary>
    /// The WEIGHTS keyword for specifying multiplication factors in sorted set operations.
    /// </summary>
    public const string WeightsKeyword = "WEIGHTS";

    /// <summary>
    /// The AGGREGATE keyword for specifying aggregation method in sorted set operations.
    /// </summary>
    public const string AggregateKeyword = "AGGREGATE";

    /// <summary>
    /// The MIN keyword for minimum aggregation in sorted set operations.
    /// </summary>
    public const string MinKeyword = "MIN";

    /// <summary>
    /// The MAX keyword for maximum aggregation in sorted set operations.
    /// </summary>
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
    /// The ALPHA keyword for sorting elements lexicographically instead of numerically.
    /// </summary>
    public const string AlphaKeyword = "ALPHA";

    /// <summary>
    /// The ASC keyword for sorting elements from small to large.
    /// </summary>
    public const string AscKeyword = "ASC";

    /// <summary>
    /// The DESC keyword for sorting elements from large to small.
    /// </summary>
    public const string DescKeyword = "DESC";

    /// <summary>
    /// The BY keyword for sorting elements using external keys as weights.
    /// </summary>
    public const string ByKeyword = "BY";

    /// <summary>
    /// The GET keyword for retrieving external keys based on sorted elements.
    /// </summary>
    public const string GetKeyword = "GET";

    /// <summary>
    /// The STORE keyword for storing the result at the specified key instead of returning it.
    /// </summary>
    public const string StoreKeyword = "STORE";

    /// <summary>
    /// The FIELDS keyword for hash field expiration commands.
    /// </summary>
    public const string FieldsKeyword = "FIELDS";

    /// <summary>
    /// The FXX keyword for hash field operations to update only existing fields.
    /// </summary>
    public const string FxxKeyword = "FXX";

    /// <summary>
    /// The FNX keyword for hash field operations to set only new fields.
    /// </summary>
    public const string FnxKeyword = "FNX";

    /// <summary>
    /// The SYNC keyword for synchronous flush operations.
    /// </summary>
    public const string SyncKeyword = "SYNC";

    /// <summary>
    /// The ASYNC keyword for asynchronous flush operations.
    /// </summary>
    public const string AsyncKeyword = "ASYNC";

    /// <summary>
    /// The VERSION keyword for specifying the version in LOLWUT commands.
    /// </summary>
    public const string VersionKeyword = "VERSION";
}
