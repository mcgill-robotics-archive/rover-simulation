using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Collections;

namespace roverstd
{
    public class ArrayPool<T> : IDisposable where T : struct
    {
        private readonly BlockingCollection<NativeArray<T>> m_FreeArrays;
        private readonly ConcurrentQueue<NativeArray<T>> m_DeallocationQueue;
        private int m_LengthForEach;

        public ArrayPool(int numArrays, int lengthForEach)
        {
            m_LengthForEach = lengthForEach;
            m_FreeArrays = new BlockingCollection<NativeArray<T>>();
            m_DeallocationQueue = new ConcurrentQueue<NativeArray<T>>();
            for (int i = 0; i < numArrays; i++)
            {
                NativeArray<T> arr = new NativeArray<T>(lengthForEach, Allocator.Persistent);
                m_FreeArrays.Add(arr);
            }
        }

        /// <summary>
        /// Must be called from the main thread
        /// </summary>
        public void EnsureExtraCapacity()
        {
            if (m_FreeArrays.Count < 20)
            {
                while (m_FreeArrays.Count < 60)
                {
                    m_FreeArrays.Add(new NativeArray<T>(m_LengthForEach, Allocator.Persistent));
                }

                ClearDeallocationQueue();
            }
        }

        /// <summary>
        /// Must be called from the main thread
        /// </summary>
        public void ClearDeallocationQueue()
        {
            while (!m_DeallocationQueue.IsEmpty)
            {
                m_DeallocationQueue.TryDequeue(out NativeArray<T> arr);
                arr.Dispose();
            }
        }

        public int FreeArrayCount => m_FreeArrays.Count;

        public NativeArray<T> ObtainArray() => m_FreeArrays.Take();
        // public NativeArray<T> ObtainArray() => new NativeArray<T>(m_LengthForEach, Allocator.Persistent);

        public void ReturnArray(NativeArray<T> array)
        {
            m_DeallocationQueue.Enqueue(array);
        }

        private void ReleaseUnmanagedResources()
        {
            foreach (NativeArray<T> arr in m_FreeArrays)
            {
                arr.Dispose();
            }

            ClearDeallocationQueue();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ArrayPool()
        {
            ReleaseUnmanagedResources();
        }
    }
}