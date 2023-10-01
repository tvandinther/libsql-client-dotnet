# libsql-client-dotnet

A .NET client library for libsql. The project contains a rust library wrapping the libsql crate and a .NET library invoking the rust library through C-compatible FFI bindings.

**This project is still in early development and not ready for production use.**

## Progress
- [x] An in memory database can be created.
- [x] The in memory database can execute SQL statements.
- [x] A result set is returned from an execution.
    - [x] It returns the number of affected rows.
    - [x] It returns the last inserted row id.
    - [x] It returns the column names.
    - [x] It returns a list of arrays (rows) of typed boxed values.
- [ ] A database can be destroyed.
