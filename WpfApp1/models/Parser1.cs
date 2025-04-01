using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp1.models;

public class ParseResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = new List<string>();
}

public class Parser1
{
    private readonly List<Token> _tokens;
    private int _currentTokenIndex;
    private Token CurrentToken => _currentTokenIndex < _tokens.Count ? _tokens[_currentTokenIndex] : null;

    public Parser1(List<Token> tokens)
    {
        _tokens = tokens;
        _currentTokenIndex = 0;
    }

    public ParseResult Parse()
    {
        var result = new ParseResult();

        try
        {
            ParseFunction(result);

            // Проверим, что разобрали все токены
            if (_currentTokenIndex < _tokens.Count)
            {
                AddError(result, $"Ожидался конец файла, но найден {CurrentToken.Value}", CurrentToken.StartIndex);
            }
        }
        catch (ParseException ex)
        {
            AddError(result, ex.Message, ex.Position);
        }

        return result;
    }

    private void ParseFunction(ParseResult result)
    {
        TryParse(() => {
            ExpectKeyword("function", "объявления функции");
            SkipSeparators();
            ParseFunctionName();
            SkipSeparators();
            ExpectPunctuation("(", "списка параметров");
            ParseParameterList(result);
            ExpectPunctuation(")", "списка параметров");
            SkipSeparators();
            ParseBody(result);
        }, result, new[] { "}" });
    }

    private void ParseFunctionName()
    {
        if (CurrentToken?.Type != TokenType.Identifier)
        {
            throw NewParseError("идентификатор имени функции");
        }
        _currentTokenIndex++;
    }

    private void ParseParameterList(ParseResult result)
    {
        TryParse(() => {
            SkipSeparators();

            if (CurrentToken?.Value == ")")
            {
                return; // Пустой список параметров
            }

            ParseIdentifier();

            while (CurrentToken?.Value == ",")
            {
                _currentTokenIndex++;
                SkipSeparators();
                ParseIdentifier();
                SkipSeparators();
            }
        }, result, new[] { ")" });
    }

    private void ParseBody(ParseResult result)
    {
        TryParse(() => {
            ExpectPunctuation("{", "начала тела функции");
            SkipSeparators();

            while (_currentTokenIndex < _tokens.Count && CurrentToken?.Value != "}")
            {
                ParseStatement(result);
                SkipSeparators();
            }

            ExpectPunctuation("}", "окончания тела функции");

            // Необязательная точка с запятой после тела функции
            if (CurrentToken?.Value == ";")
            {
                _currentTokenIndex++;
            }
        }, result, new[] { "}" });
    }

    private void ParseStatement(ParseResult result)
    {
        TryParse(() => {
            ParseReturnStatement(result);
            ExpectEndOfStatement();
        }, result, new[] { "}", ";" });
    }

    private void ParseReturnStatement(ParseResult result)
    {
        TryParse(() => {
            ExpectKeyword("return", "оператора return");
            SkipSeparators();
            ParseExpression(result);
        }, result, new[] { ";" });
    }

    private void ParseExpression(ParseResult result)
    {
        TryParse(() => {
            ParseTerm(result);

            while (_currentTokenIndex < _tokens.Count &&
                  (CurrentToken.Value == "+" || CurrentToken.Value == "-"))
            {
                _currentTokenIndex++;
                SkipSeparators();
                ParseTerm(result);
            }
        }, result, new[] { ")", ";" });
    }

    private void ParseTerm(ParseResult result)
    {
        TryParse(() => {
            ParseFactor(result);

            while (_currentTokenIndex < _tokens.Count &&
                  (CurrentToken.Value == "*" || CurrentToken.Value == "/"))
            {
                _currentTokenIndex++;
                SkipSeparators();
                ParseFactor(result);
            }
        }, result, new[] { ")", ";", "+", "-" });
    }

    private void ParseFactor(ParseResult result)
    {
        TryParse(() => {
            SkipSeparators();

            if (CurrentToken?.Value == "(")
            {
                _currentTokenIndex++;
                SkipSeparators();
                ParseExpression(result);
                ExpectPunctuation(")", "выражения в скобках");
            }
            else if (CurrentToken?.Type == TokenType.Identifier ||
                    CurrentToken?.Type == TokenType.Number)
            {
                _currentTokenIndex++;
            }
            else
            {
                throw NewParseError("идентификатор, число или '('");
            }

            SkipSeparators();
        }, result, new[] { ")", ";", "+", "-", "*", "/" });
    }

    private void ParseIdentifier()
    {
        if (CurrentToken?.Type != TokenType.Identifier)
        {
            throw NewParseError("идентификатор");
        }
        _currentTokenIndex++;
    }

    private void TryParse(Action parseAction, ParseResult result, string[] recoveryTokens)
    {
        try
        {
            parseAction();
        }
        catch (ParseException ex)
        {
            AddError(result, ex.Message, ex.Position);
            RecoverFromError(recoveryTokens);
        }
    }

    private void RecoverFromError(string[] recoveryTokens)
    {
        while (_currentTokenIndex < _tokens.Count &&
              !recoveryTokens.Contains(CurrentToken.Value))
        {
            _currentTokenIndex++;
        }

        if (_currentTokenIndex < _tokens.Count &&
            recoveryTokens.Contains(CurrentToken.Value))
        {
            _currentTokenIndex++;
        }
    }

    private void ExpectKeyword(string keyword, string context)
    {
        if (CurrentToken?.Type != TokenType.Keyword || CurrentToken?.Value != keyword)
        {
            throw NewParseError($"ключевое слово '{keyword}' ({context})");
        }
        _currentTokenIndex++;
    }

    private void ExpectPunctuation(string punctuation, string context)
    {
        if (CurrentToken?.Type != TokenType.Punctuation || CurrentToken?.Value != punctuation)
        {
            throw NewParseError($"символ '{punctuation}' ({context})");
        }
        _currentTokenIndex++;
    }

    private void ExpectEndOfStatement()
    {
        if (CurrentToken?.Type != TokenType.EndOfStatement)
        {
            throw NewParseError("символ ';'");
        }
        _currentTokenIndex++;
    }

    private void SkipSeparators()
    {
        while (_currentTokenIndex < _tokens.Count &&
              _tokens[_currentTokenIndex].Type == TokenType.Separator)
        {
            _currentTokenIndex++;
        }
    }

    private ParseException NewParseError(string expected)
    {
        return new ParseException(
            $"Ожидался {expected}, но найден {CurrentToken?.Value ?? "конец файла"}",
            CurrentToken?.StartIndex ?? _tokens.Last().EndIndex);
    }

    private void AddError(ParseResult result, string message, int position)
    {
        result.Errors.Add($"{message} (позиция {position})");
    }
}

public class ParseException : Exception
{
    public int Position { get; }

    public ParseException(string message, int position) : base(message)
    {
        Position = position;
    }
}