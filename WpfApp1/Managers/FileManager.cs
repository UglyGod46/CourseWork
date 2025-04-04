﻿using ICSharpCode.AvalonEdit;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WpfApp1
{
    public class FileManager
    {
        private readonly TextEditor inputTextEditor;
        private readonly TextBlock fileNameTextBlock;

        public FileManager(TextEditor inputTextEditor, TextBlock fileNameTextBlock)
        {
            this.inputTextEditor = inputTextEditor;
            this.fileNameTextBlock = fileNameTextBlock;
        }

        public void SaveAs(ref string currentFilePath)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Сохранить файл как"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
                SaveFile(ref currentFilePath);
            }
        }

        public void OpenFile(ref string currentFilePath)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Открыть текстовый файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                try
                {
                    string fileContent = File.ReadAllText(currentFilePath);
                    inputTextEditor.Text = fileContent;
                    fileNameTextBlock.Text = $"Открыт файл: {System.IO.Path.GetFileName(currentFilePath)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при открытии файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void CreateFile(ref string currentFilePath)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Создать новый файл"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
                try
                {
                    File.WriteAllText(currentFilePath, string.Empty);
                    inputTextEditor.Clear();
                    fileNameTextBlock.Text = $"Создан файл: {System.IO.Path.GetFileName(currentFilePath)}"; // Обновляем текст
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при создании файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void SaveFile(ref string currentFilePath)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("Нет открытого файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                File.WriteAllText(currentFilePath, inputTextEditor.Text);
                MessageBox.Show("Файл сохранён успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}