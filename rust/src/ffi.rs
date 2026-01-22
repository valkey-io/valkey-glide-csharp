// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

use std::{
    ffi::{CStr, c_char},
    slice::from_raw_parts,
};

use glide_core::{
    client::{
        AuthenticationInfo as CoreAuthenticationInfo, ConnectionRequest, ConnectionRetryStrategy,
        NodeAddress, ReadFrom as coreReadFrom, TlsMode,
    },
    request_type::RequestType,
};
use redis::{
    Cmd, Pipeline, PipelineRetryStrategy, Value,
    cluster_routing::{
        MultipleNodeRoutingInfo, ResponsePolicy, Routable, Route, RoutingInfo,
        SingleNodeRoutingInfo, SlotAddr,
    },
};

/// Convert raw C string to a rust string.
///
/// # Safety
///
/// * `ptr` must be able to be safely casted to a valid [`CStr`] via [`CStr::from_ptr`]. See the safety documentation of [`std::ffi::CStr::from_ptr`].
unsafe fn ptr_to_str(ptr: *const c_char) -> String {
    if !ptr.is_null() {
        unsafe { CStr::from_ptr(ptr) }.to_str().unwrap().into()
    } else {
        "".into()
    }
}

/// Convert raw C string to a rust string wrapped by [Option].
///
/// # Safety
///
/// * `ptr` must be able to be safely casted to a valid [`CStr`] via [`CStr::from_ptr`]. See the safety documentation of [`std::ffi::CStr::from_ptr`].
unsafe fn ptr_to_opt_str(ptr: *const c_char) -> Option<String> {
    if !ptr.is_null() {
        Some(unsafe { ptr_to_str(ptr) })
    } else {
        None
    }
}

/// A mirror of [`ConnectionRequest`] adopted for FFI.
#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub struct ConnectionConfig {
    pub address_count: usize,
    /// Pointer to an array.
    pub addresses: *const *const Address,
    pub has_tls: bool,
    pub tls_mode: TlsMode,
    pub cluster_mode: bool,
    pub has_request_timeout: bool,
    pub request_timeout: u32,
    pub has_connection_timeout: bool,
    pub connection_timeout: u32,
    pub has_read_from: bool,
    pub read_from: ReadFrom,
    pub has_connection_retry_strategy: bool,
    pub connection_retry_strategy: ConnectionRetryStrategy,
    pub has_authentication_info: bool,
    pub authentication_info: AuthenticationInfo,
    pub database_id: u32,
    pub has_protocol: bool,
    pub protocol: redis::ProtocolVersion,
    /// zero pointer is valid, means no client name is given (`None`)
    pub client_name: *const c_char,
    pub lazy_connect: bool,
    pub refresh_topology_from_initial_nodes: bool,
    pub has_pubsub_config: bool,
    pub pubsub_config: PubSubConfigInfo,

    // Root certificates for TLS connections
    pub root_certs_count: usize,
    pub root_certs: *const *const u8,
    pub root_certs_len: *const usize,
    /*
    TODO below
    pub periodic_checks: Option<PeriodicCheck>,
    pub inflight_requests_limit: Option<u32>
    */
}

#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub struct PubSubConfigInfo {
    pub channels_ptr: *const *const c_char,
    pub channel_count: u32,
    pub patterns_ptr: *const *const c_char,
    pub pattern_count: u32,
    pub sharded_channels_ptr: *const *const c_char,
    pub sharded_channel_count: u32,
}

/// Convert a C string array to a Vec of Vec<u8>
///
/// # Safety
///
/// * `ptr` must point to an array of `count` valid C string pointers
/// * Each C string pointer must be valid and null-terminated
unsafe fn convert_string_array(ptr: *const *const c_char, count: u32) -> Vec<Vec<u8>> {
    if ptr.is_null() || count == 0 {
        return Vec::new();
    }

    let slice = unsafe { std::slice::from_raw_parts(ptr, count as usize) };
    slice
        .iter()
        .map(|&str_ptr| {
            let c_str = unsafe { CStr::from_ptr(str_ptr) };
            c_str.to_bytes().to_vec()
        })
        .collect()
}

