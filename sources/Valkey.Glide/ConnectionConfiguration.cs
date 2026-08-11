// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

using Valkey.Glide.Internals;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide;

/// <summary>
/// Configuration classes and builders for establishing connections to Valkey servers.
/// </summary>
/// <seealso href="https://glide.valkey.io/reference/connection-options/">Valkey GLIDE – Connection Options</seealso>
/// <seealso href="https://glide.valkey.io/how-to/connections/timeouts-and-reconnect-strategy/">Valkey GLIDE – Timeouts and Reconnect Strategy</seealso>
public abstract class ConnectionConfiguration
{
    /// <summary>
    /// Maximum certificate/key size (10 MB).
    /// </summary>
    public const int CertificateMaxSize = 10 * 1024 * 1024;

    /// <summary>
    /// A callback for resolving server addresses before connection.
    /// </summary>
    /// <param name="host">The configured host name or IP address.</param>
    /// <param name="port">The configured port number.</param>
    /// <returns>The resolved (host, port) to use for the actual connection.</returns>
    /// <remarks>
    /// The resolver must be thread-safe and should avoid blocking operations, as it is
    /// called synchronously during the connection process.
    /// <para/>
    /// If the resolver throws an exception, the client falls back to the original
    /// (unresolved) address and logs the exception at <see cref="Level.Error"/>.
    /// <para/>
    /// The resolver is invoked once per address at initial connection time. It is not
    /// invoked on subsequent reconnection attempts.
    /// </remarks>
    public delegate (string host, ushort port) AddressResolverDelegate(string host, ushort port);

    #region Structs and Enums definitions

    internal record ConnectionConfig
    {
        public List<NodeAddress> Addresses = [];
        public bool ClusterMode;
        public uint? RequestTimeoutMs;
        public uint? ConnectionTimeoutMs;
        public ReadFrom? ReadFrom;
        public RetryStrategy? RetryStrategy;
        public AuthenticationInfo? AuthenticationInfo;
        public uint DatabaseId;
        public Protocol? Protocol;
        public string? ClientName;
        public bool LazyConnect;
        public bool RefreshTopologyFromInitialNodes;
        public BasePubSubSubscriptionConfig? PubSubSubscriptions;
        public uint? PubSubReconciliationIntervalMs;
        public CompressionConfig? CompressionConfig;
        public bool ReadOnly;
        public NodeDiscoveryMode NodeDiscoveryMode = NodeDiscoveryMode.Standard;
        public ClientSideCacheConfig? ClientSideCacheConfig;
        public CircuitBreakerConfig? CircuitBreakerConfig;
        public AddressResolverDelegate? AddressResolver;

        // TLS
        public TlsMode TlsMode = TlsMode.NoTls;
        public readonly List<byte[]> RootCertificates = [];

        // Mutual TLS
        public byte[]? ClientCertificate;
        public byte[]? ClientKey;
        public string? ClientCertificatePath;
        public string? ClientKeyPath;
        public bool CertReloadEnabled;
        public uint? CertReloadIntervalSeconds;

        // Inflight requests limit
        public uint? InflightRequestsLimit;

        // Periodic checks
        public PeriodicChecksMode? PeriodicChecksMode;
        public uint? PeriodicChecksIntervalSecs;

        internal FFI.ConnectionConfig ToFfi() => new(
            Addresses,
            ClusterMode,
            RequestTimeoutMs,
            ConnectionTimeoutMs,
            ReadFrom,
            RetryStrategy,
            AuthenticationInfo,
            DatabaseId,
            Protocol,
            ClientName,
            LazyConnect,
            RefreshTopologyFromInitialNodes,
            PubSubSubscriptions,
            PubSubReconciliationIntervalMs,
            CompressionConfig?.ToFfi(),
            ReadOnly,
            NodeDiscoveryMode,
            ClientSideCacheConfig?.ToFfi(),
            CircuitBreakerConfig?.ToFfi(),

            // TLS
            TlsMode,
            RootCertificates,

            // Mutual TLS
            ClientCertificate,
            ClientKey,
            ClientCertificatePath,
            ClientKeyPath,
            CertReloadEnabled,
            CertReloadIntervalSeconds,

            // Inflight requests limit
            InflightRequestsLimit,

            // Periodic checks
            PeriodicChecksMode,
            PeriodicChecksIntervalSecs
        );
    }

    /// <summary>
    /// Represents the strategy used to determine how and when to reconnect, in case of connection
    /// failures. The time between attempts grows exponentially, to the formula <c>rand(0 ... factor *
    /// (exponentBase ^ N))</c>, where <c>N</c> is the number of failed attempts.
    /// <para />
    /// Once the maximum value is reached, that will remain the time between retry attempts until a
    /// reconnect attempt is successful. The client will attempt to reconnect indefinitely.
    /// </summary>
    /// <param name="numberOfRetries"><inheritdoc cref="NumberOfRetries" path="/summary" /></param>
    /// <param name="factor"><inheritdoc cref="Factor" path="/summary" /></param>
    /// <param name="exponentBase"><inheritdoc cref="ExponentBase" path="/summary" /></param>
    /// <param name="jitterPercent"><inheritdoc cref="JitterPercent" path="/summary" /></param>
    [StructLayout(LayoutKind.Sequential)]
    public struct RetryStrategy(uint numberOfRetries, uint factor, uint exponentBase, uint? jitterPercent = null)
    {
        /// <summary>
        /// Number of retry attempts that the client should perform when disconnected from the server,
        /// where the time between retries increases. Once the retries have reached the maximum value, the
        /// time between retries will remain constant until a reconnect attempt is successful.
        /// </summary>
        public uint NumberOfRetries = numberOfRetries;

        /// <summary>
        /// The multiplier that will be applied to the waiting time between each retry.
        /// </summary>
        public uint Factor = factor;

