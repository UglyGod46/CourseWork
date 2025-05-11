using System;
using System.Collections.Generic;
using WpfApp1.models;

public class LicensePlateMatcher
{
    // Все допустимые буквы в российских номерах (верхний и нижний регистр)
    private static readonly HashSet<char> ValidLetters = new HashSet<char>
    {
        'А', 'В', 'Е', 'К', 'М', 'Н', 'О', 'Р', 'С', 'Т', 'У', 'Х',
        'а', 'в', 'е', 'к', 'м', 'н', 'о', 'р', 'с', 'т', 'у', 'х'
    };

    public static List<RegexMatchResult> FindMatches(string text)
    {
        var results = new List<RegexMatchResult>();
        if (string.IsNullOrEmpty(text)) return results;

        int index = 0;
        while (index < text.Length)
        {
            if (IsValidLetter(text[index]))
            {
                int start = index;
                bool isValid = true;

                // Проверяем строгий формат: Б ЦЦЦ ББ ЦЦ (8 символов)
                if (start + 8 <= text.Length)
                {
                    // Проверяем позиции согласно формату
                    isValid = IsValidLetter(text[start]) &&           // 1-я позиция - буква
                             char.IsDigit(text[start + 1]) &&        // 2-я - цифра
                             char.IsDigit(text[start + 2]) &&        // 3-я - цифра
                             char.IsDigit(text[start + 3]) &&        // 4-я - цифра
                             IsValidLetter(text[start + 4]) &&       // 5-я - буква
                             IsValidLetter(text[start + 5]) &&       // 6-я - буква
                             char.IsDigit(text[start + 6]) &&       // 7-я - цифра
                             char.IsDigit(text[start + 7]);          // 8-я - цифра

                    if (isValid)
                    {
                        string match = text.Substring(start, 8);
                        results.Add(new RegexMatchResult
                        {
                            Pattern = "Автомобильный номер (строгий формат)",
                            Match = match,
                            StartIndex = start
                        });
                        index = start + 8;
                        continue;
                    }
                }
            }
            index++;
        }
        return results;
    }

    private static bool IsValidLetter(char c)
    {
        return ValidLetters.Contains(c);
    }
}