using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Libsql.Client.Extensions
{
    internal static class CustomMarshal
    {
        public static string PtrToStringUTF8(IntPtr ptr)
        {
            var bytes = new byte[IndexOfNullByte(ptr)];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            
            return Encoding.UTF8.GetString(bytes);
        }
        
        private static int IndexOfNullByte(IntPtr ptr)
        {
            var length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
            {
                length++;
            }
            
            return length;
        }
    }
}
