using System;
using System.Collections.Generic;
using System.Text;

enum TokenType
{
    IDENTIFIER,
    INTEGER_LITERAL,

    USING,
    FUNC,
    VAR,
    SET,
    IF,
    ELSE,
    GOTO,
    RETURN,

    L_PAREN,
    R_PAREN,
    L_BRACE,
    R_BRACE,
    SEMICOLON,
    COLON,
    COMMA,
    EQUALS,
    UNDERSCORE,
    TRAILING_RETURN,

    BOOL_CHECK,

    EOF_TOKEN,

    UNKNOWN = 100
}

class Token
{
    public TokenType type;
    public string value;
    public int line;

    public Token(TokenType type, string value, int line)
    {
        this.type = type;
        this.value = value;
        this.line = line;
    }
}

enum SymbolType
{
    FUNCTION,
    ARGUMENT_VARIABLE,
    LOCAL_VARIABLE,
    LABEL,
    TYPE_NAME,

    UNDEFINED = 100
}

class Symbol
{
    public string identifier;
    public SymbolType symbolType;
    public int definitionLine;
    public int scopeDepth;
    public int paramCount;
    public bool definitionFound;
    public bool wasReferenced;

    public Symbol(string identifier, SymbolType symbolType, int scopeDepth, int definitionLine)
    {
        this.identifier = identifier;
        this.symbolType = symbolType;
        this.scopeDepth = scopeDepth;
        this.paramCount = 0;
        this.definitionFound = false;
        this.wasReferenced = false;
        this.definitionLine = definitionLine;
    }
}

class Parser
{
    private Token m_curToken;
    private string m_srcCodeTxt;
    private int m_ramBinLength_MB;
    private int m_pos;
    private int m_line;
    private int m_lastValidLine;
    private int m_curScopeDepth;
    private bool m_inFunctionScope;
    private int m_paramCount;

    private List<Symbol> m_functionsTbl;
    private List<Symbol> m_functionScopeTbl;
    private List<Symbol> m_unresolvedGotoTbl;
    private List<Symbol> m_typeNamesTbl;

    public Parser(string srcCodeTxt, int ramBinLength_MB, bool isDebugConfig)
    {
        m_curToken = new Token(TokenType.UNKNOWN, "", 1);
        m_srcCodeTxt = srcCodeTxt;
        m_ramBinLength_MB = ramBinLength_MB;
        m_pos = 0;
        m_line = 1;
        m_lastValidLine = 1;
        m_curScopeDepth = 0;
        m_inFunctionScope = false;
        m_paramCount = 0;

        m_functionsTbl = new List<Symbol>();
        m_functionScopeTbl = new List<Symbol>();
        m_unresolvedGotoTbl = new List<Symbol>();
        m_typeNamesTbl = new List<Symbol>();

        InsertFunctionSymbol("__builtin_int_add");
        InsertFunctionSymbol("__builtin_int_nand");
        InsertFunctionSymbol("__builtin_int_mem_read");
        InsertFunctionSymbol("__builtin_int_mem_write");

        MarkFunctionSymbolDefinitionFound("__builtin_int_add", 0);
        MarkFunctionSymbolDefinitionFound("__builtin_int_nand", 0);
        MarkFunctionSymbolDefinitionFound("__builtin_int_mem_read", 0);
        MarkFunctionSymbolDefinitionFound("__builtin_int_mem_write", 0);

        UpdateFunctionSymbolParamCount("__builtin_int_add", 2);
        UpdateFunctionSymbolParamCount("__builtin_int_nand", 2);
        UpdateFunctionSymbolParamCount("__builtin_int_mem_read", 1);
        UpdateFunctionSymbolParamCount("__builtin_int_mem_write", 2);

        InsertTypeNameSymbol("Int", 0);

        if (isDebugConfig)
        {
            InsertFunctionSymbol("__dbg_int");
            MarkFunctionSymbolDefinitionFound("__dbg_int", 0);
            UpdateFunctionSymbolParamCount("__dbg_int", 1);
        }
    }

    public void Parse()
    {
        IL_Emitter.InitEmitter(m_ramBinLength_MB);
        if (!ParseCompilationUnit())
        {
            PrintError("Failed to parse program");
        }
        else
        {
            PrintMessage("Program parsed successfully");
            IL_Emitter.FinishEmitter();
            IL_Emitter.RunILASM();
        }
    }

