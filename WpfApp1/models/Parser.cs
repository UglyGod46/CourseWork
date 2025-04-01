using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfApp1.models
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _currentTokenIndex;
        private readonly List<string> _errors = new List<string>();
        private readonly HashSet<string> _parameters = new HashSet<string>();
        private Token CurrentToken => _currentTokenIndex < _tokens.Count ? _tokens[_currentTokenIndex] : null;
        private bool EndOfTokens => _currentTokenIndex >= _tokens.Count;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens.Where(t => t.Type != TokenType.Separator).ToList();
            _currentTokenIndex = 0;
        }

        public ParseResult Parse()
        {
            _errors.Clear();

            if (_tokens.Count == 0)
            {
                return new ParseResult { IsValid = true, Errors = new List<string>() };
            }

            foreach (var errorToken in _tokens.Where(t => t.Type == TokenType.Error))
            {
                AddError($"Лексическая ошибка: {errorToken.Value}", errorToken.StartIndex, errorToken.EndIndex);
            }

            ParseFunction();

            return new ParseResult
            {
                IsValid = _errors.Count == 0,
                Errors = _errors
            };
        }

        private void ParseFunction()
        {
            if (CurrentToken == null)
            {
                AddError("Ожидалось ключевое слово 'function'", 0, 0);
                return;
            }

            if ((CurrentToken.Type == TokenType.Keyword && CurrentToken.Value == "function") ||
                (CurrentToken.Type == TokenType.Identifier && CurrentToken.Value == "function"))
            {
                _currentTokenIndex++;
            }
            else
            {
                AddError("Ожидалось ключевое слово 'function'",
                       CurrentToken.StartIndex, CurrentToken.EndIndex);
                _currentTokenIndex++;
            }

            if (CurrentToken?.Type == TokenType.Identifier)
            {
                if (char.IsDigit(CurrentToken.Value[0]))
                {
                    AddError("Имя функции не может начинаться с цифры",
                           CurrentToken.StartIndex, CurrentToken.EndIndex);
                }
                _currentTokenIndex++;
            }
            else
            {
                AddError("Ожидалось имя функции",
                       CurrentToken?.StartIndex ?? 0, CurrentToken?.EndIndex ?? 0);
            }

            if (!ExpectPunctuation("(", "Ожидалась '(' после имени функции"))
            {
                SkipToNextValidToken("{");
            }
            else
            {
                ParseParameterList();
                ExpectPunctuation(")", "Ожидалась ')' после параметров");
            }

            if (ExpectPunctuation("{", "Ожидалось '{' после параметров функции"))
            {
                ParseStatements();
                ExpectPunctuation("}", "Ожидалась '}' после тела функции");
                ExpectEndOfStatement(";", "Ожидалась ';' после '}'");
            }
        }

        private void ParseParameterList()
        {
            _parameters.Clear();

            if (CurrentToken?.Value == ")") return;

            if (CurrentToken?.Type == TokenType.Identifier)
            {
                _parameters.Add(CurrentToken.Value);
                _currentTokenIndex++;
            }
            else if (CurrentToken?.Value != ")")
            {
                AddError("Ожидался параметр или ')'",
                       CurrentToken?.StartIndex ?? 0, CurrentToken?.EndIndex ?? 0);
            }

            while (CurrentToken?.Value == ",")
            {
                _currentTokenIndex++;

                if (CurrentToken?.Type == TokenType.Identifier)
                {
                    if (!_parameters.Add(CurrentToken.Value))
                    {
                        AddError($"Дублирующийся параметр: '{CurrentToken.Value}'",
                               CurrentToken.StartIndex, CurrentToken.EndIndex);
                    }
                    _currentTokenIndex++;
                }
                else
                {
                    AddError("Ожидался идентификатор параметра после запятой",
                           CurrentToken?.StartIndex ?? 0, CurrentToken?.EndIndex ?? 0);
                }
            }
        }

        private void ParseStatements()
        {
            while (!EndOfTokens && CurrentToken?.Value != "}")
            {
                if (CurrentToken?.Type == TokenType.Identifier)
                {
                    if (IsReturnTypo(CurrentToken.Value))
                    {
                        AddError($"Опечатка в ключевом слове 'return': '{CurrentToken.Value}'",
                               CurrentToken.StartIndex, CurrentToken.EndIndex);
                        _currentTokenIndex++;
                        ParseReturnStatement();
                    }
                    else if (CurrentToken.Value.Equals("return", StringComparison.Ordinal))
                    {
                        _currentTokenIndex++;
                        ParseReturnStatement();
                    }
                    else
                    {
                        _currentTokenIndex++;
                    }
                }
                else
                {
                    _currentTokenIndex++;
                }
            }
        }

        private void ParseReturnStatement()
        {
            var returnToken = CurrentToken;
            _currentTokenIndex++; // Пропускаем "return" или его опечатку

            // Если после return сразу точка с запятой или конец
            if (EndOfTokens || CurrentToken?.Value == ";")
            {
                AddError("Ожидалось выражение после 'return'",
                       returnToken.EndIndex, returnToken.EndIndex + 1);
                return;
            }

            // Парсим выражение
            ParseExpression();

            // Проверяем, что после выражения идет ;
            if (CurrentToken?.Value != ";")
            {
                AddError("Ожидалась ';' после выражения return",
                       CurrentToken?.StartIndex ?? returnToken.EndIndex,
                       CurrentToken?.EndIndex ?? returnToken.EndIndex + 1);
            }
            else
            {
                _currentTokenIndex++;
            }
        }

        private bool IsValidExpressionStart()
        {
            return !EndOfTokens &&
                  (CurrentToken.Type == TokenType.Identifier ||
                   CurrentToken.Type == TokenType.Number ||
                   CurrentToken.Value == "(");
        }

        private void ParseExpression()
        {
            if (!IsValidExpressionStart())
            {
                AddError("Ожидалось выражение после оператора",
                       CurrentToken?.StartIndex ?? 0, CurrentToken?.EndIndex ?? 0);
                return;
            }

            ParseTerm();

            while (!EndOfTokens && (CurrentToken.Value == "+" || CurrentToken.Value == "-"))
            {
                var opToken = CurrentToken;
                _currentTokenIndex++;

                if (!IsValidExpressionStart())
                {
                    AddError($"Ожидалось выражение после оператора '{opToken.Value}'",
                           opToken.EndIndex, opToken.EndIndex + 1);
                    return;
                }

                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            ParseFactor();

            while (!EndOfTokens && (CurrentToken.Value == "*" || CurrentToken.Value == "/"))
            {
                var opToken = CurrentToken;
                _currentTokenIndex++;

                if (!IsValidExpressionStart())
                {
                    AddError($"Ожидалось выражение после оператора '{opToken.Value}'",
                           opToken.EndIndex, opToken.EndIndex + 1);
                    return;
                }

                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            if (CurrentToken?.Value == "(")
            {
                var bracketStart = CurrentToken.StartIndex;
                _currentTokenIndex++;
                ParseExpression();

                if (CurrentToken?.Value != ")")
                {
                    AddError("Ожидалась закрывающая скобка ')'",
                           bracketStart, CurrentToken?.EndIndex ?? bracketStart + 1);
                }
                else
                {
                    _currentTokenIndex++;
                }
            }
            else if (CurrentToken?.Type == TokenType.Identifier)
            {
                if (!_parameters.Contains(CurrentToken.Value))
                {
                    AddError($"Неизвестный параметр: '{CurrentToken.Value}'",
                           CurrentToken.StartIndex, CurrentToken.EndIndex);
                }
                _currentTokenIndex++;
            }
            else if (CurrentToken?.Type == TokenType.Number)
            {
                _currentTokenIndex++;
            }
            else if (CurrentToken?.Value != ";" && CurrentToken?.Value != ")" && CurrentToken?.Value != "}")
            {
                AddError("Ожидалось число, идентификатор или выражение в скобках",
                       CurrentToken?.StartIndex ?? 0, CurrentToken?.EndIndex ?? 0);
                _currentTokenIndex++;
            }
        }

        private bool IsOperator(string value) =>
            value == "+" || value == "-" || value == "*" || value == "/";

        private bool ExpectPunctuation(string punctuation, string errorMessage)
        {
            if (CurrentToken?.Value == punctuation)
            {
                _currentTokenIndex++;
                return true;
            }

            AddError(errorMessage, CurrentToken?.StartIndex ?? 0, CurrentToken?.EndIndex ?? 0);
            return false;
        }

        private bool ExpectEndOfStatement(string end, string errorMessage)
        {
            if (CurrentToken?.Value == end)
            {
                _currentTokenIndex++;
                return true;
            }

            AddError(errorMessage, CurrentToken?.StartIndex ?? 0, CurrentToken?.EndIndex ?? 0);
            return false;
        }

        private void SkipToNextValidToken(params string[] validTokens)
        {
            while (!EndOfTokens && !validTokens.Contains(CurrentToken?.Value))
            {
                _currentTokenIndex++;
            }
        }

        private bool IsFunctionTypo(string word)
        {
            string[] commonTypos = { "funcion", "functon", "funtion", "fuction", "functin", "functio" };
            return commonTypos.Contains(word.ToLower());
        }

        private bool IsReturnTypo(string word)
        {
            string[] commonTypos = { "retrn", "retrun", "retun", "reurn", "retur", "retun", "retur", "retu", "ret", "re", "r", "eturn", "rturn", "reurn", "retrn" };
            return commonTypos.Contains(word.ToLower());
        }

        private void AddError(string message, int start, int end)
        {
            _errors.Add($"{message} (позиции {start + 1}-{end + 1})");
        }
    }

    public class ParseResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
    }
}