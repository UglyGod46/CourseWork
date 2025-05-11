using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using WpfApp1.models;

public class Parser
{
    private List<Token> _tokens;
    private int _currentIndex;
    public List<ParseError> Errors { get; } = new List<ParseError>();
    private HashSet<string> _parameters = new HashSet<string>();

    public class ParseError
    {
        public string Message { get; }
        public string Position { get; }

        public ParseError(string message, string position)
        {
            Message = message;
            Position = position;
        }

        public override string ToString() => $"{Message}\tПозиция: {Position}";
    }

    public Parser(List<Token> tokens)
    {
        _tokens = tokens.Where(t => t.Code != 7 && t.Code != 8).ToList();
        _currentIndex = 0;
    }

    private Token CurrentToken => _currentIndex < _tokens.Count ? _tokens[_currentIndex] : null;
    private void MoveNext() => _currentIndex++;

    private void AddError(string expected, Token actual)
    {
        if (actual == null) return;
        Errors.Add(new ParseError($"{expected}, получено '{actual.Lexeme}'", actual.Position));
    }

    private void AddError1(string expected, Token actual)
    {
        if (actual == null) return;
        Errors.Add(new ParseError($"Ожидалось {expected}, получено '{actual.Lexeme}'", actual.Position));
    }

    private bool Check1(int expectedCode, string expectedName)
    {
        if (CurrentToken?.Code == expectedCode) return true;
        AddError1(expectedName, CurrentToken);
        return false;
    }

    public void Parse()
    {
        // Сначала обрабатываем лексические ошибки
        var errorTokens = _tokens.Where(t => t.Code == 15).ToList();
       
        _currentIndex = 0;

        foreach (var token in errorTokens)
        {
            Errors.Add(new ParseError($"Неожиданный символ '{token.Lexeme}'", token.Position));
        }

        // Выполняем синтаксический анализ независимо от наличия лексических ошибок
        ParseFunction();

        // Проверяем только корректные токены после функции
        if (CurrentToken != null && CurrentToken.Code != 14 && CurrentToken.Code != 15)
        {
            AddError("Неожиданный символ после функции", CurrentToken);
        }

        End();
    }

    private void ParseFunction()
    {
        // 1. Проверяем ключевое слово 'function'
        if (CurrentToken == null) return;

        if (CurrentToken.Code == 1) // Правильное ключевое слово
        {
            MoveNext();
        }
        else
        {
            // Проверяем, похоже ли на function (начинается с 'func')
            bool looksLikeFunction = CurrentToken.Lexeme?.StartsWith("", StringComparison.Ordinal) ?? false;
            if (CurrentToken.Lexeme.StartsWith("function", StringComparison.Ordinal) && CurrentToken.Code == 10)
            {
                AddError("Ожидался пробел после function", CurrentToken);
                MoveNext();
            }
            else if (looksLikeFunction && CurrentToken.Code != 15)
            {
                AddError("Ожидалось ключевое слово 'function'", CurrentToken);
                MoveNext();
            }
            else
            {

            }

            // Продолжаем анализ, даже если ключевое слово неверное
            while (CurrentToken.Code == 15)
            {
                MoveNext();
            }
        }

        while (CurrentToken.Code == 15)
        {
            MoveNext();
        }


        // 2. Проверяем имя функции

        if (CurrentToken.Code == 10)
        {
            MoveNext();
        } else if(CurrentToken?.Code == 9 || CurrentToken?.Code == 10 || CurrentToken?.Code == 17)
        {
           
        }
        else
        {
            AddError("Ожидалось имя функции", CurrentToken);
        }
        while (CurrentToken.Code == 15)
        {
            MoveNext();
        }

        // 3. Проверяем параметры
        if (CurrentToken?.Code == 9) // '('
        {
            MoveNext();
            ParseParameters();

            // Проверяем закрывающую скобку параметров
            if (CurrentToken?.Code != 11)
            {
                AddError("Ожидалась ')'", CurrentToken);

            }
            else
            {
                MoveNext();
            }
        }
        else
        {
            AddError("Ожидалась '(' после имени функции", CurrentToken);

            ParseParameters();

            // Проверяем закрывающую скобку параметров
            if (!Check1(11, "')'"))
            {
                // Если не нашли ')', пропускаем до '{'

                
            }
            else
            {
                MoveNext();
            }
        }
        while (CurrentToken.Code == 15)
        {
            MoveNext();
        }

        // 4. Проверяем тело функции
        if (CurrentToken?.Code == 12) // '{'
        {
            MoveNext();
            ParseReturnStatement();

            // Проверка на закрывающую скобку '}'
            if (CurrentToken == null || CurrentToken.Code != 13)
            {
                string errorPosition = "end";
                if (CurrentToken != null && CurrentToken.Code == 11) // Если это ')'
                {
                    AddError("Ожидалась '}'", CurrentToken);
                    errorPosition = CurrentToken.Position;
                }
                AddError("Ожидалась '}'", new Token { Position = errorPosition });
            }
            else
            {
                MoveNext();
            }
        }
        else
        {
            // Если нет '{', но есть 'return', считаем это началом тела
            if (CurrentToken?.Code != 12) // 'return'
            {
                AddError("Ожидалось '{'", CurrentToken);
                ParseReturnStatement();

                // Аналогично добавляем искусственную '}'
                if (CurrentToken == null || CurrentToken.Code != 13)
                {
                    string errorPosition = "end";
                    if (CurrentToken != null && CurrentToken.Code == 11) // Если это ')'
                    {
                        AddError("Ожидалась '}'", CurrentToken);
                        errorPosition = CurrentToken.Position;
                    }
                    AddError("Ожидалась '}'", new Token { Position = errorPosition });
                }
                else
                {
                    MoveNext();
                }
            }
            else
            {
                AddError("Ожидалось '{'", CurrentToken);
            }
        }
    }

