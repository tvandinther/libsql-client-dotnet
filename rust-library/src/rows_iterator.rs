use libsql::Row;
use super::byte_buffer::ByteBuffer;

#[repr(u8)]
pub enum ValueType {
    Integer,
    Real,
    Text,
    Blob,
    Null,
}

#[repr(C)]
pub struct Value {
    pub value_type: ValueType,
    pub value_buffer: *mut ByteBuffer,
}

impl Value {
    pub fn new(value: libsql::Value) -> Value {
        match value {
            libsql::Value::Integer(i) => Value {
                value_type: ValueType::Integer,
                value_buffer: ByteBuffer::from_vec_struct(vec![i]).box_raw(),
            },
            libsql::Value::Real(r) => Value {
                value_type: ValueType::Real,
                value_buffer:  ByteBuffer::from_vec_struct(vec![r]).box_raw(),
            },
            libsql::Value::Text(t) => Value {
                value_type: ValueType::Text,
                value_buffer: ByteBuffer::from_vec_struct(t.encode_utf16().collect::<Vec<u16>>()).box_raw(),
            },
            libsql::Value::Blob(b) => Value {
                value_type: ValueType::Blob,
                value_buffer: ByteBuffer::from_vec(b).box_raw(),
            },
            libsql::Value::Null => Value {
                value_type: ValueType::Null,
                value_buffer: std::ptr::null_mut(),
            },
        }
    }
}

pub struct RowsIterator {
    rows: Option<libsql::Rows>,
    row_length: i32,
    current_row: Option<Row>,
}

impl RowsIterator {
    pub fn new(rows: libsql::Rows) -> RowsIterator {
        let row_length = rows.column_count();
        RowsIterator {
            rows: Some(rows),
            row_length,
            current_row: None,
        }
    }

    pub fn empty() -> RowsIterator {
        RowsIterator {
            rows: None,
            row_length: 0,
            current_row: None,
        }
    }

    pub fn next(&mut self) -> bool {
        match &mut self.rows {
            Some(rows) => {
                match rows.next().unwrap() {
                    Some(row) => {
                        self.current_row = Some(row);
                        true
                    },
                    None => {
                        self.current_row = None;
                        false
                    }
                }
            }
            None => false,
        }
    }

    pub fn current(&mut self) -> *mut ByteBuffer {
        match &self.current_row {
            Some(row) => {
                byte_buffer_from_row(row, self.row_length)
            },
            None => std::ptr::null_mut(),
        }
    }

    pub fn destroy(&mut self) {
        match &mut self.rows {
            Some(rows) => {
                drop(rows);
            },
            None => (),
        }
        drop(self);
    }
}

fn byte_buffer_from_row(row: &Row, len: i32) -> *mut ByteBuffer {
    let mut values: Vec<Value> = Vec::with_capacity(len as usize);

    for i in 0..len {
        let v = row.get_value(i).unwrap();
        let value = Value::new(v);
        values.push(value);
    }

    let meta_buffer = ByteBuffer::from_vec_struct(values);
    Box::into_raw(Box::new(meta_buffer))
}
