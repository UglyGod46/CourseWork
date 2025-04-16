using System;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace WpfApp1.Managers
{
    public class TextManager
    {
        // Базовый путь к папке с PDF-файлами
        private const string BasePath = "TextFolder";

        // Полные пути к каждому файлу
        private readonly string TaskStatementPath = Path.Combine(BasePath, "TaskStatement.pdf");
        private readonly string GrammarPath = Path.Combine(BasePath, "Grammar.pdf");
        private readonly string GrammarClassificationPath = Path.Combine(BasePath, "GrammarClassification.pdf");
        private readonly string AnalysisMethodPath = Path.Combine(BasePath, "AnalysisMethod.pdf");
        private readonly string ErrorDiagnosticsPath = Path.Combine(BasePath, "ErrorDiagnostics.pdf");
        private readonly string TestExamplePath = Path.Combine(BasePath, "TestExample.pdf");
        private readonly string BibliographyPath = Path.Combine(BasePath, "Bibliography.pdf");
        private readonly string SourceCodePath = Path.Combine(BasePath, "SourceCode.pdf");

        // Методы для открытия каждого файла
        public void OpenTaskStatement() => OpenPdfFile(TaskStatementPath);
        public void OpenGrammar() => OpenPdfFile(GrammarPath);
        public void OpenGrammarClassification() => OpenPdfFile(GrammarClassificationPath);
        public void OpenAnalysisMethod() => OpenPdfFile(AnalysisMethodPath);
        public void OpenErrorDiagnostics() => OpenPdfFile(ErrorDiagnosticsPath);
        public void OpenTestExample() => OpenPdfFile(TestExamplePath);
        public void OpenBibliography() => OpenPdfFile(BibliographyPath);
        public void OpenSourceCode() => OpenPdfFile(SourceCodePath);

        private void OpenPdfFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"PDF-файл не найден по пути:\n{filePath}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Самый простой и надежный способ открыть PDF
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии PDF-файла:\n{ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