    private void CheckFunctionBodyEnd()
    {
        if (CurrentToken?.Code != 13) // Если нет '}'
        {
            // Создаем искусственный токен для ошибки
            Token endToken = new Token
            {
                Code = 13,
                Lexeme = "конец тела функции",
                Position = CurrentToken != null ? CurrentToken.Position : "end"
            };
            AddError("Ожидалась '}'", endToken);

            // Пропускаем до точки с запятой
            while (CurrentToken != null && CurrentToken.Code != 14)
                MoveNext();
        }
        else
        {
            MoveNext();
        }
    }


    private void ParseParameters()
    {
        _parameters.Clear();
        bool expectParam = true;
        int safetyCounter = 0;
        const int maxIterations = 100;

        while (CurrentToken != null &&
               CurrentToken.Code != 11 &&    // not ')'
               CurrentToken.Code != 12 &&    // not '{'
               safetyCounter++ < maxIterations)
        {
            if (CurrentToken.Code == 15) // Проверка на неожиданный токен
            {
                
                MoveNext();
                continue;
            }

            if (expectParam)
            {
                if (CurrentToken.Code == 10) // IDENT
                {
                    // Проверка что параметр не начинается с цифры
                    if (CurrentToken.Lexeme.Length > 0 && char.IsDigit(CurrentToken.Lexeme[0]))
                    {
                        AddError("Идентификатор параметра не может начинаться с цифры", CurrentToken);
                    }

                    _parameters.Add(CurrentToken.Lexeme);
                    MoveNext();
                    expectParam = false;

                    // После параметра ожидаем либо запятую, либо закрывающую скобку, либо '{'
                    if (CurrentToken?.Code == 10) // Следующий токен тоже идентификатор
                    {
                        AddError("Ожидалась ',' между идентификаторами", CurrentToken);
                        // Продолжаем как будто запятая есть
                        expectParam = true;
                    }
                    else if (CurrentToken?.Code != 17 &&
                             CurrentToken?.Code != 11 &&
                             CurrentToken?.Code != 12)
                    {
                        // Не запятая, не закрывающая скобка и не '{' - пропускаем без ошибки
                        expectParam = true;
                    }
                }
                else if (CurrentToken.Code == 17) // ','
                {
                    // Лишняя запятая
                    if (_currentIndex > 0 && _tokens[_currentIndex - 1].Code == 17) // Previous was also comma
                    {
                        AddError("Ожидался параметр после ','", CurrentToken);
                        MoveNext();
                    }
                    else if (_currentIndex == 0 || // No parameters before comma
                            _tokens[_currentIndex - 1].Code == 9) // Previous was '('
                    {
                        AddError("Ожидался параметр перед ','", CurrentToken);
                        MoveNext();
                    }
                    else // Normal case of comma between parameters
                    {
                        MoveNext();
                        expectParam = true;
                    }
                }
                else if (CurrentToken.Code == 12) // '{' - неожиданное начало тела функции
                {
                    // Если перед '{' была запятая
                    if (_currentIndex > 0 && _tokens[_currentIndex - 1].Code == 17)
                    {
                        AddError("Ожидался параметр после ','", _tokens[_currentIndex - 1]);
                    }
                    break;
                }
                else
                {
                    break;
                }
            }
            else
            {
                if (CurrentToken.Code == 17) // ','
                {
                    MoveNext();
                    expectParam = true;
                }
                else if (CurrentToken.Code == 10) // IDENT без запятой
                {
                    AddError("Ожидалась ',' между идентификаторами", CurrentToken);
                    // Продолжаем как будто запятая есть
                    expectParam = true;
                }
                else if (CurrentToken.Code != 11 && CurrentToken.Code != 12)
                {
                    // Не запятая, не закрывающая скобка и не '{' - пропускаем без ошибки
                    expectParam = true;
                }
                else
                {
                    // Встретили '}' или '{' - выходим
                    break;
                }
            }
        }

        if (safetyCounter >= maxIterations)
        {
            AddError("Ошибка разбора параметров (возможное зацикливание)", CurrentToken);
        }

        // Проверка на trailing comma выполняется только при наличии ')'
        if (CurrentToken?.Code == 11 &&
            _currentIndex > 0 &&
            _tokens[_currentIndex - 1].Code == 17)
        {
            AddError("Ожидался параметр после ','", _tokens[_currentIndex - 1]);
        }
        else if (CurrentToken?.Code == 12 &&
                 _currentIndex > 0 &&
                 _tokens[_currentIndex - 1].Code == 17)
        {
            AddError("Ожидался параметр после ','", _tokens[_currentIndex - 1]);
        }
    }


