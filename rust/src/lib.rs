// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

mod ffi;
use ffi::{
    BatchInfo, BatchOptionsInfo, CmdInfo, ConnectionConfig, PubSubCallback, PushKind,
    ResponseValue, RouteInfo, create_cmd, create_connection_request, create_pipeline, create_route,
    get_pipeline_options,
};
use glide_core::{
    GlideOpenTelemetry, GlideOpenTelemetryConfigBuilder, GlideOpenTelemetrySignalsExporter,
    GlideSpan,
    client::Client as GlideClient,
    errors::{RequestErrorType, error_message, error_type},
    request_type::RequestType,
};
use redis::cluster_routing::Routable;
use std::{
    ffi::{CStr, CString, c_char, c_void},
    slice::from_raw_parts,
    str::FromStr,
    sync::Arc,
};
use tokio::runtime::{Builder, Runtime};

#[repr(C)]
pub struct OpenTelemetryConfigFFI {
    pub has_traces: bool,
    pub traces: TracesConfigFFI,
    pub has_metrics: bool,
    pub metrics: MetricsConfigFFI,
    pub has_flush_interval: bool,
    pub flush_interval_ms: u32,
}

#[repr(C)]
pub struct TracesConfigFFI {
    pub endpoint: *const c_char,
    pub has_sample_percentage: bool,
    pub sample_percentage: u32,
}

#[repr(C)]
pub struct MetricsConfigFFI {
    pub endpoint: *const c_char,
}

#[repr(C)]
pub enum Level {
    Error = 0,
    Warn = 1,
    Info = 2,
    Debug = 3,
    Trace = 4,
    Off = 5,
}

pub struct Client {
    runtime: Runtime,
    core: Arc<CommandExecutionCore>,
    pubsub_shutdown: std::sync::Mutex<Option<tokio::sync::oneshot::Sender<()>>>,
    pubsub_task: std::sync::Mutex<Option<tokio::task::JoinHandle<()>>>,
}

/// Success callback that is called when a command succeeds.
///
/// The success callback needs to copy the given data synchronously, since it will be dropped by Rust once the callback returns.
/// The callback should be offloaded to a separate thread in order not to exhaust the client's thread pool.
///
/// # Arguments
/// * `index` is a baton-pass back to the caller language to uniquely identify the promise.
/// * `message` is the value returned by the command. The 'message' is managed by Rust and is freed when the callback returns control back to the caller.
///
/// # Safety
/// * The callback must copy the pointer in a sync manner and return ASAP. Any further data processing should be done in another thread to avoid
///   starving `tokio`'s thread pool.
/// * The callee is responsible to free memory by calling [`free_response`] with the given pointer once only.
pub type SuccessCallback = unsafe extern "C-unwind" fn(usize, *const ResponseValue) -> ();

/// Failure callback that is called when a command fails.
///
/// The failure callback needs to copy the given string synchronously, since it will be dropped by Rust once the callback returns.
/// The callback should be offloaded to a separate thread in order not to exhaust the client's thread pool.
///
/// # Arguments
/// * `index` is a baton-pass back to the caller language to uniquely identify the promise.
/// * `error_message` is an UTF-8 string storing the error message returned by server for the failed command.
///   The `error_message` is managed by Rust and is freed when the callback returns control back to the caller.
/// * `error_type` is the type of error returned by glide-core, depending on the [`RedisError`](redis::RedisError) returned.
///
/// # Safety
/// * The callback must copy the data in a sync manner and return ASAP. Any further data processing should be done in another thread to avoid
///   starving `tokio`'s thread pool.
/// * The caller must free the memory allocated for [`error_message`] right after the call to avoid memory leak.
pub type FailureCallback = unsafe extern "C-unwind" fn(
    index: usize,
    error_message: *const c_char,
    error_type: RequestErrorType,
) -> ();

struct CommandExecutionCore {
    client: GlideClient,
    success_callback: SuccessCallback,
    failure_callback: FailureCallback,
}

/// # Safety
/// Unsafe, because calls to an FFI function. See the safety documentation of [`FailureCallback`].
unsafe fn report_error(
    failure_callback: FailureCallback,
    callback_index: usize,
    error_string: String,
    error_type: RequestErrorType,
) {
    logger_core::log(logger_core::Level::Error, "ffi", &error_string);
    let err_ptr = CString::into_raw(
        CString::new(error_string).expect("Couldn't convert error message to CString"),
    );
    unsafe { failure_callback(callback_index, err_ptr, error_type) };
    // free memory
    _ = unsafe { CString::from_raw(err_ptr) };
}

/// Panic Guard as per <https://www.reddit.com/r/rust/comments/zg2xcu/comment/izi758v/>
struct PanicGuard {
    panicked: bool,
    failure_callback: FailureCallback,
    callback_index: usize,
}

impl Drop for PanicGuard {
    fn drop(&mut self) {
        if self.panicked {
            unsafe {
                report_error(
                    self.failure_callback,
                    self.callback_index,
                    "Native function panicked".into(),
                    RequestErrorType::Unspecified,
                );
            }
        }
    }
}