    bool ParseCompilationUnit()
    {
        ConsumeToken(); // Initialize m_curToken

        while (MatchTokenType(TokenType.USING))
        {
            ConsumeToken();
            if (!ParseUsingDirective())
            {
                return false;
            }
        }

        m_inFunctionScope = true;

        while (MatchTokenType(TokenType.FUNC))
        {
            ConsumeToken();
            ResetFunctionScope();
            if (!ParseFunctionDeclaration())
            {
                return false;
            }
        }

        if (!MatchTokenType(TokenType.EOF_TOKEN))
        {
            if (m_pos < m_srcCodeTxt.Length)
            {
                PrintError($"Unexpected token at line {m_curToken.line}. Found '{m_curToken.value}'");
            }
            else
            {
                PrintError($"Unexpected token at end of file. Found '{m_curToken.value}'");
            }
            return false;
        }

        if (AnyReferencedFunctionNotDefined())
        {
            return false;
        }

        if (!IsMainFunctionValid())
        {
            PrintError($"Entry point is missing. Please provide a 'main()' function implementation.");
            return false;
        }

        return true;
    }

    bool ParseUsingDirective()
    {
        if (!MatchTokenType(TokenType.FUNC))
        {
            PrintError($"Expected 'func' after 'using' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!ParseFunctionHeader())
        {
            return false;
        }

        if (!MatchTokenType(TokenType.SEMICOLON))
        {
            PrintError($"Expected ';' after using directive at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        return true;
    }

    bool ParseFunctionDeclaration()
    {
        if (!ParseFunctionHeader())
        {
            return false;
        }

        if (!ParseFunctionBody())
        {
            return false;
        }

        if (AnyUnresolvedGoto())
        {
            return false;
        }

        return true;
    }

    bool ParseFunctionHeader()
    {
        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected valid function name after 'func' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }

        bool wasForwardDeclared = false;
        if (!m_inFunctionScope)
        {
            InsertFunctionSymbol(m_curToken.value);
        }
        else
        {
            wasForwardDeclared = !InsertFunctionSymbol(m_curToken.value);
            if (IsFunctionSymbolDefinitionFound(m_curToken.value))
            {
                return false;
            }
            MarkFunctionSymbolDefinitionFound(m_curToken.value, m_lastValidLine);
        }
        var funcName = m_curToken.value;
        var defLine = m_lastValidLine;
        m_paramCount = 0;
        ConsumeToken();

        if (!MatchTokenType(TokenType.L_PAREN))
        {
            PrintError($"Expected '(' after function name at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.R_PAREN))
        {
            if (!ParseFunctionParameterList())
            {
                return false;
            }
        }

        if (!MatchTokenType(TokenType.R_PAREN))
        {
            PrintError($"Expected ')' after parameter list at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.TRAILING_RETURN))
        {
            PrintError($"Expected '->' after ')' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected type after '->' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        if (!ContainsTypeNameSymbol(m_curToken.value))
        {
            PrintError($"Undefined type name symbol at line {m_lastValidLine}. Found '{m_curToken.value}'");
            if (m_curToken.value == "int")
            {
                PrintError($"Did you mean 'Int'?");
            }
            return false;
        }
        ConsumeToken();

        if (m_inFunctionScope && wasForwardDeclared)
        {
            if (!MatchFunctionSymbolParamCount(funcName, m_paramCount))
            {
                PrintError($"Definition of function '{funcName}' at line {defLine} with different number of paramaters from declaration.");
                return false;
            }
        }
        UpdateFunctionSymbolParamCount(funcName, m_paramCount);
        if (m_inFunctionScope)
        {
            IL_Emitter.Emit_MethodHeader(funcName, m_paramCount);
        }

        return true;
    }

    bool ParseFunctionParameterList()
    {
        if (!ParseFunctionParameter())
        {
            return false;
        }
        m_paramCount++;

        while (MatchTokenType(TokenType.COMMA))
        {
            ConsumeToken();
            if (!ParseFunctionParameter())
            {
                return false;
            }
            m_paramCount++;
        }

        return true;
    }

    bool ParseFunctionParameter()
    {
        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected identifier in parameters at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }

        if (m_inFunctionScope)
        {
            if (!InsertFunctionScopeSymbol(m_curToken.value, SymbolType.ARGUMENT_VARIABLE, m_paramCount, m_lastValidLine))
            {
                return false;
            }
            MarkFunctionScopeSymbolDefinitionFound(m_curToken.value, m_lastValidLine);
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.COLON))
        {
            PrintError($"Expected ':' after identifier at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected type after ':' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        if (!ContainsTypeNameSymbol(m_curToken.value))
        {
            PrintError($"Undefined type name symbol at line {m_lastValidLine}. Found '{m_curToken.value}'");
            if (m_curToken.value == "int")
            {
                PrintError($"Did you mean 'Int'?");
            }
            return false;
        }
        ConsumeToken();

        return true;
    }

    bool ParseFunctionBody()
    {
        if (!MatchTokenType(TokenType.L_BRACE))
        {
            PrintError($"Expected '{{' after function header at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        IL_Emitter.Emit_MethodBodyBegin();
        ConsumeToken();

        m_paramCount = 0;
        while (MatchTokenType(TokenType.VAR))
        {
            ConsumeToken();
            if (!ParseLocalVariableDeclaration())
            {
                return false;
            }
            m_paramCount++;
        }
        if (m_paramCount > 0)
        {
            IL_Emitter.Emit_InitLocals(m_paramCount);
        }

        while (!MatchTokenType(TokenType.RETURN))
        {
            if (!ParseStatement())
            {
                if (MatchTokenType(TokenType.R_BRACE))
                {
                    PrintError($"Expected return statement at line {m_curToken.line}. Found '{m_curToken.value}'");
                }
                return false;
            }
        }

        if (!ParseReturnStatement())
        {
            return false;
        }

        if (!MatchTokenType(TokenType.R_BRACE))
        {
            PrintError($"Expected '}}' after return statement at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        IL_Emitter.Emit_MethodBodyEnd();
        ConsumeToken();

        return true;
    }

    bool ParseLocalVariableDeclaration()
    {
        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected identifier after 'var' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }

        if (m_inFunctionScope)
        {
            if (!InsertFunctionScopeSymbol(m_curToken.value, SymbolType.LOCAL_VARIABLE, m_paramCount, m_lastValidLine))
            {
                return false;
            }
            MarkFunctionScopeSymbolDefinitionFound(m_curToken.value, m_lastValidLine);
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.COLON))
        {
            PrintError($"Expected ':' after identifier at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected type after ':' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.SEMICOLON))
        {
            PrintError($"Expected ';' after type at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        return true;
    }

    bool ParseStatement()
    {
        if (MatchTokenType(TokenType.SET))
        {
            ConsumeToken();
            if (!ParseSetStatement())
            {
                return false;
            }
        }
        else if (MatchTokenType(TokenType.IF))
        {
            ConsumeToken();
            if (!ParseIfStatement())
            {
                return false;
            }
        }
        else if (MatchTokenType(TokenType.GOTO))
        {
            ConsumeToken();
            if (!ParseGotoStatement())
            {
                return false;
            }
        }
        else if (MatchTokenType(TokenType.UNDERSCORE))
        {
            ConsumeToken();
            if (!ParseDiscardStatement())
            {
                return false;
            }
        }
        else if (MatchTokenType(TokenType.IDENTIFIER))
        {
            var curIdentifier = m_curToken.value;
            ConsumeToken();
            if (MatchTokenType(TokenType.COLON))
            {
                ConsumeToken();
                if (!ParseLabelStatement())
                {
                    return false;
                }

                if (ContainsFunctionScopeSymbol(curIdentifier))
                {
                    if (!MatchFunctionScopeSymbolType(curIdentifier, SymbolType.LABEL))
                    {
                        PrintError($"Identifier '{curIdentifier}' at line {m_lastValidLine} is already defined");
                        return false;
                    }
                    if (IsFunctionScopeSymbolDefinitionFound(curIdentifier))
                    {
                        var sym = GetFunctionScopeSymbol(curIdentifier);
                        PrintError($"Duplicate definition for '{curIdentifier}' at line {m_lastValidLine}");
                        PrintError($"First definition: line {sym!.definitionLine}");
                        return false;
                    }
                    UpdateFunctionScopeSymbolScopeDepth(curIdentifier, m_curScopeDepth);
                }
                else
                {
                    InsertFunctionScopeSymbol(curIdentifier, SymbolType.LABEL, m_curScopeDepth, m_lastValidLine);
                }
                MarkFunctionScopeSymbolDefinitionFound(curIdentifier, m_lastValidLine);
                IL_Emitter.Emit_Label(curIdentifier);
            }
            else
            {
                PrintError($"Unexpected token at line {m_curToken.line}. Found '{m_curToken.value}'");
                return false;
            }
        }
        else
        {
            PrintError($"Unexpected token at line {m_curToken.line}. Found '{m_curToken.value}'");
            return false;
        }

        return true;
    }

    bool ParseLabelStatement()
    {
        return true;
    }

    bool ParseGotoStatement()
    {
        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected identifier after 'goto' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }

        if (!ContainsFunctionScopeSymbol(m_curToken.value))
        {
            InsertUnresolvedGotoSymbol(m_curToken.value, m_curScopeDepth, m_lastValidLine);
        }
        else if (!MatchFunctionScopeSymbolType(m_curToken.value, SymbolType.LABEL))
        {
            PrintError($"Identifier '{m_curToken.value}' at line {m_lastValidLine} is already defined");
            return false;
        }
        if (GetFunctionScopeSymbol(m_curToken.value)?.scopeDepth > m_curScopeDepth)
        {
            PrintError($"Label '{m_curToken.value}' is not available at current scope at line {m_lastValidLine}.");
            return false;
        }
        IL_Emitter.Emit_Goto(m_curToken.value);
        ConsumeToken();

        if (!MatchTokenType(TokenType.SEMICOLON))
        {
            PrintError($"Expected ';' after identifier at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        return true;
    }

    bool ParseSetStatement()
    {
        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected identifier after 'set' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }

        if (!ContainsFunctionScopeSymbol(m_curToken.value))
        {
            PrintError($"Undefined symbol '{m_curToken.value}' at line {m_lastValidLine}");
            return false;
        }
        if (!MatchFunctionScopeSymbolType(m_curToken.value, SymbolType.LOCAL_VARIABLE) &&
            !MatchFunctionScopeSymbolType(m_curToken.value, SymbolType.ARGUMENT_VARIABLE))
        {
            PrintError($"Expected variable symbol after 'set' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        var varName = m_curToken.value;
        ConsumeToken();

        if (!MatchTokenType(TokenType.EQUALS))
        {
            PrintError($"Expected '=' after identifier at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        m_paramCount = 0;
        if (!ParsePrimaryExpression())
        {
            return false;
        }

        if (!MatchTokenType(TokenType.SEMICOLON))
        {
            PrintError($"Expected ';' after expression at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        var symbol = GetFunctionScopeSymbol(varName);
        if (symbol!.symbolType == SymbolType.ARGUMENT_VARIABLE)
        {
            IL_Emitter.Emit_StoreArgVar(symbol.scopeDepth);
        }
        if (symbol!.symbolType == SymbolType.LOCAL_VARIABLE)
        {
            IL_Emitter.Emit_StoreLocalVar(symbol.scopeDepth);
        }

        return true;
    }

    bool ParseIfStatement()
    {

        if (!MatchTokenType(TokenType.L_PAREN))
        {
            PrintError($"Expected '(' after 'if' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!ParseBooleanExpression())
        {
            return false;
        }

        if (!MatchTokenType(TokenType.R_PAREN))
        {
            PrintError($"Expected ')' after boolean expression at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();
        IL_Emitter.Emit_IfBegin(m_curScopeDepth);

        if (!MatchTokenType(TokenType.L_BRACE))
        {
            PrintError($"Expected '{{' after ')' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        m_curScopeDepth++;

        while (!MatchTokenType(TokenType.R_BRACE))
        {
            if (!ParseStatement())
            {
                return false;
            }
        }

        if (!MatchTokenType(TokenType.R_BRACE))
        {
            PrintError($"Expected '}}' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        m_curScopeDepth--;

        if (MatchTokenType(TokenType.ELSE))
        {
            IL_Emitter.Emit_Else(true, m_curScopeDepth);
            ConsumeToken();
            if (!ParseElseStatement())
            {
                return false;
            }
        }
        else
        {
            IL_Emitter.Emit_Else(false, m_curScopeDepth);
        }

        IL_Emitter.Emit_IfEnd(m_curScopeDepth);

        return true;
    }

    bool ParseElseStatement()
    {
        if (!MatchTokenType(TokenType.L_BRACE))
        {
            PrintError($"Expected '{{' after ')' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        m_curScopeDepth++;

        while (!MatchTokenType(TokenType.R_BRACE))
        {
            if (!ParseStatement())
            {
                return false;
            }
        }

        if (!MatchTokenType(TokenType.R_BRACE))
        {
            PrintError($"Expected '}}' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        m_curScopeDepth--;

        return true;
    }

    bool ParseDiscardStatement()
    {
        if (!MatchTokenType(TokenType.EQUALS))
        {
            PrintError($"Expected '=' after '_' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        m_paramCount = 0;

        if (!ParseFunctionInvocation())
        {
            return false;
        }

        if (!MatchTokenType(TokenType.SEMICOLON))
        {
            PrintError($"Expected ';' after function invocation at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();
        IL_Emitter.Emit_DiscardReturn();

        return true;
    }

    bool ParseReturnStatement()
    {
        if (!MatchTokenType(TokenType.RETURN))
        {
            PrintError($"Expected 'return' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        m_paramCount = 0;
        if (!ParsePrimaryExpression())
        {
            return false;
        }

        if (!MatchTokenType(TokenType.SEMICOLON))
        {
            PrintError($"Expected ';' after expression at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();
        IL_Emitter.Emit_Return();

        return true;
    }

    bool ParsePrimaryExpression()
    {
        if (MatchTokenType(TokenType.INTEGER_LITERAL))
        {
            if (long.Parse(m_curToken.value) > 2147483647)
            {
                PrintError($"Integer overflow at line {m_lastValidLine}: '{m_curToken.value}' exceeds max allowed value {2147483647}.");
                return false;
            }
            IL_Emitter.Emit_LoadConst(m_curToken.value);
            ConsumeToken();
        }
        else if (MatchTokenType(TokenType.IDENTIFIER))
        {
            var curIdentifier = m_curToken.value;
            ConsumeToken();
            if (MatchTokenType(TokenType.L_PAREN))
            {
                if (!ContainsFunctionSymbol(curIdentifier))
                {
                    PrintError($"Undefined function '{curIdentifier}' at line {m_lastValidLine}.");
                    return false;
                }
                MarkFunctionSymbolAsReferenced(curIdentifier, m_lastValidLine);
                ConsumeToken();
                int oldParamCount = m_paramCount;
                m_paramCount = 0;
                if (!ParseFunctionInvocationTail())
                {
                    return false;
                }

                if (!MatchFunctionSymbolParamCount(curIdentifier, m_paramCount))
                {
                    PrintError($"Invalid number of arguments for function '{curIdentifier}' at line {m_lastValidLine}.");
                    return false;
                }
                IL_Emitter.Emit_Call(curIdentifier, m_paramCount);
                m_paramCount = oldParamCount;
            }
            else
            {
                if (!ContainsFunctionScopeSymbol(curIdentifier))
                {
                    PrintError($"Undefined symbol '{curIdentifier}' at line {m_lastValidLine}.");
                    return false;
                }
                var symbol = GetFunctionScopeSymbol(curIdentifier);
                if (symbol!.symbolType == SymbolType.ARGUMENT_VARIABLE)
                {
                    IL_Emitter.Emit_LoadArgVar(symbol.scopeDepth);
                }
                if (symbol!.symbolType == SymbolType.LOCAL_VARIABLE)
                {
                    IL_Emitter.Emit_LoadLocalVar(symbol.scopeDepth);
                }
            }
        }
        else
        {
            PrintError($"Expected expression at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }

        return true;
    }

    bool ParseBooleanExpression()
    {
        if (!MatchTokenType(TokenType.BOOL_CHECK))
        {
            PrintError($"Expected boolean expression after '(' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.L_PAREN))
        {
            PrintError($"Expected '(' after '__bool_check' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        m_paramCount = 0;
        if (!ParsePrimaryExpression())
        {
            return false;
        }

        if (!MatchTokenType(TokenType.R_PAREN))
        {
            PrintError($"Expected ')' after expression at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();
        IL_Emitter.Emit_BoolCheck();

        return true;
    }

    bool ParseFunctionInvocation()
    {
        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected identifier after '=' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        var funcName = m_curToken.value;
        if (!ContainsFunctionSymbol(funcName))
        {
            PrintError($"Undefined function '{funcName}' at line {m_lastValidLine}.");
            return false;
        }
        MarkFunctionSymbolAsReferenced(funcName, m_lastValidLine);
        ConsumeToken();

        if (!MatchTokenType(TokenType.L_PAREN))
        {
            PrintError($"Expected '(' after identifier at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        int oldParamCount = m_paramCount;
        m_paramCount = 0;
        if (!ParseFunctionInvocationTail())
        {
            return false;
        }

        if (!MatchFunctionSymbolParamCount(funcName, m_paramCount))
        {
            PrintError($"Invalid number of arguments for function '{funcName}' at line {m_lastValidLine}.");
            return false;
        }
        IL_Emitter.Emit_Call(funcName, m_paramCount);
        m_paramCount = oldParamCount;

        return true;
    }

    bool ParseFunctionInvocationTail()
    {
        if (!MatchTokenType(TokenType.R_PAREN))
        {
            if (!ParseFunctionArgumentList())
            {
                return false;
            }
        }

        if (!MatchTokenType(TokenType.R_PAREN))
        {
            PrintError($"Expected ')' after argument list at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        return true;
    }

    bool ParseFunctionArgumentList()
    {
        if (!ParseFunctionArgument())
        {
            return false;
        }
        m_paramCount++;

        while (MatchTokenType(TokenType.COMMA))
        {
            ConsumeToken();
            if (!ParseFunctionArgument())
            {
                return false;
            }
            m_paramCount++;
        }

        return true;
    }

    bool ParseFunctionArgument()
    {
        int oldParamCount = m_paramCount;
        m_paramCount = 0;
        if (!ParsePrimaryExpression())
        {
            return false;
        }
        m_paramCount = oldParamCount;

        return true;
    }

    Token GetNextToken()
    {
        if (m_pos >= m_srcCodeTxt.Length)
        {
            return new Token(TokenType.EOF_TOKEN, "", m_line);
        }
        var c = m_srcCodeTxt[m_pos];

        // Skip whitespaces
        while (char.IsWhiteSpace(c) || c == '\t' || c == '\r' || c == '\n')
        {
            if (c == '\n')
            {
                m_line++;
            }

            m_pos++;
            if (m_pos >= m_srcCodeTxt.Length)
            {
                return new Token(TokenType.EOF_TOKEN, "", m_line);
            }
            c = m_srcCodeTxt[m_pos];
        }

        // Skip comments
        if (c == '/' && m_srcCodeTxt[m_pos + 1] == '/')
        {
            while (c != '\n')
            {
                m_pos++;
                if (m_pos >= m_srcCodeTxt.Length)
                {
                    return new Token(TokenType.EOF_TOKEN, "", m_line);
                }
                c = m_srcCodeTxt[m_pos];
            }
            return GetNextToken();
        }

        if (char.IsLetter(c) || c == '_')
        {
            var sb = new StringBuilder();
            while (char.IsLetterOrDigit(c) || c == '_')
            {
                sb.Append(c);
                m_pos++;
                if (m_pos >= m_srcCodeTxt.Length)
                {
                    break;
                }
                c = m_srcCodeTxt[m_pos];
            }

            switch (sb.ToString())
            {
                case "using":
                    return new Token(TokenType.USING, "using", m_line);
                case "func":
                    return new Token(TokenType.FUNC, "func", m_line);
                case "var":
                    return new Token(TokenType.VAR, "var", m_line);
                case "set":
                    return new Token(TokenType.SET, "set", m_line);
                case "if":
                    return new Token(TokenType.IF, "if", m_line);
                case "else":
                    return new Token(TokenType.ELSE, "else", m_line);
                case "goto":
                    return new Token(TokenType.GOTO, "goto", m_line);
                case "return":
                    return new Token(TokenType.RETURN, "return", m_line);
                case "__bool_check":
                    return new Token(TokenType.BOOL_CHECK, "__bool_check", m_line);
                case "_":
                    return new Token(TokenType.UNDERSCORE, "_", m_line);

                // Macros
                case "__INT_MAX__":
                    return new Token(TokenType.INTEGER_LITERAL, "2147483647", m_line);
                case "__INT_WIDTH_BITS__":
                    return new Token(TokenType.INTEGER_LITERAL, "32", m_line);
                case "__INT_WIDTH_BYTES__":
                    return new Token(TokenType.INTEGER_LITERAL, "4", m_line);
                case "__INT_SIGN_BIT_MASK__":
                    return new Token(TokenType.INTEGER_LITERAL, "2147483648", m_line);

            }

            return new Token(TokenType.IDENTIFIER, sb.ToString(), m_line);
        }
        else if (char.IsDigit(c))
        {
            var sb = new StringBuilder();
            while (char.IsDigit(c))
            {
                sb.Append(c);
                m_pos++;
                if (m_pos >= m_srcCodeTxt.Length)
                {
                    break;
                }
                c = m_srcCodeTxt[m_pos];
            }
            return new Token(TokenType.INTEGER_LITERAL, sb.ToString(), m_line);
        }
        else if (c == '(')
        {
            m_pos++;
            return new Token(TokenType.L_PAREN, "(", m_line);
        }
        else if (c == ')')
        {
            m_pos++;
            return new Token(TokenType.R_PAREN, ")", m_line);
        }
        else if (c == '{')
        {
            m_pos++;
            return new Token(TokenType.L_BRACE, "{", m_line);
        }
        else if (c == '}')
        {
            m_pos++;
            return new Token(TokenType.R_BRACE, "}", m_line);
        }
        else if (c == ';')
        {
            m_pos++;
            return new Token(TokenType.SEMICOLON, ";", m_line);
        }
        else if (c == ',')
        {
            m_pos++;
            return new Token(TokenType.COMMA, ",", m_line);
        }
        else if (c == ':')
        {
            m_pos++;
            return new Token(TokenType.COLON, ":", m_line);
        }
        else if (c == '=')
        {
            m_pos++;
            return new Token(TokenType.EQUALS, "=", m_line);
        }
        else if (c == '-')
        {
            var sb = new StringBuilder();
            sb.Append(c);
            m_pos++;
            if (m_pos < m_srcCodeTxt.Length)
            {
                c = m_srcCodeTxt[m_pos];
                if (c == '>')
                {
                    sb.Append(c);
                    m_pos++;
                    return new Token(TokenType.TRAILING_RETURN, sb.ToString(), m_line);
                }
                else
                {
                    return new Token(TokenType.UNKNOWN, sb.ToString(), m_line);
                }
            }
        }

        return new Token(TokenType.UNKNOWN, $"{c}", m_line);
    }

    void ConsumeToken()
    {
        m_lastValidLine = m_curToken.line;
        m_curToken = GetNextToken();
    }

    bool MatchTokenType(TokenType type)
    {
        if (m_curToken.type == type)
        {
            return true;
        }
        return false;
    }

    bool InsertFunctionSymbol(string identifier)
    {
        foreach (var symbol in m_functionsTbl)
        {
            if (symbol.identifier == identifier)
            {
                if (m_inFunctionScope && symbol.definitionFound)
                {
                    PrintError($"Duplicate definition for function '{identifier}' at line {m_lastValidLine}");
                    PrintError($"First definition: line {symbol.definitionLine}");
                }
                return false;
            }
        }
        m_functionsTbl.Add(new Symbol(identifier, SymbolType.FUNCTION, 0, 0));
        return true;
    }

    bool ContainsFunctionSymbol(string identifier)
    {
        foreach (var symbol in m_functionsTbl)
        {
            if (symbol.identifier == identifier)
            {
                return true;
            }
        }
        return false;
    }

    void MarkFunctionSymbolDefinitionFound(string identifier, int definitionLine)
    {
        foreach (var symbol in m_functionsTbl)
        {
            if (symbol.identifier == identifier)
            {
                symbol.definitionFound = true;
                symbol.definitionLine = definitionLine;
                return;
            }
        }
    }

    void MarkFunctionSymbolAsReferenced(string identifier, int referencedLine)
    {
        foreach (var symbol in m_functionsTbl)
        {
            if (symbol.identifier == identifier)
            {
                if (!symbol.definitionFound && !symbol.wasReferenced)
                {
                    symbol.wasReferenced = true;
                    symbol.definitionLine = referencedLine;
                }
                return;
            }
        }
    }

    bool IsFunctionSymbolDefinitionFound(string identifier)
    {
        foreach (var symbol in m_functionsTbl)
        {
            if (symbol.identifier == identifier)
            {
                return symbol.definitionFound;
            }
        }
        return false;
    }

    void UpdateFunctionSymbolParamCount(string identifier, int count)
    {
        foreach (var symbol in m_functionsTbl)
        {
            if (symbol.identifier == identifier)
            {
                symbol.paramCount = count;
                return;
            }
        }
    }


    bool MatchFunctionSymbolParamCount(string identifier, int count)
    {
        foreach (var symbol in m_functionsTbl)
        {
            if (symbol.identifier == identifier)
            {
                return symbol.paramCount == count;
            }
        }
        return false;
    }

    bool AnyReferencedFunctionNotDefined()
    {
        foreach (var symbol in m_functionsTbl)
        {
            if (symbol.wasReferenced && !symbol.definitionFound)
            {
                PrintError($"Unresolved reference of undefined function '{symbol.identifier}' at line {symbol.definitionLine}.");
                return true;
            }
        }
        return false;
    }

    bool IsMainFunctionValid()
    {
        var mainName = "main";
        if (ContainsFunctionSymbol(mainName) &&
            MatchFunctionSymbolParamCount(mainName, 0) &&
            IsFunctionSymbolDefinitionFound(mainName))
        {
            return true;
        }
        return false;
    }

    void ResetFunctionScope()
    {
        m_functionScopeTbl.Clear();
        m_unresolvedGotoTbl.Clear();
        m_curScopeDepth = 0;
    }

    bool InsertFunctionScopeSymbol(string identifier, SymbolType symbolType, int scopeDepth, int definitionLine)
    {
        foreach (var symbol in m_functionScopeTbl)
        {
            if (symbol.identifier == identifier && symbol.definitionFound)
            {
                PrintError($"Duplicate definition for '{identifier}' at line {m_lastValidLine}");
                PrintError($"First definition: line {symbol.definitionLine}");
                return false;
            }
        }
        m_functionScopeTbl.Add(new Symbol(identifier, symbolType, scopeDepth, definitionLine));
        return true;
    }

    bool ContainsFunctionScopeSymbol(string identifier)
    {
        foreach (var symbol in m_functionScopeTbl)
        {
            if (symbol.identifier == identifier)
            {
                return true;
            }
        }
        return false;
    }

    void MarkFunctionScopeSymbolDefinitionFound(string identifier, int definitionLine)
    {
        foreach (var symbol in m_functionScopeTbl)
        {
            if (symbol.identifier == identifier)
            {
                symbol.definitionFound = true;
                symbol.definitionLine = definitionLine;
                return;
            }
        }
    }

    void UpdateFunctionScopeSymbolScopeDepth(string identifier, int scopeDepth)
    {
        foreach (var symbol in m_functionScopeTbl)
        {
            if (symbol.identifier == identifier)
            {
                symbol.scopeDepth = scopeDepth;
                return;
            }
        }
    }

    bool IsFunctionScopeSymbolDefinitionFound(string identifier)
    {
        foreach (var symbol in m_functionScopeTbl)
        {
            if (symbol.identifier == identifier)
            {
                return symbol.definitionFound;
            }
        }
        return false;
    }

    bool MatchFunctionScopeSymbolType(string identifier, SymbolType type)
    {
        foreach (var symbol in m_functionScopeTbl)
        {
            if (symbol.identifier == identifier)
            {
                return symbol.symbolType == type;
            }
        }
        return false;
    }

    bool AnyUnresolvedGoto()
    {
        foreach (var symbol in m_unresolvedGotoTbl)
        {
            if (!ContainsFunctionScopeSymbol(symbol.identifier))
            {
                PrintError($"Use of undefined label symbol '{symbol.identifier}' at line {symbol.definitionLine}.");
                return true;
            }

            if (GetFunctionScopeSymbol(symbol.identifier)!.scopeDepth > symbol.scopeDepth)
            {
                PrintError($"Label '{symbol.identifier}' is not available at current scope at line {symbol.definitionLine}.");
                return true;
            }
        }
        return false;
    }

    Symbol? GetFunctionScopeSymbol(string identifier)
    {
        foreach (var symbol in m_functionScopeTbl)
        {
            if (symbol.identifier == identifier)
            {
                return symbol;
            }
        }
        return null;
    }

    bool InsertUnresolvedGotoSymbol(string identifier, int scopeDepth, int definitionLine)
    {
        m_unresolvedGotoTbl.Add(new Symbol(identifier, SymbolType.LABEL, scopeDepth, definitionLine));
        return true;
    }

    bool InsertTypeNameSymbol(string identifier, int definitionLine)
    {
        foreach (var symbol in m_typeNamesTbl)
        {
            if (symbol.identifier == identifier)
            {
                PrintError($"Duplicate definition for '{identifier}' at line {m_lastValidLine}");
                PrintError($"First definition: line {symbol.definitionLine}");
                return false;
            }
        }
        m_typeNamesTbl.Add(new Symbol(identifier, SymbolType.TYPE_NAME, 0, definitionLine));
        return true;
    }

    bool ContainsTypeNameSymbol(string identifier)
    {
        foreach (var symbol in m_typeNamesTbl)
        {
            if (symbol.identifier == identifier)
            {
                return true;
            }
        }
        return false;
    }

    void PrintMessage(string message)
    {
        Console.WriteLine(message);
    }

    void PrintError(string message)
    {
        Console.WriteLine(message);
    }

}

class Program
{
    static void Main(string[] args)
    {
        string srcCodePath = "";
        int ramBinLength_MB = 100;
        bool isDebug = false;

        if (args.Length == 0)
        {
            Console.WriteLine("Nothing to parse.");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("    ILGenerator_CS <src_file_path> [--ramBinLength=<length>] [--debug]");
            Console.WriteLine();
            Console.WriteLine("Params:");
            Console.WriteLine("    src_file_path:               Path to the FFLang source code file");
            Console.WriteLine("    --debug:                     (Optional) Includes built-in: 'func __dbg_int(v: Int) -> Int'");
            Console.WriteLine("    --ramBinLength=<length>:     (Optional) Specifies 'RAM.bin' file size in MB.");
            Console.WriteLine("                                 Allowed range: 1 to 1024. Default 100");
            return;
        }

        foreach (var arg in args)
        {
            if (arg.StartsWith("--ramBinLength="))
            {
                if (int.TryParse(arg.Replace("--ramBinLength=", ""), out ramBinLength_MB))
                {
                    if (ramBinLength_MB <= 0 || ramBinLength_MB > 1024)
                    {
                        Console.WriteLine($"Invalid 'RAM.bin' size. Allowed range (in MB): 1 to 1024. Default 100");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid 'RAM.bin' size. Allowed range (in MB): 1 to 1024. Default 100");
                    return;
                }
            }
            else if (arg == "--debug")
            {
                isDebug = true;
            }
            else
            {
                srcCodePath = arg;
            }
        }

        var parser = new Parser(System.IO.File.ReadAllText(srcCodePath), ramBinLength_MB, isDebug);
        parser.Parse();
    }
}
