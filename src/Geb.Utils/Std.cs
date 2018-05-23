using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Utils
{
    using SizeT = System.Int32;

    public struct Std
    {
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }

        public static unsafe void* Malloc(UInt32 size)
        {
            return (void*)Marshal.AllocHGlobal((Int32)size);
        }

        public static unsafe void* Malloc(SizeT size)
        {
            return (void*)Marshal.AllocHGlobal(size);
        }

        public static unsafe void* Malloc(Int32 num, SizeT size)
        {
            return (void*)Marshal.AllocHGlobal(num * size);
        }

        public static unsafe void Free(void* p)
        {
            Marshal.FreeHGlobal((IntPtr)p);
        }

        public static unsafe void* Memset(void* ptr, Byte val, SizeT num)
        {
            Byte* pStart = (Byte*)ptr;
            Byte* pEnd = pStart + num;
            while (pStart < pEnd)
            {
                *pStart = val;
                pStart++;
            }
            return ptr;
        }
    }
}
