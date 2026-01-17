using System;
using System.Windows;
using System.Windows.Controls;
using Todo.Entities;
using Desktop.Repository;

namespace Desktop
{
    public partial class CreateTask : Window
    {
        private int _userId;
        private string _userName;
        private UserRepository _repository;

        public event EventHandler TaskCreated;

        public CreateTask(int userId, string userName)
        {
            InitializeComponent();
            _userId = userId;
            _userName = userName;
            _repository = new UserRepository();

            // Добавляем обработчики
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            // Находим элементы и добавляем обработчики
            var createButton = FindVisualChild<Button>(this, "CreateButton");
            var cancelButton = FindVisualChild<Button>(this, "CancelButton");

            if (createButton != null)
                createButton.Click += Button_Click;
            if (cancelButton != null)
                cancelButton.Click += Button_Click_1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем данные из полей
                string title = GetTextBoxText("TitleTextBox");
                string category = GetComboBoxValue("CategoryComboBox");
                string description = GetTextBoxText("DescriptionTextBox");
                string time = GetTextBoxText("TimeTextBox");

                // Валидация
                if (string.IsNullOrEmpty(title))
                {
                    MessageBox.Show("Введите название задачи", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(category))
                {
                    MessageBox.Show("Выберите категорию", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаем задачу
                var task = new UserTask
                {
                    UserId = _userId,
                    Title = title,
                    Description = description,
                    Category = category,
                    Date = DateTime.Now,
                    Time = string.IsNullOrEmpty(time) ? "9:00 am" : time,
                    IsCompleted = false
                };

                // Сохраняем задачу
                _repository.CreateTask(task);

                MessageBox.Show("Задача успешно создана!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Вызываем событие создания задачи
                TaskCreated?.Invoke(this, EventArgs.Empty);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании задачи: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Отмена - возвращаемся в Main_empty
            this.Close();
        }

        // Вспомогательные методы для работы с элементами
        private string GetTextBoxText(string textBoxName)
        {
            var textBox = FindVisualChild<TextBox>(this, textBoxName);
            return textBox?.Text ?? "";
        }

        private string GetComboBoxValue(string comboBoxName)
        {
            var comboBox = FindVisualChild<ComboBox>(this, comboBoxName);

            if (comboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content?.ToString() ?? "";
            }

            return comboBox?.SelectedItem?.ToString() ?? "";
        }

        private T FindVisualChild<T>(DependencyObject parent, string childName = null) where T : DependencyObject
        {
            if (parent == null) return null;

            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                {
                    if (childName == null ||
                        (child is FrameworkElement fe && fe.Name == childName))
                    {
                        return typedChild;
                    }
                }

                var result = FindVisualChild<T>(child, childName);
                if (result != null) return result;
            }

            return null;
        }
    }
}