/// Creates a new client with the given configuration.
/// The success callback needs to copy the given string synchronously, since it will be dropped by Rust once the callback returns.
/// All callbacks should be offloaded to separate threads in order not to exhaust the client's thread pool.
///
/// # Safety
///
/// * `config` must be a valid [`ConnectionConfig`] pointer. See the safety documentation of [`create_connection_request`].
/// * `success_callback` and `failure_callback` must be valid pointers to the corresponding FFI functions.
///   See the safety documentation of [`SuccessCallback`] and [`FailureCallback`].
/// * `pubsub_callback` is an optional callback. When provided, it must be a valid function pointer.
///   See the safety documentation in the FFI module for PubSubCallback.
#[allow(rustdoc::private_intra_doc_links)]
#[unsafe(no_mangle)]
pub unsafe extern "C-unwind" fn create_client(
    config: *const ConnectionConfig,
    success_callback: SuccessCallback,
    failure_callback: FailureCallback,
    #[allow(unused_variables)] pubsub_callback: Option<PubSubCallback>,
) {
    let mut panic_guard = PanicGuard {
        panicked: true,
        failure_callback,
        callback_index: 0,
    };

    let request = unsafe { create_connection_request(config) };

    let runtime = Builder::new_multi_thread()
        .enable_all()
        .worker_threads(10)
        .thread_name("GLIDE C# thread")
        .build()
        .unwrap();

    let _runtime_handle = runtime.enter();

    // Set up push notification channel if PubSub subscriptions are configured
    // The callback is optional - users can use queue-based message retrieval instead
    let is_subscriber = request.pubsub_subscriptions.is_some();

    let (push_tx, mut push_rx) = tokio::sync::mpsc::unbounded_channel();
    let tx = if is_subscriber { Some(push_tx) } else { None };

    let res = runtime.block_on(GlideClient::new(request, tx));
    match res {
        Ok(client) => {
            let core = Arc::new(CommandExecutionCore {
                success_callback,
                failure_callback,
                client,
            });

            // Set up graceful shutdown coordination for PubSub task
            // Only spawn the callback task if a callback is provided
            let (pubsub_shutdown, pubsub_task) = if is_subscriber && pubsub_callback.is_some() {
                let callback = pubsub_callback.unwrap();
                let (shutdown_tx, mut shutdown_rx) = tokio::sync::oneshot::channel();

                let task_handle = runtime.spawn(async move {
                    logger_core::log(logger_core::Level::Info, "pubsub", "PubSub task started");

                    loop {
                        tokio::select! {
                            Some(push_msg) = push_rx.recv() => {
                                unsafe {
                                    process_push_notification(push_msg, callback);
                                }
                            }
                            _ = &mut shutdown_rx => {
                                logger_core::log(
                                    logger_core::Level::Info,
                                    "pubsub",
                                    "PubSub task received shutdown signal",
                                );
                                break;
                            }
                        }
                    }

                    logger_core::log(
                        logger_core::Level::Info,
                        "pubsub",
                        "PubSub task completed gracefully",
                    );
                });

                (
                    std::sync::Mutex::new(Some(shutdown_tx)),
                    std::sync::Mutex::new(Some(task_handle)),
                )
            } else {
                (std::sync::Mutex::new(None), std::sync::Mutex::new(None))
            };

            let client_adapter = Arc::new(Client {
                runtime,
                core,
                pubsub_shutdown,
                pubsub_task,
            });
            let client_ptr = Arc::into_raw(client_adapter.clone());

            unsafe { success_callback(0, client_ptr as *const ResponseValue) };
        }
        Err(err) => {
            unsafe {
                report_error(
                    failure_callback,
                    0,
                    err.to_string(),
                    RequestErrorType::Disconnect,
                )
            };
        }
    }

    panic_guard.panicked = false;
    drop(panic_guard);
}

/// Processes a push notification message and calls the provided callback function.
///
/// This function extracts the message data from the PushInfo and invokes the C# callback
/// with the appropriate parameters using scoped lifetime management to prevent memory leaks.
///
/// # Parameters
/// - `push_msg`: The push notification message to process.
/// - `pubsub_callback`: The callback function to invoke with the processed notification.
///
/// # Safety
/// This function is unsafe because it:
/// - Calls an FFI function (`pubsub_callback`) that may have undefined behavior
/// - Assumes push_msg.data contains valid BulkString values
///
/// The caller must ensure:
/// - `pubsub_callback` is a valid function pointer to a properly implemented callback
/// - The callback copies data synchronously before returning
///
/// # Memory Safety
/// This implementation uses scoped lifetime management instead of `std::mem::forget()`.
/// Vec<u8> instances are kept alive during callback execution and automatically cleaned up
/// when the function exits, preventing memory leaks.
unsafe fn process_push_notification(push_msg: redis::PushInfo, pubsub_callback: PubSubCallback) {
    use redis::Value;

    // Convert all values to Vec<u8>, handling both BulkString and Int types
    let strings: Vec<Vec<u8>> = push_msg
        .data
        .into_iter()
        .map(|value| match value {
            Value::BulkString(bytes) => bytes,
            Value::Int(num) => num.to_string().into_bytes(),
            Value::SimpleString(s) => s.into_bytes(),
            _ => {
                logger_core::log(
                    logger_core::Level::Warn,
                    "pubsub",
                    &format!("Unexpected value type in PubSub message: {:?}", value),
                );
                Vec::new()
            }
        })
        .collect();

    // Store the kind to avoid move issues
    let push_kind = push_msg.kind.clone();

    // Validate message structure based on PushKind and convert to FFI kind
    // The FFI PushKind enum is defined in ffi.rs and matches the C# PushKind enum in FFI.structs.cs
    let (pattern, channel, message, kind) = match (push_kind.clone(), strings.len()) {
        (redis::PushKind::Message, 2) => {
            // Regular message: [channel, message]
            (None, &strings[0], &strings[1], PushKind::Message)
        }
        (redis::PushKind::PMessage, 3) => {
            // Pattern message: [pattern, channel, message]
            (
                Some(&strings[0]),
                &strings[1],
                &strings[2],
                PushKind::PMessage,
            )
        }
        (redis::PushKind::SMessage, 2) => {
            // Sharded message: [channel, message]
            (None, &strings[0], &strings[1], PushKind::SMessage)
        }
        (redis::PushKind::Subscribe, 2) => {
            // Subscribe confirmation: [channel, count]
            (None, &strings[0], &strings[1], PushKind::Subscribe)
        }
        (redis::PushKind::PSubscribe, 3) => {
            // Pattern subscribe confirmation: [pattern, channel, count]
            (
                Some(&strings[0]),
                &strings[1],
                &strings[2],
                PushKind::PSubscribe,
            )
        }
        (redis::PushKind::SSubscribe, 2) => {
            // Sharded subscribe confirmation: [channel, count]
            (None, &strings[0], &strings[1], PushKind::SSubscribe)
        }
        (redis::PushKind::Unsubscribe, 2) => {
            // Unsubscribe confirmation: [channel, count]
            (None, &strings[0], &strings[1], PushKind::Unsubscribe)
        }
        (redis::PushKind::PUnsubscribe, 3) => {
            // Pattern unsubscribe confirmation: [pattern, channel, count]
            (
                Some(&strings[0]),
                &strings[1],
                &strings[2],
                PushKind::PUnsubscribe,
            )
        }
        (redis::PushKind::SUnsubscribe, 2) => {
            // Sharded unsubscribe confirmation: [channel, count]
            (None, &strings[0], &strings[1], PushKind::SUnsubscribe)
        }
        (redis::PushKind::Disconnection, _) => {
            logger_core::log(
                logger_core::Level::Info,
                "pubsub",
                "PubSub disconnection received",
            );
            return;
        }
        (kind, len) => {
            logger_core::log(
                logger_core::Level::Error,
                "pubsub",
                &format!(
                    "Invalid PubSub message structure: kind={:?}, len={}",
                    kind, len
                ),
            );
            return;
        }
    };

    // Prepare pointers while keeping strings alive
    let pattern_ptr = pattern.map(|p| p.as_ptr()).unwrap_or(std::ptr::null());
    let pattern_len = pattern.map(|p| p.len() as u64).unwrap_or(0);
    let channel_ptr = channel.as_ptr();
    let channel_len = channel.len() as u64;
    let message_ptr = message.as_ptr();
    let message_len = message.len() as u64;

    // Call callback while strings are still alive
    unsafe {
        pubsub_callback(
            kind,
            message_ptr,
            message_len,
            channel_ptr,
            channel_len,
            pattern_ptr,
            pattern_len,
        );
    }

    // Vec<u8> instances are automatically cleaned up here
    // No memory leak, no use-after-free
}

