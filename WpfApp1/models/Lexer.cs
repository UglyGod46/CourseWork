using System.Collections.Generic;
using System;
using WpfApp1.models;

public class Lexer
{
    private readonly string _input;
    private int _position;

    public Lexer(string input)
    {
        _input = input;
        _position = 0;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_position < _input.Length)
        {
            char currentChar = _input[_position];

            if (char.IsWhiteSpace(currentChar))
            {
                MatchSeparator();
                continue;
            }

            if (char.IsLetter(currentChar) || currentChar == '_')
            {
                Token token = ParseIdentifierOrKeyword();

                // Проверяем на наличие кириллицы в идентификаторе или ключевом слове
                if (ContainsCyrillic(token.Value))
                {
                    token.Type = TokenType.Error;
                    token.Value = token.Value;
                }

                tokens.Add(token);

                if ((token.Type == TokenType.Keyword) &&
                    (token.Value.StartsWith("function") || token.Value.StartsWith("return")))
                {
                    int separatorStart = _position;
                    MatchSeparator();
                    if (separatorStart != _position)
                    {
                        tokens.Add(new Token
                        {
                            Type = TokenType.Separator,
                            Value = " ",
                            StartIndex = separatorStart,
                            EndIndex = _position - 1
                        });
                    }
                }
            }
            else if (char.IsDigit(currentChar))
            {
                tokens.Add(ParseNumber());
            }
            else if (IsOperator(currentChar))
            {
                tokens.Add(ParseOperator());
            }
            else if (IsPunctuation(currentChar))
            {
                tokens.Add(ParsePunctuation());
            }
            else if (IsEndOfStatement(currentChar))
            {
                tokens.Add(ParseEndOfStatement());
            }
            else if (IsCyrillic(currentChar))
            {
                // Обработка отдельных кириллических символов
                tokens.Add(new Token
                {
                    Type = TokenType.Error,
                    Value = "Ошибка: кириллический символ - " + currentChar,
                    StartIndex = _position,
                    EndIndex = _position
                });
                _position++;
            }
            else
            {
                tokens.Add(new Token
                {
                    Type = TokenType.Invalid,
                    Value = currentChar.ToString(),
                    StartIndex = _position,
                    EndIndex = _position
                });
                _position++;
            }
        }

        return tokens;
    }

    private bool ContainsCyrillic(string text)
    {
        foreach (char c in text)
        {
            if (IsCyrillic(c))
                return true;
        }
        return false;
    }

    private bool IsCyrillic(char c)
    {
        // Диапазоны символов кириллицы в Unicode
        return (c >= 'А' && c <= 'я') || c == 'ё' || c == 'Ё';
    }

    private Token ParseIdentifierOrKeyword()
    {
        int start = _position;
        string value = ParseWhile(c => char.IsLetterOrDigit(c) || c == '_');

        // Проверяем на наличие кириллицы в идентификаторе или ключевом слове
        if (ContainsCyrillic(value))
        {
            return new Token
            {
                Type = TokenType.Error,
                Value = value,
                StartIndex = start,
                EndIndex = _position - 1
            };
        }

        // Проверяем, является ли value ключевым словом (только если оно точно совпадает)
        var tokenType = IsKeyword(value) ? TokenType.Keyword : TokenType.Identifier;

        return new Token
        {
            Type = tokenType,
            Value = value,
            StartIndex = start,
            EndIndex = _position - 1
        };
    }

    // Остальные методы остаются без изменений
    private Token ParseNumber()
    {
        int start = _position;
        string value = ParseWhile(char.IsDigit);
        return new Token
        {
            Type = TokenType.Number,
            Value = value,
            StartIndex = start,
            EndIndex = _position - 1
        };
    }

    private Token ParseOperator()
    {
        int start = _position;
        string value = _input[_position].ToString();
        _position++;
        return new Token
        {
            Type = TokenType.Operator,
            Value = value,
            StartIndex = start,
            EndIndex = _position - 1
        };
    }

    private Token ParsePunctuation()
    {
        int start = _position;
        string value = _input[_position].ToString();
        _position++;
        return new Token
        {
            Type = TokenType.Punctuation,
            Value = value,
            StartIndex = start,
            EndIndex = _position - 1
        };
    }

    private string ParseWhile(Func<char, bool> condition)
    {
        int start = _position;
        while (_position < _input.Length && condition(_input[_position]))
        {
            _position++;
        }
        return _input.Substring(start, _position - start);
    }

    private Token ParseEndOfStatement()
    {
        int start = _position;
        string value = _input[_position].ToString();
        _position++;
        return new Token
        {
            Type = TokenType.EndOfStatement,
            Value = value,
            StartIndex = start,
            EndIndex = _position - 1
        };
    }

    private void MatchSeparator()
    {
        while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
        {
            _position++;
        }
    }

    private bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/' || c == '=';
    }

    private bool IsPunctuation(char c)
    {
        return c == '(' || c == ')' || c == '{' || c == '}' || c == ',';
    }

    private bool IsEndOfStatement(char c)
    {
        return c == ';';
    }

    private bool IsKeyword(string value)
    {
        return value == "function" || value == "return";
    }
}