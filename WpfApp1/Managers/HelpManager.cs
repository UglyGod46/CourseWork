using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public class HelpManager
    {
        public void ShowHelp()
        {

            Window helpWindow = new Window
            {
                Title = "Справка",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };


            TextBlock instructionText = new TextBlock
            {
                Text = "Инструкция по работе с программой:\n\n" +
                        "1. Для создания нового документа нажмите 'Создать' в меню 'Файл'.\n" +
                        "2. Для открытия существующего документа выберите 'Открыть'.\n" +
                        "3. Используйте 'Сохранить' для сохранения изменений.\n" +
                        "4. Для редактирования текста используйте функции 'Вырезать', 'Копировать', 'Вставить' и 'Удалить'.\n" +
                        "5. Вы можете отменить или повторить действия с помощью 'Отменить' и 'Повторить'.\n" +
                        "6. Для получения справки нажмите 'Справка' в меню."
            };


            helpWindow.Content = instructionText;
            helpWindow.ShowDialog(); 
        }

        public void ShowAbout()
        {
           
            Window aboutWindow = new Window
            {
                Title = "О программе",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

           
            TextBlock aboutText = new TextBlock
            {
                Text = "Текстовый редактор версии 1.0\n\n" +
                        "Разработано студентом АВТ-214\n" +
                        "© 2025",
                TextAlignment = TextAlignment.Center
            };

          
            aboutWindow.Content = aboutText;
            aboutWindow.ShowDialog(); 
        }
    }
}
