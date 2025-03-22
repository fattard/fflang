using System;
using System.Buffers.Binary;

internal static class Builtins
{
    // ======== Built-ins
    public const int __INT_MAX__           = 2147483647;
    public const int __INT_WIDTH_BITS__    = 32;
    public const int __INT_WIDTH_BYTES__   = 4;
    public const int __INT_SIGN_BIT_MASK__ = -2147483648;

    public static int __builtin_int_add(int a, int b)
    {
        return a + b;
    }

    public static int __builtin_int_nand(int a, int b)
    {
        return ~(a & b);
    }

    public static int __builtin_int_mem_read(int addr)
    {
        return BinaryPrimitives.ReadInt32LittleEndian(Program.s_RAM.AsSpan(addr));
    }

    public static int __builtin_int_mem_write(int addr, int data)
    {
        BinaryPrimitives.WriteInt32LittleEndian(Program.s_RAM.AsSpan(addr), data);
        return 0;
    }

    public static bool __bool_check(int v)
    {
        return v != 0;
    }

    public static int __dbg_int(int v)
    {
        Console.WriteLine($"[DEBUG] {v}");
        return 0;
    }

    // ==================================

}
