extern crate csbindgen;

fn main() {
    println!("cargo:rerun-if-changed=NULL");
    csbindgen::Builder::default()
        .input_extern_file("../libsql/crates/bindings/c/src/lib.rs")
        .input_extern_file("../libsql/crates/bindings/c/src/types.rs")
        .csharp_dll_name("libclient_lib.so")
        .csharp_namespace("Bindings")
        .csharp_class_name("Libsql")
        .generate_csharp_file("bindings/Libsql.g.cs")
        .unwrap();
}