    private void ParseReturnStatement()
    {
        if (CurrentToken == null) return;

        if (CurrentToken.Code == 6) // Правильное ключевое слово
        {
            MoveNext();
        }
        else
        {
            // Проверяем, похоже ли на function (начинается с 'func')
            bool looksLikeReturn = CurrentToken.Lexeme?.StartsWith("", StringComparison.Ordinal) ?? false;
            if (CurrentToken.Lexeme.StartsWith("return", StringComparison.Ordinal) && CurrentToken.Code == 10)
            {
                AddError("Ожидался пробел после return", CurrentToken);
            }
            else if (looksLikeReturn && CurrentToken.Code != 15)
            {
                AddError("Ожидалось ключевое слово 'return'", CurrentToken);
            }
            else
            {

            }

            MoveNext();
        }

        ParseExpression();
        
    }

    private void ParseExpression()
    {
        if (CurrentToken != null && (CurrentToken.Code == 16 || CurrentToken.Code == 18 || CurrentToken.Code == 19 || CurrentToken.Code == 20))
        {
            AddError("Выражение не может начинаться с оператора", CurrentToken);
            MoveNext();
        }
        while (CurrentToken != null && CurrentToken.Code != 13 && CurrentToken.Code != 14) // '}' или ';'
        {
            // Проверка на неожиданный токен (code == 15)
            if (CurrentToken.Code == 15)
            {
                
                MoveNext();
                continue;
            }

            switch (CurrentToken.Code)
            {
                case 9: // '('
                    MoveNext();
                    ParseExpression();
                    if (CurrentToken == null || CurrentToken.Code != 11) // ')'
                    {
                        AddError("Ожидалась ')'", CurrentToken ?? new Token { Position = "end" });
                        return;
                    }
                    MoveNext();
                    break;

                case 10: // IDENT
                case 21: // NUMBER
                    var prevToken = _currentIndex > 0 ? _tokens[_currentIndex - 1] : null;
                    MoveNext();

                    if (CurrentToken != null && CurrentToken.Code != 15 &&
                        (CurrentToken.Code == 10 || CurrentToken.Code == 21 || CurrentToken.Code == 9))
                    {
                        AddError("Ожидался оператор между выражениями", CurrentToken);
                    }
                    break;

                case 11: // ')'
                         // Проверяем, есть ли соответствующая открывающая скобка
                    if (!HasMatchingOpeningBracket())
                    {
                        AddError("Лишняя ')'", CurrentToken);
                        MoveNext();
                        return;
                    }
                    return;

                case 16: // '+'
                case 18: // '-'
                case 19: // '*'
                case 20: // '/'
                    int currentOp = CurrentToken.Code;
                    MoveNext();

                    if (CurrentToken != null &&
                        (CurrentToken.Code == 16 || CurrentToken.Code == 18 ||
                         CurrentToken.Code == 19 || CurrentToken.Code == 20))
                    {
                        AddError("Ожидался идентификатор между операторами", CurrentToken);
                        continue;
                    }

                    if (CurrentToken != null && CurrentToken.Code != 15 &&
                        CurrentToken.Code != 10 && CurrentToken.Code != 21 && CurrentToken.Code != 9)
                    {
                        AddError("Ожидалось выражение после оператора", CurrentToken);
                    }
                    break;

                default:
                    AddError("Ожидалась часть выражения", CurrentToken);
                    MoveNext();
                    break;
            }
        }
    }

    private bool HasMatchingOpeningBracket()
    {
        // Ищем соответствующую открывающую скобку
        int bracketCount = 1;
        for (int i = _currentIndex - 1; i >= 0; i--)
        {
            if (_tokens[i].Code == 11) // ')'
                bracketCount++;
            else if (_tokens[i].Code == 9) // '('
                bracketCount--;

            if (bracketCount == 0)
                return true;
        }
        return false;
    }

    private bool IsInvalidIdentifier(string lexeme)
    {
        // Проверяем, что первый символ - цифра, а после идут буквы/подчеркивание
        if (lexeme.Length == 0) return false;

        // Если первый символ цифра, а в строке есть буквы/подчеркивание
        if (char.IsDigit(lexeme[0]))
        {
            return lexeme.Skip(1).Any(c => char.IsLetter(c) || c == '_');
        }
        return false;
    }

    private void End()
    {
        if (CurrentToken == null || CurrentToken.Code != 14 )
        {
            // Создаем искусственный токен для ошибки
            Token endToken = new Token
            {
                Code = 0, // Специальный код для отсутствующего токена
                Lexeme = "конец функции",
                Position = CurrentToken != null ? CurrentToken.Position : "end"
            };
            AddError("Ожидался ';' после объявления функции", endToken);
        }
        else
        {
            MoveNext();
        }
    }
}
