using System;

namespace roverstd
{
    public interface ICopyMovable<T> : IDisposable
    {
        T Copy();

        void CopyAssign(T t);

        T Move();

        void MoveAssign(T t);
    }
}