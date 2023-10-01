use libsql;
use rows_iterator::Value;
use rows_iterator::ValueType;
use std::collections::HashMap;
use std::sync::Mutex;
use std::str;
use std::slice;
use std::string;
use lazy_static::lazy_static;
use tokio::runtime::Runtime;

mod byte_buffer;
use byte_buffer::ByteBuffer;

mod rows_iterator;
use rows_iterator::RowsIterator;

const SUCCESS: i32 = 0;
const DATABASE_NOT_FOUND: i32 = -1;
// const EXECUTION_ERROR: u64 = 0;
const DATABASE_CREATION_ERROR: i32 = -3;

lazy_static! {
    static ref DATABASES: Mutex<HashMap<i32, libsql::Database>> = Mutex::new(HashMap::new());
    static ref CONNECTIONS: Mutex<HashMap<i32, libsql::Connection>> = Mutex::new(HashMap::new());
    static ref GLOBAL_RUNTIME: Runtime = Runtime::new().expect("Failed to create runtime");
}

#[repr(C)]
pub struct FFIResult<T> {
    pub error: *mut ByteBuffer,
    pub result: T,
}

#[repr(C)]
pub struct ResultSet {
    pub rows_affected: u64,
    pub last_insert_rowid: i64,
    pub columns: *mut ByteBuffer,
    pub rows_iterator_ptr: *mut RowsIterator,
}

#[no_mangle]
pub extern "C" fn rows_iterator_next(rows_iterator: *mut RowsIterator) -> bool {
    let iterator = unsafe { &mut *rows_iterator };
    iterator.next()
}

#[no_mangle]
pub extern "C" fn rows_iterator_current(rows_iterator: *mut RowsIterator) -> *mut ByteBuffer {
    let iterator = unsafe { &mut *rows_iterator };
    iterator.current()
}

#[no_mangle]
pub extern "C" fn database_new_in_memory() -> i32 {
    match libsql::Database::open_in_memory() {
        Ok(db) => {
            let mut map = DATABASES.lock().unwrap();
            let key = map.len() as i32;

            map.insert(key, db);
            key
        }
        Err(_) => DATABASE_CREATION_ERROR,
    }
}

#[no_mangle]
pub extern "C" fn database_open_connection(key: i32) -> i32 {
    let map = DATABASES.lock().unwrap();

    if let Some(db) = map.get(&key) {
        let mut map = CONNECTIONS.lock().unwrap();
        return match (*db).connect() {
            Ok(conn) => {
                map.insert(key, conn);
                return key
            }
            Err(_) => DATABASE_CREATION_ERROR,
        }
    } else {
        DATABASE_NOT_FOUND
    }
}

#[no_mangle]
pub extern "C" fn database_execute(key: i32, str_ptr: *const u16, len: i32) -> ResultSet {
    let map = CONNECTIONS.lock().unwrap();

    if let Some(conn) = map.get(&key) {
        (*GLOBAL_RUNTIME).block_on(async {
            match convert_to_str(str_ptr, len) {
                Ok(s) => {
                    let _ = conn.execute(s.as_str(), libsql::Params::None).await;
                    return ResultSet {
                        rows_affected: conn.changes(),
                        last_insert_rowid: conn.last_insert_rowid(),
                        columns: std::ptr::null_mut(),
                        rows_iterator_ptr: Box::into_raw(Box::new(RowsIterator::empty())),
                    };
                },
                Err(e) => panic!("{}", e),
            }
        })
    } else {
        panic!("Connection not found");
    }
}

#[no_mangle]
pub extern "C" fn database_query(key: i32, str_ptr: *const u16, len: i32) -> ResultSet {
    let map = CONNECTIONS.lock().unwrap();

    if let Some(conn) = map.get(&key) {
        (*GLOBAL_RUNTIME).block_on(async {
            match convert_to_str(str_ptr, len) {
                Ok(s) => {
                    let stmt = conn.prepare(s.as_str()).await.unwrap();
                    let rows = stmt.query(&libsql::Params::None).await.unwrap();
                    let cs = stmt.columns();
                    let columns: Vec<&str> = cs.iter().map(|c| c.name()).collect();
                    return ResultSet {
                        rows_affected: conn.changes(),
                        last_insert_rowid: conn.last_insert_rowid(),
                        columns: columns.alloc_str_array(),
                        rows_iterator_ptr: Box::into_raw(Box::new(RowsIterator::new(rows))),
                    };
                },
                Err(e) => panic!("{}", e),
            }
        })
    } else {
        panic!("Connection not found");
    }
}

#[no_mangle]
pub extern "C" fn database_delete(key: i32) -> i32 {
    let mut map = DATABASES.lock().unwrap();

    match map.remove(&key) {
        Some(_) => SUCCESS,
        None => DATABASE_NOT_FOUND,
    }
}

fn convert_to_str(ptr: *const u16, len: i32) -> Result<String, string::FromUtf16Error> {
    // SAFETY: The caller must ensure that the provided pointer is valid for `len` many elements.
    // The data must not be mutated for the lifetime of the slice, and it must be safe to read
    // the data for the lifetime of the slice.
    let slice = unsafe { slice::from_raw_parts(ptr, len as usize) };
    
    String::from_utf16(slice)
}

pub trait AllocStrArray {
    fn alloc_str_array(&self) -> *mut ByteBuffer;
}

impl AllocStrArray for Vec<&str> {
    fn alloc_str_array(&self) -> *mut ByteBuffer {
        let mut byte_buffers: Vec<*mut ByteBuffer> = Vec::new();

        for s in self {
            let utf16_data = s.encode_utf16().collect::<Vec<u16>>();
            let buffer = ByteBuffer::from_vec_struct(utf16_data);
            let buffer_ptr = Box::into_raw(Box::new(buffer));
            byte_buffers.push(buffer_ptr);
        }

        let meta_buffer = ByteBuffer::from_vec_of_pointers(byte_buffers);
        Box::into_raw(Box::new(meta_buffer))
    }
}

#[no_mangle]
pub unsafe extern "C" fn byte_buffer_dealloc(buffer: *mut ByteBuffer) {
    let buf = Box::from_raw(buffer);
    buf.destroy();
}

#[no_mangle]
pub unsafe extern "C" fn rows_iterator_dealloc(rows_iterator: *mut RowsIterator) {
    let mut iterator = Box::from_raw(rows_iterator);
    iterator.destroy();
}

#[no_mangle]
pub extern "C" fn export(_value: Value, _value_type: ValueType) {}
