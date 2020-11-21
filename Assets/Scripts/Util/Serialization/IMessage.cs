using System;

namespace roverstd
{
    public interface IMessage
    {
        byte TypeCode { get; }

        bool IsManaged { get; }
    }
}