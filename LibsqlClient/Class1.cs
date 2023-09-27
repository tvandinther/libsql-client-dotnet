using System.Runtime.InteropServices;
using System.Text;
using CsBindgen;

namespace LibsqlClient;

public class Class1
{
    public string Name { get; set; } = "Class1";

    // [DllImport("../../../../rust-library/target/x86_64-unknown-linux-gnu/release/libclient_lib.so", EntryPoint = "print_string")]
    // private static extern int RustPrint(byte[] str, UIntPtr len);
    public void PrintViaRust(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        int result = -5;
        unsafe
        {
            fixed (byte* pByteArray = bytes)
            {
                result = NativeMethods.my_add(1, 1);
            } 
        }
        
        Console.WriteLine(result);
        // var result = RustPrint(bytes, (UIntPtr)bytes.Length);
    
        // switch (result)
        // {
        //     case -1:
        //         Console.WriteLine("Error: Null pointer passed to Rust function.");
        //         break;
        //     case -2:
        //         Console.WriteLine("Error: Invalid UTF-8 passed to Rust function.");
        //         break;
        // }
    }
}