// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The type of keyspace or keyevent notification.
/// Compatible with StackExchange.Redis <c>KeyNotificationType</c>.
/// </summary>
public enum KeyNotificationType
{
    /// <summary>Unknown notification type.</summary>
    Unknown = 0,
    /// <summary>APPEND command.</summary>
    Append = 1,
    /// <summary>COPY command.</summary>
    Copy = 2,
    /// <summary>DEL command.</summary>
    Del = 3,
    /// <summary>EXPIRE command.</summary>
    Expire = 4,
    /// <summary>HDEL command.</summary>
    HDel = 5,
    /// <summary>Hash field expired.</summary>
    HExpired = 6,
    /// <summary>HINCRBYFLOAT command.</summary>
    HIncrByFloat = 7,
    /// <summary>HINCRBY command.</summary>
    HIncrBy = 8,
    /// <summary>HPERSIST command.</summary>
    HPersist = 9,
    /// <summary>HSET command.</summary>
    HSet = 10,
    /// <summary>INCRBYFLOAT command.</summary>
    IncrByFloat = 11,
    /// <summary>INCRBY command.</summary>
    IncrBy = 12,
    /// <summary>LINSERT command.</summary>
    LInsert = 13,
    /// <summary>LPOP command.</summary>
    LPop = 14,
    /// <summary>LPUSH command.</summary>
    LPush = 15,
    /// <summary>LREM command.</summary>
    LRem = 16,
    /// <summary>LSET command.</summary>
    LSet = 17,
    /// <summary>LTRIM command.</summary>
    LTrim = 18,
    /// <summary>Key moved from this database.</summary>
    MoveFrom = 19,
    /// <summary>Key moved to this database.</summary>
    MoveTo = 20,
    /// <summary>PERSIST command.</summary>
    Persist = 21,
    /// <summary>Key renamed from this name.</summary>
    RenameFrom = 22,
    /// <summary>Key renamed to this name.</summary>
    RenameTo = 23,
    /// <summary>RESTORE command.</summary>
    Restore = 24,
    /// <summary>RPOP command.</summary>
    RPop = 25,
    /// <summary>RPUSH command.</summary>
    RPush = 26,
    /// <summary>SADD command.</summary>
    SAdd = 27,
    /// <summary>SET command.</summary>
    Set = 28,
    /// <summary>SETRANGE command.</summary>
    SetRange = 29,
    /// <summary>SORT with STORE option.</summary>
    SortStore = 30,
    /// <summary>SREM command.</summary>
    SRem = 31,
    /// <summary>SPOP command.</summary>
    SPop = 32,
    /// <summary>XADD command.</summary>
    XAdd = 33,
    /// <summary>XDEL command.</summary>
    XDel = 34,
    /// <summary>XGROUP CREATECONSUMER command.</summary>
    XGroupCreateConsumer = 35,
    /// <summary>XGROUP CREATE command.</summary>
    XGroupCreate = 36,
    /// <summary>XGROUP DELCONSUMER command.</summary>
    XGroupDelConsumer = 37,
    /// <summary>XGROUP DESTROY command.</summary>
    XGroupDestroy = 38,
    /// <summary>XGROUP SETID command.</summary>
    XGroupSetId = 39,
    /// <summary>XSETID command.</summary>
    XSetId = 40,
    /// <summary>XTRIM command.</summary>
    XTrim = 41,
    /// <summary>ZADD command.</summary>
    ZAdd = 42,
    /// <summary>ZDIFFSTORE command.</summary>
    ZDiffStore = 43,
    /// <summary>ZINTERSTORE command.</summary>
    ZInterStore = 44,
    /// <summary>ZUNIONSTORE command.</summary>
    ZUnionStore = 45,
    /// <summary>ZINCRBY command.</summary>
    ZIncr = 46,
    /// <summary>ZREMRANGEBYRANK command.</summary>
    ZRemByRank = 47,
    /// <summary>ZREMRANGEBYSCORE command.</summary>
    ZRemByScore = 48,
    /// <summary>ZREM command.</summary>
    ZRem = 49,

    // Side-effect notifications

    /// <summary>Key expired due to TTL.</summary>
    Expired = 1000,
    /// <summary>Key evicted due to maxmemory policy.</summary>
    Evicted = 1001,
    /// <summary>New key created.</summary>
    New = 1002,
    /// <summary>Key overwritten.</summary>
    Overwritten = 1003,
    /// <summary>Key type changed.</summary>
    TypeChanged = 1004,
}
