extern crate csbindgen;

fn main() {
    csbindgen::Builder::default()
        .input_extern_file("src/client.rs")
        .csharp_dll_name("libclient_lib.so")
        .csharp_namespace("ClientLib")
        .csharp_class_name("Client")
        .generate_csharp_file("bindings/ClientLib.g.cs")
        .unwrap();
}