internal partial class ILGenerator
{
    // === Offset allocation for global variables
    //
    // g_offset = 0
    //
    // & s_ramBinLength_MB                 := g_offset + 64
    // & s_isDebugConfig                   := g_offset + 68
    // & s_tmpBuff_ptr                     := g_offset + 72
    //
    // & s_curToken_type                   := g_offset + 120
    // & s_curToken_value_ptr              := g_offset + 124
    // & s_curToken_line                   := g_offset + 128
    //
    // & s_tokenStrBuff_ptr                := g_offset + 140
    // & s_tokenStrBuff_pos                := g_offset + 144
    //
    // & s_srcCodeTxt_ptr                  := g_offset + 160
    // & s_pos                             := g_offset + 164
    // & s_line                            := g_offset + 168
    // & s_lastValidLine                   := g_offset + 172
    // & s_curScopeDepth                   := g_offset + 176
    // & s_inFunctionScope                 := g_offset + 180
    // & s_paramCount                      := g_offset + 184
    // & s_ifLabelCounter                  := g_offset + 188
    //
    // & s_symbol_sizeof                   := g_offset + 192
    // & s_symbol_field_size               := g_offset + 196
    //
    // & s_functionsTbl_ptr                := g_offset + 200
    // & s_functionsTbl_count              := g_offset + 204
    // & s_functionsTbl_names_ptr          := g_offset + 208
    // & s_functionsTbl_names_offset       := g_offset + 212
    // & s_functionScopeTbl_ptr            := g_offset + 216
    // & s_functionScopeTbl_count          := g_offset + 220
    // & s_functionScopeTbl_names_ptr      := g_offset + 224
    // & s_functionScopeTbl_names_offset   := g_offset + 228
    // & s_unresolvedGotoTbl_ptr           := g_offset + 232
    // & s_unresolvedGotoTbl_count         := g_offset + 236
    // & s_unresolvedGotoTbl_names_ptr     := g_offset + 240
    // & s_unresolvedGotoTbl_names_offset  := g_offset + 244
    // & s_typeNamesTbl_ptr                := g_offset + 248
    // & s_typeNamesTbl_count              := g_offset + 252
    // & s_typeNamesTbl_names_ptr          := g_offset + 256
    // & s_typeNamesTbl_names_offset       := g_offset + 260
    //
    // & s_IL_Emitter_strBuffer_ptr        := g_offset + 280
    // & s_IL_Emitter_strBuffer_pos        := g_offset + 284
    // & s_IL_Emitter_baseILBeginTxt_ptr   := g_offset + 288
    // & s_IL_Emitter_baseILEndTxt_ptr     := g_offset + 292
    // & s_IL_Emitter_RuntimeJsonTxt_ptr   := g_offset + 296
    //
    //
    // === Hardcoded pointers
    //
    // s_curToken_value_ptr                = g_offset + 320    (max len: 128 bytes)
    // s_tokenStrBuff_ptr                  = g_offset + 448    (max len: 128 bytes)
    // s_tmpBuff_ptr                       = g_offset + 576    (max len: 128 bytes)
    // s_tmpBuff2_ptr                      = g_offset + 704    (max len: 128 bytes)
    // s_tmpBuff3_ptr                      = g_offset + 832    (max len: 128 bytes)
    //
    // s_functionsTbl_ptr                  = g_offset + 2048   (max len: 16 KB)  ->  512 elements
    // s_functionScopeTbl_ptr              = g_offset + 18432  (max len: 64 KB)  -> 2048 elements
    // s_unresolvedGotoTbl_ptr             = g_offset + 83968  (max len: 16 KB)  ->  512 elements
    // s_typeNamesTbl_ptr                  = g_offset + 100352 (max len:  1 KB)  ->   32 elements
    //
    // s_functionsTbl_names_ptr            = g_offset + 102400 (max len:  64 KB) ->  512 elements
    // s_functionScopeTbl_names_ptr        = g_offset + 167936 (max len: 256 KB) -> 2048 elements
    // s_unresolvedGotoTbl_names_ptr       = g_offset + 430080 (max len:  64 KB) ->  512 elements
    // s_typeNamesTbl_names_ptr            = g_offset + 495616 (max len:   4 KB) ->   32 elements
    //
    // s_IL_Emitter_baseILBeginTxt_ptr     = g_offset + 499712 (max len: 10 KB)
    // s_IL_Emitter_baseILEndTxt_ptr       = g_offset + 509952 (max len:  1 KB)
    // s_IL_Emitter_RuntimeJsonTxt_ptr     = g_offset + 510976 (max len:  1 KB)
    //
    // s_srcCodeTxt_ptr                    = g_offset + 524288 (max len: 128 KB) [input FFLang source]
    //
    // s_IL_Emitter_strBuffer_ptr          = g_offset + 655360 (max len: 256 KB) [output IL source]
    //


    // ======== FFLang program

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

    public static int intToStr(int v)
    //func intToStr(v: Int) -> Int
    {
        var tmpBuff = (int)default;
        var pos = (int)default;
        var count = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        //TODO: millions is enough for the context of this program address space

        /* set */ count = 0;
    LOOP_M:
        if (__bool_check(_gte(v, 1000000)))
        {
            /* set */ count = _add(count, 1);
            /* set */ v = _sub(v, 1000000);
            goto LOOP_M;
        }

        if (__bool_check(_or(_gt(count, 0), _gt(pos, 0))))
        {
            _ = memWrite8(_add(tmpBuff, pos), _add(48, count)); // digit x______
            /* set */ pos = _add(pos, 1); // pos++
        }

        /* set */ count = 0;
    LOOP_KKK:
        if (__bool_check(_gte(v, 100000)))
        {
            /* set */ count = _add(count, 1);
            /* set */ v = _sub(v, 100000);
            goto LOOP_KKK;
        }

        if (__bool_check(_or(_gt(count, 0), _gt(pos, 0))))
        {
            _ = memWrite8(_add(tmpBuff, pos), _add(48, count)); // digit _x_____
            /* set */ pos = _add(pos, 1); // pos++
        }

        /* set */ count = 0;
    LOOP_KK:
        if (__bool_check(_gte(v, 10000)))
        {
            /* set */ count = _add(count, 1);
            /* set */ v = _sub(v, 10000);
            goto LOOP_KK;
        }

        if (__bool_check(_or(_gt(count, 0), _gt(pos, 0))))
        {
            _ = memWrite8(_add(tmpBuff, pos), _add(48, count)); // digit __x____
            /* set */ pos = _add(pos, 1); // pos++
        }

        /* set */ count = 0;
    LOOP_K:
        if (__bool_check(_gte(v, 1000)))
        {
            /* set */ count = _add(count, 1);
            /* set */ v = _sub(v, 1000);
            goto LOOP_K;
        }

        if (__bool_check(_or(_gt(count, 0), _gt(pos, 0))))
        {
            _ = memWrite8(_add(tmpBuff, pos), _add(48, count)); // digit ___x___
            /* set */ pos = _add(pos, 1); // pos++
        }

        /* set */ count = 0;
    LOOP_H:
        if (__bool_check(_gte(v, 100)))
        {
            /* set */ count = _add(count, 1);
            /* set */ v = _sub(v, 100);
            goto LOOP_H;
        }

        if (__bool_check(_or(_gt(count, 0), _gt(pos, 0))))
        {
            _ = memWrite8(_add(tmpBuff, pos), _add(48, count)); // digit ____x__
            /* set */ pos = _add(pos, 1); // pos++
        }

        /* set */ count = 0;
    LOOP_D:
        if (__bool_check(_gte(v, 10)))
        {
            /* set */ count = _add(count, 1);
            /* set */ v = _sub(v, 10);
            goto LOOP_D;
        }

        if (__bool_check(_or(_gt(count, 0), _gt(pos, 0))))
        {
            _ = memWrite8(_add(tmpBuff, pos), _add(48, count)); // digit _____x_
            /* set */ pos = _add(pos, 1); // pos++
        }

        _ = memWrite8(_add(tmpBuff, pos), _add(48, v)); // digit ______x
        _ = memWrite8(_add(tmpBuff, _add(pos, 1)), 0); // '\0'

        return tmpBuff;
    }

    #endregion Helpers

    #region Tmp Buffer

    public static int getTmpBuffer_ptr()
    //func getTmpBuffer_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 72)); // s_tmpBuff_ptr
    }

    #endregion Tmp Buffer

    #region Token struct

    // ===== Token - value-type
    //
    // -- enum TokenType
    //   IDENTIFIER         = 0
    //   INTEGER_LITERAL    = 1
    //
    //   USING              = 2
    //   FUNC               = 3
    //   VAR                = 4
    //   SET                = 5
    //   IF                 = 6
    //   ELSE               = 7
    //   GOTO               = 8
    //   RETURN             = 9
    //
    //   L_PAREN            = 10
    //   R_PAREN            = 11
    //   L_BRACE            = 12
    //   R_BRACE            = 13
    //   SEMICOLON          = 14
    //   COLON              = 15
    //   COMMA              = 16
    //   EQUALS             = 17
    //   UNDERSCORE         = 18
    //   TRAILING_RETURN    = 19
    //
    //   BOOL_CHECK         = 20
    //
    //   EOF_TOKEN          = 21
    //
    //   UNKNOWN            = 100
    // -------
    //
    // Token struct
    //  - type: TokenType
    //  - value: str_ptr
    //  - line: Int
    //
    //

    public static int getCurTokenType()
    //func getCurTokenType() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 120)); // s_curToken_type
    }

    public static int getCurTokenValue_ptr()
    //func getCurTokenValue_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 124)); // s_curToken_value_ptr
    }

    public static int getCurTokenLine()
    //func getCurTokenLine() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 128)); // s_curToken_line
    }

    public static int setCurToken(int tokenType, int valueStr_ptr, int line)
    //func setCurToken(tokenType: Int, valueStr_ptr: Int, line: Int) -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        _ = memWrite(_add(g_offset, 120), tokenType); // s_curToken_type = tokenType
        _ = strCpy(valueStr_ptr, getCurTokenValue_ptr()); // s_curToken_value_ptr = valueStr_ptr
        _ = memWrite(_add(g_offset, 128), line); // s_curToken_line = line

#if false
        _ = printMessage($"{getCurTokenType()} {Program.ReadString(getCurTokenValue_ptr(), 128)} line: {getCurTokenLine()}");
