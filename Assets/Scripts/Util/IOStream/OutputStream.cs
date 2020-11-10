using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using JetBrains.Annotations;
using Rover.Except;

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

        /// <summary>
        /// must be int, float, short or ushort
        /// </summary>
        /// <typeparam name="T">must be int, float, short or ushort</typeparam>
        /// <param name="arr">the array to write</param>
        public void WriteArray<T>(T[] arr)
        {
            int length = arr.Length;
            Write(length);
            Type type = typeof(T);
            if (type == typeof(int))
            {
                int[] intArr = arr.Cast<int>().ToArray();
                for (int i = 0; i < length; i++)
                {
                    Write(intArr[i]);
                }

                return;
            }

            if (type == typeof(float))
            {
                float[] intArr = arr.Cast<float>().ToArray();
                for (int i = 0; i < length; i++)
                {
                    Write(intArr[i]);
                }

                return;
            }

            if (type == typeof(short))
            {
                short[] intArr = arr.Cast<short>().ToArray();
                for (int i = 0; i < length; i++)
                {
                    Write(intArr[i]);
                }

                return;
            }

            if (type == typeof(ushort))
            {
                ushort[] intArr = arr.Cast<ushort>().ToArray();
                for (int i = 0; i < length; i++)
                {
                    Write(intArr[i]);
                }

                return;
            }

            throw new InvalidTypeException();
        }

        public void Write(ISerializable value)
        {
            value.Serialize(this);
        }
    }
}