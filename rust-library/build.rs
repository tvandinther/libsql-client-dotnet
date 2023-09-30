extern crate csbindgen;

fn main() {
    csbindgen::Builder::default()
        .input_extern_file("src/lib.rs")
        .csharp_dll_name("libclient_lib.so")
        .csharp_namespace("Bindings")
        .csharp_class_name("Libsql")
        .generate_csharp_file("bindings/Libsql.g.cs")
        .unwrap();
}