/// Convert PubSubConfigInfo to the format expected by glide-core
///
/// # Safety
///
/// * All pointers in `config` must be valid or null
/// * String arrays must contain valid C strings
unsafe fn convert_pubsub_config(
    config: &PubSubConfigInfo,
) -> std::collections::HashMap<redis::PubSubSubscriptionKind, std::collections::HashSet<Vec<u8>>> {
    use redis::PubSubSubscriptionKind;
    use std::collections::{HashMap, HashSet};

    let mut subscriptions = HashMap::new();

    // Convert exact channels
    if config.channel_count > 0 {
        let channels = unsafe { convert_string_array(config.channels_ptr, config.channel_count) };
        subscriptions.insert(
            PubSubSubscriptionKind::Exact,
            channels.into_iter().collect::<HashSet<_>>(),
        );
    }

    // Convert patterns
    if config.pattern_count > 0 {
        let patterns = unsafe { convert_string_array(config.patterns_ptr, config.pattern_count) };
        subscriptions.insert(
            PubSubSubscriptionKind::Pattern,
            patterns.into_iter().collect::<HashSet<_>>(),
        );
    }

    // Convert sharded channels
    if config.sharded_channel_count > 0 {
        let sharded = unsafe {
            convert_string_array(config.sharded_channels_ptr, config.sharded_channel_count)
        };
        subscriptions.insert(
            PubSubSubscriptionKind::Sharded,
            sharded.into_iter().collect::<HashSet<_>>(),
        );
    }

    subscriptions
}

/// Convert connection configuration to a corresponding object.
///
/// # Safety
///
/// * `config_ptr` must not be `null`.
/// * `config_ptr` must be a valid pointer to a [`ConnectionConfig`] struct.
/// * Dereferenced [`ConnectionConfig`] struct and all nested structs must contain valid pointers.
///   See the safety documentation of [`convert_node_addresses`], [`ptr_to_str`] and [`ptr_to_opt_str`].
pub(crate) unsafe fn create_connection_request(
    config_ptr: *const ConnectionConfig,
) -> ConnectionRequest {
    let config = unsafe { *config_ptr };
    ConnectionRequest {
        read_from: if config.has_read_from {
            Some(match config.read_from.strategy {
                ReadFromStrategy::Primary => coreReadFrom::Primary,
                ReadFromStrategy::PreferReplica => coreReadFrom::PreferReplica,
                ReadFromStrategy::AZAffinity => {
                    coreReadFrom::AZAffinity(unsafe { ptr_to_str(config.read_from.az) })
                }
                ReadFromStrategy::AZAffinityReplicasAndPrimary => {
                    coreReadFrom::AZAffinityReplicasAndPrimary(unsafe {
                        ptr_to_str(config.read_from.az)
                    })
                }
            })
        } else {
            None
        },
        client_name: unsafe { ptr_to_opt_str(config.client_name) },
        lib_name: option_env!("GLIDE_NAME").map(|s| s.to_string()),
        authentication_info: if config.has_authentication_info {
            let auth_info = config.authentication_info;
            let iam_config = if auth_info.has_iam_credentials {
                Some(glide_core::client::IamAuthenticationConfig {
                    cluster_name: unsafe { ptr_to_str(auth_info.iam_credentials.cluster_name) },
                    region: unsafe { ptr_to_str(auth_info.iam_credentials.region) },
                    service_type: match auth_info.iam_credentials.service_type {
                        ServiceType::ElastiCache => glide_core::iam::ServiceType::ElastiCache,
                        ServiceType::MemoryDB => glide_core::iam::ServiceType::MemoryDB,
                    },
                    refresh_interval_seconds: if auth_info
                        .iam_credentials
                        .has_refresh_interval_seconds
                    {
                        Some(auth_info.iam_credentials.refresh_interval_seconds)
                    } else {
                        None
                    },
                })
            } else {
                None
            };

            Some(CoreAuthenticationInfo {
                username: unsafe { ptr_to_opt_str(auth_info.username) },
                password: unsafe { ptr_to_opt_str(auth_info.password) },
                iam_config,
            })
        } else {
            None
        },
        database_id: config.database_id.into(),
        protocol: if config.has_protocol {
            Some(config.protocol)
        } else {
            None
        },
        tls_mode: if config.has_tls {
            Some(config.tls_mode)
        } else {
            None
        },
        addresses: unsafe { convert_node_addresses(config.addresses, config.address_count) },
        cluster_mode_enabled: config.cluster_mode,
        request_timeout: if config.has_request_timeout {
            Some(config.request_timeout)
        } else {
            None
        },
        connection_timeout: if config.has_connection_timeout {
            Some(config.connection_timeout)
        } else {
            None
        },
        connection_retry_strategy: if config.has_connection_retry_strategy {
            Some(config.connection_retry_strategy)
        } else {
            None
        },
        lazy_connect: config.lazy_connect,
        refresh_topology_from_initial_nodes: config.refresh_topology_from_initial_nodes,
        pubsub_subscriptions: if config.has_pubsub_config {
            let subscriptions = unsafe { convert_pubsub_config(&config.pubsub_config) };
            if subscriptions.is_empty() {
                None
            } else {
                Some(subscriptions)
            }
        } else {
            None
        },
        root_certs: unsafe {
            convert_byte_array_to_owned(
                config.root_certs,
                config.root_certs_count,
                config.root_certs_len,
            )
        },

        // Unimplemented configuration options.
        client_cert: Vec::new(),
        client_key: Vec::new(),
        compression_config: None,
        tcp_nodelay: false,
        pubsub_reconciliation_interval_ms: None,
        periodic_checks: None,
        inflight_requests_limit: None,
    }
}

