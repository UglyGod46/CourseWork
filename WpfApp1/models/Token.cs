using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.models
{
    public enum TokenType
    {
        Keyword,         // Ключевые слова (function, return)
        Identifier,      // Идентификаторы (имена переменных, функций)
        Number,          // Числа
        Operator,        // Операторы (+, -, *, /, =)
        Punctuation,     // Разделители ((), {}, ,)
        EndOfStatement,  // Конец оператора (;)
        Invalid,         // Недопустимые символы
        Error            // Ошибка (например, первое слово не function)
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public override string ToString()
        {
            return $"{(int)Type} - {Type} - {Value} - с {StartIndex + 1} по {EndIndex + 1} символ";
        }
    }
}