        /// <summary>
        /// The exponent base configured for the strategy.
        /// </summary>
        public uint ExponentBase = exponentBase;

        [MarshalAs(UnmanagedType.U1)]
        internal bool HasJitterPercent = jitterPercent is not null;

        /// <summary>
        /// The Jitter precent configured for the strategy.
        /// </summary>
        public uint JitterPercent = jitterPercent ?? 0;
    }

    /// <summary>
    /// Represents the client's read from strategy and Availability zone if applicable.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ReadFrom
    {
        /// <summary>
        /// The read from strategy.
        /// </summary>
        public ReadFromStrategy Strategy;

        /// <summary>
        /// The Availability Zone (AZ) identifier.
        /// </summary>
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string? Az;

        /// <summary>
        /// Constructs a read from strategy without an Availability Zone (AZ).
        /// </summary>
        /// <param name="strategy">A strategy that does not require an Availability Zone.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="strategy"/> requires an Availability Zone.</exception>
        public ReadFrom(ReadFromStrategy strategy)
        {
            if (strategy.IsAzReadFromStrategy())
            {
                throw new ArgumentException($"Availability zone must be specified for strategy '{strategy}'.");
            }

            Strategy = strategy;
            Az = null;
        }

        /// <summary>
        /// Constructs a read from strategy with an Availability Zone (AZ).
        /// </summary>
        /// <param name="strategy">A strategy that requires an Availability Zone.</param>
        /// <param name="az">The corresponding Availability Zone.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="strategy"/> does not accept an Availability Zone.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="az"/> is empty or whitespace.</exception>
        public ReadFrom(ReadFromStrategy strategy, string az)
        {
            if (!strategy.IsAzReadFromStrategy())
            {
                throw new ArgumentException($"Availability zone cannot be specified for strategy '{strategy}'.");
            }

            if (string.IsNullOrWhiteSpace(az))
            {
                throw new ArgumentException("Availability zone cannot be empty or whitespace");
            }

            Strategy = strategy;
            Az = az;
        }
    }

    /// <summary>
    /// Represents the client's read from strategy.
    /// </summary>
    /// <seealso href="https://glide.valkey.io/how-to/connections/read-strategy/">Valkey GLIDE – Read Strategy</seealso>
    public enum ReadFromStrategy : uint
    {
        /// <summary>
        /// Always read from the primary, to get the freshest data.
        /// </summary>
        Primary = 0,

        /// <summary>
        /// Read from replicas in round-robin, falling back to the primary if none are available.
        /// </summary>
        PreferReplica = 1,

        /// <summary>
        /// Read from replicas in the client's Availability Zone (AZ), falling back to other nodes if needed.
        /// </summary>
        AzAffinity,

        /// <summary>
        /// Read from replicas or the primary in the client's Availability Zone (AZ), falling back to other nodes if needed.
        /// </summary>
        AzAffinityReplicasAndPrimary,

        /// <summary>
        /// Read from all nodes (primary and replicas) in round-robin.
        /// </summary>
        AllNodes,
    }

    /// <summary>
    /// Represents the communication protocol with the server.
    /// </summary>
    public enum Protocol : uint
    {
        /// <summary>
        /// Use RESP2 to communicate with the server nodes.
        /// </summary>
        RESP2 = 0,

        /// <summary>
        /// Use RESP3 to communicate with the server nodes.
        /// </summary>
        RESP3 = 1,
    }

    #endregion

    private static readonly string DEFAULT_HOST = "localhost";
    private static readonly ushort DEFAULT_PORT = 6379;

    /// <summary>
    /// Basic class which holds common configuration for all types of clients.<br />
    /// Refer to derived classes for more details: <see cref="StandaloneClientConfiguration" /> and <see cref="ClusterClientConfiguration" />.
    /// </summary>
    public abstract class BaseClientConfiguration
    {
        internal ConnectionConfig Request = new();

        internal ConnectionConfig ToRequest() => Request;
    }

    /// <summary>
    /// Configuration for a standalone client. <br />
    /// Use <see cref="StandaloneClientConfigurationBuilder" /> or
    /// <see cref="StandaloneClientConfiguration(List{ValueTuple{string?, ushort?}}, bool?, TimeSpan?, TimeSpan?, ReadFrom?, RetryStrategy?, string?, string?, uint?, Protocol?, string?, bool)" /> to create an instance.
    /// </summary>
    public sealed class StandaloneClientConfiguration : BaseClientConfiguration
    {
        internal StandaloneClientConfiguration() { }

