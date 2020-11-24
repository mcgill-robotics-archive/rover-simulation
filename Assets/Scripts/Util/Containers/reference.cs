using System.Runtime.CompilerServices;

namespace roverstd
{
    public class reference<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public reference(T val)
        {
            value = val;
        }

        public T value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(reference<T> box)
        {
            return box.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator reference<T>(T val)
        {
            return new reference<T>(val);
        }
    }
}