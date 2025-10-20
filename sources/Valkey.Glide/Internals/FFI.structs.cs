﻿// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Buffers;
using System.Runtime.InteropServices;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Route;

namespace Valkey.Glide.Internals;

// FFI-ready structs, helper methods and wrappers
internal partial class FFI
{
    internal abstract class Marshallable : IDisposable
    {
        private IntPtr _ptr = IntPtr.Zero;

        public IntPtr ToPtr()
        {
            if (_ptr == IntPtr.Zero)
            {
                _ptr = AllocateAndCopy();
            }
            return _ptr;
        }

        public void Dispose()
        {
            if (_ptr != IntPtr.Zero)
            {
                FreeMemory();
                FreeStructPtr(_ptr);
                _ptr = IntPtr.Zero;
            }
        }

        // All unmanaged memory allocations should happen only on this call and never before.
        protected abstract IntPtr AllocateAndCopy();

        protected abstract void FreeMemory();
    }

    // A wrapper for a command, resposible for marshalling (allocating and freeing) the required data
    internal class Cmd : Marshallable
    {
        private IntPtr[] _argPtrs = [];
        private GCHandle _pinnedArgs;
        private nuint[] _lengths = [];
        private GCHandle _pinnedLengths;
        private readonly GlideString[] _args;
        private CmdInfo _cmd;

        public Cmd(RequestType requestType, GlideString[] arguments)
        {
            _cmd = new() { RequestType = requestType, ArgCount = (nuint)arguments.Length };
            _args = arguments;
        }

        protected override void FreeMemory()
        {
            for (nuint i = 0; i < _cmd.ArgCount; i++)
            {
                Marshal.FreeHGlobal(_argPtrs[i]);
            }
            _pinnedArgs.Free();
            PoolReturn(_argPtrs);
            _pinnedLengths.Free();
            PoolReturn(_lengths);
        }

        protected override IntPtr AllocateAndCopy()
        {
            // 1. Allocate memory for arguments and for for arguments' lenghts
            _argPtrs = PoolRent<IntPtr>(_args.Length);
            _lengths = PoolRent<nuint>(_args.Length);

            // 2. Copy data into allocated array in unmanaged memory
            for (int i = 0; i < _args.Length; i++)
            {
                // 2.1 Copy an argument
                _argPtrs[i] = Marshal.AllocHGlobal(_args[i].Length);
                Marshal.Copy(_args[i].Bytes, 0, _argPtrs[i], _args[i].Length);
                // 2.2 Copy arg's len
                _lengths[i] = (nuint)_args[i].Length;
            }

            // 3. Pin it
            // We need to pin the array in place, in order to ensure that the GC doesn't move it while the operation is running.
            _pinnedArgs = GCHandle.Alloc(_argPtrs, GCHandleType.Pinned);
            _cmd.Args = _pinnedArgs.AddrOfPinnedObject();
            _pinnedLengths = GCHandle.Alloc(_lengths, GCHandleType.Pinned);
            _cmd.ArgLengths = _pinnedLengths.AddrOfPinnedObject();

            return StructToPtr(_cmd);
        }
    }

    internal class Batch : Marshallable
    {
        private readonly Cmd[] _cmds;
        private IntPtr[] _cmdPtrs;
        private GCHandle _pinnedCmds;
        private BatchInfo _batch;

        public Batch(Cmd[] cmds, bool isAtomic)
        {
            _cmds = cmds;
            _batch = new() { IsAtomic = isAtomic, CmdCount = (nuint)cmds.Length };
            _cmdPtrs = [];
        }

        protected override void FreeMemory()
        {
            for (int i = 0; i < _cmds.Length; i++)
            {
                _cmds[i].Dispose();
            }
            _pinnedCmds.Free();
            ArrayPool<IntPtr>.Shared.Return(_cmdPtrs);
        }

        protected override IntPtr AllocateAndCopy()
        {
            // 1. Allocate memory for commands and marshal them
            _cmdPtrs = ArrayPool<IntPtr>.Shared.Rent(_cmds.Length);
            for (int i = 0; i < _cmds.Length; i++)
            {
                _cmdPtrs[i] = _cmds[i].ToPtr();
            }

            // 2. Pin it
            _pinnedCmds = GCHandle.Alloc(_cmdPtrs, GCHandleType.Pinned);
            _batch.Cmds = _pinnedCmds.AddrOfPinnedObject();

            return StructToPtr(_batch);
        }
    }