        /// <summary>
        /// Configuration for a standalone client.
        /// </summary>
        /// <param name="addresses"><inheritdoc cref="ClientConfigurationBuilder{T}.Addresses" path="/summary" /></param>
        /// <param name="useTls"><inheritdoc cref="ClientConfigurationBuilder{T}.UseTls" path="/summary" /></param>
        /// <param name="requestTimeout"><inheritdoc cref="ClientConfigurationBuilder{T}.RequestTimeout" path="/summary" /></param>
        /// <param name="connectionTimeout"><inheritdoc cref="ClientConfigurationBuilder{T}.ConnectionTimeout" path="/summary" /></param>
        /// <param name="readFrom"><inheritdoc cref="ClientConfigurationBuilder{T}.ReadFrom" path="/summary" /></param>
        /// <param name="retryStrategy"><inheritdoc cref="ClientConfigurationBuilder{T}.ConnectionRetryStrategy" path="/summary" /></param>
        /// <param name="username">The username for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        /// <param name="databaseId"><inheritdoc cref="ClientConfigurationBuilder{T}.DatabaseId" path="/summary" /></param>
        /// <param name="protocol"><inheritdoc cref="ClientConfigurationBuilder{T}.ProtocolVersion" path="/summary" /></param>
        /// <param name="clientName"><inheritdoc cref="ClientConfigurationBuilder{T}.ClientName" path="/summary" /></param>
        /// <param name="lazyConnect"><inheritdoc cref="ClientConfigurationBuilder{T}.LazyConnect" path="/summary" /></param>
        public StandaloneClientConfiguration(
            List<(string? host, ushort? port)> addresses,
            bool? useTls = null,
            TimeSpan? requestTimeout = null,
            TimeSpan? connectionTimeout = null,
            ReadFrom? readFrom = null,
            RetryStrategy? retryStrategy = null,
            string? username = null,
            string? password = null,
            uint? databaseId = null,
            Protocol? protocol = null,
            string? clientName = null,
            bool lazyConnect = false
            )
        {
            StandaloneClientConfigurationBuilder builder = new();
            addresses.ForEach(addr => builder.Addresses += addr);
            builder.UseTls = useTls ?? false;
            _ = requestTimeout.HasValue ? builder.RequestTimeout = requestTimeout.Value : new();
            _ = connectionTimeout.HasValue ? builder.ConnectionTimeout = connectionTimeout.Value : new();
            _ = readFrom.HasValue ? builder.ReadFrom = readFrom.Value : new();
            _ = retryStrategy.HasValue ? builder.ConnectionRetryStrategy = retryStrategy.Value : new();
            _ = (username ?? password) is not null ? builder.WithAuthentication(username, password!) : new();
            _ = databaseId.HasValue ? builder.DatabaseId = databaseId.Value : new();
            _ = protocol.HasValue ? builder.ProtocolVersion = protocol.Value : new();
            _ = clientName is not null ? builder.ClientName = clientName : "";
            builder.LazyConnect = lazyConnect;
            Request = builder.Build().Request;
        }
    }

    /// <summary>
    /// Configuration for a cluster client. Use <see cref="ClusterClientConfigurationBuilder" /> to create an instance.
    /// </summary>
    public sealed class ClusterClientConfiguration : BaseClientConfiguration
    {
        internal ClusterClientConfiguration() { }

        /// <summary>
        /// Configuration for a cluster client.
        /// </summary>
        /// <param name="addresses"><inheritdoc cref="ClientConfigurationBuilder{T}.Addresses" path="/summary" /></param>
        /// <param name="useTls"><inheritdoc cref="ClientConfigurationBuilder{T}.UseTls" path="/summary" /></param>
        /// <param name="requestTimeout"><inheritdoc cref="ClientConfigurationBuilder{T}.RequestTimeout" path="/summary" /></param>
        /// <param name="connectionTimeout"><inheritdoc cref="ClientConfigurationBuilder{T}.ConnectionTimeout" path="/summary" /></param>
        /// <param name="readFrom"><inheritdoc cref="ClientConfigurationBuilder{T}.ReadFrom" path="/summary" /></param>
        /// <param name="retryStrategy"><inheritdoc cref="ClientConfigurationBuilder{T}.ConnectionRetryStrategy" path="/summary" /></param>
        /// <param name="username">The username for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        /// <param name="databaseId"><inheritdoc cref="ClientConfigurationBuilder{T}.DatabaseId" path="/summary" /></param>
        /// <param name="protocol"><inheritdoc cref="ClientConfigurationBuilder{T}.ProtocolVersion" path="/summary" /></param>
        /// <param name="clientName"><inheritdoc cref="ClientConfigurationBuilder{T}.ClientName" path="/summary" /></param>
        /// <param name="lazyConnect"><inheritdoc cref="ClientConfigurationBuilder{T}.LazyConnect" path="/summary" /></param>
        public ClusterClientConfiguration(
            List<(string? host, ushort? port)> addresses,
            bool? useTls = null,
            TimeSpan? requestTimeout = null,
            TimeSpan? connectionTimeout = null,
            ReadFrom? readFrom = null,
            RetryStrategy? retryStrategy = null,
            string? username = null,
            string? password = null,
            uint? databaseId = null,
            Protocol? protocol = null,
            string? clientName = null,
            bool lazyConnect = false
            )
        {
            ClusterClientConfigurationBuilder builder = new();
            addresses.ForEach(addr => builder.Addresses += addr);
            builder.UseTls = useTls ?? false;
            _ = requestTimeout.HasValue ? builder.RequestTimeout = requestTimeout.Value : new();
            _ = connectionTimeout.HasValue ? builder.ConnectionTimeout = connectionTimeout.Value : new();
            _ = readFrom.HasValue ? builder.ReadFrom = readFrom.Value : new();
            _ = retryStrategy.HasValue ? builder.ConnectionRetryStrategy = retryStrategy.Value : new();
            _ = (username ?? password) is not null ? builder.WithAuthentication(username, password!) : new();
            _ = databaseId.HasValue ? builder.DatabaseId = databaseId.Value : new();
            _ = protocol.HasValue ? builder.ProtocolVersion = protocol.Value : new();
            _ = clientName is not null ? builder.ClientName = clientName : "";
            builder.LazyConnect = lazyConnect;
            Request = builder.Build().Request;
        }
    }

