using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WpfApp1.models
{
    public class RegexMatchResult
    {
        public string Pattern { get; set; }
        public string Match { get; set; }
        public int StartIndex { get; set; }
    }

    public static class LicensePlateMatcher
    {
        private static readonly HashSet<char> ValidLetters = new HashSet<char>
        {
            'А', 'В', 'Е', 'К', 'М', 'Н', 'О', 'Р', 'С', 'Т', 'У', 'Х',
            'а', 'в', 'е', 'к', 'м', 'н', 'о', 'р', 'с', 'т', 'у', 'х'
        };

        public static List<RegexMatchResult> FindMatches(string text)
        {
            var results = new List<RegexMatchResult>();
            if (string.IsNullOrEmpty(text)) return results;

            for (int i = 0; i <= text.Length - 8; i++)
            {
                if (!IsValidLetter(text[i])) continue;

                // Проверяем формат: Буква + 3 цифры + 2 буквы + 2 цифры
                bool isValid = true;
                for (int j = 0; j < 8; j++)
                {
                    bool shouldBeLetter = (j == 0 || j == 4 || j == 5);
                    bool shouldBeDigit = (j == 1 || j == 2 || j == 3 || j == 6 || j == 7);

                    if (shouldBeLetter && !IsValidLetter(text[i + j]))
                    {
                        isValid = false;
                        break;
                    }
                    if (shouldBeDigit && !char.IsDigit(text[i + j]))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    // Проверяем границы слова
                    bool isWordBoundary = true;
                    if (i > 0 && char.IsLetterOrDigit(text[i - 1]))
                        isWordBoundary = false;
                    if (i + 8 < text.Length && char.IsLetterOrDigit(text[i + 8]))
                        isWordBoundary = false;

                    if (isWordBoundary)
                    {
                        results.Add(new RegexMatchResult
                        {
                            Pattern = "Автомобильный номер (автомат)",
                            Match = text.Substring(i, 8),
                            StartIndex = i
                        });
                        i += 7; // Пропускаем найденный номер
                    }
                }
            }
            return results;
        }

        private static bool IsValidLetter(char c)
        {
            return ValidLetters.Contains(c);
        }
    }

    public static class RegexMatcher
    {
        public const string pattern12 = @"\b\d*[1-9]\b";
        public const string pattern19 = @"\b[a-zA-Z$_][a-zA-Z0-9]*\b";
        public const string pattern22 = @"\b[АВЕКМНОРСТУХа-рстух]\d{3}[АВЕКМНОРСТУХа-рстух]{2}\d{2}\b";

        public static List<RegexMatchResult> FindMatches(string text)
        {
            var results = new List<RegexMatchResult>();
            AddMatches(results, pattern12, text);
            AddMatches(results, pattern19, text);
            AddMatches(results, pattern22, text);
            return results;
        }

        public static void AddMatches(List<RegexMatchResult> list, string pattern, string text)
        {
            var regex = new Regex(pattern);
            foreach (Match match in regex.Matches(text))
            {
                list.Add(new RegexMatchResult
                {
                    Pattern = pattern,
                    Match = match.Value,
                    StartIndex = match.Index
                });
            }
        }
    }
}