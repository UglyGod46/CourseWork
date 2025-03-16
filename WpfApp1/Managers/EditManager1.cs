using ICSharpCode.AvalonEdit;

public class EditManager
{
    private readonly TextEditor inputTextEditor;

    public EditManager(TextEditor inputTextEditor)
    {
        this.inputTextEditor = inputTextEditor;
    }

    public void Cut()
    {
        inputTextEditor.Cut();
    }

    public void Copy()
    {
        inputTextEditor.Copy();
    }

    public void Paste()
    {
        inputTextEditor.Paste();
    }

    public void Delete()
    {
        inputTextEditor.SelectedText = string.Empty;
    }

    public void SelectAll()
    {
        inputTextEditor.SelectAll();
    }

    public void Undo()
    {
        inputTextEditor.Undo();
    }

    public void Redo()
    {
        inputTextEditor.Redo();
    }
}