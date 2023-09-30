use libsql;
use std::collections::HashMap;
use std::sync::Mutex;
use lazy_static::lazy_static;

lazy_static! {
    static ref DATABASES: Mutex<HashMap<u32, libsql::Database>> = Mutex::new(HashMap::new());
}

#[no_mangle]
pub extern "C" fn database_new_in_memory() -> u32 {
    let db = libsql::Database::open_in_memory().unwrap();

    let mut map = DATABASES.lock().unwrap();

    // Generate a unique key for this database instance.
    let key = map.len() as u32; // Or use a more complex method to generate a unique key.

    map.insert(key, db);
    key
}

#[no_mangle]
pub extern "C" fn database_do_something(key: u32) {
    let map = DATABASES.lock().unwrap();

    if let Some(db) = map.get(&key) {
        // Do something with db
    } else {
        // Handle error: invalid key
    }
}

#[no_mangle]
pub extern "C" fn database_delete(key: u32) {
    let mut map = DATABASES.lock().unwrap();

    if map.remove(&key).is_none() {
        // Handle error: invalid key
    }
}
