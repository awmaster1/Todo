using Lab4;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Desktop
{
    public partial class Main_empty : Window
    {
        private int _userId;
        private string _userName;

        public Main_empty()
        {
            InitializeComponent();
        }

        public void SetUserInfo(string userName, int userId)
        {
            _userName = userName;
            _userId = userId;

            // Обновляем приветствие
            var welcomeLabel = FindVisualChild<Label>(this, "WelcomeLabel");
            if (welcomeLabel != null)
            {
                welcomeLabel.Content = $"Добро пожаловать, {userName}!";
            }
        }

        // Обработчик клика по кнопке (связывается через XAML)
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно создания задачи
            CreateTask createTaskWindow = new CreateTask(_userId, _userName);
            createTaskWindow.Owner = this;
            createTaskWindow.TaskCreated += OnTaskCreated;
            createTaskWindow.Show();
        }

        private void OnTaskCreated(object sender, EventArgs e)
        {
            // Закрываем это окно и открываем Main
            Main mainWindow = new Main();
            mainWindow.SetUserName(_userName);
            mainWindow.LoadUserTasks(_userId);
            mainWindow.Show();
            this.Close();
        }

        // Вспомогательный метод для поиска элементов
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