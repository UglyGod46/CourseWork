using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfApp1
{
    public class EditManager
    {
        private readonly RichTextBox inputRichTextBox;

        public EditManager(RichTextBox inputRichTextBox)
        {
            this.inputRichTextBox = inputRichTextBox;
        }

        public void Cut()
        {
            if (inputRichTextBox.Selection.IsEmpty)
                return;

            TextRange selectedText = new TextRange(inputRichTextBox.Selection.Start, inputRichTextBox.Selection.End);
            Clipboard.SetText(selectedText.Text);
            selectedText.Text = string.Empty; // Удалить выделенный текст
        }

        public void Copy()
        {
            if (inputRichTextBox.Selection.IsEmpty)
                return;

            TextRange selectedText = new TextRange(inputRichTextBox.Selection.Start, inputRichTextBox.Selection.End);
            Clipboard.SetText(selectedText.Text);
        }

        public void Paste()
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                inputRichTextBox.AppendText(clipboardText);
            }
        }

        public void Delete()
        {
            if (inputRichTextBox.Selection.IsEmpty)
                return;

            TextRange selectedText = new TextRange(inputRichTextBox.Selection.Start, inputRichTextBox.Selection.End);
            selectedText.Text = string.Empty; // Удалить выделенный текст
        }

        public void SelectAll()
        {
            inputRichTextBox.SelectAll();
            inputRichTextBox.Focus(); // Установить фокус на RichTextBox
        }

        public void Undo()
        {
            if (inputRichTextBox.CanUndo)
            {
                inputRichTextBox.Undo();
            }
        }

        public void Redo()
        {
            if (inputRichTextBox.CanRedo)
            {
                inputRichTextBox.Redo();
            }
        }
    }
}
