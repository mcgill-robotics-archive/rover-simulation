using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEditor.Build.Reporting;
using UnityEngine.UIElements;


namespace roverstd
{
#if UNITY_64
    using size_t_value_type = System.UInt64;

#else
    using size_t_value_type = System.UInt32;
#endif

    public static unsafe class Native
    {
#if UNITY_64
        private static readonly UnmanagedType size_t_marshal_type = UnmanagedType.U8;
#else
        private static readonly UnmanagedType size_t_marshal_type = UnmanagedType.U4;
#endif

        public struct size_t
        {
            public size_t_value_type Value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public size_t(size_t_value_type value)
            {
                Value = value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator size_t(size_t_value_type value)
            {
                return new size_t(value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator size_t_value_type(size_t value)
            {
                return value.Value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator size_t(int value)
            {
                return new size_t((size_t_value_type) value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator size_t(long value)
            {
                return new size_t((size_t_value_type) value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static implicit operator int(size_t value)
            {
                return (int) value.Value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static size_t operator -(size_t lhs, size_t rhs)
            {
                return lhs.Value - rhs.Value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static size_t operator +(size_t lhs, size_t rhs)
            {
                return lhs.Value + rhs.Value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static long operator -(size_t lhs)
            {
                return -(long)lhs.Value;
            }
        }


#if PLATFORM_STANDALONE_WIN
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "RtlCopyMemory",
             CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity]
        private static extern void RtlCopyMemory(void* Destination, void* Source, size_t Length);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "RtlFillMemory",
             CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity]
        private static extern void RtlFillMemory(void* Destination, size_t Length, int Fill);

        [DllImport("msvcrt.dll", EntryPoint = "malloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* malloc(size_t num);

        [DllImport("msvcrt.dll", EntryPoint = "free"), SuppressUnmanagedCodeSecurity]
        public static extern void free(void* ptr);

        [DllImport("msvcrt.dll", EntryPoint = "calloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* calloc(size_t num, size_t size);

        [DllImport("msvcrt.dll", EntryPoint = "realloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* realloc(void* ptr, size_t size);

#else
        // linux
        [DllImport("libc.so", EntryPoint = "memcpy"), SuppressUnmanagedCodeSecurity]
        public static extern void memcpy(void* dest, void* src, size_t length);

        [DllImport("libc.so", EntryPoint = "memset"), SuppressUnmanagedCodeSecurity]
        public static extern void memset(void* ptr, int value, size_t num);

        [DllImport("libc.so", EntryPoint = "malloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* malloc(size_t num);

        [DllImport("libc.so", EntryPoint = "free"), SuppressUnmanagedCodeSecurity]
        public static extern void free(void* ptr);

        [DllImport("libc.so", EntryPoint = "calloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* calloc(size_t num, size_t size);

        [DllImport("libc.so", EntryPoint = "realloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* realloc(void* ptr, size_t size);

#endif
#if PLATFORM_STANDALONE_WIN
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void memcpy(void* dest, void* src, size_t length)
        {
#if MEMCPY_PREFER_NATIVE
            RtlCopyMemory(dest, src, length);
#else
            Buffer.MemoryCopy(src, dest, length, length);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void memset(void* ptr, int value, size_t num)
        {
            RtlFillMemory(ptr, num, value);
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf<T>(T[] arr) where T : unmanaged
        {
            return arr.Length * sizeof(T);
        }
    }
}