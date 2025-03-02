using Microsoft.Win32;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private string currentFilePath;
        private readonly FileManager fileManager;
        private readonly EditManager editManager;
        private readonly HelpManager helpManager;

        public MainWindow()
        {
            InitializeComponent();
            fileManager = new FileManager(InputRichTextBox, FileNameTextBlock);
            editManager = new EditManager(InputRichTextBox);
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

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (FontSizeComboBox.SelectedItem is ComboBoxItem selectedItem && double.TryParse(selectedItem.Content.ToString(), out double fontSize))
            {
                
                ApplyFontSize(InputRichTextBox, fontSize);

                
                ApplyFontSize(OutputRichTextBox, fontSize);
            }
        }

        private void ApplyFontSize(RichTextBox richTextBox, double fontSize)
        {
            
            if (richTextBox == null)
            {
                MessageBox.Show("RichTextBox инициализирован.");
                return;
            }

        
            TextSelection selection = richTextBox.Selection;

           
            if (selection != null)
            {
                
                if (!selection.IsEmpty)
                {
                    selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                }
                else
                {
                
                    richTextBox.FontSize = fontSize;
                }
            }
            else
            {
           
                richTextBox.FontSize = fontSize;
            }
        }
        private void InputRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            InputRichTextBox.TextChanged -= InputRichTextBox_TextChanged;
        
            TextRange textRange = new TextRange(InputRichTextBox.Document.ContentStart, InputRichTextBox.Document.ContentEnd);
            textRange.ClearAllProperties();

            HighlightKeywords();

            InputRichTextBox.TextChanged += InputRichTextBox_TextChanged;
        }

        private void HighlightKeywords()
        {
            string[] keywords = { "function", "if", "else", "while", "for", "return", "var", "let", "const" };
            foreach (string keyword in keywords)
            {
                HighlightText(keyword, Brushes.Blue, FontWeights.Bold);
            }
        }


        private void HighlightText(string text, Brush foreground, FontWeight fontWeight)
        {
            TextPointer position = InputRichTextBox.Document.ContentStart;

            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);
                    int index = textRun.IndexOf(text, StringComparison.OrdinalIgnoreCase);

                    if (index >= 0)
                    {
                        TextPointer start = position.GetPositionAtOffset(index);
                        TextPointer end = start.GetPositionAtOffset(text.Length);
                        TextRange range = new TextRange(start, end);

                        range.ApplyPropertyValue(TextElement.ForegroundProperty, foreground);
                        range.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }
        }
    }
}
    

