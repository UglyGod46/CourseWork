using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WpfApp1.Managers;
using WpfApp1.models;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private string currentFilePath;
        private readonly FileManager fileManager;
        private readonly EditManager editManager;
        private readonly HelpManager helpManager;
        private readonly TextManager textManager = new TextManager();


        private double _fontSize = 14;

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    // Обновляем размер шрифта для всех элементов
                    InputTextEditor.FontSize = value;
                    LexerOutputRichTextBox.FontSize = value;
                    ParserOutputRichTextBox.FontSize = value;
                    OnPropertyChanged(nameof(FontSize)); // Уведомляем об изменении
                }
            }
        }

        // Реализация INotifyPropertyChanged для уведомлений об изменении свойств
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            InputTextEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");

            fileManager = new FileManager(InputTextEditor, FileNameTextBlock);
            editManager = new EditManager(InputTextEditor);
            helpManager = new HelpManager();
        }



        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            fileManager.SaveAs(ref currentFilePath);
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            editManager.Redo();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            editManager.Undo();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            fileManager.OpenFile(ref currentFilePath);
        }

        private void CreateFile_Click(object sender, RoutedEventArgs e)
        {
            fileManager.CreateFile(ref currentFilePath);
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            fileManager.SaveFile(ref currentFilePath);
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            editManager.Cut();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            editManager.Copy();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            editManager.Paste();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            editManager.Delete();
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            editManager.SelectAll();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            helpManager.OpenUsersGuide();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            helpManager.ShowAbout();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TextTaskStatement_Click(object sender, RoutedEventArgs e)
        {
            textManager.OpenTaskStatement();
        }

        private void TextGrammar_Click(object sender, RoutedEventArgs e)
        {
            textManager.OpenGrammar();
        }

        private void TextGrammarClassification_Click(object sender, RoutedEventArgs e)
        {
            textManager.OpenGrammarClassification();
        }

        private void TextAnalysisMethod_Click(object sender, RoutedEventArgs e)
        {
            textManager.OpenAnalysisMethod();
        }

        private void TextErrorDiagnostics_Click(object sender, RoutedEventArgs e)
        {
            textManager.OpenErrorDiagnostics();
        }

        private void TextTestExample_Click(object sender, RoutedEventArgs e)
        {
            textManager.OpenTestExample();
        }

        private void TextBibliography_Click(object sender, RoutedEventArgs e)
        {
            textManager.OpenBibliography();
        }

        private void TextSourceCode_Click(object sender, RoutedEventArgs e)
        {
            textManager.OpenSourceCode();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputTextEditor.Text))
            {
                var result = MessageBox.Show("Вы хотите сохранить изменения?", "Подтверждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    fileManager.SaveAs(ref currentFilePath);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Pattern12Button_Click(object sender, RoutedEventArgs e)
        {
            ShowRegexMatches(RegexMatcher.pattern12, "Числа, не заканчивающиеся на 0");
        }

        private void Pattern19Button_Click(object sender, RoutedEventArgs e)
        {
            ShowRegexMatches(RegexMatcher.pattern19, "Идентификаторы");
        }

        private void Pattern22Button_Click(object sender, RoutedEventArgs e)
        {
            ShowRegexMatches(RegexMatcher.pattern22, "Автомобильные номера");
        }

        private void ShowRegexMatches(string pattern, string patternName)
        {
            RegexOutputRichTextBox.Document.Blocks.Clear();
            string input = InputTextEditor.Text;

            var results = new List<RegexMatchResult>();
            RegexMatcher.AddMatches(results, pattern, input);

            // Создаем FlowDocument для таблицы
            FlowDocument flowDoc = new FlowDocument();
            Table table = new Table();

            // Добавляем столбцы
            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            // Создаем группу строк для заголовка
            TableRowGroup headerGroup = new TableRowGroup();
            TableRow headerRow = new TableRow { Background = Brushes.LightGray };

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Шаблон")) { FontWeight = FontWeights.Bold }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Совпадение")) { FontWeight = FontWeights.Bold }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Позиция")) { FontWeight = FontWeights.Bold }));

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);

            // Добавляем данные
            TableRowGroup dataGroup = new TableRowGroup();
            foreach (var result in results)
            {
                TableRow row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(patternName))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(result.Match))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(result.StartIndex.ToString()))));
                dataGroup.Rows.Add(row);
            }

            if (results.Count == 0)
            {
                TableRow noMatchRow = new TableRow();
                noMatchRow.Cells.Add(new TableCell(new Paragraph(new Run("Совпадений не найдено")) { Foreground = Brushes.Gray }));
                noMatchRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                noMatchRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                dataGroup.Rows.Add(noMatchRow);
            }

            table.RowGroups.Add(dataGroup);
            flowDoc.Blocks.Add(table);
            RegexOutputRichTextBox.Document = flowDoc;
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ParserOutputRichTextBox.Document.Blocks.Clear();

                string input = InputTextEditor.Text;
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Введите текст для анализа", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var tokens = input.Split(new[] { ' ', '\t', '\n', '\r' },
                                      StringSplitOptions.RemoveEmptyEntries)
                                 .ToList();

                var parser = new RecursiveDescentParser();
                var (isValid, callStack, errors) = parser.Parse(tokens);

                FlowDocument flowDoc = new FlowDocument();

                // Заголовок
                Paragraph header = new Paragraph(new Run("Результат синтаксического анализа"))
                {
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                flowDoc.Blocks.Add(header);

                // Статус анализа
                flowDoc.Blocks.Add(new Paragraph(new Run($"Статус: {(isValid ? "✅ Корректное предложение" : "❌ Обнаружены синтаксические ошибки")}"))
                {
                    FontWeight = FontWeights.Bold,
                    Foreground = isValid ? Brushes.DarkGreen : Brushes.Red,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                // Вывод ошибок
                if (errors.Count > 0)
                {
                    Paragraph errorsHeader = new Paragraph(new Run("Обнаруженные ошибки:"))
                    {
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.DarkRed,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    flowDoc.Blocks.Add(errorsHeader);

                    foreach (var error in errors)
                    {
                        flowDoc.Blocks.Add(new Paragraph(new Run($"• {error}"))
                        {
                            Foreground = Brushes.Red,
                            Margin = new Thickness(20, 2, 0, 2)
                        });
                    }
                }

                // Разделитель
                flowDoc.Blocks.Add(new Paragraph(new Run("Процесс разбора:"))
                {
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                });

                // Вывод последовательности вызовов
                foreach (var call in callStack)
                {
                    Brush foreground = Brushes.Black;
                    if (call.Contains("λ-правило")) foreground = Brushes.Gray;
                    if (call.Contains("[Ошибка]")) foreground = Brushes.Red;
                    if (call.Contains("Соответствие")) foreground = Brushes.DarkGreen;

                    flowDoc.Blocks.Add(new Paragraph(new Run(call))
                    {
                        FontFamily = new FontFamily("Consolas"),
                        Margin = new Thickness(0),
                        Foreground = foreground
                    });
                }

                ParserOutputRichTextBox.Document = flowDoc;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при анализе: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetPatternName(string pattern)
        {
            if (pattern == RegexMatcher.pattern12) return "Числа без нуля";
            if (pattern == RegexMatcher.pattern19) return "Идентификаторы";
            if (pattern == RegexMatcher.pattern22) return "Номера авто";
            return "Неизвестный шаблон";
        }




        private int GetPositionValue(string position)
        {
            if (string.IsNullOrEmpty(position) || position == "end")
                return int.MaxValue;

            // Обрабатываем позиции вида "10" или "12-16"
            if (position.Contains("-"))
            {
                var parts = position.Split('-');
                if (parts.Length > 0 && int.TryParse(parts[0], out int start))
                    return start;
            }
            else if (int.TryParse(position, out int singlePos))
            {
                return singlePos;
            }

            return int.MaxValue - 1; // Нечисловые позиции перед "end"
        }
    }

    public class RecursiveDescentParser
    {
        private List<string> tokens;
        private int currentTokenIndex;
        private List<string> callStack;
        private List<string> errors;
        private int indentLevel;

        private readonly HashSet<string> nouns = new HashSet<string> { "flight", "passenger", "trip", "morning" };
        private readonly HashSet<string> verbs = new HashSet<string> { "is", "prefers", "like", "need", "depend", "fly" };
        private readonly HashSet<string> adjectives = new HashSet<string> { "non-stop", "first", "direct" };

        public (bool isValid, List<string> callStack, List<string> errors) Parse(List<string> inputTokens)
        {
            tokens = inputTokens;
            currentTokenIndex = 0;
            callStack = new List<string>();
            errors = new List<string>();
            indentLevel = 0;

            try
            {
                LogCall("Начало разбора");
                S();

                if (currentTokenIndex < tokens.Count)
                {
                    AddError($"Ожидался конец предложения, но найдены лишние токены: {string.Join(" ", tokens.Skip(currentTokenIndex))}");
                }

                LogReturn();
            }
            catch (Exception ex)
            {
                AddError($"Критическая ошибка: {ex.Message}");
            }

            return (errors.Count == 0, callStack, errors);
        }

        private void S()
        {
            LogCall("S -> <Noun phrase> <Verb phrase>");
            var npResult = NounPhrase();
            var vpResult = VerbPhrase();

            if (!npResult || !vpResult)
            {
                AddError("Неполная структура предложения");
            }
            LogReturn();
        }

        private bool NounPhrase()
        {
            LogCall("NounPhrase -> <Noun> | <Adjective phrase> <Noun> | λ");

            if (currentTokenIndex >= tokens.Count)
            {
                LogCall("Применено λ-правило: NounPhrase -> λ (нет токенов)");
                LogReturn();
                return true;
            }

            // Сохраняем позицию для возможного отката
            int startPos = currentTokenIndex;
            bool hasAdjectives = false;

            // Пробуем вариант с прилагательными
            if (IsAdjective(tokens[currentTokenIndex]))
            {
                hasAdjectives = true;
                AdjectivePhrase();
            }

            // После прилагательных должно быть существительное
            if (hasAdjectives)
            {
                if (currentTokenIndex < tokens.Count && IsNoun(tokens[currentTokenIndex]))
                {
                    MatchNoun();
                    LogReturn();
                    return true;
                }
                else
                {
                    AddError($"Ожидалось существительное после прилагательных");
                    currentTokenIndex = startPos; // Откат к началу
                    LogReturn();
                    return false;
                }
            }

            // Вариант с существительным
            if (IsNoun(tokens[currentTokenIndex]))
            {
                MatchNoun();
                LogReturn();
                return true;
            }

            // λ-правило
            LogCall("Применено λ-правило: NounPhrase -> λ (нет подходящих токенов)");
            LogReturn();
            return true;
        }

        private bool VerbPhrase()
        {
            LogCall("VerbPhrase -> <Verb> <Noun phrase>");

            if (currentTokenIndex >= tokens.Count)
            {
                AddError("Ожидался глагол, но достигнут конец предложения");
                LogReturn();
                return false;
            }

            if (IsVerb(tokens[currentTokenIndex]))
            {
                MatchVerb();
                bool npResult = NounPhrase();
                LogReturn();
                return npResult;
            }
            else
            {
                AddError($"Ожидался глагол, но найдено '{tokens[currentTokenIndex]}'");
                LogReturn();
                return false;
            }
        }

        private void AdjectivePhrase()
        {
            LogCall("AdjectivePhrase -> <Adjective phrase> <Adjective> | λ");

            // Рекурсивно собираем все прилагательные
            while (currentTokenIndex < tokens.Count && IsAdjective(tokens[currentTokenIndex]))
            {
                MatchAdjective();
            }

            LogCall("AdjectivePhrase завершено (λ или все прилагательные обработаны)");
            LogReturn();
        }

        private void MatchNoun()
        {
            if (currentTokenIndex < tokens.Count && IsNoun(tokens[currentTokenIndex]))
            {
                LogCall($"Соответствие: Noun -> '{tokens[currentTokenIndex]}'");
                currentTokenIndex++;
            }
            else
            {
                AddError($"Ожидалось существительное, но найдено: {(currentTokenIndex < tokens.Count ? tokens[currentTokenIndex] : "конец предложения")}");
            }
        }

        private void MatchVerb()
        {
            if (currentTokenIndex < tokens.Count && IsVerb(tokens[currentTokenIndex]))
            {
                LogCall($"Соответствие: Verb -> '{tokens[currentTokenIndex]}'");
                currentTokenIndex++;
            }
            else
            {
                AddError($"Ожидался глагол, но найдено: {(currentTokenIndex < tokens.Count ? tokens[currentTokenIndex] : "конец предложения")}");
            }
        }

        private void MatchAdjective()
        {
            if (currentTokenIndex < tokens.Count && IsAdjective(tokens[currentTokenIndex]))
            {
                LogCall($"Соответствие: Adjective -> '{tokens[currentTokenIndex]}'");
                currentTokenIndex++;
            }
            else
            {
                AddError($"Ожидалось прилагательное, но найдено: {(currentTokenIndex < tokens.Count ? tokens[currentTokenIndex] : "конец предложения")}");
            }
        }

        private bool IsNoun(string token) => nouns.Contains(token.ToLower());
        private bool IsVerb(string token) => verbs.Contains(token.ToLower());
        private bool IsAdjective(string token) => adjectives.Contains(token.ToLower());

        private void LogCall(string message)
        {
            callStack.Add($"{new string(' ', indentLevel * 2)}Вход: {message}");
            indentLevel++;
        }

        private void LogReturn()
        {
            indentLevel--;
            if (indentLevel >= 0 && callStack.Count > 0)
            {
                var lastCall = callStack.LastOrDefault(c => c.StartsWith(new string(' ', indentLevel * 2) + "Вход: "));
                if (lastCall != null)
                {
                    callStack.Add($"{new string(' ', indentLevel * 2)}Выход: {lastCall.Substring(lastCall.IndexOf(':') + 2)}");
                }
            }
        }

        private void AddError(string errorMessage)
        {
            string error = $"{new string(' ', indentLevel * 2)}[Ошибка] {errorMessage}";
            errors.Add(error);
            callStack.Add(error);
        }
    }
}