/// Closes the given client, deallocating it from the heap.
/// This function should only be called once per pointer created by [`create_client`].
/// After calling this function the `client_ptr` is not in a valid state.
///
/// Implements graceful shutdown coordination for PubSub tasks with timeout.
///
/// # Safety
///
/// * `client_ptr` must not be `null`.
/// * `client_ptr` must be able to be safely casted to a valid [`Arc<Client>`] via [`Arc::from_raw`]. See the safety documentation of [`Arc::from_raw`].
#[unsafe(no_mangle)]
pub extern "C" fn close_client(client_ptr: *const c_void) {
    assert!(!client_ptr.is_null());

    // Get a reference to the client to access shutdown coordination
    let client = unsafe { &*(client_ptr as *const Client) };

    // Take ownership of shutdown sender and signal graceful shutdown
    if let Ok(mut guard) = client.pubsub_shutdown.lock() {
        if let Some(shutdown_tx) = guard.take() {
            logger_core::log(
                logger_core::Level::Debug,
                "pubsub",
                "Signaling PubSub task to shutdown",
            );

            // Send shutdown signal (ignore error if receiver already dropped)
            let _ = shutdown_tx.send(());
        }
    }

    // Take ownership of task handle and wait for completion with timeout
    if let Ok(mut guard) = client.pubsub_task.lock() {
        if let Some(task_handle) = guard.take() {
            let timeout = std::time::Duration::from_secs(5);

            logger_core::log(
                logger_core::Level::Debug,
                "pubsub",
                &format!(
                    "Waiting for PubSub task to complete (timeout: {:?})",
                    timeout
                ),
            );

            let result = client
                .runtime
                .block_on(async { tokio::time::timeout(timeout, task_handle).await });

            match result {
                Ok(Ok(())) => {
                    logger_core::log(
                        logger_core::Level::Info,
                        "pubsub",
                        "PubSub task completed successfully",
                    );
                }
                Ok(Err(e)) => {
                    logger_core::log(
                        logger_core::Level::Warn,
                        "pubsub",
                        &format!("PubSub task completed with error: {:?}", e),
                    );
                }
                Err(_) => {
                    logger_core::log(
                        logger_core::Level::Warn,
                        "pubsub",
                        &format!(
                            "PubSub task did not complete within timeout ({:?})",
                            timeout
                        ),
                    );
                }
            }
        }
    }

    // This will bring the strong count down to 0 once all client requests are done.
    unsafe { Arc::decrement_strong_count(client_ptr as *const Client) };
}

/// Execute a command.
///
/// # Safety
/// * `client_ptr` must not be `null`.
/// * `client_ptr` must be able to be safely casted to a valid [`Arc<Client>`] via [`Arc::from_raw`]. See the safety documentation of [`Arc::from_raw`].
/// * This function should only be called should with a pointer created by [`create_client`], before [`close_client`] was called with the pointer.
/// * Pointers to callbacks stored in [`Client`] should remain valid. See the safety documentation of [`SuccessCallback`] and [`FailureCallback`].
/// * `cmd_ptr` must not be `null`.
/// * `cmd_ptr` must be able to be safely casted to a valid [`CmdInfo`]. See the safety documentation of [`create_cmd`].
/// * `route_info` could be `null`, but if it is not `null`, it must be a valid [`RouteInfo`] pointer. See the safety documentation of [`create_route`].
#[allow(rustdoc::private_intra_doc_links)]
#[unsafe(no_mangle)]
pub unsafe extern "C-unwind" fn command(
    client_ptr: *const c_void,
    callback_index: usize,
    cmd_ptr: *const CmdInfo,
    route_info: *const RouteInfo,
) {
    let client = unsafe {
        // we increment the strong count to ensure that the client is not dropped just because we turned it into an Arc.
        Arc::increment_strong_count(client_ptr);
        Arc::from_raw(client_ptr as *mut Client)
    };
    let core = client.core.clone();

    let mut panic_guard = PanicGuard {
        panicked: true,
        failure_callback: core.failure_callback,
        callback_index,
    };

    let cmd = match unsafe { create_cmd(cmd_ptr) } {
        Ok(cmd) => cmd,
        Err(err) => {
            unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    err,
                    RequestErrorType::Unspecified,
                );
            }
            return;
        }
    };

    let route = unsafe { create_route(route_info, Some(&cmd)) };

    client.runtime.spawn(async move {
        let mut panic_guard = PanicGuard {
            panicked: true,
            failure_callback: core.failure_callback,
            callback_index,
        };

        let result = core.client.clone().send_command(&cmd, route).await;
        match result {
            Ok(value) => {
                let ptr = Box::into_raw(Box::new(ResponseValue::from_value(value)));
                unsafe { (core.success_callback)(callback_index, ptr) };
            }
            Err(err) => unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    error_message(&err),
                    error_type(&err),
                );
            },
        };
        panic_guard.panicked = false;
        drop(panic_guard);
    });

    panic_guard.panicked = false;
    drop(panic_guard);
}

