// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

//! Bridge between glide-core's synchronous IAM credentials provider and a managed C# provider.
//!
//! The C# binding passes an opaque handle together with two static entry points. Rust owns the
//! handle from the moment [`CredentialsProviderRegistration::take`] runs and releases it exactly once
//! when the last reference to the registration is dropped.

use std::{
    ffi::c_void,
    slice::from_raw_parts,
    sync::Arc,
    time::{Duration, SystemTime},
};

use glide_core::iam::{CredentialsProvider, GlideIAMError};

/// Callback that invokes the managed credentials provider.
///
/// # Arguments
/// * `handle` is the opaque managed handle passed to [`crate::create_client`].
/// * `sink` is an opaque pointer that must only be passed to [`set_iam_credentials`] before the
///   callback returns.
///
/// # Returns
/// One of the `STATUS_*` constants defined in this module.
///
/// # Safety
/// The callback must not unwind and must not retain `sink` after it returns.
pub type IamCredentialsInvokeCallback =
    unsafe extern "C" fn(handle: *mut c_void, sink: *mut c_void) -> i32;

/// Callback that releases the managed credentials provider handle.
///
/// # Safety
/// The callback must not unwind. It is called exactly once per registered handle.
pub type IamCredentialsReleaseCallback = unsafe extern "C" fn(handle: *mut c_void);

// Invocation statuses. Must match `IamCredentialsStatus` in the C# bridge.
const STATUS_SUCCESS: i32 = 0;
const STATUS_NULL_TASK: i32 = 1;
const STATUS_NULL_RESULT: i32 = 2;
const STATUS_FAULTED: i32 = 3;
const STATUS_CANCELLED: i32 = 4;
const STATUS_BUSY: i32 = 5;
const STATUS_TIMED_OUT: i32 = 6;
const STATUS_INVALID_CREDENTIALS: i32 = 7;
const STATUS_INTERNAL: i32 = 8;

/// Credentials as returned by glide-core's [`CredentialsProvider`]:
/// `(access_key_id, secret_access_key, session_token, expires_at)`.
type ProvidedCredentials = (String, String, Option<String>, Option<SystemTime>);

/// FFI view of AWS credentials written by the managed provider.
///
/// All pointers reference UTF-8 bytes that are only valid for the duration of the
/// [`set_iam_credentials`] call.
#[repr(C)]
#[derive(Clone, Copy)]
pub struct AwsCredentialsFfi {
    pub access_key_id: *const u8,
    pub access_key_id_len: usize,
    pub secret_access_key: *const u8,
    pub secret_access_key_len: usize,
    pub has_session_token: bool,
    pub session_token: *const u8,
    pub session_token_len: usize,
    pub has_expires_at: bool,
    pub expires_at_unix_ms: i64,
}

// Layout checked by `FFITests.AwsCredentialsFfi_MatchesRustLayout` in the C# unit tests.
const _: () = {
    use std::mem::{offset_of, size_of};
    assert!(size_of::<AwsCredentialsFfi>() == 72);
    assert!(offset_of!(AwsCredentialsFfi, access_key_id) == 0);
    assert!(offset_of!(AwsCredentialsFfi, access_key_id_len) == 8);
    assert!(offset_of!(AwsCredentialsFfi, secret_access_key) == 16);
    assert!(offset_of!(AwsCredentialsFfi, secret_access_key_len) == 24);
    assert!(offset_of!(AwsCredentialsFfi, has_session_token) == 32);
    assert!(offset_of!(AwsCredentialsFfi, session_token) == 40);
    assert!(offset_of!(AwsCredentialsFfi, session_token_len) == 48);
    assert!(offset_of!(AwsCredentialsFfi, has_expires_at) == 56);
    assert!(offset_of!(AwsCredentialsFfi, expires_at_unix_ms) == 64);
};

/// Owns a managed credentials provider handle and the callbacks used to invoke and release it.
pub struct CredentialsProviderRegistration {
    handle: *mut c_void,
    invoke: IamCredentialsInvokeCallback,
    release: IamCredentialsReleaseCallback,
}

// SAFETY: The managed handle is a GCHandle that may be used from any thread, and the managed
// entry points are thread-safe.
unsafe impl Send for CredentialsProviderRegistration {}
unsafe impl Sync for CredentialsProviderRegistration {}

impl Drop for CredentialsProviderRegistration {
    fn drop(&mut self) {
        // SAFETY: `take` guarantees a non-null handle, and `drop` runs exactly once.
        unsafe { (self.release)(self.handle) };
    }
}

impl CredentialsProviderRegistration {
    /// Takes ownership of a managed credentials provider handle.
    ///
    /// Returns `Ok(None)` if `handle` is null. Returns an error if `handle` is non-null but a
    /// callback is missing; the handle is released first when `release` is available.
    ///
    /// # Safety
    /// * If non-null, `handle` must be a managed handle that `release` frees exactly once.
    /// * `invoke` and `release` must be valid function pointers when provided.
    pub unsafe fn take(
        handle: *mut c_void,
        invoke: Option<IamCredentialsInvokeCallback>,
        release: Option<IamCredentialsReleaseCallback>,
    ) -> Result<Option<Arc<Self>>, String> {
        if handle.is_null() {
            return Ok(None);
        }

        let Some(release) = release else {
            return Err("Credentials provider release callback is missing".to_string());
        };
        let Some(invoke) = invoke else {
            unsafe { release(handle) };
            return Err("Credentials provider invoke callback is missing".to_string());
        };
        Ok(Some(Arc::new(Self {
            handle,
            invoke,
            release,
        })))
    }

