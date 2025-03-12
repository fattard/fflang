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


    // === Allocations for global variables
    //
    // g_offset = 0
    //
    // & s_IL_Emitter_strBuffer         := g_offset + 64
    // & s_IL_Emitter_strBuffer_pos     := g_offset + 68
    // & s_IL_Emitter_ifElseScopeCount  := g_offset + 72
    // & s_IL_Emitter_baseILBegin_txt   := g_offset + 76
    // & s_IL_Emitter_baseILEnd_txt     := g_offset + 80
    // & s_IL_Emitter_RuntimeJson_txt   := g_offset + 84
    //
    // & s_tokenStrBuff                 := g_offset + 120
    // & s_tokenStrBuff_pos             := g_offset + 124
    //
    // & s_curToken_type                := g_offset + 140
    // & s_curToken_value               := g_offset + 144
    // & s_curToken_line                := g_offset + 148
    //
    // & s_srcCodeTxt                   := g_offset + 160
    // & s_pos                          := g_offset + 164
    // & s_line                         := g_offset + 168
    // & s_lastValidLine                := g_offset + 172
    // & s_curScopeDepth                := g_offset + 176
    // & s_inFunctionScope              := g_offset + 180
    // & s_paramCount                   := g_offset + 184
    //
    //
    //
    // === Hardcoded pointers
    //
    // globals data arena               = g_offset          (max len: 32 KB)
    //
    // s_curToken_value                 = g_offset + 200    (max len: 256 bytes)
    // s_tokenStrBuff                   = g_offset + 460    (max len: 256 bytes)
    // s_tmpBuff                        = g_offset + 800    (max len: 256 bytes)
    //
    // s_IL_Emitter_baseILBegin_txt     = 32768             (max len: 8 KB)
    // s_IL_Emitter_baseILEnd_txt       = 40960             (max len: 128 bytes)
    // s_IL_Emitter_runtimeJson_txt     = 41088             (max len: 512 bytes)
    // s_srcCodeTxt                     = 65536             (max len: 64 KB)
    // s_IL_Emitter_strBuffer           = 131072            (max len: 64 KB)
    //

    public static int initGlobals()
    //func initGlobals() -> Int
    {
        var g_offset = (int)default;

        /* set */ g_offset = 0;

        // Clean globals data arena - 32 KB len
        _ = memSet(g_offset, 0, 32768);

        // Write hardcoded pointers
        _ = memWrite(_add(g_offset, 64), 131072); // s_IL_Emitter_strBuffer
        _ = memWrite(_add(g_offset, 76), 32768); // s_IL_Emitter_baseILBegin_txt
        _ = memWrite(_add(g_offset, 80), 40960); // s_IL_Emitter_baseILEnd_txt
        _ = memWrite(_add(g_offset, 84), 41088); // s_IL_Emitter_runtimeJson_txt
        _ = memWrite(_add(g_offset, 120), _add(g_offset, 460)); // s_tokenStrBuff
        _ = memWrite(_add(g_offset, 144), _add(g_offset, 200)); // s_curToken_value
        _ = memWrite(_add(g_offset, 160), 65536); // s_srcCodeTxt


        // Clean s_IL_Emitter_strBuffer - 64 KB length
        _ = memSet(IL_Emitter_getStrBuffer_ptr(), 0, 65536);

        return 0;
    }

    public static int initParser()
    //func initParser() -> Int
    {
        var g_offset = (int)default;

        /* set */ g_offset = 0;

        _ = memWrite(_add(g_offset, 140), 100); // s_curToken_type = TokenType.UNKNOWN
        _ = memWrite(getCurTokenValue_ptr(), 0); // s_curToken_value[0] = '\0'
        _ = memWrite(_add(g_offset, 148), 1); // s_curToken_line = 1

        _ = memWrite(Lexer_getTokenStrBuff_ptr(), 0); // s_tokenStrBuff[0] = '\0'
        _ = memWrite(_add(g_offset, 124), 0); // s_tokenStrBuff_pos = 0

        _ = memWrite(_add(g_offset, 164), 0); // s_pos = 0
        _ = memWrite(_add(g_offset, 168), 1); // s_line = 1
        _ = memWrite(_add(g_offset, 172), 1); // s_lastValidLine = 1
        _ = memWrite(_add(g_offset, 176), 0); // s_curScopeDepth = 0
        _ = memWrite(_add(g_offset, 180), 0); // s_inFunctionScope = 0
        _ = memWrite(_add(g_offset, 172), 0); // s_paramCount = 0

        return 0;
    }

    public static int Lexer_getTokenStrBuff_ptr()
    //func Lexer_getTokenStrBuff_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 120)); // s_tokenStrBuff
    }

    public static int Lexer_getTokenStrBuffPos()
    //func Lexer_getTokenStrBuffPos() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 124)); // s_tokenStrBuff_pos
    }

    public static int Lexer_resetTokenStrBuffPos()
    //func Lexer_resetTokenStrBuffPos() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memWrite(_add(g_offset, 124), 0); // s_tokenStrBuff_pos
    }

    public static int Lexer_getSrcCodeTxt_ptr()
    //func Lexer_getSrcCodeTxt_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 160)); // s_srcCodeTxt
    }

    public static int Lexer_getPos()
    //func Lexer_getPos() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 164)); // s_pos
    }

    public static int Lexer_getLine()
    //func Lexer_getLine() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 168)); // s_line
    }

    public static int Lexer_getLastValidLine()
    //func Lexer_getLastValidLine() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 172)); // s_lastValidLine
    }

    public static int Lexer_incPos()
    //func Lexer_incPos() -> Int
    {
        var g_offset = (int)default;
        var pos = (int)default;

        /* set */ g_offset = 0;

        /* set */ pos = _add(memRead(_add(g_offset, 164)), 1); // s_pos + 1
        _ = memWrite(_add(g_offset, 164), pos); // s_pos

        // returns incremented pos
        return pos;
    }

    public static int Lexer_incLine()
    //func Lexer_incLine() -> Int
    {
        var g_offset = (int)default;
        var line = (int)default;

        /* set */ g_offset = 0;

        /* set */ line = _add(memRead(_add(g_offset, 168)), 1); // s_line + 1
        _ = memWrite(_add(g_offset, 168), line); // s_line

        // returns incremented line
        return line;
    }

    public static int getCurTokenType()
    //func getCurTokenType() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 140)); // s_curToken_type
    }

    public static int getCurTokenValue_ptr()
    //func getCurTokenValue_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 144)); // s_curToken_value
    }

    public static int getCurTokenLine()
    //func getCurTokenLine() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 148)); // s_curToken_line
    }

    public static int setCurToken(int type, int valueStr_ptr, int line)
    //func setCurToken(type: Int, valueStr_ptr: Int, line: Int) -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        _ = memWrite(_add(g_offset, 140), type);
        _ = memWrite(_add(g_offset, 148), line);
        _ = strCpy(valueStr_ptr, getCurTokenValue_ptr());

        return 0;
    }

    public static int matchTokenType(int type)
    //func matchTokenType(type: Int) -> Int
    {
        return _eq(type, getCurTokenType());
    }

    public static int notMatchTokenType(int type)
    //func notMatchTokenType(type: Int) -> Int
    {
        return _neq(type, getCurTokenType());
    }

    public static int getCurScopeDepth()
    //func getCurScopeDepth() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 176)); // s_curScopeDepth
    }

    public static int incCurScopeDepth()
    //func incCurScopeDepth() -> Int
    {
        var g_offset = (int)default;
        var depth = (int)default;

        /* set */ g_offset = 0;

        /* set */ depth = _add(memRead(_add(g_offset, 176)), 1); // s_curScopeDepth + 1
        _ = memWrite(_add(g_offset, 176), depth); // s_curScopeDepth

        // returns incremented curScopeDepth
        return depth;
    }

    public static int decCurScopeDepth()
    //func decCurScopeDepth() -> Int
    {
        var g_offset = (int)default;
        var depth = (int)default;

        /* set */ g_offset = 0;

        /* set */ depth = _sub(memRead(_add(g_offset, 176)), 1); // s_curScopeDepth - 1
        _ = memWrite(_add(g_offset, 176), depth); // s_curScopeDepth

        // returns decremented curScopeDepth
        return depth;
    }

    public static int isInFunctionScope()
    //func isInFunctionScope() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 180)); // s_inFunctionScope
    }

    public static int getParamCount()
    //func getParamCount() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 184)); // s_paramCount
    }

    public static int setParamCount(int count)
    //func setParamCount(count: Int) -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        _ = memWrite(_add(g_offset, 184), count); // s_paramCount
        return 0;
    }

    public static int incParamCount()
    //func incParamCount() -> Int
    {
        var g_offset = (int)default;
        var count = (int)default;

        /* set */ g_offset = 0;

        /* set */ count = _add(memRead(_add(g_offset, 184)), 1); // s_paramCount + 1
        _ = memWrite(_add(g_offset, 184), count); // s_paramCount

        // returns incremented paramCount
        return count;
    }

    public static int getKeyword_using()
    //func getKeyword_using() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 117); // 'u'
        _ = memWrite(_add(tmpBuff, 1), 115); // 's'
        _ = memWrite(_add(tmpBuff, 2), 105); // 'i'
        _ = memWrite(_add(tmpBuff, 3), 110); // 'n'
        _ = memWrite(_add(tmpBuff, 4), 103); // 'g'
        _ = memWrite(_add(tmpBuff, 5),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_func()
    //func getKeyword_func() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 102); // 'f'
        _ = memWrite(_add(tmpBuff, 1), 117); // 'u'
        _ = memWrite(_add(tmpBuff, 2), 110); // 'n'
        _ = memWrite(_add(tmpBuff, 3),  99); // 'c'
        _ = memWrite(_add(tmpBuff, 4),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_var()
    //func getKeyword_var() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 118); // 'v'
        _ = memWrite(_add(tmpBuff, 1),  97); // 'a'
        _ = memWrite(_add(tmpBuff, 2), 114); // 'r'
        _ = memWrite(_add(tmpBuff, 3),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_set()
    //func getKeyword_set() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 115); // 's'
        _ = memWrite(_add(tmpBuff, 1), 101); // 'e'
        _ = memWrite(_add(tmpBuff, 2), 116); // 't'
        _ = memWrite(_add(tmpBuff, 3),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_return()
    //func getKeyword_return() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 114); // 'r'
        _ = memWrite(_add(tmpBuff, 1), 101); // 'e'
        _ = memWrite(_add(tmpBuff, 2), 116); // 't'
        _ = memWrite(_add(tmpBuff, 3), 117); // 'u'
        _ = memWrite(_add(tmpBuff, 4), 114); // 'r'
        _ = memWrite(_add(tmpBuff, 5), 110); // 'n'
        _ = memWrite(_add(tmpBuff, 6),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_goto()
    //func getKeyword_goto() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 103); // 'g'
        _ = memWrite(_add(tmpBuff, 1), 111); // 'o'
        _ = memWrite(_add(tmpBuff, 2), 116); // 't'
        _ = memWrite(_add(tmpBuff, 3), 111); // 'o'
        _ = memWrite(_add(tmpBuff, 4),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_if()
    //func getKeyword_if() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 105); // 'i'
        _ = memWrite(_add(tmpBuff, 1), 102); // 'f'
        _ = memWrite(_add(tmpBuff, 2),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_else()
    //func getKeyword_else() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 101); // 'e'
        _ = memWrite(_add(tmpBuff, 1), 108); // 'l'
        _ = memWrite(_add(tmpBuff, 2), 115); // 's'
        _ = memWrite(_add(tmpBuff, 3), 101); // 'e'
        _ = memWrite(_add(tmpBuff, 4),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_underscoreSymbol()
    //func getKeyword_underscoreSymbol() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff, 0), 95); // '_'
        _ = memWrite(_add(tmpBuff, 1),  0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_boolCheck()
    //func getKeyword_boolCheck() -> Int
    {
        var g_offset = (int)default;
        var tmpBuff = (int)default;

        /* set */ tmpBuff = _add(g_offset, 800);

        _ = memWrite(_add(tmpBuff,  0),  95); // '_'
        _ = memWrite(_add(tmpBuff,  1),  95); // '_'
        _ = memWrite(_add(tmpBuff,  2),  98); // 'b'
        _ = memWrite(_add(tmpBuff,  3), 111); // 'o'
        _ = memWrite(_add(tmpBuff,  4), 111); // 'o'
        _ = memWrite(_add(tmpBuff,  5), 108); // 'l'
        _ = memWrite(_add(tmpBuff,  6),  95); // '_'
        _ = memWrite(_add(tmpBuff,  7),  99); // 'c'
        _ = memWrite(_add(tmpBuff,  8), 104); // 'h'
        _ = memWrite(_add(tmpBuff,  9), 101); // 'e'
        _ = memWrite(_add(tmpBuff, 10),  99); // 'c'
        _ = memWrite(_add(tmpBuff, 11), 107); // 'k'
        _ = memWrite(_add(tmpBuff, 12),   0); // '\0'

        return tmpBuff;
    }

    public static int getNextToken()
    //func getNextToken() -> Int
    {
        var srcTxt_ptr = (int)default;
        var tokenBuffer_ptr = (int)default;
        var tokenType = (int)default;
        var tokenLine = (int)default;
        var c = (int)default;
        var pos = (int)default;
        var tmp_c = (int)default;
        var tmp_pos = (int)default;
        var line = (int)default;
        var readCount = (int)default;

        /* set */ srcTxt_ptr = Lexer_getSrcCodeTxt_ptr();
        /* set */ tokenBuffer_ptr = Lexer_getTokenStrBuff_ptr();
        /* set */ pos = Lexer_getPos();
        /* set */ line = Lexer_getLine();

        _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
        _ = Lexer_resetTokenStrBuffPos();

        /* set */ c = memRead8(_add(srcTxt_ptr, pos));
        if (__bool_check(_or(_eq(c, 0), _gte(pos, 65536)))) // '\0' || >= 64 KB
        {
            /* set */ tokenType = 21; // TokenType.EOF_TOKEN
            /* set */ tokenLine = line;
            _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
            goto END;
        }

    // Skip whitespaces
    LOOP_SKIP_WHITESPACE:
        //  ' ' || '\t' || '\r' || '\n'
        if (__bool_check(_or(_eq(c, 32), _or(_eq(c, 9), _or(_eq(c, 13), _eq(c, 10))))))
        {
            if (__bool_check(_eq(c, 10))) // '\n'
            {
                /* set */ line = Lexer_incLine();
            }

            /* set */ pos = Lexer_incPos();
            if (__bool_check(_or(_eq(c, 0), _gte(pos, 65536)))) // '\0' || >= 64 KB
            {
                /* set */ tokenType = 21; // TokenType.EOF_TOKEN
                /* set */ tokenLine = line;
                _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
                goto END;
            }
            /* set */ c = memRead8(_add(srcTxt_ptr, pos));
            goto LOOP_SKIP_WHITESPACE;
        }

        // Skip comments
        if (__bool_check(_eq(c, 47))) // '/'
        {
            /* set */ tmp_pos = _add(pos, 1);
            /* set */ tmp_c = memRead8(_add(srcTxt_ptr, tmp_pos));
            if (__bool_check(_eq(tmp_c, 47))) // '/'
            {
            LOOP_SKIP_COMMENTS:
                if (__bool_check(_neq(c, 10))) // '\n'
                {
                    /* set */ pos = Lexer_incPos();
                    if (__bool_check(_or(_eq(c, 0), _gte(pos, 65536)))) // '\0' || >= 64 KB
                    {
                        /* set */ tokenType = 21; // TokenType.EOF_TOKEN
                        /* set */ tokenLine = line;
                        _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
                        goto END;
                    }
                    /* set */ c = memRead8(_add(srcTxt_ptr, pos));
                    goto LOOP_SKIP_COMMENTS;
                }

                // Restart checks at whitespaces loop, to avoid recursion
                goto LOOP_SKIP_WHITESPACE;
            }
        }

        if (__bool_check(_or(isLetter(c), _eq(c, 95)))) // letter || '_'
        {
            _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'

        LOOP_IDENTIFIER:
            if (__bool_check(_or(isLetter(c), _eq(c, 95)))) // letter || '_'
            {
                /* set */ readCount = Lexer_tokenBufferAppendChar(c);
                if (__bool_check(_gte(c, 256)))
                {
                    /* set */ tokenType = 100; // TokenType.UNKNOWN
                    /* set */ tokenLine = line;
                    _ = memWrite8(_add(tokenBuffer_ptr, readCount), 0); // s_tokenStrBuff[0] = '\0'
                    goto END;
                }
                /* set */ pos = Lexer_incPos();
                if (__bool_check(_or(_eq(c, 0), _gte(pos, 65536)))) // '\0' || >= 64 KB
                {
                    /* set */ tokenType = 21; // TokenType.EOF_TOKEN
                    /* set */ tokenLine = line;
                    _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
                    goto END;
                }
                /* set */ c = memRead8(_add(srcTxt_ptr, pos));
                goto LOOP_IDENTIFIER;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 117), strEquals(tokenBuffer_ptr, getKeyword_using()))))
            {
                /* set */ tokenType = 2; // TokenType.USING
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 102), strEquals(tokenBuffer_ptr, getKeyword_func()))))
            {
                /* set */ tokenType = 3; // TokenType.FUNC
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 118), strEquals(tokenBuffer_ptr, getKeyword_var()))))
            {
                /* set */ tokenType = 4; // TokenType.VAR
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 115), strEquals(tokenBuffer_ptr, getKeyword_set()))))
            {
                /* set */ tokenType = 5; // TokenType.SET
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 105), strEquals(tokenBuffer_ptr, getKeyword_if()))))
            {
                /* set */ tokenType = 6; // TokenType.IF
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 101), strEquals(tokenBuffer_ptr, getKeyword_else()))))
            {
                /* set */ tokenType = 7; // TokenType.ELSE
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 103), strEquals(tokenBuffer_ptr, getKeyword_goto()))))
            {
                /* set */ tokenType = 8; // TokenType.GOTO
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 114), strEquals(tokenBuffer_ptr, getKeyword_return()))))
            {
                /* set */ tokenType = 9; // TokenType.RETURN
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(strEquals(tokenBuffer_ptr, getKeyword_underscoreSymbol())))
            {
                /* set */ tokenType = 18; // TokenType.UNDERSCORE
                /* set */ tokenLine = line;
                goto END;
            }

            if (__bool_check(strEquals(tokenBuffer_ptr, getKeyword_boolCheck())))
            {
                /* set */ tokenType = 20; // TokenType.BOOL_CHECK
                /* set */ tokenLine = line;
                goto END;
            }

            /* set */ tokenType = 0; // TokenType.IDENTIFIER
            /* set */ tokenLine = line;
            goto END;
        }

        if (__bool_check(isDigit(c)))
        {
            _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'

        LOOP_DIGIT:
            if (__bool_check(isDigit(c)))
            {
                /* set */ readCount = Lexer_tokenBufferAppendChar(c);
                if (__bool_check(_gte(c, 256)))
                {
                    /* set */ tokenType = 100; // TokenType.UNKNOWN
                    /* set */ tokenLine = line;
                    _ = memWrite8(_add(tokenBuffer_ptr, readCount), 0); // s_tokenStrBuff[0] = '\0'
                    goto END;
                }
                /* set */ pos = Lexer_incPos();
                if (__bool_check(_or(_eq(c, 0), _gte(pos, 65536)))) // '\0' || >= 64 KB
                {
                    /* set */ tokenType = 21; // TokenType.EOF_TOKEN
                    /* set */ tokenLine = line;
                    _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
                    goto END;
                }
                /* set */ c = memRead8(_add(srcTxt_ptr, pos));
                goto LOOP_DIGIT;
            }

            /* set */ tokenType = 1; // TokenType.INTEGER_LITERAL
            /* set */ tokenLine = line;
            goto END;
        }

        if (__bool_check(_eq(c, 40))) // '('
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 10; // TokenType.L_PAREN
            /* set */ tokenLine = line;
            goto END;
        }
        if (__bool_check(_eq(c, 41))) // ')'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 11; // TokenType.R_PAREN
            /* set */ tokenLine = line;
            goto END;
        }
        if (__bool_check(_eq(c, 123))) // '{'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 12; // TokenType.L_BRACE
            /* set */ tokenLine = line;
            goto END;
        }
        if (__bool_check(_eq(c, 125))) // '}'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 13; // TokenType.R_BRACE
            /* set */ tokenLine = line;
            goto END;
        }
        if (__bool_check(_eq(c, 59))) // ';'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 14; // TokenType.SEMICOLON
            /* set */ tokenLine = line;
            goto END;
        }
        if (__bool_check(_eq(c, 44))) // ','
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 16; // TokenType.COMMA
            /* set */ tokenLine = line;
            goto END;
        }
        if (__bool_check(_eq(c, 58))) // ':'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 15; // TokenType.COLON
            /* set */ tokenLine = line;
            goto END;
        }
        if (__bool_check(_eq(c, 61))) // '='
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 17; // TokenType.EQUALS
            /* set */ tokenLine = line;
            goto END;
        }
        if (__bool_check(_eq(c, 45))) // '-'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            if (__bool_check(_and(_neq(c, 0), _lt(pos, 65536)))) // != '\0' && < 64 KB
            {
                /* set */ c = memRead8(_add(srcTxt_ptr, pos));
                if (__bool_check(_eq(c, 62))) // '>'
                {
                    /* set */ pos = Lexer_incPos();
                    _ = Lexer_tokenBufferAppendChar(c);
                    /* set */ tokenType = 19; // TokenType.TRAILING_RETURN
                    /* set */ tokenLine = line;
                    goto END;
                }
                else
                {
                    /* set */ tokenType = 100; // TokenType.UNKNOWN
                    /* set */ tokenLine = line;
                    goto END;
                }
            }
        }

        /* set */ tokenType = 100; // TokenType.UNKNOWN
        /* set */ tokenLine = line;
        _ = Lexer_tokenBufferAppendChar(c);

    END:
        _ = setCurToken(tokenType, Lexer_getTokenStrBuff_ptr(), tokenLine);
        return tokenType;
    }

    public static int consumeToken()
    //func consumeToken() -> Int
    {
        var g_offset = (int)default;
        var validLine = (int)default;

        /* set */ g_offset = 0;

        /* set */ validLine = getCurTokenLine();
        _ = memWrite(_add(g_offset, 172), validLine); // s_lastValidLine

        _ = getNextToken();

        return 0;
    }

    public static int skipUTF8BOMMark()
    //func skipUTF8BOMMark() -> Int
    {
        var g_offset = (int)default;
        var pos = (int)default;
        var srcTxt_ptr = (int)default;
        var c = (int)default;

        /* set */ g_offset = 0;
        /* set */ pos = 0;
        /* set */ srcTxt_ptr = Lexer_getSrcCodeTxt_ptr();

        /* set */ c = memRead8(_add(srcTxt_ptr, pos));

        if (__bool_check(_eq(c, 239))) // 0xEF
        {
            // Assume all 3 mark bytes are in place and skip them
            _ = memWrite(_add(g_offset, 164), _add(pos, 3)); // s_pos
        }

        return 0;
    }

    public static int Lexer_tokenBufferAppendChar(int c)
    //func Lexer_tokenBufferAppendChar(c: Int) -> Int
    {
        var g_offset = (int)default;
        var pos = (int)default;
        var strBuffer = (int)default;

        /* set */ g_offset = 0;
        /* set */ strBuffer = Lexer_getTokenStrBuff_ptr(); // s_tokenStrBuff
        /* set */ pos = Lexer_getTokenStrBuffPos(); // s_tokenStrBuff_pos

        _ = memWrite8(_add(strBuffer, pos), c); // s_tokenStrBuff[pos] = c
        /* set */ pos = _add(pos, 1); // pos++

        if (__bool_check(_gte(pos, 256))) // max len 256 bytes
        {
            _ = printError($"The token strBuffer limit is {256} bytes");
            goto END;
        }

        // Insert termination
        _ = memWrite8(_add(strBuffer, pos), 0); // s_tokenStrBuff[pos] = '\0'

        _ = memWrite(_add(g_offset, 124), pos); // update s_tokenStrBuff_pos

    END:
        // return cur len
        return pos;
    }

    public static int IL_Emitter_initEmitter()
    //func IL_Emitter_initEmitter() -> Int
    {
        var result = (int)default;

        /* set */ result = IL_Emitter_appendTxt(IL_Emitter_getBaseILBeginTxt_ptr());

        return result;
    }

    public static int IL_Emitter_finishEmitter()
    //func IL_Emitter_finishEmitter() -> Int
    {
        var result = (int)default;

        /* set */ result = IL_Emitter_appendTxt(IL_Emitter_getBaseILEndTxt_ptr());

        return result;
    }

    public static int IL_Emitter_getStrBuffer_ptr()
    //func IL_Emitter_getStrBuffer_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 64)); // s_IL_Emitter_strBuffer
    }

    public static int IL_Emitter_getBaseILBeginTxt_ptr()
    //func IL_Emitter_getBaseILBeginTxt_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 76)); // s_IL_Emitter_baseILBegin_txt
    }

    public static int IL_Emitter_getBaseILEndTxt_ptr()
    //func IL_Emitter_getBaseILEndTxt_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 80)); // s_IL_Emitter_baseILEnd_txt
    }

    public static int IL_Emitter_getRuntimeJsonTxt_ptr()
    //func IL_Emitter_getRuntimeJsonTxt_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 84)); // s_IL_Emitter_runtimeJson_txt
    }

    public static int IL_Emitter_appendChar(int c)
    //func IL_Emitter_appendChar(c: Int) -> Int
    {
        var g_offset = (int)default;
        var pos = (int)default;
        var strBuffer = (int)default;
        var result = (int)default;

        /* set */ g_offset = 0;

        /* set */ result = 1;
        /* set */ strBuffer = IL_Emitter_getStrBuffer_ptr(); // s_IL_Emitter_strBuffer
        /* set */ pos = memRead(_add(g_offset, 68)); // s_IL_Emitter_strBuffer_pos

        _ = memWrite8(_add(strBuffer, pos), c); // s_IL_Emitter_strBuffer[pos] = c
        /* set */ pos = _add(pos, 1); // pos++

        if (__bool_check(_gte(pos, 65536))) // max len 64 KB
        {
            /* set */ result = 0;
            _ = printError($"The IL_Emitter strBuffer limit is {65536} bytes");
            goto END;
        }

        // Insert termination
        _ = memWrite8(_add(strBuffer, pos), 0); // s_IL_Emitter_strBuffer[pos] = '\0'

        _ = memWrite(_add(g_offset, 68), pos); // update s_IL_Emitter_strBuffer_pos

    END:
        return result;
    }

    public static int IL_Emitter_appendTxt(int txt_ptr)
    //func IL_Emitter_appendTxt(txt_ptr: Int) -> Int
    {
        var pos = (int)default;
        var c = (int)default;
        var result = (int)default;

        /* set */ result = 1;

    LOOP:
        /* set */ c = memRead8(_add(pos, txt_ptr));
        /* set */ result = IL_Emitter_appendChar(c);

        if (__bool_check(_eq(result, 0)))
        {
            goto END;
        }

        /* set */ pos = _add(pos, 1);

        if (__bool_check(_eq(c, 0))) // '\0'
        {
            goto END;
        }

        goto LOOP;

    END:
        return result;
    }

    public static int parse()
    //func parse() -> Int
    {
        var result = (int)default;

        /* set */ result = IL_Emitter_initEmitter();
        if (__bool_check(_eq(result, 0)))
        {
            _ = printError($"Failed initEmitter");
            goto END;
        }

        /* set */ result = parseCompilationUnit();
        if (__bool_check(_eq(result, 0)))
        {
            _ = printError("Failed to parse program");
            goto END;
        }
        else
        {
            _ = printMessage("Program parsed successfully");
            _ = IL_Emitter_finishEmitter();
        }

    END:
        return result;
    }

    public static int parseCompilationUnit()
    //func parseCompilationUnit() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        _ = skipUTF8BOMMark(); // Skips UTF8 BOM mark, if any

        _ = consumeToken(); // Initialize m_curToken

    #if false
        while (getNextToken() < 21)
        {
            var str = System.Text.Encoding.UTF8.GetString(Program.s_RAM.AsSpan(200, 256));
            str = str.Substring(0, str.IndexOf('\0'));
            System.Console.WriteLine($"{getCurTokenType()} {str}");
        }
    #endif

        return result;
    }


    public static int main()
    //func main() -> Int
    {
        _ = initGlobals();
        _ = initParser();

        _ = parse();

        return 0;
    }

    public static int printError(string message)
    //func printError(message_ptr: Int) -> Int
    {
        System.Console.WriteLine(message);
        return 0;
    }

    public static int printMessage(string message)
    //func printMessage(message_ptr: Int) -> Int
    {
        System.Console.WriteLine(message);
        return 0;
    }
}
