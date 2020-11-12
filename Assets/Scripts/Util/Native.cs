﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEditor.Build.Reporting;
using UnityEngine.UIElements;


namespace Assets.Scripts.Util
{
#if UNITY_64
    using size_t = System.UInt64;

#else
    using size_t = System.UInt32;
#endif
    public static unsafe class Native
    {
#if PLATFORM_STANDALONE_WIN
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "RtlCopyMemory",
             CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity]
        private static extern void RtlCopyMemory(void* Destination, void* Source, size_t Length);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "RtlFillMemory",
             CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity]
        private static extern void RtlFillMemory(void* Destination, size_t Length, int Fill);

#else
        // linux
        private static extern void
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void memcpy(void* dest, void* src, size_t length)
        {
#if PLATFORM_STANDALONE_WIN
            RtlCopyMemory(dest, src, length);
#else
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void memcpy(void* dest, void* src, int length)
        {
            memcpy(dest, src, (size_t) length);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void memset(void* ptr, int value, size_t num)
        {
#if PLATFORM_STANDALONE_WIN
            RtlFillMemory(ptr, num, value);
#else
#endif
        }
    }
}