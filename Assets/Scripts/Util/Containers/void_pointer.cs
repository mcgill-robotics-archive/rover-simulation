using System.Runtime.CompilerServices;

namespace roverstd
{
    public unsafe struct void_pointer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void_pointer(void* ptr)
        {
            value = ptr;
        }

        public void* value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(void_pointer ptr)
        {
            return ptr.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void_pointer(void* ptr)
        {
            return new void_pointer(ptr);
        }
    }
}