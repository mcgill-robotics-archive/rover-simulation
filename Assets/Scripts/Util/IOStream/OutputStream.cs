using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using JetBrains.Annotations;

namespace Rover.Util.IOStream
{
 
    public interface IOutputStream<T>
    {
        int Count { get; }

        void Write(T t);

        void Write(T[] arr);

        T[] ToArray();
    }

    public class ByteArrayOutputStream : IOutputStream<byte>
    {
        private readonly List<byte> m_BackingList;

        public int Count
        {
            get
            {
                lock (m_BackingList)
                {
                    return m_BackingList.Count;
                }
            }
        }

        public ByteArrayOutputStream(int initialCapacity = 8)
        {
            m_BackingList = new List<byte>(initialCapacity);
        }

        public void Write(byte t)
        {
            lock (m_BackingList)
            {
                m_BackingList.Add(t);
            }
        }

        public void Write(byte[] arr)
        {
            lock (m_BackingList)
            {
                m_BackingList.AddRange(arr);
            }
        }

        public byte[] ToArray()
        {
            lock (m_BackingList)
            {
                return m_BackingList.ToArray();
            }
        }

        public void Write(short value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(int value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(double value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(char value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(bool value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(ISerializable value)
        {
            value.Serialize(this);
        }
    }
}