/// A mirror of [`NodeAddress`] adopted for FFI.
#[repr(C)]
pub struct Address {
    pub host: *const c_char,
    pub port: u16,
}

impl From<&Address> for NodeAddress {
    fn from(addr: &Address) -> Self {
        NodeAddress {
            host: unsafe { ptr_to_str(addr.host) },
            port: addr.port,
        }
    }
}

/// Convert raw array pointer of [`Address`]es to a vector of [`NodeAddress`]es.
///
/// # Safety
///
/// * `len` must not be greater than [`isize::MAX`]. See the safety documentation of [`std::slice::from_raw_parts`].
/// * `data` must not be `null`.
/// * `data` must point to `len` consecutive properly initialized [`Address`] structs.
/// * Each [`Address`] dereferenced by `data` must contain a valid string pointer. See the safety documentation of [`ptr_to_str`].
unsafe fn convert_node_addresses(data: *const *const Address, len: usize) -> Vec<NodeAddress> {
    unsafe { std::slice::from_raw_parts(data as *mut Address, len) }
        .iter()
        .map(NodeAddress::from)
        .collect()
}

/// A mirror of [`coreReadFrom`] adopted for FFI.
#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub struct ReadFrom {
    pub strategy: ReadFromStrategy,
    pub az: *const c_char,
}

#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub enum ReadFromStrategy {
    Primary,
    PreferReplica,
    AZAffinity,
    AZAffinityReplicasAndPrimary,
}

/// A mirror of [`AuthenticationInfo`] adopted for FFI.
#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub struct AuthenticationInfo {
    pub username: *const c_char,
    pub password: *const c_char,
    pub has_iam_credentials: bool,
    pub iam_credentials: IamCredentials,
}

