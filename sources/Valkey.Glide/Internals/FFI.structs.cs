// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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

        /// <summary>
        /// The node discovery mode marshalled into the underlying FFI request. Exposed for testing
        /// that the value is correctly wired through to the FFI layer.
        /// </summary>
        internal NodeDiscoveryMode NodeDiscoveryMode => _request.NodeDiscoveryMode;

        public ConnectionConfig(
            List<NodeAddress> addresses,
            bool clusterMode,
            uint? requestTimeout,
            uint? connectionTimeout,
            ReadFrom? readFrom,
            RetryStrategy? retryStrategy,
            AuthenticationInfo? authenticationInfo,
            uint databaseId,
            ConnectionConfiguration.Protocol? protocol,
            string? clientName,
            bool lazyConnect,
            bool refreshTopologyFromInitialNodes,
            BasePubSubSubscriptionConfig? pubSubSubscriptions,
            uint? pubSubReconciliationIntervalMs,
            CompressionConfig? compressionConfig,
            bool readOnly,
            NodeDiscoveryMode nodeDiscoveryMode,
            ClientSideCacheConfig? clientSideCacheConfig,
            CircuitBreakerConfig? circuitBreakerConfig,

            // TLS
            TlsMode tlsMode,
            List<byte[]> rootCertificates,

            // Mutual TLS
            byte[]? clientCertificate,
            byte[]? clientKey,
            string? clientCertificatePath,
            string? clientKeyPath,
            bool certReloadEnabled,
            uint? certReloadIntervalSeconds,

            // Inflight requests limit
            uint? inflightRequestsLimit,

            // Periodic checks
            PeriodicChecksMode? periodicChecksMode,
            uint? periodicChecksIntervalSec)
        {
            _request = new()
            {
                AddressCount = (nuint)addresses.Count,
                Addresses = MarshallAddress(addresses),
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
                LazyConnect = lazyConnect,
                RefreshTopologyFromInitialNodes = refreshTopologyFromInitialNodes,
                PubSubConfig = MarshalPubSubConfig(pubSubSubscriptions),
                HasPubSubReconciliationIntervalMs = pubSubReconciliationIntervalMs.HasValue,
                PubSubReconciliationIntervalMs = pubSubReconciliationIntervalMs ?? default,
                HasCompressionConfig = compressionConfig.HasValue,
                CompressionConfig = compressionConfig ?? default,
                ReadOnly = readOnly,
                NodeDiscoveryMode = nodeDiscoveryMode,
                HasClientSideCacheConfig = clientSideCacheConfig.HasValue,
                ClientSideCacheConfig = clientSideCacheConfig ?? default,

                // Circuit breaker configuration
                HasCircuitBreakerConfig = circuitBreakerConfig.HasValue,
                CircuitBreakerConfig = circuitBreakerConfig ?? default,

                // TLS configuration
                TlsMode = tlsMode,
                RootCertsCount = (nuint)rootCertificates.Count,
                RootCertsPtr = MarshallRootCertificates(rootCertificates),
                RootCertsLensPtr = MarshallRootCertificatesLengths(rootCertificates),

                // Mutual TLS configuration
                ClientCertLen = (nuint)(clientCertificate?.Length ?? 0),
                ClientCertPtr = MarshalBytes(clientCertificate),
                ClientKeyLen = (nuint)(clientKey?.Length ?? 0),
                ClientKeyPtr = MarshalBytes(clientKey),
                ClientCertPath = clientCertificatePath,
                ClientKeyPath = clientKeyPath,
                CertReloadEnabled = certReloadEnabled,
                HasCertReloadIntervalSeconds = certReloadIntervalSeconds.HasValue,
                CertReloadIntervalSeconds = certReloadIntervalSeconds ?? 0,

                // Inflight requests limit
                HasInflightRequestsLimit = inflightRequestsLimit.HasValue,
                InflightRequestsLimit = inflightRequestsLimit ?? default,

                // Periodic checks configuration
                HasPeriodicChecksConfig = periodicChecksMode.HasValue,
                PeriodicChecksMode = periodicChecksMode ?? default,
                PeriodicChecksIntervalSec = periodicChecksIntervalSec ?? 0,
            };
        }

        protected override void FreeMemory()
        {
            // Free addresses.
            if (_request.AddressCount > 0)
            {
                Marshal.FreeHGlobal(_request.Addresses);
            }

            // Free PubSub configuration
            var pubSubConfig = _request.PubSubConfig;
            FreeStringArray(pubSubConfig.ChannelsPtr, pubSubConfig.ChannelCount);
            FreeStringArray(pubSubConfig.PatternsPtr, pubSubConfig.PatternCount);
            FreeStringArray(pubSubConfig.ShardedChannelsPtr, pubSubConfig.ShardedChannelCount);

            // Free TLS root certificates.
            if (_request.RootCertsCount > 0)
            {
                for (int i = 0; i < (int)_request.RootCertsCount; i++)
                {
                    IntPtr certPtr = Marshal.ReadIntPtr(_request.RootCertsPtr, i * IntPtr.Size);
                    Marshal.FreeHGlobal(certPtr);
                }

                Marshal.FreeHGlobal(_request.RootCertsPtr);
                Marshal.FreeHGlobal(_request.RootCertsLensPtr);
            }

            // Free mutual TLS certificate and key.
            if (_request.ClientCertPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_request.ClientCertPtr);
            }

            if (_request.ClientKeyPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_request.ClientKeyPtr);
            }
        }

        /// <summary>
        /// Frees an array of strings allocated in unmanaged memory.
        /// </summary>
        /// <param name="arrayPtr">Pointer to the array of string pointers.</param>
        /// <param name="count">Number of strings in the array.</param>
        private static void FreeStringArray(IntPtr arrayPtr, uint count)
        {
            if (arrayPtr == IntPtr.Zero)
            {
                return;
            }

            // Free each string in the array
            for (int i = 0; i < count; i++)
            {
                IntPtr stringPtr = Marshal.ReadIntPtr(arrayPtr, i * IntPtr.Size);
                Marshal.FreeHGlobal(stringPtr);
            }

            // Free the array itself
            Marshal.FreeHGlobal(arrayPtr);
        }

        protected override IntPtr AllocateAndCopy()
            => StructToPtr(_request);

        /// <summary>
        /// Marshals the node addresses.
        /// </summary>
        /// <param name="addresses">List of node addresses.</param>
        /// <returns>Pointer to an array of NodeAddress structs.</returns>
        private IntPtr MarshallAddress(List<NodeAddress> addresses)
        {
            if (addresses.Count == 0)
            {
                return IntPtr.Zero;
            }

            // Allocate memory for addresses.
            int addressSize = Marshal.SizeOf(typeof(NodeAddress));
            IntPtr addressesPtr = Marshal.AllocHGlobal(addressSize * addresses.Count);

            // Copy addresses to allocated memory.
            for (int i = 0; i < addresses.Count; i++)
            {
                Marshal.StructureToPtr(addresses[i], addressesPtr + (i * addressSize), false);
            }

            return addressesPtr;
        }

        /// <summary>
        /// Marshals the root certificates.
        /// </summary>
        /// <param name="rootCerts">Root certificate byte arrays.</param>
        /// <returns>Pointer to an array of root certificate pointers.</returns>
        private IntPtr MarshallRootCertificates(List<byte[]> rootCerts)
        {
            if (rootCerts.Count == 0)
            {
                return IntPtr.Zero;
            }

            IntPtr certsPtr = Marshal.AllocHGlobal(IntPtr.Size * rootCerts.Count);

            for (int i = 0; i < rootCerts.Count; i++)
            {
                Marshal.WriteIntPtr(certsPtr, i * IntPtr.Size, MarshalBytes(rootCerts[i]));
            }

            return certsPtr;
        }

        /// <summary>
        /// Marshals the lengths of root certificates.
        /// </summary>
        /// <param name="rootCerts">Root certificate byte arrays.</param>
        /// <returns>Pointer to an array of root certificate lengths.</returns>
        private IntPtr MarshallRootCertificatesLengths(List<byte[]> rootCerts)
        {
            if (rootCerts.Count == 0)
            {
                return IntPtr.Zero;
            }

            IntPtr certsLengthsPtr = Marshal.AllocHGlobal(IntPtr.Size * rootCerts.Count);

            for (int i = 0; i < rootCerts.Count; i++)
            {
                // Note: IntPtr and Rust's usize are the same size (pointer-sized integer).
                // We use IntPtr here to represent the numeric length value that Rust expects as usize.
                IntPtr certLen = new(rootCerts[i].Length);
                Marshal.WriteIntPtr(certsLengthsPtr, i * IntPtr.Size, certLen);
            }

            return certsLengthsPtr;
        }

        /// <summary>
        /// Copies a byte array into unmanaged memory for FFI.
        /// Returns <see cref="IntPtr.Zero"/> if null.
        /// </summary>
        private static IntPtr MarshalBytes(byte[]? data)
        {
            if (data is null)
            {
                return IntPtr.Zero;
            }

            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            return ptr;
        }

        /// <summary>
        /// Marshals the pub/sub configuration.
        /// </summary>
        /// <param name="config">The pub/sub subscription configuration.</param>
        /// <returns>The marshaled PubSubConfigInfo struct.</returns>
        private PubSubConfigInfo MarshalPubSubConfig(BasePubSubSubscriptionConfig? config)
        {
            PubSubConfigInfo pubSubConfig = new();

            if (config == null)
            {
                return pubSubConfig;
            }

            var subscriptions = config.Subscriptions;

            // Marshal exact channels.
            if (subscriptions.TryGetValue(PubSubChannelMode.Exact, out ISet<ValkeyKey>? channels) && channels.Count > 0)
            {
                pubSubConfig.ChannelsPtr = MarshalStrings(channels.ToGlideStrings());
                pubSubConfig.ChannelCount = (uint)channels.Count;
            }

            // Marshal patterns.
            if (subscriptions.TryGetValue(PubSubChannelMode.Pattern, out ISet<ValkeyKey>? patterns) && patterns.Count > 0)
            {
                pubSubConfig.PatternsPtr = MarshalStrings(patterns.ToGlideStrings());
                pubSubConfig.PatternCount = (uint)patterns.Count;
            }

            // Marshal sharded channels - only for cluster clients.
            if (subscriptions.TryGetValue(PubSubChannelMode.Sharded, out ISet<ValkeyKey>? shardedChannels) && shardedChannels.Count > 0)
            {
                pubSubConfig.ShardedChannelsPtr = MarshalStrings(shardedChannels.ToGlideStrings());
                pubSubConfig.ShardedChannelCount = (uint)shardedChannels.Count;
            }

            return pubSubConfig;
        }

        /// <summary>
        /// Marshals an array of <see cref="GlideString"/> values.
        /// </summary>
        private static IntPtr MarshalStrings(GlideString[] strings)
        {
            if (strings.Length == 0)
            {
                return IntPtr.Zero;
            }

            // Allocate memory for strings.
            IntPtr arrayPtr = Marshal.AllocHGlobal(IntPtr.Size * strings.Length);

            // Copy strings to allocated memory.
            for (int i = 0; i < strings.Length; i++)
            {
                Marshal.WriteIntPtr(arrayPtr, i * IntPtr.Size, MarshalString(strings[i]));
            }

            return arrayPtr;
        }

        /// <summary>
        /// Marshals a <see cref="GlideString"/> into unmanaged memory.
        /// </summary>
        private static IntPtr MarshalString(GlideString str)
        {
            byte[] bytes = str.Bytes;
            int length = bytes.Length;

            IntPtr ptr = Marshal.AllocHGlobal(length + 1);
            Marshal.Copy(bytes, 0, ptr, length);
            Marshal.WriteByte(ptr, length, 0); // null terminator

            return ptr;
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
    /// <param name="messageLen">The length of the message data in bytes (unsigned).</param>
    /// <param name="channelPtr">Pointer to the raw channel name bytes.</param>
    /// <param name="channelLen">The length of the channel name in bytes (unsigned).</param>
    /// <param name="patternPtr">Pointer to the raw pattern bytes (null if no pattern).</param>
    /// <param name="patternLen">The length of the pattern in bytes (unsigned, 0 if no pattern).</param>
    /// <returns>A managed PubSubMessage object.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameters are invalid or marshaling fails.</exception>
    internal static PubSubMessage MarshalPubSubMessage(
        PushKind pushKind,
        IntPtr messagePtr,
        ulong messageLen,
        IntPtr channelPtr,
        ulong channelLen,
        IntPtr patternPtr,
        ulong patternLen)
    {
        try
        {
            // Marshal message bytes.
            if (messagePtr == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid message data: pointer is null");
            }

            if (messageLen == 0)
            {
                throw new ArgumentException("Invalid message data: length is zero");
            }

            byte[] messageBytes = new byte[messageLen];
            Marshal.Copy(messagePtr, messageBytes, 0, (int)messageLen);

            // Marshal channel bytes.
            if (channelPtr == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid channel data: pointer is null");
            }

            if (channelLen == 0)
            {
                throw new ArgumentException("Invalid channel data: length is zero");
            }

            byte[] channelBytes = new byte[channelLen];
            Marshal.Copy(channelPtr, channelBytes, 0, (int)channelLen);

            // Create message based on push kind.
            if (pushKind == PushKind.PushMessage)
            {
                return PubSubMessage.FromChannel(messageBytes, channelBytes);
            }
            else if (pushKind == PushKind.PushSMessage)
            {
                return PubSubMessage.FromShardedChannel(messageBytes, channelBytes);
            }
            else if (pushKind == PushKind.PushPMessage)
            {
                // Marshal pattern bytes.
                if (patternPtr == IntPtr.Zero)
                {
                    throw new ArgumentException("Invalid pattern data: pointer is null for pattern message");
                }

                if (patternLen == 0)
                {
                    throw new ArgumentException("Invalid pattern data: length is zero for pattern message");
                }

                byte[] patternBytes = new byte[patternLen];
                Marshal.Copy(patternPtr, patternBytes, 0, (int)patternLen);

                return PubSubMessage.FromPattern(messageBytes, channelBytes, patternBytes);
            }
            else
            {
                throw new InvalidOperationException($"Unexpected PushKind for message: {pushKind}");
            }
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException($"Failed to marshal PubSub message from FFI callback parameters: {ex.Message}", ex);
        }
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



    [StructLayout(LayoutKind.Sequential)]
    private struct RouteInfo
    {
        public RouteType Type;
        public int SlotId;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? SlotKey;
        public SlotType SlotType;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? Host;
        public int Port;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ConnectionRequest
    {
        public nuint AddressCount;
        public IntPtr Addresses; // ** NodeAddress - array pointer

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

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? ClientName;

        [MarshalAs(UnmanagedType.U1)]
        public bool LazyConnect;

        [MarshalAs(UnmanagedType.U1)]
        public bool RefreshTopologyFromInitialNodes;

        public PubSubConfigInfo PubSubConfig;

        [MarshalAs(UnmanagedType.U1)]
        public bool HasPubSubReconciliationIntervalMs;
        public uint PubSubReconciliationIntervalMs;

        [MarshalAs(UnmanagedType.U1)]
        public bool HasCompressionConfig;
        public CompressionConfig CompressionConfig;

        [MarshalAs(UnmanagedType.U1)]
        public bool ReadOnly;

        public NodeDiscoveryMode NodeDiscoveryMode;

        #region Client-Side Cache

        [MarshalAs(UnmanagedType.U1)]
        public bool HasClientSideCacheConfig;
        public ClientSideCacheConfig ClientSideCacheConfig;

        #endregion Client-Side Cache
        #region Circuit Breaker

        [MarshalAs(UnmanagedType.U1)]
        public bool HasCircuitBreakerConfig;
        public CircuitBreakerConfig CircuitBreakerConfig;

        #endregion Circuit Breaker
        #region TLS

        public TlsMode TlsMode;

        public nuint RootCertsCount;
        public IntPtr RootCertsPtr;
        public IntPtr RootCertsLensPtr;

        #endregion TLS
        #region Mutual TLS

        public nuint ClientCertLen;
        public IntPtr ClientCertPtr;

        public nuint ClientKeyLen;
        public IntPtr ClientKeyPtr;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? ClientCertPath;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? ClientKeyPath;

        [MarshalAs(UnmanagedType.U1)]
        public bool CertReloadEnabled;

        [MarshalAs(UnmanagedType.U1)]
        public bool HasCertReloadIntervalSeconds;
        public uint CertReloadIntervalSeconds;

        #endregion Mutual TLS
        #region Inflight Requests Limit

        [MarshalAs(UnmanagedType.U1)]
        public bool HasInflightRequestsLimit;
        public uint InflightRequestsLimit;

        #endregion Inflight Requests Limit
        #region Periodic Checks

        [MarshalAs(UnmanagedType.U1)]
        public bool HasPeriodicChecksConfig;
        public PeriodicChecksMode PeriodicChecksMode;
        public uint PeriodicChecksIntervalSec;

        #endregion Periodic Checks
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

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct NodeAddress(string host, ushort port)
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public readonly string Host = host;
        public readonly ushort Port = port;
    }


    [StructLayout(LayoutKind.Sequential)]
    private readonly struct ScriptHashBuffer
    {
        public readonly IntPtr Ptr;
        public readonly UIntPtr Len;
        public readonly UIntPtr Capacity;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CompressionConfig(
        nuint minSize,
        int? level,
        CompressionBackend backend,
        bool enabled,
        ulong? maxDecompressedSize = null)
    {
        /// <summary>
        /// Minimum value size in bytes to compress.
        /// </summary>
        public nuint MinCompressionSize = minSize;

        /// <summary>
        /// Compression level for the backend.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool HasCompressionLevel = level.HasValue;
        public int CompressionLevel = level ?? default;

        /// <summary>
        /// The compression backend to use.
        /// </summary>
        public CompressionBackend Backend = backend;

        /// <summary>
        /// Whether compression is enabled.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool Enabled = enabled;

        /// <summary>Whether a max decompressed size was explicitly specified.</summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool HasMaxDecompressedSize = maxDecompressedSize.HasValue;

        /// <summary>Maximum allowed size for decompressed data (prevents decompression bombs).</summary>
        public ulong MaxDecompressedSize = maxDecompressedSize ?? default;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ClientSideCacheConfig(
        string cacheId,
        ulong maxCacheKb,
        ulong entryTtlMs,
        bool hasEvictionPolicy,
        EvictionPolicy evictionPolicy,
        bool enableMetrics,
        bool serverAssisted)
    {
        /// <summary>
        /// Unique identifier for the cache instance.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public readonly string CacheId = cacheId;

        /// <summary>
        /// Maximum size of the cache in kilobytes.
        /// </summary>
        public readonly ulong MaxCacheKb = maxCacheKb;

        /// <summary>
        /// Time-To-Live for cached entries in milliseconds (0 = no expiration).
        /// </summary>
        public readonly ulong EntryTtlMs = entryTtlMs;

        /// <summary>
        /// Whether an eviction policy was explicitly specified.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool HasEvictionPolicy = hasEvictionPolicy;

        /// <summary>
        /// The eviction policy for the cache.
        /// </summary>
        public readonly EvictionPolicy EvictionPolicy = evictionPolicy;

        /// <summary>
        /// Whether cache metrics collection is enabled.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool EnableMetrics = enableMetrics;

        /// <summary>
        /// Whether server-assisted client-side caching is enabled.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool ServerAssisted = serverAssisted;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct CircuitBreakerConfig(
        uint windowSizeMs,
        float failureRateThreshold,
        uint minErrors,
        uint openTimeoutMs,
        bool countTimeouts,
        uint consecutiveSuccesses)
    {
        /// <summary>
        /// Sliding window duration in milliseconds for error counting (0 = default).
        /// </summary>
        public readonly uint WindowSizeMs = windowSizeMs;

        /// <summary>
        /// Error rate threshold that triggers the circuit breaker (0 = default).
        /// </summary>
        public readonly float FailureRateThreshold = failureRateThreshold;

        /// <summary>
        /// Minimum errors in the window before the failure rate is evaluated (0 = default).
        /// </summary>
        public readonly uint MinErrors = minErrors;

        /// <summary>
        /// Duration in milliseconds the circuit breaker stays open before probing (0 = default).
        /// </summary>
        public readonly uint OpenTimeoutMs = openTimeoutMs;

        /// <summary>
        /// Whether timeout errors count toward tripping the circuit breaker.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool CountTimeouts = countTimeouts;

        /// <summary>
        /// Consecutive successful probes needed to close the circuit breaker (0 = default).
        /// </summary>
        public readonly uint ConsecutiveSuccesses = consecutiveSuccesses;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Statistics
    {
        /// <summary>
        /// Total number of connections opened to Valkey.
        /// </summary>
        public readonly ulong TotalConnections;

        /// <summary>
        /// Total number of GLIDE clients.
        /// </summary>
        public readonly ulong TotalClients;

        /// <summary>
        /// Total number of values compressed.
        /// </summary>
        public readonly ulong TotalValuesCompressed;

        /// <summary>
        /// Total number of values decompressed.
        /// </summary>
        public readonly ulong TotalValuesDecompressed;

        /// <summary>
        /// Total original bytes before compression.
        /// </summary>
        public readonly ulong TotalOriginalBytes;

        /// <summary>
        /// Total bytes after compression.
        /// </summary>
        public readonly ulong TotalBytesCompressed;

        /// <summary>
        /// Total bytes after decompression.
        /// </summary>
        public readonly ulong TotalBytesDecompressed;

        /// <summary>
        /// Number of times compression was skipped.
        /// </summary>
        public readonly ulong CompressionSkippedCount;

        /// <summary>
        /// Number of subscriptions that are out of sync.
        /// </summary>
        public readonly ulong SubscriptionOutOfSyncCount;

        /// <summary>
        /// Timestamp of the last subscription synchronization.
        /// </summary>
        public readonly ulong SubscriptionLastSyncTimestamp;
    }

    /// <summary>
    /// Stores a script in Rust core and returns its SHA1 hash.
    /// </summary>
    /// <param name="script">The Lua script code.</param>
    /// <returns>The SHA1 hash of the script.</returns>
    /// <exception cref="ArgumentException">Thrown when script is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when script storage fails.</exception>
    internal static string StoreScript(string script)
    {
        if (string.IsNullOrEmpty(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        byte[] scriptBytes = System.Text.Encoding.UTF8.GetBytes(script);
        IntPtr hashBufferPtr = IntPtr.Zero;

        try
        {
            unsafe
            {
                fixed (byte* scriptPtr = scriptBytes)
                {
                    hashBufferPtr = StoreScriptFfi((IntPtr)scriptPtr, (UIntPtr)scriptBytes.Length);
                }
            }

            if (hashBufferPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to store script in Rust core");
            }

            // Read the ScriptHashBuffer struct
            ScriptHashBuffer buffer = Marshal.PtrToStructure<ScriptHashBuffer>(hashBufferPtr);

            // Read the hash bytes from the buffer
            byte[] hashBytes = new byte[(int)buffer.Len];
            Marshal.Copy(buffer.Ptr, hashBytes, 0, (int)buffer.Len);

            // Convert to string
            string hash = System.Text.Encoding.UTF8.GetString(hashBytes);

            return hash;
        }
        finally
        {
            if (hashBufferPtr != IntPtr.Zero)
            {
                FreeScriptHashBuffer(hashBufferPtr);
            }
        }
    }

    /// <summary>
    /// Removes a script from Rust core storage.
    /// </summary>
    /// <param name="hash">The SHA1 hash of the script to remove.</param>
    /// <exception cref="ArgumentException">Thrown when hash is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when script removal fails.</exception>
    internal static void DropScript(string hash)
    {
        if (string.IsNullOrEmpty(hash))
        {
            throw new ArgumentException("Hash cannot be null or empty", nameof(hash));
        }

        byte[] hashBytes = System.Text.Encoding.UTF8.GetBytes(hash);
        IntPtr errorBuffer = IntPtr.Zero;

        try
        {
            unsafe
            {
                fixed (byte* hashPtr = hashBytes)
                {
                    errorBuffer = DropScriptFfi((IntPtr)hashPtr, (UIntPtr)hashBytes.Length);
                }
            }

            if (errorBuffer != IntPtr.Zero)
            {
                string error = Marshal.PtrToStringAnsi(errorBuffer)
                    ?? "Unknown error dropping script";
                throw new InvalidOperationException($"Failed to drop script: {error}");
            }
        }
        finally
        {
            if (errorBuffer != IntPtr.Zero)
            {
                FreeString(errorBuffer);
            }
        }
    }


    // ========================================================================================
    // OpenTelemetry
    // ========================================================================================

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct OpenTelemetryConfig(TracesConfig? traces, MetricsConfig? metrics, uint? flushIntervalMs)
    {
        /// <summary>
        /// Traces configuration for OpenTelemetry.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool HasTraces = traces.HasValue;
        public readonly TracesConfig Traces = traces ?? default;

        /// <summary>
        /// Metrics configuration for OpenTelemetry.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool HasMetrics = metrics.HasValue;
        public readonly MetricsConfig Metrics = metrics ?? default;

        /// <summary>
        /// The flush interval in milliseconds for OpenTelemetry.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool HasFlushIntervalMs = flushIntervalMs.HasValue;
        public readonly uint FlushIntervalMs = flushIntervalMs ?? default;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct TracesConfig(string endpoint, uint? samplePercentage)
    {
        /// <summary>
        /// Endpoint for OpenTelemetry traces.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public readonly string Endpoint = endpoint;

        /// <summary>
        /// Sample percentage for OpenTelemetry traces.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool HasSamplePercentage = samplePercentage.HasValue;
        public readonly uint SamplePercentage = samplePercentage ?? default;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct MetricsConfig(string endpoint)
    {
        /// <summary>
        /// Endpoint for OpenTelemetry metrics.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public readonly string Endpoint = endpoint;
    }

    // ========================================================================================
    // Authentication
    // ========================================================================================

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct AuthenticationInfo(string? username, string? password, IamCredentials? iamCredentials)
    {
        /// <summary>
        /// Username for authentication.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public readonly string? Username = username;

        /// <summary>
        /// Password for authentication.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public readonly string? Password = password;

        /// <summary>
        /// IAM credentials for authentication.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool HasIamCredentials = iamCredentials.HasValue;
        public readonly IamCredentials IamCredentials = iamCredentials ?? default;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct IamCredentials(string clusterName, string region, ServiceType serviceType, uint? refreshIntervalSeconds)
    {
        /// <summary>
        /// The name of the cluster for IAM authentication.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public readonly string ClusterName = clusterName;

        /// <summary>
        /// The AWS region for IAM authentication.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public readonly string Region = region;

        /// <summary>
        /// The AWS service type for IAM authentication.
        /// </summary>
        public readonly ServiceType ServiceType = serviceType;

        /// <summary>
        /// The refresh interval in seconds for IAM authentication.
        /// </summary>
        public readonly bool HasRefreshIntervalSeconds = refreshIntervalSeconds.HasValue;
        public readonly uint? RefreshIntervalSeconds = refreshIntervalSeconds ?? default;
    }


    #region Monitor

    /// <summary>
    /// FFI-safe configuration struct passed to <see cref="CreateMonitorClientFfi"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonitorConfigFfi
    {
        /// <summary>
        /// The server host.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string Host;

        /// <summary>
        /// The server port.
        /// </summary>
        public ushort Port;

        /// <summary>
        /// Whether to use TLS for the connection.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool UseTls;

        /// <summary>
        /// The database number to select.
        /// </summary>
        public ushort Database;

        /// <summary>
        /// The username for authentication,
        /// or <see langword="null"/> if not set.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? Username;

        /// <summary>
        /// The password for authentication,
        /// or <see langword="null"/> if not set.</summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? Password;
    }

    /// <summary>
    /// Response struct returned by <see cref="CreateMonitorClientFfi"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MonitorConnectionResponse
    {
        /// <summary>
        /// Pointer to the monitor client on success,
        /// or <see cref="IntPtr.Zero"/> on failure.
        /// </summary>
        public IntPtr ConnPtr;

        /// <summary>
        /// Error message on failure,
        /// or <see cref="IntPtr.Zero"/> on success.
        /// </summary>
        public IntPtr ConnectionErrorMessage;
    }

    #endregion Monitor
}
