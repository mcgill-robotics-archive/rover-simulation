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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(cpointer<T> ptr)
        {
            return ptr.value != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static cpointer<T> operator++(cpointer<T> ptr)
        {
            ptr.value++;
            return ptr;
        }
    }
}