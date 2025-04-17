using System.Windows;
using System.Windows.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace WpfApp1
{
    public class HelpManager
    {
        // Базовый путь к папке с PDF-файлами
        private const string BasePath = "TextFolder";

        private readonly string UsersGuidePath = Path.Combine(BasePath, "Users_Guide.pdf");
        public void OpenUsersGuide() => OpenPdfFile(UsersGuidePath);

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

        public void ShowAbout()
        {
            Window aboutWindow = new Window
            {
                Title = "О программе",
                Width = 400,
                Height = 250,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            TextBlock aboutText = new TextBlock
            {
                Text = "О программе\n\n" +
                       "Название: Курсовая работа — Парсер функции JavaScript\n" +
                       "Разработчик: Студент АВТ-214 Патласов Д.А.\n" +
                       "Описание: Программа представляет собой текстовый редактор с функциями лексического анализа кода на языке JavaScript. Она позволяет пользователю писать, редактировать и сохранять код, а также предоставляет базовую подсветку синтаксиса и инструменты редактирования.\n" +
                       "Технологии: WPF на языке C#.\n",
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(10),
                FontSize = 14
            };

            aboutWindow.Content = aboutText;
            aboutWindow.ShowDialog();
        }
    }
}
