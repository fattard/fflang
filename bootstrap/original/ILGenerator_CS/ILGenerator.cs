internal partial class ILGenerator
{

    // ======== FFLang program

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

    public static int _xor(int a, int b)
    //func _xor(a: Int, b: Int) -> Int
    {
        var nand_ab = (int)default;
        /* set */ nand_ab = __builtin_int_nand(a, b);
        return __builtin_int_nand(__builtin_int_nand(a, nand_ab), __builtin_int_nand(b, nand_ab));
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

    public static int _eq(int a, int b)
    //func _eq(a: Int, b: Int) -> Int
    {
        var result = (int)default;
        if (__bool_check(_xor(a, b)))
        {
            /* set */ result = 0;
        }
        else
        {
            /* set */ result = 1;
        }

        return result;
    }

    public static int _neq(int a, int b)
    //func _neq(a: Int, b: Int) -> Int
    {
        var result = (int)default;
        if (__bool_check(_xor(a, b)))
        {
            /* set */ result = 1;
        }
        else
        {
            /* set */ result = 0;
        }

        return result;
    }

    public static int _isNeg(int a)
    //func _isNeg(a: Int) -> Int
    {
        var result = (int)default;
        if (__bool_check(_and(a, _not(__INT_MAX__))))
        {
            /* set */ result = 1;
        }
        else
        {
            /* set */ result = 0;
        }

        return result;
    }

    public static int _lt(int a, int b)
    //func _lt(a: Int, b: Int) -> Int
    {
        return _isNeg(_sub(a, b));
    }

    public static int _gt(int a, int b)
    //func _gt(a: Int, b: Int) -> Int
    {
        return _lt(b, a);
    }

    public static int _lte(int a, int b)
    //func _lte(a: Int, b: Int) -> Int
    {
        return _or(_lt(a, b), _eq(a, b));
    }

    public static int _gte(int a, int b)
    //func _gte(a: Int, b: Int) -> Int
    {
        return _or(_gt(a, b), _eq(a, b));
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

    public static int memRead8(int addr)
    //func memRead8(addr: Int) -> Int
    {
        // Little-endian
        return _and(__builtin_int_mem_read(addr), 255);
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

    public static int memCpy(int srcAddr, int destAddr, int len)
    //func memCpy(srcAddr: Int, destAddr: Int, len: Int) -> Int
    {
        var v = (int)default;
        var i = (int)default;

    LOOP:
        if (__bool_check(_sub(len, i)))
        {
            /* set */ v = memRead8(_add(srcAddr, i));
            _ = memWrite8(_add(destAddr, i), v);

            /* set */ i = _add(i, 1);

            goto LOOP;
        }

        return 0;
    }

    public static int memSet(int addr, int data, int len)
    //func memSet(addr: Int, data: Int, len: Int) -> Int
    {
        var i = (int)default;

    LOOP:
        if (__bool_check(_sub(len, i)))
        {
            _ = memWrite8(_add(addr, i), data);

            /* set */ i = _add(i, 1);

            goto LOOP;
        }

        return 0;
    }

    public static int strEquals(int s1, int s2)
    //func strEquals(s1: Int, s2: Int) -> Int
    {
        var pos = (int)default;
        var result = (int)default;
        var c1 = (int)default;
        var c2 = (int)default;

        /* set */ result = 1;

    LOOP:
        /* set */ c1 = memRead8(_add(pos, s1));
        /* set */ c2 = memRead8(_add(pos, s2));
        if (__bool_check(_neq(c1, c2)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(_or(_eq(c1, 0), _eq(c2, 0)))) // '\0'
        {
            goto END;
        }

        /* set */ pos = _add(pos, 1);

        goto LOOP;

    END:
        return result;
    }

    public static int strCpy(int srcStr, int dest)
    //func strCpy(srcStr: Int, dest: Int) -> Int
    {
        var pos = (int)default;
        var c = (int)default;

    LOOP:
        /* set */ c = memRead8(_add(pos, srcStr));
        _ = memWrite8(_add(pos, dest), c);

        /* set */ pos = _add(pos, 1);

        if (__bool_check(_eq(c, 0))) // '\0'
        {
            goto END;
        }

        goto LOOP;

    END:
        // Returns the length of the copied string srcStr
        return pos;
    }

    public static int isDigit(int c)
    //func isDigit(c: Int) -> Int
    {
        var result = (int)default;

        if (__bool_check(_gte(c, 48))) // '0'
        {
            if (__bool_check(_lte(c, 57))) // '9'
            {
                /* set */ result = 1;
            }
        }

        return result;
    }

    public static int isLetter(int c)
    //func isLetter(c: Int) -> Int
    {
        var result = (int)default;

        //[A-Z]
        if (__bool_check(_gte(c, 65))) // 'A'
        {
            if (__bool_check(_lte(c, 90))) // 'Z'
            {
                /* set */ result = 1;
                goto END;
            }
        }

        // [a-z]
        if (__bool_check(_gte(c, 97))) // 'a'
        {
            if (__bool_check(_lte(c, 122))) // 'z'
            {
                /* set */ result = 1;
                goto END;
            }
        }

    END:
        return result;
    }

    public static int main()
    //func main() -> Int
    {
        return 0;
    }
}
