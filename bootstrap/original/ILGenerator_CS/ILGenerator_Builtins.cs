using System;
using System.Buffers.Binary;

internal partial class ILGenerator
{

    // ======== Built-ins
    const int __INT_MAX__           = 2147483647;
    const int __INT_WIDTH_BITS__    = 32;
    const int __INT_WIDTH_BYTES__   = 4;
    const int __INT_SIGN_BIT_MASK__ = -2147483648;

    private static int __builtin_int_add(int a, int b)
    {
        return a + b;
    }

    private static int __builtin_int_nand(int a, int b)
    {
        return ~(a & b);
    }

    private static int __builtin_int_mem_read(int addr)
    {
        return BinaryPrimitives.ReadInt32LittleEndian(Program.s_RAM.AsSpan(addr));
    }

    private static int __builtin_int_mem_write(int addr, int data)
    {
        BinaryPrimitives.WriteInt32LittleEndian(Program.s_RAM.AsSpan(addr), data);
        return 0;
    }

    private static bool __bool_check(int v)
    {
        return v != 0;
    }

    private static int __dbg_int(int v)
    {
        Console.WriteLine($"[DEBUG] {v}");
        return 0;
    }

    // ==================================

}
