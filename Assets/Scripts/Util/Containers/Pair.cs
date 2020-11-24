using System;

namespace roverstd
{
    public struct pair<T, U> : IEquatable<pair<T, U>> where T : struct where U : struct
    {
        public T first;
        public U second;

        public Type first_type => typeof(T);
        public Type second_type => typeof(U);

        public pair(T first, U second)
        {
            this.first = first;
            this.second = second;
        }

        public bool Equals(pair<T, U> other)
        {
            return first.Equals(other.first) && second.Equals(other.second);
        }

        public override bool Equals(object obj)
        {
            return obj is pair<T, U> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (first.GetHashCode() * 397) ^ second.GetHashCode();
            }
        }
    }
}
