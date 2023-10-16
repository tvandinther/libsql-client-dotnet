using System.Runtime.InteropServices;
using System.Text;

namespace LibsqlClient.Extensions;

public static class StringExtensions
{
    public static unsafe byte* GetPtr(this string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length + 1);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        Marshal.WriteByte(ptr + bytes.Length, 0);

        return (byte*)ptr;
    }
    
    // public static unsafe void FreePtr(this byte* ptr)
    // {
    //     Marshal.FreeHGlobal(ptr);
    // }
}