#endif

        return 0;
    }

    public static int matchTokenType(int tokenType)
    //func matchTokenType(tokenType: Int) -> Int
    {
        return _eq(tokenType, getCurTokenType());
    }

    public static int notMatchTokenType(int tokenType)
    //func notMatchTokenType(tokenType: Int) -> Int
    {
        return _neq(tokenType, getCurTokenType());
    }

    #endregion Token struct

    #region Lexer

    public static int Lexer_getTokenStrBuff_ptr()
    //func Lexer_getTokenStrBuff_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 140)); // s_tokenStrBuff_ptr
    }

    public static int Lexer_getTokenStrBuff_pos()
    //func Lexer_getTokenStrBuff_pos() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 144)); // s_tokenStrBuff_pos
    }

    public static int Lexer_resetTokenStrBuff_pos()
    //func Lexer_resetTokenStrBuff_pos() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memWrite(_add(g_offset, 144), 0); // s_tokenStrBuff_pos
    }

    public static int Lexer_tokenBufferAppendChar(int c)
    //func Lexer_tokenBufferAppendChar(c: Int) -> Int
    {
        var g_offset = (int)default;
        var pos = (int)default;
        var strBuffer = (int)default;

        /* set */ g_offset = 0;
        /* set */ strBuffer = Lexer_getTokenStrBuff_ptr(); // s_tokenStrBuff_ptr
        /* set */ pos = Lexer_getTokenStrBuff_pos(); // s_tokenStrBuff_pos

        if (__bool_check(_gte(pos, 128))) // max len 128 bytes
        {
            _ = printError($"The token strBuffer limit is {128} bytes");
            goto END;
        }

        _ = memWrite8(_add(strBuffer, pos), c); // s_tokenStrBuff[pos] = c
        /* set */ pos = _add(pos, 1); // pos++

        // Insert termination
        _ = memWrite8(_add(strBuffer, pos), 0); // s_tokenStrBuff[pos] = '\0'

        _ = memWrite(_add(g_offset, 144), pos); // update s_tokenStrBuff_pos

    END:
        // return cur len
        return pos;
    }

    public static int Lexer_getSrcCodeTxt_ptr()
    //func Lexer_getSrcCodeTxt_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 160)); // s_srcCodeTxt_ptr
    }

    public static int Lexer_getPos()
    //func Lexer_getPos() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 164)); // s_pos
    }

    public static int Lexer_incPos()
    //func Lexer_incPos() -> Int
    {
        var g_offset = (int)default;
        var pos = (int)default;

        /* set */ g_offset = 0;

        /* set */ pos = _add(memRead(_add(g_offset, 164)), 1); // s_pos + 1
        _ = memWrite(_add(g_offset, 164), pos); // s_pos = s_pos + 1

        // returns incremented pos
        return pos;
    }

    public static int Lexer_getLine()
    //func Lexer_getLine() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 168)); // s_line
    }

    public static int Lexer_incLine()
    //func Lexer_incLine() -> Int
    {
        var g_offset = (int)default;
        var line = (int)default;

        /* set */ g_offset = 0;

        /* set */ line = _add(memRead(_add(g_offset, 168)), 1); // s_line + 1
        _ = memWrite(_add(g_offset, 168), line); // s_line = s_line + 1

        // returns incremented line
        return line;
    }

    public static int Lexer_getLastValidLine()
    //func Lexer_getLastValidLine() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 172)); // s_lastValidLine
    }

    #endregion Lexer

    #region Parser fields

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
        _ = memWrite(_add(g_offset, 176), depth); // s_curScopeDepth = s_curScopeDepth + 1

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
        _ = memWrite(_add(g_offset, 176), depth); // s_curScopeDepth = s_curScopeDepth - 1

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

        _ = memWrite(_add(g_offset, 184), count); // s_paramCount = count
        return 0;
    }

    public static int incParamCount()
    //func incParamCount() -> Int
    {
        var g_offset = (int)default;
        var count = (int)default;

        /* set */ g_offset = 0;

        /* set */ count = _add(memRead(_add(g_offset, 184)), 1); // s_paramCount + 1
        _ = memWrite(_add(g_offset, 184), count); // s_paramCount = s_paramCount + 1

        // returns incremented paramCount
        return count;
    }

    public static int getIfLabelCounter()
    //func getIfLabelCounter() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 188)); // s_ifLabelCounter
    }

    public static int resetIfLabelCounter()
    //func resetIfLabelCounter() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        _ = memWrite(_add(g_offset, 188), 0); // s_ifLabelCounter
        return 0;
    }

    public static int incIfLabelCounter()
    //func incIfLabelCounter() -> Int
    {
        var g_offset = (int)default;
        var count = (int)default;

        /* set */ g_offset = 0;

        /* set */ count = _add(memRead(_add(g_offset, 188)), 1); // s_ifLabelCounter + 1
        _ = memWrite(_add(g_offset, 188), count); // s_ifLabelCounter = s_ifLabelCounter + 1

        // returns incremented ifLabelCounter
        return count;
    }

    #endregion Parser fields

    #region Const Strings

    public static int getTypeName_Int()
    //func getTypeName_Int() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0),  73); // 'I'
        _ = memWrite8(_add(tmpBuff, 1), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 2), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 3),   0); // '\0'

        return tmpBuff;
    }

    public static int getMacroName_int_max()
    //func getMacroName_int_max() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  1), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  2), 73); // 'I'
        _ = memWrite8(_add(tmpBuff,  3), 78); // 'N'
        _ = memWrite8(_add(tmpBuff,  4), 84); // 'T'
        _ = memWrite8(_add(tmpBuff,  5), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  6), 77); // 'M'
        _ = memWrite8(_add(tmpBuff,  7), 65); // 'A'
        _ = memWrite8(_add(tmpBuff,  8), 88); // 'X'
        _ = memWrite8(_add(tmpBuff,  9), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 10), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 11),  0); // '\0'

        return tmpBuff;
    }

    public static int getMacroName_int_width_bits()
    //func getMacroName_int_width_bits() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  1), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  2), 73); // 'I'
        _ = memWrite8(_add(tmpBuff,  3), 78); // 'N'
        _ = memWrite8(_add(tmpBuff,  4), 84); // 'T'
        _ = memWrite8(_add(tmpBuff,  5), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  6), 87); // 'W'
        _ = memWrite8(_add(tmpBuff,  7), 73); // 'I'
        _ = memWrite8(_add(tmpBuff,  8), 68); // 'D'
        _ = memWrite8(_add(tmpBuff,  9), 84); // 'T'
        _ = memWrite8(_add(tmpBuff, 10), 72); // 'H'
        _ = memWrite8(_add(tmpBuff, 11), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 12), 66); // 'B'
        _ = memWrite8(_add(tmpBuff, 13), 73); // 'I'
        _ = memWrite8(_add(tmpBuff, 14), 84); // 'T'
        _ = memWrite8(_add(tmpBuff, 15), 83); // 'S'
        _ = memWrite8(_add(tmpBuff, 16), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 17), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 18),  0); // '\0'

        return tmpBuff;
    }

    public static int getMacroName_int_width_bytes()
    //func getMacroName_int_width_bytes() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  1), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  2), 73); // 'I'
        _ = memWrite8(_add(tmpBuff,  3), 78); // 'N'
        _ = memWrite8(_add(tmpBuff,  4), 84); // 'T'
        _ = memWrite8(_add(tmpBuff,  5), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  6), 87); // 'W'
        _ = memWrite8(_add(tmpBuff,  7), 73); // 'I'
        _ = memWrite8(_add(tmpBuff,  8), 68); // 'D'
        _ = memWrite8(_add(tmpBuff,  9), 84); // 'T'
        _ = memWrite8(_add(tmpBuff, 10), 72); // 'H'
        _ = memWrite8(_add(tmpBuff, 11), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 12), 66); // 'B'
        _ = memWrite8(_add(tmpBuff, 13), 89); // 'Y'
        _ = memWrite8(_add(tmpBuff, 14), 84); // 'T'
        _ = memWrite8(_add(tmpBuff, 15), 69); // 'E'
        _ = memWrite8(_add(tmpBuff, 16), 83); // 'S'
        _ = memWrite8(_add(tmpBuff, 17), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 18), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 19),  0); // '\0'

        return tmpBuff;
    }

    public static int getMacroName_int_sign_bit_mask()
    //func getMacroName_int_sign_bit_mask() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  1), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  2), 73); // 'I'
        _ = memWrite8(_add(tmpBuff,  3), 78); // 'N'
        _ = memWrite8(_add(tmpBuff,  4), 84); // 'T'
        _ = memWrite8(_add(tmpBuff,  5), 95); // '_'
        _ = memWrite8(_add(tmpBuff,  6), 83); // 'S'
        _ = memWrite8(_add(tmpBuff,  7), 73); // 'I'
        _ = memWrite8(_add(tmpBuff,  8), 71); // 'G'
        _ = memWrite8(_add(tmpBuff,  9), 78); // 'N'
        _ = memWrite8(_add(tmpBuff, 10), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 11), 66); // 'B'
        _ = memWrite8(_add(tmpBuff, 12), 73); // 'I'
        _ = memWrite8(_add(tmpBuff, 13), 84); // 'T'
        _ = memWrite8(_add(tmpBuff, 14), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 15), 77); // 'M'
        _ = memWrite8(_add(tmpBuff, 16), 65); // 'A'
        _ = memWrite8(_add(tmpBuff, 17), 83); // 'S'
        _ = memWrite8(_add(tmpBuff, 18), 75); // 'K'
        _ = memWrite8(_add(tmpBuff, 19), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 20), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 21),  0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_using()
    //func getKeyword_using() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 117); // 'u'
        _ = memWrite8(_add(tmpBuff, 1), 115); // 's'
        _ = memWrite8(_add(tmpBuff, 2), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 3), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 4), 103); // 'g'
        _ = memWrite8(_add(tmpBuff, 5),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_func()
    //func getKeyword_func() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 102); // 'f'
        _ = memWrite8(_add(tmpBuff, 1), 117); // 'u'
        _ = memWrite8(_add(tmpBuff, 2), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 3),  99); // 'c'
        _ = memWrite8(_add(tmpBuff, 4),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_var()
    //func getKeyword_var() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 118); // 'v'
        _ = memWrite8(_add(tmpBuff, 1),  97); // 'a'
        _ = memWrite8(_add(tmpBuff, 2), 114); // 'r'
        _ = memWrite8(_add(tmpBuff, 3),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_set()
    //func getKeyword_set() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 115); // 's'
        _ = memWrite8(_add(tmpBuff, 1), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 2), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 3),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_return()
    //func getKeyword_return() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 114); // 'r'
        _ = memWrite8(_add(tmpBuff, 1), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 2), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 3), 117); // 'u'
        _ = memWrite8(_add(tmpBuff, 4), 114); // 'r'
        _ = memWrite8(_add(tmpBuff, 5), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 6),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_goto()
    //func getKeyword_goto() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 103); // 'g'
        _ = memWrite8(_add(tmpBuff, 1), 111); // 'o'
        _ = memWrite8(_add(tmpBuff, 2), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 3), 111); // 'o'
        _ = memWrite8(_add(tmpBuff, 4),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_if()
    //func getKeyword_if() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 1), 102); // 'f'
        _ = memWrite8(_add(tmpBuff, 2),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_else()
    //func getKeyword_else() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 1), 108); // 'l'
        _ = memWrite8(_add(tmpBuff, 2), 115); // 's'
        _ = memWrite8(_add(tmpBuff, 3), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 4),   0); // '\0'

        return tmpBuff;
    }

    public static int getKeyword_underscoreSymbol()
    //func getKeyword_underscoreSymbol() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0), 95); // '_'
        _ = memWrite8(_add(tmpBuff, 1),  0); // '\0'

        return tmpBuff;
    }

    public static int getBuiltin_boolCheck()
    //func getBuiltin_boolCheck() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  1),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  2),  98); // 'b'
        _ = memWrite8(_add(tmpBuff,  3), 111); // 'o'
        _ = memWrite8(_add(tmpBuff,  4), 111); // 'o'
        _ = memWrite8(_add(tmpBuff,  5), 108); // 'l'
        _ = memWrite8(_add(tmpBuff,  6),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  7),  99); // 'c'
        _ = memWrite8(_add(tmpBuff,  8), 104); // 'h'
        _ = memWrite8(_add(tmpBuff,  9), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 10),  99); // 'c'
        _ = memWrite8(_add(tmpBuff, 11), 107); // 'k'
        _ = memWrite8(_add(tmpBuff, 12),   0); // '\0'

        return tmpBuff;
    }

    public static int getBuiltin_dbg_int()
    //func getBuiltin_dbg_int() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff, 0),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 1),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 2), 100); // 'd'
        _ = memWrite8(_add(tmpBuff, 3),  98); // 'b'
        _ = memWrite8(_add(tmpBuff, 4), 103); // 'g'
        _ = memWrite8(_add(tmpBuff, 5),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 6), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 7), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 8), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 9),   0); // '\0'

        return tmpBuff;
    }

    public static int getBuiltin_add()
    //func getBuiltin_add() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  1),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  2),  98); // 'b'
        _ = memWrite8(_add(tmpBuff,  3), 117); // 'u'
        _ = memWrite8(_add(tmpBuff,  4), 105); // 'i'
        _ = memWrite8(_add(tmpBuff,  5), 108); // 'l'
        _ = memWrite8(_add(tmpBuff,  6), 116); // 't'
        _ = memWrite8(_add(tmpBuff,  7), 105); // 'i'
        _ = memWrite8(_add(tmpBuff,  8), 110); // 'n'
        _ = memWrite8(_add(tmpBuff,  9),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 10), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 11), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 12), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 13),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 14),  97); // 'a'
        _ = memWrite8(_add(tmpBuff, 15), 100); // 'd'
        _ = memWrite8(_add(tmpBuff, 16), 100); // 'd'
        _ = memWrite8(_add(tmpBuff, 17),   0); // '\0'

        return tmpBuff;
    }

    public static int getBuiltin_nand()
    //func getBuiltin_nand() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  1),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  2),  98); // 'b'
        _ = memWrite8(_add(tmpBuff,  3), 117); // 'u'
        _ = memWrite8(_add(tmpBuff,  4), 105); // 'i'
        _ = memWrite8(_add(tmpBuff,  5), 108); // 'l'
        _ = memWrite8(_add(tmpBuff,  6), 116); // 't'
        _ = memWrite8(_add(tmpBuff,  7), 105); // 'i'
        _ = memWrite8(_add(tmpBuff,  8), 110); // 'n'
        _ = memWrite8(_add(tmpBuff,  9),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 10), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 11), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 12), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 13),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 14), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 15),  97); // 'a'
        _ = memWrite8(_add(tmpBuff, 16), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 17), 100); // 'd'
        _ = memWrite8(_add(tmpBuff, 18),   0); // '\0'

        return tmpBuff;
    }

    public static int getBuiltin_mem_read()
    //func getBuiltin_mem_read() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  1),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  2),  98); // 'b'
        _ = memWrite8(_add(tmpBuff,  3), 117); // 'u'
        _ = memWrite8(_add(tmpBuff,  4), 105); // 'i'
        _ = memWrite8(_add(tmpBuff,  5), 108); // 'l'
        _ = memWrite8(_add(tmpBuff,  6), 116); // 't'
        _ = memWrite8(_add(tmpBuff,  7), 105); // 'i'
        _ = memWrite8(_add(tmpBuff,  8), 110); // 'n'
        _ = memWrite8(_add(tmpBuff,  9),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 10), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 11), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 12), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 13),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 14), 109); // 'm'
        _ = memWrite8(_add(tmpBuff, 15), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 16), 109); // 'm'
        _ = memWrite8(_add(tmpBuff, 17),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 18), 114); // 'r'
        _ = memWrite8(_add(tmpBuff, 19), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 20),  97); // 'a'
        _ = memWrite8(_add(tmpBuff, 21), 100); // 'd'
        _ = memWrite8(_add(tmpBuff, 22),   0); // '\0'

        return tmpBuff;
    }

    public static int getBuiltin_mem_write()
    //func getBuiltin_mem_write() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  1),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  2),  98); // 'b'
        _ = memWrite8(_add(tmpBuff,  3), 117); // 'u'
        _ = memWrite8(_add(tmpBuff,  4), 105); // 'i'
        _ = memWrite8(_add(tmpBuff,  5), 108); // 'l'
        _ = memWrite8(_add(tmpBuff,  6), 116); // 't'
        _ = memWrite8(_add(tmpBuff,  7), 105); // 'i'
        _ = memWrite8(_add(tmpBuff,  8), 110); // 'n'
        _ = memWrite8(_add(tmpBuff,  9),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 10), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 11), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 12), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 13),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 14), 109); // 'm'
        _ = memWrite8(_add(tmpBuff, 15), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 16), 109); // 'm'
        _ = memWrite8(_add(tmpBuff, 17),  95); // '_'
        _ = memWrite8(_add(tmpBuff, 18), 119); // 'w'
        _ = memWrite8(_add(tmpBuff, 19), 114); // 'r'
        _ = memWrite8(_add(tmpBuff, 20), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 21), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 22), 101); // 'e'
        _ = memWrite8(_add(tmpBuff, 23),   0); // '\0'

        return tmpBuff;
    }

    public static int replaceMacro_int_width_bytes(int dest_ptr)
    //func replaceMacro_int_width_bytes(dest_ptr: Int) -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0), 52); // '4'
        _ = memWrite8(_add(tmpBuff,  1),  0); // '\0'

        _ = strCpy(tmpBuff, dest_ptr);

        return 0;
    }

    public static int replaceMacro_int_width_bits(int dest_ptr)
    //func replaceMacro_int_width_bits(dest_ptr: Int) -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0), 51); // '3'
        _ = memWrite8(_add(tmpBuff,  1), 50); // '2'
        _ = memWrite8(_add(tmpBuff,  2),  0); // '\0'

        _ = strCpy(tmpBuff, dest_ptr);

        return 0;
    }

    public static int replaceMacro_int_max(int dest_ptr)
    //func replaceMacro_int_max(dest_ptr: Int) -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0), 50); // '2'
        _ = memWrite8(_add(tmpBuff,  1), 49); // '1'
        _ = memWrite8(_add(tmpBuff,  2), 52); // '4'
        _ = memWrite8(_add(tmpBuff,  3), 55); // '7'
        _ = memWrite8(_add(tmpBuff,  4), 52); // '4'
        _ = memWrite8(_add(tmpBuff,  5), 56); // '8'
        _ = memWrite8(_add(tmpBuff,  6), 51); // '3'
        _ = memWrite8(_add(tmpBuff,  7), 54); // '6'
        _ = memWrite8(_add(tmpBuff,  8), 52); // '4'
        _ = memWrite8(_add(tmpBuff,  9), 55); // '7'
        _ = memWrite8(_add(tmpBuff, 10),  0); // '\0'

        _ = strCpy(tmpBuff, dest_ptr);

        return 0;
    }

    public static int replaceMacro_int_sign_bit_mask(int dest_ptr)
    //func replaceMacro_int_sign_bit_mask(dest_ptr: Int) -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();

        _ = memWrite8(_add(tmpBuff,  0), 45); // '-'
        _ = memWrite8(_add(tmpBuff,  1), 50); // '2'
        _ = memWrite8(_add(tmpBuff,  2), 49); // '1'
        _ = memWrite8(_add(tmpBuff,  3), 52); // '4'
        _ = memWrite8(_add(tmpBuff,  4), 55); // '7'
        _ = memWrite8(_add(tmpBuff,  5), 52); // '4'
        _ = memWrite8(_add(tmpBuff,  6), 56); // '8'
        _ = memWrite8(_add(tmpBuff,  7), 51); // '3'
        _ = memWrite8(_add(tmpBuff,  8), 54); // '6'
        _ = memWrite8(_add(tmpBuff,  9), 52); // '4'
        _ = memWrite8(_add(tmpBuff, 10), 56); // '8'
        _ = memWrite8(_add(tmpBuff, 11),  0); // '\0'

        _ = strCpy(tmpBuff, dest_ptr);

        return 0;
    }

    #endregion Const Strings

    #region Symbols

    // ===== Symbol - value-type
    //
    // -- enum SymbolType
    //   FUNCTION           = 0
    //   ARGUMENT_VARIABLE  = 1
    //   LOCAL_VARIABLE     = 2
    //   LABEL              = 3
    //   TYPE_NAME          = 4
    //
    //   UNDEFINED          = 100
    // -------
    //
    // Symbol struct
    //  - identifierStr_ptr: str_ptr
    //  - symbolType: SymbolType
    //  - definitionLine: Int
    //  - scopeDepth: Int
    //  - paramCount: Int
    //  - definitionFound: Int (1 or 0)
    //  - wasReferenced: Int (1 or 0)
    //  - address: Int (-unused-)
    //

    public static int symbol_sizeOf()
    //func symbol_sizeOf() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 192)); // s_symbol_sizeof
    }

    public static int symbol_fieldSize()
    //func symbol_fieldSize() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 196)); // s_symbol_field_size
    }

    public static int symbol_getField_ptr(int _self, int fieldIdx)
    //func symbol_getField_ptr(_self: Int, fieldIdx: Int) -> Int
    {
        return _add(_self, _mul(symbol_fieldSize(), fieldIdx));
    }

    public static int makeSymbol(int destTblSlot_ptr, int identifierStr_ptr, int symbolType, int scopeDepth, int definitionLine)
    //func makeSymbol(destTblSlot_ptr: Int,identifierStr_ptr: Int,symbolType: Int,scopeDepth: Int,definitionLine: Int) -> Int
    {
        var offset = (int)default;
        var fieldSize = (int)default;

        /* set */ fieldSize = symbol_fieldSize();

        _ = memWrite(_add(destTblSlot_ptr, offset), identifierStr_ptr); // self.identifierStr_ptr
        /* set */ offset = _add(offset, fieldSize);
        _ = memWrite(_add(destTblSlot_ptr, offset), symbolType); // self.symbolType
        /* set */ offset = _add(offset, fieldSize);
        _ = memWrite(_add(destTblSlot_ptr, offset), definitionLine); // self.definitionLine
        /* set */ offset = _add(offset, fieldSize);
        _ = memWrite(_add(destTblSlot_ptr, offset), scopeDepth); // self.scopeDepth
        /* set */ offset = _add(offset, fieldSize);
        _ = memWrite(_add(destTblSlot_ptr, offset), 0); // self.paramCount
        /* set */ offset = _add(offset, fieldSize);
        _ = memWrite(_add(destTblSlot_ptr, offset), 0); // self.definitionFound
        /* set */ offset = _add(offset, fieldSize);
        _ = memWrite(_add(destTblSlot_ptr, offset), 0); // self.wasReferenced
        /* set */ offset = _add(offset, fieldSize);
        _ = memWrite(_add(destTblSlot_ptr, offset), 0); // self.address [unused in IL]

        return 0;
    }

    public static int getFunctionTbl_ptr()
    //func getFunctionTbl_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 200)); // s_functionsTbl_ptr
    }

    public static int getFunctionTbl_count()
    //func getFunctionTbl_count() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 204)); // s_functionsTbl_count
    }

    public static int getFunctionTbl_names_ptr()
    //func getFunctionTbl_names_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 208)); // s_functionsTbl_names_ptr
    }

    public static int getFunctionTbl_names_offset()
    //func getFunctionTbl_names_offset() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 212)); // s_functionsTbl_names_offset
    }

    public static int getFunctionScopeTbl_ptr()
    //func getFunctionScopeTbl_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 216)); // s_functionScopeTbl_ptr
    }

    public static int getFunctionScopeTbl_count()
    //func getFunctionScopeTbl_count() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 220)); // s_functionScopeTbl_count
    }

    public static int getFunctionScopeTbl_names_ptr()
    //func getFunctionScopeTbl_names_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 224)); // s_functionScopeTbl_names_ptr
    }

    public static int getFunctionScopeTbl_names_offset()
    //func getFunctionScopeTbl_names_offset() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 228)); // s_functionScopeTbl_names_offset
    }

    public static int getUnresolvedGotoTbl_ptr()
    //func getUnresolvedGotoTbl_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 232)); // s_unresolvedGotoTbl_ptr
    }

    public static int getUnresolvedGotoTbl_count()
    //func getUnresolvedGotoTbl_count() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 236)); // s_unresolvedGotoTbl_count
    }

    public static int getUnresolvedGotoTbl_names_ptr()
    //func getUnresolvedGotoTbl_names_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 240)); // s_unresolvedGotoTbl_names_ptr
    }

    public static int getUnresolvedGotoTbl_names_offset()
    //func getUnresolvedGotoTbl_names_offset() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 244)); // s_unresolvedGotoTbl_names_offset
    }

    public static int getTypeNamesTbl_ptr()
    //func getTypeNamesTbl_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 248)); // s_typeNamesTbl_ptr
    }

    public static int getTypeNamesTbl_count()
    //func getTypeNamesTbl_count() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 252)); // s_typeNamesTbl_count
    }

    public static int getTypeNamesTbl_names_ptr()
    //func getTypeNamesTbl_names_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 256)); // s_typeNamesTbl_names_ptr
    }

    public static int getTypeNamesTbl_names_offset()
    //func getTypeNamesTbl_names_offset() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 260)); // s_typeNamesTbl_names_offset
    }

    public static int allocateFunctionSymbolStr(int identifierStr_ptr)
    //func allocateFunctionSymbolStr(identifierStr_ptr: Int) -> Int
    {
        var g_offset = (int)default;
        var str_base = (int)default;
        var str_offset = (int)default;
        var str_dest = (int)default;
        var str_len = (int)default;

        /* set */ g_offset = 0;
        /* set */ str_base = getFunctionTbl_names_ptr();
        /* set */ str_offset = getFunctionTbl_names_offset();
        /* set */ str_dest = _add(str_base, str_offset);

        /* set */ str_len = strCpy(identifierStr_ptr, str_dest);
        _ = memWrite(_add(g_offset, 212), _add(str_offset, str_len)); // update s_functionsTbl_names_offset

        // returns the ptr offset of the allocated string
        return str_dest;
    }

    public static int insertFunctionSymbol(int identifierStr_ptr)
    //func insertFunctionSymbol(identifierStr_ptr: Int) -> Int
    {
        var g_offset = (int)default;
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionTbl_ptr = (int)default;

        /* set */ g_offset = 0;
        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

        /* set */ result = 1;

        if (__bool_check(_gte(count, 512)))
        {
            _ = printError($"Reached limit of function symbols: {512}");
            /* set */ result = 0;
            goto END;
        }

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto INSERT_SYMBOL;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            // s_inFunctionScope && symbol.definitionFound
            if (__bool_check(_and(isInFunctionScope(), memRead(symbol_getField_ptr(slotOffset, 5)))))
            {
                _ = printError($"Duplicate definition for function '{Program.ReadString(identifierStr_ptr, 128)}' at line {Lexer_getLastValidLine()}");
                _ = printError($"First definition: line {memRead(symbol_getField_ptr(slotOffset, 2))}"); // symbol.definitionLine
            }

            /* set */ result = 0;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    INSERT_SYMBOL:
        _ = makeSymbol(slotOffset, allocateFunctionSymbolStr(identifierStr_ptr), 0, 0, 0);
        _ = memWrite(_add(g_offset, 204), _add(count, 1)); // s_functionsTbl_count++

    END:
        return result;
    }

    public static int containsFunctionSymbol(int identifierStr_ptr)
    //func containsFunctionSymbol(identifierStr_ptr: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionTbl_ptr = (int)default;

        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            /* set */ result = 1;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int markFunctionSymbolDefinitionFound(int identifierStr_ptr, int definitionLine)
    //func markFunctionSymbolDefinitionFound(identifierStr_ptr: Int, definitionLine: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var functionTbl_ptr = (int)default;

        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            _ = memWrite(symbol_getField_ptr(slotOffset, 5), 1); // symbol.definitionFound = 1
            _ = memWrite(symbol_getField_ptr(slotOffset, 2), definitionLine); // symbol.definitionLine = definitionLine
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return 0;
    }

    public static int markFunctionSymbolAsReferenced(int identifierStr_ptr, int referencedLine)
    //func markFunctionSymbolAsReferenced(identifierStr_ptr: Int, referencedLine: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var functionTbl_ptr = (int)default;

        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            // !symbol.definitionFound && !symbol.wasReferenced
            if (__bool_check(_and(_neq(memRead(symbol_getField_ptr(slotOffset, 5)), 1), _neq(memRead(symbol_getField_ptr(slotOffset, 6)), 1))))
            {
                _ = memWrite(symbol_getField_ptr(slotOffset, 6), 1); // symbol.wasReferenced = 1
                _ = memWrite(symbol_getField_ptr(slotOffset, 2), referencedLine); // symbol.definitionLine = referencedLine
            }
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return 0;
    }

    public static int isFunctionSymbolDefinitionFound(int identifierStr_ptr)
    //func isFunctionSymbolDefinitionFound(identifierStr_ptr: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionTbl_ptr = (int)default;

        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            /* set */ result = memRead(symbol_getField_ptr(slotOffset, 5)); // result = symbol.definitionFound
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int updateFunctionSymbolParamCount(int identifierStr_ptr, int paramCount)
    //func updateFunctionSymbolParamCount(identifierStr_ptr: Int, paramCount: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var functionTbl_ptr = (int)default;

        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            _ = memWrite(symbol_getField_ptr(slotOffset, 4), paramCount); // symbol.paramCount = paramCount
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return 0;
    }

    public static int matchFunctionSymbolParamCount(int identifierStr_ptr, int paramCount)
    //func matchFunctionSymbolParamCount(identifierStr_ptr: Int, paramCount: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var functionTbl_ptr = (int)default;
        var result = (int)default;

        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            if (__bool_check(_eq(memRead(symbol_getField_ptr(slotOffset, 4)), paramCount)))
            {
                /* set */ result = 1;
            }
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int getFunctionSymbol_ptr(int identifierStr_ptr)
    //func getFunctionSymbol_ptr(identifierStr_ptr: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var functionTbl_ptr = (int)default;
        var result = (int)default;

        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            /* set */ result = slotOffset;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int anyReferencedFunctionNotDefined()
    //func anyReferencedFunctionNotDefined() -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionTbl_ptr = (int)default;

        /* set */ functionTbl_ptr = getFunctionTbl_ptr();
        /* set */ count = getFunctionTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        // symbol.wasReferenced && !symbol.definitionFound
        if (__bool_check(_and(_eq(memRead(symbol_getField_ptr(slotOffset, 6)), 1), _neq(memRead(symbol_getField_ptr(slotOffset, 5)), 1))))
        {
            _ = printError($"Unresolved reference of undefined function '{Program.ReadString(slotOffset, 128)}' at line {memRead(symbol_getField_ptr(slotOffset, 2))}.");
            /* set */ result = 1;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int isMainFunctionValid()
    //func isMainFunctionValid() -> Int
    {
        var mainName = (int)default;
        var result = (int)default;

        /* set */ mainName = getTmpBuffer_ptr();

        _ = memWrite8(_add(mainName, 0), 109); // 'm'
        _ = memWrite8(_add(mainName, 1),  97); // 'a'
        _ = memWrite8(_add(mainName, 2), 105); // 'i'
        _ = memWrite8(_add(mainName, 3), 110); // 'n'
        _ = memWrite8(_add(mainName, 4),   0); // '\0'

        if (__bool_check(_and(_and(containsFunctionSymbol(mainName), matchFunctionSymbolParamCount(mainName, 0)), isFunctionSymbolDefinitionFound(mainName))))
        {
            /* set */ result = 1;
        }

        return result;
    }

    public static int allocateFunctionScopeSymbolStr(int identifierStr_ptr)
    //func allocateFunctionScopeSymbolStr(identifierStr_ptr: Int) -> Int
    {
        var g_offset = (int)default;
        var str_base = (int)default;
        var str_offset = (int)default;
        var str_dest = (int)default;
        var str_len = (int)default;

        /* set */ g_offset = 0;
        /* set */ str_base = getFunctionScopeTbl_names_ptr();
        /* set */ str_offset = getFunctionScopeTbl_names_offset();
        /* set */ str_dest = _add(str_base, str_offset);

        /* set */ str_len = strCpy(identifierStr_ptr, str_dest);
        _ = memWrite(_add(g_offset, 228), _add(str_offset, str_len)); // update s_functionScopeTbl_names_offset

        // returns the ptr offset of the allocated string
        return str_dest;
    }

    public static int resetFunctionScope()
    //func resetFunctionScope() -> Int
    {
        var g_offset = (int)default;

        /* set */ g_offset = 0;

        _ = memWrite(_add(g_offset, 220), 0); // s_functionScopeTbl_count = 0
        _ = memWrite(_add(g_offset, 236), 0); // s_unresolvedGotoTbl_count = 0
        _ = memWrite(_add(g_offset, 176), 0); // s_curScopeDepth = 0
        _ = resetIfLabelCounter();

        return 0;
    }

    public static int insertFunctionScopeSymbol(int identifierStr_ptr, int symbolType, int scopeDepth, int definitionLine)
    //func insertFunctionScopeSymbol(identifierStr_ptr: Int, symbolType: Int, scopeDepth: Int, definitionLine: Int) -> Int
    {
        var g_offset = (int)default;
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionScopeTbl_ptr = (int)default;

        /* set */ g_offset = 0;
        /* set */ functionScopeTbl_ptr = getFunctionScopeTbl_ptr();
        /* set */ count = getFunctionScopeTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

        /* set */ result = 1;

        if (__bool_check(_gte(count, 2048)))
        {
            _ = printError($"Reached limit of function scope symbols: {2048}");
            /* set */ result = 0;
            goto END;
        }

    LOOP:
        /* set */ slotOffset = _add(functionScopeTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto INSERT_SYMBOL;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            _ = printError($"Duplicate definition for '{Program.ReadString(identifierStr_ptr, 128)}' at line {Lexer_getLastValidLine()}");
            _ = printError($"First definition: line {memRead(symbol_getField_ptr(slotOffset, 2))}"); // symbol.definitionLine

            /* set */ result = 0;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    INSERT_SYMBOL:
        _ = makeSymbol(slotOffset, allocateFunctionScopeSymbolStr(identifierStr_ptr), symbolType, scopeDepth, definitionLine);
        _ = memWrite(_add(g_offset, 220), _add(count, 1)); // update s_functionScopeTbl_count

    END:
        return result;
    }

    public static int containsFunctionScopeSymbol(int identifierStr_ptr)
    //func containsFunctionScopeSymbol(identifierStr_ptr: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionScopeTbl_ptr = (int)default;

        /* set */ functionScopeTbl_ptr = getFunctionScopeTbl_ptr();
        /* set */ count = getFunctionScopeTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionScopeTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            /* set */ result = 1;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int updateFunctionScopeSymbolScopeDepth(int identifierStr_ptr, int scopeDepth)
    //func updateFunctionScopeSymbolScopeDepth(identifierStr_ptr: Int, scopeDepth: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var functionScopeTbl_ptr = (int)default;

        /* set */ functionScopeTbl_ptr = getFunctionScopeTbl_ptr();
        /* set */ count = getFunctionScopeTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionScopeTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            _ = memWrite(symbol_getField_ptr(slotOffset, 3), scopeDepth); // symbol.scopeDepth = scopeDepth
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return 0;
    }

    public static int markFunctionScopeSymbolDefinitionFound(int identifierStr_ptr, int definitionLine)
    //func markFunctionScopeSymbolDefinitionFound(identifierStr_ptr: Int, definitionLine: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var functionScopeTbl_ptr = (int)default;

        /* set */ functionScopeTbl_ptr = getFunctionScopeTbl_ptr();
        /* set */ count = getFunctionScopeTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionScopeTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            _ = memWrite(symbol_getField_ptr(slotOffset, 5), 1); // symbol.definitionFound = 1
            _ = memWrite(symbol_getField_ptr(slotOffset, 2), definitionLine); // symbol.definitionLine = definitionLine
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return 0;
    }

    public static int isFunctionScopeSymbolDefinitionFound(int identifierStr_ptr)
    //func isFunctionScopeSymbolDefinitionFound(identifierStr_ptr: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionScopeTbl_ptr = (int)default;

        /* set */ functionScopeTbl_ptr = getFunctionScopeTbl_ptr();
        /* set */ count = getFunctionScopeTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionScopeTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            /* set */ result = memRead(symbol_getField_ptr(slotOffset, 5)); // result = symbol.definitionFound
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int matchFunctionScopeSymbolType(int identifierStr_ptr, int symbolType)
    //func matchFunctionScopeSymbolType(identifierStr_ptr: Int, symbolType: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionScopeTbl_ptr = (int)default;

        /* set */ functionScopeTbl_ptr = getFunctionScopeTbl_ptr();
        /* set */ count = getFunctionScopeTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionScopeTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            if (__bool_check(_eq(memRead(symbol_getField_ptr(slotOffset, 1)), symbolType)))
            {
                /* set */ result = 1;
            }
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int getFunctionScopeSymbol_ptr(int identifierStr_ptr)
    //func getFunctionScopeSymbol_ptr(identifierStr_ptr: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var functionScopeTbl_ptr = (int)default;

        /* set */ functionScopeTbl_ptr = getFunctionScopeTbl_ptr();
        /* set */ count = getFunctionScopeTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(functionScopeTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            /* set */ result = slotOffset;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int allocateUnresolvedGotoSymbolStr(int identifierStr_ptr)
    //func allocateUnresolvedGotoSymbolStr(identifierStr_ptr: Int) -> Int
    {
        var g_offset = (int)default;
        var str_base = (int)default;
        var str_offset = (int)default;
        var str_dest = (int)default;
        var str_len = (int)default;

        /* set */ g_offset = 0;
        /* set */ str_base = getUnresolvedGotoTbl_names_ptr();
        /* set */ str_offset = getUnresolvedGotoTbl_names_offset();
        /* set */ str_dest = _add(str_base, str_offset);

        /* set */ str_len = strCpy(identifierStr_ptr, str_dest);
        _ = memWrite(_add(g_offset, 244), _add(str_offset, str_len)); // update s_unresolvedGotoTbl_names_offset

        // returns the ptr offset of the allocated string
        return str_dest;
    }

    public static int insertUnresolvedGotoSymbol(int identifierStr_ptr, int scopeDepth, int definitionLine)
    //func insertUnresolvedGotoSymbol(identifierStr_ptr: Int, scopeDepth: Int, definitionLine: Int) -> Int
    {
        var g_offset = (int)default;
        var count = (int)default;
        var symbolSize = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var unresolvedGotoTbl_ptr = (int)default;

        /* set */ g_offset = 0;
        /* set */ unresolvedGotoTbl_ptr = getUnresolvedGotoTbl_ptr();
        /* set */ count = getUnresolvedGotoTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

        /* set */ result = 1;

        if (__bool_check(_gte(count, 512)))
        {
            _ = printError($"Reached limit of unresolved goto symbols: {512}");
            /* set */ result = 0;
            goto END;
        }

        /* set */ slotOffset = _add(unresolvedGotoTbl_ptr, _mul(count, symbolSize));

        _ = makeSymbol(slotOffset, allocateUnresolvedGotoSymbolStr(identifierStr_ptr), 3, scopeDepth, definitionLine);
        _ = memWrite(_add(g_offset, 236), _add(count, 1)); // update s_unresolvedGotoTbl_count

    END:
        return result;
    }

    public static int anyUnresolvedGoto()
    //func anyUnresolvedGoto() -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var unresolvedGotoTbl_ptr = (int)default;

        /* set */ unresolvedGotoTbl_ptr = getUnresolvedGotoTbl_ptr();
        /* set */ count = getUnresolvedGotoTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(unresolvedGotoTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(_neq(containsFunctionScopeSymbol(slotOffset), 1)))
        {
            _ = printError($"Use of undefined label symbol '{Program.ReadString(slotOffset, 128)}' at line {memRead(symbol_getField_ptr(slotOffset, 2))}.");
            /* set */ result = 1;
            goto END;
        }

        if (__bool_check(_gt(memRead(symbol_getField_ptr(getFunctionScopeSymbol_ptr(slotOffset),2)) ,memRead(symbol_getField_ptr(slotOffset, 2)))))
        {
            _ = printError($"Label '{Program.ReadString(slotOffset, 128)}' is not available at current scope at line {memRead(symbol_getField_ptr(slotOffset, 2))}.");
            /* set */ result = 1;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    public static int allocateTypeNameSymbolStr(int identifierStr_ptr)
    //func allocateTypeNameSymbolStr(identifierStr_ptr: Int) -> Int
    {
        var g_offset = (int)default;
        var str_base = (int)default;
        var str_offset = (int)default;
        var str_dest = (int)default;
        var str_len = (int)default;

        /* set */ g_offset = 0;
        /* set */ str_base = getTypeNamesTbl_names_ptr();
        /* set */ str_offset = getTypeNamesTbl_names_offset();
        /* set */ str_dest = _add(str_base, str_offset);

        /* set */ str_len = strCpy(identifierStr_ptr, str_dest);
        _ = memWrite(_add(g_offset, 260), _add(str_offset, str_len)); // update s_typeNamesTbl_names_offset

        // returns the ptr offset of the allocated string
        return str_dest;
    }

    public static int insertTypeNameSymbol(int identifierStr_ptr, int definitionLine)
    //func insertTypeNameSymbol(identifierStr_ptr: Int, definitionLine: Int) -> Int
    {
        var g_offset = (int)default;
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var typeNamesTbl_ptr = (int)default;

        /* set */ g_offset = 0;
        /* set */ typeNamesTbl_ptr = getTypeNamesTbl_ptr();
        /* set */ count = getTypeNamesTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

        /* set */ result = 1;

        if (__bool_check(_gte(count, 32)))
        {
            _ = printError($"Reached limit of type name symbols: {32}");
            /* set */ result = 0;
            goto END;
        }

    LOOP:
        /* set */ slotOffset = _add(typeNamesTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto INSERT_SYMBOL;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            _ = printError($"Duplicate definition for '{Program.ReadString(identifierStr_ptr, 128)}' at line {Lexer_getLastValidLine()}");
            _ = printError($"First definition: line {memRead(symbol_getField_ptr(slotOffset, 2))}"); // symbol.definitionLine

            /* set */ result = 0;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    INSERT_SYMBOL:
        _ = makeSymbol(slotOffset, allocateTypeNameSymbolStr(identifierStr_ptr), 4, 0, definitionLine);
        _ = memWrite(_add(g_offset, 252), _add(count, 1)); // update s_typeNamesTbl_count

    END:
        return result;
    }

    public static int containsTypeNameSymbol(int identifierStr_ptr)
    //func containsTypeNameSymbol(identifierStr_ptr: Int) -> Int
    {
        var count = (int)default;
        var symbolSize = (int)default;
        var i = (int)default;
        var slotOffset = (int)default;
        var fieldSize = (int)default;
        var result = (int)default;
        var typeNamesTbl_ptr = (int)default;

        /* set */ typeNamesTbl_ptr = getTypeNamesTbl_ptr();
        /* set */ count = getTypeNamesTbl_count();
        /* set */ symbolSize = symbol_sizeOf();
        /* set */ fieldSize = symbol_fieldSize();

    LOOP:
        /* set */ slotOffset = _add(typeNamesTbl_ptr, _mul(i, symbolSize));

        if (__bool_check(_eq(i, count)))
        {
            goto END;
        }

        if (__bool_check(strEquals(memRead(slotOffset), identifierStr_ptr)))
        {
            /* set */ result = 1;
            goto END;
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    END:
        return result;
    }

    #endregion Symbols

    #region IL Emitter

    public static int IL_Emitter_getStrBuffer_ptr()
    //func IL_Emitter_getStrBuffer_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 280)); // s_IL_Emitter_strBuffer_ptr
    }

    public static int IL_Emitter_getStrBuffer_pos()
    //func IL_Emitter_getStrBuffer_pos() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 284)); // s_IL_Emitter_strBuffer_pos
    }

    public static int IL_Emitter_getBaseILBeginTxt_ptr()
    //func IL_Emitter_getBaseILBeginTxt_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 288)); // s_IL_Emitter_baseILBeginTxt_ptr
    }

    public static int IL_Emitter_getBaseILEndTxt_ptr()
    //func IL_Emitter_getBaseILEndTxt_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 292)); // s_IL_Emitter_baseILEndTxt_ptr
    }

    public static int IL_Emitter_getRuntimeJsonTxt_ptr()
    //func IL_Emitter_getRuntimeJsonTxt_ptr() -> Int
    {
        var g_offset = (int)default;
        /* set */ g_offset = 0;

        return memRead(_add(g_offset, 296)); // s_IL_Emitter_RuntimeJsonTxt_ptr
    }

    public static int IL_Emitter_getGlobalFunctionsNamespaceStr()
    //func IL_Emitter_getGlobalFunctionsNamespaceStr() -> Int
    {
        var tmpBuff = (int)default;

        /* set */ tmpBuff = getTmpBuffer_ptr();
        /* set */ tmpBuff = _add(tmpBuff, 128); // s_tmpBuff2_ptr

        _ = memWrite8(_add(tmpBuff,  0),  70); // 'F'
        _ = memWrite8(_add(tmpBuff,  1),  70); // 'F'
        _ = memWrite8(_add(tmpBuff,  2),  76); // 'L'
        _ = memWrite8(_add(tmpBuff,  3),  97); // 'a'
        _ = memWrite8(_add(tmpBuff,  4), 110); // 'n'
        _ = memWrite8(_add(tmpBuff,  5), 103); // 'g'
        _ = memWrite8(_add(tmpBuff,  6),  95); // '_'
        _ = memWrite8(_add(tmpBuff,  7),  71); // 'G'
        _ = memWrite8(_add(tmpBuff,  8), 108); // 'l'
        _ = memWrite8(_add(tmpBuff,  9), 111); // 'o'
        _ = memWrite8(_add(tmpBuff, 10),  98); // 'b'
        _ = memWrite8(_add(tmpBuff, 11),  97); // 'a'
        _ = memWrite8(_add(tmpBuff, 12), 108); // 'l'
        _ = memWrite8(_add(tmpBuff, 13),  46); // '.'
        _ = memWrite8(_add(tmpBuff, 14),  70); // 'F'
        _ = memWrite8(_add(tmpBuff, 15), 117); // 'u'
        _ = memWrite8(_add(tmpBuff, 16), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 17),  99); // 'c'
        _ = memWrite8(_add(tmpBuff, 18), 116); // 't'
        _ = memWrite8(_add(tmpBuff, 19), 105); // 'i'
        _ = memWrite8(_add(tmpBuff, 20), 111); // 'o'
        _ = memWrite8(_add(tmpBuff, 21), 110); // 'n'
        _ = memWrite8(_add(tmpBuff, 22), 115); // 's'
        _ = memWrite8(_add(tmpBuff, 23),  58); // ':'
        _ = memWrite8(_add(tmpBuff, 24),  58); // ':'
        _ = memWrite8(_add(tmpBuff, 25),   0); // '\0'

        return tmpBuff;
    }

    public static int IL_Emitter_appendChar(int c)
    //func IL_Emitter_appendChar(c: Int) -> Int
    {
        var g_offset = (int)default;
        var pos = (int)default;
        var strBuffer = (int)default;
        var result = (int)default;

        /* set */ result = 1;

        // Ignore '\0'
        if (__bool_check(_eq(c, 0)))
        {
            goto END;
        }

        /* set */ g_offset = 0;

        /* set */ strBuffer = IL_Emitter_getStrBuffer_ptr();
        /* set */ pos = IL_Emitter_getStrBuffer_pos();

        if (__bool_check(_gte(pos, 262144))) // max len 256 KB
        {
            /* set */ result = 0;
            _ = printError($"The IL_Emitter strBuffer limit is {262144} bytes");
            goto END;
        }

        _ = memWrite8(_add(strBuffer, pos), c); // s_IL_Emitter_strBuffer[pos] = c
        /* set */ pos = _add(pos, 1); // pos++

        // Insert termination
        _ = memWrite8(_add(strBuffer, pos), 0); // s_IL_Emitter_strBuffer[pos] = '\0'

        _ = memWrite(_add(g_offset, 284), pos); // update s_IL_Emitter_strBuffer_pos

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

    public static int IL_Emitter_emit_label(int labelStr_ptr)
    //func IL_Emitter_emit_label(labelStr_ptr: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendTxt(labelStr_ptr));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(58));  // ':'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_goto(int labelStr_ptr)
    //func IL_Emitter_emit_goto(labelStr_ptr: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(98));  // 'b'
        /* set */ result = _and(result, IL_Emitter_appendChar(114)); // 'r'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendTxt(labelStr_ptr));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_call(int functionNameStr_ptr, int argsCount)
    //func IL_Emitter_emit_call(functionNameStr_ptr: Int, argsCount: Int) -> Int
    {
        var result = (int)default;
        var i = (int)default;
        var last_i = (int)default;

        /* set */ result = 1;
        /* set */ last_i = _sub(argsCount, 1);

        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(110)); // 'n'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(51));  // '3'
        /* set */ result = _and(result, IL_Emitter_appendChar(50));  // '2'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendTxt(IL_Emitter_getGlobalFunctionsNamespaceStr()));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendTxt(functionNameStr_ptr));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(40));  // '('

    LOOP:
        if (__bool_check(_eq(i, argsCount)))
        {
            goto ARGS_TAIL;
        }

        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(110)); // 'n'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(51));  // '3'
        /* set */ result = _and(result, IL_Emitter_appendChar(50));  // '2'

        if (__bool_check(_lt(i, last_i)))
        {
            /* set */ result = _and(result, IL_Emitter_appendChar(44));  // ','
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    ARGS_TAIL:
        /* set */ result = _and(result, IL_Emitter_appendChar(41));  // ')'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_boolCheck()
    //func IL_Emitter_emit_boolCheck() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(98));  // 'b'
        /* set */ result = _and(result, IL_Emitter_appendChar(111)); // 'o'
        /* set */ result = _and(result, IL_Emitter_appendChar(111)); // 'o'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendTxt(IL_Emitter_getGlobalFunctionsNamespaceStr()));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendTxt(getBuiltin_boolCheck()));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(40));  // '('
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(110)); // 'n'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(51));  // '3'
        /* set */ result = _and(result, IL_Emitter_appendChar(50));  // '2'
        /* set */ result = _and(result, IL_Emitter_appendChar(41));  // ')'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_ifBegin(int ifLabelCounter)
    //func IL_Emitter_emit_ifBegin(ifLabelCounter: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(98));  // 'b'
        /* set */ result = _and(result, IL_Emitter_appendChar(114)); // 'r'
        /* set */ result = _and(result, IL_Emitter_appendChar(102)); // 'f'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(101)); // 'e'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(73));  // 'I'
        /* set */ result = _and(result, IL_Emitter_appendChar(70));  // 'F'
        /* set */ result = _and(result, IL_Emitter_appendChar(95));  // '_'
        /* set */ result = _and(result, IL_Emitter_appendChar(70));  // 'F'
        /* set */ result = _and(result, IL_Emitter_appendChar(65));  // 'A'
        /* set */ result = _and(result, IL_Emitter_appendChar(76));  // 'L'
        /* set */ result = _and(result, IL_Emitter_appendChar(83));  // 'S'
        /* set */ result = _and(result, IL_Emitter_appendChar(69));  // 'E'
        /* set */ result = _and(result, IL_Emitter_appendChar(95));  // '_'
        /* set */ result = _and(result, IL_Emitter_appendChar(36));  // '$'
        /* set */ result = _and(result, IL_Emitter_appendTxt(intToStr(ifLabelCounter)));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_else(int hasBlock, int ifLabelCounter)
    //func IL_Emitter_emit_else(hasBlock: Int, ifLabelCounter: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(_eq(hasBlock, 1)))
        {
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '

            /* set */ result = _and(result, IL_Emitter_appendChar(98));  // 'b'
            /* set */ result = _and(result, IL_Emitter_appendChar(114)); // 'r'
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
            /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
            /* set */ result = _and(result, IL_Emitter_appendChar(73));  // 'I'
            /* set */ result = _and(result, IL_Emitter_appendChar(70));  // 'F'
            /* set */ result = _and(result, IL_Emitter_appendChar(95));  // '_'
            /* set */ result = _and(result, IL_Emitter_appendChar(69));  // 'E'
            /* set */ result = _and(result, IL_Emitter_appendChar(78));  // 'N'
            /* set */ result = _and(result, IL_Emitter_appendChar(68));  // 'D'
            /* set */ result = _and(result, IL_Emitter_appendChar(95));  // '_'
            /* set */ result = _and(result, IL_Emitter_appendChar(36));  // '$'
            /* set */ result = _and(result, IL_Emitter_appendTxt(intToStr(ifLabelCounter)));
            /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
            /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
            /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'
        }

        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(73));  // 'I'
        /* set */ result = _and(result, IL_Emitter_appendChar(70));  // 'F'
        /* set */ result = _and(result, IL_Emitter_appendChar(95));  // '_'
        /* set */ result = _and(result, IL_Emitter_appendChar(70));  // 'F'
        /* set */ result = _and(result, IL_Emitter_appendChar(65));  // 'A'
        /* set */ result = _and(result, IL_Emitter_appendChar(76));  // 'L'
        /* set */ result = _and(result, IL_Emitter_appendChar(83));  // 'S'
        /* set */ result = _and(result, IL_Emitter_appendChar(69));  // 'E'
        /* set */ result = _and(result, IL_Emitter_appendChar(95));  // '_'
        /* set */ result = _and(result, IL_Emitter_appendChar(36));  // '$'
        /* set */ result = _and(result, IL_Emitter_appendTxt(intToStr(ifLabelCounter)));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(58));  // ':'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_ifEnd(int ifLabelCounter)
    //func IL_Emitter_emit_ifEnd(ifLabelCounter: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(73));  // 'I'
        /* set */ result = _and(result, IL_Emitter_appendChar(70));  // 'F'
        /* set */ result = _and(result, IL_Emitter_appendChar(95));  // '_'
        /* set */ result = _and(result, IL_Emitter_appendChar(69));  // 'E'
        /* set */ result = _and(result, IL_Emitter_appendChar(78));  // 'N'
        /* set */ result = _and(result, IL_Emitter_appendChar(68));  // 'D'
        /* set */ result = _and(result, IL_Emitter_appendChar(95));  // '_'
        /* set */ result = _and(result, IL_Emitter_appendChar(36));  // '$'
        /* set */ result = _and(result, IL_Emitter_appendTxt(intToStr(ifLabelCounter)));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(58));  // ':'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_discardReturn()
    //func IL_Emitter_emit_discardReturn() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(112)); // 'p'
        /* set */ result = _and(result, IL_Emitter_appendChar(111)); // 'o'
        /* set */ result = _and(result, IL_Emitter_appendChar(112)); // 'p'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_return()
    //func IL_Emitter_emit_return() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(114)); // 'r'
        /* set */ result = _and(result, IL_Emitter_appendChar(101)); // 'e'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_loadConst(int vStr_ptr)
    //func IL_Emitter_emit_loadConst(vStr_ptr: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(100)); // 'd'
        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(46));  // '.'
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(52));  // '4'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendTxt(vStr_ptr));
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_loadLocalVar(int index)
    //func IL_Emitter_emit_loadLocalVar(index: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(100)); // 'd'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(111)); // 'o'
        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(46));  // '.'
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendTxt(intToStr(index)));
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_loadArgVar(int index)
    //func IL_Emitter_emit_loadArgVar(index: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(100)); // 'd'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(114)); // 'r'
        /* set */ result = _and(result, IL_Emitter_appendChar(103)); // 'g'
        /* set */ result = _and(result, IL_Emitter_appendChar(46));  // '.'
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendTxt(intToStr(index)));
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_storeLocalVar(int index)
    //func IL_Emitter_emit_storeLocalVar(index: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(111)); // 'o'
        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(46));  // '.'
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendTxt(intToStr(index)));
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_storeArgVar(int index)
    //func IL_Emitter_emit_storeArgVar(index: Int) -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(114)); // 'r'
        /* set */ result = _and(result, IL_Emitter_appendChar(103)); // 'g'
        /* set */ result = _and(result, IL_Emitter_appendChar(46));  // '.'
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendTxt(intToStr(index)));
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_methodHeader(int nameStr_ptr, int paramCount)
    //func IL_Emitter_emit_methodHeader(nameStr_ptr: Int, paramCount: Int) -> Int
    {
        var result = (int)default;
        var i = (int)default;
        var last_i = (int)default;

        /* set */ result = 1;
        /* set */ last_i = _sub(paramCount, 1);

        /* set */ result = _and(result, IL_Emitter_appendChar(46));  // '.'
        /* set */ result = _and(result, IL_Emitter_appendChar(109)); // 'm'
        /* set */ result = _and(result, IL_Emitter_appendChar(101)); // 'e'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(104)); // 'h'
        /* set */ result = _and(result, IL_Emitter_appendChar(111)); // 'o'
        /* set */ result = _and(result, IL_Emitter_appendChar(100)); // 'd'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(112)); // 'p'
        /* set */ result = _and(result, IL_Emitter_appendChar(117)); // 'u'
        /* set */ result = _and(result, IL_Emitter_appendChar(98));  // 'b'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(104)); // 'h'
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(100)); // 'd'
        /* set */ result = _and(result, IL_Emitter_appendChar(101)); // 'e'
        /* set */ result = _and(result, IL_Emitter_appendChar(98));  // 'b'
        /* set */ result = _and(result, IL_Emitter_appendChar(121)); // 'y'
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(103)); // 'g'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(110)); // 'n'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(51));  // '3'
        /* set */ result = _and(result, IL_Emitter_appendChar(50));  // '2'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendTxt(nameStr_ptr));
        /* set */ result = _and(result, IL_Emitter_appendChar(39));  // '\''
        /* set */ result = _and(result, IL_Emitter_appendChar(40));  // '('

    LOOP:
        if (__bool_check(_eq(i, paramCount)))
        {
            goto PARAMS_TAIL;
        }

        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(110)); // 'n'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(51));  // '3'
        /* set */ result = _and(result, IL_Emitter_appendChar(50));  // '2'

        if (__bool_check(_lt(i, last_i)))
        {
            /* set */ result = _and(result, IL_Emitter_appendChar(44));  // ','
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    PARAMS_TAIL:
        /* set */ result = _and(result, IL_Emitter_appendChar(41));  // ')'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(109)); // 'm'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(110)); // 'n'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(103)); // 'g'
        /* set */ result = _and(result, IL_Emitter_appendChar(101)); // 'e'
        /* set */ result = _and(result, IL_Emitter_appendChar(100)); // 'd'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        return result;
    }

    public static int IL_Emitter_emit_methodBodyBegin()
    //func IL_Emitter_emit_methodBodyBegin() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(123)); // '{'
        /* set */ result = _and(result, IL_Emitter_appendChar(13)); // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10)); // '\n'

        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(46));  // '.'
        /* set */ result = _and(result, IL_Emitter_appendChar(109)); // 'm'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(120)); // 'x'
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(107)); // 'k'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(56));  // '8'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        /* set */ result = _and(result, IL_Emitter_appendChar(13)); // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10)); // '\n'

        return result;
    }

    public static int IL_Emitter_emit_methodBodyEnd()
    //func IL_Emitter_emit_methodBodyEnd() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        /* set */ result = _and(result, IL_Emitter_appendChar(125)); // '}'
        /* set */ result = _and(result, IL_Emitter_appendChar(13)); // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10)); // '\n'

        /* set */ result = _and(result, IL_Emitter_appendChar(13)); // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10)); // '\n'

        return result;
    }

    public static int IL_Emitter_emit_initLocals(int count)
    //func IL_Emitter_emit_initLocals(count: Int) -> Int
    {
        var result = (int)default;
        var i = (int)default;
        var last_i = (int)default;

        /* set */ result = 1;
        /* set */ last_i = _sub(count, 1);

        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(32)); // ' '

        /* set */ result = _and(result, IL_Emitter_appendChar(46));  // '.'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(111)); // 'o'
        /* set */ result = _and(result, IL_Emitter_appendChar(99));  // 'c'
        /* set */ result = _and(result, IL_Emitter_appendChar(97));  // 'a'
        /* set */ result = _and(result, IL_Emitter_appendChar(108)); // 'l'
        /* set */ result = _and(result, IL_Emitter_appendChar(115)); // 's'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(110)); // 'n'
        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        /* set */ result = _and(result, IL_Emitter_appendChar(40));  // '('

    LOOP:
        if (__bool_check(_eq(i, count)))
        {
            goto LOCALS_TAIL;
        }

        /* set */ result = _and(result, IL_Emitter_appendChar(105)); // 'i'
        /* set */ result = _and(result, IL_Emitter_appendChar(110)); // 'n'
        /* set */ result = _and(result, IL_Emitter_appendChar(116)); // 't'
        /* set */ result = _and(result, IL_Emitter_appendChar(51));  // '3'
        /* set */ result = _and(result, IL_Emitter_appendChar(50));  // '2'

        if (__bool_check(_lt(i, last_i)))
        {
            /* set */ result = _and(result, IL_Emitter_appendChar(44));  // ','
            /* set */ result = _and(result, IL_Emitter_appendChar(32));  // ' '
        }

        /* set */ i = _add(i, 1); // i++
        goto LOOP;

    LOCALS_TAIL:
        /* set */ result = _and(result, IL_Emitter_appendChar(41));  // ')'
        /* set */ result = _and(result, IL_Emitter_appendChar(13));  // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10));  // '\n'

        /* set */ result = _and(result, IL_Emitter_appendChar(13)); // '\r'
        /* set */ result = _and(result, IL_Emitter_appendChar(10)); // '\n'

        return result;
    }

    public static int IL_Emitter_initEmitter(int ramBinLength_MB)
    //func IL_Emitter_initEmitter(ramBinLength_MB: Int) -> Int
    {
        var g_offset = (int)default;
        var result = (int)default;

        /* set */ g_offset = 0;

        // Clean s_IL_Emitter_strBuffer - 256 KB length
        _ = memSet(IL_Emitter_getStrBuffer_ptr(), 0, 262144);
        _ = memWrite(_add(g_offset, 284), 0); // s_IL_Emitter_strBuffer_pos = 0

        /* set */ result = IL_Emitter_appendTxt(IL_Emitter_getBaseILBeginTxt_ptr());

        //TODO: set custom ramBinLength size

        return result;
    }

    public static int IL_Emitter_finishEmitter()
    //func IL_Emitter_finishEmitter() -> Int
    {
        var result = (int)default;

        /* set */ result = IL_Emitter_appendTxt(IL_Emitter_getBaseILEndTxt_ptr());

        return result;
    }

    #endregion IL Emitter

    #region Parser

    public static int getNextToken()
    //func getNextToken() -> Int
    {
        var srcTxt_ptr = (int)default;
        var tokenBuffer_ptr = (int)default;
        var tokenType = (int)default;
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
        _ = Lexer_resetTokenStrBuff_pos();

        /* set */ c = memRead8(_add(srcTxt_ptr, pos));
        if (__bool_check(_or(_eq(c, 0), _gte(pos, 131072)))) // '\0' || >= 128 KB
        {
            /* set */ tokenType = 21; // TokenType.EOF_TOKEN
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
            /* set */ c = memRead8(_add(srcTxt_ptr, pos));
            if (__bool_check(_or(_eq(c, 0), _gte(pos, 131072)))) // '\0' || >= 128 KB
            {
                /* set */ tokenType = 21; // TokenType.EOF_TOKEN
                _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
                goto END;
            }
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
                    /* set */ c = memRead8(_add(srcTxt_ptr, pos));
                    if (__bool_check(_or(_eq(c, 0), _gte(pos, 131072)))) // '\0' || >= 128 KB
                    {
                        /* set */ tokenType = 21; // TokenType.EOF_TOKEN
                        _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
                        goto END;
                    }
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
                    _ = memWrite8(_add(tokenBuffer_ptr, readCount), 0); // s_tokenStrBuff[0] = '\0'
                    goto END;
                }
                /* set */ pos = Lexer_incPos();
                /* set */ c = memRead8(_add(srcTxt_ptr, pos));
                if (__bool_check(_or(_eq(c, 0), _gte(pos, 131072)))) // '\0' || >= 128 KB
                {
                    /* set */ tokenType = 21; // TokenType.EOF_TOKEN
                    _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
                    goto END;
                }
                goto LOOP_IDENTIFIER;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 117), strEquals(tokenBuffer_ptr, getKeyword_using()))))
            {
                /* set */ tokenType = 2; // TokenType.USING
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 102), strEquals(tokenBuffer_ptr, getKeyword_func()))))
            {
                /* set */ tokenType = 3; // TokenType.FUNC
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 118), strEquals(tokenBuffer_ptr, getKeyword_var()))))
            {
                /* set */ tokenType = 4; // TokenType.VAR
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 115), strEquals(tokenBuffer_ptr, getKeyword_set()))))
            {
                /* set */ tokenType = 5; // TokenType.SET
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 105), strEquals(tokenBuffer_ptr, getKeyword_if()))))
            {
                /* set */ tokenType = 6; // TokenType.IF
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 101), strEquals(tokenBuffer_ptr, getKeyword_else()))))
            {
                /* set */ tokenType = 7; // TokenType.ELSE
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 103), strEquals(tokenBuffer_ptr, getKeyword_goto()))))
            {
                /* set */ tokenType = 8; // TokenType.GOTO
                goto END;
            }

            if (__bool_check(_and(_eq(memRead8(tokenBuffer_ptr), 114), strEquals(tokenBuffer_ptr, getKeyword_return()))))
            {
                /* set */ tokenType = 9; // TokenType.RETURN
                goto END;
            }

            if (__bool_check(strEquals(tokenBuffer_ptr, getKeyword_underscoreSymbol())))
            {
                /* set */ tokenType = 18; // TokenType.UNDERSCORE
                goto END;
            }

            if (__bool_check(strEquals(tokenBuffer_ptr, getBuiltin_boolCheck())))
            {
                /* set */ tokenType = 20; // TokenType.BOOL_CHECK
                goto END;
            }

            if (__bool_check(strEquals(tokenBuffer_ptr, getMacroName_int_max())))
            {
                _ = replaceMacro_int_max(Lexer_getTokenStrBuff_ptr());
                /* set */ tokenType = 1; // TokenType.INTEGER_LITERAL
                goto END;
            }

            if (__bool_check(strEquals(tokenBuffer_ptr, getMacroName_int_width_bits())))
            {
                _ = replaceMacro_int_width_bits(Lexer_getTokenStrBuff_ptr());
                /* set */ tokenType = 1; // TokenType.INTEGER_LITERAL
                goto END;
            }

            if (__bool_check(strEquals(tokenBuffer_ptr, getMacroName_int_width_bytes())))
            {
                _ = replaceMacro_int_width_bytes(Lexer_getTokenStrBuff_ptr());
                /* set */ tokenType = 1; // TokenType.INTEGER_LITERAL
                goto END;
            }

            if (__bool_check(strEquals(tokenBuffer_ptr, getMacroName_int_sign_bit_mask())))
            {
                _ = replaceMacro_int_sign_bit_mask(Lexer_getTokenStrBuff_ptr());
                /* set */ tokenType = 1; // TokenType.INTEGER_LITERAL
                goto END;
            }

            /* set */ tokenType = 0; // TokenType.IDENTIFIER
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
                    _ = memWrite8(_add(tokenBuffer_ptr, readCount), 0); // s_tokenStrBuff[0] = '\0'
                    goto END;
                }
                /* set */ pos = Lexer_incPos();
                /* set */ c = memRead8(_add(srcTxt_ptr, pos));
                if (__bool_check(_or(_eq(c, 0), _gte(pos, 131072)))) // '\0' || >= 128 KB
                {
                    /* set */ tokenType = 21; // TokenType.EOF_TOKEN
                    _ = memWrite8(tokenBuffer_ptr, 0); // s_tokenStrBuff[0] = '\0'
                    goto END;
                }
                goto LOOP_DIGIT;
            }

            /* set */ tokenType = 1; // TokenType.INTEGER_LITERAL
            goto END;
        }

        if (__bool_check(_eq(c, 40))) // '('
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 10; // TokenType.L_PAREN
            goto END;
        }
        if (__bool_check(_eq(c, 41))) // ')'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 11; // TokenType.R_PAREN
            goto END;
        }
        if (__bool_check(_eq(c, 123))) // '{'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 12; // TokenType.L_BRACE
            goto END;
        }
        if (__bool_check(_eq(c, 125))) // '}'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 13; // TokenType.R_BRACE
            goto END;
        }
        if (__bool_check(_eq(c, 59))) // ';'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 14; // TokenType.SEMICOLON
            goto END;
        }
        if (__bool_check(_eq(c, 44))) // ','
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 16; // TokenType.COMMA
            goto END;
        }
        if (__bool_check(_eq(c, 58))) // ':'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 15; // TokenType.COLON
            goto END;
        }
        if (__bool_check(_eq(c, 61))) // '='
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ tokenType = 17; // TokenType.EQUALS
            goto END;
        }
        if (__bool_check(_eq(c, 45))) // '-'
        {
            /* set */ pos = Lexer_incPos();
            _ = Lexer_tokenBufferAppendChar(c);
            /* set */ c = memRead8(_add(srcTxt_ptr, pos));
            if (__bool_check(_and(_neq(c, 0), _lt(pos, 131072)))) // != '\0' && < 128 KB
            {
                if (__bool_check(_eq(c, 62))) // '>'
                {
                    /* set */ pos = Lexer_incPos();
                    _ = Lexer_tokenBufferAppendChar(c);
                    /* set */ tokenType = 19; // TokenType.TRAILING_RETURN
                    goto END;
                }
                else
                {
                    /* set */ tokenType = 100; // TokenType.UNKNOWN
                    goto END;
                }
            }
        }

        /* set */ tokenType = 100; // TokenType.UNKNOWN
        _ = Lexer_tokenBufferAppendChar(c);

    END:
        _ = setCurToken(tokenType, Lexer_getTokenStrBuff_ptr(), line);
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

    public static int parseCompilationUnit()
    //func parseCompilationUnit() -> Int
    {
        var g_offset = (int)default;
        var result = (int)default;

        /* set */ result = 1;

        _ = skipUTF8BOMMark(); // Skips UTF8 BOM mark, if any

    #if false
        while (getNextToken() < 21)
        {
            _ = printMessage($"{getCurTokenType()} {Program.ReadString(getCurTokenValue_ptr(), 128)}");
        }
    #endif

        _ = consumeToken(); // Initialize s_curToken

    LOOP_USING:
        if (__bool_check(notMatchTokenType(2))) // TokenType.USING
        {
            goto END_USING;
        }

        _ = consumeToken();
        _ = resetFunctionScope();
        if (__bool_check(_neq(parseUsingDirective(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        goto LOOP_USING;

    END_USING:
        _ = memWrite(_add(g_offset, 180), 1); // s_inFunctionScope = 1

    LOOP_FUNCTIONS:
        if (__bool_check(notMatchTokenType(3))) // TokenType.FUNC
        {
            goto END_FUNCTIONS;
        }

        _ = consumeToken();
        _ = resetFunctionScope();
        if (__bool_check(_neq(parseFunctionDeclaration(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        goto LOOP_FUNCTIONS;

    END_FUNCTIONS:
        if (__bool_check(notMatchTokenType(21))) // TokenType.EOF
        {
            //if (__bool_check(_lt(Lexer_getPos(), strLen(Lexer_getSrcCodeTxt_ptr()))))
            //{
            //_ = printError($"Unexpected token at line {getCurTokenLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            //}
            //else
            //{
            //_ = printError($"Unexpected token at end of file. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            //}

            _ = printError($"Unexpected token at line {getCurTokenLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");

            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(anyReferencedFunctionNotDefined()))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(_neq(isMainFunctionValid(), 1)))
        {
            _ = printError($"Entry point is missing. Please provide a 'main()' function implementation.");
            /* set */ result = 0;
            goto END;
        }

    END:
        return result;
    }

    public static int parseUsingDirective()
    //func parseUsingDirective() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(3))) // TokenType.FUNC
        {
            _ = printError($"Expected 'func' after 'using' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(_neq(parseFunctionHeader(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(notMatchTokenType(14))) // TokenType.SEMICOLON
        {
            _ = printError($"Expected ';' after using directive at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

    END:
        return result;
    }

    public static int parseFunctionDeclaration()
    //func parseFunctionDeclaration() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(_neq(parseFunctionHeader(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(_neq(parseFunctionBody(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(anyUnresolvedGoto()))
        {
            /* set */ result = 0;
            goto END;
        }

    END:
        return result;
    }

    public static int parseFunctionHeader()
    //func parseFunctionHeader() -> Int
    {
        var result = (int)default;
        var wasForwardDeclared = (int)default;
        var funcName = (int)default;
        var defLine = (int)default;
        var tmpStr = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected valid function name after 'func' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }

        /* set */ wasForwardDeclared = 0;
        if (__bool_check(_neq(isInFunctionScope(), 1)))
        {
            _ = insertFunctionSymbol(getCurTokenValue_ptr());
        }
        else
        {
            if (__bool_check(_neq(insertFunctionSymbol(getCurTokenValue_ptr()), 1)))
            {
                /* set */ wasForwardDeclared = 1;
            }
            if (__bool_check(isFunctionSymbolDefinitionFound(getCurTokenValue_ptr())))
            {
                /* set */ result = 0;
                goto END;
            }
            _ = markFunctionSymbolDefinitionFound(getCurTokenValue_ptr(), Lexer_getLastValidLine());
        }
        /* set */ funcName = memRead(getFunctionSymbol_ptr(getCurTokenValue_ptr()));
        /* set */ defLine = Lexer_getLastValidLine();
        _ = setParamCount(0);
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(10))) // TokenType.L_PAREN
        {
            _ = printError($"Expected '(' after function name at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(11))) // TokenType.R_PAREN
        {
            if (__bool_check(_neq(parseFunctionParameterList(), 1)))
            {
                /* set */ result = 0;
                goto END;
            }
        }

        if (__bool_check(notMatchTokenType(11))) // TokenType.R_PAREN
        {
            _ = printError($"Expected ')' after parameter list at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(19))) // TokenType.TRAILING_RETURN
        {
            _ = printError($"Expected '->' after ')' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected type after '->' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        if (__bool_check(_neq(containsTypeNameSymbol(getCurTokenValue_ptr()), 1)))
        {
            _ = printError($"Undefined type name symbol at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ tmpStr = getTypeName_Int();
            _ = memWrite8(tmpStr, 105); // 'i'
            if (__bool_check(strEquals(getCurTokenValue_ptr(), tmpStr)))
            {
                _ = printError($"Did you mean 'Int'?");
            }
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        // s_inFunctionScope && wasForwardDeclared
        if (__bool_check(_and(isInFunctionScope(), wasForwardDeclared)))
        {
            if (__bool_check(_neq(matchFunctionSymbolParamCount(funcName, getParamCount()), 1)))
            {
                _ = printError($"Definition of function '{Program.ReadString(funcName, 128)}' at line {defLine} with different number of paramaters from declaration.");
                /* set */ result = 0;
                goto END;
            }
        }
        _ = updateFunctionSymbolParamCount(funcName, getParamCount());
        if (__bool_check(isInFunctionScope()))
        {
            _ = IL_Emitter_emit_methodHeader(funcName, getParamCount());
        }

    END:
        return result;
    }

    public static int parseFunctionParameterList()
    //func parseFunctionParameterList() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(_neq(parseFunctionParameter(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }
        _ = incParamCount();

    LOOP:
        if (__bool_check(notMatchTokenType(16))) // TokenType.COMMA
        {
            goto END;
        }

        _ = consumeToken();
        if (__bool_check(_neq(parseFunctionParameter(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }
        _ = incParamCount();

        goto LOOP;

    END:
        return result;
    }

    public static int parseFunctionParameter()
    //func parseFunctionParameter() -> Int
    {
        var result = (int)default;
        var tmpStr = (int)default;

        /* set */ result = 1;


        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected identifier in parameters at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(isInFunctionScope()))
        {
            if (__bool_check(_neq(insertFunctionScopeSymbol(getCurTokenValue_ptr(), 1, getParamCount(), Lexer_getLastValidLine()), 1)))
            {
                /* set */ result = 0;
                goto END;
            }
            _ = markFunctionScopeSymbolDefinitionFound(getCurTokenValue_ptr(), Lexer_getLastValidLine());
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(15))) // TokenType.COLON
        {
            _ = printError($"Expected ':' after identifier at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected type after ':' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        if (__bool_check(_neq(containsTypeNameSymbol(getCurTokenValue_ptr()), 1)))
        {
            _ = printError($"Undefined type name symbol at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ tmpStr = getTypeName_Int();
            _ = memWrite8(tmpStr, 105); // 'i'
            if (__bool_check(strEquals(getCurTokenValue_ptr(), tmpStr)))
            {
                _ = printError($"Did you mean 'Int'?");
            }
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

    END:
        return result;
    }

    public static int parseFunctionBody()
    //func parseFunctionBody() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(12))) // TokenType.L_BRACE
        {
            _ = printError($"Expected '{{' after function header at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = IL_Emitter_emit_methodBodyBegin();
        _ = consumeToken();

        _ = setParamCount(0);

    LOOP_LOCAL_VARS:
        if (__bool_check(notMatchTokenType(4))) // TokenType.VAR
        {
            goto END_LOCAL_VARS;
        }

        _ = consumeToken();
        if (__bool_check(_neq(parseLocalVariableDeclaration(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }
        _ = incParamCount();

        goto LOOP_LOCAL_VARS;

    END_LOCAL_VARS:
        if (__bool_check(_gt(getParamCount(), 0)))
        {
            _ = IL_Emitter_emit_initLocals(getParamCount());
        }

    LOOP_STATEMENTS:
        if (__bool_check(matchTokenType(9))) // TokenType.RETURN
        {
            goto RETURN_STATEMENT;
        }

        if (__bool_check(_neq(parseStatement(), 1)))
        {
            if (__bool_check(matchTokenType(13))) // TokenType.R_BRACE
            {
                _ = printError($"Expected return statement at line {getCurTokenLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            }
            /* set */ result = 0;
            goto END;
        }

        goto LOOP_STATEMENTS;

    RETURN_STATEMENT:
        if (__bool_check(_neq(parseReturnStatement(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(notMatchTokenType(13))) // TokenType.R_BRACE
        {
            _ = printError($"Expected '}}' after return statement at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = IL_Emitter_emit_methodBodyEnd();
        _ = consumeToken();

    END:
        return result;
    }

    public static int parseLocalVariableDeclaration()
    //func parseLocalVariableDeclaration() -> Int
    {
        var result = (int)default;
        var tmpStr = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected identifier after 'var' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(isInFunctionScope()))
        {
            if (__bool_check(_neq(insertFunctionScopeSymbol(getCurTokenValue_ptr(), 2, getParamCount(), Lexer_getLastValidLine()), 1)))
            {
                /* set */ result = 0;
                goto END;
            }
            _ = markFunctionScopeSymbolDefinitionFound(getCurTokenValue_ptr(), Lexer_getLastValidLine());
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(15))) // TokenType.COLON
        {
            _ = printError($"Expected ':' after identifier at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected type after ':' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        if (__bool_check(_neq(containsTypeNameSymbol(getCurTokenValue_ptr()), 1)))
        {
            _ = printError($"Undefined type name symbol at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ tmpStr = getTypeName_Int();
            _ = memWrite8(tmpStr, 105); // 'i'
            if (__bool_check(strEquals(getCurTokenValue_ptr(), tmpStr)))
            {
                _ = printError($"Did you mean 'Int'?");
            }
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(14))) // TokenType.SEMICOLON
        {
            _ = printError($"Expected ';' after type at at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();


    END:
        return result;
    }

    public static int parseStatement()
    //func parseStatement() -> Int
    {
        var result = (int)default;
        var curIdentifier = (int)default;

        /* set */ result = 1;

        if (__bool_check(matchTokenType(5))) // TokenType.SET
        {
            _ = consumeToken();
            if (__bool_check(_neq(parseSetStatement(), 1)))
            {
                /* set */ result = 0;
            }
            goto END;
        }

        if (__bool_check(matchTokenType(6))) // TokenType.IF
        {
            _ = consumeToken();
            if (__bool_check(_neq(parseIfStatement(), 1)))
            {
                /* set */ result = 0;
            }
            goto END;
        }

        if (__bool_check(matchTokenType(8))) // TokenType.GOTO
        {
            _ = consumeToken();
            if (__bool_check(_neq(parseGotoStatement(), 1)))
            {
                /* set */ result = 0;
            }
            goto END;
        }

        if (__bool_check(matchTokenType(18))) // TokenType.UNDERSCORE
        {
            _ = consumeToken();
            if (__bool_check(_neq(parseDiscardStatement(), 1)))
            {
                /* set */ result = 0;
            }
            goto END;
        }

        if (__bool_check(matchTokenType(0))) // TokenType.IDENTIFIER
        {
            /* set */ curIdentifier = _add(getTmpBuffer_ptr(), 128);
            _ = strCpy(getCurTokenValue_ptr(), curIdentifier);
            _ = consumeToken();
            if (__bool_check(matchTokenType(15))) // TokenType.COLON
            {
                _ = consumeToken();
                if (__bool_check(_neq(parseLabelStatement(), 1)))
                {
                    /* set */ result = 0;
                    goto END;
                }

                if (__bool_check(containsFunctionScopeSymbol(curIdentifier)))
                {
                    if (__bool_check(_neq(matchFunctionScopeSymbolType(curIdentifier, 3), 1))) // SymbolType.LABEL
                    {
                        _ = printError($"Identifier '{Program.ReadString(curIdentifier, 128)}' at line {Lexer_getLastValidLine()} is already defined");
                        /* set */ result = 0;
                        goto END;
                    }
                    if (__bool_check(isFunctionScopeSymbolDefinitionFound(curIdentifier)))
                    {
                        /* set */ curIdentifier = memRead(getFunctionScopeSymbol_ptr(curIdentifier));
                        _ = printError($"Duplicate definition for '{Program.ReadString(curIdentifier, 128)}' at line {Lexer_getLastValidLine()}");
                        _ = printError($"First definition: line {memRead(symbol_getField_ptr(curIdentifier, 2))}");
                        /* set */ result = 0;
                        goto END;
                    }
                    _ = updateFunctionScopeSymbolScopeDepth(curIdentifier, getCurScopeDepth());
                }
                else
                {
                    _ = insertFunctionScopeSymbol(curIdentifier, 3, getCurScopeDepth(), Lexer_getLastValidLine());
                }
                _ = markFunctionScopeSymbolDefinitionFound(curIdentifier, Lexer_getLastValidLine());
                _ = IL_Emitter_emit_label(curIdentifier);
                goto END;
            }
            else
            {
                _ = printError($"Unexpected token at line {getCurTokenLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
                /* set */ result = 0;
                goto END;
            }
        }

        _ = printError($"Unexpected token at line {getCurTokenLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
        /* set */ result = 0;

    END:
        return result;
    }

    public static int parseLabelStatement()
    //func parseLabelStatement() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        return result;
    }

    public static int parseGotoStatement()
    //func parseGotoStatement() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected identifier after 'goto' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(_neq(containsFunctionScopeSymbol(getCurTokenValue_ptr()), 1)))
        {
            _ = insertUnresolvedGotoSymbol(getCurTokenValue_ptr(), getCurScopeDepth(), Lexer_getLastValidLine());
        }
        else { if (__bool_check(_neq(matchFunctionScopeSymbolType(getCurTokenValue_ptr(), 3), 1))) // SymbolType.LABEL
        {
            _ = printError($"Identifier '{Program.ReadString(getCurTokenValue_ptr(), 128)}' at line {Lexer_getLastValidLine()} is already defined.");
            /* set */ result = 0;
            goto END;
        }}
        if (__bool_check(_gt(memRead(symbol_getField_ptr(getFunctionScopeSymbol_ptr(getCurTokenValue_ptr()), 3)), getCurScopeDepth())))
        {
            _ = printError($"Label '{Program.ReadString(getCurTokenValue_ptr(), 128)}' is not available at current scope at line {Lexer_getLastValidLine()}.");
            /* set */ result = 0;
            goto END;
        }
        _ = IL_Emitter_emit_goto(getCurTokenValue_ptr());
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(14))) // TokenType.SEMICOLON
        {
            _ = printError($"Expected ';' after identifier at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

    END:
        return result;
    }

    public static int parseSetStatement()
    //func parseSetStatement() -> Int
    {
        var result = (int)default;
        var varName = (int)default;
        var symbol = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected identifier after 'set' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(_neq(containsFunctionScopeSymbol(getCurTokenValue_ptr()), 1)))
        {
            _ = printError($"Undefined symbol '{Program.ReadString(getCurTokenValue_ptr(), 128)}' at line {Lexer_getLastValidLine()}");
            /* set */ result = 0;
            goto END;
        }

        // Needs either SymbolType.LOCAL_VARIABLE or SymbolType.ARGUMENT_VARIABLE
        if (__bool_check(_and(_neq(matchFunctionScopeSymbolType(getCurTokenValue_ptr(), 2), 1), _neq(matchFunctionScopeSymbolType(getCurTokenValue_ptr(), 1), 1))))
        {
            _ = printError($"Expected variable symbol after 'set' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        /* set */ varName = _add(getTmpBuffer_ptr(), 128); // s_tmpBuff2_ptr
        _ = strCpy(getCurTokenValue_ptr(), varName);
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(17))) // TokenType.EQUALS
        {
            _ = printError($"Expected '=' after identifier at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        _ = setParamCount(0);
        if (__bool_check(_neq(parsePrimaryExpression(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(notMatchTokenType(14))) // TokenType.SEMICOLON
        {
            _ = printError($"Expected ';' after expression at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        /* set */ symbol = getFunctionScopeSymbol_ptr(varName);
        if (__bool_check(_eq(memRead(symbol_getField_ptr(symbol, 1)), 1))) // SymbolType.ARGUMENT_VARIABLE
        {
            _ = IL_Emitter_emit_storeArgVar(memRead(symbol_getField_ptr(symbol, 3)));
            goto END;
        }
        if (__bool_check(_eq(memRead(symbol_getField_ptr(symbol, 1)), 2))) // SymbolType.LOCAL_VARIABLE
        {
            _ = IL_Emitter_emit_storeLocalVar(memRead(symbol_getField_ptr(symbol, 3)));
            goto END;
        }

    END:
        return result;
    }

    public static int parseIfStatement()
    //func parseIfStatement() -> Int
    {
        var result = (int)default;
        var curIfLabelCount = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(10))) // TokenType.L_PAREN
        {
            _ = printError($"Expected '(' after 'if' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(_neq(parseBooleanExpression(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(notMatchTokenType(11))) // TokenType.R_PAREN
        {
            _ = printError($"Expected ')' after boolean expression at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();
        /* set */ curIfLabelCount = getIfLabelCounter();
        _ = incIfLabelCounter();
        _ = IL_Emitter_emit_ifBegin(curIfLabelCount);

        if (__bool_check(notMatchTokenType(12))) // TokenType.L_BRACE
        {
            _ = printError($"Expected '{{' after ')' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        _ = incCurScopeDepth();
    LOOP:
        if (__bool_check(matchTokenType(13))) // TokenType.R_BRACE
        {
            goto SCOPE_END;
        }

        if (__bool_check(_neq(parseStatement(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        goto LOOP;

    SCOPE_END:
        if (__bool_check(notMatchTokenType(13))) // TokenType.R_BRACE
        {
            _ = printError($"Expected '}}' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        _ = decCurScopeDepth();

        if (__bool_check(matchTokenType(7))) // TokenType.ELSE
        {
            _ = IL_Emitter_emit_else(1, curIfLabelCount);
            _ = consumeToken();
            if (__bool_check(_neq(parseElseStatement(), 1)))
            {
                /* set */ result = 0;
                goto END;
            }
        }
        else
        {
            _ = IL_Emitter_emit_else(0, curIfLabelCount);
        }

        _ = IL_Emitter_emit_ifEnd(curIfLabelCount);

    END:
        return result;
    }

    public static int parseElseStatement()
    //func parseElseStatement() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(12))) // TokenType.L_BRACE
        {
            _ = printError($"Expected '{{' after ')' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        _ = incCurScopeDepth();
    LOOP:
        if (__bool_check(matchTokenType(13))) // TokenType.R_BRACE
        {
            goto SCOPE_END;
        }

        if (__bool_check(_neq(parseStatement(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        goto LOOP;

    SCOPE_END:
        if (__bool_check(notMatchTokenType(13))) // TokenType.R_BRACE
        {
            _ = printError($"Expected '}}' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        _ = decCurScopeDepth();

    END:
        return result;
    }

    public static int parseDiscardStatement()
    //func parseDiscardStatement() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(17))) // TokenType.EQUALS
        {
            _ = printError($"Expected '=' after '_' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        _ = setParamCount(0);
        if (__bool_check(_neq(parseFunctionInvocation(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(notMatchTokenType(14))) // TokenType.SEMICOLON
        {
            _ = printError($"Expected ';' after function invocation at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();
        _ = IL_Emitter_emit_discardReturn();

    END:
        return result;
    }

    public static int parseReturnStatement()
    //func parseReturnStatement() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(9))) // TokenType.RETURN
        {
            _ = printError($"Expected 'return' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        _ = setParamCount(0);
        if (__bool_check(_neq(parsePrimaryExpression(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(notMatchTokenType(14))) // TokenType.SEMICOLON
        {
            _ = printError($"Expected ';' after expression at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();
        _ = IL_Emitter_emit_return();

    END:
        return result;
    }

    public static int parsePrimaryExpression()
    //func parsePrimaryExpression() -> Int
    {
        var result = (int)default;
        var curIdentifierStr = (int)default;
        var oldParamCount = (int)default;
        var symbol = (int)default;

        /* set */ result = 1;

        if (__bool_check(matchTokenType(1))) // TokenType.INTEGER_LITERAL
        {
            if (long.Parse(Program.ReadString(Lexer_getTokenStrBuff_ptr(), 128)) > 2147483647)
            {
                _ = printError($"Integer overflow at line {Lexer_getLastValidLine()}: '{Program.ReadString(Lexer_getTokenStrBuff_ptr(), 128)}' exceeds max allowed value {2147483647}.");
                /* set */ result = 0;
                goto END;
            }
            _ = IL_Emitter_emit_loadConst(getCurTokenValue_ptr());
            _ = consumeToken();
            goto END;
        }

        if (__bool_check(matchTokenType(0))) // TokenType.IDENTIFIER
        {
            /* set */ curIdentifierStr = _add(getTmpBuffer_ptr(), 128); // s_tmpBuff2_ptr
            _ = strCpy(Lexer_getTokenStrBuff_ptr(), curIdentifierStr);
            _ = consumeToken();
            if (__bool_check(matchTokenType(10))) // TokenType.L_PAREN
            {
                if (__bool_check(_neq(containsFunctionSymbol(curIdentifierStr), 1)))
                {
                    _ = printError($"Undefined function '{Program.ReadString(curIdentifierStr, 128)}' at line {Lexer_getLastValidLine()}.");
                    /* set */ result = 0;
                    goto END;
                }
                /* set */ curIdentifierStr = memRead(getFunctionSymbol_ptr(curIdentifierStr));
                _ = markFunctionSymbolAsReferenced(curIdentifierStr, Lexer_getLastValidLine());
                _ = consumeToken();
                /* set */ oldParamCount = getParamCount();
                _ = setParamCount(0);
                if (__bool_check(_neq(parseFunctionInvocationTail(), 1)))
                {
                    /* set */ result = 0;
                    goto END;
                }
                if (__bool_check(_neq(matchFunctionSymbolParamCount(curIdentifierStr, getParamCount()), 1)))
                {
                    _ = printError($"Invalid number of arguments for function '{Program.ReadString(curIdentifierStr, 128)}' at line {Lexer_getLastValidLine()}.");
                    /* set */ result = 0;
                    goto END;
                }
                _ = IL_Emitter_emit_call(curIdentifierStr, getParamCount());
                _ = setParamCount(oldParamCount);

                goto END;
            }
            else
            {
                if (__bool_check(_neq(containsFunctionScopeSymbol(curIdentifierStr), 1)))
                {
                    _ = printError($"Undefined symbol '{Program.ReadString(curIdentifierStr, 128)}' at line {Lexer_getLastValidLine()}.");
                    /* set */ result = 0;
                    goto END;
                }
                /* set */ symbol = getFunctionScopeSymbol_ptr(curIdentifierStr);
                if (__bool_check(_eq(memRead(symbol_getField_ptr(symbol, 1)), 1))) // SymbolType.ARGUMENT_VARIABLE
                {
                    _ = IL_Emitter_emit_loadArgVar(memRead(symbol_getField_ptr(symbol, 3))); // symbol.scopeDepth
                    goto END;
                }
                if (__bool_check(_eq(memRead(symbol_getField_ptr(symbol, 1)), 2))) // SymbolType.LOCAL_VARIABLE
                {
                    _ = IL_Emitter_emit_loadLocalVar(memRead(symbol_getField_ptr(symbol, 3))); // symbol.scopeDepth
                    goto END;
                }
            }
        }
        else
        {
            _ = printError($"Expected expression at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
        }

    END:
        return result;
    }

    public static int parseBooleanExpression()
    //func parseBooleanExpression() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(20))) // TokenType.BOOL_CHECK
        {
            _ = printError($"Expected boolean expression after '(' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(10))) // TokenType.L_PAREN
        {
            _ = printError($"Expected boolean expression after '__bool_check' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        _ = setParamCount(0);
        if (__bool_check(_neq(parsePrimaryExpression(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(notMatchTokenType(11))) // TokenType.R_PAREN
        {
            _ = printError($"Expected ')' after expression at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();
        _ = IL_Emitter_emit_boolCheck();

    END:
        return result;
    }

    public static int parseFunctionInvocation()
    //func parseFunctionInvocation() -> Int
    {
        var result = (int)default;
        var funcName = (int)default;
        var oldParamCount = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(0))) // TokenType.IDENTIFIER
        {
            _ = printError($"Expected identifier after '=' at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        /* set */ funcName = _add(getTmpBuffer_ptr(), 128); // s_tmpBuff2_ptr
        _ = strCpy(getCurTokenValue_ptr(), funcName);
        if (__bool_check(_neq(containsFunctionSymbol(funcName), 1)))
        {
            _ = printError($"Undefined function '{Program.ReadString(funcName, 128)}' at line {Lexer_getLastValidLine()}.");
            /* set */ result = 0;
            goto END;
        }
        /* set */ funcName = memRead(getFunctionSymbol_ptr(funcName));
        _ = markFunctionSymbolAsReferenced(funcName, Lexer_getLastValidLine());
        _ = consumeToken();

        if (__bool_check(notMatchTokenType(10))) // TokenType.L_PAREN
        {
            _ = printError($"Expected '(' after identifier at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

        /* set */ oldParamCount = getParamCount();
        _ = setParamCount(0);
        if (__bool_check(_neq(parseFunctionInvocationTail(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }

        if (__bool_check(_neq(matchFunctionSymbolParamCount(funcName, getParamCount()), 1)))
        {
            _ = printError($"Invalid number of arguments for function '{Program.ReadString(funcName, 128)}' at line {Lexer_getLastValidLine()}.");
            /* set */ result = 0;
            goto END;
        }

        _ = IL_Emitter_emit_call(funcName, getParamCount());
        _ = setParamCount(oldParamCount);

    END:
        return result;
    }

    public static int parseFunctionInvocationTail()
    //func parseFunctionInvocationTail() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(notMatchTokenType(11))) // TokenType.R_PAREN
        {
            if (__bool_check(_neq(parseFunctionArgumentList(), 1)))
            {
                /* set */ result = 0;
                goto END;
            }
        }

        if (__bool_check(notMatchTokenType(11))) // TokenType.R_PAREN
        {
            _ = printError($"Expected ')' after argument list at line {Lexer_getLastValidLine()}. Found '{Program.ReadString(getCurTokenValue_ptr(), 128)}'");
            /* set */ result = 0;
            goto END;
        }
        _ = consumeToken();

    END:
        return result;
    }

    public static int parseFunctionArgumentList()
    //func parseFunctionArgumentList() -> Int
    {
        var result = (int)default;

        /* set */ result = 1;

        if (__bool_check(_neq(parseFunctionArgument(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }
        _ = incParamCount();

    LOOP:
        if (__bool_check(notMatchTokenType(16))) // TokenType.COMMA
        {
            goto END;
        }

        _ = consumeToken();
        if (__bool_check(_neq(parseFunctionArgument(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }
        _ = incParamCount();

        goto LOOP;

    END:
        return result;
    }

    public static int parseFunctionArgument()
    //func parseFunctionArgument() -> Int
    {
        var result = (int)default;
        var oldParamCount = (int)default;

        /* set */ result = 1;

        /* set */ oldParamCount = getParamCount();
        _ = setParamCount(0);
        if (__bool_check(_neq(parsePrimaryExpression(), 1)))
        {
            /* set */ result = 0;
            goto END;
        }
        _ = setParamCount(oldParamCount);

    END:
        return result;
    }

    public static int parse()
    //func parse() -> Int
    {
        var g_offset = (int)default;
        var result = (int)default;

        /* set */ result = IL_Emitter_initEmitter(memRead(_add(g_offset, 64)));
        if (__bool_check(_eq(result, 0)))
        {
            _ = printError($"Failed initEmitter");
            goto END;
        }

        /* set */ result = parseCompilationUnit();
        if (__bool_check(_neq(result, 1)))
        {
            _ = printError("Failed to parse program");
            /* set */ result = Lexer_getLastValidLine();
            goto END;
        }
        else
        {
            /* set */ result = IL_Emitter_finishEmitter();
            if (__bool_check(_eq(result, 0)))
            {
                _ = printError($"Failed finishEmitter");
                /* set */ result = Lexer_getLastValidLine();
            }
            else
            {
                _ = printMessage("Program parsed successfully");
                /* set */ result = 0;
            }
        }

    END:
        return result;
    }

    #endregion Parser

    #region Initialization

    public static int initGlobals()
    //func initGlobals() -> Int
    {
        var g_offset = (int)default;
        var ramBinLengthMB = (int)default;
        var isDebugConfig = (int)default;

        /* set */ g_offset = 0;

        // Store input data
        /* set */ ramBinLengthMB = memRead(_add(g_offset, 64)); // s_ramBinLength_MB
        /* set */ isDebugConfig = memRead(_add(g_offset, 68)); // s_isDebugConfig

        // Clean globals data arena - 2 KB len
        _ = memSet(g_offset, 0, 2048);

        // Restore input data
        _ = memWrite(_add(g_offset, 64), ramBinLengthMB); // s_ramBinLength_MB
        _ = memWrite(_add(g_offset, 68), isDebugConfig); // s_isDebugConfig

        // Clean symbol tables arena - 98 + 388 => 488 KB len
        _ = memSet(_add(g_offset, 2048), 0, 497664);

        // Write hardcoded pointers
        _ = memWrite(_add(g_offset, 72), _add(g_offset, 576)); // s_tmpBuff_ptr

        _ = memWrite(_add(g_offset, 124), _add(g_offset, 320)); // s_curToken_value_ptr
        _ = memWrite(_add(g_offset, 140), _add(g_offset, 448)); // s_tokenStrBuff_ptr

        _ = memWrite(_add(g_offset, 160), _add(g_offset, 524288)); // s_srcCodeTxt_ptr

        _ = memWrite(_add(g_offset, 200), _add(g_offset, 2048)); // s_functionsTbl_ptr
        _ = memWrite(_add(g_offset, 208), _add(g_offset, 102400)); // s_functionsTbl_names_ptr
        _ = memWrite(_add(g_offset, 216), _add(g_offset, 18432)); // s_functionScopeTbl_ptr
        _ = memWrite(_add(g_offset, 224), _add(g_offset, 167936)); // s_functionScopeTbl_names_ptr
        _ = memWrite(_add(g_offset, 232), _add(g_offset, 83968)); // s_unresolvedGotoTbl_ptr
        _ = memWrite(_add(g_offset, 240), _add(g_offset, 430080)); // s_unresolvedGotoTbl_names_ptr
        _ = memWrite(_add(g_offset, 248), _add(g_offset, 100352)); // s_typeNamesTbl_ptr
        _ = memWrite(_add(g_offset, 256), _add(g_offset, 495616)); // s_typeNamesTbl_names_ptr

        _ = memWrite(_add(g_offset, 280), _add(g_offset, 655360)); // s_IL_Emitter_strBuffer_ptr
        _ = memWrite(_add(g_offset, 288), _add(g_offset, 499712)); // s_IL_Emitter_baseILBeginTxt_ptr
        _ = memWrite(_add(g_offset, 292), _add(g_offset, 509952)); // s_IL_Emitter_baseILEndTxt_ptr
        _ = memWrite(_add(g_offset, 296), _add(g_offset, 510976)); // s_IL_Emitter_RuntimeJsonTxt_ptr

        return 0;
    }

    public static int initParser()
    //func initParser() -> Int
    {
        var g_offset = (int)default;

        /* set */ g_offset = 0;

        _ = memWrite(getTmpBuffer_ptr(), 0); // s_tmpBuff[0] = '\0'

        _ = memWrite(_add(g_offset, 120), 100); // s_curToken_type = TokenType.UNKNOWN
        _ = memWrite(getCurTokenValue_ptr(), 0); // s_curToken_value[0] = '\0'
        _ = memWrite(_add(g_offset, 128), 1); // s_curToken_line = 1

        _ = memWrite(Lexer_getTokenStrBuff_ptr(), 0); // s_tokenStrBuff[0] = '\0'
        _ = memWrite(_add(g_offset, 144), 0); // s_tokenStrBuff_pos = 0

        _ = memWrite(_add(g_offset, 164), 0); // s_pos = 0
        _ = memWrite(_add(g_offset, 168), 1); // s_line = 1
        _ = memWrite(_add(g_offset, 172), 1); // s_lastValidLine = 1
        _ = memWrite(_add(g_offset, 176), 0); // s_curScopeDepth = 0
        _ = memWrite(_add(g_offset, 180), 0); // s_inFunctionScope = 0
        _ = memWrite(_add(g_offset, 184), 0); // s_paramCount = 0
        _ = memWrite(_add(g_offset, 188), 0); // s_ifLabelCounter = 0

        _ = memWrite(_add(g_offset, 192), _mul(8, __INT_WIDTH_BYTES__)); // s_symbol_sizeof
        _ = memWrite(_add(g_offset, 196), __INT_WIDTH_BYTES__); // s_symbol_field_size

        _ = memWrite(_add(g_offset, 204), 0); // s_functionsTbl_count = 0
        _ = memWrite(_add(g_offset, 212), 0); // s_functionsTbl_names_offset = 0
        _ = memWrite(_add(g_offset, 220), 0); // s_functionScopeTbl_count = 0
        _ = memWrite(_add(g_offset, 228), 0); // s_functionScopeTbl_names_offset = 0
        _ = memWrite(_add(g_offset, 236), 0); // s_unresolvedGotoTbl_count = 0
        _ = memWrite(_add(g_offset, 244), 0); // s_unresolvedGotoTbl_names_offset = 0
        _ = memWrite(_add(g_offset, 252), 0); // s_typeNamesTbl_count = 0
        _ = memWrite(_add(g_offset, 260), 0); // s_typeNamesTbl_names_offset = 0

        _ = insertFunctionSymbol(getBuiltin_add());
        _ = insertFunctionSymbol(getBuiltin_nand());
        _ = insertFunctionSymbol(getBuiltin_mem_read());
        _ = insertFunctionSymbol(getBuiltin_mem_write());

        _ = markFunctionSymbolDefinitionFound(getBuiltin_add(), 0);
        _ = markFunctionSymbolDefinitionFound(getBuiltin_nand(), 0);
        _ = markFunctionSymbolDefinitionFound(getBuiltin_mem_read(), 0);
        _ = markFunctionSymbolDefinitionFound(getBuiltin_mem_write(), 0);

        _ = updateFunctionSymbolParamCount(getBuiltin_add(), 2);
        _ = updateFunctionSymbolParamCount(getBuiltin_nand(), 2);
        _ = updateFunctionSymbolParamCount(getBuiltin_mem_read(), 1);
        _ = updateFunctionSymbolParamCount(getBuiltin_mem_write(), 2);

        _ = insertTypeNameSymbol(getTypeName_Int(), 0);

        // s_isDebugConfig
        if (__bool_check(_eq(memRead(_add(g_offset, 64)), 1)))
        {
            _ = insertFunctionSymbol(getBuiltin_dbg_int());
            _ = markFunctionSymbolDefinitionFound(getBuiltin_dbg_int(), 0);
            _ = updateFunctionSymbolParamCount(getBuiltin_dbg_int(), 1);
        }

        return 0;
    }

    #endregion Initialization

    public static int main()
    //func main() -> Int
    {
        var lineError = (int)default;

        _ = initGlobals();
        _ = initParser();

        /* set */ lineError = parse();

        return lineError;
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
