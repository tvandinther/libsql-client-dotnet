using System.Runtime.InteropServices;

namespace LibsqlClient;

internal unsafe struct Error
{
    public readonly byte* Ptr;
    
    public Error()
    {
        Ptr = (byte*)0;
    }

    public unsafe void ThrowIfNonZero(int exitCode, string message)
    {
        if (exitCode == 0) return;
        
        var text = Marshal.PtrToStringAuto((IntPtr)Ptr);
        
        throw new Exception($"{message}: {text}");
    }
}