    /// <summary>
    /// Builder for configuration of common parameters for standalone and cluster client.
    /// </summary>
    /// <typeparam name="T">Derived builder class</typeparam>
    public abstract class ClientConfigurationBuilder<T>
        where T : ClientConfigurationBuilder<T>, new()
    {
        internal ConnectionConfig Config;

        /// <summary>
        /// Initializes a new instance of the ClientConfigurationBuilder class.
        /// </summary>
        /// <param name="clusterMode">Whether this is a cluster mode configuration.</param>
        protected ClientConfigurationBuilder(bool clusterMode)
        {
            Config = new ConnectionConfig { ClusterMode = clusterMode };
        }

        #region Address

        /// <inheritdoc cref="Addresses" />
        /// <b>Add</b> a new address to the list.<br />
        /// See also <seealso cref="Addresses" />.
        protected (string? host, ushort? port) Address
        {
            set => Config.Addresses.Add(new NodeAddress
            (
                value.host ?? DEFAULT_HOST,
                value.port ?? DEFAULT_PORT
            ));
        }

        /// <inheritdoc cref="Address" />
        public T WithAddress(string? host, ushort? port)
        {
            Address = (host, port);
            return (T)this;
        }

        /// <summary>
        /// <b>Add</b> a new address to the list with default port.
        /// </summary>
        public T WithAddress(string host)
        {
            Address = (host, DEFAULT_PORT);
            return (T)this;
        }

        /// <summary>
        /// Syntax sugar helper class for adding addresses.
        /// </summary>
        public sealed class AddressBuilder
        {
            private readonly ClientConfigurationBuilder<T> _owner;

            internal AddressBuilder(ClientConfigurationBuilder<T> owner)
            {
                _owner = owner;
            }

            /// <inheritdoc cref="WithAddress(string?, ushort?)" />
            public static AddressBuilder operator +(AddressBuilder builder, (string? host, ushort? port) address)
            {
                _ = builder._owner.WithAddress(address.host, address.port);
                return builder;
            }

            /// <inheritdoc cref="WithAddress(string)" />
            public static AddressBuilder operator +(AddressBuilder builder, string host)
            {
                _ = builder._owner.WithAddress(host);
                return builder;
            }
        }

        /// <summary>
        /// DNS Addresses and ports of known nodes in the cluster. If the server is in cluster mode the
        /// list can be partial, as the client will attempt to map out the cluster and find all nodes. If
        /// the server is in standalone mode, only nodes whose addresses were provided will be used by the
        /// client.
        /// <para />
        /// For example: <code>
        /// [
        ///   ("sample-address-0001.use1.cache.amazonaws.com", 6378),
        ///   ("sample-address-0002.use2.cache.amazonaws.com"),
        ///   ("sample-address-0002.use3.cache.amazonaws.com", 6380)
        /// ]</code>
        /// </summary>
        public AddressBuilder Addresses
        {
            get => new(this);
            set { } // needed for +=
        }

        #endregion
        #region TLS

        /// <summary>
        /// Configure whether to use Transport Layer Security (TLS) when connecting to the server.
        /// <br />
        /// Must match the TLS connection of the server or cluster.
        /// </summary>
        /// <seealso href="https://glide.valkey.io/tutorials/tls/">Valkey GLIDE – Setting up TLS</seealso>
        /// <seealso href="https://glide.valkey.io/how-to/security/tls/">Valkey GLIDE – Configure TLS</seealso>
        public bool UseTls
        {
            get => Config.TlsMode is TlsMode.SecureTls or TlsMode.InsecureTls;
            set
            {
                if (value)
                {
                    if (Config.TlsMode == TlsMode.NoTls)
                    {
                        Config.TlsMode = TlsMode.SecureTls;
                    }
                }
                else
                {
                    Config.TlsMode = TlsMode.NoTls;
                }
            }
        }

        /// <inheritdoc cref="UseTls" />
        public T WithTls(bool useTls = true)
        {
            UseTls = useTls;
            return (T)this;
        }

        /// <summary>
        /// Configure whether to bypass certificate verification when using
        /// Transport Layer Security (TLS) to connect to the server.
        /// <br />
        /// <b>SECURITY WARNING</b>: Insecure mode is only for development and testing environments.
        /// <b>It is strongly discouraged in production environments</b> as it introduces security risks such as man-in-the-middle attacks.
        /// <br />
        /// Requires <see cref="UseTls"/> to be enabled, otherwise throws <see cref="ArgumentException"/>.
        /// </summary>
        /// <exception cref="ArgumentException">If <see cref="UseTls"/> is not enabled.</exception>
        /// <seealso href="https://glide.valkey.io/how-to/security/tls/#insecure-tls-mode">Valkey GLIDE – Configure TLS</seealso>
        public bool UseInsecureTls
        {
            get => Config.TlsMode == TlsMode.InsecureTls;
            set
            {
                if (Config.TlsMode == TlsMode.NoTls)
                {
                    throw new ArgumentException("Cannot configure insecure TLS when TLS is disabled.");
                }

                if (value)
                {
                    var msg = "SECURITY WARNING: Insecure TLS mode enabled. "
                        + "Certificate verification is disabled. "
                        + "This is strongly discouraged in production environments."
                        + "See https://glide.valkey.io/how-to/security/tls/#insecure-tls-mode for more details.";
                    Logger.Log(Level.Warn, GetType().Name, msg);
                }

                Config.TlsMode =
                    value
                    ? TlsMode.InsecureTls
                    : TlsMode.SecureTls;
            }
        }

        /// <inheritdoc cref="UseInsecureTls" />
        public T WithInsecureTls(bool useInsecure = true)
        {
            UseInsecureTls = useInsecure;
            return (T)this;
        }

        /// <summary>
        /// Trusted root certificates for TLS connections.
        /// When provided, these certificates will be used instead of the system's default trust store.
        /// </summary>
        internal List<byte[]> TrustedCertificates => Config.RootCertificates;

        /// <summary>
        /// Adds an additional trusted certificate for TLS connections.
        /// </summary>
        /// <param name="certificatePath">Trusted certificate file path</param>
        /// <returns>This builder for method chaining</returns>
        /// <exception cref="ArgumentException">If the certificate is null, empty, or too large.</exception>
        /// <exception cref="FileNotFoundException">If the certificate file does not exist</exception>
        /// <seealso href="https://glide.valkey.io/tutorials/tls/">Valkey GLIDE – Setting up TLS</seealso>
        /// <seealso href="https://glide.valkey.io/how-to/security/tls/">Valkey GLIDE – Configure TLS</seealso>
        public T WithTrustedCertificate(string certificatePath)
        {
            GuardClauses.ThrowIfCertificateNotSupported(certificatePath, nameof(certificatePath));
            return WithTrustedCertificate(File.ReadAllBytes(certificatePath));
        }

        /// <summary>
        /// Adds an additional trusted certificate for TLS connections.
        /// </summary>
        /// <param name="certificateData">Trusted certificate data</param>
        /// <returns>This builder for method chaining</returns>
        /// <exception cref="ArgumentException">If the certificate is empty or too large.</exception>
        /// <seealso href="https://glide.valkey.io/tutorials/tls/">Valkey GLIDE – Setting up TLS</seealso>
        /// <seealso href="https://glide.valkey.io/how-to/security/tls/">Valkey GLIDE – Configure TLS</seealso>
        public T WithTrustedCertificate(byte[] certificateData)
        {
            GuardClauses.ThrowIfCertificateNotSupported(certificateData, nameof(certificateData));
            TrustedCertificates.Add(certificateData);
            return (T)this;
        }

        #endregion
        #region Mutual TLS

        /// <summary>
        /// Configures mutual TLS for the given client certificate and key data.
        /// </summary>
        /// <param name="certificateData">Client certificate data</param>
        /// <param name="keyData">Client key data</param>
        /// <returns>This builder for method chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="certificateData"/> or <paramref name="keyData"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="certificateData"/> or <paramref name="keyData"/> is empty or too large.</exception>
        /// <seealso href="https://glide.valkey.io/how-to/security/tls/">Valkey GLIDE – Configure TLS</seealso>
        public T WithClientCertificate(byte[] certificateData, byte[] keyData)
        {
            GuardClauses.ThrowIfCertificateNotSupported(certificateData, nameof(certificateData));
            GuardClauses.ThrowIfCertificateNotSupported(keyData, nameof(keyData));

            ClearMutualTls();

            Config.ClientCertificate = certificateData;
            Config.ClientKey = keyData;

            return (T)this;
        }

        /// <summary>
        /// Configures mutual TLS for the given client certificate and key path.<p/>
        /// The client periodically reloads the client certificate and key to support certificate rotation.
        /// </summary>
        /// <param name="certificatePath">Client certificate file path</param>
        /// <param name="keyPath">Client key file path</param>
        /// <returns>This builder for method chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="certificatePath"/> or <paramref name="keyPath"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="certificatePath"/> or <paramref name="keyPath"/> is empty.</exception>
        /// <seealso href="https://glide.valkey.io/how-to/security/tls/">Valkey GLIDE – Configure TLS</seealso>
        public T WithClientCertificate(string certificatePath, string keyPath)
        {
            ArgumentException.ThrowIfNullOrEmpty(certificatePath, nameof(certificatePath));
            ArgumentException.ThrowIfNullOrEmpty(keyPath, nameof(keyPath));

            ClearMutualTls();

            Config.ClientCertificatePath = certificatePath;
            Config.ClientKeyPath = keyPath;
            Config.CertReloadEnabled = true;

            return (T)this;
        }

        /// <summary>
        /// Configures mutual TLS for the given client certificate and key path.<p/>
        /// The client reloads the client certificate and key at the specified interval to support certificate rotation.
        /// </summary>
        /// <param name="certificatePath">Client certificate file path</param>
        /// <param name="keyPath">Client key file path</param>
        /// <param name="reloadInterval">The interval at which to reload the client certificate and key. Rounded to the nearest second.</param>
        /// <returns>This builder for method chaining.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="certificatePath"/> or <paramref name="keyPath"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="certificatePath"/> or <paramref name="keyPath"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="reloadInterval"/> is not positive or exceeds <see cref="uint.MaxValue"/> seconds.</exception>
        /// <seealso href="https://glide.valkey.io/how-to/security/tls/">Valkey GLIDE – Configure TLS</seealso>
        public T WithClientCertificate(string certificatePath, string keyPath, TimeSpan reloadInterval)
        {
            ArgumentException.ThrowIfNullOrEmpty(certificatePath, nameof(certificatePath));
            ArgumentException.ThrowIfNullOrEmpty(keyPath, nameof(keyPath));

            ClearMutualTls();

            Config.ClientCertificatePath = certificatePath;
            Config.ClientKeyPath = keyPath;
            Config.CertReloadEnabled = true;
            Config.CertReloadIntervalSeconds = TimeUtils.ToPositiveUintSecs(reloadInterval, nameof(reloadInterval));

            return (T)this;
        }

        /// <summary>
        /// Clears the mutual TLS properties.
        /// </summary>
        private void ClearMutualTls()
        {
            Config.ClientCertificate = null;
            Config.ClientKey = null;
            Config.ClientCertificatePath = null;
            Config.ClientKeyPath = null;
            Config.CertReloadEnabled = false;
            Config.CertReloadIntervalSeconds = null;
        }

        #endregion
        #region Request Timeout

        /// <summary>
        /// The duration that the client should wait for a request to complete. This
        /// duration encompasses sending the request, awaiting for a response from the server, and any
        /// required reconnections or retries. If the specified timeout is exceeded for a pending request,
        /// it will result in a timeout error.<br />
        /// If not set, a default value of <c>250</c> milliseconds is used.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive.</exception>
        // TODO #512: Make nullable and default to GLIDE core.
        public TimeSpan RequestTimeout
        {
            get => TimeSpan.FromMilliseconds(Config.RequestTimeoutMs ?? 250);
            set => Config.RequestTimeoutMs = TimeUtils.ToPositiveUintMs(value, nameof(RequestTimeout));
        }

        /// <inheritdoc cref="RequestTimeout" />
        public T WithRequestTimeout(TimeSpan requestTimeout)
        {
            RequestTimeout = requestTimeout;
            return (T)this;
        }

        #endregion
        #region Connection Timeout

        /// <summary>
        /// The duration to wait for a TCP/TLS connection to complete.
        /// This applies both during initial client creation and any reconnections that may occur during request processing.<br />
        /// <b>Note</b>: A high connection timeout may lead to prolonged blocking of the entire command pipeline.<br />
        /// If not set, a default value of <c>250</c> milliseconds is used.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the value is not positive.</exception>
        // TODO #512: Make nullable and default to GLIDE core.
        public TimeSpan ConnectionTimeout
        {
            get => TimeSpan.FromMilliseconds(Config.ConnectionTimeoutMs ?? 250);
            set => Config.ConnectionTimeoutMs = TimeUtils.ToPositiveUintMs(value, nameof(ConnectionTimeout));
        }

        /// <inheritdoc cref="ConnectionTimeout" />
        public T WithConnectionTimeout(TimeSpan connectionTimeout)
        {
            ConnectionTimeout = connectionTimeout;
            return (T)this;
        }

        #endregion
        #region Read From

        /// <summary>
        /// Configure the client's read from strategy. If not set, <seealso cref="ReadFromStrategy.Primary" /> will be used.
        /// </summary>
        public ReadFrom ReadFrom
        {
            set => Config.ReadFrom = value;
        }

        /// <inheritdoc cref="ReadFrom" />
        public T WithReadFrom(ReadFrom readFrom)
        {
            ReadFrom = readFrom;
            return (T)this;
        }

        #endregion
        #region Authentication

        /// <summary>
        /// Configure server credentials for authentication process.
        /// Supports both password-based and IAM authentication.
        /// </summary>
        /// <param name="credentials">The server credentials for authentication.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public T WithCredentials(ServerCredentials credentials)
        {
            ArgumentNullException.ThrowIfNull(credentials);

            IamCredentials? iamCredentials = null;
            if (credentials.IamAuthConfig != null)
            {
                FFI.ServiceType serviceType = credentials.IamAuthConfig.ServiceType switch
                {
                    ServiceType.ElastiCache => FFI.ServiceType.ElastiCache,
                    ServiceType.MemoryDB => FFI.ServiceType.MemoryDB,
                    _ => throw new ArgumentOutOfRangeException(nameof(credentials.IamAuthConfig.ServiceType))
                };

                iamCredentials = new IamCredentials(
                    credentials.IamAuthConfig.ClusterName,
                    credentials.IamAuthConfig.Region,
                    serviceType,
                    credentials.IamAuthConfig.RefreshIntervalSeconds
                );
            }

            Config.AuthenticationInfo = new AuthenticationInfo
            (
                credentials.Username,
                credentials.Password != null ? new string(credentials.Password) : null,
                iamCredentials
            );

            return (T)this;
        }

        /// <summary>
        /// Configure server credentials for password-based authentication.
        /// </summary>
        /// <param name="username">The username for authentication. If null, "default" will be used.</param>
        /// <param name="password">The password for authentication.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public T WithAuthentication(string? username, string password)
            => WithCredentials(new ServerCredentials(username, password));

        /// <summary>
        /// Configure server credentials for password-based authentication with username "default".
        /// </summary>
        /// <param name="password">The password for authentication.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public T WithAuthentication(string password)
            => WithCredentials(new ServerCredentials(password));

        /// <summary>
        /// Configure server credentials for IAM authentication.
        /// </summary>
        /// <param name="username">The username for authentication.</param>
        /// <param name="iamConfig">The IAM authentication configuration.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public T WithAuthentication(string username, IamAuthConfig iamConfig)
            => WithCredentials(new ServerCredentials(username, iamConfig));

        #endregion
        #region Protocol

        /// <summary>
        /// Configure the protocol version to use. If not set, <seealso cref="Protocol.RESP3" /> will be used.<br />
        /// See also <seealso cref="Protocol" />.
        /// </summary>
        public Protocol ProtocolVersion
        {
            get => Config.Protocol ?? Protocol.RESP3;
            set => Config.Protocol = value;
        }

        /// <inheritdoc cref="ProtocolVersion" />
        public T WithProtocolVersion(Protocol protocol)
        {
            ProtocolVersion = protocol;
            return (T)this;
        }

        #endregion
        #region Client Name

        /// <summary>
        /// Client name to be used for the client. Will be used with <c>CLIENT SETNAME</c> command during connection establishment.
        /// </summary>
        public string? ClientName
        {
            get => Config.ClientName;
            set => Config.ClientName = value;
        }

        /// <inheritdoc cref="ClientName" />
        public T WithClientName(string? clientName)
        {
            ClientName = clientName;
            return (T)this;
        }

        #endregion
        #region Connection Retry Strategy

        /// <summary>
        /// Strategy used to determine how and when to reconnect, in case of connection failures.<br />
        /// See also <seealso cref="RetryStrategy" />
        /// </summary>
        public RetryStrategy ConnectionRetryStrategy
        {
            set => Config.RetryStrategy = value;
        }

        /// <inheritdoc cref="ConnectionRetryStrategy" />
        public T WithConnectionRetryStrategy(RetryStrategy connectionRetryStrategy)
        {
            ConnectionRetryStrategy = connectionRetryStrategy;
            return (T)this;
        }

        /// <inheritdoc cref="ConnectionRetryStrategy" />
        /// <inheritdoc cref="RetryStrategy(uint, uint, uint, uint?)" />
        public T WithConnectionRetryStrategy(uint numberOfRetries, uint factor, uint exponentBase, uint? jitterPercent = null)
            => WithConnectionRetryStrategy(new RetryStrategy(numberOfRetries, factor, exponentBase, jitterPercent));

        #endregion
        #region Database ID

        /// <summary>
        /// Index of the logical database to connect to. Must be non-negative and within the range
        /// supported by the server configuration. If not specified, defaults to database 0.
        /// For cluster mode, requires Valkey 9.0+ with cluster-databases configuration enabled.
        /// </summary>
        public uint DatabaseId
        {
            set => Config.DatabaseId = value;
        }

        /// <inheritdoc cref="DatabaseId" />
        public T WithDatabaseId(uint dataBaseId)
        {
            DatabaseId = dataBaseId;
            return (T)this;
        }

        #endregion
        #region Lazy Connect

        /// <summary>
        /// Configure whether to defer connections until the first command is executed.<br />
        /// If not explicitly set, a default value of <c>false</c> will be used.
        /// </summary>
        public bool LazyConnect
        {
            get => Config.LazyConnect;
            set => Config.LazyConnect = value;
        }

        /// <inheritdoc cref="LazyConnect" />
        public T WithLazyConnect(bool lazyConnect)
        {
            LazyConnect = lazyConnect;
            return (T)this;
        }

        #endregion
        #region PubSub Reconciliation Interval

        /// <summary>
        /// The interval between pub/sub subscription reconciliation attempts.
        /// </summary>
        public TimeSpan? PubSubReconciliationInterval
        {
            get => Config.PubSubReconciliationIntervalMs is { } ms ? TimeSpan.FromMilliseconds(ms) : null;
            set => Config.PubSubReconciliationIntervalMs = value.HasValue ? TimeUtils.ToPositiveUintMs(value.Value, nameof(PubSubReconciliationInterval)) : null;
        }

        /// <inheritdoc cref="PubSubReconciliationInterval" />
        public T WithPubSubReconciliationInterval(TimeSpan interval)
        {
            PubSubReconciliationInterval = interval;
            return (T)this;
        }

        #endregion
        #region Compression

        /// <summary>
        /// Compression configuration for transparent value compression.
        /// When enabled, values are automatically compressed before sending to the server
        /// and decompressed when receiving from the server.
        /// </summary>
        public CompressionConfig? CompressionConfig
        {
            get => Config.CompressionConfig;
            set => Config.CompressionConfig = value;
        }

        /// <inheritdoc cref="CompressionConfig" />
        public T WithCompression(CompressionConfig compressionConfig)
        {
            CompressionConfig = compressionConfig;
            return (T)this;
        }

        #endregion
        #region Client-Side Cache

        /// <summary>
        /// Client-side cache configuration for local caching of read command responses.
        /// When enabled, the client stores responses locally to reduce network round-trips
        /// and server load. The cache uses TTL-based expiration.
        /// </summary>
        /// <seealso cref="ClientSideCacheConfig"/>
        public ClientSideCacheConfig? ClientSideCacheConfig
        {
            get => Config.ClientSideCacheConfig;
            set => Config.ClientSideCacheConfig = value;
        }

        /// <inheritdoc cref="ClientSideCacheConfig" />
        public T WithClientSideCache(ClientSideCacheConfig clientSideCacheConfig)
        {
            ArgumentNullException.ThrowIfNull(clientSideCacheConfig);
            ClientSideCacheConfig = clientSideCacheConfig;
            return (T)this;
        }

        #endregion
        #region Circuit Breaker

        /// <summary>
        /// Circuit breaker configuration for the client. When set, enables a circuit breaker
        /// that detects unhealthy core state and rejects requests at the client boundary.
        /// </summary>
        /// <seealso cref="CircuitBreakerConfig"/>
        public CircuitBreakerConfig? CircuitBreakerConfig
        {
            get => Config.CircuitBreakerConfig;
            set => Config.CircuitBreakerConfig = value;
        }

        /// <inheritdoc cref="CircuitBreakerConfig" />
        public T WithCircuitBreaker(CircuitBreakerConfig circuitBreakerConfig)
        {
            ArgumentNullException.ThrowIfNull(circuitBreakerConfig, nameof(circuitBreakerConfig));
            CircuitBreakerConfig = circuitBreakerConfig;
            return (T)this;
        }

        #endregion
        #region Inflight Requests Limit

        /// <summary>
        /// The maximum number of concurrent requests allowed to be in-flight. When this limit is
        /// reached, new requests will immediately fail with a <see cref="Errors.RequestException"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is zero.</exception>
        /// <seealso href="https://glide.valkey.io/how-to/connections/limit-inflight-requests/">Valkey GLIDE – Limit Inflight Requests</seealso>
        public uint? InflightRequestsLimit
        {
            get => Config.InflightRequestsLimit;
            set
            {
                if (value.HasValue)
                {
                    ArgumentOutOfRangeException.ThrowIfZero(value.Value, nameof(value));
                }
                Config.InflightRequestsLimit = value;
            }
        }

        /// <inheritdoc cref="InflightRequestsLimit" />
        public T WithInflightRequestsLimit(uint inflightRequestsLimit)
        {
            InflightRequestsLimit = inflightRequestsLimit;
            return (T)this;
        }

        #endregion
        #region Address Resolver

        /// <summary>
        /// Optional callback for resolving server addresses before connection.
        /// When set, the callback is invoked for each server address and can return a different host/port.
        /// </summary>
        /// <seealso cref="AddressResolverDelegate"/>
        public AddressResolverDelegate? AddressResolver
        {
            get => Config.AddressResolver;
            set => Config.AddressResolver = value;
        }

        /// <inheritdoc cref="AddressResolver" />
        public T WithAddressResolver(AddressResolverDelegate addressResolver)
        {
            AddressResolver = addressResolver;
            return (T)this;
        }

        #endregion

        internal ConnectionConfig Build() => Config;
    }

