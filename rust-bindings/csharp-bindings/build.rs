extern crate csbindgen;

fn main() {
    println!("cargo:rerun-if-changed=NULL");
    csbindgen::Builder::default()
        .input_extern_file("../libsql/bindings/c/src/lib.rs")
        .input_extern_file("../libsql/bindings/c/src/types.rs")
        .csharp_dll_name("csharp_bindings")
        .csharp_namespace("Libsql")
        .csharp_class_name("Bindings")
        .generate_csharp_file("bindings/Libsql.g.cs")
        .unwrap();
}