/// A mirror of [`IamCredentials`] adopted for FFI.
#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub struct IamCredentials {
    pub cluster_name: *const c_char,
    pub region: *const c_char,
    pub service_type: ServiceType,
    pub has_refresh_interval_seconds: bool,
    pub refresh_interval_seconds: u32,
}

#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub enum ServiceType {
    ElastiCache = 0,
    MemoryDB = 1,
}

#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub enum RouteType {
    Random,
    AllNodes,
    AllPrimaries,
    SlotId,
    SlotKey,
    ByAddress,
}

/// A mirror of [`SlotAddr`]
#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub enum SlotType {
    Primary,
    Replica,
}

impl From<&SlotType> for SlotAddr {
    fn from(val: &SlotType) -> Self {
        match val {
            SlotType::Primary => SlotAddr::Master,
            SlotType::Replica => SlotAddr::ReplicaRequired,
        }
    }
}

/// A structure which represents a route. To avoid extra pointer mandgling, it has fields for all route types.
/// Depending on [`RouteType`], the struct stores:
/// * Only `route_type` is filled, if route is a simple route;
/// * `route_type`, `slot_id` and `slot_type`, if route is a Slot ID route;
/// * `route_type`, `slot_key` and `slot_type`, if route is a Slot key route;
/// * `route_type`, `hostname` and `port`, if route is a Address route;
#[repr(C)]
#[derive(Debug, Clone, Copy)]
pub struct RouteInfo {
    pub route_type: RouteType,
    pub slot_id: i32,
    /// zero pointer is valid, means no slot key is given (`None`)
    pub slot_key: *const c_char,
    pub slot_type: SlotType,
    /// zero pointer is valid, means no hostname is given (`None`)
    pub hostname: *const c_char,
    pub port: i32,
}

/// Convert route configuration to a corresponding object.
///
/// # Safety
/// * `route_ptr` could be `null`, but if it is not `null`, it must be a valid pointer to a [`RouteInfo`] struct.
/// * `slot_key` and `hostname` in dereferenced [`RouteInfo`] struct must contain valid string pointers when corresponding `route_type` is set.
///   See description of [`RouteInfo`] and the safety documentation of [`ptr_to_str`].
pub(crate) unsafe fn create_route(
    route_ptr: *const RouteInfo,
    cmd: Option<&Cmd>,
) -> Option<RoutingInfo> {
    if route_ptr.is_null() {
        return None;
    }
    let route = unsafe { *route_ptr };
    match route.route_type {
        RouteType::Random => Some(RoutingInfo::SingleNode(SingleNodeRoutingInfo::Random)),
        RouteType::AllNodes => Some(RoutingInfo::MultiNode((
            MultipleNodeRoutingInfo::AllNodes,
            cmd.and_then(|c| ResponsePolicy::for_command(&c.command().unwrap())),
        ))),
        RouteType::AllPrimaries => Some(RoutingInfo::MultiNode((
            MultipleNodeRoutingInfo::AllMasters,
            cmd.and_then(|c| ResponsePolicy::for_command(&c.command().unwrap())),
        ))),
        RouteType::SlotId => Some(RoutingInfo::SingleNode(
            SingleNodeRoutingInfo::SpecificNode(Route::new(
                route.slot_id as u16,
                (&route.slot_type).into(),
            )),
        )),
        RouteType::SlotKey => Some(RoutingInfo::SingleNode(
            SingleNodeRoutingInfo::SpecificNode(Route::new(
                redis::cluster_topology::get_slot(unsafe { ptr_to_str(route.slot_key) }.as_bytes()),
                (&route.slot_type).into(),
            )),
        )),
        RouteType::ByAddress => Some(RoutingInfo::SingleNode(SingleNodeRoutingInfo::ByAddress {
            host: unsafe { ptr_to_str(route.hostname) },
            port: route.port as u16,
        })),
    }
}

