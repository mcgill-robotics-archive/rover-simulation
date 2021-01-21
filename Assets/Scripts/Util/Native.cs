using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;


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

        [DllImport("msvcrt.dll", EntryPoint = "fopen"), SuppressUnmanagedCodeSecurity]
        public static extern void* fopen(string filename, string mode);

        [DllImport("msvcrt.dll", EntryPoint = "fclose"), SuppressUnmanagedCodeSecurity]
        public static extern int fclose(void* file);

        [DllImport("msvcrt.dll", EntryPoint = "fread"), SuppressUnmanagedCodeSecurity]
        public static extern size_t fread(void* ptr, size_t size, size_t nmemb, void* stream);

        [DllImport("msvcrt.dll", EntryPoint = "fwrite"), SuppressUnmanagedCodeSecurity]
        public static extern size_t fwrite(void* ptr, size_t size, size_t nmemb, void* stream);

#elif PLATFORM_STANDALONE_OSX
        [DllImport("libSystem.dylib", EntryPoint = "memcpy"), SuppressUnmanagedCodeSecurity]
        public static extern void memcpy(void* dest, void* src, size_t length);

        [DllImport("libSystem.dylib", EntryPoint = "memset"), SuppressUnmanagedCodeSecurity]
        public static extern void memset(void* ptr, int value, size_t num);

        [DllImport("libSystem.dylib", EntryPoint = "malloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* malloc(size_t num);

        [DllImport("libSystem.dylib", EntryPoint = "free"), SuppressUnmanagedCodeSecurity]
        public static extern void free(void* ptr);

        [DllImport("libSystem.dylib", EntryPoint = "calloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* calloc(size_t num, size_t size);

        [DllImport("libSystem.dylib", EntryPoint = "realloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* realloc(void* ptr, size_t size);

        [DllImport("libSystem.dylib", EntryPoint = "fopen"), SuppressUnmanagedCodeSecurity]
        public static extern void* fopen(string filename, string mode);

        [DllImport("libSystem.dylib", EntryPoint = "fclose"), SuppressUnmanagedCodeSecurity]
        public static extern int fclose(void* file);

        [DllImport("libSystem.dylib", EntryPoint = "fread"), SuppressUnmanagedCodeSecurity]
        public static extern size_t fread(void* ptr, size_t size, size_t nmemb, void* stream);

        [DllImport("libSystem.dylib", EntryPoint = "fwrite"), SuppressUnmanagedCodeSecurity]
        public static extern size_t fwrite(void* ptr, size_t size, size_t nmemb, void* stream);


#else
        // linux
        [DllImport("libc.so.6", EntryPoint = "memcpy"), SuppressUnmanagedCodeSecurity]
        public static extern void memcpy(void* dest, void* src, size_t length);

        [DllImport("libc.so.6", EntryPoint = "memset"), SuppressUnmanagedCodeSecurity]
        public static extern void memset(void* ptr, int value, size_t num);

        [DllImport("libc.so.6", EntryPoint = "malloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* malloc(size_t num);

        [DllImport("libc.so.6", EntryPoint = "free"), SuppressUnmanagedCodeSecurity]
        public static extern void free(void* ptr);

        [DllImport("libc.so.6", EntryPoint = "calloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* calloc(size_t num, size_t size);

        [DllImport("libc.so.6", EntryPoint = "realloc"), SuppressUnmanagedCodeSecurity]
        public static extern void* realloc(void* ptr, size_t size);

        [DllImport("libc.so.6", EntryPoint = "fopen"), SuppressUnmanagedCodeSecurity]
        public static extern void* fopen(string filename, string mode);

        [DllImport("libc.so.6", EntryPoint = "fclose"), SuppressUnmanagedCodeSecurity]
        public static extern int fclose(void* file);

        [DllImport("libc.so.6", EntryPoint = "fread"), SuppressUnmanagedCodeSecurity]
        public static extern size_t fread(void* ptr, size_t size, size_t nmemb, void* stream);

        [DllImport("libc.so.6", EntryPoint = "fwrite"), SuppressUnmanagedCodeSecurity]
        public static extern size_t fwrite(void* ptr, size_t size, size_t nmemb, void* stream);


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