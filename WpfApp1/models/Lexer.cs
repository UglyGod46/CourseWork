using System.Collections.Generic;
using System;
using WpfApp1.models;
using System.Linq;

public class Lexer
{
    private readonly Dictionary<string, int> _keywords = new Dictionary<string, int>
    {
    { "function", 1 }, { "return", 6 }
    };

    // Допустимые символы для идентификаторов (только буквы, цифры и _)
    private readonly HashSet<char> _validIdChars = new HashSet<char>(
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789"
    );

    private bool _expectSpace = false;

    public List<Token> Analyze(string input)
    {
        var tokens = new List<Token>();
        int pos = 0;
        _expectSpace = false;

        while (pos < input.Length)
        {
            char current = input[pos];

            // Пропускаем пробелы и переводы строк
            if (char.IsWhiteSpace(current))
            {
                if (_expectSpace && current == ' ')
                {
                    tokens.Add(new Token
                    {
                        Code = 7,
                        Type = "Пробел",
                        Lexeme = " ",
                        Position = $"{pos + 1}"
                    });
                    _expectSpace = false;
                }
                pos++;
                continue;
            }

            // Обработка идентификаторов и ключевых слов
            if (char.IsLetter(current) || current == '_' || char.IsDigit(current))
            {
                int start = pos;
                bool isValidId = true;
                bool startsWithDigit = char.IsDigit(current); // Проверка на начало с цифры

                // Собираем весь идентификатор
                while (pos < input.Length &&
                      (char.IsLetterOrDigit(input[pos]) || input[pos] == '_' || !IsSeparator(input[pos])))
                {
                    if (!_validIdChars.Contains(input[pos]))
                        isValidId = false;
                    pos++;
                }

                string value = input.Substring(start, pos - start);

                // Если начинается с цифры, но содержит буквы - это невалидный идентификатор
                if (startsWithDigit && value.Any(char.IsLetter))
                {
                    tokens.Add(new Token
                    {
                        Code = 15,
                        Type = "Ошибка",
                        Lexeme = value,
                        Position = $"{start + 1}-{pos}",
                        ErrorMessage = "Идентификатор не может начинаться с цифры"
                    });
                    continue;
                }

                // Проверяем на ключевые слова только если нет ошибок
                if (isValidId && !startsWithDigit)
                {
                    if (_keywords.TryGetValue(value, out int code))
                    {
                        tokens.Add(new Token
                        {
                            Code = code,
                            Type = "Ключевое слово",
                            Lexeme = value,
                            Position = $"{start + 1}-{pos}"
                        });

                        if (code == 1 || code == 6) _expectSpace = true;
                    }
                    else
                    {
                        tokens.Add(new Token
                        {
                            Code = 10,
                            Type = "Идентификатор",
                            Lexeme = value,
                            Position = $"{start + 1}-{pos}"
                        });
                    }
                }
                else if (!startsWithDigit) // Если не начинается с цифры, но есть другие ошибки
                {
                    tokens.Add(new Token
                    {
                        Code = 15,
                        Type = "Ошибка",
                        Lexeme = value,
                        Position = $"{start + 1}-{pos}"
                    });
                }
                continue;
            }

            // Числа
            if (char.IsDigit(current))
            {
                int start = pos;
                while (pos < input.Length && char.IsDigit(input[pos]))
                    pos++;

                tokens.Add(new Token
                {
                    Code = 21,
                    Type = "Число",
                    Lexeme = input.Substring(start, pos - start),
                    Position = $"{start + 1}-{pos}"
                });
                continue;
            }

            // Обработка операторов и символов
            switch (current)
            {
                case '+':
                    tokens.Add(new Token { Code = 16, Type = "Оператор", Lexeme = "+", Position = $"{pos + 1}" });
                    break;
                case '-':
                    tokens.Add(new Token { Code = 18, Type = "Оператор", Lexeme = "-", Position = $"{pos + 1}" });
                    break;
                case '*':
                    tokens.Add(new Token { Code = 19, Type = "Оператор", Lexeme = "*", Position = $"{pos + 1}" });
                    break;
                case '/':
                    tokens.Add(new Token { Code = 20, Type = "Оператор", Lexeme = "/", Position = $"{pos + 1}" });
                    break;
                case '(':
                    tokens.Add(new Token { Code = 9, Type = "Начало параметров", Lexeme = "(", Position = $"{pos + 1}" });
                    break;
                case ')':
                    tokens.Add(new Token { Code = 11, Type = "Конец параметров", Lexeme = ")", Position = $"{pos + 1}" });
                    break;
                case '{':
                    tokens.Add(new Token { Code = 12, Type = "Начало тела", Lexeme = "{", Position = $"{pos + 1}" });
                    break;
                case '}':
                    tokens.Add(new Token { Code = 13, Type = "Конец тела", Lexeme = "}", Position = $"{pos + 1}" });
                    break;
                case ',':
                    tokens.Add(new Token { Code = 17, Type = "Запятая", Lexeme = ",", Position = $"{pos + 1}" });
                    break;
                case ';':
                    tokens.Add(new Token { Code = 14, Type = "Конец объявления", Lexeme = ";", Position = $"{pos + 1}" });
                    break;
                default:
                    // Одиночные недопустимые символы
                    tokens.Add(new Token
                    {
                        Code = 15,
                        Type = "Ошибка",
                        Lexeme = current.ToString(),
                        Position = $"{pos + 1}"
                    });
                    break;
            }

            pos++;
            _expectSpace = false;
        }

        return tokens;
    }

    private bool IsSeparator(char c)
    {
        return char.IsWhiteSpace(c) ||
               c == '+' || c == '-' || c == '*' || c == '/' ||
               c == '(' || c == ')' || c == '{' || c == '}' ||
               c == ',' || c == ';';
    }
}