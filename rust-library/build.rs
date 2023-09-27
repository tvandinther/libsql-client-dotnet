extern crate csbindgen;

fn main() {
    println!("cargo:warning=build.rs is running!");
    csbindgen::Builder::default()
        .input_extern_file("src/client.rs")
        .csharp_dll_name("libclient_lib.so")
        .generate_csharp_file("bindings/NativeMethods.g.cs")
        .unwrap();
}