    /// Invokes the managed provider and returns the copied credentials.
    fn call(&self) -> Result<ProvidedCredentials, GlideIAMError> {
        let mut sink: Option<ProvidedCredentials> = None;
        let sink_pointer = &mut sink as *mut Option<ProvidedCredentials> as *mut c_void;

        // SAFETY: `handle` is owned by this registration and is valid until `drop`.
        // `sink_pointer` points to a local that outlives the callback.
        let status = unsafe { (self.invoke)(self.handle, sink_pointer) };

        match (status, sink) {
            (STATUS_SUCCESS, Some(credentials)) => Ok(credentials),
            (STATUS_SUCCESS, None) => Err(credentials_error(
                "Custom credentials provider returned no credentials",
            )),
            (status, _) => Err(credentials_error(status_message(status))),
        }
    }
}

/// Wraps a registration in a glide-core credentials provider.
pub fn into_core_provider(
    registration: Arc<CredentialsProviderRegistration>,
) -> CredentialsProvider {
    Arc::new(move || registration.call())
}

/// Returns the fixed error message for a non-success invocation status.
fn status_message(status: i32) -> &'static str {
    match status {
        STATUS_NULL_TASK => "Custom credentials provider returned a null task",
        STATUS_NULL_RESULT => "Custom credentials provider returned null credentials",
        STATUS_FAULTED => "Custom credentials provider failed",
        STATUS_CANCELLED => "Custom credentials provider was cancelled",
        STATUS_BUSY => "Custom credentials provider is still running a previous invocation",
        STATUS_TIMED_OUT => "Custom credentials provider did not complete within 9 seconds",
        STATUS_INVALID_CREDENTIALS => {
            "Custom credentials provider returned credentials that could not be marshalled"
        }
        STATUS_INTERNAL => "Custom credentials provider invocation failed unexpectedly",
        _ => "Custom credentials provider returned an unknown status",
    }
}

fn credentials_error(message: &str) -> GlideIAMError {
    GlideIAMError::CredentialsError(message.to_string())
}

/// Copies length-delimited UTF-8 bytes into an owned string.
///
/// # Safety
/// If `length` is non-zero, `pointer` must point to `length` readable bytes.
unsafe fn copy_utf8(pointer: *const u8, length: usize) -> Option<String> {
    if length == 0 {
        return Some(String::new());
    }
    if pointer.is_null() {
        return None;
    }
    let bytes = unsafe { from_raw_parts(pointer, length) };
    String::from_utf8(bytes.to_vec()).ok()
}

/// Copies an optional length-delimited UTF-8 field. Returns `None` if a present field is invalid.
///
/// # Safety
/// See [`copy_utf8`].
unsafe fn copy_optional_utf8(
    present: bool,
    pointer: *const u8,
    length: usize,
) -> Option<Option<String>> {
    if !present {
        return Some(None);
    }
    unsafe { copy_utf8(pointer, length) }.map(Some)
}

/// Converts an optional expiry in Unix milliseconds. Non-positive values are absent.
/// Returns `None` if the expiry is out of range.
fn expiry_from_unix_millis(present: bool, unix_millis: i64) -> Option<Option<SystemTime>> {
    if !present || unix_millis <= 0 {
        return Some(None);
    }
    SystemTime::UNIX_EPOCH
        .checked_add(Duration::from_millis(unix_millis as u64))
        .map(Some)
}

/// Copies credentials written by the managed provider into the invocation sink.
///
/// Returns `true` if the credentials were copied, or `false` if a pointer is null or a field is
/// invalid. Nothing is written to the sink on failure.
///
/// # Safety
/// * `sink` must be the pointer passed to the current [`IamCredentialsInvokeCallback`] call, and
///   this function must only be called before that callback returns.
/// * `credentials` must point to a valid [`AwsCredentialsFfi`] whose byte pointers reference
///   their declared lengths of readable memory.
#[unsafe(no_mangle)]
pub unsafe extern "C" fn set_iam_credentials(
    sink: *mut c_void,
    credentials: *const AwsCredentialsFfi,
) -> bool {
    if sink.is_null() || credentials.is_null() {
        return false;
    }
    let credentials = unsafe { *credentials };

    let Some(access_key_id) =
        (unsafe { copy_utf8(credentials.access_key_id, credentials.access_key_id_len) })
    else {
        return false;
    };
    let Some(secret_access_key) = (unsafe {
        copy_utf8(
            credentials.secret_access_key,
            credentials.secret_access_key_len,
        )
    }) else {
        return false;
    };
    let Some(session_token) = (unsafe {
        copy_optional_utf8(
            credentials.has_session_token,
            credentials.session_token,
            credentials.session_token_len,
        )
    }) else {
        return false;
    };
    let Some(expires_at) =
        expiry_from_unix_millis(credentials.has_expires_at, credentials.expires_at_unix_ms)
    else {
        return false;
    };

    let sink = unsafe { &mut *(sink as *mut Option<ProvidedCredentials>) };
    *sink = Some((access_key_id, secret_access_key, session_token, expires_at));
    true
}
