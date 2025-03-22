using static Builtins;

internal static class PrepareStaticStrings
{
    #region Helpers

    public static int _not(int a)
    //func _not(a: Int) -> Int
    {
        return __builtin_int_nand(a, a);
    }

    public static int _and(int a, int b)
    //func _and(a: Int, b: Int) -> Int
    {
        var y = (int)default;
        /* set */ y = __builtin_int_nand(a, b);
        return __builtin_int_nand(y, y);
    }

    public static int _or(int a, int b)
    //func _or(a: Int, b: Int) -> Int
    {
        return __builtin_int_nand(__builtin_int_nand(a, a), __builtin_int_nand(b, b));
    }

    public static int _add(int a, int b)
    //func _add(a: Int, b: Int) -> Int
    {
        return __builtin_int_add(a, b);
    }

    public static int _sub(int a, int b)
    //func _sub(a: Int, b: Int) -> Int
    {
        return __builtin_int_add(a, __builtin_int_add(_not(b), 1));
    }

    public static int _mul(int a, int b)
    //func _mul(a: Int, b: Int) -> Int
    {
        var result = (int)default;

    LOOP:
        if (__bool_check(b))
        {
            /* set */ result = _add(result, a);
            /* set */ b = _sub(b, 1);
            goto LOOP;
        }

        return result;
    }

    public static int memRead(int addr)
    //func memRead(addr: Int) -> Int
    {
        return __builtin_int_mem_read(addr);
    }

    public static int memWrite(int addr, int data)
    //func memWrite(addr: Int, data: Int) -> Int
    {
        _ = __builtin_int_mem_write(addr, data);
        return 0;
    }

    public static int memWrite8(int addr, int data)
    //func memWrite8(addr: Int, data: Int) -> Int
    {
        var v = (int)default;

        // Little-endian
        /* set */ v = _and(__builtin_int_mem_read(addr), _not(255));
        _ = __builtin_int_mem_write(addr, _or(v, _and(data, 255)));
        return 0;
    }

    #endregion Helpers

    public static int updateStrTbl()
    //func updateStrTbl() -> Int
    {
        var g_offset = (int)default;
        var strTbl_pos = (int)default;
        var strBuff_pos = (int)default;

        /* set */ strTbl_pos = memRead(_add(g_offset, 80));
        /* set */ strBuff_pos = memRead(_add(g_offset, 84));

        _ = memWrite(strTbl_pos, strBuff_pos);
        _ = memWrite(_add(g_offset, 80), _add(strTbl_pos, __INT_WIDTH_BYTES__));

        return 0;
    }

    public static int setStrBuffPos(int offs)
    //func setStrBuffPos(offs: Int) -> Int
    {
        var g_offset = (int)default;
        _ = memWrite(_add(g_offset, 84), offs);

        return 0;
    }

    // Write Char
    public static int wc(int c)
    //func wc(c: Int) -> Int
    {
        var g_offset = (int)default;
        var strBuff_pos = (int)default;

        /* set */ strBuff_pos = memRead(_add(g_offset, 84));
        _ = memWrite8(strBuff_pos, c);
        _ = memWrite(_add(g_offset, 84), _add(strBuff_pos, 1));

        return 0;
    }

    public static int writeTypeName_Int()
    //func writeTypeName_Int() -> Int
    {
        _ = updateStrTbl();

        _ = wc(73);  // 'I'
        _ = wc(110); // 'n'
        _ = wc(116); // 't'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeMacroName_int_max()
    //func writeMacroName_int_max() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(73); // 'I'
        _ = wc(78); // 'N'
        _ = wc(84); // 'T'
        _ = wc(95); // '_'
        _ = wc(77); // 'M'
        _ = wc(65); // 'A'
        _ = wc(88); // 'X'
        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(0);  // '\0'

        return 0;
    }

    public static int writeMacroName_int_min()
    //func writeMacroName_int_min() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(73); // 'I'
        _ = wc(78); // 'N'
        _ = wc(84); // 'T'
        _ = wc(95); // '_'
        _ = wc(77); // 'M'
        _ = wc(73); // 'I'
        _ = wc(78); // 'N'
        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(0);  // '\0'

