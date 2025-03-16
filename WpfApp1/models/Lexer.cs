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
                MatchSeparator(); // Пропускаем пробелы
                continue;
            }

            if (char.IsLetter(currentChar))
            {
                Token token = ParseIdentifierOrKeyword();

                // Проверяем, является ли первая лексема ключевым словом "function"
                if (tokens.Count == 0 && (token.Type != TokenType.Keyword || token.Value != "function"))
                {
                    tokens.Add(new Token
                    {
                        Type = TokenType.Error,
                        Value = "Ошибка: Первое слово должно быть 'function'",
                        StartIndex = token.StartIndex,
                        EndIndex = token.EndIndex
                    });
                    return tokens; // Завершаем анализ
                }

                tokens.Add(token);

                // Если это ключевое слово "function", проверяем, что следующая лексема — идентификатор
                if (token.Type == TokenType.Keyword && token.Value == "function")
                {
                    // Пропускаем пробелы после "function"
                    MatchSeparator();

                    // Проверяем, что следующая лексема — идентификатор
                    if (_position >= _input.Length || !char.IsLetter(_input[_position]))
                    {
                        tokens.Add(new Token
                        {
                            Type = TokenType.Error,
                            Value = "Ошибка: После 'function' должно быть название функции",
                            StartIndex = _position,
                            EndIndex = _position
                        });
                        return tokens; // Завершаем анализ
                    }

                    // Добавляем идентификатор (название функции)
                    Token identifierToken = ParseIdentifierOrKeyword();
                    tokens.Add(identifierToken);
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
            else if (IsEndOfStatement(currentChar)) // Обработка конца оператора
            {
                tokens.Add(ParseEndOfStatement());
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

    private Token ParseIdentifierOrKeyword()
    {
        int start = _position;
        string value = ParseWhile(c => char.IsLetterOrDigit(c) || c == '_');

        var tokenType = IsKeyword(value) ? TokenType.Keyword : TokenType.Identifier;

        // Проверка обязательного разделителя после ключевого слова `function`
        if (tokenType == TokenType.Keyword && (value == "function" || value == "return"))
        {
            MatchSeparator(); // Пропускаем пробелы после `function` и `return`
        }

        return new Token
        {
            Type = tokenType,
            Value = value,
            StartIndex = start,
            EndIndex = _position - 1
        };
    }

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
