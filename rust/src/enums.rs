// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

/// The cache metric to retrieve from the client-side cache.
/// Must match the C# `FFI.CacheMetricsType` enum.
#[repr(u32)]
#[derive(Debug, Clone, Copy)]
pub enum CacheMetricsType {
    HitRate = 0,
    MissRate = 1,
    EntryCount = 2,
    Evictions = 3,
    Expirations = 4,
    TotalLookups = 5,
}

/// The periodic topology checks mode for cluster clients.
/// Must match [`glide_core::client::PeriodicCheck`] in glide-core.
#[repr(u32)]
#[derive(Clone, Copy)]
pub enum PeriodicChecksMode {
    Enabled = 0,
    Disabled = 1,
    ManualInterval = 2,
}

/// The push notification kind received from the server.
/// Must match [`redis::PushKind`] in glide-core.
#[repr(u32)]
#[derive(Clone, Copy, PartialEq, Eq)]
pub enum PushKind {
    Disconnection = 0,
    Other = 1,
    Invalidate = 2,
    Message = 3,
    PMessage = 4,
    SMessage = 5,
    Unsubscribe = 6,
    PUnsubscribe = 7,
    SUnsubscribe = 8,
    Subscribe = 9,
    PSubscribe = 10,
    SSubscribe = 11,
}

/// The command routing type for cluster clients.
/// Must match [`redis::cluster_routing::RoutingInfo`] in glide-core.
#[repr(C)]
#[derive(Clone, Copy)]
pub enum RouteType {
    Random = 0,
    AllNodes = 1,
    AllPrimaries = 2,
    SlotId = 3,
    SlotKey = 4,
    ByAddress = 5,
}

/// The AWS service type for IAM authentication.
/// Must match [`glide_core::iam::ServiceType`] in glide-core.
#[repr(C)]
#[derive(Clone, Copy)]
pub enum ServiceType {
    ElastiCache = 0,
    MemoryDB = 1,
}
