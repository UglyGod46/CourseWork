using System;
using System.Collections.Generic;
using System.Linq;
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

    private bool Check(int expectedCode, string expectedName)
    {
        if (CurrentToken?.Code == expectedCode) return true;
        AddError(expectedName, CurrentToken);
        return false;
    }

    private bool Check1(int expectedCode, string expectedName)
    {
        if (CurrentToken?.Code == expectedCode) return true;
        AddError1(expectedName, CurrentToken);
        return false;
    }

    public void Parse()
    {
        ParseFunction();
        End(); // Проверяем точку с запятой после функции

        // Проверяем, что больше нет токенов
        if (CurrentToken != null)
        {
            AddError("Неожиданный токен после функции", CurrentToken);
        }
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
            bool looksLikeFunction = CurrentToken.Lexeme?.StartsWith("func", StringComparison.Ordinal) ?? false;
            if (CurrentToken.Code == 15)
            {
                AddError("Неожиданный токен", CurrentToken);
            }
            else if (CurrentToken.Lexeme.StartsWith("function", StringComparison.Ordinal) && CurrentToken.Code == 10)
            {
                AddError("Ожидался пробел после function", CurrentToken);
            }
            else if (looksLikeFunction)
            {
                AddError("Ожидалось ключевое слово 'function'", CurrentToken);
            }
            else
            {
                AddError("Ожидалось объявление функции (ожидалось 'function')", CurrentToken);
            }

            // Продолжаем анализ, даже если ключевое слово неверное
            MoveNext(); 
        }

        if (CurrentToken?.Code == 15)
        {
            AddError("Неожиданный токен", CurrentToken);
            MoveNext();
        }

        // 2. Проверяем имя функции

        if (CurrentToken.Code == 10)
        {
            MoveNext();
        } else if (CurrentToken.Code == 15)
        {
            AddError("Неожиданный токен", CurrentToken);
            MoveNext();
        }
        else
        {
            AddError("Ожидалось имя функции", CurrentToken);
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

        // 4. Проверяем тело функции
        if (CurrentToken?.Code == 12) // '{'
        {
            MoveNext();
            ParseReturnStatement();

            if (!Check1(13, "'}'"))
            {
                // Пропускаем до ';'
                while (CurrentToken != null && CurrentToken.Code != 14)
                    MoveNext();
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

                if (!Check1(13, "'}'"))
                {
                    // Пропускаем до ';'
                    while (CurrentToken != null && CurrentToken.Code != 14)
                        MoveNext();
                }
                else
                {
                    MoveNext();
                }
            }
            else
            {
                AddError("'{'", CurrentToken);
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
                AddError("Неожиданный токен в параметрах", CurrentToken);
                MoveNext();
                continue;
            }

            if (expectParam)
            {
                if (CurrentToken.Code == 10) // IDENT
                {
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

        if (CurrentToken.Code == 6) // Правильное ключевое слово return
        {
            MoveNext();
        }
        else
        {
            // Проверяем, похоже ли на return (начинается с 'r' или 'ret')
            bool looksLikeReturn = CurrentToken.Lexeme?.StartsWith("r", StringComparison.Ordinal) ?? false;
            if(CurrentToken.Code == 15)
            {
                AddError("Неожиданный токен", CurrentToken);
            }
            else if (looksLikeReturn)
            {
                AddError("Ожидался 'return'", CurrentToken);
            }
            else
            {
                AddError("Ожидался оператор return", CurrentToken);
            }

            // Продолжаем анализ, даже если ключевое слово неверное
            MoveNext();
        }

        ParseExpression();
    }

    private void ParseExpression()
    {
        while (CurrentToken != null && CurrentToken.Code != 13 && CurrentToken.Code != 14) // '}' или ';'
        {
            // Проверка на неожиданный токен (code == 15)
            if (CurrentToken.Code == 15)
            {
                AddError("Неожиданный токен в выражении", CurrentToken);
                MoveNext();
                continue;
            }

            switch (CurrentToken.Code)
            {
                case 9: // '('
                    MoveNext();
                    ParseExpression();
                    if (!Check1(11, "')'")) return;
                    MoveNext();
                    break;

                case 10: // IDENT
                case 21: // NUMBER
                         // Сохраняем предыдущий токен
                    var prevToken = _currentIndex > 0 ? _tokens[_currentIndex - 1] : null;
                    MoveNext();

                    // Пропускаем проверку если следующий токен неожиданный (code == 15)
                    if (CurrentToken != null && CurrentToken.Code != 15 &&
                        (CurrentToken.Code == 10 || CurrentToken.Code == 21 || CurrentToken.Code == 9))
                    {
                        // Между двумя идентификаторами/числами/скобкой должен быть оператор
                        AddError("Ожидался оператор между выражениями", CurrentToken);
                    }
                    break;

                case 16:
                case 18:
                case 19:
                case 20: // Операторы
                    MoveNext();
                    // Пропускаем проверку если следующий токен неожиданный (code == 15)
                    if (CurrentToken != null && CurrentToken.Code != 15 &&
                        CurrentToken.Code != 10 && CurrentToken.Code != 21 && CurrentToken.Code != 9)
                    {
                        AddError("Ожидалось выражение после оператора", CurrentToken);
                    }
                    break;

                case 11: // ')'
                    return;

                default:
                    AddError("Ожидалась часть выражения", CurrentToken);
                    MoveNext();
                    break;
            }
        }
    }


    private void End()
    {
        int lengthHistory = Errors.Count;
        bool result = Check1(14, "';'");

        if (!result && lengthHistory == Errors.Count)
        {
            // Создаем искусственный токен для ошибки
            Token endToken = new Token
            {
                Code = 14,
                Lexeme = "конец функции",
                Position = CurrentToken != null ? CurrentToken.Position : "end"
            };
            AddError("Ожидался ';' после объявления функции", endToken);
        }
        else if (result)
        {
            MoveNext(); // Пропускаем точку с запятой
        }
    }
}
