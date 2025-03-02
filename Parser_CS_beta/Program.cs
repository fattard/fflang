using System;
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

class Parser
{
    private Token m_curToken;
    private string m_srcCodeTxt;
    private int m_pos;
    private int m_line;
    private int m_lastValidLine;

    public Parser(string srcCodeTxt)
    {
        m_curToken = new Token(TokenType.UNKNOWN, "", 1);
        m_srcCodeTxt = srcCodeTxt;
        m_pos = 0;
        m_line = 1;
        m_lastValidLine = 1;
    }

    public void Parse()
    {
        if (!ParseCompilationUnit())
        {
            PrintError("Failed to parse program");
        }
        else
        {
            PrintMessage("Program parsed successfully");
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

        while (MatchTokenType(TokenType.FUNC))
        {
            ConsumeToken();
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

        return true;
    }

    bool ParseFunctionHeader()
    {
        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected valid function name after 'func' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
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
        ConsumeToken();

        return true;
    }

    bool ParseFunctionParameterList()
    {
        if (!ParseFunctionParameter())
        {
            return false;
        }

        while (MatchTokenType(TokenType.COMMA))
        {
            ConsumeToken();
            if (!ParseFunctionParameter())
            {
                return false;
            }
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

        return true;
    }

    bool ParseFunctionBody()
    {
        if (!MatchTokenType(TokenType.L_BRACE))
        {
            PrintError($"Expected '{{' after function header at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        while (MatchTokenType(TokenType.VAR))
        {
            ConsumeToken();
            if (!ParseLocalVariableDeclaration())
            {
                return false;
            }
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

        if ( !MatchTokenType(TokenType.R_BRACE))
        {
            PrintError($"Expected '}}' after return statement at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
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
            ConsumeToken();
            if (MatchTokenType(TokenType.COLON))
            {
                ConsumeToken();
                if (!ParseLabelStatement())
                {
                    return false;
                }
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
        ConsumeToken();

        if (!MatchTokenType(TokenType.EQUALS))
        {
            PrintError($"Expected '=' after identifier at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

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

        if (!MatchTokenType(TokenType.L_BRACE))
        {
            PrintError($"Expected '{{' after ')' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

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

        if (MatchTokenType(TokenType.ELSE))
        {
            ConsumeToken();
            if (!ParseElseStatement())
            {
                return false;
            }
        }

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

        return true;
    }

    bool ParsePrimaryExpression()
    {
        if (MatchTokenType(TokenType.INTEGER_LITERAL))
        {
            ConsumeToken();
        }
        else if (MatchTokenType(TokenType.IDENTIFIER))
        {
            ConsumeToken();
            if (MatchTokenType(TokenType.L_PAREN))
            {
                ConsumeToken();
                if (!ParseFunctionInvocationTail())
                {
                    return false;
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

        return true;
    }

    bool ParseFunctionInvocation()
    {
        if (!MatchTokenType(TokenType.IDENTIFIER))
        {
            PrintError($"Expected identifier after '=' at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!MatchTokenType(TokenType.L_PAREN))
        {
            PrintError($"Expected '(' after identifier at line {m_lastValidLine}. Found '{m_curToken.value}'");
            return false;
        }
        ConsumeToken();

        if (!ParseFunctionInvocationTail())
        {
            return false;
        }

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

        while (MatchTokenType(TokenType.COMMA))
        {
            ConsumeToken();
            if (!ParseFunctionArgument())
            {
                return false;
            }
        }

        return true;
    }

    bool ParseFunctionArgument()
    {
        if (!ParsePrimaryExpression())
        {
            return false;
        }

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
        if (args.Length == 0)
        {
            Console.WriteLine("Nothing to parse.");
        }
        else if (args.Length == 1)
        {
            var parser = new Parser(System.IO.File.ReadAllText(args[0]));
            parser.Parse();
        }
        else
        {
            Console.WriteLine("Invalid arguments. Provide a single file path.");
        }
    }
}
