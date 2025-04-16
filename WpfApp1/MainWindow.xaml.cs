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
            helpManager.ShowHelp();
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

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем RichTextBox
            LexerOutputRichTextBox.Document.Blocks.Clear();
            ParserOutputRichTextBox.Document.Blocks.Clear();

            string input = InputTextEditor.Text;

            // Лексический анализ
            Lexer lexer = new Lexer();
            List<Token> tokens = lexer.Analyze(input);

            // Вывод токенов в виде таблицы
            var lexerTable = new Table();
            lexerTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            lexerTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            lexerTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            var lexerHeaderRowGroup = new TableRowGroup();
            var lexerHeaderRow = new TableRow { Background = Brushes.LightGray };
            lexerHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Тип")) { FontWeight = FontWeights.Bold }));
            lexerHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Лексема")) { FontWeight = FontWeights.Bold }));
            lexerHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Позиция")) { FontWeight = FontWeights.Bold }));
            lexerHeaderRowGroup.Rows.Add(lexerHeaderRow);
            lexerTable.RowGroups.Add(lexerHeaderRowGroup);

            var lexerDataRowGroup = new TableRowGroup();
            foreach (var token in tokens)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(token.Type))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(token.Lexeme))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(token.Position))));
                lexerDataRowGroup.Rows.Add(row);
            }
            lexerTable.RowGroups.Add(lexerDataRowGroup);
            LexerOutputRichTextBox.Document.Blocks.Add(lexerTable);

            // Синтаксический анализ
            Parser parser = new Parser(tokens);
            parser.Parse();

            // Сортируем ошибки: сначала по числовой позиции, затем "end" в конце
            var sortedErrors = parser.Errors.OrderBy(error =>
            {
                if (error.Position == "end")
                    return int.MaxValue;
                return GetPositionValue(error.Position);
            }).ThenBy(error => error.Position).ToList();

            // Вывод ошибок парсера
            var parserTable = new Table();
            parserTable.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            parserTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            var parserHeaderRowGroup = new TableRowGroup();
            var parserHeaderRow = new TableRow { Background = Brushes.LightGray };

            // Добавляем строку с количеством ошибок
            var errorCountParagraph = new Paragraph();
            errorCountParagraph.Inlines.Add(new Run($"Ошибки синтаксического анализа (всего: {sortedErrors.Count})")
            {
                FontWeight = FontWeights.Bold,
                Foreground = sortedErrors.Count > 0 ? Brushes.Red : Brushes.Green
            });

            parserHeaderRow.Cells.Add(new TableCell(errorCountParagraph));
            parserHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Позиция")) { FontWeight = FontWeights.Bold }));
            parserHeaderRowGroup.Rows.Add(parserHeaderRow);
            parserTable.RowGroups.Add(parserHeaderRowGroup);

            var parserDataRowGroup = new TableRowGroup();
            if (sortedErrors.Count == 0)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run("✓ Синтаксический анализ завершен успешно"))
                {
                    Foreground = Brushes.Green,
                    FontWeight = FontWeights.Bold
                }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                parserDataRowGroup.Rows.Add(row);
            }
            else
            {
                foreach (var error in sortedErrors)
                {
                    var row = new TableRow();
                    row.Cells.Add(new TableCell(new Paragraph(new Run(error.Message)) { Foreground = Brushes.Red }));
                    row.Cells.Add(new TableCell(new Paragraph(new Run(error.Position))));
                    parserDataRowGroup.Rows.Add(row);
                }
            }
            parserTable.RowGroups.Add(parserDataRowGroup);
            ParserOutputRichTextBox.Document.Blocks.Add(parserTable);
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