using System;
using System.Runtime.InteropServices;

namespace Libsql.Client
{
    internal unsafe struct Error
    {
        public readonly byte* Ptr;
    
        public Error(int ptr = 0)
        {
            Ptr = (byte*)ptr;
        }

        public unsafe void ThrowIfNonZero(int exitCode, string message)
        {
            if (exitCode == 0) return;
        
            var text = Marshal.PtrToStringAuto((IntPtr)Ptr);
        
            throw new Exception($"{message}: {text}");
        }
    }
}