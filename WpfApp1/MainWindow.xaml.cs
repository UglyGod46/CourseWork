using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WpfApp1.models;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private string currentFilePath;
        private readonly FileManager fileManager;
        private readonly EditManager editManager;
        private readonly HelpManager helpManager;

        private double _fontSize = 14; 

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    InputTextEditor.FontSize = value;
                    OutputRichTextBox.FontSize = value; 
                }
            }
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
            OutputRichTextBox.Document.Blocks.Clear();
            string input = InputTextEditor.Text;

            // Лексический анализ
            Lexer lexer = new Lexer(input);
            List<Token> tokens = lexer.Tokenize();

            // Синтаксический анализ
            Parser1 parser = new Parser1(tokens);
            ParseResult result = parser.Parse();

            // Основной заголовок
            var headerParagraph = new Paragraph();
            if (result.IsValid)
            {
                headerParagraph.Inlines.Add(new Run("✓ Синтаксический анализ завершен успешно")
                {
                    Foreground = Brushes.Green,
                    FontWeight = FontWeights.Bold,
                    FontSize = 14
                });
            }
            else
            {
                headerParagraph.Inlines.Add(new Run($"× Найдено {result.Errors.Count} синтаксических ошибок")
                {
                    Foreground = Brushes.Red,
                    FontWeight = FontWeights.Bold,
                    FontSize = 14
                });
            }
            OutputRichTextBox.Document.Blocks.Add(headerParagraph);

            if (!result.IsValid)
            {
                // Создаем таблицу для ошибок
                var errorsTable = new Table
                {
                    CellSpacing = 5,
                    Background = Brushes.White,
                    Margin = new Thickness(0, 10, 0, 0),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1)
                };

                // Настраиваем колонки
                errorsTable.Columns.Add(new TableColumn { Width = new GridLength(70, GridUnitType.Star) });
                errorsTable.Columns.Add(new TableColumn { Width = new GridLength(30, GridUnitType.Star) });

                // Группа строк для заголовка
                var headerGroup = new TableRowGroup();
                var headerRow = new TableRow
                {
                    Background = Brushes.LightGray,
                    FontWeight = FontWeights.Bold
                };

                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Описание ошибки"))
                {
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    BorderBrush = Brushes.Gray,
                    Padding = new Thickness(5)
                }));

                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Позиция"))
                {
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = Brushes.Gray,
                    Padding = new Thickness(5)
                }));

                headerGroup.Rows.Add(headerRow);
                errorsTable.RowGroups.Add(headerGroup);

                // Группа строк для данных
                var dataGroup = new TableRowGroup();

                foreach (var error in result.Errors)
                {
                    var errorParts = error.Split(new[] { '(' }, 2);
                    var row = new TableRow();

                    // Ячейка с описанием ошибки
                    var descriptionCell = new TableCell(new Paragraph(new Run(errorParts[0].Trim())))
                    {
                        BorderThickness = new Thickness(0, 0, 1, 0),
                        BorderBrush = Brushes.LightGray,
                        Padding = new Thickness(5)
                    };
                    row.Cells.Add(descriptionCell);

                    // Ячейка с позицией
                    var positionCell = new TableCell();
                    if (errorParts.Length > 1)
                    {
                        positionCell.Blocks.Add(new Paragraph(new Run(errorParts[1].TrimEnd(')'))));
                    }
                    positionCell.Padding = new Thickness(5);
                    row.Cells.Add(positionCell);

                    dataGroup.Rows.Add(row);
                }

                errorsTable.RowGroups.Add(dataGroup);
                OutputRichTextBox.Document.Blocks.Add(errorsTable);
            }
        }
    }
}