/// Execute a batch.
///
/// # Safety
/// * `client_ptr` must not be `null`.
/// * `client_ptr` must be able to be safely casted to a valid [`Arc<Client>`] via [`Arc::from_raw`]. See the safety documentation of [`Arc::from_raw`].
/// * This function should only be called should with a pointer created by [`create_client`], before [`close_client`] was called with the pointer.
/// * `batch_ptr` must not be `null`.
/// * `batch_ptr` must be able to be safely casted to a valid [`BatchInfo`]. See the safety documentation of [`create_pipeline`].
/// * `options_ptr` could be `null`, but if it is not `null`, it must be a valid [`BatchOptionsInfo`] pointer. See the safety documentation of [`get_pipeline_options`].
#[allow(rustdoc::private_intra_doc_links)]
#[unsafe(no_mangle)]
pub unsafe extern "C-unwind" fn batch(
    client_ptr: *const c_void,
    callback_index: usize,
    batch_ptr: *const BatchInfo,
    raise_on_error: bool,
    options_ptr: *const BatchOptionsInfo,
) {
    let client = unsafe {
        // we increment the strong count to ensure that the client is not dropped just because we turned it into an Arc.
        Arc::increment_strong_count(client_ptr);
        Arc::from_raw(client_ptr as *mut Client)
    };
    let core = client.core.clone();

    let mut panic_guard = PanicGuard {
        panicked: true,
        failure_callback: core.failure_callback,
        callback_index,
    };

    let pipeline = match unsafe { create_pipeline(batch_ptr) } {
        Ok(pipeline) => pipeline,
        Err(err) => {
            unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    err,
                    RequestErrorType::Unspecified,
                );
            }
            return;
        }
    };

    let (routing, timeout, pipeline_retry_strategy) = unsafe { get_pipeline_options(options_ptr) };

    client.runtime.spawn(async move {
        let mut panic_guard = PanicGuard {
            panicked: true,
            failure_callback: core.failure_callback,
            callback_index,
        };

        let result = if pipeline.is_atomic() {
            core.client
                .clone()
                .send_transaction(&pipeline, routing, timeout, raise_on_error)
                .await
        } else {
            core.client
                .clone()
                .send_pipeline(
                    &pipeline,
                    routing,
                    raise_on_error,
                    timeout,
                    pipeline_retry_strategy,
                )
                .await
        };
        match result {
            Ok(value) => {
                let ptr = Box::into_raw(Box::new(ResponseValue::from_value(value)));
                unsafe { (core.success_callback)(callback_index, ptr) };
            }
            Err(err) => unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    error_message(&err),
                    error_type(&err),
                );
            },
        };
        panic_guard.panicked = false;
        drop(panic_guard);
    });

    panic_guard.panicked = false;
    drop(panic_guard);
}

/// Free the memory allocated for a [`ResponseValue`] and nested structure.
///
/// # Safety
/// * `ptr` must not be `null`.
/// * `ptr` must be able to be safely casted to a valid [`Box<ResponseValue>`] via [`Box::from_raw`]. See the safety documentation of [`Box::from_raw`].
#[allow(rustdoc::private_intra_doc_links)]
#[unsafe(no_mangle)]
pub unsafe extern "C" fn free_response(ptr: *mut ResponseValue) {
    unsafe {
        Box::leak(Box::from_raw(ptr)).free_memory();
    }
}

/// Frees memory allocated for a C string.
///
/// # Parameters
/// * `str_ptr`: Pointer to the C string to free.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn free_string(str_ptr: *mut c_char) {
    if !str_ptr.is_null() {
        unsafe { let _ = CString::from_raw(str_ptr); };
    }
}

impl From<logger_core::Level> for Level {
    fn from(level: logger_core::Level) -> Self {
        match level {
            logger_core::Level::Error => Level::Error,
            logger_core::Level::Warn => Level::Warn,
            logger_core::Level::Info => Level::Info,
            logger_core::Level::Debug => Level::Debug,
            logger_core::Level::Trace => Level::Trace,
            logger_core::Level::Off => Level::Off,
        }
    }
}

impl From<Level> for logger_core::Level {
    fn from(level: Level) -> logger_core::Level {
        match level {
            Level::Error => logger_core::Level::Error,
            Level::Warn => logger_core::Level::Warn,
            Level::Info => logger_core::Level::Info,
            Level::Debug => logger_core::Level::Debug,
            Level::Trace => logger_core::Level::Trace,
            Level::Off => logger_core::Level::Off,
        }
    }
}

/// # Safety
///
/// * `message` and `log_identifier` must not be `null`.
/// * `message` and `log_identifier` must be able to be safely casted to a valid [`CStr`] via [`CStr::from_ptr`]. See the safety documentation of [`CStr::from_ptr`].
#[unsafe(no_mangle)]
#[allow(improper_ctypes_definitions)]
pub unsafe extern "C" fn log(
    log_level: Level,
    log_identifier: *const c_char,
    message: *const c_char,
) {
    logger_core::log(
        log_level.into(),
        unsafe { CStr::from_ptr(log_identifier) }
            .to_str()
            .expect("Can not read log_identifier argument."),
        unsafe { CStr::from_ptr(message) }
            .to_str()
            .expect("Can not read message argument."),
    );
}

/// # Safety
///
/// * `file_name` must not be `null`.
/// * `file_name` must be able to be safely casted to a valid [`CStr`] via [`CStr::from_ptr`]. See the safety documentation of [`CStr::from_ptr`].
#[unsafe(no_mangle)]
#[allow(improper_ctypes_definitions)]
pub unsafe extern "C" fn init(level: Option<Level>, file_name: *const c_char) -> Level {
    let file_name_as_str = if file_name.is_null() {
        None
    } else {
        Some(
            unsafe { CStr::from_ptr(file_name) }
                .to_str()
                .expect("Can not read string argument."),
        )
    };

    let logger_level = logger_core::init(level.map(|level| level.into()), file_name_as_str);
    logger_level.into()
}

#[repr(C)]
pub struct ScriptHashBuffer {
    pub ptr: *mut u8,
    pub len: usize,
    pub capacity: usize,
}

