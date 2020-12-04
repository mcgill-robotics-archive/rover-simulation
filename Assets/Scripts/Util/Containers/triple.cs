using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace roverstd
{
    public struct triple<T1, T2, T3> : IEquatable<triple<T1, T2, T3>>
    {
        public T1 item1;
        public T2 item2;
        public T3 item3;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public triple(T1 t1, T2 t2, T3 t3)
        {
            item1 = t1;
            item2 = t2;
            item3 = t3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(triple<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(item1, other.item1) && EqualityComparer<T2>.Default.Equals(item2, other.item2) && EqualityComparer<T3>.Default.Equals(item3, other.item3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is triple<T1, T2, T3> other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = EqualityComparer<T1>.Default.GetHashCode(item1);
                hashCode = (hashCode * 397) ^ EqualityComparer<T2>.Default.GetHashCode(item2);
                hashCode = (hashCode * 397) ^ EqualityComparer<T3>.Default.GetHashCode(item3);
                return hashCode;
            }
        }
    }
}