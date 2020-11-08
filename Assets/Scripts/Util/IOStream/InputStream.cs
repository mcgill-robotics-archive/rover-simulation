using System;
using System.IO;
using System.Threading;

namespace Rover.Util.IOStream
{

    public interface IInputStream<out T>
    {
        int Count { get; }

        T Read();

        T[] Read(int count);

        T[] ToArray();
    }

    public class ByteArrayInputStream : IInputStream<byte>
    {
        private readonly byte[] m_Array;
        private int m_NextIndex;

        public ByteArrayInputStream(byte[] arr)
        {
            m_Array = arr.Clone() as byte[];
            m_NextIndex = 0;
        }

        public int Count => m_Array.Length;

        public byte Read()
        {
            lock (m_Array)
            {
                if (m_NextIndex == Count)
                {
                    throw new EndOfStreamException();
                }

                return m_Array[m_NextIndex++];
            }
        }

        public byte[] Read(int count)
        {
            lock (m_Array)
            {
                byte[] arr = new byte[count];
                if (count > Count - m_NextIndex)
                {
                    throw new EndOfStreamException();
                }

                Array.Copy(m_Array, m_NextIndex, arr, 0, count);
                return arr;
            }
        }

        public short ReadShort()
        {
            lock (m_Array)
            {
                short value = BitConverter.ToInt16(m_Array, m_NextIndex);
                m_NextIndex += sizeof(short);
                return value;
            }
        }


        public int ReadInt()
        {
            lock (m_Array)
            {
                int value = BitConverter.ToInt32(m_Array, m_NextIndex);
                m_NextIndex += sizeof(int);
                return value;
            }
        }


        public long ReadLong()
        {
            lock (m_Array)
            {
                long value = BitConverter.ToInt64(m_Array, m_NextIndex);
                m_NextIndex += sizeof(long);
                return value;
            }
        }


        public float ReadFloat()
        {
            lock (m_Array)
            {
                float value = BitConverter.ToSingle(m_Array, m_NextIndex);
                m_NextIndex += sizeof(float);
                return value;
            }
        }


        public double ReadDouble()
        {
            lock (m_Array)
            {
                double value = BitConverter.ToDouble(m_Array, m_NextIndex);
                m_NextIndex += sizeof(double);
                return value;
            }
        }


        public char ReadChar()
        {
            lock (m_Array)
            {
                char value = BitConverter.ToChar(m_Array, m_NextIndex);
                m_NextIndex += sizeof(char);
                return value;
            }
        }


        public bool ReadBool()
        {
            lock (m_Array)
            {
                bool value = BitConverter.ToBoolean(m_Array, m_NextIndex);
                m_NextIndex += sizeof(bool);
                return value;
            }
        }

        public byte[] ToArray()
        {
            return m_Array.Clone() as byte[];
        }
    }
}