/// Converts a double pointer to borrowed byte slices.
///
/// # Safety
///
/// * `data` and `data_len` must not be `null`.
/// * `data` must point to `len` consecutive byte array pointers.
/// * `data_len` must point to `len` consecutive array lengths.
/// * `data`, `data_len` and also each pointer stored in `data` must be able to be safely casted to a valid slice via [`from_raw_parts`].
///   See the safety documentation of [`from_raw_parts`].
/// * The returned slices borrow from the caller's memory and must not outlive it.
pub(crate) unsafe fn convert_byte_array_to_slices<'a>(
    data: *const *const u8,
    len: usize,
    data_len: *const usize,
) -> Vec<&'a [u8]> {
    if len == 0 {
        return Vec::new();
    }

    let array_ptrs = unsafe { from_raw_parts(data, len) };
    let array_lengths = unsafe { from_raw_parts(data_len, len) };
    let mut result = Vec::with_capacity(array_ptrs.len());
    for (i, &arr_ptr) in array_ptrs.iter().enumerate() {
        let slice = unsafe { from_raw_parts(arr_ptr, array_lengths[i]) };
        result.push(slice);
    }
    result
}

/// Converts a double pointer to owned byte vectors by copying the data.
///
/// # Safety
/// * See the safety documentation of [`convert_byte_array_to_slices`].
/// * The returned vectors own their data and are safe to use after the caller's memory is freed.
pub(crate) unsafe fn convert_byte_array_to_owned(
    data: *const *const u8,
    len: usize,
    data_len: *const usize,
) -> Vec<Vec<u8>> {
    unsafe { convert_byte_array_to_slices(data, len, data_len) }
        .into_iter()
        .map(|slice| slice.to_vec())
        .collect()
}

pub(crate) fn convert_vec_to_pointer<T>(mut vec: Vec<T>) -> (*const T, usize) {
    vec.shrink_to_fit();
    let vec_ptr = vec.as_ptr();
    let len = vec.len();
    std::mem::forget(vec);
    (vec_ptr, len)
}

#[repr(C)]
#[derive(Default, Debug, Clone)]
pub enum ValueType {
    #[default]
    Null = 0,
    Int = 1,
    Float = 2,
    Bool = 3,
    String = 4,
    Array = 5,
    Map = 6,
    Set = 7,
    BulkString = 8,
    OK = 9,
    Error = 10,
}

/// Represents FFI-safe variant of [`Value`].
/// * For [`Value::Nil`] and [`Value::Okay`], only [`ResponseValue::typ`] is stored.
/// * Simple values such as [`Value::Int`], [`Value::Double`], and [`Value::Boolean`] are stored in [`ResponseValue::val`],
///   while corresponding [`ResponseValue::typ`] is set.
/// * For complex values, such as [`Value::BulkString`], [`Value::VerbatimString`], [`Value::SimpleString`], only a pointer
///   is stored in [`ResponseValue::val`], while corresponding [`ResponseValue::typ`] and [`ResponseValue::size`] are set.
/// * Way more complex types are stored by reference. For [`Value::Array`], [`Value::Set`] and [`Value::Map`], in
///   [`ResponseValue::val`] a pointer to an array of another [`ResponseValue`] is stored and [`ResponseValue::size`] contains
///   the array length (for a map - it is 2x map size).
#[repr(C)]
#[derive(Default, Debug, Clone)]
pub struct ResponseValue {
    pub typ: ValueType,
    pub val: i64,
    /// For [`Value::BulkString`], [`Value::VerbatimString`], [`Value::SimpleString`] - size in bytes.
    /// For Maps, sets and arrays - amount of values [`ResponseValue::val`] points to.
    pub size: u32,
}

