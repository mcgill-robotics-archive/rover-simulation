using System;

namespace Rover.Util.Containers
{
    public struct Pair<T, U> : IEquatable<Pair<T, U>> where T : struct where U : struct
    {
        public T First;
        public U Second;

        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        public bool Equals(Pair<T, U> other)
        {
            return First.Equals(other.First) && Second.Equals(other.Second);
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<T, U> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (First.GetHashCode() * 397) ^ Second.GetHashCode();
            }
        }
    }
}
