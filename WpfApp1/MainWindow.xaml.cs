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

        private double _fontSize = 14; // Поле для хранения значения

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    InputTextEditor.FontSize = value; // Обновляем размер шрифта вручную
                    OutputRichTextBox.FontSize = value; // Обновляем размер шрифта для RichTextBox
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Устанавливаем DataContext

            // Настройка подсветки синтаксиса
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

            Lexer lexer = new Lexer(input);
            List<Token> tokens = lexer.Tokenize();

            var table = new Table();
            OutputRichTextBox.Document.Blocks.Add(table);

            table.CellSpacing = 10;
            table.Background = Brushes.White;

            for (int i = 0; i < 4; i++)
            {
                table.Columns.Add(new TableColumn());
            }

            var headerRow = new TableRow { Background = Brushes.LightGray };
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Условный код"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Тип лексемы"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Лексема"))));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Местоположение"))));
            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups[0].Rows.Add(headerRow);

            foreach (var token in tokens)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(((int)token.Type).ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(token.Type.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(token.Value))));
                row.Cells.Add(new TableCell(new Paragraph(new Run($"с {token.StartIndex + 1} по {token.EndIndex + 1} символ"))));
                table.RowGroups[0].Rows.Add(row);
            }
        }
    }
}