impl ResponseValue {
    /// Build [`ResponseValue`] from a [`Value`].
    pub(crate) fn from_value(value: Value) -> Self {
        match value {
            Value::Nil => ResponseValue {
                typ: ValueType::Null,
                ..Default::default()
            },
            Value::Int(int) => ResponseValue {
                typ: ValueType::Int,
                val: int,
                size: 0,
            },
            Value::BulkString(text) => {
                let (vec_ptr, len) = convert_vec_to_pointer(text);
                ResponseValue {
                    typ: ValueType::BulkString,
                    val: vec_ptr as i64,
                    size: len as u32,
                }
            }
            Value::Array(values) => {
                let vec: Vec<ResponseValue> =
                    values.into_iter().map(ResponseValue::from_value).collect();
                let (vec_ptr, len) = convert_vec_to_pointer(vec);
                ResponseValue {
                    typ: ValueType::Array,
                    val: vec_ptr as i64,
                    size: len as u32,
                }
            }
            Value::Set(values) => {
                let vec: Vec<ResponseValue> =
                    values.into_iter().map(ResponseValue::from_value).collect();
                let (vec_ptr, len) = convert_vec_to_pointer(vec);
                ResponseValue {
                    typ: ValueType::Set,
                    val: vec_ptr as i64,
                    size: len as u32,
                }
            }
            Value::Okay => ResponseValue {
                typ: ValueType::OK,
                ..Default::default()
            },
            Value::Map(items) => {
                let vec: Vec<ResponseValue> = items
                    .into_iter()
                    .flat_map(|(k, v)| {
                        vec![ResponseValue::from_value(k), ResponseValue::from_value(v)]
                    })
                    .collect();
                let (vec_ptr, len) = convert_vec_to_pointer(vec);
                ResponseValue {
                    typ: ValueType::Map,
                    val: vec_ptr as i64,
                    size: len as u32,
                }
            }
            Value::Double(num) => ResponseValue {
                typ: ValueType::Float,
                val: num.to_bits() as i64,
                size: 0,
            },
            Value::Boolean(boolean) => ResponseValue {
                typ: ValueType::Bool,
                val: if boolean { 1 } else { 0 },
                size: 0,
            },
            Value::VerbatimString { format: _, text } | Value::SimpleString(text) => {
                let (vec_ptr, len) = convert_vec_to_pointer(text.into_bytes());
                ResponseValue {
                    typ: ValueType::String,
                    val: vec_ptr as i64,
                    size: len as u32,
                }
            }
            Value::ServerError(err) => {
                let (vec_ptr, len) =
                    convert_vec_to_pointer(err.details().unwrap().as_bytes().to_vec());
                ResponseValue {
                    typ: ValueType::Error,
                    val: vec_ptr as i64,
                    size: len as u32,
                }
            }
            _ => todo!(), // push, bigint, attribute
        }
    }

    /// Restore ownership and free all memory allocated by the current [`ResponseValue`] and referenced [`ResponseValue`] recursively.
    ///
    /// # Safety
    /// * [`ResponseValue::val`] must not be `null` if [`ResponseValue::typ`] is [`ValueType::Array`] or [`ValueType::Set`] or [`ValueType::Map`] or [`ValueType::String`] or [`ValueType::BulkString`].
    /// * [`ResponseValue::val`] must be able to be safely casted to a valid [`Vec<u8>`] (when [`ResponseValue::typ`] is [`ValueType::String`] or [`ValueType::BulkString`])
    ///   or [`Vec<ResponseValue>`] in other cases via [`Vec::from_raw_parts`]. See the safety documentation of [`Vec::from_raw_parts`].
    pub(crate) unsafe fn free_memory(&self) {
        match self.typ {
            ValueType::Array | ValueType::Set | ValueType::Map => {
                let vec = unsafe {
                    Vec::from_raw_parts(
                        self.val as *mut ResponseValue,
                        self.size as usize,
                        self.size as usize,
                    )
                };
                for val in vec {
                    unsafe { val.free_memory() };
                }
            }
            ValueType::String | ValueType::BulkString | ValueType::Error => {
                let _ = unsafe {
                    Vec::from_raw_parts(self.val as *mut u8, self.size as usize, self.size as usize)
                };
            }
            _ => (),
        }
    }
}

