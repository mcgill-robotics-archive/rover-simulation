using System.Runtime.CompilerServices;

namespace roverstd
{
    public unsafe struct cpointer<T> where T : unmanaged
    {
        public T* value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public cpointer(T* val)
        {
            value = val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T*(cpointer<T> ptr)
        {
            return ptr.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator cpointer<T>(T* val)
        {
            return new cpointer<T>(val);
        }
    }
}