    /// <summary>
    /// Represents the configuration settings for a Standalone GLIDE client.
    /// </summary>
    public class StandaloneClientConfigurationBuilder : ClientConfigurationBuilder<StandaloneClientConfigurationBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the StandaloneClientConfigurationBuilder class.
        /// </summary>
        public StandaloneClientConfigurationBuilder() : base(false) { }

        /// <summary>
        /// Complete the configuration with given settings.
        /// </summary>
        public new StandaloneClientConfiguration Build() => new() { Request = base.Build() };

        #region Node Discovery Mode
        /// <summary>
        /// Controls how the client discovers node roles and topology during connection
        /// initialization. If not set, defaults to <see cref="NodeDiscoveryMode.Standard" />.
        /// See <see cref="NodeDiscoveryMode" /> for the available modes and their details.
        /// </summary>
        public NodeDiscoveryMode NodeDiscoveryMode
        {
            get => Config.NodeDiscoveryMode;
            set => Config.NodeDiscoveryMode = value;
        }

        /// <inheritdoc cref="NodeDiscoveryMode" />
        public StandaloneClientConfigurationBuilder WithNodeDiscoveryMode(NodeDiscoveryMode nodeDiscoveryMode)
        {
            NodeDiscoveryMode = nodeDiscoveryMode;
            return this;
        }
        #endregion

