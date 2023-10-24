using System;
using System.Runtime.InteropServices;

namespace Libsql.Client
{
    internal unsafe struct Error
    {
        public byte* Ptr;

        public unsafe void ThrowIfNonZero(int exitCode, string message)
        {
            if (exitCode == 0) return;
        
            var text = Marshal.PtrToStringAnsi((IntPtr)Ptr);
        
            throw new LibsqlException($"{message}: {text}");
        }
    }
}