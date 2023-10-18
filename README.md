# libsql-client-dotnet

A .NET client library for libsql.

**This project is still in early development and not ready for production use.**

### Currently Supported Features

- Creating a database:
  - In memory.
  - From file.
- Executing SQL statements:
  - Non-parameterised.

### Planned Features

- Positional and named arguments.
- Remote databases.
- Embedded replicas.
- Prepared statements.
- Batched statements.
- Transactions.

---

### Progress
- A database can be created:
  - [x] In memory.
  - [x] From file.
  - [ ] From connection string.
- [x] A database can be destroyed/closed/deallocated.
- [ ] An embedded replica can be created.
  - [ ] An embeded replica can be synced.
- The database can execute SQL statements:
  - [x] Non-parameterised.
  - [ ] Parameterised with positional arguments.
  - [ ] Parameterised with named arguments.
- [ ] Prepared statements.
- [ ] Batched statements.
- [ ] Transactions.
- [x] A result set is returned from an execution.
  - [x] With the column names.
  - [x] With an enumerable of enumerable (rows) of typed boxed values.
  - [ ] With the number of affected rows.
  - [ ] With the last inserted row id.
