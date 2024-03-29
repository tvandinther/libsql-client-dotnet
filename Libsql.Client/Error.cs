﻿using System;
using Libsql.Client.Extensions;

namespace Libsql.Client
{
    internal unsafe struct Error
    {
        public byte* Ptr;

        public unsafe void ThrowIfNonZero(int exitCode, string message)
        {
            if (exitCode == 0) return;
        
            var text = CustomMarshal.PtrToStringUTF8((IntPtr)Ptr);
        
            throw new LibsqlException($"{message}: {text}");
        }
    }
}