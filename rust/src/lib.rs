// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

mod ffi;
use ffi::{
    BatchInfo, BatchOptionsInfo, CmdInfo, ConnectionConfig, PubSubCallback, ResponseValue,
    RouteInfo, create_cmd, create_connection_request, create_pipeline, create_route,
    get_pipeline_options,
};
use glide_core::{
    client::Client as GlideClient,
    errors::{RequestErrorType, error_message, error_type},
};
use std::{
    ffi::{CStr, CString, c_char, c_void},
    sync::Arc,
};
use tokio::runtime::{Builder, Runtime};

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
    // These values MUST match the C# PushKind enum in FFI.structs.cs
    let (pattern, channel, message, kind) = match (push_kind.clone(), strings.len()) {
        (redis::PushKind::Message, 2) => {
            // Regular message: [channel, message] -> PushMessage = 3
            (None, &strings[0], &strings[1], 3u32)
        }
        (redis::PushKind::PMessage, 3) => {
            // Pattern message: [pattern, channel, message] -> PushPMessage = 4
            (Some(&strings[0]), &strings[1], &strings[2], 4u32)
        }
        (redis::PushKind::SMessage, 2) => {
            // Sharded message: [channel, message] -> PushSMessage = 5
            (None, &strings[0], &strings[1], 5u32)
        }
        (redis::PushKind::Subscribe, 2) => {
            // Subscribe confirmation: [channel, count] -> PushSubscribe = 9
            (None, &strings[0], &strings[1], 9u32)
        }
        (redis::PushKind::PSubscribe, 3) => {
            // Pattern subscribe confirmation: [pattern, channel, count] -> PushPSubscribe = 10
            (Some(&strings[0]), &strings[1], &strings[2], 10u32)
        }
        (redis::PushKind::SSubscribe, 2) => {
            // Sharded subscribe confirmation: [channel, count] -> PushSSubscribe = 11
            (None, &strings[0], &strings[1], 11u32)
        }
        (redis::PushKind::Unsubscribe, 2) => {
            // Unsubscribe confirmation: [channel, count] -> PushUnsubscribe = 6
            (None, &strings[0], &strings[1], 6u32)
        }
        (redis::PushKind::PUnsubscribe, 3) => {
            // Pattern unsubscribe confirmation: [pattern, channel, count] -> PushPUnsubscribe = 7
            (Some(&strings[0]), &strings[1], &strings[2], 7u32)
        }
        (redis::PushKind::SUnsubscribe, 2) => {
            // Sharded unsubscribe confirmation: [channel, count] -> PushSUnsubscribe = 8
            (None, &strings[0], &strings[1], 8u32)
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
    let pattern_len = pattern.map(|p| p.len() as i64).unwrap_or(0);
    let channel_ptr = channel.as_ptr();
    let channel_len = channel.len() as i64;
    let message_ptr = message.as_ptr();
    let message_len = message.len() as i64;

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
