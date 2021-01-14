using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Collections;

namespace roverstd
{
    public class ArrayPool<T> : IDisposable where T : struct
    {
        private readonly BlockingCollection<NativeArray<T>> m_FreeArrays;
        private readonly ISet<NativeArray<T>> m_AllArrays;

        public ArrayPool(int numArrays, int lengthForEach)
        {
            m_FreeArrays = new BlockingCollection<NativeArray<T>>();
            m_AllArrays = new HashSet<NativeArray<T>>();
            for (int i = 0; i < numArrays; i++)
            {
                NativeArray<T> arr = new NativeArray<T>(lengthForEach, Allocator.Persistent);
                m_FreeArrays.Add(arr);
                m_AllArrays.Add(arr);
            }
        }

        public int FreeArrayCount => m_FreeArrays.Count;

        public NativeArray<T> ObtainArray() => m_FreeArrays.Take();

        public void ReturnArray(NativeArray<T> array)
        {
            if (!m_AllArrays.Contains(array))
            {
                return;
            }

            m_FreeArrays.Add(array);
        }

        private void ReleaseUnmanagedResources()
        {
            foreach (NativeArray<T> arr in m_AllArrays)
            {
                arr.Dispose();
            }
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