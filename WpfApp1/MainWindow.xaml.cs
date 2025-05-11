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
            RegexOutputRichTextBox.Document.Blocks.Clear();
            string input = InputTextEditor.Text;

            // Используем все шаблоны
            var allResults = RegexMatcher.FindMatches(input);

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
            foreach (var result in allResults)
            {
                TableRow row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(GetPatternName(result.Pattern)))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(result.Match))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(result.StartIndex.ToString()))));
                dataGroup.Rows.Add(row);
            }

            if (allResults.Count == 0)
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
}