using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Geb.Utils
{
    using SizeT = System.Int32;

    public struct Std
    {
        public static unsafe void* Malloc(SizeT size)
        {
            return (void*)Marshal.AllocHGlobal(size);
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