    // A wrapper for a route
    internal class Route : Marshallable
    {
        private readonly RouteInfo _info;

        public Route(
            RouteType requestType,
            (int slotId, SlotType slotType)? slotIdInfo = null,
            (string slotKey, SlotType slotType)? slotKeyInfo = null,
            (string host, int port)? address = null)
        {
            _info = new()
            {
                Type = requestType,
                SlotId = slotIdInfo?.slotId ?? 0,
                SlotKey = slotKeyInfo?.slotKey,
                SlotType = slotIdInfo?.slotType ?? slotKeyInfo?.slotType ?? 0,
                Host = address?.host,
                Port = address?.port ?? 0,
            };
        }

        protected override void FreeMemory() { }

        protected override IntPtr AllocateAndCopy() => StructToPtr(_info);
    }

    internal class BatchOptions : Marshallable
    {
        private BatchOptionsInfo _info;
        private readonly Route? _route;

        public BatchOptions(
            bool? retryServerError = false,
            bool? retryConnectionError = false,
            uint? timeout = null,
            Route? route = null
            )
        {
            _route = route;
            _info = new()
            {
                RetryServerError = retryServerError ?? false,
                RetryConnectionError = retryConnectionError ?? false,
                HasTimeout = timeout is not null,
                Timeout = timeout ?? 0,
                Route = IntPtr.Zero,
            };
        }

        protected override void FreeMemory() => _route?.Dispose();

        protected override IntPtr AllocateAndCopy()
        {
            _info.Route = _route?.ToPtr() ?? IntPtr.Zero;
            return StructToPtr(_info);
        }
    }

    // A wrapper for connection request
    internal class ConnectionConfig : Marshallable
    {
        private ConnectionRequest _request;
        private readonly List<NodeAddress> _addresses;
        private readonly BasePubSubSubscriptionConfig? _pubSubConfig;
        private IntPtr _pubSubChannelsPtr = IntPtr.Zero;
        private IntPtr _pubSubPatternsPtr = IntPtr.Zero;
        private IntPtr _pubSubShardedChannelsPtr = IntPtr.Zero;

        public ConnectionConfig(
            List<NodeAddress> addresses,
            TlsMode? tlsMode,
            bool clusterMode,
            uint? requestTimeout,
            uint? connectionTimeout,
            ReadFrom? readFrom,
            RetryStrategy? retryStrategy,
            AuthenticationInfo? authenticationInfo,
            uint databaseId,
            ConnectionConfiguration.Protocol? protocol,
            string? clientName,
            BasePubSubSubscriptionConfig? pubSubSubscriptions)
        {
            _addresses = addresses;
            _pubSubConfig = pubSubSubscriptions;
            _request = new()
            {
                AddressCount = (nuint)addresses.Count,
                HasTlsMode = tlsMode.HasValue,
                TlsMode = tlsMode ?? default,
                ClusterMode = clusterMode,
                HasRequestTimeout = requestTimeout.HasValue,
                RequestTimeout = requestTimeout ?? default,
                HasConnectionTimeout = connectionTimeout.HasValue,
                ConnectionTimeout = connectionTimeout ?? default,
                HasReadFrom = readFrom.HasValue,
                ReadFrom = readFrom ?? default,
                HasConnectionRetryStrategy = retryStrategy.HasValue,
                ConnectionRetryStrategy = retryStrategy ?? default,
                HasAuthenticationInfo = authenticationInfo.HasValue,
                AuthenticationInfo = authenticationInfo ?? default,
                DatabaseId = databaseId,
                HasProtocol = protocol.HasValue,
                Protocol = protocol ?? default,
                ClientName = clientName,
                HasPubSubConfig = pubSubSubscriptions != null,
                PubSubConfig = new PubSubConfigInfo()
            };
        }