/// Store a Lua script in the script cache and return its SHA1 hash.
///
/// # Parameters
///
/// * `script_bytes`: Pointer to the script bytes.
/// * `script_len`: Length of the script in bytes.
///
/// # Returns
///
/// A pointer to a `ScriptHashBuffer` containing the SHA1 hash of the script.
/// The caller is responsible for freeing this memory using [`free_script_hash_buffer`].
///
/// # Safety
///
/// * `script_bytes` must point to `script_len` consecutive properly initialized bytes.
/// * The returned buffer must be freed by the caller using [`free_script_hash_buffer`].
#[unsafe(no_mangle)]
pub unsafe extern "C" fn store_script(
    script_bytes: *const u8,
    script_len: usize,
) -> *mut ScriptHashBuffer {
    let script = unsafe { std::slice::from_raw_parts(script_bytes, script_len) };
    let hash = glide_core::scripts_container::add_script(script);
    let mut hash = std::mem::ManuallyDrop::new(hash);
    let script_hash_buffer = ScriptHashBuffer {
        ptr: hash.as_mut_ptr(),
        len: hash.len(),
        capacity: hash.capacity(),
    };
    Box::into_raw(Box::new(script_hash_buffer))
}

/// Free a `ScriptHashBuffer` obtained from [`store_script`].
///
/// # Parameters
///
/// * `buffer`: Pointer to the `ScriptHashBuffer`.
///
/// # Safety
///
/// * `buffer` must be a pointer returned from [`store_script`].
/// * This function must be called exactly once per buffer.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn free_script_hash_buffer(buffer: *mut ScriptHashBuffer) {
    if buffer.is_null() {
        return;
    }
    let buffer = unsafe { Box::from_raw(buffer) };
    let _hash = unsafe { String::from_raw_parts(buffer.ptr, buffer.len, buffer.capacity) };
}

/// Remove a script from the script cache.
///
/// Returns a null pointer if it succeeds and a C string error message if it fails.
///
/// # Parameters
///
/// * `hash`: The SHA1 hash of the script to remove as a byte array.
/// * `len`: The length of `hash`.
///
/// # Returns
///
/// A null pointer on success, or a pointer to a C string error message on failure.
/// The caller is responsible for freeing the error message using [`free_drop_script_error`].
///
/// # Safety
///
/// * `hash` must be a valid pointer to a UTF-8 string.
/// * The returned error pointer (if not null) must be freed using [`free_drop_script_error`].
#[unsafe(no_mangle)]
pub unsafe extern "C" fn drop_script(hash: *mut u8, len: usize) -> *mut c_char {
    if hash.is_null() {
        return CString::new("Hash pointer was null.").unwrap().into_raw();
    }

    let slice = std::ptr::slice_from_raw_parts_mut(hash, len);
    let Ok(hash_str) = std::str::from_utf8(unsafe { &*slice }) else {
        return CString::new("Unable to convert hash to UTF-8 string.")
            .unwrap()
            .into_raw();
    };

    glide_core::scripts_container::remove_script(hash_str);
    std::ptr::null_mut()
}

/// Executes a Lua script using EVALSHA with automatic fallback to EVAL.
///
/// # Parameters
///
/// * `client_ptr`: Pointer to a valid `GlideClient` returned from [`create_client`].
/// * `callback_index`: Unique identifier for the callback.
/// * `hash`: SHA1 hash of the script as a null-terminated C string.
/// * `keys_count`: Number of keys in the keys array.
/// * `keys`: Array of pointers to key data.
/// * `keys_len`: Array of key lengths.
/// * `args_count`: Number of arguments in the args array.
/// * `args`: Array of pointers to argument data.
/// * `args_len`: Array of argument lengths.
/// * `route_bytes`: Optional routing information (not used, reserved for future).
/// * `route_bytes_len`: Length of route_bytes.
///
/// # Safety
///
/// * `client_ptr` must not be `null` and must be obtained from [`create_client`].
/// * `hash` must be a valid null-terminated C string.
/// * `keys` and `keys_len` must be valid arrays of size `keys_count`, or both null if `keys_count` is 0.
/// * `args` and `args_len` must be valid arrays of size `args_count`, or both null if `args_count` is 0.
#[unsafe(no_mangle)]
pub unsafe extern "C-unwind" fn invoke_script(
    client_ptr: *const c_void,
    callback_index: usize,
    hash: *const c_char,
    keys_count: usize,
    keys: *const usize,
    keys_len: *const usize,
    args_count: usize,
    args: *const usize,
    args_len: *const usize,
    _route_bytes: *const u8,
    _route_bytes_len: usize,
) {
    let client = unsafe {
        Arc::increment_strong_count(client_ptr);
        Arc::from_raw(client_ptr as *mut Client)
    };
    let core = client.core.clone();

    let mut panic_guard = PanicGuard {
        panicked: true,
        failure_callback: core.failure_callback,
        callback_index,
    };

    // Convert hash to Rust string
    let hash_str = match unsafe { CStr::from_ptr(hash).to_str() } {
        Ok(s) => s.to_string(),
        Err(e) => {
            unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    format!("Invalid hash string: {}", e),
                    RequestErrorType::Unspecified,
                );
            }
            return;
        }
    };

    // Convert keys
    let keys_vec: Vec<&[u8]> = if !keys.is_null() && !keys_len.is_null() && keys_count > 0 {
        unsafe {
            ffi::convert_string_pointer_array_to_vector(
                keys as *const *const u8,
                keys_count,
                keys_len,
            )
        }
    } else {
        Vec::new()
    };

    // Convert args
    let args_vec: Vec<&[u8]> = if !args.is_null() && !args_len.is_null() && args_count > 0 {
        unsafe {
            ffi::convert_string_pointer_array_to_vector(
                args as *const *const u8,
                args_count,
                args_len,
            )
        }
    } else {
        Vec::new()
    };

    client.runtime.spawn(async move {
        let mut panic_guard = PanicGuard {
            panicked: true,
            failure_callback: core.failure_callback,
            callback_index,
        };

        let result = core
            .client
            .clone()
            .invoke_script(&hash_str, &keys_vec, &args_vec, None)
            .await;

        match result {
            Ok(value) => {
                let ptr = Box::into_raw(Box::new(ResponseValue::from_value(value)));
                unsafe { (core.success_callback)(callback_index, ptr) };
            }
            Err(err) => unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    error_message(&err),
                    error_type(&err),
                );
            },
        };
        panic_guard.panicked = false;
        drop(panic_guard);
    });

    panic_guard.panicked = false;
    drop(panic_guard);
}

