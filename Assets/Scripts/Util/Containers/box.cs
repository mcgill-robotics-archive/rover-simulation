using System.Runtime.CompilerServices;

namespace roverstd
{
    public struct box<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public box(T val)
        {
            value = val;
        }

        public T value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(box<T> box)
        {
            return box.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator box<T>(T val)
        {
            return new box<T>(val);
        }
    }
}