        protected override void FreeMemory()
        {
            Marshal.FreeHGlobal(_request.Addresses);

            if (_pubSubConfig != null)
            {
                int channelCount = _pubSubConfig.Subscriptions.TryGetValue(0, out List<string>? channels) ? channels.Count : 0;
                int patternCount = _pubSubConfig.Subscriptions.TryGetValue(1, out List<string>? patterns) ? patterns.Count : 0;
                int shardedChannelCount = _pubSubConfig.Subscriptions.TryGetValue(2, out List<string>? shardedChannels) ? shardedChannels.Count : 0;

                FreeStringArray(_pubSubChannelsPtr, channelCount);
                FreeStringArray(_pubSubPatternsPtr, patternCount);
                FreeStringArray(_pubSubShardedChannelsPtr, shardedChannelCount);
            }
        }

        private static void FreeStringArray(IntPtr arrayPtr, int count)
        {
            if (arrayPtr == IntPtr.Zero || count == 0)
            {
                return;
            }

            // Free each string in the array
            for (int i = 0; i < count; i++)
            {
                IntPtr stringPtr = Marshal.ReadIntPtr(arrayPtr, i * IntPtr.Size);
                if (stringPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(stringPtr);
                }
            }

            // Free the array itself
            Marshal.FreeHGlobal(arrayPtr);
        }

        protected override IntPtr AllocateAndCopy()
        {
            int addressSize = Marshal.SizeOf(typeof(NodeAddress));
            _request.Addresses = Marshal.AllocHGlobal(addressSize * (int)_request.AddressCount);
            for (int i = 0; i < (int)_request.AddressCount; i++)
            {
                Marshal.StructureToPtr(_addresses[i], _request.Addresses + (i * addressSize), false);
            }

            // Marshal PubSub configuration if present
            if (_pubSubConfig != null)
            {
                _request.PubSubConfig = MarshalPubSubConfig(_pubSubConfig);
            }

            return StructToPtr(_request);
        }



        private PubSubConfigInfo MarshalPubSubConfig(BasePubSubSubscriptionConfig config)
        {
            var pubSubInfo = new PubSubConfigInfo();

            // Marshal exact channels (mode 0)
            if (config.Subscriptions.TryGetValue(0, out List<string>? channels) && channels.Count > 0)
            {
                _pubSubChannelsPtr = MarshalStringArray(channels);
                pubSubInfo.ChannelsPtr = _pubSubChannelsPtr;
                pubSubInfo.ChannelCount = (uint)channels.Count;
            }

            // Marshal patterns (mode 1)
            if (config.Subscriptions.TryGetValue(1, out List<string>? patterns) && patterns.Count > 0)
            {
                _pubSubPatternsPtr = MarshalStringArray(patterns);
                pubSubInfo.PatternsPtr = _pubSubPatternsPtr;
                pubSubInfo.PatternCount = (uint)patterns.Count;
            }

            // Marshal sharded channels (mode 2) - only for cluster clients
            if (config.Subscriptions.TryGetValue(2, out List<string>? shardedChannels) && shardedChannels.Count > 0)
            {
                _pubSubShardedChannelsPtr = MarshalStringArray(shardedChannels);
                pubSubInfo.ShardedChannelsPtr = _pubSubShardedChannelsPtr;
                pubSubInfo.ShardedChannelCount = (uint)shardedChannels.Count;
            }

            return pubSubInfo;
        }

        private static IntPtr MarshalStringArray(List<string> strings)
        {
            if (strings.Count == 0)
            {
                return IntPtr.Zero;
            }

            // Allocate array of string pointers
            IntPtr arrayPtr = Marshal.AllocHGlobal(IntPtr.Size * strings.Count);

            for (int i = 0; i < strings.Count; i++)
            {
                // Allocate and copy each string
                IntPtr stringPtr = Marshal.StringToHGlobalAnsi(strings[i]);
                Marshal.WriteIntPtr(arrayPtr, i * IntPtr.Size, stringPtr);
            }

            return arrayPtr;
        }
    }

    private static IntPtr StructToPtr<T>(T @struct) where T : struct
    {
        IntPtr result = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
        Marshal.StructureToPtr(@struct, result, false);
        return result;
    }

    private static void FreeStructPtr(IntPtr ptr) => Marshal.FreeHGlobal(ptr);