/// Execute a cluster scan request.
///
/// # Safety
/// * `client_ptr` must be a valid Client pointer from create_client
/// * `cursor` must be "0" for initial scan or a valid cursor ID from previous scan
/// * `args` and `arg_lengths` must be valid arrays of length `arg_count`
/// * `args` format: [b"MATCH", pattern_arg, b"COUNT", count, b"TYPE", type] (all optional)
#[unsafe(no_mangle)]
pub unsafe extern "C-unwind" fn request_cluster_scan(
    client_ptr: *const c_void,
    callback_index: usize,
    cursor: *const c_char,
    arg_count: u64,
    args: *const usize,
    arg_lengths: *const u64,
) {
    // Build client and add panic guard.
    let client = unsafe {
        Arc::increment_strong_count(client_ptr);
        Arc::from_raw(client_ptr as *mut Client)
    };
    let core = client.core.clone();

    let mut panic_guard = PanicGuard {
        panicked: true,
        failure_callback: core.failure_callback,
        callback_index,
    };

    // Get the cluster scan state.
    let cursor_id = unsafe { CStr::from_ptr(cursor) }
        .to_str()
        .unwrap_or("0")
        .to_owned();

    let scan_state_cursor = if cursor_id == "0" {
        redis::ScanStateRC::new()
    } else {
        match glide_core::cluster_scan_container::get_cluster_scan_cursor(cursor_id.clone()) {
            Ok(existing_cursor) => existing_cursor,
            Err(_error) => {
                unsafe {
                    report_error(
                        core.failure_callback,
                        callback_index,
                        format!("Invalid cursor ID: {}", cursor_id),
                        RequestErrorType::Unspecified,
                    );
                }
                panic_guard.panicked = false;
                return;
            }
        }
    };

    // Build cluster scan arguments.
    let cluster_scan_args = match unsafe {
        build_cluster_scan_args(
            arg_count,
            args,
            arg_lengths,
            core.failure_callback,
            callback_index,
        )
    } {
        Some(args) => args,
        None => {
            panic_guard.panicked = false;
            return;
        }
    };

    // Run cluster scan.
    client.runtime.spawn(async move {
        let mut async_panic_guard = PanicGuard {
            panicked: true,
            failure_callback: core.failure_callback,
            callback_index,
        };

        let result = core
            .client
            .clone()
            .cluster_scan(&scan_state_cursor, cluster_scan_args)
            .await;
        match result {
            Ok(value) => {
                let ptr = Box::into_raw(Box::new(ResponseValue::from_value(value)));
                unsafe { (core.success_callback)(callback_index, ptr) };
            }
            Err(err) => unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    glide_core::errors::error_message(&err),
                    glide_core::errors::error_type(&err),
                );
            },
        };

        async_panic_guard.panicked = false;
    });

    panic_guard.panicked = false;
}

/// Remove a cluster scan cursor from the Rust core container.
///
/// This should be called when the C# ClusterScanCursor is disposed or finalized
/// to clean up resources allocated by the Rust core for cluster scan operations.
///
/// # Safety
/// * `cursor_id` must be a valid C string or null
#[unsafe(no_mangle)]
pub unsafe extern "C" fn remove_cluster_scan_cursor(cursor_id: *const c_char) {
    if cursor_id.is_null() {
        return;
    }

    if let Ok(cursor_str) = unsafe { CStr::from_ptr(cursor_id).to_str() } {
        glide_core::cluster_scan_container::remove_scan_state_cursor(cursor_str.to_string());
    }
}

/// Build cluster scan arguments from C-style arrays.
///
/// # Arguments
///
/// * `arg_count` - The number of arguments in the arrays
/// * `args` - Pointer to an array of pointers to argument data
/// * `arg_lengths` - Pointer to an array of argument lengths
/// * `failure_callback` - Callback function to invoke on error
/// * `callback_index` - Index to pass to the callback function
///
/// # Safety
/// * `args` and `arg_lengths` must be valid arrays of length `arg_count`
/// * Each pointer in `args` must point to valid memory of the corresponding length
unsafe fn build_cluster_scan_args(
    arg_count: u64,
    args: *const usize,
    arg_lengths: *const u64,
    failure_callback: FailureCallback,
    callback_index: usize,
) -> Option<redis::ClusterScanArgs> {
    if arg_count == 0 {
        return Some(redis::ClusterScanArgs::builder().build());
    }

    let arg_vec = unsafe { convert_string_pointer_array_to_vector(args, arg_count, arg_lengths) };

    // Parse arguments from vector.
    let mut pattern_arg: &[u8] = &[];
    let mut type_arg: &[u8] = &[];
    let mut count_arg: &[u8] = &[];

    let mut iter = arg_vec.iter().peekable();
    while let Some(arg) = iter.next() {
        match *arg {
            b"MATCH" => match iter.next() {
                Some(p) => pattern_arg = p,
                None => {
                    unsafe {
                        report_error(
                            failure_callback,
                            callback_index,
                            "No argument following MATCH.".into(),
                            RequestErrorType::Unspecified,
                        );
                    }
                    return None;
                }
            },
            b"TYPE" => match iter.next() {
                Some(t) => type_arg = t,
                None => {
                    unsafe {
                        report_error(
                            failure_callback,
                            callback_index,
                            "No argument following TYPE.".into(),
                            RequestErrorType::Unspecified,
                        );
                    }
                    return None;
                }
            },
            b"COUNT" => match iter.next() {
                Some(c) => count_arg = c,
                None => {
                    unsafe {
                        report_error(
                            failure_callback,
                            callback_index,
                            "No argument following COUNT.".into(),
                            RequestErrorType::Unspecified,
                        );
                    }
                    return None;
                }
            },
            _ => {
                unsafe {
                    report_error(
                        failure_callback,
                        callback_index,
                        "Unknown cluster scan argument".into(),
                        RequestErrorType::Unspecified,
                    );
                }
                return None;
            }
        }
    }

    // Build cluster scan arguments.
    let mut cluster_scan_args_builder = redis::ClusterScanArgs::builder();

    if !pattern_arg.is_empty() {
        cluster_scan_args_builder = cluster_scan_args_builder.with_match_pattern(pattern_arg);
    }

    if !type_arg.is_empty() {
        let converted_type = match std::str::from_utf8(type_arg) {
            Ok(t) => redis::ObjectType::from(t.to_string()),
            Err(_) => {
                unsafe {
                    report_error(
                        failure_callback,
                        callback_index,
                        "Invalid UTF-8 in TYPE argument".into(),
                        RequestErrorType::Unspecified,
                    );
                }
                return None;
            }
        };

        cluster_scan_args_builder = cluster_scan_args_builder.with_object_type(converted_type);
    }

    if !count_arg.is_empty() {
        let count_str = match std::str::from_utf8(count_arg) {
            Ok(c) => c,
            Err(_) => {
                unsafe {
                    report_error(
                        failure_callback,
                        callback_index,
                        "Invalid UTF-8 in COUNT argument".into(),
                        RequestErrorType::Unspecified,
                    );
                }
                return None;
            }
        };

        let converted_count = match count_str.parse::<u32>() {
            Ok(c) => c,
            Err(_) => {
                unsafe {
                    report_error(
                        failure_callback,
                        callback_index,
                        "Invalid COUNT value".into(),
                        RequestErrorType::Unspecified,
                    );
                }
                return None;
            }
        };

        cluster_scan_args_builder = cluster_scan_args_builder.with_count(converted_count);
    }

    Some(cluster_scan_args_builder.build())
}