#[repr(C)]
#[derive(Clone, Debug, Copy)]
pub struct CmdInfo {
    pub request_type: RequestType,
    pub args: *const *const u8,
    pub arg_count: usize,
    pub args_len: *const usize,
}

#[repr(C)]
#[derive(Clone, Debug, Copy)]
pub struct BatchInfo {
    pub cmd_count: usize,
    pub cmds: *const *const CmdInfo,
    pub is_atomic: bool,
}

#[repr(C)]
#[derive(Clone, Debug, Copy)]
pub struct BatchOptionsInfo {
    // two params from PipelineRetryStrategy
    pub retry_server_error: bool,
    pub retry_connection_error: bool,
    pub has_timeout: bool,
    pub timeout: u32,
    pub route_info: *const RouteInfo,
}

/// Convert [`CmdInfo`] to a [`Cmd`].
///
/// # Safety
/// * `cmd_ptr` must be able to be safely casted to a valid [`CmdInfo`]
/// * `args` and `args_len` in a referred [`CmdInfo`] structure must not be `null`.
/// * `args` in a referred [`CmdInfo`] structure must point to `arg_count` consecutive byte array pointers.
/// * `args_len` in a referred [`CmdInfo`] structure must point to `arg_count` consecutive array lengths. See the safety documentation of [`convert_byte_array_to_slices`].
pub(crate) unsafe fn create_cmd(ptr: *const CmdInfo) -> Result<Cmd, String> {
    let info = unsafe { *ptr };
    let arg_vec = unsafe { convert_byte_array_to_slices(info.args, info.arg_count, info.args_len) };

    let Some(mut cmd) = info.request_type.get_command() else {
        return Err("Couldn't fetch command type".into());
    };
    for command_arg in arg_vec {
        cmd.arg(command_arg);
    }
    Ok(cmd)
}

/// Convert [`BatchInfo`] to a [`Pipeline`].
///
/// # Safety
/// * `ptr` must be able to be safely casted to a valid [`BatchInfo`].
/// * `cmds` in a referred [`BatchInfo`] structure must not be `null`.
/// * `cmds` in a referred [`BatchInfo`] structure must point to `cmd_count` consecutive [`CmdInfo`] pointers.
///   They must be able to be safely casted to a valid to a slice of the corresponding type via [`from_raw_parts`]. See the safety documentation of [`from_raw_parts`].
/// * Every pointer stored in `cmds` must not be `null` and must point to a valid [`CmdInfo`] structure.
/// * All data in referred [`CmdInfo`] structure(s) should be valid. See the safety documentation of [`create_cmd`].
pub(crate) unsafe fn create_pipeline(ptr: *const BatchInfo) -> Result<Pipeline, String> {
    let info = unsafe { *ptr };
    let cmd_pointers = unsafe { from_raw_parts(info.cmds, info.cmd_count) };
    let mut pipeline = Pipeline::with_capacity(info.cmd_count);
    for (i, cmd_ptr) in cmd_pointers.iter().enumerate() {
        match unsafe { create_cmd(*cmd_ptr) } {
            Ok(cmd) => pipeline.add_command(cmd),
            Err(err) => return Err(format!("Coudln't create {i:?}'th command: {err:?}")),
        };
    }
    if info.is_atomic {
        pipeline.atomic();
    }

    Ok(pipeline)
}

/// Convert [`BatchOptionsInfo`] to a tuple of corresponding values.
///
/// # Safety
/// * `ptr` could be `null`, but if it is not `null`, it must be a valid pointer to a [`BatchOptionsInfo`] struct.
/// * `route_info` in dereferenced [`BatchOptionsInfo`] struct must contain a [`RouteInfo`] pointer.
///   See description of [`RouteInfo`] and the safety documentation of [`create_route`].
pub(crate) unsafe fn get_pipeline_options(
    ptr: *const BatchOptionsInfo,
) -> (Option<RoutingInfo>, Option<u32>, PipelineRetryStrategy) {
    if ptr.is_null() {
        return (None, None, PipelineRetryStrategy::new(false, false));
    }
    let info = unsafe { *ptr };
    let timeout = if info.has_timeout {
        Some(info.timeout)
    } else {
        None
    };
    let route = unsafe { create_route(info.route_info, None) };

    (
        route,
        timeout,
        PipelineRetryStrategy::new(info.retry_server_error, info.retry_connection_error),
    )
}