        return 0;
    }

    public static int writeMacroName_int_width_bits()
    //func writeMacroName_int_width_bits() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(73); // 'I'
        _ = wc(78); // 'N'
        _ = wc(84); // 'T'
        _ = wc(95); // '_'
        _ = wc(87); // 'W'
        _ = wc(73); // 'I'
        _ = wc(68); // 'D'
        _ = wc(84); // 'T'
        _ = wc(72); // 'H'
        _ = wc(95); // '_'
        _ = wc(66); // 'B'
        _ = wc(73); // 'I'
        _ = wc(84); // 'T'
        _ = wc(83); // 'S'
        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(0);  // '\0'

        return 0;
    }

    public static int writeMacroName_int_width_bytes()
    //func writeMacroName_int_width_bytes() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(73); // 'I'
        _ = wc(78); // 'N'
        _ = wc(84); // 'T'
        _ = wc(95); // '_'
        _ = wc(87); // 'W'
        _ = wc(73); // 'I'
        _ = wc(68); // 'D'
        _ = wc(84); // 'T'
        _ = wc(72); // 'H'
        _ = wc(95); // '_'
        _ = wc(66); // 'B'
        _ = wc(89); // 'Y'
        _ = wc(84); // 'T'
        _ = wc(69); // 'E'
        _ = wc(83); // 'S'
        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(0);  // '\0'

        return 0;
    }

    public static int writeMacroName_int_sign_bit_mask()
    //func writeMacroName_int_sign_bit_mask() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(73); // 'I'
        _ = wc(78); // 'N'
        _ = wc(84); // 'T'
        _ = wc(95); // '_'
        _ = wc(83); // 'S'
        _ = wc(73); // 'I'
        _ = wc(71); // 'G'
        _ = wc(78); // 'N'
        _ = wc(95); // '_'
        _ = wc(66); // 'B'
        _ = wc(73); // 'I'
        _ = wc(84); // 'T'
        _ = wc(95); // '_'
        _ = wc(77); // 'M'
        _ = wc(65); // 'A'
        _ = wc(83); // 'S'
        _ = wc(75); // 'K'
        _ = wc(95); // '_'
        _ = wc(95); // '_'
        _ = wc(0);  // '\0'

        return 0;
    }

    public static int writeKeyword_using()
    //func writeKeyword_using() -> Int
    {
        _ = updateStrTbl();

        _ = wc(117); // 'u'
        _ = wc(115); // 's'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(103); // 'g'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeKeyword_func()
    //func writeKeyword_func() -> Int
    {
        _ = updateStrTbl();

        _ = wc(102); // 'f'
        _ = wc(117); // 'u'
        _ = wc(110); // 'n'
        _ = wc(99);  // 'c'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeKeyword_var()
    //func writeKeyword_var() -> Int
    {
        _ = updateStrTbl();

        _ = wc(118); // 'v'
        _ = wc(97);  // 'a'
        _ = wc(114); // 'r'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeKeyword_set()
    //func writeKeyword_set() -> Int
    {
        _ = updateStrTbl();

        _ = wc(115); // 's'
        _ = wc(101); // 'e'
        _ = wc(116); // 't'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeKeyword_return()
    //func writeKeyword_return() -> Int
    {
        _ = updateStrTbl();

        _ = wc(114); // 'r'
        _ = wc(101); // 'e'
        _ = wc(116); // 't'
        _ = wc(117); // 'u'
        _ = wc(114); // 'r'
        _ = wc(110); // 'n'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeKeyword_goto()
    //func writeKeyword_goto() -> Int
    {
        _ = updateStrTbl();

        _ = wc(103); // 'g'
        _ = wc(111); // 'o'
        _ = wc(116); // 't'
        _ = wc(111); // 'o'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeKeyword_if()
    //func writeKeyword_if() -> Int
    {
        _ = updateStrTbl();

        _ = wc(105); // 'i'
        _ = wc(102); // 'f'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeKeyword_else()
    //func writeKeyword_else() -> Int
    {
        _ = updateStrTbl();

        _ = wc(101); // 'e'
        _ = wc(108); // 'l'
        _ = wc(115); // 's'
        _ = wc(101); // 'e'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeKeyword_underscoreSymbol()
    //func writeKeyword_underscoreSymbol() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95); // '_'
        _ = wc(0);  // '\0'

        return 0;
    }

    public static int write_boolCheck()
    //func write_boolCheck() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95);  // '_'
        _ = wc(95);  // '_'
        _ = wc(98);  // 'b'
        _ = wc(111); // 'o'
        _ = wc(111); // 'o'
        _ = wc(108); // 'l'
        _ = wc(95);  // '_'
        _ = wc(99);  // 'c'
        _ = wc(104); // 'h'
        _ = wc(101); // 'e'
        _ = wc(99);  // 'c'
        _ = wc(107); // 'k'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeBuiltin_add()
    //func writeBuiltin_add() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95);  // '_'
        _ = wc(95);  // '_'
        _ = wc(98);  // 'b'
        _ = wc(117); // 'u'
        _ = wc(105); // 'i'
        _ = wc(108); // 'l'
        _ = wc(116); // 't'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(95);  // '_'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(116); // 't'
        _ = wc(95);  // '_'
        _ = wc(97);  // 'a'
        _ = wc(100); // 'd'
        _ = wc(100); // 'd'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeBuiltin_nand()
    //func writeBuiltin_nand() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95);  // '_'
        _ = wc(95);  // '_'
        _ = wc(98);  // 'b'
        _ = wc(117); // 'u'
        _ = wc(105); // 'i'
        _ = wc(108); // 'l'
        _ = wc(116); // 't'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(95);  // '_'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(116); // 't'
        _ = wc(95);  // '_'
        _ = wc(110); // 'n'
        _ = wc(97);  // 'a'
        _ = wc(110); // 'n'
        _ = wc(100); // 'd'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeBuiltin_mem_read()
    //func writeBuiltin_mem_read() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95);  // '_'
        _ = wc(95);  // '_'
        _ = wc(98);  // 'b'
        _ = wc(117); // 'u'
        _ = wc(105); // 'i'
        _ = wc(108); // 'l'
        _ = wc(116); // 't'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(95);  // '_'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(116); // 't'
        _ = wc(95);  // '_'
        _ = wc(109); // 'm'
        _ = wc(101); // 'e'
        _ = wc(109); // 'm'
        _ = wc(95);  // '_'
        _ = wc(114); // 'r'
        _ = wc(101); // 'e'
        _ = wc(97);  // 'a'
        _ = wc(100); // 'd'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int writeBuiltin_mem_write()
    //func writeBuiltin_mem_write() -> Int
    {
        _ = updateStrTbl();

        _ = wc(95);  // '_'
        _ = wc(95);  // '_'
        _ = wc(98);  // 'b'
        _ = wc(117); // 'u'
        _ = wc(105); // 'i'
        _ = wc(108); // 'l'
        _ = wc(116); // 't'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(95);  // '_'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(116); // 't'
        _ = wc(95);  // '_'
        _ = wc(109); // 'm'
        _ = wc(101); // 'e'
        _ = wc(109); // 'm'
        _ = wc(95);  // '_'
        _ = wc(119); // 'w'
        _ = wc(114); // 'r'
        _ = wc(105); // 'i'
        _ = wc(116); // 't'
        _ = wc(101); // 'e'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int write_main()
    //func write_main() -> Int
    {
        _ = updateStrTbl();

        _ = wc(109); // 'm'
        _ = wc(97);  // 'a'
        _ = wc(105); // 'i'
        _ = wc(110); // 'n'
        _ = wc(0);   // '\0'

        return 0;
    }

    public static int main()
    //func main() -> Int
    {
        var g_offset = (int)default;
        var strTbl_pos = (int)default;
        var strBuff_pos = (int)default;

        /* set */ strTbl_pos = _add(g_offset, 80);
        /* set */ strBuff_pos = _add(g_offset, 84);

        _ = memWrite(strTbl_pos, 512000);
        _ = memWrite(strBuff_pos, _add(512000, 1024));

        _ = write_main(); // ID 0
        _ = write_boolCheck(); // ID 1
        _ = writeBuiltin_add(); // ID 2
        _ = writeBuiltin_nand(); // ID 3
        _ = writeBuiltin_mem_read(); // ID 4
        _ = writeBuiltin_mem_write(); // ID 5
        _ = writeTypeName_Int(); // ID 6
        _ = writeMacroName_int_max(); // ID 7
        _ = writeMacroName_int_min(); // ID 8
        _ = writeMacroName_int_width_bits(); // ID 9
        _ = writeMacroName_int_width_bytes(); // ID 10
        _ = writeMacroName_int_sign_bit_mask(); // ID 11
        _ = writeKeyword_using(); // ID 12
        _ = writeKeyword_func(); // ID 13
        _ = writeKeyword_var(); // ID 14
        _ = writeKeyword_set(); // ID 15
        _ = writeKeyword_return(); // ID 16
        _ = writeKeyword_goto(); // ID 17
        _ = writeKeyword_if(); // ID 18
        _ = writeKeyword_else(); // ID 19
        _ = writeKeyword_underscoreSymbol(); // ID 20

        return 0;
    }
}