/// Converts an array of pointers to strings to a vector of strings.
///
/// # Arguments
///
/// * `data` - Pointer to an array of pointers to string data
/// * `len` - The number of strings in the array
/// * `data_len` - Pointer to an array of string lengths
///
/// # Safety
///
/// `convert_string_pointer_array_to_vector` returns a `Vec` of u8 slice which holds pointers of C
/// strings. The returned `Vec<&'a [u8]>` is meant to be copied into Rust code. Storing them
/// for later use will cause the program to crash as the pointers will be freed by the caller.
unsafe fn convert_string_pointer_array_to_vector<'a>(
    data: *const usize,
    len: u64,
    data_len: *const u64,
) -> Vec<&'a [u8]> {
    let string_ptrs = unsafe { from_raw_parts(data, len as usize) };
    let string_lengths = unsafe { from_raw_parts(data_len, len as usize) };

    let mut result = Vec::<&[u8]>::with_capacity(string_ptrs.len());
    for (i, &str_ptr) in string_ptrs.iter().enumerate() {
        let slice = unsafe { from_raw_parts(str_ptr as *const u8, string_lengths[i] as usize) };
        result.push(slice);
    }

    result
}

/// Manually refresh the IAM authentication token.
///
/// This function triggers an immediate refresh of the IAM token and updates the connection.
/// It is only available if the client was created with IAM authentication.
///
/// # Arguments
///
/// * `client_ptr` - A pointer to a valid client returned from [`create_client`].
/// * `callback_index` - A unique identifier for the callback to be called when the command completes.
///
/// # Safety
///
/// * `client_ptr` must not be `null`.
/// * `client_ptr` must be able to be safely casted to a valid [`Arc<Client>`] via [`Arc::from_raw`]. See the safety documentation of [`Arc::from_raw`].
/// * This function should only be called with a `client_ptr` created by [`create_client`], before [`close_client`] was called with the pointer.
#[unsafe(no_mangle)]
pub unsafe extern "C-unwind" fn refresh_iam_token(
    client_ptr: *const c_void,
    callback_index: usize,
) {
    let client = unsafe {
        Arc::increment_strong_count(client_ptr);
        Arc::from_raw(client_ptr as *mut Client)
    };
    let core = client.core.clone();

    let mut panic_guard = PanicGuard {
        panicked: true,
        failure_callback: core.failure_callback,
        callback_index,
    };

    client.runtime.spawn(async move {
        let mut async_panic_guard = PanicGuard {
            panicked: true,
            failure_callback: core.failure_callback,
            callback_index,
        };

        let result = core.client.clone().refresh_iam_token().await;
        match result {
            Ok(()) => {
                let response = ResponseValue::from_value(redis::Value::Okay);
                let ptr = Box::into_raw(Box::new(response));
                unsafe { (core.success_callback)(callback_index, ptr) };
            }
            Err(err) => unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    error_message(&err),
                    error_type(&err),
                );
            },
        };

        async_panic_guard.panicked = false;
    });

    panic_guard.panicked = false;
}

/// Update connection password
///
/// # Arguments
/// * `client_ptr` - Pointer to the client
/// * `callback_index` - Callback index for async response
/// * `password` - New password (null for password removal)
/// * `immediate_auth` - Whether to authenticate immediately
///
/// # Safety
/// * `client_ptr` must be a valid pointer to a Client
/// * `password` must be a valid C string or null
#[unsafe(no_mangle)]
pub unsafe extern "C-unwind" fn update_connection_password(
    client_ptr: *const c_void,
    callback_index: usize,
    password_ptr: *const c_char,
    immediate_auth: bool,
) {
    // Build client and add panic guard.
    let client = unsafe {
        Arc::increment_strong_count(client_ptr);
        Arc::from_raw(client_ptr as *mut Client)
    };
    let core = client.core.clone();

    let mut panic_guard = PanicGuard {
        panicked: true,
        failure_callback: core.failure_callback,
        callback_index,
    };

    // Build password option.
    let password = if password_ptr.is_null() {
        None
    } else {
        match unsafe { CStr::from_ptr(password_ptr).to_str() } {
            Ok(password_str) => {
                if password_str.is_empty() {
                    None
                } else {
                    Some(password_str.into())
                }
            }
            Err(_) => {
                unsafe {
                    report_error(
                        core.failure_callback,
                        callback_index,
                        "Invalid password argument".into(),
                        RequestErrorType::Unspecified,
                    );
                }
                panic_guard.panicked = false;
                return;
            }
        }
    };

    // Run password update.
    client.runtime.spawn(async move {
        let mut async_panic_guard = PanicGuard {
            panicked: true,
            failure_callback: core.failure_callback,
            callback_index,
        };

        let result = core.client.clone().update_connection_password(password, immediate_auth).await;
        match result {
            Ok(value) => {
                let response = ResponseValue::from_value(value);
                let ptr = Box::into_raw(Box::new(response));
                unsafe { (core.success_callback)(callback_index, ptr) };
            }
            Err(err) => unsafe {
                report_error(
                    core.failure_callback,
                    callback_index,
                    error_message(&err),
                    error_type(&err),
                );
            },
        };

        async_panic_guard.panicked = false;
    });

    panic_guard.panicked = false;
}

// ========================================================================================
// OpenTelemetry
// ========================================================================================

