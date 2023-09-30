using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bindings;

partial struct ByteBuffer
{
    public unsafe Span<byte> AsSpan()
    {
        return new Span<byte>(ptr, length);
    }

    public unsafe Span<T> AsSpan<T>()
    {
        return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<T>(ptr), length / Unsafe.SizeOf<T>());
    }
}


