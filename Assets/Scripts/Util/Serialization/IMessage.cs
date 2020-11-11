using System;

namespace Rover
{
    public interface IMessage
    {
        byte TypeCode { get; }
    }
}