/// Initializes OpenTelemetry with the given configuration.
///
/// # Arguments
/// * `config` - A pointer to an OpenTelemetryConfig struct containing the configuration.
///
/// # Returns
/// * `null` on success
/// * A pointer to a C string containing an error message on failure. The caller is responsible for freeing this string.
///
/// # Safety
/// * `config` must be a valid pointer to an OpenTelemetryConfig struct.
/// * The configuration and its underlying pointers must remain valid until the function returns.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn init_otel(config: *const OpenTelemetryConfigFFI) -> *const c_char {
    let config = unsafe { &*config };
    let mut otel_config = GlideOpenTelemetryConfigBuilder::default();

    // Configure traces if provided.
    if config.has_traces {
        let endpoint = unsafe { CStr::from_ptr(config.traces.endpoint) }
            .to_string_lossy()
            .to_string();

        match GlideOpenTelemetrySignalsExporter::from_str(&endpoint) {
            Ok(exporter) => {
                let sample_percentage = if config.traces.has_sample_percentage {
                    Some(config.traces.sample_percentage)
                } else {
                    None
                };
                otel_config = otel_config.with_trace_exporter(exporter, sample_percentage);
            }
            Err(e) => {
                let error_msg = format!("Invalid traces configuration: {e}");
                return CString::new(error_msg).unwrap().into_raw();
            }
        }
    }

    // Configure metrics if provided.
    if config.has_metrics {
        let endpoint = unsafe { CStr::from_ptr(config.metrics.endpoint) }
            .to_string_lossy()
            .to_string();

        match GlideOpenTelemetrySignalsExporter::from_str(&endpoint) {
            Ok(exporter) => {
                otel_config = otel_config.with_metrics_exporter(exporter);
            }
            Err(e) => {
                let error_msg = format!("Invalid metrics configuration: {e}");
                return CString::new(error_msg).unwrap().into_raw();
            }
        }
    }

    // Configure flush interval if provided.
    if config.has_flush_interval {
        otel_config = otel_config.with_flush_interval(std::time::Duration::from_millis(
            config.flush_interval_ms as u64,
        ));
    }

    // Initialize OpenTelemetry synchronously.
    match glide_core::client::get_or_init_runtime() {
        Ok(glide_runtime) => {
            match glide_runtime
                .runtime
                .block_on(async { GlideOpenTelemetry::initialise(otel_config.build()) })
            {
                Ok(_) => std::ptr::null(), // Success
                Err(e) => {
                    let error_msg = format!("Failed to initialize OpenTelemetry: {e}");
                    CString::new(error_msg).unwrap().into_raw()
                }
            }
        }
        Err(e) => {
            let error_msg = format!("Failed to get runtime: {e}");
            CString::new(error_msg).unwrap().into_raw()
        }
    }
}

/// Creates an OpenTelemetry span for the given request type.
///
/// # Parameters
/// * `request_type`: The type of request to create a span for
///
/// # Returns
/// * A pointer to the created span, or null if span creation fails.
#[unsafe(no_mangle)]
pub extern "C" fn create_otel_span(request_type: u32) -> *const c_void {
    let command_name = match get_command_name(request_type) {
        Some(name) => name,
        None => return std::ptr::null(),
    };

    create_span(&command_name)
}

/// Creates an OpenTelemetry batch span.
///
/// # Returns
/// * A pointer to the created span, or null if span creation fails.
#[unsafe(no_mangle)]
pub extern "C" fn create_batch_otel_span() -> *const c_void {
    let command_name = "Batch";
    create_span(&command_name)
}

/// Drops an OpenTelemetry span given its pointer as u64.
///
/// # Parameters
/// * `span_ptr`: A pointer to the span to drop
///
/// # Safety
/// * `span_ptr` must be a valid pointer to a span created by the create_otel_span functions, or 0
#[unsafe(no_mangle)]
pub unsafe extern "C" fn drop_otel_span(span_ptr: *const c_void) {
    if span_ptr.is_null() {
        logger_core::log_debug("ffi_otel", "drop_otel_span: Ignoring null span pointer");
        return;
    }

    // Attempt to safely drop the span
    unsafe {
        // Use std::panic::catch_unwind to handle potential panics during Arc::from_raw
        let result = std::panic::catch_unwind(|| {
            Arc::from_raw(span_ptr as *const GlideSpan);
        });

        match result {
            Ok(_) => {
                logger_core::log_debug(
                    "ffi_otel",
                    format!(
                        "drop_otel_span: Successfully dropped span with pointer {:p}",
                        span_ptr
                    ),
                );
            }
            Err(_) => {
                logger_core::log_error(
                    "ffi_otel",
                    format!(
                        "drop_otel_span: Panic occurred while dropping span pointer {:p} - likely invalid pointer",
                        span_ptr
                    ),
                );
            }
        }
    }
}

/// Returns the command name for the given request type value.
/// Returns None if the command name cannot be determined.
fn get_command_name(request_type_u32: u32) -> Option<String> {
    let request_type = unsafe { std::mem::transmute::<u32, RequestType>(request_type_u32) };

    // Validate request type and extract command.
    let cmd = match request_type.get_command() {
        Some(cmd) => cmd,
        None => {
            logger_core::log_error(
                "ffi_otel",
                "get_command_name: RequestType has no command available",
            );
            return None;
        }
    };

    // Validate command bytes.
    let cmd_bytes = match cmd.command() {
        Some(bytes) => bytes,
        None => {
            logger_core::log_error(
                "ffi_otel",
                "get_command_name: Command has no bytes available",
            );
            return None;
        }
    };

    // Validate UTF-8 encoding.
    let command_name = match std::str::from_utf8(cmd_bytes.as_slice()) {
        Ok(name) => name,
        Err(e) => {
            logger_core::log_error(
                "ffi_otel",
                format!("get_command_name: Command bytes are not valid UTF-8: {e}"),
            );
            return None;
        }
    };

    // Validate command name length (reasonable limit to prevent abuse)
    if command_name.len() > 256 {
        logger_core::log_error(
            "ffi_otel",
            format!(
                "get_command_name: Command name too long ({} chars), max 256",
                command_name.len()
            ),
        );
        return None;
    }

    Some(command_name.to_string())
}

/// Returns a new span for the given command name.
fn create_span(command_name: &str) -> *const c_void {
    let span = GlideOpenTelemetry::new_span(&command_name);
    let span_ptr = Arc::into_raw(Arc::new(span)) as *const c_void;

    logger_core::log_debug(
        "ffi_otel",
        format!(
            "create_span: Successfully created span '{command_name}' with pointer {:p}",
            span_ptr
        ),
    );

    span_ptr
}