    private static T[] PoolRent<T>(int len) => ArrayPool<T>.Shared.Rent(len);

    private static void PoolReturn<T>(T[] arr) => ArrayPool<T>.Shared.Return(arr);

    /// <summary>
    /// Marshals raw byte arrays from FFI callback parameters to a managed PubSubMessage object.
    /// </summary>
    /// <param name="pushKind">The type of push notification.</param>
    /// <param name="messagePtr">Pointer to the raw message bytes.</param>
    /// <param name="messageLen">The length of the message data in bytes.</param>
    /// <param name="channelPtr">Pointer to the raw channel name bytes.</param>
    /// <param name="channelLen">The length of the channel name in bytes.</param>
    /// <param name="patternPtr">Pointer to the raw pattern bytes (null if no pattern).</param>
    /// <param name="patternLen">The length of the pattern in bytes (0 if no pattern).</param>
    /// <returns>A managed PubSubMessage object.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameters are invalid or marshaling fails.</exception>
    internal static PubSubMessage MarshalPubSubMessage(
        PushKind pushKind,
        IntPtr messagePtr,
        long messageLen,
        IntPtr channelPtr,
        long channelLen,
        IntPtr patternPtr,
        long patternLen)
    {
        try
        {
            // Validate input parameters
            if (messagePtr == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid message data: pointer is null");
            }

            if (channelPtr == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid channel data: pointer is null");
            }

            if (messageLen < 0)
            {
                throw new ArgumentException("Invalid message data: length cannot be negative");
            }

            if (channelLen <= 0)
            {
                throw new ArgumentException("Invalid channel data: pointer is null or length is zero");
            }

            // Marshal message bytes to string
            byte[] messageBytes = new byte[messageLen];
            if (messageLen > 0)
            {
                Marshal.Copy(messagePtr, messageBytes, 0, (int)messageLen);
            }
            string message = System.Text.Encoding.UTF8.GetString(messageBytes);

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("PubSub message content cannot be null or empty after marshaling");
            }

            // Marshal channel bytes to string
            byte[] channelBytes = new byte[channelLen];
            Marshal.Copy(channelPtr, channelBytes, 0, (int)channelLen);
            string channel = System.Text.Encoding.UTF8.GetString(channelBytes);

            if (string.IsNullOrEmpty(channel))
            {
                throw new ArgumentException("PubSub channel name cannot be null or empty after marshaling");
            }

            // Marshal pattern bytes to string if present
            string? pattern = null;
            if (patternPtr != IntPtr.Zero && patternLen > 0)
            {
                byte[] patternBytes = new byte[patternLen];
                Marshal.Copy(patternPtr, patternBytes, 0, (int)patternLen);
                pattern = System.Text.Encoding.UTF8.GetString(patternBytes);

                if (string.IsNullOrEmpty(pattern))
                {
                    throw new ArgumentException("PubSub pattern cannot be empty when pattern pointer is provided");
                }
            }

            // Create PubSubMessage based on whether pattern is present
            return pattern == null
                ? new PubSubMessage(message, channel)
                : new PubSubMessage(message, channel, pattern);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException($"Failed to marshal PubSub message from FFI callback parameters: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates a function pointer for the PubSub message callback that can be passed to native code.
    /// </summary>
    /// <param name="callback">The managed callback delegate.</param>
    /// <returns>A function pointer that can be passed to native code.</returns>
    internal static IntPtr CreatePubSubCallbackPtr(PubSubMessageCallback callback)
    {
        return Marshal.GetFunctionPointerForDelegate(callback);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CmdInfo
    {
        public RequestType RequestType;
        public IntPtr Args;
        public nuint ArgCount;
        public IntPtr ArgLengths;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BatchInfo
    {
        public nuint CmdCount;
        public IntPtr Cmds;
        [MarshalAs(UnmanagedType.U1)]
        public bool IsAtomic;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BatchOptionsInfo
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool RetryServerError;
        [MarshalAs(UnmanagedType.U1)]
        public bool RetryConnectionError;
        [MarshalAs(UnmanagedType.U1)]
        public bool HasTimeout;
        public uint Timeout;
        public IntPtr Route;
    }

    // TODO: generate this with a bindings generator
    internal enum RequestType : int
    {
        /// Invalid request type
        InvalidRequest = 0,
        /// An unknown command, where all arguments are defined by the user.
        CustomCommand = 1,

        //// Bitmap commands
        BitCount = 101,
        BitField = 102,
        BitFieldReadOnly = 103,
        BitOp = 104,
        BitPos = 105,
        GetBit = 106,
        SetBit = 107,

        //// Cluster commands
        Asking = 201,
        ClusterAddSlots = 202,
        ClusterAddSlotsRange = 203,
        ClusterBumpEpoch = 204,
        ClusterCountFailureReports = 205,
        ClusterCountKeysInSlot = 206,
        ClusterDelSlots = 207,
        ClusterDelSlotsRange = 208,
        ClusterFailover = 209,
        ClusterFlushSlots = 210,
        ClusterForget = 211,
        ClusterGetKeysInSlot = 212,
        ClusterInfo = 213,
        ClusterKeySlot = 214,
        ClusterLinks = 215,
        ClusterMeet = 216,
        ClusterMyId = 217,
        ClusterMyShardId = 218,
        ClusterNodes = 219,
        ClusterReplicas = 220,
        ClusterReplicate = 221,
        ClusterReset = 222,
        ClusterSaveConfig = 223,
        ClusterSetConfigEpoch = 224,
        ClusterSetslot = 225,
        ClusterShards = 226,
        ClusterSlaves = 227,
        ClusterSlots = 228,
        ReadOnly = 229,
        ReadWrite = 230,

        //// Connection Management commands
        Auth = 301,
        ClientCaching = 302,
        ClientGetName = 303,
        ClientGetRedir = 304,
        ClientId = 305,
        ClientInfo = 306,
        ClientKillSimple = 307,
        ClientKill = 308,
        ClientList = 309,
        ClientNoEvict = 310,
        ClientNoTouch = 311,
        ClientPause = 312,
        ClientReply = 313,
        ClientSetInfo = 314,
        ClientSetName = 315,
        ClientTracking = 316,
        ClientTrackingInfo = 317,
        ClientUnblock = 318,
        ClientUnpause = 319,
        Echo = 320,
        Hello = 321,
        Ping = 322,
        Quit = 323, // deprecated in 7.2.0
        Reset = 324,
        Select = 325,

        //// Generic commands
        Copy = 401,
        Del = 402,
        Dump = 403,
        Exists = 404,
        Expire = 405,
        ExpireAt = 406,
        ExpireTime = 407,
        Keys = 408,
        Migrate = 409,
        Move = 410,
        ObjectEncoding = 411,
        ObjectFreq = 412,
        ObjectIdleTime = 413,
        ObjectRefCount = 414,
        Persist = 415,
        PExpire = 416,
        PExpireAt = 417,
        PExpireTime = 418,
        PTTL = 419,
        RandomKey = 420,
        Rename = 421,
        RenameNX = 422,
        Restore = 423,
        Scan = 424,
        Sort = 425,
        SortReadOnly = 426,
        Touch = 427,
        TTL = 428,
        Type = 429,
        Unlink = 430,
        Wait = 431,
        WaitAof = 432,

        //// Geospatial indices commands
        GeoAdd = 501,
        GeoDist = 502,
        GeoHash = 503,
        GeoPos = 504,
        GeoRadius = 505,
        GeoRadiusReadOnly = 506, // deprecated in 6.2.0
        GeoRadiusByMember = 507,
        GeoRadiusByMemberReadOnly = 508, // deprecated in 6.2.0
        GeoSearch = 509,
        GeoSearchStore = 510,

        //// Hash commands
        HDel = 601,
        HExists = 602,
        HGet = 603,
        HGetAll = 604,
        HIncrBy = 605,
        HIncrByFloat = 606,
        HKeys = 607,
        HLen = 608,
        HMGet = 609,
        HMSet = 610,
        HRandField = 611,
        HScan = 612,
        HSet = 613,
        HSetNX = 614,
        HStrlen = 615,
        HVals = 616,

        //// HyperLogLog commands
        PfAdd = 701,
        PfCount = 702,
        PfMerge = 703,

        //// List commands
        BLMove = 801,
        BLMPop = 802,
        BLPop = 803,
        BRPop = 804,
        BRPopLPush = 805, // deprecated in 6.2.0
        LIndex = 806,
        LInsert = 807,
        LLen = 808,
        LMove = 809,
        LMPop = 810,
        LPop = 811,
        LPos = 812,
        LPush = 813,
        LPushX = 814,
        LRange = 815,
        LRem = 816,
        LSet = 817,
        LTrim = 818,
        RPop = 819,
        RPopLPush = 820, // deprecated in 6.2.0
        RPush = 821,
        RPushX = 822,

        //// Pub/Sub commands
        PSubscribe = 901,
        Publish = 902,
        PubSubChannels = 903,
        PubSubNumPat = 904,
        PubSubNumSub = 905,
        PubSubShardChannels = 906,
        PubSubShardNumSub = 907,
        PUnsubscribe = 908,
        SPublish = 909,
        SSubscribe = 910,
        Subscribe = 911,
        SUnsubscribe = 912,
        Unsubscribe = 913,

        //// Scripting and Functions commands
        Eval = 1001,
        EvalReadOnly = 1002,
        EvalSha = 1003,
        EvalShaReadOnly = 1004,
        FCall = 1005,
        FCallReadOnly = 1006,
        FunctionDelete = 1007,
        FunctionDump = 1008,
        FunctionFlush = 1009,
        FunctionKill = 1010,
        FunctionList = 1011,
        FunctionLoad = 1012,
        FunctionRestore = 1013,
        FunctionStats = 1014,
        ScriptDebug = 1015,
        ScriptExists = 1016,
        ScriptFlush = 1017,
        ScriptKill = 1018,
        ScriptLoad = 1019,
        ScriptShow = 1020,

        //// Server management commands
        AclCat = 1101,
        AclDelUser = 1102,
        AclDryRun = 1103,
        AclGenPass = 1104,
        AclGetUser = 1105,
        AclList = 1106,
        AclLoad = 1107,
        AclLog = 1108,
        AclSave = 1109,
        AclSetSser = 1110,
        AclUsers = 1111,
        AclWhoami = 1112,
        BgRewriteAof = 1113,
        BgSave = 1114,
        Command_ = 1115, // Command - renamed to avoid collisions
        CommandCount = 1116,
        CommandDocs = 1117,
        CommandGetKeys = 1118,
        CommandGetKeysAndFlags = 1119,
        CommandInfo = 1120,
        CommandList = 1121,
        ConfigGet = 1122,
        ConfigResetStat = 1123,
        ConfigRewrite = 1124,
        ConfigSet = 1125,
        DBSize = 1126,
        FailOver = 1127,
        FlushAll = 1128,
        FlushDB = 1129,
        Info = 1130,
        LastSave = 1131,
        LatencyDoctor = 1132,
        LatencyGraph = 1133,
        LatencyHistogram = 1134,
        LatencyHistory = 1135,
        LatencyLatest = 1136,
        LatencyReset = 1137,
        Lolwut = 1138,
        MemoryDoctor = 1139,
        MemoryMallocStats = 1140,
        MemoryPurge = 1141,
        MemoryStats = 1142,
        MemoryUsage = 1143,
        ModuleList = 1144,
        ModuleLoad = 1145,
        ModuleLoadEx = 1146,
        ModuleUnload = 1147,
        Monitor = 1148,
        PSync = 1149,
        ReplConf = 1150,
        ReplicaOf = 1151,
        RestoreAsking = 1152,
        Role = 1153,
        Save = 1154,
        ShutDown = 1155,
        SlaveOf = 1156,
        SlowLogGet = 1157,
        SlowLogLen = 1158,
        SlowLogReset = 1159,
        SwapDb = 1160,
        Sync = 1161,
        Time = 1162,

        //// Set commands
        SAdd = 1201,
        SCard = 1202,
        SDiff = 1203,
        SDiffStore = 1204,
        SInter = 1205,
        SInterCard = 1206,
        SInterStore = 1207,
        SIsMember = 1208,
        SMembers = 1209,
        SMIsMember = 1210,
        SMove = 1211,
        SPop = 1212,
        SRandMember = 1213,
        SRem = 1214,
        SScan = 1215,
        SUnion = 1216,
        SUnionStore = 1217,

        //// Sorted set commands
        BZMPop = 1301,
        BZPopMax = 1302,
        BZPopMin = 1303,
        ZAdd = 1304,
        ZCard = 1305,
        ZCount = 1306,
        ZDiff = 1307,
        ZDiffStore = 1308,
        ZIncrBy = 1309,
        ZInter = 1310,
        ZInterCard = 1311,
        ZInterStore = 1312,
        ZLexCount = 1313,
        ZMPop = 1314,
        ZMScore = 1315,
        ZPopMax = 1316,
        ZPopMin = 1317,
        ZRandMember = 1318,
        ZRange = 1319,
        ZRangeByLex = 1320,
        ZRangeByScore = 1321,
        ZRangeStore = 1322,
        ZRank = 1323,
        ZRem = 1324,
        ZRemRangeByLex = 1325,
        ZRemRangeByRank = 1326,
        ZRemRangeByScore = 1327,
        ZRevRange = 1328,
        ZRevRangeByLex = 1329,
        ZRevRangeByScore = 1330,
        ZRevRank = 1331,
        ZScan = 1332,
        ZScore = 1333,
        ZUnion = 1334,
        ZUnionStore = 1335,

        //// Stream commands
        XAck = 1401,
        XAdd = 1402,
        XAutoClaim = 1403,
        XClaim = 1404,
        XDel = 1405,
        XGroupCreate = 1406,
        XGroupCreateConsumer = 1407,
        XGroupDelConsumer = 1408,
        XGroupDestroy = 1409,
        XGroupSetId = 1410,
        XInfoConsumers = 1411,
        XInfoGroups = 1412,
        XInfoStream = 1413,
        XLen = 1414,
        XPending = 1415,
        XRange = 1416,
        XRead = 1417,
        XReadGroup = 1418,
        XRevRange = 1419,
        XSetId = 1420,
        XTrim = 1421,

        //// String commands
        Append = 1501,
        Decr = 1502,
        DecrBy = 1503,
        Get = 1504,
        GetDel = 1505,
        GetEx = 1506,
        GetRange = 1507,
        GetSet = 1508, // deprecated in 6.2.0
        Incr = 1509,
        IncrBy = 1510,
        IncrByFloat = 1511,
        LCS = 1512,
        MGet = 1513,
        MSet = 1514,
        MSetNX = 1515,
        PSetEx = 1516, // deprecated in 2.6.12
        Set = 1517,
        SetEx = 1518, // deprecated in 2.6.12
        SetNX = 1519, // deprecated in 2.6.12
        SetRange = 1520,
        Strlen = 1521,
        Substr = 1522,

        //// Transaction commands
        Discard = 1601,
        Exec = 1602,
        Multi = 1603,
        UnWatch = 1604,
        Watch = 1605,

        //// JSON commands
        JsonArrAppend = 2001,
        JsonArrIndex = 2002,
        JsonArrInsert = 2003,
        JsonArrLen = 2004,
        JsonArrPop = 2005,
        JsonArrTrim = 2006,
        JsonClear = 2007,
        JsonDebug = 2008,
        JsonDel = 2009,
        JsonForget = 2010,
        JsonGet = 2011,
        JsonMGet = 2012,
        JsonNumIncrBy = 2013,
        JsonNumMultBy = 2014,
        JsonObjKeys = 2015,
        JsonObjLen = 2016,
        JsonResp = 2017,
        JsonSet = 2018,
        JsonStrAppend = 2019,
        JsonStrLen = 2020,
        JsonToggle = 2021,
        JsonType = 2022,

        //// Vector Search commands
        FtList = 2101,
        FtAggregate = 2102,
        FtAliasAdd = 2103,
        FtAliasDel = 2104,
        FtAliasList = 2105,
        FtAliasUpdate = 2106,
        FtCreate = 2107,
        FtDropIndex = 2108,
        FtExplain = 2109,
        FtExplainCli = 2110,
        FtInfo = 2111,
        FtProfile = 2112,
        FtSearch = 2113,
    }

    internal enum RouteType : uint
    {
        Random,
        AllNodes,
        AllPrimaries,
        SlotId,
        SlotKey,
        ByAddress,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private struct RouteInfo
    {
        public RouteType Type;
        public int SlotId;
        [MarshalAs(UnmanagedType.LPStr)]
        public string? SlotKey;
        public SlotType SlotType;
        [MarshalAs(UnmanagedType.LPStr)]
        public string? Host;
        public int Port;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private struct ConnectionRequest
    {
        public nuint AddressCount;
        public IntPtr Addresses; // ** NodeAddress - array pointer
        [MarshalAs(UnmanagedType.U1)]
        public bool HasTlsMode;
        public TlsMode TlsMode;
        [MarshalAs(UnmanagedType.U1)]
        public bool ClusterMode;
        [MarshalAs(UnmanagedType.U1)]
        public bool HasRequestTimeout;
        public uint RequestTimeout;
        [MarshalAs(UnmanagedType.U1)]
        public bool HasConnectionTimeout;
        public uint ConnectionTimeout;
        [MarshalAs(UnmanagedType.U1)]
        public bool HasReadFrom;
        public ReadFrom ReadFrom;
        [MarshalAs(UnmanagedType.U1)]
        public bool HasConnectionRetryStrategy;
        public RetryStrategy ConnectionRetryStrategy;
        [MarshalAs(UnmanagedType.U1)]
        public bool HasAuthenticationInfo;
        public AuthenticationInfo AuthenticationInfo;
        public uint DatabaseId;
        [MarshalAs(UnmanagedType.U1)]
        public bool HasProtocol;
        public ConnectionConfiguration.Protocol Protocol;
        [MarshalAs(UnmanagedType.LPStr)]
        public string? ClientName;
        [MarshalAs(UnmanagedType.U1)]
        public bool HasPubSubConfig;
        public PubSubConfigInfo PubSubConfig;
        // TODO more config params, see ffi.rs
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PubSubConfigInfo
    {
        public IntPtr ChannelsPtr;
        public uint ChannelCount;
        public IntPtr PatternsPtr;
        public uint PatternCount;
        public IntPtr ShardedChannelsPtr;
        public uint ShardedChannelCount;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct NodeAddress
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Host;
        public ushort Port;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct AuthenticationInfo(string? username, string password)
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string? Username = username;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Password = password;
    }

    /// <summary>
    /// FFI structure for PubSub message data received from native code.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PubSubMessageInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string Message;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Channel;
        [MarshalAs(UnmanagedType.LPStr)]
        public string? Pattern;
    }

    internal enum TlsMode : uint
    {
        NoTls = 0,
        SecureTls = 2,
    }

    /// <summary>
    /// Enum representing the type of push notification received from the server.
    /// This matches the PushKind enum in the Rust FFI layer.
    /// </summary>
    internal enum PushKind
    {
        /// <summary>Disconnection notification.</summary>
        PushDisconnection = 0,
        /// <summary>Other/unknown push notification type.</summary>
        PushOther = 1,
        /// <summary>Cache invalidation notification.</summary>
        PushInvalidate = 2,
        /// <summary>Regular channel message (SUBSCRIBE).</summary>
        PushMessage = 3,
        /// <summary>Pattern-based message (PSUBSCRIBE).</summary>
        PushPMessage = 4,
        /// <summary>Sharded channel message (SSUBSCRIBE).</summary>
        PushSMessage = 5,
        /// <summary>Unsubscribe confirmation.</summary>
        PushUnsubscribe = 6,
        /// <summary>Pattern unsubscribe confirmation.</summary>
        PushPUnsubscribe = 7,
        /// <summary>Sharded unsubscribe confirmation.</summary>
        PushSUnsubscribe = 8,
        /// <summary>Subscribe confirmation.</summary>
        PushSubscribe = 9,
        /// <summary>Pattern subscribe confirmation.</summary>
        PushPSubscribe = 10,
        /// <summary>Sharded subscribe confirmation.</summary>
        PushSSubscribe = 11,
    }


}
