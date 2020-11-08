using System;
using System.IO;

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

        public byte[] ToArray()
        {
            return m_Array.Clone() as byte[];
        }
    }
}