        #region PubSub Subscriptions
        /// <summary>
        /// Configure PubSub subscriptions for the standalone client.
        /// </summary>
        /// <param name="config">The PubSub subscription configuration.</param>
        /// <returns>This configuration builder instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when config is null.</exception>
        /// <exception cref="ArgumentException">Thrown when config is invalid.</exception>
        public StandaloneClientConfigurationBuilder WithPubSubSubscriptions(StandalonePubSubSubscriptionConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            Config.PubSubSubscriptions = config;
            return this;
        }
        #endregion
    }

    /// <summary>
    /// Represents the configuration settings for a Cluster GLIDE client.
    /// </summary>
    public class ClusterClientConfigurationBuilder : ClientConfigurationBuilder<ClusterClientConfigurationBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the ClusterClientConfigurationBuilder class.
        /// </summary>
        public ClusterClientConfigurationBuilder() : base(true) { }

        /// <summary>
        /// Complete the configuration with given settings.
        /// </summary>
        public new ClusterClientConfiguration Build() => new() { Request = base.Build() };

        #region Refresh Topology
        /// <summary>
        /// Enables refreshing the cluster topology using only the initial nodes.
        /// <para />
        /// When this option is enabled, all topology updates (both the periodic checks and on-demand
        /// refreshes triggered by topology changes) will query only the initial nodes provided when
        /// creating the client, rather than using the internal cluster view.
        /// <para />
        /// If not set, defaults to <c>false</c> (uses internal cluster view for topology refresh).
        /// </summary>
        public bool RefreshTopologyFromInitialNodes
        {
            get => Config.RefreshTopologyFromInitialNodes;
            set => Config.RefreshTopologyFromInitialNodes = value;
        }

        /// <inheritdoc cref="RefreshTopologyFromInitialNodes" />
        public ClusterClientConfigurationBuilder WithRefreshTopologyFromInitialNodes(bool refreshTopologyFromInitialNodes)
        {
            RefreshTopologyFromInitialNodes = refreshTopologyFromInitialNodes;
            return this;
        }

        #endregion
        #region Periodic Checks

        /// <summary>
        /// Enables periodic topology checks.
        /// </summary>
        /// <seealso href="https://glide.valkey.io/how-to/connections/periodic-checks/">Valkey GLIDE – Configure Periodic Checks</seealso>
        /// <returns>This configuration builder instance for method chaining.</returns>
        public ClusterClientConfigurationBuilder WithPeriodicChecks()
        {
            Config.PeriodicChecksMode = PeriodicChecksMode.Enabled;
            Config.PeriodicChecksIntervalSecs = null;
            return this;
        }

        /// <summary>
        /// Enables periodic topology checks at the specified interval.
        /// </summary>
        /// <seealso href="https://glide.valkey.io/how-to/connections/periodic-checks/">Valkey GLIDE – Configure Periodic Checks</seealso>
        /// <param name="interval">The interval between periodic topology checks.</param>
        /// <returns>This configuration builder instance for method chaining.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="interval"/> is not positive or exceeds <see cref="uint.MaxValue"/> seconds.</exception>
        public ClusterClientConfigurationBuilder WithPeriodicChecks(TimeSpan interval)
        {
            Config.PeriodicChecksMode = PeriodicChecksMode.ManualInterval;
            Config.PeriodicChecksIntervalSecs = TimeUtils.ToPositiveUintSecs(interval, nameof(interval));
            return this;
        }

        /// <summary>
        /// Disables periodic topology checks.
        /// </summary>
        /// <seealso href="https://glide.valkey.io/how-to/connections/periodic-checks/">Valkey GLIDE – Configure Periodic Checks</seealso>
        /// <returns>This configuration builder instance for method chaining.</returns>
        public ClusterClientConfigurationBuilder WithoutPeriodicChecks()
        {
            Config.PeriodicChecksMode = PeriodicChecksMode.Disabled;
            Config.PeriodicChecksIntervalSecs = null;
            return this;
        }

        #endregion
        #region PubSub Subscriptions

        /// <summary>
        /// Configure PubSub subscriptions for the cluster client.
        /// </summary>
        /// <param name="config">The PubSub subscription configuration.</param>
        /// <returns>This configuration builder instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown when config is null.</exception>
        /// <exception cref="ArgumentException">Thrown when config is invalid.</exception>
        public ClusterClientConfigurationBuilder WithPubSubSubscriptions(ClusterPubSubSubscriptionConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);
            Config.PubSubSubscriptions = config;
            return this;
        }

        #endregion
    }
}

/// <summary>
/// Internal helpers for <see cref="ConnectionConfiguration.ReadFromStrategy"/>.
/// </summary>
internal static class ReadFromStrategyExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> if the strategy requires an Availability Zone (AZ).
    /// </summary>
    internal static bool IsAzReadFromStrategy(this ConnectionConfiguration.ReadFromStrategy strategy) =>
        strategy is ConnectionConfiguration.ReadFromStrategy.AzAffinity
            or ConnectionConfiguration.ReadFromStrategy.AzAffinityReplicasAndPrimary;
}
