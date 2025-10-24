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

/// Free an error message from a failed drop_script call.
///
/// # Parameters
///
/// * `error`: The error to free.
///
/// # Safety
///
/// * `error` must be an error returned by [`drop_script`].
/// * This function must be called exactly once per error.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn free_drop_script_error(error: *mut c_char) {
    if !error.is_null() {
        _ = unsafe { CString::from_raw(error) };
    }
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
        let key_ptrs = unsafe { std::slice::from_raw_parts(keys as *const *const u8, keys_count) };
        let key_lens = unsafe { std::slice::from_raw_parts(keys_len, keys_count) };
        key_ptrs
            .iter()
            .zip(key_lens.iter())
            .map(|(&ptr, &len)| unsafe { std::slice::from_raw_parts(ptr, len) })
            .collect()
    } else {
        Vec::new()
    };

    // Convert args
    let args_vec: Vec<&[u8]> = if !args.is_null() && !args_len.is_null() && args_count > 0 {
        let arg_ptrs = unsafe { std::slice::from_raw_parts(args as *const *const u8, args_count) };
        let arg_lens = unsafe { std::slice::from_raw_parts(args_len, args_count) };
        arg_ptrs
            .iter()
            .zip(arg_lens.iter())
            .map(|(&ptr, &len)| unsafe { std::slice::from_raw_parts(ptr, len) })
            .collect()
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
