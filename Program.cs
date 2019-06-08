using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using static System.Runtime.Intrinsics.X86.Avx;
using static System.Runtime.Intrinsics.X86.Avx2;

namespace bugbugbug
{
    class Program
    {
        static ReadOnlySpan<byte> PermTable => new byte[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, /* 0*/
            0, 1, 2, 3, 4, 5, 6, 7, /* 0*/
            0, 1, 2, 3, 4, 5, 6, 7, /* 0*/
            0, 1, 2, 3, 4, 5, 6, 7, /* 0*/
            0, 1, 2, 3, 4, 5, 6, 7, /* 0*/
            0, 1, 2, 3, 4, 5, 6, 7, /* 0*/
            0, 1, 2, 3, 4, 5, 6, 7, /* 0*/
            0, 1, 2, 3, 4, 5, 6, 7, /* 0*/
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe Vector256<int> GetPermutation(byte* pBase, int pvbyte)
        {
            Debug.Assert(pvbyte >= 0);
            Debug.Assert(pvbyte < 255);
            Debug.Assert(pBase != null);
            //return ConvertToVector256Int32(Unsafe.Read<Vector128<byte>>(PermTablePtr + pvbyte * 8));
            //return ConvertToVector256Int32(Sse2.LoadVector128(pBase + pvbyte * 8));
            //Console.WriteLine($"R: {(ulong) pBase} {(ulong) pvbyte} {(ulong)(pBase + pvbyte * 8)}");
            return ConvertToVector256Int32(pBase + pvbyte * 8);
        }

        static unsafe void Main(string[] args)
        {
            var src = new int[1024];
            fixed (int* pSrc = &src[0])
            fixed (byte *pBase = &PermTable[0])
            {
                
                for (var i = 0; i < 100; i++)
                {
                    var srcv = LoadDquVector256(pSrc + i);
                    var pe = i & 0x7;
                    var permuted = PermuteVar8x32(srcv, GetPermutation(pBase, (int)pe));
                    Store(pSrc + i, permuted);
                }
            }
        }
    }

}
