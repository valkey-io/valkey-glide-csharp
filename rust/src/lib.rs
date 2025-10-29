// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

mod ffi;
use ffi::{
    BatchInfo, BatchOptionsInfo, CmdInfo, ConnectionConfig, ResponseValue, RouteInfo, create_cmd,
    create_connection_request, create_pipeline, create_route, get_pipeline_options,
};
use glide_core::{
    client::Client as GlideClient,
    errors::{RequestErrorType, error_message, error_type},
};
use std::{
    ffi::{CStr, CString, c_char, c_void},
    slice::from_raw_parts,
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
#[allow(rustdoc::private_intra_doc_links)]
#[unsafe(no_mangle)]
pub unsafe extern "C-unwind" fn create_client(
    config: *const ConnectionConfig,
    success_callback: SuccessCallback,
    failure_callback: FailureCallback,
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
    let res = runtime.block_on(GlideClient::new(request, None));
    match res {
        Ok(client) => {
            let core = Arc::new(CommandExecutionCore {
                success_callback,
                failure_callback,
                client,
            });

            let client_ptr = Arc::into_raw(Arc::new(Client { runtime, core }));
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

/// Closes the given client, deallocating it from the heap.
/// This function should only be called once per pointer created by [`create_client`].
/// After calling this function the `client_ptr` is not in a valid state.
///
/// # Safety
///
/// * `client_ptr` must not be `null`.
/// * `client_ptr` must be able to be safely casted to a valid [`Arc<Client>`] via [`Arc::from_raw`]. See the safety documentation of [`Arc::from_raw`].
#[unsafe(no_mangle)]
pub extern "C" fn close_client(client_ptr: *const c_void) {
    assert!(!client_ptr.is_null());
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
                    (core.failure_callback)(
                        callback_index,
                        format!("Invalid cursor ID: {}", cursor_id).as_ptr() as *const c_char,
                        RequestErrorType::Unspecified,
                    );
                }
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
        None => return,
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
        drop(async_panic_guard);
    });

    panic_guard.panicked = false;
    drop(panic_guard);
}