/// FFI-safe version of [`redis::PushKind`] for C# interop.
/// This enum maps to the `PushKind` enum in `sources/Valkey.Glide/Internals/FFI.structs.cs`.
///
/// The `#[repr(u32)]` attribute ensures a stable memory layout compatible with C# marshaling.
/// Each variant corresponds to a specific Redis/Valkey PubSub notification type.
#[repr(u32)]
#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum PushKind {
    /// Disconnection notification sent from the library when connection is closed.
    Disconnection = 0,
    /// Other/unknown push notification type.
    Other = 1,
    /// Cache invalidation notification received when a key is changed/deleted.
    Invalidate = 2,
    /// Regular channel message received via SUBSCRIBE.
    Message = 3,
    /// Pattern-based message received via PSUBSCRIBE.
    PMessage = 4,
    /// Sharded channel message received via SSUBSCRIBE.
    SMessage = 5,
    /// Unsubscribe confirmation.
    Unsubscribe = 6,
    /// Pattern unsubscribe confirmation.
    PUnsubscribe = 7,
    /// Sharded unsubscribe confirmation.
    SUnsubscribe = 8,
    /// Subscribe confirmation.
    Subscribe = 9,
    /// Pattern subscribe confirmation.
    PSubscribe = 10,
    /// Sharded subscribe confirmation.
    SSubscribe = 11,
}

impl From<&redis::PushKind> for PushKind {
    fn from(kind: &redis::PushKind) -> Self {
        match kind {
            redis::PushKind::Disconnection => PushKind::Disconnection,
            redis::PushKind::Other(_) => PushKind::Other,
            redis::PushKind::Invalidate => PushKind::Invalidate,
            redis::PushKind::Message => PushKind::Message,
            redis::PushKind::PMessage => PushKind::PMessage,
            redis::PushKind::SMessage => PushKind::SMessage,
            redis::PushKind::Unsubscribe => PushKind::Unsubscribe,
            redis::PushKind::PUnsubscribe => PushKind::PUnsubscribe,
            redis::PushKind::SUnsubscribe => PushKind::SUnsubscribe,
            redis::PushKind::Subscribe => PushKind::Subscribe,
            redis::PushKind::PSubscribe => PushKind::PSubscribe,
            redis::PushKind::SSubscribe => PushKind::SSubscribe,
        }
    }
}

/// FFI callback function type for PubSub messages.
/// This callback is invoked by Rust when a PubSub message is received.
/// The callback signature matches the C# expectations for marshaling PubSub data.
///
/// # Parameters
/// * `push_kind` - The type of push notification. See [`PushKind`] for valid values.
/// * `message_ptr` - Pointer to the raw message bytes
/// * `message_len` - Length of the message data in bytes (unsigned, cannot be negative)
/// * `channel_ptr` - Pointer to the raw channel name bytes
/// * `channel_len` - Length of the channel name in bytes (unsigned, cannot be negative)
/// * `pattern_ptr` - Pointer to the raw pattern bytes (null if no pattern)
/// * `pattern_len` - Length of the pattern in bytes (unsigned, 0 if no pattern)
pub type PubSubCallback = unsafe extern "C" fn(
    push_kind: PushKind,
    message_ptr: *const u8,
    message_len: u64,
    channel_ptr: *const u8,
    channel_len: u64,
    pattern_ptr: *const u8,
    pattern_len: u64,
);
